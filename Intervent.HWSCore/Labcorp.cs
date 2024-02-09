using Intervent.HWS.AuthenticationService;
using Intervent.HWS.OrderService;
using Intervent.HWS.ReportService;
using Intervent.HWS.Request;
using System.Globalization;

namespace Intervent.HWS
{
    public class Labcorp
    {
        public static readonly string ClientId = "Intervent";
        public static readonly string Npi = "1477659720";

        public static readonly string New = "NEW";
        public static readonly string Received = "RECEIVED";

        private static Dictionary<string, string> LabCorpSegmentMapping = new Dictionary<string, string>{
           {"001032", "GLUCOSE"}, //GLUCOSE, SERUM
            {"001065", "CHOLESTEROL"},
            {"001172", "TRIGLYCERIDES"},
            {"011817", "HDL"},
            {"012059", "LDL"},
            {"011916", "VLDL"},
            {"101148","HEIGHT"},
            {"101149","WEIGHT"},
            {"101150","BMI"},
            {"101151","WAIST"},
            {"101144","SYSTOLIC"},
            {"101145","DIASTOLIC"},
            {"101300","DATE"},
            {"DATE","101300"},
            {"001481","HEMOGLOBIN A1c"},
        };

        private static Dictionary<string, string> QuestSegmentMapping = new Dictionary<string, string>{
           {"25015300", "GLUCOSE"}, //GLUCOSE, SERUM
            {"25003000", "CHOLESTEROL"},
            {"25002900", "TRIGLYCERIDES"},
            {"25015900", "HDL"},
            {"25016900", "LDL"},
            {"312","HEIGHT"},
            {"97014703","WEIGHT"},
            {"310","BMI"},
           // {"101151","WAIST"},
            {"97014704","SYSTOLIC"},
            {"97014705","DIASTOLIC"},
            {"86002583","COTININE"},
            {"7432","DATE"},
            {"DATE","7432"},
        };

        private static Dictionary<string, string> DynacareSegmentMapping = new Dictionary<string, string>{
           {"111G", "GLUCOSE"}, //GLUCOSE, SERUM
            {"111H", "Non-Fasting GLUCOSE"},
            {"055M", "CHOLESTEROL"},
            {"243M", "TRIGLYCERIDES"},
            {"117M", "HDL"},
            {"LDLM", "LDL"},
            {"HT","HEIGHT"},
            {"WT","WEIGHT"},
            {"310","BMI"},
            {"WM","WAIST"},
            {"BPSYS","SYSTOLIC"},
            {"BPDIA","DIASTOLIC"},
            { "ARMB", "BPArm"},
            {"093B","HEMOGLOBIN A1c"},
            {"7432","DATE"},
            {"DATE","7432"},
        };

        public static LabOrderResponse PlaceLabOrder(LabCorpPlaceOrderRequest request)
        {
            LabOrderResponse orderResponse = new LabOrderResponse();
            string errMsg;
            string sessionId = StartSession(out errMsg, request.labCorpUserName, request.labCorpPassword);
            orderResponse.Error = errMsg;

            if (!string.IsNullOrEmpty(sessionId))
            {
                orderResponse = PlaceOrder(sessionId, request);
                EndSession(sessionId);
            }
            return orderResponse;
        }

        private static string StartSession(string labCorpUserName, string labCorpPassword)
        {
            string t;
            return StartSession(out t, labCorpUserName, labCorpPassword);
        }

        //StartSession may return 
        private static string StartSession(out string errMsg, string labCorpUserName, string labCorpPassword)
        {
            var retryCount = 0;
            errMsg = null;
            while (retryCount < 3)
            {
                try
                {
                    AuthenticationServiceClient authenticationClient = new AuthenticationServiceClient();
                    var response = authenticationClient.StartSession(labCorpUserName, labCorpPassword, ClientId);
                    if (response.ErrorId == 0 && !string.IsNullOrEmpty(response.SessionId))
                    {
                        return response.SessionId;
                    }
                }
                catch (Exception e) { errMsg = e.Message; }
                retryCount++;
            }

            return null;
        }

        //EndSession
        private static void EndSession(string sessionId)
        {
            try
            {
                AuthenticationServiceClient authenticationClient = new AuthenticationServiceClient();
                authenticationClient.EndSession(sessionId);
            }
            catch { }
        }

