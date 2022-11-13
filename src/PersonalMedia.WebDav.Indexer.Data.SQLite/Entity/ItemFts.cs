namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Entity;

public class ItemFts
{
    public int RowId { get; set; }
    public decimal? Rank { get; set; }
    public string Name { get; set; } = string.Empty;
}