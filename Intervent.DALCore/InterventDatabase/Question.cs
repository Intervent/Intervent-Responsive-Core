namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Question")]
    public partial class Question
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; }

        public int ControlTypeId { get; set; }

        [StringLength(500)]
        public string? ValuesStaticMethodName { get; set; }

        [StringLength(10)]
        public string? DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public int ValidatorTypeId { get; set; }

        [StringLength(500)]
        public string? ValidatorExpression { get; set; }

        [StringLength(1000)]
        public string? HelpText { get; set; }

        public int QuestionCategoryId { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public int SortOrder { get; set; }

        public int? ParentQuestionId { get; set; }

        [StringLength(100)]
        public string? ParentQuestionValue { get; set; }

        [StringLength(100)]
        public string? RestrictionCriteria { get; set; }

        [StringLength(500)]
        public string? ValidatorErrorMessage { get; set; }

        public bool? AllowDecimalInput { get; set; }

        public int? RowPosition { get; set; }

        public int? ColumnPosition { get; set; }

        [StringLength(50)]
        public string? MappingTableName { get; set; }

        [StringLength(50)]
        public string? MappingColumnName { get; set; }

        [StringLength(25)]
        public string? ControlSuffix { get; set; }

        [StringLength(25)]
        public string? Placeholder { get; set; }

        [StringLength(500)]
        public string? RequiredExpression { get; set; }

        [StringLength(500)]
        public string? ValidateOn { get; set; }
    }
}
