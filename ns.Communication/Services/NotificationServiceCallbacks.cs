using ns.Communication.CommunicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    public class NotificationServiceCallbacks : INotificationServiceCallbacks {

        public void OnOperationAdded(OperationCommunicationModel model) {
        }

        public void OnToolAdded(ToolCommunicationModel model) {
        }
    }
}