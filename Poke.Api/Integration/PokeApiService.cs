using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Poke.Api.Model;

namespace Poke.Api.Integration
{
    public interface IPokeApiService
    {
        Task<Pokemon> GetPokeInfo(string name);
    }
    
    public class PokeApiService : IPokeApiService
    {
        private readonly HttpClient _client;
 
        public PokeApiService(HttpClient client)
        {
            _client = client;
        }
 
        public async Task<Pokemon> GetPokeInfo(string name)
        {
            try
            {
                var response = await _client.GetAsync($"pokemon-species/{name}");
                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Pokemon>(stringResponse);
                }
 
                Console.WriteLine($"GetPokeInfo request is not successful: {response.ReasonPhrase}");
                throw new CustomException(response.ReasonPhrase, (int)response.StatusCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}