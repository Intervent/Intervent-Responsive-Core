using Intervent.Business;
using Intervent.Business.Eligibility;
using Intervent.Business.EmailTriggers;
using Intervent.Business.Notification;
using Intervent.Business.FollowUp;
using Intervent.Web.DataLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Intervent.Web.DTO;
using Intervent.Business.CampaignServices;
using Intervent.Business.Claims;
using Intervent.Business.Account;
using Intervent.Business.Mail;
using Intervent.Business.Tasks;

namespace EligibilityConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Utility.InitMapper();
            //KitReader reader = new KitReader();
            //reader.CloneKit(38);
            //forcing the assembly to load to resolve edmx issues
            Assembly.Load("Intervent.DALCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

            //new IntuityEligibilityManager().ProcessIntuityEligibility();
            //new IntuityEligibilityManager().ProcessIntuityEligibilityNoOptIn();
            //new IntuityEligibilityManager().ConstructFullfilmentFile();
            //new IntuityEligibilityManager().ConstructEligibilityFulfilmentFile();
            //new IntuityEligibilityManager().ProcessShipment();
            new IntuityEligibilityManager().ProcessFailedIntuityEligibilityRecords();

            //KitReader reader = new KitReader();
            //reader.SerializeNow(reader.ExportKit(87));
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3;
            //new AccountManager().DeleteOldPictures();
            //new LabcorpServicesManager().PullLabValues();
            //new FollowUpManager().AutomateFollowUp();
            //new ExternalServicesManager().PullLivongoData();
            //new IntuityManager().SendNoShow(1020, DateTime.UtcNow);
            //new IntuityManager().SendEligibilityUpdate(46);
            //new EligibilityManager().LoadEligibilityFiles();
            //new LabManager().SendLabRequest();
            //new LabManager().ProcessDynacareLabs();
            //new EmailTriggerManager().ProcessEmailTriggers();
            //new OutreachEmailManager().IncompleteHRA();
            //new OutreachEmailManager().IncompleteProfile();
            //new NotificationManager().ProcessQueuedEvents();
            //new OutreachEmailManager().SystemDowntime("from 11:00pm to 11:59pm on March 15th, 2020");
            //new OutreachManager().SendOutreachData();
            //new AppointmentManager().SendAppointmentList();B
            //new TrackingManager().SendTrackingData();
            //new OutreachManager().SendOutreachData();
            //new OutreachManager().GetOutReacData();
            //new OutreachManager().GetOutReachandTrackingData();
            //new AppointmentManager().PullTextResponse();
            //new RecipeManager().SendRecipe();  
            //new IncentiveManager().ProcessIncentives();
            //new GlucometerManager().LoadGluometerFiles();
            //new LabcorpServicesManager().PullLabValues("NEW");
            //var response = new HraManager().GetHRAAssessment("218");
            //string json = JsonConvert.SerializeObject(response);
            //System.IO.File.WriteAllText(@"c:\intervent\hraquestions.txt", json);
            //saveorderfile();
            // savereportfile();
            //new HraManager().SaveHRAQuestion(new APISaveHRAQuestionRequest());
            //new ClaimManager().ProcessClaims();
            // new TaskManager().CreateTaskforMissedAppt();
            //CheckifExcelInstalled();
            //var list =  new AppointmentManager().SendSms();
            //new IntuityManager().SendReminderList(list);
            //testLab();
            //new ExternalReader().TerminateExpiredUser(44);

            //List<string> inputFilePaths = new List<string>();
            //inputFilePaths.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Claims", "finalSqlBegin.txt"));
            //inputFilePaths.Add(@"C:\Intervent\ClaimsProcess\201806\insuranceSummarySql.txt");
            //inputFilePaths.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Claims", "finalSqlEnd.txt"));
            //var finalSqlFilePath = @"C:\Intervent\ClaimsProcess\TestAutomation.txt";

            //string contents = "";
            //foreach (var inputFilePath in inputFilePaths)
            //{
            //    contents += File.ReadAllText(inputFilePath);
            //}

            //File.WriteAllText(finalSqlFilePath, contents);

            //List<string> files = new List<string>();
            ////string basePath
            //        files.Add(finalSqlFilePath);
            //string cs = ConfigurationManager.ConnectionStrings["InterventDatabase"].ConnectionString;
            //AutomateSqlLoad.ProcessSql(files, cs);

            //if (args != null && args.Count() > 0)
            //{
            //    if (args[0] == "AutomateSql")
            //    {
            //        var files = File.ReadLines(args[1]);
            //        string cs = ConfigurationManager.ConnectionStrings["InterventDatabase"].ConnectionString;
            //        AutomateSqlLoad.ProcessSql(files, cs);
            //    }
            //    else if(args[0] == "SplitFiles")
            //    {
            //        Splitfiles();
            //    }
            //}

        }

        static void Splitfiles()
        {
            List<string> SQLinputFilePaths = new List<string>();
            SQLinputFilePaths.Add(@"C:\ClaimsSplit\AETNAsqlFile.txt");
            SQLinputFilePaths.Add(@"C:\ClaimsSplit\BCBSsqlFile.txt");
            SQLinputFilePaths.Add(@"C:\ClaimsSplit\CVSsqlFile.txt");
            SQLinputFilePaths.Add(@"C:\ClaimsSplit\UHCsqlFile.txt");
            var i = 0;

            List<string> finalSQLinputFilePaths = new List<string>();
            int maxLines = 20000;
            int fileSequence = 0;
            string finalSqlFilePath = null;
            string beginFilePath = @"C:\ClaimsSplit\finalSqlBegin.txt";// Path.Combine("C:", "ClaimsSplit", "finalSqlBegin.txt");
            string endFilePath = @"C:\ClaimsSplit\finalSqlEnd.txt";//Path.Combine("C:", "ClaimsSplit", "finalSqlEnd.txt");
            foreach (var sqlfilepath in SQLinputFilePaths)
            {
                StreamWriter writer = null;
                try
                {
                    using (StreamReader inputfile = new System.IO.StreamReader(sqlfilepath))
                    {
                        int count = 0;
                        string line;

                        while ((line = inputfile.ReadLine()) != null)
                        {

                            if (writer == null || count >= maxLines)
                            {
                                if (writer != null)
                                {
                                    //wait until the next SET
                                    if (!line.TrimStart().StartsWith("SET"))
                                    {
                                        writer.WriteLine(line);
                                        string tempLine = null;
                                        while ((tempLine = inputfile.ReadLine()) != null)
                                        {
                                            if (tempLine.TrimStart().StartsWith("SET"))
                                            {
                                                line = tempLine;
                                                break;
                                            }
                                            else
                                            {
                                                writer.WriteLine(tempLine);
                                            }
                                        }
                                    }
                                    //write the end of file content
                                    using (StreamReader endFile = new System.IO.StreamReader(endFilePath))
                                    {
                                        string endLine;

                                        while ((endLine = endFile.ReadLine()) != null)
                                        {
                                            writer.WriteLine(endLine);
                                        }
                                    }
                                    writer.Close();
                                    
                                    writer = null;
                                    fileSequence++;
                                }
                                finalSqlFilePath = Path.Combine(@"C:\ClaimsSplit\FinalFiles", $"finalInsuranceSummarySql" + fileSequence + ".txt");
                                writer = new System.IO.StreamWriter(finalSqlFilePath, true);
                                finalSQLinputFilePaths.Add(finalSqlFilePath);
                                //write the start of file content
                                using (StreamReader beginFile = new System.IO.StreamReader(beginFilePath))
                                {
                                    string beginLine;

                                    while ((beginLine = beginFile.ReadLine()) != null)
                                    {
                                        writer.WriteLine(beginLine);
                                    }
                                }

                                count = 0;
                            }

                            writer.WriteLine(line);

                            ++count;
                        }
                        if(writer != null)
                        {
                            //write the end of file content
                            using (StreamReader endFile = new System.IO.StreamReader(endFilePath))
                            {
                                string endLine;

                                while ((endLine = endFile.ReadLine()) != null)
                                {
                                    writer.WriteLine(endLine);
                                }
                            }
                            writer.Close();
                            fileSequence++;
                        }
                    }
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
        }

        static void testLab()
        {
            List<string> order = new List<string>();
            order.Add("Testp30763640326699");
            order.Add("LastT49463641432015");
            LabReader reader = new LabReader();
            var result = reader.GetLabsFromOrderNumber(order);
        }
        

        static void CheckifExcelInstalled()
        {
            var exist = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
            Console.WriteLine(exist);
            Console.ReadKey();
        }

        static void saveorderfile()		
         {
             string orderNumber = "MOTLE50402636707026";		
             /*var data = Intervent.HWS.Labcorp.PullOrderConfirmation(orderNumber);		
             using (var fs = new FileStream(orderNumber + ".pdf", FileMode.Create, FileAccess.Write))		
             {		
                 fs.Write(data, 0, data.Length);		
             }	*/	
         }

        static void savereportfile()
        {
            string orderNumber = "User042763640998533";
            //Intervent.HWS.Labcorp.PullOrderReport();
            /*using (var fs = new FileStream(orderNumber + "report.pdf", FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
            }*/
        }
    }
}
