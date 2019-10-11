using DCC.FraudDetection.Models;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCC.FraudDetection.Services
{
    public class FraudDataAccess : IRavenDbAccess<FraudUIAlerts>
    {
        IDocumentStoreHolder _documentStore;

        /// <summary>
        /// Gets the RavenDB document session created for the current request. 
        /// Changes will be saved automatically when the action finishes executing without error.
        /// </summary>
        public IAsyncDocumentSession DbAsyncSession { get; private set; }

        public IDocumentSession DbSession { get; private set; }


        public FraudDataAccess(IDocumentStoreHolder documentStore)
        {
            try
            {
                _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
                DbAsyncSession = _documentStore.Store.OpenAsyncSession();
                DbSession = _documentStore.Store.OpenSession();

                // RavenDB best practice: during save, wait for the indexes to update.
                // This way, Post-Redirect-Get scenarios won't be affected by stale indexes.
                // For more info, see https://ravendb.net/docs/article-page/3.5/Csharp/client-api/session/saving-changes
                DbSession.Advanced.WaitForIndexesAfterSaveChanges(timeout: TimeSpan.FromSeconds(30), throwOnTimeout: false);
                DbAsyncSession.Advanced.WaitForIndexesAfterSaveChanges(timeout: TimeSpan.FromSeconds(30), throwOnTimeout: false);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public FraudUIAlerts AddItem(FraudUIAlerts item)
        {
            DbSession.Store(item, item.id);
            DbSession.SaveChanges();
            return item;
        }

        public async Task<FraudUIAlerts> AddItemAsync(FraudUIAlerts item)
        {
            await DbAsyncSession.StoreAsync(item, item.id);
            await DbAsyncSession.SaveChangesAsync();
            return item;
        }

        public void DeleteItem(string id)
        {
            DbSession.Delete(id);
            DbSession.SaveChanges();
        }

        public List<FraudUIAlerts> GetAllItems()
        {
            var temp = DbSession.Query<FraudUIAlerts>().ToList();

            return temp;
        }

        public FraudUIAlerts GetItem(string id)
        {
            return DbSession.Query<FraudUIAlerts>().FirstOrDefault(x => x.id == id);

        }

        public IAsyncDocumentSession GetDbAsyncSession()
        {
            return DbAsyncSession;
        }

        public IDocumentSession GetDbSession()
        {
            return DbSession;
        }

        public List<FraudUIAlerts> GetItems(Func<FraudUIAlerts, bool> filter = null)
        {
            return DbSession.Query<FraudUIAlerts>().Where(filter ?? (s => true)).ToList();
        }

        public List<FraudUIAlerts> GetAllItems(Func<FraudUIAlerts, bool> filter)
        {
            return DbSession.Query<FraudUIAlerts>().Where(filter ?? (s => true)).ToList();
        }

        public FraudUIAlerts Update(FraudUIAlerts item)
        {
            var db = DbSession.Load<FraudUIAlerts>(item.id);
            db.FraudTransaction = item.FraudTransaction;
            db.AgentAssigned = db.AgentAssigned;

            DbSession.Store(db, db.id);
            DbSession.SaveChanges();
            return db;
        }

        public async Task<FraudUIAlerts> UpdateAsync(FraudUIAlerts item)
        {
            var db = await DbAsyncSession.LoadAsync<FraudUIAlerts>(item.id);
            db = item;
            await DbAsyncSession.StoreAsync(db, db.id);
            await DbAsyncSession.SaveChangesAsync();

            return item;
        }

        public Task<FraudUIAlerts> LoadAsync(string id)
        {
            throw new NotImplementedException();
        }

        public FraudUIAlerts Load(string id)
        {
            throw new NotImplementedException();
        }
    }
}
