using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Клас Order: використовує КОМПОЗИЦІЮ/агрегацію для зв'язку з IMenuItem
/// і виконує розрахунки замовлення.
/// </summary>
public class Order
{
    // Використання композиції (агрегації): Order складається з колекції IMenuItem.
    private readonly List<IMenuItem> _items = new List<IMenuItem>();

    public void AddItem(IMenuItem item)
    {
        _items.Add(item);
    }

    /// <summary>
    /// Обчислення: Загальна калорійність замовлення.
    /// </summary>
    public int GetTotalCalories()
    {
        // Використання LINQ для сумування калорій
        return _items.Sum(item => item.Calories);
    }

    /// <summary>
    /// Обчислення: Загальна вартість замовлення.
    /// </summary>
    public double GetTotalPrice()
    {
        // Використання LINQ для сумування цін
        return _items.Sum(item => item.Price);
    }

    /// <summary>
    /// Обчислення: Середня вартість одного елемента в замовленні.
    /// </summary>
    public double GetAverageItemPrice()
    {
        // Перевірка на порожнє замовлення
        if (!_items.Any())
        {
            return 0.0;
        }
        // Використання LINQ для обчислення середнього
        return GetTotalPrice() / _items.Count;
    }
    
    // Додатковий метод для виведення всіх елементів
    public IEnumerable<IMenuItem> GetItems()
    {
        return _items;
    }
}