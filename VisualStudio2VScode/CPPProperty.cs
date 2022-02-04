using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace VisualStudio2VScode
{
    [Serializable]
    public partial class CCPPProperty: ICloneable
    {
        // read json from file
        public static CCPPProperty ReadFile(string szFilePath)
        {
            CCPPProperty cppProperty = null;

            try
            {
                // check RD setting file is exist or not, if not return internal setting
                if (File.Exists(szFilePath) == false)
                {
                    Console.WriteLine("File not exist path: {0}", szFilePath);
                    return null;
                }

                // read file
                cppProperty = JsonConvert.DeserializeObject<CCPPProperty>(File.ReadAllText(szFilePath));
            }
            catch (Exception e)
            {
                Console.WriteLine("Read CCPPProperty fail: {0}", e.ToString());
            }

            return cppProperty;
        }

        // write file to json
        public CCPPProperty WriteFile(string szFilePath)
        {
            CCPPProperty cppProperty = null;

            try
            {
                // create folder if it is not exist
                if (System.IO.Directory.Exists(Path.GetDirectoryName(szFilePath)) == false)
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(szFilePath));
                }

                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(szFilePath))
                {
                    // serialize JSON to a string and then write string to a file
                    var serializer = new JsonSerializer();
                    serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    serializer.Serialize(file, this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("write CCPPProperty fail: {0}", e.ToString());
            }

            return cppProperty;
        }

        public CCPPProperty(CCPPProperty org )
        {
            if (org == null)
            {
                version = 0;
                configurations = null;
                return;
            }

            version = org.version;
            configurations = new List<CCPPConfiguration>();

            foreach(CCPPConfiguration config in org.configurations)
            {
                configurations.Add(new CCPPConfiguration(config));
            }
        }

        public object Clone()
        {
            return new CCPPProperty(this);
        }

        public List<CCPPConfiguration> configurations;

        public int version;
    }

    public partial class CCPPConfiguration
    {
        public CCPPConfiguration(CCPPConfiguration org)
        {
            if( org == null)
            {
                name = null;
                includePath = null;
                defines = null;
                intelliSenseMode = "msvc-x86";
                cStandard = "c11";
                cppStandard = "c++11";
                return;
            }

            name = string.Copy(org.name);

            includePath = new List<string>(org.includePath);

            defines = new List<string>(org.defines);

            intelliSenseMode = string.Copy(org.intelliSenseMode);

            cStandard = string.Copy(org.cStandard);

            cppStandard = string.Copy(org.cppStandard);
        }

        // config name
        public string name;

        // include path
        public List<string> includePath;

        // project define
        public List<string> defines;

        // intelliSense Mode
        public string intelliSenseMode;

        // C standard
        public string cStandard;

        // cpp standard
        public string cppStandard;
    }
}
