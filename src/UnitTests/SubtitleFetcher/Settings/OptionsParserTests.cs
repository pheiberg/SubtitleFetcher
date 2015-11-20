using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;
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

        [Test]
        public void ParseOptions_NoArguments_SuccessfullWithNoDownloadersSet()
        {
            var options = _sut.ParseOptions(new string[0]);

            Assert.That(options.DownloaderNames, Is.Empty);
        }

        [Test]
        public void ParseOptions_NoArguments_SuccessfullWithMinimalLoggingSet()
        {
            var options = _sut.ParseOptions(new string[0]);

            Assert.That(options.Logging, Is.EqualTo(LogLevel.Minimal));
        }

        [Test, AutoFakeData]
        public void ParseOptions_Downloaders_SuccessfullWithDownloadersSet(
            IEnumerable<string> downloaders)
        {
            var options = _sut.ParseOptions(new[] { "-d", string.Join(",", downloaders)});

            Assert.That(options.DownloaderNames, Is.EquivalentTo(downloaders));
        }

        [Test]
        public void ParseOptions_InvalidOption_FailedWithErrorsSet()
        {
            var options = _sut.ParseOptions(new[] { "--unknownOption" });

            Assert.That(options.ParseErrors, Is.Not.Empty);
        }

        [Test]
        [TestCase("Minimal", LogLevel.Minimal)]
        [TestCase("Debug", LogLevel.Debug)]
        [TestCase("Verbose", LogLevel.Verbose)]
        public void ParseOptions_LoggingOptionSet_CorrectOptionSet(string loggingMode, LogLevel expected)
        {
            var options = _sut.ParseOptions(new[] { "--logging", loggingMode });

            Assert.That(options.Logging, Is.EqualTo(expected));
        }

        [Test]
        public void ParseOptions_InvalidLoggingOptionSet_FailedWithErrors()
        {
            var options = _sut.ParseOptions(new[] { "--logging", "invalidLoggingMode" });

            Assert.That(options.ParseErrors, Is.Not.Empty);
        }

        [Test]
        public void ParseOptions_InvalidLangaugeOptionSet_FailedWithErrors()
        {
            var invalidLanguageCode = "xx";

            var options = _sut.ParseOptions(new[] { "-l", invalidLanguageCode });

            var languageErrors = options.CustomParseErrors.Where(e => e.Message.StartsWith("Invalid language"));
            Assert.That(languageErrors, Is.Not.Empty);
        }

        [Test]
        public void ParseOptions_ValidLangugeOptionSet_SuccessfulWithLanguagesSet()
        {
            var expectedLanguages = KnownLanguages.AllLanguages.Take(3);
            var expectedLanguageCodes = expectedLanguages.Select(l => l.TwoLetterIsoName);
            var languageString = string.Join(",", expectedLanguageCodes);

            var options = _sut.ParseOptions(new[] { "-l", languageString });

            Assert.That(options.Languages, Is.EquivalentTo(expectedLanguageCodes));
        }

        [Test]
        public void ParseOptions_InvalidDownloaderOptionSet_FailedWithErrors()
        {
            var invalidDownloader = "xx";

            var options = _sut.ParseOptions(new[] { "-d", invalidDownloader });

            var downloaderErrors = options.CustomParseErrors.Where(e => e.Message.StartsWith("Invalid downloader"));
            Assert.That(downloaderErrors.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ParseOptions_ValidDownloaderOptionSet_SuccessfulWithDownloadersSet()
        {
            var expectedDownloaders = new []{ "SubDb", "OpenSubtitles" };
            var downloaderString = string.Join(",", expectedDownloaders);

            var options = _sut.ParseOptions(new[] { "-d", downloaderString });

            Assert.That(options.DownloaderNames, Is.EquivalentTo(expectedDownloaders));
        }
    }
}
