using System.Threading.Tasks;

namespace Huiali.Blockchain.Interface
{ 
    public interface ISmartContractHandler
    {
        Task<BlockchainTransactionReceipt> SendRequest(string contractAddress, string abi, string functionName, params object[] parameters);
    }
}
