using Intervent.Business;
using Intervent.Business.Eligibility;
using Intervent.Business.FollowUp;
using Intervent.Business.Mail;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System;
using System.ServiceProcess;
using System.Timers;
using System.Reflection;
using Intervent.Business.Notification;
using Intervent.HWS;
using Intervent.Business.Organization;
using Intervent.Business.EmailTriggers;
using Intervent.Business.Claims;

namespace Intervent.Services.Eligibility
{
    public partial class EligibilityService : ServiceBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private bool workerProcessing = false;
        private bool appointmentProcessing = false;
        private bool appointmentHourlyProcessing = false;
        private bool smsProcessing = false;
        private bool smsHourlyProcessing = false;
        private bool hourlyProcessing = false;
        private bool TrackingDone = false;
        private bool claimsActivityDone = false;
        private bool AutoDialerDone = false;
        private bool OutreachEmailSent = false;
        private bool hProcessing = false;
        private bool hhProcessing = false;
        private bool mProcessing = false;

        public EligibilityService()
        {
            InitializeComponent();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Assembly.Load("Intervent.DALCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                log.Info("Service started");
                Utility.InitMapper();
                //  AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                //timer every 15 minutes 900000
                System.Timers.Timer timer = new System.Timers.Timer(900000);
                timer.Elapsed += timer_Elapsed;
                timer.Start();
                System.Timers.Timer appointmentScheduler = new System.Timers.Timer(120000);
                appointmentScheduler.Elapsed += appointmentScheduler_Elapsed;
                appointmentScheduler.Start();
                System.Timers.Timer smsScheduler = new System.Timers.Timer(900000);
                smsScheduler.Elapsed += SmsScheduler_Elapsed;
                smsScheduler.Start();
                System.Timers.Timer hourlyScheduler = new System.Timers.Timer(3600000);
                hourlyScheduler.Elapsed += hourlyScheduler_Elapsed;
                hourlyScheduler.Start();
                System.Timers.Timer CRMScheduler = new System.Timers.Timer(120000);
                CRMScheduler.Elapsed += CRMScheduler_Elapsed;
                CRMScheduler.Start();
                System.Timers.Timer messageScheduler = new System.Timers.Timer(3600000);
                messageScheduler.Elapsed += messageScheduler_Elapsed;
                messageScheduler.Start();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }

        }

