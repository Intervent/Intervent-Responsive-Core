using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class FollowUpUtility
    {
        public static FollowUpResponse CreateFollowUp(int FollowUpId, int userinProgramId, int userId)
        {
            FollowUpDto dto = new FollowUpDto();
            dto.UsersinProgramsId = userinProgramId;
            dto.CreatedBy = userId;
            dto.Id = FollowUpId;
            FollowUpReader reader = new FollowUpReader();
            var followUp = reader.CreateFollowUp(dto);
            return followUp;
        }

        public static FollowUpResponse ReadFollowUp(int followupid)
        {
            FollowUpReader reader = new FollowUpReader();
            ReadFollowUpRequest request = new ReadFollowUpRequest();
            request.FollowUpId = followupid;
            return reader.ReadFollowUp(request);
        }

        public static FollowUpResponse AddEditMedicalCondition(FollowUp_MedicalConditionsDto model, int? integrationWith, string organizationCode, string uniqueId, int followUpId, int userId, int participantId, int participantPortalId)
        {
            AddEditFUMedicalConditionsRequest request = new AddEditFUMedicalConditionsRequest();
            if (integrationWith.HasValue && integrationWith == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(participantId))
            {
                request.IsIntuityUser = true;
                request.OrganizationCode = organizationCode;
                request.UniqueId = uniqueId;
            }
            request.medicalCondition = model;
            request.medicalCondition.Id = followUpId;
            FollowUpReader reader = new FollowUpReader();
            request.ParticipantId = participantId;
            request.UserId = userId;
            request.PortalId = participantPortalId;
            return reader.AddEditMedicalCondition(request);

        }

        public static FollowUpMedicalConditionsResponse ReadMedicalCondition(int followupid, int userinProgramId)
        {
            FollowUpReader reader = new FollowUpReader();
            ReadFollowUpRequest request = new ReadFollowUpRequest();
            request.FollowUpId = followupid;
            request.UsersInProgramsId = userinProgramId;
            return reader.ReadMedicalCondition(request);
        }

        public static FollowUpResponse AddEditOtherRisks(FollowUp_OtherRiskFactorsDto model, int? integrationWith, string organizationCode, string uniqueId, int followUpId, int userId, int participantId, int participantPortalId)
        {
            AddEditFUOtherRisksRequest request = new AddEditFUOtherRisksRequest();
            if (integrationWith.HasValue && integrationWith == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(participantId))
            {
                request.IsIntuityUser = true;
                request.OrganizationCode = organizationCode;
                request.UniqueId = uniqueId;
            }
            request.otherRiskFactors = model;
            request.otherRiskFactors.Id = followUpId;
            FollowUpReader reader = new FollowUpReader();
            request.ParticipantId = participantId;
            request.UserId = userId;
            request.PortalId = participantPortalId;
            return reader.AddEditOtherRiskFactors(request);
        }

        public static ReadFollowUpOtherRiskFactorsResponse ReadOtherRisk(int followupid, int hraId)
        {
            ReadFollowUpOtherRiskFactorsResponse response = new ReadFollowUpOtherRiskFactorsResponse();
            ReadFollowUpRequest request = new ReadFollowUpRequest();
            request.FollowUpId = followupid;
            FollowUpReader reader = new FollowUpReader();
            if (followupid > 0)
                response.otherRisks = reader.ReadOtherRiskFactor(request).otherRisks;
            HRAReader hraReader = new HRAReader();
            ReadHRARequest hraRequest = new ReadHRARequest();
            hraRequest.hraId = hraId;
            response.HRA_otherRisks = hraReader.ReadHRA(hraRequest).hra.OtherRiskFactors;
            return response;
        }

        public static FollowUpResponse AddEditHealthConditions(FollowUp_HealthConditionsDto model, int? integrationWith, string organizationCode, string uniqueId, int followUpId, int userId, int participantId, int participantPortalId)
        {
            AddEditFUHealthConditionRequest request = new AddEditFUHealthConditionRequest();
            if (integrationWith.HasValue && integrationWith == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(participantId))
            {
                request.IsIntuityUser = true;
                request.OrganizationCode = organizationCode;
                request.UniqueId = uniqueId;
            }
            request.healthConditionsDto = model;
            request.healthConditionsDto.Id = followUpId;
            FollowUpReader reader = new FollowUpReader();
            request.ParticipantId = participantId;
            request.UserId = userId;
            request.PortalId = participantPortalId;
            return reader.AddEditHealthConditions(request);
        }

        public static FollowUpHealthConditionsResponse ReadHealthConditions(int followupid)
        {
            FollowUpReader reader = new FollowUpReader();
            ReadFollowUpRequest request = new ReadFollowUpRequest();
            request.FollowUpId = followupid;
            return reader.ReadHealthCondition(request);
        }

        public static FollowUpResponse AddEditHealthNumbers(FollowUp_HealthNumbersDto model, bool bloodwork, int? integrationWith, string organizationCode, string uniqueId, int followUpId, int userId, int participantId, int participantPortalId)
        {
            AddEditFUHealthNumbersRequest request = new AddEditFUHealthNumbersRequest();
            if (integrationWith.HasValue && integrationWith.Value == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(participantId))
            {
                request.IsIntuityUser = true;
                request.OrganizationCode = organizationCode;
                request.UniqueId = uniqueId;
            }
            request.bloodwork = bloodwork;

            request.healthNumbersDto = model;
            if (request.healthNumbersDto.Height != null && request.healthNumbersDto.Weight != null)
            {
                request.healthNumbersDto.BMI = (float)CommonReader.GetBMI(request.healthNumbersDto.Height.Value, request.healthNumbersDto.Weight.Value);
            }
            request.healthNumbersDto.Id = followUpId;
            FollowUpReader reader = new FollowUpReader();
            request.ParticipantId = participantId;
            request.UserId = userId;
            request.PortalId = participantPortalId;
            return reader.AddEditHealthNumbers(request);
        }

        public static ReadFUHealthNumberResponse ReadFUHealthNumber(int followupid)
        {
            FollowUpReader reader = new FollowUpReader();
            return reader.ReadFUHealthNumber(new ReadFUHealthNumberRequest { followupId = followupid });
        }

        public static GetAllFollowUpsResponse GetAllFollowUps(int participantId)
        {
            FollowUpReader reader = new FollowUpReader();
            GetAllFollowUpsRequest request = new GetAllFollowUpsRequest();
            request.UserId = participantId;
            return reader.GetAllFollowUps(request);
        }

        public static CheckFollowupValidityResponse CheckFollowupValidity(int usersInProgramsId, DateTime bloodTestDate)
        {
            FollowUpReader reader = new FollowUpReader();
            return reader.CheckFollowupValidity(usersInProgramsId, null, bloodTestDate);
        }

        public static int GetFolowUpCompletionPercent(string followupPageSeqDone)
        {
            string[] followUpPageSeq = Intervent.Web.DTO.Constants.FollowUpPageSeq;
            string followupPageString = string.Join(".", followUpPageSeq);
            if (followupPageString == followupPageSeqDone)
                return 100;
            if (String.IsNullOrEmpty(followupPageSeqDone))
                return 0;

            decimal pageSeq = followUpPageSeq.Length;

            decimal pageSeqDone = followupPageSeqDone.Split(new Char[] { '.' }).Length - 1;

            return Convert.ToInt32((pageSeqDone / pageSeq) * 100);
        }
    }
}