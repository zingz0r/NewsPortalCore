using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NewsPortal.WPF.Persistences
{
    class NewsPortalPersistence : INewsPortalPersistence
    {
        private readonly HttpClient _client;

        public NewsPortalPersistence(String baseAddress)
        {
            _client = new HttpClient {BaseAddress = new Uri(baseAddress)};
        }

        public async Task<Boolean> LoginAsync(String userName, String userPassword)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/account/login/" + userName + "/" + userPassword);
                var serializedJson = JObject.Parse(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode && serializedJson.ContainsKey("response"))
                {
                    return serializedJson["response"].ToObject<bool>();
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<Boolean> LogoutAsync()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/account/logout");
                return !response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }
    }
}
