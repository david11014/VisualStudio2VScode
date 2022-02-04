using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace VisualStudio2VScode
{
    public partial class Studio2ScodeConverter
    {
        // constructor
        public Studio2ScodeConverter(string szCppPropertyTemplatePath, string szDefSetting)
        {
            m_CppPropertyTemplate = CCPPProperty.ReadFile(szCppPropertyTemplatePath);

            m_DefineSetting = DefineSetting.ReadFile(szDefSetting);
        }

        // convert and create VS code workspace envirement
        public void Convert( string szSlnPath, string szWorkSpaceJsonTemplatePath)
        {
            try
            {
                szSlnPath = System.IO.Path.GetFullPath(szSlnPath);

                // solution folder
                m_szSlnFolder = System.IO.Path.GetDirectoryName(szSlnPath) + @"\";

                // default project define
                m_DefDefine = new List<string>();

                // add custom define
                AddCustomDefine();

                // get project path
                List<string> ProjList = null;
                AnalysSolution(szSlnPath, out ProjList);

                // workspace
                JObject WorkSpaceJson = JObject.Parse(File.ReadAllText(szWorkSpaceJsonTemplatePath));

                // folders of project
                JArray WorkspaceRootFolders = null;
                JToken folders = null;

                if( WorkSpaceJson.TryGetValue("folders", out folders) == true )
                {
                    WorkspaceRootFolders = (JArray)folders;
                }
                else
                {
                    WorkSpaceJson.Add("folders", new JArray());
                    WorkspaceRootFolders = (JArray)WorkSpaceJson.GetValue("folders");
                }

                // add root folder
                JObject RootFolderPath = new JObject();
                RootFolderPath.Add("name", Path.GetFileNameWithoutExtension(szSlnPath) + @" root folder");
                RootFolderPath.Add("path", @".");
                WorkspaceRootFolders.Add(RootFolderPath);

                foreach (string ProjPath in ProjList)
                {
                    // create cpp property file
                    CreateCppProperty(ProjPath);

                    // gupdate workspace root folder
                    JObject FolderPath = new JObject();
                    FolderPath.Add("path", System.IO.Path.GetDirectoryName(ProjPath));
                    WorkspaceRootFolders.Add(FolderPath);
                }

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(m_szSlnFolder + @"\..\" + Path.GetFileNameWithoutExtension(szSlnPath) + @".code-workspace"))
                {
                    // serialize JSON to a string and then write string to a file
                    var serializer = new JsonSerializer();
                    serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    serializer.Serialize(file, WorkSpaceJson);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Convert fail: {0}", e.ToString());
            }
        }
    }

    public partial class Studio2ScodeConverter
    {
        // cpp property template
        private CCPPProperty m_CppPropertyTemplate;

        // define setting
        private DefineSetting m_DefineSetting;

        // solution folder
        private string m_szSlnFolder;

        List<string> m_DefDefine;

        // analys sln file
        private void AnalysSolution(string szSlnPath, out List<string> ProjList)
        {
            // read solution file
            string szSlnContent = File.ReadAllText(szSlnPath);

            // use regex search project list
            string ProjectPathPattern = @"Project\(""{[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}}""\) = ""(\w+)"", ""([\w\\\.]+)"", ""{[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}}""";
            Regex ProjPathRegx = new Regex(ProjectPathPattern);
            Match ProjPathMatch = ProjPathRegx.Match(szSlnContent);

            // project path string builder
            StringBuilder ProjectPathBuilder = new StringBuilder();

            // get project path
            ProjList = new List<string>();
            while (ProjPathMatch.Success)
            {
                ProjectPathBuilder.Clear();
                ProjectPathBuilder.AppendFormat(@"{0}\{1}", Path.GetDirectoryName(szSlnPath), ProjPathMatch.Groups[2].ToString());
                ProjList.Add(ProjectPathBuilder.ToString());

                ProjPathMatch = ProjPathMatch.NextMatch();
            }
        }

        // create cpp property file
        private void CreateCppProperty(string szProjPath)
        {
            string zPattern = @"\$\(\w+\)";
            Regex rx = new Regex(zPattern);
            MatchEvaluator CostunEvaluator = new MatchEvaluator(CustomValueReplace);
            MatchEvaluator DefaultEvaluator = new MatchEvaluator(DefReplace);

            // generate property object from teplate
            CCPPProperty CppProperty = (CCPPProperty)m_CppPropertyTemplate.Clone();
            CppProperty.configurations.Clear();

            // get config template
            CCPPConfiguration ConfigTemplate = new CCPPConfiguration(m_CppPropertyTemplate.configurations[0]);

            // read proj file
            CVCProg VisualStudioProg = CVCProg.ReadFile(szProjPath);

            // convert vs proj file to cpp property file
            try
            {
                Console.WriteLine("convert proj {0}", VisualStudioProg.Name);

                foreach (CProjConfiguration Config in VisualStudioProg.Configurations.Configuration)
                {
                    Console.WriteLine("  convert config {0}", Config.Name);
                    foreach (CTool tool in Config.Tools)
                    {
                        if (tool.Name == "VCCLCompilerTool")
                        {
                            CCPPConfiguration CppConfig = new CCPPConfiguration(ConfigTemplate);
                            CppConfig.name = Config.Name;

                            // convert include dir
                            if (tool.AdditionalIncludeDirectories != null)
                            {
                                string[] ProjIncludeDir = SeprateValue(tool.AdditionalIncludeDirectories);

                                for (int i = 0; i < ProjIncludeDir.Length; i++)
                                {
                                    ProjIncludeDir[i] = Regex.Replace(ProjIncludeDir[i], zPattern, CostunEvaluator);
                                    ProjIncludeDir[i] = Regex.Replace(ProjIncludeDir[i], zPattern, DefaultEvaluator);

                                    if ( Path.IsPathRooted( ProjIncludeDir[i] ) == false )
                                    {
                                        ProjIncludeDir[i] = Path.GetDirectoryName(szProjPath) + @"/" + ProjIncludeDir[i];
                                    }

                                    ProjIncludeDir[i] = Path.GetFullPath(ProjIncludeDir[i]);

                                    ProjIncludeDir[i] = ProjIncludeDir[i].Replace(@"\", @"/");
                                }

                                CppConfig.includePath.InsertRange(0, ProjIncludeDir);
                            }

                            // convert project preprocesser def
                            if (tool.PreprocessorDefinitions != null)
                            {
                                string[] ProjPreprocesserDef = SeprateValue(tool.PreprocessorDefinitions);

                                for (int i = 0; i < ProjPreprocesserDef.Length; i++)
                                {
                                    ProjPreprocesserDef[i] = Regex.Replace(ProjPreprocesserDef[i], zPattern, CostunEvaluator);
                                    ProjPreprocesserDef[i] = Regex.Replace(ProjPreprocesserDef[i], zPattern, DefaultEvaluator);
                                }

                                if(m_DefDefine != null)
                                {
                                    CppConfig.defines.InsertRange(0, m_DefDefine);
                                }

                                CppConfig.defines.InsertRange(0, ProjPreprocesserDef);
                            }

                            // add config
                            CppProperty.configurations.Add(CppConfig);

                            // the VCCLCompilerTool only has one in each config
                            break;
                        }
                    }
                }

                
                string szFilPath = Path.GetDirectoryName(szProjPath) + @"\.vscode\c_cpp_properties.json";
                Console.WriteLine("  Write cpp property file {0}", szFilPath);
                CppProperty.WriteFile(szFilPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("CreateCppProperty fail: {0}", e.ToString());
            }
        }

        // clean and seprate vlaue
        private string[] SeprateValue( string OrgValue )
        {
            if(OrgValue == null || OrgValue == "")
            {
                return new string[0];
            }

            char[] delimiters = { ',', ';' };

            // remove '"'
            string szCleanString =  Regex.Replace(OrgValue, "\"", "");

            // split string by ';'
            return szCleanString.Split(delimiters);
        }

        // add the costum define from setting
        private void AddCustomDefine()
        {
            if( m_DefineSetting == null||m_DefDefine==null)
            {
                return;
            }

            if(m_DefineSetting.Defines == null)
            {
                return;
            }

            foreach( string def in m_DefineSetting.Defines)
            {
                m_DefDefine.Add(string.Copy(def));
            }
        }

        // replace $() define
        private string CustomValueReplace(Match match)
        {
            if(m_DefineSetting == null)
            {
                return match.Value;
            }

            if (m_DefineSetting.ProgValueMap == null){
                return match.Value;
            }
            JToken value;
            if( m_DefineSetting.ProgValueMap.TryGetValue(match.Value, out value) == false)
            {
                return match.Value;
            }

            return (string)value;
        }

        // replace $() define
        private string DefReplace(Match match)
        {
            switch (match.Value)
            {
                case "$(SolutionDir)":
                    return m_szSlnFolder;
            }

            return match.Value;
        }
    }
}
