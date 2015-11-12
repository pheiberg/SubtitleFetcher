using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using SharpCompress.Reader;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public class OpenSubtitlesApi
    {
        private readonly OpenSubtitlesSettings _settings;
        private readonly IOpenSubtitlesProxy _proxy;

        public OpenSubtitlesApi(OpenSubtitlesSettings settings, IOpenSubtitlesProxy proxy)
        {
            _settings = settings;
            _proxy = proxy;
        }

        public string Login()
        {
            var response = _proxy.Login(_settings.Username, _settings.Password, _settings.Language, _settings.UserAgent);
            AssertResponse(response);
            return response.token;
        }

        public IEnumerable<Language> GetSubLanguages(string language)
        {
            var response = _proxy.GetSubLanguages(language);
            AssertResponse(response);
            
            return response.data.Select(r => ParseLanguage(r.ISO639)).Where(l => l != null);
        }

        private static Language ParseLanguage(string code)
        {
            return KnownLanguages.GetLanguageByTwoLetterIso(code);
        }

        public IEnumerable<OpenSubtitle> SearchSubtitlesFromFile(string token, IEnumerable<string> languages, string hash, long byteSize)
        {
            var request = new SearchSubtitlesRequest
            {
                sublanguageid = string.Join(",", languages),
                moviehash = hash,
                moviebytesize = byteSize.ToString()
            };
            
            return SearchSubtitlesInternal(token, request);
        }

        public IEnumerable<OpenSubtitle> SearchSubtitlesFromImdb(string token, IEnumerable<string> languages, string imdbId)
        {
            if (string.IsNullOrEmpty(imdbId))
                throw new ArgumentNullException(nameof(imdbId));

            var request = new SearchSubtitlesRequest
            {
                sublanguageid = string.Join(",", languages),
                imdbid = imdbId
            };
            return SearchSubtitlesInternal(token, request);
        }

        public IEnumerable<OpenSubtitle> SearchSubtitlesFromQuery(string token, IEnumerable<string> languages, string query, int? season = null, int? episode = null)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));

            var request = new SearchSubtitlesRequest
            {
                sublanguageid = string.Join(",", languages),
                query = query,
                season = season,
                episode = episode
            };
            return SearchSubtitlesInternal(token, request);
        }

        private IEnumerable<OpenSubtitle> SearchSubtitlesInternal(string token, SearchSubtitlesRequest request)
        {
            var response = _proxy.SearchSubtitles(token, new[] { request });
            AssertResponse(response);
            
            var subtitleData = response.data;
            if (subtitleData == null)
                return Enumerable.Empty<OpenSubtitle>();

            return subtitleData.Select(BuildSubtitle);
        }

        private static OpenSubtitle BuildSubtitle(SearchSubtitlesInfo data)
        {
            OpenSubtitlesKind kind;
            if(!Enum.TryParse(data.MovieKind, true, out kind))
            {
                kind = OpenSubtitlesKind.None;
            }

            int seriesEpisode;
            if (!int.TryParse(data.SeriesEpisode, out seriesEpisode))
            {
                seriesEpisode = 0;
            }

            int seriesSeason;
            if (!int.TryParse(data.SeriesSeason, out seriesSeason))
            {
                seriesSeason = 0;
            }

            var sub = new OpenSubtitle
            {
                IDMovie = data.IDMovie,
                IDMovieImdb = data.IDMovieImdb,
                IDSubMovieFile = data.IDSubMovieFile,
                IDSubtitleFile = data.IDSubtitleFile,
                IDSubtitle = data.IDSubtitle,
                ISO639 = data.ISO639,
                LanguageName = data.LanguageName,
                MatchedBy = data.MatchedBy,
                MovieByteSize = data.MovieByteSize,
                MovieFPS = data.MovieFPS,
                MovieName = data.MovieName,
                MovieReleaseName = data.MovieReleaseName,
                MovieHash = data.MovieHash,
                MovieImdbRating = data.MovieImdbRating,
                MovieKind = kind,
                MovieNameEng = data.MovieNameEng,
                MovieTimeMS = data.MovieTimeMS,
                MovieYear = data.MovieYear,
                QueryNumber = data.QueryNumber,
                SeriesEpisode = seriesEpisode,
                SeriesIMDBParent = data.SeriesIMDBParent,
                SeriesSeason = seriesSeason,
                SubFileName = data.SubFileName,
                SubActualCD = data.SubActualCD,
                SubAddDate = data.SubAddDate,
                SubAuthorComment = data.SubAuthorComment,
                SubBad = data.SubBad,
                SubComments = data.SubComments,
                SubDownloadLink = data.SubDownloadLink,
                SubDownloadsCnt = data.SubDownloadsCnt,
                SubEncoding = data.SubEncoding,
                SubFeatured = data.SubFeatured,
                SubFormat = data.SubFormat,
                SubHD = data.SubHD == "1",
                SubHash = data.SubHash,
                SubHearingImpaired = data.SubHearingImpaired == "1",
                SubLanguageID = data.SubLanguageID,
                SubLastTS = data.SubLastTS,
                SubRating = data.SubRating,
                SubSize = data.SubSize,
                SubSumCD = data.SubSumCD,
                SubtitlesLink = data.SubtitlesLink,
                UserID = data.UserID,
                UserNickName = data.UserNickName,
                UserRank = data.UserRank,
                ZipDownloadLink = data.ZipDownloadLink
            };
            
            return sub;
        }

        private void AssertResponse(ResponseBase response)
        {
            if (null == response)
                throw new ArgumentNullException(nameof(response));

            if (string.IsNullOrEmpty(response.status))
                return;

            var stringCode = response.status.Substring(0, 3);
            int responseCode = int.Parse(stringCode);
            if (responseCode >= 400)
                throw new OpenSubtitlesHttpException($"HTTP {responseCode} - {response.status}");
        }

        public FileInfo DownloadSubtitle(string downloadLink, string fileName)
        {
            if (null == downloadLink)
                throw new ArgumentNullException(nameof(downloadLink));

            string subtitleFile = Path.Combine(Path.GetTempPath(), fileName);
            string tempZipName = Path.GetTempFileName();

            try
            {
                var webClient = new WebDownloader();
                var data = webClient.DownloadData(downloadLink);
                using(var fileStream = new MemoryStream(data))
                {
                    UnzipSubtitleToFile(fileStream, subtitleFile);
                }
            }
            finally
            {
                File.Delete(tempZipName);
            }

            return new FileInfo(subtitleFile);
        }

        private static void UnzipSubtitleToFile(Stream zipFile, string subFileName)
        {
            using (var reader = ReaderFactory.Open(zipFile))
            {
                reader.MoveToNextEntry();
                reader.WriteEntryTo(subFileName);
            }
        }
    }
}