
namespace Apianime.models.Catalogo;

public abstract class CatalogoItem
{
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Tipo { get; set; } // "Anime" ou "Filme"
    public string? Sinopse { get; set; }
    public string? Imagem { get; set; }
    public double? Nota { get; set; }
    public string? Origem { get; set; } // "MAL" ou "TMDb"
}
