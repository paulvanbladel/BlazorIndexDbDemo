using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TG.Blazor.IndexedDB;
using BlazorIndexDbDemo.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configure IndexedDB
builder.Services.AddIndexedDB(dbStore =>
{
    dbStore.DbName = "LoanDatabase";
    dbStore.Version = 2;
    
    dbStore.Stores.Add(new StoreSchema
    {
        Name = "Loans",
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = true },
        Indexes = new List<IndexSpec>
        {
            new IndexSpec { Name = "name", KeyPath = "name", Auto = false }
        }
    });
    
    dbStore.Stores.Add(new StoreSchema
    {
        Name = "Metadata",
        PrimaryKey = new IndexSpec { Name = "key", KeyPath = "key", Auto = false }
    });
});

builder.Services.AddScoped<ILoanCacheService, LoanCacheService>();

await builder.Build().RunAsync();
