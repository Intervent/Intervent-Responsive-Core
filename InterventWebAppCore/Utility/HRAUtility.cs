using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class HRAUtility
    {
        public static int CreateHRA(int userId, int participantId, int systemAdminId, int? userinProgramId, int? portalId, bool upload)
        {
            CreateHRARequest request = new CreateHRARequest();
            HRADto HA = new HRADto();
            HA.UserId = userId;
            HA.PortalId = portalId.Value;
            request.HRA = HA;
            request.languageCode = Translate.GetCurrentLanguage();
            if (!upload)
            {
                if (participantId != userId)
                    HA.CreatedBy = userId;
                if (userinProgramId.HasValue)
                    request.UserinProgramId = userinProgramId.Value;
            }
            else
            {
                ProgramReader programReader = new ProgramReader();
                var usersinProgram = programReader.GetUserProgramsByPortal(new GetUserProgramHistoryRequest { portalId = portalId.Value, userId = userId }).Where(x => x.IsActive).FirstOrDefault();
                if (usersinProgram != null)
                    request.UserinProgramId = usersinProgram.Id;
                HA.CreatedBy = systemAdminId;
            }
            HRAReader reader = new HRAReader();
            return reader.CreateHRA(request).HRAId;
        }

        public static GetAllHRAsforUserResponse GetAllHRAs(int participantId)
        {
            HRAReader reader = new HRAReader();
            GetAllHRAsforUserRequest request = new GetAllHRAsforUserRequest();
            request.UserId = participantId;
            request.IncludeInactivePortalHRAs = true;
            return reader.GetAllHRAsforUser(request);
        }
        public static ReadHRAResponse ReadHRA(int hraid)
        {
            HRAReader reader = new HRAReader();
            ReadHRARequest request = new ReadHRARequest();
            request.hraId = hraid;
            return reader.ReadHRA(request);
        }

        public static ReadHRAResponse ReadHRAByPortal(int userId, int portalId)
        {
            HRAReader reader = new HRAReader();
            ReadHRARequest request = new ReadHRARequest();
            request.userId = userId;
            request.portalId = portalId;
            return reader.ReadHRAByPortal(request);
        }

        public static HRADto AddEditMedicalCondition(MedicalConditionModel model)
        {
            AddEditMedicalConditionsRequest request = new AddEditMedicalConditionsRequest();

            MedicalConditionsDto medicalCondition = new MedicalConditionsDto();
            if (model.integrationWith.HasValue && model.integrationWith.Value == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(model.participantId))
                request.IsIntuityUser = true;
            if (!model.hraId.HasValue)
                model.hraId = CreateHRA(model.userId, model.participantId, model.systemAdminId, model.userinProgramId, model.participantPortalId, false);

            medicalCondition = model.medicalConditions;
            medicalCondition.HRAId = model.hraId.Value;
            request.medicalCondition = medicalCondition;
            //Update core hra info
            request.medicalCondition.HRA = UpdateHRADetails("MC.", model.hraCompleteDate, model.hraPageSeqDone, model.hraPageSeq);

            //track user changes
            request.UserId = model.userId;
            request.UpdatedByUserId = model.userId;
            request.StoreHistory = true;

            HRAReader reader = new HRAReader();
            var response = reader.AddEditMedicalCondition(request);

            return request.medicalCondition.HRA;
        }

        public static HRADto AddEditOtherRisks(OtherRisksModel model)
        {
            OtherRiskFactorsDto OtherRiskFactors = new OtherRiskFactorsDto();

            if (!model.hraId.HasValue)
                model.hraId = CreateHRA(model.userId, model.participantId, model.systemAdminId, model.userinProgramId, model.participantPortalId, false);

            OtherRiskFactors = model.otherRisks;
            OtherRiskFactors.HRAId = model.hraId.Value;

            AddEditOtherRiskFactorsRequest request = new AddEditOtherRiskFactorsRequest();
            if (model.integrationWith.HasValue && model.integrationWith == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(model.participantId))
                request.IsIntuityUser = true;
            request.OtherRiskFactors = OtherRiskFactors;

            //Update core hra info
            request.OtherRiskFactors.HRA = UpdateHRADetails("OR.", model.hraCompleteDate, model.hraPageSeqDone, model.hraPageSeq);
            //track user changes
            request.UserId = model.userId;
            request.UpdatedByUserId = model.userId;
            request.StoreHistory = true;

            HRAReader reader = new HRAReader();
            bool isSmoker = false;
            if (request.OtherRiskFactors.SmokeCig == 1 || request.OtherRiskFactors.OtherTobacco == 1 || request.OtherRiskFactors.ECig == 1)
                isSmoker = true;
            reader.AddEditSmokeDependentQuestions(new AddEditSmokeDependentQuestions
            {
                HRAId = model.hraId.Value,
                isSmoker = isSmoker,
                StoreHistory = true,
                UpdatedByUserId = model.userId,
                UserId = model.userId
            });

            reader.AddEditOtherRisks(request);
            return request.OtherRiskFactors.HRA;
        }

        public static HRADto AddEditHSP(HSPModel model)
        {
            AddEditHSPRequest request = new AddEditHSPRequest();
            if (model.integrationWith.HasValue && model.integrationWith == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(model.participantId))
                request.IsIntuityUser = true;
            HSPDto HSP = new HSPDto();

            if (!model.hraId.HasValue)
                model.hraId = CreateHRA(model.userId, model.participantId, model.systemAdminId, model.userinProgramId, model.participantPortalId, false);

            HSP = model.hsp;
            HSP.HRAId = model.hraId.Value;
            request.HSP = HSP;
            //Update core hra info
            request.HSP.HRA = UpdateHRADetails("YL.", model.hraCompleteDate, model.hraPageSeqDone, model.hraPageSeq);

            //track user changes
            request.UserId = model.userId;
            request.UpdatedByUserId = model.userId;
            request.StoreHistory = true;


            HRAReader reader = new HRAReader();
            reader.AddEditHSP(request);

            return request.HSP.HRA;
        }

        public static HRADto AddEditExams(ExamsModel model)
        {
            ExamsandShotsDto exams = new ExamsandShotsDto();

            if (!model.hraId.HasValue)
                model.hraId = CreateHRA(model.userId, model.participantId, model.systemAdminId, model.userinProgramId, model.participantPortalId, false);

            exams.HRAId = model.hraId.Value;
            exams.PhysicalExam = model.PhysicalExamBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.StoolTest = model.StoolTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.SigTest = model.SigTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.ColStoolTest = model.ColStoolTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.ColTest = model.ColTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.PSATest = model.PSATestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.PapTest = model.PapTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.BoneTest = model.BoneTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.Mammogram = model.MammogramBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.DentalExam = model.DentalExamBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.BPCheck = model.BPCheckBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.CholTest = model.CholTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.GlucoseTest = model.GlucoseTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.EyeExam = model.EyeExamBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.NoTest = model.NoTestBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.TetanusShot = model.TetanusShotBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.FluShot = model.FluShotBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.MMR = model.MMRBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.Varicella = model.VaricellaBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.HepBShot = model.HepBShotBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.ShinglesShot = model.ShinglesShotBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.HPVShot = model.HPVShotBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.PneumoniaShot = model.PneumoniaShotBool == true ? Convert.ToByte(1) : Convert.ToByte(2);
            exams.NoShots = model.NoShotsBool == true ? Convert.ToByte(1) : Convert.ToByte(2);

            AddEditExamsandShotsRequest request = new AddEditExamsandShotsRequest();
            if (model.integrationWith.HasValue && model.integrationWith.Value == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(model.participantId))
                request.IsIntuityUser = true;
            request.exams = exams;

            //Update core hra info
            request.exams.HRA = UpdateHRADetails("EC.", model.hraCompleteDate, model.hraPageSeqDone, model.hraPageSeq);

            //track user changes
            request.UserId = model.userId;
            request.UpdatedByUserId = model.userId;
            request.StoreHistory = true;

            HRAReader reader = new HRAReader();
            reader.AddEditExams(request);

            return request.exams.HRA;
        }

        internal static HRADto AddEditInterest(InterestModel model)
        {
            AddEditInterestsRequest request = new AddEditInterestsRequest();
            if (model.integrationWith.HasValue && model.integrationWith.Value == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(model.participantId))
                request.IsIntuityUser = true;
            InterestsDto interest = new InterestsDto();
            if (!model.hraId.HasValue)
                model.hraId = CreateHRA(model.userId, model.participantId, model.systemAdminId, model.userinProgramId, model.participantPortalId, false);

            interest = model.interests;
            interest.HRAId = model.hraId.Value;
            request.interest = interest;
            //Update core hra info
            request.interest.HRA = UpdateHRADetails("IN.", model.hraCompleteDate, model.hraPageSeqDone, model.hraPageSeq);

            //track user changes
            request.UserId = model.userId;
            request.UpdatedByUserId = model.userId;
            request.StoreHistory = true;

            HRAReader reader = new HRAReader();
            reader.AddEditInterest(request);

            return request.interest.HRA;
        }

        public static HRADto AddEditHealthNumbers(HealthNumbersModel model, bool bloodwork)
        {
            AddEditHealthNumbersRequest request = new AddEditHealthNumbersRequest();
            if (model.integrationWith.HasValue && model.integrationWith.Value == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(model.participantId))
                request.IsIntuityUser = true;
            HealthNumbersDto numbers = new HealthNumbersDto();
            numbers = model.HealthNumbers;
            if (!string.IsNullOrEmpty(model.BloodTestDate))
                numbers.BloodTestDate = Convert.ToDateTime(model.BloodTestDate);
            if (!numbers.Height.HasValue && !bloodwork)
            {
                numbers.Height = (model.HeightFt * 12);
                if (model.HeightInch.HasValue && model.HeightInch.Value > 0)
                    numbers.Height = model.HeightInch + numbers.Height;
            }
            request.bloodwork = bloodwork;
            request.HealthNumbers = numbers;
            request.addtoWellnessData = true;
            //track user changes
            request.StoreHistory = true;
            request.UpdatedByUserId = model.userId;
            request.UserId = model.userId;
            if (!model.hraId.HasValue)
                model.hraId = CreateHRA(model.userId, model.participantId, model.systemAdminId, model.userinProgramId, model.participantPortalId, false);
            numbers.HRAId = model.hraId.Value;
            request.HealthNumbers.HRA = UpdateHRADetails("YN.", model.hraCompleteDate, model.hraPageSeqDone, model.hraPageSeq);
            request.HealthNumbers.HRA.UserId = model.participantId;
            HRAReader reader = new HRAReader();
            reader.AddEditHealthNumbers(request);

            return request.HealthNumbers.HRA;
        }

        public static HRADto UpdateHRADetails(string page, DateTime? hraCompleteDate, string hraPageSeqDone, string hraPageSeq)
        {
            HRADto HRA = new HRADto();
            if (hraCompleteDate.HasValue)
            {
                HRA.CompleteDate = hraCompleteDate;
            }
            else
            {
                if (hraPageSeqDone == null || (hraPageSeqDone != null && !hraPageSeqDone.Contains(page)))
                {
                    string pageSeqString = hraPageSeq;

                    if (hraPageSeqDone != null)
                    {
                        HRA.HAPageSeqDone = hraPageSeqDone.ToString() + page;
                    }
                    else
                        HRA.HAPageSeqDone = page;

                    string[] pageSeqArray = pageSeqString.Split('.').Take(pageSeqString.Split('.').Length - 1).ToArray();

                    Array.Sort(pageSeqArray);

                    string[] pageSeqDoneArray = HRA.HAPageSeqDone.Split('.').Take(HRA.HAPageSeqDone.Split('.').Length - 1).ToArray();

                    Array.Sort(pageSeqDoneArray);

                    if (pageSeqArray.SequenceEqual(pageSeqDoneArray))
                        HRA.CompleteDate = DateTime.UtcNow;
                }
            }
            return HRA;
        }

        public static int GetHRACompletionPercent(string HRAPageSeq, string HRAPageSeqDone)
        {
            if (HRAPageSeq == HRAPageSeqDone)
                return 100;
            if (string.IsNullOrEmpty(HRAPageSeqDone))
                return 0;

            decimal pageSeq = HRAPageSeq.Split(new Char[] { '.' }).Length - 1;

            decimal pageSeqDone = HRAPageSeqDone.Split(new Char[] { '.' }).Length - 1;

            return Convert.ToInt32((pageSeqDone / pageSeq) * 100);
        }

        public static ChangeTelHRAResponse ChangeTelHRARequest(ChangeTelHRARequest request)
        {
            HRAReader reader = new HRAReader();
            return reader.ChangeTelHRA(request);
        }

        public static ValidateStratificationResponse IsValidStratificationWindow(ValidateStratificationRequest request)
        {
            HRAReader reader = new HRAReader();
            return reader.IsValidStratificationWindow(request);
        }

        public static bool hasCompletedHealthNumbers(HealthNumbersDto dto = null, LabDto lab = null, FollowUp_HealthNumbersDto followUpDto = null)
        {
            if (dto == null && lab != null)
            {
                dto = new HealthNumbersDto();
                dto.BloodTestDate = lab.BloodTestDate;
                dto.TotalChol = lab.TotalChol;
                dto.Trig = lab.Trig;
                dto.Glucose = lab.Glucose;
                dto.HDL = lab.HDL;
                dto.LDL = lab.LDL;
                dto.A1C = lab.A1C;
            }
            else if (dto == null && followUpDto != null)
            {
                dto = new HealthNumbersDto();
                dto.BloodTestDate = followUpDto.BloodTestDate;
                dto.TotalChol = followUpDto.TotalChol;
                dto.Trig = followUpDto.Trig;
                dto.Glucose = followUpDto.Glucose;
                dto.HDL = followUpDto.HDL;
                dto.LDL = followUpDto.LDL;
                dto.A1C = followUpDto.A1C;
            }

            HRAReader reader = new HRAReader();
            return reader.hasCompletedHealthNumbers(dto);
        }
    }
}