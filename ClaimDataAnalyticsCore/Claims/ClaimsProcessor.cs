using ClaimDataAnalytics.Claims.Classmaps;
using ClaimDataAnalytics.Claims.DB;
using ClaimDataAnalytics.Claims.LayoutTransformer;
using ClaimDataAnalytics.Claims.Model;
using ClaimDataAnalytics.Claims.SqlTranslator;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataAnalytics.Claims
{
    public class ClaimsProcessor
    {
        string _basePath;
        const string SQLFiles = "FinalFiles";
        string _claimInputFilesPath;
        string _finalSqlBeginPath;
        string _finalSqlEndPath;
        DateTime beginDate;
        DateTime endDate;

        public ClaimsProcessor(string basePath, string claimInputFilesPath, string finalSqlBeginPath, string finalSqlEndPath, DateTime beginDate1, DateTime endDate1)
        {
            _basePath = basePath;
            _claimInputFilesPath = claimInputFilesPath;
            _finalSqlBeginPath = finalSqlBeginPath;
            _finalSqlEndPath = finalSqlEndPath;
            beginDate = beginDate1;
            endDate = endDate1;
        }

        public void CreateSqlFiles()
        {
            var claimsProviders = ClaimProviderDto.GetAll();

            //var beginDate = new DateTime(2016, 10, 1);
            //var endDate = new DateTime(2018, 12, 1);
            // create the folders for each month
            for (var currentDate = beginDate; currentDate <= endDate; currentDate = currentDate.AddMonths(1))
            {
                var folderName = _basePath + @"\" + currentDate.ToString("yyyyMM");
                Directory.CreateDirectory(folderName);

                foreach (var provider in claimsProviders)
                {
                    Directory.CreateDirectory(Path.Combine(folderName, provider.DirectoryName));
                    Directory.CreateDirectory(Path.Combine(folderName, SQLFiles));
                }
            }


            //move the files to the respective folders
            foreach (var file in Directory.GetFiles(_claimInputFilesPath))
            {
                string fileName = Path.GetFileName(file);
                var splitFileName = fileName.Split(new char[] { '_' });
                DateTime fileCreateDate = DateTime.ParseExact(splitFileName[0], "yyyyMMdd", CultureInfo.InvariantCulture);
                string providerName = splitFileName[1];
                string companyName = splitFileName[2];
                var folderName = Path.Combine(_basePath, fileCreateDate.ToString("yyyyMM"), providerName);
                var provider = ClaimProviderDto.GetByProviderName(providerName);

                string extension = Path.GetExtension(file);
                string fileNameWithoutPath = Path.GetFileNameWithoutExtension(file);
                var destinationFilePath = Path.Combine(folderName, String.Concat(fileNameWithoutPath, ".csv"));
                if (provider.IsFixedLayout)
                {
                    BaseFixedLayoutTransformer transformer = (BaseFixedLayoutTransformer)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(provider.TransformerClassName);
                    transformer.Transform(file, destinationFilePath);
                }
                else
                {
                    File.Copy(file, destinationFilePath);
                }

            }


            //get the common id files
            var crothalIDs = new Dblayer().GetClaimsCrothalIDs();
            var crothalIDsDict = crothalIDs.ToDictionary(x => x.OldUniqueId, x => x, StringComparer.OrdinalIgnoreCase);


            //map all the input files to a common model

            for (var currentDate = beginDate; currentDate <= endDate; currentDate = currentDate.AddMonths(1))
            {
                //var newDate = new DateTime(2018, 1, 1);
                //Console.WriteLine(currentDate);
                //if (DateTime.Compare(currentDate.Date,newDate.Date) != 0)
                //    continue;
                //if (currentDate.Year != 2017 || currentDate.Month != 1)
                //    continue;
                Console.WriteLine($"Processing - {currentDate}");
                var folderName = _basePath + @"\" + currentDate.ToString("yyyyMM");
                string iffPath = Path.Combine(folderName, "iff.csv");
                if (!File.Exists(iffPath))
                {
                    using (TextWriter writer = new StreamWriter(iffPath))
                    {
                        using (var csv = new CsvWriter(writer))
                        {
                            csv.Configuration.Encoding = Encoding.UTF8;
                            csv.Configuration.Delimiter = "|";
                            csv.Configuration.RegisterClassMap<InputFlatFileClassMap>();
                            csv.WriteHeader(typeof(ClaimsInputFlatFileModel));
                        }
                    }
                }

                var eligibilityRecords = new Dblayer().GetEligibilityIDs(currentDate);
                Console.WriteLine(eligibilityRecords.Count());
                var eligibilitySSNKey = eligibilityRecords.Where(x => x.SSN != "" && x.SSN != null && !String.IsNullOrEmpty(x.UserEnrollmentType) && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => String.Concat(x.SSN, x.UserEnrollmentType), x => x, StringComparer.OrdinalIgnoreCase);
                // Console.WriteLine(eligibilitySSNKey.Count());
                var eligibilityUniqueIdKey = eligibilityRecords.Where(x => x.UniqueID != "" && x.UniqueID != null && !String.IsNullOrEmpty(x.UserEnrollmentType) && x.UserEnrollmentType != "C" && !String.IsNullOrEmpty(x.MedicalPlanCode)).ToDictionary(x => x.UniqueID, x => x, StringComparer.OrdinalIgnoreCase);
                // Console.WriteLine(eligibilityUniqueIdKey.Count());
                foreach (var claimProvider in claimsProviders)
                {
                    //if (claimProvider != ClaimProviderDto.CVS_CAREMARK)
                    //    continue;
                    // Console.WriteLine("Directory name " + claimProvider.DirectoryName);
                    var path = Path.Combine(folderName, claimProvider.DirectoryName);
                    Console.WriteLine("Folder name " + path);
                    var files = Directory.GetFiles(path);
                    foreach (var file in files)
                    {
                        Console.WriteLine(file);
                        var fileSplit = file.Split(new char[] { '_' });
                        var companyName = fileSplit[2];
                       
                        using (var sr = new StreamReader(file))
                        using (TextWriter sw = new StreamWriter(iffPath, true))
                        {
                            using (CsvReader reader = new CsvReader(sr))
                            using (CsvWriter writer = new CsvWriter(sw))
                            {
                                // writer config
                                writer.Configuration.Encoding = Encoding.UTF8;
                                writer.Configuration.Delimiter = "|";
                                writer.Configuration.RegisterClassMap<InputFlatFileClassMap>();

                                // reader config
                                reader.Configuration.Delimiter = claimProvider.Delimiter;
                                reader.Configuration.HasHeaderRecord = claimProvider.HasHeader;
                                reader.Configuration.IsHeaderCaseSensitive = false;
                                reader.Configuration.SkipEmptyRecords = true;
                                reader.Configuration.TrimFields = true;
                                reader.Configuration.TrimHeaders = true;
                                reader.Configuration.WillThrowOnMissingField = true;
                                if (claimProvider == ClaimProviderDto.BCBS)
                                    reader.Configuration.RegisterClassMap<BCBSFileClassMap>();
                                else if (claimProvider == ClaimProviderDto.AETNA)
                                    reader.Configuration.RegisterClassMap<AetnaFileClassMap>();
                                else if (claimProvider == ClaimProviderDto.CVS_CAREMARK)
                                    reader.Configuration.RegisterClassMap<CVSCaremarkClassMap>();
                                else if (claimProvider == ClaimProviderDto.UHC)
                                    reader.Configuration.RegisterClassMap<UHCClassMap>();
                                else
                                {
                                    throw new ApplicationException("Class map not found");
                                }
                                int row = 0;
                                while (reader.Read())
                                {
                                    row++;
                                    try
                                    {
                                        //Data->Values->ResultsView->[0] will give you the index
                                        // "Row: '2' (1 based)\r\nType: 'Intervent.Web.DTO.ClaimsInputFlatFileModel'\r\nField Index: '125' (0 based)\r\nField Value: '000000003}'\r\n"
                                        ClaimsInputFlatFileModel record = reader.GetRecord<ClaimsInputFlatFileModel>();
                                        if (claimProvider == ClaimProviderDto.CVS_CAREMARK && (reader[0] == "PA" || reader[0] == "PT"))//ignore first line
                                            continue;

                                        //set the provider name
                                        record.ProviderName = claimProvider.Name;
                                        record.InputFileRowId = row;
                                        record.CreateDate = currentDate;
                                        record.OrgName = companyName;
                                        if (RunCrothallIdMap(currentDate))
                                        {
                                            record.CrothalIdChanges(crothalIDsDict);//only for CVS

                                        }
                                        if (claimProvider.SetSSN)
                                        {
                                            if (record.UniqueId.EndsWith("XXX"))
                                                record.SubscriberSSN = record.UniqueId.Replace("XXX", "EH9"); 
                                            //just populate the ids
                                            //record.SetSSN(eligibilityUniqueIdKey, childreneligibilityUniqueIdKey);
                                        }
                                        else
                                        {
                                            record.SetUniqueId(eligibilitySSNKey, eligibilityUniqueIdKey);
                                        }
                                        writer.WriteRecord(record);
                                    }
                                    catch (CsvHelperException ex)
                                    {
                                        string msg = "";
                                        foreach (System.Collections.DictionaryEntry de in ex.Data)
                                        {
                                            msg += de.Value;
                                        }

                                        throw new ApplicationException($"error occurred while reading the csv {row} - {msg}");
                                    }
                                    catch (Exception)
                                    {
                                        throw new ApplicationException($"error occurred while reading the csv {row}");
                                    }
                                }
                            }
                        }

                    }

                }

                string iffSqlPath = Path.Combine(folderName, "iff.sql");
               
                using (var sr = new StreamReader(iffPath))
                using (var sw = new StreamWriter(iffSqlPath, true))
                {
                    using (CsvReader reader = new CsvReader(sr))
                    {
                        //reader config
                        reader.Configuration.Delimiter = "|";
                        reader.Configuration.HasHeaderRecord = true;
                        reader.Configuration.IsHeaderCaseSensitive = false;
                        reader.Configuration.SkipEmptyRecords = true;
                        reader.Configuration.TrimFields = true;
                        reader.Configuration.TrimHeaders = true;
                        reader.Configuration.WillThrowOnMissingField = true;
                        reader.Configuration.RegisterClassMap<InputFlatFileClassMapRead>();
                        int row = 0;
                        try
                        {

                            while (reader.Read())
                            {
                                row++;
                                ClaimsInputFlatFileModel record = reader.GetRecord<ClaimsInputFlatFileModel>();

                                #region Nullable Datetime corrections
                                //csv helper sets a default date for nullable properties
                                if (record.ServiceStartDate.HasValue && record.ServiceStartDate.Value.Year == 1)
                                {
                                    record.ServiceStartDate = null;
                                }

                                if (record.MemberDateOfBirth.HasValue && record.MemberDateOfBirth.Value.Year == 1)
                                {
                                    record.MemberDateOfBirth = null;
                                }

                                #endregion

                                record.CreateDate = currentDate;
                                sw.WriteLine(ClaimSqlTranslator.CreateClaimSql(record));
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
        }

        bool RunCrothallIdMap(DateTime date)
        {
            if (date.Year >= 2018)
                return true;
            else if (date.Year == 2017 && date.Month > 10)
                return true;
            else
                return false;
        }

        public void SplitFiles(int maxLines)
        {
            for (var currentDate = beginDate; currentDate <= endDate; currentDate = currentDate.AddMonths(1))
            {
                //var newDate = new DateTime(2018, 1, 1);
                //Console.WriteLine(currentDate);
                //if (DateTime.Compare(currentDate.Date,newDate.Date) != 0)
                //    continue;
                //if (currentDate.Year != 2017 || currentDate.Month != 1)
                //    continue;
                //Console.WriteLine(currentDate);
                var folderName = _basePath + @"\" + currentDate.ToString("yyyyMM");
                string iffPath = Path.Combine(folderName, "iff.sql");
                SplitFiles(maxLines, iffPath, folderName);
            }
        }

        void SplitFiles(int maxLines, string filePath, string outputDirectory)
        {
            if (!File.Exists(filePath))
                return;
            var SQLinputFilePaths = new List<String>();
            SQLinputFilePaths.Add(filePath);
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
                                finalSqlFilePath = Path.Combine(outputDirectory, "FinalFiles", $"finalSql" + fileSequence + ".sql");
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
    }
}
