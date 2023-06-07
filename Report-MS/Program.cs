using Redis.OM;
using Report_MS.HostedService;
using Report_MS.Models;
using Report_MS.Models.Repository;
using Report_MS.Repository;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration["MONGO_CONNECTION_STRING"];
var mongoDatabaseName = builder.Configuration["MONGO_DATABASE_NAME"];
const string mongoCollectionName = "reports"; // Replace with the desired collection name

// Add services to the container.

builder.Services.AddHostedService<IndexCreationService>();
builder.Services.AddSingleton<IMongoRepository<Report>>(_ =>
    new MongoRepository<Report>(mongoConnectionString, mongoDatabaseName, mongoCollectionName));
builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration["REDIS_CONNECTION_STRING"]));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

