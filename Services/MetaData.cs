using System.Diagnostics;
using legoog.Models;
using legoog.Services;
using legoog.Services.DB;

namespace legoog.Services;

public class GetMetaData
{

    private Data? data;
    private DBService? dBService;
    const string pathconst = @"C:\tmp\crawlerdata\";
    const string titleParam = "<title";
    string[] otherParam = { "<p", "<a", "<h1", "<h2", "<h3", "<h4", "<h5", "<h6", "<i" };


    public void startMetadata()
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
                    data!.title = getPlainText(line); // later set it to the Title of the class Data
                }
            }
            keyWord(file); 
            getOtherMetaData(file);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    // TODO: update it, so it counts all words and then sets the keyword
    private void keyWord(string file)
    {
        data.keyword = new();
        try
        {
            using StreamReader sr = new StreamReader(file);
            string content = sr.ReadToEnd();
            var lines = content.Split(new[] { "r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.Contains("<h1"))
                {
                    string key = getPlainText(line);
                    data.keyword.Add(key);
                }
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
                    Thread keyWordCountTH = new Thread(() => containsKeyWord(line));
                    keyWordCountTH.Start();
                }
            }
        }
        // dBService.searchData.Add()
        deleteChachedFile(file);
    }


    // checks if the line contains keyword --> gets higher keywordcount 
    private void containsKeyWord(string line)
    {
        foreach (var keywords in data!.keyword!) {
            
            if (line.Contains(keywords))
            {
                data.keywordCount++;
            }
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

    private void deleteChachedFile(string file)
    {
        if (File.Exists(file)) File.Delete(file);
        getAllFiles(); // --> so it loops everytime over all files --> if there are any new
    }
}
