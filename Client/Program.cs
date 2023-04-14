global using BlazorDB;
global using Blazored.LocalStorage;
global using Blazored.SessionStorage;
global using MudBlazor.Services;
global using CurrieTechnologies.Razor.SweetAlert2;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAppAcademics.Client;
using Microsoft.AspNetCore.Components.Authorization;
using WebAppAcademics.Client.Authentication;
using WebAppAcademics.Client.OfflineAuth;
using WebAppAcademics.Client.OfflineRepo;
using WebAppAcademics.Client.OfflineServices;
using WebAppAcademics.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient(new OfflineBackendHandler())
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<IOfflineAuthenticationService, OfflineAuthenticationService>();
builder.Services.AddScoped<IHttpService, HttpService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddAPIServices();
builder.Services.AddMudServices();
builder.Services.AddSweetAlert2(options => {
    options.Theme = SweetAlertTheme.Dark;
    options.SetThemeForColorSchemePreference(ColorScheme.Light, SweetAlertTheme.Bootstrap4);
});
builder.Services.AddBlazorDBServices();
builder.Services.AddOfflineRepoServices();

await builder.Build().RunAsync();
