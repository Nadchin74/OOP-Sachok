using System;
using System.Collections.Generic;
using CinemaSystem.Domain;

namespace CinemaSystem.Application
{
    public interface IShowingRepository
    {
        Showing GetById(Guid id);
        void Add(Showing showing);
    }

    public interface IBookingRepository
    {
        void Add(Booking booking);
    }

    public class BookingService
    {
        private readonly IShowingRepository _showingRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IShowingRepository showingRepository, IBookingRepository bookingRepository)
        {
            _showingRepository = showingRepository;
            _bookingRepository = bookingRepository;
        }

        // Use Case: Бронювання квитків
        public Booking BookTickets(Guid showingId, string customerName, List<int> standardSeats, List<int> vipSeats)
        {
            var showing = _showingRepository.GetById(showingId);
            if (showing == null) throw new ArgumentException("Сеанс не знайдено");

            var booking = new Booking(showing, customerName);

            foreach (var seat in standardSeats)
            {
                showing.BookSeat(seat);
                booking.AddTicket(new StandardTicket(seat, showing.BasePrice));
            }

            foreach (var seat in vipSeats)
            {
                showing.BookSeat(seat);
                booking.AddTicket(new VipTicket(seat, showing.BasePrice));
            }

            _bookingRepository.Add(booking);
            return booking;
        }
    }
}