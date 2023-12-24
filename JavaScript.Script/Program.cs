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
