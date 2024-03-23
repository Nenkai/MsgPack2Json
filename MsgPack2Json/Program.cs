// See https://aka.ms/new-console-template for more information

using System.Buffers;
using System.Globalization;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json;
using System.Text.Json;


namespace MsgPack2Yml;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Invalid arguments.");
            Console.WriteLine("MsgPack2Json <msg/json files or directory containing files>");
            return;
        }


        if (Directory.Exists(args[0]))
        {
            foreach (var file in Directory.EnumerateFiles(args[0]))
            {
                try
                {
                    ProcessFile(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process '{file}': {ex.Message}");
                }
            }
        }
        else
        {
            foreach (var arg in args)
            {
                try
                {
                    ProcessFile(arg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process '{args[0]}': {ex.Message}");
                }
            }
        }
    }

    private static void ProcessFile(string file)
    {
        string ext = Path.GetExtension(file);
        switch (ext)
        {
            case ".msg":
                {
                    var bin = File.ReadAllBytes(file);
                    string json = MessagePackSerializer.ConvertToJson(bin);
                    var jsonObject = JsonDocument.Parse(json);
                    json = System.Text.Json.JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions() 
                    { 
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    });

                    File.WriteAllText(Path.ChangeExtension(file, ".json"), json);
                    Console.WriteLine($"OK: {file} -> .msg");
                    break;
                }
            case ".json":
                {
                    var json = File.ReadAllText(file);
                    File.WriteAllBytes(Path.ChangeExtension(file, ".msg"), MessagePackSerializer.ConvertFromJson(json));
                    Console.WriteLine($"OK: {file} -> .json");
                    break;
                }
            default:
                Console.WriteLine($"ERROR: {file} not .msg or .json file");
                return;
        }
    }
}