using lab2;

// Створюємо декілька об'єктів пісень
Song song1 = new("The Weeknd", "Blinding Lights");
Song song2 = new("Imagine Dragons", "Believer");
Song song3 = new("Okean Elzy", "Bez boyu");

// 1. Створюємо об'єкт класу Playlist
Console.WriteLine("--- Створення плейлиста та додавання пісень ---");
Playlist myPlaylist = new();

// 2. Використовуємо оператор + для додавання пісень
myPlaylist += song1; // Більш зручний синтаксис для myPlaylist = myPlaylist + song1
myPlaylist += song2;
myPlaylist += song3;

Console.WriteLine($"У плейлисті {myPlaylist.Count} пісень:");
for (int i = 0; i < myPlaylist.Count; i++)
{
    // 3. Доступ через індексатор [int]
    Console.WriteLine($"  Трек #{i + 1}: {myPlaylist[i]}");
}
Console.WriteLine();


// 4. Демонстрація індексатора [string]
Console.WriteLine("--- Пошук пісні за назвою 'Believer' ---");
Song? foundSong = myPlaylist["Believer"]; // Змінна має тип Song? бо може бути null
Console.WriteLine(foundSong != null ? $"Знайдено: {foundSong}" : "Пісню не знайдено.");
Console.WriteLine();


// 5. Демонстрація оператора - для видалення пісні
Console.WriteLine("--- Видалення пісні 'Blinding Lights' ---");
myPlaylist -= song1;

Console.WriteLine($"Тепер у плейлисті {myPlaylist.Count} пісень:");
for (int i = 0; i < myPlaylist.Count; i++)
{
    Console.WriteLine($"  Трек #{i + 1}: {myPlaylist[i]}");
}