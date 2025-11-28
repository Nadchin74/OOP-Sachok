using System;
using System.IO;
using System.Net.Http;
using System.Threading;

// ======================================================================
// 1. Імітація Класів, що Генерують Винятки
// ======================================================================

/// <summary>
/// Клас, що імітує роботу з файлами та викидає винятки вводу/виводу.
/// </summary>
public class FileProcessor
{
    private int _attemptCount = 0;

    /// <summary>
    /// Метод LoadConfiguration імітує IOException перші 2 рази, потім успіх.
    /// </summary>
    public string LoadConfiguration(string path)
    {
        _attemptCount++;
        Console.WriteLine($"[FileProcessor] Спроба {_attemptCount}: Завантаження конфігурації з {path}...");

        if (_attemptCount <= 2)
        {
            // Імітація тимчасової помилки (наприклад, файл зайнятий іншим процесом)
            throw new IOException($"IOException: Файл зайнятий або недоступний. (Спроба {_attemptCount})");
        }

        return $"Конфігурація '{path}' успішно завантажена.";
    }
}

/// <summary>
/// Клас, що імітує мережеві запити та викидає мережеві винятки.
/// </summary>
public class NetworkClient
{
    private int _attemptCount = 0;

    /// <summary>
    /// Метод DownloadConfiguration імітує HttpRequestException перші 3 рази, потім успіх.
    /// </summary>
    public string DownloadConfiguration(string url)
    {
        _attemptCount++;
        Console.WriteLine($"[NetworkClient] Спроба {_attemptCount}: Завантаження конфігурації з {url}...");

        if (_attemptCount <= 3)
        {
            // Імітація тимчасової мережевої помилки (наприклад, перевищено час очікування)
            throw new HttpRequestException($"HttpRequestException: Перевищено час очікування мережі. (Спроба {_attemptCount})");
        }

        return $"Конфігурація '{url}' успішно завантажена через мережу.";
    }
}

// ======================================================================
// 2. Узагальнений Допоміжний Клас RetryHelper
// ======================================================================

/// <summary>
/// Допоміжний клас для виконання операцій з патерном Retry та експоненційною затримкою.
/// </summary>
public static class RetryHelper
{
    /// <summary>
    /// Виконує операцію з повторними спробами та експоненційною затримкою.
    /// </summary>
    /// <typeparam name="T">Тип результату, що повертається операцією.</typeparam>
    /// <param name="operation">Делегат Func<T>, що представляє операцію.</param>
    /// <param name="retryCount">Максимальна кількість повторних спроб (за замовчуванням 3).</param>
    /// <param name="initialDelay">Початкова затримка між спробами (за замовчуванням 1 секунда).</param>
    /// <param name="shouldRetry">Опціональний делегат, що визначає, чи слід повторювати для винятку.</param>
    public static T ExecuteWithRetry<T>(
        Func<T> operation, 
        int retryCount = 3, 
        TimeSpan? initialDelay = null, // Використовуємо Nullable TimeSpan
        Func<Exception, bool> shouldRetry = null)
    {
        // Встановлення початкової затримки (за замовчуванням 1 секунда, якщо не вказано)
        TimeSpan delay = initialDelay ?? TimeSpan.FromSeconds(1); 
        
        // Визначаємо стандартну функцію shouldRetry, якщо користувач її не надав
        Func<Exception, bool> retryPolicy = shouldRetry ?? (_ => true);

        // Основний цикл повторних спроб
        for (int attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                // 1. Виконання операції
                return operation();
            }
            catch (Exception ex)
            {
                // 2. Логування помилки
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  [ЛОГ] Невдача на спробі {attempt}/{retryCount}: {ex.GetType().Name}. Причина: {ex.Message}");
                Console.ResetColor();

                // 3. Перевірка політики повтору
                if (attempt >= retryCount || !retryPolicy(ex))
                {
                    // Якщо досягнуто ліміту спроб АБО політика забороняє повтор
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  [ЛОГ] Досягнуто ліміту спроб або виняток не підлягає повтору. Викидаємо оригінальний виняток.");
                    Console.ResetColor();
                    throw; // Викидаємо оригінальний виняток, що призвів до невдачі
                }

                // 4. Реалізація Експоненційної Затримки
                // Формула: initialDelay * 2^(attempt - 1)
                // Наприклад: 1с, 2с, 4с, 8с...
                TimeSpan currentDelay = TimeSpan.FromMilliseconds(
                    delay.TotalMilliseconds * Math.Pow(2, attempt - 1));

                Console.WriteLine($"  [ЛОГ] Очікування перед наступною спробою ({attempt + 1}): {currentDelay.TotalSeconds:F1} сек...");
                Thread.Sleep(currentDelay);
            }
        }
        
