using Intervent.Business.EmailTriggers;
using Intervent.Web.DataLayer;
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

namespace Intervent.Services.ClaimProcess
{
    public partial class ClaimProcessService : ServiceBase
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private bool serviceStarted = false;
        private bool workerProcessing = false;
        Thread workerThread;

        public ClaimProcessService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service started");
            ThreadStart st = new ThreadStart(WorkerFunction);
            workerThread = new Thread(st);

            serviceStarted = true;

            workerThread.Start();
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
                        Web.DTO.EmailRequestDto emailParams = new Web.DTO.EmailRequestDto() { FunctionalityName = "Nightly Email Triggers", AdditionalMessage = ex.StackTrace };
                        Business.Mail.NotificationTemplate notification = new Business.Mail.GenericErrorTemplate(emailParams);
                        notification.SendEmail();
                    }
                    catch
                    {

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
