using System.ComponentModel;
using System.Reflection;

namespace Intervent.Web.DTO
{
    public class ListOptions
    {
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

        public static string DefaultLanguage = "en-us";

        public static IEnumerable<ControlValue> SmokingDetails()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Never smoke", DisplayText = "L54", Value = "1" });
            values.Add(new ControlValue() { Text = "Quit within the past 6 months", DisplayText = "L55", Value = "2" });
            values.Add(new ControlValue() { Text = "Quit more than 6 months ago", DisplayText = "L56", Value = "3" });
            return values;
        }

        public static IEnumerable<ControlValue> GetStateOfHealthLists()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Excellent", DisplayText = "L89", Value = "1" });
            values.Add(new ControlValue() { Text = "Very Good", DisplayText = "L90", Value = "2" });
            values.Add(new ControlValue() { Text = "Good", DisplayText = "L91", Value = "3" });
            values.Add(new ControlValue() { Text = "Fair", DisplayText = "L92", Value = "4" });
            values.Add(new ControlValue() { Text = "Poor", DisplayText = "L93", Value = "5" });
            return values;
        }

        public static IEnumerable<ControlValue> GetLifeSatisfactionList()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Completely satisfied", DisplayText = "L95", Value = "1" });
            values.Add(new ControlValue() { Text = "Mostly satisfied", DisplayText = "L96", Value = "2" });
            values.Add(new ControlValue() { Text = "Partly satisfied", DisplayText = "L97", Value = "3" });
            values.Add(new ControlValue() { Text = "Not satisfied", DisplayText = "L98", Value = "4" });
            return values;
        }

        public static IEnumerable<ControlValue> GetJobSatisfactionList()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Completely satisfied", DisplayText = "L95", Value = "1" });
            values.Add(new ControlValue() { Text = "Mostly satisfied", DisplayText = "L96", Value = "2" });
            values.Add(new ControlValue() { Text = "Partly satisfied", DisplayText = "L97", Value = "3" });
            values.Add(new ControlValue() { Text = "Not satisfied", DisplayText = "L98", Value = "4" });
            values.Add(new ControlValue() { Text = "Not applicable", DisplayText = "L100", Value = "5" });
            return values;
        }

        public static IEnumerable<ControlValue> GetRelaxMedList()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Almost every day", DisplayText = "L102", Value = "1" });
            values.Add(new ControlValue() { Text = "A few times per month", DisplayText = "L103", Value = "2" });
            values.Add(new ControlValue() { Text = "Rarely", DisplayText = "L104", Value = "3" });
            values.Add(new ControlValue() { Text = "Never", DisplayText = "L105", Value = "4" });
            return values;
        }

        public static IEnumerable<ControlValue> GetMissDays()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "0 days", DisplayText = "L2580", Value = "1" });
            values.Add(new ControlValue() { Text = "1-2 days", DisplayText = "L2581", Value = "2" });
            values.Add(new ControlValue() { Text = "3-5 days", DisplayText = "L2582", Value = "3" });
            values.Add(new ControlValue() { Text = "6-10 days", DisplayText = "L2583", Value = "4" });
            values.Add(new ControlValue() { Text = "11-15 days", DisplayText = "L2584", Value = "5" });
            values.Add(new ControlValue() { Text = "16+ days", DisplayText = "L2585", Value = "6" });
            values.Add(new ControlValue() { Text = "Don't know", DisplayText = "L192", Value = "7" });
            values.Add(new ControlValue() { Text = "Not applicable", DisplayText = "L100", Value = "8" });
            return values;
        }

        public static IEnumerable<ControlValue> GetTimes()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "0 times", DisplayText = "L2586", Value = "1" });
            values.Add(new ControlValue() { Text = "Once", DisplayText = "L194", Value = "2" });
            values.Add(new ControlValue() { Text = "Twice", DisplayText = "L195", Value = "3" });
            values.Add(new ControlValue() { Text = "3-4 times", DisplayText = "L2587", Value = "4" });
            values.Add(new ControlValue() { Text = "5+ times", DisplayText = "L2588", Value = "5" });
            values.Add(new ControlValue() { Text = "Don't know", DisplayText = "L192", Value = "6" });
            return values;
        }

        public static IEnumerable<ControlValue> GetHealthProblems()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "No health problems", DisplayText = "L196", Value = "1" });
            values.Add(new ControlValue() { Text = "None of the time", DisplayText = "L197", Value = "2" });
            values.Add(new ControlValue() { Text = "Some of the time", DisplayText = "L198", Value = "3" });
            values.Add(new ControlValue() { Text = "Most of the time", DisplayText = "L199", Value = "4" });
            values.Add(new ControlValue() { Text = "All of the time", DisplayText = "L200", Value = "5" });
            values.Add(new ControlValue() { Text = "Don't know", DisplayText = "L192", Value = "6" });
            values.Add(new ControlValue() { Text = "Not applicable", DisplayText = "L100", Value = "7" });
            return values;
        }

        public static IEnumerable<ControlValue> ArmUsed()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Left", DisplayText = "L202", Value = "1" });
            values.Add(new ControlValue() { Text = "Right", DisplayText = "L203", Value = "2" });
            values.Add(new ControlValue() { Text = "Don't know", DisplayText = "L122", Value = "3" });
            return values;
        }

        public static IEnumerable<ControlValue> GetNamePrefixList()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Dr", DisplayText = "L2589", Value = "DR" });
            values.Add(new ControlValue() { Text = "Mr", DisplayText = "L2590", Value = "MR" });
            values.Add(new ControlValue() { Text = "Miss", DisplayText = "L2591", Value = "MISS" });
            values.Add(new ControlValue() { Text = "Mrs", DisplayText = "L2592", Value = "MRS" });
            values.Add(new ControlValue() { Text = "Ms", DisplayText = "L2593", Value = "MS" });
            return values;
        }

        public static IEnumerable<ControlValue> GetGenderList(bool? includeOthers)
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Male", DisplayText = "L928", Value = "1" });
            values.Add(new ControlValue() { Text = "Female", DisplayText = "L929", Value = "2" });
            if (includeOthers.HasValue && includeOthers.Value)
                values.Add(new ControlValue() { Text = "Others", DisplayText = "Others", Value = "3" });
            return values;
        }

        public static IEnumerable<ControlValue> GetNoteType()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Email", DisplayText = "Email", Value = "1" });
            values.Add(new ControlValue() { Text = "Call", DisplayText = "Call", Value = "2" });
            return values;
        }

        public static IEnumerable<ControlValue> GetRaceList()
        {
            List<ControlValue> race = new List<ControlValue>();
            race.Add(new ControlValue() { Value = "1", DisplayText = "L177", Text = "Caucasian" });
            race.Add(new ControlValue() { Value = "2", DisplayText = "L174", Text = "African American" });
            race.Add(new ControlValue() { Value = "3", DisplayText = "L175", Text = "American Indian/Native American" });
            race.Add(new ControlValue() { Value = "4", DisplayText = "L176", Text = "Asian" });
            race.Add(new ControlValue() { Value = "5", DisplayText = "L178", Text = "Hispanic" });
            race.Add(new ControlValue() { Value = "6", DisplayText = "L179", Text = "Pacific Islander" });
            race.Add(new ControlValue() { Value = "7", DisplayText = "L3014", Text = "Other" });
            return race;
        }

        public static IEnumerable<ControlValue> GetCanriskMaleRaceList()
        {
            List<ControlValue> race = new List<ControlValue>();
            race.Add(new ControlValue() { Value = "1", DisplayText = "L3733", Text = "White (Caucasian)" });
            race.Add(new ControlValue() { Value = "2", DisplayText = "L3734", Text = "Aboriginal" });
            race.Add(new ControlValue() { Value = "3", DisplayText = "L3735", Text = "Black (Afro-Caribbean)" });
            race.Add(new ControlValue() { Value = "4", DisplayText = "L3736", Text = "East Asian (Chinese, Vietnamese, Filipino, Korean, etc.)" });
            race.Add(new ControlValue() { Value = "5", DisplayText = "L3212", Text = "South Asian (East Indian, Pakistani, Sri Lankan, etc.)" });
            race.Add(new ControlValue() { Value = "6", DisplayText = "L3737", Text = "Other non-white (Latin American, Arab, West Asian)" });
            race.Add(new ControlValue() { Value = "7", DisplayText = "L3838", Text = "Don’t know" });
            return race;
        }

        public static IEnumerable<ControlValue> GetCanriskFemaleRaceList()
        {
            List<ControlValue> race = new List<ControlValue>();
            race.Add(new ControlValue() { Value = "1", DisplayText = "L3839", Text = "White (Caucasian)" });
            race.Add(new ControlValue() { Value = "2", DisplayText = "L3840", Text = "Aboriginal" });
            race.Add(new ControlValue() { Value = "3", DisplayText = "L3841", Text = "Black (Afro-Caribbean)" });
            race.Add(new ControlValue() { Value = "4", DisplayText = "L3842", Text = "East Asian (Chinese, Vietnamese, Filipino, Korean, etc.)" });
            race.Add(new ControlValue() { Value = "5", DisplayText = "L3843", Text = "South Asian (East Indian, Pakistani, Sri Lankan, etc.)" });
            race.Add(new ControlValue() { Value = "6", DisplayText = "L3844", Text = "Other non-white (Latin American, Arab, West Asian)" });
            race.Add(new ControlValue() { Value = "7", DisplayText = "L3838", Text = "Don’t know" });
            return race;
        }

        public static IEnumerable<ControlValue> GetPreferredContactMode()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "By Home Phone", DisplayText = "L577", Value = "1" });
            values.Add(new ControlValue() { Text = "By Work Phone", DisplayText = "L578", Value = "2" });
            values.Add(new ControlValue() { Text = "By Cell Phone", DisplayText = "L579", Value = "3" });
            return values;
        }

        public static IEnumerable<ControlValue> GetPreferredContactTimes()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Before 12 pm", DisplayText = "L2594", Value = "1" });
            values.Add(new ControlValue() { Text = "12 pm to 3 pm", DisplayText = "L2595", Value = "2" });
            values.Add(new ControlValue() { Text = "3 pm to 6 pm", DisplayText = "L2596", Value = "3" });
            values.Add(new ControlValue() { Text = "After 6 pm", DisplayText = "L2597", Value = "4" });
            return values;
        }

        public static IEnumerable<ControlValue> GetSource()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Employer", DisplayText = "L181", Value = "1" });
            values.Add(new ControlValue() { Text = "Doctor or other healthcare provider", DisplayText = "L182", Value = "2" });
            values.Add(new ControlValue() { Text = "Cardiac Rehab Program", DisplayText = "L183", Value = "3" });
            values.Add(new ControlValue() { Text = "Internet Ad", DisplayText = "L184", Value = "4" });
            values.Add(new ControlValue() { Text = "Radio", DisplayText = "L185", Value = "5" });
            values.Add(new ControlValue() { Text = "TV", DisplayText = "L186", Value = "6" });
            values.Add(new ControlValue() { Text = "Word Of Mouth", DisplayText = "L187", Value = "7" });
            values.Add(new ControlValue() { Text = "Social Media", DisplayText = "L3194", Value = "9" });
            values.Add(new ControlValue() { Text = "Recommendation from Family/Friend", DisplayText = "L3195", Value = "10" });
            values.Add(new ControlValue() { Text = "Prefer not to answer", DisplayText = "L3880", Value = "11" });
            values.Add(new ControlValue() { Text = "Other", DisplayText = "L3014", Value = "8" });
            return values;
        }

        public static IEnumerable<ControlValue> GetLanguagePreference()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "English", Value = "en-us" });
            values.Add(new ControlValue() { Text = "Spanish", Value = "es" });
            return values;
        }

        public static IEnumerable<ControlValue> GetUnits()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Imperial (US)", DisplayText = "L2598", Value = "1" });
            values.Add(new ControlValue() { Text = "Metric (Non-US)", DisplayText = "L2599", Value = "2" });
            return values;
        }

        public static IEnumerable<ControlValue> GetExercisefrequency()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Never", DisplayText = "L105", Value = "0" });
            values.Add(new ControlValue() { Text = "Less than once a week", DisplayText = "L3085", Value = "0.5" });
            values.Add(new ControlValue() { Text = "Once a week", DisplayText = "L3086", Value = "1" });
            values.Add(new ControlValue() { Text = "Two to three times a week", DisplayText = "L3087", Value = "2.5" });
            values.Add(new ControlValue() { Text = "Four or more times a week", DisplayText = "L3088", Value = "5" });
            return values;
        }

        public static IEnumerable<ControlValue> GetExerciseIntensity()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "I take it easy without breaking into a sweat or heavy breathing", DisplayText = "L3089", Value = "1" });
            values.Add(new ControlValue() { Text = "I push myself hard enough to break into a sweat and/or breathe heavy", DisplayText = "L3090", Value = "2" });
            values.Add(new ControlValue() { Text = "I push myself to near exhaustion", DisplayText = "L3091", Value = "3" });
            return values;
        }

        public static IEnumerable<ControlValue> GetExerciseMinutes()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Less than 15 minutes", DisplayText = "L3092", Value = "0.1" });
            values.Add(new ControlValue() { Text = "Between 15 and 29 minutes", DisplayText = "L3093", Value = "0.38" });
            values.Add(new ControlValue() { Text = "Between 30 and 60 minutes", DisplayText = "L3094", Value = "0.75" });
            values.Add(new ControlValue() { Text = "More than 60 minutes", DisplayText = "L3095", Value = "1" });
            return values;
        }

        public static IEnumerable<ControlValue> GetLabRejectionReasons()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "Verify fasting status", DisplayText = "", Value = "1" });
            values.Add(new ControlValue() { Text = "Missing glucose", DisplayText = "", Value = "2" });
            values.Add(new ControlValue() { Text = "Missing lipids", DisplayText = "", Value = "3" });
            values.Add(new ControlValue() { Text = "Missing lipids and glucose", DisplayText = "", Value = "4" });
            values.Add(new ControlValue() { Text = "Unable to read", DisplayText = "", Value = "5" });
            values.Add(new ControlValue() { Text = "Missing doctor signature or lab results", DisplayText = "", Value = "6" });
            values.Add(new ControlValue() { Text = "Out of date", DisplayText = "", Value = "7" });
            values.Add(new ControlValue() { Text = "Missing name and / or date of birth", DisplayText = "", Value = "8" });
            values.Add(new ControlValue() { Text = "Missing A1C", DisplayText = "", Value = "9" });
            values.Add(new ControlValue() { Text = "Missing A1C and fasting glucose", DisplayText = "", Value = "10" });
            values.Add(new ControlValue() { Text = "Error", DisplayText = "", Value = "11" });
            return values;
        }

        public static IEnumerable<ControlValue> GetDiabetesTypes()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "", DisplayText = "None", Value = "0" });
            values.Add(new ControlValue() { Text = "", DisplayText = "Type 1", Value = "1" });
            values.Add(new ControlValue() { Text = "", DisplayText = "Type 2", Value = "2" });
            values.Add(new ControlValue() { Text = "", DisplayText = "Gestational", Value = "3" });
            values.Add(new ControlValue() { Text = "", DisplayText = "Other", Value = "4" });
            values.Add(new ControlValue() { Text = "", DisplayText = "Unknown", Value = "5" });
            values.Add(new ControlValue() { Text = "", DisplayText = "NotSupported", Value = "6" });
            return values;
        }

        public static IEnumerable<ControlValue> GetA1cTestReasons()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = "DID_NOT_HAD", DisplayText = "I have not previously had an A1c test", Value = "1" });
            values.Add(new ControlValue() { Text = "UNCERTAIN", DisplayText = "I am uncertain whether I have had an A1c test", Value = "2" });
            return values;
        }

        public static IEnumerable<ControlValue> GetEligibilityStatusList()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = EligibilityStatus.New.ToString(), DisplayText = GetDescription(EligibilityStatus.New), Value = ((int)EligibilityStatus.New).ToString() });
            values.Add(new ControlValue() { Text = EligibilityStatus.Eligible.ToString(), DisplayText = GetDescription(EligibilityStatus.Eligible), Value = ((int)EligibilityStatus.Eligible).ToString() });
            values.Add(new ControlValue() { Text = EligibilityStatus.NotEligible.ToString(), DisplayText = GetDescription(EligibilityStatus.NotEligible), Value = ((int)EligibilityStatus.NotEligible).ToString() });
            values.Add(new ControlValue() { Text = EligibilityStatus.ContactCustomer.ToString(), DisplayText = GetDescription(EligibilityStatus.ContactCustomer), Value = ((int)EligibilityStatus.ContactCustomer).ToString() });
            return values;
        }

        public static IEnumerable<ControlValue> GetUserStatusList()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = EligibilityUserStatusDto.Active.Description, DisplayText = EligibilityUserStatusDto.Active.Description, Value = EligibilityUserStatusDto.Active.Key });
            values.Add(new ControlValue() { Text = EligibilityUserStatusDto.Retired.Description, DisplayText = EligibilityUserStatusDto.Retired.Description, Value = EligibilityUserStatusDto.Retired.Key });
            values.Add(new ControlValue() { Text = EligibilityUserStatusDto.Terminated.Description, DisplayText = EligibilityUserStatusDto.Terminated.Description, Value = EligibilityUserStatusDto.Terminated.Key });
            values.Add(new ControlValue() { Text = EligibilityUserStatusDto.LoA.Description, DisplayText = EligibilityUserStatusDto.LoA.Description, Value = EligibilityUserStatusDto.LoA.Key });
            values.Add(new ControlValue() { Text = EligibilityUserStatusDto.Cobra.Description, DisplayText = EligibilityUserStatusDto.Cobra.Description, Value = EligibilityUserStatusDto.Cobra.Key });
            return values;
        }

        public static IEnumerable<ControlValue> GetEligibilityReasonsList()
        {
            List<ControlValue> values = new List<ControlValue>();
            values.Add(new ControlValue() { Text = EligibilityReason.None.ToString(), Value = ((int)EligibilityReason.None).ToString(), DisplayText = GetDescription(EligibilityReason.None) });
            values.Add(new ControlValue() { Text = EligibilityReason.ToldDiabetes.ToString(), Value = ((int)EligibilityReason.ToldDiabetes).ToString(), DisplayText = GetDescription(EligibilityReason.ToldDiabetes) });
            values.Add(new ControlValue() { Text = EligibilityReason.TakeMedication.ToString(), Value = ((int)EligibilityReason.TakeMedication).ToString(), DisplayText = GetDescription(EligibilityReason.TakeMedication) });
            values.Add(new ControlValue() { Text = EligibilityReason.ToldDiabetes_FastingGlucose.ToString(), Value = ((int)EligibilityReason.ToldDiabetes_FastingGlucose).ToString(), DisplayText = GetDescription(EligibilityReason.ToldDiabetes_FastingGlucose) });
            values.Add(new ControlValue() { Text = EligibilityReason.ToldDiabetes_NonFastingGlucose.ToString(), Value = ((int)EligibilityReason.ToldDiabetes_NonFastingGlucose).ToString(), DisplayText = GetDescription(EligibilityReason.ToldDiabetes_NonFastingGlucose) });
            values.Add(new ControlValue() { Text = EligibilityReason.ToldDiabetes_A1CGlucose.ToString(), Value = ((int)EligibilityReason.ToldDiabetes_A1CGlucose).ToString(), DisplayText = GetDescription(EligibilityReason.ToldDiabetes_A1CGlucose) });
            values.Add(new ControlValue() { Text = EligibilityReason.AnsweredDiabetes.ToString(), Value = ((int)EligibilityReason.AnsweredDiabetes).ToString(), DisplayText = GetDescription(EligibilityReason.AnsweredDiabetes) });
            values.Add(new ControlValue() { Text = EligibilityReason.Answered_Diabetes_Med.ToString(), Value = ((int)EligibilityReason.Answered_Diabetes_Med).ToString(), DisplayText = GetDescription(EligibilityReason.Answered_Diabetes_Med) });
            values.Add(new ControlValue() { Text = EligibilityReason.A1C_OutOfRange.ToString(), Value = ((int)EligibilityReason.A1C_OutOfRange).ToString(), DisplayText = GetDescription(EligibilityReason.A1C_OutOfRange) });
            values.Add(new ControlValue() { Text = EligibilityReason.NoMedHistoryLabNotOutofRange.ToString(), Value = ((int)EligibilityReason.NoMedHistoryLabNotOutofRange).ToString(), DisplayText = GetDescription(EligibilityReason.NoMedHistoryLabNotOutofRange) });
            values.Add(new ControlValue() { Text = EligibilityReason.Terminated.ToString(), Value = ((int)EligibilityReason.Terminated).ToString(), DisplayText = GetDescription(EligibilityReason.Terminated) });
            values.Add(new ControlValue() { Text = EligibilityReason.Retired.ToString(), Value = ((int)EligibilityReason.Retired).ToString(), DisplayText = GetDescription(EligibilityReason.Retired) });
            values.Add(new ControlValue() { Text = EligibilityReason.LoA.ToString(), Value = ((int)EligibilityReason.LoA).ToString(), DisplayText = GetDescription(EligibilityReason.LoA) });
            return values;
        }

        public static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }


        public static dynamic CovertIntoImperial(dynamic model, IList<MeasurementsDto> Measurements)
        {
            model.TotalChol = model.TotalChol != null ? (float)Math.Round(ToImperial(model.TotalChol, BioLookup.Cholesterol, Measurements), 2) : model.TotalChol;
            model.Trig = model.Trig != null ? (float)Math.Round(ToImperial(model.Trig, BioLookup.Triglycerides, Measurements), 2) : model.Trig;
            model.HDL = model.HDL != null ? (float)Math.Round(ToImperial(model.HDL, BioLookup.HDL, Measurements), 2) : model.HDL;
            model.LDL = model.LDL != null ? (float)Math.Round(ToImperial(model.LDL, BioLookup.LDL, Measurements), 2) : model.LDL;
            model.Glucose = model.Glucose != null ? (float)Math.Round(ToImperial(model.Glucose, BioLookup.Glucose, Measurements), 2) : model.Glucose;
            model.Height = model.HeightCM != null ? (float)Math.Round(ToImperial(model.HeightCM, BioLookup.Height, Measurements), 2) : model.HeightCM;
            model.Weight = model.Weight != null ? (float)Math.Round(ToImperial(model.Weight, BioLookup.Weight, Measurements), 2) : model.Weight;
            model.Waist = model.Waist != null ? (float)Math.Round(ToImperial(model.Waist, BioLookup.Waist, Measurements), 2) : model.Waist;

            return model;
        }

        public static float ToImperial(float value, int type, IList<MeasurementsDto> Measurements)
        {
            switch (type)
            {
                case BioLookup.Glucose:
                    return (value * Measurements[BioLookup.Glucose].ConversionValue.Value);
                case BioLookup.LDL:
                    return (value / Measurements[BioLookup.LDL].ConversionValue.Value);
                case BioLookup.HDL:
                    return (value / Measurements[BioLookup.HDL].ConversionValue.Value);
                case BioLookup.Cholesterol:
                    return (value / Measurements[BioLookup.Cholesterol].ConversionValue.Value);
                case BioLookup.Triglycerides:
                    return (value / Measurements[BioLookup.Triglycerides].ConversionValue.Value);
                case BioLookup.Weight:
                    return (value * Measurements[BioLookup.Weight].ConversionValue.Value);
                case BioLookup.Height:
                    return (value / Measurements[BioLookup.Height].ConversionValue.Value);
                case BioLookup.Waist:
                    return (value / Measurements[BioLookup.Waist].ConversionValue.Value);
            }
            return value;
        }

        public static dynamic CovertIntoMetric(dynamic model, IList<MeasurementsDto> Measurements)
        {
            model.TotalChol = model.TotalChol != null ? (float)Math.Round(ToMetric(model.TotalChol, BioLookup.Cholesterol, Measurements), 1) : model.TotalChol;
            model.Trig = model.Trig != null ? (float)Math.Round(ToMetric(model.Trig, BioLookup.Triglycerides, Measurements), 1) : model.Trig;
            model.HDL = model.HDL != null ? (float)Math.Round(ToMetric(model.HDL, BioLookup.HDL, Measurements), 1) : model.HDL;
            model.LDL = model.LDL != null ? (float)Math.Round(ToMetric(model.LDL, BioLookup.LDL, Measurements), 1) : model.LDL;
            model.Glucose = model.Glucose != null ? (float)Math.Round(ToMetric(model.Glucose, BioLookup.Glucose, Measurements), 1) : model.Glucose;
            model.HeightCM = model.Height != null ? (float)Math.Round(ToMetric(model.Height, BioLookup.Height, Measurements), 1) : model.Height;
            model.Weight = model.Weight != null ? (float)Math.Round(ToMetric(model.Weight, BioLookup.Weight, Measurements), 1) : model.Weight;
            model.Waist = model.Waist != null ? (float)Math.Round(ToMetric(model.Waist, BioLookup.Waist, Measurements), 1) : model.Waist;
            return model;
        }

        public static float ToMetric(float value, int type, IList<MeasurementsDto> Measurements)
        {
            switch (type)
            {
                case BioLookup.Glucose:
                    return (float)Math.Round((value / Measurements[BioLookup.Glucose].ConversionValue.Value), 1);
                case BioLookup.LDL:
                    return (float)Math.Round((value * Measurements[BioLookup.LDL].ConversionValue.Value), 1);
                case BioLookup.HDL:
                    return (float)Math.Round((value * Measurements[BioLookup.HDL].ConversionValue.Value), 1);
                case BioLookup.Cholesterol:
                    return (float)Math.Round((value * Measurements[BioLookup.Cholesterol].ConversionValue.Value), 1);
                case BioLookup.Triglycerides:
                    return (float)Math.Round((value * Measurements[BioLookup.Triglycerides].ConversionValue.Value), 1);
                case BioLookup.Weight:
                    return (float)Math.Round((value / Measurements[BioLookup.Weight].ConversionValue.Value), 1);
                case BioLookup.Height:
                    return (float)Math.Round((value * Measurements[BioLookup.Height].ConversionValue.Value), 1);
                case BioLookup.Waist:
                    return (float)Math.Round((value * Measurements[BioLookup.Waist].ConversionValue.Value), 1);
                    //case "Calories":
                    //    return (float)(value.Value * 4.1868);
            }
            return value;
        }

        public static int[] AlbertaUsers = { 67352, 67704, 67905, 68466, 68738, 68644, 68819, 68927, 68900, 69375, 68711, 69332, 69436, 68631, 69302, 69371, 69793, 69969, 70118 };
    }
}
