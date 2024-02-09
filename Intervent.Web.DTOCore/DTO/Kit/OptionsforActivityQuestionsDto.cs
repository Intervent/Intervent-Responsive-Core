namespace Intervent.Web.DTO
{
    public class ColumnDto
    {
        public int ActivityId { get; set; }

        public int RowId { get; set; }

        public int ColumnId { get; set; }

        public string QuestionText { get; set; }

        public byte? QuestionType { get; set; }

        public short RowSpan { get; set; }

        public short ColumnSpan { get; set; }

        public string Style { get; set; }

        public List<LanguageItemDto> LanguageTextValue { get; set; }
    }

    public class ChkColumnDto
    {
        public int IndexId { get; set; }

        public int QuestionId { get; set; }


        public short? Sequence { get; set; }

        //public int? ColSpan { get; set; }

        //public string style { get; set; }

        public string IndexText { get; set; }

        public string QuestionText { get; set; }

        public List<LanguageItemDto> LanguageTextValue { get; set; }

        public List<LanguageItemDto> IndexTextValue { get; set; }
    }
}
