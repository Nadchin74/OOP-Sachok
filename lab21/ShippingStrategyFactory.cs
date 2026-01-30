using System;

namespace Lab21
{
    public static class ShippingStrategyFactory
    {
        public static IShippingStrategy CreateStrategy(string deliveryType)
        {
            switch (deliveryType?.ToLower().Trim())
            {
                case "standard":
                    return new StandardShippingStrategy();
                case "express":
                    return new ExpressShippingStrategy();
                case "international":
                    return new InternationalShippingStrategy();
                case "night":
                    return new NightShippingStrategy();
                default:
                    // Можна повернути null або викинути виняток, залежно від бізнес-логіки.
                    // Тут викидаємо виняток для наочності.
                    throw new ArgumentException("Невідомий тип доставки.");
            }
        }
    }
}