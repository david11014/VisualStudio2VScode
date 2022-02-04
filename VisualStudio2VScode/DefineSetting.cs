using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace VisualStudio2VScode
{
    [Serializable]
    class DefineSetting
    {
        // read json from file
        public static DefineSetting ReadFile(string szFilePath)
        {
            DefineSetting setting = null;

            try
            {
                // check RD setting file is exist or not, if not return internal setting
                if (File.Exists(szFilePath) == false)
                {
                    Console.WriteLine("File not exist path: {0}", szFilePath);
                    return null;
                }

                // read file
                setting = JsonConvert.DeserializeObject<DefineSetting>(File.ReadAllText(szFilePath));
            }
            catch (Exception e)
            {
                Console.WriteLine("Read CCPPProperty fail: {0}", e.ToString());
            }

            return setting;
        }

        public JObject ProgValueMap;

        public List<string> Defines;
    }
}
