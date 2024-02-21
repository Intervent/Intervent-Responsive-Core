using Intervent.Business.Notification;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using NLog.Fluent;
using System.Configuration;
using System.Text;
using static Intervent.Web.DTO.ListOptions;

namespace Intervent.Business
{
    public class LabManager
    {
        const int LMCPortalId = 46;
        public int ProcessDynacareLabs()
        {
            List<LabResponse> labResponse = new List<LabResponse>();
            var filepath = ConfigurationManager.AppSettings["DynacareInboundFilePath"].ToString();
            if (string.IsNullOrEmpty(filepath))
            {
                string extraMessage = "Folder path is empty";
                Log.Warn(extraMessage);
            }
            var count = 0;
            try
            {
                var fileNames = Directory.GetFiles(filepath);
                if (fileNames.Count() == 0)
                {
                    Log.Debug($"No files found.");
                    return count;
                }
                string directoryName = "ARCHIVES";
                string archiveDirectoryPath = Path.Combine(filepath, directoryName);
                if (!Directory.Exists(archiveDirectoryPath))
                {
                    Directory.CreateDirectory(archiveDirectoryPath);
                }
                foreach (var file in fileNames)
                {
                    string hl7Value = "";
                    byte[] binData = File.ReadAllBytes(file);
                    hl7Value = Encoding.UTF8.GetString(binData);
                    labResponse = Labcorp.LoadHL7File(hl7Value, "Dynacare");
                    count += labResponse.Count();
                    PostLabData(labResponse, file);
                    File.Move(file, Path.Combine(archiveDirectoryPath, Path.GetFileName(file)));
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "error occurred while processing labs: " + ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return count;
        }

        private void PostLabData(List<LabResponse> labResponse, string file)
        {
            try
            {
                LabReader labReader = new LabReader();
                ParticipantReader partReader = new ParticipantReader();
                PortalReader portalReader = new PortalReader();
                CommonReader commonReader = new CommonReader();
                AccountReader accountReader = new AccountReader();
                ParticipantReader participantReader = new ParticipantReader();
                var orderNumbers = labResponse.Where(x => !String.IsNullOrEmpty((x.OrderId))).Select(x => x.OrderId).ToList();
                var labsDto = labReader.GetLabsFromOrderNumber(orderNumbers);
                if (labsDto != null && labsDto.Count > 0)
                {
                    List<LabDto> OrdersToRemove = new List<LabDto>();
                    for (int i = 0; i < labsDto.Count; i++)
                    {
                        if (labsDto[i].BloodTestDate.HasValue)
                        {
                            var lab = labResponse.Where(x => x.OrderId == labsDto[i].OrderNo).FirstOrDefault();
                            if (lab.BloodTestDate.Value.Date != labsDto[i].BloodTestDate.Value.Date)
                                OrdersToRemove.Add(labsDto[i]);
                        }
                    }
                    for (int i = 0; i < OrdersToRemove.Count; i++)
                    {
                        labsDto.Remove(OrdersToRemove[i]);
                    }
                }
                var dtoOrders = labsDto.Select(y => y.OrderNo).ToList();
                if ((dtoOrders.Count == 0 && labResponse.Count != 0) || orderNumbers.Count() != dtoOrders.Count())
                {
                    var labs = labResponse.Where(x => !labsDto.Select(y => y.OrderNo).Contains(x.OrderId)).ToList().Select(x => new LabUserInfo { FirstName = x.FirstName, LastName = x.LastName, DOB = x.DOB }).ToList();
                    if (labs.Count > 0)
                    {
                        for (int i = 0; i < labs.Count; i++)
                        {
                            var labDto = accountReader.GetUserByBasicDetail(labs[i].FirstName, labs[i].LastName, labs[i].DOB.Value);
                            if (labDto != null)
                            {
                                labsDto.Add(labDto);
                                foreach (var lab in labResponse.Where(p => (String.IsNullOrEmpty(p.OrderId) || p.OrderId != labDto.OrderNo) && p.FirstName == labs[i].FirstName && p.LastName == labs[i].LastName && p.DOB == labs[i].DOB.Value))
                                {
                                    LogReader reader = new LogReader();
                                    string loggerName = "LabManager.PostLabData";
                                    var logEvent = LogEventInfo.Create(LogLevel.Info, loggerName, "Order mismatch: Order no. in the file: " + (String.IsNullOrEmpty(lab.OrderId) ? "NULL" : lab.OrderId) + "; Order no. in the system: " + labDto.OrderNo + ";");
                                    reader.WriteLogMessage(logEvent);
                                    lab.OrderId = labDto.OrderNo;
                                }
                            }
                            else
                            {
                                LabReader reader = new LabReader();
                                var Errors = new List<string>();
                                Errors.Add($"No matching Dynacare labs for the file: {Path.GetFileName(file)} Name: {labs[i].FirstName} {labs[i].LastName} DOB: {labs[i].DOB} ");
                                LogLabErrorsRequest request = new LogLabErrorsRequest();
                                request.Errors = Errors;
                                request.portalId = LMCPortalId;
                                reader.LogLabErrors(request);
                            }
                        }
                    }
                }
                if (labsDto.Count() == 0)
                {
                    return;
                }
                var portal = portalReader.ReadPortal(new Web.DTO.ReadPortalRequest { portalId = labsDto.FirstOrDefault().PortalId });
                foreach (var lab in labResponse)
                {
                    var labDto = labsDto.Where(x => x.OrderNo.ToUpper() == lab.OrderId.ToUpper()).FirstOrDefault();
                    bool? eligibility = null;
                    var user = accountReader.GetUserDetails(labDto.UserId);
                    var canRisk = participantReader.GetCanriskResponse(new GetCanriskRequest() { uniqueId = user.UniqueId });

                    var age = commonReader.GetAge(user.DOB.Value);
                    if (labDto.DateCompleted != null)
                    {
                        eligibility = CheckProgramCondition(age, labDto.A1C, labDto.Glucose, labDto.Weight, canRisk, labDto.DidYouFast, labDto.BMI).eligibility;
                    }
                    labDto.TotalChol = lab.TotalChol;
                    labDto.BPArm = lab.BPArm;
                    labDto.A1C = lab.A1C;
                    labDto.DBP = lab.DBP;
                    labDto.SBP = lab.SBP;
                    labDto.Glucose = lab.Glucose;
                    labDto.Trig = lab.Trig;
                    labDto.Height = lab.Height;
                    labDto.HeightCM = lab.Height;
                    labDto.Waist = lab.Waist;
                    labDto.Weight = lab.Weight;
                    labDto.HDL = lab.HDL;
                    labDto.LDL = lab.LDL;
                    labDto.BMI = lab.BMI;
                    labDto.DidYouFast = lab.DidYouFast;
                    labDto.BloodTestDate = lab.BloodTestDate;
                    labDto = ListOptions.CovertIntoImperial(labDto, commonReader.MeasurementRange());
                    if (labDto.Height.HasValue && labDto.Weight.HasValue && !labDto.BMI.HasValue)
                    {
                        labDto.BMI = (float)CommonReader.GetBMI(labDto.Height.Value, labDto.Weight.Value);
                    }
                    var userParticipation = participantReader.ReadUserParticipation(new ReadUserParticipationRequest { UserId = labDto.UserId });
                    var labRequest = new Web.DTO.AddLabRequest();
                    labRequest.Lab = labDto;
                    labRequest.SaveNew = true;
                    labRequest.Id = labDto.Id;
                    labRequest.HRAValidity = portal.portal.HRAValidity ?? 45;
                    labRequest.updatedBy = Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]);
                    if (userParticipation.usersinProgram != null)
                        labRequest.userinprogramId = userParticipation.usersinProgram.Id;
                    labRequest.overrideCurrentValue = true;
                    labReader.AddUpdateLab(labRequest);/*update it to system admin value */
                    var User = partReader.GetUsersforLabs(labDto.UserId);
                    if (User.Labs2 != null && User.Labs2.Where(x => x.PortalId == portal.portal.Id).Count() == 1)
                        CheckProgramEligibility(user, age, labDto.A1C, labDto.Glucose, labDto.Weight, eligibility, canRisk, labDto.DidYouFast, labDto.BMI);
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "Error occurred while processing the file: " + Path.GetFileName(file) + ". Error message : " + ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        public static string GetOrderNumber(int userId, int portalId)
        {
            ParticipantReader partReader = new ParticipantReader();
            var user = partReader.GetUsersforLabs(userId);
            if (user != null && user.Labs2 != null && user.Labs2.Count > 0)
            {
                var labOrders = user.Labs2.Where(x => x.PortalId == portalId && (!string.IsNullOrEmpty(x.OrderNo))).ToList();

                return user.Id.ToString() + user.FirstName[0] + user.LastName[0] + Convert.ToString(labOrders.Count() + 1);
            }
            else
                return user.Id.ToString() + user.FirstName[0] + user.LastName[0] + "1";
        }

        public static void CheckProgramEligibility(UserDto user, int age, float? A1C, float? Glucose, float? Weight, bool? prevEligibility, CanriskQuestionnaireDto canRisk, byte? didYouFast, float? BMI)
        {
            CommonReader commonReader = new CommonReader();
            var newEligibility = CheckProgramCondition(age, A1C, Glucose, Weight, canRisk, didYouFast, BMI).eligibility;
            if (prevEligibility == null || prevEligibility != newEligibility)
            {
                if (newEligibility)
                {
                    AddLabNotificationEvent(user.Id, user.Email, NotificationEventTypeDto.QualifyforDPP.Id);
                    commonReader.AddDashboardMessage(user.Id, IncentiveMessageTypes.DPP_Eligibile, null, null);
                }
                else
                {
                    AddLabNotificationEvent(user.Id, user.Email, NotificationEventTypeDto.NotQualifyforDPP.Id, user.FirstName);
                    PortalReader portalReader = new PortalReader();
                    ReadOrganizationRequest orgRequest = new ReadOrganizationRequest();
                    orgRequest.orgId = user.OrganizationId;
                    var orgResponse = portalReader.ReadOrganization(orgRequest).organization;
                    commonReader.AddDashboardMessage(user.Id, IncentiveMessageTypes.DPP_Ineligibile, null, null, orgResponse.ContactNumber);
                }
            }
        }

        public static void AddLabNotificationEvent(int userId, string emailId, int notificationEventTypeId, string Username = "")
        {
            NotificationManager notificationManager = new NotificationManager();
            AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
            NotificationEventDto notifyEventDto = new NotificationEventDto();
            notifyEventDto.NotificationEventTypeId = notificationEventTypeId;
            notifyEventDto.ToEmailAddress = emailId;
            notifyEventDto.UserId = userId;
            if (Username.Length != 0)
            {
                NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                xsltDto.UserFirstName = Username;
                notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
            }
            notificationRequest.NotificationEvent = notifyEventDto;
            notificationManager.AddOrEditNotificationEvent(notificationRequest);
        }

        public static CheckProgramConditionResponse CheckProgramCondition(int age, float? A1C, float? Glucose, float? Weight,
            CanriskQuestionnaireDto canrisk, byte? didYouFast, float? BMI)
        {
            CommonReader commonReader = new CommonReader();
            CheckProgramConditionResponse response = new CheckProgramConditionResponse();
            float? canriskBMI = (float)CommonReader.GetBMI(canrisk.Height.Value, canrisk.Weight.Value);
            double? glucMin = null;
            double? glucMax = null;
            double? glucDiab = null;
            if (Glucose.HasValue)
            {
                if (didYouFast.HasValue && didYouFast == 1)
                {
                    glucMin = ListOptions.ToImperial((float)(6.1), BioLookup.Glucose, commonReader.MeasurementRange());
                    glucMax = ListOptions.ToImperial((float)(6.9), BioLookup.Glucose, commonReader.MeasurementRange());
                    glucDiab = ListOptions.ToImperial((float)(7.0), BioLookup.Glucose, commonReader.MeasurementRange());
                }
                else if (didYouFast.HasValue && didYouFast == 2)
                {
                    glucDiab = ListOptions.ToImperial((float)(11.1), BioLookup.Glucose, commonReader.MeasurementRange());
                }
            }
            if ((Weight > 440) || !A1C.HasValue || (A1C.HasValue && A1C >= (float)6.5) || (Glucose.HasValue && Glucose >= (float)glucDiab))
                response.eligibility = false;
            else if ((18 <= age && age <= 74 && canrisk.CanriskScore >= 33) || (A1C.HasValue && 6 <= A1C && A1C <= (float)6.4) || (Glucose.HasValue && didYouFast.HasValue && didYouFast == 1 && (float)glucMin <= Glucose && Glucose <= (float)glucMax))
                response.eligibility = true;
            else if ((age >= 45 && BMI >= 30))
            {
                response.eligibility = true;
                response.labSource = true;
            }
            else if ((age >= 45 && canriskBMI >= 30))
            {
                response.eligibility = true;
                response.canriskSource = true;
            }
            return response;
        }
    }
}
