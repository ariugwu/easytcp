using System.Net;
using easyTcp.Client.Domain.Render;

namespace easyTcp.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Domain.Connection.Client();
            client.Start(IPAddress.Parse("127.0.0.1"), 4000, new DefaultRenderStrategy());
        }
    }
}
