namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_STDandProstateRisk
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? STD_Active { get; set; }

        public byte? STD_MulPart { get; set; }

        public byte? STD_With { get; set; }

        public byte? STD_UsingDrugs { get; set; }

        public byte? STD_UseCondoms { get; set; }

        public byte? STD_HaveTattoo { get; set; }

        public byte? STD_BodyPierce { get; set; }

        public byte? STD_LivedWithHIV { get; set; }

        public byte? STD_InJail { get; set; }

        public byte? STD_BloodTrans { get; set; }

        public byte? STD_TestedforHIV { get; set; }

        public byte? STD_SexWithSTD { get; set; }

        public byte? STD_HadSexDis { get; set; }

        public byte? STD_UsedDrugs { get; set; }

        public byte? STD_SexWithUnknown { get; set; }

        public byte? STD_HepBVaccine { get; set; }

        public byte? STDRiskScreen { get; set; }

        public byte? Pro_NotEmpty { get; set; }

        public byte? Pro_AgaininTwo { get; set; }

        public byte? Pro_StartandStop { get; set; }

        public byte? Pro_DifftoPost { get; set; }

        public byte? Pro_WeakSystem { get; set; }

        public byte? Pro_StraintoBegin { get; set; }

        public byte? Pro_GetupatNight { get; set; }

        public byte? ProsRiskScreen { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
