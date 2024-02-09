using Intervent.Framework.Exceptions.Claims;

using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Intervent.Business.Claims
{
    public class AutomateSqlLoad
    {
        public static void ProcessSql(IEnumerable<string> filePaths, string cs)
        {
            try
            {
                Console.WriteLine("Automating SQL");
                //check file paths
                foreach (string filePath in filePaths)
                {
                    if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    {
                        throw new AutomateClaimException($"File path {filePath} is not valid");
                    }
                }

                //check connection string is valid
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    try
                    {
                        conn.Open(); // throws if invalid
                    }
                    catch (Exception)
                    {

                        throw new AutomateClaimException("Could not connect to the database"); ;
                    }
                    finally
                    {
                        if (conn.State != System.Data.ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                }

                //file to keep track of last processed record in case of failures
                string trackFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trackfile" + DateTime.Now.ToString("MMyyyy") + ".json");
                if (!File.Exists(trackFilePath))
                {
                    var trackLastProcessRecords1 = new List<TrackFile>();
                    foreach (string filePath in filePaths)
                    {
                        var t = new TrackFile();
                        t.FileName = filePath;
                        t.LastProcessedRecord = 0;
                        t.IsCompleted = false;
                        trackLastProcessRecords1.Add(t);
                    }

                    File.WriteAllText(trackFilePath, JsonConvert.SerializeObject(trackLastProcessRecords1));
                }

                //load the tracking files
                var trackLastProcessRecords = JsonConvert.DeserializeObject<List<TrackFile>>(File.ReadAllText(trackFilePath));
                foreach (var rec in trackLastProcessRecords)
                {
                    Console.WriteLine($"begin processing {Path.GetFileName(rec.FileName)}");
                    //if completed skip file
                    if (rec.IsCompleted)
                    {
                        Console.WriteLine("File previously run.");
                        continue;
                    }

                    using (var connection = new SqlConnection(cs))
                    {
                        try
                        {
                            /*var server = new Server(new ServerConnection(connection));
                            server.ConnectionContext.ExecuteNonQuery(File.ReadAllText(rec.FileName));
                            rec.IsCompleted = true;
                            Console.WriteLine($"begin processing {Path.GetFileName(rec.FileName)}");*/
                        }
                        catch (Exception ex)
                        {
                            string fn = Path.GetFileNameWithoutExtension(rec.FileName);
                            File.WriteAllText(rec.FileName.Replace(fn, fn + "log"), JsonConvert.SerializeObject(ex));
                            throw;
                        }
                        finally
                        {
                            File.WriteAllText(trackFilePath, JsonConvert.SerializeObject(trackLastProcessRecords));
                        }
                    }
                }

            }
            catch (AutomateClaimException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public class TrackFile
        {
            public string FileName { get; set; }

            public int LastProcessedRecord { get; set; }

            public bool IsCompleted { get; set; }
        }


    }
}
