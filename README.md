# JavaScript

```
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Xml.Linq;
using JavaScript;
using NUnit.Framework;
using System.Linq;
using Newtonsoft.Json.Linq;

Console.WriteLine("Version: {0}", Environment.Version.ToString());
Lang.Print(DateTime.Now);
{
    var json = Lang.ToJson(new[] { 777, 888 });
    Lang.Print(json); // to stdout
    Lang.Log(json); // to stderr
    Assert.AreEqual("[777,888]", json);
}
{
    var jsonData = Lang.FromObject(new[] { 777, 888 });
    Lang.Print(jsonData, "(1)");
    Assert.AreEqual("[777,888]", Lang.ToJson(jsonData));
    Lang.Print(jsonData[0], "(2)");
    Assert.AreEqual(777, (int)jsonData[0]);
    jsonData = Lang.FromObject(jsonData);
    Lang.Print(jsonData, "(3)");
    Assert.AreEqual("[777,888]", Lang.ToJson(jsonData));

    jsonData = Lang.FromObject(new { a = 1, b = 2 });
    Lang.Print(jsonData, "(4)");
    Assert.AreEqual("{\"a\":1,\"b\":2}", Lang.ToJson(jsonData));
    Lang.Print(jsonData.a, "(5)");
    Assert.AreEqual("1", Lang.ToJson(jsonData.a));
    jsonData.a = 777;
    Lang.Print(jsonData, "(6)");
    Assert.AreEqual("{\"a\":777,\"b\":2}", Lang.ToJson(jsonData));
    Lang.Print(Lang.FullName(jsonData), "(7)");
    Assert.AreEqual("System.Dynamic.ExpandoObject", Lang.FullName(jsonData));
    jsonData.c = 888;
    Lang.Print(jsonData, "(9)");
    Assert.AreEqual("{\"a\":777,\"b\":2,\"c\":888}", Lang.ToJson(jsonData));
}

{
    string json = @"{
  'channel': {
    'title': 'James Newton-King',
    'link': 'http://james.newtonking.com',
    'description': 'James Newton-King\'s blog.',
    'item': [
      {
        'title': 'Json.NET 1.3 + New license + Now on CodePlex',
        'description': 'Announcing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'CodePlex'
        ]
      },
      {
        'title': 'LINQ to JSON beta',
        'description': 'Announcing LINQ to JSON',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'LINQ'
        ]
      }
    ]
  }
}";
    var rss = Lang.FromJson(json);
    Lang.Print(rss, "rss");
    var categories = rss.channel.item[0].categories;
    Lang.Print(categories, "categories");
    Assert.AreEqual("[\"Json.NET\",\"CodePlex\"]", Lang.ToJson(categories));
}
{
    Lang.Print("before (10)");
    var n = Lang.FromJson("18446744073709551615");
    Lang.Print(n, "(10)");
    Assert.AreEqual(18446744073709551615, (UInt64)n);
}
{
    dynamic flexible = new ExpandoObject();
    flexible.Int = 3;
    flexible.String = "hi";
    flexible.Deep = new ExpandoObject();
    flexible.Deep.Deeper = 777;
    var dictionary = (IDictionary<string, object>)flexible;
    dictionary.Add("Bool", false);
    Lang.Print(flexible, "(11)");
    Assert.AreEqual("{\"Int\":3,\"String\":\"hi\",\"Deep\":{\"Deeper\":777},\"Bool\":false}", Lang.ToJson(flexible));
}
var settings = Lang.FromJson("{ a: null }");
settings.a = (settings.a != null ? settings.a : Lang.FromJson("{}"));
settings.a.b = 123;
settings.a.c = 456;
Lang.Print(settings, "(16)");
Assert.AreEqual("{\"a\":{\"b\":123,\"c\":456}}", Lang.ToJson(settings));
var results = Lang.FromJson(@"{ 'a': 123 /* my commnet */}");
Lang.Print(results, "(17)");
{
    string json = @"{
  'channel': {
    'title': 'James Newton-King',
    'link': { '#text': 'http://james.newtonking.com', '@target': '_blank' },
    'description': { '#cdata-section': 'James Newton-King\'s blog.' },
    'item': [
      {
        'title': 'Json.NET 1.3 + New license + Now on CodePlex',
        'description': 'Announcing the release of Json.NET 1.3, the MIT license and the source on CodePlex',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'CodePlex'
        ]
      },
      {
        'title': 'LINQ to JSON beta',
        'description': 'Announcing LINQ to JSON',
        'link': 'http://james.newtonking.com/projects/json-net.aspx',
        'categories': [
          'Json.NET',
          'LINQ'
        ]
      }
    ]
  }
}";
    var rss = Lang.FromJson(json);
}
XElement data = new XElement("メンバー情報",
                             new XAttribute("属性1", true),
                             new XElement("名前", "田中"),
                             new XElement("住所", "大阪府大阪市"),
                             new XElement("年齢", "35"));
XElement list = new XElement("チーム", new XAttribute("属性2", DateTime.Now));
list.Add(data);
XDocument yourResult = new XDocument(list);
Lang.Print(yourResult, "(24)");
Lang.Print(Lang.FromObject(yourResult), "(25)");
Lang.Print((long)"0".FromJson(), "(26)");
Lang.Print((bool)"false".FromJson(), "(27)");
Lang.Print((bool)"true".FromJson(), "(28)");
Lang.Print(DateTime.Now, "(29)");
```

