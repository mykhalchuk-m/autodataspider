using System;
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using System.Text;
using EfTidyNet;
using System.Net;
using System.Collections.Generic;

namespace AutoDataSpider
{
    class Transformator
    {
        private static int _counter = 3;

        /// <summary>
        /// transform process
        /// </summary>
        /// <param name="FileName">url to details page</param>
        /// <param name="xsl">path to xsl file</param>
        /// <returns>string of transformed xml</returns>
        public string TransformToXml(string FileName, string xsl, string nativeXml, string transformXml, int? page, string url)
        {
            try
            {
                var validator = new XmlValidator();
                validator.ValidateXMLfromUrl(FileName, nativeXml, page, url);
                string xmlData = RunNativeTransformation(xsl, nativeXml, transformXml);
                return xmlData;
            }
            catch (Exception e)
            {
                if (_counter-- > 0)
                {
                    string xmlData = TransformToXml(FileName, xsl, nativeXml, transformXml, page, url);
                    return xmlData;
                }
                else
                {
                    _counter = 3;
                    return "";
                }
            }
        }

        /// <summary>
        /// transform a xml file into other xml file with needed to us information
        /// </summary>
        /// <param name="xsl">path to xsl file</param>
        private string RunNativeTransformation(string xsl, string nativeXml, string transformXml)
        {
            XmlTextWriter writer = null;
            try
            {
                XslCompiledTransform transform = new XslCompiledTransform();

                transform.Load(xsl);

                XsltArgumentList argList = new XsltArgumentList();
                argList.AddExtensionObject("urn:customFunctions", new CustomFunctions());

                using (writer = new XmlTextWriter(transformXml, Encoding.UTF8))
                {
                    transform.Transform(nativeXml, argList, writer);
                    writer.Close();
                }

                StreamReader reader = File.OpenText(transformXml);

                StringBuilder str = new StringBuilder();
                string temp = "";
                while ((temp = reader.ReadLine()) != null)
                {
                    str.Append(temp);
                }
                reader.Close();

                return str.ToString();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }
    }
}
