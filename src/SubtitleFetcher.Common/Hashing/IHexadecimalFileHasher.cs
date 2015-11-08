namespace SubtitleFetcher.Common.Hashing
{
    public interface IHexadecimalFileHasher
    {
        string ComputeHash(string filePath);
    }
}