# GScript (JavaScript) Example

```
using System;
using System.Dynamic;
using JavaScript;
using NUnit.Framework;


Console.WriteLine("Version: {0}", Environment.Version.ToString());

{
    Lang.Print("Sample(A)");
    var engine = Lang.CreateEngine();
    dynamic? result = engine.Evaluate("""
                                 let obj = { '?': 'datetime' };
                                 print(obj);
                                 obj;
                                 """).ToDynamic();
    result.Add("#", 123);
    result.Put("#", 456);
    Lang.Print(Lang.FullName(result));
    Lang.Print(result, "result(1)");
    var keys = result.Keys();
    foreach(var key in keys)
    {
        Lang.Print(key);
        Lang.Print(result.Get(key));
    }
    //Lang.Print(((ExpandoObject)result).Get("?"), "?");
    Lang.Print(result.Get("?"), "?");
    Lang.Print(result.ContainsKey("?"), "?");
    Lang.Print(result.Get("!"), "!");
    Lang.Print(result.ContainsKey("!"), "!");
}

{
    Lang.Print("Sample(B)");
    var engine = Lang.CreateEngine();
    dynamic? result = engine.Evaluate("""
                                 let ary = [11, 22];
                                 print(ary);
                                 ary;
                                 """).ToDynamic();
    Lang.Print(Lang.FullName(result));
    Lang.Print(result);
    Lang.Print(result.Count);
    Lang.Print(result[1]);
}

//System.Environment.Exit(0);

{
    Lang.Print("Sample(01)");
    var engine = Lang.CreateEngine();
    engine.Execute(@"
    print('Hello World to stdout');
    log('Hello World to stderr');
");
}

{
    Lang.Print("Sample(02)");
    dynamic square = Lang.CreateEngine()
        .SetValue("x", 3) // define a new variable
        .Evaluate("x * x") // evaluate a statement
        .ToDynamic(); // converts the value to .NET
    Lang.Print(square, "square");
}

{
    Lang.Print("Sample(03)");
    var engine = Lang.CreateEngine();
    var list = engine.Evaluate(@"
    let list = [];
    for (let i=0; i<3; i++)
    {
      let x = i + 1;
      log(x, 'x');
      list.push(x);
    }
    print(list, 'list(1)');
    list").ToDynamic();
    Lang.Print(Lang.FullName(list));
    Lang.Print(list, "list(2)");
}

{
    Lang.Print("Sample(04)");
    var engine = Lang.CreateEngine();
    var dict = engine.Evaluate(@"
    let dict = {};
    dict['a'] = 11;
    dict['b'] = 22;
    dict['c'] = null;
    print(dict, 'dict(1)');
    dict").ToDynamic();
    Lang.Print(Lang.FullName(dict));
    Lang.Print(dict, "dict(2)");
}

{
    Lang.Print("Sample(05)");
    var p = new Person
    {
        Name = "Mickey Mouse"
    };
    var engine = Lang.CreateEngine()
        .SetValue("p", p)
        .Execute("p.Name = 'Minnie'");
    Assert.AreEqual("Minnie", p.Name);
}

{
    Lang.Print("Sample(06)");
    var engine = Lang.CreateEngine();
    var add = engine
        .Execute("function add(a, b) { return a + b; }")
        .GetValue("add");
    dynamic x = engine.Invoke(add, 1, 2).ToDynamic(); // -> 3
    Lang.Print(x, "x");
}

{
    Lang.Print("Sample(07)");
    var engine = Lang.CreateEngine()
       .Execute("function add(a, b) { return a + b; }");
    dynamic y = engine.Invoke("add", 1, 2).ToDynamic(); // -> 3
    Lang.Print(y, "y");
}

{
    Lang.Print("Sample(08)");
    var engine = Lang.CreateEngine();
    engine.Execute("""
                   var file = new System.IO.StreamWriter('log.txt');
                   file.WriteLine('Hello World !');
                   file.Dispose();
                   """);
}

{
    Lang.Print("Sample(09)");
    var engine = Lang.CreateEngine(typeof(JavaScript.Lang).Assembly);
    engine.Execute("""
                   var JavaScript = importNamespace('JavaScript');
                   var Lang = JavaScript.Lang;
                   Lang.Print(777-1, "Result");
                   """);
}

{
    Lang.Print("Sample(10)");
    var engine = Lang.CreateEngine(typeof(JavaScript.Lang).Assembly);
    engine.SetValue("MyMath", GScript.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(MyMath)));
    engine.Execute("""
                   var JavaScript = importNamespace('JavaScript');
                   var Lang = JavaScript.Lang;
                   var o = new MyMath();
                   Lang.Print(o.Add2(111, 222));
                   """);
}

{
    Lang.Print("Sample(11)");
    var engine = Lang.CreateEngine(typeof(JavaScript.Lang).Assembly);
    engine.Execute("""
                   var JavaScript = importNamespace('JavaScript');
                   var Lang = JavaScript.Lang;
                   var ListOfString = System.Collections.Generic.List(System.String);
                   var list = new ListOfString();
                   list.Add('foo');
                   list.Add(1); // automatically converted to String
                   Lang.Print(list.Count); // 2
                   """);
}

class Person
{
    public string Name;
    public int age;
};

class MyMath
{
    public int Add2(int a, int b)
    {
        return a + b;
    }
}
```

