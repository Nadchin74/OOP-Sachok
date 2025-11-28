using System;

public class Program
{
    public static void Main(string[] args)
    {
        // 1. Створення конкретних реалізацій (Dish та Drink)
        IMenuItem steak = new Dish("Філе-міньйон", 450.00, 600, 30);
        IMenuItem tea = new Drink("Зелений чай", 35.50, 5, 0.4);
        IMenuItem cake = new Dish("Чізкейк Нью-Йорк", 120.00, 480, 5);
        IMenuItem water = new Drink("Мінеральна вода (0.5)", 30.00, 0, 0.5);

        // 2. Створення об'єкта Order (Композиція)
        Order clientOrder = new Order();
        clientOrder.AddItem(steak);
        clientOrder.AddItem(tea);
        clientOrder.AddItem(cake);
        clientOrder.AddItem(water); // Додаємо ще один напій

        Console.WriteLine("=======================================");
        Console.WriteLine("       ЗАМОВЛЕННЯ РЕСТОРАННОГО МЕНЮ");
        Console.WriteLine("=======================================");
        
        // 3. Виведення деталей замовлення (використання поліморфізму)
        foreach (var item in clientOrder.GetItems())
        {
            Console.WriteLine(item.GetDetails());
        }

        Console.WriteLine("\n--- Результати обчислень ---");
        // 4. Обчислення загальних показників (вимоги до завдання)
        Console.WriteLine($"Загальна вартість: {clientOrder.GetTotalPrice():C}");
        Console.WriteLine($"Загальна калорійність: {clientOrder.GetTotalCalories()} ккал");
        Console.WriteLine($"Середня вартість елемента: {clientOrder.GetAverageItemPrice():C}");
        Console.WriteLine("=======================================");
    }
}