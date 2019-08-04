using Huiali.Blockchain.Interface;
using Huiali.Blockchain.KeyStorage;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Huiali.Blockchain
{
    public class AccountHandler : IAccountHandler
    {
        private readonly KeyVaultSetting _keyVaultSetting;
        public AccountHandler(IOptions<KeyVaultSetting> keyVaultSetting)
        {
            _keyVaultSetting = keyVaultSetting.Value;
        }

        public async Task<string> CreateUser()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var address = ecKey.GetPublicAddress();
            var privateKey = ecKey.GetPrivateKey();
            await KeyVaultHelpler.CreateSecret(_keyVaultSetting.VaultAddress, _keyVaultSetting.ClientId, _keyVaultSetting.ClientSecret,
             address, privateKey);
            return address;
        }
    }
}
