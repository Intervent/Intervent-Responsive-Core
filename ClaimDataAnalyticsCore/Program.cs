using ClaimDataAnalytics.Claims;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ClaimDataAnalytics
{
    class Program
    {
        static void Main(string[] args)
        {
            var cs = ConfigurationManager.ConnectionStrings["AnalyticsDb"].ConnectionString;
            var beginDate = new DateTime(2017, 1, 1);
            var endDate = new DateTime(2019, 2, 1);
            var maxLines = 10000;
            //CheckConnection();
            string basePath = @"C:\Users\Admin\Desktop\Ramji\Reporting";
            //for (var currentDate = beginDate; currentDate <= endDate; currentDate = currentDate.AddMonths(1))
            //{
            //    var folderName = $@"{basePath}\Claims" + @"\" + currentDate.ToString("yyyyMM");
            //    Directory.CreateDirectory(folderName);
            //    Directory.CreateDirectory(Path.Combine(folderName, "FinalFiles"));

            //}
            //return;

            // string basePath = @"C:\Intervent\ClaimsProcess";
            //Step 1 - create the eligibility sql
            Eligibility.Eligibility eligibility = new Eligibility.Eligibility($@"{basePath}\Eligibility\Import", $@"{basePath}\Eligibility\Export", $@"{basePath}\FinalSqlBegin.txt", $@"{basePath}\FinalSqlEnd.txt");
            //eligibility.CreateSql();
            //eligibility.SplitFiles(maxLines);
            AutomateScripts(Directory.GetFiles($@"{basePath}\Eligibility\Export\FinalFiles").Where(x => x.EndsWith(".sql")));
            //// Console.WriteLine(eligibility.GetRecordsCount());
            //Step 2 - load claim files
            var claims = new ClaimsProcessor($@"{basePath}\Claims", $@"{basePath}\ClaimsImport", $@"{basePath}\FinalSqlBegin.txt", $@"{basePath}\FinalSqlEnd.txt", beginDate, endDate);
            //claims.CreateSqlFiles();
            //claims.SplitFiles(maxLines * 2);

            //var claimsBasepath = $@"{basePath}\Claims";
            //for (var currentDate = beginDate; currentDate <= endDate; currentDate = currentDate.AddMonths(1))
            //{
            //    //var newDate = new DateTime(2018, 1, 1);
            //    //Console.WriteLine(currentDate);
            //    //if (DateTime.Compare(currentDate.Date,newDate.Date) != 0)
            //    //    continue;
            //    //if (currentDate.Year != 2017 || currentDate.Month != 1)
            //    //    continue;
            //    Console.WriteLine(currentDate);

            //    var folderName = claimsBasepath + @"\" + currentDate.ToString("yyyyMM") + @"\FinalFiles";
            //    AutomateScripts(Directory.GetFiles(folderName).Where(x => x.EndsWith(".sql")));
            //}

            //SplitFiles(78809, 10, $@"{basePath}\Claims\201802\CVS\20180201_CVS_Compass_file.csv", $@"{basePath}\Claims\201802\CVS\20180201_CVS_Compass_file.txt");
            //PrintLines(155259, 155279, $@"{basePath}\Claims\201701\BCBS\20170101_BCBS_Compass_file.csv", $@"{basePath}\Claims\201701\BCBS\20170101_BCBS_Compass_file_min.csv");
        }

        static void CheckConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["AnalyticsDb"].ConnectionString;
            Console.WriteLine(cs);
            using (SqlConnection cx = new SqlConnection(cs))
            {
                cx.Open();
                // Use the connection
            }
        }

        static void AutomateScripts(IEnumerable<string> files)
        {
            Console.WriteLine(DateTime.Now.ToString());
            foreach (var file in files)
            {
                Console.WriteLine($"File processing is {file}");
                ProcessStartInfo info = new ProcessStartInfo("sqlcmd", $@" -U claimAnalytics -P pwd! -i {file}");

                //  Indicades if the Operative System shell is used, in this case it is not
                info.UseShellExecute = false;

                //No new window is required
                info.CreateNoWindow = true;

                //The windows style will be hidden
                info.WindowStyle = ProcessWindowStyle.Hidden;

                //The output will be read by the starndar output process
                info.RedirectStandardOutput = true;

                Process proc = new Process();

                proc.StartInfo = info;

                //Start the process
                proc.Start();

                proc.WaitForExit();

                Console.WriteLine($"Finished processing {file}");
            }
            Console.WriteLine(DateTime.Now.ToString());

        }

        static void SplitFiles(int start, int maxLines, string inputFilePath, string outputFilePath)
        {
            int row = 0;
            using (StreamReader sr = new System.IO.StreamReader(inputFilePath))
            using (StreamWriter sw = new System.IO.StreamWriter(outputFilePath, true))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (row > maxLines)
                        break;
                    if (row > start)
                    {
                        sw.WriteLine(line);
                    }
                    row++;
                }
            }

        }

        static void PrintLines(int startNumber, int endNumber, string filePath, string outputFilePath)
        {
            string line;
            int i = 0;
            using (var sr = new StreamReader(filePath))
            using (var sw = new StreamWriter(outputFilePath))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    i++;
                    if (i < startNumber)
                        continue;
                    else if (i >= startNumber && i <= endNumber)
                    {
                        sw.WriteLine(line);
                    }
                    else
                        break;
                }
            }
        }
    }
}
