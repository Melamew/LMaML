using System.Collections.Generic;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Commands
{
    public class SetPlaylistCommand : AddFilesCommand
    {
        public SetPlaylistCommand(IEnumerable<StorableTaggedFile> files) : base(files)
        {
        }
    }
}
