using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace DCC.FraudDetection.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        private IConfiguration _configuration;
        private KeyVaultClient _kv;
        private string _clientId;
        private string _clientSecret;


        public KeyVaultService(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration["KeyVault:ClientId"];
            _clientSecret = _configuration["KeyVault:ClientSecret"];

            // using managed identities
            _kv = new KeyVaultClient(GetAccessTokenAsync);
        }
        public async Task<X509Certificate2> GetCert(string keyvalutPath, string certName)
        {
            try
            {
                var cert = await _kv.GetCertificateAsync(keyvalutPath, certName);
                var secretRetrieved = await _kv.GetSecretAsync(cert.SecretIdentifier.ToString()).ConfigureAwait(false);
                var pfxBytes = Convert.FromBase64String(secretRetrieved.Value);
                var certificate = new X509Certificate2(pfxBytes);

                return certificate;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {

            ClientCredential clientCredential = new ClientCredential(_clientId, _clientSecret);
            AuthenticationContext authenticationContext = new AuthenticationContext(authority, false);
            AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resource, clientCredential).ConfigureAwait(false);
            return result.AccessToken;

        }
    }
}
