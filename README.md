DownloadWebsite is a app to download https://tretton37.com/
To run the app navigate to C:\Users\Lawl\code\DownloadWebsite\DownloadWebsite\bin\Debug\netcoreapp3.1\DownloadWebsite.exe and execute.

Features included are
-Download of all HTML and CSS files
-Download of assets such as images
-Progress is shown by displaying the website currently being downloaded
-Cancelations support by pressing ctrl + C

The features where chosen because together they make a reasonable MVP in regards to specifications given
Cancelation support was added to take advantage of the use of asynchronous code
To improve performance the app downloads assets simultaneously in parallel.
But all pages are not downloaded simultaneously in parallel to avoid spamming the website and to avoid redundant calls of files already downloaded.
Some unit tests were added to ensure the code behaves correctly after possible future changes.
