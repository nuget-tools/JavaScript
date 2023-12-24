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
