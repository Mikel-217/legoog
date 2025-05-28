using legoog.Models;
using legoog.Services;
using legoog.Services.DB;

namespace Program;

public class PMAin
{

    // just for testing --> gets replaced as soon as there is a web project :)
    public static void Main()
    {
        const string pathconst = @"C:\tmp\crawlerdata\";
        if(!Directory.Exists(pathconst)) Directory.CreateDirectory(pathconst);

        Crawler cr = new Crawler();
        GetMetaData meta = new GetMetaData();
        
        Thread crTH = new Thread(() => cr.startCrawler());
        Thread metaTH = new Thread(() => meta.startMetadata());
        crTH.Start();
        metaTH.Start();
        Console.WriteLine("Both Threads started sucessfully");
    }
}
