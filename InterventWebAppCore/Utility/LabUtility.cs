using Intervent.Business;
using Intervent.DAL;
using Intervent.HWS;
using Intervent.HWS.Request;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Text.RegularExpressions;

namespace InterventWebApp
{
    public class LabUtility
    {
        public static ReadLabWorkResponse ReadLabWork(int? id, int participantId, int participantPortalId)
        {
            LabReader reader = new LabReader();
            ReadLabRequest request = new ReadLabRequest();
            if (id.HasValue)
                request.Id = id;
            request.UserId = participantId;
            request.PortalId = participantPortalId;
            return reader.ReadLabWork(request);
        }

        public static ListLabsResponse ListLabs(int participantId)
        {
            LabReader reader = new LabReader();
            ReadLabRequest request = new ReadLabRequest();
            request.UserId = participantId;
            return reader.ListLabs(request);
        }

        public static string SubmitLabOrder(int participantId, int participantPortalId, int organizationId, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            AccountReader accountReader = new AccountReader();
            PortalReader portalReader = new PortalReader();
            var user = accountReader.GetUserDetails(participantId);
            LabCorpPlaceOrderRequest request = new LabCorpPlaceOrderRequest();
            request.City = user.City;
            request.AddressLine1 = user.Address;
            request.AddressLine2 = user.Address2;
            request.Comments = user.FirstName + " " + user.LastName;
            if (user.DOB.HasValue)
                request.DOB = user.DOB.Value;
            request.FirstName = user.FirstName;
            request.Gender = user.Gender.Value;
            request.LastName = user.LastName;
            request.PatientId = user.Id.ToString();
            if (user.State.HasValue && user.Country.HasValue)
            {
                var states = CommonUtility.ListStates(user.Country.Value);
                var state = states.FirstOrDefault(f => f.Id == user.State.Value);
                if (state != null)
                {
                    request.State = state.Code;
                }
            }
            request.ZipCode = user.Zip;
            var lastName = user.LastName;
            if (lastName.Length > 5)
                lastName = lastName.Substring(0, 5);
            lastName = Regex.Replace(lastName, @"[ '&().,/:;-]+", "");
            request.OrderNumber = lastName.ToUpper() + user.Id + DateTime.Now.Ticks;
            if (organizationId == 90)
            {
                request.LabProcedures = "322000|FASTIN|Yes,303756||,001453||";
            }
            else
            {
                var labProcedures = portalReader.GetPortalLabProcedures(participantPortalId);
                if (labProcedures.Count() > 0)
                    request.LabProcedures = string.Join(",", labProcedures.Select(x => x.LabProcedure.ProcedureCode + "|" + x.LabProcedure.ProcedureAnswerCode + "|" + x.LabProcedure.ProcedureAnswer));
            }
            try
            {
                request.labCorpUserName = labCorpUserName;
                request.labCorpPassword = labCorpPassword;
                request.labCorpAccountNumber = labCorpAccountNumber;
                var response = Labcorp.PlaceLabOrder(request);

                if (response.Status)
                {
                    LabReader reader = new LabReader();
                    return request.OrderNumber;
                }
                else
                {
                    LogReader reader = new LogReader();
                    var logEvent = new LogEventInfo(NLog.LogLevel.Warn, user.Id.ToString(), null, "LabCorp Response:" + response.Error, null, null);
                    reader.WriteLogMessage(logEvent);
                }
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, user.Id.ToString(), null, ex.Message, null, ex);
                reader.WriteLogMessage(logEvent);
            }

            return null;
        }

