using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Collections.Specialized;

namespace IntelligentInSites.Api.Rest
{

    /// <summary>
    /// A struct used to hold the body text, and status code of an HTTP response.
    /// </summary>
    public struct APIResponse
    {
        private HttpStatusCode code;
        private string responseData;

        public HttpStatusCode Code
        {
            get { return code; }
            set { code = value; }
        }

        public string ResponseData
        {
            get { return responseData; }
            set { responseData = value; }
        }

        public override bool Equals(object obj)
        {
            return code == ((APIResponse)obj).Code && responseData == ((APIResponse)obj).ResponseData;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(APIResponse obj1, APIResponse obj2)
        {
            if (obj1.Equals(obj2))
                return true;
            else
                return false;
        }

        public static bool operator !=(APIResponse obj1, APIResponse obj2)
        {
            if (obj1.Equals(obj2))
                return false;
            else
                return true;
        }
    }

    public enum UriScheme
    {
        Http,
        Https
    }

    /// <summary>
    /// A sample HTTP client class using <c>HttpWebRequest</c> and <c>HttpWebResponse</c>. 
    /// This class simplifies HTTP GET and POST requests against the Intelligent InSites API. 
    /// Credentials are submitted using basic access authentication.
    /// </summary>
    public class APIClient
    {
        private string authHeaderValue;
        private string hostIP;
        private string scheme;
        private string sessionCookie;
        //The number of seconds before the web request times out.
        private int webTimeout = 60;
        private readonly string boundary = "BOUNDARY_-BOUNDARY7--_";

        /// <summary>
        /// <c>BasicClient</c> constructor.
        /// </summary>
        /// <param name="host">The hostname of the InSites server.</param>
        /// <param name="username">A username associated with an InSites login.</param>
        /// <param name="password">The password of the specified user.</param>
        public APIClient(UriScheme scheme, string host, string username, string password, int port)
        {
            Byte[] byteAuthorizationToken = System.Text.Encoding.ASCII.GetBytes(username + ":" + password);
            this.authHeaderValue = Convert.ToBase64String(byteAuthorizationToken);
            this.hostIP = host;
            this.scheme = scheme.ToString().ToLower();
            this.sessionCookie = string.Empty;
        }

        /// <summary>
        /// Performs an HTTP GET request.
        /// </summary>
        /// <param name="path">The portion of a resource's URL following the host IP.</param>
        /// <param name="parameters">A <c>Dictionary</c> of parameter names and values.</param>
        /// <returns>The APIResponse containing the status code, and the body of the HTTP response.</returns>
        public APIResponse Get(string path, APIParams parameters)
        {
            string queryString = string.Empty;
            foreach (KeyValuePair<string, object> param in parameters)
            {
                if (param.Value is string)
                {
                    queryString += string.Format("&{0}={1}", HttpUtility.UrlEncode(param.Key), HttpUtility.UrlEncode(param.Value.ToString()));
                }
            }
            queryString = queryString.TrimStart(new char[] { '&' });

            return Get(path, queryString);
        }

        /// <summary>
        /// Performs an HTTP GET request.
        /// </summary>
        /// <param name="path">The portion of a resource's URL following the host IP.</param>
        /// <param name="parameters">The query string portion of the url.</param>
        /// <example>
        /// <code>Get("/api/2.0/rest/entities.xml", "limit=5&filter=current-location+eq+'Bxck'");</code>
        /// </example>
        /// <returns>The APIResponse containing the status code, and the body of the HTTP response.</returns>
        public APIResponse Get(string path, string parameters)
        {
            string url = string.Format(CultureInfo.InvariantCulture, "{0}://{1}{2}?{3}", scheme, hostIP, path, parameters);

            //Construct the HttpWebRequest
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.Headers.Add("Authorization", "Basic " + authHeaderValue);
            if (this.sessionCookie.Length > 0)
                webRequest.Headers.Add("Cookie", sessionCookie);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Timeout = this.webTimeout * 1000;

            return Execute(webRequest);
        }

