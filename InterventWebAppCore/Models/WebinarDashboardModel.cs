namespace InterventWebApp
{
    public class WebinarDashboardModel
    {
        public List<WebinarDetails> upcomingWebinars { get; set; }

        public List<WebinarDetails> completedWebinars { get; set; }
    }

    public class WebinarOccurrence
    {
        public string OccurrenceId { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string Status { get; set; }

        public int Duration { get; set; }

        public string VideoUrl { get; set; }

        public string Handout { get; set; }
    }

    public class WebinarDetails
    {
        public int id { get; set; }

        public string webinarId { get; set; }

        public string topic { get; set; }

        public string agenda { get; set; }

        public string presentedByName { get; set; }

        public string presentedByImageUrl { get; set; }

        public string presentedByBio { get; set; }

        public bool isRegistered { get; set; }

        public string userJoinUrl { get; set; }

        public DateTime startDate { get; set; }

        public string startTime { get; set; }

        public string endTime { get; set; }

        public string dateDetails { get; set; }

        public string daysCount { get; set; }

        public string webinarImageUrl { get; set; }

        public string webinarVideoId { get; set; }

        public string handout { get; set; }

        public int type { get; set; }

        public List<WebinarOccurrence> occurrences { get; set; }

        public WebinarDetails()
        {
        }

        public WebinarDetails(WebinarDetails model)
        {
            this.id = model.id;
            this.webinarId = model.webinarId;
            this.topic = model.topic;
            this.agenda = model.agenda;
            this.presentedByName = model.presentedByName;
            this.presentedByImageUrl = model.presentedByImageUrl;
            this.presentedByBio = model.presentedByBio;
            this.isRegistered = model.isRegistered;
            this.startTime = model.startTime;
            this.endTime = model.endTime;
            this.dateDetails = model.dateDetails;
            this.daysCount = model.daysCount;
            this.webinarImageUrl = model.webinarImageUrl;
            this.webinarVideoId = model.webinarVideoId;
            this.handout = model.handout;
            this.type = model.type;
        }
    }
}
