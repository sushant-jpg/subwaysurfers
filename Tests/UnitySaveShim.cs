// Only substitutes engine JSON / disk location in the headless domain checks.
// This is not a substitute for compiling the game in Unity.
namespace UnityEngine {
 public static class Application { public static string persistentDataPath=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"lumen-checks-"+System.Guid.NewGuid()); }
 public static class Debug { public static void LogWarning(object value)=>System.Console.WriteLine(value); }
 public static class JsonUtility {
  static readonly System.Text.Json.JsonSerializerOptions Options=new(){IncludeFields=true,WriteIndented=true};
  public static string ToJson<T>(T value,bool pretty)=>System.Text.Json.JsonSerializer.Serialize(value,Options);
  public static T FromJson<T>(string json)=>System.Text.Json.JsonSerializer.Deserialize<T>(json,Options);
 }
}
