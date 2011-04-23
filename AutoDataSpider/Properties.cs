using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoDataSpider
{
    class Properties
    {
        private static string defaultDirectory = "props";
        
        public static IDictionary<String, String> ReadPropertuesFile(string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(defaultDirectory + "/" + fileName))
            {
                if (!string.IsNullOrEmpty(line) && !line.StartsWith(";") && !line.StartsWith("#") && !line.StartsWith("'") && line.Contains("="))
                {
                    int index = line.IndexOf("=");
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    //string[] kv = line.Split(new char[] { '=' });

                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }
    }
}
