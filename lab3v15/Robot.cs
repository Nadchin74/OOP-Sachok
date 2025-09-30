namespace lab3v15;

// Оголошуємо абстрактний базовий клас. Не можна створити об'єкт цього класу.
public abstract class Robot
{
    // Поля, доступні для дочірніх класів
    protected string Name;
    protected int TasksPerHour;

    // Конструктор базового класу
    public Robot(string name, int tasksPerHour)
    {
        this.Name = name;
        this.TasksPerHour = tasksPerHour;
        Console.WriteLine($"Робот '{Name}' створений.");
    }

    // Деструктор
    ~Robot()
    {
        Console.WriteLine($"Робот '{Name}' знищений.");
    }

    // Абстрактний метод, який ОБОВ'ЯЗКОВО мають реалізувати дочірні класи
    public abstract void Work();

    // Віртуальний метод, який дочірні класи МОЖУТЬ перевизначити
    public virtual void ShowInfo()
    {
        Console.WriteLine($"Я робот, моє ім'я - {Name}.");
        // Розрахунок згідно з варіантом: кількість виконаних завдань за годину
        Console.WriteLine($"Продуктивність: {TasksPerHour} завдань на годину.");
    }
}