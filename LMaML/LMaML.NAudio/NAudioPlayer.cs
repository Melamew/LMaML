using System;
using LMaML.Infrastructure.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace LMaML.NAudio
{
    public class NAudioPlayer : IAudioPlayer, IDisposable
    {
        private readonly IWavePlayer player;
        //private readonly WaveMixerStream32 mixerStream;
        private readonly MixingSampleProvider mixerStream;

        public NAudioPlayer()
        {
            player = new DirectSoundOut(50);
            mixerStream = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
                              {
                                  ReadFully = true,
                              };
            player.Init(mixerStream, true);
            player.Play();
        }

        internal void DoPlay()
        {
            if (player.PlaybackState != PlaybackState.Playing)
                player.Play();
        }

        #region Implementation of IAudioPlayer

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public ITrack CreateChannel(string file)
        {
            var stream = CreateInput(file);
            return new NAudioTrack(this, mixerStream, stream);
        }

        private static AudioFileReader CreateInput(string file)
        {
            var reader = new AudioFileReader(file);
            return reader;
        }

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        /// <param name="dir">The dir.</param>
        public void LoadPlugins(string dir)
        {
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            player.Stop();
            player.Dispose();
        }

        #endregion
    }
}
