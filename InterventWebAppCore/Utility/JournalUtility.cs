using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class JournalUtility
    {
        #region Stress Diary

        public static object AddtoStressDiary(StressDiaryModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            StressDiaryDto stress = new StressDiaryDto();
            stress = model.stressDiary;
            stress.UserId = participantId;
            stress.Active = true;
            return reader.AddtoStressDiary(stress);
        }

        public static ListStressDiaryResponse ListStress(DiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListStressDiaryRequest request = new ListStressDiaryRequest();
            request.userId = participantId;
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            return reader.ListStress(request);
        }

        public static ReadStressResponse ReadStress(int id)
        {
            JournalReader reader = new JournalReader();
            ReadFoodRequest request = new ReadFoodRequest();
            request.id = id;
            return reader.ReadStress(request);

        }

        #endregion

        #region Weight Loss Diary

        public static ListWeightLossResponse ListWeightLossJournal(DiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListWeightLossRequest request = new ListWeightLossRequest();
            request.userId = participantId;
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            return reader.ListWeightLossJournal(request);
        }

        public static WeightLossJournalDto WeightLossDetails(int id)
        {
            JournalReader reader = new JournalReader();
            return reader.WeightLossDetails(id);
        }

        public static bool AddToWeightLossJournal(WeightLossJournalDto weightLoss, int userId, int participantId)
        {
            JournalReader reader = new JournalReader();
            weightLoss.UserId = participantId;
            weightLoss.UpdatedBy = userId;
            return reader.AddToWeightLossJournal(weightLoss);
        }

        #endregion

        #region Food Diary

        public static ListFoodGroupResponse ListFoodGroup()
        {
            JournalReader reader = new JournalReader();
            ListFoodGroupRequest request = new ListFoodGroupRequest();
            return reader.ListFoodGroup(request);
        }

        public static ListMealTypeResponse ListMealType()
        {
            JournalReader reader = new JournalReader();
            ListMealTypeRequest request = new ListMealTypeRequest();
            return reader.ListMealType(request);
        }

        public static object AddtoFoodDiary(FoodDiaryDetailsModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            FoodDiaryDto foodDiary = new FoodDiaryDto();
            foodDiary = model.FoodDiary;
            foodDiary.UserId = participantId;
            return reader.AddtoFoodDiary(foodDiary);
        }

        public static ListFoodDairyResponse ListFood(FoodDiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListFoodDiaryRequest request = new ListFoodDiaryRequest();
            request.ParticipantId = participantId;
            request.startDate = model.startDate.Date;
            request.endDate = model.endDate.Date;
            return reader.ListFood(request);
        }


        public static DeleteFoodResponse DeleteFood(int foodId)
        {
            JournalReader reader = new JournalReader();
            DeleteFoodRequest request = new DeleteFoodRequest();
            request.foodId = foodId;
            return reader.DeleteFood(request);
        }

        public static ListFoodDairyResponse ListFoodDetails(FoodDiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListFoodDiaryRequest request = new ListFoodDiaryRequest();
            request.ParticipantId = participantId;
            request.startDate = model.startDate.Date;
            return reader.ListFoodDetails(request);
        }

        #endregion

        #region Exercise Diary

        public static object AddtoExerciseDiary(int? id, DateTime ExDate, int Type, int? Duration, byte? RPE, short? HeartRate, int? Points, int? StepsPerDay, string Notes, int participantId)
        {
            JournalReader reader = new JournalReader();
            ExerciseDiaryDto Ex = new ExerciseDiaryDto();

            if (id.HasValue)
                Ex.Id = id.Value;
            Ex.UserId = participantId;
            Ex.Date = ExDate;
            Ex.Type = Type;
            Ex.Duration = Duration;
            Ex.RPE = RPE;
            Ex.HeartRate = HeartRate;
            Ex.Points = Points;
            Ex.StepsPerDay = StepsPerDay;
            Ex.Notes = Notes;
            return reader.AddtoExerciseDiary(Ex);
        }

        public static ListExerciseDairyResponse ListExercise(DiaryListModel model, int participanntId)
        {
            JournalReader reader = new JournalReader();
            ListExerciseDairyRequest request = new ListExerciseDairyRequest();
            request.ParticipantId = participanntId;
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            request.startDate = model.startDate;
            request.endDate = model.endDate;
            return reader.ListExercise(request);

        }

        public static ListExerciseTypeResponse ListExerciseType()
        {
            JournalReader reader = new JournalReader();
            return reader.ListExerciseType();
        }

        public static ReadExerciseDiaryResponse ReadExercise(int id)
        {
            JournalReader reader = new JournalReader();
            ReadExerciseDiaryRequest request = new ReadExerciseDiaryRequest();
            request.id = id;
            return reader.ReadExercise(request);

        }

        public static double GetExercisePoints(string Exercise, int Internsity, int Duration)
        {
            int vNum = 0;
            if (Internsity >= 12 && Internsity <= 13)
                vNum = 2;
            else if (Internsity > 13)
                vNum = 4;
            return ((Convert.ToInt16(Exercise.Substring(vNum, 2)) * Duration) / 100);
        }

        #endregion

        #region Tobacco Log
        public static bool AddtoTobaccoLog(TobaccoLogModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            TobaccoLogDto tobaccoLog = new TobaccoLogDto();
            tobaccoLog = model.tobaccoLog;
            tobaccoLog.Active = true;
            tobaccoLog.UserId = participantId;
            return reader.AddtoTobaccoLog(tobaccoLog);
        }

        public static ListTobaccoLogResponse ListTobaccoLog(DiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListTobaccoLogRequest request = new ListTobaccoLogRequest();
            request.userId = participantId;
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            return reader.ListTobaccoLog(request);
        }

        public static ReadTobaccoLogResponse ReadTobaccoLog(int id)
        {
            JournalReader reader = new JournalReader();
            ReadTobaccoLogRequest request = new ReadTobaccoLogRequest();
            request.id = id;
            return reader.ReadTobaccoLog(request);

        }
        #endregion

        #region Strength Training Log

        public static AddEditTrainingLogResponse AddEditTrainingLog(StrengthTrainingLogModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            StrengthTrainingLogDto trainingLog = new StrengthTrainingLogDto();
            trainingLog = model.trainingLog;
            IList<StrengthTrainingSetDto> trainingLogSet;
            trainingLogSet = model.trainingLogSet;
            trainingLog.UserId = participantId;
            AddEditTrainingLogRequest request = new AddEditTrainingLogRequest();
            request.trainingLog = trainingLog;
            request.trainingLogSet = trainingLogSet;
            return reader.AddEditTrainingLog(request);
        }

        public static ListTrainingTypeResponse ListTrainingType()
        {
            JournalReader reader = new JournalReader();
            return reader.ListTrainingType();
        }

        public static ListStrengthTrainingLogresponse ListTraining(StrengthTrainingListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListStrengthTrainingLogRequest request = new ListStrengthTrainingLogRequest();
            request.ParticipantId = participantId;
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            return reader.ListTraining(request);
        }

        public static ReadTrainingResponse ReadTraining(int id)
        {
            JournalReader reader = new JournalReader();
            return reader.ReadTraining(new ReadTrainingRequest { id = id });

        }

        #endregion

        #region Stress Management Log

        public static object AddEditStressManagement(StressManagementLogModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            StressManagementLogDto stressManagementLog = new StressManagementLogDto();
            stressManagementLog = model.streesLog;
            stressManagementLog.UserId = participantId;
            return reader.AddEditStressManagement(stressManagementLog);
        }

        public static ListStressManagementLogResponse ListStressManagement(DiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListStressManagementLogRequest request = new ListStressManagementLogRequest();
            request.userId = participantId;
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            return reader.ListStressManagement(request);
        }

        public static ReadStressManagementResponse ReadStressManagement(int id)
        {
            JournalReader reader = new JournalReader();
            return reader.ReadStressManagement(new ReadStressManagementRequest { id = id });
        }

        #endregion

        #region Sleep Log

        public static bool AddtoSleepLog(SleepLogModel model, int participantId, string participantTimeZoneName)
        {
            JournalReader reader = new JournalReader();
            SleepLogDto sleepLog = new SleepLogDto();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(participantTimeZoneName);
            sleepLog = model.sleepLog;
            sleepLog.UserId = participantId;
            sleepLog.Start = TimeZoneInfo.ConvertTimeToUtc(sleepLog.Start, custTZone);
            sleepLog.End = TimeZoneInfo.ConvertTimeToUtc(sleepLog.End, custTZone);
            return reader.AddtoSleepLog(sleepLog);
        }

        public static ListSleepLogResponse ListSleepLog(DiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListSleepLogRequest request = new ListSleepLogRequest();
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            request.userId = participantId;
            return reader.ListSleepLog(request);
        }

        public static ReadSleepLogResponse ReadSleepLog(int id, string participantTimeZoneName)
        {
            JournalReader reader = new JournalReader();
            ReadSleepLogRequest request = new ReadSleepLogRequest();
            request.id = id;
            var response = reader.ReadSleepLog(request);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(participantTimeZoneName);
            response.sleepLog.Start = TimeZoneInfo.ConvertTimeFromUtc(response.sleepLog.Start, custTZone);
            response.sleepLog.End = TimeZoneInfo.ConvertTimeFromUtc(response.sleepLog.End, custTZone);
            response.sleepLog.StartDT = response.sleepLog.Start.ToString();
            response.sleepLog.EndDT = response.sleepLog.End.ToString();
            return response;
        }
        #endregion

        #region Daily Vitals 
        public static AddEditDailyVitalsResponse AddEditDailyVitals(int userId, int? adminId, int portalId, string timeZone, VitalsModel model)
        {
            JournalReader reader = new JournalReader();
            ParticipantReader participantReader = new ParticipantReader();
            VitalsLogDto vitalsLog = model.DailyVitals;
            vitalsLog.UserId = userId;
            vitalsLog.TimeZoneId = timeZone;
            if (model.DailyVitals.HasWeight.HasValue && model.DailyVitals.Weight.HasValue)
            {
                int updatedBy = adminId.HasValue ? adminId.Value : userId;
                var wellnessData = participantReader.ListWellnessData(new ListWellnessDataRequest { userId = userId }).WellnessData.Where(x => x.CollectedOn.Date == DateTime.UtcNow.Date && x.SourceFollowUp == null && x.SourceHRA == null && x.UpdatedBy == updatedBy).OrderByDescending(x => x.Id).FirstOrDefault();
                AddEditWellnessDataRequest WDrequest = new AddEditWellnessDataRequest();
                if (wellnessData != null)
                {
                    wellnessData.Weight = model.DailyVitals.Weight;
                    wellnessData.UpdatedOn = DateTime.UtcNow;
                    WDrequest.WellnessData = wellnessData;
                }
                else
                {
                    WDrequest.WellnessData = new WellnessDataDto();
                    WDrequest.WellnessData.UserId = userId;
                    WDrequest.WellnessData.Weight = model.DailyVitals.Weight;
                    WDrequest.WellnessData.UpdatedBy = updatedBy;
                    WDrequest.WellnessData.CollectedOn = DateTime.UtcNow;
                }
                participantReader.AddEditWellnessData(WDrequest);
                AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                healthDataRequest.HealthData = new HealthDataDto();
                healthDataRequest.HealthData.UserId = userId;
                healthDataRequest.HealthData.Weight = model.DailyVitals.Weight.Value;
                healthDataRequest.HealthData.Source = (int)HealthDataSource.VitalsLog;
                healthDataRequest.HealthData.CreatedBy = updatedBy;
                healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                participantReader.AddtoHealthData(healthDataRequest);
            }
            AddEditDailyVitalsResponse response = reader.AddEditDailyVitals(vitalsLog);
            if (!response.hasPendingVitals)
            {
                SaveVitalsCompletionIncentive(userId, adminId, portalId);
            }
            return response;
        }

        public static bool SaveVitalsCompletionIncentive(int userId, int? adminId, int portalId)
        {
            IncentiveReader incentiveReader = new IncentiveReader();
            AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
            incentivesRequest.incentiveType = IncentiveTypes.Vitals_Completion;
            incentivesRequest.userId = userId;
            incentivesRequest.portalId = portalId;
            incentivesRequest.isEligible = true;
            incentiveReader.AwardIncentives(incentivesRequest);
            return true;
        }

        public static ReadVitalsResponse ReadVitals(int id)
        {
            JournalReader reader = new JournalReader();
            ReadVitalsRequest request = new ReadVitalsRequest();
            request.Id = id;
            return reader.ReadVitals(request);
        }

        public static ListVitalsLogResponse ListVitalsLog(DiaryListModel model, int participantId)
        {
            JournalReader reader = new JournalReader();
            ListVitalsLogRequest request = new ListVitalsLogRequest();
            request.Page = model.page;
            request.PageSize = model.pageSize;
            request.TotalRecords = model.totalRecords;
            request.userId = participantId;
            return reader.ListVitalsLog(request);
        }
        #endregion
    }
}



