namespace Intervent.Web.DTO
{
    public class AssignedRecipeDto
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public int RecipeId { get; set; }

        public DateTime Date { get; set; }

        public bool Completed { get; set; }

        public OrganizationDto Organization { get; set; }
    }
}
