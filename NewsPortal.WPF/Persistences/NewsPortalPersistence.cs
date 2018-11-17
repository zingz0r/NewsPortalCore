using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Data.DTO;
using Newtonsoft.Json.Linq;

namespace NewsPortal.WPF.Persistences
{
    class NewsPortalPersistence : INewsPortalPersistence
    {
        private readonly HttpClient _client;

        public AuthorDTO ActualUser { get; private set; }

        public NewsPortalPersistence(String baseAddress)
        {
            _client = new HttpClient {BaseAddress = new Uri(baseAddress)};
        }

        public async Task<IEnumerable<ArticleDTO>> ReadArticlesAsync()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/articles/");
                if (response.IsSuccessStatusCode)
                {
                    // Microsoft.Extensions.Identity.Stores - has to be installed through nuget
                    IEnumerable<ArticleDTO> articles = await response.Content.ReadAsAsync<IEnumerable<ArticleDTO>>();

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

        public async Task<bool> CreateArticleAsync(ArticleDTO article)
        {
            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync("api/articles/", article); // az értékeket azonnal JSON formátumra alakítjuk
                article.Id = (await response.Content.ReadAsAsync<ArticleDTO>()).Id; // a válaszüzenetben megkapjuk a végleges azonosítót
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<bool> UpdateArticleAsync(ArticleDTO article)
        {
            try
            {
                HttpResponseMessage response = await _client.PutAsJsonAsync("api/articles/", article);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<bool> DeleteArticleAsync(ArticleDTO article)
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

        public async Task<bool> LoginAsync(String userName, String userPassword)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("api/account/login/" + userName + "/" + userPassword);

                if (response.IsSuccessStatusCode)
                {
                    // Microsoft.Extensions.Identity.Stores - has to be installed through nuget
                        ActualUser = await response.Content.ReadAsAsync<AuthorDTO>();
                        return true;
                }
                else
                {
                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
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

                ActualUser = null;
                return !response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new PersistenceUnavailableException(ex);
            }
        }
    }
}
