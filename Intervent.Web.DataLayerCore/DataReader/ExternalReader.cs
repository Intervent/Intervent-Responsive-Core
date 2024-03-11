using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using NLog;


namespace Intervent.Web.DataLayer
{
    public class ExternalReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        public void BulkAddGlucose(List<EXT_Glucose> request)
        {
            ParticipantReader reader = new ParticipantReader();
            if (request != null && request.Count > 0)
            {
                using (var scope = new System.Transactions.TransactionScope())
                {
                    using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        foreach (EXT_Glucose dal in request)
                        {
                            if (!Exists(dal))
                            {
                                var response = context1.EXT_Glucose.Add(dal).Entity;
                                reader.CheckforGlucoseAlert(response);
                                context1.SaveChanges();
                            }
                        }
                    }

                    scope.Complete();
                }
            }
        }

        public int GetGlucoseAlertCount(int userId, ContactRequirementsAlertDto request, DateTime dateRange)
        {
            return context.EXT_Glucose.Where(x => x.UserId == userId && x.Value >= request.Min && x.Value <= request.Max && x.EffectiveDateTime > dateRange).Select(x => x.EffectiveDateTime.Value.Date).Distinct().Count();
        }

        public bool AddGlucose(EXT_Glucose request)
        {
            ParticipantReader reader = new ParticipantReader();
            try
            {
                var extGlucose = context.EXT_Glucose.Where(x => x.ExtId == request.ExtId && x.UserId == request.UserId).FirstOrDefault();

                if (extGlucose == null)
                {
                    var response = context.EXT_Glucose.Add(request).Entity;
                    context.SaveChanges();
                    reader.CheckforGlucoseAlert(response);
                    request.Id = response.Id;
                }
                else
                {
                    extGlucose.Value = request.Value;
                    extGlucose.Code = request.Code;
                    extGlucose.ExtId = request.ExtId;
                    extGlucose.EffectiveDateTime = request.EffectiveDateTime;
                    context.EXT_Glucose.Attach(extGlucose);
                    context.Entry(extGlucose).State = EntityState.Modified;
                    context.SaveChanges();
                    request.Id = extGlucose.Id;
                }
                return true;
            }
            catch (DbUpdateException ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "User Id : " + request.UserId + " ,Error : " + ex.ToString(), null, "AddGlucose", null, ex);
                logReader.WriteLogMessage(logEvent);
                return false;
            }
        }

        public bool AddGlucoseTags(Ext_GlucoseTags request)
        {
            var extGlucose = context.Ext_GlucoseTags.Where(x => x.GlucoseId == request.GlucoseId && x.TagName == request.TagName).FirstOrDefault();
            if (extGlucose == null)
            {
                request.UpdatedDate = request.CreatedDate = DateTime.UtcNow;
                context.Ext_GlucoseTags.Add(request);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddGlucoseTarget(Ext_GlucoseSetting request)
        {
            var extGlucose = context.Ext_GlucoseSetting.Where(x => x.GlucoseId == request.GlucoseId).FirstOrDefault();
            if (extGlucose == null)
            {
                context.Ext_GlucoseSetting.Add(request);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Exists(EXT_Glucose glucoseRequest)
        {
            if (!string.IsNullOrEmpty(glucoseRequest.ExtId))
            {
                return (context.EXT_Glucose.Where(ext => ext.ExtId == glucoseRequest.ExtId).FirstOrDefault() != null);
            }
            else
                return (context.EXT_Glucose.Where(ext => ext.UniqueId == glucoseRequest.UniqueId && ext.EffectiveDateTime == glucoseRequest.EffectiveDateTime).FirstOrDefault() != null);
        }

        public EXT_Glucose GetFirstSyncDateByUserId(EXT_Glucose glucoseRequest)
        {
            return context.EXT_Glucose.OrderBy(x => x.DateTime).Where(ext => ext.UserId == glucoseRequest.UserId).FirstOrDefault();
        }

        public ValidateIntuityUserResponse IsValidIntuityUserId(GetIntuityUserRequest request)
        {
            ValidateIntuityUserResponse response = new ValidateIntuityUserResponse();
            var intuityUser = context.IntuityUsers.Include("User").Include("User.Organization")
                .Where(x => (request.externalUserId.HasValue && x.ExternalUserId == request.externalUserId) || (request.userId.HasValue && x.UserId == request.userId)).FirstOrDefault();
            response.isValidIntuityUser = intuityUser != null;
            return response;
        }

        public IntuityEligibility GetIntuityEligibilityByDevices(string[] devices)
        {
            return context.IntuityEligibilities.Include("Organization").Where(x => devices.Contains(x.SerialNumber)).FirstOrDefault();
        }
        #region Intuity
        public GetIntuityUsersResponse AddEditIntuityUsers(IntuityUsers request)
        {
            GetIntuityUsersResponse response = new GetIntuityUsersResponse();
            var intuityUser = context.IntuityUsers.Where(x => x.UserId == request.UserId).FirstOrDefault();
            if (intuityUser != null)
            {
                intuityUser.ExternalUserId = request.ExternalUserId;
                intuityUser.IsEligible = request.IsEligible;
                intuityUser.IsCoachingActive = request.IsCoachingActive;
                intuityUser.DateUpdated = DateTime.UtcNow;
                //intuityUser.AuthToken = Guid.NewGuid().ToString();
                intuityUser.ExpiryOn = request.ExpiryOn;
                context.IntuityUsers.Attach(intuityUser);
                context.Entry(intuityUser).State = EntityState.Modified;
                context.SaveChanges();
                response.IsNewUser = false;
            }
            else
            {
                intuityUser = new IntuityUsers();
                intuityUser.UserId = request.UserId;
                intuityUser.ExternalUserId = request.ExternalUserId;
                intuityUser.IsEligible = request.IsEligible;
                intuityUser.IsCoachingActive = request.IsCoachingActive;
                intuityUser.DateCreated = intuityUser.DateUpdated = DateTime.UtcNow;
                intuityUser.AuthToken = Guid.NewGuid().ToString();
                intuityUser.ExpiryOn = request.ExpiryOn;
                context.IntuityUsers.Add(intuityUser);
                context.SaveChanges();
                response.IsNewUser = true;
            }
            response.IntuityUsers = intuityUser;
            return response;
        }

        public IntuityUsers GetIntuityUsersByUserId(int userId)
        {
            return context.IntuityUsers.Where(ext => ext.UserId == userId).FirstOrDefault();
        }

        public string GetValidUserNameByToken(string token)
        {
            var user = context.IntuityUsers.Include("User").Where(ext => ext.AuthToken == token && ext.ExpiryOn > DateTime.UtcNow).FirstOrDefault();
            //how do we hanldle termination date
            if (user != null && user.IsCoachingActive && user.ExpiryOn > DateTime.UtcNow && IsUserValid(user.User.UniqueId, null, user.User.OrganizationId, user.ExpiryOn))
            {
                return user.User.UserName;
            }
            return null;
        }

        public void TerminateIntuityUsers(int portalId)
        {
            //Find all terminated user
            var eligibilityList = context.Eligibilities.Include("Portal").Where(e => e.PortalId == portalId && e.TerminatedDate.HasValue).ToList();
            foreach (var elig in eligibilityList)
            {
                var intUser = context.IntuityUsers.Include("User").Include("User.Organization").Where(iu => iu.User.UniqueId == elig.UniqueId && iu.User.OrganizationId == elig.Portal.OrganizationId).FirstOrDefault();
                //still active user but terminated
                if (intUser != null && intUser.IsCoachingActive && intUser.ExpiryOn > DateTime.UtcNow)
                {
                    intUser.DateUpdated = DateTime.Now;
                    //intuityUser.AuthToken = Guid.NewGuid().ToString();
                    intUser.ExpiryOn = elig.TerminatedDate.Value;
                    context.IntuityUsers.Attach(intUser);
                    context.Entry(intUser).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public GetIntuityUserResponse GetIntuityUser(int portalId, bool onlyNew, string DTCOrgCode)
        {
            GetIntuityUserResponse response = new GetIntuityUserResponse();
            response.UserList = new List<IntuityUserWrapper>();
            response.NewUsers = new List<EligibilityDto>();
            var eligibilityList = context.Eligibilities.Include("Portal").Include("Portal.Organization").Where(e => e.PortalId == portalId).ToList();
            foreach (var elig in eligibilityList)
            {
                var intUser = context.IntuityUsers.Include("User").Include("User.Organization").Where(iu => iu.User.UniqueId == elig.UniqueId && iu.User.OrganizationId == elig.Portal.OrganizationId).FirstOrDefault();
                if (!onlyNew)
                {
                    if (intUser != null)
                    {
                        IntuityUserWrapper user = new IntuityUserWrapper();
                        user.IntUser = intUser;
                        user.Eligibility = elig;
                        response.UserList.Add(user);
                    }
                }
                else
                {
                    if (intUser == null)
                    {
                        if (elig.Portal.Organization.Code == DTCOrgCode)
                        {
                            if (!string.IsNullOrEmpty(elig.Portal.Organization.Code) && elig.CoachingEnabled.HasValue && elig.CoachingEnabled.Value
                                && elig.CoachingExpirationDate.HasValue && elig.CoachingExpirationDate > DateTime.UtcNow && (!string.IsNullOrEmpty(elig.Email2)
                                || !string.IsNullOrEmpty(elig.Email)))
                            {
                                if (string.IsNullOrEmpty(elig.Email2))
                                    elig.Email2 = elig.Email;
                                var eligibilityDto = ParticipantReader.MapToEligibilityDto(elig);
                                response.NewUsers.Add(eligibilityDto);
                            }
                        }
                        else
                        {
                            if (elig.TerminatedDate == null)
                            {
                                var intuityEligibility = context.IntuityEligibilities.Include("Organization").Where(i => i.UniqueId == elig.UniqueId && i.OrganizationId == elig.Portal.OrganizationId).FirstOrDefault();
                                if (intuityEligibility != null && intuityEligibility.EligibilityStatus == (byte)EligibilityStatus.Eligible && !string.IsNullOrEmpty(intuityEligibility.email))
                                {
                                    elig.Email2 = intuityEligibility.email;
                                    elig.UniqueId = intuityEligibility.Organization.Code + "-" + intuityEligibility.UniqueId;
                                    var eligibilityDto = ParticipantReader.MapToEligibilityDto(elig);
                                    response.NewUsers.Add(eligibilityDto);
                                }
                            }
                        }
                    }
                }
            }
            return response;
        }

        private bool IsUserValid(string uniqueId, int? portalId, int? orgId, DateTime expiryDate)
        {
            if (!string.IsNullOrEmpty(uniqueId))
            {
                if (!portalId.HasValue && orgId.HasValue)
                {
                    var portal = context.Portals.Where(x => x.OrganizationId == orgId.Value && x.Active).FirstOrDefault();
                    if (portal != null)
                    {
                        portalId = portal.Id;
                    }
                }
                if (portalId.HasValue)
                {
                    var eligibility = context.Eligibilities.Where(x => x.UniqueId == uniqueId && x.PortalId == portalId).FirstOrDefault();
                    if (eligibility != null)
                    {
                        var dateTime = IsValid(eligibility.TerminatedDate, eligibility.MedicalPlanStartDate, eligibility.MedicalPlanEndDate, expiryDate);
                        if (dateTime > DateTime.UtcNow)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public DateTime IsValid(DateTime? terminatedDate, DateTime? medicalPlanStartDate, DateTime? medicalPlanEndDate, DateTime expiryDate)
        {
            if (medicalPlanEndDate.HasValue)
            {
                expiryDate = medicalPlanEndDate.Value;
            }
            else if (terminatedDate.HasValue)
            {
                expiryDate = terminatedDate.Value;
            }
            return expiryDate;
        }

        public IntuityUsers GetIntuityUsersByExtUserId(int userId)
        {
            return context.IntuityUsers.Where(ext => ext.ExternalUserId == userId).FirstOrDefault();
        }

        public void AddExtFood(EXT_Nutrition food)
        {
            if (food != null && !string.IsNullOrEmpty(food.Meal))
            {
                var foodDAL = context.EXT_Nutrition.Where(x => x.UserId == food.UserId && x.ExternalId == food.ExternalId).FirstOrDefault();
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    if (foodDAL != null)
                    {
                        food.Id = foodDAL.Id;
                        foodDAL = food;
                        context1.EXT_Nutrition.Attach(foodDAL);
                        context1.Entry(foodDAL).State = EntityState.Modified;
                    }
                    else
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        context1.EXT_Nutrition.Add(food);
                    }
                    context1.SaveChanges();
                }
            }
        }

        public void AddExtSummary(EXT_Summaries summary)
        {
            var summaryDAL = context.EXT_Summaries.Where(x => x.UserId == summary.UserId && x.ExternalId == summary.ExternalId).FirstOrDefault();

            using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                if (summaryDAL != null)
                {
                    summary.Id = summaryDAL.Id;
                    summaryDAL = summary;
                    context1.EXT_Summaries.Attach(summaryDAL);
                    context1.Entry(summaryDAL).State = EntityState.Modified;
                }
                else
                {
                    context1.EXT_Summaries.Add(summary);
                }
                context1.SaveChanges();
            }
        }

        public void AddExtWeight(EXT_Weights weight, int systemAdminId)
        {
            if (weight.Weight.HasValue)
            {
                var weighDAL = context.EXT_Weights.Where(x => x.UserId == weight.UserId && x.ExternalId == weight.ExternalId).FirstOrDefault();

                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    if (weighDAL != null)
                    {
                        weight.Id = weighDAL.Id;
                        weighDAL = weight;
                        context1.EXT_Weights.Attach(weighDAL);
                        context1.Entry(weighDAL).State = EntityState.Modified;
                    }
                    else
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        context1.EXT_Weights.Add(weight);
                    }
                    context1.SaveChanges();
                }

                ParticipantReader participantReader = new ParticipantReader();
                AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                healthDataRequest.HealthData = new HealthDataDto();
                healthDataRequest.HealthData.UserId = weight.UserId;
                healthDataRequest.HealthData.Weight = (float)weight.Weight.Value;
                healthDataRequest.HealthData.Source = (int)HealthDataSource.Devices;
                healthDataRequest.HealthData.CreatedBy = systemAdminId;
                healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                participantReader.AddtoHealthData(healthDataRequest);
            }
        }

        public void UpdateBodyFatToExtWeight(EXT_Weights weight)
        {
            var weighDAL = context.EXT_Weights.Where(x => x.UserId == weight.UserId && x.TimeStamp.Date == weight.TimeStamp.Date && x.Source == weight.Source && x.InputMethod == weight.InputMethod).ToList();
            if (weighDAL.Count() > 0)
            {
                foreach (var weightData in weighDAL)
                {
                    weightData.FatPercent = weight.FatPercent;
                    context.EXT_Weights.Attach(weightData);
                    context.Entry(weightData).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public void BulkAddExtNutrition(List<EXT_Nutrition> nutritions)
        {
            try
            {
                if (nutritions != null && nutritions.Count > 0)
                {
                    using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        foreach (var nutrition in nutritions)
                        {
                            var nutritionDAL = context.EXT_Nutrition.Where(x => x.ExternalId == nutrition.ExternalId && x.Meal == nutrition.Meal && x.Name == nutrition.Name).FirstOrDefault();

                            if (nutritionDAL == null)
                            {
                                context1.EXT_Nutrition.Add(nutrition);
                            }
                            else
                            {
                                nutrition.Id = nutritionDAL.Id;
                                nutritionDAL = nutrition;
                                context1.EXT_Nutrition.Attach(nutritionDAL);
                                context1.Entry(nutritionDAL).State = EntityState.Modified;
                            }
                        }
                        context1.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Nutritions", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        public void AddExtWorkout(EXT_Workouts workout)
        {
            var workoutDAL = context.EXT_Workouts.Where(x => x.UserId == workout.UserId && x.ExternalId == workout.ExternalId).FirstOrDefault();

            using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                if (workoutDAL != null)
                {
                    workout.Id = workoutDAL.Id;
                    workoutDAL = workout;
                    context1.EXT_Workouts.Attach(workoutDAL);
                    context1.Entry(workoutDAL).State = EntityState.Modified;
                }
                else
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    context1.EXT_Workouts.Add(workout);
                }
                context1.SaveChanges();
            }
        }

        public void AddExtSleep(EXT_Sleeps sleep)
        {
            var sleepDAL = context.EXT_Sleeps.Where(x => x.UserId == sleep.UserId && x.ExternalId == sleep.ExternalId).FirstOrDefault();

            using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                if (sleepDAL != null)
                {
                    EXT_Sleeps sleepModify = sleepDAL;
                    if (sleep.SleepScore.HasValue)
                        sleepModify.SleepScore = sleep.SleepScore;
                    if (sleep.LightDuration.HasValue)
                        sleepModify.LightDuration = sleep.LightDuration;
                    if (sleep.RemDuration.HasValue)
                        sleepModify.RemDuration = sleep.RemDuration;
                    if (sleep.DeepDuration.HasValue)
                        sleepModify.DeepDuration = sleep.DeepDuration;
                    if (sleep.TotalSleepDuration.HasValue)
                        sleepModify.TotalSleepDuration = sleep.TotalSleepDuration;
                    if (sleep.AwakeCount.HasValue)
                        sleepModify.AwakeCount = sleep.AwakeCount;
                    if (sleep.WakeCount.HasValue)
                        sleepModify.WakeCount = sleep.WakeCount;
                    if (sleep.AwakeDuration.HasValue)
                        sleepModify.AwakeDuration = sleep.AwakeDuration;
                    if (sleep.TimetoBed.HasValue)
                        sleepModify.TimetoBed = sleep.TimetoBed;
                    if (sleep.TimetoWake.HasValue)
                        sleepModify.TimetoWake = sleep.TimetoWake;

                    sleepModify.InputMethod = sleep.InputMethod;
                    sleepModify.IsActive = sleep.IsActive;
                    sleepModify.StartTimeStamp = sleep.StartTimeStamp;
                    sleepModify.Source = sleep.Source;
                    context.EXT_Sleeps.Attach(sleepDAL);
                    context.Entry(sleepDAL).State = EntityState.Modified;
                }
                else
                {
                    //context.Configuration.AutoDetectChangesEnabled = false;
                    context.EXT_Sleeps.Add(sleep);
                }
                context.SaveChanges();
            }
        }

        public void AddExtBloodPressure(EXT_BloodPressures bloodPressure)
        {
            if (bloodPressure != null && bloodPressure.Systolic.HasValue && bloodPressure.Diastolic.HasValue)
            {
                var bloodPressureDAL = context.EXT_BloodPressures.Where(x => x.UserId == bloodPressure.UserId && x.ExternalId == bloodPressure.ExternalId).FirstOrDefault();

                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    if (bloodPressureDAL != null)
                    {
                        bloodPressure.Id = bloodPressureDAL.Id;
                        bloodPressureDAL = bloodPressure;
                        context1.EXT_BloodPressures.Attach(bloodPressureDAL);
                        context1.Entry(bloodPressureDAL).State = EntityState.Modified;
                    }
                    else
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        context1.EXT_BloodPressures.Add(bloodPressure);
                    }
                    context1.SaveChanges();
                }
            }
        }

        public void UpdateHeartRateToExtBloodPressure(EXT_BloodPressures bloodPressure)
        {
            var bloodPressureDAL = context.EXT_BloodPressures.Where(x => x.UserId == bloodPressure.UserId && x.TimeStamp.Date == bloodPressure.TimeStamp.Date && x.Source == bloodPressure.Source && x.InputMethod == bloodPressure.InputMethod).ToList();
            if (bloodPressureDAL.Count() > 0)
            {
                foreach (var bp in bloodPressureDAL)
                {
                    bp.RestingHeartRate = bloodPressure.RestingHeartRate;
                    context.EXT_BloodPressures.Attach(bp);
                    context.Entry(bp).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        #endregion
    }
}
