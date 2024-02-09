using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class ChallengesModel
    {
        public string Name { get; set; }

        public string MemberSince { get; set; }

        public string Rank { get; set; }

        public double Points { get; set; }

        public double Currency { get; set; }

        public int Keys { get; set; }

        public int IncentiveType { get; set; }

        public string nextRaffle { get; set; }

        public IList<CustomIncentiveTypesDto> customIncentives { get; set; }

        public List<RafflesTrackModel> raffles { get; set; }

        public List<UserRafflesDto> userRaffles { get; set; }

        public List<ChallengesViewModel> challenges { get; set; }

        public string employerIncentiveText { get; set; }

        public string OrganizationName { get; set; }

        public bool HasActivePortal { get; set; }

        public string DateFormat { get; set; }
    }

    public class RafflesTrackModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RaffleDate { get; set; }

        public string Description { get; set; }

        public int Points { get; set; }
    }
}
