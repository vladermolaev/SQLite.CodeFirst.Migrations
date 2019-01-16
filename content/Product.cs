namespace SampleDataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product : IProduct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductId { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public int Price { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }
    }
}