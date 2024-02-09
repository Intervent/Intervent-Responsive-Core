using CsvHelper;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;

namespace Intervent.Business.CampaignServices
{
    public class GlucometerManager : BaseManager
    {
        ParticipantReader _participantReader = new ParticipantReader();
        public void LoadGluometerFiles(string glucometerDataFilePath)
        {
            try
            {
                var files = Directory.GetFiles(glucometerDataFilePath, "*.csv");
                foreach (var file in files)
                {
                    if (file != null)
                    {
                        List<GlucometerUserDto> listdto = new List<GlucometerUserDto>();
                        string uniqueid = null;
                        using (var sr = new StreamReader(file))
                        {
                            CsvReader reader = new CsvReader(sr);
                            reader.Configuration.Delimiter = ",";
                            reader.Configuration.IsHeaderCaseSensitive = false;
                            reader.Configuration.RegisterClassMap<GlucometerCsvClassMap>();
                            reader.Configuration.SkipEmptyRecords = true;
                            reader.Configuration.TrimFields = true;
                            reader.Configuration.TrimHeaders = true;
                            reader.Configuration.WillThrowOnMissingField = true;
                            while (reader.Read())
                            {
                                uniqueid = reader.GetField<string>("client_uid");
                                if (!String.IsNullOrEmpty(uniqueid))
                                {
                                    PortalReader reaader = new PortalReader();
                                    GlucometerUserDto model = new GlucometerUserDto();
                                    model.UniqueId = uniqueid;
                                    model.RegisteredDate = Convert.ToDateTime(reader.GetField<string>("registeredDate"));
                                    if (!String.IsNullOrEmpty(reader.GetField<string>("activationDate")))
                                        model.ActivationDate = Convert.ToDateTime(reader.GetField<string>("activationDate"));
                                    if (!String.IsNullOrEmpty(reader.GetField<string>("client_code")))
                                    {
                                        var client_code = reader.GetField<string>("client_code").ToLower();
                                        if (client_code.IndexOf("compass") > -1)
                                        {
                                            model.OrganizationId = 47;
                                        }
                                        else if (client_code.IndexOf("crothall") > -1)
                                        {
                                            model.OrganizationId = 48;
                                        }
                                    }
                                    listdto.Add(model);
                                }
                            }
                            if (listdto.Count() > 0)
                            {
                                _participantReader.AddGlucometerData(listdto);
                            }
                        }
                    }
                    string archiveDirectory = Path.Combine(glucometerDataFilePath, "ARCHIVE");
                    //create archive directory if directory does not exist
                    if (!System.IO.Directory.Exists(archiveDirectory))
                    {
                        System.IO.Directory.CreateDirectory(archiveDirectory);
                    }
                    string fileName = file.Split(new char[] { '\\' }).Last();
                    string destinationFile = Path.Combine(archiveDirectory, fileName.Replace(".", "." + DateTime.UtcNow.ToString("yyyyMMdd_HHmm") + "."));
                    File.Move(file, destinationFile);
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }
    }
}
