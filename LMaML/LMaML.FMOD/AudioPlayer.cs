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
    public class AudioPlayer : IAudioPlayer
    {
        private global::FMOD.System fmodSystem;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlayer" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AudioPlayer(ILogger logger)
        {
            this.logger = logger;
            fmodSystem = new global::FMOD.System();
            var result = Factory.System_Create(ref fmodSystem);
            if (result != RESULT.OK)
                throw GetException("Unable to create FMOD System", result);
            result = fmodSystem.init(10, INITFLAGS.NORMAL, IntPtr.Zero);
            if (result != RESULT.OK)
                throw GetException("Unable to Initialize FMOD System", result);
        }

        private IChannel PlaySound(Sound sound)
        {
            var result = new AudioChannel(sound, fmodSystem, logger);
            result.Play(100f);
            return result;
        }

        /// <summary>
        /// Stops the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public void StopChannel(IChannel channel)
        {
            channel.Guard("channel");
            channel.Stop();
        }

        /// <summary>
        /// Plays the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Can't find file</exception>
        public IChannel PlayFile(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
                throw new FileNotFoundException("Can't find file", file ?? string.Empty);
            var sound = CreateSoundFromFile(file);
            return PlaySound(sound);
        }

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public IChannel CreateChannel(string file)
        {
            var sound = CreateSoundFromFile(file);
            return new AudioChannel(sound, fmodSystem, logger);
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