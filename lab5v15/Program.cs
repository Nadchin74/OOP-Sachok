using System;
using System.Linq;

public class Program
{
    public static void Main(string[] args)
    {
        PostOffice po = new PostOffice();
        Console.WriteLine("--- Ініціалізація відправлень ---");
        
        // Створення коректних відправлень
        po.AddShipment(new Shipment(101, "Київ", new DateTime(2025, 11, 1), new DateTime(2025, 11, 5), ShipmentStatus.Delivered)); // 4 дні
        po.AddShipment(new Shipment(102, "Львів", new DateTime(2025, 11, 5), new DateTime(2025, 11, 15), ShipmentStatus.Delivered)); // 10 днів
        po.AddShipment(new Shipment(103, "Київ", new DateTime(2025, 11, 10), null, ShipmentStatus.Lost));
        po.AddShipment(new Shipment(104, "Одеса", new DateTime(2025, 11, 12), new DateTime(2025, 11, 14), ShipmentStatus.Delivered)); // 2 дні
        po.AddShipment(new Shipment(105, "Львів", new DateTime(2025, 11, 15), new DateTime(2025, 11, 20), ShipmentStatus.Damaged));
        po.AddShipment(new Shipment(106, "Київ", new DateTime(2025, 11, 20), null, ShipmentStatus.InTransit));
        
        
        // ===============================================
        // 1. Демонстрація Обробки Винятків (try-catch)
        // ===============================================
        Console.WriteLine("\n--- 1. Демонстрація InvalidShipmentDatesException ---");
        try
        {
            // Спроба створити відправлення з датою доставки раніше дати відправлення
            DateTime shipDate = new DateTime(2025, 11, 25);
            DateTime earlyDelivery = new DateTime(2025, 11, 20);
            
            Console.WriteLine($"Спроба відправити 107: {shipDate:d} -> {earlyDelivery:d}");
            po.AddShipment(new Shipment(107, "Харків", shipDate, earlyDelivery, ShipmentStatus.Delivered));
        }
        catch (InvalidShipmentDatesException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ПОМИЛКА ВИКЛЮЧЕННЯ]: {ex.Message}");
            Console.WriteLine($"Дати: Відправлення {ex.ShipmentDate:d}, Доставка {ex.DeliveryDate:d}");
            Console.ResetColor();
        }

        // ===============================================
        // 2. Демонстрація Обчислень з Колекціями
        // ===============================================
        Console.WriteLine("\n--- 2. Результати Обчислень ---");
        
        // Середній термін доставки
        double avgTime = po.CalculateAverageDeliveryTime();
        Console.WriteLine($"Середній термін доставки (доставлені): {avgTime:F2} днів");

        // Частка втрачених/пошкоджених
        double lossFraction = po.CalculateLossDamageFraction();
        Console.WriteLine($"Частка втрачених/пошкоджених: {lossFraction:F2}%");

        // Топ-напрямки
        Console.WriteLine("\nТоп-2 напрямки за кількістю відправлень:");
        foreach (var dest in po.GetTopDestinations(2))
        {
            Console.WriteLine($"  -> {dest}");
        }

        // ===============================================
        // 3. Демонстрація Узагальненого Методу TopN<T>
        // ===============================================
        Console.WriteLine("\n--- 3. Демонстрація Generics (TopN<Shipment>) ---");
        
        // Знаходимо топ-2 найшвидших відправлення
        var fastestShipments = po.GetAllShipments()
                                 .Where(s => s.Status == ShipmentStatus.Delivered)
                                 .TopN(2, s => (double)s.DeliveryDays * -1); // Множимо на -1, щоб знайти MIN
                                 
        Console.WriteLine("Топ-2 найшвидших доставлених відправлення:");
        int rank = 1;
        foreach (var s in fastestShipments)
        {
            Console.WriteLine($"  {rank++}. {s.DestinationCity} (ID: {s.Id}) - {s.DeliveryDays} днів.");
        }
    }
}