namespace Intervent.Web.DTO
{
    public class EXT_GlucoseDto
    {
        public int Id { get; set; }
        public string UniqueId { get; set; }
        public DateTime? EffectiveDateTime { get; set; }
        public DateTime? DateTime { get; set; }
        public int Value { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
    }
}
