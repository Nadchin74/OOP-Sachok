using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Клас-утиліта, що містить узагальнений метод TopN.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Узагальнений метод, що повертає топ-N елементів з колекції.
    /// </summary>
    /// <typeparam name="T">Тип елемента колекції.</typeparam>
    /// <param name="source">Вхідна колекція.</param>
    /// <param name="n">Кількість елементів для повернення.</param>
    /// <param name="metricSelector">Функція, що повертає числову метрику для сортування (наприклад, ціна, термін доставки).</param>
    /// <returns>Колекція топ-N елементів.</returns>
    public static IEnumerable<T> TopN<T>(
        this IEnumerable<T> source, 
        int n, 
        Func<T, double> metricSelector)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (metricSelector == null) throw new ArgumentNullException(nameof(metricSelector));
        if (n <= 0) return Enumerable.Empty<T>();
        
        // Сортуємо колекцію за числовою метрикою у порядку спадання
        return source
            .OrderByDescending(metricSelector)
            .Take(n);
    }
}