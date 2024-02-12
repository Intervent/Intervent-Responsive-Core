using ClaimDataAnalytics.Eligibility.Classmap;
using ClaimDataAnalytics.Eligibility.CsvModel;
using ClaimDataAnalytics.Eligibility.SqlTranslator;
using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ClaimDataAnalytics.Eligibility
{
    public class Eligibility
    {
        string _baseDirectory;
        string _outputDirectory;
        string _finalSqlBeginPath;
        string _finalSqlEndPath;
        public Eligibility(string baseDirectory, string outputDirectory, string finalSqlBeginPath, string finalSqlEndPath)
        {
            _baseDirectory = baseDirectory;
            _outputDirectory = outputDirectory;
            _finalSqlBeginPath = finalSqlBeginPath;
            _finalSqlEndPath = finalSqlEndPath;
        }

        public void CreateSql()
        {
            //loop through the csv file format yyyymmdd_company_v1.csv
            var files = Directory.GetFiles(_baseDirectory, "*.csv");
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                Console.WriteLine(fileName);
                var fileSplit = fileName.Split(new char[] { '_' });
                //Console.WriteLine(fileSplit[0]);
                DateTime fileCreateDate = DateTime.ParseExact(fileSplit[0], "yyyyMMdd", CultureInfo.InvariantCulture);
                var companyName = fileSplit[1];
                var templateVersion = fileSplit[2];
                var outputFile = Path.Combine(_outputDirectory, fileName).Replace(".csv", ".txt").Replace(".CSV", ".txt");
                int row = 0;
                using (var sr = new StreamReader(file))
                using (var sw = new StreamWriter(outputFile))
                {
                    CsvReader reader = new CsvReader(sr);
                    reader.Configuration.Delimiter = "|";
                    reader.Configuration.IsHeaderCaseSensitive = false;
                    if (templateVersion == "v1")
                    {
                        reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV1>();
                    }
                    else if (templateVersion == "v2")
                    {
                        reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV2>();
                    }
                    else if (templateVersion == "v3")
                    {
                        reader.Configuration.RegisterClassMap<EligibilityCsvClassMapV3>();
                    }
                    else
                    {
                        reader.Configuration.RegisterClassMap<EligibilityCsvClassMap>();
                    }
                    reader.Configuration.SkipEmptyRecords = true;
                    reader.Configuration.TrimFields = true;
                    reader.Configuration.TrimHeaders = true;
                    reader.Configuration.WillThrowOnMissingField = true;
                    try
                    {
                        while (reader.Read())
                        {
                            row++;
                            if (reader.CurrentRecord[0] == "H" || reader.CurrentRecord[0] == "T")
                            {
                                continue;
                            }
                            EligibilityCsvModel record = reader.GetRecord<EligibilityCsvModel>();
                            #region Nullable Datetime corrections
                            //csv helper sets a default date for nullable properties
                            if (record.HireDate.HasValue && record.HireDate.Value.Year == 1)
                            {
                                record.HireDate = null;
                            }
                            if (record.DeathDate.HasValue && record.DeathDate.Value.Year == 1)
                            {
                                record.DeathDate = null;
                            }

                            if (record.TerminatedDate.HasValue && record.TerminatedDate.Value.Year == 1)
                            {
                                record.TerminatedDate = null;
                            }
                            if (record.DOB.HasValue && record.DOB.Value.Year == 1)
                            {
                                record.DOB = null;
                            }
                            if (record.MedicalPlanEndDate.HasValue && record.MedicalPlanEndDate.Value.Year == 1)
                            {
                                record.MedicalPlanEndDate = null;
                            }

                            if (record.MedicalPlanStartDate.HasValue && record.MedicalPlanStartDate.Value.Year == 1)
                            {
                                record.MedicalPlanStartDate = null;
                            }

                            if (record.DentalPlanEndDate.HasValue && record.DentalPlanEndDate.Value.Year == 1)
                            {
                                record.DentalPlanEndDate = null;
                            }

                            if (record.DentalPlanStartDate.HasValue && record.DentalPlanStartDate.Value.Year == 1)
                            {
                                record.DentalPlanStartDate = null;
                            }

                            if (record.VisionPlanEndDate.HasValue && record.VisionPlanEndDate.Value.Year == 1)
                            {
                                record.VisionPlanEndDate = null;
                            }

                            if (record.VisionPlanStartDate.HasValue && record.VisionPlanStartDate.Value.Year == 1)
                            {
                                record.VisionPlanStartDate = null;
                            }


                            #endregion

                            record.CompanyName = companyName;
                            record.CreateDate = fileCreateDate;
                            sw.WriteLine(EligibiltySql.CreateSql(record));
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Line number is " + row);
                        throw;
                    }
                }
            }
        }

        public void SplitFiles(int maxLines)
        {
            var SQLinputFilePaths = Directory.GetFiles(_outputDirectory, "*.txt");
            int fileSequence = 0;
            string finalSqlFilePath = null;
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
                                    //write the end of file content
                                    using (StreamReader endFile = new System.IO.StreamReader(_finalSqlEndPath))
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
                                finalSqlFilePath = Path.Combine(_outputDirectory, "FinalFiles", $"finalSql" + fileSequence + ".sql");
                                writer = new System.IO.StreamWriter(finalSqlFilePath, true);
                                //write the start of file content
                                using (StreamReader beginFile = new System.IO.StreamReader(_finalSqlBeginPath))
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
                        if (writer != null)
                        {
                            //write the end of file content
                            using (StreamReader endFile = new System.IO.StreamReader(_finalSqlEndPath))
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

        public int GetRecordsCount()
        {
            var SQLinputFilePaths = Directory.GetFiles(_outputDirectory, "*.txt");
            int totalRecords = 0;
            foreach (var file in SQLinputFilePaths)
            {
                var lines = File.ReadAllLines(file);
                totalRecords += lines.Count();
            }
            return totalRecords;
        }

    }
}
