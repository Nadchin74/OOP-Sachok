namespace lab20
{
    // Інтерфейс для логіки валідації
    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    // Інтерфейс для доступу до даних (Repository Pattern)
    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }

    // Інтерфейс для служби сповіщень
    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }
}