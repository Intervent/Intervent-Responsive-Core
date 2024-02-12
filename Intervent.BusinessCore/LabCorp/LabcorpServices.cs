using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public class LabcorpServicesManager : BaseManager
    {
        private static string UserName = ConfigurationManager.AppSettings["LabCorpUserName"];
        private static string Password = ConfigurationManager.AppSettings["LabCorpPassword"];
        private static string AccountNumber = ConfigurationManager.AppSettings["LabCorpAccountNumber"];

        private static Logger log = LogManager.GetCurrentClassLogger();

        public int PullLabValues(string status)
        {
            LabReader reader = new LabReader();
            var count = 0;
            try
            {
                Dictionary<string, LabResponse> labOrders = new Dictionary<string, LabResponse>();
                var Orders = Labcorp.GetHL7Reports(status, UserName, Password);
                foreach (var order in Orders.SearchResult)
                {
                    var report = Labcorp.PullHL7Report(order.ReportAccessionId, UserName, Password, AccountNumber);
                    if (report != null)
                    {
                        report.FirstName = order.PatientFirstName;
                        report.LastName = order.PatientLastName;
                        report.DOB = order.PatientDob;
                        if (!string.IsNullOrEmpty(order.PlacerOrderNumber))
                        {
                            labOrders.Add(order.PlacerOrderNumber.ToUpper(), report);
                        }
                        else
                        {
                            FindandUpdateLab(report);
                        }
                    }
                }
                if (labOrders != null && labOrders.Count > 0)
                {
                    List<string> orderNumbers = new List<string>(labOrders.Keys);
                    var labsDto = reader.GetLabsFromOrderNumber(orderNumbers);
                    count = labsDto.Count();
                    if (labsDto != null && labsDto.Count() > 0)
                    {
                        foreach (var lab in labsDto)
                        {
                            var order = labOrders[lab.OrderNo];
                            UpdateLabFromLabCorp(lab, order);
                        }
                    }
                    if (labsDto.Count < labOrders.Count)
                    {
                        var dtoordernos = labsDto.Select(x => x.OrderNo).ToList();
                        var missingList = orderNumbers.Except(dtoordernos).ToList();
                        foreach (var order in missingList)
                        {
                            var orderResponse = labOrders[order];
                            FindandUpdateLab(orderResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return count;
        }

        private void FindandUpdateLab(LabResponse lab)
        {
            if (!string.IsNullOrEmpty(lab.FirstName) && !string.IsNullOrEmpty(lab.LastName) && lab.DOB.HasValue)
            {
                AccountReader accountReader = new AccountReader();
                var labOrder = accountReader.GetUserByBasicDetail(lab.FirstName, lab.LastName, lab.DOB.Value);
                if (labOrder != null)
                {
                    UpdateLabFromLabCorp(labOrder, lab);
                    LogReader logReader = new LogReader();
                    var logEvent = new LogEventInfo(LogLevel.Info, "LabService", null, string.Format("Missing LabOrder and found user match {0}", labOrder.UserId), null, null);
                    logReader.WriteLogMessage(logEvent);
                    return;
                }

            }
            LabReader reader = new LabReader();
            var Errors = new List<string>();
            Errors.Add("Order Number is Missing");
            LogLabErrorsRequest request = new LogLabErrorsRequest();
            request.Errors = Errors;
            request.data = lab.ReportData;
            reader.LogLabErrors(request);
        }

        private void UpdateLabFromLabCorp(LabDto lab, LabResponse order)
        {
            try
            {
                LabReader reader = new LabReader();
                var dateCompleted = lab.DateCompleted;
                lab.HDL = order.HDL;
                lab.LDL = order.LDL;
                lab.TotalChol = order.TotalChol;
                lab.Trig = order.Trig;
                lab.A1C = order.A1C;
                lab.BloodTestDate = order.BloodTestDate;
                lab.DidYouFast = order.DidYouFast;
                lab.Glucose = order.Glucose;
                lab.Height = order.Height;
                lab.SBP = order.SBP;
                lab.DBP = order.DBP;
                lab.Waist = order.Waist;
                lab.Weight = order.Weight;
                lab.DateCompleted = DateTime.UtcNow;
                lab.HL7 = order.HL7;
                lab.BMI = order.BMI;
                lab.LabSelection = 2;
                var response = reader.UpdateOrder(new UpdateLabRequest { Lab = lab });
                if (order.ReportData != null)
                {
                    AddLabDataRequest request = new AddLabDataRequest();
                    request.data = order.ReportData;
                    request.labId = lab.Id;
                    reader.AddLabData(request);
                }
                var alertResponse = reader.SetLabAlert(lab.Id);
                if (response.organizationId == 90)
                {
                    new TwilioManager().SendSms(ConfigurationManager.AppSettings["Mobile_number"], "We have received lab result for Captiva user " + lab.UserId + ".");
                }
                else if (!string.IsNullOrEmpty(alertResponse))
                {
                    LabManager.AddLabNotificationEvent(lab.UserId, ConfigurationManager.AppSettings["LabNotificationEmail"].ToString(), NotificationEventTypeDto.CriticalAlert.Id);
                    new TwilioManager().SendSms(ConfigurationManager.AppSettings["Mobile_number"], "A critical alert has been triggered! Please respond promptly.");
                }
                if (response.emailId != null && !response.emailId.Contains("noemail.myintervent.com") && !response.emailId.Contains("samlnoemail.com") && !dateCompleted.HasValue)
                    LabManager.AddLabNotificationEvent(lab.UserId, response.emailId, NotificationEventTypeDto.LabSuccessfullyCompleted.Id);
                var newLab = reader.CheckLabErrors(lab);
                UpadateHealthNumbers(newLab);
                UpdateFUHealthNumbers(newLab);
                if (lab.Weight.HasValue || (lab.SBP.HasValue && lab.DBP.HasValue))
                {
                    ParticipantReader participantReader = new ParticipantReader();
                    AddEditWellnessDataRequest WDrequest = new AddEditWellnessDataRequest();
                    WDrequest.WellnessData = new WellnessDataDto();
                    WDrequest.WellnessData.Weight = lab.Weight;
                    WDrequest.WellnessData.SBP = lab.SBP;
                    WDrequest.WellnessData.DBP = lab.DBP;
                    WDrequest.WellnessData.UserId = lab.UserId;
                    WDrequest.WellnessData.CollectedOn = DateTime.UtcNow;
                    participantReader.AddEditWellnessData(WDrequest);
                    if (lab.Weight.HasValue)
                    {
                        AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                        healthDataRequest.HealthData = new HealthDataDto();
                        healthDataRequest.HealthData.UserId = lab.UserId;
                        healthDataRequest.HealthData.Weight = lab.Weight.Value;
                        healthDataRequest.HealthData.Source = (int)HealthDataSource.HRA;
                        healthDataRequest.HealthData.CreatedBy = SystemAdminId;
                        healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                    }
                }
                if (!dateCompleted.HasValue)
                {
                    CommonReader commonReader = new CommonReader();
                    commonReader.AddDashboardMessage(lab.UserId, IncentiveMessageTypes.LabResults, null, null);
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "LabService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public void UpadateHealthNumbers(LabDto lab)
        {
            HRAReader hraReader = new HRAReader();
            PortalReader portalReader = new PortalReader();
            var portal = portalReader.ReadPortal(new ReadPortalRequest { portalId = lab.PortalId }).portal;
            if (lab.BloodTestDate.HasValue)
            {
                hraReader.UpdateHealthNumbersFromLab(new UpdateHealthNumbersFromLabRequest { lab = lab, HRAValidity = portal.HRAValidity.HasValue ? portal.HRAValidity.Value : 30, overrideCurrentValue = true, updatedBy = SystemAdminId });
            }
        }

        public void UpdateFUHealthNumbers(LabDto lab)
        {
            FollowUpReader followUpReader = new FollowUpReader();
            ParticipantReader part_reader = new ParticipantReader();
            var user = part_reader.ReadUserParticipation(new ReadUserParticipationRequest { UserId = lab.UserId });
            if (user.usersinProgram != null)
                followUpReader.UpdateFUHealthNumbersfromLab(new UpdateFUHealthNumbersfromLabRequest { Lab = lab, UsersInProgramsId = user.usersinProgram.Id, updatedBy = SystemAdminId });
        }

        public void LabReminder()
        {
            LabReader reader = new LabReader();
            //Doctors Office Reminder
            List<UserDto> users = reader.TriggerLabs((int)LabChoices.DoctorsOffice);
            foreach (var user in users)
            {
                LabManager.AddLabNotificationEvent(user.Id, user.Email, NotificationEventTypeDto.DoctorsOfficeReminder.Id);
            }
            //LabCorp Reminder
            List<UserDto> Users = reader.TriggerLabs((int)LabChoices.LabCorp);
            foreach (var user in Users)
            {
                LabManager.AddLabNotificationEvent(user.Id, user.Email, NotificationEventTypeDto.LabCorpReminder.Id);
            }
        }
    }

}