        // Повинно бути недосяжним, але для безпеки (якщо цикл не поверне результат)
        throw new InvalidOperationException("Операція не завершилася успішно після всіх спроб."); 
    }
}

// ======================================================================
// 3. Демонстрація в Main()
// ======================================================================

public class Program
{
    public static void Main(string[] args)
    {
        // ----------------------------------------------------------------------
        // Сценарій 1: Завантаження файлу конфігурації (FileProcessor)
        // Імітує 2 IOException, потім успіх.
        // ----------------------------------------------------------------------
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("СЦЕНАРІЙ 1: Retry для FileProcessor (Імітація IOException)");
        Console.WriteLine("=======================================================");

        var fileProcessor = new FileProcessor();
        string fileResult = "";

        try
        {
            // Початкова затримка 500 мс, 4 спроби (потрібно 3 спроби для успіху)
            fileResult = RetryHelper.ExecuteWithRetry(
                () => fileProcessor.LoadConfiguration("settings.json"),
                retryCount: 4,
                initialDelay: TimeSpan.FromMilliseconds(500) 
            );
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[УСПІХ] Результат FileProcessor: {fileResult}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ФІНАЛЬНА ПОМИЛКА] FileProcessor: {ex.GetType().Name}");
            Console.ResetColor();
        }

        // ----------------------------------------------------------------------
        // Сценарій 2: Завантаження з мережі (NetworkClient)
        // Імітує 3 HttpRequestException, потім успіх. Демонстрація shouldRetry.
        // ----------------------------------------------------------------------
        Console.WriteLine("\n\n=======================================================");
        Console.WriteLine("СЦЕНАРІЙ 2: Retry для NetworkClient (Імітація HttpRequestException)");
        Console.WriteLine("=======================================================");

        var networkClient = new NetworkClient();
        string networkResult = "";
        
        // Політика: Повторювати тільки для HttpRequestException.
        // Це демонструє вибірковий повтор.
        Func<Exception, bool> networkRetryPolicy = (ex) => 
        {
            // Коментар: Вибірково повторюємо лише для мережевих помилок
            return ex is HttpRequestException;
        };

        try
        {
            // Початкова затримка 1 секунда, 5 спроб (потрібно 4 спроби для успіху)
            networkResult = RetryHelper.ExecuteWithRetry(
                () => networkClient.DownloadConfiguration("https://api.config.com"),
                retryCount: 5,
                initialDelay: TimeSpan.FromSeconds(1),
                shouldRetry: networkRetryPolicy // Застосовуємо вибіркову політику
            );
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[УСПІХ] Результат NetworkClient: {networkResult}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ФІНАЛЬНА ПОМИЛКА] NetworkClient: {ex.GetType().Name}");
            Console.ResetColor();
        }
        
        // ----------------------------------------------------------------------
        // Сценарій 3: Демонстрація Непідходящого Винятку
        // Спроба повтору для винятку, який не підлягає повтору (FileNotFoundException)
        // ----------------------------------------------------------------------
        Console.WriteLine("\n\n=======================================================");
        Console.WriteLine("СЦЕНАРІЙ 3: Демонстрація непідходящого винятку (FileNotFound)");
        Console.WriteLine("=======================================================");
        
        // Операція, яка одразу генерує виняток FileNotFoundException
        Func<string> alwaysFails = () => 
        {
            throw new FileNotFoundException("Файл не знайдено назавжди.");
        };

        // Політика: Повторювати тільки для IOException, але не для FileNotFoundException
        Func<Exception, bool> strictFilePolicy = (ex) => 
        {
            return ex is IOException && ex is not FileNotFoundException;
        };

        try
        {
            RetryHelper.ExecuteWithRetry(
                alwaysFails,
                retryCount: 3,
                initialDelay: TimeSpan.FromSeconds(1),
                shouldRetry: strictFilePolicy // strictFilePolicy поверне false на першій спробі
            );
        }
        catch (FileNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n[УСПІШНИЙ ПРОВАЛ] Виняток {ex.GetType().Name} був одразу викинутий завдяки shouldRetry.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
             Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ФІНАЛЬНА ПОМИЛКА] Загальна помилка: {ex.GetType().Name}");
            Console.ResetColor();
        }
    }
}