namespace Lab21
{
    public class ExpressShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            // (distance * 2.5 + weight * 1.0) + 50 фіксована доплата
            return ((distance * 2.5m) + (weight * 1.0m)) + 50m;
        }
    }
}