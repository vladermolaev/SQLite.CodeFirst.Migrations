using System.Data.Entity;

namespace SampleDataModel
{
    // This is a sample data model class which implicitly inherits from DbContext.
    // Entity Framework creates a new database according to your model classes and
    // also abstracts access to the database via your model classes. The only custom
    // thing that SQLiteDB.SqLiteDbContext adds is semi-automatic migration
    // functionality when database already exists and its version is below
    // CurrentSchemaVersion (see Migrations/readme.txt for details).
    public class DataModel : SQLiteDB.SqLiteDbContext<DataModel>
    {
        private const int CurrentSchemaVersion = 0;
        public DataModel() : base("name=DataModel", CurrentSchemaVersion) { }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        //public virtual DbSet<Order> Orders { get; set; }
    }
}
