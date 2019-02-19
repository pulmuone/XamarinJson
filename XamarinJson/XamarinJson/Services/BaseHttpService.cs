using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XamarinJson.Helpers;

namespace XamarinJson.Services
{
    public class BaseHttpService
    {
        private static readonly BaseHttpService instance = new BaseHttpService();

        private readonly string CONNECT_URL_AUTH = string.Format(@"http://{0}:{1}{2}", "172.28.200.100", "8080", @"/PostgresqlWebApp/AuthorizationServer");
        private readonly string CONNECT_URL_GET = string.Format(@"http://{0}:{1}{2}", "172.28.200.100", "8080", @"/PostgresqlWebApp/GetServlet");
        private readonly string CONNECT_URL_SET = string.Format(@"http://{0}:{1}{2}", "172.28.200.100", "8080", @"/PostgresqlWebApp/SetServletForJSON");

        private string CONNECT_URL = string.Empty;

        public static BaseHttpService Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task<string> SendRequestAsync(HttpCommand httpCommand, object requestData = null)
        {
            string result = string.Empty;


            // Default to GET
            //var method = httpMethod ?? HttpMethod.Get;
            if (httpCommand.Equals(HttpCommand.GET))
            {
                CONNECT_URL = CONNECT_URL_GET;
            }
            else
            {
                CONNECT_URL = CONNECT_URL_SET;
            }

            // Serialize request data
            var data = requestData == null
                ? null
                : JsonConvert.SerializeObject(requestData);


            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(CONNECT_URL)))
                {
                    request.Headers.Add("jwt", Settings.AuthToken);

                    if (data != null)
                    {
                        request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    }

                    using (var handler = new HttpClientHandler())
                    {
                        using (var client = new HttpClient(handler))
                        {
                            Console.WriteLine(client.Timeout.ToString());
                            //client.Timeout = new TimeSpan(0, 3, 0); //3분
                            //using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                            using (var response = await client.SendAsync(request))
                            {
                                var content = response.Content == null
                                    ? null
                                    : await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode)
                                {
                                    //result = JsonConvert.DeserializeObject<T>(content);
                                    result = content;
                                }
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //var properties = new Dictionary<string, string>
                //{
                //    {"BaseHttpService", "SendRequestAsync" },
                //    {"UserID", Settings.Userid}
                //};
                //Crashes.TrackError(ex, properties);

                return "ERROR " + ex.Message.ToString();
            }
        }


        public async Task<string> AuthorizationAsync(string id, string pw)
        {
            HttpResponseMessage response = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(CONNECT_URL_AUTH.ToString());
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //client.Timeout = new TimeSpan(0, 3, 0); //3분

                    using (var request = new HttpRequestMessage())
                    {
                        request.Method = HttpMethod.Post;
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", id, pw))));

                        response = await client.SendAsync(request);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                return "ERROR " + ex.Message.ToString();
            }
            finally
            {
                if (response != null) response.Dispose();
            }
        }
    }
}
