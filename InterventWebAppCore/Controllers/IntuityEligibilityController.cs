using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace InterventWebApp
{
    public class IntuityEligibilityController : BaseController
    {
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpGet]
        public PartialViewResult GetIntuityDetails(string uniqueId, int organizationid, int portalId, string eligibilityStatus)
        {
            IntuityEligibilityModel model = new IntuityEligibilityModel();
            model.OrganizationCode = PortalUtility.ReadOrganization(null, organizationid).organization.Code;
            model.UniqueId = uniqueId;
            model.EligibilityStatus = eligibilityStatus;
            var response = IntuityUtility.GetIntuityDetails(uniqueId, organizationid, portalId);
            model.UserEligible = response.UserEligible;
            model.ShowNewForm = response.ShowNewForm;
            model.ShowFulfillment = response.ShowFulfillment;
            model.IntuityEligibility = response.IntuityEligibility;
            model.Countries = CommonUtility.ListCountries().Where(x => x.Code == "US").OrderBy(t => t.Code == "US" ? 1 : 2).ThenBy(t => t.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.States = CommonUtility.ListStates(int.Parse(model.Countries.FirstOrDefault().Value)).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.DiabetesTypes = ListOptions.GetDiabetesTypes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.EligibilityStatusList = ListOptions.GetEligibilityStatusList().Select(x => new SelectListItem { Text = x.DisplayText, Value = x.Value });
            model.EligibilityReasonsList = ListOptions.GetEligibilityReasonsList().Select(x => new SelectListItem { Text = x.DisplayText, Value = x.Value });
            model.NoA1cTestReason = ListOptions.GetA1cTestReasons().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            if (response.IntuityEligibility != null && response.IntuityEligibilityLog != null)
            {
                model.IsUserExists = IntuityUtility.GetUserDetails(uniqueId, organizationid).User != null;
                model.OrganizationId = organizationid;
                model.PortalId = portalId;
                model.OverrideStatus = response.OverrideStatus;
                model.IntuityEligibilityId = response.IntuityEligibilityId;
                model.IntuityEligibilityLog = response.IntuityEligibilityLog;
                model.IntuityFulfillments = response.IntuityFulfillments;
                model.IntuityQOH = response.IntuityQOH;
                model.FormSubmittedDate = response.FormSubmittedDate;
                model.PatternsRegDate = response.PatternsRegDate;
                model.IntuityFulfillmentRequests = JsonConvert.SerializeObject(response.IntuityFulfillmentRequests);
                if (model.IntuityEligibilityLog != null)
                {
                    if (model.IntuityEligibilityLog.Height.HasValue)
                    {
                        var height = CommonUtility.ToFeetInches(model.IntuityEligibilityLog.Height.Value);
                        model.HeightFeet = height.Key;
                        model.HeightInch = (float)Math.Round(height.Value, 2);
                    }
                    model.diabetesDate = model.IntuityEligibilityLog.DiabetesDate;
                    model.a1cTestDate = model.IntuityEligibilityLog.A1CTestDate;
                }
            }
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_IntuityEligibility", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult UpdateIntuityEligibility(IntuityEligibilityModel model)
        {
            UpdateIntuityResponse response = IntuityUtility.UpdateIntuityEligibility(model, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = response.Status, Reason = response.EligibilityReason, Status = response.EligibilityStatus });
        }

        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult UpdateEligiblityStatus(string unique_id, int organizationid, int eligiblityStatus, int eligiblityReason)
        {
            bool response = IntuityUtility.UpdateEligiblityStatus(unique_id, organizationid, eligiblityStatus, eligiblityReason, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", response });
        }

        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult UpdateIntuityFulfillmentRequest(int intuityEligibilityId, int quantity, string reason, bool sendMeter)
        {
            UpdateIntuityFulfillmentResponse response = IntuityUtility.UpdateIntuityFulfillmentRequest(intuityEligibilityId, quantity, reason, sendMeter, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", response.Status, FulfillmentRequestsList = JsonConvert.SerializeObject(response.IntuityFulfillmentRequests) });
        }

        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult UpdateIntuityQuantityonHand(int intuityEligibilityId, int quantity)
        {
            bool response = IntuityUtility.UpdateIntuityQuantityonHandRequest(intuityEligibilityId, quantity, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", response });
        }

        [ModuleControl(Modules.Eligibility, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult UpdateIntuityOptingOut(IntuityEligibilityModel model, bool OptingOut)
        {
            UpdateIntuityOptingOutResponse response = IntuityUtility.UpdateIntuityOptingOut(model, OptingOut, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(new { Result = "OK", response.Status, OptingOut = response.OptingOut.HasValue ? response.OptingOut.Value.ToShortDateString() : "" });
        }
    }
}