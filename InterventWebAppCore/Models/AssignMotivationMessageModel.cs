namespace InterventWebApp
{
    public class AssignMotivationMessageModel
    {
        public int Id { get; set; }

        public string OrganizationIds { get; set; }

        public string MessageTypes { get; set; }
    }

    public class MotivationMessageModel
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string MessageContent { get; set; }
    }
}