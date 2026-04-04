// https://learn.microsoft.com/pl-pl/training/modules/build-web-api-minimal-database/5-exercise-use-sqlite-database
namespace Shared.API.Models
{
    public record BudgetCategory
    {
        public int BudgetCategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
