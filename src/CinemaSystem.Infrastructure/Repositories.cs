using System;
using System.Collections.Generic;
using System.Linq;
using CinemaSystem.Application;
using CinemaSystem.Domain;

namespace CinemaSystem.Infrastructure
{
    public class InMemoryShowingRepository : IShowingRepository
    {
        private readonly List<Showing> _showings = new();
        public void Add(Showing showing) => _showings.Add(showing);
        public Showing GetById(Guid id) => _showings.FirstOrDefault(s => s.Id == id);
    }

    public class InMemoryBookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings = new();
        public void Add(Booking booking) => _bookings.Add(booking);
    }
}