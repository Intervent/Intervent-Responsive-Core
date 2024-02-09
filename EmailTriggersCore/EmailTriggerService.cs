using Intervent.Business.EmailTriggers;
using Intervent.Business.Mail;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intervent.Services.EmailTriggers
{
    public partial class EmailTriggerService : ServiceBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private bool serviceStarted = false;
        private bool workerProcessing = false;
        Thread workerThread;

        public EmailTriggerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                log.Info("Service started");
                ThreadStart st = new ThreadStart(WorkerFunction);
                workerThread = new Thread(st);

                serviceStarted = true;

                workerThread.Start();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.StackTrace);
            }
            
        }

        protected override void OnStop()
        {
            serviceStarted = false;
            while (workerProcessing) ;//wait until the work is completed
            workerThread.Join(new TimeSpan(0, 0, 15));
            log.Info("Service stopped");
        }

        private void WorkerFunction()
        {
            while (serviceStarted)
            {
                // System.Diagnostics.Debugger.Break();
                workerProcessing = true;

                try
                {
                    log.Debug("Begin worker processing");
                    //forcing the assembly to load to resolve edmx issues for console app
                    //Assembly.Load("Intervent.DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                    Utility.InitMapper();
                    new EmailTriggerManager().ProcessEmailTriggers();
                    log.Debug("End worker processing");
                }

                catch (Exception ex)
                {
                    log.Error(ex);
                    try
                    {
                        EmailRequestDto emailParams = new EmailRequestDto() { FunctionalityName = "Nightly Email Triggers", AdditionalMessage = ex.StackTrace };
                        NotificationTemplate notification = new GenericErrorTemplate(emailParams);
                        notification.SendEmail();
                    }
                    catch
                    {
                        log.Error(ex.Message + ex.StackTrace);
                    }
                }
                workerProcessing = false;
                if (serviceStarted)
                {
                    Thread.Sleep(new TimeSpan(0, 30, 0));//30 minutes
                }
            }

            // time to end the thread
            Thread.CurrentThread.Abort();
        }
    }
}
