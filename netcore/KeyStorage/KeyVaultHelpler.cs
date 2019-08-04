using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Huiali.Blockchain.KeyStorage
{
    public static class KeyVaultHelpler
    {
        public static X509Certificate2 GetCertByThumbprint(string vaultAddress, string clientId, string clientSecret, string thumbprint)
        {
            var keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
            {
                var authContext = new AuthenticationContext(authority);
                var clientCred = new ClientCredential(clientId, clientSecret);
                var result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                    throw new InvalidOperationException("Failed to obtain the JWT token");

                return result.AccessToken;
            });

            const int maxResults = 20;
            var results = Task.Run(() => keyVaultClient.GetCertificatesAsync(vaultAddress, maxResults))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (results == null) return null;
            return (results.Select(m => new {m, thumbprintNext = ByteArrayToString(m.X509Thumbprint).ToLower()})
                .Where(@t => @t.thumbprintNext == thumbprint.ToLower())
                .Select(@t => @t.m.Identifier.Name)
                .Select(name =>
                    Task.Run(() => keyVaultClient.GetCertificateAsync(vaultAddress, name))
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult())
                .Select(certificateBundle => certificateBundle.SecretIdentifier.Identifier)
                .Select(identifier => Task.Run(() => keyVaultClient.GetSecretAsync(identifier))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult())
                .Select(secret => new X509Certificate2(Convert.FromBase64String(secret.Value)))).FirstOrDefault();
        }


        public static async Task CreateSecret(string vaultAddress, string clientId, string clientSecret, string secretName, string secretValue)
        {
            var keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
            {
                var authContext = new AuthenticationContext(authority);
                var clientCred = new ClientCredential(clientId, clientSecret);
                var result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                    throw new InvalidOperationException("Failed to obtain the JWT token");

                return result.AccessToken;
            });
            var retryWithExponentialBackoff = new RetryWithExponentialBackoff();
            Task t = keyVaultClient.SetSecretAsync(vaultAddress, secretName, secretValue, null, "plaintext");
            await retryWithExponentialBackoff.RunAsync(() => t);
        }


        public static string GetSecret(string vaultAddress, string clientId, string clientSecret, string secretName)
        {
            var keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
            {
                var authContext = new AuthenticationContext(authority);
                var clientCred = new ClientCredential(clientId, clientSecret);
                var result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                    throw new InvalidOperationException("Failed to obtain the JWT token");

                return result.AccessToken;
            });

            var secret = Task.Run(() => keyVaultClient.GetSecretAsync(vaultAddress, secretName)).ConfigureAwait(false).GetAwaiter().GetResult();
            return secret.Value;
        }

        private static string ByteArrayToString(IEnumerable<byte> byteArray)
        {
            var stringBuilder = new StringBuilder();
            foreach (var t in byteArray)
            {
                stringBuilder.Append(t.ToString("X2"));
            }
            var @string = stringBuilder.ToString();
            return @string;
        }
    }
}