        private void CRMScheduler_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                new CRMManager().GetIntuityInboundOutboundCallData();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }
        }

        private void SmsScheduler_Elapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            if (now.Hour == 2)
            {
                if (!smsHourlyProcessing)
                {
                    smsHourlyProcessing = true;
                }
            }
            else if (!smsProcessing)
            {
                smsProcessing = true;
                smsHourlyProcessing = false;
                //text message to Twilio
                new TwilioManager().SendAppointmentSms();
                new TwilioManager().LegacySendSms();
                smsProcessing = false;
            }
        }

        void appointmentScheduler_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var now = DateTime.Now;
                //every day between 5 am and 5:15 am because of the timer
                if (now.Hour == 5)
                {
                    LogReader reader = new LogReader();
                    if (!appointmentHourlyProcessing)
                    {
                        appointmentHourlyProcessing = true;
                        new IntuityEligibilityManager().ProcessShipment();
                        new IntuityEligibilityManager().CheckGlucoseActivity();
                        new EmailTriggerManager().ProcessEmailForSelfHelpKit();
                    }
                }
                else if (!appointmentProcessing)
                {
                    appointmentProcessing = true;
                    appointmentHourlyProcessing = false;
                    new NotificationManager().ProcessQueuedEvents();
                    appointmentProcessing = false;
                    if (now.Hour == 11 && now.Minute >= 54)
                    {
                        LogReader reader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Email process running as of " + DateTime.Now.ToString(), null, null);
                        reader.WriteLogMessage(logEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }
        }

        //1 am - Incentive, 2 am -Followup, 4 am on Monday - Tracking, 6am - Autodial list from five9
        //7am - Get Lab values, 9am - Send birthday message, 10 pm - Livongo, 11 pm - Recipe, Else - Outreach
        //TimeZone - CST
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!workerProcessing)
                {
                    workerProcessing = true;
                    var now = DateTime.Now;
                    var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    if (now.Hour == 1 && !hourlyProcessing)
                    {
                        hourlyProcessing = true;
                        var Count = new IncentiveManager().ProcessIncentives();

                        if (Count.HRAIncentivesCount > 0 || Count.PCPIncentivesCount > 0 || Count.TCIncentivesCount > 0 || Count.HealthNumbersIncentiveCount > 0)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Processed " + Count.HRAIncentivesCount + " HRA Incentive(s); " + Count.HealthNumbersIncentiveCount + " Health Numbers Incentive(s);" + Count.PCPIncentivesCount + " PCP Incentive(s) and " + Count.TCIncentivesCount + " Tobacco Incentive(s)  and " + Count.BiometricIncentiveCount + " Biometric Incentive(s) on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);

                        }
                    }
                    else if (now.Hour == 2 && !hourlyProcessing)
                    {
                        hourlyProcessing = true;
                        var count = new FollowUpManager().AutomateFollowUp();
                        if (count > 0)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Assigned follow-up to " + count + " user(s) on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                        new PortalManager().DeactivateInactivePortal();
                        if (!claimsActivityDone)
                        {
                            claimsActivityDone = true;

                            new EbenClaimManager().ProcessRxClaimsFiles();
                            new IntuityEligibilityManager().CreateIntuityEligibility();
                        }
                    }
                    else if (now.DayOfWeek == DayOfWeek.Monday && now.Hour == 4 && !hourlyProcessing && !TrackingDone)
                    {
                        hourlyProcessing = true;
                        var count = new TrackingManager().SendTrackingData();
                        TrackingDone = true;
                        if (count > 0)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Sent " + count + " record(s) for tracking on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                    }
                    else if (now.Hour == 6 && !hourlyProcessing && !AutoDialerDone)
                    {
                        hourlyProcessing = true;
                        var count = new OutreachManager().GetOutReachandTrackingData();
                        AutoDialerDone = true;
                        if (count > 0)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Processed " + count + " OutReach and Tracking Data on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                    }
                    else if (now.Hour == 7)
                    {
                        hourlyProcessing = true;
                        var count = new LabcorpServicesManager().PullLabValues(Labcorp.New);
                        if (count > 0)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Processed labs for " + count + " user(s) on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                        TrackingDone = false;   //Setting it to false so that it runs each monday
                        OutreachEmailSent = false; //Setting it to false so that birthday wishes are sent everyday
                        AutoDialerDone = false; //Setting it to false so that it runs everyday     
                        claimsActivityDone = false; //Setting it to false so that it runs everyday
                    }
                    else if (now.Hour == 21 && !hourlyProcessing)
                    {
                        hourlyProcessing = true;
                        var processedCount = 0; var eligCount = 0; var runInPeriod = 0;
                        processedCount = new LabManager().ProcessDynacareLabs();
                        eligCount = new LMCManager().SendInterventCanriskParticipants();
                        runInPeriod = new LMCManager().CompIntroKitsOnTime();
                        if (processedCount > 0 || eligCount > 0 || runInPeriod > 0)
                        {
                            LogReader reader = new LogReader();
                            if (processedCount > 0)
                            {
                                var processedEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Processed " + processedCount + " record(s) from Dynacare on " + DateTime.Now.ToString(), null, null);
                                reader.WriteLogMessage(processedEvent);
                            }
                            if (eligCount > 0)
                            {
                                var processedEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Sent " + eligCount + "Part A eligibility record(s) to LMC on " + DateTime.Now.ToString(), null, null);
                                reader.WriteLogMessage(processedEvent);
                            }
                            if (runInPeriod > 0)
                            {
                                var processedEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Updated run in period for  " + runInPeriod + " particpants on " + DateTime.Now.ToString(), null, null);
                                reader.WriteLogMessage(processedEvent);
                            }
                        }
                    }
                    else if (now.Hour == 22 && !hourlyProcessing)
                    {
                        hourlyProcessing = true;
                    }
                    else if (now.Hour == 23 && !hourlyProcessing)
                    {
                        hourlyProcessing = true;
                        var recipeCount = new RecipeManager().SendRecipe();
                        if (recipeCount > 0)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Assigned recipe to " + recipeCount + " user(s) on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }

                        if(now.Day == 15 || now.Day == DateTime.DaysInMonth(now.Year, now.Month))
                        {
                            new InvoiceManager().ProcessInvoiceBilling();
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Invoice Service", null, "Generate Invoice process initiated on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                    }
                    else if (now.Hour == 9 && !OutreachEmailSent)
                    {
                        var count = new OutreachEmailManager().BirthdayWishes();
                        if (count > 1)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Sent birthday wishes to " + count + " user(s) on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                        try
                        {
                            var Duplicatecount = new LMCManager().DeactivateDuplicates();
                            if (Duplicatecount > 1)
                            {
                                LogReader reader = new LogReader();
                                var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Deactivated " + Duplicatecount + " LMC eligiblities(s) on " + DateTime.Now.ToString(), null, null);
                                reader.WriteLogMessage(logEvent);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                            reader.WriteLogMessage(logEvent);
                        }
                        var userKeysCount = new IncentiveManager().ProcessUserKeys();
                        if (userKeysCount > 1)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Processed " + userKeysCount + " user keys on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                        OutreachEmailSent = true;
                    }
                    else if (now.Hour == 23 && !hourlyProcessing)
                    {
                        hourlyProcessing = true;
                        var count = new AdminManager().AssignNewsletter();
                        if (count > 0)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Assigned newsletter to " + count + " user(s) on " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                        new ContactRequirementManager().ProcessContactRequirementAlerts();
                    }
                    //else if (now.Hour == 23 && !hourlyProcessing)
                    //{
                    //    //new ContactRequirementManager().ProcessContactRequirementAlerts();
                    //}
                    else //process once for 15 minutes
                    {
                        hourlyProcessing = false;
                        new OutreachManager().SendOutreachData();
                        new OutreachManager().SendCanriskOutreachData();
                        if (now.Hour == 10 && now.Minute >= 45)
                        {
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Outreach process running as of " + DateTime.Now.ToString(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }

                    }
                    workerProcessing = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }
        }

        void hourlyScheduler_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var now = DateTime.Now;
                if (now.Hour == 16)
                {
                    if (!hhProcessing)
                    {
                        hhProcessing = true;
                        try
                        {
                            //Appointment reminder call
                            new AppointmentManager().SendApptReminder();
                            new AppointmentManager().SendLegacyApptReminder();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message + ex.StackTrace);
                        }
                    }
                }
                else if (!hProcessing)
                {
                    hProcessing = true;
                    hhProcessing = false;
                    //Load eligbility files
                    new EligibilityManager().LoadEligibilityFiles();
                    //Process subject files for CAPTIVA
                    new CaptivaManager().ProcessCaptivaSubjectFiles();
                    //Process Billing Notes
                    new PlacerManager().ProcessBillingNotes();
                    //Process biometric files for CAPTIVA
                    var biometricCount = new CaptivaManager().ProcessCaptivaBiometricFiles();
                    if (biometricCount > 0)
                    {
                        LogReader reader = new LogReader();
                        var processedEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Captiva Biometric Files processed. Records count : " + biometricCount, null, null);
                        reader.WriteLogMessage(processedEvent);
                    }
                    
                    //Process Medication files for CAPTIVA
                    var count = new CaptivaManager().ProcessCaptivaMedicationsFiles();
                    if (count > 0)
                    {
                        LogReader reader = new LogReader();
                        var processedEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Captiva Medication Files processed. Records count : " + count, null, null);
                        reader.WriteLogMessage(processedEvent);
                    }
                    //Process TEamBP reports
                    new PlacerManager().ProcessReportList();
                    //Intuity Account Verification
                    new IntuityEligibilityManager().VerifyIntuityUser();

                    new WebinarManager().GetWebinarDetails();
                    
                    new WearableManager().FetchWearableLogs();
                    hProcessing = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }
        }

        void messageScheduler_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var now = DateTime.Now;
                if (now.Hour == 12)
                {
                    if (!mProcessing)
                    {
                        mProcessing = true;
                        try
                        {
                            var count = new MotivationMessageManager().ProcessMotivationMessage();
                            if (count > 0)
                            {
                                LogReader reader = new LogReader();
                                var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Assigned motivation message to " + count + " user(s) on " + DateTime.Now.ToString(), null, null);
                                reader.WriteLogMessage(logEvent);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message + ex.StackTrace);
                        }
                    }
                }
                else
                    mProcessing = false;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }
        }

        protected override void OnStop()
        {
            log.Info("Service stopped");
            try
            {
                EmailRequestDto emailParams = new EmailRequestDto() { DateandTime = DateTime.Now.ToString() };
                NotificationTemplate notification = new ServiceAlertTemplate(emailParams);
                notification.SendEmail();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }

        }
    }
}
