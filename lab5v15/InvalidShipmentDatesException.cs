using System;

/// <summary>
/// Виняток, що виникає, коли дата доставки (DeliveryDate) вказана раніше
/// за дату відправлення (ShipmentDate).
/// </summary>
public class InvalidShipmentDatesException : Exception
{
    public DateTime ShipmentDate { get; }
    public DateTime DeliveryDate { get; }

    public InvalidShipmentDatesException(DateTime shipmentDate, DateTime deliveryDate)
        : base($"Помилка дат: Дата доставки ({deliveryDate:d}) не може бути раніше дати відправлення ({shipmentDate:d}).")
    {
        ShipmentDate = shipmentDate;
        DeliveryDate = deliveryDate;
    }
}