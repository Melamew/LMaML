using System;
using LMaML.Infrastructure.Domain.Concrete;
using iLynx.Common;

namespace LMaML.Playlist.ViewModels
{
    public class FileItem : NonComponentNotificationBase
    {
        private readonly StorableTaggedFile file;
        private bool isPlaying;

        public FileItem(StorableTaggedFile file)
        {
            this.file = file;
        }

        public string Artist { get { return file.Artist.Name; } }

        public string Title { get { return file.Title.Name; } }

        public TimeSpan Length { get { return file.Duration; } }

        public StorableTaggedFile File
        {
            get { return file; }
        }

        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (value == isPlaying) return;
                isPlaying = value;
                RaisePropertyChanged(() => IsPlaying);
            }
        }
    }
}
