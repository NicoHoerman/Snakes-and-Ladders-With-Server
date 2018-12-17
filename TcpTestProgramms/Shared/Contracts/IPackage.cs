using Shared.Communications;

namespace Shared.Contract
{
    public interface IPackage
    {
        int Id { get; }
        DataPackage data { get; set; }
        ICommunication communication { get; set; }
    }
}
