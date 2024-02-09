namespace Intervent.Web.DTO
{
    public class KeyValueDTO
    {
        public string Value { get; set; }

        public string Text { get; set; }

        public KeyValueDTO()
        {
        }

        public KeyValueDTO(string text, string value)
        {
            Text = text;
            Value = value;
        }
    }
}