        public static bool UpdateLabSelection(int id, byte selection, int? diaognosticLabId, int userId, int participantId, int participantPortalId, int? integrationWith, string participantLanguagePreference, int organizationId, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            LabReader reader = new LabReader();
            UpdateLabSelectionRequest request = new UpdateLabSelectionRequest();
            var portal = PortalUtility.ReadPortal(participantPortalId).portal;
            if (!CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                if (selection == (int)LabChoices.LabCorp)
                {
                    ReadLabRequest readLabRequest = new ReadLabRequest();
                    readLabRequest.Id = id;
                    var lab = reader.ReadLabWork(readLabRequest).Lab;
                    if (string.IsNullOrEmpty(lab.OrderNo))
                        request.LabOrderNumber = SubmitLabOrder(participantId, participantPortalId, organizationId, labCorpUserName, labCorpPassword, labCorpAccountNumber);
                    else
                        request.LabOrderNumber = lab.OrderNo;
                    if (String.IsNullOrEmpty(request.LabOrderNumber))
                    {
                        return false;
                    }
                    if (NotificationUtility.hasValidEmail(participantId))
                        NotificationUtility.CreateLabNotificationEvent(NotificationEventTypeDto.CompleteLabswithLabCorp, participantId, portal.LabCorpAttachment, request.LabOrderNumber);
                }
                else if (selection == (int)LabChoices.DoctorsOffice && NotificationUtility.hasValidEmail(participantId))
                {
                    NotificationUtility.CreateLabNotificationEvent(NotificationEventTypeDto.SubmitLabResults, participantId, portal.DoctorOfficeAttachment, null);
                }
            }
            else
            {
                if (!diaognosticLabId.HasValue)
                {
                    if (selection == (int)LabChoices.DoctorsOffice)
                    {
                        AdminUtility.AddEditTask(null, (int)TaskTypes.CDPP_Lab_Choice, null, participantId, userId, "Rejecting this lab choice", false);
                        CreateCDPPLabChoiceTask(false, false, userId, participantId);
                        request.LabOrderNumber = string.Empty;
                    }
                    else
                    {
                        AdminUtility.AddEditTask(null, (int)TaskTypes.CDPP_Doctor_Choice, null, participantId, userId, "Rejecting this lab choice", false);
                        CreateCDPPLabChoiceTask(true, false, userId, participantId);
                        request.LabOrderNumber = LabManager.GetOrderNumber(participantId, portal.Id);
                    }
                    request.IntegratedWithLMC = true;
                }
            }
            request.Id = id;
            request.Selection = selection;
            request.DiagnosticLabId = diaognosticLabId;
            request.UpdatedBy = userId;
            request.LanguageCode = participantLanguagePreference;
            reader.AddUpdateLabSelection(request);
            return true;
        }

        public static void UpdateLabResultFile(int? id, string file, int participantId, int participantPortalId, int userId, int? adminId, int? integrationWith, string reason = null, bool? addLabRecord = null)
        {
            LabReader reader = new LabReader();
            if ((!id.HasValue || id == 0) && addLabRecord.HasValue && addLabRecord.Value)
            {
                UpdateLabSelectionRequest addLabRequest = new UpdateLabSelectionRequest();
                addLabRequest.UserId = participantId;
                addLabRequest.PortalId = participantPortalId;
                addLabRequest.Selection = Convert.ToByte((int)LabChoices.DoctorsOffice);
                addLabRequest.UpdatedBy = userId;
                addLabRequest.switchCount = 1;
                addLabRequest.additionalLab = true;
                addLabRequest.IntegratedWithLMC = CommonUtility.IsIntegratedWithLMC(integrationWith);
                id = reader.AddUpdateLabSelection(addLabRequest).labId;
            }
            UpdateLabResultFileRequest request = new UpdateLabResultFileRequest();
            request.Id = id;
            request.FileName = file;
            request.UpdatedBy = userId;
            if (!string.IsNullOrEmpty(reason))
            {
                request.RejectionReason = Convert.ToByte(reason);
                request.RejectedBy = userId;
                request.RejectedOn = DateTime.UtcNow;
            }
            else
            {
                request.RejectionReason = null;
                request.RejectedBy = null;
                request.RejectedOn = null;
            }
            reader.UpdateLabResultFile(request);
            AdminReader adminReader = new AdminReader();
            AddEditTaskRequest addTaskRequest = new AddEditTaskRequest();
            addTaskRequest.task = new TasksDto();
            addTaskRequest.task.Owner = addTaskRequest.task.UserId = participantId;
            addTaskRequest.task.UpdatedBy = addTaskRequest.task.CreatedBy = addTaskRequest.task.UpdatedBy = adminId.HasValue ? adminId.Value : participantId;
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
                addTaskRequest.task.Owner = addTaskRequest.task.UserId = participantId;
            addTaskRequest.task.UpdatedBy = addTaskRequest.task.CreatedBy = addTaskRequest.task.UpdatedBy = adminId.HasValue ? adminId.Value : participantId;
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
                addTaskRequest.task.TaskTypeId = (int)TaskTypes.CDPP_Doctor_Labs;
            else
                addTaskRequest.task.TaskTypeId = (int)TaskTypes.Know_Your_Numbers;
            if (!string.IsNullOrEmpty(file))
            {
                //Create Task
                /* if (!CommonUtility.HasAdminRole(User.RoleCode()))
                 {
                     if (CommonUtility.IsIntegratedWithLMC(integrationWith))
                     {
                         CreateCDPPLabChoiceTask(false, true, userId, participantId);
                     }
                     else
                     {
                         addTaskRequest.task.IsActive = true;
                         addTaskRequest.task.Status = Intervent.Web.DTO.TaskStatus.N.ToString();
                         addTaskRequest.task.Comment = "Please process the uploaded lab.";
                         adminReader.AddEditTask(addTaskRequest);
                     }
                 }*/
            }
            else
            {
                addTaskRequest.task.Comment = "Participant removed the uploaded labs.";
                addTaskRequest.task.IsActive = false;
                adminReader.AddEditTask(addTaskRequest);
            }
        }

