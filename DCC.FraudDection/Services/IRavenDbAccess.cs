using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DCC.FraudDetection.Services
{
    public interface IRavenDbAccess<T>
    {
        List<T> GetAllItems();
        T GetItem(string id);
        T AddItem(T item);
        void DeleteItem(string id);
        Task<T> AddItemAsync(T item);
        List<T> GetAllItems(Func<T, bool> filter = null);
        Task<T> UpdateAsync(T item);
        T Update(T item);
        Task<T> LoadAsync(string id);
        T Load(string id);
        IAsyncDocumentSession GetDbAsyncSession();
        IDocumentSession GetDbSession();

    }
}
