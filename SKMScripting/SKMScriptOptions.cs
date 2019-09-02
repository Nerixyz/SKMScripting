using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace SKMScripting
{
    public class SKMScriptOptions
    {
        [Option('i', "ip", Default = "127.0.0.1", HelpText = "The ip of the console")]
        public string IpAddress { get; set; }
        
        [Option('s', "script", Required = true, HelpText = "Path to the *.cs(x) file to execute")]
        public string ScriptPath { get; set; }
    }
}