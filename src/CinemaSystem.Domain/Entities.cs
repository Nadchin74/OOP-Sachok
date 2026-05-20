using System;
using System.Collections.Generic;

namespace CinemaSystem.Domain
{
    public interface IEntity
    {
        Guid Id { get; }
    }

    // Абстрактний клас квитка для демонстрації поліморфізму
    public abstract class Ticket : IEntity
    {
        public Guid Id { get; protected set; }
        public int SeatNumber { get; protected set; }
        public decimal BasePrice { get; protected set; }

        protected Ticket(int seatNumber, decimal basePrice)
        {
            if (seatNumber <= 0) throw new ArgumentException("Місце має бути > 0");
            if (basePrice < 0) throw new ArgumentException("Ціна не може бути від'ємною");
            
            Id = Guid.NewGuid();
            SeatNumber = seatNumber;
            BasePrice = basePrice;
        }

        public abstract decimal CalculateFinalPrice();
    }

    public class StandardTicket : Ticket
    {
        public StandardTicket(int seatNumber, decimal basePrice) : base(seatNumber, basePrice) { }
        public override decimal CalculateFinalPrice() => BasePrice;
    }

    public class VipTicket : Ticket
    {
        public VipTicket(int seatNumber, decimal basePrice) : base(seatNumber, basePrice) { }
        public override decimal CalculateFinalPrice() => BasePrice * 1.5m; // VIP на 50% дорожче
    }

    public class Movie : IEntity
    {
        public Guid Id { get; }
        public string Title { get; }
        public int DurationMinutes { get; }

        public Movie(string title, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Назва не може бути порожньою");
            if (durationMinutes <= 0) throw new ArgumentException("Тривалість має бути > 0");
            
            Id = Guid.NewGuid();
            Title = title;
            DurationMinutes = durationMinutes;
        }
    }

    public class Showing : IEntity
    {
        public Guid Id { get; }
        public Movie Movie { get; }
        public DateTime StartTime { get; }
        public decimal BasePrice { get; }
        public int TotalSeats { get; }
        
        // Інкапсуляція списку зайнятих місць
        private readonly HashSet<int> _bookedSeats = new();

        public Showing(Movie movie, DateTime startTime, decimal basePrice, int totalSeats)
        {
            Id = Guid.NewGuid();
            Movie = movie ?? throw new ArgumentNullException(nameof(movie));
            StartTime = startTime;
            BasePrice = basePrice;
            TotalSeats = totalSeats > 0 ? totalSeats : throw new ArgumentException("Мають бути місця");
        }

        public bool IsSeatAvailable(int seatNumber) => 
            !_bookedSeats.Contains(seatNumber) && seatNumber > 0 && seatNumber <= TotalSeats;

        public void BookSeat(int seatNumber)
        {
            if (!IsSeatAvailable(seatNumber))
                throw new InvalidOperationException($"Місце {seatNumber} недоступне.");
            _bookedSeats.Add(seatNumber);
        }
    }

    public class Booking : IEntity
    {
        public Guid Id { get; }
        public Showing Showing { get; }
        public string CustomerName { get; }
        
        private readonly List<Ticket> _tickets = new();
        public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

        public Booking(Showing showing, string customerName)
        {
            Id = Guid.NewGuid();
            Showing = showing ?? throw new ArgumentNullException(nameof(showing));
            CustomerName = string.IsNullOrWhiteSpace(customerName) ? throw new ArgumentException("Ім'я клієнта обов'язкове") : customerName;
        }

        public void AddTicket(Ticket ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            _tickets.Add(ticket);
        }

        public decimal GetTotalAmount()
        {
            decimal total = 0;
            foreach (var ticket in _tickets)
            {
                total += ticket.CalculateFinalPrice();
            }
            return total;
        }
    }
}