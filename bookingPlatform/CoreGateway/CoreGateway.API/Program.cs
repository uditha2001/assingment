using CoreGateway.API.Service;
using CoreGateway.API.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

// Add services
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, AuthServiceIMPL>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
builder.Services.Configure<ServiceUrls>(
    builder.Configuration.GetSection("ServiceUrls"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();


//app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        var path = context.Request.Path;
        var pathValue = path.Value?.TrimEnd('/') ?? "";

        var publicPaths = new[]
        {
        "/api/v1/user/login",
        "/api/v1/user",
        "/api/v1/product/allProducts",
        "/api/v1/authentication",
        "/uploads",
        "/swagger"
    };

        bool isPublic = publicPaths.Any(p =>
        {
            var pTrimmed = p.TrimEnd('/');
            return pathValue.Equals(pTrimmed, StringComparison.OrdinalIgnoreCase)
                || pathValue.StartsWith(pTrimmed + "/", StringComparison.OrdinalIgnoreCase);
        });

        if (!isPublic) // Only require authentication for non-public paths
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await next();
    });

});


app.Run();
