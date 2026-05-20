using System;
using System.Collections.Generic;
using System.Linq;
using CinemaSystem.Application;
using CinemaSystem.Domain;
using CinemaSystem.Infrastructure;

namespace CinemaSystem.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            // Dependency Injection (Manually for Console)
            IShowingRepository showingRepo = new InMemoryShowingRepository();
            IBookingRepository bookingRepo = new InMemoryBookingRepository();
            BookingService bookingService = new BookingService(showingRepo, bookingRepo);

            // Початкові дані
            var movie = new Movie("Дюна: Частина друга", 166);
            var showing = new Showing(movie, DateTime.Now.AddDays(1), 200m, 50);
            showingRepo.Add(showing);

            Console.WriteLine("=== Система управління Кінотеатром ===");
            Console.WriteLine($"Доступний сеанс: '{showing.Movie.Title}', Базова ціна: {showing.BasePrice} грн\n");
            
            Console.Write("Введіть ваше ім'я: ");
            string name = Console.ReadLine();

            Console.Write("Номери стандартних місць через кому (або Enter): ");
            var standardInput = Console.ReadLine();
            var stdSeats = string.IsNullOrWhiteSpace(standardInput) ? new List<int>() : standardInput.Split(',').Select(int.Parse).ToList();

            Console.Write("Номери VIP місць через кому (VIP на 50% дорожче, або Enter): ");
            var vipInput = Console.ReadLine();
            var vipSeats = string.IsNullOrWhiteSpace(vipInput) ? new List<int>() : vipInput.Split(',').Select(int.Parse).ToList();

            try
            {
                var booking = bookingService.BookTickets(showing.Id, name, stdSeats, vipSeats);
                Console.WriteLine("\n✅ Бронювання успішне!");
                Console.WriteLine($"Клієнт: {booking.CustomerName}");
                Console.WriteLine($"Квитків: {booking.Tickets.Count}");
                Console.WriteLine($"Загальна сума до сплати: {booking.GetTotalAmount()} грн");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Помилка: {ex.Message}");
            }
        }
    }
}