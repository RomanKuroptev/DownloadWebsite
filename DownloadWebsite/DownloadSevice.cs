using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// Service for downloading website with assets to local storage
    /// </summary>
    public class DownloadSevice
    {
        private readonly string localPath;
        private readonly string storeLocation;

        public DownloadSevice(string localPath, string storeLocation)
        {
            this.localPath = localPath;
            this.storeLocation = storeLocation;
        }

        /// <summary>
        /// Recursivly traverses and downloads all hmtl files and assets on specified URI
        /// </summary>        
        public async Task DownloadRecursiveAsync(Uri uri, HttpClient client, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            try
            {
                Console.SetCursorPosition(0, 1);
                ClearCurrentConsoleLine();
                Console.Write(uri);

                HtmlWeb hw = new HtmlWeb();
                hw.OverrideEncoding = Encoding.UTF8;
                HtmlDocument doc = new HtmlDocument();
                try
                {
                    doc.Load(await client.GetStreamAsync(uri), Encoding.UTF8);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{uri} {e.Message}");
                    return;
                }
                List<string> image_links = new List<string>();
                HtmlNodeCollection imageNodes = doc.DocumentNode.SelectNodes("//img");
                HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//link");

                if (linkNodes != null)
                {
                    image_links.AddRange(linkNodes?.Select(link => link.GetAttributeValue("href", "")));
                }

                if (imageNodes != null)
                {
                    image_links.AddRange(imageNodes?.Select(link => link.GetAttributeValue("src", "")));
                }

                List<Task> tasks = image_links.Distinct().Select(link => DownloadAssets(uri, link, client, ct)).ToList();

                await Task.WhenAll(tasks);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");

                foreach (HtmlNode node in nodes)
                {
                    await Traverse(uri, node, client, ct);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private async Task DownloadAssets(Uri uri, string link, HttpClient client, CancellationToken ct)
        {
            link = UrlHelper.RemovePrefix(link);
            if (!UrlHelper.IsAbsoluteUrl(link) && PathHelper.CheckIfNewFileOrEmpty(link, localPath))
            {
                link = UrlHelper.Normalize(link);
                PathHelper.CreateDirectory(link, storeLocation, localPath);
                await DownloadFileAsync(uri, $"{localPath}{link}", client, ct);
            }
        }

        private async Task Traverse(Uri uri, HtmlNode link, HttpClient client, CancellationToken ct)
        {
            string href = link.Attributes["href"].Value.ToString();

            href = UrlHelper.Normalize(href);
            if (!UrlHelper.IsAbsoluteUrl(href) && PathHelper.CheckIfNewFileOrEmpty(href, localPath))
            {
                PathHelper.CreateDirectory(href, storeLocation, localPath);
                string newUrl = $"https://{uri.Host}{href}";
                await DownloadHtmlAsync(new Uri(newUrl), client, ct);
                await DownloadRecursiveAsync(new Uri(newUrl), client, ct);
            }
        }

        private async Task DownloadHtmlAsync(Uri uri, HttpClient client, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                await DownloadFileAsync(uri, $"{localPath}{uri.LocalPath}{uri.Fragment}.html", client, ct);
            }
            else if (uri.LocalPath == "/")
            {
                await DownloadFileAsync(uri, $"{localPath}{uri.LocalPath}index", client, ct);
            }
            else if (uri.LocalPath.Contains("."))
            {
                await DownloadFileAsync(uri, $"{localPath}{uri.LocalPath}", client, ct);
            }
            else
            {
                await DownloadFileAsync(uri, $"{localPath}{uri.LocalPath}.html", client, ct);
            }
        }

        private async Task DownloadFileAsync(Uri requestUri, string filename, HttpClient httpClient, CancellationToken ct)
        {
            if (filename == null)
                throw new ArgumentNullException(filename);

            try
            {
                using (Stream stream = await httpClient.GetStreamAsync(requestUri))
                {
                    byte[] bytes = Encoding.Default.GetBytes(filename);
                    filename = Encoding.UTF8.GetString(bytes);

                    using (FileStream fileStream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        await stream.CopyToAsync(fileStream, ct);
                    }
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }
        private void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
