using System;
using System.Text;

namespace IndependentWork16
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Демонстрація SRP (ShoppingCart) ===\n");

            // 1. Створення конкретних реалізацій (dependencies)
            IProductCatalog catalog = new ProductCatalog();
            ICartCalculator calculator = new StandardCartCalculator();
            ICartStorage storage = new JsonFileCartStorage();

            // 2. Впровадження залежностей у сервіс
            ShoppingCartService cartService = new ShoppingCartService(catalog, calculator, storage);

            // 3. Сценарій використання
            // Спроба додати товар, якого немає
            cartService.AddToCart("Samsung Phone", 20000, 1);

            // Додавання існуючих товарів
            cartService.AddToCart("Laptop", 35000, 1);
            cartService.AddToCart("Mouse", 800, 2);

            Console.WriteLine("\n--- Оформлення замовлення ---");
            cartService.Checkout();

            Console.ReadLine();
        }
    }
}