namespace SampleDataModel
{
    public interface IProduct
    {
        long ProductId { get; set; }
        string Name { get; set; }
        int Price { get; set; }
        string Description { get; set; }
    }
}