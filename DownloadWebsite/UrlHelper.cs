using System;
using System.Linq;

namespace ConsoleApp1
{
    /// <summary>
    /// Helper class for various url operations
    /// </summary>
    public static class UrlHelper
    {
        public static string RemovePrefix(string url)
        {
            if (url.StartsWith("/.."))
            {
                while (url.Contains("/.."))
                {
                    url = url.Remove(0, 3);
                }
            }
            if (url.StartsWith(".."))
            {
                while (url.Contains(".."))
                {
                    url = url.Remove(0, 2);
                }
            }

            return url;
        }

        public static bool IsFile(string url)
        {
            return url.Split('/').Last().Contains('.');
        }

        public static string Normalize(string url)
        {
            if (!url.StartsWith("/") && !url.StartsWith("http") && !url.StartsWith("mailto"))
            {
                url = '/' + url;
            }
            if (url.Contains("?"))
            {
                url = url.Replace("?", "questionMark");
            }
            return url;
        }

        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result) || url.StartsWith("mailto") || url.StartsWith("/javascript");
        }
    }
}
