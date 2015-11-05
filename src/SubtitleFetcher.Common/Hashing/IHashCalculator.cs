namespace SubtitleFetcher.Common.Hashing
{
    public interface IHashCalculator 
    {
        byte[] ComputeHash(byte[] buffer);
    }
}