// https://learn.microsoft.com/pl-pl/training/modules/build-web-api-minimal-database/3-exercise-add-entity-framework-core
namespace Shared.API.Models
{
    public sealed class BudgetCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
