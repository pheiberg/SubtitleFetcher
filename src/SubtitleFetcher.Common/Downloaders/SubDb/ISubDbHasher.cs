namespace SubtitleFetcher.Common.Downloaders.SubDb
{
    public interface ISubDbHasher
    {
        string ComputeHash(string filePath);
    }
}