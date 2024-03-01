using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Intervent.Web.DataLayer
{
    public class LabReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ReadLabWorkResponse ReadLabWork(ReadLabRequest request)
        {
            ReadLabWorkResponse response = new ReadLabWorkResponse();
            var labWork = context.Labs.Where(x => (request.Id.HasValue && x.Id == request.Id) || (!request.Id.HasValue && x.UserId == request.UserId && x.PortalId == request.PortalId)).OrderByDescending(x => x.Id).FirstOrDefault();
            if (labWork != null)
                response.Lab = Utility.mapper.Map<DAL.Lab, LabDto>(labWork);
            return response;
        }

        public ReadLabReportResponse ReadLabReport(ReadLabReportRequest request)
        {
            ReadLabReportResponse response = new ReadLabReportResponse();

            if (!request.fromLog)
            {
                var labWork = context.LabDatas.Where(x => x.LabId == request.Id).FirstOrDefault();
                if (labWork != null)
                    response.Data = labWork.ReportData;
            }
            else
            {
                var labdata = context.LabErrorLogs.Where(x => x.Id == request.Id).FirstOrDefault();
                if (labdata != null)
                    response.Data = labdata.Data;
            }
            var lab = context.Labs.Where(x => x.Id == request.Id).Include("User2").FirstOrDefault();
            if (lab != null)
            {
                response.UserId = lab.UserId;
                response.OrganizationId = lab.User2.OrganizationId;
            }

            return response;
        }

        public ListLabsResponse ListLabs(ReadLabRequest request)
        {
            ListLabsResponse response = new ListLabsResponse();
            var labWork = context.Labs.Where(x => x.UserId == request.UserId).Include("User").Include("User1").Include("User2").OrderByDescending(x => x.Id).ToList();
            if (labWork != null)
            {
                response.Labs = Utility.mapper.Map<IList<DAL.Lab>, IList<LabDto>>(labWork);
            }
            return response;
        }
        public AddLabSelectionResponse AddUpdateLabSelection(UpdateLabSelectionRequest request)
        {
            AddLabSelectionResponse response = new AddLabSelectionResponse();
            if (request.Id.HasValue)
            {
                var labWork = context.Labs.Where(x => (request.Id.HasValue && x.Id == request.Id)).FirstOrDefault();
                labWork.LabSelection = request.Selection;
                if (!string.IsNullOrEmpty(request.LabOrderNumber))
                    labWork.OrderNo = request.LabOrderNumber;
                else
                {
                    if (request.IntegratedWithLMC && !request.DiagnosticLabId.HasValue)
                        labWork.OrderNo = request.LabOrderNumber;
                }
                labWork.RejectionReason = null;
                labWork.RejectedBy = null;
                labWork.RejectedOn = null;
                labWork.DiagnosticLabId = request.DiagnosticLabId;
                labWork.ModifiedBy = request.UpdatedBy;
                labWork.ModifiedOn = DateTime.UtcNow;
                labWork.SwitchCount = 1;
                context.Labs.Attach(labWork);
                context.Entry(labWork).State = EntityState.Modified;
                context.SaveChanges();
                response.labId = labWork.Id;
                response.OrderId = labWork.OrderNo;
            }
            else
            {
                var labWork = new DAL.Lab();
                labWork.LabSelection = request.Selection;
                labWork.PortalId = request.PortalId;
                labWork.UserId = request.UserId;
                labWork.OrderNo = request.LabOrderNumber;
                labWork.DiagnosticLabId = request.DiagnosticLabId;
                labWork.CreatedBy = labWork.ModifiedBy = request.UpdatedBy;
                labWork.CreatedOn = labWork.ModifiedOn = DateTime.UtcNow;
                labWork.SwitchCount = request.switchCount;
                labWork.AdditionalLab = request.additionalLab;
                var lab = context.Labs.Add(labWork).Entity;
                context.SaveChanges();
                CommonReader commonReader = new CommonReader();
                response.labId = lab.Id;
                response.OrderId = labWork.OrderNo;
            }
            response.status = true;
            return response;
        }

        public string SetLabAlert(int labId)
        {
            StoredProcedures sp = new StoredProcedures();
            var alert = sp.SetLabAlert(labId);
            return alert;
        }

        public void UpdateLabResultFile(UpdateLabResultFileRequest request)
        {
            var lab = context.Labs.Where(x => x.Id == request.Id).FirstOrDefault();
            if (lab != null)
            {
                lab.LabOrder = request.FileName;
                lab.ModifiedBy = request.UpdatedBy;
                lab.ModifiedOn = DateTime.UtcNow;
                lab.RejectedBy = request.RejectedBy;
                lab.RejectionReason = request.RejectionReason;
                lab.RejectedOn = request.RejectedOn;
                context.Labs.Attach(lab);
                context.Entry(lab).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public UpdateLabResponse AddUpdateLab(AddLabRequest request)
        {
            UpdateLabResponse response = new UpdateLabResponse();
            var labWork = context.Labs.Where(x => x.Id == request.Id).FirstOrDefault();
            if (labWork != null && request.Lab != null)
            {
                labWork.Height = request.Lab.Height;
                labWork.Weight = request.Lab.Weight;
                labWork.Waist = request.Lab.Waist;
                labWork.BPArm = request.Lab.BPArm;
                labWork.SBP = request.Lab.SBP;
                labWork.DBP = request.Lab.DBP;
                labWork.HDL = request.Lab.HDL;
                labWork.LDL = request.Lab.LDL;
                labWork.TotalChol = request.Lab.TotalChol;
                labWork.Trig = request.Lab.Trig;
                labWork.A1C = request.Lab.A1C;
                labWork.BloodTestDate = request.Lab.BloodTestDate;
                labWork.DidYouFast = request.Lab.DidYouFast;
                labWork.Glucose = request.Lab.Glucose;
                labWork.BMI = request.Lab.BMI;
                labWork.HighCotinine = request.Lab.HighCotinine;
                if (!labWork.DateCompleted.HasValue)
                    labWork.DateCompleted = DateTime.UtcNow;
                labWork.ModifiedOn = DateTime.UtcNow;
                labWork.ModifiedBy = request.updatedBy;
                context.SaveChanges();
                request.Lab.Id = response.labId = labWork.Id;
                response.success = true;
            }
            else if (request.Lab != null && request.Lab.Id == 0)
            {
                DAL.Lab labDAL = new DAL.Lab();
                labDAL = Utility.mapper.Map<LabDto, Lab>(request.Lab);
                labDAL.CreatedOn = labDAL.DateCompleted = DateTime.UtcNow;
                context.Labs.Add(labDAL);
                context.SaveChanges();
                request.Lab.Id = response.labId = labDAL.Id;
                response.success = true;
            }
            if (response.success && !request.skipAssessmentUpdate)
            {
                HRAReader reader = new HRAReader();
                if (request.SaveNew)
                    request.Lab = CheckLabErrors(request.Lab);
                reader.UpdateHealthNumbersFromLab(new UpdateHealthNumbersFromLabRequest { lab = request.Lab, HRAValidity = request.HRAValidity, saveNew = request.SaveNew, overrideCurrentValue = request.overrideCurrentValue, updatedBy = request.updatedBy });
                if (request.userinprogramId.HasValue)
                {
                    UpdateFUHealthNumbersfromLabRequest followUprequest = new UpdateFUHealthNumbersfromLabRequest();
                    ProgramReader programReader = new ProgramReader();
                    FollowUpReader followUpReader = new FollowUpReader();
                    followUprequest.Lab = request.Lab;
                    followUprequest.UsersInProgramsId = request.userinprogramId.Value;
                    followUprequest.updatedBy = request.updatedBy;
                    followUpReader.UpdateFUHealthNumbersfromLab(followUprequest);
                }
                var lab = context.Labs.Where(x => x.UserId == request.Lab.UserId && x.PortalId == request.Lab.PortalId).FirstOrDefault();
                response.criticalalert = SetLabAlert(lab.Id);
                if (!request.updateLab)
                {
                    CommonReader commonReader = new CommonReader();
                    commonReader.AddDashboardMessage(request.Lab.UserId, IncentiveMessageTypes.LabResults, null, null);
                }
                if (request.Lab.UserId != request.updatedBy)
                    response.updatedByAdmin = true;
            }
            if (request.Lab.Weight.HasValue)
            {
                ParticipantReader participantReader = new ParticipantReader();
                AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                healthDataRequest.HealthData = new HealthDataDto();
                healthDataRequest.HealthData.UserId = request.Lab.UserId;
                healthDataRequest.HealthData.Weight = request.Lab.Weight.Value;
                healthDataRequest.HealthData.Source = (int)HealthDataSource.Labs;
                healthDataRequest.HealthData.CreatedBy = request.updatedBy;
                healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                participantReader.AddtoHealthData(healthDataRequest);
            }
            return response;
        }

        public UpdateLabResponse UpdateOrder(UpdateLabRequest labRequest)
        {
            var existingLab = context.Labs.Where(x => x.Id == labRequest.Lab.Id).FirstOrDefault();
            UpdateLabResponse response = new UpdateLabResponse();
            labRequest.Lab.User = null;
            labRequest.Lab.User1 = null;
            labRequest.Lab.User2 = null;
            var labDAL = Utility.mapper.Map<LabDto, Lab>(labRequest.Lab);
            labDAL.ModifiedOn = DateTime.UtcNow;
            context.Entry(existingLab).CurrentValues.SetValues(labDAL);
            context.SaveChanges();
            response.success = true;
            GetUserRequest request = new GetUserRequest();
            request.id = labDAL.UserId;
            var user = new AccountReader().ReadUser(request).User;
            response.emailId = user.Email;
            response.organizationId = user.OrganizationId;
            response.languagePreference = user.LanguagePreference;
            return response;
        }

        public void AddLabData(AddLabDataRequest request)
        {
            try
            {
                var labUpdate = context.LabDatas.Where(x => x.LabId == request.labId).FirstOrDefault();
                if (labUpdate == null)
                {
                    LabData labData = new LabData();
                    labData.LabId = request.labId;
                    labData.ReportData = request.data;
                    labData.DateCreated = DateTime.UtcNow;
                    context.LabDatas.Add(labData);
                    context.SaveChanges();
                }
                else
                {
                    labUpdate.ReportData = request.data;
                    context.LabDatas.Attach(labUpdate);
                    context.Entry(labUpdate).State = EntityState.Modified;
                    context.SaveChanges();
                }

            }
            catch { }
        }

        public ListLabReferenceRangesResponse GetLabReferences()
        {
            ListLabReferenceRangesResponse response = new ListLabReferenceRangesResponse();
            var referenceRange = context.LabReferenceRanges.ToList();
            response.LabReferences = Utility.mapper.Map<IList<DAL.LabReferenceRanx>, IList<LabReferenceRangeDto>>(referenceRange);
            return response;
        }

        public TakeActionResponse TakeAction(TakeActionRequest request)
        {
            TakeActionResponse response = new TakeActionResponse();
            var lab = context.Labs.Where(x => x.Id == request.Id).FirstOrDefault();
            if (lab != null)
            {
                lab.ReviewedOn = System.DateTime.UtcNow;
                lab.ReviewedBy = request.AdminId;
                context.Labs.Attach(lab);
                context.Entry(lab).State = EntityState.Modified;
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        public void UpdateAdditionalLab(int labId)
        {
            var lab = context.Labs.Where(x => x.Id == labId).FirstOrDefault();
            if (lab != null && lab.AdditionalLab)
            {
                lab.AdditionalLab = false;
                context.Labs.Attach(lab);
                context.Entry(lab).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public List<UserDto> TriggerLabs(int labSelection)
        {
            List<UserDto> users = new List<UserDto>();
            DateTime dateTime = DateTime.UtcNow.AddDays(-14);
            var labs = context.Labs.Include("User").Where(x => !(x.DateCompleted.HasValue) && x.ModifiedOn < dateTime && x.ReminderEmailSent == null && x.LabSelection == labSelection).ToList();
            if (labs != null && labs.Count > 0)
            {
                foreach (var Lab in labs)
                {
                    UserDto User = new UserDto();
                    Lab.ReminderEmailSent = 1;
                    context.Labs.Attach(Lab);
                    context.Entry(Lab).State = EntityState.Modified;
                    context.SaveChanges();
                    User.Id = Lab.UserId;
                    User.Email = Lab.User.Email;
                    users.Add(User);
                }
            }
            return users;
        }

        public void LogLabErrors(LogLabErrorsRequest request)
        {
            foreach (var error in request.Errors)
            {
                DAL.LabErrorLog laberror = new DAL.LabErrorLog();
                laberror.UserId = request.userId;
                laberror.PortalId = request.portalId;
                laberror.Error = error;
                laberror.LogDate = DateTime.UtcNow;
                laberror.Data = request.data;
                context.LabErrorLogs.Add(laberror);
                context.SaveChanges();
            }
        }

        public List<LabDto> GetLabsFromOrderNumber(List<string> ordernumbers)
        {
            var labs = context.Labs.Include("User2").Include("Portal").Where(x => ordernumbers.Contains(x.OrderNo.ToUpper())).ToList();
            if (labs != null)
            {
                List<string> oldOrderNo = new List<string>();
                List<DAL.Lab> newOrder = new List<Lab>();
                foreach (var lab in labs)
                {
                    if (!lab.Portal.Active)
                    {
                        DAL.Lab newLabOrder = context.Labs.Include("User2").Include("Portal").Where(x => x.UserId == lab.UserId && x.LabSelection == (int)LabChoices.LabCorp && x.Portal.Active).FirstOrDefault();
                        if (newLabOrder != null)
                        {
                            oldOrderNo.Add(lab.OrderNo);
                            //change the order #
                            newLabOrder.OrderNo = lab.OrderNo;
                            newOrder.Add(newLabOrder);
                        }
                    }
                }

                if (oldOrderNo.Count > 0)
                {
                    labs.RemoveAll(l => oldOrderNo.Contains(l.OrderNo));
                    LogReader reader = new LogReader();
                    string loggerName = "LabReader.GetLabsFromOrderNumber";
                    var logEvent = LogEventInfo.Create(LogLevel.Error, loggerName, "Labcorp sent results for old orderno " + String.Join(", ", oldOrderNo.ToArray()));
                    reader.WriteLogMessage(logEvent);
                    labs.AddRange(newOrder);
                }
                return Utility.mapper.Map<List<DAL.Lab>, List<LabDto>>(labs);
            }
            return null;
        }

        public LabDto CheckLabErrors(LabDto lab)
        {
            CommonReader commonReader = new CommonReader();
            List<string> errors = new List<string>();
            List<MeasurementsDto> measurements = commonReader.MeasurementRange();
            if (lab.SBP.HasValue)
            {
                if (lab.SBP.Value < measurements[(int)Measurements.SBP].Min || lab.SBP.Value > measurements[(int)Measurements.SBP].Max)
                {
                    errors.Add("SBP is not within range : " + lab.SBP.Value);
                    lab.SBP = null;
                    lab.DBP = null;
                }
            }
            if (lab.DBP.HasValue)
            {
                if (lab.DBP.Value < measurements[(int)Measurements.DBP].Min || lab.DBP.Value > measurements[(int)Measurements.DBP].Max)
                {
                    errors.Add("DBP is not within range : " + lab.DBP.Value);
                    lab.SBP = null;
                    lab.DBP = null;
                }
            }
            if (lab.TotalChol.HasValue)
            {
                if (lab.TotalChol.Value < measurements[(int)Measurements.Cholesterol].Min || lab.TotalChol.Value > measurements[(int)Measurements.Cholesterol].Max)
                {
                    errors.Add("TotalChol is not within range : " + lab.TotalChol.Value);
                    lab.TotalChol = null;
                }
                else if (lab.HDL.HasValue && lab.LDL.HasValue)
                {
                    if ((lab.HDL.Value + lab.LDL.Value) > lab.TotalChol.Value)
                    {
                        errors.Add("Total Cholesterol should be greater than sum of LDL and HDL : " + lab.TotalChol.Value);
                        lab.TotalChol = null;
                    }
                    else if (lab.HDL.Value >= lab.TotalChol.Value || lab.LDL.Value >= lab.TotalChol.Value)
                    {
                        errors.Add("Total Cholesterol should be greater than sum of LDL and HDL : " + lab.TotalChol.Value);
                        lab.TotalChol = null;
                    }
                }
            }
            if (lab.Trig.HasValue)
            {
                if (lab.Trig.Value < measurements[(int)Measurements.Triglycerides].Min || lab.Trig.Value > measurements[(int)Measurements.Triglycerides].Max)
                {
                    errors.Add("Trig is not within range : " + lab.Trig.Value);
                    lab.Trig = null;
                }
                else if (lab.TotalChol.HasValue && lab.Trig.Value <= measurements[(int)Measurements.Triglycerides].Limit && (lab.Trig.Value / 5) >= lab.TotalChol.Value)
                {
                    errors.Add("Triglyceride divided by 5 must always be less than total cholesterol : " + lab.Trig.Value);
                    lab.Trig = null;
                }
            }
            if (lab.HDL.HasValue)
            {
                if (lab.HDL.Value < measurements[(int)Measurements.HDL].Min || lab.HDL.Value > measurements[(int)Measurements.HDL].Max)
                {
                    errors.Add("HDL is not within range : " + lab.HDL.Value);
                    lab.HDL = null;
                }
            }
            if (lab.LDL.HasValue)
            {
                if (lab.LDL.Value < measurements[(int)Measurements.LDL].Min || lab.LDL.Value > measurements[(int)Measurements.LDL].Max)
                {
                    errors.Add("LDL is not within range : " + lab.LDL.Value);
                    lab.LDL = null;
                }
            }
            if (lab.Glucose.HasValue)
            {
                if (lab.Glucose.Value < measurements[(int)Measurements.Glucose].Min || lab.Glucose.Value > measurements[(int)Measurements.Glucose].Max)
                {
                    errors.Add("Glucose is not within range : " + lab.Glucose.Value);
                    lab.Glucose = null;
                }
            }
            if (lab.A1C.HasValue)
            {
                if (lab.A1C.Value < measurements[(int)Measurements.A1C].Min || lab.A1C.Value > measurements[(int)Measurements.A1C].Max)
                {
                    errors.Add("A1C is not within range : " + lab.A1C.Value);
                    lab.A1C = null;
                }
            }
            if (lab.Height.HasValue)
            {
                if (lab.Height.Value < measurements[(int)Measurements.Height].Min * 12 || lab.Height.Value > measurements[(int)Measurements.Height].Max * 12)
                {
                    errors.Add("Height is not within range : " + lab.Height.Value);
                    lab.Height = null;
                }
            }
            if (lab.Weight.HasValue)
            {
                if (lab.Weight.Value < measurements[(int)Measurements.Weight].Min || lab.Weight.Value > measurements[(int)Measurements.Weight].Max)
                {
                    errors.Add("Weight is not within range : " + lab.Weight.Value);
                    lab.Weight = null;
                }
            }
            if (errors.Count > 0)
            {
                LabReader labReader = new LabReader();
                LogLabErrorsRequest request = new LogLabErrorsRequest();
                request.Errors = errors;
                request.portalId = lab.PortalId;
                request.userId = lab.UserId;
                labReader.LogLabErrors(request);
            }
            return lab;
        }

        public void BulkAddLabOrders(List<LabDto> labRequests)
        {
            foreach (var lab in labRequests)
            {
                var labWork = new DAL.Lab();
                labWork.LabSelection = lab.LabSelection;
                labWork.PortalId = lab.PortalId;
                labWork.UserId = lab.UserId;
                labWork.OrderNo = lab.OrderNo;
                labWork.CreatedBy = lab.CreatedBy;
                labWork.CreatedOn = DateTime.UtcNow;
                context.Labs.Add(labWork);
                context.SaveChanges();
            }
        }

        public List<DiagnosticLab> GetDiagnosticLabs()
        {
            DiagnosticLab labDetails = new DiagnosticLab();
            List<DiagnosticLab> lstDiagnosticLab = new List<DiagnosticLab>();
            return context.DiagnosticLabs.ToList();
        }
    }
}
