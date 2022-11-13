using CommunityToolkit.Diagnostics;

namespace PersonalMedia.WebDav.Indexer.Data.SQLite.Entity;

public class Item
{
    private IEnumerable<Item>? _children;

    public int Id { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? Created { get; set; }
    public DateTime? Modified { get; set; }
    public long? Size { get; set; }
    public string Path { get; set; } = string.Empty;
    public string ETag { get; set; } = string.Empty;
    public ItemType Type { get; set; }

    public IEnumerable<Item> Children
    {
        set => _children = value;
        get => _children ?? ThrowHelper.ThrowInvalidOperationException<IEnumerable<Item>>(
                   "Uninitialized property: " + nameof(Children));
    }
}
