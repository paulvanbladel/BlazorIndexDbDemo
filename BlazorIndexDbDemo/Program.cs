using BlazorIndexDbDemo.Client.Pages;
using BlazorIndexDbDemo.Components;
using BlazorIndexDbDemo.Services;
using BlazorIndexDbDemo.Client.Services;
using Microsoft.AspNetCore.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add controller services for API endpoints
builder.Services.AddControllers();

// Add HttpClient for client-side components  
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Register LoanHashService as singleton
builder.Services.AddSingleton<ILoanHashService, LoanHashService>();

// Register server-side stub for LoanCacheService (used during prerendering)
builder.Services.AddScoped<ILoanCacheService, ServerLoanCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Map API controllers
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorIndexDbDemo.Client._Imports).Assembly);

app.Run();
