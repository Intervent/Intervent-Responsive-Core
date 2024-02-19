using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Intervent.DAL
{
    public partial class InterventDatabase : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public InterventDatabase(DbContextOptions<InterventDatabase> options) : base(options) { }

        public static DbContextOptions<InterventDatabase> GetInterventDatabaseOption()
        {
			IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			return new DbContextOptionsBuilder<InterventDatabase>().UseSqlServer(configuration.GetConnectionString("DefaultConnection")).Options;
        }

        public virtual DbSet<IncentiveReportSP_Result> IncentiveReportSP_Result { get; set; }
        public virtual DbSet<LegacyAppointmentRemainderResult> LegacyAppointmentRemainderResult { get; set; }
        public virtual DbSet<GetCoachAvailability_Result> GetCoachAvailability_Result { get; set; }
        public virtual DbSet<GetMessages_Result> GetMessages_Result { get; set; }
        public virtual DbSet<GetCoachList_Result> GetCoachList_Result { get; set; }
        public virtual DbSet<FilteredCoachListResult> FilteredCoachListResult { get; set; }
        public virtual DbSet<GetDataforOutreach_Result> GetDataforOutreach_Result { get; set; }
        public virtual DbSet<GetDataforTracking_Result> GetDataforTracking_Result { get; set; }
        public virtual DbSet<ListUsers_Result> ListUsers_Result { get; set; }
        public virtual DbSet<ListEligibilityResult> ListEligibilityResult { get; set; }
        public virtual DbSet<ListSearchUsers_Result> ListSearchUsers_Result { get; set; }
        public virtual DbSet<CoachTrackingReport_Result> CoachTrackingReport_Result { get; set; }
        public virtual DbSet<ProcessIncentives_Result> ProcessIncentives_Result { get; set; }
        public virtual DbSet<GetTobaccoIncentive_Result> GetTobaccoIncentive_Result { get; set; }
        public virtual DbSet<Livongo_Result> Livongo_Result { get; set; }
        public virtual DbSet<ParticipantProfile_Result> ParticipantProfile_Result { get; set; }
        public virtual DbSet<GetDashboadMessages_Result> GetDashboadMessages_Result { get; set; }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<ActionStepType> ActionStepTypes { get; set; }
        public virtual DbSet<AccessTokens> AccessTokens { get; set; }
        public virtual DbSet<ActivitiesinStep> ActivitiesinSteps { get; set; }
        public virtual DbSet<AdminModule> AdminModules { get; set; }
        public virtual DbSet<AdminProperty> AdminProperties { get; set; }
        public virtual DbSet<AdminTask> AdminTasks { get; set; }
        public virtual DbSet<AppointmentFeedback> AppointmentFeedbacks { get; set; }
        public virtual DbSet<AppointmentType> AppointmentTypes { get; set; }
        public virtual DbSet<FollowUpType> FollowUpTypes { get; set; }
        public virtual DbSet<ApptCallInterval> ApptCallInterval { get; set; }
        public virtual DbSet<ApptCallTemplate> ApptCallTemplate { get; set; }
        public virtual DbSet<AssignedRecipe> AssignedRecipes { get; set; }
        public virtual DbSet<Availability> Availabilities { get; set; }
        public virtual DbSet<AWV> AWVs { get; set; }
        public virtual DbSet<AWV_AidtoQuitTobacco> AWV_AidtoQuitTobacco { get; set; }
        public virtual DbSet<AWV_AlcoholUse> AWV_AlcoholUse { get; set; }
        public virtual DbSet<AWV_Allergies> AWV_Allergies { get; set; }
        public virtual DbSet<AWV_Biometrics> AWV_Biometrics { get; set; }
        public virtual DbSet<AWV_DepressionScreens> AWV_DepressionScreens { get; set; }
        public virtual DbSet<AWV_General> AWV_General { get; set; }
        public virtual DbSet<AWV_Goals> AWV_Goals { get; set; }
        public virtual DbSet<AWV_HomeScreens> AWV_HomeScreens { get; set; }
        public virtual DbSet<AWV_HospitalVisits> AWV_HospitalVisits { get; set; }
        public virtual DbSet<AWV_MedicalHistandProviders> AWV_MedicalHistandProviders { get; set; }
        public virtual DbSet<AWV_Medications> AWV_Medications { get; set; }
        public virtual DbSet<AWV_OsteoporosisScreens> AWV_OsteoporosisScreens { get; set; }
        public virtual DbSet<AWV_PreventiveServices> AWV_PreventiveServices { get; set; }
        public virtual DbSet<AWV_PreventiveServicesType> AWV_PreventiveServicesType { get; set; }
        public virtual DbSet<AWV_Services> AWV_Services { get; set; }
        public virtual DbSet<AWV_STDandProstateRisk> AWV_STDandProstateRisk { get; set; }
        public virtual DbSet<AWV_TobaccoAid> AWV_TobaccoAid { get; set; }
        public virtual DbSet<AWV_TobaccoUse> AWV_TobaccoUse { get; set; }
        public virtual DbSet<AWV_UrinaryScreens> AWV_UrinaryScreens { get; set; }
        public virtual DbSet<CancellationReason> CancellationReasons { get; set; }
        public virtual DbSet<CandidateCondition> CandidateConditions { get; set; }
        public virtual DbSet<CandidateMedication> CandidateMedications { get; set; }
        public virtual DbSet<CandidateReasonForLastChange> CandidateReasonForLastChanges { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CouponCode> CouponCodes { get; set; }
        public virtual DbSet<CustomIncentiveType> CustomIncentiveTypes { get; set; }
        public virtual DbSet<DashboardMessageType> DashboardMessageTypes { get; set; }
        public virtual DbSet<DeclinedEnrollmentReason> DeclinedEnrollmentReasons { get; set; }
        public virtual DbSet<Drug_Allergy> Drug_Allergy { get; set; }
        public virtual DbSet<Drug_Condition> Drug_Condition { get; set; }
        public virtual DbSet<Drug_Duration> Drug_Duration { get; set; }
        public virtual DbSet<Drug_Formulation> Drug_Formulation { get; set; }
        public virtual DbSet<Drug_Frequency> Drug_Frequency { get; set; }
        public virtual DbSet<Drug_Ingredients> Drug_Ingredients { get; set; }
        public virtual DbSet<Drug_Products> Drug_Products { get; set; }
        public virtual DbSet<Drug_Products_FDA> Drug_Products_FDA { get; set; }
        public virtual DbSet<Drug_ProductsForm> Drug_ProductsForm { get; set; }
        public virtual DbSet<Eligibility> Eligibilities { get; set; }
        public virtual DbSet<EligibilityImportLog> EligibilityImportLogs { get; set; }
        public virtual DbSet<IntuityEligibility> IntuityEligibilities { get; set; }
        public virtual DbSet<IntuityEligibilityLog> IntuityEligibilityLogs { get; set; }
        public virtual DbSet<IntuityFulfillments> IntuityFulfillments { get; set; }
        public virtual DbSet<IntuityFulfillmentRequests> IntuityFulfillmentRequests { get; set; }
        public virtual DbSet<IntuityQOH> IntuityQOHs { get; set; }
        public virtual DbSet<IntuityEPData> IntuityEPData { get; set; }
        public virtual DbSet<EligibilityNote> EligibilityNotes { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<ExerciseDiary> ExerciseDiaries { get; set; }
        public virtual DbSet<WorkoutGoals> Workout_Goals { get; set; }
        public virtual DbSet<ExerciseType> ExerciseTypes { get; set; }
        public virtual DbSet<EXT_Glucose> EXT_Glucose { get; set; }
        public virtual DbSet<Ext_GlucoseSetting> Ext_GlucoseSetting { get; set; }
        public virtual DbSet<Ext_GlucoseTags> Ext_GlucoseTags { get; set; }
        public virtual DbSet<ExternalUser> ExternalUsers { get; set; }
        public virtual DbSet<FaxedReport> FaxedReports { get; set; }
        public virtual DbSet<FollowUp> FollowUps { get; set; }
        public virtual DbSet<FollowUp_Goals> FollowUp_Goals { get; set; }
        public virtual DbSet<FollowUp_HealthConditions> FollowUp_HealthConditions { get; set; }
        public virtual DbSet<FollowUp_HealthNumbers> FollowUp_HealthNumbers { get; set; }
        public virtual DbSet<FollowUp_MedicalConditions> FollowUp_MedicalConditions { get; set; }
        public virtual DbSet<FollowUp_OtherRiskFactors> FollowUp_OtherRiskFactors { get; set; }
        public virtual DbSet<FoodDiary> FoodDiaries { get; set; }
        public virtual DbSet<FoodGroup> FoodGroups { get; set; }
        public virtual DbSet<GlucometerUser> GlucometerUsers { get; set; }
        public virtual DbSet<HRA_ActionSteps> HRA_ActionSteps { get; set; }
        public virtual DbSet<HRA_ExamsandShots> HRA_ExamsandShots { get; set; }
        public virtual DbSet<HRA_Goals> HRA_Goals { get; set; }
        public virtual DbSet<HRA_HealthNumbers> HRA_HealthNumbers { get; set; }
        public virtual DbSet<HRA_HSP> HRA_HSP { get; set; }
        public virtual DbSet<HRA_Interests> HRA_Interests { get; set; }
        public virtual DbSet<HRA_MedicalConditions> HRA_MedicalConditions { get; set; }
        public virtual DbSet<HRA_OtherRiskFactors> HRA_OtherRiskFactors { get; set; }
        public virtual DbSet<HRA> HRAs { get; set; }
        public virtual DbSet<IncentiveType> IncentiveTypes { get; set; }
        public virtual DbSet<InsuranceSummary> InsuranceSummaries { get; set; }
        public virtual DbSet<ClaimConditionCode> ClaimConditionCodes { get; set; }
        public virtual DbSet<Kit> Kits { get; set; }
        public virtual DbSet<KitsinProgram> KitsinPrograms { get; set; }
        public virtual DbSet<KitTranslation> KitTranslations { get; set; }
        public virtual DbSet<KitsinUserProgram> KitsinUserPrograms { get; set; }
        public virtual DbSet<KitsinUserProgramGoal> KitsinUserProgramGoals { get; set; }
        public virtual DbSet<KitTopic> KitTopics { get; set; }
        public virtual DbSet<LabData> LabDatas { get; set; }
        public virtual DbSet<LabErrorLog> LabErrorLogs { get; set; }
        public virtual DbSet<LabReferenceRanx> LabReferenceRanges { get; set; }
        public virtual DbSet<Lab> Labs { get; set; }
        public virtual DbSet<DiagnosticLab> DiagnosticLabs { get; set; }
        public virtual DbSet<LanguageItems> LanguageItems { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Mail> Mails { get; set; }
        public virtual DbSet<MealType> MealTypes { get; set; }
        public virtual DbSet<Measurement> Measurements { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<NoteType> NoteTypes { get; set; }
        public virtual DbSet<ReferralTypes> ReferralTypes { get; set; }
        public virtual DbSet<NotificationCategory> NotificationCategories { get; set; }
        public virtual DbSet<NotificationContactMethod> NotificationContactMethods { get; set; }
        public virtual DbSet<NotificationEvent> NotificationEvents { get; set; }
        public virtual DbSet<NotificationEventType> NotificationEventTypes { get; set; }
        public virtual DbSet<NotificationMessage> NotificationMessages { get; set; }
        public virtual DbSet<NotificationStatu> NotificationStatus { get; set; }
        public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public virtual DbSet<NotificationTemplateRenderer> NotificationTemplateRenderers { get; set; }
        public virtual DbSet<OptionsforActivityQuestion> OptionsforActivityQuestions { get; set; }
        public virtual DbSet<OptionsforQuiz> OptionsforQuizs { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OutreachLog> OutreachLogs { get; set; }
        public virtual DbSet<PassiveQuestionsInActivity> PassiveQuestionsInActivities { get; set; }
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public virtual DbSet<PortalIncentive> PortalIncentives { get; set; }
        public virtual DbSet<Portal> Portals { get; set; }
        public virtual DbSet<PortalFollowUp> PortalFollowUps { get; set; }
        public virtual DbSet<Profession> Professions { get; set; }
        public virtual DbSet<ProgramInactiveReason> ProgramInactiveReasons { get; set; }
        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<ProgramsinPortal> ProgramsinPortals { get; set; }
        public virtual DbSet<PromptsInKit> PromptsInKits { get; set; }
        public virtual DbSet<PromptsinKitsCompleted> PromptsinKitsCompleteds { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionsinActivity> QuestionsinActivities { get; set; }
        public virtual DbSet<QuizinKit> QuizinKits { get; set; }
        public virtual DbSet<QuizinStep> QuizinSteps { get; set; }
        public virtual DbSet<Race> Races { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeTranslation> RecipeTranslations { get; set; }
        public virtual DbSet<RecipeTag> RecipeTags { get; set; }
        public virtual DbSet<ScreeningDataErrorLog> ScreeningDataErrorLogs { get; set; }
        public virtual DbSet<SleepLog> SleepLogs { get; set; }
        public virtual DbSet<Specialization> Specializations { get; set; }
        public virtual DbSet<SSOAttributeMapping> SSOAttributeMappings { get; set; }
        public virtual DbSet<SSOProvider> SSOProviders { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<StepsinKit> StepsinKits { get; set; }
        public virtual DbSet<StrengthTrainingLog> StrengthTrainingLogs { get; set; }
        public virtual DbSet<StrengthTrainingSet> StrengthTrainingSets { get; set; }
        public virtual DbSet<StrengthTrainingType> StrengthTrainingTypes { get; set; }
        public virtual DbSet<StressDiary> StressDiaries { get; set; }
        public virtual DbSet<StressManagementLog> StressManagementLogs { get; set; }
        public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public virtual DbSet<SurveyResponse> SurveyResponses { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<TaskType> TaskTypes { get; set; }
        public virtual DbSet<Testimonial> Testimonials { get; set; }
        public virtual DbSet<TimeZone> TimeZones { get; set; }
        public virtual DbSet<TobaccoLog> TobaccoLogs { get; set; }
        public virtual DbSet<UserChoice> UserChoices { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserDashboardMessage> UserDashboardMessages { get; set; }
        public virtual DbSet<UserDoctorInfo> UserDoctorInfoes { get; set; }
        public virtual DbSet<UserDrugAllergy> UserDrugAllergies { get; set; }
        public virtual DbSet<UserDrug> UserDrugs { get; set; }
        public virtual DbSet<UserEligibilitySetting> UserEligibilitySettings { get; set; }
        public virtual DbSet<UserForm> UserForms { get; set; }
        public virtual DbSet<UserHistory> UserHistories { get; set; }
        public virtual DbSet<UserHistoryCategory> UserHistoryCategories { get; set; }
        public virtual DbSet<UserInactiveReason> UserInactiveReasons { get; set; }
        public virtual DbSet<UserIncentive> UserIncentives { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserQuizChoice> UserQuizChoices { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersinProgram> UsersinPrograms { get; set; }
        public virtual DbSet<UserTrackingStatus> UserTrackingStatuses { get; set; }
        public virtual DbSet<VitalsLog> VitalsLogs { get; set; }
        public virtual DbSet<WeightLossJournal> WeightLossJournals { get; set; }
        public virtual DbSet<WellnessData> WellnessDatas { get; set; }
        public virtual DbSet<MedicalPlanCode> MedicalPlanCodes { get; set; }
        public virtual DbSet<ClaimLoadExcludeIds> ClaimLoadExcludeId { get; set; }
        public virtual DbSet<CrothalIDChanges> CrothalIDChange { get; set; }
        public virtual DbSet<ClaimCode> ClaimCodes { get; set; }
        public virtual DbSet<TherapeuticClassCode> TherapeuticClassCodes { get; set; }
        public virtual DbSet<LivongoICDCodes> LivongoICDCodess { get; set; }
        public virtual DbSet<LivongoNDCCodes> LivongoNDCCodes { get; set; }
        public virtual DbSet<RaffleTypes> RaffleTypes { get; set; }
        public virtual DbSet<RafflesinPortals> RafflesinPortals { get; set; }
        public virtual DbSet<UserRaffles> UserRaffles { get; set; }
        public virtual DbSet<UserKeys> UserKeys { get; set; }
        public virtual DbSet<IntuityUsers> IntuityUsers { get; set; }
        public virtual DbSet<EXT_Summaries> EXT_Summaries { get; set; }
        public virtual DbSet<EXT_Weights> EXT_Weights { get; set; }
        public virtual DbSet<EXT_Workouts> EXT_Workouts { get; set; }
        public virtual DbSet<EXT_Sleeps> EXT_Sleeps { get; set; }
        public virtual DbSet<EXT_BloodPressures> EXT_BloodPressures { get; set; }
        public virtual DbSet<EXT_Nutrition> EXT_Nutrition { get; set; }
        public virtual DbSet<RaffleDates> RaffleDates { get; set; }
        public virtual DbSet<CanriskQuestionnaire> CanriskQuestionnaires { get; set; }
        public virtual DbSet<CanriskPostalCode> CanriskPostalCodes { get; set; }
        public virtual DbSet<CanriskTracking> CanriskTrackings { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<Newsletter> Newsletter { get; set; }
        public virtual DbSet<AssignedNewsletter> AssignedNewsletters { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageRecipient> MessageRecipients { get; set; }
        public virtual DbSet<FavoriteContact> FavoriteContacts { get; set; }
        public virtual DbSet<NotificationTemplateTranslation> NotificationTemplateTranslations { get; set; }
        public virtual DbSet<HRA_ExercisePlan> HRA_ExercisePlans { get; set; }
        public virtual DbSet<KitsinPortalFollowUp> KitsinPortalFollowUps { get; set; }
        public virtual DbSet<CRM_AccountType> CRM_AccountTypes { get; set; }
        public virtual DbSet<CRM_CallerProfileType> CRM_CallerProfileTypes { get; set; }
        public virtual DbSet<CRM_ChangeLog> CRM_ChangeLogs { get; set; }
        public virtual DbSet<CRM_ComplaintClassificationType> CRM_ComplaintClassificationTypes { get; set; }
        public virtual DbSet<CRM_Contact> CRM_Contacts { get; set; }
        public virtual DbSet<CRM_Note> CRM_Notes { get; set; }
        public virtual DbSet<CRM_Disposition> CRM_Dispositions { get; set; }
        public virtual DbSet<CRM_DispositionCategories> CRM_DispositionCategories { get; set; }
        public virtual DbSet<CRM_PogoMeterNumbers> CRM_PogoMeterNumbers { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<IntuityEventType> IntuityEventTypes { get; set; }
        public virtual DbSet<IntuityEvent> IntuityEvents { get; set; }
        public virtual DbSet<OrganizationsforWebinar> OrganizationsforWebinars { get; set; }
        public virtual DbSet<RegisteredUsersforWebinar> RegisteredUsersforWebinars { get; set; }
        public virtual DbSet<Webinar> Webinars { get; set; }
        public virtual DbSet<HCPList> HCPLists { get; set; }
        public virtual DbSet<HCPSpecialty> HCPSpecialties { get; set; }
        public virtual DbSet<InsuranceType> InsuranceTypes { get; set; }
        public virtual DbSet<PortalCoachingConditions> PortalCoachingConditions { get; set; }
        public virtual DbSet<CoachingConditions> CoachingConditions { get; set; }
        public virtual DbSet<ReportFeedback> ReportFeedbacks { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<CarePlanReport> CarePlanReports { get; set; }
        public virtual DbSet<LabProcedure> LabProcedures { get; set; }
        public virtual DbSet<PortalLabProcedure> PortalLabProcedures { get; set; }
        public virtual DbSet<IVICDCode> IVICDCodes { get; set; }
        public virtual DbSet<IVNDCCode> IVNDCCodes { get; set; }
        public virtual DbSet<UserLoggedInDevice> UserLoggedInDevices { get; set; }
        public virtual DbSet<ContactRequirement> ContactRequirements { get; set; }
        public virtual DbSet<ContactRequirementsAlert> ContactRequirementsAlert { get; set; }
        public virtual DbSet<InquiryType> InquiryTypes { get; set; }
        public virtual DbSet<BillingServiceType> BillingServiceTypes { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<InvoiceBilledDetail> InvoiceBilledDetails { get; set; }
        public virtual DbSet<UserTimeTracker> UserTimeTracker { get; set; }
        public virtual DbSet<TimeTrackerDisposition> TimeTrackerDispositions { get; set; }
        public virtual DbSet<ExternalReport> ExternalReports { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<BillingNote> BillingNotes { get; set; }
        public virtual DbSet<TeamsBP_PPR> TeamsBP_PPR { get; set; }
        public virtual DbSet<SecurityCode> SecurityCodes { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<QADOrders> QADOrders { get; set; }
        public virtual DbSet<HealthData> HealthDatas { get; set; }
        public virtual DbSet<AssignedMotivationMessage> AssignedMotivationMessages { get; set; }
        public virtual DbSet<MotivationMessage> MotivationMessages { get; set; }
        public virtual DbSet<WebinarOccurrence> WebinarOccurrences { get; set; }
        public virtual DbSet<UserWearableDevice> UserWearableDevices { get; set; }
        public virtual DbSet<WearableDevice> WearableDevices { get; set; }
        public virtual DbSet<PreviousPassword> PreviousPasswords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IncentiveReportSP_Result>().HasNoKey();
            modelBuilder.Entity<FilteredCoachListResult>().HasNoKey();
            modelBuilder.Entity<GetDataforOutreach_Result>().HasNoKey();
            modelBuilder.Entity<GetDataforTracking_Result>().HasNoKey();
            modelBuilder.Entity<GetMessages_Result>().HasNoKey();
            modelBuilder.Entity<LegacyAppointmentRemainderResult>().HasNoKey();
            modelBuilder.Entity<GetCoachAvailability_Result>().HasNoKey();
            modelBuilder.Entity<GetCoachList_Result>().HasNoKey();
            modelBuilder.Entity<ListUsers_Result>().HasNoKey();
            modelBuilder.Entity<ListEligibilityResult>().HasNoKey();
            modelBuilder.Entity<ListSearchUsers_Result>().HasNoKey();
            modelBuilder.Entity<CoachTrackingReport_Result>().HasNoKey();
            modelBuilder.Entity<ProcessIncentives_Result>().HasNoKey();
            modelBuilder.Entity<GetTobaccoIncentive_Result>().HasNoKey();
            modelBuilder.Entity<Livongo_Result>().HasNoKey();
            modelBuilder.Entity<ParticipantProfile_Result>().HasNoKey();
            modelBuilder.Entity<GetDashboadMessages_Result>().HasNoKey();

            modelBuilder.Entity<ApplicationUser>().ToTable("Users", "dbo").Property(p => p.Id);
            modelBuilder.Entity<IdentityRole<int>>().ToTable("UserRoles", "dbo").Property(p => p.Id);
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserUserRoles", "dbo").Property(p => p.UserId);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Organization)
                .WithMany()
                .HasForeignKey(u => u.OrganizationId);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.UserDoctorInfoes)
                .WithOne(e => e.appUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(e => e.AdminProperty)
                .WithOne(e => e.appUser)
                .HasForeignKey<AdminProperty>(c => c.Id);

            modelBuilder.Entity<ActionStepType>()
                .Property(e => e.RiskFactor)
                .IsUnicode(false);

            modelBuilder.Entity<ActionStepType>()
                .Property(e => e.Statement)
                .IsUnicode(false);

            modelBuilder.Entity<ActionStepType>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<ActionStepType>()
                .Property(e => e.StatementIfNull)
                .IsUnicode(false);

            modelBuilder.Entity<ActionStepType>()
                .Property(e => e.HelpStatement)
                .IsUnicode(false);

            modelBuilder.Entity<ActionStepType>()
                .HasMany(e => e.HRA_ActionSteps)
                .WithOne(e => e.ActionStepType)
                .HasForeignKey(e => e.Type)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivitiesinStep>()
                .Property(e => e.TopText)
                .IsUnicode(false);

            modelBuilder.Entity<ActivitiesinStep>()
                .Property(e => e.BottomText)
                .IsUnicode(false);

            modelBuilder.Entity<ActivitiesinStep>()
                .Property(e => e.BottomTextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<ActivitiesinStep>()
                .Property(e => e.TopTextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<ActivitiesinStep>()
                .HasMany(e => e.PassiveQuestionsInActivities)
                .WithOne(e => e.ActivitiesinStep)
                .HasForeignKey(e => e.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivitiesinStep>()
                .HasMany(e => e.QuestionsinActivities)
                .WithOne(e => e.ActivitiesinStep)
                .HasForeignKey(e => e.ActivityId);

            modelBuilder.Entity<AdminProperty>()
                .Property(e => e.Profile)
                .IsUnicode(false);

            modelBuilder.Entity<AdminTask>()
                .Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false);

            modelBuilder.Entity<AdminTask>()
                .Property(e => e.Comment)
                .IsUnicode(false);

            modelBuilder.Entity<Appointment>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<Appointment>()
                .HasOne(e => e.AppointmentFeedback)
                .WithOne(e => e.Appointment);

            modelBuilder.Entity<AppointmentType>()
                .HasMany(e => e.Appointments)
                .WithOne(e => e.AppointmentType)
                .HasForeignKey(e => e.Type);

            modelBuilder.Entity<ApptCallTemplate>()
                .Property(e => e.TemplateName)
                .IsUnicode(false);

            modelBuilder.Entity<ApptCallTemplate>()
                .HasMany(e => e.ApptCallIntervals)
                .WithOne(e => e.ApptCallTemplate)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AWV>()
                .Property(e => e.ConductedBy)
                .IsUnicode(false);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_AlcoholUse)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_AlcoholUse>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_Allergies)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_Allergies>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_General)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_General>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_HospitalVisits)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_HospitalVisits>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_Medications)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_Medications>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_OsteoporosisScreens)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_OsteoporosisScreens>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_Services)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_Services>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_STDandProstateRisk)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_STDandProstateRisk>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_Biometrics)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_Biometrics>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_DepressionScreens)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_DepressionScreens>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_Goals)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_Goals>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_HomeScreens)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_HomeScreens>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_MedicalHistandProviders)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_MedicalHistandProviders>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasMany(e => e.AWV_PreventiveServices)
                .WithOne(e => e.AWV)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AWV>()
                .HasMany(e => e.AWV_TobaccoAid)
                .WithOne(e => e.AWV)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_TobaccoUse)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_TobaccoUse>(c => c.Id);

            modelBuilder.Entity<AWV>()
                .HasOne(e => e.AWV_UrinaryScreens)
                .WithOne(e => e.AWV)
                .HasForeignKey<AWV_UrinaryScreens>(c => c.Id);

            modelBuilder.Entity<AWV_AidtoQuitTobacco>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_TobaccoAid>()
                .HasKey(e => new { e.Id, e.AidId, e.TypeId });

            modelBuilder.Entity<AWV_AidtoQuitTobacco>()
                .HasMany(e => e.AWV_TobaccoAid)
                .WithOne(e => e.AWV_AidtoQuitTobacco)
                .HasForeignKey(e => e.AidId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AWV_Allergies>()
                .Property(e => e.DrugName)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_Allergies>()
                .Property(e => e.Allergy)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_General>()
                .Property(e => e.GoToHelp)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_General>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_HospitalVisits>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_HospitalVisits>()
                .Property(e => e.Facility)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_HospitalVisits>()
                .Property(e => e.AttendingPhysician)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_HospitalVisits>()
                .Property(e => e.Surgeries)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_MedicalHistandProviders>()
                .Property(e => e.OtherChronProbandSurg)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_MedicalHistandProviders>()
                .Property(e => e.Comments)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_Medications>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_Medications>()
                .Property(e => e.Dosage)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_Medications>()
                .Property(e => e.Frequency)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_PreventiveServices>()
                .HasKey(e => new { e.Id, e.Type });

            modelBuilder.Entity<AWV_PreventiveServicesType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_PreventiveServicesType>()
                .HasMany(e => e.AWV_PreventiveServices)
                .WithOne(e => e.AWV_PreventiveServicesType)
                .HasForeignKey(e => e.Type)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AWV_Services>()
                .Property(e => e.ProviderName)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_Services>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_Services>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_TobaccoUse>()
                .Property(e => e.HelpedOthers)
                .IsUnicode(false);

            modelBuilder.Entity<AWV_TobaccoUse>()
                .Property(e => e.FailedOthers)
                .IsUnicode(false);

            modelBuilder.Entity<CancellationReason>()
                .HasMany(e => e.Appointments)
                .WithOne(e => e.CancellationReason)
                .HasForeignKey(e => e.InActiveReason);

            modelBuilder.Entity<CandidateCondition>()
                .Property(e => e.ConditionName)
                .IsUnicode(false);

            modelBuilder.Entity<CandidateCondition>()
                .Property(e => e.ConditionType)
                .IsUnicode(false);

            modelBuilder.Entity<CandidateCondition>()
                .Property(e => e.BilledAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateCondition>()
                .Property(e => e.Copay)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateCondition>()
                .Property(e => e.Deductible)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateCondition>()
                .Property(e => e.Coinsurance)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateCondition>()
                .Property(e => e.NetPaid)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateMedication>()
                .Property(e => e.MedicationName)
                .IsUnicode(false);

            modelBuilder.Entity<CandidateMedication>()
                .Property(e => e.MedicationType)
                .IsUnicode(false);

            modelBuilder.Entity<CandidateMedication>()
                .Property(e => e.Total_Amount_Paid_by_All_Source)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateMedication>()
                .Property(e => e.Patient_Pay_Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateMedication>()
                .Property(e => e.Amount_of_Copay)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateMedication>()
                .Property(e => e.Amount_of_Coinsurance)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateMedication>()
                .Property(e => e.Net_Amount_Due__Total_Amount_Billed_Paid_)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CandidateReasonForLastChange>()
                .Property(e => e.ConditionType)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.DateFormat)
                .IsUnicode(false);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.UserDoctorInfoes)
                .WithOne(e => e.Country1)
                .HasForeignKey(e => e.Country);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.Users)
                .WithOne(e => e.Country1)
                .HasForeignKey(e => e.Country);

            modelBuilder.Entity<DashboardMessageType>()
                .Property(e => e.MessageTemplate)
                .IsUnicode(false);

            modelBuilder.Entity<DashboardMessageType>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<DashboardMessageType>()
                .Property(e => e.Image)
                .IsUnicode(false);

            modelBuilder.Entity<DashboardMessageType>()
                .Property(e => e.Alt)
                .IsUnicode(false);

            modelBuilder.Entity<DashboardMessageType>()
                .Property(e => e.Url)
                .IsUnicode(false);

            modelBuilder.Entity<DashboardMessageType>()
                .Property(e => e.LanguageItem)
                .IsUnicode(false);

            modelBuilder.Entity<DashboardMessageType>()
                .HasMany(e => e.UserDashboardMessages)
                .WithOne(e => e.DashboardMessageType)
                .HasForeignKey(e => e.MessageType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Allergy>()
                .HasMany(e => e.UserDrugAllergies)
                .WithOne(e => e.Drug_Allergy)
                .HasForeignKey(e => e.AllergyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Condition>()
                .HasMany(e => e.UserDrugs)
                .WithOne(e => e.Drug_Condition)
                .HasForeignKey(e => e.Condition)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Duration>()
                .HasMany(e => e.UserDrugs)
                .WithOne(e => e.Drug_Duration)
                .HasForeignKey(e => e.Duration)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Formulation>()
                .HasMany(e => e.UserDrugs)
                .WithOne(e => e.Drug_Formulation)
                .HasForeignKey(e => e.Formulation)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Frequency>()
                .HasMany(e => e.UserDrugs)
                .WithOne(e => e.Drug_Frequency)
                .HasForeignKey(e => e.Frequency)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.PRODUCT_CATEGORIZATION)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.CLASS)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.DRUG_IDENTIFICATION_NUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.BRAND_NAME)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.DESCRIPTOR)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.PEDIATRIC_FLAG)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.ACCESSION_NUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.NUMBER_OF_AIS)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .Property(e => e.AI_GROUP_NO)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products>()
                .HasMany(e => e.Drug_Ingredients)
                .WithOne(e => e.Drug_Products)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Products>()
                .HasMany(e => e.Drug_ProductsForm)
                .WithOne(e => e.Drug_Products)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Drug_Products_FDA>()
                .Property(e => e.Form)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products_FDA>()
                .Property(e => e.Strength)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products_FDA>()
                .Property(e => e.DrugName)
                .IsUnicode(false);

            modelBuilder.Entity<Drug_Products_FDA>()
                .Property(e => e.ActiveIngredient)
                .IsUnicode(false);

            modelBuilder.Entity<ExerciseType>()
                .Property(e => e.Activity)
                .HasMaxLength(50);

            modelBuilder.Entity<ExerciseType>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<ExerciseType>()
                .HasMany(e => e.ExerciseDiaries)
                .WithOne(e => e.ExerciseType)
                .HasForeignKey(e => e.Type);

            modelBuilder.Entity<EXT_Glucose>()
                .HasMany(e => e.Ext_GlucoseSetting)
                .WithOne(e => e.EXT_Glucose)
                .HasForeignKey(e => e.GlucoseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EXT_Glucose>()
                .HasMany(e => e.Ext_GlucoseTags)
                .WithOne(e => e.EXT_Glucose)
                .HasForeignKey(e => e.GlucoseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FollowUp>()
                .Property(e => e.PageSeqDone)
                .IsUnicode(false);

            modelBuilder.Entity<FollowUp>()
                .HasOne(e => e.FollowUp_Goals)
                .WithOne(e => e.FollowUp)
                .HasForeignKey<FollowUp_Goals>(c => c.Id);

            modelBuilder.Entity<FollowUp>()
                .HasOne(e => e.FollowUp_HealthConditions)
                .WithOne(e => e.FollowUp)
                .HasForeignKey<FollowUp_HealthConditions>(c => c.Id);

            modelBuilder.Entity<FollowUp>()
                .HasOne(e => e.FollowUp_MedicalConditions)
                .WithOne(e => e.FollowUp)
                .HasForeignKey<FollowUp_MedicalConditions>(c => c.Id);

            modelBuilder.Entity<FollowUp>()
                .HasOne(e => e.FollowUp_HealthNumbers)
                .WithOne(e => e.FollowUp)
                .HasForeignKey<FollowUp_HealthNumbers>(c => c.Id);

            modelBuilder.Entity<FollowUp>()
                .HasOne(e => e.FollowUp_OtherRiskFactors)
                .WithOne(e => e.FollowUp)
                .HasForeignKey<FollowUp_OtherRiskFactors>(c => c.Id);

            modelBuilder.Entity<FollowUp_OtherRiskFactors>()
                .Property(e => e.ObstCig)
                .IsUnicode(false);

            modelBuilder.Entity<FoodDiary>()
                .Property(e => e.MealPlan)
                .IsUnicode(false);

            modelBuilder.Entity<FoodGroup>()
                .HasMany(e => e.FoodDiaries)
                .WithOne(e => e.FoodGroup)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HRA_Goals>()
                .Property(e => e.ExerPlan)
                .IsUnicode(false);

            modelBuilder.Entity<HRA_Goals>()
                .Property(e => e.NutMealPlan)
                .IsUnicode(false);

            modelBuilder.Entity<HRA_Goals>()
                .Property(e => e.NutMaintMealPlan)
                .IsUnicode(false);

            modelBuilder.Entity<HRA_Goals>()
                .Property(e => e.NutLowBMIMealPlan)
                .IsUnicode(false);

            modelBuilder.Entity<HRA>()
                .HasMany(e => e.HRA_ActionSteps)
                .WithOne(e => e.HRA)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HRA>()
                .HasOne(e => e.HRA_ExamsandShots)
                .WithOne(e => e.HRA)
                .HasForeignKey<HRA_ExamsandShots>(c => c.Id);

            modelBuilder.Entity<HRA>()
                .HasOne(e => e.HRA_Goals)
                .WithOne(e => e.HRA)
                .HasForeignKey<HRA_Goals>(c => c.Id);

            modelBuilder.Entity<HRA>()
                .HasOne(e => e.HRA_HealthNumbers)
                .WithOne(e => e.HRA)
                .HasForeignKey<HRA_HealthNumbers>(c => c.Id);

            modelBuilder.Entity<HRA>()
                .HasOne(e => e.HRA_HSP)
                .WithOne(e => e.HRA)
                .HasForeignKey<HRA_HSP>(c => c.Id);

            modelBuilder.Entity<HRA>()
                .HasOne(e => e.HRA_Interests)
                .WithOne(e => e.HRA)
                .HasForeignKey<HRA_Interests>(c => c.Id);

            modelBuilder.Entity<HRA>()
                .HasOne(e => e.HRA_MedicalConditions)
                .WithOne(e => e.HRA)
                .HasForeignKey<HRA_MedicalConditions>(c => c.Id);

            modelBuilder.Entity<HRA>()
                .HasOne(e => e.HRA_OtherRiskFactors)
                .WithOne(e => e.HRA)
                .HasForeignKey<HRA_OtherRiskFactors>(c => c.Id);

            modelBuilder.Entity<HRA>()
                .HasMany(e => e.ReportFeedbacks)
                .WithOne(e => e.HRA)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IncentiveType>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<IncentiveType>()
                .HasMany(e => e.PortalIncentives)
                .WithOne(e => e.IncentiveType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InsuranceSummary>()
                .Property(e => e.EnrollType)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceSummary>()
                .Property(e => e.DataSrc)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceSummary>()
                .Property(e => e.UniqueID)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceSummary>()
                .HasMany(e => e.CandidateConditions)
                .WithOne(e => e.InsuranceSummary)
                .HasForeignKey(e => e.ClaimsID);

            modelBuilder.Entity<InsuranceSummary>()
                .HasMany(e => e.CandidateMedications)
                .WithOne(e => e.InsuranceSummary)
                .HasForeignKey(e => e.ClaimsID);

            modelBuilder.Entity<InsuranceSummary>()
                .HasMany(e => e.ClaimConditionCodes)
                .WithOne(e => e.InsuranceSummary)
                .HasForeignKey(e => e.ClaimsID);

            modelBuilder.Entity<InsuranceSummary>()
                .HasMany(e => e.CandidateReasonForLastChanges)
                .WithOne(e => e.InsuranceSummary)
                .HasForeignKey(e => e.ClaimsId);

            modelBuilder.Entity<ClaimConditionCode>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Kit>()
                .Property(e => e.InvId)
                .IsUnicode(false);

            modelBuilder.Entity<Kit>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Kit>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Kit>()
                .Property(e => e.KeyConcepts)
                .IsUnicode(false);

            modelBuilder.Entity<Kit>()
                .Property(e => e.Pdf)
                .IsUnicode(false);

            modelBuilder.Entity<Kit>()
                .Property(e => e.Audio)
                .IsUnicode(false);

            modelBuilder.Entity<Kit>()
                .HasMany(e => e.KitsinPrograms)
                .WithOne(e => e.Kit)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitsinProgram>()
                .HasKey(e => new { e.ProgramId, e.KitId });

            modelBuilder.Entity<Kit>()
                .HasMany(e => e.KitsinUserPrograms)
                .WithOne(e => e.Kit)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitsinUserProgram>()
                .HasMany(e => e.KitsinUserProgramGoals)
                .WithOne(e => e.KitsinUserProgram)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Kit>()
                .HasMany(e => e.PromptsInKits)
                .WithOne(e => e.Kit)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Kit>()
                .HasMany(e => e.StepsinKits)
                .WithOne(e => e.Kit)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitsinUserProgram>()
                .HasMany(e => e.PromptsinKitsCompleteds)
                .WithOne(e => e.KitsinUserProgram)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitsinUserProgram>()
                .HasMany(e => e.UserQuizChoices)
                .WithOne(e => e.KitsinUserProgram)
                .HasForeignKey(e => e.KitsInUserProgramsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitsinUserProgram>()
                .HasMany(e => e.UserChoices)
                .WithOne(e => e.KitsinUserProgram)
                .HasForeignKey(e => e.KitsInUserProgramsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitTopic>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<KitTopic>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<KitTopic>()
                .Property(e => e.LanguageItem)
                .IsUnicode(false);

            modelBuilder.Entity<KitTopic>()
                .HasMany(e => e.Kits)
                .WithOne(e => e.KitTopic)
                .HasForeignKey(e => e.Topic)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitTranslation>()
              .Property(e => e.LanguageCode)
              .IsUnicode(false);

            modelBuilder.Entity<KitTranslation>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<KitTranslation>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<KitTranslation>()
                .Property(e => e.KeyConcepts)
                .IsUnicode(false);

            modelBuilder.Entity<KitTranslation>()
                .Property(e => e.Pdf)
                .IsUnicode(false);

            modelBuilder.Entity<KitTranslation>()
                .Property(e => e.Audio)
                .IsUnicode(false);

            modelBuilder.Entity<Lab>()
                .Property(e => e.OrderNo)
                .IsUnicode(false);

            modelBuilder.Entity<Lab>()
                .Property(e => e.CoachAlert)
                .IsUnicode(false);

            modelBuilder.Entity<Lab>()
                .Property(e => e.CriticalAlert)
                .IsUnicode(false);

            modelBuilder.Entity<Lab>()
                .HasMany(e => e.LabDatas)
                .WithOne(e => e.Lab)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LanguageItems>()
                .Property(e => e.ItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<LanguageItems>()
                .HasKey(e => new { e.LanguageCode, e.ItemCode });
            //.IsUnicode(false);
            //.Property(e => e.LanguageCode)

            modelBuilder.Entity<LanguageItems>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<Mail>()
                .HasMany(e => e.Mail11)
                .WithOne(e => e.Mail2)
                .HasForeignKey(e => e.MailId);

            modelBuilder.Entity<MealType>()
                .HasMany(e => e.FoodDiaries)
                .WithOne(e => e.MealType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Note>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<NoteType>()
                .HasMany(e => e.Notes)
                .WithOne(e => e.NoteType)
                .HasForeignKey(e => e.Type);

            modelBuilder.Entity<NotificationCategory>()
                .HasMany(e => e.NotificationEventTypes)
                .WithOne(e => e.NotificationCategory)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationContactMethod>()
                .HasMany(e => e.NotificationTemplates)
                .WithOne(e => e.NotificationContactMethod)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationEvent>()
                .HasMany(e => e.NotificationMessages)
                .WithOne(e => e.NotificationEvent)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationEventType>()
                .HasMany(e => e.NotificationEvents)
                .WithOne(e => e.NotificationEventType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationEventType>()
                .HasMany(e => e.NotificationTemplates)
                .WithOne(e => e.NotificationEventType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationMessage>()
                .Property(e => e.SubjectLine)
                .IsUnicode(false);

            modelBuilder.Entity<NotificationMessage>()
                .Property(e => e.MessageBody)
                .IsUnicode(false);

            modelBuilder.Entity<NotificationMessage>()
                .Property(e => e.BccAddress)
                .IsUnicode(false);

            modelBuilder.Entity<NotificationMessage>()
                .Property(e => e.CcAddress)
                .IsUnicode(false);

            modelBuilder.Entity<NotificationStatu>()
                .HasMany(e => e.NotificationEvents)
                .WithOne(e => e.NotificationStatu)
                .HasForeignKey(e => e.NotificationStatusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationTemplate>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<NotificationTemplate>()
                .HasMany(e => e.NotificationEvents)
                .WithOne(e => e.NotificationTemplate)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationTemplateTranslation>()
                .HasKey(e => new { e.NotificationTemplateId, e.LanguageCode, e.Subject });

            modelBuilder.Entity<OptionsforActivityQuestion>()
                .Property(e => e.TextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<OptionsforActivityQuestion>()
                .Property(e => e.OptionText)
                .IsUnicode(false);

            modelBuilder.Entity<OptionsforQuiz>()
                .Property(e => e.OptionText)
                .IsUnicode(false);

            modelBuilder.Entity<OptionsforQuiz>()
                .Property(e => e.TextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<Organization>()
                .Property(e => e.Url)
                .IsUnicode(false);

            modelBuilder.Entity<Newsletter>()
              .HasMany(e => e.AssignedNewsletters)
              .WithOne(e => e.Newsletter)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.AssignedRecipes)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.AssignedNewsletters)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.Organizations1)
                .WithOne(e => e.Organization1)
                .HasForeignKey(e => e.ParentOrganizationId);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.Portals)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.ScreeningDataErrorLogs)
                .WithOne(e => e.Organization)
                .HasForeignKey(e => e.OrgId);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.SSOProviders)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.Users)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.PhoneNumber1)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.PhoneNumber2)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.PhoneNumber3)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.Company)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.Street)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<OutreachLog>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<PassiveQuestionsInActivity>()
                .HasKey(e => new { e.ActivityId, e.QuestionId });

            modelBuilder.Entity<PassiveQuestionsInActivity>()
                .Property(e => e.QuestionText)
                .IsUnicode(false);

            modelBuilder.Entity<PassiveQuestionsInActivity>()
                .Property(e => e.TextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(e => e.TransactionId)
                .IsUnicode(false);

            modelBuilder.Entity<PortalIncentive>()
                .Property(e => e.PointsText)
                .IsUnicode(false);

            modelBuilder.Entity<PortalIncentive>()
                .HasMany(e => e.UserIncentives)
                .WithOne(e => e.PortalIncentive)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .Property(e => e.IncompleteHRA)
                .IsUnicode(false);

            modelBuilder.Entity<Portal>()
                .Property(e => e.HRAOnly)
                .IsUnicode(false);

            modelBuilder.Entity<Portal>()
                .Property(e => e.HRAandBiometrics)
                .IsUnicode(false);

            modelBuilder.Entity<Portal>()
                .Property(e => e.FirstCoaching)
                .IsUnicode(false);

            modelBuilder.Entity<Portal>()
                .Property(e => e.CompletedCoaching)
                .IsUnicode(false);

            modelBuilder.Entity<Portal>()
                .Property(e => e.FirsttimeHRA)
                .IsUnicode(false);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.AWVs)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.Eligibilities)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.HRAs)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.Notes)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.PortalIncentives)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.ProgramsinPortals)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.UserTrackingStatuses)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CoachingConditions>()
                .Property(e => e.Condition)
                .HasMaxLength(50);

            modelBuilder.Entity<CoachingConditions>()
                .HasMany(e => e.PortalCoachingConditions)
                .WithOne(e => e.CoachingConditions)
                .HasForeignKey(e => e.CoachCondId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.PortalCoachingConditions)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Profession>()
                .Property(e => e.Profession1)
                .IsUnicode(false);

            modelBuilder.Entity<ProgramInactiveReason>()
                .HasMany(e => e.UsersinPrograms)
                .WithOne(e => e.ProgramInactiveReason)
                .HasForeignKey(e => e.InactiveReason);

            modelBuilder.Entity<Program>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Program>()
                .HasMany(e => e.ProgramsinPortals)
                .WithOne(e => e.Program)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProgramsinPortal>()
                .Property(e => e.Cost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<ProgramsinPortal>()
                .HasMany(e => e.UsersinPrograms)
                .WithOne(e => e.ProgramsinPortal)
                .HasForeignKey(e => e.ProgramsinPortalsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PromptsInKit>()
                .HasMany(e => e.PromptsinKitsCompleteds)
                .WithOne(e => e.PromptsInKit)
                .HasForeignKey(e => e.PromptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionsinActivity>()
                .Property(e => e.QuestionText)
                .IsUnicode(false);

            modelBuilder.Entity<QuestionsinActivity>()
                .Property(e => e.TextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<OptionsforActivityQuestion>()
                .HasMany(e => e.SubQuestions)
                .WithOne(e => e.OptionsforActivityQuestion)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionsinActivity>()
                .HasMany(e => e.OptionsforActivityQuestions)
                .WithOne(e => e.QuestionsinActivity)
                .HasForeignKey(e => e.QuestionId);

            modelBuilder.Entity<QuestionsinActivity>()
                .HasMany(e => e.PassiveQuestionsInActivities)
                .WithOne(e => e.QuestionsinActivity)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionsinActivity>()
                .HasMany(e => e.UserChoices)
                .WithOne(e => e.QuestionsinActivity)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizinKit>()
                .Property(e => e.QuizText)
                .IsUnicode(false);

            modelBuilder.Entity<QuizinKit>()
                .Property(e => e.TextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<QuizinStep>()
                .Property(e => e.QuizText)
                .IsUnicode(false);

            modelBuilder.Entity<QuizinStep>()
                .Property(e => e.TextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<QuizinStep>()
                .HasMany(e => e.OptionsforQuizs)
                .WithOne(e => e.QuizinStep)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizinStep>()
                .HasMany(e => e.UserQuizChoices)
                .WithOne(e => e.QuizinStep)
                .HasForeignKey(e => e.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Race>()
                .HasMany(e => e.Users)
                .WithOne(e => e.Race1)
                .HasForeignKey(e => e.Race);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Carbohydrate)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Fat)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Calories)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Direction)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Ingredients)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.Yield)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .Property(e => e.ServingSize)
                .IsUnicode(false);

            modelBuilder.Entity<Recipe>()
                .HasMany(e => e.AssignedRecipes)
                .WithOne(e => e.Recipe)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasMany(e => e.RecipeTags)
                .WithOne(e => e.Recipe)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasMany(e => e.RecipeTranslations)
                .WithOne(e => e.Recipe)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeTranslation>()
                .HasKey(e => new { e.RecipeId, e.LanguageCode });
            //.Property(e => e.RecipeId);

            modelBuilder.Entity<SSOProvider>()
                .HasMany(e => e.SSOAttributeMappings)
                .WithOne(e => e.SSOProvider)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<State>()
                .HasMany(e => e.UserDoctorInfoes)
                .WithOne(e => e.State1)
                .HasForeignKey(e => e.State);

            modelBuilder.Entity<State>()
                .HasMany(e => e.HCPLists)
                .WithOne(e => e.State1)
                .HasForeignKey(e => e.State);

            modelBuilder.Entity<StepsinKit>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<StepsinKit>()
                .Property(e => e.StepNo)
                .IsUnicode(false);

            modelBuilder.Entity<StepsinKit>()
                .Property(e => e.TextLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<StepsinKit>()
                .Property(e => e.NameLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<StepsinKit>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<StepsinKit>()
                .HasMany(e => e.ActivitiesinSteps)
                .WithOne(e => e.StepsinKit)
                .HasForeignKey(e => e.StepId);

            modelBuilder.Entity<StepsinKit>()
                .HasMany(e => e.QuizinSteps)
                .WithOne(e => e.StepsinKit)
                .HasForeignKey(e => e.StepId);

            modelBuilder.Entity<StrengthTrainingLog>()
                .HasMany(e => e.StrengthTrainingSets)
                .WithOne(e => e.StrengthTrainingLog)
                .HasForeignKey(e => e.TrainingLogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StrengthTrainingType>()
                .HasMany(e => e.StrengthTrainingLogs)
                .WithOne(e => e.StrengthTrainingType)
                .HasForeignKey(e => e.TrainingTypeId);

            modelBuilder.Entity<StressDiary>()
                .Property(e => e.Trigger)
                .IsUnicode(false);

            modelBuilder.Entity<StressDiary>()
                .Property(e => e.Response)
                .IsUnicode(false);

            modelBuilder.Entity<SurveyQuestion>()
                .HasMany(e => e.SurveyResponses)
                .WithOne(e => e.SurveyQuestion)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tag>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Tag>()
                .HasMany(e => e.RecipeTags)
                .WithOne(e => e.Tag)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<TaskType>()
                .HasMany(e => e.AdminTasks)
                .WithOne(e => e.TaskType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Testimonial>()
                .Property(e => e.SignedName)
                .IsUnicode(false);

            modelBuilder.Entity<Testimonial>()
                .Property(e => e.Date)
                .IsUnicode(false);

            modelBuilder.Entity<TimeZone>()
                .Property(e => e.TimeZoneId)
                .IsUnicode(false);

            modelBuilder.Entity<TimeZone>()
                .Property(e => e.TimeZoneDisplay)
                .IsUnicode(false);

            modelBuilder.Entity<TimeZone>()
                .Property(e => e.CountryFilterList)
                .IsUnicode(false);

            modelBuilder.Entity<UserDashboardMessage>()
                .Property(e => e.Url)
                .IsUnicode(false);

            modelBuilder.Entity<UserDrug>()
                .HasOne(e => e.UserDrugAllergy)
                .WithOne(e => e.UserDrug);

            modelBuilder.Entity<UserHistoryCategory>()
                .HasMany(e => e.UserHistories)
                .WithOne(e => e.UserHistoryCategory)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserInactiveReason>()
                .HasMany(e => e.Users)
                .WithOne(e => e.UserInactiveReason)
                .HasForeignKey(e => e.InactiveReason);

            modelBuilder.Entity<User>()
                .Property(e => e.UniqueId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasOne(e => e.AdminProperty)
                .WithOne(e => e.User)
                .HasForeignKey<AdminProperty>(c => c.Id);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AdminTasks)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AdminTasks1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AdminTasks2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.UpdatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AdminTasks3)
                .WithOne(e => e.User3)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Appointments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Appointments1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CoachId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Appointments2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Appointments3)
                .WithOne(e => e.User3)
                .HasForeignKey(e => e.UpdatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Appointments4)
                .WithOne(e => e.User4)
                .HasForeignKey(e => e.NSHandledBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ApptCallTemplates)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Availabilities)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.CoachId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Availabilities1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Availabilities2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.UpdatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AWVs)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.EligibilityNotes)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Emails)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.From)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Emails1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.To)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ExerciseDiaries)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExternalUser>()
                .HasKey(e => new { e.UserId, e.ExternalDevice });

            modelBuilder.Entity<User>()
                .HasMany(e => e.ExternalUsers)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FoodDiaries)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.HRAs)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.HRAs1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Mails)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.From);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Mails1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.To);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notes)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.userId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Recipes)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notes1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.Admin)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PaymentTransactions)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SleepLogs)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StrengthTrainingLogs)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StressDiaries)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StressManagementLogs)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TobaccoLogs)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TobaccoLogs1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TobaccoLogs2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.UpdatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDashboardMessages)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDoctorInfoes)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDrugs)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDrugs1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.AddedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDrugs2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.UpdatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserHistories)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserHistories1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLogin>()
                .HasKey(e => new { e.UserId, e.LoginProvider, e.ProviderKey });

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserLogs)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Users1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UsersinPrograms)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UsersinPrograms1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CoachId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UsersinPrograms2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.EnrolledBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserTrackingStatuses)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.VitalsLogs)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.WeightLossJournals)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.WellnessDatas)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserChoice>()
                .Property(e => e.ValueLangItemCode)
                .IsUnicode(false);

            modelBuilder.Entity<UsersinProgram>()
                .HasMany(e => e.FollowUps)
                .WithOne(e => e.UsersinProgram)
                .HasForeignKey(e => e.UsersinProgramsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsersinProgram>()
                .HasMany(e => e.KitsinUserPrograms)
                .WithOne(e => e.UsersinProgram)
                .HasForeignKey(e => e.UsersinProgramsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsersinProgram>()
                .HasMany(e => e.SurveyResponses)
                .WithOne(e => e.UsersinProgram)
                .HasForeignKey(e => e.UsersinProgramsId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WellnessData>()
                .Property(e => e.UsingOtherDrug)
                .IsUnicode(false);

            modelBuilder.Entity<ClaimLoadExcludeIds>()
                .HasKey(e => new { e.Id, e.TableName });

            modelBuilder.Entity<MedicalPlanCode>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<MedicalPlanCode>()
                .HasKey(e => new { e.IVEligible, e.VendorEligible });

            modelBuilder.Entity<Portal>()
                 .HasMany(e => e.Labs)
                 .WithOne(e => e.Portal)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Labs)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.ModifiedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Labs1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.ReviewedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Labs2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.Labs3)
               .WithOne(e => e.User3)
               .HasForeignKey(e => e.RejectedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserRaffles)
                .WithOne(e => e.user)
                .HasForeignKey(e => e.userId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RaffleTypes>()
               .HasMany(e => e.RafflesinPortals)
               .WithOne(e => e.RaffleTypes)
               .HasForeignKey(e => e.RaffleTypeId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
               .HasMany(e => e.RafflesinPortals)
               .WithOne(e => e.portal)
               .HasForeignKey(e => e.PortalId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RafflesinPortals>()
               .HasMany(e => e.UserRaffles)
               .WithOne(e => e.RafflesinPortal)
               .HasForeignKey(e => e.RafflePortalId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.UserKeys)
               .WithOne(e => e.user)
               .HasForeignKey(e => e.userId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
               .HasMany(e => e.UserKeys)
               .WithOne(e => e.portal)
               .HasForeignKey(e => e.portalId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(e => e.IntuityUsers)
                .WithOne(e => e.User);

            modelBuilder.Entity<User>()
               .HasMany(e => e.EXT_Nutrition)
               .WithOne(e => e.User)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.EXT_Summaries)
               .WithOne(e => e.user)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.EXT_Sleeps)
               .WithOne(e => e.user)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.EXT_Weights)
               .WithOne(e => e.user)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.EXT_Workouts)
               .WithOne(e => e.user)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.EXT_BloodPressure)
               .WithOne(e => e.user)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
               .HasMany(e => e.PreviousUserPasswords)
               .WithOne(e => e.User)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.CanriskQuestionnaires)
                .WithOne(e => e.user)
                .HasForeignKey(e => e.CompletedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RafflesinPortals>()
               .HasMany(e => e.RaffleDates)
               .WithOne(e => e.RafflesinPortals)
               .HasForeignKey(e => e.RafflesinPortalsId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Eligibility>()
               .HasMany(e => e.CanriskQuestionnaire)
               .WithOne(e => e.Eligibility)
               .HasForeignKey(e => e.EligibilityId);

            modelBuilder.Entity<Eligibility>()
               .HasMany(e => e.CanriskTracking)
               .WithOne(e => e.Eligibility)
               .HasForeignKey(e => e.EligibilityId);

            modelBuilder.Entity<FollowUpType>()
                .HasMany(e => e.PortalFollowUps)
                .WithOne(e => e.FollowUpType)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.PortalFollowUps)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasMany(e => e.MessageRecipients)
                .WithOne(e => e.Message)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.MessageRecipients)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.RecipientId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationTemplate>()
                .HasMany(e => e.NotificationTemplateTranslations)
                .WithOne(e => e.NotificationTemplate)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.IntuityEligibilities)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IntuityEligibility>()
                .HasMany(e => e.IntuityFulfillments)
                .WithOne(e => e.IntuityEligibility)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IntuityEligibility>()
                .HasMany(e => e.IntuityFulfillmentRequests)
                .WithOne(e => e.IntuityEligibility)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IntuityEligibility>()
                .HasMany(e => e.IntuityQOHs)
                .WithOne(e => e.IntuityEligibility)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IntuityEligibility>()
               .HasMany(e => e.IntuityEPDatas)
               .WithOne(e => e.IntuityEligibility)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteContact>()
                .HasKey(e => new { e.UserId, e.FavoriteContactId });

            modelBuilder.Entity<User>()
                .HasMany(e => e.MessageRecipients)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.RecipientId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.MessageRecipients1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.ReadBy);

            modelBuilder.Entity<Kit>()
                .HasMany(e => e.KitsinPortalFollowUps)
                .WithOne(e => e.Kit)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PortalFollowUp>()
                .HasMany(e => e.KitsinPortalFollowUps)
                .WithOne(e => e.PortalFollowUp)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitsinPortalFollowUp>()
                .HasKey(e => new { e.PortalFollowUpId, e.KitId });

            modelBuilder.Entity<CRM_AccountType>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.CRM_AccountTypes)
                .HasForeignKey(e => e.AccountType);

            modelBuilder.Entity<CRM_CallerProfileType>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.CRM_CallerProfileTypes)
                .HasForeignKey(e => e.CallerProfileType);

            modelBuilder.Entity<CRM_ComplaintClassificationType>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.CRM_ComplaintClassificationTypes)
                .HasForeignKey(e => e.ComplaintClassificationType);

            modelBuilder.Entity<CRM_Contact>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.CRM_Contacts)
                .HasForeignKey(e => e.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CRM_Contact>()
                .HasMany(e => e.CRM_ChangeLogs)
                .WithOne(e => e.CRM_Contacts)
                .HasForeignKey(e => e.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CRM_Contact>()
                .HasMany(e => e.CRM_PogoMeterNumbers)
                .WithOne(e => e.CRM_Contacts)
                .HasForeignKey(e => e.CRMContactId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CRM_Note>()
                .Property(e => e.Notes)
                .IsUnicode(false);
            modelBuilder.Entity<User>()
                .HasMany(e => e.CRM_ChangeLogs)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.CRM_Contacts)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.CRM_Contacts1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.CRM_Notes1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CRM_Disposition>()
               .HasMany(e => e.CRM_Notes)
               .WithOne(e => e.CRM_Dispositions)
               .HasForeignKey(e => e.Disposition);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Users)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DeptId);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.CRM_Contacts)
                .WithOne(e => e.Countries)
                .HasForeignKey(e => e.Country);

            modelBuilder.Entity<State>()
                .HasMany(e => e.CRM_Contacts)
                .WithOne(e => e.States)
                .HasForeignKey(e => e.State);

            modelBuilder.Entity<CRM_PogoMeterNumbers>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.CRM_PogoMeterNumbers)
                .HasForeignKey(e => e.PogoMeterNumber);

            modelBuilder.Entity<HCPSpecialty>()
                .HasMany(e => e.HCPLists)
                .WithOne(e => e.HCPSpecialties)
                .HasForeignKey(e => e.SpecialtyId);

            modelBuilder.Entity<HCPList>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.HCPList)
                .HasForeignKey(e => e.HCPId);

            modelBuilder.Entity<CRM_Contact>()
                .HasMany(e => e.InsuranceTypes)
                .WithMany(e => e.CRM_Contacts);
            // //.Map(m => m.ToTable("CRM_InsuranceTypes")//.MapLeftKey("CRMId")//.MapRightKey("InsuranceType"));

            modelBuilder.Entity<IntuityEventType>()
                 .HasMany(e => e.IntuityEvents)
                 .WithOne(e => e.IntuityEventType)
                 .HasForeignKey(e => e.EventType);

            modelBuilder.Entity<User>()
                .HasMany(e => e.IntuityEvents)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.OrganizationsforWebinars)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
                .HasMany(e => e.RegisteredUsersforWebinars)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Webinar>()
                .HasMany(e => e.RegisteredUsersforWebinars)
                .WithOne(e => e.Webinar)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Webinar>()
                .HasMany(e => e.OrganizationsforWebinars)
                .WithOne(e => e.Webinar)
                .HasForeignKey(e => e.WebinarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Webinars)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.PresentedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Webinars1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Webinars2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.UpdatedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.CarePlanReports)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.CreatedBy);

            modelBuilder.Entity<LabProcedure>()
               .HasMany(e => e.PortalLabProcedures)
               .WithOne(e => e.LabProcedure)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portal>()
                .HasMany(e => e.PortalLabProcedures)
                .WithOne(e => e.Portal)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IVICDCode>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<IVICDCode>()
                .Property(e => e.CodeFlag)
                .IsUnicode(false);

            modelBuilder.Entity<IVICDCode>()
                .Property(e => e.CodeSource)
                .IsUnicode(false);

            modelBuilder.Entity<IVNDCCode>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<IVNDCCode>()
                .Property(e => e.CodeFlag)
                .IsUnicode(false);

            modelBuilder.Entity<ContactRequirementsAlert>()
                .Property(e => e.AlertType)
                .IsUnicode(false);

            modelBuilder.Entity<ContactRequirementsAlert>()
                .Property(e => e.CoachingMessage)
                .IsUnicode(false);

            modelBuilder.Entity<ContactRequirementsAlert>()
                .Property(e => e.SelfHelpMessage)
                .IsUnicode(false);

            modelBuilder.Entity<ContactRequirementsAlert>()
                .HasMany(e => e.ContactRequirements)
                .WithOne(e => e.ContactRequirementsAlert)
                .HasForeignKey(e => e.AlertId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InquiryType>()
                .HasMany(e => e.CRM_Notes)
                .WithOne(e => e.InquiryType1)
                .HasForeignKey(e => e.InquiryType);

            modelBuilder.Entity<BillingServiceType>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<BillingServiceType>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<BillingServiceType>()
                .HasMany(e => e.InvoiceDetails)
                .WithOne(e => e.BillingServiceType)
                .HasForeignKey(e => e.Type)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvoiceBilledDetail>()
                .Property(e => e.Total)
                .HasPrecision(19, 4);

            modelBuilder.Entity<InvoiceBilledDetail>()
                .HasMany(e => e.InvoiceDetails)
                .WithOne(e => e.InvoiceBilledDetail)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.InvoiceBilledDetails)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.InvoiceDetails)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserTimeTracker)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.WellnessDatas1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CollectedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.WellnessDatas2)
                .WithOne(e => e.User2)
                .HasForeignKey(e => e.UpdatedBy);

            modelBuilder.Entity<TimeTrackerDisposition>()
                .HasMany(e => e.UserTimeTrackers)
                .WithOne(e => e.TimeTrackerDisposition)
                .HasForeignKey(e => e.DispositionType);

            modelBuilder.Entity<WellnessData>()
               .HasMany(e => e.TeamsBP_PPR)
               .WithOne(e => e.WellnessData)
               .HasForeignKey(e => e.WellnessId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WellnessData>()
                .HasMany(e => e.BillingNotes)
                .WithOne(e => e.WellnessData)
                .HasForeignKey(e => e.WellnessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLoggedInDevice>()
                .HasMany(e => e.UserLogs)
                .WithOne(e => e.UserLoggedInDevice)
                .HasForeignKey(e => e.LoggedInDeviceId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserLoggedInDevices)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeclinedEnrollmentReason>()
                .HasMany(e => e.Eligibilities)
                .WithOne(e => e.DeclinedEnrollmentReason1)
                .HasForeignKey(e => e.DeclinedEnrollmentReason);

            modelBuilder.Entity<DeclinedEnrollmentReason>()
                .HasMany(e => e.UserTrackingStatuses)
                .WithOne(e => e.DeclinedEnrollmentReason1)
                .HasForeignKey(e => e.DeclinedEnrollmentReason);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FaxedReports)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FaxedReports1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.SentBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReportType>()
                .HasMany(e => e.FaxedReports)
                .WithOne(e => e.ReportType1)
                .HasForeignKey(e => e.ReportType);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.Providers)
                .WithOne(e => e.Organization)
                .HasForeignKey(e => e.OrgId);

            modelBuilder.Entity<Country>()
                .HasMany(e => e.Providers)
                .WithOne(e => e.Country1)
                .HasForeignKey(e => e.Country);

            modelBuilder.Entity<State>()
                .HasMany(e => e.Providers)
                .WithOne(e => e.State1)
                .HasForeignKey(e => e.State);

            modelBuilder.Entity<User>()
               .HasMany(e => e.HealthDatas)
               .WithOne(e => e.User)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.HealthDatas1)
                .WithOne(e => e.User1)
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MotivationMessage>()
                .Property(e => e.Subject)
                .IsUnicode(false);

            modelBuilder.Entity<MotivationMessage>()
                .Property(e => e.MessageContent)
                .IsUnicode(false);

            modelBuilder.Entity<MotivationMessage>()
                .HasMany(e => e.AssignedMotivationMessages)
                .WithOne(e => e.MotivationMessage)
                .HasForeignKey(e => e.MessagesID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.AssignedMotivationMessages)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Webinar>()
                .HasMany(e => e.WebinarOccurrences)
                .WithOne(e => e.Webinar)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserWearableDevices)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WearableDevice>()
                .HasMany(e => e.UserWearableDevices)
                .WithOne(e => e.WearableDevice)
                .HasForeignKey(e => e.WearableDeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLoggedInDevice>()
                .HasMany(e => e.SecurityCodes)
                .WithOne(e => e.UserLoggedInDevice)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PreviousPassword>()
                .HasKey(e => new { e.PasswordHash, e.UserId });

            modelBuilder.Entity<LegacyAppointmentRemainderResult>()
                .HasKey(e => new { e.AppRef });

            modelBuilder.Entity<Languages>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.Languages");

                entity.Property(e => e.Language)
                    .HasMaxLength(50)
                    .HasColumnName("Language");
                entity.Property(e => e.LanguageCode).HasMaxLength(10);
                entity.Property(e => e.LanguageItem).HasMaxLength(10);

                entity.HasMany(d => d.Portal).WithMany(p => p.Languages)
                    .UsingEntity<Dictionary<string, object>>(
                        "LanguagesinPortal",
                        r => r.HasOne<Portal>().WithMany()
                            .HasForeignKey("PortalId")
                            .HasConstraintName("FK_dbo.LanguagesinPortal_dbo.Portals_PortalId"),
                        l => l.HasOne<Languages>().WithMany()
                            .HasForeignKey("LanguageId")
                            .HasConstraintName("FK_dbo.LanguagesinPortal_dbo.Languages_LanguageId"),
                        j =>
                        {
                            j.HasKey("LanguageId", "PortalId").HasName("PK_dbo.LanguagesinPortals");
                            j.ToTable("LanguagesinPortals");
                            j.HasIndex(new[] { "LanguageId" }, "IX_LanguageId");
                            j.HasIndex(new[] { "PortalId" }, "IX_PortalId");
                        });

                entity.HasMany(d => d.Users).WithMany(p => p.Languages)
                    .UsingEntity<Dictionary<string, object>>(
                        "CoachLanguage",
                        r => r.HasOne<User>().WithMany()
                            .HasForeignKey("UserId")
                            .HasConstraintName("FK_dbo.CoachLanguages_dbo.Users_UserId"),
                        l => l.HasOne<Languages>().WithMany()
                            .HasForeignKey("LanguageId")
                            .HasConstraintName("FK_dbo.CoachLanguages_dbo.Languages_LanguageId"),
                        j =>
                        {
                            j.HasKey("LanguageId", "UserId").HasName("PK_dbo.CoachLanguages");
                            j.ToTable("CoachLanguages");
                            j.HasIndex(new[] { "LanguageId" }, "IX_LanguageId");
                            j.HasIndex(new[] { "UserId" }, "IX_UserId");
                        });
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Specialization");

                entity.Property(e => e.LanguageId).HasMaxLength(50);
                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasMany(d => d.Portal).WithMany(p => p.Specializations)
                    .UsingEntity<Dictionary<string, object>>(
                        "SpecializationsinPortal",
                        r => r.HasOne<Portal>().WithMany()
                            .HasForeignKey("PortalId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_SpecializationsinPortals_Portals"),
                        l => l.HasOne<Specialization>().WithMany()
                            .HasForeignKey("SpecializationId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_SpecializationsinPortals_Specializations"),
                        j =>
                        {
                            j.HasKey("SpecializationId", "PortalId").HasName("PK_SpecializationsinPortals_1");
                            j.ToTable("SpecializationsinPortals");
                        });
            });

            modelBuilder.Entity<KitTranslation>(entity =>
            {
                entity.HasKey(e => new { e.KitId, e.LanguageCode });

                entity.Property(e => e.LanguageCode).HasMaxLength(20);
                entity.Property(e => e.Audio).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.LastUpdated).HasColumnType("datetime");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Pdf).HasMaxLength(250);
                entity.Property(e => e.PublishedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Kits).WithMany(p => p.KitTranslations)
                    .HasForeignKey(d => d.KitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_KitTranslations_Kits");
            });

            modelBuilder.Entity<NotificationEventType>(entity =>
            {
                entity.HasMany(d => d.Portals).WithMany(p => p.NotificationEventTypes)
               .UsingEntity<Dictionary<string, object>>(
                   "NotificationEventTypePortalMap",
                   r => r.HasOne<Portal>().WithMany()
                       .HasForeignKey("PortalId")
                       .OnDelete(DeleteBehavior.ClientSetNull)
                       .HasConstraintName("FK_NotificationEventTypePortalMap_Portals"),
                   l => l.HasOne<NotificationEventType>().WithMany()
                       .HasForeignKey("NotificationEventTypeId")
                       .OnDelete(DeleteBehavior.ClientSetNull)
                       .HasConstraintName("FK_NotificationEventTypePortalMap_NotificationEventType"),
                   j =>
                   {
                       j.HasKey("NotificationEventTypeId", "PortalId");
                       j.ToTable("NotificationEventTypePortalMap");
                       j.IndexerProperty<int>("NotificationEventTypeId").HasColumnName("NotificationEventTypeID");
                   });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(d => d.UserRoles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserUserRole",
                    r => r.HasOne<UserRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserUserRole_UserRole"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserUserRole_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK_UserUserRole_UserID_RoleID");
                        j.ToTable("UserUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_UserUserRole_RoleID");
                        j.HasIndex(new[] { "UserId" }, "IX_UserUserRole_UserID");
                    });

                entity.HasMany(d => d.Specializations).WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "CoachSpecialization",
                        r => r.HasOne<Specialization>().WithMany()
                            .HasForeignKey("SpecializationId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_CoachSpecialization_Specialization"),
                        l => l.HasOne<User>().WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_CoachSpecialization_Users"),
                        j =>
                        {
                            j.HasKey("UserId", "SpecializationId").HasName("PK_CoachSpecialization");
                            j.ToTable("CoachSpecializations");
                        });

                entity.HasOne(d => d.State1).WithMany(p => p.Coaches)
                    .HasForeignKey(d => d.State)
                    .HasConstraintName("FK_Users_States");
            });

            modelBuilder.Entity<AdminModule>(entity =>
            {
                entity.Property(e => e.Icon)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.IconColor)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Image)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Url)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasMany(d => d.UserRoles).WithMany(p => p.AdminModules)
                    .UsingEntity<Dictionary<string, object>>(
                        "RolesinAdminModule",
                        r => r.HasOne<UserRole>().WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_RolesinAdminModules_UserRoles"),
                        l => l.HasOne<AdminModule>().WithMany()
                            .HasForeignKey("AdminModuleId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_RolesinAdminModules_AdminModules"),
                        j =>
                        {
                            j.HasKey("AdminModuleId", "RoleId");
                            j.ToTable("RolesinAdminModules");
                        });
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasMany(d => d.UserRoles).WithMany(p => p.Organizations)
                    .UsingEntity<Dictionary<string, object>>(
                        "RolesinOrganization",
                        r => r.HasOne<UserRole>().WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_RolesinOrganizations_UserRoles"),
                        l => l.HasOne<Organization>().WithMany()
                            .HasForeignKey("OrganizationId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_RolesinOrganizations_Organizations"),
                        j =>
                        {
                            j.HasKey("OrganizationId", "RoleId");
                            j.ToTable("RolesinOrganizations");
                        });
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(10);
                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Country).WithMany(p => p.States)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_States_Countries");

                entity.HasMany(d => d.Users).WithMany(p => p.CoachStates)
                    .UsingEntity<Dictionary<string, object>>(
                        "CoachState",
                        r => r.HasOne<User>().WithMany()
                            .HasForeignKey("UserId")
                            .HasConstraintName("FK_dbo.CoachStates_dbo.Users_UserId"),
                        l => l.HasOne<State>().WithMany()
                            .HasForeignKey("StateId")
                            .HasConstraintName("FK_dbo.CoachStates_dbo.States_StateId"),
                        j =>
                        {
                            j.HasKey("StateId", "UserId").HasName("PK_dbo.CoachStates");
                            j.ToTable("CoachStates");
                        });
            });

            modelBuilder.Entity<Kit>(entity =>
            {
                entity.Property(e => e.Audio)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.InvId)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.KeyConcepts).IsUnicode(false);
                entity.Property(e => e.LastUpdated).HasColumnType("datetime");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Pdf)
                    .HasMaxLength(250)
                    .IsUnicode(false);
                entity.Property(e => e.PublishedDate).HasColumnType("datetime");

                entity.HasOne(d => d.KitTopic).WithMany(p => p.Kits)
                    .HasForeignKey(d => d.Topic)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Kits_KitTopics");

                entity.HasMany(d => d.Portal).WithMany(p => p.Kits)
                    .UsingEntity<Dictionary<string, object>>(
                        "Kitsinportal",
                        r => r.HasOne<Portal>().WithMany()
                            .HasForeignKey("PortalId")
                            .HasConstraintName("FK_dbo.Kitsinportal_dbo.Portals_PortalId"),
                        l => l.HasOne<Kit>().WithMany()
                            .HasForeignKey("KitId")
                            .HasConstraintName("FK_dbo.Kitsinportal_dbo.Kits_KitId"),
                        j =>
                        {
                            j.HasKey("KitId", "PortalId").HasName("PK_dbo.Kitsinportal");
                            j.ToTable("Kitsinportals");
                        });
            });

            modelBuilder.Entity<Drug_Ingredients>(entity =>
            {
                entity.HasOne(d => d.Drug_Products)
                    .WithMany(p => p.Drug_Ingredients)
                    .HasForeignKey(d => d.Drug_code)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Drug_Ingredients_Drug_Products");
            });

            modelBuilder.Entity<Drug_ProductsForm>(entity =>
            {
                entity.HasOne(d => d.Drug_Products)
                    .WithMany(p => p.Drug_ProductsForm)
                    .HasForeignKey(d => d.DRUG_CODE)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Drug_ProductsForm_Drug_Products");
            });
        }
    }
}
