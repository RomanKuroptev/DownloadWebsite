using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Uri uri = new Uri("https://tretton37.com/");
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Console.WriteLine($"Downloading {uri} press ctrl + c to cancel");
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Console.WriteLine("\nDownload canceled");
                    cts.Cancel();
                    eventArgs.Cancel = true;
                };

                string storeLocation = @"C:\";
                string localPath = Path.Combine(storeLocation, uri.Host);

                if (Directory.Exists(localPath))
                {
                    PathHelper.DeleteDirectory(localPath);
                }
                Directory.CreateDirectory(localPath);

                DownloadSevice downloadService = new DownloadSevice(localPath, storeLocation);
                using (HttpClient client = new HttpClient())
                {
                    await downloadService.DownloadRecursiveAsync(uri, client, cts.Token);
                }
            }
        }

        

        

        
    }

}
