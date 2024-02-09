namespace InterventWebApp
{
    public class CreateOrUpdateWebinarModel
    {
        public int id { get; set; }

        public string webinarId { get; set; }

        public string agenda { get; set; }

        public string topic { get; set; }

        public int duration { get; set; }

        public string password { get; set; }

        public string imageUrl { get; set; }

        public string videoUrl { get; set; }

        public string handout { get; set; }

        public int presentedBy { get; set; }

        public int userId { get; set; }

        public DateTime webinarDate { get; set; }

        public DateTime webinarDateUTC { get; set; }

        public int systemAdminId { get; set; }

        public string zoomClientId { get; set; }

        public string zoomClientSecret { get; set; }

        public string zoomOAuthURL { get; set; }

        public string zoomAccountId { get; set; }

        public string zoomAPIURL { get; set; }

        public string zoomUserId { get; set; }
    }
}
