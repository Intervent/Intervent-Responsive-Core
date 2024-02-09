using Intervent.Web.DTO;
using Intervent.Web.DTO.Diff;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;

namespace InterventWebApp
{
    public class CRMController : BaseController
    {
        private readonly AppSettings _appSettings;

        public CRMController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [Authorize]
        public ActionResult CRM(bool? BackToCRM)
        {
            CRMProfileModel model = new CRMProfileModel();
            if (BackToCRM.HasValue && BackToCRM.Value)
            {
                model.tempData = JsonConvert.DeserializeObject<FilterReportTempData>(TempData["filterReportData"] as string);
            }
            model.CSRLists = SchedulerUtility.GetCoachList(null, null, HttpContext.Session.GetInt32(SessionContext.OrganizationId), HttpContext.Session.GetInt32(SessionContext.AdminId), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId)).users.Where(x => x.RoleCode == "CSR").Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.ListOrganizations = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.Countries = CommonUtility.ListCountries().Where(x => x.Code == "US" || x.Code == "CA").OrderBy(t => t.Code == "US" ? 1 : 2).ThenBy(t => t.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.States = Enumerable.Empty<SelectListItem>();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddEditCRMProfile(CRMProfileModel model)
        {
            var response = CRMUtility.AddEditCRMProfile(model.CRM_Contact, HttpContext.Session.GetInt32(SessionContext.UserId).Value, _appSettings.IntuityOrgId);

            return Json(new { response = response });
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddEditEligibilityToCRM(string uniqueId, int crmId, int? eligibilityOrgId)
        {
            var response = CRMUtility.AddEditEligibilityToCRM(uniqueId, crmId, eligibilityOrgId, _appSettings.IntuityOrgId);
            return Json(new { response = response });
        }


        [Authorize]
        [HttpPost]
        public JsonResult AddEditNotes(CRMProfileModel model)
        {
            var response = CRMUtility.AddEditCRMNotes(model.CRM_Note, HttpContext.Session.GetInt32(SessionContext.UserId).Value, _appSettings.IntuityOrgId);

            return Json(new { Result = "OK", response = response });
        }

        [Authorize]
        public JsonResult SearchHCPList(string search)
        {
            var response = CRMUtility.ListHCP(search);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var hcpList = response.Select(x => new
            {
                HCPId = x.Id,
                label = textInfo.ToTitleCase(x.FirstName).ToLower() + " " + textInfo.ToTitleCase(x.LastName).ToLower(),
                LastName = textInfo.ToTitleCase(x.LastName).ToLower(),
                City = textInfo.ToTitleCase(x.City).ToLower(),
                State = textInfo.ToTitleCase(x.State1.Name).ToLower()
            });
            return Json(new { Result = "OK", HCPList = hcpList });
        }

        [Authorize]
        public JsonResult CheckUnhandledForm()
        {
            return Json(new { result = CRMUtility.SearchUnhandledForm() });
        }

        [Authorize]
        public JsonResult CRMSearchRecord(CRMSearchModel model)
        {
            var response = CRMUtility.SearchUsers(model);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            IList<CRM_DispositionsDto> Dispositions = CRMUtility.GetDispositionsList().CRM_Dispositions;
            var data = new
            {
                response.TotalRecords,
                Records = response.CRM_ContactDto.Select(x => new
                {
                    x.Id,
                    Name = x.FirstName + " " + x.LastName,
                    Email = x.Email ?? "",
                    Note = x.CRM_Notes.Count != 0 ? x.CRM_Notes.First().Notes : "",
                    Type = x.CRM_Notes.Count != 0 ? x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().Type == 2 ? "Call" : x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().Type == 1 ? "Email" : x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().Type == 3 ? "Online Webform" : "IMI Call" : "",
                    Disposition = x.CRM_Notes.Count != 0 ? x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().Disposition != null ? Dispositions.Where(d => d.Id == x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().Disposition).Select(d => d.Disposition).FirstOrDefault() : "" : "",
                    isComplaint = x.CRM_Notes.Count != 0 ? x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().Disposition != null ? Dispositions.Where(e => e.Id == x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().Disposition).Select(e => e.Complaint).FirstOrDefault() : false : false,
                    Date = x.CRM_Notes.Count != 0 ? x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().CreatedOn != null ? DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().CreatedOn, custTZone).ToString()).ToString("dd MMM yyyy") : "-" : "-",
                    Time = x.CRM_Notes.Count != 0 ? x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().CreatedOn != null ? DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(x.CRM_Notes.OrderByDescending(c => c.CreatedOn).First().CreatedOn, custTZone).ToString()).ToString("hh:mm tt") : "" : "",
                    Phone = x.PhoneNumber1
                })
            };
            return Json(data);
        }

