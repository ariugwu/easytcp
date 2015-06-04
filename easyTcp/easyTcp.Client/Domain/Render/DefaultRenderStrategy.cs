using System;
using easyTcp.Common.Model;

namespace easyTcp.Client.Domain.Render
{
    public class DefaultRenderStrategy : IRenderStrategy
    {
        public void Render(Response response)
        {
            Console.WriteLine((String)response.Payload);
        }
    }
}
