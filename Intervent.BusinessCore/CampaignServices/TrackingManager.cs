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
    public class TrackingManager : BaseManager
    {
        public int SendTrackingData()
        {
            ServiceReader reader = new ServiceReader();
            var count = 0;
            try
            {
                //Delete records in list
                using (WsAdminClient client = new WsAdminClient())
                {
                    using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
                    {
                        HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                        httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes((ConfigurationManager.AppSettings["Authorization"].ToString())));
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                        //Delete records in Intervent_Tracking_English
                        client.deleteAllFromList("Tracking.INTERVENT.English", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);

                        //Delete records in Intervent_Tracking_Spanish
                        client.deleteAllFromList("Tracking.INTERVENT.Spanish", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);

                        //Delete records in Tracking.Riverview-06.2017
                        client.deleteAllFromList("Tracking.Riverview-06.2017", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);

                        //Delete records in Tracking.SouthUniversity-05.2019
                        client.deleteAllFromList("Tracking.SouthUniversity-05.2019", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);

                        //Delete records in Tracking.CityofPooler
                        client.deleteAllFromList("Tracking.CityofPooler", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);

                        //Delete records in Tracking.eBen
                        client.deleteAllFromList("Tracking.eBen.English", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);
                        client.deleteAllFromList("Tracking.eBen.Spanish", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);

                        //Delete records in Tracking.CAPTIVA
                        client.deleteAllFromList("Tracking.CAPTIVA.English", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);
                        client.deleteAllFromList("Tracking.CAPTIVA.Spanish", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);

                        //Delete records in Tracking.MCI.English
                        client.deleteAllFromList("Tracking.MCI.English", ConfigurationManager.AppSettings["InfoEmail"].ToString(), ConfigurationManager.AppSettings["CallBackURL"].ToString(), callbackFormat.JSON);
                    }
                }

                ListOutreachRequest request = new ListOutreachRequest();

                var response = reader.GetTrackingList(request);
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
                        if (response.OutreachList[i].OrgId == 2)
                        {
                            if (response.OutreachList[i].Language == "es")
                                addRecordToList.listName = "Tracking.INTERVENT.Spanish";
                            else
                                addRecordToList.listName = "Tracking.INTERVENT.English";
                        }
                        else if (response.OutreachList[i].OrgId == 54)
                        {
                            addRecordToList.listName = "Tracking.Riverview-06.2017";
                        }
                        else if (response.OutreachList[i].OrgId == 73)
                        {
                            addRecordToList.listName = "Tracking.SouthUniversity-05.2019";
                        }
                        else if (response.OutreachList[i].OrgId == 85)
                        {
                            addRecordToList.listName = "Tracking.CityofPooler";
                        }
                        else if (response.OutreachList[i].OrgId == 92)
                        {
                            if (response.OutreachList[i].Language == "es")
                                addRecordToList.listName = "Tracking.eBen.Spanish";
                            else
                                addRecordToList.listName = "Tracking.eBen.English";
                        }
                        else if (response.OutreachList[i].OrgId == 90)
                        {
                            if (response.OutreachList[i].Language == "es")
                                addRecordToList.listName = "Tracking.CAPTIVA.Spanish";
                            else
                                addRecordToList.listName = "Tracking.CAPTIVA.English";
                        }
                        else if (response.OutreachList[i].OrgId == 98)
                        {
                            addRecordToList.listName = "Tracking.MCI.English";
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
                                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes((ConfigurationManager.AppSettings["Authorization"])));
                                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                                listImportResult res = client.addRecordToList(addRecordToList.listName, addRecordToList.listUpdateSettings, addRecordToList.record);
                                if (res.failureMessage != "")
                                {
                                    Log.Error(res.failureMessage);
                                }
                            }
                        }
                        count++;
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
    }
}
