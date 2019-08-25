using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackGammonWeb.Services
{
   public interface ITypedHubClient
    {

        Task BroadcastMessage(object message);
        Task UpdateTableState(object table);
        Task UpdateUsers(List<User> users);

    }
}
