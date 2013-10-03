using System;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IPlayerService
    /// </summary>
    public interface IPlayerService : IDisposable
    {
        /// <summary>
        /// Plays the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        void Play(StorableTaggedFile file);

        /// <summary>
        /// Plays the specified track.
        /// </summary>
        /// <param name="track">The track.</param>
        void Play(ITrack track);

        /// <summary>
        /// Plays the pause.
        /// </summary>
        void PlayPause();

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        void Next();

        /// <summary>
        /// Previouses this instance.
        /// </summary>
        void Previous();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();

        /// <summary>
        /// Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        void Seek(TimeSpan offset);

        /// <summary>
        /// Seeks the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        void Seek(double milliseconds);

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        PlayingState State { get; }

        /// <summary>
        /// Currents the channel as readonly.
        /// </summary>
        /// <value>
        /// The current channel as readonly.
        /// </value>
        /// <returns></returns>
        ITrack CurrentTrackAsReadonly { get; }

        /// <summary>
        /// Gets an FFT of the specified size for the currently playing channel.
        /// <para>
        /// Note that if there is currently no active channel this method will return an array of the specified size with default values (0f).
        /// </para>
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        float[] FFT(out float sampleRate, int size = 64);
    }
}