        #region PlaceOrder
        /// <summary>
        /// Places order to labcorp
        /// </summary>
        /// <param name="sessionId">session id</param>
        /// <param name="request">place order request</param>
        /// <returns>success/failed</returns>
        private static LabOrderResponse PlaceOrder(string sessionId, LabCorpPlaceOrderRequest request)
        {
            OrderServiceClient orderService = new OrderServiceClient();
            OrderService.PatientBase patient = new OrderService.PatientBase();
            OrderService.Guarantor gurantor = new Guarantor();
            OrderService.Clinician clinician = new OrderService.Clinician();
            Diagnosis diagnosis = new Diagnosis();

            gurantor.Dob = patient.Dob = request.DOB;
            gurantor.FirstName = patient.FirstName = request.FirstName;
            gurantor.LastName = patient.LastName = request.LastName;
            if (request.Gender == 1)
                gurantor.Gender = patient.Gender = "M";
            else
                gurantor.Gender = patient.Gender = "F";
            patient.PlacerPatientCode = patient.Code = request.PatientId;
            gurantor.City = patient.City = request.City;
            gurantor.State = patient.State = request.State;
            gurantor.StreetLine1 = patient.StreetLine1 = request.AddressLine1;
            gurantor.StreetLine2 = patient.StreetLine2 = request.AddressLine2;
            gurantor.Zip = patient.Zip = request.ZipCode;
            gurantor.Relationship = "1";

            clinician.Npi = Npi;
            clinician.FirstName = "Neil";
            clinician.LastName = "Gordon";

            diagnosis.Code = "Z02.6";
            diagnosis.Description = "Biometrics";
            diagnosis.DiagnosisType = DiagnosisType.Icd10;

            Diagnosis[] diagList = new Diagnosis[1];
            diagList[0] = diagnosis;

            RequestedProcedure[] procedureList = null;

            if (!string.IsNullOrEmpty(request.LabProcedures))
            {
                string[] labProcedures = request.LabProcedures.Split(',');
                procedureList = new RequestedProcedure[labProcedures.Count()];
                RequestedProcedureAnswer[] aoeAnswers = new RequestedProcedureAnswer[1];

                int i = 0;
                foreach (var labProcedure in labProcedures)
                {
                    var procedure = labProcedure.Split('|');
                    RequestedProcedure requestedProcedure = new RequestedProcedure();
                    requestedProcedure.Code = procedure[0];
                    if (procedure[1] != null && !String.IsNullOrEmpty(procedure[1]))
                    {
                        RequestedProcedureAnswer aoeAnswer = new RequestedProcedureAnswer();
                        aoeAnswer.AoeCode = procedure[1];
                        aoeAnswer.Answer = procedure[2];
                        aoeAnswers[0] = aoeAnswer;
                        requestedProcedure.AoeAnswers = aoeAnswers;
                    }
                    procedureList[i] = requestedProcedure;
                    i++;
                }
            }
            //max length is 20
            request.OrderNumber = request.OrderNumber.Substring(0, 19);
            LabOrderResponse orderresponse = new LabOrderResponse();
            var response = orderService.SubmitOrderWithBillOptionClient(sessionId, "LCA", request.OrderNumber, request.labCorpAccountNumber, "ROUTINE", false, request.Comments, new System.DateTimeOffset(DateTime.Now), new System.DateTimeOffset(DateTime.Now), patient, clinician, null, gurantor, diagList, procedureList, false);
            if (response.Status == OrderStatus.Successful)
            {
                orderresponse.Status = true;
            }
            else
            {
                orderresponse.Status = false;
                orderresponse.Error = response.ValidationError.Count() + " + " + response.ValidationError.FirstOrDefault().Message.ToString();
            }
            return orderresponse;

        }
        #endregion

        #region Read HL7 Report

        public static ReportQueryResponse GetHL7Reports(string status, string labCorpUserName, string labCorpPassword)
        {
            string sessionId = StartSession(labCorpUserName, labCorpPassword);
            ReportServiceClient reportService = new ReportServiceClient();

            var response = reportService.GetLatestReportAccessions2(sessionId, 20, status);
            EndSession(sessionId);
            return response;
        }

        public static LabResponse PullHL7Report(int accessionId, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            string sessionId = StartSession(labCorpUserName, labCorpPassword);
            LabResponse labResponse = null;
            ReportServiceClient reportService = new ReportServiceClient();

            var response = reportService.GetHL7Report(sessionId, accessionId);
            if (response != null && !string.IsNullOrEmpty(response.HL7))
            {
                labResponse = ConvertFromHL7(response.HL7, LabCorpSegmentMapping);
                labResponse.ReportData = GetReportData(accessionId, labCorpUserName, labCorpPassword, labCorpAccountNumber);
            }
            EndSession(sessionId);
            return labResponse;
        }

