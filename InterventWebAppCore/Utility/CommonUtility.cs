using Intervent.Business.Account;
using Intervent.Business.Claims;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace InterventWebApp
{
    public static class CommonUtility
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static string GeneratePassword(int length, int minNonAlphanumericChars)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
            var random = new Random();

            var nonAlphanumericChars = new char[minNonAlphanumericChars];
            for (int i = 0; i < minNonAlphanumericChars; i++)
            {
                nonAlphanumericChars[i] = allowedChars[random.Next(allowedChars.Length)];
            }

            var passwordChars = Enumerable.Repeat(allowedChars, length - minNonAlphanumericChars)
                                           .Select(s => s[random.Next(s.Length)])
                                           .ToArray();

            passwordChars = passwordChars.Concat(nonAlphanumericChars).ToArray();
            Array.Sort(passwordChars);

            return new string(passwordChars);
        }

        public static int GetRandom()
        {
            Random rnd = new Random();
            return rnd.Next(99999);
        }

        public static string NullCheck(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : value;
        }

        public static string NullCheck(byte? value)
        {
            return value.HasValue ? value.ToString() : "";
        }

        public static float ToImperial(float value, int type, int unit)
        {
            var Measurements = CommonUtility.ListMeasurements(unit).Measurements;
            return ListOptions.ToImperial(value, type, Measurements);
        }

        public static float ToMetric(float value, int type, int unit)
        {
            var Measurements = CommonUtility.ListMeasurements(unit).Measurements;
            if (unit == (int)Unit.Metric)
            {
                return ListOptions.ToMetric(value, type, Measurements);
            }
            return value;
        }

        public static dynamic ConvertIntoImperial(dynamic model, int unit)
        {
            model = ListOptions.CovertIntoImperial(model, ListMeasurements(unit).Measurements);
            return model;
        }

        public static dynamic ConvertIntoMetric(dynamic model, int unit)
        {
            model = ListOptions.CovertIntoMetric(model, ListMeasurements(unit).Measurements);
            return model;
        }

        public static dynamic Roundoff(dynamic model, int unit)
        {
            int RoundoffValue = unit == (int)Unit.Imperial ? 0 : 1;
            model.TotalChol = model.TotalChol != null ? (float)Math.Round(model.TotalChol, RoundoffValue) : model.TotalChol;
            model.Trig = model.Trig != null ? (float)Math.Round(model.Trig, RoundoffValue) : model.Trig;
            model.HDL = model.HDL != null ? (float)Math.Round(model.HDL, RoundoffValue) : model.HDL;
            model.LDL = model.LDL != null ? (float)Math.Round(model.LDL, RoundoffValue) : model.LDL;
            model.Glucose = model.Glucose != null ? (float)Math.Round(model.Glucose, RoundoffValue) : model.Glucose;
            model.HeightCM = model.Height != null ? (float)Math.Round(model.Height, 1) : model.Height;
            model.Weight = model.Weight != null ? (float)Math.Round(model.Weight, 1) : model.Weight;
            model.Waist = model.Waist != null ? (float)Math.Round(model.Waist, 1) : model.Waist;
            return model;
        }

        public static List<MeasurementsDto> MeasurementRange()
        {
            CommonReader reader = new CommonReader();
            return reader.MeasurementRange();
        }

        public static dynamic CheckIfUpdated(dynamic model, string type, int unit)
        {
            dynamic response = model, responseCopy = model;
            model = CommonUtility.ConvertIntoImperial(model, unit);
            if (type == "Labs")
            {
                LabReader reader = new LabReader();
                response = reader.ReadLabWork(new ReadLabRequest { Id = model.Id }).Lab;
                if (response != null)
                {
                    responseCopy = reader.ReadLabWork(new ReadLabRequest { Id = model.Id }).Lab;
                    responseCopy = CommonUtility.ConvertIntoMetric(responseCopy, unit);
                    responseCopy = CommonUtility.ConvertIntoImperial(responseCopy, unit);
                }
            }
            // #TODO
            //else if (type == "HRA")
            //{
            //    response = ReportUtility.ReadHRAReport(hraId).hra.HealthNumbers;
            //    if (response != null)
            //    {
            //        responseCopy = ReportUtility.ReadHRAReport(hraId).hra.HealthNumbers;
            //        responseCopy = CommonUtility.ConvertIntoMetric(responseCopy, unit);
            //        responseCopy = CommonUtility.ConvertIntoImperial(responseCopy, unit);
            //    }
            //}
            //else if (type == "FollowUp")
            //{
            //    var followupid = Convert.ToInt32(HttpContext.Current.Session[SessionContext.FollowUpId]);
            //    response = FollowUpUtility.ReadFUHealthNumber(followupid).FollowUp_HealthNumbersDto;
            //    if (response != null)
            //    {
            //        responseCopy = FollowUpUtility.ReadFUHealthNumber(followupid).FollowUp_HealthNumbersDto;
            //        responseCopy = CommonUtility.ConvertIntoMetric(responseCopy, unit);
            //        responseCopy = CommonUtility.ConvertIntoImperial(responseCopy, unit);
            //    }
            //}
            if (response != null)
            {
                model.Height = model.Height != responseCopy.Height ? model.Height : response.Height;
                model.Weight = model.Weight != responseCopy.Weight ? model.Weight : response.Weight;
                model.Waist = model.Waist != responseCopy.Waist ? model.Waist : response.Waist;
                model.BPArm = model.BPArm != responseCopy.BPArm ? model.BPArm : response.BPArm;
                model.SBP = model.SBP != responseCopy.SBP ? model.SBP : response.SBP;
                model.DBP = model.DBP != responseCopy.DBP ? model.DBP : response.DBP;
                model.HDL = model.HDL != responseCopy.HDL ? model.HDL : response.HDL;
                model.LDL = model.LDL != responseCopy.LDL ? model.LDL : response.LDL;
                model.TotalChol = model.TotalChol != responseCopy.TotalChol ? model.TotalChol : response.TotalChol;
                model.Trig = model.Trig != responseCopy.Trig ? model.Trig : response.Trig;
                model.A1C = model.A1C != responseCopy.A1C ? model.A1C : response.A1C;
                model.Glucose = model.Glucose != responseCopy.Glucose ? model.Glucose : response.Glucose;
            }
            return model;
        }

        public static ListLanguagesResponse GetLanguages(string languageCode = null)
        {
            CommonReader reader = new CommonReader();
            return reader.GetLanguages(languageCode);
        }

        public static List<FollowUpTypeDto> GetFollowUpTypes()
        {
            CommonReader reader = new CommonReader();
            return reader.GetFollowUpTypes();
        }

        public static string GetFollowUpType(int followUpId, int usersinProgramId)
        {
            ProgramReader programReader = new ProgramReader();
            var userinProgram = programReader.GetUserinProgramDetails(usersinProgramId);
            int followupIndex = userinProgram.FollowUps.ToList().FindIndex(x => x.Id == followUpId);
            var portalFollowups = PortalUtility.GetPortalFollowUps(userinProgram.ProgramsinPortal.PortalId, userinProgram.ProgramsinPortal.program.ProgramType).portalFollowUps.OrderBy(y => y.FollowUpType.Days).ToArray();
            return portalFollowups[followupIndex].FollowUpType.Type;
        }

        public static IncentivePointsResponse GetIncentivePoints(int participantPortalId, int participantId)
        {
            IncentivePointsResponse response = new IncentivePointsResponse();
            var userIncentive = PortalUtility.ReadIncentive(participantPortalId, participantId);
            double Points = 0, Currency = 0;
            foreach (var incentive in userIncentive.PortalIncentives)
            {
                if (incentive.IsActive == true)
                {
                    if (incentive.IsPoint && !incentive.isGiftCard)
                    {
                        foreach (var UserIncentive in incentive.UserIncentives)
                        {
                            if (UserIncentive.IsActive == true)
                            {
                                if (incentive.IncentiveTypeId == 9)
                                    Points = Points + UserIncentive.Points;
                                else
                                    Points = Points + incentive.Points;
                            }
                        }
                    }
                    else if (!incentive.IsPoint && !incentive.isGiftCard && !incentive.removeSurcharge)
                    {
                        foreach (var UserIncentive in incentive.UserIncentives)
                        {
                            if (UserIncentive.IsActive == true)
                            {
                                if (incentive.IncentiveTypeId == 9)
                                    Currency = Currency + UserIncentive.Points;
                                else
                                    Currency = Currency + incentive.Points;
                            }
                        }
                    }
                }
            }
            response.Points = Points;
            response.Currency = Currency;
            return response;
        }

        public static string GetIncentivePointsString(string url, int participantPortalId, int participantId)
        {
            var incentivePoints = GetIncentivePoints(participantPortalId, participantId);
            return ("<a class='points' href='" + url + "'>" + incentivePoints.Points + "<span class='currency' href=''>" + incentivePoints.Currency + "</span></a>");
        }

        public static bool HasAdminRole(string role)
        {
            var roleList = new List<string>() { RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR };
            if (!string.IsNullOrEmpty(role) && RoleContains(roleList, role))
                return true;
            return false;
        }

        public static bool IsParticipantView(string isParticipantView)
        {
            if (string.IsNullOrEmpty(isParticipantView) && isParticipantView == "true")
                return true;
            return false;
        }

        public static bool IsSuperAdmin(string role)
        {
            var roleList = new List<string>() { RoleCode.Administrator };
            if (!string.IsNullOrEmpty(role) && RoleContains(roleList, role))
                return true;
            return false;
        }

        public static bool RoleContains(List<string> roleList, string roleListWithcomma)
        {
            if (roleListWithcomma.Contains(","))
            {
                var roles = roleListWithcomma.Split(',');
                foreach (var role in roles)
                {

                    if (roleList.Contains(role))
                        return true;

                }
            }
            return roleList.Contains(roleListWithcomma);
        }

        public static bool RoleContains(string[] roleList, string roleListWithcomma)
        {
            if (roleListWithcomma.Contains(","))
            {
                var roles = roleListWithcomma.Split(',');
                foreach (var role in roles)
                {

                    if (roleList.Contains(role))
                        return true;

                }
            }
            return roleList.Contains(roleListWithcomma);
        }

        public static SelectList GetTrainingSet()
        {
            List<KeyValue> set = new List<KeyValue>();
            set.Add(new KeyValue() { Value = "1", Text = Translate.Message("L2162") });
            set.Add(new KeyValue() { Value = "2", Text = Translate.Message("L2163") });
            set.Add(new KeyValue() { Value = "3", Text = Translate.Message("L2164") });
            SelectList trainingSetList = new SelectList(set, "Value", "Text");

            return trainingSetList;
        }

        public static SelectList GetRatingList()
        {
            List<KeyValue> list = new List<KeyValue>();
            list.Add(new KeyValue() { Value = "0", Text = Translate.Message("L2600") });
            list.Add(new KeyValue() { Value = "1", Text = Translate.Message("L2601") });
            list.Add(new KeyValue() { Value = "2", Text = Translate.Message("L2602") });
            list.Add(new KeyValue() { Value = "3", Text = Translate.Message("L2603") });
            list.Add(new KeyValue() { Value = "4", Text = Translate.Message("L2604") });
            list.Add(new KeyValue() { Value = "5", Text = Translate.Message("L2605") });
            SelectList ratingList = new SelectList(list, "Value", "Text");

            return ratingList;
        }

        public static ReadTimeZonesResponse GetTimeZones(int? timeZoneId)
        {
            CommonReader reader = new CommonReader();
            ReadTimeZonesRequest request = new ReadTimeZonesRequest();
            if (timeZoneId != null)
                request.Id = timeZoneId;
            ReadTimeZonesResponse response = reader.GetTimeZones(request);

            return response;
        }

        public static IList<CountryDto> ListCountries()
        {
            CommonReader reader = new CommonReader();
            ListCountriesRequest request = new ListCountriesRequest();
            return reader.ListCountries(request).Countries;
        }

        public static GetCountryResponse GetCountry(string code)
        {
            CommonReader reader = new CommonReader();
            GetCountryRequest request = new GetCountryRequest();
            request.code = code;
            return reader.GetCountry(request);
        }

        public static IList<StateDto> ListStates(int CountryId)
        {
            CommonReader reader = new CommonReader();
            ListStatesRequest request = new ListStatesRequest();
            request.CountryId = CountryId;
            return reader.ListStates(request).States;
        }

        public static IList<ReportTypeDto> ListReportTypes(int? hraId, int? followUpId, bool carePlan)
        {
            CommonReader reader = new CommonReader();
            ListReportTypesRequest request = new ListReportTypesRequest();
            request.hasHRA = hraId.HasValue ? true : false;
            request.hasFollowUp = followUpId.HasValue ? true : false;
            request.hasCarePlan = (carePlan && request.hasHRA.Value) ? true : false;
            return reader.ListReportTypes(request).reportTypes;
        }

        public static IList<RaceDto> ListRace(int? CountryId)
        {
            CommonReader reader = new CommonReader();
            ListRaceRequest request = new ListRaceRequest();
            request.CountryId = CountryId;
            return reader.ListRace(request).Race;
        }

        public static bool CheckIfOther(int type)
        {
            CommonReader reader = new CommonReader();
            return reader.CheckIfOther(type);
        }

        public static IList<StateDto> ListAllStates()
        {
            CommonReader reader = new CommonReader();
            return reader.ListAllStates().States;
        }

        public static GetStateResponse GetState(string code)
        {
            CommonReader reader = new CommonReader();
            GetStateRequest request = new GetStateRequest();
            request.stateCode = code;
            return reader.GetState(request);
        }

        public static IList<NoteTypesDto> GetNotesTypeList(bool isNoteExist = false, bool isCoachNoteExist = false, bool isBioNoteExist = false, bool showHRAReview = false)
        {
            CommonReader reader = new CommonReader();

            GetNoteTypesRequest request = new GetNoteTypesRequest();
            request.isNoteExist = isNoteExist;
            request.isCoachNoteExist = isCoachNoteExist;
            request.isBioNoteExist = isBioNoteExist;
            request.showHRAReviewNote = showHRAReview;
            return reader.GetNotesTypeList(request).NoteType;

        }

        public static KeyValuePair<int, double> ToFeetInches(float inches)
        {
            return new KeyValuePair<int, double>((int)inches / 12, inches % 12);
        }

        public static string[,] GetExercisePlan(int hraId)
        {
            string[,] exPlan = new string[12, 3];
            GetExercisePlanResponse ExercisePlanResult = new GetExercisePlanResponse();
            CommonReader reader = new CommonReader();
            GetExercisePlanRequest request = new GetExercisePlanRequest();
            request.hraId = hraId;
            ExercisePlanResult = reader.GetExercisePlan(request);
            if (ExercisePlanResult.ExercisePlan.Count() > 0)
            {
                for (int i = 0; i < 12; i++)
                {
                    exPlan[i, 0] = ExercisePlanResult.ExercisePlan[i].DaysPerWeek;
                    exPlan[i, 1] = ExercisePlanResult.ExercisePlan[i].Duration;
                    exPlan[i, 2] = ExercisePlanResult.ExercisePlan[i].Completed ? "1" : "0";
                }
            }

            return exPlan;
        }

        public static string[,] GetNutPlan(string plan)
        {
            string[,] mealPlan = new string[5, 6];

            switch (plan)
            {
                case "A":
                    mealPlan[0, 0] = ""; mealPlan[0, 1] = "1"; mealPlan[0, 2] = ""; mealPlan[0, 3] = "1"; mealPlan[0, 4] = "2";
                    mealPlan[1, 0] = "1"; mealPlan[1, 1] = "1"; mealPlan[1, 2] = ""; mealPlan[1, 3] = ""; mealPlan[1, 4] = "2";
                    mealPlan[2, 0] = "1"; mealPlan[2, 1] = "1"; mealPlan[2, 2] = "1"; mealPlan[2, 3] = ""; mealPlan[2, 4] = "3";
                    mealPlan[3, 0] = ""; mealPlan[3, 1] = "1"; mealPlan[3, 2] = ""; mealPlan[3, 3] = "2"; mealPlan[3, 4] = "3";
                    mealPlan[4, 0] = "1"; mealPlan[4, 1] = "1"; mealPlan[4, 2] = "1"; mealPlan[4, 3] = "1"; mealPlan[4, 4] = "4";
                    break;

                case "B":
                    mealPlan[0, 0] = ""; mealPlan[0, 1] = "1"; mealPlan[0, 2] = ""; mealPlan[0, 3] = "1"; mealPlan[0, 4] = "2";
                    mealPlan[1, 0] = "1"; mealPlan[1, 1] = "1"; mealPlan[1, 2] = ""; mealPlan[1, 3] = ""; mealPlan[1, 4] = "2";
                    mealPlan[2, 0] = "1"; mealPlan[2, 1] = "1"; mealPlan[2, 2] = "1"; mealPlan[2, 3] = ""; mealPlan[2, 4] = "3";
                    mealPlan[3, 0] = ""; mealPlan[3, 1] = "1"; mealPlan[3, 2] = ""; mealPlan[3, 3] = "2"; mealPlan[3, 4] = "3";
                    mealPlan[4, 0] = "2"; mealPlan[4, 1] = "1"; mealPlan[4, 2] = "2"; mealPlan[4, 3] = "1"; mealPlan[4, 4] = "6";
                    break;

                case "C":
                    mealPlan[0, 0] = ""; mealPlan[0, 1] = "1"; mealPlan[0, 2] = ""; mealPlan[0, 3] = "1"; mealPlan[0, 4] = "2";
                    mealPlan[1, 0] = "1"; mealPlan[1, 1] = "1"; mealPlan[1, 2] = "1"; mealPlan[1, 3] = ""; mealPlan[1, 4] = "3";
                    mealPlan[2, 0] = "1"; mealPlan[2, 1] = "1"; mealPlan[2, 2] = ""; mealPlan[2, 3] = "1"; mealPlan[2, 4] = "3";
                    mealPlan[3, 0] = ""; mealPlan[3, 1] = "1"; mealPlan[3, 2] = ""; mealPlan[3, 3] = "2"; mealPlan[3, 4] = "3";
                    mealPlan[4, 0] = "2"; mealPlan[4, 1] = "2"; mealPlan[4, 2] = "2"; mealPlan[4, 3] = "1"; mealPlan[4, 4] = "7";
                    break;

                case "D":
                    mealPlan[0, 0] = ""; mealPlan[0, 1] = "1"; mealPlan[0, 2] = ""; mealPlan[0, 3] = "1"; mealPlan[0, 4] = "2";
                    mealPlan[1, 0] = "1"; mealPlan[1, 1] = "1"; mealPlan[1, 2] = "1"; mealPlan[1, 3] = ""; mealPlan[1, 4] = "3";
                    mealPlan[2, 0] = "1"; mealPlan[2, 1] = "1"; mealPlan[2, 2] = ""; mealPlan[2, 3] = "1"; mealPlan[2, 4] = "3";
                    mealPlan[3, 0] = ""; mealPlan[3, 1] = "1"; mealPlan[3, 2] = "1"; mealPlan[3, 3] = "2"; mealPlan[3, 4] = "4";
                    mealPlan[4, 0] = "2"; mealPlan[4, 1] = "2"; mealPlan[4, 2] = "2"; mealPlan[4, 3] = "1"; mealPlan[4, 4] = "7";
                    break;

                case "E":
                    mealPlan[0, 0] = ""; mealPlan[0, 1] = "1"; mealPlan[0, 2] = ""; mealPlan[0, 3] = "1"; mealPlan[0, 4] = "2";
                    mealPlan[1, 0] = "1"; mealPlan[1, 1] = "1"; mealPlan[1, 2] = "1"; mealPlan[1, 3] = ""; mealPlan[1, 4] = "3";
                    mealPlan[2, 0] = "2"; mealPlan[2, 1] = "1"; mealPlan[2, 2] = ""; mealPlan[2, 3] = "1"; mealPlan[2, 4] = "4";
                    mealPlan[3, 0] = ""; mealPlan[3, 1] = "1"; mealPlan[3, 2] = "1"; mealPlan[3, 3] = "2"; mealPlan[3, 4] = "4";
                    mealPlan[4, 0] = "2"; mealPlan[4, 1] = "2"; mealPlan[4, 2] = "2"; mealPlan[4, 3] = "1"; mealPlan[4, 4] = "7";
                    break;

                case "F":
                    mealPlan[0, 0] = ""; mealPlan[0, 1] = "1"; mealPlan[0, 2] = ""; mealPlan[0, 3] = "1"; mealPlan[0, 4] = "2";
                    mealPlan[1, 0] = "1"; mealPlan[1, 1] = "1"; mealPlan[1, 2] = "1"; mealPlan[1, 3] = ""; mealPlan[1, 4] = "3";
                    mealPlan[2, 0] = "2"; mealPlan[2, 1] = "1"; mealPlan[2, 2] = ""; mealPlan[2, 3] = "1"; mealPlan[2, 4] = "4";
                    mealPlan[3, 0] = ""; mealPlan[3, 1] = "1"; mealPlan[3, 2] = "1"; mealPlan[3, 3] = "2"; mealPlan[3, 4] = "4";
                    mealPlan[4, 0] = "2"; mealPlan[4, 1] = "2"; mealPlan[4, 2] = "2"; mealPlan[4, 3] = "3"; mealPlan[4, 4] = "9";
                    break;

                case "G":
                    mealPlan[0, 0] = ""; mealPlan[0, 1] = "1"; mealPlan[0, 2] = ""; mealPlan[0, 3] = "1"; mealPlan[0, 4] = "2";
                    mealPlan[1, 0] = "1"; mealPlan[1, 1] = "1"; mealPlan[1, 2] = "1"; mealPlan[1, 3] = ""; mealPlan[1, 4] = "3";
                    mealPlan[2, 0] = "2"; mealPlan[2, 1] = "1"; mealPlan[2, 2] = ""; mealPlan[2, 3] = "1"; mealPlan[2, 4] = "4";
                    mealPlan[3, 0] = ""; mealPlan[3, 1] = "2"; mealPlan[3, 2] = "1"; mealPlan[3, 3] = "2"; mealPlan[3, 4] = "5";
                    mealPlan[4, 0] = "2"; mealPlan[4, 1] = "2"; mealPlan[4, 2] = "2"; mealPlan[4, 3] = "3"; mealPlan[4, 4] = "9";
                    break;
            }
            return mealPlan;
        }

        public static List<KeyValue> GetYears(bool forProfile)
        {
            List<KeyValue> years = new List<KeyValue>();
            int ageLimit = forProfile ? 13 : 18;
            for (int i = DateTime.Now.Year - ageLimit; i > DateTime.Now.Year - 100; i--)
            {
                years.Add(new KeyValue() { Value = i.ToString(), Text = i.ToString() });
            }
            return years;
        }

        public static List<KeyValue> GetRPE()
        {
            List<KeyValue> rpes = new List<KeyValue>();
            for (int i = 6; i <= 20; i++)
            {
                rpes.Add(new KeyValue() { Value = i.ToString(), Text = i.ToString() });
            }
            return rpes;
        }

        public static List<KeyValue> GetMonths()
        {
            List<KeyValue> months = new List<KeyValue>();
            months.Add(new KeyValue() { Value = "1", Text = @Translate.Message("L1401") });
            months.Add(new KeyValue() { Value = "2", Text = @Translate.Message("L1402") });
            months.Add(new KeyValue() { Value = "3", Text = @Translate.Message("L1403") });
            months.Add(new KeyValue() { Value = "4", Text = @Translate.Message("L1404") });
            months.Add(new KeyValue() { Value = "5", Text = @Translate.Message("L1405") });
            months.Add(new KeyValue() { Value = "6", Text = @Translate.Message("L1406") });
            months.Add(new KeyValue() { Value = "7", Text = @Translate.Message("L1407") });
            months.Add(new KeyValue() { Value = "8", Text = @Translate.Message("L1408") });
            months.Add(new KeyValue() { Value = "9", Text = @Translate.Message("L1409") });
            months.Add(new KeyValue() { Value = "10", Text = @Translate.Message("L1410") });
            months.Add(new KeyValue() { Value = "11", Text = @Translate.Message("L1411") });
            months.Add(new KeyValue() { Value = "12", Text = @Translate.Message("L1412") });
            return months;
        }

        public static List<KeyValue> GetDays()
        {
            List<KeyValue> days = new List<KeyValue>();
            for (int i = 1; i <= 31; i++)
            {
                days.Add(new KeyValue() { Value = i.ToString(), Text = i.ToString() });
            }
            return days;
        }

        public static SelectList GetDayOfWeek()
        {
            List<KeyValue> days = new List<KeyValue>();

            days.Add(new KeyValue() { Value = DayOfWeek.Monday.ToString(), Text = DayOfWeek.Monday.ToString() });
            days.Add(new KeyValue() { Value = DayOfWeek.Tuesday.ToString(), Text = DayOfWeek.Tuesday.ToString() });
            days.Add(new KeyValue() { Value = DayOfWeek.Wednesday.ToString(), Text = DayOfWeek.Wednesday.ToString() });
            days.Add(new KeyValue() { Value = DayOfWeek.Thursday.ToString(), Text = DayOfWeek.Thursday.ToString() });
            days.Add(new KeyValue() { Value = DayOfWeek.Friday.ToString(), Text = DayOfWeek.Friday.ToString() });
            days.Add(new KeyValue() { Value = DayOfWeek.Saturday.ToString(), Text = DayOfWeek.Saturday.ToString() });
            days.Add(new KeyValue() { Value = DayOfWeek.Sunday.ToString(), Text = DayOfWeek.Sunday.ToString() });

            SelectList daysList = new SelectList(days, "Value", "Text");

            return daysList;
        }

        public static SelectList GetPreferredTime()
        {
            List<KeyValue> time = new List<KeyValue>();
            time.Add(new KeyValue() { Value = "12:00 AM", Text = "12am - 3am" });
            time.Add(new KeyValue() { Value = "3:00 AM", Text = "3am - 6am" });
            time.Add(new KeyValue() { Value = "6:00 AM", Text = "6am - 9am" });
            time.Add(new KeyValue() { Value = "9:00 AM", Text = "9am - 12pm" });
            time.Add(new KeyValue() { Value = "12:00 PM", Text = "12pm - 3pm" });
            time.Add(new KeyValue() { Value = "3:00 PM", Text = "3pm - 6pm" });
            time.Add(new KeyValue() { Value = "6:00 PM", Text = "6pm - 9pm" });
            time.Add(new KeyValue() { Value = "9:00 PM", Text = "9pm - 12am" });
            SelectList startTime = new SelectList(time, "Value", "Text");

            return startTime;
        }

        public static double GetBMI(float height, float weight)
        {
            return CommonReader.GetBMI(height, weight);
        }

        public static int GetAge(DateTime birthday)
        {
            CommonReader commonReader = new CommonReader();
            return commonReader.GetAge(birthday);
        }

        public static List<LanguagesDto> GetPortalLanguages(int? orgId = null, int? participantPortalId = null)
        {
            PortalReader reader = new PortalReader();
            int portalId;
            if (!orgId.HasValue)
            {
                if (participantPortalId.HasValue)
                {
                    portalId = Convert.ToInt16(participantPortalId);
                }
                else
                {
                    CommonReader commonReader = new CommonReader();
                    return commonReader.GetLanguages().Languages;
                }
            }
            else
            {
                portalId = new AccountManager().CurrentPortalId(orgId.Value).PortalId ?? 0;
            }
            return reader.GetPortalLanguages(portalId);
        }

        public static double GetNewWellnessScore(float score, byte hraVer)
        {
            if (hraVer == (byte)HRAVersions.IntuityVersion)
                return Math.Round((score + 151.9) * 100 / 251.9, 1);
            else
                return Math.Round((score + 159.6) * 100 / 259.6, 1);
        }

        public static SelectList GetExerciseIntensity()
        {
            List<KeyValue> exerciseIntensity = new List<KeyValue>();
            exerciseIntensity.Add(new KeyValue() { Value = "1", Text = @Translate.Message("L1400") });
            exerciseIntensity.Add(new KeyValue() { Value = "2", Text = @Translate.Message("L1413") });
            exerciseIntensity.Add(new KeyValue() { Value = "3", Text = @Translate.Message("L1414") });
            SelectList exerciseIntensityList = new SelectList(exerciseIntensity, "Value", "Text");
            return exerciseIntensityList;
        }

        public static SelectList GetContractStatus()
        {
            List<KeyValue> contractStatus = new List<KeyValue>();
            contractStatus.Add(new KeyValue() { Value = "1", Text = @Translate.Message("L1415") });
            contractStatus.Add(new KeyValue() { Value = "2", Text = @Translate.Message("L1416") });
            SelectList contractStatusList = new SelectList(contractStatus, "Value", "Text");
            return contractStatusList;
        }

        public static SelectList GetRatings()
        {
            List<KeyValue> rating = new List<KeyValue>();
            rating.Add(new KeyValue() { Value = "5", Text = "5 - " + Translate.Message("L1430") });
            rating.Add(new KeyValue() { Value = "4", Text = "4" });
            rating.Add(new KeyValue() { Value = "3", Text = "3" });
            rating.Add(new KeyValue() { Value = "2", Text = "2" });
            rating.Add(new KeyValue() { Value = "1", Text = "1 - " + Translate.Message("L1431") });
            rating.Add(new KeyValue() { Value = "0", Text = "N/A - " + Translate.Message("L1432") });
            SelectList ratings = new SelectList(rating, "Value", "Text");
            return ratings;
        }

        public static IEnumerable<SelectListItem> GetUserHistoryCategories()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (UserHistoryCategoryDto dto in UserHistoryCategoryDto.GetAll())
            {
                items.Add(new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Description });
            }
            return items;
        }

        public static void SendEmail(string emailAddress, string body, string subject, string InfoEmail, string SecureEmail, string SMPTAddress, string PortNumber, string SecureEmailPassword, string MailAttachmentPath, string attachment = null)
        {
            try
            {
                var message = new MailMessage(InfoEmail, emailAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    From = new MailAddress(SecureEmail, "INTERVENT Support")
                };
                if (!string.IsNullOrEmpty(attachment))
                {
                    string path = MailAttachmentPath + "\\" + attachment;
                    message.Attachments.Add(new Attachment(path));
                }
                var client = new SmtpClient(SMPTAddress, Convert.ToInt32(PortNumber));
                client.Credentials = new NetworkCredential(SecureEmail, SecureEmailPassword);
                client.EnableSsl = true;
                client.Send(message);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static ReadMedicalPlanResponse ReadMedicalPlan(string code)
        {
            CommonReader reader = new CommonReader();
            ReadMedicalPlanRequest request = new ReadMedicalPlanRequest();
            request.code = code;
            return reader.ReadMedicalPlan(request);
        }

        public static SelectList GetSleepLogSelectList()
        {
            List<KeyValue> sleepLogList = new List<KeyValue>();
            sleepLogList.Add(new KeyValue() { Value = "1", Text = Translate.Message("L91") });
            sleepLogList.Add(new KeyValue() { Value = "2", Text = Translate.Message("L92") });
            sleepLogList.Add(new KeyValue() { Value = "3", Text = Translate.Message("L93") });
            SelectList sleeplogSelectList = new SelectList(sleepLogList, "Value", "Text");
            return sleeplogSelectList;
        }

        public static SelectList GetPromptRefTypeList()
        {
            List<KeyValue> refTypeList = new List<KeyValue>();
            refTypeList.Add(new KeyValue() { Value = "1", Text = "Step" });
            refTypeList.Add(new KeyValue() { Value = "2", Text = "Activity" });
            SelectList refTypeSelectList = new SelectList(refTypeList, "Value", "Text");
            return refTypeSelectList;
        }

        public static SelectList GetEligibilityModuleList()
        {
            List<KeyValue> moduleTypeList = new List<KeyValue>();
            moduleTypeList.Add(new KeyValue() { Value = "1", Text = "Claims" });
            moduleTypeList.Add(new KeyValue() { Value = "2", Text = "Outreach" });
            SelectList refTypeSelectList = new SelectList(moduleTypeList, "Value", "Text");
            return refTypeSelectList;
        }

        public static SelectList GetEligibilityNoteTypeList()
        {
            List<KeyValue> notesTypeList = new List<KeyValue>();
            notesTypeList.Add(new KeyValue() { Value = "1", Text = "Phone call" });
            notesTypeList.Add(new KeyValue() { Value = "2", Text = "Email" });
            notesTypeList.Add(new KeyValue() { Value = "3", Text = "Complaint" });
            notesTypeList.Add(new KeyValue() { Value = "4", Text = "Marketing Feedback" });
            SelectList refTypeSelectList = new SelectList(notesTypeList, "Value", "Text");
            return refTypeSelectList;
        }
        public static SelectList GetDiabetesTypeList()
        {
            List<KeyValue> diabetesTypeList = new List<KeyValue>();
            diabetesTypeList.Add(new KeyValue() { Value = "1", Text = "Type 1" });
            diabetesTypeList.Add(new KeyValue() { Value = "2", Text = "Type 2" });
            diabetesTypeList.Add(new KeyValue() { Value = "3", Text = "Gestational" });
            diabetesTypeList.Add(new KeyValue() { Value = "4", Text = "Other" });
            diabetesTypeList.Add(new KeyValue() { Value = "5", Text = "Unknown" });
            SelectList refTypeSelectList = new SelectList(diabetesTypeList, "Value", "Text");
            return refTypeSelectList;
        }
        public static SelectList GetClaudicationScale()
        {
            List<KeyValue> diabetesTypeList = new List<KeyValue>();
            diabetesTypeList.Add(new KeyValue() { Value = "0", Text = "0=no discomfort" });
            diabetesTypeList.Add(new KeyValue() { Value = "1", Text = "1= minimal discomfort" });
            diabetesTypeList.Add(new KeyValue() { Value = "2", Text = "2=moderate pain" });
            diabetesTypeList.Add(new KeyValue() { Value = "3", Text = "3=intense pain" });
            diabetesTypeList.Add(new KeyValue() { Value = "4", Text = "4=unbearable pain" });
            SelectList refTypeSelectList = new SelectList(diabetesTypeList, "Value", "Text");
            return refTypeSelectList;
        }

        public static SelectList GetGlucoseCheckCountList()
        {
            List<KeyValue> glucoseTestCountList = new List<KeyValue>();
            glucoseTestCountList.Add(new KeyValue() { Value = "1", Text = "Less than once per day" });
            glucoseTestCountList.Add(new KeyValue() { Value = "2", Text = "1" });
            glucoseTestCountList.Add(new KeyValue() { Value = "3", Text = "2" });
            glucoseTestCountList.Add(new KeyValue() { Value = "4", Text = "3" });
            glucoseTestCountList.Add(new KeyValue() { Value = "5", Text = "4" });
            glucoseTestCountList.Add(new KeyValue() { Value = "6", Text = "5" });
            glucoseTestCountList.Add(new KeyValue() { Value = "7", Text = "6" });
            glucoseTestCountList.Add(new KeyValue() { Value = "8", Text = "7" });
            glucoseTestCountList.Add(new KeyValue() { Value = "9", Text = "More than 7 times per day" });
            glucoseTestCountList.Add(new KeyValue() { Value = "10", Text = "Other" });

            SelectList countList = new SelectList(glucoseTestCountList, "Value", "Text");
            return countList;
        }

        #region Measuements

        public static ListMeasurementsResponse ListMeasurements(int measurementType)
        {
            CommonReader reader = new CommonReader();
            /*if (HttpContext.Current.Cache["Measurements"] == null)
            {
                HttpContext.Current.Cache["Measurements"] = reader.GetMeasurements();
            }*/
            IList<MeasurementsDto> measurements = reader.GetMeasurements() as IList<MeasurementsDto>;
            return reader.ListMeasurements(measurementType, measurements);
        }

        public static string GetMeasurementText(int unitId, int unit)
        {
            var MeasurementList = ListMeasurements(unit);
            var Measurements = MeasurementList.Measurements;
            var unitText = Measurements[unitId].MeasurementUnit;
            return unitText;
        }

        #endregion

        #region Claims
        public static IEnumerable<SelectListItem> GetEnrollmentStatusList()
        {
            return new ParticipantReader().GetEnrollmentStatus().Select(x => new SelectListItem() { Text = x.Description, Value = x.Code.ToString() });
        }

        public static IEnumerable<SelectListItem> GetDeclinedEnrollmentReasons()
        {
            return new ParticipantReader().GetDeclinedEnrollmentReasons().Select(x => new SelectListItem() { Text = x.Reason, Value = x.Id.ToString() });
        }

        public static IEnumerable<SelectListItem> GetDiagnosisCodeList()
        {
            return new ClaimManager().GetDiagnosisCodes().Select(x => new SelectListItem() { Text = x.Description, Value = x.Code });
        }
        #endregion

        public static void UpdateLastVisited(string url, int userId)
        {
            CommonReader reader = new CommonReader();
            UpdateLastVisitedRequest request = new UpdateLastVisitedRequest();
            request.url = url;
            request.UserId = userId;
            reader.UpdateLastVisited(request);
        }

        public static GetPregnencyStatusResponse GetPregancyStatus(int hraId, int userId, DateTime? date)
        {
            HRAReader reader = new HRAReader();
            CommonReader commonReader = new CommonReader();
            GetPregnencyStatusResponse response = commonReader.GetPregnencyStatus(userId, hraId);
            if (response.pregDueDate.HasValue && date.HasValue)
            {
                GetPregnencyDaysRequest request = new GetPregnencyDaysRequest();
                request.startDate = date.Value;
                request.PregDueDate = response.pregDueDate;
                int days = reader.GetPregnencyDays(request).days;
                if (days <= 91)
                    response.Trimester = "1st Trimester";
                else if (days >= 92 && days <= 182)
                    response.Trimester = "2nd Trimester";
                else if (days >= 183)
                    response.Trimester = "3rd Trimester";
            }
            return response;
        }

        public static List<RaffleTypesDto> GetRaffleTypes()
        {
            CommonReader reader = new CommonReader();
            var response = reader.GetRaffleTypes();
            return response;
        }

        public static List<PortalIncentiveDto> GetGiftCards(int incentiveType, int participantPortalId)
        {
            CommonReader reader = new CommonReader();
            var portalId = participantPortalId;
            var response = reader.GetGiftCards(incentiveType, portalId);
            return response;
        }

        public static dynamic dateFormater(DateTime date, bool? includeTime, string dateFormat)
        {
            if (includeTime.HasValue && includeTime.Value)
                return date.ToString(dateFormat + " hh:mm tt");
            else
                return date.ToString(dateFormat);
        }

        public static List<SelectListItem> GetBoolQuestion()
        {
            List<SelectListItem> versions = new List<SelectListItem>();
            versions.Add(new SelectListItem() { Text = "Yes", Value = true.ToString() });
            versions.Add(new SelectListItem() { Text = "No", Value = false.ToString() });

            return versions;
        }

        public static List<SelectListItem> GetHRAOption()
        {
            List<SelectListItem> versions = new List<SelectListItem>();
            versions.Add(new SelectListItem() { Text = "Yes", Value = "1" });
            versions.Add(new SelectListItem() { Text = "No", Value = "2" });
            versions.Add(new SelectListItem() { Text = "Optional", Value = "3" });
            return versions;
        }

        public static List<SelectListItem> GetHRAStatus()
        {
            List<SelectListItem> versions = new List<SelectListItem>();
            versions.Add(new SelectListItem() { Text = "Yes", Value = ((byte)HRAStatus.Yes).ToString() });
            versions.Add(new SelectListItem() { Text = "No", Value = ((byte)HRAStatus.No).ToString() });
            versions.Add(new SelectListItem() { Text = "Optional", Value = ((byte)HRAStatus.Optional).ToString() });

            return versions;
        }

        public static List<SelectListItem> GetHRAVersions()
        {
            List<SelectListItem> versions = new List<SelectListItem>();
            versions.Add(new SelectListItem() { Text = "Full Version", Value = ((byte)HRAVersions.FullVersion).ToString() });
            versions.Add(new SelectListItem() { Text = "Intuity Version", Value = ((byte)HRAVersions.IntuityVersion).ToString() });
            versions.Add(new SelectListItem() { Text = "Activate Version", Value = ((byte)HRAVersions.ActivateVersion).ToString() });
            versions.Add(new SelectListItem() { Text = "Captiva Version", Value = ((byte)HRAVersions.CaptivaVersion).ToString() });

            return versions;
        }

        public static List<SelectListItem> AssessmentNames()
        {
            List<SelectListItem> assessmentNames = new List<SelectListItem>();
            assessmentNames.Add(new SelectListItem() { Text = Translate.Message("L535"), Value = "L535" });
            assessmentNames.Add(new SelectListItem() { Text = Translate.Message("L3845"), Value = "L3845" });
            assessmentNames.Add(new SelectListItem() { Text = Translate.Message("L4425"), Value = "L4425" });

            return assessmentNames;
        }

        public static List<SelectListItem> GetIntegrationsList()
        {
            List<SelectListItem> integrations = new List<SelectListItem>();
            integrations.Add(new SelectListItem() { Text = "Intuity", Value = ((byte)Integrations.Intuity).ToString() });
            integrations.Add(new SelectListItem() { Text = "Activate", Value = ((byte)Integrations.Activate).ToString() });
            integrations.Add(new SelectListItem() { Text = "LMC", Value = ((byte)Integrations.LMC).ToString() });
            integrations.Add(new SelectListItem() { Text = "Captiva", Value = ((byte)Integrations.Captiva).ToString() });
            return integrations;
        }

        public static List<SelectListItem> GetCarePlanTypes()
        {
            List<SelectListItem> formats = new List<SelectListItem>();
            formats.Add(new SelectListItem() { Text = "Stroke", Value = ((byte)CarePlanTypes.Stroke).ToString() });
            formats.Add(new SelectListItem() { Text = "CVD", Value = ((byte)CarePlanTypes.CVD).ToString() });
            formats.Add(new SelectListItem() { Text = "Blood Pressure", Value = ((byte)CarePlanTypes.BloodPressure).ToString() });
            formats.Add(new SelectListItem() { Text = "Weight Management", Value = ((byte)CarePlanTypes.WeightManagement).ToString() });

            return formats;
        }

        public static List<SelectListItem> GetEligibilityFormats()
        {
            List<SelectListItem> formats = new List<SelectListItem>();
            formats.Add(new SelectListItem() { Text = "Standard Format", Value = ((byte)EligibilityFormats.StandardFormat).ToString() });
            formats.Add(new SelectListItem() { Text = "Compass Format", Value = ((byte)EligibilityFormats.CompassFormat).ToString() });
            formats.Add(new SelectListItem() { Text = "LMC Format", Value = ((byte)EligibilityFormats.LMCFormat).ToString() });
            formats.Add(new SelectListItem() { Text = "Intuity Format", Value = ((byte)EligibilityFormats.IntuityFormat).ToString() });
            formats.Add(new SelectListItem() { Text = "MetLife Gulf Format", Value = ((byte)EligibilityFormats.MetLifeGulfFormat).ToString() });
            formats.Add(new SelectListItem() { Text = "Captiva Format", Value = ((byte)EligibilityFormats.CaptivaFormat).ToString() });

            return formats;
        }

        public static List<SelectListItem> GetEligibilitytoIntuity()
        {
            List<SelectListItem> formats = new List<SelectListItem>();
            formats.Add(new SelectListItem() { Text = "All Records", Value = ((byte)EligibilitytoIntuity.All).ToString() });
            formats.Add(new SelectListItem() { Text = "Only Diabetes", Value = ((byte)EligibilitytoIntuity.OnlyDiabetes).ToString() });

            return formats;
        }

        public static IList<ReferralTypesDto> GetReferralTypes()
        {
            CommonReader reader = new CommonReader();
            return reader.GetReferralTypeList().ReferralType;
        }

        public static bool IsIntegratedWithLMC(int? integrationWith)
        {
            if (integrationWith.HasValue && integrationWith.Value == (byte)Integrations.LMC)
                return true;
            else
                return false;
        }

        public static bool IsIntegratedWithIntuity(int? integrationWith)
        {
            if (integrationWith.HasValue && integrationWith.Value == (byte)Integrations.Intuity)
                return true;
            else
                return false;
        }

        public static bool IsIntegratedWithCaptiva(int? integrationWith)
        {
            if (integrationWith.HasValue && integrationWith.Value == (byte)Integrations.Captiva)
                return true;
            else
                return false;
        }

        public static string GetGenderSpecificImage(UserDto user)
        {
            return user.Gender == 1 ? "avatar-male.svg" : "avatar-female.svg";
        }

        public static ListDepartmentResponse ListDepartments(int organizationId)
        {
            ListDepartmentRequest request = new ListDepartmentRequest();
            request.organizatioId = organizationId;
            CommonReader reader = new CommonReader();
            return reader.ListDepartments(request);
        }

        public static SelectList GetDailyVitalsQuestionsList()
        {
            List<KeyValue> dailyVaitals = new List<KeyValue>();
            dailyVaitals.Add(new KeyValue() { Value = "1", Text = Translate.Message("L3369") + " " + Translate.Message("L4428") });
            dailyVaitals.Add(new KeyValue() { Value = "2", Text = Translate.Message("L2820") });
            dailyVaitals.Add(new KeyValue() { Value = "3", Text = Translate.Message("L2821") });
            dailyVaitals.Add(new KeyValue() { Value = "4", Text = Translate.Message("L2822") });
            dailyVaitals.Add(new KeyValue() { Value = "5", Text = Translate.Message("L2823") });
            dailyVaitals.Add(new KeyValue() { Value = "6", Text = Translate.Message("L2824") });
            dailyVaitals.Add(new KeyValue() { Value = "7", Text = Translate.Message("L2825") });
            dailyVaitals.Add(new KeyValue() { Value = "8", Text = Translate.Message("L2827") });
            dailyVaitals.Add(new KeyValue() { Value = "9", Text = Translate.Message("L2828") });
            dailyVaitals.Add(new KeyValue() { Value = "10", Text = Translate.Message("L2829") });
            SelectList dailyVaitalsList = new SelectList(dailyVaitals, "Value", "Text");
            return dailyVaitalsList;
        }

        public static SelectList GetReasonUnableToMonitorBP()
        {
            List<KeyValue> reason = new List<KeyValue>();
            reason.Add(new KeyValue() { Value = "1", Text = "Forgot" });
            reason.Add(new KeyValue() { Value = "2", Text = "Don’t know how to use" });
            reason.Add(new KeyValue() { Value = "3", Text = "Tried but did not work" });
            reason.Add(new KeyValue() { Value = "4", Text = "Device not working" });
            reason.Add(new KeyValue() { Value = "5", Text = "Don’t want to/Not interested" });
            reason.Add(new KeyValue() { Value = "6", Text = "Other (text)" });
            SelectList reasons = new SelectList(reason, "Value", "Text");
            return reasons;
        }

        public static SelectList GetFactorsHelpfulInMonitoringBP()
        {
            List<KeyValue> factor = new List<KeyValue>();
            factor.Add(new KeyValue() { Value = "1", Text = "Motivated (improve health)" });
            factor.Add(new KeyValue() { Value = "2", Text = "Device easy to use" });
            factor.Add(new KeyValue() { Value = "3", Text = "Social support" });
            factor.Add(new KeyValue() { Value = "4", Text = "Making time/Scheduling time" });
            factor.Add(new KeyValue() { Value = "5", Text = "Using reminders" });
            factor.Add(new KeyValue() { Value = "6", Text = "Other (text)" });
            SelectList factors = new SelectList(factor, "Value", "Text");
            return factors;
        }

        public static SelectList GetReasonsNotTakingMedications()
        {
            List<KeyValue> reason = new List<KeyValue>();
            reason.Add(new KeyValue() { Value = "1", Text = "Forgot" });
            reason.Add(new KeyValue() { Value = "2", Text = "Ran out of medication(s)" });
            reason.Add(new KeyValue() { Value = "3", Text = "Cannot afford" });
            reason.Add(new KeyValue() { Value = "4", Text = "Did not renew prescription" });
            reason.Add(new KeyValue() { Value = "5", Text = "Lost medication" });
            reason.Add(new KeyValue() { Value = "6", Text = "Other (text)" });
            SelectList reasons = new SelectList(reason, "Value", "Text");
            return reasons;
        }

        public static SelectList GetReasonsAbleToAchievePhysicalActivityGoals()
        {
            List<KeyValue> reason = new List<KeyValue>();
            reason.Add(new KeyValue() { Value = "1", Text = "Motivated (feel better, improve health, etc.)" });
            reason.Add(new KeyValue() { Value = "2", Text = "Social support" });
            reason.Add(new KeyValue() { Value = "3", Text = "Conducive environment" });
            reason.Add(new KeyValue() { Value = "4", Text = "Other (text)" });
            SelectList reasons = new SelectList(reason, "Value", "Text");
            return reasons;
        }

        public static SelectList GetReasonsUnableToAchievePhysicalActivityGoals()
        {
            List<KeyValue> reason = new List<KeyValue>();
            reason.Add(new KeyValue() { Value = "1", Text = "Motivation low" });
            reason.Add(new KeyValue() { Value = "2", Text = "Social support low" });
            reason.Add(new KeyValue() { Value = "3", Text = "Environment not conducive" });
            reason.Add(new KeyValue() { Value = "4", Text = "Intervening health/medical issues" });
            reason.Add(new KeyValue() { Value = "5", Text = "Lack of time (family responsibilities, work conflict, etc.)" });
            reason.Add(new KeyValue() { Value = "6", Text = "Other (text)" });
            SelectList reasons = new SelectList(reason, "Value", "Text");
            return reasons;
        }

        public static SelectList GetReasonsUnableToFollowHealthyDietGoals()
        {
            List<KeyValue> reason = new List<KeyValue>();
            reason.Add(new KeyValue() { Value = "1", Text = "Can’t afford" });
            reason.Add(new KeyValue() { Value = "2", Text = "Don’t know how to read labels" });
            reason.Add(new KeyValue() { Value = "3", Text = "Don’t like food without salt" });
            reason.Add(new KeyValue() { Value = "4", Text = "Family won’t eat food without salt" });
            reason.Add(new KeyValue() { Value = "5", Text = "Other (text)" });
            SelectList reasons = new SelectList(reason, "Value", "Text");
            return reasons;
        }

        public static SelectList GetHowSeriousWasInjury()
        {
            List<KeyValue> injury = new List<KeyValue>();
            injury.Add(new KeyValue() { Value = "1", Text = "Minor (application of dressing, ice, cleaning, dressing, bruising or abrasion)" });
            injury.Add(new KeyValue() { Value = "2", Text = "Moderate (suturing, application of steri-strips/skin glue, splinting, muscle/joint strain)" });
            injury.Add(new KeyValue() { Value = "3", Text = "Major (surgery, casting, traction, consultation for neurological or internal injury, receipt of blood products)" });
            SelectList injuryList = new SelectList(injury, "Value", "Text");
            return injuryList;
        }

        public static SelectList GetReasonsUnableToAttendRehabilitationTherapy()
        {
            List<KeyValue> reason = new List<KeyValue>();
            reason.Add(new KeyValue() { Value = "1", Text = "Did not schedule" });
            reason.Add(new KeyValue() { Value = "2", Text = "Not motivated/Not interested" });
            reason.Add(new KeyValue() { Value = "3", Text = "No transport" });
            reason.Add(new KeyValue() { Value = "4", Text = "Can’t afford" });
            reason.Add(new KeyValue() { Value = "5", Text = "Time conflicts" });
            reason.Add(new KeyValue() { Value = "6", Text = "Other (text)" });
            SelectList reasons = new SelectList(reason, "Value", "Text");
            return reasons;
        }

        public static List<SelectListItem> GetProviderDetails()
        {
            List<SelectListItem> formats = new List<SelectListItem>();
            formats.Add(new SelectListItem() { Text = "Plain Text", Value = ((byte)ProviderDetails.PlainText).ToString() });
            formats.Add(new SelectListItem() { Text = "Drop Down", Value = ((byte)ProviderDetails.DropDown).ToString() });

            return formats;
        }

        public static List<ProviderDto> GetProvidersList(int orgId)
        {
            CommonReader reader = new CommonReader();
            return reader.GetProvidersList(orgId);
        }

        public static string Encrypt(string encryptString)
        {
            CommonReader reader = new CommonReader();
            return reader.Encrypt(encryptString);
        }

        public static string Decrypt(string cipherText)
        {
            CommonReader reader = new CommonReader();
            return reader.Decrypt(cipherText);
        }

        public static List<SelectListItem> GetWebinarTypes()
        {
            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "Webinar", Value = ((int)WebinarTypes.Webinar).ToString() });
            types.Add(new SelectListItem() { Text = "Recurring webinar with no fixed time", Value = ((int)WebinarTypes.RecurringWebinarNoFixedTime).ToString() });
            types.Add(new SelectListItem() { Text = "Recurring webinar with a fixed time", Value = ((int)WebinarTypes.RecurringWebinarFixedTime).ToString() });
            return types;
        }

        public static List<SelectListItem> GetAssessmentTypes()
        {
            List<SelectListItem> assessmentTypes = new List<SelectListItem>();
            assessmentTypes.Add(new SelectListItem() { Text = "HRA", Value = ((byte)ReportTypes.HRA).ToString() });
            assessmentTypes.Add(new SelectListItem() { Text = "Follow-up", Value = ((byte)ReportTypes.FollowUp).ToString() });
            return assessmentTypes;
        }
    }

    //any change to this must also be made in ListOptions.cs
    public static class BioLookup
    {
        public const int Cholesterol = 0;
        public const int Triglycerides = 1;
        public const int HDL = 2;
        public const int LDL = 3;
        public const int Glucose = 4;
        public const int Weight = 5;
        public const int Height = 6;
        public const int Waist = 7;
        public const int SBP = 8;
        public const int DBP = 9;
        public const int A1c = 10;
        public const int HeartRate = 11;
        public const int RMR = 12;
        public const int CAC = 13;
        public const int CRF = 14;
        public const int RHR = 15;
    }

    public static class Coachinglevel
    {
        public const int IncompleteHRA = 1;
        public const int HRAOnly = 2;
        public const int HRAandBiometrics = 3;
        public const int FirstCoaching = 4;
        public const int CompletedCoaching = 5;
        public const int FirstTimeHRA = 6;
    }
    public static class GlucoseRange
    {
        public const int GlucoseRange1 = 54;
        public const int GlucoseRange2 = 70;
        public const int GlucoseRange3 = 180;
        public const int NonDiabetesGlucoseRange3 = 140;
        public const int GlucoseRange4 = 250;

        public const int PreMealRange1 = 54;
        public const int PreMealRange2 = 70;
        public const int PreMealRange3 = 130;
        public const int NonDiabetesPreMealRange3 = 100;
        public const int PreMealRange4 = 250;
        public const int PreMealGoalFrom = 80;
        public const int PreMealGoalTo = 130;
        public const int NonDiabetesPreMealGoalTo = 100;

        public const int PostMealRange1 = 54;
        public const int PostMealRange2 = 70;
        public const int PostMealRange3 = 180;
        public const int NonDiabetesPostMealRange3 = 140;
        public const int PostMealRange4 = 250;
        public const int PostMealGoalFrom = 80;
        public const int PostMealGoalTo = 180;
        public const int NonDiabetesPostMealGoalFrom = 70;
        public const int NonDiabetesPostMealGoalTo = 140;

    }

    public static class DeviceGlucoseRange
    {
        public const int GlucoseRange1 = 54;
        public const int GlucoseRange2 = 70;
        public const int GlucoseRange3 = 180;
        public const int NonDiabetesGlucoseRange3 = 139;
        public const int NonDiabetesGlucoseRange4 = 199;
        public const int GlucoseRange4 = 250;

        public const int PreMealRange1 = 54;
        public const int PreMealRange2 = 70;
        public const int PreMealRange3 = 130;
        public const int NonDiabetesPreMealRange3 = 99;
        public const int PreMealRange4 = 180;
        public const int NonDiabetesPreMealRange4 = 125;
        public const int PreMealGoalFrom = 80;
        public const int PreMealGoalTo = 130;
        public const int NonDiabetesPreMealGoalFrom = 70;
        public const int NonDiabetesPreMealGoalTo = 99;


        public const int PostMealRange1 = 54;
        public const int PostMealRange2 = 70;
        public const int PostMealRange3 = 180;
        public const int NonDiabetesPostMealRange3 = 139;
        public const int PostMealRange4 = 250;
        public const int NonDiabetesPostMealRange4 = 199;
        public const int PostMealGoalFrom = 80;
        public const int PostMealGoalTo = 180;
        public const int NonDiabetesPostMealGoalFrom = 70;
        public const int NonDiabetesPostMealGoalTo = 139;

    }

    public static class RoutineRange
    {
        public const int StepsRange1 = 5000;
        public const int StepsRange2 = 7500;
        public const int StepsRange3 = 10000;
        public const int StepsRange4 = 15000;
        public const int TimeRange1 = 30;
        public const int TimeRange2 = 45;
        public const int TimeRange3 = 60;
        public const int TimeRange4 = 300;
    }

    public static class Biometrics
    {
        public const int SystolicRange1 = 120;
        public const int SystolicRange2 = 140;
        public const int SystolicRange3 = 160;

        public const int DiastolicRange1 = 80;
        public const int DiastolicRange2 = 90;
        public const int DiastolicRange3 = 100;
    }

    public static class LabReference
    {
        public const int GlucoseFasting = 0;
        public const int GlucoseNonFasting = 1;
        public const int TotalCholesterol = 2;
        public const int Triglycerides = 3;
        public const int HDL = 4;
        public const int LDL = 5;
        public const int SystolicBloodPressure = 6;
        public const int DiastolicBloodPressure = 7;
        public const int WaistCircumference = 8;
        public const int Weight = 9;
        public const int HbA1c = 10;
    }

    public static class LabSelection
    {
        public const int DoctorsOffice = 1;
        public const int LabCorp = 2;
        public const int None = 3;
    }

    public enum NoteTypes
    {
        Coaching = 1,
        Outreach = 2,
        Tracking = 3,
        Note = 4,
        Other = 5,
        OtherReferrals = 6,
        BiometricReview = 7,
        PCPReferral = 8,
        Navigation_Call = 9,
        Critical_Alert = 10,
        HRA_Review = 11,
        Health_History = 12
    }

    public enum CRMNoteTypes
    {
        Email = 1,
        Call = 2,
        WebForm = 3
    }

    public enum InquiryType
    {
        GeneralProductInformation = 1,
        ProductCostandCoverage = 2,
        POGOTroubleshooting = 3,
        PatternsTroubleshooting = 4,
        OrderInquiry = 5,
        CancelOrder = 6,
        Subscription = 7,
        RequestControlSolutions = 8,
        HealthCoaching = 9
    }

    public enum ExerciseTypes
    {
        Steps = 90
    }

    public static class DashboardMessageTypes
    {
        public const string KitAudio = "Kit Audio";
        public const string NewsLetter = "NewsLetter";
        public const string ProfileComplete = "Profile_Complete";
        public const string Appointment = "Appointment";
    }

    public enum MessageStatus
    {
        Pending = 1,
        Active = 2,
        FollowUp = 3
    }

    public static class EbenPersonnelType
    {
        public const string Employee = "00";
        public const string Spouse = "01";
    }

    public class KeyValue
    {
        public string Value { get; set; }

        public String Text { get; set; }
    }

    public class AgeInfo
    {
        public int Age { get; set; }

        public string heartAge { get; set; }

        public int? arterialAge { get; set; }

        public string heartAgeMessage { get; set; }
    }

    public enum ReportTypes
    {
        HRA = 1,
        FollowUp = 2
    }

    public static class LanguageCode
    {
        public const string English = "en-us";
    }

    public static class FileSizeFormatter
    {
        static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }

    public enum SecurityCodeValidityType
    {
        Active = 1,
        Validated = 2,
        Expired = 3
    }

    public enum WearableDeviceType
    {
        Web = 1,
        Mobile = 2,
        Both = 3
    }

    public enum MediOrbisProgramStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2
    }


}