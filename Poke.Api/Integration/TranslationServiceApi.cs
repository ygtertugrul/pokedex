using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Poke.Api.Model;

namespace Poke.Api.Integration
{
    public interface ITranslationServiceApi
    {
        Task<string> GetYodaTranslation(string description);
        Task<string> GetShakespeareTranslation(string description);
    }
    
    public class TranslationServiceApi : ITranslationServiceApi
    {
        private readonly HttpClient _client;
 
        public TranslationServiceApi(HttpClient client, IMemoryCache memoryCache)
        {
            _client = client;
        }
 
        public async Task<string> GetYodaTranslation(string description)
        {
            try
            {
                var response = await _client.GetAsync($"yoda.json?text={description}");
                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TranslationResponse>(stringResponse).contents.translated;
                }
 
                //If request is not successful, then return null in order to proceed with default Description
                Console.WriteLine($"Yoda Translation request is not successful: {response.ReasonPhrase}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public async Task<string> GetShakespeareTranslation(string description)
        {
            try
            {
                var response = await _client.GetAsync($"shakespeare.json?text={description}");
                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TranslationResponse>(stringResponse).contents.translated;
                }
 
                //If request is not successful, then return null in order to proceed with default Description
                Console.WriteLine($"Shakespeare Translation request is not successful: {response.ReasonPhrase}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}