using System;
using System.IO;
using FMOD;
using LMaML.Infrastructure.Audio;
using iLynx.Common;

namespace LMaML.FMOD
{
    /// <summary>
    /// AudioPlayer
    /// </summary>
    public class AudioPlayer : ComponentBase, IAudioPlayer, IDisposable
    {
        private global::FMOD.System fmodSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlayer" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AudioPlayer(ILogger logger)
            : base(logger)
        {
            logger.Guard("logger");
            fmodSystem = new global::FMOD.System();
            var result = Factory.System_Create(ref fmodSystem);
            if (result != RESULT.OK)
                throw GetException("Unable to create FMOD System", result);
            result = fmodSystem.init(10, INITFLAGS.NORMAL, IntPtr.Zero);
            if (result != RESULT.OK)
                throw GetException("Unable to Initialize FMOD System", result);
        }

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public IChannel CreateChannel(string file)
        {
            var sound = CreateSoundFromFile(file);
            return new AudioChannel(sound, fmodSystem, Logger);
        }

        private Sound CreateSoundFromFile(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                throw new FileNotFoundException("Can't find file", file ?? string.Empty);
            Sound sound = null;
            var result = fmodSystem.createStream(file, MODE.HARDWARE, ref sound);
            if (result != RESULT.OK)
                throw GetException("Unable to create sound", result);
            return sound;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AudioPlayer" /> class.
        /// </summary>
        ~AudioPlayer()
        {
            if (!disposed)
                Dispose();
        }

        private bool disposed;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            if (fmodSystem == null) return;
            var result = fmodSystem.close();
            if (result != RESULT.OK)
                throw GetException("Unable to close FMOD System", result);
            fmodSystem = null;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static Exception GetException(string text, RESULT result)
        {
            return new Exception(string.Format("{0}{1}Error Code: {2}", text, Environment.NewLine, result));
        }
    }
}