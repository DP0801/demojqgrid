using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace demojqgrid.Helpers
{
    /// <summary>
    /// HTTP Helper.
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// Sends the request.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="data">The data.</param>
        /// <param name="queryStringParams">The query string parameters.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="headers">The headers.</param>
        /// <returns>
        /// HttpResponse.
        /// </returns>
        /// <exception cref="Exception">Error occurred while communicating with gateway.</exception>
        public static async Task<HttpResponse> SendRequest(string accessToken, HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            HttpClient httpClient = new HttpClient();

            var queryString = BuildQueryString(queryStringParams);
            HttpRequestMessage request = new HttpRequestMessage(verb, endpoint + queryString);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            try
            {
                if (data != null)
                {
                    request.Content = new StringContent(data, Encoding.UTF8, contentType);
                }

                HttpResponseMessage response = await httpClient.SendAsync(request);
                return new HttpResponse
                {
                    StatusCode = response.StatusCode,
                    RequestUrl = response.RequestMessage.RequestUri.ToString(),
                    RawResponse = await response.Content.ReadAsStringAsync(),
                };
            }
            catch (Exception exc)
            {
                throw new Exception("Error occurred while communicating with gateway.", exc);
            }
        }

        /// <summary>Sends the request with basic authentication.</summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="data">The data.</param>
        /// <param name="queryStringParams">The query string parameters.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>HttpResponse.</returns>
        /// <exception cref="Exception">Error occurred while communicating with gateway.</exception>
        public static async Task<HttpResponse> SendRequestWithBasicAuthentication(string accessToken, HttpMethod verb, string endpoint, string data = null, Dictionary<string, string> queryStringParams = null, string contentType = null)
        {
            HttpClient httpClient = new HttpClient();

            var queryString = BuildQueryString(queryStringParams);
            HttpRequestMessage request = new HttpRequestMessage(verb, endpoint + queryString);
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add("Authorization", $"Basic {accessToken}");
            }

            try
            {
                if (data != null)
                {
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await httpClient.SendAsync(request);
                return new HttpResponse
                {
                    StatusCode = response.StatusCode,
                    RequestUrl = response.RequestMessage.RequestUri.ToString(),
                    RawResponse = await response.Content.ReadAsStringAsync(),
                };
            }
            catch (Exception exc)
            {
                throw new Exception("Error occurred while communicating with gateway.", exc);
            }
        }

        /// <summary>Sends the form URL encoded request.</summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="queryStringParams">The query string parameters.</param>
        /// <param name="formBodyParams">The form body parameters.</param>
        /// <returns>HttpResponse.</returns>
        public static async Task<HttpResponse> SendFormUrlEncodedRequest(string endpoint, Dictionary<string, string> queryStringParams, List<KeyValuePair<string, string>> formBodyParams)
        {
            HttpClient httpClient = new HttpClient();
            var queryString = queryStringParams != null && queryStringParams.Count > 0 ? BuildQueryString(queryStringParams) : string.Empty;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint + queryString)
                {
                    Content = new FormUrlEncodedContent(formBodyParams),
                };

                HttpResponseMessage response = await httpClient.SendAsync(request);
                return new HttpResponse
                {
                    StatusCode = response.StatusCode,
                    RequestUrl = response.RequestMessage.RequestUri.ToString(),
                    RawResponse = await response.Content.ReadAsStringAsync(),
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Sends the request with basic authentication.</summary>
        /// <param name="verb">The verb.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="data">The data.</param>
        /// <param name="queryStringParams">The query string parameters.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>SendRequestWithBasicAuthentication.</returns>
        /// <exception cref="Exception">Error occurred while communicating with gateway.</exception>
        public static async Task<HttpResponse> SendRequestWithBasicAuthentication(HttpMethod verb, string endpoint, string username, string password, string data = null, Dictionary<string, string> queryStringParams = null, string contentType = null)
        {
            HttpClient httpClient = new HttpClient();

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{username}:{password}");
            string val = System.Convert.ToBase64String(plainTextBytes);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + val);

            var queryString = BuildQueryString(queryStringParams);
            HttpRequestMessage request = new HttpRequestMessage(verb, endpoint + queryString);

            try
            {
                if (data != null)
                {
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await httpClient.SendAsync(request);
                return new HttpResponse
                {
                    StatusCode = response.StatusCode,
                    RequestUrl = response.RequestMessage.RequestUri.ToString(),
                    RawResponse = await response.Content.ReadAsStringAsync(),
                };
            }
            catch (Exception exc)
            {
                throw new Exception("Error occurred while communicating with gateway.", exc);
            }
        }

        private static string BuildQueryString(Dictionary<string, string> queryStringParams)
        {
            if (queryStringParams == null)
            {
                return string.Empty;
            }

            return string.Format("?{0}", string.Join("&", queryStringParams.Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))));
        }
    }
}