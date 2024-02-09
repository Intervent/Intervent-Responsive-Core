namespace Intervent.Web.DTO
{
    public class PromptDto
    {
        public int Id { get; set; }

        public int KitId { get; set; }

        public string Description { get; set; }

        public bool IsBottom { get; set; }

        public int RefId { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? UpdatedBy { get; set; }

        public int? RefType { get; set; }

        public byte? DisplayOrder { get; set; }
    }

    public class PromptsinKitsCompletedDto
    {
        public int Id { get; set; }

        public int PromptId { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int KitsInUserProgramId { get; set; }

        public bool IsActive { get; set; }
    }
}
