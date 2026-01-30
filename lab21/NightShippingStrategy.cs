namespace Lab21
{
    public class NightShippingStrategy : IShippingStrategy
    {
        public decimal CalculateCost(decimal distance, decimal weight)
        {
            // Стандартний тариф + нічна націнка 200 грн
            return (distance * 1.5m) + (weight * 0.5m) + 200m;
        }
    }
}