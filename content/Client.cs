using System.Data.Entity;

namespace SampleDataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ClientId { get; set; }

        [Required]
        [StringLength(128)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(128)]
        public string LastName { get; set; }
    }
}