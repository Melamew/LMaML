using System;
using LMaML.Infrastructure.Services.Interfaces;

namespace LMaML.Infrastructure.Audio
{
    public interface IAggregateAudioPlayer : IAudioPlayer
    {
        /// <summary>
        /// Registers the specified <see cref="IPlayerService"/> to handle the specified storage type.
        /// </summary>
        /// <param name="storageType"></param>
        /// <param name="player"></param>
        void RegisterPlayer(Guid storageType, IAudioPlayer player);

        /// <summary>
        /// Un-Registers the <see cref="IPlayerService"/> currently registered to handle the specified storage type.
        /// </summary>
        /// <param name="storageType"></param>
        void UnRegisterPlayer(Guid storageType);

        /// <summary>
        /// Retrieves the player that has been registered to handle the speicfied storage type.
        /// </summary>
        /// <param name="storageType"></param>
        /// <returns></returns>
        IAudioPlayer GetPlayer(Guid storageType);
    }
}