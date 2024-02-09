namespace Intervent.DAL
{
    public partial class RecipeTag
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public bool IsActive { get; set; }

        public int TagId { get; set; }

        public virtual Recipe Recipe { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
