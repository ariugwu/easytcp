using System.Collections.Generic;
using System.Net;
using easyTcp.Common.Model;

namespace easyTcp.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var wat = new Domain.Connection.AsyncListener() { Connections = new List<StateObject>() };
            wat.StartListening(IPAddress.Parse("127.0.0.1"), 4000);
        }
    }
}
