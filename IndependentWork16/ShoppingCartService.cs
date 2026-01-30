using System;
using System.Collections.Generic;

namespace IndependentWork16
{
    public class ShoppingCartService
    {
        private readonly List<CartItem> _cartItems;
        
        // Залежності (DIP - Dependency Inversion Principle)
        private readonly IProductCatalog _catalog;
        private readonly ICartCalculator _calculator;
        private readonly ICartStorage _storage;

        // Впровадження залежностей через конструктор (Dependency Injection)
        public ShoppingCartService(IProductCatalog catalog, ICartCalculator calculator, ICartStorage storage)
        {
            _cartItems = new List<CartItem>();
            _catalog = catalog;
            _calculator = calculator;
            _storage = storage;
        }

        public void AddToCart(string productName, decimal price, int quantity)
        {
            // 1. Перевірка через каталог
            if (!_catalog.IsProductAvailable(productName))
            {
                Console.WriteLine($"[Error]: Товар '{productName}' не знайдено в каталозі.");
                return;
            }

            // 2. Додавання
            _cartItems.Add(new CartItem { Name = productName, Price = price, Quantity = quantity });
            Console.WriteLine($"[Cart]: Додано '{productName}' ({quantity} шт.)");
        }

        public void Checkout()
        {
            if (_cartItems.Count == 0)
            {
                Console.WriteLine("[Cart]: Кошик порожній.");
                return;
            }

            // 3. Розрахунок через калькулятор
            decimal total = _calculator.CalculateTotal(_cartItems);
            Console.WriteLine($"[Checkout]: Загальна сума до сплати: {total} грн.");

            // 4. Збереження через сховище
            _storage.Save(_cartItems);
            
            // Очищення після покупки
            _cartItems.Clear();
            Console.WriteLine("[Cart]: Покупка завершена, кошик очищено.");
        }
    }
}