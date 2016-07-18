//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     .NET Micro Framework MFSvcUtil.Exe
//     Runtime Version:2.0.00001.0001
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Text;
using System.Xml;
using Dpws.Device;
using Dpws.Device.Services;
using Ws.Services;
using Ws.Services.WsaAddressing;
using Ws.Services.Xml;
using Ws.Services.Binding;
using Ws.Services.Soap;

namespace polistudio
{
    
    
    public class IService : DpwsHostedService
    {
        
        private IIService m_service;
        
        public IService(IIService service, ProtocolVersion version) : 
                base(version)
        {
            // Set the service implementation properties
            m_service = service;

            // Set base service properties
            ServiceNamespace = new WsXmlNamespace("ise", "http://polistudio/");
            ServiceID = "urn:uuid:1bf44406-ce21-4fe4-a712-05fe65f6ced2";
            ServiceTypeName = "IService";

            // Add service types here
            ServiceOperations.Add(new WsServiceOperation("http://polistudio/IService", "login"));
            ServiceOperations.Add(new WsServiceOperation("http://polistudio/IService", "portRequest"));
            ServiceOperations.Add(new WsServiceOperation("http://polistudio/IService", "logout"));

            // Add event sources here
        }
        
        public IService(IIService service) : 
                this(service, new ProtocolVersion10())
        {
        }
        
        public virtual WsMessage login(WsMessage request)
        {
            // Build request object
            loginDataContractSerializer reqDcs;
            reqDcs = new loginDataContractSerializer("login", "http://polistudio/");
            login req;
            req = ((login)(reqDcs.ReadObject(request.Reader)));
            request.Reader.Dispose();
            request.Reader = null;

            // Create response object
            // Call service operation to process request and return response.
            loginResponse resp;
            resp = m_service.login(req);

            // Create response header
            WsWsaHeader respHeader = new WsWsaHeader("http://polistudio/IService/loginResponse", request.Header.MessageID, m_version.AnonymousUri, null, null, null);
            WsMessage response = new WsMessage(respHeader, resp, WsPrefix.Wsdp);
            
            // Create response serializer
            loginResponseDataContractSerializer respDcs;
            respDcs = new loginResponseDataContractSerializer("loginResponse", "http://polistudio/");
            response.Serializer = respDcs;
            return response;
        }
        
        public virtual WsMessage portRequest(WsMessage request)
        {
            // Build request object
            portRequestDataContractSerializer reqDcs;
            reqDcs = new portRequestDataContractSerializer("portRequest", "http://polistudio/");
            portRequest req;
            req = ((portRequest)(reqDcs.ReadObject(request.Reader)));
            request.Reader.Dispose();
            request.Reader = null;

            // Create response object
            // Call service operation to process request and return response.
            portRequestResponse resp;
            resp = m_service.portRequest(req);

            // Create response header
            WsWsaHeader respHeader = new WsWsaHeader("http://polistudio/IService/portRequestResponse", request.Header.MessageID, m_version.AnonymousUri, null, null, null);
            WsMessage response = new WsMessage(respHeader, resp, WsPrefix.Wsdp);

            // Create response serializer
            portRequestResponseDataContractSerializer respDcs;
            respDcs = new portRequestResponseDataContractSerializer("portRequestResponse", "http://polistudio/");
            response.Serializer = respDcs;
            return response;
        }
        
        public virtual WsMessage logout(WsMessage request)
        {
            // Build request object
            logoutDataContractSerializer reqDcs;
            reqDcs = new logoutDataContractSerializer("logout", "http://polistudio/");
            logout req;
            req = ((logout)(reqDcs.ReadObject(request.Reader)));
            request.Reader.Dispose();
            request.Reader = null;

            // Create response object
            // Call service operation to process request and return response.
            logoutResponse resp;
            resp = m_service.logout(req);

            // Create response header
            WsWsaHeader respHeader = new WsWsaHeader("http://polistudio/IService/logoutResponse", request.Header.MessageID, m_version.AnonymousUri, null, null, null);
            WsMessage response = new WsMessage(respHeader, resp, WsPrefix.Wsdp);

            // Create response serializer
            logoutResponseDataContractSerializer respDcs;
            respDcs = new logoutResponseDataContractSerializer("logoutResponse", "http://polistudio/");
            response.Serializer = respDcs;
            return response;
        }
    }
}