        [Authorize]
        public ActionResult AddEditNotes(int contactId, int? noteId)
        {
            CRMProfileModel model = new CRMProfileModel();
            model.CRM_Note = new CRM_NoteDto();
            model.CRM_Contact = CRMUtility.GetContact(contactId).CRM_Contact;
            if (noteId.HasValue)
                model.CRM_Note = model.CRM_Contact.CRM_Notes.Where(x => x.Id == noteId).FirstOrDefault();
            model.CRM_Note.ContactId = contactId;
            model.AccountTypes = CRMUtility.GetAccountTypes().CRM_AccountTypes.Where(x => x.IsActive).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.InquiryTypes = CRMUtility.GetInquiryTypes().InquiryTypes.Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            model.CallerProfileTypes = CRMUtility.CallerProfileTypes().CRM_CallerProfileTypes.Where(x => x.IsActive).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.ComplaintClassificationTypes = CRMUtility.ComplaintClassificationTypes().CRM_ComplaintClassificationTypes.Where(x => x.IsActive).Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            model.PogoMeterNumbers = CRMUtility.GetPogoMeterNumbers(contactId).Where(x => x.Id == model.CRM_Note.PogoMeterNumber || x.IsActive).Select(x => new SelectListItem { Text = x.PogoMeterNumber, Value = x.Id.ToString() });
            model.Dispositions = CRMUtility.GetDispositionsList().CRM_Dispositions.Where(d => d.Disposition == "Inbound email" || d.Disposition == "Outbound email" || d.Disposition == "Email Complaint" || d.Disposition == "Handled" || d.Disposition == "Complaint Follow up").Select(x => new SelectListItem { Text = x.Disposition, Value = x.Id.ToString() });
            model.States = CommonUtility.ListStates(236).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_CRMNotes", model);
        }


