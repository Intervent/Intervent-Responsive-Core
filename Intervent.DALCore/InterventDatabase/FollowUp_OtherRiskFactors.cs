namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FollowUp_OtherRiskFactors
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? HeartHist { get; set; }

        public byte? SmokeCig { get; set; }

        public float? NoOfCig { get; set; }

        public byte? RdyQuitCig { get; set; }

        [StringLength(500)]
        public string? ObstCig { get; set; }

        public byte? QuitCig { get; set; }

        [Column(TypeName = "date")]
        public DateTime? QuitCigDate { get; set; }

        public byte? OtherTobacco { get; set; }

        public byte? Cigar { get; set; }

        public byte? Pipe { get; set; }

        public byte? SmokelessTob { get; set; }

        public byte? RdyQuitTob { get; set; }

        public byte? QuitCigar { get; set; }

        [Column(TypeName = "date")]
        public DateTime? QuitCigarDate { get; set; }

        public byte? QuitPipe { get; set; }

        [Column(TypeName = "date")]
        public DateTime? QuitPipeDate { get; set; }

        public byte? QuitSmokelessTob { get; set; }

        [Column(TypeName = "date")]
        public DateTime? QuitSmokelessTobDate { get; set; }

        public byte? ECig { get; set; }

        public byte? RdyQuitECig { get; set; }

        public byte? QuitECig { get; set; }

        [Column(TypeName = "date")]
        public DateTime? QuitECigDate { get; set; }

        public byte? VigExer { get; set; }

        public byte? ModExer { get; set; }

        public byte? ExertPain { get; set; }

        public byte? LowFatDiet { get; set; }

        public byte? HealthyCarb { get; set; }

        public byte? FeelStress { get; set; }

        public byte? FeelAnxiety { get; set; }

        public byte? FeelDepression { get; set; }

        public byte? WaterPipes { get; set; }

        public byte? OtherFormofTob { get; set; }

        public float? ExerciseMins { get; set; }

        public byte? TwoAlcohol { get; set; }

        public byte? OneAlcohol { get; set; }

        public byte? OverWeight { get; set; }

        public byte? OnlyDepression { get; set; }

        public byte? SleepApnea { get; set; }

        public byte? FeelTired { get; set; }

        public byte? Snore { get; set; }

        public byte? BreathPause { get; set; }

        public byte? Headache { get; set; }

        public byte? Sleepy { get; set; }

        public byte? NoIssue { get; set; }

        public byte? SleepApneaMed { get; set; }

        public byte? Walking { get; set; }

        public byte? Jogging { get; set; }

        public byte? Treadmill { get; set; }

        public byte? Cycling { get; set; }

        public byte? StairMach { get; set; }

        public byte? EllipticTrainer { get; set; }

        public byte? RowingMach { get; set; }

        public byte? AerobicMach { get; set; }

        public byte? AerobicDance { get; set; }

        public byte? OutdoorCycling { get; set; }

        public byte? SwimmingLaps { get; set; }

        public byte? RacquetSports { get; set; }

        public byte? OtherAerobic { get; set; }

        public byte? NoAerobic { get; set; }

        public float? WalkingActivityEachWeek { get; set; }

        public float? WalkingExerciseEachTime { get; set; }

        public byte? WalkingIntenseExercise { get; set; }
        public float? JoggingActivityEachWeek { get; set; }

        public float? JoggingExerciseEachTime { get; set; }

        public byte? JoggingIntenseExercise { get; set; }

        public float? TreadmillActivityEachWeek { get; set; }

        public float? TreadmillExerciseEachTime { get; set; }

        public byte? TreadmillIntenseExercise { get; set; }

        public float? CyclingActivityEachWeek { get; set; }

        public float? CyclingExerciseEachTime { get; set; }

        public byte? CyclingIntenseExercise { get; set; }

        public float? StairMachActivityEachWeek { get; set; }

        public float? StairMachExerciseEachTime { get; set; }

        public byte? StairMachIntenseExercise { get; set; }

        public float? EllipticTrainerActivityEachWeek { get; set; }

        public float? EllipticTrainerExerciseEachTime { get; set; }

        public byte? EllipticTrainerIntenseExercise { get; set; }

        public float? RowingMachActivityEachWeek { get; set; }

        public float? RowingMachExerciseEachTime { get; set; }

        public byte? RowingMachIntenseExercise { get; set; }

        public float? AerobicMachActivityEachWeek { get; set; }

        public float? AerobicMachExerciseEachTime { get; set; }

        public byte? AerobicMachIntenseExercise { get; set; }

        public float? AerobicDanceActivityEachWeek { get; set; }

        public float? AerobicDanceExerciseEachTime { get; set; }

        public byte? AerobicDanceIntenseExercise { get; set; }

        public float? OutdoorCyclingActivityEachWeek { get; set; }

        public float? OutdoorCyclingExerciseEachTime { get; set; }

        public byte? OutdoorCyclingIntenseExercise { get; set; }

        public float? SwimmingLapsActivityEachWeek { get; set; }

        public float? SwimmingLapsExerciseEachTime { get; set; }

        public byte? SwimmingLapsIntenseExercise { get; set; }

        public float? RacquetSportsActivityEachWeek { get; set; }

        public float? RacquetSportsExerciseEachTime { get; set; }

        public byte? RacquetSportsIntenseExercise { get; set; }

        public float? OtherAerobicActivityEachWeek { get; set; }

        public float? OtherAerobicExerciseEachTime { get; set; }

        public byte? OtherAerobicIntenseExercise { get; set; }

        public byte? MusclesActivities { get; set; }

        public float? MusclesActivitiesDays { get; set; }

        public byte? FlexibilityActivities { get; set; }

        public float? FlexibilityActivitiesDays { get; set; }

        public byte? PhysicallyActiveLevel { get; set; }

        public virtual FollowUp FollowUp { get; set; }
    }
}