        public static void UpdateLabs(int systemAdminId, AddLabModel labModel)
        {
            LabReader reader = new LabReader();
            AddLabRequest request = new AddLabRequest();
            request.HRAValidity = labModel.HRAValidity.HasValue ? labModel.HRAValidity.Value : 30;
            request.SaveNew = labModel.SaveNew;
            if (!labModel.Lab.Height.HasValue)
            {
                labModel.Lab.Height = (labModel.HeightFt * 12);
                if (labModel.HeightInch.HasValue && labModel.HeightInch.Value > 0)
                    labModel.Lab.Height = labModel.Lab.Height + labModel.HeightInch;
            }
            if (labModel.BloodTestDate != null)
                labModel.Lab.BloodTestDate = Convert.ToDateTime(labModel.BloodTestDate);
            request.Id = labModel.Id;
            request.updatedBy = labModel.updatedBy.HasValue ? labModel.updatedBy.Value : systemAdminId;
            request.Lab = labModel.Lab;
            if (request.Id == 0 && !labModel.fromEligibility)
                request.Lab.AdditionalLab = true;
            if (request.Lab.PortalId == 0)
                request.Lab.PortalId = labModel.participantPortalId;
            if (request.Lab.UserId == 0)
                request.Lab.UserId = labModel.participantId;
            if (request.Lab.Height != null && request.Lab.Weight != null)
            {
                request.Lab.BMI = (float)CommonReader.GetBMI(request.Lab.Height.Value, request.Lab.Weight.Value);
            }
            if (labModel.userinProgramId.HasValue)
            {
                request.userinprogramId = labModel.userinProgramId.Value;
            }
            else
            {
                ProgramReader programReader = new ProgramReader();
                var usersinProgram = programReader.GetUserProgramsByPortal(new GetUserProgramHistoryRequest { portalId = request.Lab.PortalId, userId = request.Lab.UserId }).Where(x => x.IsActive).FirstOrDefault();
                if (usersinProgram != null)
                    request.userinprogramId = usersinProgram.Id;
            }
            request.updateLab = labModel.updateLab;
            var lab = new LabDto();

            if (labModel.Id > 0)
            {
                lab = reader.ReadLabWork(new ReadLabRequest { Id = request.Id }).Lab;
            }
            request.overrideCurrentValue = true;
            var response = reader.AddUpdateLab(request);
            if (CommonUtility.IsIntegratedWithLMC(labModel.integrationWith))
            {
                ReadLabRequest readLabRequest = new ReadLabRequest();
                readLabRequest.UserId = labModel.participantId;
                var labs = reader.ListLabs(readLabRequest).Labs;
                if (labs != null && (labs.OrderBy(x => x.Id).FirstOrDefault().Id == labModel.Id || (labs.Count == 1 && labModel.Id == 0)))
                {
                    bool? eligibility = null;
                    ParticipantReader participantReader = new ParticipantReader();
                    AccountReader accountReader = new AccountReader();
                    var user = accountReader.GetUserDetails(labModel.participantId);
                    var canrisk = participantReader.GetCanriskResponse(new GetCanriskRequest { uniqueId = user.UniqueId });
                    var age = CommonUtility.GetAge(user.DOB.Value);
                    if (lab.Id > 0 && lab.DateCompleted != null)
                        eligibility = LabManager.CheckProgramCondition(age, lab.A1C, lab.Glucose, lab.Weight, canrisk, lab.DidYouFast, lab.BMI).eligibility;
                    else
                        eligibility = null;
                    LabManager.CheckProgramEligibility(user, age, labModel.Lab.A1C, labModel.Lab.Glucose, labModel.Lab.Weight, eligibility, canrisk, labModel.Lab.DidYouFast, request.Lab.BMI);
                }
            }
            else if (!labModel.updateLab && response.success && response.updatedByAdmin && NotificationUtility.hasValidEmail(request.Lab.UserId))
            {
                NotificationUtility.CreateLabNotificationEvent(NotificationEventTypeDto.LabSuccessfullyCompleted, request.Lab.UserId, null, null);
            }
        }

