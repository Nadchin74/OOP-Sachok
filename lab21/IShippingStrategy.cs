namespace Lab21
{
    // Інтерфейс Стратегії
    public interface IShippingStrategy
    {
        decimal CalculateCost(decimal distance, decimal weight);
    }
}