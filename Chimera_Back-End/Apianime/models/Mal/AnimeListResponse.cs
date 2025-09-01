namespace Apianime.models.Mal;

public class AnimeListResponse
{
    public List<AnimeData> data { get; set; } = new();
}

public class AnimeData
{
    public Anime node { get; set; }
}

public class Anime
{
    public int id { get; set; }
    public string title { get; set; }
    public MainPicture main_picture { get; set; }
    public double? mean { get; set; }
}

public class MainPicture
{
    public string medium { get; set; }
    public string large { get; set; }
}
