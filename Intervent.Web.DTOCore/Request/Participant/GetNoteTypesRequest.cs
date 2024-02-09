namespace Intervent.Web.DTO
{
    public class GetNoteTypesRequest
    {
        public bool isNoteExist { get; set; }

        public bool isCoachNoteExist { get; set; }

        public bool isBioNoteExist { get; set; }

        public bool showHRAReviewNote { get; set; }
    }
}
