using Microsoft.Extensions.Configuration;
using MongoDB.Entities;
using SearchService.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SearchService.Services
{
    public class AuctionSvcHttpClient(HttpClient client, IConfiguration config)
    {
        private readonly HttpClient _client = client;
        private readonly IConfiguration _config = config;

        public async Task<List<Item>> GetItemsForSearchDb()
        {
            var lastUpdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(a => a.UpdateAt))
                .Project(x => x.UpdateAt.ToString())
                .ExecuteFirstAsync();

            return await _client.GetFromJsonAsync<List<Item>>(
                _config["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated);
        }
    }
}
