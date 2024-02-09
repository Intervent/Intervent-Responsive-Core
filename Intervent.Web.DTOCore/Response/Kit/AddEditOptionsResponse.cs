namespace Intervent.Web.DTO
{
    public class AddEditOptionsResponse
    {
        public bool success { get; set; }

        public int id { get; set; }
        public IList<OptionsforActivityQuestionDto> options { get; set; }
    }

    public class AddEditTableOptionResponse
    {
        public bool IsSuccess { get; set; }
    }

    public class ListRowResponse
    {
        public IList<OptionsforActivityQuestionDto> Rows { get; set; }

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
    }


    public class ListColumnResponse
    {
        public List<ColumnDto> Columns { get; set; }
    }

    public class TableCheckboxResponse
    {
        public List<ChkColumnDto> Questions { get; set; }

        public List<ChkColumnDto> Options { get; set; }

        public int IndexId { get; set; }

        public short ColSpan { get; set; }
    }
}

