namespace lab3v15;

// Клас DeliveryRobot наслідується від Robot
public class DeliveryRobot : Robot
{
    private double MaxPayload;

    // Конструктор викликає конструктор базового класу за допомогою : base(...)
    public DeliveryRobot(string name, int tasksPerHour, double maxPayload) : base(name, tasksPerHour)
    {
        this.MaxPayload = maxPayload;
        Console.WriteLine(" -> Це робот-доставник.");
    }

    // Деструктор
    ~DeliveryRobot()
    {
        Console.WriteLine($"Робот-доставник '{Name}' завершив доставку.");
    }

    // Перевизначення (override) абстрактного методу Work
    public override void Work()
    {
        Console.WriteLine($"Робот {Name} вирушає доставляти вантаж.");
    }

    // Перевизначення (override) віртуального методу ShowInfo для додавання деталей
    public override void ShowInfo()
    {
        base.ShowInfo(); // Викликаємо метод базового класу
        Console.WriteLine($"Моя спеціалізація - доставка. Максимальне навантаження: {MaxPayload} кг.");
    }
}