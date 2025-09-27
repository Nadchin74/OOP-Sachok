namespace lab2;

public class Playlist
{
    // Приватний список для зберігання пісень (інкапсуляція)
    private List<Song> _songs = new List<Song>();

    // Індексатор за числовим індексом [i]
    public Song this[int index]
    {
        get => _songs[index];
        set => _songs[index] = value;
    }

    // Індексатор за назвою пісні [назва], може повертати null
    public Song? this[string title]
    {
        get => _songs.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }
    
    // Властивість для отримання кількості пісень
    public int Count => _songs.Count;

    // Перевантажений оператор + для додавання пісні
    public static Playlist operator +(Playlist playlist, Song song)
    {
        playlist._songs.Add(song);
        return playlist;
    }

    // Перевантажений оператор - для видалення пісні
    public static Playlist operator -(Playlist playlist, Song songToRemove)
    {
        playlist._songs.RemoveAll(s => s.Title == songToRemove.Title && s.Artist == songToRemove.Artist);
        return playlist;
    }
}