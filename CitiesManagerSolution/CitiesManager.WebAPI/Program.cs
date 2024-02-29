using Microsoft.AspNetCore.Mvc;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using CitiesManager.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CitiesManager.Core.Services;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));

    // Authorization policy
    var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

    // Adding this as a global filter, the policy get's applied
    // for all the controllers
    options.Filters.Add(new AuthorizeFilter(policy));

}).AddXmlSerializerFormatters();


// Generates description for all endpoints
builder.Services.AddEndpointsApiExplorer();

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
    // One V is also for the '.' in version number 
    // (Firts V = 1 (or 2), Second V = . , Third V = 0 (or any other number)) 
    options.GroupNameFormat = "'v'VVV"; // v1
    
    // substitue version number in the endpoint for swagger.json file
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default")));


// Identity 
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
})

    .AddEntityFrameworkStores<ApplicationDbContext>()

    .AddDefaultTokenProviders()

    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()

    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


builder.Services.AddTransient<IJwtService, JwtService>();


// JWT 
// Setting that the Default cookie for Authorization of the user
// is the Jwt Authentication Scheme instead of MVC identity cookie
builder.Services.AddAuthentication(options =>
{
    // Default scheme JWT token
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
    // what are the properties that has to be processed in 
    // the JWT token, how to process the token and 
    // what things within the token have to be validated
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
    new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Creates an endpoint for swagger.json file  with Open API specificaitons
app.UseSwagger();

// Creates swagger UR for testing all Web API endpoints / action methods
app.UseSwaggerUI(options =>
{
    // default: localhost:portNum/swagger/v1/swagger.json
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");

});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
