using System;

namespace easyTcp.Common.Model.Client.Parse
{
    public class DefaultParseStrategy : IParseStrategy
    {
        public string CommandPrompt()
        {
            Console.Write("> ");
            return Console.ReadLine();
        }

        public Request Parse(string command)
        {
            return new Request(){ Payload = command, Type = typeof(string)};
        }
    }
}
