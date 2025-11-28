using System;

public enum ShipmentStatus { Delivered, Lost, Damaged, InTransit }

/// <summary>
/// Сутність "Відправлення". Контролює логіку дат при створенні.
/// </summary>
public class Shipment
{
    public int Id { get; }
    public string DestinationCity { get; }
    public DateTime ShipmentDate { get; }
    public DateTime DeliveryDate { get; }
    public ShipmentStatus Status { get; }

    /// <summary>
    /// Обчислювана властивість: Термін доставки у днях.
    /// </summary>
    public int DeliveryDays => (DeliveryDate - ShipmentDate).Days;

    public Shipment(int id, string destination, DateTime shipDate, DateTime? deliveryDate, ShipmentStatus status)
    {
        // 1. Контроль вхідних даних + власний виняток
        if (deliveryDate.HasValue && deliveryDate.Value < shipDate)
        {
            throw new InvalidShipmentDatesException(shipDate, deliveryDate.Value);
        }

        Id = id;
        DestinationCity = destination;
        ShipmentDate = shipDate;
        // Якщо доставка не відбулася (Lost, Damaged), встановлюємо дату відправлення + 1 (для безпечного розрахунку)
        // або використовуємо сьогоднішню дату для InTransit
        DeliveryDate = deliveryDate ?? (status == ShipmentStatus.InTransit ? DateTime.Now.Date : shipDate.AddDays(1)); 
        Status = status;
    }
}