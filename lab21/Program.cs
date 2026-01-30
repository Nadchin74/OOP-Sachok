using System;
using System.Text;

namespace Lab21
{
    class Program
    {
        static void Main(string[] args)
        {
            // Налаштування кодування для коректного відображення кирилиці
            Console.OutputEncoding = Encoding.UTF8;

            DeliveryService deliveryService = new DeliveryService();

            while (true)
            {
                try
                {
                    Console.WriteLine("--- Калькулятор Доставки ---");
                    Console.WriteLine("Доступні типи: Standard, Express, International, Night");
                    Console.Write("Введіть тип доставки: ");
                    string type = Console.ReadLine();

                    Console.Write("Введіть відстань (км): ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal distance))
                    {
                        Console.WriteLine("Некоректна відстань!");
                        continue;
                    }

                    Console.Write("Введіть вагу (кг): ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal weight))
                    {
                        Console.WriteLine("Некоректна вага!");
                        continue;
                    }

                    // 1. Створюємо стратегію через Фабрику
                    IShippingStrategy strategy = ShippingStrategyFactory.CreateStrategy(type);

                    // 2. Виконуємо розрахунок через Сервіс
                    decimal cost = deliveryService.CalculateDeliveryCost(distance, weight, strategy);

                    Console.WriteLine($"\n>> Вартість ({type}): {cost:F2} грн");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"\n[Помилка] {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[Критична помилка] {ex.Message}");
                }

                Console.WriteLine("\nНатисніть Enter для продовження...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}