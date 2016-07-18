using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace polistudio{
    // Use a data contract as illustrated in the sample below to add composite types to service operations.

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
   
    //[ServiceContract (Namespace = "http://polistudio/", SessionMode=SessionMode.Required)]
    [ServiceContract (Namespace = "http://polistudio/")]
    public interface IService
    {
        //[OperationContract( IsInitiating = true)]
        [OperationContract]
        String login(String username, String password);

        //[OperationContract(IsInitiating = false)]
        [OperationContract]
        int portRequest(String token);

        //[OperationContract(IsInitiating = false, IsTerminating = true)]
        [OperationContract]
        Boolean logout(String token);

    }
}