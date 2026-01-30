namespace Lab21
{
    public class StandardShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            // distance * 1.5 + weight * 0.5
            return (distance * 1.5m) + (weight * 0.5m);
        }
    }
}