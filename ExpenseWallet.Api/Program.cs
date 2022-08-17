using Core.ExpenseWallet.Interfaces;
using Core.ExpenseWallet.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json")
     .Build();

var stitchSettings = configuration.GetSection("StitchSettings")
    .Get<StitchSettings>();

builder.Services.AddSingleton<IStitchSettings>(x => stitchSettings);
builder.Services.AddSingleton<IInputOutputHelper, IOHelper>();
builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddSingleton<IEncryptionHelper, EncryptionHelper>();
builder.Services.AddSingleton<IUrlService, UrlService>();
builder.Services.AddSingleton<ITokenBuilder, TokenBuilder>();
builder.Services.AddSingleton<IStitchRequestHelper, StitchRequestHelper>();
builder.Services.AddSingleton<IFloatService, FloatService>();
builder.Services.AddSingleton<IWalletService, WalletService>();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
