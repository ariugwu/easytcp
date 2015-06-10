using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyTcp.Common.Model.Client.Parse
{
    public class DefaultParseStrategy : IParseStrategy
    {
        public Request Parse(string command)
        {
            return new Request(){ Payload = command, Type = typeof(string)};
        }
    }
}
