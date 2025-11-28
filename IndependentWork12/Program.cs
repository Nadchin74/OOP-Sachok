using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    class Program
    {
        // Спільний ресурс для демонстрації проблем безпеки
        static int _sharedCounter = 0;
        static object _lockObject = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("=== Самостійна робота №12: PLINQ (Productivity & Safety) ===\n");

            // ЕТАП 1: Дослідження Продуктивності
            // ---------------------------------------------------------
            int dataSize = 5_000_000; // 5 мільйонів елементів
            Console.WriteLine($"[1] Генерація даних ({dataSize:N0} елементів)...");
            var sourceData = GenerateData(dataSize);
            Console.WriteLine("Дані згенеровано.\n");

            Console.WriteLine("--- Порівняння продуктивності (Heavy Calculation) ---");
            
            // 1. Звичайний LINQ (Послідовний)
            Console.Write("Запуск звичайного LINQ... ");
            Stopwatch sw = Stopwatch.StartNew();
            var linqResult = sourceData
                .Select(x => HeavyComputation(x))
                .Where(r => r > 100) // Фільтрація для прикладу
                .ToList(); // Матеріалізація для виконання запиту
            sw.Stop();
            Console.WriteLine($"Готово. Час: {sw.ElapsedMilliseconds} мс");

            // 2. PLINQ (Паралельний)
            Console.Write("Запуск PLINQ (.AsParallel)... ");
            sw.Restart();
            var plinqResult = sourceData
                .AsParallel() // Вмикаємо паралелізм
                .Select(x => HeavyComputation(x))
                .Where(r => r > 100)
                .ToList();
            sw.Stop();
            Console.WriteLine($"Готово. Час: {sw.ElapsedMilliseconds} мс");

            Console.WriteLine($"\nРезультати ідентичні? {linqResult.Count == plinqResult.Count}");
            Console.WriteLine("-----------------------------------------------------\n");


            // ЕТАП 2: Дослідження Безпеки (Concurrency Issues)
            // ---------------------------------------------------------
            Console.WriteLine("--- Дослідження Безпеки (Race Condition) ---");
            
            // Сценарій: Підрахунок кількості парних чисел шляхом інкременту зовнішньої змінної.
            // Це АНТИПАТЕРН у PLINQ, але гарна демонстрація проблеми.
            
            int smallDataSize = 100_000;
            var range = Enumerable.Range(0, smallDataSize).ToList();

            // А. Небезпечний підхід
            _sharedCounter = 0;
            range.AsParallel().ForAll(n => 
            {
                // Небезпечно! Кілька потоків читають і пишуть одночасно.
                if (n % 2 == 0)
                {
                    _sharedCounter++; 
                }
            });
            Console.WriteLine($"[Небезпечно] Очікувано: {smallDataSize / 2}, Отримано: {_sharedCounter}");
            Console.WriteLine("Висновок: Дані втрачено через стан гонитви (Race Condition).\n");

            // Б. Безпечний підхід (Lock)
            _sharedCounter = 0;
            range.AsParallel().ForAll(n =>
            {
                if (n % 2 == 0)
                {
                    // Блокуємо доступ до змінної для інших потоків на момент запису
                    lock (_lockObject)
                    {
                        _sharedCounter++;
                    }
                }
            });
            Console.WriteLine($"[Безпечно з lock] Очікувано: {smallDataSize / 2}, Отримано: {_sharedCounter}");
            
            // В. Безпечний та швидкий підхід (Interlocked)
            _sharedCounter = 0;
            range.AsParallel().ForAll(n =>
            {
                if (n % 2 == 0)
                {
                    // Атомарна операція (швидше за lock)
                    Interlocked.Increment(ref _sharedCounter);
                }
            });
            Console.WriteLine($"[Безпечно з Interlocked] Очікувано: {smallDataSize / 2}, Отримано: {_sharedCounter}");

            Console.WriteLine("\n=== Завершено ===");
        }

        // Допоміжний метод для генерації випадкових даних
        static List<int> GenerateData(int count)
        {
            var rand = new Random();
            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(rand.Next(1, 100));
            }
            return list;
        }

        // "Важка" обчислювальна операція
        // Ми робимо багато математичних дій, щоб завантажити процесор
        static double HeavyComputation(int number)
        {
            double result = number;
            // Цикл для збільшення навантаження на CPU
            for (int i = 0; i < 50; i++)
            {
                result = Math.Sqrt(Math.Pow(result, 2) + Math.Sin(i) * Math.Cos(number));
            }
            return result;
        }
    }
}