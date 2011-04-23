using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EfTidyNet;
using System.Text.RegularExpressions;

namespace AutoDataSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            WebTransformator webTransformator = new WebTransformator();
            webTransformator.RunTransformation();
        }

        private static IDictionary<String, String> ReadPropertuesFile(string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string line in File.ReadAllLines(fileName))
            {
                if (!string.IsNullOrEmpty(line) && !line.StartsWith(";") && !line.StartsWith("#") && !line.StartsWith("'") && line.Contains("="))
                {
                    int index = line.IndexOf("=");
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }
        
        private static string clearTags(string text, string tagName)
        {
            string result = text;
            while (true)
            {
                string beginTag = "<" + tagName;
                string endTag = "</" + tagName + ">";
                int beginIndex = result.IndexOf(beginTag);
                int endIndex = result.IndexOf(endTag) + endTag.Length;
                if (beginIndex != -1)
                {
                    result = result.Remove(beginIndex, endIndex - beginIndex);
                }
                else
                {
                    break;
                }
            }
            return result;
        }
    }
}
