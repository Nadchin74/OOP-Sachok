using System;

namespace lab20
{
    // SRP: Цей клас є оркестратором (координатором).
    // Його відповідальність — керувати потоком виконання процесу замовлення.
    // Він не знає деталей реалізації (як пишеться в БД чи як шлеться пошта), він лише викликає методи.
    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        // Dependency Injection (DI): Залежності передаються ззовні.
        public OrderService(
            IOrderValidator validator, 
            IOrderRepository repository, 
            IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"\n--- Початок обробки замовлення #{order.Id} ---");

            // Крок 1: Валідація
            if (!_validator.IsValid(order))
            {
                Console.WriteLine($"[Error] Валідація не пройдена.");
                return;
            }

            // Крок 2: Бізнес-логіка
            order.Status = OrderStatus.Processed;

            // Крок 3: Збереження
            _repository.Save(order);

            // Крок 4: Сповіщення
            _emailService.SendOrderConfirmation(order);
            
            Console.WriteLine($"--- Замовлення оброблено успішно ---\n");
        }
    }
}