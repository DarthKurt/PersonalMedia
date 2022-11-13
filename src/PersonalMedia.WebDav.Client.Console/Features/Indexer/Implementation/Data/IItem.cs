namespace PersonalMedia.WebDav.Client.Console.Features.Indexer.Implementation.Data;

internal interface IItem
{
    string Name { get; }
    DateTime? Created { get; }
    DateTime? Modified { get; }
    long? Size { get; }
    string Path { get; }
}