        [Authorize]
        public ActionResult AddEditCRMContact(int? contactId)
        {
            CRMProfileModel model = new CRMProfileModel();
            model.States = Enumerable.Empty<SelectListItem>();
            model.States2 = Enumerable.Empty<SelectListItem>();
            if (contactId.HasValue)
            {
                model.CRM_Contact = CRMUtility.GetContact(contactId.Value).CRM_Contact;
                if (model.CRM_Contact.Country != null)
                    model.States = CommonUtility.ListStates(model.CRM_Contact.Country.Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
                if (model.CRM_Contact.DOB.HasValue)
                    model.dob = model.CRM_Contact.DOB.Value.Date.ToShortDateString();
                if (model.CRM_Contact.Country2 != null)
                    model.States2 = CommonUtility.ListStates(model.CRM_Contact.Country2.Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
                model.CRM_Contact.CRM_PogoMeterNumbers = model.CRM_Contact.CRM_PogoMeterNumbers.Where(x => x.IsActive == true).ToList();
            }
            model.GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.InsuranceTypes = CRMUtility.GetInsuranceTypes().InsuranceTypes.Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            model.ListOrganizations = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.Countries = CommonUtility.ListCountries().Where(x => x.Code == "US" || x.Code == "CA").OrderBy(t => t.Code == "US" ? 1 : 2).ThenBy(t => t.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_CRMContact", model);
        }

        [Authorize]
        public ActionResult ListNotes(int contactId, int totalNotes, bool isComplaint, bool isRequiresRMA, string noteSearch, bool fromInfoPage)
        {
            CRMProfileModel model = GetCRMNotes(contactId, totalNotes, isComplaint, isRequiresRMA, noteSearch, fromInfoPage);
            FilterReportTempData crmTempData = new FilterReportTempData
            {
                contactId = contactId
            };
            TempData["filterReportData"] = JsonConvert.SerializeObject(crmTempData);
            return PartialView("_CRMListNotes", model);
        }

        [Authorize]
        public ActionResult ListNotesDetails(int contactId, int totalNotes, bool isComplaint, bool isRequiresRMA, string noteSearch, bool fromInfoPage)
        {
            CRMProfileModel model = GetCRMNotes(contactId, totalNotes, isComplaint, isRequiresRMA, noteSearch, fromInfoPage);
            return PartialView("_CRMNotesDetails", model);
        }

        public CRMProfileModel GetCRMNotes(int contactId, int totalNotes, bool isComplaint, bool isRequiresRMA, string noteSearch, bool fromInfoPage)
        {
            CRMProfileModel model = new CRMProfileModel();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            model.CRM_Contact = CRMUtility.GetContact(contactId).CRM_Contact;
            IList<CRM_DispositionsDto> Dispositions = CRMUtility.GetDispositionsList().CRM_Dispositions;
            if (isComplaint)
            {
                model.TotalRecords = model.CRM_Contact.CRM_Notes.Where(n => isRequiresRMA ? n.RequiredRMA == 1 : (n.RequiredRMA == 1 || n.RequiredRMA == 2 || n.RequiredRMA == null)).Where(m => string.IsNullOrEmpty(noteSearch) || (m.Notes != null && m.Notes.Contains(noteSearch))).OrderByDescending(c => c.CreatedOn).Where(d => d.Disposition == Dispositions.Where(e => e.Id == d.Disposition && e.Complaint == isComplaint).Select(f => f.Id).FirstOrDefault()).Count();
                model.CRM_Contact.CRM_Notes = model.CRM_Contact.CRM_Notes.Where(n => isRequiresRMA ? n.RequiredRMA == 1 : (n.RequiredRMA == 1 || n.RequiredRMA == 2 || n.RequiredRMA == null)).Where(m => string.IsNullOrEmpty(noteSearch) || (m.Notes != null && m.Notes.Contains(noteSearch))).OrderByDescending(c => c.CreatedOn).Where(d => d.Disposition == Dispositions.Where(e => e.Id == d.Disposition && e.Complaint == isComplaint).Select(f => f.Id).FirstOrDefault()).Take(totalNotes).ToList();
            }
            else
            {
                model.TotalRecords = model.CRM_Contact.CRM_Notes.Where(n => isRequiresRMA ? n.RequiredRMA == 1 : (n.RequiredRMA == 1 || n.RequiredRMA == 2 || n.RequiredRMA == null)).Where(m => string.IsNullOrEmpty(noteSearch) || (m.Notes != null && m.Notes.Contains(noteSearch))).Count();
                model.CRM_Contact.CRM_Notes = model.CRM_Contact.CRM_Notes.Where(n => isRequiresRMA ? n.RequiredRMA == 1 : (n.RequiredRMA == 1 || n.RequiredRMA == 2 || n.RequiredRMA == null)).Where(m => string.IsNullOrEmpty(noteSearch) || (m.Notes != null && m.Notes.Contains(noteSearch))).OrderByDescending(c => c.CreatedOn).Take(totalNotes).ToList();
            }
            if (model.CRM_Contact.CRM_Notes.Count > 0)
            {
                model.CRM_Contact.CRM_Notes = model.CRM_Contact.CRM_Notes.Select(d => { d.CreatedOnDateText = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(d.CreatedOn, custTZone).ToString()).ToString("dd MMM yyyy"); return d; }).ToList();
                model.CRM_Contact.CRM_Notes = model.CRM_Contact.CRM_Notes.Select(t => { t.CreatedOnTimeText = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(t.CreatedOn, custTZone).ToString()).ToString("hh:mm tt"); return t; }).ToList();
            }
            model.fromInfoPage = fromInfoPage;
            if (!string.IsNullOrEmpty(model.CRM_Contact.UniqueId))
            {
                model.eligibilityId = CRMUtility.GetEligibilityByUniqueId(model.CRM_Contact.UniqueId).Id.Value;
                var userResponse = AccountUtility.GetUserByUniqueId(null, model.CRM_Contact.UniqueId);
                if (userResponse.User != null)
                    model.userId = userResponse.User.Id;
            }
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return model;
        }

        [Authorize]
        public JsonResult GetChangeLog(int refId, bool isContactLog)
        {
            var changeLogHistory = CRMUtility.GetChangeLog(refId, isContactLog).crm_ChangeLog.ToList();
            var changeLog = ChangeLogDetailedInfo(changeLogHistory);
            return Json(new { Result = "OK", changeLog = changeLog });
        }

        public IList<ParticipantHistoryChangesModel> ChangeLogDetailedInfo(IList<CRM_ChangeLogDto> changeLogHistory)
        {
            IList<ParticipantHistoryChangesModel> participantHistoryChangesModel = new List<ParticipantHistoryChangesModel>();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            if (changeLogHistory != null)
            {
                IEnumerable<string> tinyintDataTypeProperties = TinyIntDataTypeProperties();
                IEnumerable<string> dropdownDataTypeProperties = DropdownDataTypeProperties();
                IEnumerable<string> dateTimeProperties = DateTimeProperties();
                IDictionary<string, string> propertyName = PropertyNameMapper();
                foreach (var changeLog in changeLogHistory)
                {
                    IList<PropertyCompare> changes = JsonConvert.DeserializeObject<IList<PropertyCompare>>(changeLog.Changes);
                    foreach (PropertyCompare change in changes)
                    {
                        if (tinyintDataTypeProperties.Contains(change.PropertyName))
                        {
                            if (!string.IsNullOrEmpty(change.CurrentValue))
                            {
                                if (change.CurrentValue == "1")
                                {
                                    change.CurrentValue = "Yes";
                                }
                                else if (change.CurrentValue == "2")
                                {
                                    change.CurrentValue = "No";
                                }
                                else if (change.CurrentValue == "3")
                                {
                                    change.CurrentValue = "NA";
                                }
                            }
                            if (!string.IsNullOrEmpty(change.NewValue))
                            {
                                if (change.NewValue == "1")
                                {
                                    change.NewValue = "Yes";
                                }
                                else if (change.NewValue == "2")
                                {
                                    change.NewValue = "No";
                                }
                                else if (change.NewValue == "3")
                                {
                                    change.NewValue = "NA";
                                }
                            }
                        }
                        else if (dropdownDataTypeProperties.Contains(change.PropertyName))//dropdown
                        {
                            if (!string.IsNullOrEmpty(change.CurrentValue))
                            {
                                change.CurrentValue = GetDropDownVal(change.PropertyName, change.CurrentValue);
                            }
                            if (!string.IsNullOrEmpty(change.NewValue))
                            {
                                change.NewValue = GetDropDownVal(change.PropertyName, change.NewValue);
                            }
                        }
                        else if (dateTimeProperties.Contains(change.PropertyName))
                        {
                            if (!string.IsNullOrEmpty(change.CurrentValue))
                            {
                                change.CurrentValue = Convert.ToDateTime(change.CurrentValue).ToString(HttpContext.Session.GetString(SessionContext.DateFormat));
                            }
                            if (!string.IsNullOrEmpty(change.NewValue))
                            {
                                change.NewValue = Convert.ToDateTime(change.NewValue).ToString(HttpContext.Session.GetString(SessionContext.DateFormat));
                            }
                        }
                        if (propertyName.ContainsKey(change.PropertyName))
                        {
                            change.PropertyName = propertyName.Where(v => v.Key == change.PropertyName).Select(v => v.Value).FirstOrDefault();
                        }
                    }
                    changes.Remove(changes.Where(x => x.PropertyName.Equals("UpdatedOn")).FirstOrDefault());
                    changes.Remove(changes.Where(x => x.PropertyName.Equals("UpdatedBy")).FirstOrDefault());
                    var property = new PropertyCompare();
                    property.NewValue = DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(changeLog.LogDate, custTZone).ToString()).ToString("dd MMM yyyy") + "<span>" + DateTime.Parse(TimeZoneInfo.ConvertTimeFromUtc(changeLog.LogDate, custTZone).ToString()).ToString("hh:mm tt") + "</span>";
                    property.PropertyName = "Updated On";
                    changes.Add(property);
                    property = new PropertyCompare();
                    property.NewValue = changeLog.User.FirstName + " " + changeLog.User.LastName;
                    property.PropertyName = "Updated By";
                    changes.Add(property);
                    var test = new ParticipantHistoryChangesModel();
                    test.Changes = changes;
                    participantHistoryChangesModel.Add(test);
                }
            }
            return participantHistoryChangesModel;
        }

