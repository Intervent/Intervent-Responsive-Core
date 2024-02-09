using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Intervent.DAL
{
    public partial class StoredProcedures
    {
        readonly InterventDatabase _interventDatabase = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public virtual void CalcAWVGoal(string gender, int? ageYears, float? totalChol, float? hdl, int? sbp, int? dbp, byte? highBPMed, int? race, bool? diabetes, byte? smokeCig, SqlParameter lifetimeASCVD, SqlParameter lifetimeASCVDGoal, SqlParameter tenYearASCVD, SqlParameter tenYearASCVDGoal, SqlParameter tenYearProb, SqlParameter tenYearLow)
        {
            SqlParameter genderParameter = gender != null ? new SqlParameter("@Gender", gender) : new SqlParameter("@Gender", DBNull.Value);
            SqlParameter ageYearsParameter = ageYears.HasValue ? new SqlParameter("@AgeYears", ageYears) : new SqlParameter("@AgeYears", DBNull.Value);
            SqlParameter totalCholParameter = totalChol.HasValue ? new SqlParameter("@TotalChol", totalChol) : new SqlParameter("@TotalChol", DBNull.Value);
            SqlParameter hdlParameter = hdl.HasValue ? new SqlParameter("@Hdl", hdl) : new SqlParameter("@Hdl", DBNull.Value);
            SqlParameter sbpParameter = sbp.HasValue ? new SqlParameter("@Sbp", sbp) : new SqlParameter("@Sbp", DBNull.Value);
            SqlParameter dbpParameter = dbp.HasValue ? new SqlParameter("@Dbp", dbp) : new SqlParameter("@Dbp", DBNull.Value);
            SqlParameter highBPMedParameter = highBPMed.HasValue ? new SqlParameter("@HighBPMed", highBPMed) : new SqlParameter("@HighBPMed", DBNull.Value);
            SqlParameter raceParameter = race.HasValue ? new SqlParameter("@Race", race) : new SqlParameter("@Race", DBNull.Value);
            SqlParameter diabetesParameter = diabetes.HasValue ? new SqlParameter("@Diabetes", diabetes) : new SqlParameter("@Diabetes", DBNull.Value);
            SqlParameter smokeCigParameter = smokeCig.HasValue ? new SqlParameter("@SmokeCig", smokeCig) : new SqlParameter("@SmokeCig", DBNull.Value);
            _interventDatabase.Database.ExecuteSqlRaw("EXEC CalcAWVGoal @Gender, @AgeYears, @TotalChol, @Hdl, @Sbp, @Dbp, @HighBPMed, @Race, @Diabetes, @SmokeCig, @LifetimeASCVD OUT, @LifetimeASCVDGoal OUT, @TenYearASCVD OUT, @TenYearASCVDGoal OUT, @TenYearProb OUT, @TenYearLow OUT", genderParameter, ageYearsParameter, totalCholParameter, hdlParameter, sbpParameter, dbpParameter, highBPMedParameter, raceParameter, diabetesParameter, smokeCigParameter, lifetimeASCVD, lifetimeASCVDGoal, tenYearASCVD, tenYearASCVDGoal, tenYearProb, tenYearLow);
        }

        public List<LegacyAppointmentRemainderResult> GetTextMessageCallListFromLegacySystem(int? AppRef = null, string messageSID = null, string Operation = "select")
        {
            SqlParameter AppRefParameter = AppRef.HasValue ? new SqlParameter("@AppRef", AppRef) : new SqlParameter("@AppRef", DBNull.Value);
            SqlParameter MessageSIDParameter = messageSID != null ? new SqlParameter("@MessageSID", messageSID) : new SqlParameter("@MessageSID", DBNull.Value);
            SqlParameter OperationParameter = new SqlParameter("@Operation", Operation);
            SqlParameter apptStartParameter = new SqlParameter("@ApptFrom", DateTime.UtcNow);
            SqlParameter apptEndParameter = new SqlParameter("@ApptTo", DateTime.UtcNow.AddHours(1).AddMinutes(15));
            return _interventDatabase.LegacyAppointmentRemainderResult.FromSqlRaw("EXEC GetApptRemainderListForLegacySystem @ApptFrom, @ApptTo, @Operation, @MessageSID, @AppRef", apptStartParameter, apptEndParameter, OperationParameter, MessageSIDParameter, AppRefParameter).ToList();
        }

        public List<LegacyAppointmentRemainderResult> GetCallListFromLegacySystem()
        {
            return _interventDatabase.LegacyAppointmentRemainderResult.FromSqlRaw("GetApptForCallsfromLegSys").ToList();
        }

        public virtual int CreateOptions(int questionId, int? optionId, short sequenceNo, string optionText)
        {
            SqlParameter questionIdParameter = new SqlParameter("@QuestionId", questionId);
            SqlParameter optionTextParameter = new SqlParameter("@OptionText", optionText);
            SqlParameter sequenceNoParameter = new SqlParameter("@SequenceNo", sequenceNo);
            SqlParameter optionIdParameter = (optionId.HasValue && optionId.Value > 0) ? new SqlParameter("@OptionId", optionId.Value) : new SqlParameter("@OptionId", DBNull.Value);

            return _interventDatabase.Database.ExecuteSqlRaw("InsertOptions @QuestionId, @OptionId,@OptionText,@SequenceNo", questionIdParameter, optionIdParameter, optionTextParameter, sequenceNoParameter);
        }

        public virtual int InsertQuestions(int activityId, string questionText, short questionType, bool isActive, int parentId)
        {
            SqlParameter activityIdParameter = new SqlParameter("@ActivityId", activityId);
            SqlParameter questionTextParameter = new SqlParameter("@QuestionText", questionText);
            SqlParameter questionTypeParameter = new SqlParameter("@QuestionType", questionType);
            SqlParameter isActivetParameter = new SqlParameter("@IsActive", isActive);
            SqlParameter parentIdParameter = new SqlParameter("@ParentId", parentId);

            var result = _interventDatabase.Database.ExecuteSqlRaw("InsertQuestions @ActivityId, @QuestionText, @QuestionType, @IsActive, @ParentId", activityIdParameter, questionTextParameter, questionTypeParameter, isActivetParameter, parentIdParameter);
            return result;
        }

        public void DeleteCoachAvailability(SqlParameter sqlParameter, SqlParameter updatedBy, SqlParameter updatedOn)
        {
            _interventDatabase.Database.ExecuteSqlRaw("DeleteCoachAvailability @UpdatedBy, @UpdatedOn, @AvailId", updatedBy, updatedOn, sqlParameter);
        }

        public virtual int EligibilityUpdateTermDate(int? portalId)
        {
            SqlParameter portalIdParameter = portalId.HasValue ? new SqlParameter("@PortalId", portalId) : new SqlParameter("@PortalId", DBNull.Value);
            var result = _interventDatabase.Database.ExecuteSqlRaw("EligibilityUpdateTermDate @PortalId", portalIdParameter);
            return result;
        }

        public virtual void GenFollowUpGoals(int? id)
        {
            SqlParameter idParameter = id.HasValue ? new SqlParameter("@Id", id) : new SqlParameter("@Id", DBNull.Value);

            _interventDatabase.Database.ExecuteSqlRaw("GenFollowUpGoals @Id", idParameter);
        }

        public virtual float? GetAvgWellnessScore(int? id)
        {
            SqlParameter idParameter = id.HasValue ? new SqlParameter("@Id", id) : new SqlParameter("@Id", DBNull.Value);

            var result = _interventDatabase.Database.ExecuteSqlRaw("GetAvgWellnessScore @Id", idParameter);
            return result;
        }

        public virtual List<GetCoachAvailability_Result> GetCoachAvailability(int? organizationId, int? coachId, DateTime? startDate, DateTime? endDate, bool? video, string specialities, bool? adminView, string languages, int? stateId)
        {
            SqlParameter organizationIdParameter = organizationId.HasValue ? new SqlParameter("@OrganizationId", organizationId) : new SqlParameter("@OrganizationId", DBNull.Value);
            SqlParameter coachIdParameter = coachId.HasValue ? new SqlParameter("@CoachId", coachId) : new SqlParameter("@CoachId", DBNull.Value);
            SqlParameter startDateParameter = startDate.HasValue ? new SqlParameter("@StartDate", startDate) : new SqlParameter("@StartDate", DBNull.Value);
            SqlParameter endDateParameter = endDate.HasValue ? new SqlParameter("@EndDate", endDate) : new SqlParameter("@EndDate", DBNull.Value);
            SqlParameter videoParameter = video.HasValue ? new SqlParameter("@Video", video) : new SqlParameter("@Video", DBNull.Value);
            SqlParameter specialitiesManagerParameter = !String.IsNullOrEmpty(specialities) ? new SqlParameter("@SpecialityManager", specialities) : new SqlParameter("@SpecialityManager", DBNull.Value);
            SqlParameter languagesManagerParameter = !String.IsNullOrEmpty(languages) ? new SqlParameter("@LanguageManager", languages) : new SqlParameter("@LanguageManager", DBNull.Value);
            SqlParameter stateIdParameter = stateId.HasValue ? new SqlParameter("@stateId", stateId) : new SqlParameter("@stateId", DBNull.Value);
            SqlParameter adminViewParameter = adminView.HasValue ? new SqlParameter("@adminView", adminView) : new SqlParameter("@adminView", DBNull.Value);

            return _interventDatabase.GetCoachAvailability_Result.FromSqlRaw("GetCoachAvailability @OrganizationId, @CoachId, @StartDate, @EndDate, @Video, @SpecialityManager, @LanguageManager, @adminView, @stateId", organizationIdParameter, coachIdParameter, startDateParameter, endDateParameter, videoParameter, specialitiesManagerParameter, languagesManagerParameter, adminViewParameter, stateIdParameter).ToList();
        }

        public virtual List<GetMessages_Result> GetMessages(bool? isAdmin, string searchText, bool? drafts, int? userId, int? sysAdminId, int? coachId, bool? unread, int? PageNumber, int? pageSize, int? total)
        {
            SqlParameter isAdminParameter = isAdmin.HasValue ? new SqlParameter("@isAdmin", isAdmin) : new SqlParameter("@isAdmin", DBNull.Value);
            SqlParameter searchTextParameter = !string.IsNullOrEmpty(searchText) ? new SqlParameter("@searchText", searchText) : new SqlParameter("@searchText", DBNull.Value);
            SqlParameter draftsParameter = drafts.HasValue ? new SqlParameter("@drafts", drafts) : new SqlParameter("@drafts", DBNull.Value);
            SqlParameter userIdParameter = userId.HasValue ? new SqlParameter("@userId", userId) : new SqlParameter("@userId", DBNull.Value);
            SqlParameter sysAdminIdParameter = sysAdminId.HasValue ? new SqlParameter("@sysAdminId", sysAdminId) : new SqlParameter("@sysAdminId", DBNull.Value);
            SqlParameter unreadParameter = unread.HasValue ? new SqlParameter("@unread", unread) : new SqlParameter("@unread", DBNull.Value);
            SqlParameter coachIdParameter = coachId.HasValue ? new SqlParameter("@coachId", coachId) : new SqlParameter("@coachId", DBNull.Value);
            SqlParameter PageNumberParameter = PageNumber.HasValue ? new SqlParameter("@PageNumber", PageNumber) : new SqlParameter("@PageNumber", DBNull.Value);
            SqlParameter pageSizeParameter = pageSize.HasValue ? new SqlParameter("@PageSize", pageSize) : new SqlParameter("@pageSize", DBNull.Value);
            SqlParameter totalParameter = total.HasValue ? new SqlParameter("@total", total) : new SqlParameter("@total", DBNull.Value);

            return _interventDatabase.GetMessages_Result.FromSqlRaw("GetMessages @isAdmin, @searchText, @Drafts, @userId, @sysAdminId, @coachId, @unread, @PageNumber, @pageSize, @total",
            isAdminParameter, searchTextParameter, draftsParameter, userIdParameter, sysAdminIdParameter, coachIdParameter, unreadParameter, PageNumberParameter, pageSizeParameter, totalParameter).ToList();
        }

        public virtual List<GetCoachList_Result> GetCoachList(string organizationIds, bool? active, int? stateId, bool? allowAppt)
        {
            SqlParameter organizationIdParameter = organizationIds.Length != 0 ? new SqlParameter("@OrganizationIds", organizationIds) : new SqlParameter("@OrganizationIds", DBNull.Value);
            SqlParameter activeParameter = active.HasValue ? new SqlParameter("@Active", active) : new SqlParameter("@Active", DBNull.Value);
            SqlParameter stateIdParameter = stateId.HasValue ? new SqlParameter("@stateId", stateId) : new SqlParameter("@stateId", DBNull.Value);
            SqlParameter allowApptParameter = allowAppt.HasValue ? new SqlParameter("@AllowAppt", allowAppt) : new SqlParameter("@AllowAppt", DBNull.Value);
            return _interventDatabase.GetCoachList_Result.FromSqlRaw("GetCoachList @OrganizationIds, @Active, @stateId, @AllowAppt", organizationIdParameter, activeParameter, stateIdParameter, allowApptParameter).ToList();
        }

        public virtual List<FilteredCoachListResult> GetFilteredCoachList(string coachName, string speciality, string language, int? organizationId, DateTime? startDate, DateTime? endDate, int? stateId)
        {
            SqlParameter coachNameParameter = !string.IsNullOrEmpty(coachName) ? new SqlParameter("@coachName", coachName) : new SqlParameter("@coachName", DBNull.Value);
            SqlParameter specialityParameter = !string.IsNullOrEmpty(speciality) ? new SqlParameter("@speciality", speciality) : new SqlParameter("@speciality", DBNull.Value);
            SqlParameter languageParameter = !string.IsNullOrEmpty(language) ? new SqlParameter("@language", language) : new SqlParameter("@language", DBNull.Value);
            SqlParameter startDateParameter = startDate != DateTime.MinValue ? new SqlParameter("@startDate", startDate) : new SqlParameter("@startDate", DBNull.Value);
            SqlParameter endDateParameter = endDate != DateTime.MinValue ? new SqlParameter("@endDate", endDate) : new SqlParameter("@endDate", DBNull.Value);
            SqlParameter organizationIdParameter = organizationId.HasValue ? new SqlParameter("@OrganizationId", organizationId) : new SqlParameter("@OrganizationId", DBNull.Value);
            SqlParameter stateIdParameter = stateId.HasValue ? new SqlParameter("@stateId", stateId) : new SqlParameter("@stateId", DBNull.Value);
            return _interventDatabase.FilteredCoachListResult.FromSqlRaw("GetFilteredCoachList @coachName, @speciality, @language, @OrganizationId, @startDate, @endDate, @stateId", coachNameParameter, specialityParameter, languageParameter, organizationIdParameter, startDateParameter, endDateParameter, stateIdParameter).ToList();
        }

        public virtual List<GetDataforOutreach_Result> GetDataforOutreach()
        {
            return _interventDatabase.GetDataforOutreach_Result.FromSqlRaw("GetDataforOutreach").ToList();
        }

        public virtual List<GetDataforTracking_Result> GetDataforTracking()
        {
            return _interventDatabase.GetDataforTracking_Result.FromSqlRaw("GetDataforTracking").ToList();
        }

        public virtual List<ListUsers_Result> ListUsers(string firstName, string lastName, int? organizationId, int? id, string email, string telephone, string riskCode, DateTime? hRAFrom, DateTime? hRATo, int? userId, int? pageNumber, int? pageSize, bool? total)
        {
            SqlParameter firstNameParameter = firstName != null ? new SqlParameter("@FirstName", firstName) : new SqlParameter("@FirstName", DBNull.Value);
            SqlParameter lastNameParameter = lastName != null ? new SqlParameter("@LastName", lastName) : new SqlParameter("@LastName", DBNull.Value);
            SqlParameter organizationIdParameter = organizationId != null ? new SqlParameter("@OrganizationId", organizationId) : new SqlParameter("@OrganizationId", DBNull.Value);
            SqlParameter idParameter = id != null ? new SqlParameter("@Id", id) : new SqlParameter("@Id", DBNull.Value);
            SqlParameter emailParameter = email != null ? new SqlParameter("@Email", email) : new SqlParameter("@Email", DBNull.Value);
            SqlParameter telephoneParameter = telephone != null ? new SqlParameter("@Telephone", telephone) : new SqlParameter("@Telephone", DBNull.Value);
            SqlParameter riskCodeParameter = riskCode != null ? new SqlParameter("@RiskCode", riskCode) : new SqlParameter("@RiskCode", DBNull.Value);
            SqlParameter userIdParameter = userId != null ? new SqlParameter("@UserId", userId) : new SqlParameter("@UserId", DBNull.Value);
            SqlParameter hRAFromParameter = hRAFrom != null ? new SqlParameter("@HRAFrom", hRAFrom) : new SqlParameter("@HRAFrom", DBNull.Value);
            SqlParameter hRAToParameter = hRATo != null ? new SqlParameter("@HRATo", hRATo) : new SqlParameter("@HRATo", DBNull.Value);
            SqlParameter pageNumberParameter = pageNumber != null ? new SqlParameter("@PageNumber", pageNumber) : new SqlParameter("@PageNumber", DBNull.Value);
            SqlParameter pageSizeParameter = pageSize != null ? new SqlParameter("@PageSize", pageSize) : new SqlParameter("@PageSize", DBNull.Value);
            SqlParameter totalParameter = total != null ? new SqlParameter("@total", total) : new SqlParameter("@total", DBNull.Value);

            return _interventDatabase.ListUsers_Result.FromSqlRaw("ListUsers @FirstName, @LastName, @OrganizationId, @Id, @Email, @Telephone, @RiskCode, @HRAFrom, @HRATo, @UserId, @PageNumber, @PageSize, @total", firstNameParameter, lastNameParameter, organizationIdParameter, idParameter, emailParameter, telephoneParameter, riskCodeParameter, hRAFromParameter, hRAToParameter, userIdParameter, pageNumberParameter, pageSizeParameter, totalParameter).ToList();
        }

        public virtual List<GetDashboadMessages_Result> GetDashboadMessages(int userId, DateTime? @portalStartDate, bool isBoth, int messageType, bool newMessage, int pageNumber, int pageSize)
        {
            SqlParameter userIdParameter = new SqlParameter("@userId", userId);
            SqlParameter portalStartDateParameter = portalStartDate.HasValue ? new SqlParameter("@portalStartDate", portalStartDate) : new SqlParameter("@portalStartDate", DBNull.Value);
            SqlParameter isBothParameter = new SqlParameter("@isBoth", isBoth);
            SqlParameter messageTypeParameter = new SqlParameter("@messageType", messageType);
            SqlParameter newMessageParameter = new SqlParameter("@newMessage", newMessage);
            SqlParameter pageNumberParameter = new SqlParameter("@pageNumber", pageNumber);
            SqlParameter pageSizeParameter = new SqlParameter("@pageSize", pageSize);

            return _interventDatabase.GetDashboadMessages_Result.FromSqlRaw("GetDashboadMessages @userId, @portalStartDate, @isBoth, @messageType, @newMessage, @pageNumber, @pageSize",
                userIdParameter, portalStartDateParameter, isBothParameter, messageTypeParameter, newMessageParameter, pageNumberParameter, pageSizeParameter).ToList();
        }

        public virtual List<Livongo_Result> LivongoWeeklyHRAProcess()
        {
            return _interventDatabase.Livongo_Result.FromSqlRaw("LivongoWeeklyHRAProcess").ToList();
        }

        public virtual ParticipantProfile_Result ParticipantProfile(int? id, int? user)
        {
            SqlParameter idParameter = id.HasValue ? new SqlParameter("@Id", id) : new SqlParameter("@Id", DBNull.Value);
            SqlParameter userParameter = user.HasValue ? new SqlParameter("@User", user) : new SqlParameter("@User", DBNull.Value);

            return _interventDatabase.ParticipantProfile_Result.FromSqlRaw("ParticipantProfile @Id, @User", idParameter, userParameter).ToList().FirstOrDefault();
        }

        public virtual void SetCoachAvailability(SqlParameter avail, SqlParameter sqlCoachId, SqlParameter sqlRefId, SqlParameter createdBy, SqlParameter createdOn)
        {
            _interventDatabase.Database.ExecuteSqlRaw("SetCoachAvailability @RefId, @CoachId, @CreatedBy, @CreatedOn, @Avail", sqlRefId, sqlCoachId, createdBy, createdOn, avail);
        }

        public virtual void StratifyHRA(int? id)
        {
            SqlParameter idParameter = id.HasValue ? new SqlParameter("@Id", id) : new SqlParameter("@Id", DBNull.Value);

            _interventDatabase.Database.ExecuteSqlRaw("StratifyHRA @Id", idParameter);
        }

        public virtual string SetLabAlert(int? id)
        {
            SqlParameter idParameter = id.HasValue ? new SqlParameter("@Id", id) : new SqlParameter("@Id", DBNull.Value);

            var result = _interventDatabase.Database.ExecuteSqlRaw("SetLabAlert @Id", idParameter);
            return result.ToString();
        }

        public virtual List<GetTobaccoIncentive_Result> GetTobaccoIncentive(int? portalId)
        {
            SqlParameter portalIdParameter = portalId.HasValue ? new SqlParameter("@PortalId", portalId) : new SqlParameter("@PortalId", DBNull.Value);

            return _interventDatabase.GetTobaccoIncentive_Result.FromSqlRaw("GetTobaccoIncentive @PortalId", portalIdParameter).ToList();
        }

        public virtual List<IncentiveReportSP_Result> IncentiveReport(int? portalId)
        {
            SqlParameter portalIdParameter = portalId.HasValue ? new SqlParameter("@PortalId", portalId) : new SqlParameter("@PortalId", DBNull.Value);

            return _interventDatabase.IncentiveReportSP_Result.FromSqlRaw("IncentiveReport @PortalId", portalIdParameter).ToList();
        }

        public virtual ProcessIncentives_Result ProcessIncentives()
        {
            return _interventDatabase.ProcessIncentives_Result.FromSqlRaw("EXEC ProcessIncentives").Single();
        }

        public virtual int ProcessUserKeys(int portalId)
        {
            SqlParameter portalIdParameter = new SqlParameter("@PortalId", portalId);
            var result = _interventDatabase.Database.ExecuteSqlRaw("ProcessUserKeys @PortalId", portalIdParameter);
            return result;
        }

        public virtual List<CoachTrackingReport_Result> GetCoachTrackingReport(int? orgId, int? coachId, int? userId, int? pageNumber, int? pageSize, bool? total, bool? forPrint)
        {
            SqlParameter orgIdParameter = orgId.HasValue ? new SqlParameter("@OrganizationId", orgId) : new SqlParameter("@OrganizationId", DBNull.Value);
            SqlParameter coachIdParameter = coachId.HasValue ? new SqlParameter("@CoachId", coachId) : new SqlParameter("@CoachId", DBNull.Value);
            SqlParameter userIdParameter = userId.HasValue ? new SqlParameter("@UserId", userId) : new SqlParameter("@UserId", DBNull.Value);
            SqlParameter pageNumberParameter = pageNumber != null ? new SqlParameter("@PageNumber", pageNumber) : new SqlParameter("@PageNumber", DBNull.Value);
            SqlParameter pageSizeParameter = pageSize != null ? new SqlParameter("@PageSize", pageSize) : new SqlParameter("@PageSize", DBNull.Value);
            SqlParameter totalParameter = total != null ? new SqlParameter("@total", total) : new SqlParameter("@total", DBNull.Value);
            SqlParameter ForPrintParameter = forPrint != null ? new SqlParameter("@ForPrint", forPrint) : new SqlParameter("@ForPrint", DBNull.Value);
            return _interventDatabase.CoachTrackingReport_Result.FromSqlRaw("GetCoachTrackingReport @OrganizationId, @CoachId, @UserId, @ForPrint, @PageNumber, @PageSize, @total", orgIdParameter, coachIdParameter, userIdParameter, ForPrintParameter, pageNumberParameter, pageSizeParameter, totalParameter).ToList();
        }

        public List<ListSearchUsers_Result> ListAdvancedSearchUsers(string searchText, int? organizationId, DateTime? HraStartDate, DateTime? HraEndDate, int? coachId, string medicalCondition, byte? programType, string recentStats, int? contactRequirement, int? userId, int? pageNumber, int? pageSize)
        {
            SqlParameter searchTextParameter = searchText != null ? new SqlParameter("@SearchText", searchText) : new SqlParameter("@SearchText", DBNull.Value);
            SqlParameter organizationIdParameter = organizationId.HasValue ? new SqlParameter("@OrganizationId", organizationId) : new SqlParameter("@OrganizationId", DBNull.Value);
            SqlParameter hraStartDateParameter = HraStartDate.HasValue ? new SqlParameter("@HraStartDate", HraStartDate) : new SqlParameter("@HraStartDate", DBNull.Value);
            SqlParameter hraEndDateParameter = HraEndDate.HasValue ? new SqlParameter("@HraEndDate", HraEndDate) : new SqlParameter("@HraEndDate", DBNull.Value);
            SqlParameter coachIdParameter = coachId.HasValue ? new SqlParameter("@CoachId", coachId) : new SqlParameter("@CoachId", DBNull.Value);
            SqlParameter medicalConditionParameter = medicalCondition != null ? new SqlParameter("@MedicalCondition", medicalCondition) : new SqlParameter("@MedicalCondition", DBNull.Value);
            SqlParameter programTypeParameter = programType.HasValue ? new SqlParameter("@ProgramType", programType) : new SqlParameter("@ProgramType", DBNull.Value);
            SqlParameter recentStatsParameter = recentStats != null ? new SqlParameter("@RecentStats", recentStats) : new SqlParameter("@RecentStats", DBNull.Value);
            SqlParameter contactRequirementParameter = contactRequirement.HasValue ? new SqlParameter("@ContactRequirement", contactRequirement) : new SqlParameter("@ContactRequirement", DBNull.Value);
            SqlParameter userIdParameter = userId.HasValue ? new SqlParameter("@UserId", userId) : new SqlParameter("@UserId", DBNull.Value);
            SqlParameter pageNumberParameter = pageNumber.HasValue ? new SqlParameter("@PageNumber", pageNumber) : new SqlParameter("@PageNumber", DBNull.Value);
            SqlParameter pageSizeParameter = pageSize.HasValue ? new SqlParameter("@PageSize", pageSize) : new SqlParameter("@PageSize", DBNull.Value);

            return _interventDatabase.ListSearchUsers_Result.FromSqlRaw("ListSearchUsers @SearchText, @OrganizationId, @HraStartDate, @HraEndDate, @CoachId, @MedicalCondition, @ProgramType, @RecentStats, @ContactRequirement, @UserId, @PageNumber, @PageSize", searchTextParameter, organizationIdParameter, hraStartDateParameter, hraEndDateParameter, coachIdParameter, medicalConditionParameter, programTypeParameter, recentStatsParameter, contactRequirementParameter, userIdParameter, pageNumberParameter, pageSizeParameter).ToList();
        }

        public virtual List<ListEligibilityResult> ListEligibility(int UserId, string FirstName, string LastName, int? PortalId, string UniqueId, string Email, string Telephone, string Language, DateTime? EligibilityStartDate, DateTime? EligibilityEndDate, DateTime? ClaimStartDate, DateTime? ClaimEndDate, byte? enrollmentStatus, string ClaimDiagnosisCode, bool? canrisk, bool? coachingEnabled, int? pageNumber, int? pageSize, bool? total)
        {
            SqlParameter userId = new SqlParameter("@UserId", UserId);
            SqlParameter firstName = !string.IsNullOrEmpty(FirstName) ? new SqlParameter("@FirstName", FirstName) : new SqlParameter("@FirstName", DBNull.Value);
            SqlParameter lastName = !string.IsNullOrEmpty(LastName) ? new SqlParameter("@LastName", LastName) : new SqlParameter("@LastName", DBNull.Value);
            SqlParameter portalId = PortalId.HasValue ? new SqlParameter("@PortalId", PortalId) : new SqlParameter("@PortalId", DBNull.Value);
            SqlParameter uniqueId = !string.IsNullOrEmpty(UniqueId) ? new SqlParameter("@UniqueId", UniqueId) : new SqlParameter("@UniqueId", DBNull.Value);
            SqlParameter email = !string.IsNullOrEmpty(Email) ? new SqlParameter("@Email", Email) : new SqlParameter("@Email", DBNull.Value);
            SqlParameter telephone = !string.IsNullOrEmpty(Telephone) ? new SqlParameter("@Telephone", Telephone) : new SqlParameter("@Telephone", DBNull.Value);
            SqlParameter language = !string.IsNullOrEmpty(Language) ? new SqlParameter("@Language", Language) : new SqlParameter("@Language", DBNull.Value);
            SqlParameter eligibilityStartDate = EligibilityStartDate != null ? new SqlParameter("@EligibilityStartDate", EligibilityStartDate) : new SqlParameter("@EligibilityStartDate", DBNull.Value);
            SqlParameter eligibilityEndDate = EligibilityEndDate != null ? new SqlParameter("@EligibilityEndDate", EligibilityEndDate) : new SqlParameter("@EligibilityEndDate", DBNull.Value);
            SqlParameter claimStartDate = ClaimStartDate != null ? new SqlParameter("@ClaimStartDate", ClaimStartDate) : new SqlParameter("@ClaimStartDate", DBNull.Value);
            SqlParameter claimEndDate = ClaimEndDate != null ? new SqlParameter("@ClaimEndDate", ClaimEndDate) : new SqlParameter("@ClaimEndDate", DBNull.Value);
            SqlParameter EnrollmentStatus = enrollmentStatus != null ? new SqlParameter("@EnrollmentStatus", enrollmentStatus) : new SqlParameter("@EnrollmentStatus", DBNull.Value);
            SqlParameter claimDiagnosisCode = !string.IsNullOrEmpty(ClaimDiagnosisCode) ? new SqlParameter("@ClaimDiagnosisCode", ClaimDiagnosisCode) : new SqlParameter("@ClaimDiagnosisCode", DBNull.Value);
            SqlParameter pageNumberParameter = pageNumber != null ? new SqlParameter("@PageNumber", pageNumber) : new SqlParameter("@PageNumber", DBNull.Value);
            SqlParameter pageSizeParameter = pageSize != null ? new SqlParameter("@PageSize", pageSize) : new SqlParameter("@PageSize", DBNull.Value);
            SqlParameter totalParameter = total != null ? new SqlParameter("@total", total) : new SqlParameter("@total", DBNull.Value);
            SqlParameter canriskParameter = canrisk != null ? new SqlParameter("@canrisk", canrisk) : new SqlParameter("@canrisk", DBNull.Value);
            SqlParameter coachingenabledParameter = coachingEnabled != null ? new SqlParameter("@CoachingEnabled", coachingEnabled) : new SqlParameter("@CoachingEnabled", DBNull.Value);

            return _interventDatabase.ListEligibilityResult.FromSqlRaw("ListEligibility @UserId, @FirstName, @LastName, @PortalId, @UniqueId, @Email, @Telephone, @Language, @EligibilityStartDate, @EligibilityEndDate, @ClaimStartDate, @ClaimEndDate, @EnrollmentStatus, @ClaimDiagnosisCode, @canrisk, @CoachingEnabled, @PageNumber, @PageSize, @total", userId, firstName, lastName, portalId, uniqueId, email, telephone, language, eligibilityStartDate, eligibilityEndDate, claimStartDate, claimEndDate, EnrollmentStatus, claimDiagnosisCode, canriskParameter, coachingenabledParameter, pageNumberParameter, pageSizeParameter, totalParameter).ToList();
        }
    }
}
