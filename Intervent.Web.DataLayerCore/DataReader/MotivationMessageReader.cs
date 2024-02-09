using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class MotivationMessageReader
    {
        InterventDatabase dbcontext = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        public AddEditMotivationMessageResponse AddEditMotivationMessage(AddEditMotivationMessageRequest request)
        {
            AddEditMotivationMessageResponse response = new AddEditMotivationMessageResponse();
            DAL.MotivationMessage motivationMessageDAL = new DAL.MotivationMessage();
            if (request.motivationMessage.Id > 0)
            {
                motivationMessageDAL = dbcontext.MotivationMessages.Where(x => x.Id == request.motivationMessage.Id).FirstOrDefault();
                if (motivationMessageDAL != null)
                {
                    motivationMessageDAL.MessageContent = request.motivationMessage.MessageContent;
                    motivationMessageDAL.Subject = request.motivationMessage.Subject;
                    dbcontext.Entry(motivationMessageDAL).State = EntityState.Modified;
                }
            }
            else
            {
                motivationMessageDAL.MessageContent = request.motivationMessage.MessageContent;
                motivationMessageDAL.Subject = request.motivationMessage.Subject;
                dbcontext.MotivationMessages.Add(motivationMessageDAL);
            }
            dbcontext.SaveChanges();
            response.MotivationMessageId = motivationMessageDAL.Id;
            response.Status = true;
            return response;
        }

        public bool AssignOrRemoveMotivationMessage(AssignMotivationMessagesRequest request)
        {
            if (request.isRemove)
            {
                var assignedMotivationMessages = dbcontext.AssignedMotivationMessages.Where(x => x.Id == request.AssignedMessageId.Value).FirstOrDefault();
                if (assignedMotivationMessages != null)
                {
                    assignedMotivationMessages.Active = false;
                    dbcontext.AssignedMotivationMessages.Attach(assignedMotivationMessages);
                    dbcontext.Entry(assignedMotivationMessages).State = EntityState.Modified;
                    dbcontext.SaveChanges();
                }
            }
            else
            {
                var organizationIds = request.OrganizationIds.Split(',');

                if (organizationIds.Length > 0)
                {
                    foreach (var orgId in organizationIds)
                    {
                        DAL.AssignedMotivationMessage dal = new DAL.AssignedMotivationMessage();
                        dal.MessagesID = request.MessageId;
                        dal.Active = true;
                        dal.OrganizationID = Convert.ToInt16(orgId);
                        dal.MessageType = request.MessageTypes;
                        dal.Date = DateTime.UtcNow;
                        dbcontext.AssignedMotivationMessages.Add(dal);
                        dbcontext.SaveChanges();
                    }
                }
            }
            return true;
        }

        public int AddMotivationMessageToUserDashbaord(string MsgBody, int OrgId)
        {
            int count = 0;
            CommonReader reader = new CommonReader();
            List<UserDashboardMessageDto> dashboardMessagelist = new List<UserDashboardMessageDto>();

            List<UserDto> userlist = new List<UserDto>();
            var users = dbcontext.Users.Include("Organization").Include("Organization.Portals").Where(x => x.OrganizationId == OrgId && x.IsActive == true && x.Organization.Portals.Any(y => y.Active == true)).ToList();
            userlist = Utility.mapper.Map<List<DAL.User>, List<UserDto>>(users);

            if (userlist != null && userlist.Count > 0)
            {
                count = count + userlist.Count;
                for (int i = 0; i < userlist.Count(); i++)
                {
                    reader.AddDashboardMessage(userlist[i].Id, IncentiveMessageTypes.Motivation_Message, null, null, MsgBody);
                }
            }
            return count;
        }

        public void MarkMessageComplete(int AssignMsgId)
        {
            var messsagesDal = dbcontext.AssignedMotivationMessages.Where(x => x.Id == AssignMsgId && x.Completed == false).FirstOrDefault();
            if (messsagesDal != null)
            {
                messsagesDal.Completed = true;
                dbcontext.AssignedMotivationMessages.Attach(messsagesDal);
                dbcontext.Entry(messsagesDal).State = EntityState.Modified;
                dbcontext.SaveChanges();
            }
        }

        public ListMotivationMessagesResponse ListMotivationMessages(ListMotivationMessagesRequest request)
        {
            ListMotivationMessagesResponse response = new ListMotivationMessagesResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = dbcontext.MotivationMessages.Count();

            }
            if (request.PageSize == 0)
            {
                request.PageSize = totalRecords;
            }
            var motivationMessages = new List<MotivationMessage>();
            motivationMessages = dbcontext.MotivationMessages.OrderByDescending(x => x.Id).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            response.MotivationMessages = Utility.mapper.Map<List<DAL.MotivationMessage>, List<MotivationMessagesDto>>(motivationMessages);
            response.TotalRecords = totalRecords;
            return response;
        }

        public GetMotivationMessageResponse GetMotivationMessage(GetMotivationMessageRequest request)
        {
            GetMotivationMessageResponse response = new GetMotivationMessageResponse();
            var motivationMessage = dbcontext.MotivationMessages.Where(x => x.Id == request.Id).FirstOrDefault();
            response.MotivationMessage = Utility.mapper.Map<DAL.MotivationMessage, MotivationMessagesDto>(motivationMessage);
            return response;
        }

        public ListAssignedMotivationMessagesResponse ListAssignedMotivationMessages(ListAssignedMotivationMessagesRequest request)
        {
            ListAssignedMotivationMessagesResponse response = new ListAssignedMotivationMessagesResponse();
            var assignedMessages = dbcontext.AssignedMotivationMessages.Include("Organization").OrderBy(x => x.Date).Where(x => x.MessagesID == request.motivationMessageId && x.Active).ToList();
            response.assignedMotivationMessages = Utility.mapper.Map<IList<DAL.AssignedMotivationMessage>, IList<AssignedMotivationMessageDto>>(assignedMessages);
            return response;
        }

        public IList<AssignedMotivationMessageDto> FetchAssignedMotivationMessages()
        {
            var assignedMessages = dbcontext.AssignedMotivationMessages.Include("MotivationMessage").Include("Organization").Where(x => x.Completed == false && x.Active).OrderBy(x => x.Date).ToList();
            return Utility.mapper.Map<IList<DAL.AssignedMotivationMessage>, IList<AssignedMotivationMessageDto>>(assignedMessages);
        }
    }
}