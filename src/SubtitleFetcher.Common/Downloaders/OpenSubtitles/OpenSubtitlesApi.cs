using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CookComputing.XmlRpc;
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
                sublanguageid = languages.ToArray(),
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
                sublanguageid = languages.ToArray(),
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
                sublanguageid = languages.ToArray(),
                query = query,
                season = season,
                episode = episode
            };
            return SearchSubtitlesInternal(token, request);
        }

        private IEnumerable<OpenSubtitle> SearchSubtitlesInternal(string token, SearchSubtitlesRequest request)
        {
            var tracer = new Tracer();
            tracer.SubscribeTo(_proxy);

            var response = _proxy.SearchSubtitles(token, new[] { request });
            AssertResponse(response);
            
            var subtitleData = response.data as object[];
            if (subtitleData == null)
                yield break;

            foreach (XmlRpcStruct data in subtitleData)
                yield return BuildSubtitle(data);
        }

        private static OpenSubtitle BuildSubtitle(XmlRpcStruct data)
        {
            var sub = new OpenSubtitle();
            var properties = sub.GetType().GetProperties();
            var matchingProperties = properties.Where(property => data.ContainsKey(property.Name));
            foreach (var property in matchingProperties)
            {
                var dataValue = GetDataValue(data, property);
                property.SetMethod.Invoke(sub, new[] { dataValue });
            }
            return sub;
        }

        private static object GetDataValue(XmlRpcStruct data, PropertyInfo property)
        {
            object dataValue = data[property.Name];
            return property.PropertyType.IsInstanceOfType(dataValue) ? dataValue : ConvertValueToType(dataValue, property.PropertyType);
        }

        private static object ConvertValueToType(object dataValue, Type targetType)
        {
            Type dataFieldType = dataValue.GetType();
            var typeConverter = TypeDescriptor.GetConverter(targetType);
            return typeConverter.CanConvertTo(dataFieldType) ? typeConverter.ConvertTo(dataValue, dataFieldType) : null;
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

        public FileInfo DownloadSubtitle(OpenSubtitle subtitle)
        {
            if (null == subtitle)
                throw new ArgumentNullException(nameof(subtitle));

            string subtitleFile = Path.Combine(Path.GetTempPath(), subtitle.SubFileName);
            //string tempZipName = Path.GetTempFileName();

            //try
            //{
            //    var webClient = new WebClient();
            //    webClient.DownloadFile(subtitle.DownloadLink, tempZipName);
            //    UnzipSubtitleFileToFile(tempZipName, subtitleFile);

            //}
            //finally
            //{
            //    File.Delete(tempZipName);
            //}

            return new FileInfo(subtitleFile);
        }

        private void UnzipSubtitleFileToFile(string tempZipName, string destinationfile)
        {

        }
    }
}

public class Tracer : XmlRpcLogger
{
    protected override void OnRequest(object sender,
      XmlRpcRequestEventArgs e)
    {
        DumpStream(e.RequestStream);
    }

    protected override void OnResponse(object sender,
      XmlRpcResponseEventArgs e)
    {
        DumpStream(e.ResponseStream);
    }

    private void DumpStream(Stream stm)
    {
        stm.Position = 0;
        TextReader trdr = new StreamReader(stm);
        string s = trdr.ReadLine();
        while (s != null)
        {
            Trace.WriteLine(s);
            s = trdr.ReadLine();
        }
    }
}