using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SKMNET.Exceptions;

namespace SKMScripting
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<SKMScriptOptions>(args)
                .WithParsed((options) => StartExecuteScript(options).Wait()).WithNotParsed((errs) =>
                {
                    Logger.Log(
                        $"Arguments couldn't be parsed: \n\nError:\n{string.Join('\n', errs.Select(err => $"{err.Tag.ToString()}: {err.ToString()}"))}",
                        LogLevel.Error);
                });
        }

        private static async Task StartExecuteScript(SKMScriptOptions options)
        {
            try
            {

                //string extension = Path.GetExtension(options.ScriptPath);
                string fileContent = await File.ReadAllTextAsync(options.ScriptPath);
                //Debug.Assert(extension == "cs", "Extension has to be cs");
                await new SKMScript(options, fileContent).ExecuteAsync();
                Logger.Log("a");
            }
            catch (SKMConnectException connectException)
            {
                Logger.Log($"Could not connect to {options.IpAddress}");
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
            }
        }
    }

    public static class Logger
    {
        public static void Log(string content, LogLevel level = LogLevel.Debug)
        {
            switch (level)
            {
                case LogLevel.Debug:
                {
                    Console.WriteLine($"[D] {content}");
                    break;
                }
                case LogLevel.Info:
                {
                    Console.WriteLine($"[#] {content}");
                    break;
                }
                case LogLevel.Warning:
                {
                    ConsoleColor old = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[!] {content}");
                    Console.ForegroundColor = old;
                    break;
                }
                case LogLevel.Error:
                {
                    ConsoleColor oldBg = Console.BackgroundColor;
                    ConsoleColor oldFg = Console.ForegroundColor;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"[Error] {content}");
                    Console.BackgroundColor = oldBg;
                    Console.ForegroundColor = oldFg;
                    break;
                }
                default: throw new NotImplementedException();
            }
        }

        public static void Log(Exception ex) =>
            Log($"Exception thrown:\n{ex.Message}\nat: {ex.Source}\nStack trace: {ex.StackTrace}", LogLevel.Error);

        public static void LogInfo(string content) => Log(content, LogLevel.Info);
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
}