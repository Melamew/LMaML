using System.Collections.Generic;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Commands
{
    public class RemoveFilesCommand : BusMessage
    {
        public IEnumerable<StorableTaggedFile> Files { get; private set; }

        public RemoveFilesCommand(IEnumerable<StorableTaggedFile> files)
        {
            Files = files;
        }
    }
}