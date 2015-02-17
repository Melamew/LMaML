using System.Collections.Generic;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Commands
{
    public class AddFilesCommand : BusMessage
    {
        public IEnumerable<StorableTaggedFile> Files { get; set; }

        public AddFilesCommand(IEnumerable<StorableTaggedFile> files)
        {
            Files = files;
        }
    }
}