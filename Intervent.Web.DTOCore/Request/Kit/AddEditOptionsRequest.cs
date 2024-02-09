namespace Intervent.Web.DTO
{
    public class AddEditOptionsRequest
    {
        public OptionsforActivityQuestionDto OptionsForQuestions { get; set; }

        public string language { get; set; }
    }

    public class AddEditTableOptionRequest
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public int ParentId { get; set; }

        public int ActivityId { get; set; }
    }

    public class AddRowRequest
    {
        public int ParentId { get; set; }
    }


    public class AddEditColumnRequest
    {

        public ColumnDto Column { get; set; }
        public string Language { get; set; }

    }

    public class AddEditTableCheckboxRequest
    {

        public string Language { get; set; }
        public int ParentQuestionId { get; set; }
        public int ActivityId { get; set; }
        public bool IsOption { get; set; }

        public ChkColumnDto Question { get; set; }
    }
}
