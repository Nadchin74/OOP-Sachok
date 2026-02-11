using System;
using System.Text;

namespace lab22
{
    // ==========================================
    // 1. ПОРУШЕННЯ LSP (Liskov Substitution Principle)
    // ==========================================

    public class Printer
    {
        public virtual void PrintColorImage()
        {
            Console.WriteLine("Друк кольорового зображення... Успішно!");
        }
    }

    public class BlackAndWhitePrinter : Printer
    {
        public override void PrintColorImage()
        {
            // ПОРУШЕННЯ: Нащадок не може виконати дію, яку гарантує базовий клас.
            // Клієнтський код не очікує тут винятку.
            throw new NotSupportedException("Помилка: Чорно-білий принтер не підтримує кольоровий друк!");
        }
    }

    // ==========================================
    // 2. РЕФАКТОРИНГ: ДОТРИМАННЯ LSP (Зміна ієрархії)
    // ==========================================

    public interface IPrinter
    {
        void PrintText(string text);
    }

    public interface IColorPrinter : IPrinter
    {
        void PrintColorImage();
    }

    public class BetterBWPrinter : IPrinter
    {
        public void PrintText(string text) => Console.WriteLine($"Ч/Б друк тексту: {text}");
    }

    public class BetterColorPrinter : IColorPrinter
    {
        public void PrintText(string text) => Console.WriteLine($"Кольоровий друк тексту: {text}");
        public void PrintColorImage() => Console.WriteLine("Друк яскравого кольорового зображення... Успішно!");
    }

    class Program
    {
        // Клієнтський метод для старої структури (з помилкою)
        static void LegacyClientMethod(Printer printer)
        {
            printer.PrintColorImage();
        }

        // Клієнтський метод для нової структури (безпечний)
        static void SafeClientMethod(IColorPrinter printer)
        {
            printer.PrintColorImage();
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("--- Демонстрація порушення LSP ---");
            Printer badPrinter = new BlackAndWhitePrinter();
            try
            {
                LegacyClientMethod(badPrinter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\n--- Демонстрація коректного рішення (LSP Compliant) ---");
            IColorPrinter goodColorPrinter = new BetterColorPrinter();
            IPrinter goodBWPrinter = new BetterBWPrinter();

            SafeClientMethod(goodColorPrinter); // Працює коректно
            goodBWPrinter.PrintText("Звіт по лабораторній"); // BW принтер робить тільки те, що вміє

            Console.WriteLine("\nПрограма завершена успішно.");
        }
    }
}