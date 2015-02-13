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
            IAudioPlayer player;
            if (!players.TryGetValue(file.StorageType, out player))
                throw new FormatException("Could not find an audioplayer that is associated with the specified file");
            return player.CreateChannel(file);
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
    }
}
