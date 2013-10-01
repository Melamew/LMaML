using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMaML.Infrastructure.Audio;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Mix;
using Bassh = Un4seen.Bass.Bass;

namespace LMaML.Bass
{
    public class BassPlayer : IAudioPlayer
    {
        private readonly int mixerHandle;

        public BassPlayer()
        {
            if (!Bassh.BASS_Init(-1, 96000, BASSInit.BASS_DEVICE_LATENCY, IntPtr.Zero))
                throw new InvalidOperationException("Could not initialize BASS");
            Bassh.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 500);
            Bassh.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);
            Trace.WriteLine(Bassh.BASS_GetInfo());
            mixerHandle = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIXER_NONSTOP);
            Bassh.BASS_ChannelSetSync(mixerHandle, BASSSync.BASS_SYNC_STALL, 0, OnMixerStall, IntPtr.Zero);
            Bassh.BASS_ChannelPlay(mixerHandle, true);
        }

        ~BassPlayer()
        {
            Bassh.BASS_Stop();
            Bassh.BASS_Free();
        }

        private static void OnMixerStall(int handle,
                          int channel,
                          int data,
                          IntPtr user)
        {

        }

        #region Implementation of IAudioPlayer

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public ITrack CreateChannel(string file)
        {
            int channelHandle;
            Func<string, long, long, BASSFlag, int> creator;
            
            if (file.EndsWith(".flac")) // Bad...
                creator = BassFlac.BASS_FLAC_StreamCreateFile;
            else
                creator = Bassh.BASS_StreamCreateFile;
            channelHandle = creator(file, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);// | BASSFlag.BASS_STREAM_PRESCAN);
            Debug.WriteLine(Bassh.BASS_ChannelGetInfo(channelHandle));
            if (0 == channelHandle) throw new InvalidOperationException("Unable to create stream");
            BassMix.BASS_Mixer_StreamAddChannel(mixerHandle, channelHandle, BASSFlag.BASS_MIXER_PAUSE | BASSFlag.BASS_MIXER_BUFFER | BASSFlag.BASS_MIXER_NORAMPIN);
            return new BassTrack(channelHandle);
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
