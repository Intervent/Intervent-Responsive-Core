using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Intervent.Web.DTO.AWV;
namespace InterventWebApp
{
    public class AWVReportModel
    {
        public UserDto User { get; set; }

        public string Token { get; set; }
        public AnnulWellnessVisitGeneralInfoDto AWV { get; set; }

        public int AWVId { get; set; }
        public string DrComments { get; set; }
        public IList<ScoreCardItem> PHistoryItems { get; set; }
        public IList<ScoreCardItem> FHistoryItems { get; set; }
        public IList<ScoreCardItem> PsychoSocialItems { get; set; }
        public IList<ScoreCardItem> LFactorsItems { get; set; }
        public IList<ScoreCardItem> FAbilityItems { get; set; }
        public IList<ScoreCardItem> CMItems { get; set; }
        public IList<ScoreCardItem> PEItems { get; set; }


        public IList<ActionStep> LifetimeActionSteps { get; set; }
        public IList<ActionStep> GapActionSteps { get; set; }
        public IList<ActionStep> OtherActionSteps { get; set; }

        public List<MeasurementsandGoals> ClinicalMeasurement { get; set; }
        public IList<AWVReportGoals> LifeStyleGoals { get; set; }
        public IList<AWVReportGoals> OtherGoals { get; set; }
        public IList<string> PersonalSafetyGoals { get; set; }

        public IList<AWVReportPlan> Immunizations { get; set; }
        public IList<AWVReportPlan> PreventiveServices { get; set; }

        public IList<string> DrReferrals { get; set; }
        public IList<HealthReferrals> HealthReferrals { get; set; }

        public string FastingNotion = "";
        double? BMI = null;
        public AWVReportModel(GoalsDto goal)
        {
            User = goal.User;
            AWV = goal.AWV;
            AWVId = goal.Id;
            DrComments = goal.DrComments;

            if (goal.Biometrics != null && !goal.Biometrics.Fasting.HasValue)
                FastingNotion = "^";

            #region Personal
            PHistoryItems = new List<ScoreCardItem>();
            AddScoreCard(goal.Cardiovascular, PHistoryItems, "Cardiovascular disease");
            AddScoreCard(goal.HighBP, PHistoryItems, "High blood pressure");
            AddScoreCard(goal.CholTrig, PHistoryItems, "Abnormal cholesterol or triglycerides");
            AddScoreCard(goal.Diabetes, PHistoryItems, "Diabetes");
            AddScoreCard(goal.Cancer, PHistoryItems, "Cancer");
            AddScoreCard(goal.AsthmaCOPD, PHistoryItems, "Asthma/COPD");
            AddScoreCard(goal.OtherChronic, PHistoryItems, "Other Chronic Condition(s)");
            #endregion

            #region Family
            FHistoryItems = new List<ScoreCardItem>();
            AddScoreCardItem(goal.FHCardiovascular, FHistoryItems, "Cardiovascular disease at an early age");
            AddScoreCardItem(goal.FHDiabetes, FHistoryItems, "Diabetes");
            AddScoreCardItem(goal.FHCancer, FHistoryItems, "Cancer");
            #endregion

            #region PsychoSocial
            PsychoSocialItems = new List<ScoreCardItem>();
            AddScoreCardItem(goal.Depression, PsychoSocialItems, "Depression");
            AddScoreCardItem(goal.LifeSatisfication, PsychoSocialItems, "Life satisfaction");
            AddScoreCardItem(goal.AngerRisk, PsychoSocialItems, "Anger");
            AddScoreCardItem(goal.LonelyRisk, PsychoSocialItems, "Loneliness/Social isolation");
            AddScoreCardItem(goal.BodilyPain, PsychoSocialItems, "Bodily pain");
            AddScoreCardItem(goal.Fatigue, PsychoSocialItems, "Fatigue");
            AddScoreCardItem(goal.Cognition, PsychoSocialItems, "Cognition");
            #endregion

            #region Lifestyle
            LFactorsItems = new List<ScoreCardItem>();
            AddScoreCardItem(goal.TobaccoUse, LFactorsItems, "Tobacco use");
            AddScoreCardItem(goal.PhysicalActivity, LFactorsItems, "Physical activity");
            AddScoreCardItem(goal.Nutrition, LFactorsItems, "Nutrition");
            AddScoreCardItem(goal.Alcohol, LFactorsItems, "Alcohol");
            AddScoreCardItem(goal.Stress, LFactorsItems, "Stress");
            AddScoreCardItem(goal.Safety, LFactorsItems, "Personal safety");
            AddScoreCardItem(goal.OralHealth, LFactorsItems, "Oral health");
            AddScoreCardItem(goal.SexHealth, LFactorsItems, "Sexual health");
            #endregion

            #region Functional Ability
            FAbilityItems = new List<ScoreCardItem>();
            AddScoreCardItem(goal.DailyActivity, FAbilityItems, "Activities of Daily Living");
            AddScoreCardItem(goal.Falls, FAbilityItems, "Falls");
            AddScoreCardItem(goal.HearingImp, FAbilityItems, "Hearing impairment");
            AddScoreCardItem(goal.VisionImp, FAbilityItems, "Vision impairment");
            AddScoreCardItem(goal.UrinaryInc, FAbilityItems, "Urinary incontinence");
            if (User.Gender.HasValue(1))
                AddScoreCardItem(goal.Prostate, FAbilityItems, "Prostate symptoms");
            #endregion

            #region Clinical Measurement
            CMItems = new List<ScoreCardItem>();
            AddScoreCardItem(goal.Weight, CMItems, "Weight");
            AddScoreCardItem(goal.BP, CMItems, "Blood pressure");
            AddScoreCardItem(goal.Cholesterol, CMItems, "Cholesterol/triglycerides");
            AddScoreCardItem(goal.GlucA1C, CMItems, "Glucose/A1C");
            #endregion

            #region Preventive Screening
            PEItems = new List<ScoreCardItem>();
            AddScordCardForScreening(goal);
            AddScordCardForImmunization(goal);
            #endregion

            //Action Steps
            AddLifeStyleActionSteps(goal.Lifestyle);
            AddPreventiveCareActionSteps(goal.PreventiveCare);
            AddOthereActionSteps(goal.Other);
            //Goals
            AddClinicalMeaseurement(goal);
            AddLifeStyleGoals(goal);
            AddOtherGoals(goal);
            AddSafetyGoals(goal.SafetyGoals);

            AddPreventiveScreening(goal);
            AddImmunizations(goal);

            AddDrReferral(goal.DrReferralCode);

            AddHealthReferral(goal);
        }

