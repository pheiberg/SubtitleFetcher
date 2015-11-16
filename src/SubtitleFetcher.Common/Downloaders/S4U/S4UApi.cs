using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using Newtonsoft.Json;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class S4UApi
    {
        private readonly string _baseUrl;
        private readonly LimitsBuilder _limitsBuilder;
        private const string SiteUrl = "http://api.s4u.se/1.0";

        public S4UApi(S4USettings settings)
        {
            _baseUrl =  $"{SiteUrl}/{settings.ApiKey}/xml/serie/";
            _limitsBuilder = new LimitsBuilder();
        }

        public S4UResponse SearchByImdbId(string imdbId, S4ULimits limits = null)
        {
            if(imdbId == null) throw new ArgumentNullException(nameof(imdbId));
            
            throw new NotImplementedException();
        }

        public S4UResponse SearchByTvdbId(string tvdbId, string tvdbEpisodeId = null, S4ULimits limits = null)
        {
            if (tvdbId == null) throw new ArgumentNullException(nameof(tvdbId));

            throw new NotImplementedException();
        }

        public Response SearchByTitle(string title, S4ULimits limits = null)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));

            string limitsString = _limitsBuilder.BuildString(limits);
            var uri = new Uri(_baseUrl + "title/" + title + limitsString);
            using (var jsonStream = new WebDownloader().OpenRead(uri))
            {
                return  DeserializeFromStream(jsonStream);
            }
        }

        public S4UResponse SearchByRelease(string release, S4ULimits limits = null)
        {
            if (release == null) throw new ArgumentNullException(nameof(release));

            string limitsString = _limitsBuilder.BuildString(limits);
            var uri = new Uri(_baseUrl + "release/" + release + limitsString);
            using (var xmlStream = new WebDownloader().OpenRead(uri))
            {
                var result = DeserializeFromStream(jsonStream);
            }
            return null;
        }
        
        public static Response DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var t = serializer.Deserialize(jsonTextReader);
                return serializer.Deserialize<Response>(jsonTextReader);
            }
        }
    }

    public class S4UResponse
    {
        public S4UInfo Info { get; set; }

        public S4USerie Serie { get; set; }
    }

    public class S4UInfo
    {
        public bool Status { get; set; }

        public string Version { get; set; }

        public int DL_Left { get; set; }

        public int Hits { get; set; }

        public int Hits_Movie { get; set; }

        public int Hits_Movie_Sub { get; set; }

        public int Hits_Serie { get; set; }

        public int Hits_Serie_Sub { get; set; }
 
        public Error[] Error { get; set; }

        public Warning[] Warning { get; set; }
    }

    public class Warning
    {
    }

    public class Error
    {
    }

    public class S4USerie
    {
        /// <summary>
        /// Number of subtitles for this serie
        /// </summary>
        public int Sub_Hits { get; set; }

        /// <summary>
        /// NImdb id for this serie
        /// </summary>
        public string Imdb { get; set; }

        /// <summary>
        /// Serie title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Swedish serie title if exists
        /// </summary>
        public string Title_Sv { get; set; }

        /// <summary>
        /// Serie release year
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// TheTVDB id for this serie
        /// </summary>
        public string TVDB { get; set; }

        public int Seasons { get; set; }

        public IList<S4USub> Subs { get; set; } 
    }

    public class S4USub
    {
        public string Id { get; set; }

        public string Rls { get; set; }

        public string Rls_Group { get; set; }

        public string Translate { get; set; }

        public string File_Name { get; set; }

        public string File_Type { get; set; }

        public int File_Count { get; set; }

        public string File_Relation { get; set; }

        public string Download_File { get; set; }

        public string Download_Zip { get; set; }

        public string TVDB_Ep { get; set; }

        public string Ep_Title { get; set; }

        public string Season { get; set; }
        public string Episode { get; set; }
    }



    public class Info
    {
        public string status { get; set; }
        public string version { get; set; }
        public int dl_left { get; set; }
        public string query { get; set; }
        public int hits { get; set; }
        public int hits_movie { get; set; }
        public int hits_movie_sub { get; set; }
        public int hits_serie { get; set; }
        public int hits_serie_sub { get; set; }
        public List<object> error { get; set; }
        public List<object> warning { get; set; }
    }

    public class Sub
    {
        public string id { get; set; }
        public string rls { get; set; }
        public string rls_group { get; set; }
        public string translate { get; set; }
        public string file_name { get; set; }
        public string file_type { get; set; }
        public string file_count { get; set; }
        public string file_relation { get; set; }
        public string download_file { get; set; }
        public string download_zip { get; set; }
        public string tvdb_ep { get; set; }
        public string ep_title { get; set; }
        public string season { get; set; }
        public string episode { get; set; }
    }

    public class Serie
    {
        public int sub_hits { get; set; }
        public string imdb { get; set; }
        public string title { get; set; }
        public string title_sv { get; set; }
        public string year { get; set; }
        public string tvdb { get; set; }
        public string seasons { get; set; }
        public List<Sub> subs { get; set; }
    }

    public class Response
    {
        public Info Info { get; set; }
        public List<Serie> serie { get; set; }
    }

}
