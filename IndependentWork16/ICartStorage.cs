using System;
using System.Collections.Generic;

namespace IndependentWork16
{
    public interface ICartStorage
    {
        void Save(List<CartItem> items);
    }

    public class JsonFileCartStorage : ICartStorage
    {
        public void Save(List<CartItem> items)
        {
            // Імітація збереження у файл
            Console.WriteLine($"[Storage]: Збережено {items.Count} позицій у файл cart.json.");
        }
    }
}