# Dynamic Data Example

```
using System;
using JavaScript;
using NUnit.Framework;

Console.WriteLine("Version: {0}", Environment.Version.ToString());

Lang.Print("(0)");
dynamic ary = Lang.FromJson("[]");
Lang.Print(ary, "(1)");
ary.Add(111);
ary.Add(222);
Lang.Print(ary, "(2)");
Lang.Print(ary.Count, "(3)");
for (int i = 0; i < ary.Count; i++)
{
    Lang.Print(ary[i], "(4)");
}
dynamic obj = Lang.FromJson("{}");
Lang.Print(obj, "(5)");
obj.a = 111;
obj.b = 222;
Lang.Print(obj, "(6)");
foreach(var pair in obj)
{
    Lang.Print(pair, "(7)");
    Lang.Print(pair.Key, "(8)");
    Lang.Print(pair.Value, "(9)");
}
//JS.Print(obj.ContainsKey("a"), "(10)");
//JS.Print(obj.ContainsKey("c"), "(11)");
obj.c = ary;
Lang.Print(obj, "(12)");
Assert.AreEqual(222, (int)obj.c[1]);
var obj2 = obj;// JS.FromNewton(obj);
Lang.Print(obj2, "(13)");
var engine = Lang.CreateEngine();
engine.SetValue("obj2", obj2);
Lang.Print("(14)");
engine.Execute("""
               let ary = obj2.c;
               for (let i=0; i<ary.length; i++)
               {
                 print(ary[i]);
                 ary[i]++;
               }
               """);
Lang.Print(obj2, "(14)");
var ary2 = Lang.FromJson("['a', 'b', null]");
Lang.Print(Lang.FullName(ary2[2]), "(15)");
```
