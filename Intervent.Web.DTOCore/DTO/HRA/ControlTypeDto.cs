namespace Intervent.Web.DTO
{
    public sealed class ControlTypeDto
    {
        public int Id { get; set; }

        public string Description { get; set; }
        private ControlTypeDto(int id, string desc)
        {
            Id = id;
            Description = desc;
        }

        public static ControlTypeDto VRadio = new ControlTypeDto(1, "VRadio");
        public static ControlTypeDto Textbox = new ControlTypeDto(2, "Textbox");
        public static ControlTypeDto Dropdown = new ControlTypeDto(3, "Dropdown");
        public static ControlTypeDto StartHeadingWithYesNoLabel = new ControlTypeDto(4, "StartHeadingWithYesNoLabel");
        public static ControlTypeDto HRadioNoLabel = new ControlTypeDto(5, "HRadioNoLabel");
        public static ControlTypeDto Date = new ControlTypeDto(6, "Date");
        public static ControlTypeDto VCheckboxNoLabelWithNoOption = new ControlTypeDto(7, "VCheckboxNoLabelWithNoOption");
        public static ControlTypeDto GroupWithNoneQuestion = new ControlTypeDto(8, "GroupWithNoneQuestion");
        public static ControlTypeDto HCheckbox = new ControlTypeDto(9, "HCheckbox");
        public static ControlTypeDto Fiedset = new ControlTypeDto(10, "Fieldset");
        public static ControlTypeDto Table = new ControlTypeDto(11, "Table");
        public static ControlTypeDto HRadio = new ControlTypeDto(12, "HRadio");
        public static ControlTypeDto NumericTextBox = new ControlTypeDto(13, "NumericTextBox");
        public static ControlTypeDto Label = new ControlTypeDto(14, "Label");
        public static ControlTypeDto GroupWithAllQuestion = new ControlTypeDto(15, "GroupWithAllQuestion");

        static IEnumerable<ControlTypeDto> GetAll()
        {
            List<ControlTypeDto> lst = new List<ControlTypeDto>();
            lst.Add(ControlTypeDto.VRadio);
            lst.Add(ControlTypeDto.Textbox);
            lst.Add(ControlTypeDto.Dropdown);
            lst.Add(ControlTypeDto.StartHeadingWithYesNoLabel);
            lst.Add(ControlTypeDto.HRadioNoLabel);
            lst.Add(ControlTypeDto.Date);
            lst.Add(ControlTypeDto.VCheckboxNoLabelWithNoOption);
            lst.Add(ControlTypeDto.GroupWithNoneQuestion);
            lst.Add(ControlTypeDto.HCheckbox);
            lst.Add(ControlTypeDto.Fiedset);
            lst.Add(ControlTypeDto.Table);
            lst.Add(ControlTypeDto.HRadio);
            lst.Add(ControlTypeDto.NumericTextBox);
            lst.Add(ControlTypeDto.Label);
            lst.Add(ControlTypeDto.GroupWithAllQuestion);
            return lst;
        }

        public static ControlTypeDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
