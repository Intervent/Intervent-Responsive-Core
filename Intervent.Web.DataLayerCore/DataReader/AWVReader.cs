using Intervent.DAL;
using Intervent.Web.DTO.AWV;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Data;
using System.Transactions;

namespace Intervent.Web.DataLayer
{
    public class AWVReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public void SaveComments(int id, string comments)
        {
            var wellnessData = context.AWVs.Where(x => x.Id == id).FirstOrDefault();
            if (wellnessData != null)
            {
                wellnessData.DrComments = comments;
                context.AWVs.Attach(wellnessData);
                context.Entry(wellnessData).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public string GetToken(UserManager<ApplicationUser> userManager, string externalId, string portalName, DateTime assessmentDate, out string validateionString)
        {
            string username = portalName + externalId;
            var reader = new AccountReader(userManager);
            validateionString = string.Empty;
            int? userId = reader.GetUserIdByNameAsync(username).Result;
            if (!userId.HasValue)
            {
                validateionString = "ExternalId not found";
                return string.Empty;
            }
            var wellnessDataList = context.AWVs.Where(x => x.UserId == userId.Value).ToList();
            if (wellnessDataList == null || wellnessDataList.Count == 0)
            {
                validateionString = "Wellness Data missing for the provided assessment date";
                return string.Empty;
            }
            DAL.AWV wellnessData = null;
            foreach (var awv in wellnessDataList)
            {
                if (awv.AssessmentDate.Date == assessmentDate.Date && awv.AssessmentDate.Year == assessmentDate.Year && awv.AssessmentDate.Month == assessmentDate.Month)
                {
                    wellnessData = awv;
                    break;
                }
            }
            if (wellnessData == null)
            {
                validateionString = "Wellness Data missing for the provided assessment date";
                return string.Empty;
            }
            GenerateToken(wellnessData, username);
            wellnessData.DateUpdated = DateTime.UtcNow;
            context.AWVs.Attach(wellnessData);
            context.Entry(wellnessData).State = EntityState.Modified;
            context.SaveChanges();
            return wellnessData.Token;
        }

        private void GenerateToken(DAL.AWV wellnessData, string username)
        {
            wellnessData.Token = username + GenerateUniqueString();
        }

        private string GenerateUniqueString()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            GuidString = GuidString.Replace("/", "");
            return GuidString;
        }

        public GoalsDto GetAWVReportDetails(int Id)
        {
            DAL.AWV wellnessData = null;
            wellnessData = context.AWVs.Include("AWV_Biometrics").Where(x => x.Id == Id).FirstOrDefault();
            DAL.User user = null;
            if (wellnessData != null)
            {
                var goal = context.AWV_Goals.Where(x => x.Id == wellnessData.Id).FirstOrDefault();
                if (goal == null)
                {
                    var awv = context.AWVs.Include("AWV_MedicalHistandProviders").Include("AWV_OsteoporosisScreens")
                        .Include("AWV_UrinaryScreens").Include("AWV_General").Include("AWV_HomeScreens").Include("AWV_DepressionScreens")
                        .Include("AWV_STDandProstateRisk").Include("AWV_AlcoholUse").Include("AWV_PreventiveServices").Include("AWV_TobaccoUse").Include("AWV_TobaccoAid").Include("User").Include("User.State1")
                        .Where(x => x.Id == Id).First();
                    awv.AWV_Biometrics = wellnessData.AWV_Biometrics;
                    goal = GenerateAWVGoals(awv);
                    user = awv.User;
                }
                if (goal != null)
                {
                    if (string.IsNullOrEmpty(user.UserName))
                    {
                        AccountReader accReader = new AccountReader();
                        user = accReader.GetUserById(wellnessData.UserId);
                    }
                    var goalDto = Utility.mapper.Map<DAL.AWV_Goals, GoalsDto>(goal);
                    goalDto.User = Utility.mapper.Map<DAL.User, DTO.UserDto>(user);
                    goalDto.DrComments = wellnessData.DrComments;
                    goalDto.AWV = MapAWVFromDAL(wellnessData);
                    goalDto.PrintToken = wellnessData.Token;
                    if (wellnessData.AWV_Biometrics != null)
                        goalDto.Biometrics = Utility.mapper.Map<DAL.AWV_Biometrics, BiometricsDto>(wellnessData.AWV_Biometrics);
                    return goalDto;
                }
            }
            return null;
        }

        #region CreateAWV

        public string CreateWellnessVisit(UserManager<ApplicationUser> userManager, PostAnualWellnessVisitDto AWV, string externalId, string portalName)
        {
            bool success = false;
            int awvId = 0, userId = 0;
            string token = string.Empty;
            bool userCreated = false;
            AccountReader accountReader = new AccountReader(userManager);
            try
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    int orgId;
                    int portalId = ReadPoral(portalName, out orgId);
                    string username = portalName + externalId;
                    userId = CreateUser(AWV.GeneralInformation, username, externalId, accountReader, orgId, out userCreated);
                    if (userId > 0)
                    {
                        var awv = CreateAWV(userId, portalId, AWV.GeneralInformation, username);
                        awvId = awv.Id;
                        token = awv.Token;
                        CreateAlcoholUse(awvId, AWV.AlcoholMisUse);
                        CreateBiometrics(awvId, AWV.Biometric);
                        CreateAllergies(awvId, AWV.MedicalHistory.Allergies);
                        CreateDepressionScreens(awvId, AWV.HealthRiskScreens.DepressionScreening);
                        CreateGeneral(awvId, AWV.GeneralHealth);
                        CreateHomeScreens(awvId, AWV.HealthRiskScreens.HealthScreening);
                        CreateUrinaryScreens(awvId, AWV.HealthRiskScreens.UrinaryScreening);
                        CreateHospitalVisits(awvId, AWV.MedicalHistory.HospitalVisit);
                        CreateMedicalHistandProviders(awvId, AWV.MedicalHistory.MedicalConditions, AWV.MedicalHistory.Surgeries, AWV.MedicalHistory.FamilyHistory, AWV.MedicalHistory.Adopted, AWV.MedicalHistory.Comments, AWV.MedicalHistory.OtherChronProbandSurg);
                        CreateMedications(awvId, AWV.MedicalHistory.Medications);
                        CreateOsteoporosisScreens(awvId, AWV.HealthRiskScreens.OsteoporosisScreens);
                        CreatePreventiveServices(awvId, AWV.PreventiveServices);
                        CreateServices(awvId, AWV.MedicalHistory.Services);
                        CreateSTDandProstateRisk(awvId, AWV.STDRisk);
                        CreateTobaccoUse(awvId, AWV.QuitTobacco);
                        trans.Complete();
                        success = true;
                    }
                }
                if (success)
                {
                    //context.AcceptAllChanges();
                    return token;
                }
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                string loggerName = "AWVController.CreateAnualWellnessVisit";
                var logEvent = LogEventInfo.Create(LogLevel.Error, loggerName, ex.Message);
                reader.WriteLogMessage(logEvent);
            }
            //user does not come under the same transactionscope :(
            if (userId > 0 && userCreated)
            {
                accountReader.DeleteUserAsync(userId);
            }

