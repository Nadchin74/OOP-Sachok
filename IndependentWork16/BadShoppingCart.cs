using System;
using System.Collections.Generic;

namespace IndependentWork16
{
    // Цей клас порушує SRP
    public class BadShoppingCart
    {
        private List<string> _items = new List<string>();
        private List<decimal> _prices = new List<decimal>();

        public void AddItem(string name, decimal price)
        {
            // Логіка перевірки (Catalog responsibility)
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Error: Name is empty");
                return;
            }

            _items.Add(name);
            _prices.Add(price);
            Console.WriteLine($"Item {name} added.");
        }

        // Логіка розрахунку (Calculator responsibility)
        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var price in _prices)
            {
                total += price;
            }
            return total;
        }

        // Логіка збереження (Storage responsibility)
        public void SaveCart()
        {
            Console.WriteLine("Saving cart to database...");
            // Code to save to SQL...
        }
    }
}