namespace Intervent.Web.DTO
{
    public class TasksDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TaskTypeId { get; set; }

        public string Status { get; set; }

        public int Owner { get; set; }

        public string Comment { get; set; }

        public TaskTypesDto TaskType { get; set; }

        public int UpdatedBy { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public bool IsActive { get; set; }

        public UserDto User { get; set; }

        public UserDto User1 { get; set; }

        public UserDto User2 { get; set; }


    }
}