        private static byte[] GetReportData(int accessionId, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            try
            {
                string sessionId = StartSession(labCorpUserName, labCorpPassword);
                ReportServiceClient reportService = new ReportServiceClient();
                var result = reportService.GetReportFileByReportAccessionId(sessionId, labCorpAccountNumber, accessionId);
                EndSession(sessionId);
                if (result != null && result.File != null)
                    return result.File.Bytes;
            }
            catch { }
            return null;
        }

        public static byte[] PullOrderConfirmation(string orderNumber, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            OrderServiceClient orderService = new OrderServiceClient();
            var sessionId = StartSession(labCorpUserName, labCorpPassword);
            try
            {
                var order = orderService.GetOrderFileByHtiOrderNumber(sessionId, labCorpAccountNumber, orderNumber);
                return order.File.Bytes;
            }
            catch { }
            finally
            {
                EndSession(sessionId);
            }
            return null;
        }

        public static void PullOrderReport(string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            ReportServiceClient reportService = new ReportServiceClient();
            var sessionId = StartSession(labCorpUserName, labCorpPassword);
            var results = reportService.GetLatestReportAccessions(sessionId, labCorpAccountNumber, 6);
            for (int i = 0; i < results.SearchResult.Count(); i++)
            {
                var id = results.SearchResult[i].ReportAccessionId;
                var report = reportService.GetReportFileByReportAccessionId(sessionId, labCorpAccountNumber, id);
                using (var fs = new FileStream(id + "report.pdf", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(report.File.Bytes, 0, report.File.Bytes.Length);
                }
            }
            EndSession(sessionId);
            //return report.File.Bytes;
        }

        public static byte[] PullOrderReport(string orderNumber, string labCorpUserName, string labCorpPassword, string labCorpAccountNumber)
        {
            ReportServiceClient reportService = new ReportServiceClient();
            var sessionId = StartSession(labCorpUserName, labCorpPassword);
            var results = reportService.GetLatestReportAccessions(sessionId, labCorpAccountNumber, 6);
            for (int i = 0; i < results.SearchResult.Count(); i++)
            {
                if (results.SearchResult[i].PlacerOrderNumber.ToUpper().Equals(orderNumber.ToUpper()))
                {
                    var id = results.SearchResult[i].ReportAccessionId;
                    var report = reportService.GetReportFileByReportAccessionId(sessionId, labCorpAccountNumber, id);
                    return report.File.Bytes;
                }
            }
            EndSession(sessionId);
            return null;
        }

        public static List<LabResponse> LoadHL7File(string hl7Value, string filetype)
        {
            List<LabResponse> labResponse = new List<LabResponse>();
            var recordList = hl7Value.Split(new string[] { "MSH|" }, StringSplitOptions.None);
            foreach (var hl7 in recordList)
            {
                var mapping = filetype == "Quest" ? QuestSegmentMapping : DynacareSegmentMapping;
                var dynacareProcessing = filetype == "Quest" ? false : true;
                var response = ConvertFromHL7(hl7, mapping, dynacareProcessing);
                if (response != null
                    && (dynacareProcessing || ((!string.IsNullOrEmpty(response.UniqueId) && labResponse.Find(l => l.UniqueId == response.UniqueId) == null)
                    || (!string.IsNullOrEmpty(response.OrderId) && labResponse.Find(l => l.OrderId == response.OrderId) == null))))
                {
                    labResponse.Add(response);
                }
            }
            return labResponse;
        }

        private static LabResponse ConvertFromHL7(string hl7Value, Dictionary<string, string> SegmentMapping, bool dynacareProcessing = false)
        {
            LabResponse response = new LabResponse();
            response.HL7 = hl7Value;//.Substring(0, hl7Value.IndexOf("ZEF|"));
            try
            {
                string[] lines = hl7Value.Split(new string[] { "\r" }, StringSplitOptions.None);
                if (lines.Where(x => x.Length > 0).Count() == 0)
                {
                    return null;
                }
                foreach (string line in lines)
                {
                    if (line.Contains("OBR") && (line.Contains(SegmentMapping["DATE"]) || dynacareProcessing))
                    {
                        var lineItems = line.Split('|');
                        if (lineItems.Count() > 7)
                        {
                            string value = lineItems[7];
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (value.Length == 14)
                                    response.BloodTestDate = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                                else if (value.Length > 8)
                                    response.BloodTestDate = DateTime.ParseExact(value, "yyyyMMddHHmm", CultureInfo.InvariantCulture);

                            }
                        }
                    }
                    else if (line.Contains("OBX"))
                    {
                        var lineItems = line.Split('|');
                        if (lineItems.Count() > 5)
                        {
                            string code = lineItems[3].Split('^')[0];
                            string value = lineItems[5].TrimEnd('.');
                            float f;
                            if (SegmentMapping.ContainsKey(code) && float.TryParse(value, out f))
                            {
                                var name = SegmentMapping[code];
                                switch (name)
                                {
                                    case "GLUCOSE":
                                        response.Glucose = f;
                                        break;
                                    case "Non-Fasting GLUCOSE":
                                        response.Glucose = f;
                                        response.DidYouFast = 2;
                                        break;
                                    case "CHOLESTEROL":
                                        response.TotalChol = f;
                                        break;
                                    case "TRIGLYCERIDES":
                                        response.Trig = f;
                                        break;
                                    case "HDL":
                                        response.HDL = f;
                                        break;
                                    case "LDL":
                                        response.LDL = f;
                                        break;
                                    case "HEIGHT":
                                        response.Height = f;
                                        break;
                                    case "WEIGHT":
                                        response.Weight = f;
                                        break;
                                    case "BMI":
                                        response.BMI = f;
                                        break;
                                    case "WAIST":
                                        response.Waist = f;
                                        break;
                                    case "SYSTOLIC":
                                        response.SBP = short.Parse(value);
                                        break;
                                    case "DIASTOLIC":
                                        response.DBP = short.Parse(value);
                                        break;
                                    case "HEMOGLOBIN A1c":
                                        response.A1C = float.Parse(value);
                                        break;
                                }
                            }
                            else if (SegmentMapping.ContainsKey(code))
                            {
                                var name = SegmentMapping[code];
                                if (name == "COTININE")
                                {
                                    if (value == "POSITIVE")
                                        response.HighCotinine = true;
                                    else if (value == "NEGATIVE")
                                        response.HighCotinine = false;
                                }
                            }
                        }
                    }
                    else if (line.Contains("Z_FAST")) //Labcorp fasting
                    {
                        var lineItems = line.Split('|');
                        if (lineItems.Count() > 3)
                        {
                            string value = lineItems[3];
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (value == "Y")
                                {
                                    response.DidYouFast = 1;
                                }
                                else if (value == "N")
                                {
                                    response.DidYouFast = 2;
                                }
                            }
                        }
                    }
                    else if (line.Contains("NTE|") && !dynacareProcessing)//Quest fasting
                    {
                        var lineItems = line.Split('|');
                        if (lineItems.Count() >= 4 && lineItems[3].Contains("FASTING"))
                        {
                            var fasting = lineItems[3].Split(':');
                            if (fasting.Count() >= 2)
                            {
                                if (fasting[1] == "YES")
                                {
                                    response.DidYouFast = 1;
                                }
                                else
                                {
                                    response.DidYouFast = 2;
                                }
                            }
                        }
                    }
                    else if (line.Contains("PID|"))
                    {
                        var lineItems = line.Split('|');
                        if (lineItems.Count() >= 5)
                        {
                            response.UniqueId = lineItems[4];
                            var names = lineItems[5].Split('^');
                            response.FirstName = names[1];
                            response.LastName = names[0];
                            if (lineItems.Count() >= 8)
                                response.DOB = DateTime.ParseExact(lineItems[7], "yyyyMMdd", CultureInfo.InvariantCulture);
                        }
                    }
                    else if (line.Contains("ORC"))
                    {
                        var lineItems = line.Split('|');
                        if (lineItems.Count() > 9)
                        {
                            string value = lineItems[9];
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (value.Length == 14)
                                    response.BloodTestDate = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                                else if (value.Length > 8)
                                    response.BloodTestDate = DateTime.ParseExact(value, "yyyyMMddHHmm", CultureInfo.InvariantCulture);

                            }
                        }
                        if (dynacareProcessing && lineItems.Count() > 2)
                        {
                            // Dynacare Order Id
                            string code = lineItems[2].Split('^')[0];
                            response.OrderId = code;
                        }
                    }
                }
                if (response.Glucose.HasValue && dynacareProcessing && !response.DidYouFast.HasValue)
                    response.DidYouFast = 1;
            }
            catch { }
            return response;
        }

        #endregion
    }
}
