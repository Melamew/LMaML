using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Commands
{
    public class PlayFileCommand : BusMessage
    {
        public StorableTaggedFile File { get; private set; }

        public PlayFileCommand(StorableTaggedFile file)
        {
            File = file;
        }
    }
}