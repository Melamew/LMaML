using System.Collections.Generic;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Commands
{
    public class GetPlaylistCommand : BusMessage<List<StorableTaggedFile>>
    {
        
    }
}