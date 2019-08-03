using BackGammonWeb.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackGammonWeb.Hubs
{
    public class ServerNub:Hub<ITypedHubClient>
    {
        public ServerNub()
        {

        }
    }
}
