using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "PizzaStore API",
            Description = "Making the Pizzas you love",
            Version = "v1",
        }
    );
});

builder.Services.AddDbContext<SharedDbContext>(options => options.UseInMemoryDatabase("items"));

var app = builder.Build();
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
