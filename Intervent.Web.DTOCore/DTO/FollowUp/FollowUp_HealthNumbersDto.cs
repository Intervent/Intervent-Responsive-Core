namespace Intervent.Web.DTO
{
    public class FollowUp_HealthNumbersDto
    {
        public int Id { get; set; }

        public byte? BPArm { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public byte? DidYouFast { get; set; }

        private DateTime? _bloodTestDate;

        private string _bloodTestDateShort;

        public DateTime? BloodTestDate
        {
            get
            {
                return _bloodTestDate;
            }
            set
            {
                _bloodTestDate = value;
                if (_bloodTestDate.HasValue)
                    _bloodTestDateShort = _bloodTestDate.Value.ToShortDateString();
            }
        }

        public string BloodTestDateShort
        {
            get
            {
                return _bloodTestDateShort;
            }
            set
            {
                _bloodTestDateShort = value;
                if (!string.IsNullOrEmpty(_bloodTestDateShort))
                    _bloodTestDate = Convert.ToDateTime(_bloodTestDateShort);
            }
        }
        public float? TotalChol { get; set; }

        public float? LDL { get; set; }

        public float? HDL { get; set; }

        public float? Trig { get; set; }

        public float? Glucose { get; set; }

        public float? A1C { get; set; }

        public float? Weight { get; set; }

        private float? _height;

        private int _heightft;

        private float _heightinch;

        public float? HeightCM { get; set; }

        public float? Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                if (_height.HasValue)
                {
                    _heightft = (int)_height.Value / 12;
                    _heightinch = _height.Value % 12; ;
                }
            }
        }

        public int HeightFt
        {
            get
            {
                return _heightft;
            }
            set
            {
                _heightft = value;
                updateHeight();
            }
        }

        private void updateHeight()
        {
            if (_heightft > 0)
                _height = (_heightft * 12);
            if (_heightinch > 0)
                _height = _height + _heightinch;
        }

        public float HeightInch
        {
            get
            {
                return _heightinch;
            }
            set
            {
                _heightinch = value;
                updateHeight();
            }

        }

        public float? Waist { get; set; }

        public short? RMR { get; set; }

        public short? THRFrom { get; set; }

        public short? THRTo { get; set; }

        public float? BMI { get; set; }

        public float? CRF { get; set; }

        public float? RHR { get; set; }

        public int? LabId { get; set; }

        public FollowUpDto FollowUp { get; set; }

        public LabDto Lab { get; set; }
    }
}
