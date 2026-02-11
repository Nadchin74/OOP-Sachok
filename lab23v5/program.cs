using System;

namespace lab23
{
    // =========================================================
    // ЧАСТИНА 1: ПОГАНИЙ КОД (Порушення ISP та DIP)
    // =========================================================

    // "Товстий" клас або інтерфейс, який робить все одразу (Порушення SRP/ISP)
    // Низькорівневі модулі
    public class SqlDatabase
    {
        public void SaveUser(string user) => Console.WriteLine($"[SQL] User '{user}' saved.");
    }

    public class SmtpClientService
    {
        public void SendEmail(string to, string body) => Console.WriteLine($"[SMTP] Sending email to {to}: {body}");
    }

    public class SmsGateway
    {
        public void SendSms(string phone, string msg) => Console.WriteLine($"[SMS] Sending SMS to {phone}: {msg}");
    }

    // Високорівневий модуль
    public class BadUserAccountManager
    {
        // ПОРУШЕННЯ DIP: Жорстка залежність від конкретних класів
        private SqlDatabase _db;
        private SmtpClientService _email;
        private SmsGateway _sms;

        public BadUserAccountManager()
        {
            // ПОРУШЕННЯ DIP: Створення залежностей всередині класу ("Hardcoded dependencies")
            // Ми не можемо замінити базу даних або сервіс пошти без зміни коду цього класу.
            _db = new SqlDatabase();
            _email = new SmtpClientService();
            _sms = new SmsGateway();
        }

        public void RegisterUser(string name, string email, string phone)
        {
            _db.SaveUser(name);
            _email.SendEmail(email, "Welcome!");
            // ПОРУШЕННЯ ISP (концептуально): Якщо цей метод має тільки реєструвати, 
            // навіщо йому знати про SMS, якщо, наприклад, SMS потрібне тільки для 2FA?
            // Тут клас "знає" і залежить від усього одразу.
            _sms.SendSms(phone, "Your code is 1234"); 
        }
    }

    // =========================================================
    // ЧАСТИНА 2: РЕФАКТОРИНГ (ISP + DIP + DI)
    // =========================================================

    // 1. ISP: Розділяємо великі задачі на вузькі інтерфейси
    public interface IRepository
    {
        void Save(string data);
    }

    public interface INotificationService
    {
        void Send(string contact, string message);
    }

    // Можна навіть ще вужче, якщо логіка Email і SMS сильно відрізняється
    public interface IEmailSender : INotificationService { }
    public interface ISmsSender : INotificationService { }

    // 2. Реалізація інтерфейсів (Низькорівневі модулі)
    public class MongoDbRepository : IRepository
    {
        public void Save(string data) => Console.WriteLine($"[MongoDB] Data '{data}' saved (Refactored).");
    }

    public class SecureEmailService : IEmailSender
    {
        public void Send(string contact, string message) => Console.WriteLine($"[SecureEmail] To {contact}: {message}");
    }

    public class FastSmsService : ISmsSender
    {
        public void Send(string contact, string message) => Console.WriteLine($"[FastSMS] To {contact}: {message}");
    }

    // 3. Високорівневий модуль
    public class BetterUserAccountManager
    {
        // Залежимо від абстракцій, а не деталей
        private readonly IRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        // DI: Впровадження залежностей через конструктор
        public BetterUserAccountManager(IRepository repo, IEmailSender email, ISmsSender sms)
        {
            _repository = repo;
            _emailSender = email;
            _smsSender = sms;
        }

        public void RegisterUser(string name, string email, string phone)
        {
            Console.WriteLine("--- Starting Registration ---");
            _repository.Save(name);
            _emailSender.Send(email, "Welcome to our platform!");
            _smsSender.Send(phone, "Verify code: 5555");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Demonstation of Bad Design (Tight Coupling) ===");
            var badManager = new BadUserAccountManager();
            badManager.RegisterUser("Ivan", "ivan@test.com", "+380990000000");

            Console.WriteLine("\n=== Demonstration of Refactored Design (DI + ISP) ===");
            
            // Composition Root: Тут ми збираємо наш "конструктор Lego"
            // Ми можемо легко підмінити MongoDb на SqlDatabase або EmailService на MockEmailService
            IRepository db = new MongoDbRepository();
            IEmailSender email = new SecureEmailService();
            ISmsSender sms = new FastSmsService();

            // Впроваджуємо залежності (Injection)
            var goodManager = new BetterUserAccountManager(db, email, sms);
            
            goodManager.RegisterUser("Maria", "maria@test.com", "+380661112233");

            Console.ReadLine();
        }
    }
}