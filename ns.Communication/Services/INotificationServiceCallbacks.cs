using ns.Communication.CommunicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    public interface INotificationServiceCallbacks {

        [OperationContract(IsOneWay = true)]
        void OnOperationAdded(OperationCommunicationModel model);

        [OperationContract(IsOneWay = true)]
        void OnToolAdded(ToolCommunicationModel model);
    }
}