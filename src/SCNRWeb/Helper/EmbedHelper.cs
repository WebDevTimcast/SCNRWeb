using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SCNRWeb.Helper
{
    public class EmbedHelper
    {
        public static string Process(string html)
        {
            //RemoveBraces(ref html);
            var pieces = FindAllUrls(html ?? "");

            return string.Join("", pieces);
        }

        //private static void RemoveBraces(ref string html)
        //{
        //    string pattern = @"\[[^\]]*\]";
        //    html = Regex.Replace(html, pattern, "");
        //}

        private static IEnumerable<Piece> FindAllUrls(string html)
        {
            string pattern = @"((https?|ftps?):\/\/[^""<\s]+)(?![^<>]*>|[^""]*?<\/a)";
            var matches = Regex.Matches(html, pattern);

            int last = 0;
            foreach (Match match in matches)
            {
                yield return new StringPiece() { Str = html.Substring(last, match.Index - last) };
                yield return UrlPiece.Classify(match.Value);

                last = match.Index + match.Length;
            }

            yield return new StringPiece() { Str = html.Substring(last) };
        }

        private abstract class Piece
        {
            public string Str;

            public override string ToString()
            {
                return Str;
            }
        }

        private class StringPiece : Piece
        {
        }

        private class UrlPiece : Piece
        {
            public static UrlPiece Classify(string str)
            {
                switch (str.ToLower())
                {
                    case string s when s.Contains("youtube.com"):
                        return new YoutubePiece() { Str = str };
                    case string s when s.Contains("twitter.com") || s.Contains("://x.com") || s.Contains("://www.x.com"):
                        return new TwitterPiece() { Str = str };
                    default:
                        return new UrlPiece() { Str = str };
                }
            }
        }

        private class TwitterPiece : UrlPiece
        {
            public override string ToString()
            {
                string pattern = @"https?:\/\/twitter\.com\/(?:#!\/)?(\w+)\/status(es)?\/(\d+)(?:.*)+";
                string replacePattern = @"<blockquote class=""twitter-tweet""><a class=""twitter-timeline"" href=""https://twitter.com/$1/status/$3"">Loading...</a></blockquote><script async src=""https://platform.twitter.com/widgets.js"" charset=""utf-8""></script>";

                var fixedStr = Regex.Replace(Str, pattern, replacePattern, RegexOptions.IgnoreCase);
                return fixedStr;
            }
        }

        private class YoutubePiece : UrlPiece
        {
            public override string ToString()
            {
                string pattern = @"http(?:s?)://(?:www\.)?youtu(?:be\.com/watch\?v=|\.be/)([\w\-]+)(&(amp;)?[\w\?=]*)?";

                var replacePattern = @"<iframe loading=""lazy"" title=""Jon Stewart On Vaccine Science And The Wuhan Lab Theory"" width=""500"" height=""281"" src=""https://www.youtube.com/embed/$1?feature=oembed"" frameborder=""0"" allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"" allowfullscreen></iframe>";

                var fixedStr = Regex.Replace(Str, pattern, replacePattern, RegexOptions.IgnoreCase);
                return fixedStr;
            }
        }
    }
}
