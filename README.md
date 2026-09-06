# Currency Converter

A small ASP.NET Core MVC web app that shows what one unit of a chosen base currency buys in every other supported currency,
with a 30-day trend for each. Rates come from the European Central Bank via the free Frankfurter API.
Built as a development prototype for my individual project.

## Prerequisites

- .NET 8 SDK. Check with `dotnet --list-sdks`; you need an `8.0.x` line.
- git

## Run it

```bash
git clone https://github.com/alexyun0429/CurrencyConverter.git
cd CurrencyConverter
dotnet run --project src/CurrencyConverter.Web
```

Open the `http://localhost:xxxx` address printed in the terminal. Pick a base currency from
the dropdown; the page reloads with rates against that base.

## Run the tests

```bash
dotnet test
```

Six tests, no network access needed. They take well under a second.

## JSON endpoint

The same data is available as JSON for other systems to consume:

```bash
curl http://localhost:xxxx/api/rates/AUD
```

Replace the port with the one your app printed. An unknown currency code returns HTTP 400
with an error message.

## Project layout

```
src/CurrencyConverter.Web/     the web app (Controllers, Models, Services, Views)
tests/CurrencyConverter.Tests/ xUnit tests for the service layer
global.json                    pins the .NET SDK to 8.0.3xx
NuGet.config                   uses nuget.org only
```

## Configuration

`src/CurrencyConverter.Web/appsettings.json`:

| Key                     | Default                           | Meaning                                     |
| ----------------------- | --------------------------------- | ------------------------------------------- |
| `Frankfurter:BaseUrl`   | `https://api.frankfurter.dev/v1/` | Rate source                                 |
| `Cache:RatesMinutes`    | `10`                              | How long a base currency's rates are cached |
| `Cache:CurrenciesHours` | `24`                              | How long the currency list is cached        |

No API key is needed. There are no secrets in this repository.
