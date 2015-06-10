using System;

namespace easyTcp.Common.Model.Server
{
    public class DefaultProcessStartegy : IProcessStrategy
    {
        public Response ProcessRequest(Request request)
        {
            return new Response() { Type = typeof(String), Payload = "This is a default response to the 'Process Request'." };
        }
    }
}
