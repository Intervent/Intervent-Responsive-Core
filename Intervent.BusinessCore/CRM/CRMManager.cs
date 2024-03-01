using Five9Service;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Intervent.Business
{
    public class CRMManager : BaseManager
    {
        private static List<CRM_DispositionsDto> dispositions;
        private static int _intuityOrgId = Convert.ToInt32(ConfigurationManager.AppSettings["IntuityOrgId"]);
        private static int SystemAdminId = Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]);
        private static string DTCOrgCode = ConfigurationManager.AppSettings["DTCOrgCode"];

        public int GetIntuityInboundOutboundCallData()
        {
            var inboundCount = 0;
            var updatedCount = 0;
            CRMReader reader = new CRMReader();
            try
            {
                using (WsAdminClient client = new WsAdminClient())
                {
                    using (new OperationContextScope(client.InnerChannel))
                    {
                        HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                        httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes((System.Configuration.ConfigurationManager.AppSettings["Authorization"])));
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                        customReportCriteria customReportCriteria = new customReportCriteria();
                        customReportCriteria.time = new reportTimeCriteria();
                        customReportCriteria.time.startSpecified = true;
                        customReportCriteria.time.start = DateTime.UtcNow.AddDays(-1);
                        customReportCriteria.time.endSpecified = true;
                        customReportCriteria.time.end = DateTime.UtcNow;

                        runReport runInboundReport = new runReport();
                        runInboundReport.reportName = "Intuity Inbound Call Log - Full Information";
                        runInboundReport.folderName = "Intuity";
                        runInboundReport.criteria = customReportCriteria;
                        string res = client.runReport(runInboundReport.folderName, runInboundReport.reportName, runInboundReport.criteria);
                        var reportRunning = true;
                        dispositions = reader.GetDispositionsList().CRM_Dispositions;
                        List<CRM_NoteDto> CRM_Notes = new List<CRM_NoteDto>();

                        while (reportRunning)
                        {
                            reportRunning = client.isReportRunning(res, 0);
                        }
                        if (!reportRunning)
                        {
                            reportRowData response = client.getReportResult(res);
                            if (response.records == null)
                                return 0;
                            var records = response.records.ToList();
                            inboundCount = records.Count();

                            if (inboundCount > 0)
                            {
                                CRM_Notes = GetCRMNotes(records);
                            }
                        }

                        CRM_Notes = CRM_Notes.OrderByDescending(n => n.ContactId).ThenBy(n => n.CallId).ThenBy(n => n.CallTime).ToList();

                        foreach (CRM_NoteDto note in CRM_Notes)
                        {
                            if (note.CallType.ToUpper() == "QUEUE CALLBACK" || note.Disposition == 78 ||
                                (note.CallType.ToUpper() == "MANUAL" && !note.Disposition.HasValue) ||
                                note.CallType.ToUpper() == "3RD PARTY TRANSFER" || note.CallType.ToUpper() == "3RD PARTY CONFERENCE")
                                continue;
                            GetCRMNotesRequest noteRequest = new GetCRMNotesRequest();
                            noteRequest.CallId = note.CallId;
                            var existNotes = reader.GetCRMNoteId(noteRequest);
                            if (existNotes != null && existNotes.Count() > 0)
                            {
                                var firstNote = existNotes.FirstOrDefault();
                                if (note.Disposition == 64 && existNotes.Where(x => x.Disposition == 75).Count() > 0)
                                {
                                    var first75Note = existNotes.Where(x => x.Disposition == 75).FirstOrDefault();
                                    note.Id = first75Note.Id;
                                    note.ContactId = first75Note.ContactId;
                                }
                                else if (note.Survey1 != firstNote.Survey1 || note.Survey2 != firstNote.Survey2 || note.Survey3 != firstNote.Survey3 || note.Survey4 != firstNote.Survey4)
                                {
                                    note.Id = firstNote.Id;
                                    note.ContactId = firstNote.ContactId;
                                }
                                else if (existNotes.Count >= 1 && (firstNote.Disposition == note.Disposition))
                                    continue;
                                else if (note.Disposition != 75 && existNotes.Where(x => x.Disposition == 75).Count() > 0)
                                {
                                    var first75Note = existNotes.Where(x => x.Disposition == 75).FirstOrDefault();
                                    note.Id = first75Note.Id;
                                    note.ContactId = first75Note.ContactId;
                                }
                                else if (note.Disposition == 49 && firstNote.Disposition != 49)
                                {
                                    note.Id = firstNote.Id;
                                    note.ContactId = firstNote.ContactId;
                                }
                            }
                            if (note.ContactId == 0)
                            {
                                note.ContactId = GetOrAddContact(note, reader);
                            }
                            //check call back exists
                            var callbackNote = CRM_Notes.Where(x => x.CallId == note.CallId && x.CallType.ToUpper() == "QUEUE CALLBACK").FirstOrDefault();
                            if (callbackNote != null)
                            {
                                note.TimeStamp = callbackNote.TimeStamp;
                                note.SpeedofAnswer = callbackNote.SpeedofAnswer;
                                note.QueueWaitTime = callbackNote.QueueWaitTime;
                                note.CallType = callbackNote.CallType;
                                note.CallTime = callbackNote.CallTime;
                                note.TalkTime = callbackNote.TalkTime;
                                note.Holds = callbackNote.Holds;
                                note.HoldTime = callbackNote.HoldTime;
                                note.AfterCallWorkTime = callbackNote.AfterCallWorkTime;
                                note.HandleTime = callbackNote.HandleTime;
                                note.Abandoned = callbackNote.Abandoned;
                                note.AbandonRate = callbackNote.AbandonRate;
                                note.Conferences = callbackNote.Conferences;
                                note.Transfers = callbackNote.Transfers;
                                note.CreatedBy = callbackNote.CreatedBy;
                                note.Survey1 = callbackNote.Survey1;
                                note.Survey2 = callbackNote.Survey2;
                                note.Survey3 = callbackNote.Survey3;
                                note.Survey4 = callbackNote.Survey4;
                            }
                            if (note.Disposition != 53 && note.Disposition != 43 && note.Disposition != 8 && note.Id == 0) //insert voicemail processed, Caller disconnected and sent to voicemail as new record
                            {
                                noteRequest = new GetCRMNotesRequest();
                                noteRequest.ContactId = note.ContactId;
                                noteRequest.CallTime = note.TimeStamp;
                                var currentNotes = reader.GetCRMNoteId(noteRequest);
                                if (currentNotes != null && currentNotes.Count() > 0)
                                    note.Id = currentNotes.FirstOrDefault().Id;
                            }
                            if (note.Id == 0)
                            {
                                GetCRMContactRequest contactRequest = new GetCRMContactRequest();
                                contactRequest.contactId = note.ContactId;
                                var crmContact = reader.GetContact(contactRequest);
                                if (crmContact.CRM_Contact == null || crmContact.CRM_Contact.Id == 0)
                                {
                                    note.ContactId = GetOrAddContact(note, reader);
                                }
                            }
                            AddUpdateCRMNoteRequest addUpdateNote = new AddUpdateCRMNoteRequest();
                            addUpdateNote.CRM_Note = note;
                            addUpdateNote.systemAdminId = SystemAdminId;
                            reader.AddOrUpdateCRMNotes(addUpdateNote, DTCOrgCode);
                            updatedCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }

            return updatedCount;
        }

        public static List<CRM_NoteDto> GetCRMNotes(List<record1> records)
        {
            List<CRM_NoteDto> notes = new List<CRM_NoteDto>();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            foreach (var cell in records)
            {
                CRM_NoteDto newNote = new CRM_NoteDto();

                newNote.CallId = Convert.ToInt64(cell.values[3]);
                newNote.TimeStamp = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(cell.values[4] + " " + cell.values[5]), custTZone);
                if (ValidateCell(cell.values[7]))
                    newNote.Ani = new string(cell.values[7].Where(char.IsDigit).ToArray());
                if (ValidateCell(cell.values[8]))
                    newNote.Dnis = new string(cell.values[8].Where(char.IsDigit).ToArray());
                if (ValidateCell(cell.values[10]))
                    newNote.Campaign = cell.values[10];
                if (cell.values[11] != null)
                {
                    newNote.Calls = Convert.ToByte(cell.values[11]);
                }
                newNote.CallType = cell.values[12];
                if (newNote.CallType.ToUpper() == "MANUAL" || newNote.CallType.ToUpper() == "QUEUE CALLBACK")
                    newNote.CampaignType = 2;
                else
                    newNote.CampaignType = 1;
                if (ValidateCell(cell.values[13]))
                    newNote.SpeedofAnswer = TimeSpan.Parse(cell.values[13]);
                if (ValidateCell(cell.values[14]))
                    newNote.QueueWaitTime = TimeSpan.Parse(cell.values[14]);
                if (ValidateCell(cell.values[15]))
                    newNote.CallTime = TimeSpan.Parse(cell.values[15]);
                if (ValidateCell(cell.values[20]))
                    newNote.HoldTime = TimeSpan.Parse(cell.values[20]);
                if (ValidateCell(cell.values[21]))
                    newNote.AfterCallWorkTime = TimeSpan.Parse(cell.values[21]);
                if (ValidateCell(cell.values[22]))
                    newNote.HandleTime = TimeSpan.Parse(cell.values[22]);
                if (ValidateCell(cell.values[23]))
                    newNote.Abandoned = Convert.ToByte(cell.values[23]);
                if (ValidateCell(cell.values[24]))
                    newNote.AbandonRate = float.Parse(cell.values[24].Replace("%", ""));
                if (ValidateCell(cell.values[25]))
                    newNote.Conferences = (short)Convert.ToInt32(cell.values[25]);
                if (ValidateCell(cell.values[26]))
                    newNote.Transfers = (short)Convert.ToInt32(cell.values[26]);
                if (ValidateCell(cell.values[27]))
                    newNote.QueueCallBackWaitTime = TimeSpan.Parse(cell.values[27]);
                if (ValidateCell(cell.values[28]))
                    newNote.Voicemails = Convert.ToByte(cell.values[28]);
                if (ValidateCell(cell.values[29]))
                    newNote.VoicemailsHandleTime = TimeSpan.Parse(cell.values[29]);
                if (ValidateCell(cell.values[30]))
                    newNote.ContactId = Convert.ToInt32(cell.values[30]);
                if (ValidateCell(cell.values[16]))
                    newNote.TalkTime = TimeSpan.Parse(cell.values[16]);
                if (ValidateCell(cell.values[18]))
                {
                    newNote.Disposition = dispositions.Where(d => d.Disposition.ToUpper().Equals(cell.values[18].Trim().ToUpper())).Select(d => d.Id).FirstOrDefault();
                    newNote.EligibleforActivity = dispositions.Where(d => d.Disposition.ToUpper().Equals(cell.values[18].Trim().ToUpper())).Select(d => d.EligibleforActivity).FirstOrDefault();
                }
                if (ValidateCell(cell.values[19]))
                    newNote.Holds = Convert.ToByte(cell.values[19]);
                if (ValidateCell(cell.values[45]))
                    newNote.Survey1 = Convert.ToByte(cell.values[45]);
                if (ValidateCell(cell.values[46]))
                    newNote.Survey2 = Convert.ToByte(cell.values[46]);
                if (ValidateCell(cell.values[47]))
                    newNote.Survey3 = Convert.ToByte(cell.values[47]);
                if (ValidateCell(cell.values[48]))
                    newNote.Survey4 = Convert.ToByte(cell.values[48]);
                //Contact
                CRM_ContactDto contact = new CRM_ContactDto();
                if (ValidateCell(cell.values[32]))
                    contact.PhoneNumber1 = new string(cell.values[32].Where(char.IsDigit).ToArray());
                if (ValidateCell(cell.values[33]))
                    contact.PhoneNumber2 = new string(cell.values[33].Where(char.IsDigit).ToArray());
                if (ValidateCell(cell.values[34]))
                    contact.PhoneNumber3 = new string(cell.values[34].Where(char.IsDigit).ToArray());
                if (ValidateCell(cell.values[35]))
                    contact.FirstName = cell.values[35];
                if (ValidateCell(cell.values[36]))
                    contact.LastName = cell.values[36];
                contact.CreatedOn = newNote.TimeStamp.Value;
                if (ValidateCell(cell.values[38]))
                    contact.Email = cell.values[38];
                if (ValidateCell(cell.values[39]))
                    contact.PogoMeterNumber = cell.values[39];
                if (ValidateCell(cell.values[40]))
                    contact.DOB = Convert.ToDateTime(cell.values[40]);
                if (ValidateCell(cell.values[41]))
                    contact.Address = cell.values[41];
                if (ValidateCell(cell.values[42]))
                    contact.City = cell.values[42];
                if (ValidateCell(cell.values[43]))
                {
                    var state = new CommonReader().GetState(new GetStateRequest() { stateCode = cell.values[43] }).state;
                    if (state != null)
                        contact.State = state.Id;
                }
                if (ValidateCell(cell.values[44]))
                    contact.Zip = cell.values[44];
                var cell0 = cell.values[0];
                var createdBy = new AccountReader().GetUserIdByNameAsync(cell0).Result;
                if (createdBy.HasValue)
                    newNote.CreatedBy = createdBy.Value;
                else
                    newNote.CreatedBy = SystemAdminId;
                newNote.CRM_Contacts = contact;
                notes.Add(newNote);
            }
            return notes;
        }

        public static int GetOrAddContact(CRM_NoteDto note, CRMReader reader)
        {
            GetContactByEmailorPhoneNoRequest phoneNoRequest = new GetContactByEmailorPhoneNoRequest();
            phoneNoRequest.phoneNo = note.CampaignType.Equals(1) ? note.Ani : note.Dnis;
            var contact = reader.GetContactByEmailorPhoneNo(phoneNoRequest).CRM_Contact;

            if (contact == null || contact.Id == 0)
            {
                AddEditCRMProfileRequest addRequest = new AddEditCRMProfileRequest();
                note.CRM_Contacts.OrganizationId = 67;
                if (string.IsNullOrEmpty(note.CRM_Contacts.FirstName))
                    note.CRM_Contacts.FirstName = "First" + note.CallId.Value.ToString();
                if (string.IsNullOrEmpty(note.CRM_Contacts.LastName))
                    note.CRM_Contacts.LastName = "Last" + note.CallId.Value.ToString();
                if (string.IsNullOrEmpty(note.CRM_Contacts.PhoneNumber1))
                    note.CRM_Contacts.PhoneNumber1 = note.CampaignType.Equals(1) ? note.Ani : note.Dnis;
                if (note.CRM_Contacts.OptedIn == 0)
                    note.CRM_Contacts.OptedIn = 3;
                addRequest.crm_Contact = note.CRM_Contacts;
                addRequest.userId = SystemAdminId;
                addRequest.intuityOrgId = _intuityOrgId;
                return reader.AddEditCRMProfile(addRequest).contactId;
            }
            return contact.Id;
        }

        public static bool ValidateCell(string value)
        {
            return !string.IsNullOrEmpty(value) && !value.Equals("-") && !value.Equals("--");
        }
    }
}