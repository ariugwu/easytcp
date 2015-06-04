using easyTcp.Common.Model;

namespace easyTcp.Client.Domain.Render
{
    public interface IRenderStrategy
    {
        void Render(Response response);
    }
}
