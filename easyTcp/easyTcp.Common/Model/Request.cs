using System;

namespace easyTcp.Common.Model
{
    [Serializable]
    public class Request
    {
        public Type Type { get; set; }
        public Object Payload { get; set; }
    }
}
