/// <summary>
/// Інтерфейс IMenuItem: визначає контракт для всіх елементів меню.
/// </summary>
public interface IMenuItem
{
    string Name { get; }
    double Price { get; }
    int Calories { get; }

    /// <summary>
    /// Абстрактна вимога: повертає детальний опис елемента.
    /// </summary>
    string GetDetails();
}