        public static AddLabSelectionResponse AddLabSelection(int labSource, int participantId, int userId, int participantPortalId, int? integrationWith, int? followUpId, string participantEmail, int organizationId, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            LabReader reader = new LabReader();
            AddLabSelectionResponse response = new AddLabSelectionResponse();
            ReadLabRequest readLabRequest = new ReadLabRequest();
            readLabRequest.UserId = participantId;
            //Avoid duplicate entries
            var lab = reader.ListLabs(readLabRequest).Labs.Where(x => x.PortalId == participantPortalId
            && x.CreatedOn.Value.AddHours(1) > DateTime.UtcNow && !x.DateCompleted.HasValue).FirstOrDefault();
            if (lab == null)
            {
                UpdateLabSelectionRequest request = new UpdateLabSelectionRequest();
                var portal = PortalUtility.ReadPortal(participantPortalId).portal;
                if (labSource == (int)LabChoices.LabCorp)
                {
                    if (!integrationWith.HasValue || (!CommonUtility.IsIntegratedWithLMC(integrationWith)))
                    {

                        request.LabOrderNumber = SubmitLabOrder(participantId, participantPortalId, organizationId, labCorpUserName, labCorpPassword, labCorpAccountNumber);
                        if (String.IsNullOrEmpty(request.LabOrderNumber))
                        {
                            var labresponse = new AddLabSelectionResponse();
                            labresponse.status = false;
                            return labresponse;
                        }
                        if (NotificationUtility.hasValidEmail(participantId))
                        {
                            NotificationUtility.CreateLabNotificationEvent(NotificationEventTypeDto.CompleteLabswithLabCorp, participantId, portal.LabCorpAttachment, request.LabOrderNumber);
                        }
                    }
                    else
                    {
                        request.LabOrderNumber = LabManager.GetOrderNumber(participantId, participantPortalId);
                    }
                }
                request.UserId = participantId;
                request.PortalId = participantPortalId;
                request.Selection = Convert.ToByte(labSource);
                request.UpdatedBy = userId;
                if (CommonUtility.IsIntegratedWithLMC(integrationWith))
                    request.switchCount = 1;
                response = reader.AddUpdateLabSelection(request);
                if (labSource == (int)LabChoices.DoctorsOffice)
                {
                    if (NotificationUtility.hasValidEmail(participantId) && (!integrationWith.HasValue ||
                        (!CommonUtility.IsIntegratedWithLMC(integrationWith))))
                    {
                        NotificationUtility.CreateLabNotificationEvent(NotificationEventTypeDto.SubmitLabResults, participantId, portal.DoctorOfficeAttachment, null);
                    }
                }
                if (CommonUtility.IsIntegratedWithLMC(integrationWith))
                {
                    if (labSource == (int)LabChoices.DoctorsOffice)
                        CreateCDPPLabChoiceTask(false, false, userId, participantId);
                    else
                        CreateCDPPLabChoiceTask(true, false, userId, participantId);
                    if (!followUpId.HasValue)
                        LabManager.AddLabNotificationEvent(participantId, participantEmail, NotificationEventTypeDto.OneStepCloser.Id);
                    else
                        LabManager.AddLabNotificationEvent(participantId, participantEmail, NotificationEventTypeDto.FollowUpLabs.Id);
                }
            }
            else
            {
                response.duplicate = true;
                response.labId = lab.Id;
                response.OrderId = lab.OrderNo;
                response.labSelection = (int)lab.LabSelection;
            }
            return response;
        }

