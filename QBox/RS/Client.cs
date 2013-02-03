﻿using System;
using System.Net;
using System.IO;
using QBox.Util;

namespace QBox.RS
{
    public class Client
    {
        public virtual void SetAuth(HttpWebRequest request, Stream body) { }
        
        public CallRet Call(string url)
        {
            Console.WriteLine("URL: " + url);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                SetAuth(request, null);
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    return HandleResult(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new CallRet(HttpStatusCode.BadRequest, e);
            }
        }

        public CallRet CallWithBinary(string url, string contentType, Stream body, long length)
        {
            Console.WriteLine("URL: " + url);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = contentType;
                request.ContentLength = length;
                SetAuth(request, body);
                using (Stream requestStream = request.GetRequestStream())
                {
                    StreamUtil.CopyN(body, requestStream, length);
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    return HandleResult(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new CallRet(HttpStatusCode.BadRequest, e);
            }
        }

        public static CallRet HandleResult(HttpWebResponse response)
        {
            HttpStatusCode statusCode = response.StatusCode;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseStr = reader.ReadToEnd();
                return new CallRet(statusCode, responseStr);
            }
        }
    }
}
