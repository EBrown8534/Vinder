using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Stack_Exchange_Voting_Utility.Models
{
    public class ErrorLog
    {
        public ErrorLog() { }

        public ErrorLog(Exception e, HttpRequestBase request)
        {
            Exception = e.ToString();
            StackTrace = e.StackTrace;
            Cookies = string.Join(",", request.Cookies.AllKeys.Select(x => $"\"{x}\":\"{request.Cookies[x]}\""));
            Form = string.Join(",", request.Form.AllKeys.Select(x => $"\"{x}\":\"{request.Form[x]}\""));
            QueryString = string.Join(",", request.QueryString.AllKeys.Select(x => $"\"{x}\":\"{request.QueryString[x]}\""));
            RequestedUrl = request.Url.ToString();
            RawUrl = request.RawUrl;
            Headers = string.Join(",", request.Headers.AllKeys.Select(x => $"\"{x}\":\"{request.Headers[x]}\""));
            HttpMethod = request.HttpMethod;
            ApplicationPath = request.ApplicationPath;
            Encoding = request.ContentEncoding.EncodingName;
            ContentLength = request.ContentLength;
            ContentType = request.ContentType;
            FilePath = request.FilePath;
            IsAuthenticated = request.IsAuthenticated;
            ServerVariables = string.Join(",", request.ServerVariables.AllKeys.Select(x => $"\"{x}\":\"{request.ServerVariables[x]}\""));
            RequestType = request.RequestType;
            PathInfo = request.PathInfo;
            PhysicalPath = request.PhysicalPath;
        }

        public ErrorLog(Exception e, HttpRequest request)
        {
            Exception = e.ToString();
            StackTrace = e.StackTrace;
            Cookies = string.Join(",", request.Cookies.AllKeys.Select(x => $"\"{x}\":\"{request.Cookies[x]}\""));
            Form = string.Join(",", request.Form.AllKeys.Select(x => $"\"{x}\":\"{request.Form[x]}\""));
            QueryString = string.Join(",", request.QueryString.AllKeys.Select(x => $"\"{x}\":\"{request.QueryString[x]}\""));
            RequestedUrl = request.Url.ToString();
            RawUrl = request.RawUrl;
            Headers = string.Join(",", request.Headers.AllKeys.Select(x => $"\"{x}\":\"{request.Headers[x]}\""));
            HttpMethod = request.HttpMethod;
            ApplicationPath = request.ApplicationPath;
            Encoding = request.ContentEncoding.EncodingName;
            ContentLength = request.ContentLength;
            ContentType = request.ContentType;
            FilePath = request.FilePath;
            IsAuthenticated = request.IsAuthenticated;
            ServerVariables = string.Join(",", request.ServerVariables.AllKeys.Select(x => $"\"{x}\":\"{request.ServerVariables[x]}\""));
            RequestType = request.RequestType;
            PathInfo = request.PathInfo;
            PhysicalPath = request.PhysicalPath;
        }

        [Key]
        public int Id { get; set; }

        public int ContentLength { get; set; }
        public string Encoding { get; set; }
        public string ApplicationPath { get; set; }
        public string HttpMethod { get; set; }
        public string Headers { get; set; }
        public string RawUrl { get; set; }
        public string RequestedUrl { get; set; }
        public string QueryString { get; set; }
        public string Form { get; set; }
        public string Cookies { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string ContentType { get; set; }
        public string FilePath { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ServerVariables { get; set; }
        public string RequestType { get; set; }
        public string PathInfo { get; set; }
        public string PhysicalPath { get; set; }
    }
}