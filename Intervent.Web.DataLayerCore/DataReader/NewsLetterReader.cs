using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class NewsLetterReader
    {
        InterventDatabase dbcontext = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ListNewsletterResponse ListNewsletters(GetNewsletterListRequest request)
        {
            ListNewsletterResponse response = new ListNewsletterResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = dbcontext.Newsletter.Count();

            }
            if (request.PageSize == 0)
            {
                request.PageSize = totalRecords;
            }
            var newsletters = new List<Newsletter>();
            newsletters = dbcontext.Newsletter.OrderByDescending(x => x.Id).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            response.Newsletters = Utility.mapper.Map<IList<DAL.Newsletter>, IList<NewsletterDto>>(newsletters);
            response.TotalRecords = totalRecords;
            return response;
        }

        public IList<AssignedNewsletterDto> ListAssignedNewsletter(int newsletterId)
        {
            var assignedNewsletter = dbcontext.AssignedNewsletters.Include("Organization").OrderBy(x => x.Date).Where(x => x.NewsletterId == newsletterId).ToList();
            return Utility.mapper.Map<IList<DAL.AssignedNewsletter>, IList<AssignedNewsletterDto>>(assignedNewsletter);
        }

        public AddNewsletterResponse AddEditNewsletter(AddNewsletterRequest request)
        {
            string oldPDF = "";
            AddNewsletterResponse response = new AddNewsletterResponse();
            DAL.Newsletter newsletterDAL = new DAL.Newsletter();
            if (request.newsletter.Id > 0)
            {
                newsletterDAL = dbcontext.Newsletter.Where(x => x.Id == request.newsletter.Id).FirstOrDefault();
                oldPDF = newsletterDAL.Pdf;
                if (newsletterDAL != null)
                {
                    newsletterDAL.Name = request.newsletter.Name;
                    newsletterDAL.Pdf = request.newsletter.Pdf;
                    dbcontext.Entry(newsletterDAL).State = EntityState.Modified;
                    dbcontext.SaveChanges();
                }
            }
            else
            {
                newsletterDAL.Name = request.newsletter.Name;
                newsletterDAL.Pdf = request.newsletter.Pdf;
                dbcontext.Newsletter.Add(newsletterDAL);
                dbcontext.SaveChanges();
            }
            response.NewsletterId = newsletterDAL.Id;
            response.OldPdf = oldPDF;

            return response;
        }

        public bool AssignNewsletter(AssignNewsletterRequest request)
        {
            if (request.newletterList != null && request.newletterList.Count > 0)
            {
                var newletterId = request.newletterList[0].NewsletterId;
                for (int i = 0; i < request.newletterList.Count; i++)
                {
                    DAL.AssignedNewsletter dal = new DAL.AssignedNewsletter();
                    dal.NewsletterId = newletterId;
                    dal.OrganizationId = request.newletterList[i].OrganizationId;
                    dal.Date = DateTime.UtcNow;
                    dbcontext.AssignedNewsletters.Add(dal);
                    dbcontext.SaveChanges();
                }
            }
            return true;
        }

        public string GetNewsletterPath(int newsletterId)
        {
            string strNewsletterPath = "~/Pdf/" + dbcontext.Newsletter.Where(x => x.Id == newsletterId).Select(x => x.Pdf).FirstOrDefault();
            return strNewsletterPath;
        }
        public int AddNewsletterToUserDashbaord()
        {
            int count = 0;
            var assignedNewsletters = dbcontext.AssignedNewsletters.Include("Newsletter").Where(x => x.Completed == null).ToList();
            if (assignedNewsletters != null && assignedNewsletters.Count > 0)
            {
                List<UserDashboardMessageDto> dashboardMessagelist = new List<UserDashboardMessageDto>();
                foreach (var newsletter in assignedNewsletters)
                {
                    var Id = newsletter.Newsletter.Id;
                    var Name = newsletter.Newsletter.Name;
                    var Pdf = newsletter.Newsletter.Pdf;
                    var orgId = newsletter.OrganizationId;
                    List<UserDto> userlist = new List<UserDto>();
                    var users = dbcontext.Users.Include("Organization").Include("Organization.Portals").Where(x => x.OrganizationId == orgId && x.IsActive == true && x.Organization.Portals.Any(y => y.Active == true)).ToList();
                    userlist = Utility.mapper.Map<List<DAL.User>, List<UserDto>>(users);
                    if (userlist != null && userlist.Count > 0)
                    {
                        count = count + userlist.Count;
                        for (int i = 0; i < userlist.Count(); i++)
                        {
                            var dashboardMessage = dbcontext.DashboardMessageTypes.Where(x => x.Type == IncentiveMessageTypes.Newsletter).FirstOrDefault();
                            if (dashboardMessage != null)
                            {
                                UserDashboardMessageDto dto = new UserDashboardMessageDto();
                                dto.Parameters = Name;
                                dto.UserId = userlist[i].Id;
                                dto.Url = dashboardMessage.Url + Pdf;
                                dto.New = true;
                                dto.MessageType = dashboardMessage.Id;
                                dto.Active = true;
                                dto.CreatedOn = DateTime.UtcNow;
                                dashboardMessagelist.Add(dto);
                            }
                        }
                    }

                }
                if (dashboardMessagelist != null && dashboardMessagelist.Count > 0)
                {
                    for (int i = 0; i < dashboardMessagelist.Count; i = i + 1000)
                    {
                        var list = dashboardMessagelist.Skip(i).Take(1000);
                        AssignNewsletter(list);
                    }
                }
                foreach (var newsletter in assignedNewsletters)
                {
                    var newsletterDal = dbcontext.AssignedNewsletters.Where(x => x.Id == newsletter.Id).FirstOrDefault();
                    if (newsletterDal != null)
                    {
                        newsletterDal.Completed = true;
                        dbcontext.AssignedNewsletters.Attach(newsletterDal);
                        dbcontext.Entry(newsletterDal).State = EntityState.Modified;
                        dbcontext.SaveChanges();
                    }
                }
            }
            return count;
        }

        public void AssignNewsletter(IEnumerable<UserDashboardMessageDto> request)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                /*using (var dbContext = new InterventDatabase())
                {
                    //dbContext.Configuration.AutoDetectChangesEnabled = false;
                    foreach (UserDashboardMessageDto req in request)
                    {
                        var userDashboardModel = new DAL.UserDashboardMessage();
                        dbContext.UserDashboardMessages.Add(CommonReader.MapToUserDashboardDAL(req, userDashboardModel));
                    }
                    dbContext.SaveChanges();
                }*/
                scope.Complete();
            }
        }
    }
}