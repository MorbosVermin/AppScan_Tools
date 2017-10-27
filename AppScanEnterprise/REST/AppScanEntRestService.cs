using Com.WaitWha.AppScanEnterprise.Utils;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Com.WaitWha.AppScanEnterprise.REST
{
    public class AppScanEntRestService
    {
        static ILog Log = LogManager.GetLogger(typeof(AppScanEntRestService));
        static readonly string TOKEN_NAME = "asc_xsrf_token";
        static readonly string DEFAULT_FEATUREKEY = "AppScanServerInternal";

        HttpClient Client { get; set; }
        string Username { get; set; }
        SecureString Password { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseAddress">i.e. https://server.com:9443/ase/api </param>
        /// <param name="userAgent">User Agent to use in the connection.</param>
        public AppScanEntRestService(Uri baseAddress, string username, SecureString password, string userAgent)
        {
            Username = username;
            Password = password;

            Client = new HttpClient()
            {
                BaseAddress = baseAddress
            };

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
        }

        #region Core HTTP Functionality

        /// <summary>
        /// Sends a GET request to the REST API at the requestUri given. If NameValuePairs is given, then this will
        /// appended to the query string for the call. 
        /// </summary>
        /// <param name="requestUri">Request URI</param>
        /// <param name="nameValuePairs">Name Value Pairs (parameter names and values)</param>
        /// <returns></returns>
        async Task<HttpResponseMessage> Get(string requestUri, NameValuePairs nameValuePairs = null)
        {
            if (nameValuePairs != null)
            {
                requestUri += nameValuePairs.ToQueryString(true);
            }

            Uri uri = new Uri(Client.BaseAddress, requestUri);
            Log.Debug(String.Format("Sending HTTP GET request to {0}.", uri));
            HttpResponseMessage response = await Client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            return response;
        }

        /// <summary>
        /// Sends a POST request to the REST API at the requestUri (e.g. auth/login) and containing the JSON given.
        /// </summary>
        /// <param name="requestUri">Request URI</param>
        /// <param name="json">JSON</param>
        /// <returns>Response from the server if the status code is 200</returns>
        async Task<HttpResponseMessage> Post(string requestUri, string json)
        {
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            Uri uri = new Uri(Client.BaseAddress, requestUri);
            Log.Debug(String.Format("Sending HTTP POST request to {0}: {1} bytes (application/json)",
                uri,
                Encoding.UTF8.GetByteCount(json)));

            HttpResponseMessage response = await Client.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            return response;
        }

        /// <summary>
        /// Sends a HTTP DELETE request to the server at the requestUri given.
        /// </summary>
        /// <param name="requestUri">Request URI</param>
        /// <returns>Response from the server if the status code is 200</returns>
        async Task<HttpResponseMessage> Delete(string requestUri)
        {
            Uri uri = new Uri(Client.BaseAddress, requestUri);
            Log.Debug(String.Format("Sending HTTP DELETE request to {0}.", uri));
            HttpResponseMessage response = await Client.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();

            return response;
        }

        /// <summary>
        /// Sends a HTTP PUT request to the server at the requestUri given. This will append the given
        /// name value pairs into the body of the request as JSON.
        /// </summary>
        /// <param name="requestUri">Request URI</param>
        /// <param name="nameValuePairs">Name value pairs which will be sent in the body of the request as JSON.</param>
        /// <returns>Response from the server if the status code is 200</returns>
        async Task<HttpResponseMessage> Put(string requestUri, NameValuePairs nameValuePairs)
        {
            StringContent content = new StringContent(nameValuePairs.ToJson(), Encoding.UTF8, "application/json");
            return await Put(requestUri, content);
        }

        /// <summary>
        /// Sends a HTTP PUT request to the server at the requestUri given. This will append the given
        /// name value pairs into the body of the request as JSON.
        /// </summary>
        /// <param name="requestUri">Request URI</param>
        /// <param name="content">Customized string content to send in the request.</param>
        /// <returns>Response from the server if the status code is 200</returns>
        async Task<HttpResponseMessage> Put(string requestUri, StringContent content)
        {
            Uri uri = new Uri(Client.BaseAddress, requestUri);
            Log.Debug(String.Format("Sending HTTP PUT request to {0}", uri));
            HttpResponseMessage response = await Client.PutAsync(requestUri, content);
            response.EnsureSuccessStatusCode();

            return response;
        }

        #endregion

        Dictionary<string, dynamic> GenericJsonParse(HttpResponseMessage response)
        {
            string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
        }

        /// <summary>
        /// Logs in using ASE credentials provided in the constructor. 
        /// </summary>
        /// <returns>Token for use in further calls to the API.</returns>
        void Login()
        {
            NameValuePairs nv = new NameValuePairs();
            nv.Add("userId", Username);
            nv.Add("password", StringUtils.GetUnsecureString(Password));
            nv.Add("featureKey", DEFAULT_FEATUREKEY);
            
            try
            {
                HttpResponseMessage response = Post("login", nv.ToJson()).GetAwaiter().GetResult();
                Dictionary<string, dynamic> ret = GenericJsonParse(response);
                Client.DefaultRequestHeaders.TryAddWithoutValidation(TOKEN_NAME, ret["sessionId"]);

                Log.Debug(String.Format("Successfully logged into ASE as {1}: {0}", ret["sessionId"], Username));

            }catch(Exception e)
            {
                Log.Error(String.Format("Failed to login to ASE as {0}: {1}", Username, e.Message), e);
                throw e;

            }finally
            {
                nv = null; //clean up values from memory.
            }
            
        }

        /// <summary>
        /// Logs out of REST API
        /// </summary>
        void Logout()
        {
            Get("logout").GetAwaiter();
            Client.DefaultRequestHeaders.Remove(TOKEN_NAME);
            Log.Debug("Successfully logged out of ASE.");
        }

        public async Task<List<Application>> GetApplications()
        {
            Login();
            NameValuePairs nv = new NameValuePairs();
            nv.Add("columns", WebUtility.UrlEncode("name,url"));

            HttpResponseMessage response = await Get("applications", nv);
            string json = await response.Content.ReadAsStringAsync();

            Logout();
            return JsonConvert.DeserializeObject<List<Application>>(json);
        }

        public async Task<List<Scan>> GetScans(Application application)
        {
            Login();
            NameValuePairs nv = new NameValuePairs();
            nv.Add("query", WebUtility.UrlEncode(String.Format("Application Name={0}", application.name)));

            HttpResponseMessage response = await Get("scans", nv);
            string json = await response.Content.ReadAsStringAsync();

            Logout();
            return JsonConvert.DeserializeObject<List<Scan>>(json);
        }

    }
}
