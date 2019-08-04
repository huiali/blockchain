using System.Threading.Tasks;

namespace Huiali.Blockchain.Interface
{
    public interface IAccountHandler
    {
        Task<string> CreateUser();
    }
}
