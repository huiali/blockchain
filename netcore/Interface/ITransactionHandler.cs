using System.Numerics;
using System.Threading.Tasks;

namespace Huiali.Blockchain.Interface
{
    public interface ITransactionHandler
    {
        Task<BlockchainTransactionReceipt> SendTransaction(string fromAccountAddress, string toAccountAddress, BigInteger amount);

        Task<BigInteger> GetBalance(string account);


        Task InitAccountBalance(params string[] accountHash);
    }
}
