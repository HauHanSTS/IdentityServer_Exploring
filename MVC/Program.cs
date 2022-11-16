using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MVC;
using MVC.Middlewares;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Authentiction service
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options => {
        options.Authority = "https://localhost:5001";
        options.ClientId = "MVC";
        options.ClientSecret = "secret";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;

        options.Scope.Add(GetScopes.Gets());

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        { 
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseRefreshTokens();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .RequireAuthorization();

app.Run();
