using System;
using System.IO;
using System.Xml;

namespace AutoDataSpider
{
    class WebTransformator
    {
        private const string DestTempFile = "dest.temp.xml";
        private const string OutputXml = "listings.avto.xml";
        private const string PageProcessorXsl = "PageProcessor.xsl";
        private static int _counter = 3;

        public void RunTransformation()
        {
            try
            {
                var pages = GetPageCount();
                for (int page = 1; page <= pages; page++)
                {
                    Transformator transformator = new Transformator();
                    transformator.TransformToXml("params.properties", PageProcessorXsl, DestTempFile, OutputXml, page, "");

                    var fileName = "cars_auto_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
                    RenameFile("xmls", fileName);
                }
            }
            catch (Exception e)
            {
                if (_counter-- > 0)
                {
                    RunTransformation();
                    Console.WriteLine(e.StackTrace);
                }
                else
                {
                    Console.WriteLine(e.StackTrace);
                    throw;
                }
            }
        }

        private void RenameFile(string folderToStore, string id)
        {
            Directory.CreateDirectory(folderToStore);
            string newName = folderToStore + "/" + id + ".xml";
            if (File.Exists(newName))
            {
                File.Delete(newName);
            }
            File.Copy(OutputXml, newName);
        }


        private int GetPageCount()
        {
            XmlValidator xmlValidator = new XmlValidator();
            var urls = Properties.ReadPropertuesFile("confs.properties");
            xmlValidator.ValidateXMLfromUrl(urls["table.data.page"], "", null, "");
            int result = 0;

            XmlDocument doc = new XmlDocument();

            doc.Load(DestTempFile);
            XmlNamespaceManager nspace = new XmlNamespaceManager(doc.NameTable);
            nspace.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");

            string xpath = "//xhtml:div[@id='MarketGrid']//xhtml:div[@class='pages']//xhtml:div[@class='pages_figure']/xhtml:a[last()-1]";
            XmlNodeList elems = doc.DocumentElement.SelectNodes(xpath, nspace);
            string value = elems.Item(0).InnerText;

            result = RetriveDecimalData(value);

            return result;
        }

        private int RetriveDecimalData(string data)
        {
            var chars = data.ToCharArray();
            string temp = "";
            foreach (var c in chars)
            {
                int res;
                bool result = Int32.TryParse(c.ToString(), out res);
                if (result)
                {
                    temp += res;
                }
            }
            return Int32.Parse(temp);
        }
    }
}
