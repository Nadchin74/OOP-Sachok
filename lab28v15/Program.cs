using System;
using System.Threading.Tasks;
using lab28v15;

Console.OutputEncoding = System.Text.Encoding.UTF8; // Для коректного відображення української мови

string filePath = "characters_data.json";

// 1. Створення репозиторію та додавання об'єктів
var repository = new CharacterRepository();

var char1 = new Character 
{ 
    Id = 1, 
    Name = "Геральт", 
    Level = 35, 
    CharacterClass = "Відьмак" 
};
char1.Inventory.Add(new Item { Id = 101, Name = "Срібний меч", PowerBonus = 50 });
char1.Inventory.Add(new Item { Id = 102, Name = "Зілля Ластівка", PowerBonus = 0 });

var char2 = new Character 
{ 
    Id = 2, 
    Name = "Ельф-лучник", 
    Level = 20, 
    CharacterClass = "Слідопит" 
};
char2.Inventory.Add(new Item { Id = 103, Name = "Ельфійський лук", PowerBonus = 35 });

repository.Add(char1);
repository.Add(char2);

// 2. Асинхронне збереження у файл
Console.WriteLine("Зберігаємо дані у файл...");
await repository.SaveToFileAsync(filePath);
Console.WriteLine($"Дані успішно збережено у {filePath}.\n");

// 3. Створення нового пустого репозиторію для чистоти експерименту
var newRepository = new CharacterRepository();

// 4. Завантаження з файлу
Console.WriteLine("Завантажуємо дані з файлу...");
await newRepository.LoadFromFileAsync(filePath);

// 5. Виведення результату
Console.WriteLine("\nЗавантажені персонажі:");
var loadedCharacters = newRepository.GetAll();

foreach (var character in loadedCharacters)
{
    Console.WriteLine($"[ID: {character.Id}] {character.Name} ({character.CharacterClass}), Рівень: {character.Level}");
    Console.WriteLine(" Інвентар:");
    foreach (var item in character.Inventory)
    {
        Console.WriteLine($"  - {item.Name} (Бонус: +{item.PowerBonus})");
    }
    Console.WriteLine();
}

// Демонстрація пошуку за ID
Console.WriteLine("Шукаємо персонажа з ID 1:");
var foundChar = newRepository.GetById(1);
if (foundChar != null)
{
    Console.WriteLine($"Знайдено: {foundChar.Name}");
}