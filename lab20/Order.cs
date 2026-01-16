using System;

namespace lab20
{
    // Статуси замовлення
    public enum OrderStatus
    {
        New,
        PendingValidation,
        Processed,
        Shipped,
        Delivered,
        Cancelled
    }

    // SRP: Цей клас відповідає лише за зберігання даних (DTO).
    // Він не містить жодної бізнес-логіки.
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order(int id, string customerName, decimal totalAmount)
        {
            Id = id;
            CustomerName = customerName;
            TotalAmount = totalAmount;
            Status = OrderStatus.New;
        }
    }
}