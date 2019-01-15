using Shared.Communications;

namespace Shared.Contract
{
    public interface IPackage
    {
        int Id { get; }
        DataPackage Data { get; set; }
        ICommunication Communication { get; set; }
    }
}
