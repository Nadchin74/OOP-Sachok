using System;
using System.Collections.Generic;

namespace lab24
{
    // =========================================================
    // ПАТЕРН STRATEGY
    // =========================================================

    // 1. Інтерфейс стратегії
    public interface INumericOperationStrategy
    {
        double Execute(double value);
        string Name { get; } // Додав властивість для зручності логування
    }

    // 2. Конкретні стратегії
    public class SquareOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Square (x^2)";
        public double Execute(double value) => value * value;
    }

    public class CubeOperationStrategy : INumericOperationStrategy
    {
        public string Name => "Cube (x^3)";
        public double Execute(double value) => value * value * value;
    }

    public class SquareRootOperationStrategy : INumericOperationStrategy
    {
        public string Name => "SquareRoot (sqrt)";
        public double Execute(double value)
        {
            if (value < 0) throw new ArgumentException("Cannot calculate square root of a negative number.");
            return Math.Sqrt(value);
        }
    }

    // 3. Контекст (Processor)
    public class NumericProcessor
    {
        private INumericOperationStrategy _strategy;

        public NumericProcessor(INumericOperationStrategy initialStrategy)
        {
            _strategy = initialStrategy ?? throw new ArgumentNullException(nameof(initialStrategy));
        }

        public void SetStrategy(INumericOperationStrategy strategy)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[Processor] Стратегію змінено на: {_strategy.Name}");
            Console.ResetColor();
        }

        public double Process(double input)
        {
            return _strategy.Execute(input);
        }
        
        public string GetCurrentStrategyName() => _strategy.Name;
    }

    // =========================================================
    // ПАТЕРН OBSERVER (через події C#)
    // =========================================================

    // 4. Суб'єкт (Publisher)
    public class ResultPublisher
    {
        // Подія, на яку підписуються спостерігачі
        public event Action<double, string> ResultCalculated;

        public void PublishResult(double result, string operationName)
        {
            // Виклик події (якщо є підписники)
            ResultCalculated?.Invoke(result, operationName);
        }
    }

    // 5. Спостерігачі (Observers)

    public class ConsoleLoggerObserver
    {
        public void OnResultCalculated(double result, string operation)
        {
            Console.WriteLine($"[ConsoleLogger] Операція: {operation}, Результат: {result:F2}");
        }
    }

    public class HistoryLoggerObserver
    {
        private readonly List<string> _history = new List<string>();

        public void OnResultCalculated(double result, string operation)
        {
            string record = $"{DateTime.Now:HH:mm:ss} | {operation} -> {result:F2}";
            _history.Add(record);
            Console.WriteLine($"[HistoryLogger] Запис додано в історію. Всього записів: {_history.Count}");
        }

        public void PrintHistory()
        {
            Console.WriteLine("\n--- Історія обчислень ---");
            foreach (var item in _history)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("-------------------------");
        }
    }

    public class ThresholdNotifierObserver
    {
        private readonly double _threshold;

        public ThresholdNotifierObserver(double threshold)
        {
            _threshold = threshold;
        }

        public void OnResultCalculated(double result, string operation)
        {
            if (result > _threshold)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ThresholdNotifier] УВАГА! Результат {result:F2} перевищує поріг {_threshold}!");
                Console.ResetColor();
            }
        }
    }

    // =========================================================
    // ДЕМОНСТРАЦІЯ
    // =========================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Ініціалізація компонентів
            var strategySquare = new SquareOperationStrategy();
            var strategyCube = new CubeOperationStrategy();
            var strategySqrt = new SquareRootOperationStrategy();

            var processor = new NumericProcessor(strategySquare);
            var publisher = new ResultPublisher();

            // 2. Створення спостерігачів
            var consoleLogger = new ConsoleLoggerObserver();
            var historyLogger = new HistoryLoggerObserver();
            var alertObserver = new ThresholdNotifierObserver(100.0); // Поріг 100

            // 3. Підписка на події (Observer Pattern wiring)
            publisher.ResultCalculated += consoleLogger.OnResultCalculated;
            publisher.ResultCalculated += historyLogger.OnResultCalculated;
            publisher.ResultCalculated += alertObserver.OnResultCalculated;

            Console.WriteLine("=== Початок роботи системи ===");

            // --- Тест 1: Квадрат ---
            double input1 = 5;
            double res1 = processor.Process(input1);
            publisher.PublishResult(res1, processor.GetCurrentStrategyName());

            // --- Тест 2: Зміна стратегії на Куб (Strategy Pattern) ---
            processor.SetStrategy(strategyCube);
            double input2 = 5; // 5^3 = 125 (має спрацювати Alert, бо > 100)
            double res2 = processor.Process(input2);
            publisher.PublishResult(res2, processor.GetCurrentStrategyName());

            // --- Тест 3: Зміна стратегії на Корінь ---
            processor.SetStrategy(strategySqrt);
            double input3 = 144;
            double res3 = processor.Process(input3);
            publisher.PublishResult(res3, processor.GetCurrentStrategyName());

            // 4. Перегляд історії
            historyLogger.PrintHistory();

            // 5. Відписка (опціонально, але гарна практика)
            publisher.ResultCalculated -= consoleLogger.OnResultCalculated;

            Console.WriteLine("\nРоботу завершено.");
        }
    }
}