        public static LabDto ProcessLabResults(LabDto lab, IList<LabReferenceRangeDto> LabReferences, string gender)
        {
            string letterandAction = "";
            int Normal = 0, OutofRange = 0, Abnormal = 0, Critical = 0, glucoseRisk = 0, cholRisk = 0, weightRisk = 0;

            if (lab.Glucose.HasValue)
            {
                if (lab.DidYouFast == 1)
                {
                    if (lab.Glucose >= LabReferences[LabReference.GlucoseFasting].NormalMin && lab.Glucose <= LabReferences[LabReference.GlucoseFasting].NormalMax)
                    {
                        Normal++;
                    }
                    else if (lab.Glucose >= LabReferences[LabReference.GlucoseFasting].AbnormalMin && lab.Glucose <= LabReferences[LabReference.GlucoseFasting].AbnormalMax)
                    {
                        Abnormal++;
                        glucoseRisk = 3;
                    }
                    else if (lab.Glucose < LabReferences[LabReference.GlucoseFasting].CriticalLessthan || lab.Glucose > LabReferences[LabReference.GlucoseFasting].CriticalGreaterthan)
                    {
                        Critical++;
                        glucoseRisk = 4;
                    }
                    else
                    {
                        OutofRange++;
                        glucoseRisk = 2;
                    }
                }
                else if (lab.DidYouFast == 2)
                {
                    if (lab.Glucose >= LabReferences[LabReference.GlucoseNonFasting].NormalMin && lab.Glucose <= LabReferences[LabReference.GlucoseNonFasting].NormalMax)
                    {
                        Normal++;
                    }
                    else if (lab.Glucose >= LabReferences[LabReference.GlucoseNonFasting].AbnormalMin && lab.Glucose <= LabReferences[LabReference.GlucoseNonFasting].AbnormalMax)
                    {
                        Abnormal++;
                        glucoseRisk = 3;
                    }
                    else if (lab.Glucose < LabReferences[LabReference.GlucoseNonFasting].CriticalLessthan || lab.Glucose > LabReferences[LabReference.GlucoseNonFasting].CriticalGreaterthan)
                    {
                        Critical++;
                        glucoseRisk = 4;
                    }
                    else
                    {
                        OutofRange++;
                        glucoseRisk = 2;
                    }
                }
            }

            if (lab.TotalChol.HasValue)
            {
                if (lab.TotalChol <= LabReferences[LabReference.TotalCholesterol].NormalMax)
                {
                    Normal++;
                }
                else if (lab.TotalChol >= LabReferences[LabReference.TotalCholesterol].OutofRangeMin && lab.TotalChol <= LabReferences[LabReference.TotalCholesterol].OutofRangeMax)
                {
                    OutofRange++;
                    cholRisk = 2;
                }
                else if (lab.TotalChol >= LabReferences[LabReference.TotalCholesterol].AbnormalMin)
                {
                    Abnormal++;
                    cholRisk = 3;
                }
            }

            if (lab.Trig.HasValue)
            {
                if (lab.Trig <= LabReferences[LabReference.Triglycerides].NormalMax)
                {
                    Normal++;
                }
                else if (lab.Trig >= LabReferences[LabReference.Triglycerides].OutofRangeMin && lab.Trig <= LabReferences[LabReference.Triglycerides].OutofRangeMax)
                {
                    OutofRange++;
                }
                else if (lab.Trig >= LabReferences[LabReference.Triglycerides].AbnormalMin && lab.Trig <= LabReferences[LabReference.Triglycerides].AbnormalMax)
                {
                    Abnormal++;
                }
                else if (lab.Trig > LabReferences[LabReference.Triglycerides].CriticalGreaterthan)
                {
                    Critical++;
                }
            }

            if (lab.HDL.HasValue)
            {
                if (gender == "1")
                {
                    if (lab.HDL >= LabReferences[LabReference.HDL].NormalforMale)
                    {
                        Normal++;
                    }
                    else if (lab.HDL < LabReferences[LabReference.HDL].OutofRangeforMale)
                    {
                        OutofRange++;
                    }
                }
                else if (gender == "2")
                {
                    if (lab.HDL >= LabReferences[LabReference.HDL].NormalforFemale)
                    {
                        Normal++;
                    }
                    else if (lab.HDL < LabReferences[LabReference.HDL].OutofRangeforFemale)
                    {
                        OutofRange++;
                    }
                }
            }

            if (lab.LDL.HasValue)
            {
                if (lab.LDL <= LabReferences[LabReference.LDL].NormalMax)
                {
                    Normal++;
                }
                else if (lab.LDL >= LabReferences[LabReference.LDL].OutofRangeMin && lab.LDL <= LabReferences[LabReference.LDL].OutofRangeMax && cholRisk != 2)
                {
                    OutofRange++;
                }
                else if (lab.LDL >= LabReferences[LabReference.LDL].AbnormalMin && cholRisk != 3)
                {
                    Abnormal++;
                }
            }

            if (lab.SBP.HasValue || lab.DBP.HasValue)
            {
                for (int j = 0; j < 2; j++)
                {
                    int refernce = 0;
                    float value = 0;
                    if (j == 0)
                    {
                        if (lab.SBP.HasValue)
                        {
                            value = lab.SBP.Value;
                            refernce = LabReference.SystolicBloodPressure;
                        }
                        else break;
                    }
                    else if (j == 1)
                    {
                        if (lab.DBP.HasValue)
                        {
                            value = lab.DBP.Value;
                            refernce = LabReference.DiastolicBloodPressure;
                        }
                        else break;
                    }
                    if (value <= LabReferences[refernce].NormalMax)
                    {
                        Normal++;
                    }
                    else if ((!LabReferences[refernce].OutofRangeMin.HasValue || value >= LabReferences[refernce].OutofRangeMin) && value <= LabReferences[refernce].OutofRangeMax)
                    {
                        OutofRange++;
                    }
                    else if (value >= LabReferences[refernce].AbnormalMin && value <= LabReferences[refernce].AbnormalMax)
                    {
                        Abnormal++;
                    }
                    else if (value > LabReferences[refernce].CriticalGreaterthan)
                    {
                        Critical++;
                    }
                }
            }

            if (lab.Waist.HasValue)
            {
                if (gender == "1")
                {
                    if (lab.Waist <= LabReferences[LabReference.WaistCircumference].NormalforMale)
                    {
                        Normal++;
                    }
                    else if (lab.Waist > LabReferences[LabReference.WaistCircumference].OutofRangeforMale)
                    {
                        OutofRange++;
                        weightRisk = 2;
                    }
                }
                else if (gender == "2")
                {
                    if (lab.Waist <= LabReferences[LabReference.WaistCircumference].NormalforFemale)
                    {
                        Normal++;
                        weightRisk = 1;
                    }
                    else if (lab.Waist > LabReferences[LabReference.WaistCircumference].OutofRangeforFemale)
                    {
                        OutofRange++;
                        weightRisk = 2;
                    }
                }
            }

            if (lab.BMI.HasValue)
            {
                if (lab.BMI >= LabReferences[LabReference.Weight].NormalMin && lab.BMI <= LabReferences[LabReference.Weight].NormalMax)
                {
                    Normal++;
                }
                else if ((lab.BMI < LabReferences[LabReference.Weight].OutofRangeMin || lab.BMI > LabReferences[LabReference.Weight].OutofRangeMax) && weightRisk != 2)
                {
                    OutofRange++;
                }
            }
            if (lab.HighCotinine.HasValue)
            {
                if (lab.HighCotinine.Value)
                {
                    Normal++;
                }
                else
                {
                    OutofRange++;
                }
            }
            if (lab.A1C.HasValue)
            {
                if (lab.A1C <= LabReferences[LabReference.HbA1c].NormalMax)
                {
                    Normal++;
                }
                else if (lab.A1C >= LabReferences[LabReference.HbA1c].AbnormalMin && lab.A1C <= LabReferences[LabReference.HbA1c].AbnormalMax && glucoseRisk != 3)
                {
                    Abnormal++;
                }
                else if (lab.A1C > LabReferences[LabReference.HbA1c].CriticalGreaterthan && glucoseRisk != 4)
                {
                    Critical++;
                }
            }
            if (Critical >= 1)
                letterandAction = Constants.CRITICAL;
            else if (OutofRange >= 3 || Abnormal >= 1)
                letterandAction = Constants.ABNORMAL;
            else if (OutofRange == 1 || OutofRange == 2)
                letterandAction = Constants.OUTOFRANGE;
            else
                letterandAction = Constants.NORMAL;
            lab.LetterofAction = letterandAction;
            return lab;
        }

