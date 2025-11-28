/// <summary>
/// Клас Drink: конкретна реалізація для напоїв.
/// Додає специфічну властивість: VolumeLiters.
/// </summary>
public class Drink : MenuItemBase
{
    public double VolumeLiters { get; private set; }

    public Drink(string name, double price, int calories, double volume)
        : base(name, price, calories) // Виклик конструктора базового класу
    {
        this.VolumeLiters = volume;
    }

    /// <summary>
    /// Поліморфна реалізація: повертає деталі напою.
    /// </summary>
    public override string GetDetails()
    {
        return $"[Напій] {Name}. Ціна: {Price:C}. Калорії: {Calories} ккал. Об'єм: {VolumeLiters:F2} л.";
    }
}