        static IEnumerable<string> TinyIntDataTypeProperties()
        {
            List<string> lst = new List<string>();
            lst.Add("RequiredEscalation");
            lst.Add("RequiredRMA");
            lst.Add("FirstCallResolution");
            lst.Add("OptedIn");
            return lst;
        }

        static IEnumerable<string> DropdownDataTypeProperties()
        {
            List<string> lst = new List<string>();
            lst.Add("Type");
            lst.Add("CallerProfileType");
            lst.Add("AccountType");
            lst.Add("ComplaintClassificationType");
            lst.Add("Disposition");
            lst.Add("State");
            lst.Add("Country");
            lst.Add("State2");
            lst.Add("Country2");

            return lst;
        }

        static IEnumerable<string> DateTimeProperties()
        {
            List<string> lst = new List<string>();
            lst.Add("DOB");

            return lst;
        }

        static IDictionary<string, string> PropertyNameMapper()
        {
            IDictionary<string, string> propertyName = new Dictionary<string, string>();
            propertyName.Add("FirstName", "First Name");
            propertyName.Add("LastName", "Last Name");
            propertyName.Add("PhoneNumber1", "Phone Number 1");
            propertyName.Add("PhoneNumber2", "Phone Number 2");
            propertyName.Add("PhoneNumber3", "Phone Number 3");
            propertyName.Add("PogoMeterNumber", "Pogo Meter Number");
            propertyName.Add("CallerProfileType", "Caller Profile");
            propertyName.Add("FullName", "Full Name");
            propertyName.Add("AccountType", "Account Type");
            propertyName.Add("RequiredEscalation", "Required Escalation");
            propertyName.Add("RequiredRMA", "RMA Required");
            propertyName.Add("RMANumber", "RMA Number");
            propertyName.Add("ComplaintClassificationType", "Complaint Classification");
            propertyName.Add("FirstCallResolution", "First Call Resolution");
            propertyName.Add("MasterControlNo", "Master Control Number");
            propertyName.Add("CartridgeLotNumber", "Cartridge Lot Number");
            propertyName.Add("Address2", "Secondary Address");
            propertyName.Add("City2", "Secondary City");
            propertyName.Add("State2", "Secondary State");
            propertyName.Add("Country2", "Secondary Country");
            propertyName.Add("Zip2", "Secondary Zip");
            propertyName.Add("OptedIn", "Opted-in");

            return propertyName;
        }

