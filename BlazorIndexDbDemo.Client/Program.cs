using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorIndexDbDemo.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register LoanCacheService for client-side IndexedDB operations
builder.Services.AddScoped<ILoanCacheService, LoanCacheService>();

await builder.Build().RunAsync();
