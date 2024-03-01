using Intervent.DAL;
using Intervent.Framework.Clone;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;


namespace Intervent.Web.DataLayer
{
    public class HRAReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public CreateHRAResponse CreateHRA(CreateHRARequest request)
        {
            CreateHRAResponse response = new CreateHRAResponse();
            var hra = context.HRAs.Where(x => x.UserId == request.HRA.UserId && x.PortalId == request.HRA.PortalId).FirstOrDefault();
            if (hra == null)
            {
                DAL.HRA HRA = new DAL.HRA();
                HRA.UserId = request.HRA.UserId;
                HRA.PortalId = request.HRA.PortalId;
                HRA.StartDate = DateTime.UtcNow;
                HRA.CreatedBy = request.HRA.CreatedBy;
                HRA.Language = request.languageCode;
                context.HRAs.Add(HRA);
                context.SaveChanges();
                response.HRAId = HRA.Id;
                if (request.UserinProgramId.HasValue)
                {
                    var program = context.UsersinPrograms.Where(x => x.HRAId == null && x.Id == request.UserinProgramId.Value).FirstOrDefault();
                    if (program != null)
                    {
                        program.HRAId = response.HRAId;
                        context.UsersinPrograms.Attach(program);
                        context.Entry(program).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            else
            {
                response.HRAId = hra.Id;
            }
            return response;
        }

        public AddEditMedicalConditionsResponse AddEditMedicalCondition(AddEditMedicalConditionsRequest request, string DTCOrgCode)
        {
            AddEditMedicalConditionsResponse response = new AddEditMedicalConditionsResponse();
            var dataUpdated = false;
            var medicalCondition = context.HRA_MedicalConditions.Where(x => x.Id == request.medicalCondition.HRAId).FirstOrDefault();

            if (medicalCondition != null)
            {
                DAL.HRA_MedicalConditions currentMedicalCondition = null;
                if (request.StoreHistory)
                    currentMedicalCondition = CloneUtil.DeepClone<DAL.HRA_MedicalConditions>(medicalCondition);
                medicalCondition.Id = request.medicalCondition.HRAId;
                medicalCondition.BreastFeed = request.medicalCondition.BreastFeed;
                medicalCondition.Pregnant = request.medicalCondition.Pregnant;
                medicalCondition.PregDueDate = request.medicalCondition.PregDueDate;
                medicalCondition.PlanBecPreg = request.medicalCondition.PlanBecPreg;
                medicalCondition.Hysterectomy = request.medicalCondition.Hysterectomy;
                medicalCondition.OvariesRemoved = request.medicalCondition.OvariesRemoved;
                medicalCondition.Stroke = request.medicalCondition.Stroke;
                medicalCondition.HeartAttack = request.medicalCondition.HeartAttack;
                medicalCondition.Angina = request.medicalCondition.Angina;
                medicalCondition.ToldBabyNine = request.medicalCondition.ToldBabyNine;
                medicalCondition.ToldBlock = request.medicalCondition.ToldBlock;
                medicalCondition.ToldHeartBlock = request.medicalCondition.ToldHeartBlock;
                medicalCondition.ToldHighBP = request.medicalCondition.ToldHighBP;
                medicalCondition.ToldHighChol = request.medicalCondition.ToldHighChol;
                medicalCondition.ToldDiabetes = request.medicalCondition.ToldDiabetes;
                medicalCondition.ToldGestDiab = request.medicalCondition.ToldGestDiab;
                medicalCondition.ToldPolycyst = request.medicalCondition.ToldPolycyst;
                medicalCondition.ToldAsthma = request.medicalCondition.ToldAsthma;
                medicalCondition.ToldBronchitis = request.medicalCondition.ToldBronchitis;
                medicalCondition.ToldCancer = request.medicalCondition.ToldCancer;
                medicalCondition.ToldKidneyDisease = request.medicalCondition.ToldKidneyDisease;
                medicalCondition.OtherChronic = request.medicalCondition.OtherChronic;
                medicalCondition.OtherChronicProb = request.medicalCondition.OtherChronicProb;
                medicalCondition.HighBPMed = request.medicalCondition.HighBPMed;
                medicalCondition.HighCholMed = request.medicalCondition.HighCholMed;
                medicalCondition.DiabetesMed = request.medicalCondition.DiabetesMed;
                medicalCondition.Insulin = request.medicalCondition.Insulin;
                medicalCondition.AnginaMed = request.medicalCondition.AnginaMed;
                medicalCondition.HeartFailMed = request.medicalCondition.HeartFailMed;
                medicalCondition.HeartCondMed = request.medicalCondition.HeartCondMed;
                medicalCondition.AsthmaMed = request.medicalCondition.AsthmaMed;
                medicalCondition.BronchitisMed = request.medicalCondition.BronchitisMed;
                medicalCondition.ArthritisMed = request.medicalCondition.ArthritisMed;
                medicalCondition.OtherChronicMed = request.medicalCondition.OtherChronicMed;
                medicalCondition.BloodThinMed = request.medicalCondition.BloodThinMed;
                medicalCondition.AllergyMed = request.medicalCondition.AllergyMed;
                medicalCondition.RefluxMed = request.medicalCondition.RefluxMed;
                medicalCondition.UlcerMed = request.medicalCondition.UlcerMed;
                medicalCondition.MigraineMed = request.medicalCondition.MigraineMed;
                medicalCondition.OsteoporosisMed = request.medicalCondition.OsteoporosisMed;
                medicalCondition.AnxietyMed = request.medicalCondition.AnxietyMed;
                medicalCondition.DepressionMed = request.medicalCondition.DepressionMed;
                medicalCondition.BackPainMed = request.medicalCondition.BackPainMed;
                medicalCondition.NoPrescMed = request.medicalCondition.NoPrescMed;
                medicalCondition.Osteoarthritis = request.medicalCondition.Osteoarthritis;
                medicalCondition.OtherOrthritis = request.medicalCondition.OtherOrthritis;
                medicalCondition.Rheumatoid = request.medicalCondition.Rheumatoid;
                medicalCondition.Psoriatic = request.medicalCondition.Psoriatic;
                medicalCondition.Spondylitis = request.medicalCondition.Spondylitis;
                medicalCondition.Crohns = request.medicalCondition.Crohns;
                medicalCondition.Psoriasis = request.medicalCondition.Psoriasis;
                context.HRA_MedicalConditions.Attach(medicalCondition);
                context.Entry(medicalCondition).State = EntityState.Modified;

                if (request.StoreHistory)
                    dataUpdated = UserHistoryReader.LogUserChanges(currentMedicalCondition, medicalCondition, request.UserId, request.UpdatedByUserId, UserHistoryCategoryDto.HRA);
            }
            else
            {
                DAL.HRA_MedicalConditions newmedicalCondition = new DAL.HRA_MedicalConditions();

                newmedicalCondition.Id = request.medicalCondition.HRAId;
                newmedicalCondition.BreastFeed = request.medicalCondition.BreastFeed;
                newmedicalCondition.Pregnant = request.medicalCondition.Pregnant;
                newmedicalCondition.PregDueDate = request.medicalCondition.PregDueDate;
                newmedicalCondition.PlanBecPreg = request.medicalCondition.PlanBecPreg;
                newmedicalCondition.Hysterectomy = request.medicalCondition.Hysterectomy;
                newmedicalCondition.OvariesRemoved = request.medicalCondition.OvariesRemoved;
                newmedicalCondition.Stroke = request.medicalCondition.Stroke;
                newmedicalCondition.HeartAttack = request.medicalCondition.HeartAttack;
                newmedicalCondition.Angina = request.medicalCondition.Angina;
                newmedicalCondition.ToldBabyNine = request.medicalCondition.ToldBabyNine;
                newmedicalCondition.ToldBlock = request.medicalCondition.ToldBlock;
                newmedicalCondition.ToldHeartBlock = request.medicalCondition.ToldHeartBlock;
                newmedicalCondition.ToldHighBP = request.medicalCondition.ToldHighBP;
                newmedicalCondition.ToldHighChol = request.medicalCondition.ToldHighChol;
                newmedicalCondition.ToldDiabetes = request.medicalCondition.ToldDiabetes;
                newmedicalCondition.ToldGestDiab = request.medicalCondition.ToldGestDiab;
                newmedicalCondition.ToldPolycyst = request.medicalCondition.ToldPolycyst;
                newmedicalCondition.ToldAsthma = request.medicalCondition.ToldAsthma;
                newmedicalCondition.ToldBronchitis = request.medicalCondition.ToldBronchitis;
                newmedicalCondition.ToldCancer = request.medicalCondition.ToldCancer;
                newmedicalCondition.ToldKidneyDisease = request.medicalCondition.ToldKidneyDisease;
                newmedicalCondition.OtherChronic = request.medicalCondition.OtherChronic;
                newmedicalCondition.OtherChronicProb = request.medicalCondition.OtherChronicProb;
                newmedicalCondition.HighBPMed = request.medicalCondition.HighBPMed;
                newmedicalCondition.HighCholMed = request.medicalCondition.HighCholMed;
                newmedicalCondition.DiabetesMed = request.medicalCondition.DiabetesMed;
                newmedicalCondition.Insulin = request.medicalCondition.Insulin;
                newmedicalCondition.AnginaMed = request.medicalCondition.AnginaMed;
                newmedicalCondition.HeartFailMed = request.medicalCondition.HeartFailMed;
                newmedicalCondition.HeartCondMed = request.medicalCondition.HeartCondMed;
                newmedicalCondition.AsthmaMed = request.medicalCondition.AsthmaMed;
                newmedicalCondition.BronchitisMed = request.medicalCondition.BronchitisMed;
                newmedicalCondition.ArthritisMed = request.medicalCondition.ArthritisMed;
                newmedicalCondition.OtherChronicMed = request.medicalCondition.OtherChronicMed;
                newmedicalCondition.BloodThinMed = request.medicalCondition.BloodThinMed;
                newmedicalCondition.AllergyMed = request.medicalCondition.AllergyMed;
                newmedicalCondition.RefluxMed = request.medicalCondition.RefluxMed;
                newmedicalCondition.UlcerMed = request.medicalCondition.UlcerMed;
                newmedicalCondition.MigraineMed = request.medicalCondition.MigraineMed;
                newmedicalCondition.OsteoporosisMed = request.medicalCondition.OsteoporosisMed;
                newmedicalCondition.AnxietyMed = request.medicalCondition.AnxietyMed;
                newmedicalCondition.DepressionMed = request.medicalCondition.DepressionMed;
                newmedicalCondition.BackPainMed = request.medicalCondition.BackPainMed;
                newmedicalCondition.NoPrescMed = request.medicalCondition.NoPrescMed;
                newmedicalCondition.Osteoarthritis = request.medicalCondition.Osteoarthritis;
                newmedicalCondition.OtherOrthritis = request.medicalCondition.OtherOrthritis;
                newmedicalCondition.Rheumatoid = request.medicalCondition.Rheumatoid;
                newmedicalCondition.Psoriatic = request.medicalCondition.Psoriatic;
                newmedicalCondition.Spondylitis = request.medicalCondition.Spondylitis;
                newmedicalCondition.Crohns = request.medicalCondition.Crohns;
                newmedicalCondition.Psoriasis = request.medicalCondition.Psoriasis;
                context.HRA_MedicalConditions.Add(newmedicalCondition);
            }
            if (request.medicalCondition.Pregnant.HasValue)
            {
                var wellnessData = context.WellnessDatas.Where(x => x.SourceHRA == request.medicalCondition.HRAId).FirstOrDefault();
                if (wellnessData != null)
                {
                    if (request.medicalCondition.Pregnant == 1)
                    {
                        wellnessData.isPregnant = true;
                        wellnessData.DueDate = request.medicalCondition.PregDueDate;
                        context.WellnessDatas.Attach(wellnessData);
                        context.Entry(wellnessData).State = EntityState.Modified;
                    }
                    else
                    {
                        wellnessData.isPregnant = false;
                        wellnessData.DueDate = null;
                    }
                    wellnessData.UpdatedOn = DateTime.UtcNow;
                    wellnessData.UpdatedBy = request.UpdatedByUserId;
                }
                else if (request.medicalCondition.Pregnant == 1)
                {
                    DAL.WellnessData newWellnessData = new DAL.WellnessData();
                    newWellnessData.isPregnant = true;
                    newWellnessData.DueDate = request.medicalCondition.PregDueDate;
                    newWellnessData.SourceHRA = request.medicalCondition.HRAId;
                    newWellnessData.UserId = request.UserId;
                    newWellnessData.CollectedOn = newWellnessData.UpdatedOn = DateTime.UtcNow;
                    newWellnessData.UpdatedBy = request.UpdatedByUserId;
                    context.WellnessDatas.Add(newWellnessData);
                }
            }
            context.SaveChanges();

            if (request.medicalCondition.HRA != null)
            {
                UpdateHRADetails(request.medicalCondition.HRA.HAPageSeqDone, request.medicalCondition.HRAId, request.medicalCondition.HRA.CompleteDate, request.UpdatedByUserId, request.IsIntuityUser, DTCOrgCode, dataUpdated);

                if (request.medicalCondition.HRA.CompleteDate.HasValue)
                    StratifyHRA(request.medicalCondition.HRAId);
            }
            return response;
        }

        public bool IsSmoker(int hraId)
        {
            return context.HRA_OtherRiskFactors.Where(hra => hra.Id == hraId).First().SmokeCig == 1;
        }

        public AddEditOtherRiskFactorsResponse AddEditOtherRisks(AddEditOtherRiskFactorsRequest request, string DTCOrgCode)
        {
            AddEditOtherRiskFactorsResponse response = new AddEditOtherRiskFactorsResponse();
            var dataUpdated = false;
            var OtherRiskFactors = context.HRA_OtherRiskFactors.Where(x => x.Id == request.OtherRiskFactors.HRAId).FirstOrDefault();

            if (OtherRiskFactors != null)
            {
                DAL.HRA_OtherRiskFactors currentRiskFactors = null;
                if (request.StoreHistory == true)
                    currentRiskFactors = CloneUtil.DeepClone<DAL.HRA_OtherRiskFactors>(OtherRiskFactors);
                OtherRiskFactors.Id = request.OtherRiskFactors.HRAId;
                OtherRiskFactors.HeartHist = request.OtherRiskFactors.HeartHist;
                OtherRiskFactors.CancerHist = request.OtherRiskFactors.CancerHist;
                OtherRiskFactors.DiabetesHist = request.OtherRiskFactors.DiabetesHist;
                OtherRiskFactors.SmokeCig = request.OtherRiskFactors.SmokeCig;
                OtherRiskFactors.NoOfCig = request.OtherRiskFactors.NoOfCig;
                OtherRiskFactors.SmokeHist = request.OtherRiskFactors.SmokeHist;
                OtherRiskFactors.ECig = request.OtherRiskFactors.ECig;
                OtherRiskFactors.OtherTobacco = request.OtherRiskFactors.OtherTobacco;
                OtherRiskFactors.Cigar = request.OtherRiskFactors.Cigar;
                OtherRiskFactors.Pipe = request.OtherRiskFactors.Pipe;
                OtherRiskFactors.SmokelessTob = request.OtherRiskFactors.SmokelessTob;
                OtherRiskFactors.WaterPipes = request.OtherRiskFactors.WaterPipes;
                OtherRiskFactors.OtherFormofTob = request.OtherRiskFactors.OtherFormofTob;
                OtherRiskFactors.VigExer = request.OtherRiskFactors.VigExer;
                OtherRiskFactors.VigExerPFW = request.OtherRiskFactors.VigExerPFW;
                OtherRiskFactors.ModExer = request.OtherRiskFactors.ModExer;
                OtherRiskFactors.ModExerPFW = request.OtherRiskFactors.ModExerPFW;
                OtherRiskFactors.ExertPain = request.OtherRiskFactors.ExertPain;
                OtherRiskFactors.LowFatDiet = request.OtherRiskFactors.LowFatDiet;
                OtherRiskFactors.LowFatDietPFW = request.OtherRiskFactors.LowFatDietPFW;
                OtherRiskFactors.HealthyCarb = request.OtherRiskFactors.HealthyCarb;
                OtherRiskFactors.HealthyCarbPFW = request.OtherRiskFactors.HealthyCarbPFW;
                OtherRiskFactors.TwoAlcohol = request.OtherRiskFactors.TwoAlcohol;
                OtherRiskFactors.OneAlcohol = request.OtherRiskFactors.OneAlcohol;
                OtherRiskFactors.OverWeight = request.OtherRiskFactors.OverWeight;
                OtherRiskFactors.FeelStress = request.OtherRiskFactors.FeelStress;
                OtherRiskFactors.FeelStressPFW = request.OtherRiskFactors.FeelStressPFW;
                OtherRiskFactors.FeelAnxiety = request.OtherRiskFactors.FeelAnxiety;
                OtherRiskFactors.FeelAnxietyPFW = request.OtherRiskFactors.FeelAnxietyPFW;
                OtherRiskFactors.FeelDepression = request.OtherRiskFactors.FeelDepression;
                OtherRiskFactors.PhysicalProb = request.OtherRiskFactors.PhysicalProb;
                OtherRiskFactors.SleepApnea = request.OtherRiskFactors.SleepApnea;
                OtherRiskFactors.FeelTired = request.OtherRiskFactors.FeelTired;
                OtherRiskFactors.Snore = request.OtherRiskFactors.Snore;
                OtherRiskFactors.BreathPause = request.OtherRiskFactors.BreathPause;
                OtherRiskFactors.Headache = request.OtherRiskFactors.Headache;
                OtherRiskFactors.Sleepy = request.OtherRiskFactors.Sleepy;
                OtherRiskFactors.OnlyDepression = request.OtherRiskFactors.OnlyDepression;
                OtherRiskFactors.Arthritis = request.OtherRiskFactors.Arthritis;
                OtherRiskFactors.BreathProb = request.OtherRiskFactors.BreathProb;
                OtherRiskFactors.BackInjury = request.OtherRiskFactors.BackInjury;
                OtherRiskFactors.ChronicPain = request.OtherRiskFactors.ChronicPain;
                OtherRiskFactors.OtherPhysLimit = request.OtherRiskFactors.OtherPhysLimit;
                OtherRiskFactors.NoIssue = request.OtherRiskFactors.NoIssue;
                OtherRiskFactors.SleepApneaMed = request.OtherRiskFactors.SleepApneaMed;
                OtherRiskFactors.ExerciseFrequency = request.OtherRiskFactors.ExerciseFrequency;
                OtherRiskFactors.ExerciseDuration = request.OtherRiskFactors.ExerciseDuration;
                OtherRiskFactors.ExerciseIntensity = request.OtherRiskFactors.ExerciseIntensity;
                OtherRiskFactors.ExerciseMins = request.OtherRiskFactors.ExerciseMins;
                OtherRiskFactors.Walking = request.OtherRiskFactors.Walking;
                OtherRiskFactors.Jogging = request.OtherRiskFactors.Jogging;
                OtherRiskFactors.Treadmill = request.OtherRiskFactors.Treadmill;
                OtherRiskFactors.Cycling = request.OtherRiskFactors.Cycling;
                OtherRiskFactors.StairMach = request.OtherRiskFactors.StairMach;
                OtherRiskFactors.EllipticTrainer = request.OtherRiskFactors.EllipticTrainer;
                OtherRiskFactors.RowingMach = request.OtherRiskFactors.RowingMach;
                OtherRiskFactors.AerobicMach = request.OtherRiskFactors.AerobicMach;
                OtherRiskFactors.AerobicDance = request.OtherRiskFactors.AerobicDance;
                OtherRiskFactors.OutdoorCycling = request.OtherRiskFactors.OutdoorCycling;
                OtherRiskFactors.SwimmingLaps = request.OtherRiskFactors.SwimmingLaps;
                OtherRiskFactors.RacquetSports = request.OtherRiskFactors.RacquetSports;
                OtherRiskFactors.OtherAerobic = request.OtherRiskFactors.OtherAerobic;
                OtherRiskFactors.NoAerobic = request.OtherRiskFactors.NoAerobic;
                OtherRiskFactors.WalkingActivityEachWeek = request.OtherRiskFactors.WalkingActivityEachWeek;
                OtherRiskFactors.WalkingExerciseEachTime = request.OtherRiskFactors.WalkingExerciseEachTime;
                OtherRiskFactors.WalkingIntenseExercise = request.OtherRiskFactors.WalkingIntenseExercise;
                OtherRiskFactors.JoggingActivityEachWeek = request.OtherRiskFactors.JoggingActivityEachWeek;
                OtherRiskFactors.JoggingExerciseEachTime = request.OtherRiskFactors.JoggingExerciseEachTime;
                OtherRiskFactors.JoggingIntenseExercise = request.OtherRiskFactors.JoggingIntenseExercise;
                OtherRiskFactors.TreadmillActivityEachWeek = request.OtherRiskFactors.TreadmillActivityEachWeek;
                OtherRiskFactors.TreadmillExerciseEachTime = request.OtherRiskFactors.TreadmillExerciseEachTime;
                OtherRiskFactors.TreadmillIntenseExercise = request.OtherRiskFactors.TreadmillIntenseExercise;
                OtherRiskFactors.CyclingActivityEachWeek = request.OtherRiskFactors.CyclingActivityEachWeek;
                OtherRiskFactors.CyclingExerciseEachTime = request.OtherRiskFactors.CyclingExerciseEachTime;
                OtherRiskFactors.CyclingIntenseExercise = request.OtherRiskFactors.CyclingIntenseExercise;
                OtherRiskFactors.StairMachActivityEachWeek = request.OtherRiskFactors.StairMachActivityEachWeek;
                OtherRiskFactors.StairMachExerciseEachTime = request.OtherRiskFactors.StairMachExerciseEachTime;
                OtherRiskFactors.StairMachIntenseExercise = request.OtherRiskFactors.StairMachIntenseExercise;
                OtherRiskFactors.EllipticTrainerActivityEachWeek = request.OtherRiskFactors.EllipticTrainerActivityEachWeek;
                OtherRiskFactors.EllipticTrainerExerciseEachTime = request.OtherRiskFactors.EllipticTrainerExerciseEachTime;
                OtherRiskFactors.EllipticTrainerIntenseExercise = request.OtherRiskFactors.EllipticTrainerIntenseExercise;
                OtherRiskFactors.RowingMachActivityEachWeek = request.OtherRiskFactors.RowingMachActivityEachWeek;
                OtherRiskFactors.RowingMachExerciseEachTime = request.OtherRiskFactors.RowingMachExerciseEachTime;
                OtherRiskFactors.RowingMachIntenseExercise = request.OtherRiskFactors.RowingMachIntenseExercise;
                OtherRiskFactors.AerobicMachActivityEachWeek = request.OtherRiskFactors.AerobicMachActivityEachWeek;
                OtherRiskFactors.AerobicMachExerciseEachTime = request.OtherRiskFactors.AerobicMachExerciseEachTime;
                OtherRiskFactors.AerobicMachIntenseExercise = request.OtherRiskFactors.AerobicMachIntenseExercise;
                OtherRiskFactors.AerobicDanceActivityEachWeek = request.OtherRiskFactors.AerobicDanceActivityEachWeek;
                OtherRiskFactors.AerobicDanceExerciseEachTime = request.OtherRiskFactors.AerobicDanceExerciseEachTime;
                OtherRiskFactors.AerobicDanceIntenseExercise = request.OtherRiskFactors.AerobicDanceIntenseExercise;
                OtherRiskFactors.OutdoorCyclingActivityEachWeek = request.OtherRiskFactors.OutdoorCyclingActivityEachWeek;
                OtherRiskFactors.OutdoorCyclingExerciseEachTime = request.OtherRiskFactors.OutdoorCyclingExerciseEachTime;
                OtherRiskFactors.OutdoorCyclingIntenseExercise = request.OtherRiskFactors.OutdoorCyclingIntenseExercise;
                OtherRiskFactors.SwimmingLapsActivityEachWeek = request.OtherRiskFactors.SwimmingLapsActivityEachWeek;
                OtherRiskFactors.SwimmingLapsExerciseEachTime = request.OtherRiskFactors.SwimmingLapsExerciseEachTime;
                OtherRiskFactors.SwimmingLapsIntenseExercise = request.OtherRiskFactors.SwimmingLapsIntenseExercise;
                OtherRiskFactors.RacquetSportsActivityEachWeek = request.OtherRiskFactors.RacquetSportsActivityEachWeek;
                OtherRiskFactors.RacquetSportsExerciseEachTime = request.OtherRiskFactors.RacquetSportsExerciseEachTime;
                OtherRiskFactors.RacquetSportsIntenseExercise = request.OtherRiskFactors.RacquetSportsIntenseExercise;
                OtherRiskFactors.OtherAerobicActivityEachWeek = request.OtherRiskFactors.OtherAerobicActivityEachWeek;
                OtherRiskFactors.OtherAerobicExerciseEachTime = request.OtherRiskFactors.OtherAerobicExerciseEachTime;
                OtherRiskFactors.OtherAerobicIntenseExercise = request.OtherRiskFactors.OtherAerobicIntenseExercise;
                OtherRiskFactors.MusclesActivities = request.OtherRiskFactors.MusclesActivities;
                OtherRiskFactors.MusclesActivitiesDays = request.OtherRiskFactors.MusclesActivitiesDays;
                OtherRiskFactors.FlexibilityActivities = request.OtherRiskFactors.FlexibilityActivities;
                OtherRiskFactors.FlexibilityActivitiesDays = request.OtherRiskFactors.FlexibilityActivitiesDays;
                OtherRiskFactors.PhysicallyActiveLevel = request.OtherRiskFactors.PhysicallyActiveLevel;

                context.HRA_OtherRiskFactors.Attach(OtherRiskFactors);
                context.Entry(OtherRiskFactors).State = EntityState.Modified;

                if (request.StoreHistory == true)
                    dataUpdated = UserHistoryReader.LogUserChanges(currentRiskFactors, OtherRiskFactors, request.UserId, request.UpdatedByUserId, UserHistoryCategoryDto.HRA);
            }
            else
            {
                DAL.HRA_OtherRiskFactors newOtherRisks = new DAL.HRA_OtherRiskFactors();

                newOtherRisks.Id = request.OtherRiskFactors.HRAId;
                newOtherRisks.HeartHist = request.OtherRiskFactors.HeartHist;
                newOtherRisks.CancerHist = request.OtherRiskFactors.CancerHist;
                newOtherRisks.DiabetesHist = request.OtherRiskFactors.DiabetesHist;
                newOtherRisks.SmokeCig = request.OtherRiskFactors.SmokeCig;
                newOtherRisks.NoOfCig = request.OtherRiskFactors.NoOfCig;
                newOtherRisks.SmokeHist = request.OtherRiskFactors.SmokeHist;
                newOtherRisks.ECig = request.OtherRiskFactors.ECig;
                newOtherRisks.OtherTobacco = request.OtherRiskFactors.OtherTobacco;
                newOtherRisks.Cigar = request.OtherRiskFactors.Cigar;
                newOtherRisks.Pipe = request.OtherRiskFactors.Pipe;
                newOtherRisks.SmokelessTob = request.OtherRiskFactors.SmokelessTob;
                newOtherRisks.WaterPipes = request.OtherRiskFactors.WaterPipes;
                newOtherRisks.OtherFormofTob = request.OtherRiskFactors.OtherFormofTob;
                newOtherRisks.VigExer = request.OtherRiskFactors.VigExer;
                newOtherRisks.VigExerPFW = request.OtherRiskFactors.VigExerPFW;
                newOtherRisks.ModExer = request.OtherRiskFactors.ModExer;
                newOtherRisks.ModExerPFW = request.OtherRiskFactors.ModExerPFW;
                newOtherRisks.ExertPain = request.OtherRiskFactors.ExertPain;
                newOtherRisks.LowFatDiet = request.OtherRiskFactors.LowFatDiet;
                newOtherRisks.LowFatDietPFW = request.OtherRiskFactors.LowFatDietPFW;
                newOtherRisks.HealthyCarb = request.OtherRiskFactors.HealthyCarb;
                newOtherRisks.HealthyCarbPFW = request.OtherRiskFactors.HealthyCarbPFW;
                newOtherRisks.TwoAlcohol = request.OtherRiskFactors.TwoAlcohol;
                newOtherRisks.OneAlcohol = request.OtherRiskFactors.OneAlcohol;
                newOtherRisks.OverWeight = request.OtherRiskFactors.OverWeight;
                newOtherRisks.FeelStress = request.OtherRiskFactors.FeelStress;
                newOtherRisks.FeelStressPFW = request.OtherRiskFactors.FeelStressPFW;
                newOtherRisks.FeelAnxiety = request.OtherRiskFactors.FeelAnxiety;
                newOtherRisks.FeelAnxietyPFW = request.OtherRiskFactors.FeelAnxietyPFW;
                newOtherRisks.FeelDepression = request.OtherRiskFactors.FeelDepression;
                newOtherRisks.PhysicalProb = request.OtherRiskFactors.PhysicalProb;
                newOtherRisks.SleepApnea = request.OtherRiskFactors.SleepApnea;
                newOtherRisks.FeelTired = request.OtherRiskFactors.FeelTired;
                newOtherRisks.Snore = request.OtherRiskFactors.Snore;
                newOtherRisks.BreathPause = request.OtherRiskFactors.BreathPause;
                newOtherRisks.Headache = request.OtherRiskFactors.Headache;
                newOtherRisks.Sleepy = request.OtherRiskFactors.Sleepy;
                newOtherRisks.OnlyDepression = request.OtherRiskFactors.OnlyDepression;
                newOtherRisks.Arthritis = request.OtherRiskFactors.Arthritis;
                newOtherRisks.BreathProb = request.OtherRiskFactors.BreathProb;
                newOtherRisks.BackInjury = request.OtherRiskFactors.BackInjury;
                newOtherRisks.ChronicPain = request.OtherRiskFactors.ChronicPain;
                newOtherRisks.OtherPhysLimit = request.OtherRiskFactors.OtherPhysLimit;
                newOtherRisks.NoIssue = request.OtherRiskFactors.NoIssue;
                newOtherRisks.SleepApneaMed = request.OtherRiskFactors.SleepApneaMed;
                newOtherRisks.ExerciseFrequency = request.OtherRiskFactors.ExerciseFrequency;
                newOtherRisks.ExerciseDuration = request.OtherRiskFactors.ExerciseDuration;
                newOtherRisks.ExerciseIntensity = request.OtherRiskFactors.ExerciseIntensity;
                newOtherRisks.ExerciseMins = request.OtherRiskFactors.ExerciseMins;
                newOtherRisks.Walking = request.OtherRiskFactors.Walking;
                newOtherRisks.Jogging = request.OtherRiskFactors.Jogging;
                newOtherRisks.Treadmill = request.OtherRiskFactors.Treadmill;
                newOtherRisks.Cycling = request.OtherRiskFactors.Cycling;
                newOtherRisks.StairMach = request.OtherRiskFactors.StairMach;
                newOtherRisks.EllipticTrainer = request.OtherRiskFactors.EllipticTrainer;
                newOtherRisks.RowingMach = request.OtherRiskFactors.RowingMach;
                newOtherRisks.AerobicMach = request.OtherRiskFactors.AerobicMach;
                newOtherRisks.AerobicDance = request.OtherRiskFactors.AerobicDance;
                newOtherRisks.OutdoorCycling = request.OtherRiskFactors.OutdoorCycling;
                newOtherRisks.SwimmingLaps = request.OtherRiskFactors.SwimmingLaps;
                newOtherRisks.RacquetSports = request.OtherRiskFactors.RacquetSports;
                newOtherRisks.OtherAerobic = request.OtherRiskFactors.OtherAerobic;
                newOtherRisks.NoAerobic = request.OtherRiskFactors.NoAerobic;
                newOtherRisks.WalkingActivityEachWeek = request.OtherRiskFactors.WalkingActivityEachWeek;
                newOtherRisks.WalkingExerciseEachTime = request.OtherRiskFactors.WalkingExerciseEachTime;
                newOtherRisks.WalkingIntenseExercise = request.OtherRiskFactors.WalkingIntenseExercise;
                newOtherRisks.JoggingActivityEachWeek = request.OtherRiskFactors.JoggingActivityEachWeek;
                newOtherRisks.JoggingExerciseEachTime = request.OtherRiskFactors.JoggingExerciseEachTime;
                newOtherRisks.JoggingIntenseExercise = request.OtherRiskFactors.JoggingIntenseExercise;
                newOtherRisks.TreadmillActivityEachWeek = request.OtherRiskFactors.TreadmillActivityEachWeek;
                newOtherRisks.TreadmillExerciseEachTime = request.OtherRiskFactors.TreadmillExerciseEachTime;
                newOtherRisks.TreadmillIntenseExercise = request.OtherRiskFactors.TreadmillIntenseExercise;
                newOtherRisks.CyclingActivityEachWeek = request.OtherRiskFactors.CyclingActivityEachWeek;
                newOtherRisks.CyclingExerciseEachTime = request.OtherRiskFactors.CyclingExerciseEachTime;
                newOtherRisks.CyclingIntenseExercise = request.OtherRiskFactors.CyclingIntenseExercise;
                newOtherRisks.StairMachActivityEachWeek = request.OtherRiskFactors.StairMachActivityEachWeek;
                newOtherRisks.StairMachExerciseEachTime = request.OtherRiskFactors.StairMachExerciseEachTime;
                newOtherRisks.StairMachIntenseExercise = request.OtherRiskFactors.StairMachIntenseExercise;
                newOtherRisks.EllipticTrainerActivityEachWeek = request.OtherRiskFactors.EllipticTrainerActivityEachWeek;
                newOtherRisks.EllipticTrainerExerciseEachTime = request.OtherRiskFactors.EllipticTrainerExerciseEachTime;
                newOtherRisks.EllipticTrainerIntenseExercise = request.OtherRiskFactors.EllipticTrainerIntenseExercise;
                newOtherRisks.RowingMachActivityEachWeek = request.OtherRiskFactors.RowingMachActivityEachWeek;
                newOtherRisks.RowingMachExerciseEachTime = request.OtherRiskFactors.RowingMachExerciseEachTime;
                newOtherRisks.RowingMachIntenseExercise = request.OtherRiskFactors.RowingMachIntenseExercise;
                newOtherRisks.AerobicMachActivityEachWeek = request.OtherRiskFactors.AerobicMachActivityEachWeek;
                newOtherRisks.AerobicMachExerciseEachTime = request.OtherRiskFactors.AerobicMachExerciseEachTime;
                newOtherRisks.AerobicMachIntenseExercise = request.OtherRiskFactors.AerobicMachIntenseExercise;
                newOtherRisks.AerobicDanceActivityEachWeek = request.OtherRiskFactors.AerobicDanceActivityEachWeek;
                newOtherRisks.AerobicDanceExerciseEachTime = request.OtherRiskFactors.AerobicDanceExerciseEachTime;
                newOtherRisks.AerobicDanceIntenseExercise = request.OtherRiskFactors.AerobicDanceIntenseExercise;
                newOtherRisks.OutdoorCyclingActivityEachWeek = request.OtherRiskFactors.OutdoorCyclingActivityEachWeek;
                newOtherRisks.OutdoorCyclingExerciseEachTime = request.OtherRiskFactors.OutdoorCyclingExerciseEachTime;
                newOtherRisks.OutdoorCyclingIntenseExercise = request.OtherRiskFactors.OutdoorCyclingIntenseExercise;
                newOtherRisks.SwimmingLapsActivityEachWeek = request.OtherRiskFactors.SwimmingLapsActivityEachWeek;
                newOtherRisks.SwimmingLapsExerciseEachTime = request.OtherRiskFactors.SwimmingLapsExerciseEachTime;
                newOtherRisks.SwimmingLapsIntenseExercise = request.OtherRiskFactors.SwimmingLapsIntenseExercise;
                newOtherRisks.RacquetSportsActivityEachWeek = request.OtherRiskFactors.RacquetSportsActivityEachWeek;
                newOtherRisks.RacquetSportsExerciseEachTime = request.OtherRiskFactors.RacquetSportsExerciseEachTime;
                newOtherRisks.RacquetSportsIntenseExercise = request.OtherRiskFactors.RacquetSportsIntenseExercise;
                newOtherRisks.OtherAerobicActivityEachWeek = request.OtherRiskFactors.OtherAerobicActivityEachWeek;
                newOtherRisks.OtherAerobicExerciseEachTime = request.OtherRiskFactors.OtherAerobicExerciseEachTime;
                newOtherRisks.OtherAerobicIntenseExercise = request.OtherRiskFactors.OtherAerobicIntenseExercise;
                newOtherRisks.PhysicallyActiveLevel = request.OtherRiskFactors.PhysicallyActiveLevel;
                context.HRA_OtherRiskFactors.Add(newOtherRisks);
            }
            context.SaveChanges();

            if (request.OtherRiskFactors.HRA != null)
            {
                UpdateHRADetails(request.OtherRiskFactors.HRA.HAPageSeqDone, request.OtherRiskFactors.HRAId, request.OtherRiskFactors.HRA.CompleteDate, request.UpdatedByUserId, request.IsIntuityUser, DTCOrgCode, dataUpdated);

                if (request.OtherRiskFactors.HRA.CompleteDate.HasValue)
                    StratifyHRA(request.OtherRiskFactors.HRAId);
            }
            return response;
        }

        public AddEditHSPResponse AddEditHSP(AddEditHSPRequest request, string DTCOrgCode)
        {
            AddEditHSPResponse response = new AddEditHSPResponse();
            var dataUpdated = false;
            var HSP = context.HRA_HSP.Where(x => x.Id == request.HSP.HRAId).FirstOrDefault();

            if (HSP != null)
            {
                DAL.HRA_HSP currentHSP = null;
                if (request.StoreHistory == true)
                    currentHSP = CloneUtil.DeepClone<DAL.HRA_HSP>(HSP);
                HSP.Id = request.HSP.HRAId;
                HSP.StateOfHealth = request.HSP.StateOfHealth;
                HSP.LifeSatisfaction = request.HSP.LifeSatisfaction;
                HSP.JobSatisfaction = request.HSP.JobSatisfaction;
                HSP.RelaxMed = request.HSP.RelaxMed;
                HSP.WorkMissPers = request.HSP.WorkMissPers;
                HSP.WorkMissFam = request.HSP.WorkMissFam;
                HSP.EmergRoomVisit = request.HSP.EmergRoomVisit;
                HSP.AdmitHosp = request.HSP.AdmitHosp;
                HSP.DrVisitPers = request.HSP.DrVisitPers;
                HSP.ProductivityLoss = request.HSP.ProductivityLoss;
                HSP.TextDrive = request.HSP.TextDrive;
                HSP.DUI = request.HSP.DUI;
                HSP.RideDUI = request.HSP.RideDUI;
                HSP.RideNoBelt = request.HSP.RideNoBelt;
                HSP.Speed10 = request.HSP.Speed10;
                HSP.BikeNoHelmet = request.HSP.BikeNoHelmet;
                HSP.MBikeNoHelmet = request.HSP.MBikeNoHelmet;
                HSP.SmokeDetect = request.HSP.SmokeDetect;
                HSP.FireExting = request.HSP.FireExting;
                HSP.LiftRight = request.HSP.LiftRight;
                context.HRA_HSP.Attach(HSP);
                context.Entry(HSP).State = EntityState.Modified;

                if (request.StoreHistory == true)
                    dataUpdated = UserHistoryReader.LogUserChanges(currentHSP, HSP, request.UserId, request.UpdatedByUserId, UserHistoryCategoryDto.HRA);
            }
            else
            {
                DAL.HRA_HSP newHSP = new DAL.HRA_HSP();

                newHSP.Id = request.HSP.HRAId;
                newHSP.StateOfHealth = request.HSP.StateOfHealth;
                newHSP.LifeSatisfaction = request.HSP.LifeSatisfaction;
                newHSP.JobSatisfaction = request.HSP.JobSatisfaction;
                newHSP.RelaxMed = request.HSP.RelaxMed;
                newHSP.WorkMissPers = request.HSP.WorkMissPers;
                newHSP.WorkMissFam = request.HSP.WorkMissFam;
                newHSP.EmergRoomVisit = request.HSP.EmergRoomVisit;
                newHSP.AdmitHosp = request.HSP.AdmitHosp;
                newHSP.DrVisitPers = request.HSP.DrVisitPers;
                newHSP.ProductivityLoss = request.HSP.ProductivityLoss;
                newHSP.TextDrive = request.HSP.TextDrive;
                newHSP.DUI = request.HSP.DUI;
                newHSP.RideDUI = request.HSP.RideDUI;
                newHSP.RideNoBelt = request.HSP.RideNoBelt;
                newHSP.Speed10 = request.HSP.Speed10;
                newHSP.BikeNoHelmet = request.HSP.BikeNoHelmet;
                newHSP.MBikeNoHelmet = request.HSP.MBikeNoHelmet;
                newHSP.SmokeDetect = request.HSP.SmokeDetect;
                newHSP.FireExting = request.HSP.FireExting;
                newHSP.LiftRight = request.HSP.LiftRight;
                context.HRA_HSP.Add(newHSP);
            }

            context.SaveChanges();

            if (request.HSP.HRA != null)
            {
                UpdateHRADetails(request.HSP.HRA.HAPageSeqDone, request.HSP.HRAId, request.HSP.HRA.CompleteDate, request.UpdatedByUserId, request.IsIntuityUser, DTCOrgCode, dataUpdated);

                if (request.HSP.HRA.CompleteDate.HasValue)
                    StratifyHRA(request.HSP.HRAId);
            }
            return response;
        }

        public AddEditExamsandShotsResponse AddEditExams(AddEditExamsandShotsRequest request, string DTCOrgCode)
        {
            AddEditExamsandShotsResponse response = new AddEditExamsandShotsResponse();
            var dataUpdated = false;
            var exams = context.HRA_ExamsandShots.Where(x => x.Id == request.exams.HRAId).FirstOrDefault();

            if (exams != null)
            {
                DAL.HRA_ExamsandShots currentExams = null;
                if (request.StoreHistory == true)
                    currentExams = CloneUtil.DeepClone<DAL.HRA_ExamsandShots>(exams);
                exams.Id = request.exams.HRAId;
                exams.PhysicalExam = request.exams.PhysicalExam;
                exams.StoolTest = request.exams.StoolTest;
                exams.ColStoolTest = request.exams.ColStoolTest;
                exams.SigTest = request.exams.SigTest;
                exams.ColTest = request.exams.ColTest;
                exams.PSATest = request.exams.PSATest;
                exams.PapTest = request.exams.PapTest;
                exams.BoneTest = request.exams.BoneTest;
                exams.Mammogram = request.exams.Mammogram;
                exams.DentalExam = request.exams.DentalExam;
                exams.BPCheck = request.exams.BPCheck;
                exams.CholTest = request.exams.CholTest;
                exams.GlucoseTest = request.exams.GlucoseTest;
                exams.EyeExam = request.exams.EyeExam;
                exams.NoTest = request.exams.NoTest;
                exams.TetanusShot = request.exams.TetanusShot;
                exams.FluShot = request.exams.FluShot;
                exams.MMR = request.exams.MMR;
                exams.Varicella = request.exams.Varicella;
                exams.HepBShot = request.exams.HepBShot;
                exams.ShinglesShot = request.exams.ShinglesShot;
                exams.HPVShot = request.exams.HPVShot;
                exams.PneumoniaShot = request.exams.PneumoniaShot;
                exams.NoShots = request.exams.NoShots;
                context.HRA_ExamsandShots.Attach(exams);
                context.Entry(exams).State = EntityState.Modified;

                if (request.StoreHistory == true)
                    dataUpdated = UserHistoryReader.LogUserChanges(currentExams, exams, request.UserId, request.UpdatedByUserId, UserHistoryCategoryDto.HRA);
            }
            else
            {
                DAL.HRA_ExamsandShots newExams = new DAL.HRA_ExamsandShots();

                newExams.Id = request.exams.HRAId;
                newExams.PhysicalExam = request.exams.PhysicalExam;
                newExams.StoolTest = request.exams.StoolTest;
                newExams.SigTest = request.exams.SigTest;
                newExams.ColTest = request.exams.ColTest;
                newExams.ColStoolTest = request.exams.ColStoolTest;
                newExams.PSATest = request.exams.PSATest;
                newExams.PapTest = request.exams.PapTest;
                newExams.BoneTest = request.exams.BoneTest;
                newExams.Mammogram = request.exams.Mammogram;
                newExams.DentalExam = request.exams.DentalExam;
                newExams.BPCheck = request.exams.BPCheck;
                newExams.CholTest = request.exams.CholTest;
                newExams.GlucoseTest = request.exams.GlucoseTest;
                newExams.EyeExam = request.exams.EyeExam;
                newExams.NoTest = request.exams.NoTest;
                newExams.TetanusShot = request.exams.TetanusShot;
                newExams.FluShot = request.exams.FluShot;
                newExams.MMR = request.exams.MMR;
                newExams.Varicella = request.exams.Varicella;
                newExams.HepBShot = request.exams.HepBShot;
                newExams.ShinglesShot = request.exams.ShinglesShot;
                newExams.HPVShot = request.exams.HPVShot;
                newExams.PneumoniaShot = request.exams.PneumoniaShot;
                newExams.NoShots = request.exams.NoShots;
                context.HRA_ExamsandShots.Add(newExams);
            }

            context.SaveChanges();

            if (request.exams.HRA != null)
            {
                UpdateHRADetails(request.exams.HRA.HAPageSeqDone, request.exams.HRAId, request.exams.HRA.CompleteDate, request.UpdatedByUserId, request.IsIntuityUser, DTCOrgCode, dataUpdated);

                if (request.exams.HRA.CompleteDate.HasValue)
                    StratifyHRA(request.exams.HRAId);
            }
            return response;
        }

        public AddEditInterestsResponse AddEditInterest(AddEditInterestsRequest request, string DTCOrgCode)
        {
            AddEditInterestsResponse response = new AddEditInterestsResponse();
            var dataUpdated = false;
            var interest = context.HRA_Interests.Where(x => x.Id == request.interest.HRAId).FirstOrDefault();

            if (interest != null)
            {
                DAL.HRA_Interests currentInterest = null;
                if (request.StoreHistory == true)
                    currentInterest = CloneUtil.DeepClone<DAL.HRA_Interests>(interest);
                interest.Id = request.interest.HRAId;
                interest.WeightManProg = request.interest.WeightManProg;
                interest.MaternityProg = request.interest.MaternityProg;
                interest.NutProg = request.interest.NutProg;
                interest.StressManProg = request.interest.StressManProg;
                interest.QuitSmokeProg = request.interest.QuitSmokeProg;
                interest.ExerProg = request.interest.ExerProg;
                interest.CompProg = request.interest.CompProg;
                context.HRA_Interests.Attach(interest);
                context.Entry(interest).State = EntityState.Modified;
                if (request.StoreHistory == true)
                    dataUpdated = UserHistoryReader.LogUserChanges(currentInterest, interest, request.UserId, request.UpdatedByUserId, UserHistoryCategoryDto.HRA);
            }
            else
            {
                DAL.HRA_Interests newInterest = new DAL.HRA_Interests();

                newInterest.Id = request.interest.HRAId;
                newInterest.WeightManProg = request.interest.WeightManProg;
                newInterest.MaternityProg = request.interest.MaternityProg;
                newInterest.NutProg = request.interest.NutProg;
                newInterest.StressManProg = request.interest.StressManProg;
                newInterest.QuitSmokeProg = request.interest.QuitSmokeProg;
                newInterest.ExerProg = request.interest.ExerProg;
                newInterest.CompProg = request.interest.CompProg;
                context.HRA_Interests.Add(newInterest);
            }

            context.SaveChanges();

            if (request.interest.HRA != null)
            {
                UpdateHRADetails(request.interest.HRA.HAPageSeqDone, request.interest.HRAId, request.interest.HRA.CompleteDate, request.UpdatedByUserId, request.IsIntuityUser, DTCOrgCode, dataUpdated);

                if (request.interest.HRA.CompleteDate.HasValue)
                    StratifyHRA(request.interest.HRAId);
            }
            return response;
        }

        public bool AddEditSmokeDependentQuestions(AddEditSmokeDependentQuestions request)
        {
            AddEditInterestsResponse response = new AddEditInterestsResponse();
            var interest = context.HRA_Interests.Where(x => x.Id == request.HRAId).FirstOrDefault();
            if (interest != null)
            {
                HRA_Interests currentInterest = null;
                if (request.StoreHistory == true)
                    currentInterest = CloneUtil.DeepClone<HRA_Interests>(interest);
                if (!request.isSmoker)
                    interest.QuitSmokeProg = null;
                context.HRA_Interests.Attach(interest);
                context.Entry(interest).State = EntityState.Modified;
                if (request.StoreHistory == true)
                    UserHistoryReader.LogUserChanges(currentInterest, interest, request.UserId, request.UpdatedByUserId, UserHistoryCategoryDto.HRA);
            }
            context.SaveChanges();
            return true;
        }

        public void UpdateHealthNumbersFromLab(UpdateHealthNumbersFromLabRequest request)
        {
            var hra = context.HRAs.Where(x => x.UserId == request.lab.UserId && x.PortalId == request.lab.PortalId).FirstOrDefault();
            if (hra != null)
            {
                var healthNumbersDAL = context.HRA_HealthNumbers.Where(x => x.Id == hra.Id).FirstOrDefault();
                if (healthNumbersDAL != null)
                {
                    if (healthNumbersDAL.LabId.HasValue && !request.overrideCurrentValue)
                        return;
                    DAL.HRA_HealthNumbers currentNumbers = null;
                    healthNumbersDAL.HRA = null;
                    currentNumbers = CloneUtil.DeepClone<DAL.HRA_HealthNumbers>(healthNumbersDAL);
                    healthNumbersDAL.BPArm = request.lab.BPArm;
                    healthNumbersDAL.HDL = request.lab.HDL;
                    healthNumbersDAL.LDL = request.lab.LDL;
                    healthNumbersDAL.TotalChol = request.lab.TotalChol;
                    healthNumbersDAL.Trig = request.lab.Trig;
                    healthNumbersDAL.A1C = request.lab.A1C;
                    if (request.lab.SBP.HasValue && request.lab.DBP.HasValue)
                    {
                        healthNumbersDAL.SBP = request.lab.SBP;
                        healthNumbersDAL.DBP = request.lab.DBP;
                    }
                    healthNumbersDAL.Glucose = request.lab.Glucose;
                    healthNumbersDAL.DidYouFast = request.lab.DidYouFast;
                    healthNumbersDAL.BloodTestDate = request.lab.BloodTestDate;
                    healthNumbersDAL.LabId = request.lab.Id;
                    if (request.lab.Height.HasValue)
                        healthNumbersDAL.Height = request.lab.Height;
                    if (request.lab.Weight.HasValue)
                        healthNumbersDAL.Weight = request.lab.Weight;
                    if (request.lab.BMI.HasValue)
                        healthNumbersDAL.BMI = request.lab.BMI;
                    if (request.lab.Waist.HasValue)
                        healthNumbersDAL.Waist = request.lab.Waist;
                    var assessmentDate = hra.CompleteDate;
                    var validDays = request.HRAValidity;
                    if (request.lab.BloodTestDate >= assessmentDate)
                    {
                        var userinProgram = context.UsersinPrograms.Where(x => x.UserId == request.lab.UserId && x.IsActive == true && x.StartDate.HasValue && x.HRAId == hra.Id).FirstOrDefault();
                        if (userinProgram != null)
                        {
                            assessmentDate = userinProgram.StartDate.Value;
                            validDays = Constants.HRAProgramValidity;
                        }
                    }
                    if (hra.CompleteDate.HasValue && assessmentDate.HasValue && request.lab.BloodTestDate.HasValue && IsValidStratificationWindow(new ValidateStratificationRequest { AssessmentCompletionDate = assessmentDate.Value, labCompletionDate = request.lab.BloodTestDate.Value, HRAValidity = validDays }).success)
                    {
                        context.HRA_HealthNumbers.Attach(healthNumbersDAL);
                        context.Entry(healthNumbersDAL).State = EntityState.Modified;
                        context.SaveChanges();
                        StratifyHRA(hra.Id);
                        //update corresponding wellness data record
                        ParticipantReader participantReader = new ParticipantReader();
                        AddEditWellnessDataRequest WDrequest = new AddEditWellnessDataRequest();
                        WDrequest.HRAId = healthNumbersDAL.Id;
                        WDrequest.WellnessData = new WellnessDataDto();
                        WDrequest.WellnessData.Weight = healthNumbersDAL.Weight;
                        WDrequest.WellnessData.SBP = healthNumbersDAL.SBP;
                        WDrequest.WellnessData.DBP = healthNumbersDAL.DBP;
                        WDrequest.WellnessData.waist = healthNumbersDAL.Waist;
                        WDrequest.WellnessData.UserId = request.lab.UserId;
                        WDrequest.WellnessData.UpdatedBy = request.updatedBy;
                        participantReader.AddtoWellnessData(WDrequest);
                        //update additionalLab
                        LabReader labreader = new LabReader();
                        labreader.UpdateAdditionalLab(request.lab.Id);
                        UserHistoryReader.LogUserChanges(currentNumbers, healthNumbersDAL, request.lab.UserId, request.updatedBy, UserHistoryCategoryDto.HealthNumbers);
                    }
                    var numbers = Utility.mapper.Map<DAL.HRA_HealthNumbers, HealthNumbersDto>(healthNumbersDAL);
                    numbers.HRA = null;
                    numbers.HRAId = healthNumbersDAL.Id;
                    if (hasCompletedHealthNumbers(numbers))
                    {
                        ParticipantReader participantreader = new ParticipantReader();
                        MedicalPlanEligbilityRequest medrequest = new MedicalPlanEligbilityRequest();
                        medrequest.portalId = hra.PortalId;
                        medrequest.participantId = hra.UserId;
                        medrequest.checkValidCode = false;
                        medrequest.isProgram = false;
                        var medresponse = participantreader.MedicalPlanEligbility(medrequest);
                        IncentiveReader incentiveReader = new IncentiveReader();
                        AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                        incentivesRequest.incentiveType = IncentiveTypes.HRA_HealthNumbers;
                        incentivesRequest.userId = request.lab.UserId;
                        incentivesRequest.portalId = request.lab.PortalId;
                        incentivesRequest.isEligible = medresponse.IsEligbilble;
                        incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.HRA_HealthNumbersIncentive;
                        incentiveReader.AwardIncentives(incentivesRequest);
                    }
                }
                else if (request.saveNew)
                {
                    healthNumbersDAL = new HRA_HealthNumbers();
                    healthNumbersDAL.Id = hra.Id;
                    healthNumbersDAL.BPArm = request.lab.BPArm;
                    healthNumbersDAL.HDL = request.lab.HDL;
                    healthNumbersDAL.LDL = request.lab.LDL;
                    healthNumbersDAL.TotalChol = request.lab.TotalChol;
                    healthNumbersDAL.Trig = request.lab.Trig;
                    healthNumbersDAL.A1C = request.lab.A1C;
                    healthNumbersDAL.SBP = request.lab.SBP;
                    healthNumbersDAL.DBP = request.lab.DBP;
                    healthNumbersDAL.Glucose = request.lab.Glucose;
                    healthNumbersDAL.DidYouFast = request.lab.DidYouFast;
                    healthNumbersDAL.BloodTestDate = request.lab.BloodTestDate;
                    if (request.lab.Height.HasValue)
                        healthNumbersDAL.Height = request.lab.Height;
                    if (request.lab.Weight.HasValue)
                        healthNumbersDAL.Weight = request.lab.Weight;
                    if (request.lab.BMI.HasValue)
                        healthNumbersDAL.BMI = request.lab.BMI;
                    if (request.lab.Waist.HasValue)
                        healthNumbersDAL.Waist = request.lab.Waist;
                    context.HRA_HealthNumbers.Add(healthNumbersDAL);
                    context.SaveChanges();
                }
            }
        }

        public AddEditHealthNumbersResponse AddEditHealthNumbers(AddEditHealthNumbersRequest request, string DTCOrgCode)
        {
            AddEditHealthNumbersResponse response = new AddEditHealthNumbersResponse();
            var dataUpdated = false;
            var numbers = context.HRA_HealthNumbers.Where(x => x.Id == request.HealthNumbers.HRAId).FirstOrDefault();
            if (numbers != null)
            {
                DAL.HRA_HealthNumbers currentNumbers = null;
                if (request.StoreHistory == true)
                    currentNumbers = CloneUtil.DeepClone<DAL.HRA_HealthNumbers>(numbers);
                numbers.Id = request.HealthNumbers.HRAId;
                if (request.bloodwork)
                {
                    numbers.DidYouFast = request.HealthNumbers.DidYouFast;
                    numbers.BloodTestDate = request.HealthNumbers.BloodTestDate;
                    numbers.TotalChol = request.HealthNumbers.TotalChol;
                    numbers.LDL = request.HealthNumbers.LDL;
                    numbers.HDL = request.HealthNumbers.HDL;
                    numbers.Trig = request.HealthNumbers.Trig;
                    numbers.Glucose = request.HealthNumbers.Glucose;
                    numbers.A1C = request.HealthNumbers.A1C;
                }
                else
                {
                    numbers.BPArm = request.HealthNumbers.BPArm;
                    numbers.SBP = request.HealthNumbers.SBP;
                    numbers.DBP = request.HealthNumbers.DBP;
                    numbers.Weight = request.HealthNumbers.Weight;
                    numbers.Height = request.HealthNumbers.Height;
                    numbers.Waist = request.HealthNumbers.Waist;
                    numbers.RMR = request.HealthNumbers.RMR;
                    numbers.THRFrom = request.HealthNumbers.THRFrom;
                    numbers.THRTo = request.HealthNumbers.THRTo;
                    numbers.CAC = request.HealthNumbers.CAC;
                    numbers.DesiredWeight = request.HealthNumbers.DesiredWeight;
                    numbers.CRF = request.HealthNumbers.CRF;
                    numbers.RHR = request.HealthNumbers.RHR;
                    context.HRA_HealthNumbers.Attach(numbers);
                    context.Entry(numbers).State = EntityState.Modified;

                    if (request.StoreHistory == true)
                        dataUpdated = UserHistoryReader.LogUserChanges(currentNumbers, numbers, request.UserId, request.UpdatedByUserId, UserHistoryCategoryDto.HRA);
                }
            }
            else
            {
                DAL.HRA_HealthNumbers newHealthNumbers = new DAL.HRA_HealthNumbers();
                newHealthNumbers.Id = request.HealthNumbers.HRAId;
                if (request.bloodwork)
                {
                    newHealthNumbers.DidYouFast = request.HealthNumbers.DidYouFast;
                    newHealthNumbers.BloodTestDate = request.HealthNumbers.BloodTestDate;
                    newHealthNumbers.TotalChol = request.HealthNumbers.TotalChol;
                    newHealthNumbers.LDL = request.HealthNumbers.LDL;
                    newHealthNumbers.HDL = request.HealthNumbers.HDL;
                    newHealthNumbers.Trig = request.HealthNumbers.Trig;
                    newHealthNumbers.Glucose = request.HealthNumbers.Glucose;
                    newHealthNumbers.A1C = request.HealthNumbers.A1C;
                }
                else
                {
                    newHealthNumbers.BPArm = request.HealthNumbers.BPArm;
                    newHealthNumbers.SBP = request.HealthNumbers.SBP;
                    newHealthNumbers.DBP = request.HealthNumbers.DBP;
                    newHealthNumbers.Weight = request.HealthNumbers.Weight;
                    newHealthNumbers.Height = request.HealthNumbers.Height;
                    newHealthNumbers.Waist = request.HealthNumbers.Waist;
                    newHealthNumbers.RMR = request.HealthNumbers.RMR;
                    newHealthNumbers.THRFrom = request.HealthNumbers.THRFrom;
                    newHealthNumbers.THRTo = request.HealthNumbers.THRTo;
                    newHealthNumbers.CAC = request.HealthNumbers.CAC;
                    newHealthNumbers.DesiredWeight = request.HealthNumbers.DesiredWeight;
                    newHealthNumbers.CRF = request.HealthNumbers.CRF;
                    newHealthNumbers.RHR = request.HealthNumbers.RHR;
                }
                context.HRA_HealthNumbers.Add(newHealthNumbers);
            }
            context.SaveChanges();
            if (request.HealthNumbers.HRA != null)
            {
                UpdateHRADetails(request.HealthNumbers.HRA.HAPageSeqDone, request.HealthNumbers.HRAId, request.HealthNumbers.HRA.CompleteDate, request.UpdatedByUserId, request.IsIntuityUser, DTCOrgCode, dataUpdated);
                if (request.HealthNumbers.HRA.CompleteDate.HasValue)
                    StratifyHRA(request.HealthNumbers.HRAId);
            }
            if (request.addtoWellnessData == true && request.HealthNumbers.Weight != null)
            {
                ParticipantReader participantReader = new ParticipantReader();
                AddEditWellnessDataRequest WDrequest = new AddEditWellnessDataRequest();
                WDrequest.HRAId = request.HealthNumbers.HRAId;
                WDrequest.WellnessData = new WellnessDataDto();
                WDrequest.WellnessData.Weight = request.HealthNumbers.Weight;
                WDrequest.WellnessData.SBP = request.HealthNumbers.SBP;
                WDrequest.WellnessData.DBP = request.HealthNumbers.DBP;
                WDrequest.WellnessData.waist = request.HealthNumbers.Waist;
                WDrequest.WellnessData.UserId = request.HealthNumbers.HRA.UserId;
                WDrequest.WellnessData.UpdatedBy = request.UpdatedByUserId;
                participantReader.AddtoWellnessData(WDrequest);
                AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                healthDataRequest.HealthData = new HealthDataDto();
                healthDataRequest.HealthData.UserId = request.UserId;
                healthDataRequest.HealthData.Weight = request.HealthNumbers.Weight.Value;
                healthDataRequest.HealthData.Source = (int)HealthDataSource.HRA;
                healthDataRequest.HealthData.CreatedBy = request.UpdatedByUserId;
                healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                participantReader.AddtoHealthData(healthDataRequest);
            }
            return response;
        }

        public void UpdateHRADetails(string page, int haId, DateTime? completeDate, int UpdatedBy, bool isIntuityUser, string DTCOrgCode, bool? dataUpdated = null)
        {
            MedicalPlanEligbilityResponse response = new MedicalPlanEligbilityResponse();
            var HRA = context.HRAs.Include("User").Include("User.Organization").Where(x => x.Id == haId).FirstOrDefault();
            if (HRA != null && !HRA.CompleteDate.HasValue)
            {
                if (!string.IsNullOrEmpty(page))
                    HRA.HAPageSeqDone = page;
                if (completeDate.HasValue)
                {
                    HRA.CompleteDate = completeDate;
                    if (isIntuityUser)
                    {
                        IntuityReader intuityReader = new IntuityReader();
                        AddIntuityEventRequest intuityEventRequest = new AddIntuityEventRequest();
                        intuityEventRequest.intuityEvent = new IntuityEventDto();
                        intuityEventRequest.intuityEvent.UserId = HRA.User.Id;
                        intuityEventRequest.organizationCode = HRA.User.Organization.Code;
                        intuityEventRequest.intuityEvent.UniqueId = HRA.User.UniqueId;
                        intuityEventRequest.intuityEvent.EventType = (int)IntuityEventTypes.HRA_Completion;
                        intuityEventRequest.intuityEvent.CreatedBy = UpdatedBy;
                        intuityEventRequest.intuityEvent.EventDate = HRA.CompleteDate;
                        intuityReader.AddIntuityEvent(intuityEventRequest, DTCOrgCode);
                    }
                }
                context.HRAs.Attach(HRA);
                context.Entry(HRA).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (completeDate.HasValue)
            {
                ParticipantReader participantreader = new ParticipantReader();
                MedicalPlanEligbilityRequest request = new MedicalPlanEligbilityRequest();
                request.portalId = HRA.PortalId;
                request.participantId = HRA.UserId;
                request.checkValidCode = false;
                request.isProgram = false;
                response = participantreader.MedicalPlanEligbility(request);
                IncentiveReader reader = new IncentiveReader();
                AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                incentivesRequest.incentiveType = IncentiveTypes.HRA_Completion;
                incentivesRequest.userId = HRA.UserId;
                incentivesRequest.portalId = HRA.PortalId;
                incentivesRequest.isEligible = response.IsEligbilble;
                incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Incentive;
                incentivesRequest.companyIncentiveMessage = IncentiveMessageTypes.HRA_Incentive;
                reader.AwardIncentives(incentivesRequest);
            }
            if (HRA.HRA_HealthNumbers != null)
            {
                var healthNumber = Utility.mapper.Map<DAL.HRA_HealthNumbers, HealthNumbersDto>(HRA.HRA_HealthNumbers);
                if (hasCompletedHealthNumbers(healthNumber))
                {
                    IncentiveReader reader = new IncentiveReader();
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.HRA_HealthNumbers;
                    incentivesRequest.userId = HRA.UserId;
                    incentivesRequest.portalId = HRA.PortalId;
                    incentivesRequest.isEligible = response.IsEligbilble;
                    incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.HRA_HealthNumbersIncentive;
                    reader.AwardIncentives(incentivesRequest);
                }
            }
            if (dataUpdated.HasValue && dataUpdated.Value)
            {
                HRA.UpdatedBy = UpdatedBy;
                HRA.UpdatedOn = DateTime.UtcNow;
                context.HRAs.Attach(HRA);
                context.Entry(HRA).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public bool hasCompletedHealthNumbers(HealthNumbersDto healthNumbers)
        {
            return (healthNumbers.BloodTestDate.HasValue && healthNumbers.TotalChol.HasValue
                && healthNumbers.Trig.HasValue && healthNumbers.LDL.HasValue && healthNumbers.HDL.HasValue
                && (healthNumbers.Glucose.HasValue || healthNumbers.A1C.HasValue));
        }

        public void StratifyHRA(int HRAId)
        {
            StoredProcedures sp = new StoredProcedures();
            sp.StratifyHRA(HRAId);
        }

        public ReadHRAResponse ReadHRA(ReadHRARequest request)
        {
            ReadHRAResponse response = new ReadHRAResponse();
            var hra = context.HRAs.Include("User").Include("User.State1").Include("Portal").Include("HRA_ExamsandShots").Include("HRA_Interests").Include("HRA_HSP")
                .Include("HRA_MedicalConditions").Include("HRA_HealthNumbers").Include("HRA_OtherRiskFactors").Include("HRA_Goals").Include("HRA_HealthNumbers.Lab").Include("ReportFeedbacks")
                .Where(x => x.Id == request.hraId).FirstOrDefault();
            response.hra = Utility.mapper.Map<DAL.HRA, HRADto>(hra);
            return response;
        }

        public ReadHRAResponse ReadHRAByPortal(ReadHRARequest request)
        {
            ReadHRAResponse response = new ReadHRAResponse();
            var hra = context.HRAs.Include("User").Include("User.State1").Include("HRA_ExamsandShots").Include("HRA_Interests").Include("HRA_HSP")
                .Include("HRA_MedicalConditions").Include("HRA_HealthNumbers").Include("HRA_OtherRiskFactors").Include("HRA_Goals")
                .Where(x => x.UserId == request.userId && x.PortalId == request.portalId).FirstOrDefault();
            response.hra = Utility.mapper.Map<DAL.HRA, HRADto>(hra);
            return response;
        }

        public GetPregnencyDaysResponse GetPregnencyDays(GetPregnencyDaysRequest request)
        {
            GetPregnencyDaysResponse response = new GetPregnencyDaysResponse();
            if (!request.PregDueDate.HasValue)
                request.PregDueDate = context.HRA_MedicalConditions.Where(x => x.Id == request.hraId).FirstOrDefault().PregDueDate;
            if (!request.PregDueDate.HasValue)
                request.PregDueDate = context.WellnessDatas.Where(x => x.DueDate != null && x.UserId == request.userId).OrderByDescending(x => x.Id).FirstOrDefault()?.DueDate;
            if (request.PregDueDate.HasValue)
            {
                TimeSpan ts = request.PregDueDate.Value - request.startDate;
                var daysToGo = ts.Days;
                if (daysToGo < 0)
                    response.days = 0;
                response.days = (280 - daysToGo);
                return response;
            }
            response.days = 0;
            return response;
        }

        public DAL.HRA GetHRAForLab(int userId, int portalId, bool includeNumbers)
        {
            if (!includeNumbers)
                return context.HRAs.Where(x => x.UserId == userId && x.PortalId == portalId).FirstOrDefault();
            else
                return context.HRAs.Include("HRA_HealthNumbers").Where(x => x.UserId == userId && x.PortalId == portalId).FirstOrDefault();
        }

        public GetAllHRAsforUserResponse GetAllHRAsforUser(GetAllHRAsforUserRequest request)
        {
            GetAllHRAsforUserResponse response = new GetAllHRAsforUserResponse();
            if (request.IncludeInactivePortalHRAs == true)
            {
                var HRAs = context.HRAs.Include("Portal").Where(x => x.UserId == request.UserId).ToList();
                response.HRAs = Utility.mapper.Map<IList<DAL.HRA>, IList<HRADto>>(HRAs);
            }
            else
            {
                var HRAs = context.HRAs.Include("Portal").Where(x => x.UserId == request.UserId && x.Portal.Active == true && x.CompleteDate.HasValue).Select(i => i.Id).ToList();
                response.HRAIds = HRAs;
            }
            return response;
        }

        public ReadHRAResponse ReadFollowup(ReadFollowUpRequest request)
        {
            ReadHRAResponse response = new ReadHRAResponse();
            var followup = context.FollowUps.Include("UsersinProgram").Include("FollowUp_HealthNumbers")
                .Where(x => x.Id == request.FollowUpId).FirstOrDefault();

            var hra = context.HRAs.Include("User").Include("User.State1").Include("HRA_ExamsandShots").Include("HRA_Interests").Include("HRA_HSP")
                .Include("HRA_MedicalConditions").Include("HRA_HealthNumbers").Include("HRA_OtherRiskFactors").Include("HRA_Goals")
                .Where(x => x.Id == followup.UsersinProgram.HRAId).FirstOrDefault();

            response.hra = Utility.mapper.Map<DAL.HRA, HRADto>(hra);
            if (response.hra != null)
                response.hra.FollowupHealthNumber = Utility.mapper.Map<DAL.FollowUp_HealthNumbers, FollowUp_HealthNumbersDto>(followup.FollowUp_HealthNumbers);
            return response;
        }

        public ChangeTelHRAResponse ChangeTelHRA(ChangeTelHRARequest request)
        {
            ChangeTelHRAResponse response = new ChangeTelHRAResponse();
            if (request.HRAId > 0 && request.CreatedBy > 0)
            {
                var HRAdata = context.HRAs.Where(x => x.Id == request.HRAId).FirstOrDefault();
                if (HRAdata != null)
                {
                    HRAdata.CreatedBy = request.CreatedBy;
                    context.HRAs.Attach(HRAdata);
                    context.Entry(HRAdata).State = EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    response.success = false;
                    return response;
                }
            }
            response.success = true;
            return response;
        }

        public ValidateStratificationResponse IsValidStratificationWindow(ValidateStratificationRequest request)
        {
            ValidateStratificationResponse response = new ValidateStratificationResponse();
            response.success = true;
            TimeSpan diff = request.AssessmentCompletionDate.Subtract(request.labCompletionDate);
            if (diff.Days > 365 || diff.Days < -request.HRAValidity)
                response.success = false;

            return response;
        }

        public HRAQuestionResponse GetHRAQuestions(GetHRAQuestionRequest request)
        {
            HRAQuestionResponse response = new HRAQuestionResponse();
            var questions = context.Questions.Where(x => x.IsActive).ToList();
            response.Questions = Utility.mapper.Map<IList<DAL.Question>, IList<QuestionDto>>(questions);
            return response;
        }
    }
}