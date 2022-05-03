using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.CurrencyConverter.Contracts;
using Project.CurrencyConverter.Services;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddSingleton<ICurrencyConverter, CurrencyConverter>())
    .Build();

Convert(host.Services);

await host.RunAsync();

static void Convert(IServiceProvider services)
{
    using IServiceScope serviceScope = services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var currencyConverter = provider.GetRequiredService<ICurrencyConverter>();

    currencyConverter.UpdateConfiguration(new List<Tuple<string, string, double>>
    {
        new Tuple<string, string, double>("USD", "EUR", 1.1),
        new Tuple<string, string, double>("USD", "GBP", 0.8),
        new Tuple<string, string, double>("EUR", "USD", 1.0),
        new Tuple<string, string, double>("EUR", "GBP", 0.8),
        new Tuple<string, string, double>("GBP", "USD", 1.0),
        new Tuple<string, string, double>("GBP", "CCC", 1.0)
    });

    var source = "USD";
    var dest = "GBP";
    var amount = 100;

    var result = currencyConverter.Convert(source, dest, amount);

     Console.WriteLine($"{amount} {source} to {dest} is {result}");

}