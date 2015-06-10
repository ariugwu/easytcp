using System;

namespace easyTcp.Common.Model.Client.Render
{
    public class DefaultRenderStrategy : IRenderStrategy
    {
        public void Render(Response response)
        {
            Console.WriteLine((String)response.Payload);
        }
    }
}
