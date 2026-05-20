using System;
using System.Collections.Generic;
using Xunit;
using CinemaSystem.Domain;
using CinemaSystem.Application;
using CinemaSystem.Infrastructure;

namespace CinemaSystem.Tests
{
    public class DomainTests
    {
        [Fact]
        public void Movie_EmptyTitle_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Movie("", 120));
        }

        [Fact]
        public void Ticket_Polymorphism_CalculatesPricesCorrectly()
        {
            Ticket standard = new StandardTicket(1, 100m);
            Ticket vip = new VipTicket(2, 100m);

            Assert.Equal(100m, standard.CalculateFinalPrice());
            Assert.Equal(150m, vip.CalculateFinalPrice());
        }

        [Fact]
        public void Showing_BookSeat_DecreasesAvailability()
        {
            var showing = new Showing(new Movie("Test", 100), DateTime.Now, 100m, 10);
            
            Assert.True(showing.IsSeatAvailable(1));
            showing.BookSeat(1);
            Assert.False(showing.IsSeatAvailable(1));
        }

        [Fact]
        public void Showing_BookAlreadyBookedSeat_ThrowsException()
        {
            var showing = new Showing(new Movie("Test", 100), DateTime.Now, 100m, 10);
            showing.BookSeat(1);
            
            Assert.Throws<InvalidOperationException>(() => showing.BookSeat(1));
        }

        [Fact]
        public void BookingService_BookTickets_CalculatesTotalCorrectly()
        {
            var showingRepo = new InMemoryShowingRepository();
            var bookingRepo = new InMemoryBookingRepository();
            var service = new BookingService(showingRepo, bookingRepo);

            var showing = new Showing(new Movie("Test", 100), DateTime.Now, 100m, 10);
            showingRepo.Add(showing);

            var booking = service.BookTickets(showing.Id, "John", new List<int> { 1, 2 }, new List<int> { 3 });

            // 2 Standard (100+100) + 1 VIP (150) = 350
            Assert.Equal(350m, booking.GetTotalAmount());
            Assert.Equal(3, booking.Tickets.Count);
        }
    }
}