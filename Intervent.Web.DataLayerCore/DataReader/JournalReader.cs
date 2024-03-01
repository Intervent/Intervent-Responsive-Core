using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NLog;
using System.Data.Common;

namespace Intervent.Web.DataLayer
{
    public class JournalReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        #region Stress Dairy
        public bool AddtoStressDiary(StressDiaryDto stress)
        {
            try
            {
                stress.AddedOn = DateTime.UtcNow;
                var stressDAL = context.StressDiaries.Where(x => x.Id == stress.Id).FirstOrDefault();
                if (stressDAL != null)
                {
                    var updatedStressDAL = Utility.mapper.Map<StressDiaryDto, StressDiary>(stress);
                    context.Entry(stressDAL).CurrentValues.SetValues(updatedStressDAL);
                }
                else
                {
                    stressDAL = Utility.mapper.Map<StressDiaryDto, StressDiary>(stress);
                    context.StressDiaries.Add(stressDAL);
                }
                context.SaveChanges();
                return true;
            }
            catch (DbException e)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, e.Message, null, "AddtoStressDiary", null, e);
                logReader.WriteLogMessage(logEvent);
			    return false;
			}
        }

        public ListStressDiaryResponse ListStress(ListStressDiaryRequest request)
        {
            ListStressDiaryResponse response = new ListStressDiaryResponse();
            var stressDAL = context.StressDiaries.Where(x => x.UserId == request.userId).ToList();
            response.StressDiaries = Utility.mapper.Map<IList<DAL.StressDiary>, IList<StressDiaryDto>>(stressDAL);
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.StressDiaries.Where(x => x.UserId == request.userId).Count();
            }
            response.totalRecords = totalRecords;
            var stressDiary = context.StressDiaries.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList(); ;
            response.StressDiary = Utility.mapper.Map<IList<DAL.StressDiary>, IList<StressDiaryDto>>(stressDiary);
            return response;

        }

        #endregion

        #region Weight Loss Journal

        public bool AddToWeightLossJournal(WeightLossJournalDto weightLoss)
        {
            var weightLossDAL = new DAL.WeightLossJournal();
            if (weightLoss.Id > 0)
            {
                weightLossDAL = context.WeightLossJournals.Where(x => x.Id == weightLoss.Id && x.UserId == weightLoss.UserId).FirstOrDefault();
                if (weightLossDAL != null)
                {
                    weightLossDAL.Date = weightLoss.Date;
                    weightLossDAL.DayNo = weightLoss.DayNo;
                    weightLossDAL.Weight = weightLoss.Weight;
                    weightLossDAL.Waist = weightLoss.Waist;
                    weightLossDAL.Food = weightLoss.Food;
                    weightLossDAL.NotAuthorizedFood = weightLoss.NotAuthorizedFood;
                    weightLossDAL.HadWater = weightLoss.HadWater;
                    weightLossDAL.CutSodium = weightLoss.CutSodium;
                    weightLossDAL.SideEffects = weightLoss.SideEffects;
                    weightLossDAL.MotivationScale = weightLoss.MotivationScale;
                    weightLossDAL.Exercise = weightLoss.Exercise;
                    weightLossDAL.Activity = weightLoss.Activity;
                    weightLossDAL.Comments = weightLoss.Comments;
                    weightLossDAL.DateUpdated = DateTime.UtcNow;
                    context.WeightLossJournals.Attach(weightLossDAL);
                    context.Entry(weightLossDAL).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                weightLossDAL.UserId = weightLoss.UserId;
                weightLossDAL.Date = weightLoss.Date;
                weightLossDAL.DayNo = weightLoss.DayNo;
                weightLossDAL.Weight = weightLoss.Weight;
                weightLossDAL.Waist = weightLoss.Waist;
                weightLossDAL.Food = weightLoss.Food;
                weightLossDAL.NotAuthorizedFood = weightLoss.NotAuthorizedFood;
                weightLossDAL.HadWater = weightLoss.HadWater;
                weightLossDAL.CutSodium = weightLoss.CutSodium;
                weightLossDAL.SideEffects = weightLoss.SideEffects;
                weightLossDAL.MotivationScale = weightLoss.MotivationScale;
                weightLossDAL.Exercise = weightLoss.Exercise;
                weightLossDAL.Activity = weightLoss.Activity;
                weightLossDAL.Comments = weightLoss.Comments;
                weightLossDAL.DateUpdated = DateTime.UtcNow;
                weightLossDAL.IsActive = true;
                context.WeightLossJournals.Add(weightLossDAL);
                context.SaveChanges();
            }
            weightLoss.Id = weightLossDAL.Id;
            if (weightLossDAL.Weight.HasValue)
            {
                ParticipantReader participantReader = new ParticipantReader();
                AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                healthDataRequest.HealthData = new HealthDataDto();
                healthDataRequest.HealthData.UserId = weightLoss.UserId;
                healthDataRequest.HealthData.Weight = weightLossDAL.Weight.Value;
                healthDataRequest.HealthData.Source = (int)HealthDataSource.WeightLossDiary;
                healthDataRequest.HealthData.CreatedBy = weightLoss.UpdatedBy;
                healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                participantReader.AddtoHealthData(healthDataRequest);
            }
            return true;
        }

        public WeightLossJournalDto WeightLossDetails(int id)
        {
            var dal = context.WeightLossJournals.Where(x => x.Id == id).First();
            return Utility.mapper.Map<WeightLossJournal, WeightLossJournalDto>(dal);
        }

        public ListWeightLossResponse ListWeightLossJournal(ListWeightLossRequest request)
        {
            ListWeightLossResponse response = new ListWeightLossResponse();
            var weightLoss = context.WeightLossJournals.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).ToList();
            response.weightLossLists = Utility.mapper.Map<IList<DAL.WeightLossJournal>, IList<WeightLossJournalDto>>(weightLoss);
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.WeightLossJournals.Where(x => x.UserId == request.userId).Count();
            }
            response.totalRecords = totalRecords;
            var weightLossList = context.WeightLossJournals.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList(); ;
            response.weightLossList = Utility.mapper.Map<IList<DAL.WeightLossJournal>, IList<WeightLossJournalDto>>(weightLossList);
            return response;

        }

        public ReadStressResponse ReadStress(ReadFoodRequest request)
        {
            ReadStressResponse response = new ReadStressResponse();
            var stress = context.StressDiaries.Where(x => x.Id == request.id).FirstOrDefault();
            response.stress = Utility.mapper.Map<DAL.StressDiary, StressDiaryDto>(stress);
            return response;
        }

        #endregion

        #region Food Diary

        public ListFoodGroupResponse ListFoodGroup(ListFoodGroupRequest request)
        {
            ListFoodGroupResponse response = new ListFoodGroupResponse();
            var foodGroups = context.FoodGroups.ToList();
            response.FoodGroups = Utility.mapper.Map<IList<DAL.FoodGroup>, IList<FoodGroupDto>>(foodGroups);
            return response;
        }

        public ListMealTypeResponse ListMealType(ListMealTypeRequest request)
        {
            ListMealTypeResponse response = new ListMealTypeResponse();
            var mealTypes = context.MealTypes.ToList();
            response.MealTypes = Utility.mapper.Map<IList<DAL.MealType>, IList<MealTypeDto>>(mealTypes);
            return response;
        }

        public bool AddtoFoodDiary(FoodDiaryDto food)
        {
            try
            {
                var FoodDAL = context.FoodDiaries.Where(x => x.Id == food.Id).FirstOrDefault();
                if (FoodDAL != null)
                {
                    var updatedFoodDAL = Utility.mapper.Map<FoodDiaryDto, FoodDiary>(food);
                    FoodDAL.Name = updatedFoodDAL.Name;
                    if (updatedFoodDAL.ServingSize != null)
                        FoodDAL.ServingSize = updatedFoodDAL.ServingSize;
                    FoodDAL.FatGrams = updatedFoodDAL.FatGrams;
                    FoodDAL.CarbChoices = updatedFoodDAL.CarbChoices;
                    FoodDAL.CarbGrams = updatedFoodDAL.CarbGrams;
                    context.FoodDiaries.Attach(FoodDAL);
                    context.Entry(FoodDAL).State = EntityState.Modified;
                }
                else
                {
                    FoodDAL = Utility.mapper.Map<FoodDiaryDto, FoodDiary>(food);
                    context.FoodDiaries.Add(FoodDAL);
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "User : " + food.UserId + " ,Request : " + JsonConvert.SerializeObject(food), null, "AddtoFoodDiary", null, e);
                logReader.WriteLogMessage(logEvent);
                return false;
            }
        }

        public ListFoodDairyResponse ListFoodDetails(ListFoodDiaryRequest request)
        {
            ListFoodDairyResponse response = new ListFoodDairyResponse();
            var foodDiaries = context.FoodDiaries.Include("FoodGroup").Include("MealType").
                Where(x => x.UserId == request.ParticipantId).OrderBy(x => x.Date).ToList();
            response.fg1RecentItems = foodDiaries.Where(x => x.FoodGroupId == 1 && !string.IsNullOrEmpty(x.Name)).OrderByDescending(x => x.Id).GroupBy(x => x.Name).Select(x => x.Key).Take(10).ToList();
            response.fg2RecentItems = foodDiaries.Where(x => x.FoodGroupId == 2 && !string.IsNullOrEmpty(x.Name)).OrderByDescending(x => x.Id).GroupBy(x => x.Name).Select(x => x.Key).Take(10).ToList();
            response.fg3RecentItems = foodDiaries.Where(x => x.FoodGroupId == 3 && !string.IsNullOrEmpty(x.Name)).OrderByDescending(x => x.Id).GroupBy(x => x.Name).Select(x => x.Key).Take(10).ToList();
            response.fg4RecentItems = foodDiaries.Where(x => x.FoodGroupId == 4 && !string.IsNullOrEmpty(x.Name)).OrderByDescending(x => x.Id).GroupBy(x => x.Name).Select(x => x.Key).Take(10).ToList();
            response.fg5RecentItems = foodDiaries.Where(x => x.FoodGroupId == 5 && !string.IsNullOrEmpty(x.Name)).OrderByDescending(x => x.Id).GroupBy(x => x.Name).Select(x => x.Key).Take(10).ToList();
            response.fg6RecentItems = foodDiaries.Where(x => x.FoodGroupId == 6 && !string.IsNullOrEmpty(x.Name)).OrderByDescending(x => x.Id).GroupBy(x => x.Name).Select(x => x.Key).Take(10).ToList();
            var FoodDAL = foodDiaries.Where(x => x.Date == request.startDate).OrderBy(x => x.Date).ToList();
            response.FoodDiaryList = Utility.mapper.Map<IList<DAL.FoodDiary>, IList<FoodDiaryDto>>(FoodDAL);
            return response;
        }

        public ListFoodDairyResponse ListFood(ListFoodDiaryRequest request)
        {
            ListFoodDairyResponse response = new ListFoodDairyResponse();
            var FoodDAL = context.FoodDiaries.Include("FoodGroup").Include("MealType").
                Where(x => x.UserId == request.ParticipantId && x.Date >= request.startDate && x.Date <= request.endDate).OrderBy(x => x.Date).ToList();
            response.FoodDiaryList = Utility.mapper.Map<IList<DAL.FoodDiary>, IList<FoodDiaryDto>>(FoodDAL);
            return response;
        }

        public DeleteFoodResponse DeleteFood(DeleteFoodRequest request)
        {
            DeleteFoodResponse response = new DeleteFoodResponse();
            var foodDiarie = context.FoodDiaries.Where(x => x.Id == request.foodId).FirstOrDefault();
            if (foodDiarie != null)
            {
                context.Entry(foodDiarie).State = EntityState.Deleted;
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        #endregion

        #region Exercise Diary

        public bool AddtoExerciseDiary(ExerciseDiaryDto exercise)
        {
            try
            {
                var CurrentExDAL = context.ExerciseDiaries.Where(x => x.Id == exercise.Id).FirstOrDefault();
                if (CurrentExDAL != null)
                {
                    var ExDAL = Utility.mapper.Map<ExerciseDiaryDto, ExerciseDiary>(exercise);
                    context.Entry(CurrentExDAL).CurrentValues.SetValues(ExDAL);
                }
                else
                {
                    CurrentExDAL = Utility.mapper.Map<ExerciseDiaryDto, ExerciseDiary>(exercise);
                    context.ExerciseDiaries.Add(CurrentExDAL);
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "User : " + exercise.UserId + " ,Request : " + JsonConvert.SerializeObject(exercise), null, "AddtoExerciseDiary", null, e);
                logReader.WriteLogMessage(logEvent);
                return false;
            }
        }

        public ListExerciseDairyResponse ListExercise(ListExerciseDairyRequest request)
        {
            ListExerciseDairyResponse response = new ListExerciseDairyResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            var startdate = request.startDate.HasValue ? Convert.ToDateTime(request.startDate.Value) : DateTime.MinValue;
            var enddate = request.endDate.HasValue ? Convert.ToDateTime((request.endDate.Value).AddDays(1)) : DateTime.MaxValue;
            if (totalRecords == 0)
            {
                totalRecords = context.ExerciseDiaries.Include("ExerciseType").Where(x => x.UserId == request.ParticipantId && (x.Date >= startdate && x.Date <= enddate)).OrderByDescending(x => x.Date).Count();
            }
            var ExerciseDAL = context.ExerciseDiaries.Include("ExerciseType").Where(x => x.UserId == request.ParticipantId && (x.Date >= startdate && x.Date <= enddate)).OrderByDescending(x => x.Date).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            response.ExcerciseDiary = Utility.mapper.Map<IList<DAL.ExerciseDiary>, IList<ExerciseDiaryDto>>(ExerciseDAL);
            var exerciseDiaries = context.ExerciseDiaries.Include("ExerciseType").Where(x => x.UserId == request.ParticipantId && (x.Date >= startdate && x.Date <= enddate)).OrderByDescending(x => x.Date).ToList();
            response.ExcerciseDiaries = Utility.mapper.Map<IList<DAL.ExerciseDiary>, IList<ExerciseDiaryDto>>(exerciseDiaries); ;
            response.totalRecords = totalRecords;
            return response;
        }

        public ListExerciseTypeResponse ListExerciseType()
        {
            ListExerciseTypeResponse response = new ListExerciseTypeResponse();

            IList<ExerciseType> excerciseTypes = context.ExerciseTypes.OrderBy(x => x.Activity).ToList();

            response.ExcerciseType = Utility.mapper.Map<IList<DAL.ExerciseType>, IList<ExerciseTypeDto>>(excerciseTypes);

            return response;
        }

        public ReadExerciseDiaryResponse ReadExercise(ReadExerciseDiaryRequest request)
        {
            ReadExerciseDiaryResponse response = new ReadExerciseDiaryResponse();
            var Exercise = context.ExerciseDiaries.Include("ExerciseType").Where(x => x.Id == request.id).FirstOrDefault();
            response.Exercise = Utility.mapper.Map<DAL.ExerciseDiary, ExerciseDiaryDto>(Exercise);
            return response;
        }

        #endregion

        #region Tobacco Log
        public bool AddtoTobaccoLog(TobaccoLogDto tobaccoLog)
        {
            var tobaccoLogDAL = context.TobaccoLogs.Where(x => x.Id == tobaccoLog.Id).FirstOrDefault();
            if (tobaccoLogDAL != null)
            {
                tobaccoLog.UpdatedOn = DateTime.UtcNow;
                tobaccoLog.UpdatedBy = tobaccoLog.UserId;
                tobaccoLog.CreatedBy = tobaccoLogDAL.CreatedBy;
                tobaccoLog.CreatedOn = tobaccoLogDAL.CreatedOn;
                var updatedTobaccoLogDAL = Utility.mapper.Map<TobaccoLogDto, TobaccoLog>(tobaccoLog);
                context.Entry(tobaccoLogDAL).CurrentValues.SetValues(updatedTobaccoLogDAL);
            }
            else
            {
                tobaccoLog.CreatedOn = DateTime.UtcNow;
                tobaccoLog.CreatedBy = tobaccoLog.UserId;
                tobaccoLogDAL = Utility.mapper.Map<TobaccoLogDto, TobaccoLog>(tobaccoLog);
                context.TobaccoLogs.Add(tobaccoLogDAL);
            }
            context.SaveChanges();
            return true;
        }

        public ListTobaccoLogResponse ListTobaccoLog(ListTobaccoLogRequest request)
        {
            ListTobaccoLogResponse response = new ListTobaccoLogResponse();
            var tobaccoLogLists = context.TobaccoLogs.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).ToList();
            response.tobaccoLogLists = Utility.mapper.Map<IList<DAL.TobaccoLog>, IList<TobaccoLogDto>>(tobaccoLogLists);
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.TobaccoLogs.Where(x => x.UserId == request.userId).Count();
            }
            var tobaccoLogList = context.TobaccoLogs.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList(); ;
            response.tobaccoLogList = Utility.mapper.Map<IList<DAL.TobaccoLog>, IList<TobaccoLogDto>>(tobaccoLogList);
            response.totalRecords = totalRecords;
            return response;
        }

        public ReadTobaccoLogResponse ReadTobaccoLog(ReadTobaccoLogRequest request)
        {
            ReadTobaccoLogResponse response = new ReadTobaccoLogResponse();
            var tobaccoData = context.TobaccoLogs.Where(x => x.Id == request.id).FirstOrDefault();
            response.tobaccoLog = Utility.mapper.Map<DAL.TobaccoLog, TobaccoLogDto>(tobaccoData);
            return response;
        }
        #endregion

        #region Strength Training Log

        public AddEditTrainingLogResponse AddEditTrainingLog(AddEditTrainingLogRequest request)
        {
            AddEditTrainingLogResponse response = new AddEditTrainingLogResponse();
            //var TrainingDAL = context.CreateObjectSet<DAL.StrengthTrainingLog>().Where(x => x.Id == trainingLog.Id).FirstOrDefault();
            var TrainingDAL = context.StrengthTrainingLogs.Where(x => (request.trainingLog.Id > 0 && x.Id == request.trainingLog.Id) || (request.trainingLog.Id == 0 && x.Date == request.trainingLog.Date && x.TrainingTypeId == request.trainingLog.TrainingTypeId && x.MuscleGroup == request.trainingLog.MuscleGroup)).FirstOrDefault();

            if (TrainingDAL != null)
            {
                request.trainingLog.Id = TrainingDAL.Id;
                var updatedTrainingDAL = Utility.mapper.Map<StrengthTrainingLogDto, StrengthTrainingLog>(request.trainingLog);
                context.Entry(TrainingDAL).CurrentValues.SetValues(updatedTrainingDAL);
                context.SaveChanges();
                if (request.trainingLogSet != null && request.trainingLogSet.Count > 0)
                {
                    foreach (var trainingset in request.trainingLogSet)
                    {
                        trainingset.TrainingLogId = TrainingDAL.Id;
                    }
                }
                response.trainingsetCount = AddEditTrainingSet(request.trainingLogSet);
            }
            else
            {
                TrainingDAL = Utility.mapper.Map<StrengthTrainingLogDto, StrengthTrainingLog>(request.trainingLog);
                context.StrengthTrainingLogs.Add(TrainingDAL);
                context.SaveChanges();
                if (request.trainingLogSet != null && request.trainingLogSet.Count > 0)
                {
                    foreach (var trainingset in request.trainingLogSet)
                    {
                        trainingset.TrainingLogId = TrainingDAL.Id;
                    }
                }
                response.trainingsetCount = AddEditTrainingSet(request.trainingLogSet);
            }
            return response;
        }

        public int AddEditTrainingSet(IList<StrengthTrainingSetDto> trainingLogSet)
        {
            if (trainingLogSet != null && trainingLogSet.Count > 0)
            {
                int assignSet = 1;
                foreach (var trainingSet in trainingLogSet)
                {
                    if (trainingSet.Repetitions != null || trainingSet.RPE != null || trainingSet.Weight > 0)
                    {
                        trainingSet.TrainingSet = assignSet;
                        var trainingset = context.StrengthTrainingSets.Where(x => x.TrainingLogId == trainingSet.TrainingLogId && x.TrainingSet == trainingSet.TrainingSet).FirstOrDefault();

                        if (trainingset != null)
                        {
                            trainingSet.Id = trainingset.Id;
                            var updatedTrainingset = Utility.mapper.Map<StrengthTrainingSetDto, StrengthTrainingSet>(trainingSet);
                            context.Entry(trainingset).CurrentValues.SetValues(updatedTrainingset);
                            context.SaveChanges();
                        }
                        else
                        {
                            var TrainingSetDAL = Utility.mapper.Map<StrengthTrainingSetDto, StrengthTrainingSet>(trainingSet);
                            context.StrengthTrainingSets.Add(TrainingSetDAL);
                            context.SaveChanges();
                        }
                        assignSet = assignSet + 1;
                    }
                }
            }
            return 1;
        }
        public ListTrainingTypeResponse ListTrainingType()
        {
            ListTrainingTypeResponse response = new ListTrainingTypeResponse();
            var trainingTypes = context.StrengthTrainingTypes.ToList();
            response.TrainingTypes = Utility.mapper.Map<IList<DAL.StrengthTrainingType>, IList<StrengthTrainingTypeDto>>(trainingTypes);
            return response;
        }

        public ListStrengthTrainingLogresponse ListTraining(ListStrengthTrainingLogRequest request)
        {
            ListStrengthTrainingLogresponse response = new ListStrengthTrainingLogresponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.StrengthTrainingLogs.Include("StrengthTrainingType").Include("StrengthTrainingSets")
                                .Where(x => x.UserId == request.ParticipantId).Count();

            }
            var trainingDAL = context.StrengthTrainingLogs.Include("StrengthTrainingType").Include("StrengthTrainingSets")
                                .Where(x => x.UserId == request.ParticipantId).OrderByDescending(x => x.Date)
                                .Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();

            response.listStrengthTrainingLogResponse = Utility.mapper.Map<IList<DAL.StrengthTrainingLog>, IList<StrengthTrainingLogDto>>(trainingDAL);
            response.totalRecords = totalRecords;
            //Utility.mapper.Map<IList<DAL.StrengthTrainingLog>, IList<StrengthTrainingLogDto>>(trainingDAL);
            return response;
        }

        public ReadTrainingResponse ReadTraining(ReadTrainingRequest request)
        {
            ReadTrainingResponse response = new ReadTrainingResponse();
            var training = context.StrengthTrainingLogs.Include("StrengthTrainingSets").Where(x => x.Id == request.id).FirstOrDefault();
            var sets = training.StrengthTrainingSets.ToList();
            response.training = Utility.mapper.Map<DAL.StrengthTrainingLog, StrengthTrainingLogDto>(training);
            response.training.StrengthTrainingLogSet = Utility.mapper.Map<IList<DAL.StrengthTrainingSet>, IList<StrengthTrainingSetDto>>(sets);
            return response;
        }

        #endregion

        #region Stress Management Log

        public bool AddEditStressManagement(StressManagementLogDto stressManagementLog)
        {
            stressManagementLog.AddedOn = DateTime.UtcNow;
            var StressLogDAL = context.StressManagementLogs.Where(x => x.Id == stressManagementLog.Id).FirstOrDefault();
            if (StressLogDAL != null)
            {
                var UpdatedStressLogDAL = Utility.mapper.Map<StressManagementLogDto, StressManagementLog>(stressManagementLog);
                context.Entry(StressLogDAL).CurrentValues.SetValues(UpdatedStressLogDAL);
            }
            else
            {
                StressLogDAL = Utility.mapper.Map<StressManagementLogDto, StressManagementLog>(stressManagementLog);
                context.StressManagementLogs.Add(StressLogDAL);
            }
            context.SaveChanges();
            return true;
        }

        public ListStressManagementLogResponse ListStressManagement(ListStressManagementLogRequest request)
        {
            ListStressManagementLogResponse response = new ListStressManagementLogResponse();
            var stressManagementDAL = context.StressManagementLogs.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).ToList();
            response.stressManagementLogs = Utility.mapper.Map<IList<DAL.StressManagementLog>, IList<StressManagementLogDto>>(stressManagementDAL);
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.StressManagementLogs.Where(x => x.UserId == request.userId).Count();
            }
            response.totalRecords = totalRecords;
            var stressManagementLog = context.StressManagementLogs.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList(); ;
            response.stressManagementLog = Utility.mapper.Map<IList<DAL.StressManagementLog>, IList<StressManagementLogDto>>(stressManagementLog);
            return response;
        }

        public ReadStressManagementResponse ReadStressManagement(ReadStressManagementRequest request)
        {
            ReadStressManagementResponse response = new ReadStressManagementResponse();
            var stress = context.StressManagementLogs.Where(x => x.Id == request.id).FirstOrDefault();
            response.stress = Utility.mapper.Map<DAL.StressManagementLog, StressManagementLogDto>(stress);
            return response;
        }

        #endregion

        #region Sleep Log
        public bool AddtoSleepLog(SleepLogDto sleepLog)
        {
            var sleepLogDAL = context.SleepLogs.Where(x => x.Id == sleepLog.Id).FirstOrDefault();
            if (sleepLogDAL != null)
            {
                sleepLog.UpdatedOn = DateTime.UtcNow;
                sleepLog.UpdatedBy = sleepLog.UserId;
                sleepLog.CreatedBy = sleepLogDAL.CreatedBy;
                sleepLog.CreatedOn = sleepLogDAL.CreatedOn;
                var updatedSleepLogDAL = Utility.mapper.Map<SleepLogDto, SleepLog>(sleepLog);
                context.Entry(sleepLogDAL).CurrentValues.SetValues(updatedSleepLogDAL);
            }
            else
            {
                sleepLog.CreatedOn = DateTime.UtcNow;
                sleepLog.CreatedBy = sleepLog.UserId;
                sleepLogDAL = Utility.mapper.Map<SleepLogDto, SleepLog>(sleepLog);
                context.SleepLogs.Add(sleepLogDAL);
            }
            context.SaveChanges();
            return true;
        }

        public ListSleepLogResponse ListSleepLog(ListSleepLogRequest request)
        {
            ListSleepLogResponse response = new ListSleepLogResponse();
            var result = context.SleepLogs.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Start).ToList();

            if (result != null)
            {
                IList<SleepLogDto> sleepLogList = new List<SleepLogDto>();
                foreach (var data in result)
                {
                    SleepLogDto dto = new SleepLogDto();
                    TimeSpan tSpan = data.End - data.Start;
                    dto.Id = data.Id;
                    dto.UserId = data.UserId;
                    dto.Start = data.Start;
                    dto.End = data.End;
                    dto.SleepQuality = data.SleepQuality ?? default(byte);
                    dto.DaytimeAlertness = data.DaytimeAlertness ?? default(byte);
                    dto.Mood = data.Mood ?? default(byte);
                    dto.Notes = data.Notes;
                    dto.sleptHours = Convert.ToDouble(tSpan.TotalHours.ToString("#.##"));
                    sleepLogList.Add(dto);
                }
                response.sleepLogLists = sleepLogList;
            }
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.SleepLogs.Where(x => x.UserId == request.userId).Count();
            }
            response.totalRecords = totalRecords;
            var sleepLog = context.SleepLogs.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Start).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            if (result != null)
            {
                IList<SleepLogDto> sleepLogList = new List<SleepLogDto>();
                foreach (var data in sleepLog)
                {
                    SleepLogDto dto = new SleepLogDto();
                    TimeSpan tSpan = data.End - data.Start;
                    dto.Id = data.Id;
                    dto.UserId = data.UserId;
                    dto.Start = data.Start;
                    dto.End = data.End;
                    dto.SleepQuality = data.SleepQuality ?? default(byte);
                    dto.DaytimeAlertness = data.DaytimeAlertness ?? default(byte);
                    dto.Mood = data.Mood ?? default(byte);
                    dto.Notes = data.Notes;
                    dto.sleptHours = Convert.ToDouble(tSpan.TotalHours.ToString("#.##"));
                    sleepLogList.Add(dto);
                }
                response.sleepLogList = sleepLogList;
            }
            return response;
        }

        public ReadSleepLogResponse ReadSleepLog(ReadSleepLogRequest request)
        {
            ReadSleepLogResponse response = new ReadSleepLogResponse();
            var sleepLogData = context.SleepLogs.Where(x => x.Id == request.id).FirstOrDefault();
            response.sleepLog = Utility.mapper.Map<DAL.SleepLog, SleepLogDto>(sleepLogData);
            return response;
        }
        #endregion

        #region Daily Vitals Log
        public AddEditDailyVitalsResponse AddEditDailyVitals(VitalsLogDto vitalsDto)
        {
            AddEditDailyVitalsResponse response = new AddEditDailyVitalsResponse();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(vitalsDto.TimeZoneId);
            VitalsLog dailyVitals = new VitalsLog();
            if (vitalsDto.Id == 0)
                dailyVitals = context.VitalsLogs.Where(x => x.Date.Value.Date == DateTime.UtcNow.Date
                && x.UserId == vitalsDto.UserId).FirstOrDefault();
            else
                dailyVitals = context.VitalsLogs.Where(x => x.Id == vitalsDto.Id).FirstOrDefault();
            if (dailyVitals == null)
            {
                dailyVitals = new VitalsLog();
                dailyVitals.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).Date;
                dailyVitals.UserId = vitalsDto.UserId;
                context.VitalsLogs.Add(dailyVitals);
                context.SaveChanges();
            }
            if (dailyVitals.Points == null)
                dailyVitals.Points = 0;
            if (vitalsDto.HasWeight.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.HasWeight, vitalsDto.HasWeight.ToString());
                dailyVitals.HasWeight = vitalsDto.HasWeight;
                dailyVitals.Weight = vitalsDto.Weight;
            }
            if (vitalsDto.AerobicExercise.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.AerobicExercise, vitalsDto.AerobicExercise.ToString());
                dailyVitals.AerobicExercise = vitalsDto.AerobicExercise;
            }
            if (vitalsDto.HealthyEating.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.HealthyEating, vitalsDto.HealthyEating.ToString());
                dailyVitals.HealthyEating = vitalsDto.HealthyEating;
            }
            if (vitalsDto.Hydration.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.Hydration, vitalsDto.Hydration.ToString());
                dailyVitals.Hydration = vitalsDto.Hydration;
            }
            if (vitalsDto.Alcohol.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.Alcohol, vitalsDto.Alcohol.ToString());
                dailyVitals.Alcohol = vitalsDto.Alcohol;
            }
            if (vitalsDto.Tobacco.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.Tobacco, vitalsDto.Tobacco.ToString());
                dailyVitals.Tobacco = vitalsDto.Tobacco;
            }
            if (vitalsDto.Medications.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.Medications, vitalsDto.Medications.ToString());
                dailyVitals.Medications = vitalsDto.Medications;
            }
            if (vitalsDto.Sleep.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.Sleep, vitalsDto.Sleep.ToString());
                dailyVitals.Sleep = vitalsDto.Sleep;
            }
            if (vitalsDto.Stress.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.Stress, vitalsDto.Stress.ToString());
                dailyVitals.Stress = vitalsDto.Stress;
            }
            if (vitalsDto.Happy.HasValue)
            {
                dailyVitals.Points = VitalsLogPoints(dailyVitals.Points.Value, dailyVitals.Happy, vitalsDto.Happy.ToString());
                dailyVitals.Happy = vitalsDto.Happy;
            }

            context.Entry(dailyVitals).CurrentValues.SetValues(dailyVitals);
            context.SaveChanges();
            response.success = true;
            response.DailyVitalsId = dailyVitals.Id;
            var existingDailyVitals = context.VitalsLogs.Where(x => x.Id != dailyVitals.Id && x.Date.Value.Date == dailyVitals.Date.Value).FirstOrDefault();
            response.hasPendingVitals = (existingDailyVitals == null && dailyVitals.Weight.HasValue && dailyVitals.AerobicExercise.HasValue && dailyVitals.HealthyEating.HasValue && dailyVitals.Hydration.HasValue && dailyVitals.Alcohol.HasValue
                                       && dailyVitals.Tobacco.HasValue && dailyVitals.Medications.HasValue && dailyVitals.Sleep.HasValue && dailyVitals.Stress.HasValue && dailyVitals.Happy.HasValue) ? false : true;
            return response;

        }

        public int VitalsLogPoints(int points, byte? prevAns, string curAns)
        {
            if ((curAns == "1" || curAns == "3") && (prevAns == null || (curAns == "1" && (prevAns != 1 && prevAns != 3)) || (curAns == "3" && (prevAns != 1 && prevAns != 3))))
                points++;
            else if ((curAns == "2") && prevAns != null && (prevAns == 1 || prevAns == 3))
                points--;
            return points;
        }

        public ReadVitalsResponse ReadVitals(ReadVitalsRequest request)
        {
            ReadVitalsResponse response = new ReadVitalsResponse();
            var vitals = context.VitalsLogs.Where(x => x.Id == request.Id).FirstOrDefault();
            response.dailyVital = Utility.mapper.Map<VitalsLog, VitalsLogDto>(vitals);
            return response;
        }

        public ListVitalsLogResponse ListVitalsLog(ListVitalsLogRequest request)
        {
            ListVitalsLogResponse response = new ListVitalsLogResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.VitalsLogs.Where(x => x.UserId == request.userId).Count();
            }
            var vitals = context.VitalsLogs.Where(x => x.UserId == request.userId).OrderByDescending(x => x.Date).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            response.dailyVitals = Utility.mapper.Map<IList<VitalsLog>, IList<VitalsLogDto>>(vitals);
            response.totalRecords = totalRecords;
            return response;
        }
        #endregion
    }
}


