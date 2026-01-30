namespace IndependentWork16
{
    public interface IProductCatalog
    {
        bool IsProductAvailable(string name);
    }

    public class ProductCatalog : IProductCatalog
    {
        // Імітація бази даних товарів
        private readonly List<string> _availableProducts = new List<string> 
        { 
            "Laptop", "Mouse", "Keyboard", "Monitor" 
        };

        public bool IsProductAvailable(string name)
        {
            return _availableProducts.Contains(name);
        }
    }
}