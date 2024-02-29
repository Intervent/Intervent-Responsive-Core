using Intervent.Business.Account;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    public class HomeController : AccountBaseController
    {
        private readonly AppSettings _appSettings;

        public HomeController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                return RedirectToAction("Stream", "Participant");
            else
                HttpContext.Session.SetString(SessionContext.LandingPage, HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + HttpContext.Request.Path);
            TempData["OrgContactNumber"] = "1-855-494-1093";
            ViewData["DateFormat"] = "MM/DD/YYYY";
            return View();
        }

        public ActionResult Intervent(bool? signup)
        {
            var OrgId = ReadOrganization("Intervent");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Intervent/Intervent");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult POGO(bool? signup)
        {
            var OrgId = ReadOrganization("Intuity DTC");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("POGO/POGO");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult InterventPrograms()
        {
            var OrgId = ReadOrganization("Intervent");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Intervent";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Intervent/InterventPrograms");
            }
            else
                return RedirectToAction("Index");
        }
        public ActionResult Intuity()
        {
            return Redirect(_appSettings.IntuityRedirectUrl);
        }

        public ActionResult TechnicalHelp()
        {
            return View();
        }

        public ActionResult LifeHealthcare()
        {
            var OrgId = ReadOrganization("LIFE Healthcare");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("LifeHealthcare/LifeHealthcare");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult LifeHealthcarePrograms()
        {
            var OrgId = ReadOrganization("LIFE Healthcare");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                return View("LifeHealthcare/LifeHealthcarePrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Optifast()
        {
            var OrgId = ReadOrganization("Optifast", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Optifast";
                ViewData["ShortName"] = "Optifast";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Optifast/Optifast");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult OptifastInfo()
        {
            var OrgId = ReadOrganization("Optifast", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Optifast";
                ViewData["ShortName"] = "Optifast";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Optifast/OptifastInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CrothallInfo()
        {
            return RedirectToAction("Compass");
        }

        public ActionResult Crothall()
        {
            return RedirectToAction("Compass");
        }

        public ActionResult Compass(bool? signup)
        {
            var OrgId = ReadOrganization("Compass Group");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Compass Group";
                ViewData["ShortName"] = "Compass";
                ViewData["IDText"] = Translate.Message("L1361");
                ViewData["IDInfo"] = Translate.Message("L1987");
                ViewData["SignUp"] = signup;
                ViewData["OrgContactNumber"] = HttpContext.Session.GetString(SessionContext.OrgContactNumber);
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Compass/Compass");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CompassPrograms()
        {
            var OrgId = ReadOrganization("Compass Group");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Compass Group";
                ViewData["ShortName"] = "Compass";
                ViewData["IDText"] = Translate.Message("L1361");
                ViewData["IDInfo"] = Translate.Message("L1987");
                return View("Compass/CompassPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult McAllen()
        {
            var OrgId = ReadOrganization("McAllen Heart Hospital", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "McAllen Heart Hospital";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("McAllen/McAllen");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult McAllenInfo()
        {
            var OrgId = ReadOrganization("McAllen Heart Hospital", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "McAllen Heart Hospital";
                ViewData["ShortName"] = "McAllen Heart Hospital";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("McAllen/McAllenInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CHIMemorial()
        {
            if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                return RedirectToAction("Stream", "Participant");
            return View();
        }

        public ActionResult CHIGlenwood()
        {
            var OrgId = ReadOrganization("CHI Memorial Glenwood");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "CHI Memorial Glenwood";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("CHIGlenwood/CHIGlenwood");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CHIGlenwoodInfo()
        {
            var OrgId = ReadOrganization("CHI Memorial Glenwood");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "CHI Memorial Glenwood";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("CHIGlenwood/CHIGlenwoodInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CHIHixson()
        {
            var OrgId = ReadOrganization("CHI Memorial Hixson");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "CHI Memorial Hixson";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("CHIHixson/CHIHixson");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Demo(bool? signup)
        {
            var OrgId = ReadOrganization("Demo");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Demo/Demo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult DemoHRA(bool? signup)
        {
            var OrgId = ReadOrganization("DemoHRA");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Demo/DemoHRA");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult DemoCoach(bool? signup)
        {
            var OrgId = ReadOrganization("Demo Coach");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Demo/DemoCoach");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult ASPCFAQs()
        {
            var OrgId = ReadOrganization("ASPC");
            if (OrgId.HasValue)
            {
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                ViewData["OrgId"] = OrgId;
                return View("ASPC/ASPCFAQs");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult ASPC(bool? signup)
        {
            var OrgId = ReadOrganization("ASPC");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "ASPC";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("ASPC/ASPC");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult ClientTrial(bool? signup)
        {
            var OrgId = ReadOrganization("Client Trial");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("ClientTrial/ClientTrial");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CHIHixsonInfo()
        {
            var OrgId = ReadOrganization("CHI Memorial Hixson");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "CHI Memorial Hixson";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("CHIHixson/CHIHixsonInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Riverview(bool? signup)
        {
            var OrgId = ReadOrganization("Riverview");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                ViewData["Name"] = "Riverview";
                ViewData["SignUp"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Riverview/Riverview");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult RiverviewPrograms()
        {
            var OrgId = ReadOrganization("Riverview");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Riverview";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                ViewData["Name"] = "Riverview";
                return View("Riverview/RiverviewPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Janssen()
        {
            var OrgId = ReadOrganization("Janssen");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Janssen";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Janssen/Janssen");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult JanssenInfo()
        {
            var OrgId = ReadOrganization("Janssen");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Janssen";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Janssen/JanssenInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult ACC()
        {
            var OrgId = ReadOrganization("ACC");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "American College of Cardiology";
                ViewData["IDText"] = "Unique Id";
                ViewData["IDInfo"] = Translate.Message("L2217");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("ACC/ACC");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult ACCInfo()
        {
            var OrgId = ReadOrganization("ACC");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "American College of Cardiology";
                ViewData["IDText"] = "Unique Id";
                ViewData["IDInfo"] = Translate.Message("L2217");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("ACC/ACCInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult FullCircle()
        {
            var OrgId = ReadOrganization("FullCircle");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Full Circle Health Coaching LLC";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("FullCircle/FullCircle");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult FullCircleInfo()
        {
            var OrgId = ReadOrganization("FullCircle");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Full Circle Health Coaching LLC";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("FullCircle/FullCircleInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MacPapers()
        {
            var OrgId = ReadOrganization("MacPapers");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Macpapers/Macpapers");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Activate(bool forgotPassword = false, bool signup = false)
        {
            var OrgId = ReadOrganization("Activate");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["IDText"] = Translate.Message("L3075");
                ViewData["IDInfo"] = Translate.Message("L3076");
                ViewData["ForgotPassword"] = forgotPassword;
                ViewData["Signup"] = signup;
                ViewData["OrgContactNumber"] = HttpContext.Session.GetString(SessionContext.OrgContactNumber);
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Activate/Activate");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult ActivateInfo()
        {
            var OrgId = ReadOrganization("Activate");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["IDText"] = Translate.Message("L3075");
                ViewData["IDInfo"] = Translate.Message("L3076");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Activate/ActivateInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MaxisGBNdemo(bool forgotPassword = false, bool signup = false)
        {
            var OrgId = ReadOrganization("MAXIS GBN Demo");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["IDText"] = Translate.Message("L1361");
                ViewData["ForgotPassword"] = forgotPassword;
                ViewData["Signup"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Maxis/Maxis");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MaxisInfo()
        {
            var OrgId = ReadOrganization("MAXIS GBN Demo");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["IDText"] = Translate.Message("L1361");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Maxis/MaxisInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MetLifeDemo(bool forgotPassword = false, bool signup = false)
        {
            var OrgId = ReadOrganization("MetLife Demo");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["IDText"] = Translate.Message("L1361");
                ViewData["ForgotPassword"] = forgotPassword;
                ViewData["Signup"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("MetLife/MetLife");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MetLifeInfo()
        {
            var OrgId = ReadOrganization("MetLife Demo");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["IDText"] = Translate.Message("L1361");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("MetLife/MetLifeInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Training(bool? signup)
        {
            var OrgId = ReadOrganization("Training");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Training/Training");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Emirates()
        {
            var OrgId = ReadOrganization("Emirates NBD");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["Name"] = "Emirates";
                ViewData["ShortName"] = "Emirates";
                ViewData["IDText"] = "Emirates NBD Staff Number";
                ViewData["IDInfo"] = Translate.Message("L1987");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Emirates/Emirates");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult EmiratesInfo()
        {
            var OrgId = ReadOrganization("Emirates NBD");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                ViewData["IDText"] = "Emirates NBD Staff Number";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Emirates/EmiratesInfo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult SouthUniversity(bool? signup)
        {
            var OrgId = ReadOrganization("South University");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "South University";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                ViewData["SignUp"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("SouthUniversity/SouthUniversity");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult SouthUniversityPrograms()
        {
            var OrgId = ReadOrganization("South University");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "South University";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("SouthUniversity/SouthUniversityPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult FOLLOWMD(bool? signup)
        {
            var OrgId = ReadOrganization("FOLLOWMD");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = true;
                TempData["BonafideConfirmationText"] = "BY PROGRESSING YOU CONFIM THAT YOU ARE A BONAFIDE PATIENT OF FOLLOWMD";
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "FOLLOWMD";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                ViewData["SignUp"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("FOLLOWMD/FOLLOWMD");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult FOLLOWMDPrograms()
        {
            var OrgId = ReadOrganization("FOLLOWMD");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = true;
                TempData["BonafideConfirmationText"] = "BY PROGRESSING YOU CONFIM THAT YOU ARE A BONAFIDE PATIENT OF FOLLOWMD";
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "FOLLOWMD";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("FOLLOWMD/FOLLOWMDPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Poplar(bool? signup)
        {
            var OrgId = ReadOrganization("Poplar");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = true;
                TempData["BonafideConfirmationText"] = "BY PROGRESSING YOU CONFIRM THAT YOU ARE A BONAFIDE PATIENT OF THE ABOVE DOCTOR";
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Poplar";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                ViewData["SignUp"] = signup;
                ViewData["ProvidersList"] = CommonUtility.GetProvidersList(OrgId.Value).Where(x => x.Active).OrderBy(x => x.Name).ToList();
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Poplar/Poplar");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult PoplarPrograms()
        {
            var OrgId = ReadOrganization("Poplar");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = true;
                TempData["BonafideConfirmationText"] = "BY PROGRESSING YOU CONFIRM THAT YOU ARE A BONAFIDE PATIENT OF THE ABOVE DOCTOR";
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Poplar";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                ViewData["ProvidersList"] = CommonUtility.GetProvidersList(OrgId.Value).Where(x => x.Active).OrderBy(x => x.Name).ToList();
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Poplar/PoplarPrograms");
            }
            else
                return RedirectToAction("Index");
        }
        public ActionResult MCITheDoctorsOffice(bool? signup)
        {
            var OrgId = ReadOrganization("MCI The Doctor's Office");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "MCI The Doctor's Office";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                ViewData["SignUp"] = signup;
                ViewData["ProvidersList"] = CommonUtility.GetProvidersList(OrgId.Value).Where(x => x.Active).OrderBy(x => x.Name).ToList();
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("MCITheDoctorsOffice/MCITheDoctorsOffice");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MCITheDoctorsOfficePrograms()
        {
            var OrgId = ReadOrganization("MCI The Doctor's Office");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "MCI The Doctor's Office";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                ViewData["ProvidersList"] = CommonUtility.GetProvidersList(OrgId.Value).Where(x => x.Active).OrderBy(x => x.Name).ToList();
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("MCITheDoctorsOffice/MCITheDoctorsOfficePrograms");
            }
            else
                return RedirectToAction("Index");
        }
        public ActionResult CityofPooler(bool? signup)
        {
            var OrgId = ReadOrganization("City of Pooler");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "City of Pooler";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L4260");
                ViewData["SignUp"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("CityofPooler/CityofPooler");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CityofPoolerPrograms()
        {
            var OrgId = ReadOrganization("City of Pooler");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "City of Pooler";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("CityofPooler/CityofPoolerPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MetLifeGulf(bool? signup)
        {
            var OrgId = ReadOrganization("MetLife-Gulf", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "MetLife Gulf";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L4258");
                ViewData["SignUp"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("MetLifeGulf/MetLifeGulf");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MetLifeGulfPrograms()
        {
            var OrgId = ReadOrganization("MetLife-Gulf", true);
            if (OrgId.HasValue)
            {
                var portal = new AccountManager().CurrentPortalId(OrgId.Value);
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "MetLife Gulf";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("MetLifeGulf/MetLifeGulfPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult MetLifeGulfFAQs()
        {
            var OrgId = ReadOrganization("MetLife-Gulf", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                return View("MetLifeGulf/MetLifeGulfFAQs");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult eBen(bool? signup)
        {
            var OrgId = ReadOrganization("eBen");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "eBen";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L4260");
                ViewData["SignUp"] = signup;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("eBen/eBen");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult eBenPrograms()
        {
            var OrgId = ReadOrganization("eBen");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "eBen";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("eBen/eBenPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult LMCSelfHelp()
        {
            var OrgId = ReadOrganization("LMC Self Help");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("LMCSelfHelp/LMCSelfHelp");
            }
            else
                return RedirectToAction("Index");
        }

        public int? ReadOrganization(string orgName, bool skipContactNumber = false)
        {
            var response = PortalUtility.ReadOrganization(orgName, null);
            if (response != null && response.organization != null)
            {
                TempData["OrgContactEmail"] = response.organization.ContactEmail;
                HttpContext.Session.SetString(SessionContext.OrgContactEmail, response.organization.ContactEmail);
                if (!skipContactNumber)
                {
                    TempData["OrgContactNumber"] = response.organization.ContactNumber;
                    HttpContext.Session.SetString(SessionContext.OrgContactNumber, response.organization.ContactEmail);
                }
                else
                {
                    TempData["OrgContactNumber"] = null;
                    HttpContext.Session.SetString(SessionContext.OrgContactNumber, "");
                }
                TempData["OrgURL"] = response.organization.Url;
                HttpContext.Session.SetString(SessionContext.LandingPage, response.organization.Url);
                return response.organization.Id;
            }
            else
                return null;
        }

        public ActionResult ServiceAgreement(bool? modal, bool? closeModal, bool? mobileView)
        {
            HomeModel model = new HomeModel();
            model.IsModal = modal.HasValue ? modal.Value : false;
            model.CloseModal = closeModal.HasValue ? closeModal.Value : false;
            TempData["TermsPage"] = model.MobileView = mobileView.HasValue ? mobileView.Value : false;
            return PartialView("ServiceAgreement", model);
        }

        public ActionResult GoogleOAuth()
        {
            return View();
        }

        public JsonResult ChangeLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                HttpContext.Session.Remove(SessionContext.LanguagePreference);
            }
            else
            {
                HttpContext.Session.SetString(SessionContext.LanguagePreference, language);
            }
            return Json(new { Result = true });
        }

        public ActionResult Edlogics()
        {
            SamlUtility utility = new SamlUtility();
            //  utility.SamlSPRedirectToIdp(Response);
            return View();
        }

        #region Retail
        public ActionResult tlc(bool? signup)
        {
            var OrgId = ReadOrganization("Retail");
            if (OrgId.HasValue)
            {
                ViewData["SignUp"] = signup;
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Retail";
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Retail/Retail");
            }
            else
                return RedirectToAction("Index");
        }


        public ActionResult tlcPrograms()
        {
            var OrgId = ReadOrganization("Retail");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Retail";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("Retail/RetailPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        #endregion

        #region HealthBFW
        public ActionResult HealthBFW(bool? signup)
        {
            var OrgId = ReadOrganization("Health Begins From Within", true);
            if (OrgId.HasValue)
            {
                ViewData["SignUp"] = signup;
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "HealthBFW";
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("HealthBFW/HealthBFW");
            }
            else
                return RedirectToAction("Index");
        }


        public ActionResult HealthBFWPrograms()
        {
            var OrgId = ReadOrganization("Health Begins From Within", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "HealthBFW";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("HealthBFW/HealthBFWPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult NewCo(bool? signup)
        {
            var OrgId = ReadOrganization("New Co");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = false;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("NewCo/NewCo");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult JPMC(bool? signup)
        {
            var OrgId = ReadOrganization("JPMC Main", true);
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["SignUp"] = signup;
                TempData["ShowSID"] = true;
                TempData["BonafideConfirmation"] = false;
                if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
                    return RedirectToAction("Stream", "Participant");
                return View("JPMC/JPMC");
            }
            else
                return RedirectToAction("Index");
        }

        #endregion
        public ActionResult Testimonial()
        {
            return View();
        }

        public ActionResult SessionExpired()
        {
            return View();
        }

        public ActionResult CanRisk(string utm_source = "", string utm_medium = "", string utm_campaign = "", string utm_keywords = "")
        {
            var OrgId = ReadOrganization("LMC Clinical Trial");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                TempData["utm_source"] = utm_source;
                TempData["utm_medium"] = utm_medium;
                TempData["utm_campaign"] = utm_campaign;
                TempData["utm_keywords"] = utm_keywords;
                CanriskModel model = new CanriskModel
                {
                    GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                    RaceMaleList = ListOptions.GetCanriskMaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                    RaceFemaleList = ListOptions.GetCanriskFemaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                    years = CommonUtility.GetYears(false).Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                    months = CommonUtility.GetMonths().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                    days = CommonUtility.GetDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                };
                var measurementsImperial = CommonUtility.ListMeasurements(1);
                var measurementsMetric = CommonUtility.ListMeasurements(2);
                model.measurementsImperial = measurementsImperial.Measurements;
                model.measurementsMetric = measurementsMetric.Measurements;

                return View("CanRisk", model);
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CanRisk1()
        {
            var OrgId = ReadOrganization("LMC Clinical Trial");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "DD/MM/YYYY";
                CanriskModel model = new CanriskModel
                {
                    GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                    RaceMaleList = ListOptions.GetCanriskMaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                    RaceFemaleList = ListOptions.GetCanriskFemaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                    years = CommonUtility.GetYears(false).Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                    months = CommonUtility.GetMonths().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                    days = CommonUtility.GetDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                };
                var measurementsImperial = CommonUtility.ListMeasurements(1);
                var measurementsMetric = CommonUtility.ListMeasurements(2);
                model.measurementsImperial = measurementsImperial.Measurements;
                model.measurementsMetric = measurementsMetric.Measurements;
                model.languagePreference = HttpContext.Session.GetString(SessionContext.LanguagePreference);
                return View("CanRisk1", model);
            }
            else
                return RedirectToAction("Index");
        }

        public PartialViewResult CanRiskQuestionnaire(int? EligibilityId, int? UserId, string utm_source = "", string utm_medium = "", string utm_campaign = "", string utm_keywords = "")
        {
            ViewData["OrgId"] = ReadOrganization("LMC Clinical Trial");
            ViewData["DateFormat"] = HttpContext.Session.GetString(SessionContext.DateFormat) != null ? HttpContext.Session.GetString(SessionContext.DateFormat).ToUpper() : "DD/MM/YYYY";
            CanriskModel model = new CanriskModel
            {
                GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                RaceMaleList = ListOptions.GetCanriskMaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                RaceFemaleList = ListOptions.GetCanriskFemaleRaceList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value }),
                years = CommonUtility.GetYears(false).Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                months = CommonUtility.GetMonths().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }),
                days = CommonUtility.GetDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value })
            };
            model.HasAdminRole = CommonUtility.HasAdminRole(User.RoleCode());
            var measurementsImperial = CommonUtility.ListMeasurements(1);
            var measurementsMetric = CommonUtility.ListMeasurements(2);
            model.measurementsImperial = measurementsImperial.Measurements;
            model.measurementsMetric = measurementsMetric.Measurements;
            if (EligibilityId.HasValue)
            {
                model.canrisk = ParticipantUtility.GetCanriskResponse("", EligibilityId);
                var eligibility = ParticipantUtility.GetEligibility(EligibilityId, "", null).Eligibility;
                if (eligibility != null)
                {
                    if (model.canrisk == null)
                        model.canrisk = new CanriskQuestionnaireDto();
                    else if (model.canrisk.CanriskScore.HasValue)
                    {
                        model.canrisk.DOB = eligibility.DOB;
                        model.canriskEligibility = ParticipantUtility.GetCanriskEligibility(model, model.canrisk.CanriskScore.Value);
                    }
                    model.canrisk.DOB = eligibility.DOB;
                    if (eligibility.Gender != null)
                        model.canrisk.Gender = eligibility.Gender.Key == GenderDto.Male.Key ? (byte)1 : (byte)2;
                    model.canrisk.Zip = eligibility.Zip;
                    model.canrisk.Name = eligibility.FirstName + " " + eligibility.LastName;
                    string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
                    if (!String.IsNullOrEmpty(eligibility.Email) && System.Text.RegularExpressions.Regex.IsMatch(eligibility.Email, regexPattern))
                    {
                        model.canrisk.Email = eligibility.Email;
                    }
                    model.canrisk.PhoneNumber = eligibility.HomeNumber;
                    model.canrisk.EligibilityId = eligibility.Id;
                }
            }
            model.UserId = UserId;
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            if (HttpContext.Session.GetString(SessionContext.LanguagePreference) != null)
                model.languagePreference = HttpContext.Session.GetString(SessionContext.LanguagePreference);
            return PartialView("_CanriskQuestionnaire", model);
        }

        public ActionResult LMC()
        {
            var OrgId = ReadOrganization("LMC Clinical Trial");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["LanguagePreference"] = HttpContext.Session.GetString(SessionContext.LanguagePreference); ;
                if (HttpContext.Session.GetString(SessionContext.LanguagePreference) == null && HttpContext.Request.Cookies["language-preference"] != null)
                {
                    HttpContext.Session.SetString(SessionContext.LanguagePreference, HttpContext.Request.Cookies["language-preference"]);
                }
                ViewData["DateFormat"] = "DD/MM/YYYY";
                return View("LMC/LMC");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult CAPTIVA()
        {
            var OrgId = ReadOrganization("Captiva");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "MetLife Gulf";
                ViewData["IDText"] = Translate.Message("L2458");
                ViewData["IDInfo"] = Translate.Message("L2457");
                if (HttpContext.Session.GetString(SessionContext.LanguagePreference) == null && HttpContext.Request.Cookies["language-preference"] != null)
                {
                    HttpContext.Session.SetString(SessionContext.LanguagePreference, HttpContext.Request.Cookies["language-preference"]);
                }
                return View("CAPTIVA/CAPTIVA");
            }
            else
                return RedirectToAction("Index");
        }
        public ActionResult TeamsBP()
        {
            var OrgId = ReadOrganization("TEAMS-BP");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                ViewData["DateFormat"] = "MM/DD/YYYY";
                ViewData["Name"] = "Teams BP";
                if (HttpContext.Session.GetString(SessionContext.LanguagePreference) == null && HttpContext.Request.Cookies["language-preference"] != null)
                {
                    HttpContext.Session.SetString(SessionContext.LanguagePreference, HttpContext.Request.Cookies["language-preference"]);
                }
                return View("TeamsBP/TeamsBP");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult LMCFAQs()
        {
            var OrgId = ReadOrganization("LMC Clinical Trial");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                return View("LMC/LMCFAQs");
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult LMCPrograms()
        {
            var OrgId = ReadOrganization("LMC Clinical Trial");
            if (OrgId.HasValue)
            {
                ViewData["OrgId"] = OrgId;
                return View("LMC/LMCPrograms");
            }
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult SendUserMessage(ContactUsModel contactUs)
        {
            string body = "<p>Name: " + contactUs.name + "<br />Email: " + contactUs.email + " <br />Message: " + contactUs.meassage + "</p>";
            CommonUtility.SendEmail(contactUs.organizationEmail, body, "Message from " + contactUs.subject + " user", _appSettings.InfoEmail, _appSettings.SecureEmail, _appSettings.SMPTAddress, _appSettings.PortNumber, _appSettings.SecureEmailPassword, _appSettings.MailAttachmentPath);
            return Json("success");
        }

        [HttpPost]
        public JsonResult TrackCanriskParticipation(int? eligibilityId, string dob, byte gender, string guid = "", string reasonId = "", string reason = "", int page = 0)
        {
            if (string.IsNullOrEmpty(guid))
            {
                if (eligibilityId.HasValue)
                {
                    var checkGuid = ParticipantUtility.GetGuid(eligibilityId);
                    guid = (checkGuid == null) ? Guid.NewGuid().ToString() : checkGuid;
                }
                else
                {
                    guid = Guid.NewGuid().ToString();
                }
            }
            var utm_source = TempData.Peek("utm_source")?.ToString();
            var utm_keywords = TempData.Peek("utm_keywords")?.ToString();
            var utm_medium = TempData.Peek("utm_medium")?.ToString();
            var utm_campaign = TempData.Peek("utm_campaign")?.ToString();
            int? canriskReasonId = null;

            if (!string.IsNullOrWhiteSpace(reasonId))
                canriskReasonId = Convert.ToInt32(reasonId);
            ParticipantUtility.TrackCanriskParticipation(guid, page, utm_source, utm_medium, utm_campaign, utm_keywords, canriskReasonId, reason, eligibilityId, Convert.ToDateTime(dob), gender);
            return Json(guid);
        }

        [HttpPost]
        public JsonResult SubmitCanriskQuestionnaire(CanriskModel model, int OrganizationId)
        {
            var Measurements = CommonUtility.ListMeasurements(2).Measurements;
            if (ParticipantUtility.CheckIfExists(model))
            {
                return Json(new { success = false });
            }
            if (!model.WeightUnit)
                model.canrisk.Weight = (float)(model.canrisk.Weight * Measurements[BioLookup.Weight].ConversionValue);
            if (!model.HeightUnit)
                model.canrisk.Height = (float)(model.canrisk.Height / Measurements[BioLookup.Height].ConversionValue);
            if (!model.WaistUnit)
                model.canrisk.Waist = (float)(model.canrisk.Waist / Measurements[BioLookup.Waist].ConversionValue);
            if (model.canrisk.Glucose.HasValue)
                model.canrisk.Glucose = (float)(model.canrisk.Glucose * Measurements[BioLookup.Glucose].ConversionValue);
            var score = ParticipantUtility.GetCanriskScore(model);
            model.canrisk.CanriskScore = score;
            var canriskEligibility = ParticipantUtility.GetCanriskEligibility(model, score);
            model.canrisk.utm_source = TempData["utm_source"]?.ToString();
            model.canrisk.utm_medium = TempData["utm_medium"]?.ToString();
            model.canrisk.utm_campaign = TempData["utm_campaign"]?.ToString();
            model.canrisk.utm_keywords = TempData["utm_keywords"]?.ToString();
            if (HttpContext.Session.GetInt32(SessionContext.UserId).HasValue)
            {
                model.canrisk.CompletedBy = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            }
            var response = ParticipantUtility.SubmitCanriskQuestionnaire(model, OrganizationId, canriskEligibility.isEligible, HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "");
            return Json(new { success = true, score = score, uniqueId = response, canriskEligibility = canriskEligibility });
        }

        [HttpPost]
        public JsonResult SubmitCanriskQuestionnaire1(CanriskModel model)
        {
            var Measurements = CommonUtility.ListMeasurements(2).Measurements;
            if (!model.WeightUnit)
                model.canrisk.Weight = (float)(model.canrisk.Weight * Measurements[BioLookup.Weight].ConversionValue);
            if (!model.HeightUnit)
                model.canrisk.Height = (float)(model.canrisk.Height / Measurements[BioLookup.Height].ConversionValue);
            if (!model.WaistUnit)
                model.canrisk.Waist = (float)(model.canrisk.Waist / Measurements[BioLookup.Waist].ConversionValue);
            if (model.canrisk.Glucose.HasValue)
                model.canrisk.Glucose = (float)(model.canrisk.Glucose * Measurements[BioLookup.Glucose].ConversionValue);
            var score = ParticipantUtility.GetCanriskScore(model);
            model.canrisk.CanriskScore = score;
            var canriskEligibility = ParticipantUtility.GetCanriskEligibility(model, score);
            return Json(new { success = true, score = score, canriskEligibility = canriskEligibility });
        }

        [HttpPost]
        public JsonResult CheckIfEligible(string postalCode)
        {
            return Json(new { Success = ParticipantUtility.CheckIfEligible(postalCode) });
        }
        /*public ActionResult GetPdf(string fileName)
        {
            var path = environment.ContentRootPath,"/pdf");
            var file = Path.Combine(path, fileName);
            file = Path.GetFullPath(file);
            if (!file.StartsWith(path))
            {
                throw new HttpException(403, "Forbidden");
            }
            return File(file, "application/pdf");
        }*/

        [HttpPost]
        public JsonResult GetDateList()
        {
            DateItem model = new DateItem();
            model.years = CommonUtility.GetYears(true).Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.months = CommonUtility.GetMonths().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.days = CommonUtility.GetDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            return Json(new { date = model });
        }

        public ActionResult COVID19()
        {
            return View();
        }

        public ActionResult DemoWebForm()
        {
            return View();
        }

        public ActionResult AboutApp()
        {
            return View();
        }

        public ActionResult DeviceVerification()
        {
            return PartialView("_DeviceVerification");
        }
    }
}