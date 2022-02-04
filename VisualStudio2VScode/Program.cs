using System;

namespace VisualStudio2VScode
{
    class Program
    {
        static void Main(string[] args)
        {
            string szSolutionPath = "";
            string szCppPropertyTemplatePath = @".\Template\\W32\\c_cpp_properties_template.json";
            string szWorkSpaceJsonTemplatePath = @".\Template\\W32\\template.code-workspace";
            string szDefSetting = @".\Template\\W32\\DefSetting.json";

            if ( args.Length > 0 )
            {
                szSolutionPath = args[0];

                for ( int i = 1; i < args.Length; i++)
                {
                    // --CppTmp {cpp properties template file path}
                    if (args[i] == @"--CppTmp" && ( i + 1 ) < args.Length)
                    {
                        szCppPropertyTemplatePath = args[i + 1];
                        i++;
                    }
                    // --workspaceTmp {vscode workspace template file path}
                    else if (args[i] == @"--workspaceTmp" && (i + 1) < args.Length)
                    {
                        szWorkSpaceJsonTemplatePath = args[i + 1];
                        i++;
                    }
                    // --workspaceTmp {vscode workspace template file path}
                    else if (args[i] == @"--DefSetting" && (i + 1) < args.Length)
                    {
                        szDefSetting = args[i + 1];
                        i++;
                    }
                }
            }

            // check solution file is exist or not
            if (System.IO.File.Exists(szSolutionPath) == false)
            {
                Console.WriteLine("Solution file not exist path: {0}", szSolutionPath);
                return;
            }

            Studio2ScodeConverter Converter = new Studio2ScodeConverter(szCppPropertyTemplatePath, szDefSetting);
            Converter.Convert(szSolutionPath, szWorkSpaceJsonTemplatePath);
        }
    }
}
