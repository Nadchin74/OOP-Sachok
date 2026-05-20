using System;
using System.Collections.Generic;

namespace lab30v15
{
    public class DiscountCalculator
    {
        // База доступних купонів та їх відсоток знижки
        private readonly Dictionary<string, decimal> _validCoupons = new(StringComparer.OrdinalIgnoreCase)
        {
            { "SAVE10", 10m },
            { "SAVE20", 20m },
            { "HALFOFF", 50m }
        };

        // Розрахунок ціни зі знижкою у відсотках
        public decimal CalculateDiscount(decimal price, decimal discountPercentage)
        {
            if (price < 0)
                throw new ArgumentException("Ціна не може бути від'ємною.");

            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Відсоток знижки має бути в межах від 0 до 100.");

            decimal discountAmount = price * (discountPercentage / 100m);
            return price - discountAmount;
        }

        // Застосування купона до ціни
        public decimal ApplyCoupon(decimal price, string couponCode)
        {
            if (price < 0)
                throw new ArgumentException("Ціна не може бути від'ємною.");

            if (string.IsNullOrWhiteSpace(couponCode))
                throw new ArgumentException("Купон не може бути порожнім.");

            if (!_validCoupons.TryGetValue(couponCode, out decimal discountPercentage))
                throw new ArgumentException("Недійсний код купона.");

            return CalculateDiscount(price, discountPercentage);
        }
    }
}