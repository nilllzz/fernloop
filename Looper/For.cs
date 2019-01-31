using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace Looper
{
    public static class F0r
    {
        public const int FOR_LOOP_STATE = 0;
        public const string EXE_PATH = "";

        public static void Loop(int start, int end, int step, Action<int> body)
        {
            // execute for:
            int state = FOR_LOOP_STATE + start;

            if (state < end)
            {
                body(state);

                // get the file that is this:
                string newPath;
                string path = "";
                if (EXE_PATH == "")
                {
                    string stack = Environment.StackTrace;
                    string[] stackItems = stack.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    string item = stackItems[2];
                    path = item.Remove(0, item.IndexOf(" in ") + 4);
                    path = path.Remove(path.LastIndexOf(":"));
                    newPath = path + ".temp";
                }
                else
                {
                    path = EXE_PATH + ".temp";
                    newPath = path;
                }

                int newState = state + step;
                File.WriteAllText(newPath, File.ReadAllText(path));
                string content = File.ReadAllText(newPath);
                content = content.Replace("FOR_LOOP_STATE" + " = " + FOR_LOOP_STATE.ToString(),
                    "FOR_LOOP_STATE" + " = " + newState.ToString());
                content = content.Replace("EXE_PATH = \"\"",
                    "EXE_PATH = @\"" + path + "\"");

                File.WriteAllText(newPath, content);

                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters()
                {
                    GenerateInMemory = false,
                    OutputAssembly = Assembly.GetExecutingAssembly().Location + "1",
                    GenerateExecutable = false
                };
                parameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Microsoft.CSharp.dll");
                parameters.ReferencedAssemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll");
                var results = provider.CompileAssemblyFromFile(parameters, new[] { newPath });

                Assembly asm = Assembly.Load(File.ReadAllBytes(Assembly.GetExecutingAssembly().Location + "1"));
                File.Delete(Assembly.GetExecutingAssembly().Location + "1");

                var t = asm.GetType("Looper.F0r");
                var m = t.GetMethod("Loop");
                m.Invoke(null, new object[] { start, end, step, body });

                if (EXE_PATH == "")
                {
                    File.Delete(newPath);
                }
            }
        }
    }
}