        /// <summary>
        /// Performs an HTTP POST request.
        /// </summary>
        /// <param name="path">The portion of a resource's URL following the host IP.+</param>
        /// <param name="parameters">A <c>Dictionary</c> of parameter names and values.</param>
        /// <returns>The APIResponse containing the status code, and the body of the HTTP response.</returns>
        public APIResponse Post(string path, APIParams parameters)
        {
            string queryString = string.Empty;
            foreach (KeyValuePair<string, object> param in parameters)
            {
                if (param.Value is string)
                {
                    queryString += string.Format("&{0}={1}", HttpUtility.UrlEncode(param.Key), HttpUtility.UrlEncode(param.Value.ToString()));
                }
            }
            queryString = queryString.TrimStart(new char[] { '&' });

            return Post(path, queryString);
        }

        /// <summary>
        /// Performs an HTTP POST request.
        /// </summary>
        /// <param name="path">The portion of a resource's URL following the host IP.</param>
        /// <param name="parameters">The query string portion of the url.</param>
        /// <returns>The APIResponse containing the status code, and the body of the HTTP response.</returns>
        public APIResponse Post(string path, string parameters)
        {
            string url = string.Format(CultureInfo.InvariantCulture, "{0}://{1}{2}?", scheme, hostIP, path);

            //Construct the HttpWebRequest
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.Headers.Add("Authorization", "Basic " + authHeaderValue);
            if (this.sessionCookie.Length > 0)
                webRequest.Headers.Add("Cookie", sessionCookie);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            if (parameters.Length > 0)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(parameters);
                webRequest.ContentLength = buffer.Length;
                Stream requestStream = webRequest.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
            }

            return Execute(webRequest);
        }

        /// <summary>
        /// Performs an HTTP POST request using the multipart/form-data method.
        /// </summary>
        /// <param name="path">The path portion of a resource URI</param>
        /// <param name="parameters">The parameters to pass with the request</param>
        /// <returns>The APIResponse containing the status code, and the body of the HTTP response.</returns>
        public APIResponse PostMultipart(string path, APIParams parameters)
        {
            Encoding encoding = Encoding.Default;
            Stream dataStream = new MemoryStream();
            foreach (KeyValuePair<string, object> param in parameters)
            {
                if (param.Value is byte[])
                {
                    byte[] fileData = param.Value as byte[];
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{1}\";\r\nContent-Type: application/octet-stream\r\n\r\n", boundary, param.Key);
                    dataStream.Write(encoding.GetBytes(header), 0, header.Length);
                    dataStream.Write(fileData, 0, fileData.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, param.Key, param.Value.ToString());
                    dataStream.Write(encoding.GetBytes(postData), 0, postData.Length);
                }
            }

            // Add the end of the request
            string footer = "\r\n--" + boundary + "--\r\n";
            dataStream.Write(encoding.GetBytes(footer), 0, footer.Length);

            // Dump the Stream into a byte[]
            dataStream.Position = 0;
            byte[] buffer = new byte[dataStream.Length];
            dataStream.Read(buffer, 0, buffer.Length);
            dataStream.Close();

            string url = string.Format(CultureInfo.InvariantCulture, "{0}://{1}{2}", scheme, hostIP, path);

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.Headers.Add("Authorization", "Basic " + authHeaderValue);
            if (this.sessionCookie.Length > 0)
                webRequest.Headers.Add("Cookie", sessionCookie);
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.ContentLength = buffer.Length;
            Stream requestStream = webRequest.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            return Execute(webRequest);
        }

        private APIResponse Execute(HttpWebRequest webRequest)
        {
            APIResponse response = new APIResponse();
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                response = GetResponse(webResponse);
            }
            catch (WebException webException)
            {
                response = GetResponse((HttpWebResponse)webException.Response);
            }
            return response;
        }

        private APIResponse GetResponse(HttpWebResponse webResponse)
        {
            APIResponse response = new APIResponse();

            try
            {
                if (webResponse.GetResponseHeader("Set-Cookie").Length > 0)
                {
                    this.sessionCookie = webResponse.GetResponseHeader("Set-Cookie");
                }
                response.Code = webResponse.StatusCode;
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                response.ResponseData = streamReader.ReadToEnd();
                streamReader.Close();
                webResponse.Close();
            }
            catch (WebException exception)
            {
                response.Code = HttpStatusCode.BadRequest;
                response.ResponseData = exception.Message;
            }
            catch (Exception exception)
            {
                response.Code = HttpStatusCode.InternalServerError;
                response.ResponseData = exception.Message;
            }
            return response;
        }
    }
}
