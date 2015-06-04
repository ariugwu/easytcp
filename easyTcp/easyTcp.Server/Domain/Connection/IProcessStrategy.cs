using easyTcp.Common.Model;

namespace easyTcp.Server.Domain.Connection
{
    public interface IProcessStrategy
    {
        Response ProcessRequest(Request request);
    }
}
