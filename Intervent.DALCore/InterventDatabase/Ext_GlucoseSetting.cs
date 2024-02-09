namespace Intervent.DAL
{
    public partial class Ext_GlucoseSetting
    {
        public int Id { get; set; }

        public int GlucoseId { get; set; }

        public int? Hypo { get; set; }

        public int? Hyper { get; set; }

        public int? Low { get; set; }

        public int? High { get; set; }

        public double? a1c { get; set; }

        public int? PreMealLow { get; set; }

        public int? PostMealLow { get; set; }

        public int? PreMealHigh { get; set; }

        public int? PostMealHigh { get; set; }

        public virtual EXT_Glucose EXT_Glucose { get; set; }
    }
}
