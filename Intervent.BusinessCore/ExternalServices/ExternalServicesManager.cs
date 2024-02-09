using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public class ExternalServicesManager : BaseManager
    {
        public int PullLivongoData()
        {
            string AuthorizationKey = ConfigurationManager.AppSettings["LivongoAuthorizationKey"];
            string livongoURL = ConfigurationManager.AppSettings["LivongoURL"];

            int count = -1;
            String debug = "";
            LogReader logReader = new LogReader();
            try
            {
                var pullResponse = Task.Run(() => Livongo.PullData(AuthorizationKey, livongoURL));
                var extGlucose = pullResponse.Result;
                List<string> ackList;
                List<EXT_Glucose> glucose = ConvertData(extGlucose.Response, out ackList);
                debug += extGlucose.Response.entry.Length + "test" + glucose.Count;
                //var logEvent1 = new LogEventInfo(LogLevel.Error, "Service", null, debug, null, null);
                //logReader.WriteLogMessage(logEvent1);
                if (extGlucose.Response != null && extGlucose.Response.entry != null && extGlucose.Response.entry.Count() > 0)
                {
                    count = glucose.Count;
                    ExternalReader reader = new ExternalReader();
                    //# TODO : Disabled as this service is currently not in use
                    //reader.BulkAddGlucose(glucose);
                    var ackResponse = Task.Run(() => Livongo.AckData(extGlucose.Request, ackList, AuthorizationKey, livongoURL));
                    //var logEvent2 = new LogEventInfo(LogLevel.Error, "Service1", null, debug + isSuccess, null, null);
                    //logReader.WriteLogMessage(logEvent2);
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message, ex);
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return count;
        }

        private List<EXT_Glucose> ConvertData(LivongoRootobject livongoResponse, out List<string> ackList)
        {
            List<EXT_Glucose> glucose = new List<EXT_Glucose>();
            int compassOrgId = 47;
            int CrothallOrgId = 48;
            ackList = new List<string>();
            if (livongoResponse != null && livongoResponse.entry != null && livongoResponse.entry.Count() > 0)
            {
                foreach (var response in livongoResponse.entry)
                {
                    if (response.resource != null && response.resource.contained != null && response.resource.valueQuantity != null)
                    {
                        EXT_Glucose value = new EXT_Glucose();
                        foreach (var contained in response.resource.contained)
                        {
                            foreach (var id in contained.identifier)
                            {
                                if (id.system.ToLower().Contains("partnerexternalid"))
                                {
                                    value.UniqueId = id.value;
                                }
                            }
                        }

                        foreach (var ext in response.resource.extension)
                        {
                            if (ext.url.ToLower().Contains("metadestinationid"))
                            {
                                if (ext.valueString.ToUpper().Contains("COMPASS"))
                                    value.OrganizationId = compassOrgId;
                                else if (ext.valueString.ToUpper().Contains("CROTHALL"))
                                    value.OrganizationId = CrothallOrgId;
                            }
                        }
                        value.Code = "";
                        foreach (var code in response.resource.code.coding)
                        {
                            value.Code = value.Code + "~" + code.code;
                        }

                        if (!string.IsNullOrEmpty(value.Code))
                            value.Code = value.Code.Remove(0, 1);

                        value.EffectiveDateTime = response.resource.effectiveDateTime;
                        value.DateTime = DateTime.UtcNow;
                        value.Unit = response.resource.valueQuantity.unit;
                        value.Value = response.resource.valueQuantity.value;
                        value.Source = (byte)GlucoseSource.Livongo;
                        if (value.Value > 0)
                        {
                            value.IsValid = true;
                        }
                        else
                        {
                            value.IsValid = false;
                        }
                        if (!string.IsNullOrEmpty(value.UniqueId) && value.OrganizationId.HasValue)
                        {
                            glucose.Add(value);
                        }
                        ackList.Add(response.fullUrl);
                    }
                }
            }
            return glucose;
        }
    }
}
