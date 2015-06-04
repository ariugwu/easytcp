using System;
using easyTcp.Common.Model;

namespace easyTcp.Server.Domain.Connection
{
    public class DefaultProcessStartegy : IProcessStrategy
    {
        public Response ProcessRequest(Request request)
        {
            return new Response() { Type = typeof(String), Payload = "This is a default response to the 'Process Request'." };
        }
    }
}
