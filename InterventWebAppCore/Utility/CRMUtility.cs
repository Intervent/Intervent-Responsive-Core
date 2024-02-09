using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Newtonsoft.Json;
using NLog;
using System.Text.RegularExpressions;

namespace InterventWebApp
{
    public class CRMUtility
    {
        public static AddEditCRMProfileResponse ProcessWebForm(WebFormRequest request, int intuityOrgId, int systemAdminId)
        {
            AddEditCRMProfileResponse response = new AddEditCRMProfileResponse();
            try
            {
                CRMReader reader = new CRMReader();
                GetContactByEmailorPhoneNoRequest phoneNoRequest = new GetContactByEmailorPhoneNoRequest();
                phoneNoRequest.phoneNo = request.in_phone_no;
                phoneNoRequest.email = request.in_email_id;
                phoneNoRequest.byEmailorPhone = true;
                var contact = reader.GetContactByEmailorPhoneNo(phoneNoRequest).CRM_Contact;
                int crm_ContactId;

                if (contact == null)
                {
                    AddEditCRMProfileRequest crmRequest = new AddEditCRMProfileRequest();
                    crmRequest.userId = systemAdminId; //System Admin  
                    crmRequest.fromWebForm = true;
                    CRM_ContactDto crm_Contact = new CRM_ContactDto();
                    crm_Contact.FirstName = request.in_first_name;
                    crm_Contact.LastName = request.in_last_name;
                    crm_Contact.Email = request.in_email_id;
                    crm_Contact.PhoneNumber1 = request.in_phone_no;
                    crmRequest.crm_Contact = crm_Contact;
                    crmRequest.intuityOrgId = intuityOrgId;
                    var crmResponse = reader.AddEditCRMProfile(crmRequest);
                    crm_ContactId = crmResponse.contactId;
                }
                else
                {
                    bool needToUpdate = false;
                    if ((string.IsNullOrEmpty(contact.FirstName) || contact.FirstName.StartsWith("First")) && !string.IsNullOrEmpty(request.in_first_name))
                    {
                        contact.FirstName = request.in_first_name;
                        needToUpdate = true;
                    }
                    if ((string.IsNullOrEmpty(contact.LastName) || contact.LastName.StartsWith("Last")) && !string.IsNullOrEmpty(request.in_last_name))
                    {
                        contact.LastName = request.in_last_name;
                        needToUpdate = true;
                    }
                    if (string.IsNullOrEmpty(contact.Email) && !string.IsNullOrEmpty(request.in_email_id))
                    {
                        contact.Email = request.in_email_id;
                        needToUpdate = true;
                    }
                    if (!string.IsNullOrEmpty(request.in_phone_no) && (request.in_phone_no != contact.PhoneNumber1 && request.in_phone_no != contact.PhoneNumber2 && request.in_phone_no != contact.PhoneNumber3))
                    {
                        var phoneNumber = request.in_phone_no.Substring(0, 1);
                        if (phoneNumber == "1")
                            request.in_phone_no = request.in_phone_no.Remove(0, 1);
                        if (string.IsNullOrEmpty(contact.PhoneNumber1))
                        {
                            contact.PhoneNumber1 = request.in_phone_no;
                            needToUpdate = true;
                        }
                        else if (string.IsNullOrEmpty(contact.PhoneNumber2))
                        {
                            contact.PhoneNumber2 = request.in_phone_no;
                            needToUpdate = true;
                        }
                        else if (string.IsNullOrEmpty(contact.PhoneNumber3))
                        {
                            contact.PhoneNumber3 = request.in_phone_no;
                            needToUpdate = true;
                        }
                    }
                    if (needToUpdate)
                    {
                        AddEditCRMProfileRequest crmRequest = new AddEditCRMProfileRequest();
                        contact.UpdatedBy = systemAdminId; //System Admin
                        crmRequest.crm_Contact = contact;
                        crmRequest.fromWebForm = true;
                        crmRequest.intuityOrgId = intuityOrgId;
                        reader.AddEditCRMProfile(crmRequest);
                    }
                    crm_ContactId = contact.Id;
                }
                if (request.in_pogo_monitor)
                {
                    var pogoMonitorNumber = Regex.Replace(request.in_monitor_number, @"[ '&().,/:;-]+", "");
                    var pogoMeterNumber = reader.GetPogoMeterNumbers(crm_ContactId).Where(x => Regex.Replace(x.PogoMeterNumber, @"[ '&().,/:;-]+", "") == pogoMonitorNumber).FirstOrDefault();
                    if (pogoMeterNumber == null)
                    {
                        reader.AddPogoMeterNumber(crm_ContactId, pogoMonitorNumber);
                    }
                }
                AddEditCRMNoteRequest noteRequest = new AddEditCRMNoteRequest();
                noteRequest.userId = systemAdminId; //System Admin  
                CRM_NoteDto crm_Notes = new CRM_NoteDto();
                crm_Notes.ContactId = crm_ContactId;
                crm_Notes.Type = (byte)CRMNoteTypes.WebForm;
                crm_Notes.InquiryType = request.in_inquiry_type;
                crm_Notes.Notes = request.in_feedback;
                noteRequest.crm_Note = crm_Notes;
                noteRequest.intuityOrgId = intuityOrgId;
                reader.AddEditCRMNotes(noteRequest);
                response.success = true;
            }
            catch (Exception e)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(NLog.LogLevel.Error, "ProcessWebForm", null, "SaveWebForm Request : " + JsonConvert.SerializeObject(request) + ". Response : " + e.Message + "", null, e);
                logReader.WriteLogMessage(logEvent);
                response.success = false;
            }
            return response;
        }

        public static GetCRMContactResponse GetContact(int contactId)
        {
            CRMReader reader = new CRMReader();
            GetCRMContactRequest request = new GetCRMContactRequest();
            request.contactId = contactId;
            return reader.GetContact(request);
        }

