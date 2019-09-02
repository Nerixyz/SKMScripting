using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SKMNET.Client;
using SKMNET.Client.Networking.Client.SKMON;
using SKMNET.Util;

namespace SKMScripting
{
    public class SKMScript
    {
        private readonly Script _script;
        private readonly SKMScriptOptions _options;
        private readonly string _scriptContent;

        private CancellationTokenSource _scriptCancellationToken;

        public SKMScript(SKMScriptOptions options, string scriptContent)
        {
            _options = options;
            _scriptContent = scriptContent;

            _script = CSharpScript.Create(scriptContent,
                ScriptOptions.Default.WithFilePath(options.ScriptPath).AddReferences(typeof(LightingConsole).Assembly, typeof(LightingConsoleExtensions).Assembly)
                    .AddImports("SKMNET", "SKMNET.Client", "SKMScripting.LightingConsoleExtensions"), globalsType: typeof(SKMScriptGlobals));
        }

        public async Task ExecuteAsync()
        {
            Logger.Log("Compiling...", LogLevel.Info);
            try
            {
                ImmutableArray<Diagnostic> diagnostics = _script.Compile();

                LogDiagnosticsIfAny(diagnostics);
            }
            catch (CompilationErrorException compilationException)
            {
                Logger.Log("Compilation failed:", LogLevel.Error);
                LogDiagnosticsIfAny(compilationException.Diagnostics, force: true);
                return;
            }

            Logger.Log("Executing.", LogLevel.Info);
            _scriptCancellationToken = new CancellationTokenSource();
            SKMScriptGlobals globals = new SKMScriptGlobals
            {
                console = new LightingConsole(_options.IpAddress,
                    LightingConsole.ConsoleSettings.All(logger: new ScriptLogger())),
                Log = (s, l) => Logger.Log(s.ToString(), l),
                LogInfo = s => Logger.Log(s.ToString(), LogLevel.Info),
                LogDebug = s => Logger.Log(s.ToString()),
                CloseScript = () => _scriptCancellationToken.Cancel(),
            };
            await _script.RunAsync(globals, _scriptCancellationToken.Token);

           await globals.console.QueryAsync(new SKMSync(SKMSync.Action.END));
           globals.console.Dispose();
        }

        private void LogDiagnosticsIfAny(ImmutableArray<Diagnostic> diagnostics, bool force = false)
        {
            if (diagnostics == null) return;
            if(!force && diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
                throw new CompilationErrorException("Compilation failed", diagnostics);
            
            foreach (Diagnostic diagnostic in diagnostics.Where(diagnostic =>
                !diagnostic.IsSuppressed && diagnostic.Severity != DiagnosticSeverity.Hidden))
            {
                Logger.Log($"{diagnostic.Id} @ {diagnostic.Location} ({_scriptContent.Substring(diagnostic.Location.SourceSpan.Start, diagnostic.Location.SourceSpan.Length)}):\n{diagnostic.GetMessage()}",
                    diagnostic.Severity == DiagnosticSeverity.Info ? LogLevel.Info :
                    diagnostic.Severity == DiagnosticSeverity.Warning ? LogLevel.Warning : LogLevel.Error);
            }
        }
    }

    public class ScriptLogger : ILogger
    {
        void ILogger.Log(object toLog) => Logger.Log(toLog.ToString());
    }

    public class SKMScriptGlobals
    {
        public LightingConsole console;
        public Action<object, LogLevel> Log;
        public Action<object> LogDebug;
        public Action<object> LogInfo;
        public Action CloseScript;
    }
}