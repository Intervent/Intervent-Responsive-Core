using AutoMapper;
using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace Intervent.Web.DataLayer
{
    public class Utility
    {
        public static IMapper mapper;

        public static void InitMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                    .ForMember(dto => dto.Roles, opt => opt.MapFrom(src => src.UserRoles))
                    .ForMember(dto => dto.AdminProperty, opt => opt.MapFrom(src => src.AdminProperty));

                cfg.CreateMap<DAL.UserRole, RoleDto>()
                    .ForMember(dto => dto.Users, opt => opt.MapFrom(src => src.Users))
                    .ForMember(dto => dto.AdminModules, opt => opt.MapFrom(src => src.AdminModules));

                cfg.CreateMap<DAL.Question, QuestionDto>();

                cfg.CreateMap<DAL.User, ApplicationUser>();

                cfg.CreateMap<ApplicationUser, DAL.User>();

                cfg.CreateMap<UserDto, ApplicationUser>();

                cfg.CreateMap<ApplicationUser, UserDto>();

                cfg.CreateMap<DAL.UserTrackingStatus, UserTrackingStatusDto>();

                cfg.CreateMap<DAL.UserDoctorInfo, UserDoctorInfoDto>();

                cfg.CreateMap<DAL.UserLog, UserLogsDto>();

                cfg.CreateMap<DAL.UserForm, UserFormDto>();

                cfg.CreateMap<UserFormDto, DAL.UserForm>();

                cfg.CreateMap<DAL.UserInactiveReason, UserInactiveReasonDto>();

                cfg.CreateMap<DAL.AdminProperty, AdminPropertyDto>();

                cfg.CreateMap<AdminPropertyDto, DAL.AdminProperty>();

                cfg.CreateMap<DAL.TimeZone, TimeZoneDto>();

                cfg.CreateMap<DAL.PortalIncentive, PortalIncentiveDto>();

                cfg.CreateMap<DAL.SurveyResponse, SurveyResponseDto>();

                cfg.CreateMap<DAL.FollowUpType, FollowUpTypeDto>();

                cfg.CreateMap<DAL.PortalFollowUp, PortalFollowUpDto>();

                cfg.CreateMap<PortalFollowUpDto, DAL.PortalFollowUp>();

                cfg.CreateMap<SurveyResponseDto, DAL.SurveyResponse>();

                cfg.CreateMap<DAL.UserIncentive, UserIncentiveDto>()
                    .ForMember(dto => dto.PortalIncentive, opt => opt.MapFrom(src => src.PortalIncentive));

                cfg.CreateMap<PortalIncentiveDto, DAL.PortalIncentive>();
                cfg.CreateMap<UserIncentiveDto, DAL.UserIncentive>();

                cfg.CreateMap<DAL.Profession, ProfessionDto>();

                cfg.CreateMap<DAL.Organization, OrganizationDto>()
                    .ForMember(dto => dto.Portals, opt => opt.MapFrom(src => src.Portals))
                    .ForMember(dto => dto.UserRoles, opt => opt.MapFrom(src => src.UserRoles))
                    .ForMember(dto => dto.CouponCodes, opt => opt.MapFrom(src => src.CouponCodes));

                cfg.CreateMap<DAL.Portal, PortalDto>()
                    .ForMember(dto => dto.ProgramsinPortal, opt => opt.MapFrom(src => src.ProgramsinPortals));

                cfg.CreateMap<DAL.ProgramsinPortal, ProgramsinPortalDto>();

                cfg.CreateMap<DAL.PortalIncentive, PortalIncentiveDto>();

                cfg.CreateMap<PortalIncentiveDto, DAL.PortalIncentive>();

                cfg.CreateMap<DAL.WellnessData, WellnessDataDto>();

                cfg.CreateMap<DAL.EXT_Glucose, EXT_GlucoseDto>();

                cfg.CreateMap<DAL.Lab, LabDto>();
                cfg.CreateMap<LabDto, DAL.Lab>();
                cfg.CreateMap<DAL.Lab, DAL.HRA_HealthNumbers>();
                cfg.CreateMap<DAL.FilteredCoachListResult, FilteredCoachListResultDto>();

                cfg.CreateMap<WellnessDataDto, DAL.WellnessData>();

                cfg.CreateMap<DAL.Appointment, AppointmentDTO>();

                cfg.CreateMap<DAL.AppointmentType, AppointmentTypesDto>();

                cfg.CreateMap<DAL.AppointmentFeedback, AppointmentFeedbackDto>();

                cfg.CreateMap<AppointmentFeedbackDto, DAL.AppointmentFeedback>();

                cfg.CreateMap<DAL.CancellationReason, CancellationReasonDto>();

                cfg.CreateMap<DAL.IncentiveType, IncentiveTypeDto>();

                cfg.CreateMap<DAL.ProcessIncentives_Result, ProcessIncentives_ResultDto>();

                cfg.CreateMap<DAL.CustomIncentiveType, CustomIncentiveTypesDto>();

                cfg.CreateMap<DAL.Program, ProgramDto>().ForMember(dto => dto.kitsinProgram, opt => opt.MapFrom(src => src.KitsinPrograms));

                cfg.CreateMap<DAL.KitsinProgram, KitsinProgramDto>()
                    .ForMember(dto => dto.program, opt => opt.MapFrom(src => src.Program))
                    .ForMember(dto => dto.eduKit, opt => opt.MapFrom(src => src.Kit));

                cfg.CreateMap<DAL.KitsinPortalFollowUp, KitsinPortalFollowUpDto>()
                    .ForMember(dto => dto.eduKit, opt => opt.MapFrom(src => src.Kit));
                cfg.CreateMap<DAL.Kit, KitsDto>().
                    ForMember(dto => dto.StepsinKits, opt => opt.MapFrom(src => src.StepsinKits));

                cfg.CreateMap<DAL.KitTranslation, KitTranslationDto>();

                cfg.CreateMap<DAL.Tag, TagsDto>();
                cfg.CreateMap<DAL.LanguageItems, LanguageItemDto>();

                cfg.CreateMap<DAL.RecipeTag, RecipeTagsDto>().ForMember(sto => sto.Tag, opt => opt.MapFrom(src => src.Tag));

                cfg.CreateMap<DAL.Recipe, RecipesDto>().ForMember(sto => sto.RecipeTags, opt => opt.MapFrom(src => src.RecipeTags));

                cfg.CreateMap<DAL.RecipeTranslation, RecipeTranslationDto>();

                cfg.CreateMap<DAL.StressDiary, StressDiaryDto>();

                cfg.CreateMap<StressDiaryDto, DAL.StressDiary>();

                cfg.CreateMap<DAL.FoodDiary, FoodDiaryDto>();

                cfg.CreateMap<FoodDiaryDto, DAL.FoodDiary>();

                cfg.CreateMap<DAL.FoodGroup, FoodGroupDto>();

                cfg.CreateMap<DAL.MealType, MealTypeDto>();

                cfg.CreateMap<DAL.ExerciseDiary, ExerciseDiaryDto>();

                cfg.CreateMap<ExerciseDiaryDto, DAL.ExerciseDiary>();

                cfg.CreateMap<DAL.ExerciseType, ExerciseTypeDto>();

                cfg.CreateMap<DAL.StrengthTrainingType, StrengthTrainingTypeDto>();

                cfg.CreateMap<DAL.StrengthTrainingLog, StrengthTrainingLogDto>().

                    ForMember(dto => dto.StrengthTrainingLogSet, opt => opt.MapFrom(src => src.StrengthTrainingSets)); ;

                cfg.CreateMap<StrengthTrainingLogDto, DAL.StrengthTrainingLog>();

                cfg.CreateMap<DAL.StrengthTrainingSet, StrengthTrainingSetDto>();

                cfg.CreateMap<StrengthTrainingSetDto, DAL.StrengthTrainingSet>();

                cfg.CreateMap<StressManagementLogDto, DAL.StressManagementLog>();

                cfg.CreateMap<DAL.StressManagementLog, StressManagementLogDto>();

                cfg.CreateMap<DAL.TobaccoLog, TobaccoLogDto>();

                cfg.CreateMap<TobaccoLogDto, DAL.TobaccoLog>();

                cfg.CreateMap<DAL.SleepLog, SleepLogDto>();

                cfg.CreateMap<SleepLogDto, DAL.SleepLog>();

                cfg.CreateMap<DAL.Specialization, SpecializationDto>();

                cfg.CreateMap<SpecializationDto, DAL.Specialization>();

                cfg.CreateMap<DAL.StepsinKit, StepsinKitsDto>().ForMember(stk => stk.QuizInSteps, opt => opt.MapFrom(src => src.QuizinSteps));

                cfg.CreateMap<DAL.PassiveQuestionsInActivity, PassiveQuestionsInActivitiesDto>();

                cfg.CreateMap<DAL.OptionsforActivityQuestion, OptionsforActivityQuestionDto>();

                cfg.CreateMap<DAL.QuestionsinActivity, QuestionsinActivityDto>().ForMember(dto => dto.OptionsforActivityQuestions, opt => opt.MapFrom(src => src.OptionsforActivityQuestions));

                cfg.CreateMap<DAL.ActivitiesinStep, ActivitiesinStepsDto>()
                    .ForMember(dto => dto.PassiveQuestionsInActivities, opt => opt.MapFrom(src => src.PassiveQuestionsInActivities))
                    .ForMember(dto => dto.QuestionsinActivities, opt => opt.MapFrom(src => src.QuestionsinActivities));

                cfg.CreateMap<DAL.OptionsforQuiz, OptionsforQuizDto>();

                cfg.CreateMap<DAL.QuizinStep, QuizinStepDto>().ForMember(dto => dto.optionsforQuiz, opt => opt.MapFrom(src => src.OptionsforQuizs));

                cfg.CreateMap<DAL.PromptsInKit, PromptDto>();

                cfg.CreateMap<PromptDto, DAL.PromptsInKit>();

                cfg.CreateMap<DAL.KitsinUserProgramGoal, KitsinUserProgramGoalDto>();

                cfg.CreateMap<KitsinUserProgramGoalDto, DAL.KitsinUserProgramGoal>();

                cfg.CreateMap<DAL.Country, CountryDto>();

                cfg.CreateMap<DAL.IntuityEligibility, IntuityEligibilityDto>();

                cfg.CreateMap<DAL.IntuityEligibilityLog, IntuityEligibilityLogDto>();

                cfg.CreateMap<IntuityEligibilityDto, DAL.IntuityEligibility>();

                cfg.CreateMap<DAL.IntuityEligibility, IntuityEligibilityDto>();

                cfg.CreateMap<IntuityEligibilityLogDto, DAL.IntuityEligibilityLog>();

                cfg.CreateMap<IntuityFulfillmentsDto, DAL.IntuityFulfillments>();

                cfg.CreateMap<DAL.IntuityFulfillments, IntuityFulfillmentsDto>();

                cfg.CreateMap<IntuityFulfillmentRequestsDto, DAL.IntuityFulfillmentRequests>();

                cfg.CreateMap<DAL.IntuityFulfillmentRequests, IntuityFulfillmentRequestsDto>();

                cfg.CreateMap<IntuityQOHDto, DAL.IntuityQOH>();

                cfg.CreateMap<DAL.IntuityQOH, IntuityQOHDto>();

                cfg.CreateMap<DAL.State, StateDto>();

                cfg.CreateMap<DAL.Race, RaceDto>();

                cfg.CreateMap<DAL.Measurement, MeasurementsDto>();

                cfg.CreateMap<DAL.LabReferenceRanx, LabReferenceRangeDto>();

                cfg.CreateMap<DAL.HRA, HRADto>()
                    .ForMember(dto => dto.MedicalCondition, opt => opt.MapFrom(src => src.HRA_MedicalConditions))
                    .ForMember(dto => dto.OtherRiskFactors, opt => opt.MapFrom(src => src.HRA_OtherRiskFactors))
                    .ForMember(dto => dto.HSP, opt => opt.MapFrom(src => src.HRA_HSP))
                    .ForMember(dto => dto.Exams, opt => opt.MapFrom(src => src.HRA_ExamsandShots))
                    .ForMember(dto => dto.Interest, opt => opt.MapFrom(src => src.HRA_Interests))
                    .ForMember(dto => dto.HealthNumbers, opt => opt.MapFrom(src => src.HRA_HealthNumbers))
                    .ForMember(dto => dto.Goals, opt => opt.MapFrom(src => src.HRA_Goals));

                cfg.CreateMap<DAL.AWV_AlcoholUse, Intervent.Web.DTO.AWV.AlcoholUseDto>();
                cfg.CreateMap<DAL.AWV_Allergies, DTO.AWV.AllergiesDto>();
                cfg.CreateMap<DAL.AWV_DepressionScreens, DTO.AWV.DepressionScreensDto>();
                cfg.CreateMap<DAL.AWV_General, DTO.AWV.GeneralDto>();
                cfg.CreateMap<DAL.AWV_HomeScreens, DTO.AWV.HomeScreensDto>();
                cfg.CreateMap<DAL.AWV_HospitalVisits, DTO.AWV.HospitalVisitsDto>();
                cfg.CreateMap<DAL.AWV_Medications, DTO.AWV.MedicationsDto>();
                cfg.CreateMap<DAL.AWV_OsteoporosisScreens, DTO.AWV.OsteoporosisScreensDto>();
                cfg.CreateMap<DAL.AWV_PreventiveServices, DTO.AWV.PreventiveServicesDto>();
                cfg.CreateMap<DAL.AWV_Services, DTO.AWV.ServicesDto>();
                cfg.CreateMap<DAL.AWV_STDandProstateRisk, DTO.AWV.STDandProstateRiskDto>();
                cfg.CreateMap<DAL.AWV_TobaccoUse, DTO.AWV.TobaccoUseDto>();
                cfg.CreateMap<DAL.AWV_UrinaryScreens, DTO.AWV.UrinaryScreensDto>();
                cfg.CreateMap<DAL.AWV_MedicalHistandProviders, DTO.AWV.FamilyHistoryDto>();
                cfg.CreateMap<DAL.AWV_MedicalHistandProviders, DTO.AWV.SurgeriesDto>();
                cfg.CreateMap<DAL.AWV_MedicalHistandProviders, DTO.AWV.MedicalConditionsDto>();
                cfg.CreateMap<DAL.AWV_Biometrics, DTO.AWV.BiometricsDto>();
                cfg.CreateMap<DAL.AWV_Goals, DTO.AWV.GoalsDto>();
                cfg.CreateMap<DAL.AWV, DTO.AWV.AnnulWellnessVisitGeneralInfoDto>();

                cfg.CreateMap<DTO.AWV.GoalsDto, DAL.AWV_Goals>();
                cfg.CreateMap<DTO.AWV.AlcoholUseDto, DAL.AWV_AlcoholUse>();
                cfg.CreateMap<DTO.AWV.AllergiesDto, DAL.AWV_Allergies>();
                cfg.CreateMap<DTO.AWV.DepressionScreensDto, DAL.AWV_DepressionScreens>();
                cfg.CreateMap<DTO.AWV.GeneralDto, DAL.AWV_General>();
                cfg.CreateMap<DTO.AWV.HomeScreensDto, DAL.AWV_HomeScreens>();
                cfg.CreateMap<DTO.AWV.HospitalVisitsDto, DAL.AWV_HospitalVisits>();
                cfg.CreateMap<DTO.AWV.MedicationsDto, DAL.AWV_Medications>();
                cfg.CreateMap<DTO.AWV.OsteoporosisScreensDto, DAL.AWV_OsteoporosisScreens>();
                cfg.CreateMap<DTO.AWV.ServicesDto, DAL.AWV_Services>();
                cfg.CreateMap<DTO.AWV.STDandProstateRiskDto, DAL.AWV_STDandProstateRisk>();
                cfg.CreateMap<DTO.AWV.TobaccoUseDto, DAL.AWV_TobaccoUse>();
                cfg.CreateMap<DTO.AWV.UrinaryScreensDto, DAL.AWV_UrinaryScreens>();
                cfg.CreateMap<DTO.AWV.PreventiveServicesDto, DAL.AWV_PreventiveServices>();
                cfg.CreateMap<DTO.AWV.MedicalConditionsDto, DAL.AWV_MedicalHistandProviders>();
                cfg.CreateMap<DTO.AWV.SurgeriesDto, DAL.AWV_MedicalHistandProviders>();
                cfg.CreateMap<DTO.AWV.FamilyHistoryDto, DAL.AWV_MedicalHistandProviders>();
                cfg.CreateMap<DTO.AWV.BiometricsDto, DAL.AWV_Biometrics>();

                cfg.CreateMap<DAL.AWV, AWVDto>();

                cfg.CreateMap<DAL.FollowUp_MedicalConditions, FollowUp_MedicalConditionsDto>();
                cfg.CreateMap<DAL.FollowUp_HealthConditions, FollowUp_HealthConditionsDto>();
                cfg.CreateMap<DAL.FollowUp_HealthNumbers, FollowUp_HealthNumbersDto>();
                cfg.CreateMap<DAL.FollowUp_OtherRiskFactors, FollowUp_OtherRiskFactorsDto>();

                cfg.CreateMap<FollowUp_MedicalConditionsDto, DAL.FollowUp_MedicalConditions>();
                cfg.CreateMap<FollowUp_HealthConditionsDto, DAL.FollowUp_HealthConditions>();
                cfg.CreateMap<FollowUp_HealthNumbersDto, DAL.FollowUp_HealthNumbers>();
                cfg.CreateMap<FollowUp_OtherRiskFactorsDto, DAL.FollowUp_OtherRiskFactors>();

                cfg.CreateMap<DAL.FollowUp, FollowUpDto>()
                   .ForMember(dto => dto.UsersinProgram, opt => opt.MapFrom(src => src.UsersinProgram));

                cfg.CreateMap<DAL.HRA_MedicalConditions, MedicalConditionsDto>();

                cfg.CreateMap<DAL.HRA_OtherRiskFactors, OtherRiskFactorsDto>();

                cfg.CreateMap<DAL.HRA_HSP, HSPDto>();

                cfg.CreateMap<DAL.HRA_ExamsandShots, ExamsandShotsDto>();

                cfg.CreateMap<DAL.HRA_Interests, InterestsDto>();

                cfg.CreateMap<DAL.HRA_HealthNumbers, HealthNumbersDto>();

                cfg.CreateMap<DAL.HRA_Goals, HRAGoalsDto>();

                cfg.CreateMap<DAL.HRA_ActionSteps, HRA_ActionStepsDto>();

                cfg.CreateMap<DAL.ActionStepType, ActionStepTypeDto>();

                cfg.CreateMap<DAL.CouponCode, CouponCodeDto>();

                cfg.CreateMap<DAL.GetTobaccoIncentive_Result, SmokingCessationIncentiveDto>();

                //cfg.CreateMap<DAL.IncentiveValue, IncentiveValueDto>();
                //cfg.CreateMap<DAL.Incentive, IncentiveDto>()
                //    .ForMember(dto => dto.IncentiveValues, iv => iv.MapFrom(src => src.IncentiveValues))
                //    .ForMember(dto => dto.PortalIncentives, iv => iv.MapFrom(src => src.PortalIncentives));

                cfg.CreateMap<DAL.UsersinProgram, UsersinProgramDto>();

                cfg.CreateMap<DAL.ProgramInactiveReason, ProgramInactiveReasonDto>();

                cfg.CreateMap<DAL.UserDashboardMessage, UserDashboardMessageDto>();

                cfg.CreateMap<DAL.DashboardMessageType, DashboardMessageTypeDto>();

                cfg.CreateMap<DAL.DeclinedEnrollmentReason, DeclinedEnrollmentReasonsDto>();

                cfg.CreateMap<DAL.KitsinUserProgram, KitsinUserProgramDto>();

                cfg.CreateMap<DAL.KitTopic, KitTopicsDto>();

                cfg.CreateMap<DAL.Kit, KitsDto>();

                cfg.CreateMap<DAL.Note, NotesDto>()
                    .ForMember(dto => dto.User1, opt => opt.MapFrom(src => src.User1));

                cfg.CreateMap<NotesDto, DAL.Note>();

                cfg.CreateMap<DAL.NoteType, NoteTypesDto>();

                cfg.CreateMap<DAL.ReferralTypes, ReferralTypesDto>();

                cfg.CreateMap<DAL.TaskType, TaskTypesDto>();

                cfg.CreateMap<DAL.RaffleTypes, RaffleTypesDto>();

                cfg.CreateMap<DAL.RaffleDates, RaffleDatesDto>();

                cfg.CreateMap<DAL.RafflesinPortals, RafflesinPortalsDto>();

                cfg.CreateMap<DAL.UserRaffles, UserRafflesDto>();

                cfg.CreateMap<DAL.UserKeys, UserKeysDto>();

                cfg.CreateMap<RafflesinPortalsDto, DAL.RafflesinPortals>();

                cfg.CreateMap<DAL.AdminTask, TasksDto>();

                cfg.CreateMap<DAL.Eligibility, GenderDto>()
                    .ForMember(dto => dto.Key, options => options.MapFrom(x => x.Gender));

                cfg.CreateMap<DAL.Eligibility, EligibilityUserEnrollmentTypeDto>()
                    .ForMember(dto => dto.UserEnrollmentTypeKey, options => options.MapFrom(x => x.UserEnrollmentType));

                cfg.CreateMap<DAL.Eligibility, EligibilityPayTypeDto>()
                    .ForMember(dto => dto.Key, options => options.MapFrom(x => x.PayType));

                cfg.CreateMap<DAL.Eligibility, EligibilityUserStatusDto>()
                    .ForMember(dto => dto.Key, options => options.MapFrom(x => x.UserStatus));

                /* cfg.CreateMap<DAL.Eligibility, EligibilityDto>()
                     .ForMember(dto => dto.Gender, options => options.MapFrom(x => cfg.Map<DAL.Eligibility, GenderDto>(x)))
                     .ForMember(dto => dto.UserEnrollmentType, options => options.MapFrom(x => cfg.Map<DAL.Eligibility, EligibilityUserEnrollmentTypeDto>(x)))
                     .ForMember(dto => dto.PayType, options => options.MapFrom(x => cfg.Map<DAL.Eligibility, EligibilityPayTypeDto>(x)))
                     .ForMember(dto => dto.UserStatus, options => options.MapFrom(x => cfg.Map<DAL.Eligibility, EligibilityUserStatusDto>(x)))
                     .ForMember(dto => dto.UnionFlag, options => options.MapFrom(x => x.UnionFlag == "Y"))
                     .ForMember(dto => dto.TobaccoFlag, options => options.MapFrom(x => x.TobaccoFlag == "Y"))
                     .ForMember(dto => dto.EducationalAssociates, options => options.MapFrom(x => x.EducationalAssociates == "Y"));
     */
                cfg.CreateMap<DAL.WeightLossJournal, WeightLossJournalDto>();

                cfg.CreateMap<DAL.AdminModule, AdminModuleDto>();

                cfg.CreateMap<DAL.ExternalUser, ExternalUserDto>();

                cfg.CreateMap<EligibilityUserEnrollmentTypeDto, string>().ConvertUsing(x => x.UserEnrollmentTypeKey);

                cfg.CreateMap<DAL.NotificationEventType, NotificationEventTypeDto>();

                cfg.CreateMap<NotificationEventTypeDto, DAL.NotificationEventType>();

                cfg.CreateMap<DAL.NotificationEvent, NotificationEventTypeDto>()
                    .ForMember(dto => dto.Id, options => options.MapFrom(x => x.NotificationEventTypeId));

                cfg.CreateMap<DAL.NotificationTemplate, NotificationTemplateDto>();

                cfg.CreateMap<DAL.NotificationEvent, NotificationEventDto>();
                // .ForMember(dto => dto.NotificationEventType, options => options.MapFrom(x => cfg.Map<DAL.NotificationEvent, NotificationEventTypeDto>(x)));

                cfg.CreateMap<DAL.NotificationMessage, NotificationMessageDto>();

                cfg.CreateMap<OutreachLogDto, DAL.OutreachLog>();

                cfg.CreateMap<DAL.OutreachLog, OutreachLogDto>();

                cfg.CreateMap<DAL.MedicalPlanCode, MedicalPlanCodeDto>();

                cfg.CreateMap<DAL.ApptCallTemplate, AppointmentCallTemplateDto>();

                cfg.CreateMap<DAL.ApptCallInterval, AppointmentCallIntervalDto>();

                cfg.CreateMap<DAL.PromptsinKitsCompleted, PromptsinKitsCompletedDto>();

                cfg.CreateMap<PromptsinKitsCompletedDto, DAL.PromptsinKitsCompleted>();

                cfg.CreateMap<DAL.SurveyQuestion, SurveyQuestionDto>();

                cfg.CreateMap<SurveyQuestionDto, DAL.SurveyQuestion>();

                cfg.CreateMap<DAL.EligibilityNote, EligibilityNotesDto>();

                cfg.CreateMap<EligibilityNotesDto, DAL.EligibilityNote>();

                cfg.CreateMap<DAL.AssignedRecipe, AssignedRecipeDto>();

                cfg.CreateMap<AssignedRecipeDto, DAL.AssignedRecipe>();

                cfg.CreateMap<CandidateConditionsDto, DAL.CandidateCondition>();

                cfg.CreateMap<CandidateMedicationsDto, DAL.CandidateMedication>();

                cfg.CreateMap<SSOAttributeMappingDto, DAL.SSOAttributeMapping>();

                cfg.CreateMap<Drug_FrequencyDto, DAL.Drug_Frequency>();

                cfg.CreateMap<DAL.Drug_Frequency, Drug_FrequencyDto>();

                cfg.CreateMap<Drug_FormulationDto, DAL.Drug_Formulation>();

                cfg.CreateMap<DAL.Mail, EmailDto>();

                cfg.CreateMap<EmailDto, DAL.Mail>();

                cfg.CreateMap<DAL.UserDrug, UserDrugDto>();

                cfg.CreateMap<UserDrugDto, DAL.UserDrug>();

                cfg.CreateMap<DAL.UserDrugAllergy, UserDrugAllergyDto>();

                cfg.CreateMap<UserDrugAllergyDto, DAL.UserDrugAllergy>();

                cfg.CreateMap<DAL.Drug_Formulation, Drug_FormulationDto>();

                cfg.CreateMap<Drug_ConditionDto, DAL.Drug_Condition>();

                cfg.CreateMap<DAL.Drug_Condition, Drug_ConditionDto>();

                cfg.CreateMap<Drug_ProductDto, DAL.Drug_Products>();

                cfg.CreateMap<DAL.Drug_Products, Drug_ProductDto>();

                cfg.CreateMap<DAL.Drug_Duration, Drug_DurationDto>();

                cfg.CreateMap<DAL.Drug_Allergy, Drug_AllergyDto>();

                cfg.CreateMap<Drug_AllergyDto, DAL.Drug_Allergy>();

                cfg.CreateMap<ScreeningDataErrorLogDto, DAL.ScreeningDataErrorLog>();

                cfg.CreateMap<DAL.ScreeningDataErrorLog, ScreeningDataErrorLogDto>();

                cfg.CreateMap<DAL.ListUsers_Result, ListUsers_ResultsDto>();

                cfg.CreateMap<ListUsers_ResultsDto, DAL.ListUsers_Result>();

                cfg.CreateMap<DAL.ParticipantProfile_Result, ParticipantProfile_ResultDto>();

                cfg.CreateMap<ParticipantProfile_ResultDto, DAL.ParticipantProfile_Result>();

                cfg.CreateMap<KitsPortalReport_ResultDto, DAL.KitsPortalReport_Result>();

                cfg.CreateMap<DAL.KitsPortalReport_Result, KitsPortalReport_ResultDto>();

                cfg.CreateMap<Drug_IngredientsDto, DAL.Drug_Ingredients>();

                cfg.CreateMap<DAL.Drug_Ingredients, Drug_IngredientsDto>();

                cfg.CreateMap<Drug_ProductsFormDto, DAL.Drug_ProductsForm>();

                cfg.CreateMap<DAL.Drug_ProductsForm, Drug_ProductsFormDto>();

                cfg.CreateMap<LabErrorLogDto, DAL.LabErrorLog>();

                cfg.CreateMap<DAL.LabErrorLog, LabErrorLogDto>();
                cfg.CreateMap<Drug_Products_FDADto, DAL.Drug_Products_FDA>();

                cfg.CreateMap<DAL.Drug_Products_FDA, Drug_Products_FDADto>();

                cfg.CreateMap<ListLogReportDto, DAL.Log>();

                cfg.CreateMap<VitalsLogDto, DAL.VitalsLog>();

                cfg.CreateMap<DAL.VitalsLog, VitalsLogDto>();

                cfg.CreateMap<CoachTrackingReport_ResultDto, DAL.CoachTrackingReport_Result>();

                cfg.CreateMap<DAL.CoachTrackingReport_Result, CoachTrackingReport_ResultDto>();

                cfg.CreateMap<ListEligibilityResultDto, DAL.ListEligibilityResult>();

                cfg.CreateMap<DAL.ListEligibilityResult, ListEligibilityResultDto>();

                cfg.CreateMap<DAL.Log, ListLogReportDto>();
                cfg.CreateMap<DAL.EligibilityImportLog, EligibilityImportLogDto>();
                cfg.CreateMap<DAL.PaymentTransaction, PaymentTransactionDto>();


                cfg.CreateMap<SSOAttributeMappingDto, DAL.SSOAttributeMapping>();
                cfg.CreateMap<SSOProviderDto, DAL.SSOProvider>()
                .ForMember(dto => dto.SSOAttributeMappings, opt => opt.MapFrom(src => src.SSOAttributeMappings));

                cfg.CreateMap<DAL.SSOAttributeMapping, SSOAttributeMappingDto>();
                cfg.CreateMap<DAL.SSOProvider, SSOProviderDto>()
                .ForMember(dal => dal.SSOAttributeMappings, opt => opt.MapFrom(src => src.SSOAttributeMappings));

                cfg.CreateMap<EXT_WeightDto, DAL.EXT_Weights>();
                cfg.CreateMap<EXT_BloodPressureDto, DAL.EXT_BloodPressures>();
                cfg.CreateMap<EXT_NutritionDto, DAL.EXT_Nutrition>();
                cfg.CreateMap<EXT_SleepsDto, DAL.EXT_Sleeps>();
                cfg.CreateMap<EXT_WorkoutsDto, DAL.EXT_Workouts>();

                cfg.CreateMap<DAL.EXT_Weights, EXT_WeightDto>();
                cfg.CreateMap<DAL.EXT_BloodPressures, EXT_BloodPressureDto>();
                cfg.CreateMap<DAL.EXT_Nutrition, EXT_NutritionDto>();
                cfg.CreateMap<DAL.EXT_Sleeps, EXT_SleepsDto>();
                cfg.CreateMap<DAL.EXT_Workouts, EXT_WorkoutsDto>();

                cfg.CreateMap<DAL.CanriskQuestionnaire, CanriskQuestionnaireDto>();

                cfg.CreateMap<DAL.Languages, LanguagesDto>();

                cfg.CreateMap<CanriskQuestionnaireDto, DAL.CanriskQuestionnaire>();

                cfg.CreateMap<LanguagesDto, DAL.Languages>();

                cfg.CreateMap<DAL.Livongo_Result, Livongo_ResultDto>();

                cfg.CreateMap<DAL.Newsletter, NewsletterDto>();

                cfg.CreateMap<DAL.AssignedNewsletter, AssignedNewsletterDto>();

                cfg.CreateMap<MessageDto, DAL.Message>();

                cfg.CreateMap<MessageRecipientDto, DAL.MessageRecipient>();

                cfg.CreateMap<DAL.Message, MessageDto>()
                    .ForMember(dto => dto.MessageRecipients, opt => opt.MapFrom(src => src.MessageRecipients));

                cfg.CreateMap<DAL.MessageRecipient, MessageRecipientDto>();

                cfg.CreateMap<DAL.NotificationTemplateTranslation, NotificationTemplateTranslationDto>();

                cfg.CreateMap<NotificationTemplateTranslationDto, DAL.NotificationTemplateTranslation>();

                cfg.CreateMap<DAL.HRA_ExercisePlan, ExercisePlanDto>();

                cfg.CreateMap<WeeklyGoalsDto, DAL.WorkoutGoals>();

                cfg.CreateMap<DAL.EXT_Summaries, EXT_SummariesDto>();

                cfg.CreateMap<DAL.CRM_AccountType, CRM_AccountTypesDto>();

                cfg.CreateMap<DAL.CRM_CallerProfileType, CRM_CallerProfileTypesDto>();

                cfg.CreateMap<DAL.InquiryType, InquiryTypeDto>();

                cfg.CreateMap<DAL.CRM_ComplaintClassificationType, CRM_ComplaintClassificationTypesDto>();

                cfg.CreateMap<DAL.CRM_Contact, CRM_ContactDto>();

                cfg.CreateMap<CRM_ContactDto, DAL.CRM_Contact>();

                cfg.CreateMap<DAL.CRM_Note, CRM_NoteDto>();

                cfg.CreateMap<CRM_NoteDto, DAL.CRM_Note>();

                cfg.CreateMap<DAL.CRM_ChangeLog, CRM_ChangeLogDto>();

                cfg.CreateMap<CRM_ChangeLogDto, DAL.CRM_ChangeLog>();

                cfg.CreateMap<DAL.CRM_Disposition, CRM_DispositionsDto>();

                cfg.CreateMap<DAL.CRM_PogoMeterNumbers, CRM_PogoMeterNumbersDto>();

                cfg.CreateMap<CRM_PogoMeterNumbersDto, DAL.CRM_PogoMeterNumbers>();

                cfg.CreateMap<HCPListsDto, DAL.HCPList>();

                cfg.CreateMap<DAL.HCPList, HCPListsDto>();

                cfg.CreateMap<DAL.InsuranceType, InsuranceTypesDto>();

                cfg.CreateMap<DAL.Department, DepartmentDto>();

                cfg.CreateMap<DepartmentDto, DAL.Department>();

                cfg.CreateMap<DAL.BusinessUnit, BusinessUnitDto>();

                cfg.CreateMap<DAL.Webinar, WebinarDto>();

                cfg.CreateMap<WebinarDto, DAL.Webinar>();

                cfg.CreateMap<DAL.OrganizationsforWebinar, OrganizationsforWebinarDto>();

                cfg.CreateMap<OrganizationsforWebinarDto, DAL.OrganizationsforWebinar>();

                cfg.CreateMap<DAL.AccessTokens, AccessTokensDto>();

                cfg.CreateMap<AccessTokensDto, DAL.AccessTokens>();

                cfg.CreateMap<CoachingConditionsDto, DAL.CoachingConditions>();

                cfg.CreateMap<DAL.CoachingConditions, CoachingConditionsDto>();

                cfg.CreateMap<LabProcedureDto, DAL.LabProcedure>();

                cfg.CreateMap<DAL.LabProcedure, LabProcedureDto>();

                cfg.CreateMap<PortalCoachingConditionsDto, DAL.PortalCoachingConditions>();

                cfg.CreateMap<DAL.PortalCoachingConditions, PortalCoachingConditionsDto>();

                cfg.CreateMap<PortalLabProcedureDto, DAL.PortalLabProcedure>();

                cfg.CreateMap<DAL.PortalLabProcedure, PortalLabProcedureDto>();

                cfg.CreateMap<ReportFeedbackDto, DAL.ReportFeedback>();

                cfg.CreateMap<DAL.ReportFeedback, ReportFeedbackDto>();

                cfg.CreateMap<CarePlanReportDto, DAL.CarePlanReport>();

                cfg.CreateMap<DAL.CarePlanReport, CarePlanReportDto>();

                cfg.CreateMap<InsuranceSummaryDto, DAL.InsuranceSummary>();

                cfg.CreateMap<DAL.InsuranceSummary, InsuranceSummaryDto>();

                cfg.CreateMap<DAL.CandidateReasonForLastChange, CandidateReasonForLastChangeDto>();

                cfg.CreateMap<CandidateReasonForLastChangeDto, DAL.CandidateReasonForLastChange>();

                cfg.CreateMap<DAL.ClaimConditionCode, ClaimConditionCodeDto>();

                cfg.CreateMap<ClaimConditionCodeDto, DAL.ClaimConditionCode>();

                cfg.CreateMap<DAL.CandidateMedication, CandidateMedicationsDto>();

                cfg.CreateMap<CandidateMedicationsDto, DAL.CandidateMedication>();

                cfg.CreateMap<DAL.CandidateCondition, CandidateConditionsDto>();

                cfg.CreateMap<CandidateConditionsDto, DAL.CandidateCondition>();

                cfg.CreateMap<ListSearchUsers_ResultsDto, DAL.ListSearchUsers_Result>();

                cfg.CreateMap<DAL.ListSearchUsers_Result, ListSearchUsers_ResultsDto>();

                cfg.CreateMap<ContactRequirementDto, DAL.ContactRequirement>();

                cfg.CreateMap<DAL.ContactRequirement, ContactRequirementDto>();

                cfg.CreateMap<ContactRequirementsAlertDto, DAL.ContactRequirementsAlert>();

                cfg.CreateMap<DAL.ContactRequirementsAlert, ContactRequirementsAlertDto>();

                cfg.CreateMap<InvoiceDetailsDto, DAL.InvoiceDetail>();

                cfg.CreateMap<DAL.InvoiceDetail, InvoiceDetailsDto>();

                cfg.CreateMap<BillingServiceTypeDto, DAL.BillingServiceType>();

                cfg.CreateMap<DAL.BillingServiceType, BillingServiceTypeDto>();

                cfg.CreateMap<InvoiceBilledDetailsDto, DAL.InvoiceBilledDetail>();

                cfg.CreateMap<DAL.InvoiceBilledDetail, InvoiceBilledDetailsDto>();

                cfg.CreateMap<UserTimeTrackerDto, DAL.UserTimeTracker>();

                cfg.CreateMap<DAL.UserTimeTracker, UserTimeTrackerDto>();

                cfg.CreateMap<TimeTrackerDispositionDto, DAL.TimeTrackerDisposition>();

                cfg.CreateMap<DAL.TimeTrackerDisposition, TimeTrackerDispositionDto>();

                cfg.CreateMap<ExternalReportsDto, DAL.ExternalReport>();

                cfg.CreateMap<DAL.ExternalReport, ExternalReportsDto>();

                cfg.CreateMap<BillingNotesDto, DAL.BillingNote>();

                cfg.CreateMap<DAL.BillingNote, BillingNotesDto>();

                cfg.CreateMap<TeamsBP_PPRDto, DAL.TeamsBP_PPR>();

                cfg.CreateMap<DAL.TeamsBP_PPR, TeamsBP_PPRDto>();

                cfg.CreateMap<BillingNotesDto, DAL.BillingNote>();

                cfg.CreateMap<DAL.BillingNote, BillingNotesDto>();

                cfg.CreateMap<UserLoggedInDevicesDto, DAL.UserLoggedInDevice>();

                cfg.CreateMap<DAL.UserLoggedInDevice, UserLoggedInDevicesDto>();

                cfg.CreateMap<ReportTypeDto, DAL.ReportType>();

                cfg.CreateMap<DAL.ReportType, ReportTypeDto>();

                cfg.CreateMap<FaxedReportsDto, DAL.FaxedReport>();

                cfg.CreateMap<DAL.FaxedReport, FaxedReportsDto>();

                cfg.CreateMap<ProviderDto, DAL.Provider>();

                cfg.CreateMap<DAL.Provider, ProviderDto>();

                cfg.CreateMap<DAL.NotificationCategory, NotificationCategoryDto>();

                cfg.CreateMap<NotificationCategoryDto, DAL.NotificationCategory>();

                cfg.CreateMap<DAL.HealthData, HealthDataDto>();

                cfg.CreateMap<HealthDataDto, DAL.HealthData>();

                cfg.CreateMap<DAL.MotivationMessage, MotivationMessagesDto>();

                cfg.CreateMap<DAL.AssignedMotivationMessage, AssignedMotivationMessageDto>();

                cfg.CreateMap<DAL.WebinarOccurrence, WebinarOccurrenceDto>();

                cfg.CreateMap<WebinarOccurrenceDto, DAL.WebinarOccurrence>();

                cfg.CreateMap<DAL.RegisteredUsersforWebinar, RegisteredUsersforWebinarDto>();

                cfg.CreateMap<RegisteredUsersforWebinarDto, DAL.RegisteredUsersforWebinar>();

                cfg.CreateMap<DAL.UserWearableDevice, UserWearableDeviceDto>();

                cfg.CreateMap<UserWearableDeviceDto, DAL.UserWearableDevice>();

                cfg.CreateMap<DAL.WearableDevice, WearableDeviceDto>();

                cfg.CreateMap<WearableDeviceDto, DAL.WearableDevice>();

            });

            mapper = config.CreateMapper();
        }

        public static void BulkInsert(string tableName, IEnumerable<object> collection)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SomeConnectionString"].ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = "dbo." + tableName;
                    try
                    {
                        bulkCopy.WriteToServer(collection.AsDataTable());
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        connection.Close();
                    }
                }

                transaction.Commit();
            }
        }
    }
}