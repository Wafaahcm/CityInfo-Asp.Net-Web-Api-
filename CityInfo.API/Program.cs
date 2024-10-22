using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; //Rejette les demandes avec des formats non pris en charge (code 406).
}).AddNewtonsoftJson() //pour gérer la sérialisation/désérialisation JSON, souvent utilisé pour sa flexibilité et ses fonctionnalités avancées comparées à System.Text.Json.
  .AddXmlDataContractSerializerFormatters(); // Ajoute la prise en charge des réponses XML en plus du JSON.


builder.Services.AddProblemDetails();  //etourner des réponses formatées et standardisées pour les erreurs dans une API, tout en cachant les détails d'implémentation aux consommateurs

//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = ctx =>
//    {
//        ctx.ProblemDetails.Extensions.Add("additionalInfo",
//            "Additional info example");
//        ctx.ProblemDetails.Extensions.Add("server",
//           Environment.MachineName);
//    };
//});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); //enregistre ce service comme singleton pour que tu puisses l'injecter dans d'autres parties de ton application.Il permet de déterminer facilement le type MIME correct avant de retourner un fichier.
builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CityInfoDB")));

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(); //Cette condition configure le middleware pour gérer les exceptions en production, assurant ainsi une réponse appropriée aux utilisateurs sans exposer les détails techniques de l'erreur.
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();   //Permet à l'application de déterminer le bon endpoint pour chaque requête HTTP.

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});                // Associe les contrôleurs aux requêtes HTTP, permettant ainsi le traitement des actions définies dans les contrôleurs.


app.Run();
