using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class NewsLetterUtility
    {
        public static ListNewsletterResponse ListNewsletters(int page, int pageSize, int? totalRecords)
        {
            NewsLetterReader reader = new NewsLetterReader();
            GetNewsletterListRequest newsletterRequest = new GetNewsletterListRequest();
            newsletterRequest.Page = page;
            newsletterRequest.PageSize = pageSize;
            newsletterRequest.TotalRecords = totalRecords;

            return reader.ListNewsletters(newsletterRequest);
        }

        public static IList<AssignedNewsletterDto> ListAssignedNewsletters(int newsletterId)
        {
            NewsLetterReader reader = new NewsLetterReader();
            return reader.ListAssignedNewsletter(newsletterId);
        }

        public static AddNewsletterResponse AddEditNewsletter(int id, string name, string pdf)
        {
            NewsLetterReader reader = new NewsLetterReader();
            AddNewsletterRequest request = new AddNewsletterRequest();
            NewsletterDto newsletter = new NewsletterDto();
            newsletter.Id = id;
            newsletter.Name = name;
            newsletter.Pdf = pdf;
            request.newsletter = newsletter;
            return reader.AddEditNewsletter(request);
        }

        public static bool AssignNewsletter(AssignNewsletterModel model)
        {
            NewsLetterReader reader = new NewsLetterReader();
            AssignNewsletterRequest assignnewsletterRequest = new AssignNewsletterRequest();
            List<AssignedNewsletterDto> newletters = new List<AssignedNewsletterDto>();
            var OrganizationIdStr = model.OrganizationIds.Split(',');

            for (int i = 0; i < OrganizationIdStr.Length; i++)
            {
                AssignedNewsletterDto assignnewsletter = new AssignedNewsletterDto();
                assignnewsletter.NewsletterId = model.NewsletterId;
                assignnewsletter.OrganizationId = Convert.ToInt16(OrganizationIdStr[i]);
                newletters.Add(assignnewsletter);
            }
            assignnewsletterRequest.newletterList = newletters;
            return reader.AssignNewsletter(assignnewsletterRequest);

        }

        public static string GetNewsletterFilePath(int newsletterId)
        {
            NewsLetterReader reader = new NewsLetterReader();
            return reader.GetNewsletterPath(newsletterId);
        }
    }
}