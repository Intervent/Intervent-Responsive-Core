using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class SearchFreeSlotModel
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public IEnumerable<SelectListItem> Times { get; set; }

        public string Time { get; set; }

        public IEnumerable<SelectListItem> Coaches { get; set; }

        public int Coach { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        public string TimeZone { get; set; }

        public IEnumerable<SelectListItem> DaysList { get; set; }

        public string Specialization { get; set; }

        public IEnumerable<SelectListItem> Specializations { get; set; }

        public string Language { get; set; }

        public IEnumerable<SelectListItem> Languages { get; set; }

        public string Day { get; set; }

        public bool ThirtyMinutes { get; set; }

        public bool MaternityManager { get; set; }

        public bool Video { get; set; }

        public bool OnlyHealthCoach { get; set; }

        public string dateFormat { get; set; }
    }

    public class ScheduleAppointmentModel
    {
        public string AppointmentDate { get; set; }

        public IEnumerable<SelectListItem> Lengths { get; set; }

        public int Length { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }

        public int Type { get; set; }

        public int CoachID { get; set; }

        public string Comments { get; set; }

        public string CoachName { get; set; }

        public string TimeZone { get; set; }

        public string MeetingId { get; set; }
    }

    public class AppointmenstModel
    {
        public int CoachId { get; set; }

        public IList<AppointmentDTO> appointments { get; set; }

        public IEnumerable<SelectListItem> Coaches { get; set; }

        public int Coach { get; set; }

        public int ToCoachId { get; set; }

        public bool hasActivePortal { get; set; }

        public int? userinProgramId { get; set; }

        public string dateFormat { get; set; }
    }

    public class CancelAppointmentModel
    {
        public int Id { get; set; }

        public IEnumerable<SelectListItem> reasons { get; set; }

        public int reason { get; set; }

        public string comments { get; set; }

        public bool getAppt { get; set; }
    }
    public class EditAppointmentCommentsModel
    {
        public int Id { get; set; }

        public string comments { get; set; }

        public IEnumerable<SelectListItem> Lengths { get; set; }

        public int Length { get; set; }

        public bool VideoRequired { get; set; }

        public string MeetingId { get; set; }
    }
}