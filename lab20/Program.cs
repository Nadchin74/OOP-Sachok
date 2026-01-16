using System;

namespace lab20
{
    class Program
    {
        static void Main(string[] args)
        {
            // Composition Root: Створення об'єктів та налаштування залежностей
            IOrderValidator validator = new OrderValidator();
            IOrderRepository repository = new InMemoryOrderRepository();
            IEmailService emailService = new ConsoleEmailService();

            // Впроваджуємо залежності в сервіс
            OrderService orderService = new OrderService(validator, repository, emailService);

            // Тест 1: Успішне замовлення
            Order order1 = new Order(1, "Тарас Шевченко", 1000m);
            orderService.ProcessOrder(order1);

            // Тест 2: Помилкове замовлення
            Order order2 = new Order(2, "Леся Українка", -50m);
            orderService.ProcessOrder(order2);

            Console.ReadKey();
        }
    }
}