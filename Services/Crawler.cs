using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;
using System.Data.SqlClient;
using legoog.Models;
using legoog.Services;
using legoog.Services.DB;

namespace legoog.Services;

public class Crawler
{

    Data data = new();
    const string pathconst = @"C:\tmp\crawlerdata\";
    const string linkParam = "href=";

    // just for testing --> gets replaced as soon as there is a web project :)
    public void startCrawler()
    {
        getAllFiles();
    }


    private void getAllFiles()
    {
        var files = Directory.GetFiles(pathconst);
        foreach (var file in files)
        {
            Thread crAllFile = new Thread(() => getAllLinks(file));
            crAllFile.Start();
        }
    }

    private void getAllLinks(string filename)
    {
        try
        {
            using StreamReader sr = new StreamReader(filename);
            string content = sr.ReadToEnd();
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Contains(linkParam))
                {
                    Thread crAllLink = new Thread(() => getUrl(line));
                    crAllLink.Start();
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    private void getUrl(string itemURl)
    {
        string firstSplit = "href=\"";
        string secontSplit = "\"";

        string[] first = itemURl.Split(firstSplit);
        string[] url = first[1].Split(secontSplit);
        if (url[0].Contains("https://") || url[0].Contains("http://")) DownHtml(url[0]).Wait();
    }

    private async Task DownHtml(string url)
    {
        try
        {
            data.url = url;
            string file = fileName(url);
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync(new Uri(url));
            response.EnsureSuccessStatusCode();
            string htmlText = await response.Content.ReadAsStringAsync();
            if (File.Exists(Path.Combine(pathconst, file))) File.Delete(Path.Combine(pathconst, file));
            File.WriteAllText(Path.Combine(pathconst, file), htmlText);
            getAllFiles();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    private string fileName(string url)
    {
        string split = "https://";
        string[] splitetURL = url.Split(split);
        string fileName = splitetURL[1].Replace(".", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("&", "").Replace("=", "") + ".txt";
        return fileName;
    }
}
