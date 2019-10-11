using DCC.FraudDetection.Models;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DCC.FraudDetection.Services
{

    public partial class DocumentStoreHolder : IDocumentStoreHolder
    {
        private IKeyVaultService keyVault;
        public DocumentStoreHolder(IOptions<RavenSettings> ravenSettings, IKeyVaultService keyService)
        {
            var settings = ravenSettings.Value;
            keyVault = keyService;

            var cert = keyService.GetCert(settings.CertPath, settings.CertId).ConfigureAwait(false).GetAwaiter().GetResult();
            Store = new DocumentStore
            {
                Certificate = cert,
                Urls = settings.Url.Split(';'),
                Database = settings.DefaultDatabase
            }.Initialize();
        }

        public IDocumentStore Store { get; }
    }
}
