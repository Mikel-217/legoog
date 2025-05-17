using System.Diagnostics;

namespace legoog.Services;

public class Crwaler {

    const string pathconst = @"C:\tmp\crawlerdata\";
    const string linkParam = "href=";

    // just for testing --> gets replaced as soon as there is a web project :)
    public static void Main() {
        Crwaler cr = new();
        cr.getAllFiles();
        
    }

    private void getAllFiles() {
        var files = Directory.GetFiles(pathconst);
        foreach(var file in files) {
            Thread thread = new Thread(() => getAllLinks(file));
            thread.Start();
        }
    }

    private void getAllLinks(string filename) {
        try {
            using StreamReader sr = new StreamReader(filename);
            string content = sr.ReadToEnd();
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines) {
                if (line.Contains(linkParam)) {
                    Thread thread = new Thread(() => getUrl(line));
                    thread.Start();
                }
            }
        } catch(Exception e) {
            throw new Exception(e.Message);
        }
    }

    private void getUrl(string itemURl) {
        string firstSplit = "href=\"";
        string secontSplit = "\"";

        string[] first = itemURl.Split(firstSplit);
        string[] url = first[1].Split(secontSplit);
        if(url[0].Contains("https://")) DownHtml(url[0]).Wait();
    }

    // start with new Thread --> better performance
    private async Task DownHtml(string url) {
        try {
            string file = fileName(url);
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync(new Uri(url));
            response.EnsureSuccessStatusCode();
            string htmlText = await response.Content.ReadAsStringAsync();
            if(File.Exists(Path.Combine(pathconst, file))) File.Delete(Path.Combine(pathconst, file));
            File.WriteAllText(Path.Combine(pathconst, file), htmlText);
        } catch(Exception e) {
            Debug.WriteLine(e.Message);
        }
    }

    private string fileName(string url) {
        string split = "https://";
        string[] splitetURL = url.Split(split); 
        string fileName = splitetURL[1].Replace(".", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("&", "").Replace("=", "")+ ".txt";
        return fileName;
    }
}
