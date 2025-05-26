using AspNetRunBasic.Extensions;
using AspNetRunBasic.Models;

namespace AspNetRunBasic.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _Client;

        public CatalogService(HttpClient client)
        {
            _Client = client;
        }
        

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var response = await _Client.GetAsync("/Catalog");
            return await response.ReadContentAs<List<CatalogModel>>(); 
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            var response = await _Client.GetAsync($"/Catalog/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var response = await _Client.GetAsync($"/GetCatalogByCategory/{category}");
            return await response.ReadContentAs<List<CatalogModel>>();
        }
        public async Task<CatalogModel> CreateCatalog(CatalogModel model)
        {
            var response = await _Client.PostAsJson($"/Catalog",model);
            if(response.IsSuccessStatusCode)
            {
                return await response.ReadContentAs<CatalogModel>();
            }
            else
            {
                throw new Exception("Something went wrong when calling api");
            }
        }
        
    }
}
