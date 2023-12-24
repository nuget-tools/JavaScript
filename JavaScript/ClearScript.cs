using Microsoft.ClearScript.V8;

namespace JavaScript;
public class ClearScript
{
    public static V8ScriptEngine CreateEngine()
    {
        var engine = new V8ScriptEngine();
        engine.AddHostType("Lang", typeof(Lang));
        //engine.AddHostType("Util", typeof(Util));
        engine.Evaluate("""
            var $print = Lang.Print;
            var $log = Lang.log;
            var $toJson = Lang.ToJson;
            """);
        return engine;
    }
}
