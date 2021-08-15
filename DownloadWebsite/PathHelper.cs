using System;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    /// <summary>
    /// Helper class for file or directory operations
    /// </summary>
    class PathHelper
    {
        /// <summary>
        /// Check if the file already exists
        /// </summary>
        public static bool CheckIfNewFileOrEmpty(string url, string localPath)
        {
            string existFile;
            if (url.StartsWith("/") && !UrlHelper.IsFile(url))
            {
                existFile = $"{localPath}{ url.Replace("/", @"\")}.html";
            }
            else if (url.StartsWith("/#"))
            {
                existFile = $"{localPath}{ url.Replace("/", @"\")}.html";
            }
            else if (url.Contains("?"))
            {
                url = url.Replace("?", "questionMark");
                existFile = $"{localPath}\\{ url.Replace("/", @"\")}";
            }
            else
            {
                existFile = $"{localPath}\\{ url.Replace("/", @"\")}";
            }

            var notExist = !File.Exists(existFile);
            var isEmpty = url != "/";
            var isNew = notExist && isEmpty && !string.IsNullOrEmpty(url);
            return isNew;
        }

        public static void CreateDirectory(string href, string storeLocation, string localPath)
        {
            string[] path = href.Split('/');
            string[] subpath = path.Take(path.Length - 1).ToArray();

            if (!Directory.Exists(storeLocation + "//" + String.Concat(subpath)))
            {

                var newPath = CombinePaths(localPath, subpath) + "/";
                Directory.CreateDirectory(newPath);
            }
        }

        public static string CombinePaths(string path1, params string[] paths)
        {
            if (path1 == null)
            {
                throw new ArgumentNullException("path1");
            }
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            return paths.Aggregate(path1, (acc, p) => Path.Combine(acc, p));
        }

        /// <summary>
        /// Delete all files in specified directory
        /// </summary>
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
