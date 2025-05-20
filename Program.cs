using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.ExceptionServices;

namespace legoog.Services;

public class Crawler
{

    Data data = new();
    const string pathconst = @"C:\tmp\crawlerdata\";
    const string linkParam = "href=";

    // just for testing --> gets replaced as soon as there is a web project :)
    public static void Main()
    {
        Crawler cr = new();
        cr.getAllFiles();
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


public class GetMetaData
{

    private Data? data;
    const string pathconst = @"C:\tmp\crawlerdata\";
    const string titleParam = "<title";
    string[] otherParam = { "<p", "<a", "<h1", "<h2", "<h3", "<h4", "<h5", "<h6", "<i" };


    public GetMetaData()
    {
        getAllFiles();
    }

    private void getAllFiles()
    {
        var files = Directory.GetFiles(pathconst);
        foreach (var file in files)
        {
            Thread metaAllTh = new Thread(() => getSiteTitle(file));
            metaAllTh.Start();
        }
    }

    // gets the title of each site 
    private void getSiteTitle(string file)
    {
        try
        {
            using StreamReader sr = new StreamReader(file);
            string content = sr.ReadToEnd();
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Contains(titleParam))
                {
                    data.title = getPlainText(line); // later set it to the Title of the class Data
                }
                getOtherMetaData(file);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    // searchs for the other data
    private void getOtherMetaData(string file)
    {
        using StreamReader sr = new StreamReader(file);
        string content = sr.ReadToEnd();
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        foreach (var line in lines)
        {
            for (int i = 0; otherParam.Length > i; i++)
            {
                if (line.Contains(otherParam[i]))
                {
                    containsKeyWord(line);
                }
            }
        }
        insertNewSearchResult();
    }

    // TODO: update it, so it counts all words and then sets the keyword

    // checks if the line contains keyword --> gets higher keywordcount 
    private void containsKeyWord(string line)
    {
        if (line.Contains(data.keyword!))
        {
            data.keywordCount++;
        }
    }

    // seperates the plain text from html --> can be used by all classes
    public string getPlainText(string line)
    {
        string firstSplitParam = ">";
        string secontSplitParam = "<";
        string[] firstSplit = line.Split(firstSplitParam);
        string[] secondSplit = firstSplit[1].Split(secontSplitParam);
        return secondSplit[0];
    }

    private void insertNewSearchResult()
    {
        // write DB Connection and insert all data
    }
}

// save for later --> Class whitch will be used do store data at DB
public class Data
{
    public string? title { get; set; }
    public string? keyword { get; set; }
    public string? url { get; set; }
    public int? keywordCount { get; set; } = 0;  // highest keyword count --> higher placement at search
}