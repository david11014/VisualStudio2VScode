using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace VisualStudio2VScode
{
    [Serializable]
    [XmlRoot("VisualStudioProject")]
    public partial class CVCProg
    {
        public static CVCProg ReadFile(string szFilePath)
        {
            CVCProg VisualStudioProg = null;

            try
            {
                // check RD setting file is exist or not, if not return internal setting
                if (File.Exists(szFilePath) == false)
                {
                    Console.WriteLine("File not exist path: {0}", szFilePath);
                    return null;
                }

                // create serializer template
                XmlSerializer xs = new XmlSerializer(typeof(CVCProg));

                // read file
                Stream SettingFileStream = new FileStream(szFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                // deserialize XML file
                VisualStudioProg = xs.Deserialize(SettingFileStream) as CVCProg;

                // close file
                SettingFileStream.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("Read CVCProg fail: {0}", e.ToString());
            }

            return VisualStudioProg;
        }


        [XmlAttribute("Name")]
        public string Name;

        [XmlElement("Configurations")]
        public CProjConfigurations Configurations;
    }

    public partial class CProjConfigurations
    {
        [XmlElement("Configuration")]
        public CProjConfiguration[] Configuration;
    }

    public partial class CProjConfiguration
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("InheritedPropertySheets")]
        public string InheritedPropertySheets;

        [XmlElement("Tool")]
        public CTool[] Tools;
    }

    public partial class CTool
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("AdditionalIncludeDirectories")]
        public string AdditionalIncludeDirectories;

        [XmlAttribute("PreprocessorDefinitions")]
        public string PreprocessorDefinitions;
    }
}
