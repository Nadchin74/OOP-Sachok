namespace Lab21
{
    public class InternationalShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            // distance * 5.0 + weight * 2.0 + 15% податок
            decimal baseCost = (distance * 5.0m) + (weight * 2.0m);
            return baseCost * 1.15m; 
        }
    }
}