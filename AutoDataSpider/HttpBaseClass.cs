﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace AutoDataSpider
{
    /// <summary>
    ///This base class provides implementation of request 
    ///and response methods during Http Calls.
    /// </summary>

    public class HttpBaseClass
    {

        private string UserName;
        private string UserPwd;
        private string ProxyServer;
        private int ProxyPort;
        private string Request;

        public HttpBaseClass(string HttpUserName, string HttpUserPwd, string HttpProxyServer, int HttpProxyPort, string HttpRequest)
        {
            UserName = HttpUserName;
            UserPwd = HttpUserPwd;
            ProxyServer = HttpProxyServer;
            ProxyPort = HttpProxyPort;
            Request = HttpRequest;
        }

        /// <summary>
        /// This method creates secure/non secure web
        /// request based on the parameters passed.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="collHeader">This parameter of type
        ///    NameValueCollection may contain any extra header
        ///    elements to be included in this request      </param>
        /// <param name="RequestMethod">Value can POST OR GET</param>
        /// <param name="NwCred">In case of secure request this would be true</param>
        /// <returns></returns>

        public virtual HttpWebRequest CreateWebRequest(string uri, string RequestMethod, string Referer, string ContentType, bool AllowAutoRedirect, NameValueCollection collHeader)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.KeepAlive = false;
            if (!string.IsNullOrEmpty(Referer))
                webrequest.Referer = Referer;
            webrequest.Method = RequestMethod;

            if (collHeader != null)
            {
                int iCount = collHeader.Count;
                string key;
                string keyvalue;

                for (int i = 0; i < iCount; i++)
                {
                    key = collHeader.Keys[i];
                    keyvalue = collHeader[i];
                    webrequest.Headers.Add(key, keyvalue);
                }
                //Remove collection elements
                collHeader.Clear();
            }
            webrequest.ContentType = ContentType;
            
            if (!string.IsNullOrEmpty(ProxyServer))
            {
                webrequest.Proxy = new WebProxy(ProxyServer, ProxyPort);
            }
            webrequest.AllowAutoRedirect = AllowAutoRedirect;
            return webrequest;
        }

        /// <summary>
        /// This method retreives redirected URL from
        /// response header and also passes back
        /// any cookie (if there is any)
        /// </summary>
        /// <param name="webresponse"></param>
        /// <param name="Cookie"></param>
        /// <returns></returns>

        public virtual string GetRedirectURL(string host, HttpWebResponse webresponse, ref string Cookie)
        {
            string uri = "";

            WebHeaderCollection headers = webresponse.Headers;

            if ((webresponse.StatusCode == HttpStatusCode.Found) ||
              (webresponse.StatusCode == HttpStatusCode.Redirect) ||
              (webresponse.StatusCode == HttpStatusCode.Moved) ||
              (webresponse.StatusCode == HttpStatusCode.MovedPermanently))
            {
                // Get redirected uri
                uri = headers["Location"];
                uri = host + uri.Trim();
            }
            if (headers["Set-Cookie"] != null)
            {
                Cookie += headers["Set-Cookie"];
            }
            string StartURI = "http://";
            if (uri.Length > 0 && uri.StartsWith(StartURI) == false)
            {
                uri = StartURI + uri;
            }

            return uri;
        }

        public virtual string GetFinalResponse(string ReUri, string Cookie, string RequestMethod, bool NwCred, string ContrentType)
        {
            NameValueCollection collHeader = new NameValueCollection();
            if (Cookie.Length > 0)
            {
                collHeader.Add("Cookie", Cookie);
            }
            HttpWebRequest webrequest = CreateWebRequest(ReUri, RequestMethod, @"http://cars.avto.ru/search/", ContrentType, true, collHeader);
            //BuildReqStream(ref webrequest);
            HttpWebResponse webresponse;
            webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = System.Text.Encoding.GetEncoding(1251);
            StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string Response = loResponseStream.ReadToEnd();
            loResponseStream.Close();
            webresponse.Close();
            return Response;
        }

        public void BuildReqStream(ref HttpWebRequest webrequest)
        //This method build the request stream for WebRequest
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Request);
            webrequest.ContentLength = bytes.Length;
            Stream oStreamOut = webrequest.GetRequestStream();
            oStreamOut.Write(bytes, 0, bytes.Length);
            oStreamOut.Close();
        }
    }
}
