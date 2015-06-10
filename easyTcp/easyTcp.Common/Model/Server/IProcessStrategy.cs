namespace easyTcp.Common.Model.Server
{
    public interface IProcessStrategy
    {
        Response ProcessRequest(Request request);
    }
}
