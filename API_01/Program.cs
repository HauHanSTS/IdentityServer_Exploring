using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001";
        options.Audience = "API_01";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AudRequire", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("aud", "API_01");
    });
    options.AddPolicy("CanAccessResource", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "API_01.resource.read", "API_01.resource.write");
    });
    options.AddPolicy("CanAccessUser", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "API_01.user.read", "API_01.user.write");
    });
});
builder.Services.AddCors(options => {
    options.AddPolicy("Default",
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Default");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization("AudRequire");

app.Run();
