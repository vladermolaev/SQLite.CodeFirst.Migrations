namespace SampleDataModel
{
    public interface IClient
    {
        long ClientId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}