        #region Clinical Measeurement
        private void AddMeaseurement(string name, string goal, string value, string unit, IList<MeasurementsandGoals> list)
        {
            MeasurementsandGoals measurement = new MeasurementsandGoals();
            measurement.Name = name;
            measurement.Unit = unit;
            measurement.Measurement = value;
            measurement.Goal = goal;
            list.Add(measurement);
        }

        private void AddClinicalMeaseurement(GoalsDto goal)
        {
            ClinicalMeasurement = new List<MeasurementsandGoals>();
            var age = ExtensionUtility.GetAge(goal.User.DOB.Value, goal.AWV.AssessmentDate);
            if (goal.Biometrics != null)
            {
                var L1527 = "Less than or equal to";
                var L1528 = "Less than";
                string mValue, recommend, unit;
                var biometric = goal.Biometrics;
                mValue = recommend = unit = string.Empty;
                //Weight
                if (goal.LtWt.HasValue)
                    recommend = L1527 + " " + Convert.ToString(Math.Round(goal.LtWt.Value, 1));
                else
                    recommend = "BMI less than 25";
                if (biometric.Weight > 0)
                {
                    mValue = Convert.ToString(Math.Round(biometric.Weight.Value, 1));
                    //unit = "lbs";
                }
                else
                    mValue = "?";
                AddMeaseurement("Weight (pounds)", recommend, mValue, unit, ClinicalMeasurement);
                //Height
                mValue = recommend = unit = string.Empty;
                if (biometric.Height > 0)
                {
                    var height = CommonUtility.ToFeetInches(biometric.Height.Value);
                    mValue = Math.Round(biometric.Height.Value, 1) + " (" + height.Key + "' " + Math.Round(height.Value, 1) + "'')";
                }
                else
                    mValue = "?";
                AddMeaseurement("Height (inches)", "N/A", mValue, unit, ClinicalMeasurement);
                //Waist
                mValue = recommend = unit = string.Empty;
                if (goal.User.Gender == 1)
                    recommend = L1528 + " 40";
                else
                    recommend = L1528 + " 35";

                if (biometric.Waist > 0)
                {
                    mValue = Convert.ToString(Math.Round(biometric.Waist.Value, 1));
                }
                else
                    mValue = "?";
                AddMeaseurement("Waist measurement (inches)", recommend, mValue, unit, ClinicalMeasurement);
                //BMI
                mValue = recommend = unit = string.Empty;
                recommend = L1528 + " 25";

                if (biometric.Height.HasValue && biometric.Weight.HasValue)
                {
                    BMI = CommonUtility.GetBMI(biometric.Height.Value, biometric.Weight.Value);
                    mValue = Convert.ToString(BMI);
                }
                else
                    mValue = "?";
                AddMeaseurement("Body mass index (BMI;kg/m2)", recommend, mValue, unit, ClinicalMeasurement);
                //SBP
                mValue = recommend = unit = string.Empty;
                recommend = L1528 + " " + Convert.ToString(goal.LtSBP);
                if (biometric.SBP.HasValue)
                    mValue = Convert.ToString(biometric.SBP.Value);
                else
                    mValue = "?";
                AddMeaseurement("Systolic blood pressure (mmHg)", recommend, mValue, unit, ClinicalMeasurement);
                //DBP
                mValue = recommend = unit = string.Empty;
                recommend = L1528 + " " + Convert.ToString(goal.LtDBP);
                if (biometric.DBP > 0)
                    mValue = Convert.ToString(biometric.DBP);
                else
                    mValue = "?";
                AddMeaseurement("Diastolic blood pressure (mmHg)", recommend, mValue, unit, ClinicalMeasurement);
                //Cholesterol
                mValue = recommend = unit = string.Empty;

                recommend = L1528 + " 200";
                if (biometric.TotalChol.HasValue)
                    mValue = Convert.ToString(Math.Round(biometric.TotalChol.Value, 0));
                else
                    mValue = "?";
                AddMeaseurement("Total cholesterol (mg/dl)*", recommend, mValue, unit, ClinicalMeasurement);
                //Trig
                mValue = recommend = unit = string.Empty;
                recommend = "Less than 150";
                if (biometric.Trig.HasValue && biometric.Fasting.IsNullOrTrue())
                    mValue = Convert.ToString(Math.Round(biometric.Trig.Value, 0));
                else
                    mValue = "?";
                AddMeaseurement("Triglycerides, fasting (mg/dl)*" + FastingNotion, recommend, mValue, unit, ClinicalMeasurement);
                //Ldl
                mValue = recommend = unit = string.Empty;
                if (biometric.LDL.HasValue && biometric.Fasting.IsNullOrTrue())
                    mValue = Convert.ToString(Math.Round(biometric.LDL.Value, 0));
                else
                    mValue = "?";
                AddMeaseurement("LDL cholesterol, fasting (mg/dl)*" + FastingNotion, "Less than 100", mValue, unit, ClinicalMeasurement);
                //Hdl
                mValue = recommend = unit = string.Empty;
                if (goal.User.Gender.HasValue(1))
                    recommend = "Greater than or equal to 40";
                else
                    recommend = "Greater than or equal to 50";
                if (biometric.HDL.HasValue)
                    mValue = Convert.ToString(Math.Round(biometric.HDL.Value, 0));
                else
                    mValue = "?";
                AddMeaseurement("HDL cholesterol (mg/dl)*", recommend, mValue, unit, ClinicalMeasurement);
                //Glucose
                mValue = recommend = unit = string.Empty;
                if ((goal.LtGluc1.HasValue && goal.LtGluc2.HasValue) && (goal.LtGluc1 != goal.LtGluc2))
                    recommend = Convert.ToString(goal.LtGluc1) + "-" + Convert.ToString(goal.LtGluc2);
                else
                    recommend = "Less than " + Convert.ToString(goal.LtGluc1);
                if (biometric.Glucose.HasValue && biometric.Fasting.IsNullOrTrue())
                    mValue = Convert.ToString(Math.Round(biometric.Glucose.Value, 0));
                else
                    mValue = "?";
                AddMeaseurement("Glucose, fasting (mg/dl)", recommend, mValue, unit, ClinicalMeasurement);
                //A1C-->
                mValue = recommend = unit = string.Empty;
                if (goal.Diabetes.HasValue((byte)Risk.HR) || (biometric.A1C.HasValue && biometric.A1C >= (float)6.5 && !biometric.Glucose.HasValue))
                    recommend = L1528 + " 7.0";
                else
                    recommend = L1528 + " 5.7";
                if (biometric.A1C.HasValue)
                    mValue = string.Format("{0:#.0}", Convert.ToDecimal(biometric.A1C.Value));
                else
                    mValue = "?";
                AddMeaseurement("A1C (%)", recommend, mValue, unit, ClinicalMeasurement);
                //CHD
                mValue = recommend = unit = string.Empty;

                if (goal.ASCVD == true)
                    recommend = "N/A";
                else
                {
                    if (goal.TenYrLow.HasValue)
                    {
                        if (goal.TenYrLow.Value != 1)
                            recommend = L1527 + " " + Convert.ToString(Math.Round(goal.TenYrLow.Value * 100, 0));
                        else
                            recommend = L1527 + " " + Convert.ToString(Math.Round(goal.TenYrLow.Value, 0));
                    }
                    else
                    {
                        recommend = "N/A";
                    }
                }
                if (goal.ASCVD == true)
                    mValue = "N/A";
                else
                {
                    if (goal.TenYrProb > 0)
                        mValue = Convert.ToString(goal.TenYrProb * 100);
                    else if (recommend == "N/A")
                        mValue = "N/A";
                    else
                        mValue = "?";
                }
                AddMeaseurement("Framingham 10-year CHD risk score (%)", recommend, mValue, unit, ClinicalMeasurement);
                //10 Year ASCVD
                mValue = recommend = unit = string.Empty;
                if (goal.ASCVD == true)
                    recommend = "N/A";
                else
                {
                    if (!goal.TenYearASCVDGoal.HasValue)
                    {
                        if (age >= 40 && age <= 79 && !goal.User.Race.HasValue)
                            recommend = "?";
                        else
                            recommend = "N/A";
                    }
                    else
                        recommend = Translate.Message("L1527") + " " + goal.TenYearASCVDGoal.ToString();
                }
                if (goal.ASCVD == true)
                    mValue = "N/A";
                else
                {
                    if (!goal.TenYearASCVD.HasValue)
                    {
                        if (recommend == "N/A")
                            mValue = "N/A";
                        else
                            mValue = "?";
                    }
                    else
                        mValue = goal.TenYearASCVD.ToString();
                }
                AddMeaseurement("10-year ASCVD risk score (%)", recommend, mValue, unit, ClinicalMeasurement);
                //Lifetime ASCVD
                mValue = recommend = unit = string.Empty;
                if (goal.ASCVD == true)
                    recommend = "N/A";
                else
                {
                    if (!goal.LifetimeASCVDGoal.HasValue)
                        recommend = "N/A";
                    else
                        recommend = L1527 + " " + goal.LifetimeASCVDGoal.ToString();
                }
                if (goal.ASCVD == true)
                    mValue = "N/A";
                else
                {
                    if (!goal.LifetimeASCVD.HasValue)
                    {
                        if (recommend == "N/A")
                            mValue = "N/A";
                        else
                            mValue = "?";
                    }
                    else
                        mValue = goal.LifetimeASCVD.ToString();
                }
                AddMeaseurement("Lifetime ASCVD risk score (%)", recommend, mValue, unit, ClinicalMeasurement);
            }
        }
        #endregion

