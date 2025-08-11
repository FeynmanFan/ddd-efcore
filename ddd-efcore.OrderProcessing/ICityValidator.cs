namespace ddd_efcore.OrderProcessing
{
    public interface ICityValidator
    {
        bool IsValidCity(string zipCode, string cityName, string state);
    }
}