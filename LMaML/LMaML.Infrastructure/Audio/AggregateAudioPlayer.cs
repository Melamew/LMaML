using System;
using System.Collections.Generic;
using System.Reflection;
using iLynx.Common;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Audio
{
    public class AggregateAudioPlayer : IAggregateAudioPlayer
    {
        private readonly Dictionary<Guid, IAudioPlayer> players = new Dictionary<Guid, IAudioPlayer>();

        public ITrack CreateChannel(StorableTaggedFile file)
        {
            try
            {
                var player = GetPlayer(file.StorageType);
                return player.CreateChannel(file);
            }
            catch (KeyNotFoundException e)
            {
                throw new FormatException(
                    string.Format("Could not find player for the file type {0}", file.StorageType), e);
            }
        }

        public void LoadPlugins(string dir)
        {
            foreach (var player in players.Values)
            {
                try { player.LoadPlugins(dir); }
                catch (Exception e) { this.LogException(e, MethodBase.GetCurrentMethod()); }
            }
        }

        public void RegisterPlayer(Guid storageType, IAudioPlayer player)
        {
            if (players.ContainsKey(storageType))
                players[storageType] = player;
            else
                players.Add(storageType, player);
        }

        public void UnRegisterPlayer(Guid storageType)
        {
            players.Remove(storageType);
        }

        public IAudioPlayer GetPlayer(Guid storageType)
        {
            IAudioPlayer result;
            if (!players.TryGetValue(storageType, out result))
                throw new KeyNotFoundException("Could not find a player for the specified storage type");
            return result;
        }
    }
}