        #region HealthReferral
        private void AddHealthReferral(GoalsDto goal)
        {
            HealthReferrals = new List<HealthReferrals>();
            if (goal.CardioSmart.IsTrue())
                HealthReferrals.Add(new HealthReferrals("Participate in the American College of Cardiology’s <a class='link green' href='../../pdf/CardioSmartOnCallFlyer_Participant.pdf' target ='_blank'>CardioSmart OnCall</a> comprehensive telephone lifestyle health coaching program provided in collaboration with its partner, INTERVENT (please call INTERVENT at 1-855-494-1093 for more information)."));
            if (goal.DiabeticMgnt.IsTrue())
                HealthReferrals.Add(new HealthReferrals("Schedule appointment for diabetes self-management training."));
            if (goal.NutritionalMgnt.IsTrue())
                HealthReferrals.Add(new HealthReferrals("Schedule appointment for nutritional/dietary counseling."));
            if (goal.WeightMgnt.IsTrue())
                HealthReferrals.Add(new HealthReferrals("Schedule appointment for weight management/counseling."));
            if (goal.TobaccoCes.IsTrue())
                HealthReferrals.Add(new HealthReferrals("Schedule appointment for assistance with tobacco cessation."));

            var hr = (byte)Risk.HR;
            var id = (byte)Risk.ID;
            if (goal.Depression.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on mental health.", "AWV - Depression.pdf"));
            if (goal.LonelyRisk.HasValue(hr) || (goal.Depression.HasValue(hr) && goal.LonelyRisk.HasValue(id)))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on loneliness/social isolation.", "AWV - Loneliness - Isolation.pdf"));
            if (goal.Cognition.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on cognition/brain health.", "AWV - Cognition.pdf"));
            if (goal.TobaccoUse.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on tobacco.", "AWV - Tobacco.pdf"));
            if (goal.PhysicalActivity.HasValue(hr) || goal.Weight.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on physical activity.", "AWV - Physical Activity.pdf"));
            if (goal.Nutrition.HasValue(hr) || goal.Weight.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on nutrition.", "AWV - Nutrition.pdf"));
            if (goal.Alcohol.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on alcohol.", "AWV -  Alcohol.pdf"));
            if (goal.Stress.HasValue(hr) || (goal.Stress.HasValue(id) && goal.Depression.HasValue(hr)))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on stress.", "AWV - Stress.pdf"));
            if (goal.Safety.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on personal safety.", "AWV - Personal Safety.pdf"));
            if (goal.OralHealth.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on oral health.", "AWV - Oral Health.pdf"));
            if (goal.SexHealth.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on sexual health.", "AWV - Sexual Health.pdf"));
            if (goal.Falls.HasValue(hr))
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on falls.", "AWV - Falls.pdf"));
            if (BMI.HasValue && BMI.Value > 25)
                HealthReferrals.Add(new HealthReferrals("Review health and wellness content on weight management.", "AWV - Weight Management.pdf"));
        }
        #endregion

        #region Immunizations/Preventive Screening

        private void AddPlan(byte? value, string displayText, string recValue, IList<AWVReportPlan> planList, string name)
        {
            AWVReportPlan item = new AWVReportPlan();
            item.Name = name;
            item.Recommended = displayText;
            if (value.HasValue((byte)Track.NotOnTrack))
            {
                item.ClassName = "no track";
                item.StatusMessage = "Not On Track";
            }
            else if (value.HasValue((byte)Track.OnTrack))
            {
                item.ClassName = "on track";
                item.StatusMessage = "On Track";
            }
            else
            {
                item.ClassName = "track";
                if (value.HasValue((byte)Track.Optional))
                    item.StatusMessage = "Optional";
                else if (value.HasValue((byte)Track.Indeterminate))
                    item.StatusMessage = "Indeterminate";
                else
                    item.StatusMessage = "Not Applicable";
            }
            if (!string.IsNullOrEmpty(recValue))
            {
                var recList = recValue.Split('~');
                string nextDue = "";
                if (recList.Count() == 3)
                {
                    if (!string.IsNullOrEmpty(recList[0]))
                        item.LastReceived = recList[0];
                    if (recList[1] == "C")
                    {
                        if (recList[2] == Intervent.Web.DTO.AWV.Constants.Now)
                        {
                            item.NextDue = "Colonoscopy recommended now.";
                        }
                        else
                            item.NextDue = recList[2];
                    }
                    else if (recList[1] == "H")
                    {
                        if (recList[2] == Intervent.Web.DTO.AWV.Constants.Now)
                        {
                            item.NextDue = "Fecal occult blood test recommended now.";
                        }
                        else
                            item.NextDue = recList[2];
                    }
                }
                else
                {
                    if (recList.Count() == 2)
                    {
                        item.LastReceived = recList[0];
                        nextDue = recList[1];
                    }
                    else
                        nextDue = recList[0];
                    if (!string.IsNullOrEmpty(nextDue))
                    {
                        DateTime nextDueDate;
                        if (nextDue == Intervent.Web.DTO.AWV.Constants.Now)
                            item.NextDue = "Recommended now.";
                        else if (nextDue == Intervent.Web.DTO.AWV.Constants.Physician)
                            item.NextDue = "As recommended by physician.";
                        else if (nextDue == Intervent.Web.DTO.AWV.Constants.NotNeeded)
                            item.NextDue = "Not needed.";
                        else if (nextDue == Intervent.Web.DTO.AWV.Constants.YearlyFlu)
                            item.NextDue = "Repeat annually in preparation for flu season";
                        else if (nextDue == Intervent.Web.DTO.AWV.Constants.NotApplicable)
                            item.NextDue = "Not Applicable.";
                        else if (nextDue == Intervent.Web.DTO.AWV.Constants.NextPhysician)
                            item.NextDue = "At next physician office visit";
                        else if (DateTime.TryParse(nextDue, out nextDueDate) && nextDueDate <= DateTime.UtcNow)
                        {
                            item.NextDue = "Recommended now.";
                        }
                        else
                            item.NextDue = nextDue;
                    }
                }
            }
            else
                item.NextDue = "Not Applicable";
            planList.Add(item);
        }
        private void AddImmunizations(GoalsDto goal)
        {
            Immunizations = new List<AWVReportPlan>();
            string text = "Adults who have not received tetanus, diphtheria, and acellular pertussis (Tdap) vaccine or for whom vaccine status is unknown should receive a dose of Tdap followed by tetanus and diphtheria toxoids (Td) booster doses every 10 years thereafter.";
            AddPlan(goal.Tetanus, text, goal.TetanusRecDate, Immunizations, "Tetanus, Diphtheria, and Pertussis");
            text = "Depending on health condition, adults 65 or older should generally receive a dose of PCV13 vaccine followed by PPSV23 vaccine at least 1 year later.";
            AddPlan(goal.Pneumonia, text, goal.PneumoniaRecDate, Immunizations, "Pneumonia");
            AddPlan(goal.Flu, "Annually in preparation for influenza season. That is, every year by the end of October, if possible. However, getting vaccinated later is still beneficial; vaccination can continue throughout the flu season, even in January or later if needed.", goal.FluRecDate, Immunizations, "Flu");
            AddPlan(goal.Shingles, "Once at age 60 or older (even if you have had shingles before).", goal.ShinglesRecDate, Immunizations, "Shingles");
            AddPlan(goal.HepatitisB, "Adults at intermediate or high risk for Hepatitis B.", goal.HepatitisBRecDate, Immunizations, "Hepatitis B");
        }

        private void AddPreventiveScreening(GoalsDto goal)
        {
            PreventiveServices = new List<AWVReportPlan>();
            string text = "Adults age 50-85. Intervals for recommended screening strategies: <ul><li>Annual screening with high-sensitivity fecal occult blood testing</li><li>Sigmoidoscopy every 5 years, with high-sensitivity fecal occult blood testing every 3 years (or more frequently if high risk) </li><li>Screening colonoscopy every 10 years (or more frequently if high risk)</li></ul>";
            AddPlan(goal.Colorectal, text, goal.ColorectalRecDate, PreventiveServices, "Colorectal Cancer Screening");
            if (User.Gender.HasValue(2))
            {
                text = "Every 3 years for women age 21-65 (or more frequently if high risk). Not recommended for women: 1. After hysterectomy with removal of the cervix and with no history of high-grade precancer or cervical cancer;  and 2. Older than age 65 who have had adequate prior screening and are not high risk";
                AddPlan(goal.PapTest, text, goal.PapTestRecDate, PreventiveServices, "Pap Test/Screening for Cervical Cancer");
                AddPlan(goal.Mammogram, "Every 1-2 years after age 40", goal.MammogramRecDate, PreventiveServices, "Mammogram Breast Cancer Screening");
            }
            else
            {
                AddPlan(goal.PSAProstate, "Optional after age 50", goal.PSAProstateRecDate, PreventiveServices, "PSA Prostate Cancer Screening");
            }

            AddPlan(goal.BPScreening, "At least annually (more frequently for people who have values close to those warranting therapy or already on therapy)", goal.BPScreeningRecDate, PreventiveServices, "Blood Pressure");
            AddPlan(goal.Lipoproteins, "Every 5 years (more frequently for people who have lipid levels close to those warranting therapy or already on therapy)", goal.LipoproteinsRecDate, PreventiveServices, "Fasting Serum Lipids and Lipoproteins");
            text = "For those without prediabetes or diabetes who are overweight or obese and aged 40 to 70, plasma glucose or A1C test every 3 years. More frequently for those with prediabetes (at least annually) or diabetes (A1C at least every 3-6 months).";
            AddPlan(goal.DiabetesScreening, text, goal.DiabetesScreeningRecDate, PreventiveServices, "Diabetes Screening");
            AddPlan(goal.Obesity, "At least annually", goal.ObesityRecDate, PreventiveServices, "Body Mass Index Screening for Overweight / Obesity");
            text = "Annually for Medicare beneficiaries who:1. Have diabetes; 2. Have a family history of glaucoma; 3. Are African American and 50 or older; or 3. Are Hispanic and 65 or older";
            AddPlan(goal.Vision, text, goal.VisionRecDate, PreventiveServices, "Glaucoma screening");
            AddPlan(goal.Hearing, "Optional after age 50", goal.HearingRecDate, PreventiveServices, "Hearing Screening");
            if (User.Gender.HasValue(1))
                AddPlan(goal.Aortic, "Men ages 65 to 75 who have ever smoked at least 100 cigarettes in their lifetime", goal.AorticRecDate, PreventiveServices, "Abdominal Aortic Aneurysm Screening");
            text = "Women age 65 or older; women age under age 65 whose 10-year fracture risk is equal to or greater than that of a 65-year-old white woman without additional risk factors; men and women as determined by their physician (estrogen deficient women; vertebral abnormalities; glucocorticoid therapy; primary hyperparathyroidism; or on approved osteoporosis drug therapy).";
            AddPlan(goal.Osteoporosis, text, goal.OsteoporosisRecDate, PreventiveServices, "Bone Mass Measurement (Osteoporosis Screening)");
            text = "Annually in asymptomatic (no signs or symptoms of lung cancer) adults aged 55 to 80 who have a 30 pack-year smoking history and currently smoke or have quit smoking within the past 15 years. (one pack-year = smoking one pack per day for one year; 1 pack = 20 cigarettes).";
            AddPlan(goal.LungCancer, text, goal.LungCancerRecDate, PreventiveServices, "Lung Cancer Screening");
            AddPlan(goal.HIV, "Annually if at increased risk", goal.HIVRecDate, PreventiveServices, "HIV Screening");
        }
        #endregion

        #region Goals

        private void AddGoals(IList<AWVReportGoals> goalList, byte? value, string name, string recommendataion)
        {
            AWVReportGoals item = new AWVReportGoals();
            item.Name = name;
            item.Recommended = recommendataion;
            if (value.HasValue((byte)Risk.LR))
            {
                item.ClassName = "on track";
                item.StatusMessage = "Lower Risk";
            }
            else if (value.HasValue((byte)Risk.HR))
            {
                item.ClassName = "no track";
                item.StatusMessage = "Higher Risk";
            }
            else
            {
                item.ClassName = "track";
                item.StatusMessage = "Incomplete Data";
            }
            goalList.Add(item);
        }

        private void AddSafetyGoals(string safetyGoal)
        {
            PersonalSafetyGoals = new List<string>();
            if (!string.IsNullOrEmpty(safetyGoal))
            {
                var goalsList = safetyGoal.Split('~');
                foreach (var sGoal in goalsList)
                {
                    switch (sGoal)
                    {
                        case "GS":
                            PersonalSafetyGoals.Add("Having loose carpets, poor lighting, lack of handrails or grab bars in your home.");
                            break;
                        case "HT":
                            PersonalSafetyGoals.Add("Having throwrugs on hardwood floors in your house.");
                            break;
                        case "HS":
                            PersonalSafetyGoals.Add("Not having smoke alarms and carbon monoxide detectors in good working order in your house.");
                            break;
                        case "HB":
                            PersonalSafetyGoals.Add("Bathtub not containing a safety measure such as a rubber mat or strips.");
                            break;
                        case "HBM":
                            PersonalSafetyGoals.Add("Not having the area in front of your bathtub carpeted or protected by a bathmat with rubber backing.");
                            break;
                        case "HN":
                            PersonalSafetyGoals.Add("Not having night lights in your house.");
                            break;
                        case "HL":
                            PersonalSafetyGoals.Add("Having loose or frayed cords or overloaded electrical sockets in your house.");
                            break;
                        case "HU":
                            PersonalSafetyGoals.Add("Neglecting to unplug household appliances when not in use.");
                            break;
                        case "HM":
                            PersonalSafetyGoals.Add("Not keeping medicines in a safe place and having their directions clearly labeled.");
                            break;
                        case "HK":
                            PersonalSafetyGoals.Add("Not keeping knives and other sharp objects put away in a safe place.");
                            break;
                        case "HC":
                            PersonalSafetyGoals.Add("Not keeping poisons, chemicals or other toxic substances put away in a safe place.");
                            break;
                        case "HF":
                            PersonalSafetyGoals.Add("Having furniture with sharp corners, or a rickety chair, that could cause injury.");
                            break;
                    }
                }
            }
        }

        private void AddLifeStyleGoals(GoalsDto goal)
        {
            LifeStyleGoals = new List<AWVReportGoals>();
            AddGoals(LifeStyleGoals, goal.TobaccoUse, "Tobacco use", "Avoid all tobacco (including e-cigarettes) and second-hand smoke");
            AddGoals(LifeStyleGoals, goal.PhysicalActivity, "Physical activity", "Perform regular moderate-intensity aerobic physical activity (at least 150 minutes each week)");
            AddGoals(LifeStyleGoals, goal.Nutrition, "Nutrition", "Follow a healthy eating pattern (include: fruits, vegetables, whole grains, low-fat dairy, lean proteins and healthy oils; limit: saturated/trans fats, added sugars and sodium).");
            if (User.Gender.HasValue(1))
                AddGoals(LifeStyleGoals, goal.Alcohol, "Alcohol", "If you drink, no more than two per day");
            else
                AddGoals(LifeStyleGoals, goal.Alcohol, "Alcohol", "If you drink, no more than one per day");
            AddGoals(LifeStyleGoals, goal.Stress, "Stress", "Reduce and/or manage stress effectively on an ongoing basis");
            AddGoals(LifeStyleGoals, goal.Safety, "Personal safety ", "Eliminate all safety risks (see listing below)");
            AddGoals(LifeStyleGoals, goal.OralHealth, "Oral health", "Adhere to appropriate oral hygiene practices");
            AddGoals(LifeStyleGoals, goal.SexHealth, "Sexual health", "If sexually active, take appropriate safety precautions ");
        }

        private void AddOtherGoals(GoalsDto goal)
        {
            OtherGoals = new List<AWVReportGoals>();
            AddGoals(OtherGoals, goal.Depression, "Depression", "Adhere to appropriate mental health practices");
            AddGoals(OtherGoals, goal.LonelyRisk, "Loneliness/Social isolation", "Remain socially active as you age");
            AddGoals(OtherGoals, goal.Cognition, "Cognition", "Adhere to appropriate memory/brain health practices");
            AddGoals(OtherGoals, goal.Falls, "Falls", "Eliminate all safety risks (see listing below)");
        }
        #endregion

        #region Key Action Steps
        private void AddActionStep(string value, IList<ActionStep> actionSteps)
        {
            ActionStep item = new ActionStep();
            item.name = value;
            actionSteps.Add(item);
        }

        public void AddLifeStyleActionSteps(string value)
        {
            LifetimeActionSteps = new List<ActionStep>();
            if (!string.IsNullOrEmpty(value))
            {
                var options = value.Split('~');
                //
                foreach (var opt in options)
                {
                    switch (opt)
                    {
                        case "T1":
                            AddActionStep(Translate.Message("L1947"), LifetimeActionSteps);
                            break;
                        case "T2":
                            AddActionStep(Translate.Message("L1948"), LifetimeActionSteps);
                            break;
                        case "N1":
                            AddActionStep(Translate.Message("L1949"), LifetimeActionSteps);
                            break;
                        case "N2":
                            AddActionStep(Translate.Message("L1950"), LifetimeActionSteps);
                            break;
                        case "N3":
                            AddActionStep(Translate.Message("L1386"), LifetimeActionSteps);
                            break;
                        case "P1":
                            AddActionStep(Translate.Message("L1387"), LifetimeActionSteps);
                            break;
                        case "S1":
                            AddActionStep(Translate.Message("L1388"), LifetimeActionSteps);
                            break;
                        case "B1":
                            AddActionStep(Translate.Message("L1389"), LifetimeActionSteps);
                            break;
                        case "B2":
                            AddActionStep(Translate.Message("L1390"), LifetimeActionSteps);
                            break;
                        case "SF1":
                            AddActionStep(Translate.Message("L1391"), LifetimeActionSteps);
                            break;
                        case "A1":
                            AddActionStep(Translate.Message("L1392"), LifetimeActionSteps);
                            break;
                    }
                }
            }
        }

        public void AddOthereActionSteps(string value)
        {

            OtherActionSteps = new List<ActionStep>();
            if (!string.IsNullOrEmpty(value))
            {
                var options = value.Split('~');
                //
                foreach (var opt in options)
                {
                    switch (opt)
                    {
                        case "M":
                            AddActionStep(Translate.Message("L1393"), OtherActionSteps);
                            break;
                        case "HO":
                            AddActionStep(Translate.Message("L1394"), OtherActionSteps);
                            break;
                        case "A":
                            AddActionStep(Translate.Message("L1395"), OtherActionSteps);
                            break;
                        case "F":
                            AddActionStep(Translate.Message("L1396"), OtherActionSteps);
                            break;
                        case "HE":
                            AddActionStep(Translate.Message("L1397"), OtherActionSteps);
                            break;
                        case "U":
                            AddActionStep(Translate.Message("L1398"), OtherActionSteps);
                            break;
                        case "D":
                            AddActionStep(Translate.Message("L1399"), OtherActionSteps);
                            break;
                        case "S":
                            AddActionStep(Translate.Message("L1951"), OtherActionSteps);
                            break;
                        case "P":
                            AddActionStep(Translate.Message("L1952"), OtherActionSteps);
                            break;
                    }
                }
            }
        }

        public void AddPreventiveCareActionSteps(string value)
        {

            GapActionSteps = new List<ActionStep>();
            if (!string.IsNullOrEmpty(value))
            {
                var options = value.Split('~');
                //
                foreach (var opt in options)
                {
                    switch (opt)
                    {
                        case "L1":
                            AddActionStep(Translate.Message("L1936"), GapActionSteps);
                            break;
                        case "L2":
                            AddActionStep(Translate.Message("L1937"), GapActionSteps);
                            break;
                        case "B1":
                            AddActionStep(Translate.Message("L1938"), GapActionSteps);
                            break;
                        case "B2":
                            AddActionStep(Translate.Message("L1939"), GapActionSteps);
                            break;
                        case "D1":
                            AddActionStep(Translate.Message("L1940"), GapActionSteps);
                            break;
                        case "D2":
                            AddActionStep(Translate.Message("L1941"), GapActionSteps);
                            break;
                        case "D3":
                            AddActionStep(Translate.Message("L1942"), GapActionSteps);
                            break;
                        case "D4":
                            AddActionStep(Translate.Message("L1943"), GapActionSteps);
                            break;
                        case "IM":
                            AddActionStep(Translate.Message("L1944"), GapActionSteps);
                            break;
                        case "V":
                            AddActionStep(Translate.Message("L1945"), GapActionSteps);
                            break;
                        case "R":
                            AddActionStep(Translate.Message("L1946"), GapActionSteps);
                            break;
                            //TODO: Referral
                    }
                }
            }
        }

        #endregion

        #region DrReferral

        private void AddDrReferral(string drReferralCode)
        {
            DrReferrals = new List<string>();
            if (!string.IsNullOrEmpty(drReferralCode))
            {
                var codes = drReferralCode.Split('~');
                foreach (var code in codes)
                {
                    switch (code)
                    {
                        case "L337":
                            DrReferrals.Add(Translate.Message("L1953"));
                            break;
                        case "L338":
                            DrReferrals.Add(Translate.Message("L1954"));
                            break;
                        case "L340":
                            DrReferrals.Add(Translate.Message("L1955"));
                            break;
                        case "L341":
                            DrReferrals.Add(Translate.Message("L1956"));
                            break;
                        case "L342":
                            DrReferrals.Add(Translate.Message("L1957"));
                            break;
                        case "L357":
                            DrReferrals.Add(Translate.Message("L1958"));
                            break;
                        case "L360":
                            DrReferrals.Add(Translate.Message("L1959"));
                            break;
                        case "L361":
                            DrReferrals.Add(Translate.Message("L1960"));
                            break;
                        case "L1334":
                            DrReferrals.Add(Translate.Message("L1961"));
                            break;
                        case "L1335":
                            DrReferrals.Add(Translate.Message("L1962"));
                            break;
                        case "L1336":
                            DrReferrals.Add(Translate.Message("L1963"));
                            break;
                        case "L1337":
                            DrReferrals.Add(Translate.Message("L1964"));
                            break;
                    }
                }
            }
        }
        #endregion

        #region Score Card
        private void AddScoreCard(byte? value, IList<ScoreCardItem> scoreCardList, string displayText)
        {
            ScoreCardItem item = new ScoreCardItem();
            item.Name = displayText;
            if (value.HasValue((byte)Risk.HR))
            {
                item.riskClass = "at-risk";
                item.riskText = "Yes";
            }
            else if (value.HasValue((byte)Risk.LR))
            {
                item.riskClass = "not-at-risk";
                item.riskText = "No";
            }
            else
            {
                item.riskClass = "cannot-calculate";
                item.riskText = Translate.Message("Incomplete Data");
            }
            scoreCardList.Add(item);
        }
        private void AddScoreCardItem(byte? value, IList<ScoreCardItem> scoreCardList, string displayText)
        {
            ScoreCardItem item = new ScoreCardItem();
            item.Name = displayText;
            if (value.HasValue((byte)Risk.HR))
            {
                item.riskClass = "at-risk";
                item.riskText = "Higher Risk";
            }
            else if (value.HasValue((byte)Risk.LR))
            {
                item.riskClass = "not-at-risk";
                item.riskText = "Lower Risk";
            }
            else
            {
                item.riskClass = "cannot-calculate";
                item.riskText = Translate.Message("Incomplete Data");
            }
            scoreCardList.Add(item);
        }

        private void AddScordCardForScreening(GoalsDto goal)
        {
            var displayitem = "Up to date with recommended preventive screenings";
            if (goal.Colorectal.HasValue((byte)Track.NotOnTrack) || goal.PapTest.HasValue((byte)Track.NotOnTrack) || goal.Mammogram.HasValue((byte)Track.NotOnTrack) ||
                goal.PSAProstate.HasValue((byte)Track.NotOnTrack) || goal.BPScreening.HasValue((byte)Track.NotOnTrack) ||
                goal.Lipoproteins.HasValue((byte)Track.NotOnTrack) || goal.DiabetesScreening.HasValue((byte)Track.NotOnTrack) ||
                goal.Obesity.HasValue((byte)Track.NotOnTrack) || goal.Vision.HasValue((byte)Track.NotOnTrack) || goal.Hearing.HasValue((byte)Track.NotOnTrack) ||
                goal.Aortic.HasValue((byte)Track.NotOnTrack) || goal.Osteoporosis.HasValue((byte)Track.NotOnTrack) || goal.LungCancer.HasValue((byte)Track.NotOnTrack) ||
                goal.HIV.HasValue((byte)Track.NotOnTrack))
            {
                ScoreCardItem item = new ScoreCardItem();
                item.Name = displayitem;
                item.riskClass = "at-risk";
                item.riskText = "Higher Risk";
                PEItems.Add(item);
            }

            else if (goal.Colorectal.HasLowRisk() && goal.PapTest.HasLowRisk() && goal.Mammogram.HasLowRisk() && goal.PSAProstate.HasLowRisk()
                && goal.BPScreening.HasLowRisk() && goal.Lipoproteins.HasLowRisk() && goal.DiabetesScreening.HasLowRisk() && goal.Obesity.HasLowRisk()
                && goal.Vision.HasLowRisk() && goal.Hearing.HasLowRisk() && goal.Aortic.HasLowRisk() && goal.Osteoporosis.HasLowRisk()
                && goal.LungCancer.HasLowRisk() && goal.HIV.HasLowRisk())
            {
                ScoreCardItem item = new ScoreCardItem();
                item.Name = displayitem;
                item.riskClass = "not-at-risk";
                item.riskText = "Lower Risk";
                PEItems.Add(item);
            }
            else
            {
                ScoreCardItem item = new ScoreCardItem();
                item.riskClass = "cannot-calculate";
                item.riskText = "Incomplete Data";
                item.Name = displayitem;
                PEItems.Add(item);
            }
        }

        private void AddScordCardForImmunization(GoalsDto goal)
        {
            var displayitem = "Up to date with recommended vaccinations";
            if (goal.Tetanus.HasValue((byte)Track.NotOnTrack) || goal.Pneumonia.HasValue((byte)Track.NotOnTrack) ||
                goal.Flu.HasValue((byte)Track.NotOnTrack) || goal.Shingles.HasValue((byte)Track.NotOnTrack) || goal.HepatitisB.HasValue((byte)Track.NotOnTrack))
            {
                ScoreCardItem item = new ScoreCardItem();
                item.Name = displayitem;
                item.riskClass = "at-risk";
                item.riskText = "Higher Risk";
                PEItems.Add(item);
            }

            else if (goal.Tetanus.HasLowRisk() && goal.Pneumonia.HasLowRisk() && goal.Flu.HasLowRisk() && goal.Shingles.HasLowRisk() && goal.HepatitisB.HasLowRisk())
            {
                ScoreCardItem item = new ScoreCardItem();
                item.Name = displayitem;
                item.riskClass = "not-at-risk";
                item.riskText = "Lower Risk";
                PEItems.Add(item);
            }
            else
            {
                ScoreCardItem item = new ScoreCardItem();
                item.riskClass = "cannot-calculate";
                item.riskText = "Incomplete Data";
                item.Name = displayitem;
                PEItems.Add(item);
            }
        }

        private void AddScoreCardItemForImmunization(byte? value, IList<ScoreCardItem> scoreCardList, string displayText)
        {
            ScoreCardItem item = new ScoreCardItem();
            item.Name = displayText;
            if (value.HasValue((byte)Track.NotOnTrack))
            {
                item.riskClass = "at-risk";
                item.riskText = "Higher Risk";
            }
            else if (value.ContainsValue(new List<byte> { (byte)Track.OnTrack, (byte)Track.Optional, (byte)Track.NotApplicable }))
            {
                item.riskClass = "not-at-risk";
                item.riskText = "Lower Risk";
            }
            else
            {
                item.riskClass = "cannot-calculate";
                item.riskText = Translate.Message("Incomplete Data");
            }
            scoreCardList.Add(item);
        }

        #endregion
    }

    public static class ExtenstionModelUtility
    {

        public static bool HasLowRisk(this byte? value)
        {
            if (value.HasValue && value.ContainsValue(new List<byte> { (byte)Track.OnTrack, (byte)Track.Optional, (byte)Track.NotApplicable }))
                return true;

            return false;
        }
    }

}