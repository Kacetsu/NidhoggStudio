using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Plugin.Web.Hubs {
    [HubName("renderHub")]
    public class RenderHub : Hub {

        public void Init() {
            throw new Exception();
        }

        public void Count(string name, string message) {
            Clients.All.broadcastMessage(name, message);
        }

    }
}
