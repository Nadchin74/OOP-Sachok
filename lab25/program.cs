using System;
using System.Collections.Generic;

namespace lab25
{
    // --- 1. Патерн FACTORY METHOD & LOGGER ---
    public interface ILogger { void Log(string message); }

    public class ConsoleLogger : ILogger 
    { 
        public void Log(string message) => Console.WriteLine($"[Console Log]: {message}"); 
    }

    public class FileLogger : ILogger 
    { 
        public void Log(string message) => Console.WriteLine($"[File Log (Simulated)]: Writing to file... {message}"); 
    }

    public abstract class LoggerFactory { public abstract ILogger CreateLogger(); }
    public class ConsoleLoggerFactory : LoggerFactory { public override ILogger CreateLogger() => new ConsoleLogger(); }
    public class FileLoggerFactory : LoggerFactory { public override ILogger CreateLogger() => new FileLogger(); }

    // --- 2. Патерн SINGLETON ---
    public class LoggerManager
    {
        private static LoggerManager _instance;
        private LoggerFactory _factory;
        private ILogger _currentLogger;

        private LoggerManager() { }

        public static LoggerManager Instance => _instance ??= new LoggerManager();

        public void SetFactory(LoggerFactory factory)
        {
            _factory = factory;
            _currentLogger = _factory.CreateLogger();
        }

        public void Log(string message) => _currentLogger?.Log(message);
    }

    // --- 3. Патерн STRATEGY ---
    public interface IDataProcessorStrategy { string Process(string data); }

    public class EncryptDataStrategy : IDataProcessorStrategy 
    { 
        public string Process(string data) => $"Encrypted({data})"; 
    }

    public class CompressDataStrategy : IDataProcessorStrategy 
    { 
        public string Process(string data) => $"Compressed({data})"; 
    }

    public class DataContext
    {
        private IDataProcessorStrategy _strategy;
        public DataContext(IDataProcessorStrategy strategy) => _strategy = strategy;
        public void SetStrategy(IDataProcessorStrategy strategy) => _strategy = strategy;
        public string ExecuteStrategy(string data) => _strategy.Process(data);
    }

    // --- 4. Патерн OBSERVER ---
    public class DataEventArgs : EventArgs { public string Data { get; set; } }

    public class DataPublisher
    {
        public event EventHandler<DataEventArgs> DataProcessed;

        public void PublishDataProcessed(string data)
        {
            Console.WriteLine("Publisher: Notifying observers...");
            DataProcessed?.Invoke(this, new DataEventArgs { Data = data });
        }
    }

    public class ProcessingLoggerObserver
    {
        public void OnDataProcessed(object sender, DataEventArgs e)
        {
            LoggerManager.Instance.Log($"Observer caught data: {e.Data}");
        }
    }

    // --- ГОЛОВНА ПРОГРАМА (Сценарії) ---
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Сценарій 1: Повна інтеграція
            PrintHeader("Сценарій 1: Повна інтеграція");
            
            LoggerManager.Instance.SetFactory(new ConsoleLoggerFactory());
            var context = new DataContext(new EncryptDataStrategy());
            var publisher = new DataPublisher();
            var observer = new ProcessingLoggerObserver();

            publisher.DataProcessed += observer.OnDataProcessed;

            string rawData = "SecretMessage123";
            string processedData = context.ExecuteStrategy(rawData);
            publisher.PublishDataProcessed(processedData);

            // Сценарій 2: Динамічна зміна логера
            PrintHeader("Сценарій 2: Динамічна зміна логера");
            
            LoggerManager.Instance.SetFactory(new FileLoggerFactory());
            processedData = context.ExecuteStrategy("NewData");
            publisher.PublishDataProcessed(processedData);

            // Сценарій 3: Динамічна зміна стратегії
            PrintHeader("Сценарій 3: Динамічна зміна стратегії");
            
            context.SetStrategy(new CompressDataStrategy());
            processedData = context.ExecuteStrategy("BigDataChunk");
            publisher.PublishDataProcessed(processedData);

            Console.WriteLine("\nТестування завершено.");
        }

        static void PrintHeader(string title)
        {
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(title);
            Console.WriteLine(new string('-', 50));
        }
    }
}