            return string.Empty;
        }

        private int ReadPoral(string portalName, out int id)
        {
            PortalReader reader = new PortalReader();
            DTO.ReadOrganizationRequest request = new DTO.ReadOrganizationRequest();
            request.Name = portalName;
            var response = reader.ReadOrganization(request);
            id = response.organization.Id.Value;
            return response.organization.Portals[0].Id;
        }

        private int CreateUser(AnnulWellnessVisitGeneralInfoDto generalInfo, string username, string externalId, AccountReader reader, int orgId, out bool userCreated)
        {

            int? userId = reader.GetUserIdByNameAsync(username).Result;
            userCreated = false;

            DTO.UserDto user = new DTO.UserDto();
            user.FirstName = generalInfo.FirstName;
            user.LastName = generalInfo.LastName;
            user.DOB = generalInfo.DOB;
            user.PhoneNumber = generalInfo.PhoneNumber;
            user.PhoneNumberConfirmed = true;
            user.UserName = username;
            user.PasswordHash = (generalInfo.LastName.Length > 4 ? generalInfo.LastName.Substring(0, 4) : generalInfo.LastName) + generalInfo.DOB.Year;
            user.UniqueId = externalId;
            user.EmailConfirmed = true;
            user.Email = ""; //Mandatory; set to empty string
            user.Address = generalInfo.Address;
            //user.Address2 = generalInfo.Address2; #TODO Need to add address2 in GeneralInfo
            user.OrganizationId = orgId;
            user.Race = generalInfo.Race;
            user.Gender = generalInfo.Gender;
            user.City = generalInfo.City;
            CommonReader commonReader = new CommonReader();
            var stateResponse = commonReader.GetState(new DTO.GetStateRequest() { stateCode = generalInfo.State });
            if (stateResponse != null)
                user.State = stateResponse.state.Id;
            user.Zip = generalInfo.Zip;
            user.Country = 212; //default country to US;
            if (!userId.HasValue)
            {
                DTO.RegisterUserRequest request = new DTO.RegisterUserRequest();
                request.User = user;
                var response = Task.Run(() => reader.CreateUser(request));
                userId = response.Result.userId;
                userCreated = true;
            }
            else
            {
                user.Id = userId.Value;
                DTO.UpdateUserRequest request = new DTO.UpdateUserRequest();
                request.user = user;
                request.UpdatedByUserId = user.Id;
                var response = Task.Run(() => reader.UpdateUser(request));
            }
            return userId.Value;
        }

        private DAL.AWV CreateAWV(int userId, int portalId, AnnulWellnessVisitGeneralInfoDto generalInfo, string userName)
        {
            DAL.AWV dal = new DAL.AWV();
            dal.DateUpdated = dal.DateCreated = DateTime.UtcNow;
            dal.AssessmentDate = generalInfo.AssessmentDate;
            dal.IPPE = generalInfo.IsIPPE;
            dal.LastAssessmentDate = generalInfo.DateOfLastAssessment;
            dal.MedBEligDate = generalInfo.MedicareEligibilityDate;
            dal.PortalId = portalId;
            dal.SubAWV = generalInfo.IsSubsequentAWV;
            dal.UserId = userId;
            dal.AWV1 = generalInfo.IsAWV;
            dal.ProviderAddress = generalInfo.PracticeAddress;
            dal.ProviderName = generalInfo.PracticeName;
            dal.ConductedBy = generalInfo.StaffName;
            GenerateToken(dal, userName);
            context.AWVs.Add(dal);
            context.SaveChanges();
            return dal;
        }

        private void CreateAlcoholUse(int awvId, AlcoholUseDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<AlcoholUseDto, DAL.AWV_AlcoholUse>(dto);
                dal.Id = awvId;
                context.AWV_AlcoholUse.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateBiometrics(int awvId, BiometricsDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<BiometricsDto, DAL.AWV_Biometrics>(dto);
                dal.Id = awvId;
                context.AWV_Biometrics.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateAllergies(int awvId, AllergiesDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<AllergiesDto, DAL.AWV_Allergies>(dto);
                dal.Id = awvId;
                context.AWV_Allergies.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateDepressionScreens(int awvId, DepressionScreensDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<DepressionScreensDto, DAL.AWV_DepressionScreens>(dto);
                dal.Id = awvId;
                context.AWV_DepressionScreens.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateGeneral(int awvId, GeneralDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<GeneralDto, DAL.AWV_General>(dto);
                dal.Id = awvId;
                context.AWV_General.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateMedicalHistandProviders(int awvId, MedicalConditionsDto medicalDto, SurgeriesDto surgeriesDto, FamilyHistoryDto historyDto, byte? adopted, string comments, List<string> otherChronicSurgeris)
        {
            if (medicalDto != null || surgeriesDto != null || historyDto != null)
            {
                DAL.AWV_MedicalHistandProviders dal = new DAL.AWV_MedicalHistandProviders();
                if (medicalDto != null)
                    dal = Utility.mapper.Map<MedicalConditionsDto, DAL.AWV_MedicalHistandProviders>(medicalDto, dal);
                if (surgeriesDto != null)
                    dal = Utility.mapper.Map<SurgeriesDto, DAL.AWV_MedicalHistandProviders>(surgeriesDto, dal);
                if (historyDto != null)
                    dal = Utility.mapper.Map<FamilyHistoryDto, DAL.AWV_MedicalHistandProviders>(historyDto, dal);
                dal.Id = awvId;
                dal.Comments = comments;
                if (otherChronicSurgeris != null && otherChronicSurgeris.Count > 0)
                {
                    dal.OtherChronProbandSurg = string.Join("~", otherChronicSurgeris.ToArray());
                    if (dal.OtherChronProbandSurg.Length > 500)
                        dal.OtherChronProbandSurg = dal.OtherChronProbandSurg.Substring(0, 500);
                }
                dal.FHAdopted = adopted;
                context.AWV_MedicalHistandProviders.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateHomeScreens(int awvId, HomeScreensDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<HomeScreensDto, DAL.AWV_HomeScreens>(dto);
                dal.Id = awvId;
                context.AWV_HomeScreens.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateHospitalVisits(int awvId, HospitalVisitsDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<HospitalVisitsDto, DAL.AWV_HospitalVisits>(dto);
                dal.Id = awvId;
                context.AWV_HospitalVisits.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateMedications(int awvId, MedicationsDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<MedicationsDto, DAL.AWV_Medications>(dto);
                dal.Id = awvId;
                context.AWV_Medications.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateOsteoporosisScreens(int awvId, OsteoporosisScreensDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<OsteoporosisScreensDto, DAL.AWV_OsteoporosisScreens>(dto);
                dal.Id = awvId;
                context.AWV_OsteoporosisScreens.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateServices(int awvId, ServicesDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<ServicesDto, DAL.AWV_Services>(dto);
                dal.Id = awvId;
                context.AWV_Services.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateSTDandProstateRisk(int awvId, STDandProstateRiskDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<STDandProstateRiskDto, DAL.AWV_STDandProstateRisk>(dto);
                dal.Id = awvId;
                context.AWV_STDandProstateRisk.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreateTobaccoUse(int awvId, TobaccoUseDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<TobaccoUseDto, DAL.AWV_TobaccoUse>(dto);
                dal.Id = awvId;
                context.AWV_TobaccoUse.Add(dal);
                context.SaveChanges();
                CreateTobaccoAid(dto.Helped, AidType.Helped, awvId);
                CreateTobaccoAid(dto.Failed, AidType.Failed, awvId);
                CreateTobaccoAid(dto.UseWhenReady, AidType.Ready, awvId);
            }
        }
        private void CreateTobaccoAid(List<AidToQuitTobacco> list, AidType type, int awvId)
        {
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var dal = new DAL.AWV_TobaccoAid();
                    dal.Id = awvId;
                    dal.TypeId = (int)type;
                    dal.AidId = (int)item;
                    context.AWV_TobaccoAid.Add(dal);
                }
                context.SaveChanges();
            }
        }

        private void CreateUrinaryScreens(int awvId, UrinaryScreensDto dto)
        {
            if (dto != null)
            {
                var dal = Utility.mapper.Map<UrinaryScreensDto, DAL.AWV_UrinaryScreens>(dto);
                dal.Id = awvId;
                context.AWV_UrinaryScreens.Add(dal);
                context.SaveChanges();
            }
        }

        private void CreatePreventiveServices(int awvId, List<PreventiveServicesDto> dto)
        {
            if (dto != null && dto.Count > 0)
            {
                foreach (var service in dto)
                {
                    var dal = Utility.mapper.Map<PreventiveServicesDto, DAL.AWV_PreventiveServices>(service);
                    dal.Id = awvId;
                    context.AWV_PreventiveServices.Add(dal);
                    context.SaveChanges();
                }
            }
        }

        #endregion

        #region GenerateGoals
        public DAL.AWV_Goals GenerateAWVGoals(DAL.AWV wellnessVisit)
        {
            DAL.AWV_Goals goal = new DAL.AWV_Goals();
            var medicalHistandProviders = wellnessVisit.AWV_MedicalHistandProviders ?? new DAL.AWV_MedicalHistandProviders();
            var ostScreening = wellnessVisit.AWV_OsteoporosisScreens ?? new DAL.AWV_OsteoporosisScreens();
            var urinaryScreening = wellnessVisit.AWV_UrinaryScreens ?? new DAL.AWV_UrinaryScreens();
            var generalHealth = wellnessVisit.AWV_General ?? new DAL.AWV_General();
            var homeScreening = wellnessVisit.AWV_HomeScreens ?? new DAL.AWV_HomeScreens();
            var depressionScreening = wellnessVisit.AWV_DepressionScreens ?? new DAL.AWV_DepressionScreens();
            var stdProstate = wellnessVisit.AWV_STDandProstateRisk ?? new DAL.AWV_STDandProstateRisk();
            var alcoholMisUse = wellnessVisit.AWV_AlcoholUse ?? new DAL.AWV_AlcoholUse();
            var preventiveServices = wellnessVisit.AWV_PreventiveServices;
            var biometric = wellnessVisit.AWV_Biometrics;
            var tobaccosUse = wellnessVisit.AWV_TobaccoUse;
            var tobaccoAid = wellnessVisit.AWV_TobaccoAid != null ? wellnessVisit.AWV_TobaccoAid.ToList() : null;
            var user = wellnessVisit.User;
            var age = ExtensionUtility.GetAge(user.DOB.Value, wellnessVisit.AssessmentDate);
            double? bmi = null;

            #region Personl History
            if (medicalHistandProviders.HeartAttack.IsTrue() || medicalHistandProviders.HeartDisease.IsTrue()
                || medicalHistandProviders.Pacemaker.IsTrue() || medicalHistandProviders.HeartSurgery.IsTrue()
                || medicalHistandProviders.Stroke.IsTrue())
                goal.Cardiovascular = (int)Risk.HR;
            else
                goal.Cardiovascular = (int)Risk.LR;
            if (medicalHistandProviders.Hypertension.IsTrue())
                goal.HighBP = (int)Risk.HR;
            else
                goal.HighBP = (int)Risk.LR;

            if (medicalHistandProviders.HighCholesterol.IsTrue()) //based on medication || medicalHistandProviders..IsTrue())
                goal.CholTrig = (int)Risk.HR;
            else
                goal.CholTrig = (int)Risk.LR;

            if (medicalHistandProviders.Diabetes.IsTrue())
                goal.Diabetes = (int)Risk.HR;
            else
                goal.Diabetes = (int)Risk.LR;

            if (medicalHistandProviders.BreastCancer.IsTrue() || medicalHistandProviders.ColonCancer.IsTrue()
                || medicalHistandProviders.CancerOther.IsTrue() || ostScreening.CancerMed.IsTrue() || ostScreening.Cancer.IsTrue())
                goal.Cancer = (int)Risk.HR;
            else
                goal.Cancer = (int)Risk.LR;

            if (medicalHistandProviders.Asthma.IsTrue() || medicalHistandProviders.COPD.IsTrue())
                goal.AsthmaCOPD = (int)Risk.HR;
            else
                goal.AsthmaCOPD = (int)Risk.LR;
            //Other
            if (medicalHistandProviders.Anemia.IsTrue() || medicalHistandProviders.Alcoholism.IsTrue()
                || medicalHistandProviders.Arthritis.IsTrue() || medicalHistandProviders.BladderProblems.IsTrue()
                || medicalHistandProviders.Blindness.IsTrue() || medicalHistandProviders.BloodClots.IsTrue()
                || medicalHistandProviders.BloodDisorder.IsTrue() || medicalHistandProviders.ColonPolyps.IsTrue()
                || medicalHistandProviders.Cataracts.IsTrue() || medicalHistandProviders.Depression.IsTrue()
                || medicalHistandProviders.GERD.IsTrue() || medicalHistandProviders.Glaucoma.IsTrue()
                || medicalHistandProviders.HayFever.IsTrue() || medicalHistandProviders.HearingLoss.IsTrue()
                || medicalHistandProviders.Hepatitis.IsTrue() || medicalHistandProviders.Jaundice.IsTrue()
                || medicalHistandProviders.Gout.IsTrue() || medicalHistandProviders.HIVorAIDS.IsTrue()
                || medicalHistandProviders.KidneyDisease.IsTrue() || medicalHistandProviders.KidneyStones.IsTrue()
                || medicalHistandProviders.LiverDisease.IsTrue() || medicalHistandProviders.MentalIllness.IsTrue()
                || medicalHistandProviders.NeurologicDisease.IsTrue() || medicalHistandProviders.Osteoporosis.IsTrue()
                || medicalHistandProviders.PhysicalDisability.IsTrue() || medicalHistandProviders.Pneumonia.IsTrue()
                || medicalHistandProviders.RheumaticFever.IsTrue() || medicalHistandProviders.SeizureDisorder.IsTrue()
                || medicalHistandProviders.SleepDisorder.IsTrue() || medicalHistandProviders.StomachDisorder.IsTrue()
                || medicalHistandProviders.ThyroidDisease.IsTrue() || medicalHistandProviders.Tuberculosis.IsTrue()
                || medicalHistandProviders.Ulcer.IsTrue() || medicalHistandProviders.BackSurgery.IsTrue()
                || medicalHistandProviders.HipSurgery.IsTrue() || medicalHistandProviders.KneeSurgery.IsTrue()
                || medicalHistandProviders.Thyroidectomy.IsTrue() || ostScreening.Osteoporosis.IsTrue()
                || urinaryScreening.UrnRiskScore.ContainsValue(new List<byte> { 2, 3 }))
                goal.OtherChronic = (int)Risk.HR;
            else if (!(goal.Cardiovascular.IsTrue() || goal.AsthmaCOPD.IsTrue() || goal.Cancer.IsTrue() || goal.CholTrig.IsTrue()
                        || goal.Diabetes.IsTrue() || goal.HighBP.IsTrue())
                    && !string.IsNullOrEmpty(medicalHistandProviders.OtherChronProbandSurg))
                goal.OtherChronic = (int)Risk.ID;
            else
                goal.OtherChronic = (int)Risk.LR;
            #endregion

            #region Family History
            if (medicalHistandProviders.FHHeartAttack.IsTrue())
                goal.FHCardiovascular = (int)Risk.HR;
            else if (medicalHistandProviders.FHAdopted.IsTrue())
                goal.FHCardiovascular = (int)Risk.ID;
            else
                goal.FHCardiovascular = (int)Risk.LR;

            if (medicalHistandProviders.FHDiabetes.IsTrue())
                goal.FHDiabetes = (int)Risk.HR;
            else if (medicalHistandProviders.FHAdopted.IsTrue())
                goal.FHDiabetes = (int)Risk.ID;
            else
                goal.FHDiabetes = (int)Risk.LR;

            if (medicalHistandProviders.FHBreastCancer.IsTrue() || medicalHistandProviders.FHOtherCancer.IsTrue()
                || medicalHistandProviders.FHProstateCancer.IsTrue() || medicalHistandProviders.FHColonCancer.IsTrue())
                goal.FHCancer = (int)Risk.HR;
            else if (medicalHistandProviders.FHAdopted.IsTrue())
                goal.FHCancer = (int)Risk.ID;
            else
                goal.FHCancer = (int)Risk.LR;
            #endregion

            #region Psychosocial
            //Antidepressents
            if (generalHealth.FeelingDown.IsTrue() || medicalHistandProviders.Depression.IsTrue()
                || depressionScreening.FeelingDown.ContainsValue(new List<byte> { 2, 3, 4 }) || depressionScreening.PHQ9_Score.ContainsValue(new List<byte> { 2, 3, 4, 5 }))
                goal.Depression = (int)Risk.HR;
            else if (generalHealth.FeelingDown.IsFalse() || depressionScreening.FeelingDown.HasValue(1)
                || depressionScreening.PHQ9_Score.HasValue(1))
                goal.Depression = (int)Risk.LR;
            else
                goal.Depression = (int)Risk.ID;

            if (generalHealth.FeelingDown.IsTrue() || generalHealth.LittleInterest.IsTrue())
                goal.LifeSatisfication = (int)Risk.HR;
            else if (generalHealth.FeelingDown.IsFalse() || generalHealth.LittleInterest.IsFalse())
                goal.LifeSatisfication = (int)Risk.LR;
            else
                goal.LifeSatisfication = (int)Risk.ID;

            //No questions
            goal.AngerRisk = (int)Risk.ID;
            goal.LonelyRisk = (int)Risk.ID;

            if (generalHealth.BodilyPain.ContainsValue(new List<byte> { 1, 2 }))
                goal.BodilyPain = (int)Risk.HR;
            else if (generalHealth.BodilyPain.HasValue(3))
                goal.BodilyPain = (int)Risk.LR;
            else
                goal.BodilyPain = (int)Risk.ID;

            if (generalHealth.BotheredbyTiredness.IsTrue() || depressionScreening.Tired.ContainsValue(new List<byte> { 2, 3, 4 }))
                goal.Fatigue = (int)Risk.HR;
            else if (generalHealth.BotheredbyTiredness.IsFalse() || depressionScreening.Tired.HasValue(1))
                goal.Fatigue = (int)Risk.LR;
            else
                goal.Fatigue = (int)Risk.ID;

            if (generalHealth.MemoryProblems.IsTrue() || depressionScreening.TroubleConc.ContainsValue(new List<byte> { 2, 3, 4 })
                || homeScreening.Fun_KeepEvents.IsFalse() || homeScreening.Fun_TakeMed.IsFalse() || homeScreening.Fun_KeepApp.IsFalse())
                goal.Cognition = (int)Risk.HR;
            else if (generalHealth.MemoryProblems.IsFalse() || depressionScreening.TroubleConc.HasValue(1)
                || homeScreening.Fun_KeepEvents.IsTrue() || homeScreening.Fun_TakeMed.IsTrue() || homeScreening.Fun_KeepApp.IsTrue())
                goal.Cognition = (int)Risk.LR;
            else
                goal.Cognition = (int)Risk.ID;
            #endregion

            #region Lifestyle/Behavioral Factors
            if (generalHealth.TobInLast4Weeks.IsTrue())
                goal.TobaccoUse = (int)Risk.HR;
            else if (generalHealth.TobInLast4Weeks.IsFalse())
                goal.TobaccoUse = (int)Risk.LR;
            else
                goal.TobaccoUse = (int)Risk.ID;

            if (generalHealth.C20MinExerFor3Days.IsTrue() && generalHealth.ExerIntensity.ContainsValue(new List<byte> { 1, 2 }))
                goal.PhysicalActivity = (int)Risk.LR;
            else if (generalHealth.C20MinExerFor3Days.IsFalse())
                goal.PhysicalActivity = (int)Risk.HR;
            else
                goal.PhysicalActivity = (int)Risk.ID;

            if (generalHealth.BreakfastDays.ContainsValue(new List<byte> { 1, 2 }) || generalHealth.NoOfSweetDrinks.ContainsValue(new List<byte> { 2, 3 }) ||
                generalHealth.FruitandVegServ.ContainsValue(new List<byte> { 1, 2 }))
                goal.Nutrition = (int)Risk.HR;
            else if (generalHealth.BreakfastDays.HasValue(3) && generalHealth.NoOfSweetDrinks.HasValue(1)
                && generalHealth.FruitandVegServ.HasValue(3))
                goal.Nutrition = (int)Risk.LR;
            else
                goal.Nutrition = (int)Risk.ID;

            if (generalHealth.NoOfDrinks.HasValue(3))
                goal.Alcohol = (int)Risk.HR;
            else if (generalHealth.NoOfDrinks.HasValue(1))
                goal.Alcohol = (int)Risk.LR;
            else
                goal.Alcohol = (int)Risk.ID;

            goal.Stress = (int)Risk.ID;

            if (generalHealth.Fallen2Times.IsTrue() || generalHealth.SafetyIssuesAtHome.IsTrue() || generalHealth.FastenSeatbelt.IsFalse()
                || homeScreening.Home_Bathmat.IsFalse() || homeScreening.Home_ChemsSafe.IsFalse() || homeScreening.Home_KnifeSafe.IsFalse()
                || homeScreening.Home_Throwrugs.IsTrue() || homeScreening.Home_SmokeAlarm.IsFalse() || homeScreening.Home_BathtubSafety.IsFalse()
                || homeScreening.Home_LooseCords.IsTrue() || homeScreening.Home_MedSafe.IsFalse() || homeScreening.Home_NightLight.IsFalse()
                || homeScreening.Home_SharpFurn.IsTrue() || homeScreening.Home_UnplugAppl.IsFalse() || homeScreening.HomeSafetyScore.HasValue(2)
                || homeScreening.HomeSafetyProviderScore.HasValue(2))
                goal.Safety = (int)Risk.HR;
            else if ((generalHealth.SafetyIssuesAtHome.IsFalse() && generalHealth.FastenSeatbelt.IsTrue()) || homeScreening.HomeSafetyScore.HasValue(1))
                goal.Safety = (int)Risk.LR;
            else
                goal.Safety = (int)Risk.ID;

            if (generalHealth.BotheredbyTeeth.IsTrue())
                goal.OralHealth = (int)Risk.HR;
            else
                goal.OralHealth = (int)Risk.ID;

            if (generalHealth.BotheredbySex.IsTrue() || (stdProstate.STD_MulPart.IsTrue() && stdProstate.STD_UseCondoms.IsFalse())
                || stdProstate.STDRiskScreen.ContainsValue(new List<byte> { 2, 3, 4 }))
                goal.SexHealth = (int)Risk.HR;
            else if (stdProstate.STD_Active.IsFalse() || stdProstate.STD_MulPart.IsFalse())
                goal.SexHealth = (int)Risk.LR;
            else
                goal.SexHealth = (int)Risk.ID;
            #endregion

            #region Functional Ability
            if (homeScreening.FunActivityProviderScore.HasValue(2) || (!homeScreening.FunActivityScore.HasValue(1) && (generalHealth.BotheredbyDizzy.IsTrue() || generalHealth.WalkWithoutHelp.IsFalse()
                || generalHealth.DoThingsWithoutHelp.IsFalse() || generalHealth.NeedHelpWithBasics.IsTrue() || generalHealth.MemoryProblems.IsTrue()
                || homeScreening.Fun_GetOutofBed.IsFalse() || homeScreening.Fun_DressYourself.IsFalse() || homeScreening.Fun_Drive.IsFalse()
                || homeScreening.Fun_KeepApp.IsFalse() || homeScreening.Fun_KeepEvents.IsFalse() || homeScreening.Fun_OwnSHop.IsFalse()
                || homeScreening.Fun_PlayGame.IsFalse() || homeScreening.Fun_PrepOwnMeals.IsFalse() || homeScreening.Fun_TakeMed.IsFalse()
                || homeScreening.Fun_WriteCheck.IsFalse() || homeScreening.FunActivityProviderScore.HasValue(2) || homeScreening.FunActivityScore.HasValue(2))))
                goal.DailyActivity = (int)Risk.HR;
            else if (homeScreening.FunActivityScore.HasValue(1) || generalHealth.WalkWithoutHelp.IsTrue() || generalHealth.DoThingsWithoutHelp.IsTrue() || generalHealth.NeedHelpWithBasics.IsFalse() || generalHealth.MemoryProblems.IsFalse())
                goal.DailyActivity = (int)Risk.LR;
            else
                goal.DailyActivity = (int)Risk.ID;

            if (homeScreening.FallRiskProviderScore.HasValue(2) || (!homeScreening.FallRiskScore.HasValue(1) &&
                (generalHealth.Fallen2Times.IsTrue()
                || homeScreening.FallRiskScore.HasValue(2) || homeScreening.Fall_DiffStartStop.IsFalse() || homeScreening.Fall_DiffWalk.IsTrue()
                || homeScreening.Fall_EverFall.IsTrue() || homeScreening.Fall_HeavySteps.IsTrue() || homeScreening.Fall_LightHeaded.IsTrue()
                || homeScreening.Fall_LooseBalance.IsTrue() || homeScreening.Fall_Numbess.IsTrue() || homeScreening.Fall_TroubleGetOut.IsTrue())))
                goal.Falls = (int)Risk.HR;
            else if (homeScreening.FallRiskScore.HasValue(1) || generalHealth.Fallen2Times.IsFalse())
                goal.Falls = (int)Risk.LR;
            else
                goal.Falls = (int)Risk.ID;

            if (homeScreening.HearingLossProviderScore.HasValue(2) || (!homeScreening.HearingLossScore.HasValue(1) &&
                (homeScreening.HearingLossScore.HasValue(2) && medicalHistandProviders.HearingLoss.IsTrue()
                || generalHealth.UseHearingAids.IsTrue() || homeScreening.Hear_AsktoRepeat.IsTrue() || homeScreening.Hear_AudioHighVol.IsTrue()
                || homeScreening.Hear_Misunderstand.IsTrue() || homeScreening.Hear_NoisyBack.IsTrue() || homeScreening.Hear_PeopleGetAnnoyed.IsTrue()
                || homeScreening.Hear_PeopleMumble.IsTrue() || homeScreening.Hear_ProbOverTel.IsTrue() || homeScreening.Hear_StraintoUnderstand.IsTrue()
                || homeScreening.Hear_TroubleFollConv.IsTrue() || homeScreening.Hear_TroubleUnderstand.IsTrue())))
                goal.HearingImp = (int)Risk.HR;
            else if (homeScreening.HearingLossScore.HasValue(1) || generalHealth.UseHearingAids.IsFalse())
                goal.HearingImp = (int)Risk.LR;
            else
                goal.HearingImp = (int)Risk.ID;

            if (medicalHistandProviders.Blindness.IsTrue() || medicalHistandProviders.Cataracts.IsTrue() || medicalHistandProviders.EyeSurgery.IsTrue())
                goal.VisionImp = (int)Risk.HR;
            else
                goal.VisionImp = (int)Risk.ID;

            if (urinaryScreening.UrnRiskScore.HasValue(2) || generalHealth.UrineProblems.IsTrue() || urinaryScreening.AffEnt.IsTrue()
                || urinaryScreening.AffHousChores.IsTrue() || urinaryScreening.AffPhyRec.IsTrue() || urinaryScreening.BefReachToilet.IsTrue()
                || urinaryScreening.FeelFrust.IsTrue() || urinaryScreening.RushtoBath.IsTrue() || urinaryScreening.Social.IsTrue()
                || urinaryScreening.Travel.IsTrue() || urinaryScreening.WhenBend.IsTrue() || urinaryScreening.WhenCough.IsTrue()
                || urinaryScreening.WhenExer.IsTrue() || urinaryScreening.WhenUndress.IsTrue())
                goal.UrinaryInc = (int)Risk.HR;
            else if (generalHealth.UrineProblems.IsFalse())
                goal.UrinaryInc = (int)Risk.LR;
            else
                goal.UrinaryInc = (int)Risk.ID;

            //Only for male
            if (user.Gender.HasValue(1))
            {
                if (stdProstate.ProsRiskScreen.ContainsValue(new List<byte> { 2, 3, 4 }) || medicalHistandProviders.ProstateSurgery.IsTrue()
                    || stdProstate.Pro_AgaininTwo.ContainsValue(new List<byte> { 3, 4, 5, 6 }) || stdProstate.Pro_DifftoPost.ContainsValue(new List<byte> { 3, 4, 5, 6 })
                    || stdProstate.Pro_GetupatNight.ContainsValue(new List<byte> { 3, 4, 5, 6 }) || stdProstate.Pro_NotEmpty.ContainsValue(new List<byte> { 3, 4, 5, 6 })
                    || stdProstate.Pro_StartandStop.ContainsValue(new List<byte> { 3, 4, 5, 6 }) || stdProstate.Pro_StraintoBegin.ContainsValue(new List<byte> { 3, 4, 5, 6 })
                    || stdProstate.Pro_WeakSystem.ContainsValue(new List<byte> { 3, 4, 5, 6 }))
                    goal.Prostate = (int)Risk.HR;
                else if (generalHealth.UrineProblems.IsFalse())
                    goal.Prostate = (int)Risk.LR;
                else
                    goal.Prostate = (int)Risk.ID;


            }

            #endregion

            #region Clinical Measurments

            goal.Weight = goal.BP = goal.Cholesterol = goal.GlucA1C = (int)Risk.ID;
            if (biometric != null)
            {
                if (biometric.Weight.HasValue && biometric.Height.HasValue)
                {
                    bmi = CommonReader.GetBMI(biometric.Height.Value, biometric.Weight.Value);
                    if (bmi >= 25)
                        goal.Weight = (int)Risk.HR;
                    else
                        goal.Weight = (int)Risk.LR;
                }

                if (biometric.SBP.HasValue && biometric.DBP.HasValue)
                {
                    if (biometric.SBP.Value >= 140 || biometric.DBP.Value >= 90)
                        goal.BP = (int)Risk.HR;
                    else
                        goal.BP = (int)Risk.LR;
                }

                if (biometric.LDL.HasValue && biometric.HDL.HasValue && biometric.Trig.HasValue && biometric.TotalChol.HasValue)
                {
                    if (biometric.LDL.Value >= 100 || (user.Gender.HasValue(1) && biometric.HDL.Value < 40)
                                                    || (user.Gender.HasValue(2) && biometric.HDL.Value < 50)
                       || biometric.Trig.Value >= 150 || (biometric.TotalChol.Value - biometric.HDL.Value) >= 130)
                        goal.Cholesterol = (int)Risk.HR;
                    else
                        goal.Cholesterol = (int)Risk.LR;
                }

                if ((biometric.Glucose.HasValue && ((medicalHistandProviders.Diabetes.IsTrue() && biometric.Fasting.IsNullOrTrue() && biometric.Glucose.Value > 130)
                            || (medicalHistandProviders.Diabetes.IsTrue() && biometric.Glucose.Value >= 180)
                            || (medicalHistandProviders.Diabetes.IsNullOrFalse() && biometric.Fasting.IsNullOrTrue() && biometric.Glucose.Value >= 100)
                            || (medicalHistandProviders.Diabetes.IsNullOrFalse() && biometric.Glucose.Value >= 140)))
                    || (biometric.A1C.HasValue && ((medicalHistandProviders.Diabetes.IsTrue() && biometric.A1C.Value >= 7)
                            || (medicalHistandProviders.Diabetes.IsNullOrFalse() && biometric.A1C.Value >= (float)5.7))))
                    goal.GlucA1C = (int)Risk.HR;
                else if ((biometric.Glucose.HasValue && ((medicalHistandProviders.Diabetes.IsNullOrFalse() && biometric.Glucose.Value < 100)
                            || (medicalHistandProviders.Diabetes.IsTrue() && biometric.Glucose.Value >= 80 && biometric.Glucose.Value <= 130)))
                    || (biometric.A1C.HasValue && ((medicalHistandProviders.Diabetes.IsTrue() && biometric.A1C.Value < 7)
                            || (medicalHistandProviders.Diabetes.IsNullOrFalse() && biometric.A1C.Value < (float)5.7))))
                    goal.GlucA1C = (int)Risk.LR;
            }

            #endregion

            #region Preventive Screening

            goal.Colorectal = goal.PapTest = goal.Mammogram = goal.PSAProstate = goal.Vision = goal.Hearing = goal.Aortic = (int)Track.NotApplicable;
            goal.BPScreening = goal.Lipoproteins = goal.Obesity = (int)Track.NotOnTrack;
            goal.DiabetesScreening = goal.Osteoporosis = goal.LungCancer = goal.HIV = goal.Tetanus = goal.Pneumonia = goal.Flu = goal.Shingles = goal.HepatitisB = (int)Track.Indeterminate;

            goal.PSAProstateRecDate = goal.ColorectalRecDate = goal.PapTestRecDate = goal.MammogramRecDate = goal.AorticRecDate = Constants.NotApplicable;
            goal.PneumoniaRecDate = goal.DiabetesScreeningRecDate = goal.OsteoporosisRecDate = goal.VisionRecDate = goal.HIVRecDate =
                goal.TetanusRecDate = goal.ShinglesRecDate = goal.HepatitisBRecDate = Constants.Physician;
            goal.LipoproteinsRecDate = Constants.Now;
            goal.FluRecDate = Constants.YearlyFlu;
            DAL.AWV_PreventiveServices hivTest = null, hearing = null, aortic = null, pneum = null, glaucoma = null, shingelles = null, gluc = null, lipid = null, ldl = null, hbA1c = null;
            List<DAL.AWV_PreventiveServices> list = null;

            if (preventiveServices != null)
            {
                list = preventiveServices.ToList();
                hivTest = list.Find(p => p.Type == (int)PreventiveServicesType.HIV);
                lipid = list.Find(p => p.Type == (int)PreventiveServicesType.Lipid_Panel);
                ldl = list.Find(p => p.Type == (int)PreventiveServicesType.LDL);
                var colonoscopy = list.Find(p => p.Type == (int)PreventiveServicesType.Colonoscopy);
                var hemocult = list.Find(p => p.Type == (int)PreventiveServicesType.Hemocult);
                if (colonoscopy != null && colonoscopy.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 5))
                    goal.Colorectal = (int)Track.OnTrack;
                else if ((colonoscopy != null && colonoscopy.Recommend.IsTrue()) || (hemocult != null && hemocult.Recommend.IsTrue()))
                    goal.Colorectal = (int)Track.NotOnTrack;
                else if ((colonoscopy != null && colonoscopy.Testing.IsTrue()) && (hemocult != null && hemocult.Testing.IsTrue()))
                    goal.Colorectal = (int)Track.NotApplicable;
                else if ((colonoscopy != null && colonoscopy.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 10)) || (hemocult != null && hemocult.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 1)))
                    goal.Colorectal = (int)Track.OnTrack;
                else if (age >= 50 && age <= 85)
                    goal.Colorectal = (int)Track.NotOnTrack;
                else
                    goal.Colorectal = (int)Track.NotApplicable;

                goal.ColorectalRecDate = "~";
                if (colonoscopy != null && colonoscopy.Recommend.IsTrue())
                {
                    if (colonoscopy != null && colonoscopy.LastReceived.HasValue)
                        goal.ColorectalRecDate = colonoscopy.LastReceived.Value.ToShortDateString() + "~";
                    goal.ColorectalRecDate += "C~" + Constants.Now;
                }
                else if (hemocult != null && hemocult.Recommend.IsTrue())
                {

                    if (hemocult.LastReceived.HasValue)
                        goal.ColorectalRecDate = hemocult.LastReceived.Value.ToShortDateString() + "~";
                    goal.ColorectalRecDate += "H~" + Constants.Now;
                }
                else if (colonoscopy != null && colonoscopy.LastReceived.HasValue)
                {
                    var diff = ExtensionUtility.DiffInYears(colonoscopy.LastReceived.Value.AddYears(10), user.DOB.Value);
                    if (diff > 85)
                        goal.ColorectalRecDate = colonoscopy.LastReceived.Value.ToShortDateString() + "~" + Constants.Physician;
                    else
                        goal.ColorectalRecDate = colonoscopy.LastReceived.Value.ToShortDateString() + "~C~" + colonoscopy.LastReceived.Value.AddYears(10).ToShortDateString();
                }
                else if (hemocult != null && hemocult.LastReceived.HasValue)
                {
                    var diff = ExtensionUtility.DiffInYears(hemocult.LastReceived.Value.AddYears(1), user.DOB.Value);
                    if (diff > 85)
                        goal.ColorectalRecDate = hemocult.LastReceived.Value.ToShortDateString() + "~" + Constants.Physician;
                    else
                        goal.ColorectalRecDate = hemocult.LastReceived.Value.ToShortDateString() + "~H~" + hemocult.LastReceived.Value.AddYears(1).ToShortDateString();
                }
                else if (goal.Colorectal == (int)Track.NotOnTrack)
                {
                    goal.ColorectalRecDate += "C~" + Constants.Now;
                }
                else if (goal.Colorectal == (int)Track.NotApplicable)
                {
                    if (colonoscopy != null && colonoscopy.LastReceived.HasValue)
                        goal.ColorectalRecDate = colonoscopy.LastReceived.Value.ToShortDateString() + "~";
                    goal.ColorectalRecDate += Constants.NotApplicable;
                }
                else
                {
                    if (colonoscopy != null && colonoscopy.LastReceived.HasValue)
                        goal.ColorectalRecDate = colonoscopy.LastReceived.Value.ToShortDateString() + "~";
                    goal.ColorectalRecDate += Constants.Physician;
                }

                //Female Only
                if (user.Gender.HasValue(2))
                {
                    var pap = list.Find(p => p.Type == (int)PreventiveServicesType.Cervical_Cancer_Screening);
                    if (pap != null)
                    {
                        if (pap.Recommend.IsTrue())
                            goal.PapTest = (int)Track.NotOnTrack;
                        else if (pap.Testing.IsTrue())
                            goal.PapTest = (int)Track.NotApplicable;
                        else if (pap.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 3))
                            goal.PapTest = (int)Track.OnTrack;
                        else if (user.DOB.GreaterThanOrEqualToYears(wellnessVisit.AssessmentDate, 21) && user.DOB.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 65))
                            goal.PapTest = (int)Track.NotOnTrack;
                        else
                            goal.PapTest = (int)Track.NotApplicable;

                        if (pap.LastReceived.HasValue)
                            goal.PapTestRecDate = pap.LastReceived.Value.ToShortDateString() + "~";
                        else
                            goal.PapTestRecDate = "~";
                        if (goal.PapTest == (int)Track.NotOnTrack)
                            goal.PapTestRecDate += Constants.Now;
                        else if ((age + 3) < 66 && pap.LastReceived.HasValue)
                            goal.PapTestRecDate += pap.LastReceived.Value.AddYears(3).ToShortDateString();
                        else if (goal.PapTest == (int)Track.NotApplicable)
                            goal.PapTestRecDate += Constants.NotApplicable;
                        else
                            goal.PapTestRecDate += Constants.Physician;
                    }
                    else
                    {
                        if (user.DOB.GreaterThanOrEqualToYears(wellnessVisit.AssessmentDate, 21) && user.DOB.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 65))
                            goal.PapTest = (int)Track.NotOnTrack;
                        else
                            goal.PapTest = (int)Track.NotApplicable;

                        goal.PapTestRecDate = "~";
                        if (goal.PapTest == (int)Track.NotApplicable)
                            goal.PapTestRecDate += Constants.NotApplicable;
                        else if (goal.PapTest == (int)Track.NotOnTrack)
                            goal.PapTestRecDate += Constants.Now;
                    }

                    var mammogram = list.Find(p => p.Type == (int)PreventiveServicesType.Mammogram_Screen);
                    if (mammogram != null)
                    {
                        if (mammogram.Recommend.IsTrue())
                            goal.Mammogram = (int)Track.NotOnTrack;
                        else if (mammogram.Testing.IsTrue())
                            goal.Mammogram = (int)Track.NotApplicable;
                        else if (mammogram.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 1))
                            goal.Mammogram = (int)Track.OnTrack;
                        else if (age >= 40)
                            goal.Mammogram = (int)Track.NotOnTrack;
                        else
                            goal.Mammogram = (int)Track.NotApplicable;

                        if (mammogram.LastReceived.HasValue)
                            goal.MammogramRecDate = mammogram.LastReceived.Value.ToShortDateString() + "~";
                        else
                            goal.MammogramRecDate = "~";
                        if (goal.Mammogram == (int)Track.NotOnTrack)
                            goal.MammogramRecDate += Constants.Now;
                        else if (age >= 40 && mammogram.LastReceived.HasValue)
                            goal.MammogramRecDate += mammogram.LastReceived.Value.AddYears(1).ToShortDateString();
                        else if (goal.Mammogram == (int)Track.NotApplicable)
                            goal.MammogramRecDate += Constants.NotApplicable;
                        else
                            goal.MammogramRecDate += Constants.Physician;
                    }
                    else
                    {
                        if (age >= 40)
                            goal.Mammogram = (int)Track.NotOnTrack;
                        else
                            goal.Mammogram = (int)Track.NotApplicable;
                        goal.MammogramRecDate = "~";
                        if (goal.Mammogram == (int)Track.NotApplicable)
                            goal.MammogramRecDate += Constants.NotApplicable;
                        else if (goal.Mammogram == (int)Track.NotOnTrack)
                            goal.MammogramRecDate += Constants.Now;
                    }
                }

                if (user.Gender.HasValue(1))
                {
                    var psa = list.Find(p => p.Type == (int)PreventiveServicesType.PSA);

                    if (psa != null)
                    {
                        if (psa.Recommend.IsTrue())
                            goal.PSAProstate = (int)Track.NotOnTrack;
                        else if (psa.Testing.IsTrue())
                            goal.PSAProstate = (int)Track.NotApplicable;
                        else if (psa.LastReceived.HasValue)
                            goal.PSAProstate = (int)Track.OnTrack;
                        else if (age >= 50)
                            goal.PSAProstate = (int)Track.Optional;
                        else
                            goal.PSAProstate = (int)Track.NotApplicable;

                        if (psa.LastReceived.HasValue)
                            goal.PSAProstateRecDate = psa.LastReceived.Value.ToShortDateString() + "~";
                        else
                            goal.PSAProstateRecDate = "~";
                        if (goal.PSAProstate == (int)Track.NotOnTrack)
                            goal.PSAProstateRecDate += Constants.Now;
                        else if (goal.PSAProstate == (int)Track.NotApplicable)
                            goal.PSAProstateRecDate += Constants.NotApplicable;
                        else if (age >= 50 && psa.LastReceived.HasValue)
                            goal.PSAProstateRecDate += psa.LastReceived.Value.AddYears(1).ToShortDateString();
                        else
                            goal.PSAProstateRecDate += Constants.Physician;
                    }
                    else
                    {
                        if (age >= 50)
                            goal.PSAProstate = (int)Track.Optional;
                        else
                            goal.PSAProstate = (int)Track.NotApplicable;

                        goal.PSAProstateRecDate = "~";
                        if (goal.PSAProstate == (int)Track.NotApplicable)
                            goal.PSAProstateRecDate += Constants.NotApplicable;
                        else
                            goal.PSAProstateRecDate += Constants.Physician;
                    }
                }

                gluc = list.Find(p => p.Type == (int)PreventiveServicesType.Glucose);
                hbA1c = list.Find(p => p.Type == (int)PreventiveServicesType.HbA1C);
                if (gluc != null || hbA1c != null)
                {
                    DateTime? glucoseDate = null;
                    if (gluc != null && gluc.LastReceived.HasValue)
                        glucoseDate = gluc.LastReceived.Value;
                    else if (biometric != null && biometric.GlucoseDate.HasValue)
                        glucoseDate = biometric.GlucoseDate.Value;

                    if ((gluc != null && gluc.Recommend.IsTrue()) || (hbA1c != null && hbA1c.Recommend.IsTrue()))
                        goal.DiabetesScreening = (int)Track.NotOnTrack;
                    else if (gluc != null && gluc.Testing.IsTrue())
                        goal.DiabetesScreening = (int)Track.NotApplicable;
                    else if (medicalHistandProviders.Diabetes.IsTrue() && (glucoseDate.LessThanOrEqualToMonths(wellnessVisit.AssessmentDate, 6)
                        || (hbA1c != null && hbA1c.LastReceived.LessThanOrEqualToMonths(wellnessVisit.AssessmentDate, 6))))
                        goal.DiabetesScreening = (int)Track.OnTrack;
                    else if (glucoseDate.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 1))
                        goal.DiabetesScreening = (int)Track.OnTrack;
                    else if (medicalHistandProviders.Diabetes.IsTrue() && hbA1c != null && hbA1c.LastReceived.GraterThanOrEqualToMonths(wellnessVisit.AssessmentDate, 6))
                        goal.DiabetesScreening = (int)Track.NotOnTrack;
                    else if (bmi.HasValue && bmi >= 25 && age > 40 && age < 70 && glucoseDate.GreaterThanOrEqualToYears(wellnessVisit.AssessmentDate, 3) &&
                        (hbA1c == null || hbA1c.LastReceived.GreaterThanOrEqualToYears(wellnessVisit.AssessmentDate, 3)))
                        goal.DiabetesScreening = (int)Track.NotOnTrack;
                    else if (medicalHistandProviders.Diabetes.IsNullOrFalse() && (!biometric.Glucose.HasValue || biometric.Glucose.Value < 100)
                        && (!biometric.A1C.HasValue || biometric.A1C.Value < (float)5.7) && age > 70)
                        goal.DiabetesScreening = (int)Track.NotApplicable;
                    else
                        goal.DiabetesScreening = (int)Track.Indeterminate;

                    if (glucoseDate.HasValue)
                        goal.DiabetesScreeningRecDate = glucoseDate.Value.ToShortDateString() + "~";
                    else if (hbA1c != null && hbA1c.LastReceived.HasValue)
                        goal.DiabetesScreeningRecDate = hbA1c.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.DiabetesScreeningRecDate = "~";

                    if (goal.DiabetesScreening == (int)Track.NotOnTrack)
                        goal.DiabetesScreeningRecDate += Constants.Now;
                    else if (goal.DiabetesScreening == (int)Track.NotApplicable)
                        goal.DiabetesScreeningRecDate += Constants.NotApplicable;
                    else
                        goal.DiabetesScreeningRecDate += Constants.Physician;
                }

                glaucoma = list.Find(p => p.Type == (int)PreventiveServicesType.Glaucoma_Screening);
                if (glaucoma != null)
                {
                    if (glaucoma.Recommend.IsTrue())
                        goal.Vision = (int)Track.NotOnTrack;
                    else if (glaucoma.Testing.IsTrue())
                        goal.Vision = (int)Track.NotApplicable;
                    else if (glaucoma.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 1))
                        goal.Vision = (int)Track.OnTrack;
                    else if (medicalHistandProviders.Diabetes.IsTrue() || (user.Race.HasValue && user.Race == 2 && age >= 50) || (user.Race.HasValue && user.Race == 5 && age >= 65))
                        goal.Vision = (int)Track.NotOnTrack;
                    else
                        goal.Vision = (int)Track.NotApplicable;

                    if (glaucoma.LastReceived.HasValue)
                        goal.VisionRecDate = glaucoma.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.VisionRecDate = "~";
                    if (goal.Vision == (int)Track.NotOnTrack)
                        goal.VisionRecDate += Constants.Now;
                    else if (glaucoma.LastReceived.HasValue && (medicalHistandProviders.Diabetes.IsTrue() || (user.Race.HasValue && user.Race == 2 && age >= 50) || (user.Race.HasValue && user.Race == 5 && age >= 65)))
                        goal.VisionRecDate += glaucoma.LastReceived.Value.AddYears(1).ToShortDateString();
                    else
                        goal.VisionRecDate += Constants.Physician;
                }

                hearing = list.Find(p => p.Type == (int)PreventiveServicesType.Hearing_Screening);
                if (hearing != null)
                {
                    if (hearing.Recommend.IsTrue())
                        goal.Hearing = (int)Track.NotOnTrack;
                    else if (hearing.Testing.IsTrue())
                        goal.Hearing = (int)Track.NotApplicable;
                    else if (hearing.LastReceived.HasValue)
                        goal.Hearing = (int)Track.OnTrack;
                    else if (age >= 50)
                        goal.Hearing = (int)Track.Optional;
                    else
                        goal.Hearing = (int)Track.NotApplicable;

                    if (hearing.LastReceived.HasValue)
                        goal.HearingRecDate = hearing.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.HearingRecDate = "~";

                    //if (hearing.LastReceived.HasValue)
                    //goal.HearingRecDate += hearing.LastReceived.Value.ToShortDateString();
                    if (hearing.Recommend.IsTrue())
                        goal.HearingRecDate += Constants.Now;
                    else if (goal.Hearing == (byte)Track.NotApplicable)
                        goal.HearingRecDate += Constants.NotApplicable;
                    else
                        goal.HearingRecDate += Constants.Physician;
                }

                if (user.Gender.HasValue(1))
                {
                    aortic = list.Find(p => p.Type == (int)PreventiveServicesType.Abdominal_Aortic_Aneurysm_Sonogram);
                    if (aortic != null)
                    {
                        if (aortic.Recommend.IsTrue())
                            goal.Aortic = (int)Track.NotOnTrack;
                        else if (aortic.LastReceived.HasValue)
                            goal.Aortic = (int)Track.OnTrack;
                        else if (aortic.Testing.IsTrue())
                            goal.Aortic = (int)Track.NotApplicable;
                        else if (age >= 65 && age <= 75 && generalHealth.UsedTob100Times.IsTrue())
                            goal.Aortic = (int)Track.NotOnTrack;
                        else
                            goal.Aortic = (int)Track.NotApplicable;

                        if (aortic.LastReceived.HasValue)
                            goal.AorticRecDate = glaucoma.LastReceived.Value.ToShortDateString() + "~";
                        else
                            goal.AorticRecDate = "~";

                        if (goal.Aortic == (byte)Track.NotOnTrack)
                            goal.AorticRecDate += Constants.Now;
                        else if (goal.Aortic == (byte)Track.OnTrack)
                            goal.AorticRecDate += Constants.NotNeeded;
                        else
                            goal.AorticRecDate += Constants.NotApplicable;
                    }
                }

                if (gluc == null && hbA1c == null)
                {

                    if (medicalHistandProviders.Diabetes.IsNullOrFalse() && (!biometric.Glucose.HasValue || biometric.Glucose.Value < 100)
                        && (!biometric.A1C.HasValue || biometric.A1C.Value < (float)5.7) && age > 70)
                        goal.DiabetesScreening = (int)Track.NotApplicable;

                    else if (biometric != null && biometric.GlucoseDate.HasValue)
                    {
                        DateTime? glucoseDate = biometric.GlucoseDate.Value;

                        if (medicalHistandProviders.Diabetes.IsTrue() && glucoseDate.LessThanOrEqualToMonths(wellnessVisit.AssessmentDate, 6))
                            goal.DiabetesScreening = (int)Track.OnTrack;
                        else if (glucoseDate.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 1))
                            goal.DiabetesScreening = (int)Track.OnTrack;
                        else if (medicalHistandProviders.Diabetes.IsTrue() && glucoseDate.GraterThanOrEqualToMonths(wellnessVisit.AssessmentDate, 6))
                            goal.DiabetesScreening = (int)Track.NotOnTrack;
                        else if (bmi.HasValue && bmi >= 25 && age > 40 && age < 70 && glucoseDate.GreaterThanOrEqualToYears(wellnessVisit.AssessmentDate, 3))
                            goal.DiabetesScreening = (int)Track.NotOnTrack;
                        else
                            goal.DiabetesScreening = (int)Track.Indeterminate;

                        if (glucoseDate.HasValue)
                            goal.DiabetesScreeningRecDate = glucoseDate.Value.ToShortDateString() + "~";
                        else
                            goal.DiabetesScreeningRecDate = "~";

                        if (goal.DiabetesScreening == (int)Track.NotOnTrack)
                            goal.DiabetesScreeningRecDate += Constants.Now;
                        else if (goal.DiabetesScreening == (int)Track.NotApplicable)
                            goal.DiabetesScreeningRecDate += Constants.NotApplicable;
                        else
                            goal.DiabetesScreeningRecDate += Constants.Physician;
                    }
                }

                var boneDensity = list.Find(p => p.Type == (int)PreventiveServicesType.Dexa_Scan);
                if (boneDensity != null)
                {
                    if (boneDensity.Recommend.IsTrue())
                        goal.Osteoporosis = (int)Track.NotOnTrack;
                    else if (boneDensity.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 2))
                        goal.Osteoporosis = (int)Track.OnTrack;
                    else if (boneDensity.Testing.IsTrue())
                        goal.Osteoporosis = (int)Track.NotApplicable;
                    else if (user.Gender.HasValue(2) && age >= 65 && boneDensity.LastReceived.GreaterThanOrEqualToYears(wellnessVisit.AssessmentDate, 2))
                        goal.Osteoporosis = (int)Track.NotOnTrack;
                    else
                        goal.Osteoporosis = (int)Track.Indeterminate;

                    if (boneDensity.LastReceived.HasValue)
                        goal.OsteoporosisRecDate = boneDensity.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.OsteoporosisRecDate = "~";
                    if (goal.Osteoporosis == (int)Track.NotOnTrack)
                        goal.OsteoporosisRecDate += Constants.Now;
                    else if (user.Gender.HasValue(2) && age >= 65 && boneDensity.LastReceived.HasValue)
                        goal.OsteoporosisRecDate += boneDensity.LastReceived.Value.AddYears(2).ToShortDateString();
                    else if (goal.Osteoporosis == (int)Track.NotApplicable)
                        goal.OsteoporosisRecDate += Constants.NotApplicable;
                    else
                        goal.OsteoporosisRecDate += Constants.Physician;
                }

                var tetanus = list.Find(p => p.Type == (int)PreventiveServicesType.Tetanus_Diphtheria);
                if (tetanus != null)
                {
                    if (tetanus.LastReceived.HasValue)
                        goal.TetanusRecDate = tetanus.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.TetanusRecDate = "~";

                    if (tetanus.Recommend.IsTrue())
                    {
                        goal.TetanusRecDate += Constants.Now;
                        goal.Tetanus = (int)Track.NotOnTrack;
                    }
                    else if (tetanus.Testing.IsTrue())
                    {
                        goal.TetanusRecDate += Constants.NotApplicable;
                        goal.Tetanus = (int)Track.NotApplicable;
                    }
                    else
                    {
                        goal.TetanusRecDate += Constants.Physician;
                        goal.Tetanus = (int)Track.Indeterminate;
                    }
                }

                pneum = list.Find(p => p.Type == (int)PreventiveServicesType.Pneumococcal);
                if (pneum != null)
                {
                    if (pneum.LastReceived.HasValue)
                        goal.PneumoniaRecDate = pneum.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.PneumoniaRecDate = "~";

                    if (pneum.Recommend.IsTrue())
                    {
                        goal.PneumoniaRecDate += Constants.Now;
                        goal.Pneumonia = (int)Track.NotOnTrack;
                    }
                    else if (pneum.Testing.IsTrue() || age < 65)
                    {
                        goal.PneumoniaRecDate += Constants.NotApplicable;
                        goal.Pneumonia = (int)Track.NotApplicable;
                    }
                    else
                    {
                        goal.PneumoniaRecDate += Constants.Physician;
                        goal.Pneumonia = (int)Track.Indeterminate;
                    }
                }

                var flu = list.Find(p => p.Type == (int)PreventiveServicesType.Influenza);
                if (flu != null)
                {
                    if (flu.LastReceived.HasValue)
                        goal.FluRecDate = flu.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.FluRecDate = "~";

                    if (flu.Recommend.IsTrue())
                    {
                        goal.FluRecDate += Constants.Now;
                        goal.Flu = (int)Track.NotOnTrack;
                    }
                    else if (flu.Testing.IsTrue())
                    {
                        goal.FluRecDate += Constants.NotApplicable;
                        goal.Flu = (int)Track.NotApplicable;
                    }
                    else if (flu.LastReceived.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 1))
                    {
                        goal.FluRecDate += Constants.YearlyFlu;
                        goal.Flu = (int)Track.OnTrack;
                    }
                    else
                    {
                        goal.FluRecDate += Constants.YearlyFlu;
                        goal.Flu = (int)Track.Indeterminate;
                    }
                }

                shingelles = list.Find(p => p.Type == (int)PreventiveServicesType.Shingelles);
                if (shingelles != null)
                {
                    if (shingelles.LastReceived.HasValue)
                        goal.ShinglesRecDate = shingelles.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.ShinglesRecDate = "~";

                    if (shingelles.Recommend.IsTrue())
                    {
                        goal.ShinglesRecDate += Constants.Now;
                        goal.Shingles = (int)Track.NotOnTrack;
                    }
                    else if (shingelles.LastReceived.HasValue)
                    {
                        goal.ShinglesRecDate += Constants.NotNeeded;
                        goal.Shingles = (int)Track.OnTrack;
                    }
                    else if (shingelles.Testing.IsTrue() || age < 60)
                    {
                        goal.ShinglesRecDate += Constants.NotApplicable;
                        goal.Shingles = (int)Track.NotApplicable;
                    }
                    else
                    {
                        goal.ShinglesRecDate += Constants.Physician;
                        goal.Shingles = (int)Track.Indeterminate;
                    }
                }


                var hepB = list.Find(p => p.Type == (int)PreventiveServicesType.Hepatitis_B);
                if (hepB != null)
                {
                    if (hepB.LastReceived.HasValue)
                        goal.HepatitisBRecDate = hepB.LastReceived.Value.ToShortDateString() + "~";
                    else
                        goal.HepatitisBRecDate = "~";

                    if (hepB.Recommend.IsTrue())
                    {
                        goal.HepatitisBRecDate += Constants.Now;
                        goal.HepatitisB = (int)Track.NotOnTrack;
                    }
                    else if (hepB.Testing.IsTrue())
                    {
                        goal.HepatitisBRecDate += Constants.NotApplicable;
                        goal.HepatitisB = (int)Track.NotApplicable;
                    }
                    else
                    {
                        goal.HepatitisBRecDate += Constants.Physician;
                        goal.HepatitisB = (int)Track.Indeterminate;
                    }
                }
            }
            // Not related to physical services data
            else
            {
                if (age >= 50 && age <= 85)
                    goal.Colorectal = (int)Track.NotOnTrack;
                else
                    goal.Colorectal = (int)Track.NotApplicable;

                goal.ColorectalRecDate = "~";
                if (goal.Colorectal == (int)Track.NotApplicable)
                {
                    goal.ColorectalRecDate += Constants.NotApplicable;
                }
                else
                {
                    goal.ColorectalRecDate += Constants.Physician;
                }
                if (user.Gender.HasValue(2))
                {
                    if (user.DOB.GreaterThanOrEqualToYears(wellnessVisit.AssessmentDate, 21) && user.DOB.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 65))
                        goal.PapTest = (int)Track.NotOnTrack;
                    else
                        goal.PapTest = (int)Track.NotApplicable;

                    goal.PapTestRecDate = "~";
                    if (goal.PapTest == (int)Track.NotApplicable)
                        goal.PapTestRecDate += Constants.NotApplicable;
                    else if (goal.PapTest == (int)Track.NotOnTrack)
                        goal.PapTestRecDate += Constants.Now;

                    if (age >= 40)
                        goal.Mammogram = (int)Track.NotOnTrack;
                    else
                        goal.Mammogram = (int)Track.NotApplicable;

                    goal.MammogramRecDate = "~";
                    if (goal.Mammogram == (int)Track.NotApplicable)
                        goal.MammogramRecDate += Constants.NotApplicable;
                    else if (goal.Mammogram == (int)Track.NotOnTrack)
                        goal.MammogramRecDate += Constants.Now;
                }
                else
                {
                    if (age >= 50)
                        goal.PSAProstate = (int)Track.Optional;
                    else
                        goal.PSAProstate = (int)Track.NotApplicable;

                    goal.PSAProstateRecDate = "~";
                    if (goal.PSAProstate == (int)Track.NotApplicable)
                        goal.PSAProstateRecDate += Constants.NotApplicable;
                    else
                        goal.PSAProstateRecDate += Constants.Physician;
                }

            }
            if (hearing == null)
            {
                if (age >= 50)
                    goal.Hearing = (int)Track.Optional;
                else
                    goal.Hearing = (int)Track.NotApplicable;

                if (goal.Hearing == (byte)Track.NotApplicable)
                    goal.HearingRecDate = Constants.NotApplicable;
                else
                    goal.HearingRecDate = Constants.Physician;
            }
            if (aortic == null && user.Gender.HasValue(1))
            {
                if (age >= 65 && age <= 75 && generalHealth.UsedTob100Times.IsTrue())
                    goal.Aortic = (int)Track.NotOnTrack;
                else
                    goal.Aortic = (int)Track.NotApplicable;

                if (goal.Aortic == (byte)Track.NotOnTrack)
                    goal.AorticRecDate = Constants.Now;
                else
                    goal.AorticRecDate = Constants.NotApplicable;
            }
            if (pneum == null && age < 65)
            {
                goal.Pneumonia = (int)Track.NotApplicable;
                goal.PneumoniaRecDate = Constants.NotApplicable;
            }

            if (glaucoma == null)
            {
                if (medicalHistandProviders.Diabetes.IsTrue() || (user.Race.HasValue && user.Race == 2 && age >= 50) || (user.Race.HasValue && user.Race == 5 && age >= 65))
                {
                    goal.Vision = (int)Track.NotOnTrack;
                    goal.VisionRecDate = Constants.Now;
                }
            }
            if (shingelles == null)
            {
                if (age < 60)
                {
                    goal.ShinglesRecDate = Constants.NotApplicable;
                    goal.Shingles = (int)Track.NotApplicable;
                }
            }

            DateTime? lipidDate = null;
            if (lipid != null && lipid.LastReceived.HasValue)
                lipidDate = lipid.LastReceived.Value;
            else if (biometric != null)
            {
                if (biometric.LDLDate.HasValue)
                    lipidDate = biometric.LDLDate.Value;
                else if (biometric.TotalCholDate.HasValue)
                    lipidDate = biometric.TotalCholDate.Value;
            }

            if ((lipid != null && lipid.Recommend.IsTrue()) || (ldl != null && ldl.Recommend.IsTrue()))
                goal.Lipoproteins = (int)Track.NotOnTrack;
            else if (lipid != null && lipid.Testing.IsTrue())
                goal.Lipoproteins = (int)Track.NotApplicable;
            else if (lipidDate.LessThanOrEqualToYears(wellnessVisit.AssessmentDate, 5) || (ldl != null && ldl.LastReceived.LessThanOrEqualToMonths(wellnessVisit.AssessmentDate, 6)))
                goal.Lipoproteins = (int)Track.OnTrack;
            else if (biometric != null && biometric.TotalChol.HasValue && biometric.HDL.HasValue && biometric.Trig.HasValue)
                goal.Lipoproteins = (int)Track.OnTrack;
            else
                goal.Lipoproteins = (int)Track.NotOnTrack;

            if (lipidDate.HasValue)
                goal.LipoproteinsRecDate = lipidDate.Value.ToShortDateString() + "~";
            else if (ldl != null && ldl.LastReceived.HasValue)
                goal.LipoproteinsRecDate = ldl.LastReceived.Value.ToShortDateString() + "~";
            else
                goal.LipoproteinsRecDate = "~";

            if (goal.Lipoproteins == (int)Track.NotOnTrack)
                goal.LipoproteinsRecDate += Constants.Now;
            else if (lipidDate.HasValue && ldl != null && ldl.LastReceived.HasValue)
            {
                if (lipidDate.Value.AddYears(5) > ldl.LastReceived.Value.AddMonths(6))
                    goal.LipoproteinsRecDate += ldl.LastReceived.Value.AddMonths(6).ToShortDateString();
                else
                    goal.LipoproteinsRecDate += lipidDate.Value.AddYears(5).ToShortDateString();
            }
            else if (lipidDate.HasValue)
                goal.LipoproteinsRecDate += lipidDate.Value.AddYears(5).ToShortDateString();
            else if (ldl != null && ldl.LastReceived.HasValue)
                goal.LipoproteinsRecDate += ldl.LastReceived.Value.AddMonths(6).ToShortDateString();
            else if (goal.Lipoproteins == (int)Track.NotApplicable)
                goal.LipoproteinsRecDate += Constants.NotApplicable;
            else
                goal.LipoproteinsRecDate += Constants.Physician;

            if (biometric != null && biometric.BPDate.HasValue)
                goal.BPScreeningRecDate = biometric.BPDate.Value.ToShortDateString() + "~";
            else
                goal.BPScreeningRecDate = "~";

            if (biometric != null && biometric.SBP.HasValue && biometric.DBP.HasValue)
            {
                goal.BPScreeningRecDate += Constants.NextPhysician;
                goal.BPScreening = (int)Track.OnTrack;
            }
            else
            {
                goal.BPScreening = (int)Track.NotOnTrack;
                goal.BPScreeningRecDate += Constants.Now;
            }

            if (biometric != null && biometric.Height.HasValue && biometric.Weight.HasValue)
            {
                goal.ObesityRecDate = Constants.NextPhysician;
                goal.Obesity = (int)Track.OnTrack;
            }
            else
            {
                goal.ObesityRecDate = Constants.Now;
                goal.Obesity = (int)Track.NotOnTrack;
            }

            if (age < 55 || age > 80 || generalHealth.UsedTob100Times.IsFalse())
            {
                goal.LungCancerRecDate = Constants.NotApplicable;
                goal.LungCancer = (int)Track.NotApplicable;
            }
            else
            {
                goal.LungCancerRecDate = Constants.Physician;
                goal.LungCancer = (int)Track.Indeterminate;
            }

            byte? hiv;
            if ((stdProstate.STD_MulPart.IsTrue() && stdProstate.STD_UseCondoms.IsFalse()) || stdProstate.STDRiskScreen.ContainsValue(new List<byte> { 2, 3, 4 }))
                hiv = (int)Risk.HR;
            else if (stdProstate.STD_Active.IsFalse() || stdProstate.STD_MulPart.IsFalse())
                hiv = (int)Risk.LR;
            else
                hiv = (int)Risk.ID;

            if (hivTest != null && hivTest.LastReceived.HasValue)
                goal.HIVRecDate = hivTest.LastReceived.Value.ToShortDateString() + "~";
            else
                goal.HIVRecDate = "~";
            if ((hivTest != null && hivTest.Recommend.IsTrue()) || stdProstate.STDRiskScreen.HasValue(3))
            {
                goal.HIVRecDate += Constants.Now;
                goal.HIV = (int)Track.NotOnTrack;
            }
            else if ((hivTest != null && hivTest.Testing.IsTrue()) || hiv == (int)Risk.LR)
            {
                goal.HIVRecDate += Constants.NotApplicable;
                goal.HIV = (int)Track.NotApplicable;
            }
            else
            {
                goal.HIV = (int)Track.Indeterminate;
                goal.HIVRecDate += Constants.Physician;
            }
            #endregion

            #region KeyActionSteps
            DAL.AWV_PreventiveServices tob = null, nutrition = null, obesity = null, diabet = null;
            if (list != null)
            {
                tob = list.Find(p => p.Type == (int)PreventiveServicesType.Tobacco_Cessation);
                nutrition = list.Find(p => p.Type == (int)PreventiveServicesType.Nutritional);
                obesity = list.Find(p => p.Type == (int)PreventiveServicesType.Obesity_Counseling);
                diabet = list.Find(p => p.Type == (int)PreventiveServicesType.Diabetes_Training);
            }
            if (tob != null && goal.TobaccoUse.HasValue((byte)Risk.HR) && tob.Recommend.IsTrue())
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("T1");
            else if (goal.TobaccoUse.HasValue((int)Risk.HR))
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("T2");

            if (nutrition != null && goal.Nutrition.HasValue((byte)Risk.HR) && nutrition.Recommend.IsTrue())
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("N1");
            else if (goal.Nutrition.HasValue((byte)Risk.HR))
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("N2");
            else if (nutrition != null && nutrition.Recommend.IsTrue())
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("N3");

            if (goal.PhysicalActivity.HasValue((byte)Risk.HR))
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("P1");

            if (goal.Stress.HasValue((byte)Risk.HR))
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("S1");

            if (goal.Weight.HasValue((byte)Risk.HR) && obesity != null && obesity.Recommend.IsTrue())
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("B1");
            else if (goal.Weight.HasValue((byte)Risk.HR))
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("B2");

            if (goal.Cholesterol.HasValue((byte)Risk.HR))
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("L1");
            else if (biometric == null || !biometric.Trig.HasValue || !biometric.LDL.HasValue || !biometric.HDL.HasValue)
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("L2");

            if (goal.BP.HasValue((byte)Risk.HR))
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("B1");
            else if (biometric == null || !biometric.SBP.HasValue || !biometric.DBP.HasValue)
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("B2");

            if (goal.GlucA1C.HasValue((byte)Risk.HR) && diabet != null && diabet.Recommend.IsTrue())
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("D1");
            else if (goal.GlucA1C.HasValue((byte)Risk.HR))
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("D2");
            else
            {
                if (diabet != null && diabet.Recommend.IsTrue())
                    goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("D3");
                if ((!medicalHistandProviders.Diabetes.HasValue && (biometric == null || (!biometric.A1C.HasValue && !biometric.Glucose.HasValue)))
                    || (medicalHistandProviders.Diabetes.HasValue && (biometric == null || !biometric.A1C.HasValue)))
                    goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("D4");
            }

            if (goal.Safety.HasValue((byte)Risk.HR))
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("SF1");


            if (goal.Alcohol.HasValue((byte)Risk.HR))
                goal.Lifestyle = goal.Lifestyle.AppendFormatedString("A1");

            if (depressionScreening.PHQ9_ProviderScore.HasValue(2))
                goal.Other = goal.Other.AppendFormatedString("M");
            if (homeScreening.HomeSafetyProviderScore.HasValue(2))
                goal.Other = goal.Other.AppendFormatedString("HO");
            if (homeScreening.FunActivityProviderScore.HasValue(2))
                goal.Other = goal.Other.AppendFormatedString("A");
            if (homeScreening.FallRiskProviderScore.HasValue(2))
                goal.Other = goal.Other.AppendFormatedString("F");
            if (homeScreening.HearingLossProviderScore.HasValue(2))
                goal.Other = goal.Other.AppendFormatedString("HE");
            if (urinaryScreening.UrnRiskScore.ContainsValue(new List<byte> { 2, 3 }))
                goal.Other = goal.Other.AppendFormatedString("U");
            if (alcoholMisUse.AlcoholScore.ContainsValue(new List<byte> { 2, 3 }))
                goal.Other = goal.Other.AppendFormatedString("D");
            if (stdProstate.STDRiskScreen.HasValue(4))
                goal.Other = goal.Other.AppendFormatedString("S");
            if (stdProstate.ProsRiskScreen.ContainsValue(new List<byte> { 2, 3, 4 }))
                goal.Other = goal.Other.AppendFormatedString("P");

            var notOnTrack = (byte)Track.NotOnTrack;
            if (goal.Colorectal.HasValue(notOnTrack) || goal.PapTest.HasValue(notOnTrack) || goal.Mammogram.HasValue(notOnTrack)
                || goal.PSAProstate.HasValue(notOnTrack) || goal.BPScreening.HasValue(notOnTrack) || goal.Lipoproteins.HasValue(notOnTrack)
                || goal.DiabetesScreening.HasValue(notOnTrack) || goal.Obesity.HasValue(notOnTrack) || goal.Vision.HasValue(notOnTrack)
                || goal.Hearing.HasValue(notOnTrack) || goal.Aortic.HasValue(notOnTrack) || goal.Osteoporosis.HasValue(notOnTrack)
                || goal.LungCancer.HasValue(notOnTrack) || goal.HIV.HasValue(notOnTrack))
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("IM");

            if (goal.Tetanus.HasValue(notOnTrack) || goal.Pneumonia.HasValue(notOnTrack) || goal.Flu.HasValue(notOnTrack)
                || goal.Shingles.HasValue(notOnTrack) || goal.HepatitisB.HasValue(notOnTrack))
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("V");
            #endregion

            #region Personal Goals
            string Safetygoals = "";
            if (generalHealth.SafetyIssuesAtHome.IsTrue())
                Safetygoals = Safetygoals.AppendFormatedString("GS");
            if (homeScreening.Home_Throwrugs.IsTrue())
                Safetygoals = Safetygoals.AppendFormatedString("HT");
            if (homeScreening.Home_SmokeAlarm.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HS");
            if (homeScreening.Home_BathtubSafety.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HB");
            if (homeScreening.Home_Bathmat.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HBM");
            if (homeScreening.Home_NightLight.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HN");
            if (homeScreening.Home_LooseCords.IsTrue())
                Safetygoals = Safetygoals.AppendFormatedString("HL");
            if (homeScreening.Home_UnplugAppl.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HU");
            if (homeScreening.Home_MedSafe.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HM");
            if (homeScreening.Home_KnifeSafe.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HK");
            if (homeScreening.Home_ChemsSafe.IsFalse())
                Safetygoals = Safetygoals.AppendFormatedString("HC");
            if (homeScreening.Home_SharpFurn.IsTrue())
                Safetygoals = Safetygoals.AppendFormatedString("HF");
            goal.SafetyGoals = Safetygoals;
            #endregion

            #region Health Referral
            byte hr = (byte)Risk.HR;
            if (age < 85 && (!(generalHealth.BotheredbyDizzy.IsTrue() || generalHealth.MemoryProblems.IsTrue() || depressionScreening.TroubleConc.HasValue(4)
                    || depressionScreening.PHQ9_Score.HasValue(5) || depressionScreening.PHQ9_ProviderScore.HasValue(2) || homeScreening.Fun_KeepApp.IsFalse()
                    || homeScreening.Fun_KeepEvents.IsFalse() || homeScreening.HearingLossProviderScore.HasValue(2)))
                  && (goal.TobaccoUse.HasValue(hr) || goal.PhysicalActivity.HasValue(hr) || goal.Nutrition.HasValue(hr) ||
                 goal.Stress.HasValue(hr) || goal.Weight.HasValue(hr) || goal.BP.HasValue(hr) || goal.Cholesterol.HasValue(hr) || goal.GlucA1C.HasValue(hr) ||
                 (tob != null && tob.Recommend.IsTrue()) || (nutrition != null && nutrition.Recommend.IsTrue()) ||
                  (obesity != null && obesity.Recommend.IsTrue()) || (diabet != null && diabet.Recommend.IsTrue())))
                goal.CardioSmart = 1;
            if (tob != null && tob.Recommend.IsTrue())
                goal.TobaccoCes = 1;
            if (nutrition != null && nutrition.Recommend.IsTrue())
                goal.NutritionalMgnt = 1;
            if (obesity != null && obesity.Recommend.IsTrue())
                goal.WeightMgnt = 1;
            if (diabet != null && diabet.Recommend.IsTrue())
                goal.DiabeticMgnt = 1;
            #endregion

            #region Health Measurements
            goal.ASCVD = false;
            if (goal.Cardiovascular.HasValue((byte)Risk.HR))
                goal.ASCVD = true;
            if (biometric != null)
            {
                //LtWt
                if (biometric.Weight.HasValue && biometric.Height.HasValue)
                {
                    var A = biometric.Weight.Value * 0.1;  /* these are calculated BEFORE conversion */
                    var @B = biometric.Weight.Value - A;
                    /* convert the units */
                    var WeightKg = biometric.Weight.Value / 2.2;
                    var HeightMt = biometric.Height.Value / 39.37;
                    var C = 27 * (HeightMt * HeightMt) * 2.2;
                    var D = 25 * (HeightMt * HeightMt) * 2.2;
                    var F = 18.5 * (HeightMt * HeightMt) * 2.2;
                    //Set	@E	=	isnull(@GoalWt,@D) ?
                    double? LtWt = null;
                    /* weight */
                    if (@B > C)
                        LtWt = D;
                    else if (B <= C && B >= D)
                        LtWt = D;
                    else if (B <= D && biometric.Weight.Value > D)
                        LtWt = D;
                    else if (biometric.Weight.Value <= D)
                        LtWt = D;

                    if (LtWt.HasValue)
                        goal.LtWt = (float)LtWt;
                    //LtLdl, LtHdl hardocode since the risk considers bp medication
                }

                float? tenYearASCVD, tenYearASCVDGoal, lifeASCVD, lifeASCVDGoal, tenProb, tenLow;
                CalcAWVGoal(biometric.SBP, biometric.DBP, biometric.HDL, biometric.TotalChol, medicalHistandProviders.Diabetes.IsTrue(),
                    generalHealth.TobInLast4Weeks, age, user.Gender.Value, user.Race, medicalHistandProviders.Hypertension,
                    out tenProb, out tenLow, out tenYearASCVD, out tenYearASCVDGoal, out lifeASCVD, out lifeASCVDGoal);
                goal.TenYearASCVD = tenYearASCVD;
                goal.TenYearASCVDGoal = tenYearASCVDGoal;
                goal.LifetimeASCVDGoal = lifeASCVDGoal;
                goal.LifetimeASCVD = lifeASCVD;
                goal.TenYrLow = tenLow;
                goal.TenYrProb = tenProb;
            }

            if (medicalHistandProviders.Diabetes.IsTrue() || medicalHistandProviders.KidneyDisease.IsTrue())
            {
                goal.LtSBP = 130;
                goal.LtDBP = 80;
            }
            else
            {
                goal.LtSBP = 140;
                goal.LtDBP = 90;
            }

            //@InsulinUser=1 or (isnull(@Glucose,0)>125 and @DidYouFast=1) or @Diabetes=1 // dont know how to map insulin
            if (medicalHistandProviders.Diabetes.IsTrue() || (biometric != null && biometric.Glucose.HasValue
                                    && biometric.Glucose.Value > 125 && biometric.Fasting.IsNullOrTrue()))
            {
                goal.LtGluc1 = 80;	//		-- 70 ...changed July 2015
                goal.LtGluc2 = 130;
            }
            else
            {
                goal.LtGluc1 = goal.LtGluc2 = 100;
            }

            #endregion

            #region DrReferral

            bool statinRef = false;
            if ((goal.ASCVD && age <= 75) || (biometric != null && biometric.LDL.HasValue && biometric.LDL.Value >= 190)
                || (age >= 40 && age <= 75 && medicalHistandProviders.Diabetes.IsTrue() && biometric != null && biometric.LDL.HasValue &&
                biometric.LDL.Value >= 70 && biometric.LDL.Value <= 189 && goal.TenYearASCVD >= (float)7.5))
            {
                statinRef = true;
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L1334");
            }
            else if ((goal.ASCVD && age > 75) || (age >= 40 && age <= 75 && medicalHistandProviders.Diabetes.IsTrue() && biometric != null && biometric.LDL.HasValue
                        && biometric.LDL.Value >= 70 && biometric.LDL.Value <= 189 && goal.TenYearASCVD < (float)7.5))
            {
                statinRef = true;
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L1335");
            }
            else if ((age >= 40 && age <= 75 && biometric != null && biometric.LDL.HasValue && biometric.LDL.Value >= 70 && biometric.LDL.Value <= 189 && goal.TenYearASCVD >= (float)7.5))
            {
                statinRef = true;
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L1336");
            }
            else if ((age >= 40 && age <= 75 && medicalHistandProviders.Diabetes.IsTrue() && biometric != null && biometric.LDL.HasValue
                        && biometric.LDL.Value >= 70 && biometric.LDL.Value <= 189)
                    || (biometric != null && biometric.LDL.HasValue && biometric.LDL.Value >= 160 && biometric.LDL.Value <= 189)
                    || (biometric != null && biometric.LDL.HasValue && biometric.LDL.Value >= 100 && biometric.LDL.Value <= 159 && goal.FHCardiovascular.IsTrue())
                    || (biometric != null && biometric.LDL.HasValue && biometric.LDL.Value == 0 && goal.FHCardiovascular.IsTrue()))
            {
                statinRef = true;

                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L1337");
            }
            if (biometric != null && ((biometric.SBP.HasValue && biometric.SBP.Value >= 140) || (biometric.DBP.HasValue && biometric.DBP.Value >= 90)))
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L337");
            if (!statinRef && biometric != null && biometric.LDL.HasValue && biometric.LDL.Value > 129 && age < 85)
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L338");
            if (biometric != null && biometric.Trig.HasValue && biometric.Trig.Value > 449)
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L340");
            if (!statinRef && biometric != null && biometric.HDL.HasValue && biometric.Trig.HasValue && biometric.HDL.Value < 40
                && biometric.Trig.Value > 200 && biometric.Trig.Value < 499 && age < 85)
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L341");
            else if (!statinRef && biometric != null && biometric.HDL.HasValue && biometric.HDL.Value < 30 && age < 85)
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L342");
            if (goal.GlucA1C.HasValue(hr))
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L357");
            if (((medicalHistandProviders.HeartAttack.IsTrue() || medicalHistandProviders.HeartDisease.IsTrue() || medicalHistandProviders.Pacemaker.IsTrue()
                || medicalHistandProviders.HeartSurgery.IsTrue() || medicalHistandProviders.Stroke.IsTrue()) &&
                !(ostScreening.BloodThinMed.IsTrue() || medicalHistandProviders.BloodDisorder.IsTrue() || medicalHistandProviders.Ulcer.IsTrue()))
                || ((age >= 50 && age <= 69) && goal.TenYearASCVD >= 10 &&
                !(ostScreening.BloodThinMed.IsTrue() || medicalHistandProviders.BloodDisorder.IsTrue() || medicalHistandProviders.Ulcer.IsTrue())))
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L360");
            if ((tobaccoAid != null && tobaccoAid.Find(t => t.TypeId == (int)AidType.Ready && (t.AidId >= 1 && t.AidId <= 6)) != null)
                || (generalHealth.TobInLast4Weeks.IsTrue() && tobaccosUse.Cigarettes.IsTrue() && !tobaccosUse.NoReady.HasValue))
                goal.DrReferralCode = goal.DrReferralCode.AppendFormatedString("L361");
            #endregion

            if (!string.IsNullOrEmpty(goal.DrReferralCode))
                goal.PreventiveCare = goal.PreventiveCare.AppendFormatedString("R");

            goal.DateCreated = DateTime.UtcNow;
            goal.Id = wellnessVisit.Id;
            context.AWV_Goals.Add(goal);
            context.SaveChanges();

            return goal;
        }

        #endregion

        #region Misc

        private void CalcAWVGoal(short? sbp, short? dbp, float? hdl, float? totalChol, bool diabetes, byte? tobacco, int age, byte gender, int? race, byte? bpMed, out float? tenProb, out float? tenlow, out float? tenASCVD, out float? tenASCVDGoal, out float? lifeASCVD, out float? lifeASCVDGoal)
        {
            tenProb = tenlow = tenASCVD = tenASCVDGoal = lifeASCVD = lifeASCVDGoal = null;
            StoredProcedures sp = new StoredProcedures();
            SqlParameter lifetimeASCVD = new SqlParameter("@LifetimeASCVD", 0) { Direction = ParameterDirection.Output };
            SqlParameter lifetimeASCVDGoal = new SqlParameter("@LifetimeASCVDGoal", 0) { Direction = ParameterDirection.Output };
            SqlParameter tenYrASCVD = new SqlParameter("@TenYearASCVD", 0) { Direction = ParameterDirection.Output };
            SqlParameter tenYrASCVDGoal = new SqlParameter("@TenYearASCVDGoal", 0) { Direction = ParameterDirection.Output };
            SqlParameter tenYrProb = new SqlParameter("@TenYearProb", 0) { Direction = ParameterDirection.Output };
            SqlParameter tenYrLow = new SqlParameter("@TenYearLow", 0) { Direction = ParameterDirection.Output };
            sp.CalcAWVGoal(gender.ToString(), age, totalChol, hdl, sbp, dbp, bpMed, race, diabetes, tobacco, lifetimeASCVD, lifetimeASCVDGoal, tenYrASCVD, tenYrASCVDGoal, tenYrProb, tenYrLow);
            if (tenYrLow.Value != DBNull.Value)
                tenlow = (float)tenYrLow.Value;
            if (tenYrProb.Value != DBNull.Value)
                tenProb = (float)tenYrProb.Value;
            if (lifetimeASCVD.Value != DBNull.Value)
                lifeASCVD = (float)lifetimeASCVD.Value;
            if (lifetimeASCVDGoal.Value != DBNull.Value)
                lifeASCVDGoal = (float)lifetimeASCVDGoal.Value;
            if (tenYrASCVD.Value != DBNull.Value)
                tenASCVD = (float)tenYrASCVD.Value;
            if (tenYrASCVDGoal.Value != DBNull.Value)
                tenASCVDGoal = (float)tenYrASCVDGoal.Value;
        }

        public AnnulWellnessVisitGeneralInfoDto MapAWVFromDAL(DAL.AWV awv)
        {
            AnnulWellnessVisitGeneralInfoDto wellnessData = new AnnulWellnessVisitGeneralInfoDto();
            wellnessData.IsIPPE = awv.IPPE;
            wellnessData.IsAWV = awv.AWV1;
            wellnessData.IsSubsequentAWV = awv.SubAWV;
            wellnessData.PracticeAddress = awv.ProviderAddress;
            wellnessData.PracticeName = awv.ProviderName;
            wellnessData.AssessmentDate = awv.AssessmentDate;
            wellnessData.StaffName = awv.ConductedBy;
            return wellnessData;
        }
        #endregion
    }
}
