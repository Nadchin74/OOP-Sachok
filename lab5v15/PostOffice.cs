using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Клас PostOffice: використовує композицію для керування колекцією відправлень.
/// </summary>
public class PostOffice
{
    // Композиція: PostOffice містить список Shipment
    private readonly List<Shipment> _shipments = new List<Shipment>();

    public void AddShipment(Shipment shipment)
    {
        _shipments.Add(shipment);
    }

    /// <summary>
    /// Обчислення: Середній термін доставки (лише для доставлених відправлень).
    /// </summary>
    public double CalculateAverageDeliveryTime()
    {
        // Фільтруємо лише успішні доставки
        var delivered = _shipments.Where(s => s.Status == ShipmentStatus.Delivered);

        if (!delivered.Any())
        {
            return 0;
        }

        // Обчислення середнього терміну доставки
        return delivered.Average(s => s.DeliveryDays);
    }

    /// <summary>
    /// Обчислення: Частка (відсоток) втрачених або пошкоджених відправлень.
    /// </summary>
    public double CalculateLossDamageFraction()
    {
        if (!_shipments.Any())
        {
            return 0;
        }

        var lostOrDamagedCount = _shipments
            .Count(s => s.Status == ShipmentStatus.Lost || s.Status == ShipmentStatus.Damaged);

        // Повертаємо частку у відсотках
        return (double)lostOrDamagedCount / _shipments.Count * 100;
    }

    /// <summary>
    /// Обчислення: Отримання топ-N напрямків за кількістю відправлень.
    /// </summary>
    public IEnumerable<string> GetTopDestinations(int n)
    {
        return _shipments
            .GroupBy(s => s.DestinationCity)        // Групуємо за містом
            .OrderByDescending(g => g.Count())      // Сортуємо за кількістю відправлень
            .Take(n)                                // Беремо топ-N
            .Select(g => $"{g.Key} ({g.Count()})"); // Форматуємо вивід
    }

    public IEnumerable<Shipment> GetAllShipments() => _shipments;
}