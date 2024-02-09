namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_HomeScreens
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? Home_Throwrugs { get; set; }

        public byte? Home_IndoorPets { get; set; }

        public byte? Home_SmokeAlarm { get; set; }

        public byte? Home_BathtubSafety { get; set; }

        public byte? Home_Bathmat { get; set; }

        public byte? Home_NightLight { get; set; }

        public byte? Home_LooseCords { get; set; }

        public byte? Home_UnplugAppl { get; set; }

        public byte? Home_MedSafe { get; set; }

        public byte? Home_KnifeSafe { get; set; }

        public byte? Home_ChemsSafe { get; set; }

        public byte? Home_SharpFurn { get; set; }

        public byte? HomeSafetyScore { get; set; }

        public byte? Fun_GetOutofBed { get; set; }

        public byte? Fun_DressYourself { get; set; }

        public byte? Fun_PrepOwnMeals { get; set; }

        public byte? Fun_OwnSHop { get; set; }

        public byte? Fun_WriteCheck { get; set; }

        public byte? Fun_Drive { get; set; }

        public byte? Fun_KeepApp { get; set; }

        public byte? Fun_TakeMed { get; set; }

        public byte? Fun_KeepEvents { get; set; }

        public byte? Fun_PlayGame { get; set; }

        public byte? FunActivityScore { get; set; }

        public byte? Fall_Numbess { get; set; }

        public byte? Fall_HeavySteps { get; set; }

        public byte? Fall_LightHeaded { get; set; }

        public byte? Fall_DiffStartStop { get; set; }

        public byte? Fall_TroubleGetOut { get; set; }

        public byte? Fall_DiffWalk { get; set; }

        public byte? Fall_LooseBalance { get; set; }

        public byte? Fall_EverFall { get; set; }

        public byte? FallRiskScore { get; set; }

        public byte? Hear_ProbOverTel { get; set; }

        public byte? Hear_TroubleFollConv { get; set; }

        public byte? Hear_AudioHighVol { get; set; }

        public byte? Hear_StraintoUnderstand { get; set; }

        public byte? Hear_NoisyBack { get; set; }

        public byte? Hear_AsktoRepeat { get; set; }

        public byte? Hear_PeopleMumble { get; set; }

        public byte? Hear_Misunderstand { get; set; }

        public byte? Hear_TroubleUnderstand { get; set; }

        public byte? Hear_PeopleGetAnnoyed { get; set; }

        public byte? HearingLossScore { get; set; }

        public byte? HomeSafetyProviderScore { get; set; }

        public byte? FallRiskProviderScore { get; set; }

        public byte? HearingLossProviderScore { get; set; }

        public byte? FunActivityProviderScore { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
