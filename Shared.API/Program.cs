using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("Shared") ?? "Data Source=Shared.db";

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Shared API",
            Description = "",
            Version = "v1",
        }
    );
});

builder.Services.AddSqlite<SharedDbContext>(connectionString);

builder.Services.AddDbContext<SharedDbContext>(options => options.UseInMemoryDatabase("items"));

var tokenOptions = builder.Configuration.GetSection(TokenOptions.CONFIG_NAME).Get<TokenOptions>();

builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection(TokenOptions.CONFIG_NAME));

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.FromMinutes(1),
            IgnoreTrailingSlashWhenValidatingAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenOptions.SigningKey)),
            ValidateIssuerSigningKey = tokenOptions.ValidateSigningKey,
            RequireExpirationTime = true,
            RequireAudience = true,
            RequireSignedTokens = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidAudience = tokenOptions.Audience,
            ValidIssuer = tokenOptions.Issuer,
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shared API V1");
    });
}

app.MapGet("/", () => "Hello World!");

app.MapGet("/BudgetCategories", async (SharedDbContext db) => await db.BudgetCategories.ToListAsync());

app.MapPost(
    "/BudgetCategories",
    async (SharedDbContext db, BudgetCategory budgetCategory) =>
    {
        await db.BudgetCategories.AddAsync(budgetCategory);
        await db.SaveChangesAsync();
        return Results.Created($"/BudgetCategories/{budgetCategory.BudgetCategoryId}", budgetCategory);
    }
);

app.MapPut(
    "/BudgetCategories/{id}",
    async (SharedDbContext db, BudgetCategory updateCategory, int id) =>
    {
        var category = await db.BudgetCategories.FindAsync(id);
        if (category is null)
            return Results.NotFound();
        category.Name = updateCategory.Name;
        category.Description = updateCategory.Description;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
);

app.MapDelete(
    "/BudgetCategories/{id}",
    async (SharedDbContext db, int id) =>
    {
        var category = await db.BudgetCategories.FindAsync(id);
        if (category is null)
        {
            return Results.NotFound();
        }
        db.BudgetCategories.Remove(category);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
);

app.MapGet("/BudgetCategories/{id}", async (SharedDbContext db, int id) => await db.BudgetCategories.FindAsync(id));

app.Run();
