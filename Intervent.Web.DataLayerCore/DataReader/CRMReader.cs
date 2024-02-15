using Intervent.DAL;
using Intervent.Framework.Clone;
using Intervent.Web.DTO;
using Intervent.Web.DTO.Diff;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Intervent.Web.DataLayer
{
    public class CRMReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        public GetCRMContactResponse GetContact(GetCRMContactRequest request)
        {
            GetCRMContactResponse response = new GetCRMContactResponse();
            var contact = context.CRM_Contacts.Where(x => x.Id == request.contactId).Include("InsuranceTypes").Include("CRM_PogoMeterNumbers").Include("Countries").Include("States").Include("CRM_Notes").Include("CRM_Notes.CRM_PogoMeterNumbers").Include("CRM_Notes.HCPList").Include("CRM_Notes.CRM_AccountTypes").Include("CRM_Notes.CRM_ComplaintClassificationTypes").Include("CRM_Notes.CRM_CallerProfileTypes").Include("CRM_Notes.CRM_Dispositions").Include("CRM_Notes.User").Include("CRM_Notes.User1").Include("CRM_Notes.InquiryType1").Include("Organization").FirstOrDefault();
            response.CRM_Contact = Utility.mapper.Map<DAL.CRM_Contact, CRM_ContactDto>(contact);
            return response;
        }

        public GetCRMContactResponse GetContactByEmailorPhoneNo(GetContactByEmailorPhoneNoRequest request)
        {
            GetCRMContactResponse response = new GetCRMContactResponse();
            if (string.IsNullOrEmpty(request.phoneNo))
                return response;
            var phoneNumber = request.phoneNo.Substring(0, 1);
            if (phoneNumber == "1")
                request.phoneNo = request.phoneNo.Remove(0, 1);
            var contact = context.CRM_Contacts.Where(x =>
                (!string.IsNullOrEmpty(request.phoneNo)
                && (x.PhoneNumber1.Equals(request.phoneNo) || x.PhoneNumber2.Equals(request.phoneNo) || x.PhoneNumber3.Equals(request.phoneNo)))
                || (request.byEmailorPhone && !string.IsNullOrEmpty(request.email) && x.Email.Contains(request.email))).FirstOrDefault();
            response.CRM_Contact = Utility.mapper.Map<DAL.CRM_Contact, CRM_ContactDto>(contact);
            return response;
        }

        public GetAccountTypesResponse GetAccountTypes()
        {
            GetAccountTypesResponse response = new GetAccountTypesResponse();
            var accounts = context.CRM_AccountTypes.ToList();
            response.CRM_AccountTypes = Utility.mapper.Map<List<DAL.CRM_AccountType>, List<CRM_AccountTypesDto>>(accounts);
            return response;
        }

        public InquiryTypesResponse GetInquiryTypes()
        {
            InquiryTypesResponse response = new InquiryTypesResponse();
            var inquiryTypes = context.InquiryTypes.ToList();
            response.InquiryTypes = Utility.mapper.Map<List<DAL.InquiryType>, List<InquiryTypeDto>>(inquiryTypes);
            return response;
        }
        public GetCallerProfileTypesResponse CallerProfileTypes()
        {
            GetCallerProfileTypesResponse response = new GetCallerProfileTypesResponse();
            var callerProfile = context.CRM_CallerProfileTypes.ToList();
            response.CRM_CallerProfileTypes = Utility.mapper.Map<List<DAL.CRM_CallerProfileType>, List<CRM_CallerProfileTypesDto>>(callerProfile);
            return response;
        }
        public GetComplaintClassificationTypes ComplaintClassificationTypes()
        {
            GetComplaintClassificationTypes response = new GetComplaintClassificationTypes();
            var complaintClassification = context.CRM_ComplaintClassificationTypes.OrderBy(y => y.Type).ToList();
            response.CRM_ComplaintClassificationTypes = Utility.mapper.Map<List<DAL.CRM_ComplaintClassificationType>, List<CRM_ComplaintClassificationTypesDto>>(complaintClassification);
            return response;
        }

        public AddEditCRMProfileResponse AddEditCRMProfile(AddEditCRMProfileRequest request)
        {
            AddEditCRMProfileResponse response = new AddEditCRMProfileResponse();
            var CRM_Contact = context.CRM_Contacts.Where(x => x.Id == request.crm_Contact.Id).FirstOrDefault();
            if (null != request.crm_Contact.PhoneNumber1)
            {
                request.crm_Contact.PhoneNumber1 = new string(request.crm_Contact.PhoneNumber1.Where(char.IsDigit).ToArray());
            }
            if (null != request.crm_Contact.PhoneNumber2)
            {
                request.crm_Contact.PhoneNumber2 = new string(request.crm_Contact.PhoneNumber2.Where(char.IsDigit).ToArray());
            }
            if (null != request.crm_Contact.PhoneNumber3)
            {
                request.crm_Contact.PhoneNumber3 = new string(request.crm_Contact.PhoneNumber3.Where(char.IsDigit).ToArray());
            }
            if (CRM_Contact != null)
            {
                DAL.CRM_Contact currentCRM_Contact = null;
                currentCRM_Contact = CloneUtil.DeepClone<DAL.CRM_Contact>(CRM_Contact);
                CRM_Contact.FirstName = request.crm_Contact.FirstName;
                CRM_Contact.LastName = request.crm_Contact.LastName;
                CRM_Contact.Gender = request.crm_Contact.Gender;
                CRM_Contact.OrganizationId = request.intuityOrgId;
                CRM_Contact.Zip = request.crm_Contact.Zip;
                CRM_Contact.City = request.crm_Contact.City;
                CRM_Contact.PhoneNumber1 = request.crm_Contact.PhoneNumber1;
                CRM_Contact.PhoneNumber2 = request.crm_Contact.PhoneNumber2;
                CRM_Contact.PhoneNumber3 = request.crm_Contact.PhoneNumber3;
                CRM_Contact.Email = request.crm_Contact.Email;
                CRM_Contact.UpdatedBy = request.userId;
                CRM_Contact.UpdatedOn = DateTime.UtcNow;
                CRM_Contact.DOB = request.crm_Contact.DOB;
                CRM_Contact.Address = request.crm_Contact.Address;
                CRM_Contact.State = request.crm_Contact.State;
                CRM_Contact.Country = request.crm_Contact.Country;
                CRM_Contact.Address2 = request.crm_Contact.Address2;
                CRM_Contact.City2 = request.crm_Contact.City2;
                CRM_Contact.State2 = request.crm_Contact.State2;
                CRM_Contact.Country2 = request.crm_Contact.Country2;
                CRM_Contact.Zip2 = request.crm_Contact.Zip2;
                CRM_Contact.Notes = request.crm_Contact.Notes;
                CRM_Contact.OptedIn = request.crm_Contact.OptedIn;

                context.CRM_Contacts.Attach(CRM_Contact);
                LogContactChanges(currentCRM_Contact, CRM_Contact, request.crm_Contact.Id, null, (int)ChangeLogsCategory.CRM_Contacts, CRM_Contact.UpdatedBy.Value);
                context.Entry(CRM_Contact).State = EntityState.Modified;

            }
            else
            {
                request.crm_Contact.CreatedOn = DateTime.UtcNow;
                request.crm_Contact.CreatedBy = request.userId;
                request.crm_Contact.OrganizationId = request.intuityOrgId;
                CRM_Contact = Utility.mapper.Map<CRM_ContactDto, CRM_Contact>(request.crm_Contact);
                context.CRM_Contacts.Add(CRM_Contact);
            }
            context.SaveChanges();
            response.contactId = CRM_Contact.Id;
            if (!request.fromWebForm)
            {
                var crm_Contact = context.CRM_Contacts.Include("InsuranceTypes").Where(x => x.Id == response.contactId).FirstOrDefault();
                crm_Contact.InsuranceTypes.Clear();
                if (!string.IsNullOrEmpty(request.crm_Contact.InsuranceType))
                {
                    var insuranceTypes = request.crm_Contact.InsuranceType.Split('|');
                    foreach (var insuranceTypeId in insuranceTypes)
                    {
                        if (!string.IsNullOrEmpty(insuranceTypeId))
                        {
                            var id = Convert.ToInt16(insuranceTypeId);
                            var InsuranceType = context.InsuranceTypes.Where(x => x.Id == id).FirstOrDefault();
                            crm_Contact.InsuranceTypes.Add(InsuranceType);
                        }
                    }
                }
                context.SaveChanges();

                string[] pogoMeterNumberList = request.crm_Contact.PogoMeterNumber != null ? request.crm_Contact.PogoMeterNumber.Split('|') : null;
                var all_CRM_PogoMeterNumbers = context.CRM_PogoMeterNumbers.Where(x => x.CRMContactId == response.contactId && x.IsActive == true).ToList();
                var crm_PogoMeterNumbers = all_CRM_PogoMeterNumbers.Where(x => pogoMeterNumberList == null || !pogoMeterNumberList.Contains(x.PogoMeterNumber)).ToList();
                if (crm_PogoMeterNumbers.Count > 0)
                {
                    foreach (var pogoMeter in crm_PogoMeterNumbers)
                    {
                        pogoMeter.IsActive = false;
                        context.CRM_PogoMeterNumbers.Attach(pogoMeter);
                        context.Entry(pogoMeter).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                if (pogoMeterNumberList != null)
                {
                    foreach (var pogoMeterNumber in pogoMeterNumberList)
                    {
                        if (all_CRM_PogoMeterNumbers.Where(x => x.PogoMeterNumber == pogoMeterNumber).FirstOrDefault() == null)
                        {
                            DAL.CRM_PogoMeterNumbers crm_PogoMeterNumber = new CRM_PogoMeterNumbers();
                            crm_PogoMeterNumber.CRMContactId = response.contactId;
                            crm_PogoMeterNumber.PogoMeterNumber = pogoMeterNumber;
                            crm_PogoMeterNumber.IsActive = true;
                            context.CRM_PogoMeterNumbers.Add(crm_PogoMeterNumber);
                            context.SaveChanges();
                        }
                    }
                }
            }
            response.success = true;
            return response;
        }

        public AddEditEligibilityToCRMResponse AddEditEligibilityToCRM(AddEditEligibilityToCRMRequest request)
        {

            AddEditEligibilityToCRMResponse response = new AddEditEligibilityToCRMResponse();
            var crm_Contact = context.CRM_Contacts.Where(x => x.Id == request.CRMId).FirstOrDefault();
            if (crm_Contact != null)
            {
                crm_Contact.UniqueId = request.UniqueId;
                crm_Contact.OrganizationId = request.EligibilityOrgId.HasValue ? request.EligibilityOrgId.Value : request.intuityOrgId;
                context.CRM_Contacts.Attach(crm_Contact);
                context.Entry(crm_Contact).State = EntityState.Modified;
                context.SaveChanges();
                response.success = true;
            }
            return response;
        }

        public bool ValidatePhoneNoExistence(GetContactByEmailorPhoneNoRequest request)
        {
            if (null != request.phoneNo)
            {
                request.phoneNo = new string(request.phoneNo.Where(char.IsDigit).ToArray());
            }
            var count = context.CRM_Contacts.Where(x =>
                string.IsNullOrEmpty(request.phoneNo)
                || x.PhoneNumber1.Equals(request.phoneNo)
                || x.PhoneNumber2.Equals(request.phoneNo)
                || x.PhoneNumber3.Equals(request.phoneNo)).Count();
            return count == 0 ? false : true;
        }

        public ValidateCouponCodeResponse ValidateCouponCode(ValidateCouponCodeRequest request)
        {
            ValidateCouponCodeResponse response = new ValidateCouponCodeResponse();
            var crm_NotesCount = context.CRM_Notes.Where(x => x.CouponCode == request.couponCode && x.ContactId != request.contactId).Count();
            response.isValid = crm_NotesCount > 0 ? false : true;
            return response;
        }

        public AddEditCRMNotesResponse AddEditCRMNotes(AddEditCRMNoteRequest request)
        {
            AddEditCRMNotesResponse response = new AddEditCRMNotesResponse();
            if (!request.crm_Note.HCPId.HasValue && (request.crm_Note.CallerProfileType == 3 ||
                (!string.IsNullOrEmpty(request.crm_Note.CouponCode) && !string.IsNullOrEmpty(request.crm_Note.FirstName))))
            {
                DAL.HCPList HCPList = new DAL.HCPList();
                HCPList.OrganizationId = request.intuityOrgId;
                HCPList.FirstName = request.crm_Note.FirstName;
                HCPList.LastName = request.crm_Note.LastName;
                HCPList.AddressLine1 = request.crm_Note.Address1;
                HCPList.AddressLine2 = request.crm_Note.Address2;
                HCPList.City = request.crm_Note.City;
                HCPList.State = request.crm_Note.State;
                HCPList.ZipCode = request.crm_Note.Zipcode;
                HCPList.PhoneNumber = request.crm_Note.PhoneNumber;
                HCPList.Email = request.crm_Note.Email;
                HCPList.UserAdded = true;
                context.HCPLists.Add(HCPList);
                context.SaveChanges();
                request.crm_Note.HCPId = HCPList.Id;

            }
            var CRM_Note = context.CRM_Notes.Where(x => x.Id == request.crm_Note.Id).FirstOrDefault();
            if (CRM_Note != null)
            {
                DAL.CRM_Note currentCRM_Notes = null;
                currentCRM_Notes = CloneUtil.DeepClone<DAL.CRM_Note>(CRM_Note);
                CRM_Note.ContactId = request.crm_Note.ContactId;
                if (request.crm_Note.Type == 1 || request.crm_Note.Type == 3 || request.crm_Note.Type != CRM_Note.Type)
                    CRM_Note.Disposition = request.crm_Note.Disposition;
                CRM_Note.Type = request.crm_Note.Type;
                CRM_Note.InquiryType = request.crm_Note.InquiryType;
                CRM_Note.CallerProfileType = request.crm_Note.CallerProfileType;
                CRM_Note.FullName = request.crm_Note.FullName;
                CRM_Note.AccountType = request.crm_Note.AccountType;
                CRM_Note.RequiredEscalation = request.crm_Note.RequiredEscalation;
                CRM_Note.RequiredRMA = request.crm_Note.RequiredRMA;
                CRM_Note.RMANumber = request.crm_Note.RMANumber;
                CRM_Note.QADNumber = request.crm_Note.QADNumber;
                CRM_Note.ComplaintClassificationType = request.crm_Note.ComplaintClassificationType;
                CRM_Note.FirstCallResolution = request.crm_Note.FirstCallResolution;
                CRM_Note.POGOChampion = request.crm_Note.POGOChampion;
                CRM_Note.PogoMeterNumber = request.crm_Note.PogoMeterNumber;
                CRM_Note.MasterControlNo = request.crm_Note.MasterControlNo;
                CRM_Note.CartridgeLotNumber = request.crm_Note.CartridgeLotNumber;
                CRM_Note.Notes = request.crm_Note.Notes;
                CRM_Note.CouponCode = request.crm_Note.CouponCode;
                CRM_Note.CartridgePrescriptions = request.crm_Note.CartridgePrescriptions;
                CRM_Note.HCPId = request.crm_Note.HCPId; 
                CRM_Note.Language = request.crm_Note.Language;
                CRM_Note.UpdatedBy = request.userId;
                CRM_Note.UpdatedOn = DateTime.UtcNow;
                context.CRM_Notes.Attach(CRM_Note);
                LogContactChanges(currentCRM_Notes, CRM_Note, request.crm_Note.ContactId, request.crm_Note.Id, (int)ChangeLogsCategory.CRM_Notes, CRM_Note.UpdatedBy.Value);
                context.Entry(CRM_Note).State = EntityState.Modified;
            }
            else
            {
                request.crm_Note.CreatedOn = DateTime.UtcNow;
                request.crm_Note.CreatedBy = request.userId;
                CRM_Note = Utility.mapper.Map<CRM_NoteDto, CRM_Note>(request.crm_Note);
                context.CRM_Notes.Add(CRM_Note);
            }
            context.SaveChanges();
            response.success = true;
            return response;
        }

        public IList<HCPListsDto> ListHCP(string search)
        {
            var searchText = search.Split(' ');
            var doctorsLists = context.HCPLists.Include("State1").Where(x => searchText.Contains(x.FirstName) || searchText.Contains(x.LastName) || x.FirstName.Contains(search) || x.LastName.Contains(search)).ToList();
            return Utility.mapper.Map<IList<DAL.HCPList>, IList<HCPListsDto>>(doctorsLists);
        }

        public bool LogContactChanges(object existingObject, object newObject, int contactId, int? noteId, int categoryId, int updateBy)
        {
            bool dataUpdated = false;
            IEnumerable<PropertyCompare> properties = PropertyCompare.FindDifferences(existingObject, newObject, excludeVirtual: true);
            if (properties.Count() > 0)
            {
                AddContactChangeRequest change = new AddContactChangeRequest();
                change.UserChange = new CRM_ChangeLogDto();
                change.UserChange.ContactId = contactId;
                change.UserChange.UpdatedBy = updateBy;
                change.UserChange.RefId = noteId != null ? noteId : contactId;
                change.UserChange.CategoryId = categoryId;
                change.UserChange.Changes = JsonConvert.SerializeObject(properties);
                AddContactChanges(change);
                dataUpdated = true;
            }
            return dataUpdated;
        }

        public void AddContactChanges(AddContactChangeRequest request)
        {
            DAL.CRM_ChangeLog change = new DAL.CRM_ChangeLog();
            change.Changes = request.UserChange.Changes;
            change.UpdatedBy = request.UserChange.UpdatedBy;
            change.LogDate = DateTime.UtcNow;
            change.CategoryId = request.UserChange.CategoryId;
            change.ContactId = request.UserChange.ContactId;
            change.RefId = request.UserChange.RefId;
            context.CRM_ChangeLogs.Add(change);
            context.SaveChanges();
        }

        public int SearchUnhandledForms()
        {
            return context.CRM_Notes.Where(note => note.Disposition == null && note.Type == 3).Count();
        }

        public CRMSearchResponse SearchUsers(CRMSearchRequest request)
        {
            CRMSearchResponse response = new CRMSearchResponse();
            int CRM_Record_Count = 0;
            CRM_Record_Count = context.CRM_Contacts.Where(x => (String.IsNullOrEmpty(request.FirstName) || x.FirstName.Contains(request.FirstName))
                && (String.IsNullOrEmpty(request.LastName) || x.LastName.Contains(request.LastName))
                && (String.IsNullOrEmpty(request.Email) || x.Email.Contains(request.Email))
                && (!request.CRMId.HasValue || x.Id == request.CRMId)
                && (String.IsNullOrEmpty(request.RMAandQADNumber) || x.CRM_Notes.Any(note => note.RMANumber.Contains(request.RMAandQADNumber)) || x.CRM_Notes.Any(note => note.QADNumber.Contains(request.RMAandQADNumber)))
                && (String.IsNullOrEmpty(request.MasterControlNo) || x.CRM_Notes.Any(note => note.MasterControlNo.Contains(request.MasterControlNo)))
                && (!request.CSRId.HasValue || x.CRM_Notes.Any(note => note.CreatedBy == request.CSRId || note.UpdatedBy == request.CSRId))
                && (!request.ComplaintsSearch || x.CRM_Notes.Any(note => note.CRM_Dispositions.Complaint))
                && (!request.UnhandledWebforms || x.CRM_Notes.Any(note => note.Disposition == null && note.Type == 3))
                && ((String.IsNullOrEmpty(request.Phone) || x.PhoneNumber1.Contains(request.Phone))
                || (String.IsNullOrEmpty(request.Phone) || x.PhoneNumber2.Contains(request.Phone))
                || (String.IsNullOrEmpty(request.Phone) || x.PhoneNumber3.Contains(request.Phone)))).Count();

            var CRM_Record = context.CRM_Contacts.Include("Countries").Include("States").Include("CRM_Notes").Where(x => (String.IsNullOrEmpty(request.FirstName) || x.FirstName.Contains(request.FirstName))
                && (String.IsNullOrEmpty(request.LastName) || x.LastName.Contains(request.LastName))
                && (String.IsNullOrEmpty(request.Email) || x.Email.Contains(request.Email))
                && (!request.CRMId.HasValue || x.Id == request.CRMId)
                && (String.IsNullOrEmpty(request.RMAandQADNumber) || x.CRM_Notes.Any(note => note.RMANumber.Contains(request.RMAandQADNumber)) || x.CRM_Notes.Any(note => note.QADNumber.Contains(request.RMAandQADNumber)))
                && (String.IsNullOrEmpty(request.MasterControlNo) || x.CRM_Notes.Any(note => note.MasterControlNo.Contains(request.MasterControlNo)))
                && (!request.CSRId.HasValue || x.CRM_Notes.Any(note => note.CreatedBy == request.CSRId || note.UpdatedBy == request.CSRId))
                && (!request.ComplaintsSearch || x.CRM_Notes.Any(note => note.CRM_Dispositions.Complaint))
                && (!request.UnhandledWebforms || x.CRM_Notes.Any(note => note.Disposition == null && note.Type == 3))
                && ((String.IsNullOrEmpty(request.Phone) || x.PhoneNumber1.Contains(request.Phone))
                || (String.IsNullOrEmpty(request.Phone) || x.PhoneNumber2.Contains(request.Phone))
                || (String.IsNullOrEmpty(request.Phone) || x.PhoneNumber3.Contains(request.Phone)))).OrderByDescending(x => x.CRM_Notes.OrderByDescending(y => y.CreatedOn).FirstOrDefault().CreatedOn).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            response.CRM_ContactDto = Utility.mapper.Map<List<DAL.CRM_Contact>, List<CRM_ContactDto>>(CRM_Record);
            response.TotalRecords = CRM_Record_Count;
            return response;
        }

        public GetChangeLogResponse GetChangeLog(GetChangeLogRequest request)
        {
            GetChangeLogResponse response = new GetChangeLogResponse();
            int catrgoryId = request.isContactLog ? 1 : 2;
            var changeLog = context.CRM_ChangeLogs.Include("User").Where(x => x.RefId == request.refId && x.CategoryId == catrgoryId).OrderByDescending(x => x.LogDate).ToList();
            response.crm_ChangeLog = Utility.mapper.Map<List<DAL.CRM_ChangeLog>, List<CRM_ChangeLogDto>>(changeLog);
            return response;
        }

        public GetDispositionsListResponse GetDispositionsList()
        {
            GetDispositionsListResponse response = new GetDispositionsListResponse();
            var dispositions = context.CRM_Dispositions.ToList();
            response.CRM_Dispositions = Utility.mapper.Map<List<DAL.CRM_Disposition>, IList<CRM_DispositionsDto>>(dispositions).ToList();
            return response;
        }

        public List<CRM_PogoMeterNumbersDto> GetPogoMeterNumbers(int contactId)
        {
            var PogoMeterNumbers = context.CRM_PogoMeterNumbers.Where(x => x.CRMContactId == contactId).ToList();
            return Utility.mapper.Map<List<DAL.CRM_PogoMeterNumbers>, IList<CRM_PogoMeterNumbersDto>>(PogoMeterNumbers).ToList();

        }

        public bool AddPogoMeterNumber(int contactId, string pogoMonitorNumber)
        {
            DAL.CRM_PogoMeterNumbers crm_PogoMeterNumber = new CRM_PogoMeterNumbers();
            crm_PogoMeterNumber.CRMContactId = contactId;
            crm_PogoMeterNumber.PogoMeterNumber = pogoMonitorNumber;
            crm_PogoMeterNumber.IsActive = true;
            context.CRM_PogoMeterNumbers.Add(crm_PogoMeterNumber);
            context.SaveChanges();
            return true;
        }

        public IList<CRM_Note> GetCRMNoteId(GetCRMNotesRequest request)
        {
            return context.CRM_Notes.Where(x => (request.ContactId.HasValue && x.ContactId == request.ContactId && x.CallId == null
            && x.Type == 2 && x.CreatedOn > request.CallTime) || (request.CallId.HasValue && x.CallId == request.CallId))
            .OrderByDescending(x => x.CreatedOn).ToList();
        }

        public void AddOrUpdateCRMNotes(AddUpdateCRMNoteRequest request)
        {
            var crm_note = context.CRM_Notes.Where(x => x.Id == request.CRM_Note.Id).FirstOrDefault();
            if (crm_note == null)
            {
                crm_note = new CRM_Note();
                crm_note.ContactId = request.CRM_Note.ContactId;
                crm_note.Type = 2;
                crm_note.CallerProfileType = request.CRM_Note.CallerProfileType;
                crm_note.FullName = request.CRM_Note.FullName;
                crm_note.AccountType = request.CRM_Note.AccountType;
                crm_note.RequiredEscalation = request.CRM_Note.RequiredEscalation;
                crm_note.RequiredRMA = request.CRM_Note.RequiredRMA;
                crm_note.RMANumber = request.CRM_Note.RMANumber;
                crm_note.ComplaintClassificationType = request.CRM_Note.ComplaintClassificationType;
                crm_note.Notes = request.CRM_Note.Notes;
                crm_note.CreatedOn = request.CRM_Note.TimeStamp.Value;
                crm_note.CreatedBy = request.CRM_Note.CreatedBy;
                crm_note.MasterControlNo = request.CRM_Note.MasterControlNo;
                crm_note.FirstCallResolution = request.CRM_Note.FirstCallResolution;
                crm_note.POGOChampion = request.CRM_Note.POGOChampion;
                crm_note.CallId = request.CRM_Note.CallId;
                crm_note.TimeStamp = request.CRM_Note.TimeStamp;
                crm_note.Ani = request.CRM_Note.Ani;
                crm_note.Dnis = request.CRM_Note.Dnis;
                crm_note.CampaignType = request.CRM_Note.CampaignType;
                crm_note.Campaign = request.CRM_Note.Campaign;
                crm_note.Calls = request.CRM_Note.Calls;
                crm_note.CallType = request.CRM_Note.CallType;
                crm_note.SpeedofAnswer = request.CRM_Note.SpeedofAnswer;
                crm_note.QueueWaitTime = request.CRM_Note.QueueWaitTime;
                crm_note.CallTime = request.CRM_Note.CallTime;
                crm_note.Disposition = request.CRM_Note.Disposition;
                crm_note.Holds = request.CRM_Note.Holds;
                crm_note.HoldTime = request.CRM_Note.HoldTime;
                crm_note.AfterCallWorkTime = request.CRM_Note.AfterCallWorkTime;
                crm_note.HandleTime = request.CRM_Note.HandleTime;
                crm_note.Abandoned = request.CRM_Note.Abandoned;
                crm_note.AbandonRate = request.CRM_Note.AbandonRate;
                crm_note.Conferences = request.CRM_Note.Conferences;
                crm_note.Transfers = request.CRM_Note.Transfers;
                crm_note.QueueCallBackWaitTime = request.CRM_Note.QueueCallBackWaitTime;
                crm_note.Survey1 = request.CRM_Note.Survey1;
                crm_note.Survey2 = request.CRM_Note.Survey2;
                crm_note.Survey3 = request.CRM_Note.Survey3;
                crm_note.Survey4 = request.CRM_Note.Survey4;
                crm_note.TalkTime = request.CRM_Note.TalkTime;
                crm_note.Voicemails = request.CRM_Note.Voicemails;
                crm_note.VoicemailsHandleTime = request.CRM_Note.VoicemailsHandleTime;
                context.CRM_Notes.Add(crm_note);
                context.SaveChanges();
            }
            else
            {
                crm_note.CallId = request.CRM_Note.CallId;
                crm_note.TimeStamp = request.CRM_Note.TimeStamp;
                crm_note.Ani = request.CRM_Note.Ani;
                crm_note.Dnis = request.CRM_Note.Dnis;
                crm_note.CampaignType = request.CRM_Note.CampaignType;
                crm_note.Campaign = request.CRM_Note.Campaign;
                crm_note.Calls = request.CRM_Note.Calls;
                crm_note.CallType = request.CRM_Note.CallType;
                crm_note.SpeedofAnswer = request.CRM_Note.SpeedofAnswer;
                crm_note.QueueWaitTime = request.CRM_Note.QueueWaitTime;
                crm_note.CallTime = request.CRM_Note.CallTime;
                crm_note.Disposition = request.CRM_Note.Disposition;
                crm_note.Holds = request.CRM_Note.Holds;
                crm_note.HoldTime = request.CRM_Note.HoldTime;
                crm_note.AfterCallWorkTime = request.CRM_Note.AfterCallWorkTime;
                crm_note.HandleTime = request.CRM_Note.HandleTime;
                crm_note.Abandoned = request.CRM_Note.Abandoned;
                crm_note.AbandonRate = request.CRM_Note.AbandonRate;
                crm_note.Conferences = request.CRM_Note.Conferences;
                crm_note.Transfers = request.CRM_Note.Transfers;
                crm_note.QueueCallBackWaitTime = request.CRM_Note.QueueCallBackWaitTime;
                crm_note.Survey1 = request.CRM_Note.Survey1;
                crm_note.Survey2 = request.CRM_Note.Survey2;
                crm_note.Survey3 = request.CRM_Note.Survey3;
                crm_note.Survey4 = request.CRM_Note.Survey4;
                crm_note.TalkTime = request.CRM_Note.TalkTime;
                crm_note.Voicemails = request.CRM_Note.Voicemails;
                crm_note.VoicemailsHandleTime = request.CRM_Note.VoicemailsHandleTime;
                crm_note.UpdatedBy = request.CRM_Note.CreatedBy;
                if (crm_note.CreatedBy == request.systemAdminId && crm_note.CreatedBy != request.CRM_Note.CreatedBy)
                    crm_note.CreatedBy = request.CRM_Note.CreatedBy;
                crm_note.UpdatedOn = DateTime.UtcNow;
                context.CRM_Notes.Attach(crm_note);
                context.Entry(crm_note).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (request.CRM_Note.EligibleforActivity)
            {
                AccountReader accountReader = new AccountReader();
                ExternalReader externalReader = new ExternalReader();
                var crmContact = context.CRM_Contacts.Where(x => x.Id == request.CRM_Note.ContactId).FirstOrDefault();
                if (crmContact != null)
                {
                    var user = accountReader.GetUserByUserName(crmContact.Email);
                    if (!string.IsNullOrEmpty(user.UserName))
                    {
                        string phonenumber;
                        if (request.CRM_Note.CampaignType.Equals(1))
                            phonenumber = request.CRM_Note.Ani;
                        else
                            phonenumber = request.CRM_Note.Dnis;
                        var userDto = accountReader.GetUserByPhone(phonenumber);
                        if (userDto != null)
                            user = Utility.mapper.Map<UserDto, DAL.User>(userDto);
                    }
                    if (!string.IsNullOrEmpty(user.UserName) && user.Organization.IntegrationWith == (byte)IntegrationPartner.Intuity && externalReader.GetIntuityUsersByUserId(user.Id) != null)
                    {
                        IntuityReader intuityReader = new IntuityReader();
                        AddIntuityEventRequest intuityEventRequest = new AddIntuityEventRequest();
                        intuityEventRequest.intuityEvent = new IntuityEventDto();
                        intuityEventRequest.intuityEvent.UserId = user.Id;
                        intuityEventRequest.organizationCode = user.Organization.Code;
                        intuityEventRequest.intuityEvent.UniqueId = user.UniqueId;
                        intuityEventRequest.intuityEvent.EventType = (int)IntuityEventTypes.Call_Center_Interaction;
                        intuityEventRequest.intuityEvent.EventDate = DateTime.UtcNow;
                        intuityEventRequest.intuityEvent.CreatedBy = request.CRM_Note.CreatedBy;
                        intuityReader.AddIntuityEvent(intuityEventRequest);
                    }
                }
            }
        }

        public GetInsuranceTypesResponse GetInsuranceTypes()
        {
            GetInsuranceTypesResponse response = new GetInsuranceTypesResponse();
            var insuranceTypes = context.InsuranceTypes.ToList();
            response.InsuranceTypes = Utility.mapper.Map<List<DAL.InsuranceType>, List<InsuranceTypesDto>>(insuranceTypes);
            return response;
        }

        public CRM_ContactDto GetCRMContactByUniqueId(string uniqueId, int? organizationId)
        {
            var crm_Contacts = context.CRM_Contacts.Where(x => x.UniqueId == uniqueId && x.OrganizationId == organizationId).FirstOrDefault();
            return Utility.mapper.Map<DAL.CRM_Contact, CRM_ContactDto>(crm_Contacts);
        }

        public AddQADOrdersResponse BulkAddQADOrders(AddQADOrdersRequest request)
        {
            AddQADOrdersResponse response = new AddQADOrdersResponse();
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    context1.ChangeTracker.AutoDetectChangesEnabled = false;
                    foreach (QADOrdersDto qadOreder in request.qadOrders)
                    {
                        var QADOreder = context1.QADOrders.Where(x => x.Order == qadOreder.Order && x.OrderDate == qadOreder.OrderDate && x.ItemNumber == qadOreder.ItemNumber && x.QtyOrdered == qadOreder.QtyOrdered).FirstOrDefault();
                        if (QADOreder == null && !string.IsNullOrEmpty(qadOreder.Order))
                        {
                            QADOrders result = Utility.mapper.Map<QADOrdersDto, DAL.QADOrders>(qadOreder);
                            context1.QADOrders.Add(result);
                            response.count++;
                        }
                    }
                    context1.SaveChanges();
                    response.success = true;
                }
                scope.Complete();
            }
            return response;
        }

        public GetQADOrderHistoryResponse GetQADOrderHistory(GetQADOrderHistoryRequest request)
        {
            GetQADOrderHistoryResponse response = new GetQADOrderHistoryResponse();
            var qadNumbers = context.CRM_Notes.Where(x => x.ContactId == request.contactId && !string.IsNullOrEmpty(x.QADNumber)).Select(y => y.QADNumber).ToList();
            var QADOrders = context.QADOrders.Where(x => qadNumbers.Contains(x.Order)).ToList();
            response.qadOrders = Utility.mapper.Map<List<DAL.QADOrders>, IList<QADOrdersDto>>(QADOrders).ToList();
            return response;
        }
    }
}