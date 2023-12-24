﻿#pragma warning disable CS0618
using Antlr4.Runtime;
using System;
using System.Globalization;
using System.Reflection;
using JavaScript.Parser.Json5;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Dynamic;
using System.Security.Cryptography;

//using Formatting = Newtonsoft.Json.Formatting;

namespace JavaScript;

/** @brief MyClass does something
 * @details I have something more long winded to say about it.  See example 
 * in test.cs: @include test.cs */
internal class Util
{
    static Util()
    {
    }

    public static dynamic? Copy(dynamic? x)
    {
        if (x is null) return null;
        if (x is ExpandoObject)
        {
            var result = new ExpandoObject();
            var dict = x as IDictionary<string, object>;
            foreach (var pair in dict)
            {
                if (Util.FullName(pair.Value) == "System.Action") continue;
                if (Util.FullName(pair.Value) == "System.Func") continue;
                (result as IDictionary<string, object>)[pair.Key] = Copy(pair.Value);
            }
            return result;
        } else if (x is List<object?>)
        {
            var result = new List<object?>();
            var list = x as List<object?>;
            for (int i=0; i<list.Count; i++)
            {
                result.Add(list[i]);
            }
            return result;
        }
        return x;
    }
    public static string DateTimeString(DateTime x)
    {
        return x.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
    }
    public static string LoadText(string path)
    {
        return File.ReadAllText(path);
    }
    public static void WaitForever()
    {
        while (true)
        {
            Util.Sleep(3600000);
        }
    }
    public static string UserName()
    {
        return Environment.UserName;
    }
    public static string[] ExpandWildcard(string path)
    {
        string? dir = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(dir)) dir = ".";
        //Util.Print(dir, "dir");
        string fname = Path.GetFileName(path);
        //Util.Print(fname, "fname");
        string[] files = Directory.GetFiles(dir, fname);
        List<string> result = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            result.Add(Path.GetFullPath(files[i]));
        }

        return result.ToArray();
    }
    public static string[] ExpandWildcardList(params string[] pathList)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < pathList.Length; i++)
        {
            //Util.Print(pathList[i], "pathList[i]");
            string[] files = ExpandWildcard(pathList[i]);
            result.AddRange(files.ToList());
        }

        return result.ToArray();
    }
    public static int RunToConsole(string exePath, string[] args, Dictionary<string, string>? vars = null)
    {
        string argList = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0) argList += " ";
            argList += $"\"{args[i]}\"";
        }
        Process process = new Process();
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = exePath;
        process.StartInfo.Arguments = argList;
        if (vars != null)
        {
            var keys = vars.Keys;
            foreach (var key in keys)
            {
                process.StartInfo.EnvironmentVariables[key] = vars[key];
            }
        }
        process.OutputDataReceived += (sender, e) => { Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { Console.Error.WriteLine(e.Data); };
        process.Start();
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) { process.Kill(); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        process.CancelOutputRead();
        process.CancelErrorRead();
        return process.ExitCode;
    }
    /**
     * <summary> Sleeps for specified milliseconds (指定されたミリ秒間スリープする)
     * </summary>
     * <description>
     * @code
     * using System;
     * using Global;
     * Util.Print(DateTime.Now, "begin");
     * Util.Sleep(3000); // sleeps for 3 seconds
     * Util.Print(DateTime.Now, "end");
     * @endcode
     * @code
     * begin: 2023-11-05T21:30:41.8610034+09:00
     * end: 2023-11-05T21:30:45.1998930+09:00
     * @endcode
     * </description>
     * @param[in] milliseconds milliseconds (ミリ秒)
     */
    public static void Sleep(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }
    /**
     * <summary>Wait until specified DateTime (指定されたDateTimeまで待機する)
     * </summary>
     * <description>
     * @code
     * using System;
     * using Global;
     * Util.WatitUntil(DateTime.Now.AddSeconds(3)); // waits for 3 seconds
     * @endcode
     * @code
     * Please wait until 2023/11/05 Sun 21:40:42.
     * Time left: 00:00:00....Restarting process.
     * @endcode
     * </description>
     * @param[in] dt DateTime
     */
    public static void WatitUntil(DateTime dt)
    {
        uint cp = Util.GetConsoleCP();
        if (cp == 932)
            Console.WriteLine($"{dt} までお待ちください。", dt);
        else
            Console.WriteLine($"Please wait until {dt}.", dt);
        while (DateTime.Now < dt)
        {
            try
            {
                var span = dt - DateTime.Now;
                var sec = (long)span.TotalSeconds;
                Console.CursorLeft = 0;
                if (cp == 932)
                    Console.Write($"後 {span.ToString(@"hh\:mm\:ss")} お待ちください。");
                else
                    Console.Write($"Time left: {span.ToString(@"hh\:mm\:ss")}.");
            }
            catch (Exception)
            {
            }
            Thread.Sleep(1000);
        }
        if (cp == 932)
            Console.WriteLine("・・・処理を再開します。");
        else
            Console.WriteLine("...Restarting process.");
        Thread.Sleep(1000);
    }
    public static string AssemblyName(Assembly assembly)
    {
        return System.Reflection.AssemblyName.GetAssemblyName(assembly.Location).Name;
    }

    public static int FreeTcpPort()
    {
        // https://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }

    public static DateTime? AsDateTime(dynamic x)
    {
        if (x is null) return null;
        string fullName = Util.FullName(x);
        if (fullName == "Newtonsoft.Json.Linq.JValue")
        {
            return ((DateTime)x);
        }
        else if (fullName == "System.DateTime")
        {
            return (System.DateTime)x;
        }
        else if (fullName == "System.String")
        {
            if (((string)x) == "") return null;
            return DateTime.Parse((string)x);
        }
        else
        {
            throw new ArgumentException("x");
        }
    }

    /*
    public static string ShortenString(string s, int limit, string ellipsis = "...")
    {
        var enc = new UTF32Encoding();
        byte[] byteUtf32 = enc.GetBytes(s);
        if (byteUtf32.Length <= limit * 4) return s;
        ArraySegment<byte> segment = new ArraySegment<byte>(byteUtf32, 0, limit * 4);
        byteUtf32 = segment.ToArray();
        string decodedString = enc.GetString(byteUtf32);
        return decodedString + ellipsis;
    }
    public static string GetFileNameFromUrl(string url)
    {
        var list = url.Split('/');
        var fileName = list[list.Length - 1];
        return fileName;
    }
    public static string GetFileBaseNameFromUrl(string url)
    {
        var fileName = GetFileNameFromUrl(url);
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        return baseName;
    }
    public static string GetStringFromUrl(string url)
    {
        HttpWebRequest? request = WebRequest.Create(url) as HttpWebRequest;
        HttpWebResponse response = (HttpWebResponse)request!.GetResponse();
        WebHeaderCollection header = response.Headers;
        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }
    public static void DownloadBinaryFromUrl(string url, string destinationPath)
    {
        string parent = Util.ParentDirectoryPath(destinationPath);
        Directory.CreateDirectory(parent);
        WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
        var objResponse = objRequest.GetResponse();
        byte[] buffer = new byte[32768];
        using (Stream input = objResponse.GetResponseStream())
        {
            using (FileStream output = new FileStream(destinationPath, FileMode.CreateNew))
            {
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
    public static string ParentDirectoryPath(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        return di.Parent.FullName;
    }
    */
    public static string FullName(dynamic x)
    {
        if (x is null) return "null";
        string fullName = ((object)x).GetType().FullName;
        return fullName.Split('`')[0];
    }

    public static string ToJsonHelper(dynamic x, bool indent = false)
    {
        return JsonConvert.SerializeObject(x, indent ? Formatting.Indented : Formatting.None);
    }

    public static string ToJson(dynamic? x, bool indent = false)
    {
        x = Util.Copy(x);
        return Util.ToJsonHelper(x, indent);
    }
#if false
    public static dynamic? FromJson(string json)
    {
        if (String.IsNullOrEmpty(json)) return null;
        return JsonConvert.DeserializeObject(json, new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.None
        });
        /*
        return JObject.Parse(json, new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Load
        });
        */
    }
