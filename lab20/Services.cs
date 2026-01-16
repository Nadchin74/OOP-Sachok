using System;
using System.Collections.Generic;
using System.Linq;

namespace lab20
{
    // SRP: Відповідає ЛИШЕ за правила перевірки замовлення.
    // Причина для зміни: зміна бізнес-правил (наприклад, мінімальна сума).
    public class OrderValidator : IOrderValidator
    {
        public bool IsValid(Order order)
        {
            return order.TotalAmount > 0;
        }
    }

    // SRP: Відповідає ЛИШЕ за збереження/зчитування даних.
    // Причина для зміни: зміна способу зберігання (SQL, файл, пам'ять).
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly List<Order> _database = new List<Order>();

        public void Save(Order order)
        {
            _database.Add(order);
            Console.WriteLine($"[Database] Замовлення #{order.Id} збережено.");
        }

        public Order GetById(int id)
        {
            return _database.FirstOrDefault(o => o.Id == id);
        }
    }

    // SRP: Відповідає ЛИШЕ за відправку листів.
    // Причина для зміни: зміна формату листа або провайдера пошти.
    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            Console.WriteLine($"[Email] Лист для {order.CustomerName}: Ваше замовлення оброблено.");
        }
    }
}