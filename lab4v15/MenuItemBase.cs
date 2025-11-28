/// <summary>
/// Абстрактний клас MenuItemBase: містить спільну реалізацію базових властивостей.
/// </summary>
public abstract class MenuItemBase : IMenuItem
{
    // Реалізація властивостей інтерфейсу
    public string Name { get; private set; }
    public double Price { get; private set; }
    public int Calories { get; private set; }

    /// <summary>
    /// Конструктор для ініціалізації базових полів усіх елементів меню.
    /// </summary>
    public MenuItemBase(string name, double price, int calories)
    {
        Name = name;
        Price = price;
        Calories = calories;
    }

    // Вимагає реалізації у дочірніх класах (поліморфізм)
    public abstract string GetDetails();
}