using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json")
     .Build();

var stitchSettings = configuration.GetSection("StitchSettings")
    .Get<StitchSettings>();

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IStitchSettings>(x => stitchSettings);
builder.Services.AddSingleton<IInputOutputHelper, IOHelper>();
builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddSingleton<IEncryptionHelper, EncryptionHelper>();
builder.Services.AddSingleton<IUrlService, UrlService>();
builder.Services.AddSingleton<ITokenBuilder, TokenBuilder>();
builder.Services.AddSingleton<IStitchRequestHelper, StitchRequestHelper>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IFloatService, FloatService>();
builder.Services.AddSingleton<IWalletService, WalletService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=StitchService}/{action=Index}/{id?}");

app.Run();
