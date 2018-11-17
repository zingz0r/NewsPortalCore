using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NewsPortal.Data.Entity;
using Newtonsoft.Json.Linq;

namespace NewsPortal.WPF.Persistences
{
    class NewsPortalPersistence : INewsPortalPersistence
    {
        private readonly HttpClient _client;

        public NewsPortalPersistence(String baseAddress)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }
        
        public async Task<IEnumerable<Article>> ReadArticlesAsync()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/articles/");
                if (response.IsSuccessStatusCode)
                {
                    // Microsoft.Extensions.Identity.Stores - has to be installed through nuget
                    IEnumerable<Article> articles = await response.Content.ReadAsAsync<IEnumerable<Article>>();

                    return articles;
                }
                else
                {
                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);

            }
        }

        public async Task<bool> CreateArticleAsync(Article article)
        {
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/articles/", article); 
                article.Id = (await response.Content.ReadAsAsync<Article>()).Id; 
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<bool> UpdateArticleAsync(Article article)
        {
            try
            {
                HttpResponseMessage response = await _client.PutAsJsonAsync("api/articles/" + article.Id, article);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<bool> DeleteArticleAsync(Article article)
        {
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync("api/articles/" + article.Id);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<User> GetLoggedInUserInfo()
        {
            HttpResponseMessage response = await _client.GetAsync("api/account/currentuser");
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<Boolean> LoginAsync(String userName, String userPassword)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/account/login/" + userName + "/" + userPassword);
                var serializedJson = JObject.Parse(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode && serializedJson.ContainsKey("response"))
                {
                    bool isSuccessfulyLoggedIn = serializedJson["response"].ToObject<bool>();

                    return isSuccessfulyLoggedIn;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<bool> LogoutAsync()
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
