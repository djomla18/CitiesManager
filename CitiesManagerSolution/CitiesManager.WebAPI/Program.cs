using Microsoft.AspNetCore.Mvc;
using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));

}).AddXmlSerializerFormatters();


builder.Services.AddApiVersioning(config =>
{
    // Reads the version number from the request URL at apiVersion constraint
    // (FROM THE ROUTE)
    config.ApiVersionReader = new UrlSegmentApiVersionReader();

    // Reads version number from the request query string called "api-version"
    // config.ApiVersionReader = new QueryStringApiVersionReader();

    // In this case, the API version is beeing read from the query string called "version"
    // Eg: api-version: 1.0
    // config.ApiVersionReader = new QueryStringApiVersionReader("version");

    // Reads the API version from the request header called "api-version"
    // Eg: api-version: 1.0
    // config.ApiVersionReader = new HeaderApiVersionReader();

    // Set the default Api version to 1.0 when it's not specified
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
});


// Generates description for all endpoints
builder.Services.AddEndpointsApiExplorer();

// Generates openAPI specifications
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Cities Web API",
        Version = "1.0"
    });

    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Cities Web API",
        Version = "2.0"
    });

    // putanja do Projekta WebApi  // naziv fajla za komentare
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
});

builder.Services.AddVersionedApiExplorer(options =>
{
    // VV - each V reperesent the number of digis allowed for API version
    options.GroupNameFormat = "'v'VV"; // v1
    
    // substitue version number in the endpoint for swagger.json file
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();

// Creates an endpoint for swagger.json file  with Open API specificaitons
app.UseSwagger();

// Creates swagger UR for testing all Web API endpoints / action methods
app.UseSwaggerUI(options =>
{
    // default: localhost:portNum/swagger/v1/swagger.json
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");

});

app.UseAuthorization();

app.MapControllers();

app.Run();
