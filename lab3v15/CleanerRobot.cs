namespace lab3v15;

// Клас CleanerRobot наслідується від Robot
public class CleanerRobot : Robot
{
    private string CleaningArea;

    // Конструктор викликає конструктор базового класу за допомогою : base(...)
    public CleanerRobot(string name, int tasksPerHour, string cleaningArea) : base(name, tasksPerHour)
    {
        this.CleaningArea = cleaningArea;
        Console.WriteLine(" -> Це робот-прибиральник.");
    }

    // Деструктор
    ~CleanerRobot()
    {
        Console.WriteLine($"Робот-прибиральник '{Name}' припинив роботу.");
    }

    // Перевизначення (override) абстрактного методу Work
    public override void Work()
    {
        Console.WriteLine($"Робот {Name} починає прибирання в зоні '{CleaningArea}'.");
    }

    // Перевизначення (override) віртуального методу ShowInfo для додавання деталей
    public override void ShowInfo()
    {
        base.ShowInfo(); // Викликаємо метод базового класу, щоб показати основну інформацію
        Console.WriteLine($"Моя спеціалізація - прибирання. Зона відповідальності: {CleaningArea}.");
    }
}