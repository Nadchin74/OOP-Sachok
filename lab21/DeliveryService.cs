using System;

namespace Lab21
{
    public class DeliveryService
    {
        // Принцип OCP: цей клас не змінюється при додаванні нових стратегій.
        // Він залежить лише від абстракції IShippingStrategy.
        public decimal CalculateDeliveryCost(decimal distance, decimal weight, IShippingStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            return strategy.CalculateCost(distance, weight);
        }
    }
}