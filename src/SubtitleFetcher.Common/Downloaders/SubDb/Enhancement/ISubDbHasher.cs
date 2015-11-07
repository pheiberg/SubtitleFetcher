namespace SubtitleFetcher.Common.Downloaders.SubDb.Enhancement
{
    public interface ISubDbHasher
    {
        string ComputeHash(string filePath);
    }
}