        public static TakeActionResponse TakeAction(int Id, int AdminId)
        {
            LabReader reader = new LabReader();
            TakeActionRequest request = new TakeActionRequest();
            request.Id = Id;
            request.AdminId = AdminId;
            return reader.TakeAction(request);
        }

        public static ReadLabReportResponse PullLabReport(int id, bool fromLog, int userId)
        {
            LabReader reader = new LabReader();
            ReadLabReportRequest request = new ReadLabReportRequest();
            request.Id = id;
            request.fromLog = fromLog;
            var response = reader.ReadLabReport(request);
            ReadLabReportResponse reportResponse = new ReadLabReportResponse();
            if (response.Data != null && (response.UserId == userId || PortalUtility.GetFilteredOrganizationsList(userId).Select(x => x.Id).Contains(response.OrganizationId)))
                reportResponse.Data = response.Data;
            return reportResponse;
        }

        public static byte[] PullLabOrder(string orderNo, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            return Labcorp.PullOrderConfirmation(orderNo, labCorpUserName, labCorpPassword, labCorpAccountNumber);
        }

        public static ListLabReferenceRangesResponse ListLabReferenceRanges()
        {
            LabReader reader = new LabReader();
            return reader.GetLabReferences();
        }

        public static bool ResendLabRequisitionEmail(int labId, string labOrderNo, int participantId, int participantPortalId)
        {
            LabReader reader = new LabReader();
            var portal = PortalUtility.ReadPortal(participantPortalId).portal;
            if (NotificationUtility.hasValidEmail(participantId))
            {
                if (!string.IsNullOrEmpty(labOrderNo))
                {
                    NotificationUtility.CreateLabNotificationEvent(NotificationEventTypeDto.CompleteLabswithLabCorp, participantId, portal.LabCorpAttachment, labOrderNo);
                }
                else
                {
                    NotificationUtility.CreateLabNotificationEvent(NotificationEventTypeDto.SubmitLabResults, participantId, portal.DoctorOfficeAttachment, "");
                }
            }
            return true;
        }

