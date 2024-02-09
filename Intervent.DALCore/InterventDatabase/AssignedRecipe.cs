namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AssignedRecipe")]
    public partial class AssignedRecipe
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int OrganizationId { get; set; }

        public DateTime Date { get; set; }

        public bool? Completed { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
