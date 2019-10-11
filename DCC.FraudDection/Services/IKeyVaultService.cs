using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DCC.FraudDetection.Services
{
    public interface IKeyVaultService
    {
        Task<X509Certificate2> GetCert(string thumbprint, string certId);
    }
}
