using Polly;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;

public class Program
{
    private static int _databaseAttemptCount = 0;
    private static int _externalServiceAttempts = 0;

    // --- ІМІТАЦІЯ СЕРВІСІВ ---

    public static string CallDatabaseQuery(string query)
    {
        _databaseAttemptCount++;
        Console.Write($"[{DateTime.Now:HH:mm:ss}] Attempt {_databaseAttemptCount}: Executing query '{query}'...");

        // Імітуємо 2 невдачі, потім успіх
        if (_databaseAttemptCount <= 2)
        {
            Console.WriteLine(" FAILED (IOException).");
            throw new IOException($"Connection failed temporarily (Attempt {_databaseAttemptCount})");
        }

        Console.WriteLine(" SUCCESS.");
        return "Data Retrieved";
    }

    public static string CallFailingExternalService(string endpoint)
    {
        _externalServiceAttempts++;
        Console.Write($"[{DateTime.Now:HH:mm:ss}] Attempt {_externalServiceAttempts}: Calling service {endpoint}...");

        // Імітуємо збої для перших 3 спроб, щоб відкрити Circuit Breaker
        if (_externalServiceAttempts <= 3)
        {
            Console.WriteLine(" FAILED (503 Service Unavailable).");
            throw new HttpRequestException($"Service error (Attempt {_externalServiceAttempts})");
        }

        Console.WriteLine(" SUCCESS.");
        return "Service Data";
    }

    // --- MAIN ---

    public static void Main(string[] args)
    {
        Console.WriteLine("--- Polly Resilience Demo ---");

        Scenario1_Retry();
        Scenario2_CircuitBreaker();

        Console.WriteLine("\n--- Demo Completed ---");
    }

    // --- СЦЕНАРІЙ 1: Retry ---
    public static void Scenario1_Retry()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- SCENARIO 1: Retry Policy ---");
        Console.ResetColor();

        // Retry: 3 спроби з експоненційною затримкою
        var retryPolicy = Policy
            .Handle<IOException>()
            .WaitAndRetry(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(1.5, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[LOG] Retry {retryCount} after {timeSpan.TotalSeconds:F1}s. Reason: {exception.Message}");
                    Console.ResetColor();
                });

        try
        {
            string result = retryPolicy.Execute(() => CallDatabaseQuery("SELECT * FROM Users"));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"RESULT: {result}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FAILED: {ex.Message}");
            Console.ResetColor();
        }
    }

    // --- СЦЕНАРІЙ 2: Circuit Breaker ---
    public static void Scenario2_CircuitBreaker()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- SCENARIO 2: Circuit Breaker Policy ---");
        Console.ResetColor();

        // Circuit Breaker: Відкривається після 3 збоїв на 5 секунд
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreaker(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(5),
                onBreak: (ex, breakDelay) =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[CB] CIRCUIT OPENED! Blocking for {breakDelay.TotalSeconds}s.");
                    Console.ResetColor();
                },
                onReset: () =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[CB] CIRCUIT CLOSED! Service recovered.");
                    Console.ResetColor();
                });

        // 1. Провокуємо відкриття (3 збої)
        Console.WriteLine("\n[Phase 1] Provoking failures...");
        for (int i = 0; i < 4; i++)
        {
            try
            {
                circuitBreakerPolicy.Execute(() => CallFailingExternalService("https://api/data"));
            }
            catch (Exception ex) when (ex is Polly.CircuitBreaker.BrokenCircuitException)
            {
                Console.WriteLine("[CB] Request blocked by Open Circuit.");
                break; // Виходимо, бо CB вже відкритий
            }
            catch { /* Ігноруємо очікувані помилки */ }
        }

        // 2. Спроба під час блокування
        Console.WriteLine("\n[Phase 2] Attempt during blocked state...");
        try
        {
            circuitBreakerPolicy.Execute(() => CallFailingExternalService("https://api/data"));
        }
        catch (Polly.CircuitBreaker.BrokenCircuitException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[Success] Request correctly blocked.");
            Console.ResetColor();
        }

        // 3. Очікування відновлення
        Console.WriteLine("\n[Phase 3] Waiting for recovery (6s)...");
        Thread.Sleep(TimeSpan.FromSeconds(6));

        // 4. Тест відновлення (Half-Open -> Closed)
        Console.WriteLine("\n[Phase 4] Recovery check...");
        try
        {
            circuitBreakerPolicy.Execute(() => CallFailingExternalService("https://api/data"));
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("FINAL: Service recovered successfully!");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FAILED: {ex.Message}");
            Console.ResetColor();
        }
    }
}