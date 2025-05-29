using AspNetRunBasic.Models;
using AspNetRunBasic.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _client;
    private const string BasePath = "basket";          

    public BasketService(HttpClient client)
    {
        _client = client;
    }

    public async Task CheckoutBasket(BasketCheckoutModel model)
    {
        var response = await _client.PostAsJsonAsync($"{BasePath}/checkout", model);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new ApplicationException(
                $"Checkout API failed. StatusCode: {(int)response.StatusCode}, Body: {body}");
        }
    }

    public async Task<BasketModel> GetBasket(string userName)
    {
        return await _client.GetFromJsonAsync<BasketModel>($"{BasePath}/{userName}");
    }

    public async Task<BasketModel> UpdateBasket(BasketModel model)
    {
        var response = await _client.PostAsJsonAsync($"{BasePath}", model);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BasketModel>();
    }
}
