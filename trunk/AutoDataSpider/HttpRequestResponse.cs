using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace AutoDataSpider
{
    class HttpRequestResponse
    {
        private static string FILE_NOT_FOUND_MESSAGE = "Config file not found, please add properly file name in GONFIG_FILE properties.";
        public string CONFIG_FILE { get; set; }
        public int? PAGE { get; set; }
        public string REQUEST_URL { get; set; }

        public string SendRequest()
        {
            string FinalResponse = "";
            string Cookie = "";

            if (string.IsNullOrEmpty(CONFIG_FILE))
            {
                throw new FileNotFoundException(FILE_NOT_FOUND_MESSAGE);
            }

            var ConfigData = Properties.ReadPropertuesFile(CONFIG_FILE);

            HttpWebResponse webresponse;

            string UserName = ConfigData["proxy.UserName"];
            string UserPwd = ConfigData["proxy.UserPwd"];
            string ProxyServer = ConfigData["proxy.ProxyServer"];
            int ProxyPort;
            Int32.TryParse(ConfigData["proxy.ProxyPort"], out ProxyPort);
            string Request = ConfigData["request.Body"];

            HttpBaseClass BaseHttp = new HttpBaseClass(UserName, UserPwd, ProxyServer, ProxyPort, Request);

            try
            {
                string URI = "";
                if (string.IsNullOrEmpty(REQUEST_URL))
                {
                    URI = ConfigData["requrst.URI"];
                }
                else
                {
                    URI = REQUEST_URL;
                }
                string RequestMethod = ConfigData["request.RequestMethod"];
                string Referer = ConfigData["request.Referer"];
                string ContentType = ConfigData["request.ContextType"];
                bool AllowAutoRedirect = Boolean.Parse(ConfigData["request.AllowAutoRedirect"]);

                NameValueCollection collHeader = new NameValueCollection();
                string cookie = GetInitialisationCookies(CONFIG_FILE);
                collHeader.Add("Cookie", cookie);
                HttpWebRequest webrequest = BaseHttp.CreateWebRequest(URI, RequestMethod, Referer, ContentType, AllowAutoRedirect, collHeader);
                if (RequestMethod == "POST")
                {
                    BaseHttp.BuildReqStream(ref webrequest);
                }

                if (!string.IsNullOrEmpty(webrequest.Headers["Cookie"]))
                {
                    Cookie = webrequest.Headers["Cookie"];
                }

                webresponse = (HttpWebResponse)webrequest.GetResponse();

                string ReUri = BaseHttp.GetRedirectURL(ConfigData["request.DomenName"], webresponse, ref Cookie);

                webresponse.Close();
                ReUri = ReUri.Trim();
                if (ReUri.Length == 0) //No redirection URI
                {
                    ReUri = URI;
                }
                ChangePage(ref ReUri, ConfigData["request.pageparam.name"], PAGE);
                RequestMethod = "GET";
                FinalResponse = BaseHttp.GetFinalResponse(ReUri, Cookie, RequestMethod, false, "text/html");
            }
            catch (WebException e)
            {
                throw CatchHttpExceptions(FinalResponse = e.Message);
            }
            catch (System.Exception e)
            {
                throw new Exception(FinalResponse = e.Message);
            }
            finally
            {
                BaseHttp = null;
            }
            return FinalResponse;
        }

        private WebException CatchHttpExceptions(string ErrMsg)
        {
            ErrMsg = "Error During Web Interface. Error is: " + ErrMsg;
            return new WebException(ErrMsg);
        }

        private string GetInitialisationCookies(string FileName)
        {
            if (string.IsNullOrEmpty(FileName)) { return ""; }
            string Cookies = "";
            var config = Properties.ReadPropertuesFile(FileName);
            if (config.ContainsKey("cookie.path") && config.ContainsKey("cookie.ref") && config.ContainsKey("cookie.headname"))
            {
                HttpWebRequest cookreq = (HttpWebRequest)WebRequest.Create(config["cookie.path"]);
                cookreq.Referer = config["cookie.ref"];
                HttpWebResponse resp = (HttpWebResponse)cookreq.GetResponse();
                Cookies = resp.Headers[config["cookie.headname"]];
            }
            return Cookies;
        }

        private void ChangePage(ref string Url, string ParamName, int? PageNumber)
        {
            if (!PageNumber.HasValue) { return; }
            string[] Types = { "?" + ParamName + "=", "&" + ParamName + "=" };
            foreach (var type in Types)
            {
                if (Url.Contains(type))
                {
                    int startIndex = Url.IndexOf(type) + type.Length;
                    int endIndex = Url.IndexOf("&", startIndex);
                    string SubUrl = "";
                    if (endIndex != -1)
                    {
                        SubUrl = Url.Substring(0, startIndex);
                        SubUrl += PageNumber;
                        SubUrl += Url.Substring(endIndex);
                    }
                    else
                    {
                        SubUrl = Url.Substring(0, startIndex);
                        SubUrl += PageNumber;
                    }
                    Url = SubUrl;
                    return;
                }
            }
            Url += "&" + ParamName + "=" + PageNumber;
        }

    }
}
