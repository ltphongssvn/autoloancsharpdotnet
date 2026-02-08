using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AutoLoan.Web.Services;

namespace AutoLoan.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSingleton<AuthStateService>();
            builder.Services.AddTransient<AuthHeaderHandler>();

            builder.Services.AddHttpClient("API", client =>
            {
                client.BaseAddress = new Uri("https://autoloancsharpdotnet-production.up.railway.app/api/v1/");
            }).AddHttpMessageHandler<AuthHeaderHandler>();

            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

            builder.Services.AddScoped<AuthService>();

            await builder.Build().RunAsync();
        }
    }
}
