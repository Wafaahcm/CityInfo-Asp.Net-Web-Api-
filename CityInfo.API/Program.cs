using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; //Rejette les demandes avec des formats non pris en charge (code 406).
}).AddNewtonsoftJson() //pour gérer la sérialisation/désérialisation JSON, souvent utilisé pour sa flexibilité et ses fonctionnalités avancées comparées à System.Text.Json.
  .AddXmlDataContractSerializerFormatters(); // Ajoute la prise en charge des réponses XML en plus du JSON.

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


var app = builder.Build();

// Configure the HTTP request pipeline.
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
