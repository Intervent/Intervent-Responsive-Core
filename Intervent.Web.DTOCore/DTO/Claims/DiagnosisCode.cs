namespace Intervent.Web.DTO
{
    public class DiagnosisCodeDto
    {
        public string Code { get; set; }

        public string Description { get; set; }
        private DiagnosisCodeDto(string code, string desc)
        {
            Code = code;
            Description = desc;
        }

        public static DiagnosisCodeDto PREG = new DiagnosisCodeDto("PREG", "PREG");
        public static DiagnosisCodeDto PREGPOS = new DiagnosisCodeDto("PREGPOS", "PREGPOS");
        public static DiagnosisCodeDto DIAB = new DiagnosisCodeDto("DIAB", "DIAB");
        public static DiagnosisCodeDto HYPTEN = new DiagnosisCodeDto("HYPTEN", "HYPTEN");
        public static DiagnosisCodeDto OBESE = new DiagnosisCodeDto("OBESE", "OBESE");
        public static DiagnosisCodeDto HD = new DiagnosisCodeDto("HD", "HD");
        public static DiagnosisCodeDto SLEEP = new DiagnosisCodeDto("SLEEP", "SLEEP");
        public static DiagnosisCodeDto LUNG = new DiagnosisCodeDto("LUNG", "LUNG");
        public static DiagnosisCodeDto SMOKE = new DiagnosisCodeDto("SMOKE", "SMOKE");
        public static DiagnosisCodeDto DIABLV = new DiagnosisCodeDto("DIABLV", "DIABLV");
        public static DiagnosisCodeDto CKD = new DiagnosisCodeDto("CKD", "CKD");
        public static DiagnosisCodeDto PREDIAB = new DiagnosisCodeDto("PREDIAB", "PREDIAB");
        public static DiagnosisCodeDto OTHER = new DiagnosisCodeDto("OTHER", "OTHER");

        public static IEnumerable<DiagnosisCodeDto> GetAll()
        {
            List<DiagnosisCodeDto> lst = new List<DiagnosisCodeDto>();
            lst.Add(DiagnosisCodeDto.PREG);
            lst.Add(DiagnosisCodeDto.PREGPOS);
            lst.Add(DiagnosisCodeDto.DIAB);
            lst.Add(DiagnosisCodeDto.HYPTEN);
            lst.Add(DiagnosisCodeDto.OBESE);
            lst.Add(DiagnosisCodeDto.HD);
            lst.Add(DiagnosisCodeDto.SLEEP);
            lst.Add(DiagnosisCodeDto.LUNG);
            lst.Add(DiagnosisCodeDto.SMOKE);
            lst.Add(DiagnosisCodeDto.DIABLV);
            lst.Add(DiagnosisCodeDto.CKD);
            lst.Add(DiagnosisCodeDto.PREDIAB);
            lst.Add(DiagnosisCodeDto.OTHER);
            return lst;
        }

        public static DiagnosisCodeDto GetByKey(string code)
        {
            return GetAll().Where(x => x.Code == code).FirstOrDefault();
        }
    }
}
