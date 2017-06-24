using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace I4.LocalConfig
{
    public class Config
    {

        private IDictionary<string,string> _configDict;
        
        public Config (string fullFileName)
        {
            _configDict = LoadConfig(fullFileName);
        }
        public Config(string basePath,string configFileName)
        { 

            _configDict = LoadConfig(string.Format("{0}\\ConfigFiles\\{1}",basePath, configFileName));
        }
        private IDictionary<string, string> LoadConfig(string configFile)
        {
            if (!File.Exists(configFile))
            {
                throw new LocalConfigException(string.Format("Config file {0} is not exist.", configFile));
            }
            IDictionary<string,string> configDict = new Dictionary<string, string>();
            try
            {
                XDocument xDoc = XDocument.Load(configFile);
                XElement xElement = xDoc.Element("Configs");                
                foreach (XElement item in xElement.Elements())
                {
                    string key = item.Attribute("key").Value;
                    string value = item.Attribute("value").Value;
                    if (!configDict.ContainsKey(key))
                        configDict.Add(key, value);
                }
            }
            catch (Exception ex)
            {

                throw new LocalConfigException(ex.Message, ex);
            }
            return configDict;
        }
        public string GetKeyValue(string key)
        {
            if(!_configDict.ContainsKey(key))
            {
                throw new LocalConfigException(string.Format("Key {0} is not exist.", key));
            }
            string rtn  = _configDict[key];
            return rtn;
        }
    }
}
