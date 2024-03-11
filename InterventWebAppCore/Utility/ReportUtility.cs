using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public static class ReportUtility
    {
        #region HRA Report

        public static ReadHRAResponse ReadHRAReport(int hraId)
        {
            HRAReader reader = new HRAReader();
            ReadHRARequest request = new ReadHRARequest();
            request.hraId = hraId;
            var response = reader.ReadHRA(request);
            return response;
        }

        public static CarePlanReportResponse GetCarePlanReport(int type, string refId)
        {
            ReportReader reader = new ReportReader();
            CarePlanReportRequest request = new CarePlanReportRequest();
            request.type = type;
            request.refId = refId;
            return reader.GetCarePlanReport(request);
        }

        public static AddEditCarePlanReportsResponse AddEditCarePlanReport(int type, string refId, bool reportGenerated, int userId)
        {
            ReportReader reportReader = new ReportReader();
            AccountReader accountReader = new AccountReader();
            AddEditCarePlanReportsRequest request = new AddEditCarePlanReportsRequest();
            CarePlanReportDto carePlanReport = new CarePlanReportDto();
            carePlanReport.Type = type;
            carePlanReport.RefId = refId;
            carePlanReport.ReportGenerated = reportGenerated;
            carePlanReport.CreatedBy = userId;
            carePlanReport.CreatedOn = DateTime.UtcNow;
            request.carePlanReport = carePlanReport;
            var response = reportReader.AddEditCarePlanReport(request);
            if (response.status)
            {
                var user = accountReader.GetUserById(userId);
                response.userName = user.FirstName + " " + user.LastName;
            }
            return response;
        }

        public static ReadHRAGoalsResponse ReadHRAGoals(int hraId)
        {
            ReportReader reader = new ReportReader();
            ReadHRAGoalsRequest request = new ReadHRAGoalsRequest();
            request.hraId = hraId;
            var response = reader.ReadHRAGoals(request);
            return response;
        }

        public static ListKeyActionStepsResponse KeyActionSteps(int hraId, bool showSelfScheduling)
        {
            ReportReader reader = new ReportReader();
            ListKeyActionStepsRequest request = new ListKeyActionStepsRequest();
            request.hraId = hraId;
            var response = reader.KeyActionSteps(request);
            //add coahing program
            if (hraId != 0)
            {
                if (showSelfScheduling)
                {
                    ActionStep ltActionStep = new ActionStep();
                    ltActionStep.name = string.Format(Translate.Message("L236"), "href='../Participant/MyCoach'");
                    response.lifetimeActionSteps.Add(ltActionStep);
                }
            }
            //Referral to a doctor
            var hraReport = ReadHRAReport(request.hraId);
            var pcpAlerts = GetPCPAlerts(hraReport.hra.User, hraReport.hra);
            if (pcpAlerts != null && pcpAlerts.Count > 0)
            {
                if (response.gapActionSteps == null)
                    response.gapActionSteps = new List<ActionStep>();
                ActionStep gapActionStep = new ActionStep();
                gapActionStep.name = Translate.Message("L1596");
                response.gapActionSteps.Insert(0, gapActionStep);
            }
            if (hraReport.hra.HealthNumbers.CAC.HasValue)
            {
                ActionStep gapActionStepCAC = new ActionStep();
                gapActionStepCAC.name = Translate.Message("L1998");
                response.gapActionSteps.Insert(0, gapActionStepCAC);
            }
            if (response.lifetimeActionSteps.Count == 0)
            {
                ActionStep lifestyleActionStep = new ActionStep();
                lifestyleActionStep.name = Translate.Message("L1481");
                lifestyleActionStep.noneIdentified = true;
                response.lifetimeActionSteps.Add(lifestyleActionStep);
            }
            if (response.gapActionSteps.Count == 0)
            {
                ActionStep gapActionStep = new ActionStep();
                gapActionStep.name = Translate.Message("L1481");
                gapActionStep.noneIdentified = true;
                response.gapActionSteps.Add(gapActionStep);
            }
            //PCP alerts
            //Need to do it later
            return response;
        }

        public static ConditionsNeedHelpResponse GetConditionsNeedHelp(int? hraId)
        {
            ReportReader reader = new ReportReader();
            ConditionsNeedHelpRequest request = new ConditionsNeedHelpRequest();
            request.hraId = hraId.Value;
            ConditionsNeedHelpResponse response = reader.GetConditionsNeedHelp(request);
            if (response.actionSteps != null && response.actionSteps.Count > 0)
            {
                string conditionString = "";
                for (int i = 0; i < response.actionSteps.Count; i++)
                {
                    if (string.IsNullOrEmpty(conditionString))
                        conditionString = Translate.Message(response.actionSteps[i].ActionStepType.HelpStatement);
                    else if (i == response.actionSteps.Count - 1)
                        conditionString = conditionString + " " + Translate.Message("L2518") + " " + Translate.Message(response.actionSteps[i].ActionStepType.HelpStatement);
                    else
                        conditionString = conditionString + ", " + Translate.Message(response.actionSteps[i].ActionStepType.HelpStatement);
                }
                response.conditions = conditionString;
            }
            return response;
        }

        public static ADARisk GetADAScore(HRADto hra)
        {
            ADARisk adaRisk = new ADARisk();
            if (hra.MedicalCondition.ToldDiabetes == 1 || hra.MedicalCondition.DiabetesMed == 1
                || (hra.HealthNumbers.A1C.HasValue && hra.HealthNumbers.A1C >= 5.7) ||
                (hra.HealthNumbers.Glucose.HasValue && hra.HealthNumbers.Glucose >= 100 && hra.HealthNumbers.DidYouFast == 1)
                || (hra.HealthNumbers.Glucose.HasValue && hra.HealthNumbers.Glucose >= 140 && hra.HealthNumbers.DidYouFast == 2))
            {
                adaRisk.isDiaborPreDiab = true;
            }
            var points = hra.ADAScore ?? 0;
            RiskChart adachart = new RiskChart();
            adachart.currentValue = points;
            adachart.label = Translate.Message("L3590");
            adachart.riskName = "ADA";
            adaRisk.riskInfoTextADA = string.Format(Translate.Message("L3589"), points);
            adaRisk.riskText = string.Format(Translate.Message("L3675"), points);
            adaRisk.riskChartADA = adachart;
            adaRisk.GINAQuestion = hra.Portal.GINAQuestion;
            return adaRisk;
        }

        #region Wellness Score

        public static WellnessScoreInfo GetWellnessScoreInfo(float score, byte harVer, int participantId, string participantTimeZone, string userTimeZone)
        {
            WellnessScoreInfo wellnessScoreInfo = new WellnessScoreInfo();
            wellnessScoreInfo.wellnessScore = Convert.ToDecimal(score);
            var wellnessScore = Convert.ToInt32(score);
            wellnessScoreInfo.newWellnessScore = CommonUtility.GetNewWellnessScore(score, harVer);
            var wellnessScoreDetails = GetWellnessScoreDetails(wellnessScore);
            wellnessScoreInfo.wellnessText = wellnessScoreDetails.wellnessText;
            wellnessScoreInfo.wellnessHead = wellnessScoreDetails.wellnessHead;
            var previousHra = ParticipantUtility.GetPrevYearStatus(participantId, participantTimeZone, userTimeZone, true);
            if (previousHra != null && previousHra.hra != null && previousHra.hra.CompleteDate != null && previousHra.hra.WellnessScore != null)
            {
                //var previousHRADate = previousHra.hra.CompleteDate.ToString();
                //previousHRADate = previousHRADate.Remove(previousHRADate.Length - 11);
                wellnessScoreInfo.previousHraDate = previousHra.hra.CompleteDate;
                //wellnessScoreInfo.InitialWellnessScore = Convert.ToDecimal(previousHra.hra.WellnessScore);
                wellnessScoreInfo.previousnewWellnessScore = CommonUtility.GetNewWellnessScore((float)previousHra.hra.WellnessScore, harVer);
                wellnessScoreInfo.previousWellnessText = GetWellnessScoreDetails((int)previousHra.hra.WellnessScore).wellnessHead;
            }
            return wellnessScoreInfo;
        }

        public static GetWellnessScoreDetailsResponse GetWellnessScoreDetails(int wellnessScore)
        {
            GetWellnessScoreDetailsResponse wellnessScoreInfo = new GetWellnessScoreDetailsResponse();

            if (wellnessScore < 50)
            {
                wellnessScoreInfo.wellnessText = "L251";
                wellnessScoreInfo.wellnessHead = "L925";
            }
            else if (wellnessScore < 60)
            {
                wellnessScoreInfo.wellnessText = "L250";
                wellnessScoreInfo.wellnessHead = "L925";
            }
            else if (wellnessScore < 70)
            {
                wellnessScoreInfo.wellnessText = "L249";
                wellnessScoreInfo.wellnessHead = "L924";
            }
            else if (wellnessScore < 80)
            {
                wellnessScoreInfo.wellnessText = "L248";
                wellnessScoreInfo.wellnessHead = "L923";
            }
            else if (wellnessScore < 90)
            {
                wellnessScoreInfo.wellnessText = "L247";
                wellnessScoreInfo.wellnessHead = "L922";
            }
            else if (wellnessScore < 100)
            {
                wellnessScoreInfo.wellnessText = "L246";
                wellnessScoreInfo.wellnessHead = "L921";
            }
            else
            {
                wellnessScoreInfo.wellnessText = "L245";
                wellnessScoreInfo.wellnessHead = "L921";
            }
            return wellnessScoreInfo;
        }
        public static double GetAvgWellnessScore(int? currentHraId, int? hraid)
        {
            ReportReader reader = new ReportReader();
            CompareWellnessScoreRequest request = new CompareWellnessScoreRequest();
            if (hraid.HasValue)
                request.hraId = hraid.Value;
            else
                request.hraId = currentHraId.Value;
            return reader.GetAvgWellnessScore(request).score;
        }

        #endregion

        public static AgeInfo GetAgeInformation(HRADto HRA, UserDto User)
        {
            AgeInfo ageInfo = new AgeInfo();
            ageInfo.Age = HRA.Age.Value;
            if (HRA.HealthNumbers.CAC.HasValue)
            {
                double cac = Convert.ToDouble(HRA.HealthNumbers.CAC);
                ageInfo.arterialAge = Convert.ToInt32(Math.Round(39.1 + (7.25 * Math.Log(cac + 1)), 2));
            }
            else
            {
                bool missingData;
                if (HRA.HealthNumbers != null && (!HRA.HealthNumbers.TotalChol.HasValue || !HRA.HealthNumbers.HDL.HasValue
                    || !HRA.HealthNumbers.DBP.HasValue || !HRA.HealthNumbers.SBP.HasValue))
                    missingData = true;
                else
                    missingData = false;

                if (ageInfo.Age < 18 || ageInfo.Age > 74 || HRA.Goals == null || HRA.Goals.ASCVD == true || HRA.Goals.TenYrProb <= 0 || missingData == true)
                {
                    ageInfo.heartAge = "0";
                    if (HRA.Goals.ASCVD == true)
                        ageInfo.heartAgeMessage = Translate.Message("L1452");
                    else if (ageInfo.Age > 74)
                        ageInfo.heartAgeMessage = Translate.Message("L1453");
                    else if (ageInfo.Age < 18)
                        ageInfo.heartAgeMessage = Translate.Message("L1454");
                    else if (missingData == true)
                        ageInfo.heartAgeMessage = Translate.Message("L1455");
                }
                else
                {
                    var heartAge = "0";
                    int tenYearRisk = (int)Math.Ceiling(Convert.ToDecimal(HRA.Goals.TenYrProb) * 100);
                    if (User.Gender.Value == 1)
                    {
                        if (tenYearRisk <= 2)
                        {
                            if (ageInfo.Age < 30)
                                heartAge = ageInfo.Age.ToString();
                            else
                                heartAge = "30-";
                        }
                        else
                        {
                            switch (tenYearRisk)
                            {
                                case 3:
                                    heartAge = "35";
                                    break;

                                case 4:
                                    heartAge = "40";
                                    break;

                                case 5:
                                    heartAge = "45";
                                    break;

                                case 6:
                                    heartAge = "50";
                                    break;

                                case 7:
                                    heartAge = "55";
                                    break;

                                case 8:
                                    heartAge = "57";
                                    break;

                                case 9:
                                    heartAge = "60";
                                    break;

                                case 10:
                                    heartAge = "62";
                                    break;

                                case 11:
                                    heartAge = "65";
                                    break;

                                case 12:
                                    heartAge = "67";
                                    break;

                                case 13:
                                    heartAge = "68";
                                    break;

                                case 14:
                                    heartAge = "70";
                                    break;

                                default:
                                    heartAge = "74+";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (tenYearRisk <= 1)
                        {
                            if (ageInfo.Age < 35)
                                heartAge = ageInfo.Age.ToString();
                            else
                                heartAge = "35-";
                        }
                        else
                        {
                            switch (tenYearRisk)
                            {
                                case 2:
                                    heartAge = "40";
                                    break;

                                case 3:
                                    heartAge = "45";
                                    break;

                                case 4:
                                    heartAge = "47";
                                    break;

                                case 5:
                                    heartAge = "50";
                                    break;

                                case 6:
                                    heartAge = "52";
                                    break;

                                case 7:
                                    heartAge = "55";
                                    break;

                                case 8:
                                    heartAge = "60";
                                    break;

                                default:
                                    heartAge = "74+";
                                    break;
                            }
                        }
                    }
                    ageInfo.heartAge = heartAge;
                }
            }
            return ageInfo;
        }

        #region CardioVascular Disease

        public static HDSRisks GetHDSRisk(UserDto user, HRADto HRA, HRADto previousHRA, bool isAdmin = false)
        {
            HDSRisks hdsRisks = new HDSRisks();
            int age = HRA.Age.Value;
            //risk
            if (HRA.HealthNumbers.CAC.HasValue)
            {
                if (HRA.HealthNumbers.CAC >= 101)
                {
                    hdsRisks.risk = 1;
                }
                else
                {
                    hdsRisks.risk = 2;
                }
                if (HRA.HealthNumbers.CAC == 0)
                {
                    hdsRisks.calciumRiskText = Translate.Message("L1999");
                }
                else if (1 <= HRA.HealthNumbers.CAC && HRA.HealthNumbers.CAC <= 10)
                {
                    hdsRisks.calciumRiskText = Translate.Message("L2000");
                }
                else if (11 <= HRA.HealthNumbers.CAC && HRA.HealthNumbers.CAC <= 100)
                {
                    hdsRisks.calciumRiskText = Translate.Message("L2001");
                }
                else if (101 <= HRA.HealthNumbers.CAC && HRA.HealthNumbers.CAC <= 400)
                {
                    hdsRisks.calciumRiskText = Translate.Message("L2002");
                }
                else if (401 <= HRA.HealthNumbers.CAC)
                {
                    hdsRisks.calciumRiskText = Translate.Message("L2003");
                }
                //risk text
                hdsRisks.calciumScore = Translate.Message("L2005") + " " + HRA.HealthNumbers.CAC.ToString() + ".";
                //risk chart
                RiskChart chart = new RiskChart();
                chart.currentValue = (float)HRA.HealthNumbers.CAC.Value;
                chart.label = Translate.Message("L2004");
                chart.riskName = "CAC";
                hdsRisks.calciumRiskChart = chart;
            }
            else
            {
                if (HRA.Goals.TenYrProb > 0 && (age >= 18 && age <= 74))
                {
                    if (HRA.Goals.TenYrProb > HRA.Goals.TenYrLow)
                    {
                        hdsRisks.risk = 1;
                    }
                    else
                    {
                        if (hdsRisks.risk != 1)
                            hdsRisks.risk = 2;
                    }
                    //risk text
                    hdsRisks.chdRiskText = Translate.Message("L1483");
                    //risk chart
                    RiskChart chart = new RiskChart();
                    chart.currentValue = HRA.Goals.TenYrProb.Value * 100;
                    chart.currentText = Translate.Message("L1484");
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                    chart.goalValue1 = HRA.Goals.TenYrLow.Value * 100;
                    chart.goalText1 = Translate.Message("L1485");
                    chart.label = Translate.Message("L1486");
                    chart.includeChar = "%";
                    chart.riskName = "CHD";
                    if (HRA.Goals.TenYrProb > HRA.Goals.TenYrLow)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    if (previousHRA != null && previousHRA.CompleteDate.HasValue && previousHRA.Goals != null && previousHRA.Goals.TenYrProb > 0)
                    {
                        chart.startValue = previousHRA.Goals.TenYrProb.Value * 100;
                        chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                        chart.startText = Translate.Message("L2121");
                        if (chart.startValue >= chart.goalValue1)
                            chart.color3 = Constants.riskColor;
                        else
                            chart.color3 = Constants.noRiskColor;
                    }
                    hdsRisks.chdRiskChart = chart;
                }
            }
            if (HRA.Goals.TenYearASCVD.HasValue && HRA.Goals.TenYearASCVDGoal.HasValue)
            {
                //risk
                if (HRA.Goals.TenYearASCVD > HRA.Goals.TenYearASCVDGoal)
                {
                    hdsRisks.risk = 1;
                }
                else
                {
                    if (hdsRisks.risk != 1)
                        hdsRisks.risk = 2;
                }
                //risktext
                if (isAdmin)
                    hdsRisks.tenYearRiskText = "Atherosclerotic cardiovascular disease (‘ASCVD’) " + Translate.Message("L1490");
                else
                    hdsRisks.tenYearRiskText = "Atherosclerotic cardiovascular disease (‘ASCVD’) " + Translate.Message("L1490");
                //risk chart
                RiskChart chart = new RiskChart();
                chart.currentValue = HRA.Goals.TenYearASCVD.Value;
                chart.currentText = Translate.Message("L1484");
                chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                chart.goalValue1 = HRA.Goals.TenYearASCVDGoal.Value;
                chart.goalText1 = Translate.Message("L1487");
                chart.label = Translate.Message("L1488");
                chart.includeChar = "%";
                chart.riskName = "10-Year";
                if (HRA.Goals.TenYearASCVD > HRA.Goals.TenYearASCVDGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.TenYearASCVD.HasValue && previousHRA.CompleteDate.HasValue)
                {
                    chart.startValue = previousHRA.Goals.TenYearASCVD.Value;
                    chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    chart.startText = Translate.Message("L2121");
                    if (chart.startValue >= chart.goalValue1)
                        chart.color3 = Constants.riskColor;
                    else
                        chart.color3 = Constants.noRiskColor;
                }
                hdsRisks.tenYearRiskChart = chart;
            }
            if (HRA.Goals.LifetimeASCVD.HasValue && HRA.Goals.LifetimeASCVDGoal.HasValue)
            {
                if (HRA.Goals.LifetimeASCVD > HRA.Goals.LifetimeASCVDGoal)
                {
                    hdsRisks.risk = 1;
                }
                else
                {
                    if (hdsRisks.risk != 1)
                        hdsRisks.risk = 2;
                }
                //risktext
                if (isAdmin)
                    hdsRisks.lifetimeRiskText = "Atherosclerotic cardiovascular disease (‘ASCVD’) " + Translate.Message("L1491");
                else
                    hdsRisks.lifetimeRiskText = "Atherosclerotic cardiovascular disease (‘ASCVD’) " + Translate.Message("L1491");
                //risk chart
                RiskChart chart = new RiskChart();
                chart.currentValue = HRA.Goals.LifetimeASCVD.Value;
                chart.currentText = Translate.Message("L1484");
                chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                chart.goalValue1 = HRA.Goals.LifetimeASCVDGoal.Value;
                chart.goalText1 = Translate.Message("L1487");
                chart.label = Translate.Message("L1492");
                chart.includeChar = "%";
                chart.riskName = "Lifetime";
                if (HRA.Goals.LifetimeASCVD > HRA.Goals.LifetimeASCVDGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LifetimeASCVD.HasValue && previousHRA.CompleteDate.HasValue)
                {
                    chart.startValue = previousHRA.Goals.LifetimeASCVD.Value;
                    chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    chart.startText = Translate.Message("L2121");
                    if (chart.startValue >= chart.goalValue1)
                        chart.color3 = Constants.riskColor;
                    else
                        chart.color3 = Constants.noRiskColor;
                }
                hdsRisks.lifetimeRiskChart = chart;
            }
            if (!hdsRisks.risk.HasValue)
                hdsRisks.riskText = Translate.Message("L1494");
            return hdsRisks;
        }

        public static int? GetFamilyCVRisk(HRADto hra)
        {
            int? risk = null;
            if (hra.OtherRiskFactors.HeartHist == 1)
            {
                risk = 1;
            }
            else if (hra.OtherRiskFactors.HeartHist == 2)
            {
                risk = 2;
            }
            return risk;
        }

        public static int? GetKnownCVDisease(HRADto HRA)
        {
            int? risk = null;
            if (HRA.MedicalCondition.Stroke == 1 || HRA.MedicalCondition.HeartAttack == 1 || HRA.MedicalCondition.Angina == 1 || HRA.MedicalCondition.ToldHeartBlock == 1 || HRA.MedicalCondition.ToldBlock == 1 || HRA.MedicalCondition.AnginaMed == 1 || HRA.MedicalCondition.HeartFailMed == 1)
                risk = 1;
            else
            {
                if (HRA.MedicalCondition.HeartCondMed == 1)
                    risk = null;
                else
                    risk = 2;
            }
            return risk;
        }

        #endregion

        #region Blood Pressure

        public static BPRisk GetBPRisk(HRADto HRA, int? hRAVer, bool? riskOnly, bool? clinicalOnly, HRADto previousHRA, bool isAdmin = false)
        {
            BPRisk bpRisk = new BPRisk();
            //risk
            if (HRA.MedicalCondition.HighBPMed == 1)
            {
                bpRisk.riskInfoText = "(Med)";
            }
            if (clinicalOnly == true)
            {
                if (HRA.HealthNumbers.SBP >= 120 || HRA.HealthNumbers.DBP >= 80)
                {
                    bpRisk.risk = 1;
                }
                else if (HRA.HealthNumbers.SBP.HasValue && HRA.HealthNumbers.SBP.HasValue)
                {
                    bpRisk.risk = 2;
                }
            }

            if (HRA.MedicalCondition.HighBPMed == 1 && (HRA.HealthNumbers.SBP >= 120 || HRA.HealthNumbers.DBP >= 80))
            {
                if (clinicalOnly != true)
                    bpRisk.risk = 1;
                bpRisk.riskText = Translate.Message("L1495");
            }
            else if (HRA.MedicalCondition.HighBPMed == 1 && (HRA.HealthNumbers.SBP < 120 && HRA.HealthNumbers.DBP < 80))
            {
                if (clinicalOnly != true)
                    bpRisk.risk = 1;
                bpRisk.riskText = Translate.Message("L1496");
            }
            else if (HRA.MedicalCondition.HighBPMed == 1 && (!HRA.HealthNumbers.SBP.HasValue || !HRA.HealthNumbers.DBP.HasValue))
            {
                if (clinicalOnly != true)
                    bpRisk.risk = 1;
                bpRisk.riskText = Translate.Message("L1497");
            }
            else if ((HRA.MedicalCondition.HighBPMed == 2 || !HRA.MedicalCondition.HighBPMed.HasValue) && (HRA.HealthNumbers.SBP >= 130 || HRA.HealthNumbers.DBP >= 80))
            {
                if (clinicalOnly != true)
                    bpRisk.risk = 1;
                bpRisk.riskText = Translate.Message("L1498");
            }
            else if ((HRA.MedicalCondition.HighBPMed == 2 || !HRA.MedicalCondition.HighBPMed.HasValue) && (HRA.HealthNumbers.SBP < 120 && HRA.HealthNumbers.DBP < 80))
            {
                if (clinicalOnly != true)
                {
                    bpRisk.risk = 2;
                    bpRisk.preRisk = 2;
                }
                bpRisk.riskText = Translate.Message("L1499");
            }
            else if ((HRA.MedicalCondition.HighBPMed == 2 || !HRA.MedicalCondition.HighBPMed.HasValue) && (HRA.HealthNumbers.SBP >= 120 && HRA.HealthNumbers.DBP < 80))
            {
                if (clinicalOnly != true)
                {
                    bpRisk.preRisk = 1;
                    bpRisk.risk = 2;
                }
                bpRisk.riskText = Translate.Message("L1500");
            }
            else if ((HRA.MedicalCondition.HighBPMed == 2 || !HRA.MedicalCondition.HighBPMed.HasValue) && (!HRA.HealthNumbers.SBP.HasValue || !HRA.HealthNumbers.DBP.HasValue))
            {
                bpRisk.riskText = Translate.Message("L1501");
            }
            var videoText = "";
            if (isAdmin)
            {
                videoText = Translate.Message("L2208");
            }
            else
            {
                videoText = Translate.Message("L2208");
            }

            bpRisk.riskText = bpRisk.riskText + " " + videoText;

            if (hRAVer == (byte)HRAVersions.CaptivaVersion)
                bpRisk.riskText = bpRisk.riskText + " " + Translate.Message("L4300");

            if (HRA.HealthNumbers.SBP >= 180 || HRA.HealthNumbers.DBP >= 120)
                bpRisk.riskText = bpRisk.riskText + " " + Translate.Message("L2652");
            //risk chart
            if (riskOnly != true)
            {
                RiskChart chart = new RiskChart();
                if (HRA.Goals.LtSBP.HasValue)
                {
                    if (HRA.HealthNumbers.SBP.HasValue)
                        chart.currentValue = HRA.HealthNumbers.SBP.Value;
                    chart.currentText = Translate.Message("L1457");
                    chart.goalValue1 = HRA.Goals.LtSBP.Value;
                    chart.goalText1 = Translate.Message("L1502");
                    chart.label = Translate.Message("L322");
                    chart.riskName = "SBP";
                    if (HRA.HealthNumbers.SBP >= HRA.Goals.LtSBP)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                    bpRisk.sbpRiskChart = chart;
                }
                if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LtSBP.HasValue && previousHRA.HealthNumbers.SBP.HasValue && previousHRA.CompleteDate.HasValue)
                {
                    chart.startValue = previousHRA.HealthNumbers.SBP.Value;
                    chart.startText = Translate.Message("L2121");
                    if (chart.startValue >= chart.goalValue1)
                        chart.color3 = Constants.riskColor;
                    else
                        chart.color3 = Constants.noRiskColor;
                    chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    bpRisk.sbpRiskChart = chart;
                }
                chart = new RiskChart();
                if (HRA.Goals.LtDBP.HasValue)
                {
                    if (HRA.HealthNumbers.DBP.HasValue)
                        chart.currentValue = HRA.HealthNumbers.DBP.Value;
                    chart.currentText = Translate.Message("L1457");
                    chart.goalValue1 = HRA.Goals.LtDBP.Value;
                    chart.goalText1 = Translate.Message("L1502");
                    chart.label = Translate.Message("L323");
                    chart.riskName = "DBP";
                    if (HRA.HealthNumbers.DBP >= HRA.Goals.LtDBP)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                    bpRisk.dbpRiskChart = chart;
                }
                if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LtDBP.HasValue && previousHRA.HealthNumbers.DBP.HasValue && previousHRA.CompleteDate.HasValue)
                {
                    chart.startValue = previousHRA.HealthNumbers.DBP.Value;
                    chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    chart.startText = Translate.Message("L2121");
                    if (chart.startValue >= chart.goalValue1)
                        chart.color3 = Constants.riskColor;
                    else
                        chart.color3 = Constants.noRiskColor;
                    bpRisk.dbpRiskChart = chart;
                }
            }
            return bpRisk;
        }

        public static int? GetKnownBPRisk(HRADto hra)
        {
            int? risk = null;
            if (hra.MedicalCondition.ToldHighBP == 1 || hra.MedicalCondition.HighBPMed == 1)
                risk = 1;
            else if (hra.MedicalCondition.ToldHighBP == 2 && hra.MedicalCondition.HighBPMed == 2)
                risk = 2;
            return risk;
        }

        #endregion

        #region Cholesterol & Trig

        public static CTRisk GetCTRisk(UserDto user, HRADto HRA, int? hRAVer, bool? riskOnly, IList<MeasurementsDto> Measurements, HRADto previousHRA, int unit)
        {
            CTRisk ctRisk = new CTRisk();
            bool isCaptivaVersion = hRAVer == (byte)HRAVersions.CaptivaVersion;

            if (HRA.MedicalCondition.HighCholMed == 1)
                ctRisk.riskInfoText = "(Med)";
            //risk
            if ((HRA.HealthNumbers.HDL < 40 && user.Gender == 1) || (HRA.HealthNumbers.HDL < 50 && user.Gender == 2)
                || (HRA.HealthNumbers.DidYouFast == 1 && ((HRA.HealthNumbers.LDL >= HRA.Goals.LtLdl)
                || (HRA.HealthNumbers.Trig >= HRA.Goals.LtTrig) || HRA.HealthNumbers.LDL >= 130
                || (!HRA.HealthNumbers.LDL.HasValue && (HRA.HealthNumbers.TotalChol - HRA.HealthNumbers.HDL >= 160))))
                || (HRA.HealthNumbers.DidYouFast == 2 && (HRA.HealthNumbers.TotalChol - HRA.HealthNumbers.HDL >= 160)))
            {
                ctRisk.risk = 1;
                ctRisk.riskText = Translate.Message("L1597");
            }
            else if ((HRA.HealthNumbers.DidYouFast == 2) || !HRA.HealthNumbers.LDL.HasValue || !HRA.HealthNumbers.HDL.HasValue || !HRA.HealthNumbers.Trig.HasValue)
            {
                ctRisk.riskText = Translate.Message("L1501");
            }
            else
            {
                ctRisk.risk = 2;
                ctRisk.riskText = Translate.Message("L1598");
            }
            var optimalLdlVal = Math.Round(CommonUtility.ToMetric(HRA.Goals.OptimalLdl.Value, BioLookup.LDL, unit), 1);
            var hdlVal = Math.Round(CommonUtility.ToMetric(60, BioLookup.HDL, unit), 1);
            string ctriskText = string.Format(Translate.Message("L1599"), optimalLdlVal + " " + Measurements[BioLookup.LDL].MeasurementUnit, hdlVal + " " + Measurements[BioLookup.HDL].MeasurementUnit);
            ctRisk.riskText = ctRisk.riskText + " " + ctriskText;

            //risk chart
            if (riskOnly != true)
            {
                RiskChart chart = new RiskChart();
                if (HRA.Goals.LtLdl.HasValue)
                {
                    string fastingText = Translate.Message("L2551");
                    if (HRA.HealthNumbers.LDL.HasValue && (HRA.HealthNumbers.DidYouFast == 1 || isCaptivaVersion))
                    {
                        //chart.currentValue = HRA.HealthNumbers.LDL.Value;
                        chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(HRA.HealthNumbers.LDL.Value, BioLookup.LDL, unit), 1);
                        if (HRA.HealthNumbers.DidYouFast == 1)
                            fastingText = Translate.Message("L2550");
                    }

                    chart.currentText = Translate.Message("L1457");
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(HRA.Goals.LtLdl.Value, BioLookup.LDL, unit), 1); //HRA.Goals.LtLdl.Value
                    chart.goalText1 = Translate.Message("L1502");
                    chart.label = string.Format(Translate.Message("L1601"), isCaptivaVersion ? "" : Translate.Message("L2550"), " (" + Measurements[BioLookup.LDL].MeasurementUnit + ")");
                    chart.riskName = "LDL";
                    if (HRA.HealthNumbers.LDL >= HRA.Goals.LtLdl)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString() + (isCaptivaVersion ? "| (" + fastingText + ")" : "");
                    ctRisk.ldlRiskChart = chart;
                }
                if (HRA.Goals.LtHdl.HasValue)
                {
                    chart = new RiskChart();
                    if (HRA.HealthNumbers.HDL.HasValue)
                        chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(HRA.HealthNumbers.HDL.Value, BioLookup.HDL, unit), 1);//HRA.HealthNumbers.HDL.Value;
                    chart.currentText = Translate.Message("L1457");
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(HRA.Goals.LtHdl.Value, BioLookup.HDL, unit), 1); //HRA.Goals.LtHdl.Value;
                    chart.goalText1 = Translate.Message("L1502");
                    chart.label = string.Format(Translate.Message("L1602"), " (" + Measurements[BioLookup.HDL].MeasurementUnit + ")");
                    chart.riskName = "HDL";
                    if (HRA.HealthNumbers.HDL < HRA.Goals.LtHdl)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                    ctRisk.hdlRiskChart = chart;
                }
                if (HRA.Goals.LtTrig.HasValue)
                {
                    string fastingText = Translate.Message("L2551");
                    chart = new RiskChart();
                    if (HRA.HealthNumbers.Trig.HasValue && (HRA.HealthNumbers.DidYouFast == 1 || isCaptivaVersion))
                    {
                        chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(HRA.HealthNumbers.Trig.Value, BioLookup.Triglycerides, unit), 1); //HRA.HealthNumbers.Trig.Value;
                        if (HRA.HealthNumbers.DidYouFast == 1)
                            fastingText = Translate.Message("L2550");
                    }
                    chart.currentText = Translate.Message("L1457");
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(HRA.Goals.LtTrig.Value, BioLookup.Triglycerides, unit), 1); //HRA.Goals.LtTrig.Value;
                    chart.goalText1 = Translate.Message("L1502");
                    chart.label = string.Format(Translate.Message("L1600"), isCaptivaVersion ? "" : Translate.Message("L2550"), " (" + Measurements[BioLookup.Triglycerides].MeasurementUnit + ")");
                    chart.riskName = "Trig";
                    if (HRA.HealthNumbers.Trig >= HRA.Goals.LtTrig)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString() + (isCaptivaVersion ? "| (" + fastingText + ")" : "");
                    ctRisk.trigRiskChart = chart;
                }
                chart = new RiskChart();
                if (HRA.HealthNumbers.TotalChol.HasValue)
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(HRA.HealthNumbers.TotalChol.Value, BioLookup.Cholesterol, unit), 1);// HRA.HealthNumbers.TotalChol.Value;
                chart.currentText = Translate.Message("L1457");
                chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(200, BioLookup.Cholesterol, unit), 1); //200;
                chart.goalText1 = Translate.Message("L1502");
                chart.label = string.Format(Translate.Message("L324"), Measurements[BioLookup.Cholesterol].MeasurementUnit);
                chart.riskName = "Chol";
                if (HRA.HealthNumbers.TotalChol >= 200)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                ctRisk.tcRiskChart = chart;
                if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LtLdl.HasValue && previousHRA.HealthNumbers.LDL.HasValue && previousHRA.CompleteDate.HasValue && (previousHRA.HealthNumbers.DidYouFast == 1 || isCaptivaVersion))
                {
                    string fastingText = Translate.Message("L2551");
                    if (previousHRA.HealthNumbers.DidYouFast == 1)
                        fastingText = Translate.Message("L2550");
                    ctRisk.ldlRiskChart.startValue = (float)Math.Round(CommonUtility.ToMetric(previousHRA.HealthNumbers.LDL.Value, BioLookup.LDL, unit), 1);
                    ctRisk.ldlRiskChart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString() + (isCaptivaVersion ? "| (" + fastingText + ")" : "");
                    ctRisk.ldlRiskChart.startText = Translate.Message("L2121");
                    if (ctRisk.ldlRiskChart.startValue >= ctRisk.ldlRiskChart.goalValue1)
                        ctRisk.ldlRiskChart.color3 = Constants.riskColor;
                    else
                        ctRisk.ldlRiskChart.color3 = Constants.noRiskColor;
                }
                if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LtHdl.HasValue && previousHRA.HealthNumbers.HDL.HasValue && previousHRA.CompleteDate.HasValue)
                {
                    ctRisk.hdlRiskChart.startValue = (float)Math.Round(CommonUtility.ToMetric(previousHRA.HealthNumbers.HDL.Value, BioLookup.HDL, unit), 1);
                    ctRisk.hdlRiskChart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    ctRisk.hdlRiskChart.startText = Translate.Message("L2121");
                    if (ctRisk.hdlRiskChart.startValue < ctRisk.hdlRiskChart.goalValue1)
                        ctRisk.hdlRiskChart.color3 = Constants.riskColor;
                    else
                        ctRisk.hdlRiskChart.color3 = Constants.noRiskColor;
                }
                if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LtTrig.HasValue && previousHRA.HealthNumbers.Trig.HasValue && previousHRA.CompleteDate.HasValue && (previousHRA.HealthNumbers.DidYouFast == 1 || isCaptivaVersion))
                {
                    string fastingText = Translate.Message("L2551");
                    if (previousHRA.HealthNumbers.DidYouFast == 1)
                        fastingText = Translate.Message("L2550");
                    ctRisk.trigRiskChart.startValue = (float)Math.Round(CommonUtility.ToMetric(previousHRA.HealthNumbers.Trig.Value, BioLookup.Triglycerides, unit), 1);
                    ctRisk.trigRiskChart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString() + (isCaptivaVersion ? "| (" + fastingText + ")" : "");
                    ctRisk.trigRiskChart.startText = Translate.Message("L2121");
                    if (ctRisk.trigRiskChart.startValue >= ctRisk.trigRiskChart.goalValue1)
                        ctRisk.trigRiskChart.color3 = Constants.riskColor;
                    else
                        ctRisk.trigRiskChart.color3 = Constants.noRiskColor;
                }
                if (previousHRA != null && previousHRA.HealthNumbers != null && previousHRA.HealthNumbers.TotalChol.HasValue && previousHRA.CompleteDate.HasValue)
                {
                    ctRisk.tcRiskChart.startValue = (float)Math.Round(CommonUtility.ToMetric(previousHRA.HealthNumbers.TotalChol.Value, BioLookup.Cholesterol, unit), 1);
                    ctRisk.tcRiskChart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    ctRisk.tcRiskChart.startText = Translate.Message("L2121");
                    if (ctRisk.tcRiskChart.startValue >= ctRisk.tcRiskChart.goalValue1)
                        ctRisk.tcRiskChart.color3 = Constants.riskColor;
                    else
                        ctRisk.tcRiskChart.color3 = Constants.noRiskColor;
                }
            }
            return ctRisk;
        }

        public static int? GetKnownCTRisk(HRADto hra)
        {
            int? risk = null;
            if (hra.MedicalCondition.ToldHighChol == 1 || hra.MedicalCondition.HighCholMed == 1)
                risk = 1;
            else if (hra.MedicalCondition.ToldHighChol == 2 && hra.MedicalCondition.HighCholMed == 2)
                risk = 2;
            return risk;
        }

        #endregion

        public static OBRisk GetOverweightRisk(HRADto HRA, bool? riskOnly, IList<MeasurementsDto> Measurements, HRADto previousHRA, int unit, bool isadmin = false)
        {
            OBRisk obRisk = new OBRisk();
            var bmi = CommonUtility.GetBMI(HRA.HealthNumbers.Height.Value, HRA.HealthNumbers.Weight.Value);
            //risk
            if (bmi >= 30)
            {
                obRisk.risk = 1;
                obRisk.riskInfoText = HRA.Interest.WeightManProg.HasValue ? "(Obesity; Prog Interest : " + HRA.Interest.WeightManProg.Value + ")" : "(Obesity)";
            }
            else if (bmi >= 25)
            {
                obRisk.risk = 2;
                obRisk.riskInfoText = HRA.Interest.WeightManProg.HasValue ? "(Prog Interest : " + HRA.Interest.WeightManProg.Value + ")" : string.Empty;
            }
            else if (bmi < 18.5)
            {
                obRisk.risk = 3;
                obRisk.riskInfoText = HRA.Interest.WeightManProg.HasValue ? "(Prog Interest : " + HRA.Interest.WeightManProg.Value + ")" : string.Empty;
            }
            else
            {
                obRisk.risk = 0;
            }
            obRisk.safeWeight = GetWeightfromBMI(HRA.HealthNumbers.Height.Value, (float)18.5);
            //risk text
            if (isadmin)
                obRisk.riskText = Translate.Message("L1456");
            else
                obRisk.riskText = Translate.Message("L1456");
            //risk chart
            if (riskOnly != true)
            {
                RiskChart chart = new RiskChart();
                if (HRA.HealthNumbers.Weight.HasValue && HRA.Goals.StWt.HasValue && HRA.Goals.LtWt.HasValue)
                {
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(HRA.HealthNumbers.Weight.Value, BioLookup.Weight, unit), 1);//HRA.HealthNumbers.Weight.Value;
                    chart.currentText = Translate.Message("L1457");
                    chart.goalValue0 = Math.Round(CommonUtility.ToMetric((float)obRisk.safeWeight, BioLookup.Weight, unit), 1); //obRisk.safeWeight;
                    chart.goalText0 = Translate.Message("L1458");
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(HRA.Goals.StWt.Value, BioLookup.Weight, unit), 1); //HRA.Goals.StWt.Value;
                    chart.goalText1 = Translate.Message("L1459");
                    chart.goalValue2 = (float)Math.Round(CommonUtility.ToMetric(HRA.Goals.LtWt.Value, BioLookup.Weight, unit), 1);  //HRA.Goals.LtWt.Value;
                    chart.goalText2 = Translate.Message("L1460");
                    chart.label = Translate.Message("L1461");
                    chart.label = string.Format(Translate.Message("L1461"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
                    chart.riskName = "Obese";
                    if (HRA.HealthNumbers.Weight > HRA.Goals.StWt || HRA.HealthNumbers.Weight < obRisk.safeWeight)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                    obRisk.riskChart = chart;
                }
                if (previousHRA != null && previousHRA.HealthNumbers != null && previousHRA.HealthNumbers.Weight.HasValue && previousHRA.CompleteDate.HasValue && previousHRA.Goals.StWt.HasValue && previousHRA.Goals.LtWt.HasValue)
                {
                    chart.startValue = (float)Math.Round(CommonUtility.ToMetric(previousHRA.HealthNumbers.Weight.Value, BioLookup.Weight, unit), 1); ;
                    chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    chart.startText = Translate.Message("L2121");
                    if (chart.startValue > chart.goalValue1 || chart.startValue < obRisk.safeWeight)
                        chart.color3 = Constants.riskColor;
                    else
                        chart.color3 = Constants.noRiskColor;
                    obRisk.riskChart = chart;
                }
            }
            return obRisk;
        }

        public static MetRisk GetMetRisk(HRADto HRA)
        {
            MetRisk metRisk = new MetRisk();
            metRisk.riskIntro = Translate.Message("L1462");
            //risk
            if (HRA.Goals.MetSyn == 1)
            {
                metRisk.risk = 1;
            }
            else if (HRA.Goals.MetSyn == 2)
            {
                metRisk.risk = 2;
            }
            else if (HRA.Goals.MetSyn == 0)
            {
                metRisk.risk = 0;
            }
            if (!HRA.Goals.MetSynDets.Contains("?"))
            {
                metRisk.riskIntro = metRisk.riskIntro.Replace("{0}", "You have " + Convert.ToString(HRA.Goals.MetSynDets.Split('Y').Length - 1) + " metabolic risk factors.");
            }
            else
            {
                metRisk.riskIntro = metRisk.riskIntro.Replace("{0}", Translate.Message("L1505").Replace("{0}", Convert.ToString(HRA.Goals.MetSynDets.Split('Y').Length - 1)));
            }
            //risk text
            IList<string> riskText = new List<String>();
            string text;
            text = Translate.Message("L782");
            string ob = HRA.Goals.MetSynDets.Substring(0, 1);
            if (ob == "Y")
            {
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (ob == "?")
            {
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }
            string tg = HRA.Goals.MetSynDets.Substring(1, 1);
            text = Translate.Message("L786");
            if (tg == "Y")
            {
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (tg == "?")
            {
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }
            string hdl = HRA.Goals.MetSynDets.Substring(2, 1);
            text = Translate.Message("L1463");
            if (hdl == "Y")
            {
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (hdl == "?")
            {
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }
            string bp = HRA.Goals.MetSynDets.Substring(3, 1);
            text = Translate.Message("L792");
            if (bp == "Y")
            {
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (bp == "?")
            {
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }
            string gl = HRA.Goals.MetSynDets.Substring(4, 1);
            text = Translate.Message("L794");
            if (gl == "Y")
            {
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (gl == "?")
            {
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }
            metRisk.riskText = riskText;
            return metRisk;
        }

        #region Diabetes

        public static DBRisk GetDiabetesRisk(UserDto user, HRADto HRA, bool? clinicalOnly, HRADto previousHRA, int unit)
        {
            DBRisk dbRisk = new DBRisk();
            if (clinicalOnly == true)
            {
                if (HRA.MedicalCondition.ToldDiabetes == 1 || HRA.MedicalCondition.DiabetesMed == 1)
                {
                    if ((HRA.HealthNumbers.Glucose >= 180) || (HRA.HealthNumbers.Glucose >= 130 && HRA.HealthNumbers.DidYouFast == 1) || (HRA.HealthNumbers.A1C >= (float)7.0))
                    {
                        dbRisk.risk = 1;
                    }
                    else if ((HRA.HealthNumbers.Glucose.HasValue && HRA.HealthNumbers.DidYouFast == 1) || (HRA.HealthNumbers.A1C.HasValue))
                    {
                        dbRisk.risk = 2;
                    }
                }
                else if ((HRA.MedicalCondition.ToldDiabetes == 2 || !HRA.MedicalCondition.ToldDiabetes.HasValue) && (HRA.MedicalCondition.DiabetesMed == 2 || !HRA.MedicalCondition.DiabetesMed.HasValue))
                {
                    if ((HRA.HealthNumbers.Glucose >= 140) || (HRA.HealthNumbers.Glucose >= 100 && HRA.HealthNumbers.DidYouFast == 1) || (HRA.HealthNumbers.A1C >= (float)5.7))
                    {
                        dbRisk.risk = 1;
                    }
                    else if ((HRA.HealthNumbers.Glucose.HasValue && HRA.HealthNumbers.DidYouFast == 1) || (HRA.HealthNumbers.A1C.HasValue))
                    {
                        dbRisk.risk = 2;
                    }
                }
            }
            if (HRA.MedicalCondition.ToldDiabetes == 1 || HRA.MedicalCondition.DiabetesMed == 1)
            {
                dbRisk.riskText = Translate.Message("L1506");
                if (clinicalOnly != true)
                    dbRisk.risk = 1;
            }
            else if ((HRA.HealthNumbers.Glucose >= 200) || (HRA.HealthNumbers.Glucose >= 126 && HRA.HealthNumbers.DidYouFast == 1) || HRA.HealthNumbers.A1C >= (float)6.5)
            {
                dbRisk.riskText = Translate.Message("L1507");
                if (clinicalOnly != true)
                    dbRisk.risk = 1;
            }
            else if ((HRA.HealthNumbers.Glucose >= 140) || (HRA.HealthNumbers.Glucose >= 100 && HRA.HealthNumbers.DidYouFast == 1) || (HRA.HealthNumbers.A1C >= (float)5.7))
            {
                dbRisk.riskText = Translate.Message("L1508");
                if (clinicalOnly != true)
                {
                    dbRisk.risk = 2;
                    dbRisk.preRisk = 1;
                }
            }
            else if ((HRA.HealthNumbers.Glucose < 100) || (HRA.HealthNumbers.A1C < (float)5.7))
            {
                if (HRA.OtherRiskFactors.DiabetesHist == 2 && ((HRA.MedicalCondition.ToldBabyNine == 2 && HRA.MedicalCondition.ToldGestDiab == 2 && HRA.MedicalCondition.ToldPolycyst == 2) || user.Gender == 1))
                {
                    dbRisk.riskText = Translate.Message("L1509");
                }
                else if (!HRA.OtherRiskFactors.DiabetesHist.HasValue) //value if null for family history
                {
                    dbRisk.riskText = Translate.Message("L1510");
                }
                else if (HRA.OtherRiskFactors.DiabetesHist == 1 || ((HRA.MedicalCondition.ToldBabyNine == 1 || HRA.MedicalCondition.ToldGestDiab == 1 || HRA.MedicalCondition.ToldPolycyst == 1) && user.Gender == 2))
                {
                    dbRisk.riskText = Translate.Message("L1511");
                }
                else
                {
                    dbRisk.riskText = Translate.Message("L1501");
                }
                if (clinicalOnly != true)
                {
                    dbRisk.risk = 2;
                    dbRisk.preRisk = 2;
                }
            }
            else if ((!HRA.HealthNumbers.Glucose.HasValue && !HRA.HealthNumbers.A1C.HasValue) || (HRA.HealthNumbers.Glucose >= 100 && HRA.HealthNumbers.Glucose <= 139))
            {
                if (HRA.OtherRiskFactors.DiabetesHist == 1 || ((HRA.MedicalCondition.ToldBabyNine == 1 || HRA.MedicalCondition.ToldGestDiab == 1 || HRA.MedicalCondition.ToldPolycyst == 1) && user.Gender == 2))
                {
                    dbRisk.riskText = Translate.Message("L1512");
                }
                else
                {
                    dbRisk.riskText = Translate.Message("L1501");
                }
            }

            dbRisk.riskText = dbRisk.riskText + " " + Translate.Message("L1513");

            RiskChart chart = new RiskChart();
            if (HRA.Goals.LtGluc1.HasValue && HRA.Goals.LtGluc2.HasValue)
            {
                if (HRA.HealthNumbers.Glucose.HasValue && HRA.HealthNumbers.DidYouFast == 1)
                {
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(HRA.HealthNumbers.Glucose.Value, BioLookup.Glucose, unit), 1);//HRA.HealthNumbers.Glucose.Value;
                }
                chart.currentText = Translate.Message("L1457");
                if (HRA.Goals.LtGluc1 < HRA.Goals.LtGluc2)
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(HRA.Goals.LtGluc2.Value, BioLookup.Glucose, unit), 1);//HRA.Goals.LtGluc2.Value;
                else
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(HRA.Goals.LtGluc1.Value, BioLookup.Glucose, unit), 1);//HRA.Goals.LtGluc1.Value;
                chart.goalText1 = Translate.Message("L1502");
                chart.label = string.Format(Translate.Message("L1514"), " (" + CommonUtility.GetMeasurementText(BioLookup.Glucose, unit) + ")");
                chart.riskName = "Glucose";
                if (HRA.Goals.Diabetes == true)
                {
                    chart.goalValue0 = Math.Round(CommonUtility.ToMetric(HRA.Goals.LtGluc1.Value, BioLookup.Glucose, unit), 1); //HRA.Goals.LtGluc1.Value;
                    chart.goalText0 = Translate.Message("L1458");
                }
                if ((HRA.HealthNumbers.Glucose > HRA.Goals.LtGluc2 && HRA.Goals.Diabetes == true) || (HRA.HealthNumbers.Glucose >= HRA.Goals.LtGluc2 && HRA.Goals.Diabetes == false) ||
                    (HRA.HealthNumbers.Glucose < HRA.Goals.LtGluc1 && HRA.Goals.Diabetes == true)) // (HRA.HealthNumbers.Glucose < 50 && HRA.Goals.Diabetes == false)
                {
                    chart.color1 = Constants.riskColor;
                }
                else
                {
                    chart.color1 = Constants.noRiskColor;
                }
                chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                dbRisk.glucChart = chart;
            }

            if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LtGluc1.HasValue && previousHRA.Goals.LtGluc2.HasValue && previousHRA.CompleteDate.HasValue)
            {
                if (previousHRA.HealthNumbers.Glucose.HasValue && previousHRA.HealthNumbers.DidYouFast == 1)
                {
                    chart.startValue = (float)Math.Round(CommonUtility.ToMetric(previousHRA.HealthNumbers.Glucose.Value, BioLookup.Glucose, unit), 1);
                    chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    chart.startText = Translate.Message("L2121");
                    if ((chart.startValue > HRA.Goals.LtGluc2 && HRA.Goals.Diabetes == true) || (chart.startValue >= HRA.Goals.LtGluc2 && HRA.Goals.Diabetes == false) || (chart.startValue < HRA.Goals.LtGluc1 && HRA.Goals.Diabetes == true))
                        chart.color3 = Constants.riskColor;
                    else
                        chart.color3 = Constants.noRiskColor;
                }
                dbRisk.glucChart = chart;
            }
            chart = new RiskChart();
            if (HRA.Goals.LtA1c.HasValue)
            {
                chart = new RiskChart();
                if (HRA.HealthNumbers.A1C.HasValue)
                    chart.currentValue = HRA.HealthNumbers.A1C.Value;
                chart.currentText = Translate.Message("L1457");
                chart.goalValue1 = HRA.Goals.LtA1c.Value;
                chart.goalText1 = Translate.Message("L1502");
                chart.label = Translate.Message("L329");
                chart.riskName = "A1C";
                chart.includeChar = "%";
                if (HRA.HealthNumbers.A1C >= HRA.Goals.LtA1c)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                dbRisk.a1cChart = chart;
            }
            if (previousHRA != null && previousHRA.Goals != null && previousHRA.Goals.LtA1c.HasValue && previousHRA.HealthNumbers.A1C.HasValue && previousHRA.CompleteDate.HasValue)
            {
                chart.startValue = previousHRA.HealthNumbers.A1C.Value;
                chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                chart.startText = Translate.Message("L2121");
                if (chart.startValue >= chart.goalValue1)
                    chart.color3 = Constants.riskColor;
                else
                    chart.color3 = Constants.noRiskColor;
                dbRisk.a1cChart = chart;
            }
            return dbRisk;
        }

        public static int? GetFamilyDiabetesRisk(HRADto hra)
        {
            int? risk = null;
            if (hra.OtherRiskFactors.DiabetesHist == 1)
            {
                risk = 1;
            }
            else if (hra.OtherRiskFactors.DiabetesHist == 2)
            {
                risk = 2;
            }
            return risk;
        }

        public static int? GetKnownDiabetesRisk(HRADto hra, UserDto user)
        {
            int? risk = null;
            if (hra.MedicalCondition.ToldDiabetes == 1 || hra.MedicalCondition.DiabetesMed == 1 || (hra.MedicalCondition.ToldGestDiab == 1 && user.Gender == 2))
                risk = 1;
            else if (hra.MedicalCondition.ToldDiabetes == 2 && hra.MedicalCondition.DiabetesMed == 2 && ((hra.MedicalCondition.ToldGestDiab == 2 && user.Gender == 2) || user.Gender == 1))
                risk = 2;
            return risk;
        }

        #endregion

        #region Cancer

        public static CancerRisk GetCancerRisk(HRADto HRA)
        {
            var bmi = CommonUtility.GetBMI(HRA.HealthNumbers.Height.Value, HRA.HealthNumbers.Weight.Value);
            IList<string> riskText = new List<String>();
            CancerRisk cancerRisk = new CancerRisk();
            cancerRisk.riskIntro = Translate.Message("L1516");
            string text;

            text = Translate.Message("L1517");
            if (HRA.MedicalCondition.ToldCancer == 1)
            {
                cancerRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.MedicalCondition.ToldCancer == 2)
            {
                if (cancerRisk.risk != 1 && cancerRisk.risk != 3)
                    cancerRisk.risk = 2;
            }
            else
            {
                if (cancerRisk.risk != 1)
                    cancerRisk.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L303");
            if (HRA.OtherRiskFactors.CancerHist == 1)
            {
                cancerRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.OtherRiskFactors.CancerHist == 2)
            {
                if (cancerRisk.risk != 1 && cancerRisk.risk != 3)
                    cancerRisk.risk = 2;
            }
            else
            {
                if (cancerRisk.risk != 1)
                    cancerRisk.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L270");
            if (HRA.OtherRiskFactors.SmokeCig == 1 || HRA.OtherRiskFactors.OtherTobacco == 1)
            {
                cancerRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.OtherRiskFactors.SmokeCig == 2 && HRA.OtherRiskFactors.OtherTobacco == 2)
            {
                if (cancerRisk.risk != 1 && cancerRisk.risk != 3)
                    cancerRisk.risk = 2;
            }
            else
            {
                if (cancerRisk.risk != 1)
                    cancerRisk.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1518");
            if (bmi >= 25)
            {
                cancerRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (bmi < 25)
            {
                if (cancerRisk.risk != 1 && cancerRisk.risk != 3)
                    cancerRisk.risk = 2;
            }
            else
            {
                if (cancerRisk.risk != 1)
                    cancerRisk.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1519");
            if (HRA.OtherRiskFactors.LowFatDiet == 2)
            {
                cancerRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.OtherRiskFactors.LowFatDiet == 1)
            {
                if (cancerRisk.risk != 1 && cancerRisk.risk != 3)
                    cancerRisk.risk = 2;
            }
            else
            {
                if (cancerRisk.risk != 1)
                    cancerRisk.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1520");
            if (HRA.OtherRiskFactors.TwoAlcohol == 1)
            {
                cancerRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.OtherRiskFactors.TwoAlcohol == 2)
            {
                if (cancerRisk.risk != 1 && cancerRisk.risk != 3)
                    cancerRisk.risk = 2;
            }
            else
            {
                if (cancerRisk.risk != 1)
                    cancerRisk.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L801");
            if (HRA.OtherRiskFactors.ModExer == 2)
            {
                cancerRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.OtherRiskFactors.ModExer == 1)
            {
                if (cancerRisk.risk != 1 && cancerRisk.risk != 3)
                    cancerRisk.risk = 2;
            }
            else
            {
                if (cancerRisk.risk != 1)
                    cancerRisk.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }
            if (cancerRisk.risk == 2)
            {
                riskText.Add("<i class='fa fa-smile-o'></i>" + Translate.Message("L1481"));
            }
            cancerRisk.riskText = riskText;
            return cancerRisk;
        }

        public static int? GetFamilyCancerRisk(HRADto hra)
        {
            int? risk = null;
            if (hra.OtherRiskFactors.CancerHist == 1)
            {
                risk = 1;
            }
            else if (hra.OtherRiskFactors.CancerHist == 2)
            {
                risk = 2;
            }
            return risk;
        }

        public static int? GetKnownCancerRisk(HRADto hra)
        {
            int? risk = null;
            if (hra.MedicalCondition.ToldCancer == 1)
                risk = 1;
            else if (hra.MedicalCondition.ToldCancer == 2)
                risk = 2;
            return risk;
        }

        #endregion

        public static int? GetKnownBreathingDisorders(HRADto hra)
        {
            int? risk = null;
            if (hra.MedicalCondition.ToldBronchitis == 1 || hra.MedicalCondition.ToldAsthma == 1 || hra.MedicalCondition.AsthmaMed == 1 ||
                hra.MedicalCondition.BronchitisMed == 1 || (hra.OtherRiskFactors.PhysicalProb == 1 && hra.OtherRiskFactors.BreathProb == 1))
            {
                risk = 1;
            }
            else if (hra.MedicalCondition.ToldBronchitis == 2 && hra.MedicalCondition.ToldAsthma == 2 && hra.MedicalCondition.AsthmaMed == 2 &&
                hra.MedicalCondition.BronchitisMed == 2 && (hra.OtherRiskFactors.PhysicalProb == 2 || hra.OtherRiskFactors.BreathProb == 2))
            {
                risk = 2;
            }
            return risk;
        }

        public static PARisk GetPARisk(HRADto HRA, int? hraVer, bool? riskOnly)
        {
            PARisk paRisk = new PARisk();
            if (HRA.Interest.ExerProg.HasValue)
                paRisk.riskInfoText = Translate.Message("L4558") + " " + HRA.Interest.ExerProg.ToString();
            //risk
            if (HRA.OtherRiskFactors.ModExer == 2)
            {
                paRisk.risk = 1;
            }
            else if (HRA.OtherRiskFactors.ModExer == 1)
            {
                paRisk.risk = 2;
            }
            //risk text
            paRisk.riskText = Translate.Message("L1464");
            //risk chart
            if (riskOnly != true)
            {
                RiskChart chart = new RiskChart();
                if (HRA.Goals.StExPt.HasValue && HRA.Goals.LtExPt.HasValue)
                {
                    chart.currentValue = HRA.Goals.StExPt.Value;
                    chart.currentText = Translate.Message("L1459");
                    chart.goalValue1 = HRA.Goals.LtExPt.Value;
                    chart.goalText1 = Translate.Message("L1460");
                    chart.label = Translate.Message("L1465");
                    chart.riskName = "PA";
                    chart.color1 = "#67b7dc";
                    paRisk.riskChart = chart;
                }
                if (hraVer == (int)HRAVersions.ActivateVersion)
                {
                    RiskChart paichart = new RiskChart();
                    paichart.currentValue = (float)Math.Round(((HRA.OtherRiskFactors.ExerciseFrequency ?? 0) * (HRA.OtherRiskFactors.ExerciseIntensity ?? 0) * (HRA.OtherRiskFactors.ExerciseDuration ?? 0)), 2);
                    if (paichart.currentValue <= 1.50)
                    {
                        paichart.currentText = Translate.Message("L3097");
                        paRisk.color = "red";
                    }
                    else if (paichart.currentValue <= 3.75)
                    {
                        paichart.currentText = Translate.Message("L3098");
                        paRisk.color = "orange";
                    }
                    else
                    {
                        paichart.currentText = Translate.Message("L3099");
                        paRisk.color = "green";
                    }
                    paichart.label = Translate.Message("L3096");
                    paichart.riskName = "PAI";
                    paRisk.riskInfoTextPAI = string.Format(Translate.Message("L3100"), paichart.currentValue.ToString("0.00"), paichart.currentText);
                    paRisk.riskChartPAI = paichart;
                }
            }
            return paRisk;
        }

        public static NutritionRisk GetNutritionRisk(HRADto HRA)
        {
            NutritionRisk nutRisk = new NutritionRisk();
            IList<string> riskText = new List<String>();
            nutRisk.riskIntro = Translate.Message("L1466");
            if (HRA.Interest.NutProg.HasValue)
                nutRisk.riskInfoText = Translate.Message("L4558") + " " + HRA.Interest.NutProg.ToString();
            if (HRA.OtherRiskFactors.LowFatDiet == 2)
            {
                nutRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + Translate.Message("L1467"));
            }
            else if (HRA.OtherRiskFactors.LowFatDiet == 1)
            {
                if (nutRisk.risk != 1)
                    nutRisk.risk = 2;
                riskText.Add("<i class='fa fa-smile-o'></i>" + Translate.Message("L1468"));
            }
            if (HRA.OtherRiskFactors.HealthyCarb == 2)
            {
                nutRisk.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + Translate.Message("L1469"));
            }
            else if (HRA.OtherRiskFactors.HealthyCarb == 1)
            {
                if (nutRisk.risk != 1)
                    nutRisk.risk = 2;
                riskText.Add("<i class='fa fa-smile-o'></i>" + Translate.Message("L1470"));
            }
            nutRisk.riskText = riskText;
            nutRisk.goalText = Translate.Message("L1521");
            return nutRisk;
        }

        public static StressRisk GetStressRisk(HRADto HRA)
        {
            StressRisk stressRisk = new StressRisk();
            if (HRA.Interest.StressManProg.HasValue)
                stressRisk.riskInfoText = Translate.Message("L4558") + " " + HRA.Interest.StressManProg.ToString();
            if (HRA.OtherRiskFactors.FeelStress == 1 || HRA.MedicalCondition.AnxietyMed == 1)
            {
                stressRisk.risk = 1;
                stressRisk.riskText = Translate.Message("L1522") + " ";
            }
            else if (HRA.OtherRiskFactors.FeelStress == 2 && HRA.MedicalCondition.AnxietyMed == 2)
            {
                stressRisk.risk = 2;
                stressRisk.riskText = Translate.Message("L1523") + " ";
            }
            stressRisk.riskText = stressRisk.riskText + Translate.Message("L1524");
            return stressRisk;
        }

        public static SafetyRisks GetSafetyRisk(HRADto HRA)
        {
            SafetyRisks safetyRisks = new SafetyRisks();
            IList<string> riskText = new List<String>();
            safetyRisks.riskIntro = Translate.Message("L1525");
            string text;

            text = Translate.Message("L1471");
            if (HRA.HSP.TextDrive == 1)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.TextDrive == 2)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1472");
            if (HRA.HSP.DUI == 1)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.DUI == 2)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;

            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1473");
            if (HRA.HSP.RideDUI == 1)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.RideDUI == 2)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1474");
            if (HRA.HSP.RideNoBelt == 1)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.RideNoBelt == 2)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1475");
            if (HRA.HSP.Speed10 == 1)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.Speed10 == 2)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1476");
            if (HRA.HSP.BikeNoHelmet == 1)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.BikeNoHelmet == 2)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1477");
            if (HRA.HSP.MBikeNoHelmet == 1)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.MBikeNoHelmet == 2)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1478");
            if (HRA.HSP.SmokeDetect == 1)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else if (HRA.HSP.SmokeDetect == 2)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1479");
            if (HRA.HSP.FireExting == 1)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else if (HRA.HSP.FireExting == 2)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }

            text = Translate.Message("L1480");
            if (HRA.HSP.LiftRight == 2)
            {
                safetyRisks.risk = 1;
                riskText.Add("<i class='fa fa-frown-o'></i>" + text);
            }
            else if (HRA.HSP.LiftRight == 1)
            {
                if (safetyRisks.risk != 1 && safetyRisks.risk != 3)
                    safetyRisks.risk = 2;
            }
            else
            {
                if (safetyRisks.risk != 1)
                    safetyRisks.risk = 3;
                riskText.Add("<i class='fa fa-meh-o'></i>" + text);
            }
            if (safetyRisks.risk == 2)
            {
                riskText.Add("<i class='fa fa-smile-o'></i>" + Translate.Message("L1481"));
            }
            safetyRisks.riskText = riskText;
            return safetyRisks;
        }

        public static TobaccoRisk GetTobaccoRisk(HRADto HRA, bool? riskOnly, HRADto previousHRA)
        {
            TobaccoRisk tobaccoRisk = new TobaccoRisk();
            if (HRA.OtherRiskFactors.SmokeCig == 1 || HRA.OtherRiskFactors.OtherTobacco == 1)
            {
                tobaccoRisk.risk = 1;
                if (HRA.OtherRiskFactors.NoOfCig.HasValue)
                    tobaccoRisk.riskInfoText = "(Cig; " + HRA.OtherRiskFactors.NoOfCig.ToString() + "/d;" + (HRA.Interest.QuitSmokeProg.HasValue ? " " + Translate.Message("L4558") + " " + HRA.Interest.QuitSmokeProg.ToString() + ")" : ")");
            }
            else if (HRA.OtherRiskFactors.SmokeCig == 2 && HRA.OtherRiskFactors.OtherTobacco == 2)
                tobaccoRisk.risk = 2;
            //risk text
            tobaccoRisk.riskText = Translate.Message("L1482");
            //risk chart
            if (riskOnly != true && (HRA.OtherRiskFactors.NoOfCig.HasValue || (previousHRA != null && previousHRA.OtherRiskFactors != null && previousHRA.OtherRiskFactors.NoOfCig.HasValue)))
            {
                RiskChart chart = new RiskChart();
                chart.riskName = "Tobacco";
                chart.label = Translate.Message("L1060") + " " + Translate.Message("L2474");
                chart.currentText = Translate.Message("L1457");
                chart.goalValue1 = 0;
                chart.goalText1 = Translate.Message("L1502");
                if (HRA.OtherRiskFactors.NoOfCig.HasValue)
                {
                    chart.currentValue = (float)HRA.OtherRiskFactors.NoOfCig.Value;
                    if (HRA.OtherRiskFactors.NoOfCig > chart.goalValue1)
                        chart.color1 = Constants.riskColor;
                    else
                        chart.color1 = Constants.noRiskColor;
                    chart.currentYear = DateTime.Parse(HRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(HRA.CompleteDate.ToString()).Year.ToString();
                    tobaccoRisk.riskChart = chart;
                }
                if (previousHRA != null && previousHRA.OtherRiskFactors != null && previousHRA.OtherRiskFactors.NoOfCig.HasValue && previousHRA.CompleteDate.HasValue)
                {
                    chart.startValue = (float)previousHRA.OtherRiskFactors.NoOfCig.Value;
                    chart.startYear = DateTime.Parse(previousHRA.CompleteDate.ToString()).Month.ToString() + "/" + DateTime.Parse(previousHRA.CompleteDate.ToString()).Year.ToString();
                    chart.startText = Translate.Message("L2121");
                    if (chart.startValue > chart.goalValue1)
                        chart.color3 = Constants.riskColor;
                    else
                        chart.color3 = Constants.noRiskColor;
                    tobaccoRisk.riskChart = chart;
                }
            }
            return tobaccoRisk;
        }

        public static int? GetAlcoholRisk(HRADto hra)
        {
            int? risk = null;
            if (hra.OtherRiskFactors.TwoAlcohol == 1)
            {
                risk = 1;
            }
            else if (hra.OtherRiskFactors.TwoAlcohol == 2)
            {
                risk = 2;
            }
            return risk;
        }

        public static int? GetImmunizationRisk(HRADto hra, UserDto user, int age)
        {
            int? risk = null;
            if (hra.Exams.NoShots == 1 || hra.Exams.TetanusShot != 1 ||
                                  (hra.Exams.MMR != 1 && (new DateTime(1957, 1, 1) - Convert.ToDateTime(user.DOB.Value) < TimeSpan.Zero))
                                  || hra.Exams.FluShot != 1 || hra.Exams.Varicella != 1 || (hra.Exams.ShinglesShot != 1 && age >= 60) ||
                                  (hra.Exams.HPVShot != 1 && ((age < 27 && user.Gender == 2) || (age < 22 && user.Gender == 1))) ||
                                  (hra.Exams.PneumoniaShot != 1 && age > 65))
            {
                risk = 1;
            }
            else
            {
                risk = 2;
            }
            return risk;
        }

        public static MedicalConditionsandRiskFactors GetMedicalConditionsandRiskFactors(HRADto hra, int? hraVer, UserDto user, int unit)
        {
            MedicalConditionsandRiskFactors riskFactors = new MedicalConditionsandRiskFactors();
            riskFactors.highRisk = new System.Collections.Generic.List<string>();
            riskFactors.missingRisk = new System.Collections.Generic.List<string>();
            riskFactors.noRisk = new System.Collections.Generic.List<string>();

            var BMI = CommonUtility.GetBMI(hra.HealthNumbers.Height.Value, hra.HealthNumbers.Weight.Value);
            IList<MeasurementsDto> Measurements = CommonUtility.ListMeasurements(unit).Measurements;
            //Known Cardio Vascular Disease            
            string riskText = InterventWebApp.Translate.Message("L293");
            var knwonCVDisease = GetKnownCVDisease(hra);
            if (knwonCVDisease == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (knwonCVDisease == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Family history of cardiovascular disease
            riskText = InterventWebApp.Translate.Message("L294");
            var heartHist = GetFamilyCVRisk(hra);
            if (heartHist == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (heartHist == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            var bpRisk = GetBPRisk(hra, hraVer, true, null, null);
            //Hypertension
            riskText = InterventWebApp.Translate.Message("L295");
            if (bpRisk.risk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (bpRisk.risk == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else if (!bpRisk.risk.HasValue)
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Prehypertension
            riskText = InterventWebApp.Translate.Message("L792");
            if (bpRisk.risk != 1)
            {
                if (bpRisk.preRisk == 1)
                {
                    riskFactors.highRisk.Add(riskText);
                }
                else if (bpRisk.preRisk == 2)
                {
                    riskFactors.noRisk.Add(riskText);
                }
                else if (!bpRisk.preRisk.HasValue)
                {
                    riskFactors.missingRisk.Add(riskText);
                }
            }
            //Abnormal cholesterol or triglycerides
            riskText = InterventWebApp.Translate.Message("L297");
            var ctRisk = GetCTRisk(user, hra, hraVer, true, Measurements, null, unit);
            if (ctRisk.risk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (!ctRisk.risk.HasValue)
            {
                riskFactors.missingRisk.Add(riskText); ;
            }
            else
            {
                riskFactors.noRisk.Add(riskText);
            }
            //Overweight or obesity
            riskText = InterventWebApp.Translate.Message("L298");
            var obRisk = GetOverweightRisk(hra, true, Measurements, null, unit);
            if (obRisk.risk == 1 || obRisk.risk == 2)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (!obRisk.risk.HasValue)
            {
                riskFactors.missingRisk.Add(riskText); ;
            }
            else if (obRisk.risk == 0)
            {
                riskFactors.noRisk.Add(riskText);
            }
            //Diabetes
            riskText = InterventWebApp.Translate.Message("L21");
            var diabRisk = GetDiabetesRisk(user, hra, null, null, unit);
            if (diabRisk.risk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (diabRisk.risk == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            if (diabRisk.risk != 1)
            {
                //Prediabetes
                riskText = InterventWebApp.Translate.Message("L299");
                if (diabRisk.preRisk == 1)
                {
                    riskFactors.highRisk.Add(riskText);
                }
                else if (diabRisk.preRisk == 2)
                {
                    riskFactors.noRisk.Add(riskText);
                }
                else
                {
                    riskFactors.missingRisk.Add(riskText);
                }
            }
            //Other major risk factor(s) for diabetes
            riskText = InterventWebApp.Translate.Message("L300");
            if ((hra.OtherRiskFactors.DiabetesHist == 1) || (user.Gender == 2 && (hra.MedicalCondition.ToldBabyNine == 1 || hra.MedicalCondition.ToldGestDiab == 1 || hra.MedicalCondition.ToldPolycyst == 1)))
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.OtherRiskFactors.DiabetesHist == 2 && ((user.Gender == 2 && (hra.MedicalCondition.ToldBabyNine == 2 && hra.MedicalCondition.ToldGestDiab == 2 || hra.MedicalCondition.ToldPolycyst == 2)) || user.Gender == 1))
            {
                riskFactors.noRisk.Add(riskText);
            }
            //Family history of Diabetes
            riskText = Translate.Message("L1526");
            var diabHist = GetFamilyDiabetesRisk(hra);
            if (diabHist == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (diabHist == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Metabolic syndrome
            riskText = InterventWebApp.Translate.Message("L301");
            if (hra.Goals.MetSyn == 0)
            {
                riskFactors.missingRisk.Add(riskText);
            }
            else if (hra.Goals.MetSyn == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else
            {
                riskFactors.noRisk.Add(riskText);
            }
            //Cancer (personal history)
            riskText = InterventWebApp.Translate.Message("L302");
            var knownCancer = GetKnownCancerRisk(hra);
            if (knownCancer == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (knownCancer == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText); ;
            }
            //Family history of cancer
            riskText = InterventWebApp.Translate.Message("L303");
            var cancerHist = GetFamilyCancerRisk(hra);
            if (cancerHist == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (cancerHist == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Tobacco use
            riskText = InterventWebApp.Translate.Message("L270");
            var tobRisk = GetTobaccoRisk(hra, true, null);
            if (tobRisk.risk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (tobRisk.risk == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Physical inactivity
            riskText = Translate.Message("L608");
            var paRisk = GetPARisk(hra, hraVer, true);
            if (paRisk.risk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (paRisk.risk == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Diet high in “unhealthy” fats and/or cholesterol and/or refined
            riskText = InterventWebApp.Translate.Message("L304");
            var nutRisk = GetNutritionRisk(hra);
            if (nutRisk.risk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (nutRisk.risk == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Excessive stress
            riskText = InterventWebApp.Translate.Message("L305");
            var stressRisk = GetStressRisk(hra);
            if (stressRisk.risk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (stressRisk.risk == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Excessive alcohol consumption
            riskText = InterventWebApp.Translate.Message("L306");
            var alcoholRisk = GetAlcoholRisk(hra);
            if (alcoholRisk == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (alcoholRisk == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Depression, anxiety or other mental health issues
            riskText = InterventWebApp.Translate.Message("L307");
            if (hra.MedicalCondition.AnxietyMed == 1 || hra.MedicalCondition.DepressionMed == 1 || hra.OtherRiskFactors.FeelAnxiety == 1 || hra.OtherRiskFactors.FeelDepression == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.AnxietyMed == 2 && hra.MedicalCondition.DepressionMed == 2 && hra.OtherRiskFactors.FeelAnxiety == 2 && hra.OtherRiskFactors.FeelDepression == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Arthritis
            riskText = InterventWebApp.Translate.Message("L36");
            if (hra.MedicalCondition.ArthritisMed == 1 || (hra.OtherRiskFactors.PhysicalProb == 1 && hra.OtherRiskFactors.Arthritis == 1))
            {
                bool ArthritisText = false;
                if (hra.MedicalCondition.Osteoarthritis.HasValue && hra.MedicalCondition.Osteoarthritis == 1)
                {
                    riskFactors.highRisk.Add(Translate.Message("L2459"));
                    ArthritisText = true;
                }
                if (hra.MedicalCondition.Rheumatoid.HasValue && hra.MedicalCondition.Rheumatoid == 1)
                {
                    riskFactors.highRisk.Add(Translate.Message("L2460"));
                    ArthritisText = true;
                }
                if (hra.MedicalCondition.Psoriatic.HasValue && hra.MedicalCondition.Psoriatic == 1)
                {
                    riskFactors.highRisk.Add(Translate.Message("L2461"));
                    ArthritisText = true;
                }
                if (hra.MedicalCondition.Spondylitis.HasValue && hra.MedicalCondition.Spondylitis == 1)
                {
                    riskFactors.highRisk.Add(Translate.Message("L2154"));
                    ArthritisText = true;
                }
                if (ArthritisText == false)
                    riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.ArthritisMed == 2 && (hra.OtherRiskFactors.PhysicalProb == 2 || hra.OtherRiskFactors.Arthritis == 2))
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Asthma or other breathing 
            riskText = InterventWebApp.Translate.Message("L308");
            if (hra.MedicalCondition.AsthmaMed == 1 || hra.MedicalCondition.ToldAsthma == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.AsthmaMed == 2 && hra.MedicalCondition.ToldAsthma == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Chronic bronchitis or emphysema
            riskText = InterventWebApp.Translate.Message("L309");
            if (hra.MedicalCondition.ToldBronchitis == 1 || hra.MedicalCondition.BronchitisMed == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.ToldBronchitis == 2 && hra.MedicalCondition.BronchitisMed == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Low back pain/back injury
            riskText = InterventWebApp.Translate.Message("L310");
            if (hra.MedicalCondition.BackPainMed == 1 || (hra.OtherRiskFactors.PhysicalProb == 1 && hra.OtherRiskFactors.BackInjury == 1))
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.BackPainMed == 2 && (hra.OtherRiskFactors.PhysicalProb == 2 || hra.OtherRiskFactors.BackInjury == 2))
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Physical limitation that restricts your ability to participate in regular physical activity
            riskText = InterventWebApp.Translate.Message("L311");
            if (hra.OtherRiskFactors.PhysicalProb == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.OtherRiskFactors.PhysicalProb == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Ulcer/heartburn/acid reflux
            riskText = InterventWebApp.Translate.Message("L312");
            if (hra.MedicalCondition.RefluxMed == 1 || hra.MedicalCondition.UlcerMed == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.RefluxMed == 2 && hra.MedicalCondition.UlcerMed == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Osteoporosis
            riskText = InterventWebApp.Translate.Message("L43");
            if (hra.MedicalCondition.OsteoporosisMed == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.OsteoporosisMed == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Sleep apnea or symptoms compatible with a sleep disorder
            riskText = InterventWebApp.Translate.Message("L313");
            if (hra.OtherRiskFactors.SleepApnea == 1 || hra.OtherRiskFactors.FeelTired == 1 || hra.OtherRiskFactors.Snore == 1 || hra.OtherRiskFactors.BreathPause == 1 || hra.OtherRiskFactors.Headache == 1 || hra.OtherRiskFactors.Sleepy == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.OtherRiskFactors.SleepApnea == 2 && hra.OtherRiskFactors.FeelTired == 2 && hra.OtherRiskFactors.Snore == 2 && hra.OtherRiskFactors.BreathPause == 2 && hra.OtherRiskFactors.Headache == 2 && hra.OtherRiskFactors.Sleepy == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Chronic kidney disease
            riskText = InterventWebApp.Translate.Message("L27");
            if (hra.MedicalCondition.ToldKidneyDisease == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.ToldKidneyDisease == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //Inflammatory bowel disease
            riskText = Translate.Message("L2467");
            if (hra.MedicalCondition.Crohns.HasValue && hra.MedicalCondition.Crohns == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            //Psoriasis
            riskText = Translate.Message("L2464");
            if (hra.MedicalCondition.Psoriasis.HasValue && hra.MedicalCondition.Psoriasis == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            //Other chronic medical condition(s)
            riskText = InterventWebApp.Translate.Message("L314");
            if (hra.MedicalCondition.OtherChronic == 1 || hra.MedicalCondition.OtherChronicMed == 1)
            {
                riskFactors.highRisk.Add(riskText);
            }
            else if (hra.MedicalCondition.OtherChronic == 2 && hra.MedicalCondition.OtherChronicMed == 2)
            {
                riskFactors.noRisk.Add(riskText);
            }
            else
            {
                riskFactors.missingRisk.Add(riskText);
            }
            //CAC
            if (hra.HealthNumbers.CAC.HasValue)
            {
                riskText = InterventWebApp.Translate.Message("L2022");
                if (hra.HealthNumbers.CAC >= 0 && hra.HealthNumbers.CAC <= 100)
                {
                    riskFactors.noRisk.Add(riskText);
                }
                else if (hra.HealthNumbers.CAC > 100)
                {
                    riskFactors.highRisk.Add(riskText);
                }
            }
            return riskFactors;
        }

        public static List<MeasurementsandGoals> GetMeasurementsandGoals(HRADto hra, HRAGoalsDto hraGoals, UserDto user, int age, int unit, int? integrationWith)
        {
            var healthNumbers = hra.HealthNumbers;
            var MeasurementList = CommonUtility.ListMeasurements(unit);
            var Measurements = MeasurementList.Measurements;//IList<MeasurementsDto> Measurements
            List<MeasurementsandGoals> measurements = new List<MeasurementsandGoals>();
            MeasurementsandGoals measurement = new MeasurementsandGoals();
            //Weight
            measurement.Name = string.Format(Translate.Message("L318"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
            if (hraGoals.LtWt.HasValue)
                measurement.Goal = Translate.Message("L1527") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(hraGoals.LtWt.Value, BioLookup.Weight, unit), 1));
            if (healthNumbers.Weight > 0)
            {
                //measurement.Measurement = Convert.ToString(Math.Round(healthNumbers.Weight.Value, 1));
                measurement.Measurement = Convert.ToString(Math.Round(CommonUtility.ToMetric(healthNumbers.Weight.Value, BioLookup.Weight, unit), 1));
                measurement.Unit = Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit);
            }
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //Height
            measurement = new MeasurementsandGoals();
            measurement.Name = string.Format(Translate.Message("L319"), Translate.Message(Measurements[BioLookup.Height].MeasurementUnit));
            measurement.Goal = Translate.Message("L1149");
            if (healthNumbers.Height > 0)
            {
                var height = CommonUtility.ToFeetInches(healthNumbers.Height.Value);
                measurement.Measurement = Math.Round(CommonUtility.ToMetric(healthNumbers.Height.Value, BioLookup.Height, unit), 1) + " " + "(" + height.Key + "' " + Math.Round(height.Value, 2) + "'')";
            }
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //Waist
            measurement = new MeasurementsandGoals();
            measurement.Name = string.Format(Translate.Message("L320"), Translate.Message(Measurements[BioLookup.Waist].MeasurementUnit));
            if (user.Gender == 1)
                measurement.Goal = Translate.Message("L1527") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(40, BioLookup.Waist, unit), 1));
            else
                measurement.Goal = Translate.Message("L1527") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(35, BioLookup.Waist, unit), 1)); ;
            if (healthNumbers.Waist > 0)
            {
                measurement.Measurement = Convert.ToString(Math.Round(CommonUtility.ToMetric(healthNumbers.Waist.Value, BioLookup.Waist, unit), 1));
            }
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //BMI
            measurement = new MeasurementsandGoals();
            measurement.Name = Translate.Message("L321");
            measurement.Goal = Translate.Message("L1528") + " 25";
            if (healthNumbers.Height > 0 && healthNumbers.Weight > 0)
            {
                var BMI = CommonUtility.GetBMI(healthNumbers.Height.Value, healthNumbers.Weight.Value);
                measurement.Measurement = Convert.ToString(BMI);
            }
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //SBP
            measurement = new MeasurementsandGoals();
            measurement.Name = Translate.Message("L322");
            measurement.Goal = Translate.Message("L1528") + " " + Convert.ToString(hraGoals.LtSBP);
            if (healthNumbers.SBP > 0)
                measurement.Measurement = Convert.ToString(healthNumbers.SBP);
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //DBP
            measurement = new MeasurementsandGoals();
            measurement.Name = Translate.Message("L323");
            measurement.Goal = Translate.Message("L1528") + " " + Convert.ToString(hraGoals.LtDBP);
            if (healthNumbers.DBP > 0)
                measurement.Measurement = Convert.ToString(healthNumbers.DBP);
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //Cholesterol
            measurement = new MeasurementsandGoals();
            measurement.Name = string.Format(Translate.Message("L324"), Measurements[BioLookup.Cholesterol].MeasurementUnit) + "*";
            measurement.Goal = Translate.Message("L1528") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(200, BioLookup.Cholesterol, unit), 1));
            if (healthNumbers.TotalChol > 0)
                measurement.Measurement = Convert.ToString(Math.Round(CommonUtility.ToMetric(healthNumbers.TotalChol.Value, BioLookup.Cholesterol, unit), 1));
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //Trig
            measurement = new MeasurementsandGoals();
            measurement.Name = string.Format(Translate.Message("L325"), Measurements[BioLookup.Triglycerides].MeasurementUnit) + "*";
            measurement.Goal = Translate.Message("L1528") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(hraGoals.LtTrig.Value, BioLookup.Triglycerides, unit), 1));
            if (healthNumbers.Trig > 0 && healthNumbers.DidYouFast == 1)
                measurement.Measurement = Convert.ToString(Math.Round(CommonUtility.ToMetric(healthNumbers.Trig.Value, BioLookup.Triglycerides, unit), 1));
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //Ldl
            measurement = new MeasurementsandGoals();
            measurement.Name = string.Format(Translate.Message("L326"), Measurements[BioLookup.LDL].MeasurementUnit) + "*";
            measurement.Goal = Translate.Message("L1528") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(hraGoals.LtLdl.Value, BioLookup.LDL, unit), 1));
            if (healthNumbers.LDL > 0 && healthNumbers.DidYouFast == 1)
                measurement.Measurement = Convert.ToString(Math.Round(CommonUtility.ToMetric(healthNumbers.LDL.Value, BioLookup.LDL, unit), 1));
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //Hdl
            measurement = new MeasurementsandGoals();
            measurement.Name = string.Format(Translate.Message("L327"), Measurements[BioLookup.HDL].MeasurementUnit) + "*";
            measurement.Goal = Translate.Message("L1529") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(hraGoals.LtHdl.Value, BioLookup.HDL, unit), 1));
            if (healthNumbers.HDL > 0)
                measurement.Measurement = Convert.ToString(Math.Round(CommonUtility.ToMetric(healthNumbers.HDL.Value, BioLookup.HDL, unit), 1));
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //Glucose
            measurement = new MeasurementsandGoals();
            measurement.Name = string.Format(Translate.Message("L328"), Measurements[BioLookup.Glucose].MeasurementUnit);
            if (hraGoals.Diabetes == true)
                measurement.Goal = Convert.ToString(Math.Round(CommonUtility.ToMetric(hraGoals.LtGluc1.Value, BioLookup.Glucose, unit), 1)) + "-" + Convert.ToString(Math.Round(CommonUtility.ToMetric(hraGoals.LtGluc2.Value, BioLookup.Glucose, unit), 1));
            else
                measurement.Goal = Translate.Message("L1528") + " " + Convert.ToString(Math.Round(CommonUtility.ToMetric(hraGoals.LtGluc1.Value, BioLookup.Glucose, unit), 1));
            if (healthNumbers.Glucose > 0 && healthNumbers.DidYouFast == 1)
                measurement.Measurement = Convert.ToString(Math.Round(CommonUtility.ToMetric(healthNumbers.Glucose.Value, BioLookup.Glucose, unit), 1));
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //A1C-->
            measurement = new MeasurementsandGoals();
            measurement.Name = Translate.Message("L329");
            if (hraGoals.Diabetes == true)
                measurement.Goal = Translate.Message("L1528") + " 7.0";
            else
                measurement.Goal = Translate.Message("L1528") + " 5.7";
            if (healthNumbers.A1C > 0)
                measurement.Measurement = Convert.ToString(Math.Round(healthNumbers.A1C.Value, 1));
            else
                measurement.Measurement = "?";
            measurements.Add(measurement);
            //CHD
            measurement = new MeasurementsandGoals();
            measurement.Name = Translate.Message("L330");
            if (hraGoals.ASCVD == true)
                measurement.Goal = Translate.Message("L1149");
            else
            {
                if (hraGoals.TenYrLow > 0)
                {
                    if (hraGoals.TenYrLow != 1)
                        measurement.Goal = Translate.Message("L1527") + " " + Convert.ToString(Math.Round(hraGoals.TenYrLow.Value * 100, 0));
                    else
                        measurement.Goal = Translate.Message("L1527") + " " + Convert.ToString(Math.Round(hraGoals.TenYrLow.Value, 0));
                }
                else
                {
                    measurement.Goal = Translate.Message("L1149");
                }
            }
            if (hraGoals.ASCVD == true)
                measurement.Measurement = Translate.Message("L1149");
            else
            {
                if (hraGoals.TenYrProb > 0)
                    measurement.Measurement = Convert.ToString(hraGoals.TenYrProb * 100);
                else if (measurement.Goal == Translate.Message("L1149"))
                    measurement.Measurement = Translate.Message("L1149");
                else
                    measurement.Measurement = "?";
            }
            measurements.Add(measurement);
            //10 Year ASCVD
            measurement = new MeasurementsandGoals();
            measurement.Name = Translate.Message("L992");
            if (hraGoals.ASCVD == true)
                measurement.Goal = Translate.Message("L1149");
            else
            {
                if (hraGoals.TenYearASCVDGoal == null)
                {
                    if (age >= 40 && age <= 79 && !user.Race.HasValue)
                        measurement.Goal = "?";
                    else
                        measurement.Goal = Translate.Message("L1149");
                }
                else
                    measurement.Goal = Translate.Message("L1527") + " " + hraGoals.TenYearASCVDGoal.ToString();
            }
            if (hraGoals.ASCVD == true)
                measurement.Measurement = Translate.Message("L1149");
            else
            {
                if (hraGoals.TenYearASCVD == null)
                {
                    if (measurement.Goal == Translate.Message("L1149"))
                        measurement.Measurement = Translate.Message("L1149");
                    else
                        measurement.Measurement = "?";
                }
                else
                    measurement.Measurement = hraGoals.TenYearASCVD.ToString();
            }
            measurements.Add(measurement);
            //Lifetime ASCVD
            measurement = new MeasurementsandGoals();
            measurement.Name = Translate.Message("L993");
            if (hraGoals.ASCVD == true)
                measurement.Goal = Translate.Message("L1149");
            else
            {
                if (hraGoals.LifetimeASCVDGoal == null)
                    measurement.Goal = Translate.Message("L1149");
                else
                    measurement.Goal = Translate.Message("L1527") + " " + hraGoals.LifetimeASCVDGoal.ToString();
            }
            if (hraGoals.ASCVD == true)
                measurement.Measurement = Translate.Message("L1149");
            else
            {
                if (hraGoals.LifetimeASCVD == null)
                {
                    if (measurement.Goal == Translate.Message("L1149"))
                        measurement.Measurement = Translate.Message("L1149");
                    else
                        measurement.Measurement = "?";
                }
                else
                    measurement.Measurement = hraGoals.LifetimeASCVD.ToString();
            }
            measurements.Add(measurement);
            //CAC
            if (healthNumbers.CAC.HasValue)
            {
                measurement = new MeasurementsandGoals();
                measurement.Goal = "0";
                measurement.Name = Translate.Message("L2004");
                measurement.Measurement = healthNumbers.CAC.ToString();
                measurements.Add(measurement);
            }
            //ADA Score
            if (!CommonUtility.IsIntegratedWithLMC(integrationWith.HasValue ? (byte)integrationWith.Value : null))
            {
                var adaRIsk = GetADAScore(hra);
                if (!adaRIsk.isDiaborPreDiab)
                {
                    measurement = new MeasurementsandGoals();
                    measurement.Goal = Translate.Message("L1528") + " 5";
                    measurement.Name = Translate.Message("L3590") + "**";
                    measurement.Measurement = hra.ADAScore?.ToString();
                    measurements.Add(measurement);
                }
            }
            return measurements;
        }

        public static List<CheckupsandGoals> GetCheckupsandGoals(ExamsandShotsDto exams, HealthNumbersDto healthNumbers, UserDto user, int age)
        {
            List<CheckupsandGoals> checkupsandTests = new List<CheckupsandGoals>();
            //Colorectal
            CheckupsandGoals checkupsandTest = new CheckupsandGoals();
            checkupsandTest.Name = Translate.Message("L369");
            checkupsandTest.Recommended = Translate.Message("L370");
            if (exams.ColStoolTest == 1 || exams.SigTest == 1 || exams.StoolTest == 1 || exams.ColTest == 1)
            {
                checkupsandTest.Status = 1;
                checkupsandTest.StatusMessage = Translate.Message("L390");
            }
            else
            {
                if (age >= 45)
                {
                    checkupsandTest.Status = 2;
                    checkupsandTest.StatusMessage = Translate.Message("L391");
                }
                else
                {
                    checkupsandTest.Status = 5;
                    checkupsandTest.StatusMessage = Translate.Message("L371");
                }
            }
            checkupsandTests.Add(checkupsandTest);
            //Pap Test
            if (user.Gender == 2)
            {
                checkupsandTest = new CheckupsandGoals();
                checkupsandTest.Name = Translate.Message("L372");
                checkupsandTest.Recommended = Translate.Message("L373");
                if (exams.PapTest == 1)
                {
                    checkupsandTest.Status = 1;
                    checkupsandTest.StatusMessage = Translate.Message("L390");
                }
                else
                {
                    if (age < 65)
                    {
                        checkupsandTest.Status = 2;
                        checkupsandTest.StatusMessage = Translate.Message("L391");
                    }
                    else
                    {
                        checkupsandTest.Status = 5;
                        checkupsandTest.StatusMessage = Translate.Message("L371");
                    }
                }
                checkupsandTests.Add(checkupsandTest);
            }
            //Mammogram
            if (user.Gender == 2)
            {
                checkupsandTest = new CheckupsandGoals();
                checkupsandTest.Name = Translate.Message("L374");
                checkupsandTest.Recommended = Translate.Message("L375");
                if (exams.Mammogram == 1)
                {
                    checkupsandTest.Status = 1;
                    checkupsandTest.StatusMessage = Translate.Message("L390");
                }
                else
                {
                    if (age >= 40)
                    {
                        checkupsandTest.Status = 2;
                        checkupsandTest.StatusMessage = Translate.Message("L391");
                    }
                    else
                    {
                        checkupsandTest.Status = 5;
                        checkupsandTest.StatusMessage = Translate.Message("L371");
                    }
                }
                checkupsandTests.Add(checkupsandTest);
            }
            //PSA
            if (user.Gender == 1)
            {
                checkupsandTest = new CheckupsandGoals();
                checkupsandTest.Name = Translate.Message("L376");
                checkupsandTest.Recommended = Translate.Message("L377");
                if (exams.PSATest == 1)
                {
                    checkupsandTest.Status = 1;
                    checkupsandTest.StatusMessage = Translate.Message("L390");
                }
                else
                {
                    if (age >= 50)
                    {
                        checkupsandTest.Status = 4;
                        checkupsandTest.StatusMessage = Translate.Message("L393");
                    }
                    else
                    {
                        checkupsandTest.Status = 5;
                        checkupsandTest.StatusMessage = Translate.Message("L371");
                    }
                }
                checkupsandTests.Add(checkupsandTest);
            }
            //BPCheck
            checkupsandTest = new CheckupsandGoals();
            checkupsandTest.Name = Translate.Message("L276");
            checkupsandTest.Recommended = Translate.Message("L379");
            if (exams.BPCheck == 1)
            {
                checkupsandTest.Status = 1;
                checkupsandTest.StatusMessage = Translate.Message("L390");
            }
            else
            {
                checkupsandTest.Status = 2;
                checkupsandTest.StatusMessage = Translate.Message("L391");
            }
            checkupsandTests.Add(checkupsandTest);
            //CholTest
            checkupsandTest = new CheckupsandGoals();
            checkupsandTest.Name = Translate.Message("L380");
            checkupsandTest.Recommended = Translate.Message("L379");
            if (exams.CholTest == 1)
            {
                checkupsandTest.Status = 1;
                checkupsandTest.StatusMessage = Translate.Message("L390");
            }
            else
            {
                checkupsandTest.Status = 2;
                checkupsandTest.StatusMessage = Translate.Message("L391");
            }
            checkupsandTests.Add(checkupsandTest);
            //BMI
            checkupsandTest = new CheckupsandGoals();
            checkupsandTest.Name = Translate.Message("L381");
            checkupsandTest.Recommended = Translate.Message("L379");
            if (healthNumbers.Weight > 0)
            {
                checkupsandTest.Status = 1;
                checkupsandTest.StatusMessage = Translate.Message("L390");
            }
            else
            {
                checkupsandTest.Status = 2;
                checkupsandTest.StatusMessage = Translate.Message("L391");
            }
            checkupsandTests.Add(checkupsandTest);
            //Dental Exam
            checkupsandTest = new CheckupsandGoals();
            checkupsandTest.Name = Translate.Message("L382");
            checkupsandTest.Recommended = Translate.Message("L379");
            if (exams.DentalExam == 1)
            {
                checkupsandTest.Status = 1;
                checkupsandTest.StatusMessage = Translate.Message("L390");
            }
            else
            {
                checkupsandTest.Status = 2;
                checkupsandTest.StatusMessage = Translate.Message("L391");
            }
            checkupsandTests.Add(checkupsandTest);
            //Eye Exam
            checkupsandTest = new CheckupsandGoals();
            checkupsandTest.Name = Translate.Message("L384");
            checkupsandTest.Recommended = Translate.Message("L385");
            if (exams.EyeExam == 1)
            {
                checkupsandTest.Status = 1;
                checkupsandTest.StatusMessage = Translate.Message("L390");
            }
            else if (age < 40)
            {
                checkupsandTest.Status = 5;
                checkupsandTest.StatusMessage = Translate.Message("L371");

            }
            else if (age >= 40)
            {
                checkupsandTest.Status = 2;
                checkupsandTest.StatusMessage = Translate.Message("L391");
            }
            checkupsandTests.Add(checkupsandTest);
            //Hearing Test
            checkupsandTest = new CheckupsandGoals();
            checkupsandTest.Name = Translate.Message("L386");
            checkupsandTest.Recommended = Translate.Message("L387");
            checkupsandTest.Status = 3;
            checkupsandTest.StatusMessage = Translate.Message("L392");
            checkupsandTests.Add(checkupsandTest);
            //PSA
            if (user.Gender == 2)
            {
                checkupsandTest = new CheckupsandGoals();
                checkupsandTest.Name = Translate.Message("L388");
                checkupsandTest.Recommended = Translate.Message("L389");
                if (exams.BoneTest == 1)
                {
                    checkupsandTest.Status = 1;
                    checkupsandTest.StatusMessage = Translate.Message("L390");
                }
                else
                {
                    if (age >= 65)
                    {
                        checkupsandTest.Status = 4;
                        checkupsandTest.StatusMessage = Translate.Message("L393");
                    }
                    else
                    {
                        checkupsandTest.Status = 5;
                        checkupsandTest.StatusMessage = Translate.Message("L371");
                    }
                }
                checkupsandTests.Add(checkupsandTest);
            }
            return checkupsandTests;
        }

        public static List<ImmunizationandGoals> GetImmunizationandGoals(ExamsandShotsDto exams, HealthNumbersDto healthNumbers, UserDto user, int age)
        {
            List<ImmunizationandGoals> immunizationandGoals = new List<ImmunizationandGoals>();
            //TetanusShot
            ImmunizationandGoals immunizationandGoal = new ImmunizationandGoals();
            immunizationandGoal.Name = Translate.Message("L400");
            immunizationandGoal.Recommended = Translate.Message("L401");
            if (exams.TetanusShot == 1)
            {
                immunizationandGoal.Status = 1;
                immunizationandGoal.StatusMessage = Translate.Message("L390");
            }
            else
            {
                immunizationandGoal.Status = 2;
                immunizationandGoal.StatusMessage = Translate.Message("L391");
            }
            immunizationandGoals.Add(immunizationandGoal);

            //PneumoniaShot
            immunizationandGoal = new ImmunizationandGoals();
            immunizationandGoal.Name = Translate.Message("L402");
            immunizationandGoal.Recommended = Translate.Message("L403");
            if (exams.PneumoniaShot == 1)
            {
                immunizationandGoal.Status = 1;
                immunizationandGoal.StatusMessage = Translate.Message("L390");
            }
            else
            {
                if (age >= 65)
                {
                    immunizationandGoal.Status = 2;
                    immunizationandGoal.StatusMessage = Translate.Message("L391");
                }
                else
                {
                    immunizationandGoal.Status = 5;
                    immunizationandGoal.StatusMessage = Translate.Message("L371");
                }
            }
            immunizationandGoals.Add(immunizationandGoal);
            //FluShot
            immunizationandGoal = new ImmunizationandGoals();
            immunizationandGoal.Name = Translate.Message("L404");
            immunizationandGoal.Recommended = Translate.Message("L405");
            if (exams.FluShot == 1)
            {
                immunizationandGoal.Status = 1;
                immunizationandGoal.StatusMessage = Translate.Message("L390");
            }
            else
            {
                immunizationandGoal.Status = 2;
                immunizationandGoal.StatusMessage = Translate.Message("L391");
            }
            immunizationandGoals.Add(immunizationandGoal);
            //MMR
            immunizationandGoal = new ImmunizationandGoals();
            immunizationandGoal.Name = Translate.Message("L406");
            immunizationandGoal.Recommended = Translate.Message("L407");
            if (exams.MMR == 1)
            {
                immunizationandGoal.Status = 1;
                immunizationandGoal.StatusMessage = Translate.Message("L390");
            }
            else if (new DateTime(1957, 1, 1) - Convert.ToDateTime(user.DOB) >= TimeSpan.Zero)
            {
                immunizationandGoal.Status = 5;
                immunizationandGoal.StatusMessage = Translate.Message("L371");
            }
            else
            {
                immunizationandGoal.Status = 2;
                immunizationandGoal.StatusMessage = Translate.Message("L391");
            }
            immunizationandGoals.Add(immunizationandGoal);
            //Varicella
            immunizationandGoal = new ImmunizationandGoals();
            immunizationandGoal.Name = Translate.Message("L408");
            immunizationandGoal.Recommended = Translate.Message("L409");
            if (exams.Varicella == 1)
            {
                immunizationandGoal.Status = 1;
                immunizationandGoal.StatusMessage = Translate.Message("L390");
            }
            else if (exams.Varicella == 2)
            {
                immunizationandGoal.Status = 2;
                immunizationandGoal.StatusMessage = Translate.Message("L391");
            }
            else
            {
                immunizationandGoal.Status = 3;
                immunizationandGoal.StatusMessage = Translate.Message("L392");
            }
            immunizationandGoals.Add(immunizationandGoal);
            //ShinglesShot
            immunizationandGoal = new ImmunizationandGoals();
            immunizationandGoal.Name = Translate.Message("L410");
            immunizationandGoal.Recommended = Translate.Message("L411");
            if (exams.ShinglesShot == 1)
            {
                immunizationandGoal.Status = 1;
                immunizationandGoal.StatusMessage = Translate.Message("L390");
            }
            else
            {
                if (age >= 50)
                {
                    if (exams.ShinglesShot == 2)
                    {
                        immunizationandGoal.Status = 2;
                        immunizationandGoal.StatusMessage = Translate.Message("L391");
                    }
                    else
                    {
                        immunizationandGoal.Status = 3;
                        immunizationandGoal.StatusMessage = Translate.Message("L392");
                    }
                }
                else
                {
                    immunizationandGoal.Status = 5;
                    immunizationandGoal.StatusMessage = Translate.Message("L371");
                }
            }
            immunizationandGoals.Add(immunizationandGoal);
            //HPVShot
            immunizationandGoal = new ImmunizationandGoals();
            immunizationandGoal.Name = Translate.Message("L412");
            immunizationandGoal.Recommended = Translate.Message("L413");
            if (exams.HPVShot == 1)
            {
                immunizationandGoal.Status = 1;
                immunizationandGoal.StatusMessage = Translate.Message("L390");
            }
            else
            {
                if ((age < 27 && user.Gender == 2) || (age < 22 && user.Gender == 1))
                {
                    if (exams.HPVShot == 2)
                    {
                        immunizationandGoal.Status = 2;
                        immunizationandGoal.StatusMessage = Translate.Message("L391");
                    }
                    else
                    {
                        immunizationandGoal.Status = 3;
                        immunizationandGoal.StatusMessage = Translate.Message("L392");
                    }
                }
                else
                {
                    immunizationandGoal.Status = 5;
                    immunizationandGoal.StatusMessage = Translate.Message("L371");
                }
            }
            immunizationandGoals.Add(immunizationandGoal);

            return immunizationandGoals;
        }

        public static NutritionGoalModel NutritionGoal(HRAGoalsDto Goals, UsersinProgramDto program, int? programType, int? integrationWith, int? gender, bool showSelfScheduling)
        {
            NutritionGoalModel model = new NutritionGoalModel();
            model.nutGoalIntro = Translate.Message("L1530") + " "
            + Translate.Message("L1531") + " "
            + Translate.Message("L1532");
            //meal plan
            model.NutMealPlan = Goals.NutMealPlan;
            model.NutMaint = Goals.NutMaint;
            model.NutMaintMealPlan = Goals.NutMaintMealPlan;
            model.NutLowBMI = Goals.NutLowBMI;
            model.NutLowBMIMealPlan = Goals.NutLowBMIMealPlan;
            model.FirstNutPlan = model.NutMealPlan;
            if (Goals.NutLowBMI == true)
            {
                if (model.FirstNutPlan == null)
                    model.FirstNutPlan = model.NutLowBMIMealPlan;
                else
                    model.SecondNutPlan = model.NutLowBMIMealPlan;
            }
            else if (Goals.NutMaint == true)
            {
                if (model.FirstNutPlan == null)
                    model.FirstNutPlan = model.NutMaintMealPlan;
                else
                    model.SecondNutPlan = model.NutMaintMealPlan;
            }
            if (model.FirstNutPlan != null)
                model.FirstNutPlanArray = CommonUtility.GetNutPlan(model.FirstNutPlan);
            if (model.SecondNutPlan != null)
                model.SecondNutPlanArray = CommonUtility.GetNutPlan(model.SecondNutPlan);
            model.meatTitle = CommonUtility.IsIntegratedWithLMC(integrationWith.HasValue ? (byte)integrationWith.Value : null) ? Translate.Message("L3887") : Translate.Message("L1651");
            model.milkTitle = CommonUtility.IsIntegratedWithLMC(integrationWith.HasValue ? (byte)integrationWith.Value : null) ? Translate.Message("L3884") : Translate.Message("L1652");
            model.fruitTitle = CommonUtility.IsIntegratedWithLMC(integrationWith.HasValue ? (byte)integrationWith.Value : null) ? Translate.Message("L3876") : Translate.Message("L1653");
            model.nonstarchTitle = CommonUtility.IsIntegratedWithLMC(integrationWith.HasValue ? (byte)integrationWith.Value : null) ? Translate.Message("L3886") : Translate.Message("L1654");
            model.startchTitle = CommonUtility.IsIntegratedWithLMC(integrationWith.HasValue ? (byte)integrationWith.Value : null) ? Translate.Message("L3888") : Translate.Message("L1655");
            model.grainTitle = CommonUtility.IsIntegratedWithLMC(integrationWith.HasValue ? (byte)integrationWith.Value : null) ? Translate.Message("L3885") : Translate.Message("L1656");
            //fat
            model.NutFatNum = Goals.NutFatNum;
            model.NewNutFatNum = Convert.ToInt16(Goals.NutStcal * 0.35 / 9);
            model.NutLtFat = Goals.NutLtFat;
            model.NutLtFatNum = Goals.NutLtFatNum;
            model.NewNutLtFatNum = Convert.ToInt16(Goals.NutLtcal * 0.35 / 9);
            //underweight
            model.NutUnderWeight = Goals.NutUnderWeight;
            //saturated fat
            model.NutSatFatNum = Goals.NutSatFatNum;
            model.NutLtSatFat = Goals.NutLtSatFat;
            model.NutLtSatFatNum = Goals.NutLtSatFatNum;
            //trans fat
            model.NutTransFatNum = Translate.Message("L1533");
            //cholesterol
            model.NutCholNum = Translate.Message("L1533");
            //water
            model.NutDrinkWater = Goals.NutDrinkWater;
            model.NutDrinkWaterNum = Goals.NutDrinkWaterNum;
            //sugar
            model.NutSugNum = Goals.NutSugNum;
            //carb
            model.NutCarbFrom = Convert.ToInt16(Goals.NutStcal * 0.45 / 4);
            model.NutCarbTo = Convert.ToInt16(Goals.NutStcal * 0.65 / 4);
            model.NutLtCarbFrom = Convert.ToInt16(Goals.NutLtcal * 0.45 / 4);
            model.NutLtCarbTo = Convert.ToInt16(Goals.NutLtcal * 0.65 / 4);
            List<String> nutRecommendations = new List<String>();
            if (program == null)
            {
                if (programType.HasValue && programType.Value == 2)
                {
                    nutRecommendations.Add("<li>" + Translate.Message("L1534") + "</li>");
                }
                else if (showSelfScheduling)
                {
                    nutRecommendations.Add("<li>" + string.Format(Translate.Message("L236"), "href='../../Participant/MyCoach'") + "</li>");
                }
            }
            else if (program.Id > 0 && program.ProgramsinPortal.program.ProgramType == 2)
            {
                nutRecommendations.Add("<li>" + Translate.Message("L1534") + "</li>");
            }
            nutRecommendations.Add("<li>" + Translate.Message("L1536") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1537") + "<ul>");
            nutRecommendations.Add("<li>" + Translate.Message("L1538") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1539") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1540") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1541") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1542") + "</li>");
            if (gender == 1)
            {
                nutRecommendations.Add("<li>" + Translate.Message("L1543") + "</li>");
            }
            else
            {
                nutRecommendations.Add("<li>" + Translate.Message("L1544") + "</li>");
            }
            nutRecommendations.Add("</ul></li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1545") + "<ul>");
            nutRecommendations.Add("<li>" + Translate.Message("L1546") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1547") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1548") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1549") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1550") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1551") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1080") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1081") + "</li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1082") + "</li>");
            nutRecommendations.Add("</ul></li>");
            nutRecommendations.Add("<li>" + Translate.Message("L1552") + "</li>");
            model.nutRecommendations = nutRecommendations;
            return model;
        }

        public static PhysicalActivityGoalModel PhysicalActivityGoal(HRADto hra, UsersinProgramDto program, int? programType)
        {
            PhysicalActivityGoalModel model = new PhysicalActivityGoalModel();
            List<String> exGuidelines = new List<String>();
            model.ExerPlan = hra.Goals.ExerPlan;
            if (model.ExerPlan != null)
                model.ExerPlanArray = CommonUtility.GetExercisePlan(hra.Id);
            if (program == null)
            {
                if (programType.HasValue && programType.Value == 2)
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1534") + "</li>");
                }
                else
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1553") + "</li>");
                }
            }
            else if (program.Id > 0 && program.ProgramsinPortal.program.ProgramType == 2)
            {
                exGuidelines.Add("<li>" + Translate.Message("L1534") + "</li>");
            }
            exGuidelines.Add("<li>Consider " + Translate.Message("L448") + ", " + Translate.Message("L449") + " or " + Translate.Message("L450") + "</li>");
            exGuidelines.Add("<li>" + Translate.Message("L1554") + "</li>");
            exGuidelines.Add("<li>" + Translate.Message("L1555") + "</li>");
            exGuidelines.Add("<li>" + Translate.Message("L563") + ":" + " <a href='#' style='color:deepskyblue' data-open='rpe'>" + Translate.Message("L1556") + " </li>");
            if (hra != null && hra.HealthNumbers != null && hra.HealthNumbers.THRFrom.HasValue && hra.HealthNumbers.THRTo.HasValue)
            {
                exGuidelines.Add("<li>" + String.Format(Translate.Message("L2424"), hra.HealthNumbers.THRFrom, hra.HealthNumbers.THRTo) + "</li>");
            }
            exGuidelines.Add("<li>" + Translate.Message("L1558") + "</li>");
            exGuidelines.Add("<li>" + Translate.Message("L1559") + "</li>");
            exGuidelines.Add("<li>" + Translate.Message("L1560") + "</li>");
            exGuidelines.Add("<li>" + Translate.Message("L1561") + "</li>");
            if (model.ExerPlan != null)
            {
                exGuidelines.Add("<li>" + Translate.Message("L1562") + "</li>");
            }
            exGuidelines.Add("<li>" + Translate.Message("L1563") + "</li>");
            if (program == null)
            {
                if (programType.HasValue && programType.Value == 2)
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1564") + "</li>");
                    exGuidelines.Add("<li>" + Translate.Message("L1565") + "</li>");
                    exGuidelines.Add("<li>" + Translate.Message("L1566") + "</li>");
                }
                else
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1567") + "</li>");
                }
            }
            else
            {
                if (program.Id > 0 && program.ProgramsinPortal.program.ProgramType == 2)
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1564") + "</li>");
                    exGuidelines.Add("<li>" + Translate.Message("L1565") + "</li>");
                    exGuidelines.Add("<li>" + Translate.Message("L1566") + "</li>");
                }
                else
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1567") + "</li>");
                }
            }

            exGuidelines.Add("<li>" + Translate.Message("L1568") + "</li>");
            exGuidelines.Add("<li>" + Translate.Message("L1569") + "</li>");
            if (program == null)
            {
                if (programType.HasValue && programType.Value == 2)
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1570") + "</li>");
                }
            }
            else
            {
                if (program.Id > 0 && program.ProgramsinPortal.program.ProgramType == 2)
                {
                    exGuidelines.Add("<li>" + Translate.Message("L1570") + "</li>");
                }
            }
            model.exGuidelines = exGuidelines;
            return model;
        }

        public static DrReferralModel DrReferral(HRAGoalsDto Goals, int? hraVer, double? CAC)
        {
            DrReferralModel model = new DrReferralModel();
            List<String> referrals = new List<String>();
            if (hraVer == (byte)HRAVersions.CaptivaVersion)
            {
                referrals.Add(Translate.Message("L4301"));
                referrals.Add(Translate.Message("L4302"));
            }
            else
            {
                if (Goals.DrClearRef1 == true)
                {
                    referrals.Add(Translate.Message("L333"));
                }
                if (Goals.DrClearRef2 == true)
                {
                    referrals.Add(Translate.Message("L334"));
                }
                if (Goals.BloodTestRef == true)
                {
                    referrals.Add(Translate.Message("L335"));
                }
                string BPReferrals = "";
                if (Goals.MedRef1 == true)
                {
                    BPReferrals = Translate.Message("L336");
                }
                if (Goals.MedRef2 == true)
                {
                    BPReferrals = Translate.Message("L337");
                }
                if (!string.IsNullOrEmpty(BPReferrals))
                {
                    if (Goals.HypertensiveBPRef == true)
                        referrals.Add(BPReferrals + " " + Translate.Message("L2652"));
                    else
                        referrals.Add(BPReferrals);
                }
                if (Goals.ASCVDRef == null)
                {
                    if (Goals.LdlRef1 == true)
                    {
                        referrals.Add(Translate.Message("L338"));
                    }
                    if (Goals.LdlRef2 == true)
                    {
                        referrals.Add(Translate.Message("L339"));
                    }
                }
                if (Goals.TrigRef1 == true)
                {
                    referrals.Add(Translate.Message("L340"));
                }
                if (Goals.HdlRef1 == true)
                {
                    referrals.Add(Translate.Message("L341"));
                }
                if (Goals.ASCVDRef == null)
                {
                    if (Goals.HdlRef2 == true)
                    {
                        referrals.Add(Translate.Message("L342"));
                    }
                }
                if (Goals.TrigRef2 == true)
                {
                    referrals.Add(Translate.Message("L343"));
                }
                if (Goals.LdlRef3 == true)
                {
                    referrals.Add(Translate.Message("L344"));
                }
                if (Goals.ASCVDRef == null)
                {
                    if (Goals.LdlRef4 == true)
                    {
                        referrals.Add(string.Format(Translate.Message("L345"), Goals.LdlRef4Num));
                    }
                }
                if (Goals.ASCVDRef == null)
                {
                    if (Goals.LdlRef5 == true)
                    {
                        referrals.Add(Translate.Message("L346"));
                    }
                    if (Goals.LdlRef6 == true || Goals.LdlRef7 == true)
                    {
                        referrals.Add(Translate.Message("L347"));
                    }
                }
                if (Goals.TrigRef3 == true)
                {
                    referrals.Add(Translate.Message("L349"));
                }
                if (Goals.TrigRef4 == true)
                {
                    referrals.Add(Translate.Message("L350"));
                }
                if (Goals.MedRef3 == true)
                {
                    referrals.Add(Translate.Message("L351"));
                }
                if (Goals.CholRef1 == true)
                {
                    referrals.Add(Translate.Message("L352"));
                }
                if (Goals.ASCVDRef == null)
                {
                    if (Goals.CholRef2 == true)
                    {
                        referrals.Add(Translate.Message("L353"));
                    }
                }
                if (Goals.TrigRef5 == true)
                {
                    referrals.Add(Translate.Message("L354"));
                }
                if (Goals.GlucRef1 == true)
                {
                    referrals.Add(Translate.Message("L355"));
                }
                if (Goals.GlucRef2 == true)
                {
                    referrals.Add(Translate.Message("L356"));
                }
                if (Goals.GlucRef3 == true)
                {
                    referrals.Add(Translate.Message("L357"));
                }
                if (Goals.A1CRef1 == true)
                {
                    referrals.Add(Translate.Message("L358"));
                }
                if (Goals.A1CRef2 == true)
                {
                    referrals.Add(Translate.Message("L359"));
                }
                if (Goals.AspRef == true)
                {
                    referrals.Add(Translate.Message("L360"));
                }
                if (Goals.NicRef == true)
                {
                    referrals.Add(Translate.Message("L361"));
                }
                if (Goals.BPRef == true)
                {
                    referrals.Add(Translate.Message("L362"));
                }
                if (Goals.RMRRef == true)
                {
                    referrals.Add(Translate.Message("L363"));
                }
                if (Goals.ASCVDRef != null)
                //some thing missed here
                {
                    referrals.Add(Translate.Message("L" + Goals.ASCVDRef));
                }
                if (CAC.HasValue)
                {
                    referrals.Add(Translate.Message("L2023"));
                }
            }
            model.referrals = referrals;
            return model;
        }

        public static List<String> GetPCPAlerts(UserDto user, HRADto hra)
        {
            List<String> pcpAlerts = new List<String>();
            if (hra.Goals.MedRef1 == true || hra.Goals.MedRef2 == true || hra.Goals.LdlRef1 == true || hra.Goals.LdlRef2 == true || hra.Goals.TrigRef1 == true ||
                hra.Goals.HdlRef1 == true || hra.Goals.HdlRef2 == true || hra.Goals.TrigRef2 == true || hra.Goals.LdlRef3 == true || hra.Goals.MedRef3 == true ||
                 hra.Goals.GlucRef3 == true || hra.Goals.A1CRef2 == true || hra.Goals.NicRef == true || (hra.Goals.ASCVDRef.HasValue && hra.MedicalCondition.HighCholMed != 1))
            {
                pcpAlerts.Add(Translate.Message("L1571"));
            }
            return pcpAlerts;
        }

        public static RiskChartsModel GetRiskCharts(string chartCode, HRADto hra, int hraVer, UserDto user, int unit)
        {
            RiskChartsModel model = new RiskChartsModel();
            List<RiskChart> charts = new List<RiskChart>();
            if (chartCode == "HS")
            {
                var hdsRisk = GetHDSRisk(user, hra, null);
                charts.Add(hdsRisk.chdRiskChart);
                charts.Add(hdsRisk.tenYearRiskChart);
                charts.Add(hdsRisk.lifetimeRiskChart);
                model.riskCharts = charts;
                model.reportTitle = "Heart disease & stroke";
            }
            else if (chartCode == "BP")
            {
                var bpRisk = GetBPRisk(hra, hraVer, null, true, null);
                charts.Add(bpRisk.sbpRiskChart);
                charts.Add(bpRisk.dbpRiskChart);
                model.riskCharts = charts;
                model.reportTitle = "Blood pressure";
            }
            else if (chartCode == "CT")
            {
                IList<MeasurementsDto> Measurements = CommonUtility.ListMeasurements(unit).Measurements;
                var ctRisk = GetCTRisk(user, hra, hraVer, null, Measurements, null, unit);
                charts.Add(ctRisk.hdlRiskChart);
                charts.Add(ctRisk.ldlRiskChart);
                charts.Add(ctRisk.trigRiskChart);
                charts.Add(ctRisk.tcRiskChart);
                model.riskCharts = charts;
                model.reportTitle = "Cholesterol & triglycerides";
            }
            else if (chartCode == "OB")
            {
                IList<MeasurementsDto> Measurements = CommonUtility.ListMeasurements(unit).Measurements;
                var obRisk = GetOverweightRisk(hra, null, Measurements, null, unit);
                charts.Add(obRisk.riskChart);
                model.riskCharts = charts;
                model.reportTitle = "Overweight";
            }
            else if (chartCode == "MS")
            {
                var obRisk = GetMetRisk(hra);
                model.reportTitle = "Metabolic Syndrome";
            }
            else if (chartCode == "DB")
            {
                var dbRisk = GetDiabetesRisk(user, hra, true, null, unit);
                charts.Add(dbRisk.glucChart);
                charts.Add(dbRisk.a1cChart);
                model.riskCharts = charts;
                model.reportTitle = "Diabetes";
            }
            else if (chartCode == "CA")
            {
                model.reportTitle = "Cancer";
            }
            else if (chartCode == "PA")
            {
                var paRisk = GetPARisk(hra, hraVer, null);
                charts.Add(paRisk.riskChart);
                model.riskCharts = charts;
                model.reportTitle = "Physical Activity";
            }
            else if (chartCode == "NU")
            {
                model.reportTitle = "Nutrition";
            }
            else if (chartCode == "ST")
            {
                model.reportTitle = "Stress";
            }
            else if (chartCode == "SA")
            {
                model.reportTitle = "Safety";
            }
            else if (chartCode == "TO")
            {
                var tobRisk = GetTobaccoRisk(hra, null, null);
                charts.Add(tobRisk.riskChart);
                model.riskCharts = charts;
                model.reportTitle = "Tobacco";
            }
            return model;
        }

        public static List<String> GetTobaccoGoalContent(ReadHRAResponse hraReport)
        {
            List<String> tobaccoGoal = new List<String>();
            if (hraReport.hra.OtherRiskFactors.SmokeCig == 1 || hraReport.hra.OtherRiskFactors.OtherTobacco == 1)
            {
                //need to check (Model.programType != 1)
                if (1 != 1)
                {
                    tobaccoGoal.Add("<li>" + Translate.Message("L4560") + "</li>");
                    if (hraReport.hra.Interest.QuitSmokeProg >= 9)
                    {
                        tobaccoGoal.Add("<li>" + Translate.Message("L1572") + "</li>");
                    }
                    else
                    {
                        tobaccoGoal.Add("<li>" + Translate.Message("L1573") + "</li>");
                    }
                    tobaccoGoal.Add("<li>" + Translate.Message("L1574") + "</li>");
                }
                else
                {
                    tobaccoGoal.Add("<li>" + Translate.Message("L1575"));
                    tobaccoGoal.Add("<ul><li>" + Translate.Message("L1576") + "</li>");
                    tobaccoGoal.Add("<li>" + Translate.Message("L1577") + "</li>");
                    if (hraReport.hra.Interest.QuitSmokeProg >= 9)
                    {
                        tobaccoGoal.Add("<li>" + Translate.Message("L1572") + "</li>");
                    }
                    else
                    {
                        tobaccoGoal.Add("<li>" + Translate.Message("L1573") + "</li>");
                    }
                    tobaccoGoal.Add("</ul></li>");
                }
                if (hraReport.hra.OtherRiskFactors.SmokeCig == 1 && hraReport.hra.OtherRiskFactors.NoOfCig > 9)
                {
                    tobaccoGoal.Add("<li>" + Translate.Message("L1579") + "</li>");
                    tobaccoGoal.Add("<li>" + Translate.Message("L1580") + "</li>");
                }
            }
            else
            {
                tobaccoGoal.Add("<li>" + Translate.Message("L1581") + "</li>");
                tobaccoGoal.Add("<li>" + Translate.Message("L1330") + "</li>");
            }
            return tobaccoGoal;
        }

        public static List<String> GetStressGoalContent(UsersinProgramDto program, int? programType)
        {
            List<String> stressGoal = new List<String>();
            if (program == null)
            {
                if (programType.HasValue && programType.Value == 2)
                {
                    stressGoal.Add("<li>" + Translate.Message("L1582"));
                    stressGoal.Add("<ul><li>" + Translate.Message("L1583") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1584") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1585") + "</li>");
                    stressGoal.Add("</ul></li>");
                }
                else
                {
                    stressGoal.Add("<li>" + Translate.Message("L1587") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1583") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1584") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1586") + "</li>");
                }
            }
            else
            {
                if (program.Id > 0 && program.ProgramsinPortal.program.ProgramType == 2)
                {
                    stressGoal.Add("<li>" + Translate.Message("L1582"));
                    stressGoal.Add("<ul><li>" + Translate.Message("L1583") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1584") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1585") + "</li>");
                    stressGoal.Add("</ul></li>");
                }
                else
                {
                    stressGoal.Add("<li>" + Translate.Message("L1587") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1583") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1584") + "</li>");
                    stressGoal.Add("<li>" + Translate.Message("L1586") + "</li>");
                }
            }
            return stressGoal;
        }

        public static IList<string> GetMedicationGoalContent(UsersinProgramDto program, int? programType)
        {
            List<String> medicationGoal = new List<String>();
            medicationGoal.Add("<li>" + Translate.Message("L1588") + "</li>");
            medicationGoal.Add("<li>" + Translate.Message("L584") + "</li>");
            if (program == null)
            {
                if (programType.HasValue && programType.Value == 2)
                {
                    medicationGoal.Add("<li>" + Translate.Message("L1589") + "</li>");
                }
            }
            else
            {
                if (program.Id > 0 && program.ProgramsinPortal.program.ProgramType == 2)
                {
                    medicationGoal.Add("<li>" + Translate.Message("L1589") + "</li>");
                }
            }
            medicationGoal.Add("<li>" + Translate.Message("L1590") + "</li>");
            return medicationGoal;
        }

        public static IList<string> GetSupplementGoalContent()
        {
            List<String> supplementGoal = new List<String>();
            supplementGoal.Add("<li>" + Translate.Message("L1591") + "</li>");
            return supplementGoal;
        }

        public static ScorecardModel GetScorecardItems(UserDto user, HRADto hra, int? hraVer, int unit)
        {
            var BMI = CommonUtility.GetBMI(hra.HealthNumbers.Height.Value, hra.HealthNumbers.Weight.Value);
            int age = hra.Age.Value;
            ScorecardModel model = new ScorecardModel();

            #region Personal History

            IList<ScoreCardItem> PHistoryItems = new List<ScoreCardItem>();

            //Cardiovascular disease
            ScoreCardItem PHistoryItem = new ScoreCardItem();
            PHistoryItem.Name = Translate.Message("L266");
            var cvDisease = GetKnownCVDisease(hra);
            if (cvDisease == 1)
            {
                PHistoryItem.riskClass = "at-risk";
                PHistoryItem.riskText = "L265";
            }
            else if (cvDisease == 2)
            {
                PHistoryItem.riskClass = "not-at-risk";
                PHistoryItem.riskText = "L264";
            }
            else
            {
                PHistoryItem.riskClass = "cannot-calculate";
                PHistoryItem.riskText = Translate.Message("L211");
            }
            PHistoryItems.Add(PHistoryItem);

            //High blood pressure
            PHistoryItem = new ScoreCardItem();
            PHistoryItem.Name = Translate.Message("L19");
            var knownBP = GetKnownBPRisk(hra);
            if (knownBP == 1)
            {
                PHistoryItem.riskClass = "at-risk";
                PHistoryItem.riskText = "L265";
            }
            else if (knownBP == 2)
            {
                PHistoryItem.riskClass = "not-at-risk";
                PHistoryItem.riskText = "L264";
            }
            else
            {
                PHistoryItem.riskClass = "cannot-calculate";
                PHistoryItem.riskText = Translate.Message("L211");
            }
            PHistoryItems.Add(PHistoryItem);

            //Abnormal cholesterol or triglycerides
            PHistoryItem = new ScoreCardItem();
            PHistoryItem.Name = Translate.Message("L267");
            var knownCTRisk = GetKnownCTRisk(hra);
            if (knownCTRisk == 1)
            {
                PHistoryItem.riskClass = "at-risk";
                PHistoryItem.riskText = "L265";
            }
            else if (knownCTRisk == 2)
            {
                PHistoryItem.riskClass = "not-at-risk";
                PHistoryItem.riskText = "L264";
            }
            else
            {
                PHistoryItem.riskClass = "cannot-calculate";
                PHistoryItem.riskText = Translate.Message("L211");
            }
            PHistoryItems.Add(PHistoryItem);

            //Diabetes
            PHistoryItem = new ScoreCardItem();
            PHistoryItem.Name = Translate.Message("L21");
            var knownDiabetesRisk = GetKnownDiabetesRisk(hra, user);
            if (knownDiabetesRisk == 1)
            {
                PHistoryItem.riskClass = "at-risk";
                PHistoryItem.riskText = "L265";
            }
            else if (knownDiabetesRisk == 2)
            {
                PHistoryItem.riskClass = "not-at-risk";
                PHistoryItem.riskText = "L264";
            }
            else
            {
                PHistoryItem.riskClass = "cannot-calculate";
                PHistoryItem.riskText = Translate.Message("L211");
            }
            PHistoryItems.Add(PHistoryItem);

            //Cancer
            PHistoryItem = new ScoreCardItem();
            PHistoryItem.Name = Translate.Message("L26");
            var personalCancerRisk = GetKnownCancerRisk(hra);
            if (personalCancerRisk == 1)
            {
                PHistoryItem.riskClass = "at-risk";
                PHistoryItem.riskText = "L265";
            }
            else if (personalCancerRisk == 2)
            {
                PHistoryItem.riskClass = "not-at-risk";
                PHistoryItem.riskText = "L264";
            }
            else
            {
                PHistoryItem.riskClass = "cannot-calculate";
                PHistoryItem.riskText = Translate.Message("L211");
            }
            PHistoryItems.Add(PHistoryItem);

            //Breathing disorders
            PHistoryItem = new ScoreCardItem();
            PHistoryItem.Name = Translate.Message("L268");
            var knownBreathingDisorders = GetKnownBreathingDisorders(hra);
            if (knownBreathingDisorders == 1)
            {
                PHistoryItem.riskClass = "at-risk";
                PHistoryItem.riskText = "L265";
            }
            else if (knownBreathingDisorders == 2)
            {
                PHistoryItem.riskClass = "not-at-risk";
                PHistoryItem.riskText = "L264";
            }
            else
            {
                PHistoryItem.riskClass = "cannot-calculate";
                PHistoryItem.riskText = Translate.Message("L211");
            }
            PHistoryItems.Add(PHistoryItem);

            #endregion

            #region Family History

            IList<ScoreCardItem> FHistoryItems = new List<ScoreCardItem>();
            //Cardiovascular disease at an early age
            ScoreCardItem FHistoryItem = new ScoreCardItem();
            FHistoryItem.Name = Translate.Message("L269");
            var cvHist = GetFamilyCVRisk(hra);
            if (cvHist == 1)
            {
                FHistoryItem.riskClass = "at-risk";
                FHistoryItem.riskText = "L265";
            }
            else if (cvHist == 2)
            {
                FHistoryItem.riskClass = "not-at-risk";
                FHistoryItem.riskText = "L264";
            }
            else
            {
                FHistoryItem.riskClass = "cannot-calculate";
                FHistoryItem.riskText = Translate.Message("L211");
            }
            FHistoryItems.Add(FHistoryItem);
            //Diabetes
            FHistoryItem = new ScoreCardItem();
            FHistoryItem.Name = Translate.Message("L610");
            var diabHist = GetFamilyDiabetesRisk(hra);
            if (diabHist == 1)
            {
                FHistoryItem.riskClass = "at-risk";
                FHistoryItem.riskText = "L265";
            }
            else if (diabHist == 2)
            {
                FHistoryItem.riskClass = "not-at-risk";
                FHistoryItem.riskText = "L264";
            }
            else
            {
                FHistoryItem.riskClass = "cannot-calculate";
                FHistoryItem.riskText = Translate.Message("L211");
            }
            FHistoryItems.Add(FHistoryItem);
            //Cancer
            FHistoryItem = new ScoreCardItem();
            FHistoryItem.Name = Translate.Message("L26");
            var cancerHist = GetFamilyCancerRisk(hra);
            if (cancerHist == 1)
            {
                FHistoryItem.riskClass = "at-risk";
                FHistoryItem.riskText = "L265";
            }
            else if (cancerHist == 2)
            {
                FHistoryItem.riskClass = "not-at-risk";
                FHistoryItem.riskText = "L264";
            }
            else
            {
                FHistoryItem.riskClass = "cannot-calculate";
                FHistoryItem.riskText = Translate.Message("L211");
            }
            FHistoryItems.Add(FHistoryItem);

            #endregion

            #region Lifestyle Factors

            IList<ScoreCardItem> LFactorsItems = new List<ScoreCardItem>();

            //Tobacco use
            ScoreCardItem LFactorsItem = new ScoreCardItem();
            LFactorsItem.Name = Translate.Message("L270");
            var tobaccoRisk = GetTobaccoRisk(hra, true, null);
            if (tobaccoRisk.risk == 1)
            {
                LFactorsItem.riskClass = "at-risk";
                LFactorsItem.riskText = "L265";
            }
            else if (tobaccoRisk.risk == 2)
            {
                LFactorsItem.riskClass = "not-at-risk";
                LFactorsItem.riskText = "L264";
            }
            else
            {
                LFactorsItem.riskClass = "cannot-calculate";
                LFactorsItem.riskText = Translate.Message("L211");
            }
            LFactorsItems.Add(LFactorsItem);

            //Physical activity
            LFactorsItem = new ScoreCardItem();
            LFactorsItem.Name = Translate.Message("L271");
            var paRisk = GetPARisk(hra, hraVer, true);
            if (paRisk.risk == 1)
            {
                LFactorsItem.riskClass = "at-risk";
                LFactorsItem.riskText = "L265";
            }
            else if (paRisk.risk == 2)
            {
                LFactorsItem.riskClass = "not-at-risk";
                LFactorsItem.riskText = "L264";
            }
            else
            {
                LFactorsItem.riskClass = "cannot-calculate";
                LFactorsItem.riskText = Translate.Message("L211");
            }
            LFactorsItems.Add(LFactorsItem);

            //Nutrition
            LFactorsItem = new ScoreCardItem();
            LFactorsItem.Name = Translate.Message("L272");
            var nutRisk = GetNutritionRisk(hra);
            if (nutRisk.risk == 1)
            {
                LFactorsItem.riskClass = "at-risk";
                LFactorsItem.riskText = "L265";
            }
            else if (nutRisk.risk == 2)
            {
                LFactorsItem.riskClass = "not-at-risk";
                LFactorsItem.riskText = "L264";
            }
            else
            {
                LFactorsItem.riskClass = "cannot-calculate";
                LFactorsItem.riskText = Translate.Message("L211");
            }
            LFactorsItems.Add(LFactorsItem);

            //Alcohol
            LFactorsItem = new ScoreCardItem();
            LFactorsItem.Name = Translate.Message("L4471");
            var alcoholRisk = GetAlcoholRisk(hra);
            if (alcoholRisk == 1)
            {
                LFactorsItem.riskClass = "at-risk";
                LFactorsItem.riskText = "L265";
            }
            else if (alcoholRisk == 2)
            {
                LFactorsItem.riskClass = "not-at-risk";
                LFactorsItem.riskText = "L264";
            }
            else
            {
                LFactorsItem.riskClass = "cannot-calculate";
                LFactorsItem.riskText = Translate.Message("L211");
            }
            LFactorsItems.Add(LFactorsItem);

            //Stress
            LFactorsItem = new ScoreCardItem();
            LFactorsItem.Name = Translate.Message("L273");
            var stressRisk = GetStressRisk(hra);
            if (stressRisk.risk == 1)
            {
                LFactorsItem.riskClass = "at-risk";
                LFactorsItem.riskText = "L265";
            }
            else if (stressRisk.risk == 2)
            {
                LFactorsItem.riskClass = "not-at-risk";
                LFactorsItem.riskText = "L264";
            }
            else
            {
                LFactorsItem.riskClass = "cannot-calculate";
                LFactorsItem.riskText = Translate.Message("L211");
            }
            LFactorsItems.Add(LFactorsItem);

            //Personal safety
            if (hraVer != (byte)HRAVersions.IntuityVersion)
            {
                LFactorsItem = new ScoreCardItem();
                LFactorsItem.Name = Translate.Message("L274");
                var safetyRisk = GetSafetyRisk(hra);
                if (safetyRisk.risk == 1)
                {
                    LFactorsItem.riskClass = "at-risk";
                    LFactorsItem.riskText = "L265";
                }
                else if (safetyRisk.risk == 2)
                {
                    LFactorsItem.riskClass = "not-at-risk";
                    LFactorsItem.riskText = "L264";
                }
                else
                {
                    LFactorsItem.riskClass = "cannot-calculate";
                    LFactorsItem.riskText = Translate.Message("L211");
                }
                LFactorsItems.Add(LFactorsItem);
            }

            #endregion

            #region Clinical Measurements

            IList<ScoreCardItem> CMItems = new List<ScoreCardItem>();
            IList<MeasurementsDto> Measurements = CommonUtility.ListMeasurements(unit).Measurements;

            //Weight
            ScoreCardItem CMItem = new ScoreCardItem();
            CMItem.Name = Translate.Message("L275");
            var bmiRisk = GetOverweightRisk(hra, true, Measurements, null, unit);
            if ((bmiRisk.risk > 0))
            {
                CMItem.riskClass = "at-risk";
                CMItem.riskText = "L265";
            }
            else if (bmiRisk.risk == 0)
            {
                CMItem.riskClass = "not-at-risk";
                CMItem.riskText = "L264";
            }
            else
            {
                CMItem.riskClass = "cannot-calculate";
                CMItem.riskText = Translate.Message("L211");
            }
            CMItems.Add(CMItem);

            //Blood pressure
            CMItem = new ScoreCardItem();
            CMItem.Name = Translate.Message("L276");
            var bpRisk = GetBPRisk(hra, hraVer, true, true, null);
            if (bpRisk.risk == 1)
            {
                CMItem.riskClass = "at-risk";
                CMItem.riskText = "L265";
            }
            else if (bpRisk.risk == 2)
            {
                CMItem.riskClass = "not-at-risk";
                CMItem.riskText = "L264";
            }
            else
            {
                CMItem.riskClass = "cannot-calculate";
                CMItem.riskText = Translate.Message("L211");
            }
            CMItems.Add(CMItem);

            //Cholesterol/triglycerides
            CMItem = new ScoreCardItem();
            CMItem.Name = Translate.Message("L277");
            var ctRisk = GetCTRisk(user, hra, hraVer, true, Measurements, null, unit);
            if (ctRisk.risk == 1)
            {
                CMItem.riskClass = "at-risk";
                CMItem.riskText = "L265";
            }
            else if (ctRisk.risk == 2)
            {
                CMItem.riskClass = "not-at-risk";
                CMItem.riskText = "L264";
            }
            else
            {
                CMItem.riskClass = "cannot-calculate";
                CMItem.riskText = Translate.Message("L211");
            }
            CMItems.Add(CMItem);

            //Glucose/A1C
            CMItem = new ScoreCardItem();
            CMItem.Name = Translate.Message("L278");
            var diabRisk = GetDiabetesRisk(user, hra, true, null, unit);
            if (diabRisk.risk == 1)
            {
                CMItem.riskClass = "at-risk";
                CMItem.riskText = "L265";
            }
            else if (diabRisk.risk == 2)
            {
                CMItem.riskClass = "not-at-risk";
                CMItem.riskText = "L264";
            }
            else
            {
                CMItem.riskClass = "cannot-calculate";
                CMItem.riskText = Translate.Message("L211");
            }
            CMItems.Add(CMItem);

            # endregion

            #region Preventive Exams/Immunizations

            IList<ScoreCardItem> PEItems = new List<ScoreCardItem>();
            //Up to date with recommended exams
            ScoreCardItem PEItem = new ScoreCardItem();
            PEItem.Name = Translate.Message("L279");
            if ((((hra.MedicalCondition.ToldBabyNine == 1 || hra.MedicalCondition.ToldDiabetes == 1 || hra.MedicalCondition.ToldGestDiab == 1 ||
                               hra.MedicalCondition.ToldPolycyst == 1 || hra.MedicalCondition.DiabetesMed == 1) || age >= 40) && hra.Exams.GlucoseTest != 1) || hra.Exams.NoTest == 1 ||
                               (hra.Exams.SigTest != 1 && age >= 45 && hra.Exams.StoolTest != 1 && hra.Exams.ColTest != 1 && hra.Exams.ColStoolTest != 1) || (hra.Exams.BoneTest != 1
                               && age >= 65 && user.Gender == 2) || (hra.Exams.DentalExam != 1) || (hra.Exams.CholTest != 1) ||
                               (hra.Exams.EyeExam != 1 && age >= 40 && hra.Goals.Diabetes == true) || (hra.Exams.PapTest != 1 && user.Gender == 2)
                               || (hra.Exams.Mammogram != 1 && age >= 40 && user.Gender == 2) || (hra.Exams.BPCheck != 1))
            {
                PEItem.riskClass = "at-risk";
                PEItem.riskText = "L265";
            }
            else
            {
                PEItem.riskClass = "not-at-risk";
                PEItem.riskText = "L264";
            }
            PEItems.Add(PEItem);
            //Up to date with recommended vaccinations
            PEItem = new ScoreCardItem();
            PEItem.Name = Translate.Message("L280");
            var immunizationRisk = GetImmunizationRisk(hra, user, age);
            if (immunizationRisk == 1)
            {
                PEItem.riskClass = "at-risk";
                PEItem.riskText = "L265";
            }
            else
            {
                PEItem.riskClass = "not-at-risk";
                PEItem.riskText = "L264";
            }
            PEItems.Add(PEItem);

            #endregion

            //add items to model
            model.PHistoryItems = PHistoryItems;
            model.FHistoryItems = FHistoryItems;
            model.LFactorsItems = LFactorsItems;
            model.CMItems = CMItems;
            model.PEItems = PEItems;

            return model;
        }

        #endregion

        #region Follow-Up Report

        public static FollowupReport GetFollowupReportDashboard(int participantId, int followupId, int hraVer, int unit, string timeZone)
        {
            ReportReader reader = new ReportReader();
            FollowupReport reportModel = new FollowupReport();
            ReadFollowupReportRequest request = new ReadFollowupReportRequest();
            bool isCaptivaVersion = hraVer == (byte)HRAVersions.CaptivaVersion;
            request.followupId = followupId;
            request.TimeZone = timeZone;
            request.userId = participantId;

            var response = reader.GetFollowupReportDashboard(request);
            reportModel.UserName = response.UserName;
            reportModel.Address = response.Address;
            reportModel.Address2 = response.Address2;
            reportModel.City = response.City;
            reportModel.State = response.State;
            reportModel.Zip = response.Zip;
            reportModel.Picture = response.Picture;
            reportModel.DOB = response.DOB;
            reportModel.ProgramStartDate = response.ProgramStartDate;
            reportModel.AssessmentDate = response.AssessmentDate;
            reportModel.FollowupLabSource = response.FollowupLabSource;
            reportModel.ScheduledCoachingSession = response.ScheduledCoachingSession;
            reportModel.CompletedCoachingSession = response.CompletedCoachingSession;
            reportModel.AssignedKitsmodel = new List<AssignedKit>();
            if (!response.BloodTestDate.HasValue)
            {
                if (response.AssessmentDate.Value > DateTime.UtcNow.AddDays(-(response.FollowUpValidity ?? 45)))
                {
                    reportModel.labDataAlertNote = string.Format(Translate.Message("L4601"), response.FollowUpValidity ?? 45);
                }
            }

            if (response.AssignedKits != null && response.AssignedKits.Count > 0)
            {
                foreach (var report in response.AssignedKits)
                {
                    AssignedKit list = new AssignedKit();
                    list.DateAssigned = report.DateAssigned;
                    list.EducationalTopic = report.EducationalTopic;
                    list.PercentCompleted = report.PercentCompleted;
                    reportModel.AssignedKitsmodel.Add(list);
                }
            }
            //charts
            HDSRisks hdsRisks = new HDSRisks();
            CTRisk ctRisk = new CTRisk();
            OBRisk obRisk = new OBRisk();
            BPRisk bpRisk = new BPRisk();
            DBRisk dbRisk = new DBRisk();
            TobaccoRisk tobaccoRisk = new TobaccoRisk();
            RiskChart chart = new RiskChart();

            if (((response.TenYrProbStart.HasValue && response.TenYrProbStart > 0) || (response.TenYrProbCurrent.HasValue && response.TenYrProbCurrent > 0)) && response.TenYrLowGoal > 0)
            {
                //risk text
                hdsRisks.chdRiskText = Translate.Message("L1483");
                //risk chart
                chart = new RiskChart();
                if (response.TenYrProbStart.HasValue)
                    chart.startValue = response.TenYrProbStart.Value * 100;
                chart.startText = Translate.Message("L2121");
                if (response.TenYrProbCurrent.HasValue)
                    chart.currentValue = response.TenYrProbCurrent.Value * 100;
                chart.currentText = Translate.Message("L1484");
                chart.goalValue1 = response.TenYrLowGoal.Value * 100;
                chart.goalText1 = Translate.Message("L1485");
                chart.label = Translate.Message("L1486");
                chart.includeChar = "%";
                chart.riskName = "CHD";
                if (response.TenYrProbStart > response.TenYrLowGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.TenYrProbCurrent > response.TenYrLowGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                hdsRisks.chdRiskChart = chart;
            }
            if ((response.TenYearASCVDStart.HasValue || response.TenYearASCVDCurrent.HasValue) && response.TenYearASCVDGoal.HasValue)
            {
                //risktext
                hdsRisks.tenYearRiskText = "Atherosclerotic cardiovascular disease (‘ASCVD’) " + Translate.Message("L1490");
                //risk chart
                chart = new RiskChart();
                if (response.TenYearASCVDStart.HasValue)
                    chart.startValue = response.TenYearASCVDStart.Value;
                chart.startText = Translate.Message("L2121");
                if (response.TenYearASCVDCurrent.HasValue)
                    chart.currentValue = response.TenYearASCVDCurrent.Value;
                chart.currentText = Translate.Message("L1484");
                chart.goalValue1 = response.TenYearASCVDGoal.Value;
                chart.goalText1 = Translate.Message("L1487");
                chart.label = Translate.Message("L1488");
                chart.includeChar = "%";
                chart.riskName = "10-Year";
                if (response.TenYearASCVDStart > response.TenYearASCVDGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.TenYearASCVDCurrent > response.TenYearASCVDGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                hdsRisks.tenYearRiskChart = chart;
            }
            if ((response.LifetimeASCVDStart.HasValue || response.LifetimeASCVDCurrent.HasValue) && response.LifetimeASCVDGoal.HasValue)
            {
                //risktext
                hdsRisks.lifetimeRiskText = "Atherosclerotic cardiovascular disease (‘ASCVD’) " + Translate.Message("L1491");
                //risk chart
                chart = new RiskChart();
                if (response.LifetimeASCVDStart.HasValue)
                    chart.startValue = response.LifetimeASCVDStart.Value;
                chart.startText = Translate.Message("L2121");
                if (response.LifetimeASCVDCurrent.HasValue)
                    chart.currentValue = response.LifetimeASCVDCurrent.Value;
                chart.currentText = Translate.Message("L1484");
                chart.goalValue1 = response.LifetimeASCVDGoal.Value;
                chart.goalText1 = Translate.Message("L1487");
                chart.label = Translate.Message("L1492");
                chart.includeChar = "%";
                chart.riskName = "Lifetime";
                if (response.LifetimeASCVDStart > response.LifetimeASCVDGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.LifetimeASCVDCurrent > response.LifetimeASCVDGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                hdsRisks.lifetimeRiskChart = chart;
            }
            var MeasurementList = CommonUtility.ListMeasurements(unit);
            var Measurements = MeasurementList.Measurements;
            float optimalLdl;
            if (response.LDLGoal.HasValue && ((hraVer == (byte)HRAVersions.CaptivaVersion && response.LDLGoal == 70) || response.LDLGoal == 100))
                optimalLdl = (float)Math.Round(CommonUtility.ToMetric(70, BioLookup.LDL, unit), 1);
            else
                optimalLdl = (float)Math.Round(CommonUtility.ToMetric(100, BioLookup.LDL, unit), 1);
            var hdlVal = Math.Round(CommonUtility.ToMetric(60, BioLookup.HDL, unit), 1);
            ctRisk.riskText = string.Format(Translate.Message("L1599"), optimalLdl + " " + Measurements[BioLookup.LDL].MeasurementUnit, hdlVal + " " + Measurements[BioLookup.HDL].MeasurementUnit);
            if (response.HDLStart.HasValue || response.HDLCurrent.HasValue || response.HDLGoal.HasValue)
            {
                chart = new RiskChart();
                if (response.HDLStart.HasValue)
                    chart.startValue = (float)Math.Round(CommonUtility.ToMetric(response.HDLStart.Value, BioLookup.HDL, unit), 1);
                chart.startText = Translate.Message("L2121");
                if (response.HDLCurrent.HasValue)
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(response.HDLCurrent.Value, BioLookup.HDL, unit), 1);
                chart.currentText = Translate.Message("L1457");
                if (response.HDLGoal.HasValue)
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(response.HDLGoal.Value, BioLookup.HDL, unit), 1);
                chart.goalText1 = Translate.Message("L1502");
                chart.label = string.Format(Translate.Message("L1602"), " (" + Measurements[BioLookup.HDL].MeasurementUnit + ")");
                chart.riskName = "HDL";

                if (response.HDLStart < response.HDLGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.HDLCurrent < response.HDLGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                ctRisk.hdlRiskChart = chart;
            }
            if (response.LDLGoal.HasValue)
            {
                chart = new RiskChart();
                if (response.LDLStart.HasValue && (response.DidYouFastStart == 1 || isCaptivaVersion))
                    chart.startValue = (float)Math.Round(CommonUtility.ToMetric(response.LDLStart.Value, BioLookup.LDL, unit), 1);
                chart.startText = Translate.Message("L2121");
                if (response.LDLCurrent.HasValue && (response.DidYouFastCurrent == 1 || isCaptivaVersion))
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(response.LDLCurrent.Value, BioLookup.LDL, unit), 1);
                chart.currentText = Translate.Message("L1457");
                if (response.LDLGoal.HasValue)
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(response.LDLGoal.Value, BioLookup.LDL, unit), 1);
                chart.goalText1 = Translate.Message("L1502");
                chart.label = string.Format(Translate.Message("L1601"), isCaptivaVersion ? "" : Translate.Message("L2550"), " (" + Measurements[BioLookup.LDL].MeasurementUnit + ")");
                chart.riskName = "LDL";
                if (response.LDLStart >= response.LDLGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.LDLCurrent >= response.LDLGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate + (isCaptivaVersion ? "| (" + (response.DidYouFastCurrent == 1 ? Translate.Message("L2550") : Translate.Message("L2551")) + ")" : "");
                chart.startYear = response.PreviousDate + (isCaptivaVersion ? "| (" + (response.DidYouFastStart == 1 ? Translate.Message("L2550") : Translate.Message("L2551")) + ")" : "");
                ctRisk.ldlRiskChart = chart;
            }
            if (response.TrigGoal.HasValue)
            {
                chart = new RiskChart();
                if (response.TrigStart.HasValue && (response.DidYouFastStart == 1 || isCaptivaVersion))
                    chart.startValue = (float)Math.Round(CommonUtility.ToMetric(response.TrigStart.Value, BioLookup.Triglycerides, unit), 1);
                chart.startText = Translate.Message("L2121");
                if (response.TrigCurrent.HasValue && (response.DidYouFastCurrent == 1 || isCaptivaVersion))
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(response.TrigCurrent.Value, BioLookup.Triglycerides, unit), 1);
                chart.currentText = Translate.Message("L1457");
                if (response.TrigGoal.HasValue)
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(response.TrigGoal.Value, BioLookup.Triglycerides, unit), 1);
                chart.goalText1 = Translate.Message("L1502");
                chart.label = string.Format(Translate.Message("L1600"), isCaptivaVersion ? "" : Translate.Message("L2550"), " (" + Measurements[BioLookup.Triglycerides].MeasurementUnit + ")");
                chart.riskName = "Trig";
                if (response.TrigStart >= response.TrigGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.TrigCurrent >= response.TrigGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate + (isCaptivaVersion ? "| (" + (response.DidYouFastCurrent == 1 ? Translate.Message("L2550") : Translate.Message("L2551")) + ")" : "");
                chart.startYear = response.PreviousDate + (isCaptivaVersion ? "| (" + (response.DidYouFastStart == 1 ? Translate.Message("L2550") : Translate.Message("L2551")) + ")" : "");
                ctRisk.trigRiskChart = chart;
            }
            //Total Cholesterol
            chart = new RiskChart();
            if (response.TotalCholStart.HasValue)
                chart.startValue = (float)Math.Round(CommonUtility.ToMetric(response.TotalCholStart.Value, BioLookup.Cholesterol, unit), 1);
            chart.startText = Translate.Message("L2121");
            if (response.TotalCholCurrent.HasValue)
                chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(response.TotalCholCurrent.Value, BioLookup.Cholesterol, unit), 1);
            chart.currentText = Translate.Message("L1457");
            chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(200, BioLookup.Cholesterol, unit), 1);
            chart.goalText1 = Translate.Message("L1502");
            chart.label = string.Format(Translate.Message("L324"), Measurements[BioLookup.Cholesterol].MeasurementUnit);
            chart.riskName = "Chol";
            if (response.TotalCholStart >= 200)
                chart.color1 = Constants.riskColor;
            else
                chart.color1 = Constants.noRiskColor;
            if (response.TotalCholCurrent >= 200)
                chart.color2 = Constants.riskColor;
            else
                chart.color2 = Constants.noRiskColor;
            chart.currentYear = response.CurrentDate;
            chart.startYear = response.PreviousDate;
            ctRisk.tcRiskChart = chart;
            if (response.WeightSTGoal.HasValue || response.WeightLTGoal.HasValue)
            {
                chart = new RiskChart();
                if (response.WeightStart.HasValue)
                    chart.startValue = (float)Math.Round(CommonUtility.ToMetric(response.WeightStart.Value, BioLookup.Weight, unit), 1);
                chart.startText = Translate.Message("L2121");
                if (response.WeightCurrent.HasValue)
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(response.WeightCurrent.Value, BioLookup.Weight, unit), 1);
                chart.currentText = Translate.Message("L1457");
                if (response.Height.HasValue)
                    chart.goalValue0 = Math.Round(CommonUtility.ToMetric((float)GetWeightfromBMI(response.Height.Value, (float)18.5), BioLookup.Weight, unit), 1);
                chart.goalText0 = Translate.Message("L1458");
                if (response.WeightSTGoal.HasValue)
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(response.WeightSTGoal.Value, BioLookup.Weight, unit), 1);
                chart.goalText1 = Translate.Message("L1459");
                if (response.WeightLTGoal.HasValue)
                    chart.goalValue2 = (float)Math.Round(CommonUtility.ToMetric(response.WeightLTGoal.Value, BioLookup.Weight, unit), 1);
                chart.goalText2 = Translate.Message("L1460");
                chart.label = Translate.Message("L1461");
                chart.label = string.Format(Translate.Message("L1461"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
                chart.riskName = "Obese";
                if (response.WeightStart > response.WeightSTGoal || response.WeightStart < chart.goalValue0)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.WeightCurrent > response.WeightSTGoal || response.WeightCurrent < chart.goalValue0)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                obRisk.riskChart = chart;
            }
            bpRisk.riskText = Translate.Message("L2208");
            if (hraVer == (byte)HRAVersions.CaptivaVersion)
                bpRisk.riskText = Translate.Message("L2208") + " " + Translate.Message("L4300");
            if (response.SBPGoal.HasValue)
            {
                chart = new RiskChart();
                if (response.SBPStart.HasValue)
                    chart.startValue = response.SBPStart.Value;

                chart.startText = Translate.Message("L2121");
                if (response.SBPCurrent.HasValue)
                    chart.currentValue = response.SBPCurrent.Value;
                chart.currentText = Translate.Message("L1457");
                if (response.SBPGoal.HasValue)
                    chart.goalValue1 = response.SBPGoal.Value;
                chart.goalText1 = Translate.Message("L1502");
                chart.label = Translate.Message("L322");
                chart.riskName = "SBP";
                if (response.SBPStart >= response.SBPGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.SBPCurrent >= response.SBPGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                bpRisk.sbpRiskChart = chart;
            }
            if (response.DBPGoal.HasValue)
            {
                chart = new RiskChart();
                if (response.DBPStart.HasValue)
                    chart.startValue = response.DBPStart.Value;
                chart.startText = Translate.Message("L2121");
                if (response.DBPCurrent.HasValue)
                    chart.currentValue = response.DBPCurrent.Value;
                chart.currentText = Translate.Message("L1457");
                if (response.DBPGoal.HasValue)
                    chart.goalValue1 = response.DBPGoal.Value;
                chart.goalText1 = Translate.Message("L1502");
                chart.label = Translate.Message("L323");
                chart.riskName = "DBP";
                if (response.DBPStart >= response.DBPGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.DBPCurrent >= response.DBPGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                bpRisk.dbpRiskChart = chart;
            }
            if (response.GlucoseGoal1.HasValue || response.GlucoseGoal2.HasValue)
            {
                chart = new RiskChart();
                if (response.GlucoseStart.HasValue && response.DidYouFastStart == 1)
                    chart.startValue = (float)Math.Round(CommonUtility.ToMetric(response.GlucoseStart.Value, BioLookup.Glucose, unit), 1);
                chart.startText = Translate.Message("L2121");
                if (response.GlucoseCurrent.HasValue && response.DidYouFastCurrent == 1)
                    chart.currentValue = (float)Math.Round(CommonUtility.ToMetric(response.GlucoseCurrent.Value, BioLookup.Glucose, unit), 1);
                chart.currentText = Translate.Message("L1457");
                if (response.GlucoseGoal1 < response.GlucoseGoal2)
                {
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(response.GlucoseGoal2.Value, BioLookup.Glucose, unit), 1);
                }
                else
                {
                    chart.goalValue1 = (float)Math.Round(CommonUtility.ToMetric(response.GlucoseGoal1.Value, BioLookup.Glucose, unit), 1);
                }
                chart.goalText1 = Translate.Message("L1502");
                chart.label = string.Format(Translate.Message("L1514"), " (" + CommonUtility.GetMeasurementText(BioLookup.Glucose, unit) + ")");
                chart.riskName = "Glucose";
                if (response.Diabetes == true)
                {
                    chart.goalValue0 = Math.Round(CommonUtility.ToMetric(response.GlucoseGoal1.Value, BioLookup.Glucose, unit), 1);
                    chart.goalText0 = Translate.Message("L1458");
                }
                if ((response.GlucoseStart > response.GlucoseGoal2 && response.Diabetes == true) || (response.GlucoseStart >= response.GlucoseGoal2 && response.Diabetes == false) ||
                    (response.GlucoseStart < response.GlucoseGoal1 && response.Diabetes == true))
                {
                    chart.color1 = Constants.riskColor;
                }
                else
                {
                    chart.color1 = Constants.noRiskColor;
                }
                if ((response.GlucoseCurrent > response.GlucoseGoal2 && response.Diabetes == true) || (response.GlucoseCurrent >= response.GlucoseGoal2 && response.Diabetes == false) ||
                    (response.GlucoseCurrent < response.GlucoseGoal1 && response.Diabetes == true))
                {
                    chart.color2 = Constants.riskColor;
                }
                else
                {
                    chart.color2 = Constants.noRiskColor;
                }
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                dbRisk.glucChart = chart;
            }
            if (response.A1CGoal.HasValue)
            {
                chart = new RiskChart();
                if (response.A1CStart.HasValue)
                    chart.startValue = response.A1CStart.Value;
                chart.startText = Translate.Message("L2121");
                if (response.A1CCurrent.HasValue)
                    chart.currentValue = response.A1CCurrent.Value;
                chart.currentText = Translate.Message("L1457");
                if (response.A1CGoal.HasValue)
                    chart.goalValue1 = response.A1CGoal.Value;
                chart.goalText1 = Translate.Message("L1502");
                chart.label = Translate.Message("L329");
                chart.riskName = "A1C";
                chart.includeChar = "%";
                if (response.A1CStart >= response.A1CGoal)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.A1CCurrent >= response.A1CGoal)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                dbRisk.a1cChart = chart;
            }
            if (response.tobaccoStart.HasValue || response.tobaccoCurrent.HasValue)
            {
                chart = new RiskChart();
                tobaccoRisk.riskText = Translate.Message("L1482");
                if (response.tobaccoStart.HasValue)
                    chart.startValue = response.tobaccoStart.Value;
                chart.startText = Translate.Message("L2121");
                if (response.tobaccoCurrent.HasValue)
                    chart.currentValue = response.tobaccoCurrent.Value;
                chart.currentText = Translate.Message("L1457");
                chart.goalValue1 = 0;
                chart.goalText1 = Translate.Message("L1502");
                chart.label = Translate.Message("L1060") + " " + Translate.Message("L2474");
                chart.riskName = "Tobacco";
                if (response.tobaccoStart > 0)
                    chart.color1 = Constants.riskColor;
                else
                    chart.color1 = Constants.noRiskColor;
                if (response.tobaccoCurrent > 0)
                    chart.color2 = Constants.riskColor;
                else
                    chart.color2 = Constants.noRiskColor;
                chart.currentYear = response.CurrentDate;
                chart.startYear = response.PreviousDate;
                tobaccoRisk.riskChart = chart;
            }
            reportModel.hdsRisks = hdsRisks;
            reportModel.ctRisk = ctRisk;
            reportModel.overweightRisk = obRisk;
            reportModel.bpRisk = bpRisk;
            reportModel.diabetesRisk = dbRisk;
            reportModel.tobaccoRisk = tobaccoRisk;

            //referrals
            List<String> referrals = new List<String>();
            if (hraVer == (byte)HRAVersions.CaptivaVersion)
            {
                referrals.Add(Translate.Message("L4301"));
                referrals.Add(Translate.Message("L4302"));
            }
            else
            {
                if (response.CholRef == true)
                {
                    referrals.Add(Translate.Message("L335"));
                }
                string BPReferrals = "";
                if (response.BPRef == true)
                {
                    BPReferrals = Translate.Message("L2088");
                }
                if (response.ElevatedBPRef == true)
                {
                    BPReferrals = Translate.Message("L362");
                }
                if (!string.IsNullOrEmpty(BPReferrals))
                {
                    if (response.HypertensiveBPRef == true)
                        referrals.Add(BPReferrals + " " + Translate.Message("L2652"));
                    else
                        referrals.Add(BPReferrals);

                }
                if (response.LdlRef1 == true)
                {
                    referrals.Add(Translate.Message("L2089"));
                }
                if (response.LdlRef2 == true)
                {
                    referrals.Add(Translate.Message("L2090"));
                }
                if (response.HdlRef == true)
                {
                    referrals.Add(Translate.Message("L2091"));
                }
                if (response.TrigRef1 == true)
                {
                    referrals.Add(Translate.Message("L2092"));
                }
                if (response.TrigRef2 == true)
                {
                    referrals.Add(Translate.Message("L2093"));
                }
                if (response.DiabRef == true)
                {
                    referrals.Add(Translate.Message("L2094"));
                }
                if (response.NicRef == true)
                {
                    referrals.Add(Translate.Message("L361"));
                }
                if (response.AspRef == true)
                {
                    referrals.Add(Translate.Message("L2095"));
                }
                if (response.ASCVDRef != null)
                {
                    referrals.Add(Translate.Message("L" + response.ASCVDRef));
                }
            }
            reportModel.referrals = referrals;
            reportModel.NoOfWeeks = response.NoOfWeeks;

            reportModel.PreviousYearScheduledCoachingSession = response.PreviousYearScheduledCoachingSession;
            reportModel.PreviousYearCompletedCoachingSession = response.PreviousYearCompletedCoachingSession;

            reportModel.PreviousYearAssignedKitsmodel = new List<AssignedKit>();
            if (response.PreviousYearKits != null && response.PreviousYearKits.Count > 0)
            {
                foreach (var report in response.PreviousYearKits)
                {
                    AssignedKit list = new AssignedKit();
                    list.DateAssigned = report.DateAssigned;
                    list.EducationalTopic = report.EducationalTopic;
                    list.PercentCompleted = report.PercentCompleted;
                    reportModel.PreviousYearAssignedKitsmodel.Add(list);
                }
            }

            return reportModel;
        }

        #endregion

        public static double GetWeightfromBMI(float height, float bmi)
        {
            return Math.Round(2.2 * bmi * (Math.Pow((height / 39.4), 2)));
        }

        public static bool SaveReportFeedback(byte type, string comments, int hraId, int userId)
        {
            ReportReader reader = new ReportReader();
            AddEditReportFeedbackRequest request = new AddEditReportFeedbackRequest();
            request.HRAId = hraId;
            request.Type = type;
            request.Comments = comments;
            request.CreatedBy = userId;
            return reader.SaveReportFeedback(request);
        }

        public static void EditInvoiceBill(InvoiceBilledDetailsDto invoiceBill)
        {
            ReportReader reader = new ReportReader();
            reader.EditInvoiceDetails(new InvoiceDetailsRequest { Id = invoiceBill.Id });
        }

        public static GetInvoiceDetailsResponse GetAllPendingInvoiceBill()
        {
            ReportReader reader = new ReportReader();
            return reader.GetAllPendingInvoiceBill();
        }

        public static AddFaxedReportResponse AddFaxedReport(FaxedReportsDto faxedReport)
        {
            ReportReader reader = new ReportReader();
            AddFaxedReportRequest request = new AddFaxedReportRequest();
            request.faxedReport = faxedReport;
            return reader.AddFaxedReport(request);
        }

        public static GetFaxedReportsResponse GetFaxedReports(int participantId)
        {
            ReportReader reader = new ReportReader();
            return reader.GetFaxedReports(participantId);
        }

    }
}