#endif

    public static dynamic? FromJson(string json)
    {
        if (String.IsNullOrEmpty(json)) return null;
        var inputStream = new AntlrInputStream(json);
        var lexer = new JSON5Lexer(inputStream);
        var commonTokenStream = new CommonTokenStream(lexer);
        var parser = new JSON5Parser(commonTokenStream);
        var context = parser.json5();
        var result = FromObject(JSON5ToObject(context));
        return result;
    }

    public static dynamic? FromJson(byte[] json)
    {
        if (json is null) return null;
        return FromJson(Encoding.UTF8.GetString(json));
    }

    public static T? FromJson<T>(string json, T? fallback = default(T))
    {
        //if (String.IsNullOrEmpty(json)) return default(T);
        if (String.IsNullOrEmpty(json)) return fallback;
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static byte[] ToBson(dynamic x)
    {
        MemoryStream ms = new MemoryStream();
        using (BsonWriter writer = new BsonWriter(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(writer, x);
        }

        return ms.ToArray();
    }

    public static dynamic? FromBson(byte[] bson)
    {
        if (bson is null) return null;
        MemoryStream ms = new MemoryStream(bson);
        using (BsonReader reader = new BsonReader(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Deserialize(reader);
        }
    }

    public static T? FromBson<T>(byte[] bson)
    {
        if (bson is null) return default(T);
        MemoryStream ms = new MemoryStream(bson);
        using (BsonReader reader = new BsonReader(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            return serializer.Deserialize<T>(reader);
        }
    }

    public static dynamic? FromObject(dynamic x)
    {
        if (x is null) return null;
        var o = (dynamic)JObject.FromObject(new { x = x },
            new JsonSerializer
            {
                DateParseHandling = DateParseHandling.None
            });
        return o.x;
    }

    public static T? FromObject<T>(dynamic x)
    {
        dynamic? o = FromObject(x);
        if (o is null) return default(T);
        return (T)(o.ToObject<T>());
    }

    public static string? ToXml(dynamic x)
    {
        if (x is null) return null;
        if (FullName(x) == "System.Xml.Linq.XElement")
        {
            return ((XElement)x).ToString();
        }

        XDocument? doc;
        if (FullName(x) == "System.Xml.Linq.XDocument")
        {
            doc = (XDocument)x;
        }
        else
        {
            string json = ToJson(x);
            doc = JsonConvert.DeserializeXmlNode(json)?.ToXDocument();
            //return "<?>";
        }

        return doc is null ? "null" : doc.ToStringWithDeclaration();
    }

    public static XDocument? FromXml(string xml)
    {
        if (xml is null) return null;
        XDocument doc = XDocument.Parse(xml);
        return doc;
    }

    public static string ToString(dynamic x)
    {
        if (x is null) return "null";
        if (x is string) return (string)x;
        if (x is Newtonsoft.Json.Linq.JValue)
        {
            var value = (JValue)x;
            try
            {
                x = (DateTime)value;
                //return Util.DateTimeString(x); //x.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
            }
            catch (Exception)
            {
            }
        }

        if (x is System.Xml.Linq.XDocument || x is System.Xml.Linq.XElement)
        {
            string xml = ToXml(x);
            return xml;
        }
        else if (x is IEnumerable<XElement>)
        {
            XElement result = new XElement("IEnumerable");
            foreach (var e in x)
            {
                //string xml = ToXml(e);
                result.Add(e);
            }
            return ToString(result);
        }
        else if (x is System.DateTime)
        {
            //return x.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
            return Util.DateTimeString(x); //x.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz");
        }
        else
        {
            try
            {
                string json = ToJson(x, true);
                return json;
            }
            catch (Exception)
            {
                return x.ToString();
            }
        }
    }

    public static void Print(dynamic x, string? title = null)
    {
        String s = "";
        if (title != null) s = title + ": ";
        s += Util.ToString(x);
        Console.WriteLine(s);
        System.Diagnostics.Debug.WriteLine(s);
    }

    public static void Log(dynamic x, string? title = null)
    {
        String s = "";
        if (title != null) s = title + ": ";
        s += Util.ToString(x);
        Console.Error.WriteLine(s);
        System.Diagnostics.Debug.WriteLine(s);
    }

    public static XDocument ParseXml(string xml)
    {
        XDocument doc = XDocument.Parse(xml);
        return doc;
    }

    public static string[] ResourceNames(Assembly assembly)
    {
        return assembly.GetManifestResourceNames();
    }

    public static Stream? ResourceAsStream(Assembly assembly, string name)
    {
        Stream? stream = assembly.GetManifestResourceStream($"{AssemblyName(assembly)}.{name}");
        return stream;
    }

    public static string StreamAsText(Stream stream)
    {
        if (stream is null) return "";
        long pos = stream.Position;
        var streamReader = new StreamReader(stream);
        var text = streamReader.ReadToEnd();
        stream.Position = pos;
        return text;
    }

    public static string ResourceAsText(Assembly assembly, string name)
    {
        Stream stream = assembly.GetManifestResourceStream($"{AssemblyName(assembly)}.{name}");
        //if (stream is null) return "";
        //var streamReader = new StreamReader(stream);
        //var text = streamReader.ReadToEnd();
        //return text;
        return StreamAsText(stream);
    }

    public static byte[] StreamAsBytes(Stream stream)
    {
        if (stream is null) return new byte[] { };
        long pos = stream.Position;
        byte[] bytes = new byte[(int)stream.Length];
        stream.Read(bytes, 0, (int)stream.Length);
        stream.Position = pos;
        return bytes;
    }

    public static byte[] ResourceAsBytes(Assembly assembly, string name)
    {
        Stream stream = assembly.GetManifestResourceStream($"{AssemblyName(assembly)}.{name}");
        //if (stream is null) return new byte[] { };
        //byte[] bytes = new byte[(int)stream.Length];
        //stream.Read(bytes, 0, (int)stream.Length);
        //return bytes;
        return StreamAsBytes(stream);
    }

    public static dynamic? StreamAsJson(Stream stream)
    {
        string json = StreamAsText(stream);
        return FromJson(json);
    }

    public static dynamic? ResourceAsJson(Assembly assembly, string name)
    {
        string json = ResourceAsText(assembly, name);
        return FromJson(json);
    }

    /*
    public static string HttpGetString(string url)
    {
        using (var client = new HttpClient())
        {
            string json;
            var task1 = Task.Run(() => client.GetAsync(url));
            task1.Wait();
            var response = task1.Result;
            var task2 = Task.Run(() => response.Content.ReadAsStringAsync());
            task1.Wait();
            json = task2.Result;
            return json;
        }
    }
    */
    public static string FirstPart(string s, params char[] separator)
    {
        string[] split = s.Split(separator);
        if (split.Length == 0) return "";
        return split[0];
    }

    public static string LastPart(string s, params char[] separator)
    {
        string[] split = s.Split(separator);
        if (split.Length == 0) return "";
        return split[split.Length - 1];
    }

    public static string GetRidirectUrl(string url)
    {
        Task<string> task = GetRidirectUrlTask(url);
        task.Wait();
        return task.Result;
    }

    private static async Task<string> GetRidirectUrlTask(string url)
    {
        HttpClient client;
        HttpResponseMessage response;
        try
        {
            client = new HttpClient();
            response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return url;
        }

        string result = response.RequestMessage.RequestUri.ToString();
        response.Dispose();
        return result;
    }

    public static byte[]? ToUtf8Bytes(string? s)
    {
        if (s is null) return null;
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
        return bytes;
    }

    private static dynamic? JSON5ToObject(ParserRuleContext x)
    {
        //Log(FullName(x), "FullName(x)");
        string fullName = FullName(x);
        switch (fullName)
        {
            case "JavaScript.Parser.Json5.JSON5Parser+Json5Context":
                {
                    for (int i = 0; i < x.children.Count; i++)
                    {
                        //Print("  " + FullName(x.children[i]));
                        //Print("    " + JSON5Terminal((x.children[i])));
                        if (x.children[i] is Antlr4.Runtime.Tree.ErrorNodeImpl)
                        {
                            return null;
                        }
                    }

                    return JSON5ToObject((ParserRuleContext)x.children[0]);
                }
            case "JavaScript.Parser.Json5.JSON5Parser+ValueContext":
                {
                    if (x.children[0] is Antlr4.Runtime.Tree.TerminalNodeImpl)
                    {
                        string t = JSON5Terminal(x.children[0])!;
                        if (t.StartsWith("\""))
                        {
                            return Utf8Json.JsonSerializer.Deserialize<string>(t);
                        }

                        if (t.StartsWith("'"))
                        {
                            //Log(t, "t");
                            t = t.Substring(1, t.Length - 2).Replace("\\'", ",").Replace("\"", "\\\"");
                            t = "\"" + t + "\"";
                            //Log(t, "t");
                            return Utf8Json.JsonSerializer.Deserialize<string>(t);
                        }

                        switch (t)
                        {
                            case "true":
                                return true;
                            case "false":
                                return false;
                            case "null":
                                return null;
                        }

                        throw new Exception($"Unexpected JSON5Parser+ValueContext: {t}");
                        //return t;
                    }

                    return JSON5ToObject((ParserRuleContext)x.children[0]);
                }
            case "JavaScript.Parser.Json5.JSON5Parser+ArrContext":
                {
                    var result = new JArray();
                    for (int i = 0; i < x.children.Count; i++)
                    {
                        //Print("  " + FullName(x.children[i]));
                        if (x.children[i] is JSON5Parser.ValueContext value)
                        {
                            result.Add(JSON5ToObject(value));
                        }
                    }

                    return result;
                }
            case "JavaScript.Parser.Json5.JSON5Parser+ObjContext":
                {
                    var result = new JObject();
                    for (int i = 0; i < x.children.Count; i++)
                    {
                        //Print("  " + FullName(x.children[i]));
                        if (x.children[i] is JSON5Parser.PairContext pair)
                        {
                            var pairObj = JSON5ToObject(pair);
                            result[(string)pairObj!["key"]] = pairObj["value"];
                        }
                    }

                    return result;
                }
            case "JavaScript.Parser.Json5.JSON5Parser+PairContext":
                {
                    var result = new JObject();
                    for (int i = 0; i < x.children.Count; i++)
                    {
                        //Print("  " + FullName(x.children[i]));
                        if (x.children[i] is JSON5Parser.KeyContext key)
                        {
                            result["key"] = JSON5ToObject(key);
                        }

                        if (x.children[i] is JSON5Parser.ValueContext value)
                        {
                            result["value"] = JSON5ToObject(value);
                        }
                    }

                    return result;
                }
            case "JavaScript.Parser.Json5.JSON5Parser+KeyContext":
                {
                    //string t = JSON5Terminal(x.children[0])!;
                    if (x.children[0] is Antlr4.Runtime.Tree.TerminalNodeImpl)
                    {
                        string t = JSON5Terminal(x.children[0])!;
                        if (t.StartsWith("\""))
                        {
                            return Utf8Json.JsonSerializer.Deserialize<string>(t);
                        }

                        if (t.StartsWith("'"))
                        {
                            //Log(t, "t");
                            t = t.Substring(1, t.Length - 2).Replace("\\'", ",").Replace("\"", "\\\"");
                            t = "\"" + t + "\"";
                            //Log(t, "t");
                            return Utf8Json.JsonSerializer.Deserialize<string>(t);
                        }

                        return t;
                    }
                    else
                    {
                        return "?";
                    }
                }
            case "JavaScript.Parser.Json5.JSON5Parser+NumberContext":
                {
                    return decimal.Parse(JSON5Terminal(x.children[0]), CultureInfo.InvariantCulture);
                }
            default:
                Console.Error.WriteLine($"Unexpected: {fullName}");
                throw new Exception($"Unexpected: {fullName}");
        }

        //return null;
    }

    private static string? JSON5Terminal(Antlr4.Runtime.Tree.IParseTree x)
    {
        if (x is Antlr4.Runtime.Tree.TerminalNodeImpl t)
        {
            return t.ToString();
        }

        return null;
    }

    public static void AllocConsole()
    {
        WinConsole.Initialize();
#if false
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            WinConsole.Initialize();
        }
        else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
        {
            ;
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            ;
        }
#endif
    }

    public static void FreeConsole()
    {
        WinConsole.Deinitialize();
#if false
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            WinConsole.Deinitialize();
        }
        else
        {
            Console.SetOut(TextWriter.Null);

        }
#endif
    }
    public static void ReallocConsole()
    {
        FreeConsole();
        AllocConsole();
    }
    public static uint GetConsoleCP()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            return NativeMethods.GetConsoleCP();
        }
        return 65001;
    }

    public static void Message(dynamic x, string? title = null)
    {
        if (title is null) title = "Message";
        if ((x as string) != null)
        {
            var s = (string)x;
            System.Diagnostics.Debug.WriteLine(s);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                NativeMethods.MessageBoxW(IntPtr.Zero, s, title, 0);
            }
            else
            {
                Util.Log(s, title);
            }
            return;
        }

        if (FullName(x) == "System.Xml.Linq.XDocument" ||
            FullName(x) == "System.Xml.Linq.XElement")
        {
            string xml = ToXml(x);
            System.Diagnostics.Debug.WriteLine(xml);
            var s = (string)x;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                NativeMethods.MessageBoxW(IntPtr.Zero, xml, title, 0);
            }
            else
            {
                Util.Log(xml, title);
            }
        }
        else
        {
            string json = ToJson(x, true);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                NativeMethods.MessageBoxW(IntPtr.Zero, json, title, 0);
            }
            else
            {
                Util.Log(json, title);
            }
        }
    }

    public static dynamic? FromNewton(dynamic x)
    {
        dynamic? dyn = FromObject(x);
        return FromNewtonHelper(dyn);
    }

    private static dynamic? FromNewtonHelper(dynamic? x)
    {
        if (x is null) return null;
        if (x is JArray)
        {
            var result = new List<object>();
            var ary = (JArray)x;
            foreach (var elem in ary)
            {
                result.Add(FromNewtonHelper(elem));
            }
            return result /*.ToArray()*/;
        }
        else if (x is JObject)
        {
            dynamic result = new ExpandoObject();
            var obj = (JObject)x;
            foreach (var pair in obj)
            {
                ((IDictionary<String, Object>)result)[pair.Key] = FromNewtonHelper(pair.Value);
            }
            result.Keys = (Func<List<string>>)
                (() =>
                {
                    var keys = new List<string>();
                    foreach (var pair in result)
                    {
                        if (Util.FullName(pair.Value) == "System.Action") continue;
                        if (Util.FullName(pair.Value) == "System.Func") continue;
                        keys.Add(pair.Key);
                    }
                    return keys;
                });
            result.ContainsKey = (Func<string, bool>)
                ((key) =>
                {
                    var dict = (IDictionary<String, Object>)result;
                    return dict.ContainsKey(key);
                });
            result.Get = (Func<string, dynamic?>)
                ((key) =>
                {
                    try
                    {
                        var dict = (IDictionary<String, Object>)result;
                        return (dynamic?)dict[key];
                    }
                    catch (KeyNotFoundException ex)
                    {
                        return null;
                    }
                });
            result.Put = (Action<string, object?>)
                ((key, value) =>
                {
                    var dict = (IDictionary<String, Object>)result;
                    dict[key] = value;
                });
            result.Add = (Action<string, object?>)
                ((key, value) =>
                {
                    var dict = (IDictionary<String, Object>)result;
                    dict.Add(key, value);
                });
            return result;
        }
        else
        {
            var result = (JValue)x;
            return result.Value;
        }
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int MessageBoxW(
            IntPtr hWnd, string lpText, string lpCaption, uint uType);

        //[DllImport("kernel32.dll")]
        //internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        internal static extern uint GetConsoleCP();
    }
}