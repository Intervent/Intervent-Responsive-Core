namespace Intervent.Web.DTO
{
    public sealed class UserHistoryCategoryDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        UserHistoryCategoryDto(int _id, string _description)
        {
            Id = _id;
            Description = _description;
        }

        public static UserHistoryCategoryDto UserProfile = new UserHistoryCategoryDto(1, "User Profile");
        public static UserHistoryCategoryDto HRA = new UserHistoryCategoryDto(2, "HRA");
        public static UserHistoryCategoryDto UserProgram = new UserHistoryCategoryDto(3, "User Programs");
        public static UserHistoryCategoryDto FollowUp = new UserHistoryCategoryDto(4, "FollowUp");
        public static UserHistoryCategoryDto HealthNumbers = new UserHistoryCategoryDto(5, "HealthNumbers");

        public static IEnumerable<UserHistoryCategoryDto> GetAll()
        {
            List<UserHistoryCategoryDto> lst = new List<UserHistoryCategoryDto>();
            lst.Add(UserHistoryCategoryDto.UserProfile);
            lst.Add(UserHistoryCategoryDto.HRA);
            lst.Add(UserHistoryCategoryDto.UserProgram);
            lst.Add(UserHistoryCategoryDto.FollowUp);
            lst.Add(UserHistoryCategoryDto.HealthNumbers);
            return lst;
        }

        public static UserHistoryCategoryDto GetByKey(int id)
        {
            return GetAll().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