        public static LabDto CheckLabErrors(LabDto labDto)
        {
            LabReader reader = new LabReader();
            return reader.CheckLabErrors(labDto);
        }

        public static CheckProgramConditionResponse CheckProgramEligibility(int userId, int portalId, string uniqueId, string dob)
        {
            CheckProgramConditionResponse response = new CheckProgramConditionResponse();
            if (string.IsNullOrEmpty(uniqueId))
            {
                response.pending = true;
                return response;
            }
            LabReader labReader = new LabReader();
            CommonReader commonReader = new CommonReader();
            ParticipantReader participantReader = new ParticipantReader();
            var lab = labReader.ListLabs(new ReadLabRequest
            {
                UserId = userId,
                PortalId = portalId
            }).Labs.OrderBy(x => x.Id).FirstOrDefault();
            if (lab != null && lab.DateCompleted != null)
            {
                int age = commonReader.GetAge(Convert.ToDateTime(dob));
                var canRisk = participantReader.GetCanriskResponse(new GetCanriskRequest() { uniqueId = uniqueId });
                response = LabManager.CheckProgramCondition(age, lab.A1C, lab.Glucose, lab.Weight, canRisk, lab.DidYouFast, lab.BMI);
            }
            else
                response.pending = true;
            return response;
        }

        public static List<DiagnosticLab> GetDiagnosticLabs()
        {
            LabReader reader = new LabReader();
            return reader.GetDiagnosticLabs();
        }

        public static void CreateCDPPLabChoiceTask(bool IsLab, bool DoctorLabs, int userId, int participantId)
        {
            string LabChoice = "";
            if (DoctorLabs)
                LabChoice = Intervent.Web.DTO.Constants.CDPP_Doctor_Labs;
            else
                LabChoice = IsLab ? Intervent.Web.DTO.Constants.CDPP_Lab_Choice.ToString() : Intervent.Web.DTO.Constants.CDPP_Doctor_Choice.ToString();

            AdminReader reader = new AdminReader();
            int taskTypeId = reader.TaskTypes().Find(t => t.Name == LabChoice).Id;

            AdminUtility.AddEditTask(null, taskTypeId, "N", participantId, userId, LabChoice, true);
        }
    }
}