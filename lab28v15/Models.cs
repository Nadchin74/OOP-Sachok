using System.Collections.Generic;

namespace lab28v15
{
    // Клас предмету
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PowerBonus { get; set; }
    }

    // Клас персонажа
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string CharacterClass { get; set; }
        
        // Список предметів (інвентар)
        public List<Item> Inventory { get; set; } = new List<Item>();
    }
}