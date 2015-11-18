using System;
using System.Linq;
using NUnit.Framework;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Settings;

namespace UnitTests.SubtitleFetcher.Settings
{
    [TestFixture]
    public class OptionsParserTests
    {
        private readonly OptionsParser _sut = CreateSut();

        private static OptionsParser CreateSut()
        {
            var settings = new OptionsParserSettings {HelpWriter = Console.Out};
            return new OptionsParser(settings);
        }

        [Test]
        public void ParseOptions_ListLanguages_SuccessfullWithOptionSet()
        {
            var options = _sut.ParseOptions(new[] {"--list-languages"});

            Assert.That(options.ListLanguages, Is.True);
        }

        [Test]
        public void ParseOptions_ListDownloaders_SuccessfullWithOptionSet()
        {
            var options = _sut.ParseOptions(new[] { "--list-downloaders" });

            Assert.That(options.ListDownloaders, Is.True);
        }

        [Test]
        public void ParseOptions_NoArguments_SuccessfullWithNoErrors()
        {
            var options = _sut.ParseOptions(new string[0]);

            Assert.That(options.ParseErrors, Is.Empty);
        }

        [Test]
        public void ParseOptions_NoArguments_SuccessfullWithDefaultLanguageSet()
        {
            var expectedLanguage = KnownLanguages.GetLanguageByName("English");

            var options = _sut.ParseOptions(new string[0]);

            Assert.That(options.Languages.Single(), Is.EqualTo(expectedLanguage.TwoLetterIsoName));
        }
    }
}
