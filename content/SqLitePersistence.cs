// With the following implementation (SqLitePersistence) of IPersistentData
// simplest way to access your local DB is:
//    var clients = await new SqLitePersistence().GetClients();

// Or make SqLitePersistence class static (make all methods static) and then get
// clients without creating an object of the class:
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

// Current implementation uses two conventions for consistensy and simplicity
// of data access.
// 1. Name of a table is derived from name of a model class by adding 's' to the
// end, e.g. for model class 'Client' expected table name in the database is
// 'Clients'. If model class ends with 's' then its name is used as a table name
// as is, e.g. for model class 'Settings' expected table name in the database is
// also 'Settings'.
// 2. Property name for the primary key is name of the model class appended with
// 'Id', e.g. model class 'Client' is expected to have 'ClientId' property to
// access primary key column.
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SampleDataModel;

namespace SampleDataAccess
{
    internal class SqLitePersistence : IPersistentData
    {
        public void AddClient(IClient c) { AddElement((Client)c); }
        public void UpdateClient(IClient c) { UpdateElement((Client)c); }
        public void RemoveClient(int id) { RemoveElement<Client>(id); }
        public Task<List<IClient>> GetClients() { return GetElements<IClient, Client>(); }

        public void AddProduct(IProduct p) { AddElement((Product)p); }
        public void UpdateProduct(IProduct p) { UpdateElement((Product)p); }
        public void RemoveProduct(int id) { RemoveElement<Product>(id); }
        public Task<List<IProduct>> GetProducts() { return GetElements<IProduct, Product>(); }

        private static string ElementNameToTableName(string elementTypeName)
        {
            return elementTypeName.EndsWith("s") ? elementTypeName : elementTypeName + "s";
        }
        private static void AddElement<T>(T e) where T : class
        {
            using (var db = new DataModel())
            {
                var tableName = ElementNameToTableName(typeof(T).Name);
                var tableModel = (DbSet<T>)typeof(DataModel).GetProperty(tableName).GetValue(db);
                tableModel.Add(e);
                db.SaveChanges();
            }
        }
        private static void UpdateElement<T>(T e) where T : class
        {
            using (var db = new DataModel())
            {
                var tableName = ElementNameToTableName(typeof(T).Name);
                var tableModel = (DbSet<T>)typeof(DataModel).GetProperty(tableName).GetValue(db);
                var elementIdPropertyName = typeof(T).Name + "Id";
                var elementId = (long)typeof(T).GetProperty(elementIdPropertyName).GetValue(e);
                var existingElement = tableModel.Find(elementId);
                if (existingElement == null) return;
                db.Entry(existingElement).CurrentValues.SetValues(e);
                db.SaveChanges();
            }
        }
        private static void RemoveElement<T>(long elementId) where T : class, new()
        {
            using (var db = new DataModel())
            {
                var tableName = ElementNameToTableName(typeof(T).Name);
                var tableModel = (DbSet<T>)typeof(DataModel).GetProperty(tableName).GetValue(db);
                var elementIdPropertyName = typeof(T).Name + "Id";
                var e = new T();
                typeof(T).GetProperty(elementIdPropertyName).SetValue(e, elementId);
                tableModel.Attach(e);
                tableModel.Remove(e);
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    // Element with this ID is not in the table.
                }
            }
        }
        private static Task<List<IT>> GetElements<IT, T>() where T : class, IT
        {
            using (var db = new DataModel())
            {
                var tableName = ElementNameToTableName(typeof(T).Name);
                var tableModel = (DbSet<T>)typeof(DataModel).GetProperty(tableName).GetValue(db);
                return (from list in tableModel select list).ToListAsync<IT>();
            }
        }
    }
}
