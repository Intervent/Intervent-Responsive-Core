namespace Intervent.Web.DTO
{
    public class GetCoachListResponse
    {
        public IList<Coach> Coaches { get; set; }
    }

    public class Coach
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}