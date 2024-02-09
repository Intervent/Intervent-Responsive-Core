namespace Intervent.Web.DTO
{
    public class QuestionDto
    {
        public int Id { get; set; }

        public string QuestionText { get; set; }

        public int ControlTypeId { get; set; }

        public string ValuesStaticMethodName { get; set; }

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public int ValidatorTypeId { get; set; }

        public string ValidatorExpression { get; set; }

        public string HelpText { get; set; }

        public int QuestionCategoryId { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }

        public int? ParentQuestionId { get; set; }

        public string ParentQuestionValue { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public string RestrictionCriteria { get; set; }

        public string ValidatorErrorMessage { get; set; }

        public bool AllowDecimalInput { get; set; }

        public int? RowPosition { get; set; }

        public int? ColumnPosition { get; set; }

        public string MappingTableName { get; set; }

        public string MappingColumnName { get; set; }

        public string ControlSuffix { get; set; }

        public string Placeholder { get; set; }

        public string RequiredExpression { get; set; }

        public string ValidateOn { get; set; }


    }
}
