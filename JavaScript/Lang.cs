using GScript;
//using System.Dynamic;
using System.Reflection;

namespace JavaScript;

public class Lang
{
    public static GScript.Engine CreateEngine(params Assembly[] list)
    {
        var engine = new GScript.Engine(cfg =>
        {
            //cfg.AllowClr(typeof(Global.Util).Assembly);
            cfg.AllowClr();
            for (int i = 0; i < list.Length; i++)
            {
                cfg.AllowClr(list[i]);
            }
        });
        engine.SetValue("_console", new JavaScriptConsole());
        engine.Execute(@"
var print = _console.print;
var log = _console.log;
");
        return engine;
    }
    public static void Print(dynamic? x, string? title = null)
    {
        Util.Print(x, title);
    }
    public static void Log(dynamic? x, string? title = null)
    {
        Util.Log(x, title);
    }
    public static dynamic? FromJson(string json)
    {
        dynamic? x = Util.FromJson(json);
        x = Util.FromNewton(x);
        return x;
    }
    public static string ToJson(dynamic? x)
    {
        x = Util.Copy(x);
        return Util.ToJson(x);
    }
    public static dynamic? FromObject(dynamic? x)
    {
        x = Util.Copy(x);
        x = Util.FromObject(x);
        x = Util.FromNewton(x);
        return x;
    }
    public static string FullName(dynamic? x)
    {
        return Util.FullName(x);
    }
}

internal class JavaScriptConsole
{
    public void print(dynamic x, string? title = null)
    {
        Util.Print(x, title);
    }
    public void log(dynamic x, string? title = null)
    {
        Util.Log(x, title);
    }
}