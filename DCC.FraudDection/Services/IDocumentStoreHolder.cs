using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCC.FraudDetection.Services
{
    public interface IDocumentStoreHolder
    {
        IDocumentStore Store { get; }
    }

}
