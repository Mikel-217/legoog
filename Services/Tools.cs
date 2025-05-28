using System.Diagnostics;

namespace legoog.Services;


public class TextEdit
{

    public string getPlainText(string line)
    {
        try
        {
            return "";
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return "";
        }
    }

    public string fileName(string url) 
    {
        try
        {
            return "";
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return "";
        }

    }

}