/// <summary>
/// Клас Dish: конкретна реалізація для страв.
/// Додає специфічну властивість: PreparationTimeMinutes.
/// </summary>
public class Dish : MenuItemBase
{
    public int PreparationTimeMinutes { get; private set; }

    public Dish(string name, double price, int calories, int prepTime)
        : base(name, price, calories) // Виклик конструктора базового класу
    {
        this.PreparationTimeMinutes = prepTime;
    }

    /// <summary>
    /// Поліморфна реалізація: повертає деталі страви.
    /// </summary>
    public override string GetDetails()
    {
        return $"[Страва] {Name}. Ціна: {Price:C}. Калорії: {Calories} ккал. Час приготування: {PreparationTimeMinutes} хв.";
    }
}