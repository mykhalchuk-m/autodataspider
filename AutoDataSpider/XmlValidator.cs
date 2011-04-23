using System;
using System.IO;
using System.Text;

namespace AutoDataSpider
{
    class XmlValidator
    {
        private static int _counter = 3;
        private const string TempFile = "inp.temp.xml";
        private const string DestTempFile = "dest.temp.xml";

        /// <summary>
        /// retrive string data from url
        /// </summary>
        /// <param name="page">noumer of page in pagination content</param>
        /// <param name="url">url to page with data</param>
        /// <param name="fileName">name of file with config parameters</param>
        /// <returns>string that contains the text data from url</returns>
        private string RetriveData(string fileName, int? page, string url)
        {
            string str = "";
            try
            {
                var hpp = new HttpRequestResponse {CONFIG_FILE = fileName};
                if (page.HasValue) { hpp.PAGE = page.Value; }
                hpp.REQUEST_URL = url;
                str = hpp.SendRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (--_counter > 0)
                {
                    RetriveData(fileName, page, url);
                }
            }
            return str;
        }

        public void ValidateXMLfromUrl(string FileName, string xmlFileName, int? page, string url)
        {
            string buf = RetriveData(FileName, page, url);
            ValidateXMLfromString(buf, xmlFileName);
        }

        public void ValidateXMLfromString(string data, string xmlFileName)
        {
            try
            {
                data = ClearTags(data, "head");
                data = ClearTags(data, "script");
                data = ClearTags(data, "noscript");
                data = ClearTags(data, "style");

                var writer = new StreamWriter(TempFile, false, Encoding.UTF8);
                writer.WriteLine(data);
                writer.Close();

                var tn = new EfTidyNet.TidyNet();
                tn.Option.InCharEncoding(EfTidyNet.EfTidyOpt.ECharEncodingType.UTF8);
                tn.Option.OutputType(EfTidyNet.EfTidyOpt.EOutputType.XhtmlOut);
                tn.Option.OutCharEncoding(EfTidyNet.EfTidyOpt.ECharEncodingType.UTF8);
                tn.Option.ADDXmlDecl(true);
                tn.Option.AddXmlSpace(false);
                tn.Option.DoctypeMode(EfTidyNet.EfTidyOpt.EDoctypeModes.DoctypeOmit);
                tn.Option.NumericsEntities(true);
                
                xmlFileName = string.IsNullOrEmpty(xmlFileName) ? DestTempFile : xmlFileName;

                tn.TidyFileToFile(TempFile, xmlFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static string ClearTags(string text, string tagName)
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
