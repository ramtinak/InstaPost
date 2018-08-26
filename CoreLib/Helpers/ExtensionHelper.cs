using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


public static class ExtensionHelper
{
    public static void OpenUrl(this string url)
    {
        try
        {
            Process.Start(url);
        }
        catch (Exception ex) { ex.PrintException("OpenUrl"); }
    }
    public static string PrintException(this Exception ex, string name = "")
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{name} exception thrown: {ex.Message}");
        sb.AppendLine($"Source: {ex.Source}");
        sb.AppendLine($"Stack trace: {ex.StackTrace}");
        sb.AppendLine();
        return sb.Output();
    }

    public static string Output(this object source, string start = "")
    {
        string content = $"{start} {Convert.ToString(source)}";
        Debug.WriteLine(content);
        return content;
    }
}

