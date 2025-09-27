namespace lab2;

public class Song
{
    // Публічні властивості для доступу до даних
    public string Artist { get; set; }
    public string Title { get; set; }

    // Конструктор для створення об'єкта
    public Song(string artist, string title)
    {
        Artist = artist;
        Title = title;
    }

    // Метод для зручного виводу інформації про пісню в консоль
    public override string ToString()
    {
        return $"{Artist} - {Title}";
    }
}