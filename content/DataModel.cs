using System.Data.Entity;

namespace SampleDataModel
{
    public class DataModel : SQLiteDB.SqLiteDbContext<DataModel>
    {
        private const int CurrentSchemaVersion = 1;
        public DataModel() : base("name=DataModel", CurrentSchemaVersion) { }
        public virtual DbSet<Client> Clients { get; set; }
        //public virtual DbSet<Product> Products { get; set; }
        //public virtual DbSet<Order> Orders { get; set; }
    }
}
