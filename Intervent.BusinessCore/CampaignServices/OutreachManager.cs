using Five9Service;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Intervent.Business
{
    public class OutreachManager : BaseManager
    {
        public void SendOutreachData()
        {
            ServiceReader reader = new ServiceReader();
            try
            {
                ListOutreachRequest request = new ListOutreachRequest();

                var response = reader.GetOutreachList(request);

                if (response.OutreachList != null && response.OutreachList.Count() > 0)
                {
                    #region fieldsforfive9

                    fieldEntry field1 = new fieldEntry();
                    field1.columnNumber = 1;
                    field1.fieldName = "number1";
                    field1.key = true;

                    fieldEntry field2 = new fieldEntry();
                    field2.columnNumber = 2;
                    field2.fieldName = "number2";
                    field2.key = false;

                    fieldEntry field3 = new fieldEntry();
                    field3.columnNumber = 3;
                    field3.fieldName = "number3";
                    field3.key = false;

                    fieldEntry field4 = new fieldEntry();
                    field4.columnNumber = 4;
                    field4.fieldName = "first_name";
                    field4.key = false;

                    fieldEntry field5 = new fieldEntry();
                    field5.columnNumber = 5;
                    field5.fieldName = "last_name";
                    field5.key = false;

                    fieldEntry field6 = new fieldEntry();
                    field6.columnNumber = 6;
                    field6.fieldName = "company";
                    field6.key = false;

                    fieldEntry field7 = new fieldEntry();
                    field7.columnNumber = 7;
                    field7.fieldName = "street";
                    field7.key = false;

                    fieldEntry field8 = new fieldEntry();
                    field8.columnNumber = 8;
                    field8.fieldName = "city";
                    field8.key = false;

                    fieldEntry field9 = new fieldEntry();
                    field9.columnNumber = 9;
                    field9.fieldName = "state";
                    field9.key = false;

                    fieldEntry field10 = new fieldEntry();
                    field10.columnNumber = 10;
                    field10.fieldName = "zip";
                    field10.key = false;

                    #endregion

                    for (int i = 0; i < response.OutreachList.Count; i++)
                    {
                        addRecordToList addRecordToList = new addRecordToList();

                        if (response.OutreachList[i].OrgId == 54)
                        {
                            addRecordToList.listName = "Web2Campaign_Riverview17";
                        }
                        else if (response.OutreachList[i].OrgId == 63)
                        {
                            addRecordToList.listName = "Web2Campaign_MacPapers18";
                        }
                        else if (response.OutreachList[i].OrgId == 2)
                        {
                            addRecordToList.listName = "Web2Campaign_INTERVENT";
                        }
                        else if (response.OutreachList[i].OrgId == 73)
                        {
                            addRecordToList.listName = "Web2Campaign_SouthUniversity";
                        }
                        else if (response.OutreachList[i].OrgId == 85)
                        {
                            addRecordToList.listName = "Web2Campaign_CityofPooler";
                        }
                        string[] participantRecord = new string[10];
                        participantRecord[0] = response.OutreachList[i].PhoneNumber1;
                        participantRecord[1] = "";
                        participantRecord[2] = "";
                        participantRecord[3] = response.OutreachList[i].FirstName;
                        participantRecord[4] = response.OutreachList[i].LastName;
                        participantRecord[5] = response.OutreachList[i].Company;
                        participantRecord[6] = response.OutreachList[i].Street;
                        participantRecord[7] = response.OutreachList[i].City;
                        participantRecord[8] = response.OutreachList[i].State;
                        participantRecord[9] = response.OutreachList[i].Zip;
                        addRecordToList.record = participantRecord;

                        addRecordToList.listUpdateSettings = new listUpdateSettings();
                        addRecordToList.listUpdateSettings.fieldsMapping = new fieldEntry[10];
                        addRecordToList.listUpdateSettings.fieldsMapping[0] = field1;
                        addRecordToList.listUpdateSettings.fieldsMapping[1] = field2;
                        addRecordToList.listUpdateSettings.fieldsMapping[2] = field3;
                        addRecordToList.listUpdateSettings.fieldsMapping[3] = field4;
                        addRecordToList.listUpdateSettings.fieldsMapping[4] = field5;
                        addRecordToList.listUpdateSettings.fieldsMapping[5] = field6;
                        addRecordToList.listUpdateSettings.fieldsMapping[6] = field7;
                        addRecordToList.listUpdateSettings.fieldsMapping[7] = field8;
                        addRecordToList.listUpdateSettings.fieldsMapping[8] = field9;
                        addRecordToList.listUpdateSettings.fieldsMapping[9] = field10;

                        addRecordToList.listUpdateSettings.cleanListBeforeUpdate = false;
                        addRecordToList.listUpdateSettings.crmAddMode = crmAddMode.ADD_NEW;
                        addRecordToList.listUpdateSettings.crmAddModeSpecified = true;
                        addRecordToList.listUpdateSettings.crmUpdateMode = crmUpdateMode.UPDATE_FIRST;
                        addRecordToList.listUpdateSettings.crmUpdateModeSpecified = true;
                        addRecordToList.listUpdateSettings.listAddMode = listAddMode.ADD_FIRST;
                        addRecordToList.listUpdateSettings.listAddModeSpecified = true;

                        using (WsAdminClient client = new WsAdminClient())
                        {
                            using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
                            {
                                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes((System.Configuration.ConfigurationManager.AppSettings["Authorization"])));
                                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                                listImportResult res = client.addRecordToList(addRecordToList.listName, addRecordToList.listUpdateSettings, addRecordToList.record);
                                //Log the records
                                if (res.crmRecordsInserted == 1 || res.crmRecordsUpdated == 1)
                                {
                                    LogOutreachRequest logRequest = new LogOutreachRequest();
                                    OutreachLogDto outreachLog = new OutreachLogDto();
                                    outreachLog = response.OutreachList[i];
                                    outreachLog.SentOn = DateTime.UtcNow;
                                    logRequest.OutreachLog = outreachLog;
                                    //need to do bulk insert
                                    reader.LogOutreach(logRequest);
                                }
                                else if (res.failureMessage != "")
                                {
                                    Log.Error(res.failureMessage);
                                }
                            }
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
        }

        public int GetOutReachandTrackingData()
        {
            var count = 0;
            try
            {
                using (WsAdminClient client = new WsAdminClient())
                {
                    using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
                    {
                        HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                        httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes((System.Configuration.ConfigurationManager.AppSettings["Authorization"])));
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                        customReportCriteria customReportCriteria = new customReportCriteria();
                        customReportCriteria.time = new reportTimeCriteria();
                        customReportCriteria.time.startSpecified = true;
                        customReportCriteria.time.start = DateTime.Now.AddDays(-1).Date;
                        customReportCriteria.time.endSpecified = true;
                        customReportCriteria.time.end = DateTime.Now.AddDays(-1).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        runReport runReport = new runReport();
                        runReport.reportName = "Call Log";
                        runReport.folderName = "Call Log Reports";
                        runReport.criteria = customReportCriteria;
                        string res = client.runReport(runReport.folderName, runReport.reportName, runReport.criteria);
                        var reportRunning = true;
                        while (reportRunning)
                        {
                            reportRunning = client.isReportRunning(res, 0);
                        }
                        if (!reportRunning)
                        {
                            reportRowData response = client.getReportResult(res);
                            var records = response.records.Where(x => (x.values[2].ToLower().Contains("outreach") ||
                                            x.values[2].ToLower().Contains("outbound") || x.values[2].ToLower().Contains("enrollment") ||
                                            x.values[2].ToLower().Contains("tracking")) && x.values[3].ToLower().Contains("autodial") && x.values[4].Contains("[None]")).ToList();
                            count = records.Count();
                            foreach (var call in records)
                            {
                                var type = call.values[2].ToLower();
                                byte notetype = 0;
                                AccountReader reader = new AccountReader();
                                if (type.Contains("outbound") || type.Contains("outreach") || type.Contains("enrollment"))
                                {
                                    notetype = 2;
                                }
                                else if (type.Contains("tracking"))
                                {
                                    notetype = 3;
                                }
                                if (notetype != 0)
                                {
                                    var userList = reader.GetUserByFullName(call.values[8]);
                                    if (userList != null && userList.Count > 0)
                                    {
                                        var user = userList.Where(x => (!string.IsNullOrEmpty(x.HomeNumber) && x.HomeNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").IndexOf(call.values[9]) > -1)
                                        || (!string.IsNullOrEmpty(x.WorkNumber) && x.WorkNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").IndexOf(call.values[9]) > -1)
                                        || (!string.IsNullOrEmpty(x.CellNumber) && x.CellNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").IndexOf(call.values[9]) > -1)).FirstOrDefault();
                                        if (user != null)
                                        {
                                            NotesDto note = new NotesDto();
                                            note.Admin = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SystemAdminId"]);
                                            note.userId = user.Id;
                                            note.PortalId = user.Organization.Portals.LastOrDefault().Id;
                                            note.Type = notetype;
                                            note.Text = "Five9 Call: " + call.values[6];
                                            note.NotesDate = Convert.ToDateTime(call.values[1]);
                                            ParticipantReader participantReader = new ParticipantReader();
                                            participantReader.AddEditNotes(new AddNotesRequest { note = note, TimeZone = "Pacific Standard Time" });
                                        }
                                    }
                                }
                            }
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
            return count;
        }

        public void SendCanriskOutreachData()
        {
            try
            {
                ServiceReader reader = new ServiceReader();

                var response = reader.GetCanriskOutreachList();

                if (response.OutreachList != null && response.OutreachList.Count() > 0)
                {
                    #region fieldsforfive9

                    fieldEntry field1 = new fieldEntry();
                    field1.columnNumber = 1;
                    field1.fieldName = "number1";
                    field1.key = true;

                    fieldEntry field2 = new fieldEntry();
                    field2.columnNumber = 2;
                    field2.fieldName = "first_name";
                    field2.key = false;

                    fieldEntry field3 = new fieldEntry();
                    field3.columnNumber = 3;
                    field3.fieldName = "last_name";
                    field3.key = false;

                    fieldEntry field4 = new fieldEntry();
                    field4.columnNumber = 4;
                    field4.fieldName = "company";
                    field4.key = false;

                    #endregion

                    for (int i = 0; i < response.OutreachList.Count; i++)
                    {
                        addRecordToList addRecordToList = new addRecordToList();

                        if (response.OutreachList[i].Language == "fr")
                            addRecordToList.listName = "Web2Campaign_Canrisk_French";
                        else
                            addRecordToList.listName = "Web2Campaign_Canrisk_English";

                        string[] participantRecord = new string[10];
                        participantRecord[0] = response.OutreachList[i].PhoneNumber1;
                        participantRecord[1] = response.OutreachList[i].FirstName;
                        participantRecord[2] = response.OutreachList[i].LastName;
                        participantRecord[3] = "LMC Clinical Trial";
                        addRecordToList.record = participantRecord;

                        addRecordToList.listUpdateSettings = new listUpdateSettings();
                        addRecordToList.listUpdateSettings.fieldsMapping = new fieldEntry[4];
                        addRecordToList.listUpdateSettings.fieldsMapping[0] = field1;
                        addRecordToList.listUpdateSettings.fieldsMapping[1] = field2;
                        addRecordToList.listUpdateSettings.fieldsMapping[2] = field3;
                        addRecordToList.listUpdateSettings.fieldsMapping[3] = field4;

                        addRecordToList.listUpdateSettings.cleanListBeforeUpdate = false;
                        addRecordToList.listUpdateSettings.crmAddMode = crmAddMode.ADD_NEW;
                        addRecordToList.listUpdateSettings.crmAddModeSpecified = true;
                        addRecordToList.listUpdateSettings.crmUpdateMode = crmUpdateMode.UPDATE_FIRST;
                        addRecordToList.listUpdateSettings.crmUpdateModeSpecified = true;
                        addRecordToList.listUpdateSettings.listAddMode = listAddMode.ADD_FIRST;
                        addRecordToList.listUpdateSettings.listAddModeSpecified = true;

                        using (WsAdminClient client = new WsAdminClient())
                        {
                            using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
                            {
                                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes((System.Configuration.ConfigurationManager.AppSettings["Authorization"])));
                                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                                listImportResult res = client.addRecordToList(addRecordToList.listName, addRecordToList.listUpdateSettings, addRecordToList.record);
                            }
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
        }
    }
}
