namespace PortedSubtitleDownloaders.Legacy
{
    internal class SubLang
    {
        public string Code { get; }

        public string Name { get; }

        public SubLang(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}