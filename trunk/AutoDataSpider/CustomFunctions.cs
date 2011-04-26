using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;

namespace AutoDataSpider
{
    class CustomFunctions
    {
        private static bool continueTransformation = true;

        public bool TransformationShouldBeStopped()
        {
            return !continueTransformation;
        }

        public void StartTransformation()
        {
            continueTransformation = true;
        }

        private static string ID = "";
        private static string DetailsUrl = "";

        /// <summary>
        /// make xsl transformation data from url to local xml
        /// </summary>
        /// <param name="url">url to data source</param>
        /// <param name="xsl">path to xsl-file</param>
        /// <param name="nativeXml">temporary xml-file, contains native data from url</param>
        /// <param name="trasformXml">temporary xml-file, contains xsl transformed data</param>
        /// <returns>string that contains text data from trasformXml file</returns>
        public string ApplyTransformationForWebRequestResult(String url, String xsl, String nativeXml, String trasformXml)
        {
            string res = "";
            DetailsUrl = url;
            Console.WriteLine(url);
            Transformator transformator = new Transformator();
            res = transformator.TransformToXml("details.properties", xsl, nativeXml, trasformXml, null, url);
            return res;
        }

        public string ParseBaseInformation(string name, string value)
        {
            string result = "";
            if (name.Contains("Год выпуска"))
            {
                result = "<year>" + value.Trim() + "</year>";
            }
            else if (name.Contains("Пробег"))
            {
                result = "<running>" + value.Trim() + "</running>";
            }
            else if (name.Contains("Тип кузова"))
            {
                result = "<carcass>" + value.Trim() + "</carcass>";
            }
            else if (name.Contains("Цвет"))
            {
                result = "<color>" + value.Trim() + "</color>";
            }
            else if (name.Contains("Тип двигателя"))
            {
                result = "<motor.type>" + value.Trim() + "</motor.type>";
            }
            else if (name.Contains("Объём"))
            {
                result = "<motor.volume>" + value.Trim() + "</motor.volume>";
            }
            else if (name.Contains("Таможня"))
            {
                result = "<customs>" + value.Trim() + "</customs>";
            }
            else if (name.Contains("Дата публикации"))
            {
                result = "<public.date>" + value.Trim() + "</public.date>";
            }
            else if (name.Contains("Модификация"))
            {
                result = "<modification>" + value.Trim() + "</modification>";
            }
            else if (name.Contains("КПП"))
            {
                result = "<gearbox>" + value.Trim() + "</gearbox>";
            }
            else if (name.Contains("Привод"))
            {
                result = "<transmission>" + value.Trim() + "</transmission>";
            }
            else if (name.Contains("Руль"))
            {
                result = "<rudder>" + value.Trim() + "</rudder>";
            }
            else if (name.Contains("Состояние"))
            {
                result = "<state>" + value.Trim() + "</state>";
            }
            return result;
        }

        public bool IsColor(string name)
        {
            if (name.Contains("Цвет"))
            {
                return true;
            }
            return false;
        }

        public string GetCompletionData(string title, string data)
        {
            StringBuilder result = new StringBuilder();
            switch (title.Trim())
            {
                case "Безопасность":
                    {
                        MakeTag("security", data);
                        break;
                    }
                case "Комфорт":
                    {
                        MakeTag("comfort", data);
                        break;
                    }
                case "Противоугонное":
                    {
                        MakeTag("immobilizer", data);
                        break;
                    }
                case "Музыка":
                    {
                        MakeTag("music", data);
                        break;
                    }
                case "Электропривод":
                    {
                        MakeTag("electric", data);
                        break;
                    }
                case "Другие опции и оборудование":
                    {
                        MakeTag("other", data);
                        break;
                    }
            }
            return result.ToString();
        }

        private string MakeTag(string TagName, string Data)
        {
            StringBuilder result = new StringBuilder();
            if (!Data.Equals("не заполнена"))
            {
                result.Append("<" + TagName + ">");
                result.Append(SplitCorrectData(Data));
                result.Append("</" + TagName + ">");
            }
            return result.ToString();
        }

        private string SplitCorrectData(string Data)
        {
            StringBuilder result = new StringBuilder();
            string[] DataMas = Data.Split(new char[] { '\n' });
            for (int i = 0; i < DataMas.Length; i++)
            {
                result.Append(DataMas[i]);
                if (!(i == DataMas.Length - 1))
                {
                    result.Append(", ");
                }
            }
            return result.ToString();
        }

        public string GetID()
        {
            string[] SepBySlash = DetailsUrl.Split(new char[] { '/' });
            string[] SepByTere = SepBySlash.Last().Split(new char[] { '-' });
            string id = SepByTere[1];
            ID = id;
            return id;
        }

        private string GetPhotoName(string PhotoUrl)
        {
            string[] SepBySlash = PhotoUrl.Split(new char[] { '/' });
            int PointIndex = SepBySlash.Last().IndexOf(".");
            string Name = SepBySlash.Last().Substring(0, PointIndex);
            return Name;
        }

        public string DownloadPhotos(string PhotoUri)
        {
            string PhotoUrl = GetPhotoURL(PhotoUri);
            Downloader.DownloadPhotos(PhotoUrl, ID, GetPhotoName(PhotoUrl));
            return PhotoUrl + " ";
        }

        private string GetPhotoURL(string URL)
        {
            int PointIndex = URL.LastIndexOf("_");
            string FirstPart = URL.Substring(0, PointIndex);
            string PhotoUrl = "http://cars.avto.ru" + FirstPart + "_h.jpg";
            return PhotoUrl;
        }
        public string ParsePersonalInfo(string Capture, string Value)
        {
            string result = "";
            Capture = Capture.Trim();
            if (Capture.Contains("Страна"))
            {
                result = MakeTag("country", Value);
            }
            else if (Capture.Contains("Город"))
            {
                result = MakeTag("city", Value);
            }
            else if (Capture.Contains("Контактное"))
            {
                result = MakeTag("person", Value);
            }
            else if (Capture.Contains("Телефон"))
            {
                result = MakeTag("phone", Value);
            }
            else if (Capture.Contains("mail"))
            {
                result = MakeTag("email", Value);
            }
            return result;
        }

        public string ParseAvtoData()
        {
            string[] components = DetailsUrl.Split(new char[] {'/'});
            string result = MakeTag("marka", components[3]);
            result += MakeTag("model", components[4]);
            return result;
        }
    }
}