        public string GetDropDownVal(string property, string value)
        {
            var result = "";

            if (property.Equals("Type"))
            {
                result = ListOptions.GetNoteType().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).Where(x => x.Value == value).FirstOrDefault().Text;
                return result;
            }
            if (property.Equals("CallerProfileType"))
            {
                result = CRMUtility.CallerProfileTypes().CRM_CallerProfileTypes.Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("AccountType"))
            {
                result = CRMUtility.GetAccountTypes().CRM_AccountTypes.Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("ComplaintClassificationType"))
            {
                result = CRMUtility.ComplaintClassificationTypes().CRM_ComplaintClassificationTypes.Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("Disposition"))
            {
                result = CRMUtility.GetDispositionsList().CRM_Dispositions.Select(x => new SelectListItem { Text = x.Disposition, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("State"))
            {
                result = CommonUtility.ListAllStates().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("Country"))
            {
                result = CommonUtility.ListCountries().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("State2"))
            {
                result = CommonUtility.ListAllStates().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("Country2"))
            {
                result = CommonUtility.ListCountries().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).Where(x => x.Value == value).FirstOrDefault().Text;
            }
            if (property.Equals("UpdatedBy"))
            {
                var user = ParticipantUtility.ReadUserParticipation(Convert.ToInt32(value.ToString())).user;
                result = user.FirstName + " " + user.LastName;
            }
            return result;
        }

        [Authorize]
        [HttpPost]
        public JsonResult ValidatePhoneNoExistence(string phoneNumber)
        {
            var response = CRMUtility.ValidatePhoneNoExistence(phoneNumber);

            return Json(new { isPhoneNumberExist = response });
        }

        [Authorize]
        [HttpPost]
        public JsonResult ValidateCouponCode(string couponCode, int contactId)
        {
            var response = CRMUtility.ValidateCouponCode(couponCode, contactId);

            return Json(new { isValidCouponCode = response.isValid });
        }
    }
}