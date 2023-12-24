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
