namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_TobaccoUse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public DateTime? EnctoQuitDate { get; set; }

        public byte? Cigarettes { get; set; }

        public byte? SmokelessTob { get; set; }

        public byte? NoPerDay { get; set; }

        public byte? NoofYears { get; set; }

        public byte? OthersatHome { get; set; }

        public byte? TimesTried { get; set; }

        public short? SuccessFor { get; set; }

        [StringLength(100)]
        public string? HelpedOthers { get; set; }

        [StringLength(100)]
        public string? FailedOthers { get; set; }

        public byte? Motive { get; set; }

        public byte? MotiveDueto { get; set; }

        public byte? FamilyHelp { get; set; }

        public DateTime? DecFrom { get; set; }

        public DateTime? DecTo { get; set; }

        public DateTime? ChosQuitDate { get; set; }

        public byte? NoReady { get; set; }

        public byte? SmokeAtHome { get; set; }

        public byte? SmokeAtWork { get; set; }

        public byte? SmokeAtSocial { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
