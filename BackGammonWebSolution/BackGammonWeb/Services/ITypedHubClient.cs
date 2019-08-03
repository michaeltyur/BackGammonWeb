using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackGammonWeb.Services
{
   public interface ITypedHubClient
    {

        Task BroadcastMessage(object message);
        Task BroadcastTableState(object table);

    }
}
