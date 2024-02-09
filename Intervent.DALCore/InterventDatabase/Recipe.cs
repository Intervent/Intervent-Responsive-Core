namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Recipe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Recipe()
        {
            AssignedRecipes = new HashSet<AssignedRecipe>();
            RecipeTags = new HashSet<RecipeTag>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        [StringLength(512)]
        public string? ImageURL { get; set; }

        public bool IsActive { get; set; }

        [StringLength(50)]
        public string? Carbohydrate { get; set; }

        [StringLength(50)]
        public string? Fat { get; set; }

        public int? Sodium { get; set; }

        public int? Sugar { get; set; }

        [StringLength(50)]
        public string? Calories { get; set; }

        public int? Cholesterol { get; set; }

        [Required]
        public string Direction { get; set; }

        public string? Ingredients { get; set; }

        [StringLength(50)]
        public string? Yield { get; set; }

        [StringLength(50)]
        public string? ServingSize { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignedRecipe> AssignedRecipes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecipeTag> RecipeTags { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<RecipeTranslation> RecipeTranslations { get; set; }
    }
}