        public static GetAccountTypesResponse GetAccountTypes()
        {
            CRMReader reader = new CRMReader();
            var response = reader.GetAccountTypes();
            return response;
        }
        public static GetCallerProfileTypesResponse CallerProfileTypes()
        {
            CRMReader reader = new CRMReader();
            var response = reader.CallerProfileTypes();
            return response;
        }

        public static InquiryTypesResponse GetInquiryTypes()
        {
            CRMReader reader = new CRMReader();
            var response = reader.GetInquiryTypes();

            return response;
        }

        public static GetComplaintClassificationTypes ComplaintClassificationTypes()
        {
            CRMReader reader = new CRMReader();
            var response = reader.ComplaintClassificationTypes();
            return response;
        }

        public static AddEditCRMProfileResponse AddEditCRMProfile(CRM_ContactDto crm_Contact, int userId, int intuityOrgId)
        {
            CRMReader reader = new CRMReader();
            AddEditCRMProfileRequest request = new AddEditCRMProfileRequest();
            request.crm_Contact = crm_Contact;
            request.userId = userId;
            request.intuityOrgId = intuityOrgId;
            var response = reader.AddEditCRMProfile(request);

            return response;
        }

        public static AddEditEligibilityToCRMResponse AddEditEligibilityToCRM(string uniqueId, int crmId, int? eligibilityOrgId, int intuityOrgId)
        {
            CRMReader reader = new CRMReader();
            var response = reader.AddEditEligibilityToCRM(new AddEditEligibilityToCRMRequest { UniqueId = uniqueId, CRMId = crmId, EligibilityOrgId = eligibilityOrgId, intuityOrgId = intuityOrgId });
            return response;
        }

        public static AddEditCRMNotesResponse AddEditCRMNotes(CRM_NoteDto crm_Notes, int userId, int intuityOrgId)
        {
            CRMReader reader = new CRMReader();
            AddEditCRMNoteRequest request = new AddEditCRMNoteRequest();
            request.userId = userId;
            request.crm_Note = crm_Notes;
            request.intuityOrgId = intuityOrgId;
            var response = reader.AddEditCRMNotes(request);
            return response;
        }

        public static IList<HCPListsDto> ListHCP(string search)
        {
            CRMReader reader = new CRMReader();
            return reader.ListHCP(search);
        }

        public static int SearchUnhandledForm()
        {
            CRMReader reader = new CRMReader();
            return reader.SearchUnhandledForms();
        }

        public static CRMSearchResponse SearchUsers(CRMSearchModel Searchrequest)
        {
            CRMReader reader = new CRMReader();
            CRMSearchRequest request = new CRMSearchRequest();
            request.FirstName = Searchrequest.FirstName;
            request.LastName = Searchrequest.LastName;
            request.CRMId = Searchrequest.crmId;
            request.Email = Searchrequest.Email;
            request.Phone = Searchrequest.Phone;
            request.RMAandQADNumber = Searchrequest.RMAandQADNumber;
            request.MasterControlNo = Searchrequest.MasterControlNo;
            request.CSRId = Searchrequest.CSRId;
            request.Page = Searchrequest.Page;
            request.PageSize = Searchrequest.PageSize;
            request.TotalRecords = Searchrequest.TotalRecords;
            request.ComplaintsSearch = Searchrequest.ComplaintsSearch;
            request.UnhandledWebforms = Searchrequest.UnhandledWebforms;
            return reader.SearchUsers(request);
        }

        public static GetChangeLogResponse GetChangeLog(int id, bool isContactLog)
        {
            CRMReader reader = new CRMReader();
            GetChangeLogRequest request = new GetChangeLogRequest();
            request.refId = id;
            request.isContactLog = isContactLog;
            var response = reader.GetChangeLog(request);
            return response;
        }

        public static GetDispositionsListResponse GetDispositionsList()
        {
            CRMReader reader = new CRMReader();
            return reader.GetDispositionsList();
        }

        public static List<CRM_PogoMeterNumbersDto> GetPogoMeterNumbers(int contactId)
        {
            CRMReader reader = new CRMReader();
            return reader.GetPogoMeterNumbers(contactId);
        }

        internal static bool ValidatePhoneNoExistence(string phoneNumber)
        {
            CRMReader reader = new CRMReader();
            GetContactByEmailorPhoneNoRequest phoneNoRequest = new GetContactByEmailorPhoneNoRequest();
            phoneNoRequest.phoneNo = phoneNumber;
            return reader.ValidatePhoneNoExistence(phoneNoRequest);
        }

        public static ValidateCouponCodeResponse ValidateCouponCode(string couponCode, int contactId)
        {
            CRMReader reader = new CRMReader();
            ValidateCouponCodeRequest couponCodeRequest = new ValidateCouponCodeRequest();
            couponCodeRequest.couponCode = couponCode;
            couponCodeRequest.contactId = contactId;
            return reader.ValidateCouponCode(couponCodeRequest);
        }

        public static GetInsuranceTypesResponse GetInsuranceTypes()
        {
            CRMReader reader = new CRMReader();
            var response = reader.GetInsuranceTypes();
            return response;
        }

        public static EligibilityDto GetEligibilityByUniqueId(string uniqueId)
        {
            ParticipantReader participantReader = new ParticipantReader();
            EligibilityDto eligibility = participantReader.GetEligibilityByUniqueId(uniqueId, null);
            return eligibility;
        }

        public static CRM_ContactDto GetCRMContactByUniqueId(string uniqueId, int organizationId)
        {
            CRMReader crmReader = new CRMReader();
            return crmReader.GetCRMContactByUniqueId(uniqueId, organizationId);
        }


    }
}