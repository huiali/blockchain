using Huiali.Blockchain.Interface;
using Huiali.Blockchain.KeyStorage;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Threading.Tasks;

namespace Huiali.Blockchain
{
    public class TransactionHandler : ITransactionHandler
    {
        private Web3 _web3;
        private readonly KeyVaultSetting _keyVaultSetting;
        private readonly BlockChainSetting _blockChinaSetting;
        public TransactionHandler(IOptions<BlockChainSetting> blockChinaSetting, IOptions<KeyVaultSetting> keyVaultSetting)
        {
            _keyVaultSetting = keyVaultSetting.Value;
            _blockChinaSetting = blockChinaSetting.Value;
        }

        public async Task<BlockchainTransactionReceipt> SendTransaction(string fromAccountAddress, string toAccountAddress, BigInteger amount)
        {
            string privateKey = KeyVaultHelpler.GetSecret(_keyVaultSetting.VaultAddress, _keyVaultSetting.ClientId, _keyVaultSetting.ClientSecret, fromAccountAddress);
            var account = new Account(privateKey);
            _web3 = new Web3(account, _blockChinaSetting.BlockChainUrl);
            var hexAmount = UnitConversion.Convert.ToWei(amount);
            var transactionPolling = _web3.TransactionManager.TransactionReceiptService;
            var transactionReceipt = await transactionPolling.SendRequestAndWaitForReceiptAsync(() =>
                _web3.TransactionManager.SendTransactionAsync(fromAccountAddress, toAccountAddress, new HexBigInteger(hexAmount))
            );

            return new BlockchainTransactionReceipt
            {
                TransactionHash = transactionReceipt.TransactionHash,
                TransactionIndex = transactionReceipt.TransactionIndex?.Value.ToString(),
                BlockHash = transactionReceipt.BlockHash,
                BlockNumber = transactionReceipt.BlockNumber?.Value.ToString(),
                GasUsed = transactionReceipt.GasUsed?.Value.ToString(),
                Status = transactionReceipt.Status?.Value.ToString()
            };
        }

        public async Task<BigInteger> GetBalance(string account)
        {
            return await _web3.Eth.GetBalance.SendRequestAsync(account);
        }

        public async Task InitAccountBalance(params string[] accountHash)
        {
            var adminAccount = await _web3.Eth.CoinBase.SendRequestAsync();
            foreach (var item in accountHash)
            {
                await _web3.Eth.TransactionManager.SendTransactionAsync(adminAccount, item, new HexBigInteger(Web3.Convert.ToWei(1000000)));
            }
        }
    }
}
