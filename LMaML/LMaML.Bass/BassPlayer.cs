using System;
using System.Collections.Generic;
using System.Diagnostics;
using iLynx.Configuration;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using iLynx.Common;
using Bassh = Un4seen.Bass.Bass;

namespace LMaML.Bass
{
    public class BassPlayer : IAudioPlayer
    {
        private readonly ILogger logger;
        private int mixerHandle;
        private readonly IConfigurableValue<int> sampleRate;
        private readonly IConfigurableValue<int> bufferSize;
        private readonly List<int> pluginHandles = new List<int>();

        public BassPlayer(IConfigurationManager configurationManager, ILogger logger)
        {
            this.logger = logger;
            sampleRate = configurationManager.GetValue("Sample Rate", 96000, "Bass");
            bufferSize = configurationManager.GetValue("Buffer Size (ms)", 500, "Bass");
            Setup();
        }

        private void Setup()
        {
            if (!Bassh.BASS_Init(-1, sampleRate.Value, BASSInit.BASS_DEVICE_LATENCY | BASSInit.BASS_DEVICE_FREQ, IntPtr.Zero))
                throw new InvalidOperationException("Could not initialize BASS");
            Bassh.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, 4);
            Bassh.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, bufferSize.Value);
            Bassh.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, bufferSize.Value / 10);
            Trace.WriteLine(Bassh.BASS_GetInfo());
            var plugins = Bassh.BASS_PluginLoadDirectory(Environment.CurrentDirectory);
            mixerHandle = BassMix.BASS_Mixer_StreamCreate(sampleRate.Value, 2, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIXER_NONSTOP);
            Bassh.BASS_ChannelPlay(mixerHandle, true);
            if (null == plugins) return;
            foreach (var plugin in plugins)
            {
                pluginHandles.Add(plugin.Key);
                logger.Log(LogLevel.Information, this, string.Format("Plugin Loaded: {0}", plugin.Value));
            }
        }

        ~BassPlayer()
        {
            foreach (var handle in pluginHandles)
                Bassh.BASS_PluginFree(handle);
            Bassh.BASS_Stop();
            Bassh.BASS_Free();
        }

        #region Implementation of IAudioPlayer

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public ITrack CreateChannel(StorableTaggedFile file)
        {
            var fileName = file.Filename;
            var channelHandle = Bassh.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);
            Bassh.BASS_ChannelUpdate(channelHandle, 0);
            Bassh.BASS_ChannelSetAttribute(channelHandle, BASSAttribute.BASS_ATTRIB_SRC, 2);
            Debug.WriteLine(Bassh.BASS_ChannelGetInfo(channelHandle));
            if (0 == channelHandle) throw new InvalidOperationException("Unable to create stream");
            if (BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, channelHandle,
                BASSFlag.BASS_MIXER_PAUSE | BASSFlag.BASS_MIXER_BUFFER | BASSFlag.BASS_MIXER_NORAMPIN))
                return new BassTrack(channelHandle, mixerHandle, fileName);
            Trace.WriteLine(Bassh.BASS_ErrorGetCode());
            throw new InvalidOperationException("Unable to add channel to mixer.");
        }

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        /// <param name="dir">The dir.</param>
        public void LoadPlugins(string dir)
        {
        }

        #endregion
    }
}
