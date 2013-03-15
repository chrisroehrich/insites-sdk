using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Collections.Specialized;

namespace IntelligentInSites.Api.Rest {

    /// <summary>
    /// A struct used to hold the body text, and status code of an HTTP response.
    /// </summary>
    public struct ApiResponse {
        private HttpStatusCode code;
        private string responseData;

        public HttpStatusCode Code {
            get { return code; }
            set { code = value; }
        }

        public string ResponseData {
            get { return responseData; }
            set { responseData = value; }
        }

        public override bool Equals(object obj) {
            return code == ((ApiResponse)obj).Code && responseData == ((ApiResponse)obj).ResponseData;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static bool operator ==(ApiResponse obj1, ApiResponse obj2) {
            if (obj1.Equals(obj2))
                return true;
            else
                return false;
        }

        public static bool operator !=(ApiResponse obj1, ApiResponse obj2) {
            if (obj1.Equals(obj2))
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// A sample HTTP client class using <c>HttpWebRequest</c> and <c>HttpWebResponse</c>. 
    /// This class simplifies HTTP GET and POST requests against the Intelligent InSites API. 
    /// Credentials are submitted using basic access authentication.
    /// </summary>
    public class ApiClient {
        private string authHeaderValue;
        private string hostIP;
        private string protocol;
        private string sessionCookie;
        //The number of seconds before the web request times out.
        private int webTimeout = 60;

        /// <summary>
        /// <c>BasicClient</c> constructor.
        /// </summary>
        /// <param name="host">The hostname of the InSites server.</param>
        /// <param name="username">A username associated with an InSites login.</param>
        /// <param name="password">The password of the specified user.</param>
        public ApiClient(string host, string username, string password, bool useSSL = false) {
            Byte[] byteAuthorizationToken = System.Text.Encoding.ASCII.GetBytes(username + ":" + password);
            this.authHeaderValue =  Convert.ToBase64String(byteAuthorizationToken);
            this.hostIP = host;
            if (useSSL)
                this.protocol = "https://";
            else
                this.protocol = "http://";
            this.sessionCookie = string.Empty;
        }

        /// <summary>
        /// Performs an HTTP GET request.
        /// </summary>
        /// <param name="path">The portion of a resource's URL following the host IP.</param>
        /// <param name="parameters">A <c>Dictionary</c> of parameter names and values.</param>
        /// <returns>The ApiResponse containing the status code, and the body of the HTTP response.</returns>
        public ApiResponse Get(string path, Dictionary<string, object> parameters) {
            string queryString = string.Empty;
            foreach (string param in parameters.Keys) {
                queryString += string.Format("&{0}={1}", HttpUtility.UrlEncode(param), HttpUtility.UrlEncode(parameters[param].ToString()));
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
        /// <returns>The ApiResponse containing the status code, and the body of the HTTP response.</returns>
        public ApiResponse Get(string path, string parameters) {
            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}?{3}", protocol, hostIP, path, parameters);

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
        /// <param name="path">The portion of a resource's URL following the host IP.</param>
        /// <param name="parameters">A <c>Dictionary</c> of parameter names and values.</param>
        /// <returns>The ApiResponse containing the status code, and the body of the HTTP response.</returns>
        public ApiResponse Post(string path, Dictionary<string, object> parameters) {
            string queryString = string.Empty;
            foreach (string param in parameters.Keys) {
                queryString += string.Format("&{0}={1}", HttpUtility.UrlEncode(param), HttpUtility.UrlEncode(parameters[param].ToString()));
            }
            queryString = queryString.TrimStart(new char[] { '&' });

            return Post(path, queryString);
        }

        /// <summary>
        /// Performs an HTTP POST request.
        /// </summary>
        /// <param name="path">The portion of a resource's URL following the host IP.</param>
        /// <param name="parameters">The query string portion of the url.</param>
        /// <returns>The ApiResponse containing the status code, and the body of the HTTP response.</returns>
        public ApiResponse Post(string path, string parameters) {
            string url = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}?", protocol, hostIP, path);

            //Construct the HttpWebRequest
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.Headers.Add("Authorization", "Basic " + authHeaderValue);
            if (this.sessionCookie.Length > 0)
                webRequest.Headers.Add("Cookie", sessionCookie);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            if (parameters.Length > 0) {
                byte[] buffer = Encoding.ASCII.GetBytes(parameters);
                webRequest.ContentLength = buffer.Length;
                Stream requestStream = webRequest.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
            }

            return Execute(webRequest);
        }

        private ApiResponse Execute(HttpWebRequest webRequest) {
            ApiResponse response = new ApiResponse();
            try {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                response = GetResponse(webResponse);
            }
            catch (WebException webException) {
                response = GetResponse((HttpWebResponse)webException.Response);
            }
            return response;
        }

        private ApiResponse GetResponse(HttpWebResponse webResponse) {
            ApiResponse response = new ApiResponse();

            try {
                if (webResponse.GetResponseHeader("Set-Cookie").Length > 0) {
                    this.sessionCookie = webResponse.GetResponseHeader("Set-Cookie");
                }
                response.Code = webResponse.StatusCode;
                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                response.ResponseData = streamReader.ReadToEnd();
                streamReader.Close();
                webResponse.Close();
            }
            catch (WebException exception) {
                response.Code = HttpStatusCode.BadRequest;
                response.ResponseData = exception.Message;
            }
            catch (Exception exception) {
                response.Code = HttpStatusCode.InternalServerError;
                response.ResponseData = exception.Message;
            }
            return response;
        }
    }
}
