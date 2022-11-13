namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Abstractions.Data;

/// <summary>
/// Represent an item for indexing.
/// </summary>
internal interface IItem
{
    /// <summary>
    /// Name of the item.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Creation date of the item.
    /// </summary>
    DateTime? Created { get; }

    /// <summary>
    /// Modification date of the item.
    /// </summary>
    DateTime? Modified { get; }

    /// <summary>
    /// Size of the item in bytes.
    /// </summary>
    long? Size { get; }

    /// <summary>
    /// Path of the item relative to its root on the server.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// ETag of the item on teh server.
    /// </summary>
    string ETag { get; }
}