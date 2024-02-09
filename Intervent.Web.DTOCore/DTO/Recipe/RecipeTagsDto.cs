namespace Intervent.Web.DTO
{
    public class RecipeTagsDto
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public string TagName
        {
            get
            {
                if (Tag != null)
                    return Tag.Name;
                return string.Empty;
            }
        }

        public bool IsActive { get; set; }

        public int TagId { get; set; }

        public TagsDto Tag { get; set; }
    }
}
