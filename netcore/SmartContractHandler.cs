using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Huiali.Blockchain.Interface;

namespace Huiali.Blockchain
{
    public class SmartContractHandler : ISmartContractHandler
    {
        private readonly Web3 _web3;
        public SmartContractHandler(IOptions<BlockChainSetting> blockchinaSetting)
        {
            var account = new Account("xxx");
            _web3 = new Web3(account, blockchinaSetting.Value.BlockChainUrl);
        }

        public async Task<BlockchainTransactionReceipt> SendRequest(string contractAddress, string abi, string functionName, params object[] parameters)
        {
            var adminAccount = await _web3.Eth.CoinBase.SendRequestAsync();
            var gas = new HexBigInteger(900000);
            var contract = _web3.Eth.GetContract(abi, contractAddress);
            var func1 = contract.GetFunction("UpdateUserAccount");
            var transactionPolling = _web3.TransactionManager.TransactionReceiptService;
            var transactionReceipt = await transactionPolling.SendRequestAndWaitForReceiptAsync(() =>
                func1.SendTransactionAsync(
                    adminAccount,
                    gas,
                    null,
                    parameters
                )
            );
            var receipt = new BlockchainTransactionReceipt
            {
                From = adminAccount,
                TransactionHash = transactionReceipt.TransactionHash,
                BlockHash = transactionReceipt.BlockHash,
                BlockNumber = transactionReceipt.BlockNumber?.Value.ToString(),
                ContractAddress = contractAddress,
                CumulativeGasUsed = transactionReceipt.CumulativeGasUsed?.Value.ToString(),
                GasUsed = transactionReceipt.GasUsed?.Value.ToString(),
                Status = transactionReceipt.Status?.Value.ToString(),
                TransactionIndex = transactionReceipt.TransactionIndex?.Value.ToString()
            };
            return receipt;
        }
    }
}
