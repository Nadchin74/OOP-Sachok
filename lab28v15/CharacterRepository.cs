using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace lab28v15
{
    public class CharacterRepository
    {
        private List<Character> _characters = new List<Character>();

        // Додавання персонажа
        public void Add(Character character)
        {
            _characters.Add(character);
        }

        // Отримання всіх персонажів
        public IEnumerable<Character> GetAll()
        {
            return _characters;
        }

        // Отримання персонажа за ID
        public Character GetById(int id)
        {
            return _characters.FirstOrDefault(c => c.Id == id);
        }

        // Асинхронне збереження у JSON файл
        public async Task SaveToFileAsync(string filename)
        {
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true // Для красивого форматування (з відступами)
            };
            
            using FileStream createStream = File.Create(filename);
            await JsonSerializer.SerializeAsync(createStream, _characters, options);
        }

        // Асинхронне завантаження з JSON файлу
        public async Task LoadFromFileAsync(string filename)
        {
            if (!File.Exists(filename))
                return; // Якщо файлу немає, нічого не робимо

            using FileStream openStream = File.OpenRead(filename);
            _characters = await JsonSerializer.DeserializeAsync<List<Character>>(openStream) ?? new List<Character>();
        }
    }
}