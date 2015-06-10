using System.Net;
using easyTcp.Common.Model.Client.Parse;
using easyTcp.Common.Model.Client.Render;

namespace easyTcp.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Common.Model.Client.Connection.Client();
            client.Start(IPAddress.Parse("127.0.0.1"), 4000, new DefaultRenderStrategy(), new DefaultParseStrategy());

        }
    }
}
