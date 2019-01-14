// With the following implementation (SqLitePersistence) of IPersistentData
// simplest way to access your local DB is:
//    var clients = await new SqLitePersistence().GetClients();

// Or make SqLitePersistence class static (make all methods static) and then get
// clients like so:
//    var clients = await SqLitePersistence.GetClients();

// Alternatively, you may need to abstract from data access implementation at
// the call site and use IPersistentData interface with your choice of IoC
// container. The following example uses Ninject.

// First is to have small static Services class for serialized access to Ninject Kernel:
//public static class Services
//{
//    public static IKernel Kernel { get; } = new StandardKernel();
//    public static object Get(Type service) { lock (Kernel) return Kernel.Get(service); }
//    public static T Get<T>() { lock (Kernel) return Kernel.Get<T>(); }
//    public static IEnumerable<object> GetAll(Type service) { return Kernel.GetAll(service); }
//    public static IBindingToSyntax<T> Bind<T>() { return Kernel.Bind<T>(); }
//}

// Second is to bind IPersistentData interface to a singleton implementation,
// e.g. in application bootstrapper:
//    Services.Bind<SampleDataAccess.IPersistentData>().
//               To<SampleDataAccess.SqLitePersistence>().InSingletonScope();

// And third is to access the database via this service:
//    var clients = Services.Get<SampleDataAccess.IPersistentData>().GetClients();

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SampleDataModel;

namespace SampleDataAccess
{
    internal class SqLitePersistence : IPersistentData
    {
        public void AddClient(IClient c)
        {
            using (var db = new DataModel())
            {
                db.Clients.Add((Client)c);
                db.SaveChanges();
            }
        }
        public void UpdateClient(IClient c)
        {
            using (var db = new DataModel())
            {
                var existingClient = db.Clients.Find(c.ClientId);
                if (existingClient == null) return;
                db.Entry(existingClient).CurrentValues.SetValues(c);
                db.SaveChanges();
            }
        }
        public void RemoveClient(int clientId)
        {
            using (var db = new DataModel())
            {
                var c = new Client { ClientId = clientId };
                db.Clients.Attach(c);
                db.Clients.Remove(c);
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    // Client with this ID is not in the table.
                }
            }
        }
        public Task<List<IClient>> GetClients()
        {
            using (var data = new DataModel())
            {
                return (from clientList in data.Clients select clientList).ToListAsync<IClient>();
            }
        }
    }
}
