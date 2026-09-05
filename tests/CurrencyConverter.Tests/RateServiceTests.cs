using CurrencyConverter.Web.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace CurrencyConverter.Tests;

public class RateServiceTests
{
    private static RateService CreateService(FakeExchangeRateClient fake)
    {
        return new RateService(
            fake,
            new MemoryCache(new MemoryCacheOptions()),
            new ConfigurationBuilder().Build());
    }

    [Fact]
    public async Task Returns_rows_for_a_supported_base_currency()
    {
        var service = CreateService(new FakeExchangeRateClient());

        var model = await service.GetRatesAsync("aud");

        Assert.Equal("AUD", model.BaseCode);
        Assert.Null(model.ErrorMessage);
        Assert.Equal(2, model.Rows.Count);
    }

    [Fact]
    public async Task Rejects_a_base_currency_the_provider_does_not_list()
    {
        var fake = new FakeExchangeRateClient();
        var service = CreateService(fake);

        var model = await service.GetRatesAsync("ZZZ");

        Assert.NotNull(model.ErrorMessage);
        Assert.Empty(model.Rows);
        Assert.Equal(0, fake.LatestCalls); // never reached the outbound call
    }

    [Fact]
    public async Task Second_call_for_the_same_base_is_served_from_cache()
    {
        var fake = new FakeExchangeRateClient();
        var service = CreateService(fake);

        await service.GetRatesAsync("AUD");
        await service.GetRatesAsync("AUD");

        Assert.Equal(1, fake.LatestCalls);
    }

    [Fact]
    public async Task Change_percent_is_measured_from_first_to_last_history_point()
    {
        var service = CreateService(new FakeExchangeRateClient());

        var model = await service.GetRatesAsync("AUD");
        var usd = model.Rows.Single(r => r.Code == "USD");

        // 0.60 -> 0.70 is a 16.67% rise
        Assert.Equal(16.67m, Math.Round(usd.ChangePercent, 2));
    }
}
