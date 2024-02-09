using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Data;
using System.Globalization;

namespace Intervent.Web.DataLayer
{
    public class SchedulerReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        #region GetFreeSlot

        public CheckFreeSlotsResponse CheckFreeSlot(GetFreeSlotsRequest request)
        {
            var response = SearchFreeSlot(request);
            CheckFreeSlotsResponse checkResponse = new CheckFreeSlotsResponse();
            if (response != null && response.Availabilities != null && response.Availabilities.Count > 0)
            {
                checkResponse.AvailabilityCount = response.Availabilities.Where(x => x.StartTime.Date > DateTime.Now.Date)
                    .GroupBy(x => x.AvailDate).Select(x => new CountByDate
                    {
                        Date = Convert.ToDateTime(x.Key),
                        NoOfRecords = x.Count()
                    }).ToList();
            }
            return checkResponse;
        }

        public GetFreeSlotsResponse GetFreeSlotforDay(GetFreeSlotsRequest request)
        {
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            var startDate = TimeZoneInfo.ConvertTimeToUtc(request.StartDate, custTZone);
            var endDate = TimeZoneInfo.ConvertTimeToUtc(request.EndDate, custTZone);
            GetFreeSlotsResponse response;
            response = GetFreeSlots(request.thirtyMinutes, startDate, endDate, custTZone, null, request.coachId, null, request.specialities, request.hasFilter, request.OrganizationId, request.adminView, null, request.stateId);
            return response;
        }

        public GetFreeSlotsResponse SearchFreeSlot(GetFreeSlotsRequest request)
        {
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            DateTime custTZoneTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, request.TimeZone);
            if (request.isLMC)
            {
                custTZoneTimeNow = custTZoneTimeNow.AddDays((int)SelfSchedulingLimit.LMC);
            }
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MinValue;

            if (request.StartDate.Kind != DateTimeKind.Utc)
            {
                startDate = TimeZoneInfo.ConvertTimeToUtc(request.StartDate, custTZone);
                endDate = TimeZoneInfo.ConvertTimeToUtc(request.EndDate, custTZone);
            }
            else
            {
                startDate = request.StartDate;
                endDate = request.EndDate;
            }

            GetFreeSlotsResponse response;
            DateTime[] transitionTimes;

            DateTime startTimeInfo = TimeZoneInfo.ConvertTime(request.StartDate, custTZone);
            DateTime endTimeInfo = TimeZoneInfo.ConvertTime(request.EndDate, custTZone);

            if (custTZone.IsDaylightSavingTime(startTimeInfo) == custTZone.IsDaylightSavingTime(endTimeInfo))
                response = GetFreeSlots(request.thirtyMinutes, startDate, endDate, custTZone, request.day, request.coachId, request.video, request.specialities, request.hasFilter, request.OrganizationId, request.adminView, request.languages, request.stateId);
            else
            {
                if (!custTZone.IsDaylightSavingTime(startTimeInfo) && custTZone.IsDaylightSavingTime(endTimeInfo))
                {
                    transitionTimes = GetTransitionTimes(request.EndDate.Year, custTZone);
                    response = GetFreeSlots(request.thirtyMinutes, startDate, transitionTimes[0].Date.AddDays(-1).AddHours(endDate.Hour + 1).AddMinutes(endDate.Minute), custTZone, request.day, request.coachId, request.video, request.specialities, request.hasFilter, request.OrganizationId, request.adminView, request.languages, request.stateId);
                    var response2 = GetFreeSlots(request.thirtyMinutes, transitionTimes[0].Date.AddHours(startDate.Hour - 1).AddMinutes(startDate.Minute), endDate, custTZone, request.day, request.coachId, request.video, request.specialities, request.hasFilter, request.OrganizationId, request.adminView, request.languages, request.stateId);
                    if (response2.Availabilities != null)
                    {
                        if (response.Availabilities == null)
                        {
                            response.Availabilities = response2.Availabilities;
                        }
                        else
                            response.Availabilities.AddRange(response2.Availabilities);
                    }
                }
                else
                {
                    transitionTimes = GetTransitionTimes(request.StartDate.Year, custTZone);
                    if (transitionTimes[1].Year > request.EndDate.Year)
                        transitionTimes = GetTransitionTimes(request.StartDate.Year - 1, custTZone);
                    response = GetFreeSlots(request.thirtyMinutes, startDate, transitionTimes[1].Date.AddDays(-1).AddHours(endDate.Hour - 1).AddMinutes(endDate.Minute), custTZone, request.day, request.coachId, request.video, request.specialities, request.hasFilter, request.OrganizationId, request.adminView, request.languages, request.stateId);
                    var response2 = GetFreeSlots(request.thirtyMinutes, transitionTimes[1].Date.AddHours(startDate.Hour + 1).AddMinutes(startDate.Minute), endDate, custTZone, request.day, request.coachId, request.video, request.specialities, request.hasFilter, request.OrganizationId, request.adminView, request.languages, request.stateId);
                    if (response2.Availabilities != null)
                    {
                        if (response.Availabilities == null)
                        {
                            response.Availabilities = response2.Availabilities;
                        }
                        else
                            response.Availabilities.AddRange(response2.Availabilities);
                    }
                }
            }
            response.Availabilities = response.Availabilities.Where(x => x.StartTime > custTZoneTimeNow).ToList();
            return response;
        }

        private GetFreeSlotsResponse GetFreeSlots(bool thirtyMinutes, DateTime startDate, DateTime endDate, TimeZoneInfo custTZone, string[] day, int? coachId,
            bool? video, string specialities, bool hasFilter, int organizationId, bool adminView, string languages, int? stateId)
        {
            GetFreeSlotsResponse response = new GetFreeSlotsResponse();
            StoredProcedures sp = new StoredProcedures();
            List<GetCoachAvailability_Result> availability = null;
            if (hasFilter)
            {
                availability = sp.GetCoachAvailability(organizationId, coachId, startDate, endDate, video, specialities, adminView, languages, stateId);
            }
            else
            {
                availability = sp.GetCoachAvailability(organizationId, coachId, startDate, endDate, null, null, adminView, languages, stateId);
            }

            var aptStartDate = startDate.AddMinutes(-15);

            if (availability != null)
            {
                List<AvailabilityDto> availabilitiles = new List<AvailabilityDto>();

                foreach (var available in availability)
                {
                    var StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(available.StartTime.ToString()), custTZone);
                    var EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(available.EndTime.ToString()), custTZone);
                    while (StartDateTime < EndDateTime)
                    {
                        if (day == null || day.Count() == 0 || (day.Contains(StartDateTime.DayOfWeek.ToString())))
                        {
                            AvailabilityDto avail = new AvailabilityDto();
                            avail.CoachId = Convert.ToInt32((available.CoachId.ToString()));
                            if (hasFilter)
                                avail.CoachName = available.FirstName + " " + available.LastName;
                            avail.AvailDate = StartDateTime.ToShortDateString();
                            avail.StartTime = Convert.ToDateTime(StartDateTime.ToString());
                            avail.EndTime = Convert.ToDateTime(StartDateTime.AddMinutes(15).ToString());
                            avail.HasNextAvail = true;
                            avail.UserRoleCode = available.UserRoleCode;
                            availabilitiles.Add(avail);
                        }
                        StartDateTime = StartDateTime.AddMinutes(15);
                    }
                }
                //remove out of range slot (e.g: 3 - 6)
                if (startDate.Hour != endDate.Hour)
                {
                    var StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(startDate, custTZone);
                    var EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(endDate, custTZone);
                    for (int i = 0; i < availabilitiles.Count(); i++)
                    {
                        if (availabilitiles[i].StartTime.Hour < StartDateTime.Hour || ((EndDateTime.Hour != 0 && availabilitiles[i].EndTime.Hour > EndDateTime.Hour) || (EndDateTime.Hour == 0 && availabilitiles[i].EndTime.Hour > 24)))
                            availabilitiles[i].Disable = true;
                        else if (availabilitiles[i].EndTime.Hour == EndDateTime.Hour)
                        {
                            if (availabilitiles[i].EndTime.Minute > EndDateTime.Minute)
                                availabilitiles[i].Disable = true;
                        }
                    }
                    availabilitiles.RemoveAll(x => x.Disable == true);

                }
                //remove 15 min slots
                if (thirtyMinutes)
                {
                    availabilitiles = availabilitiles.OrderBy(x => x.CoachId).ThenBy(x => x.StartTime).ToList();

                    for (int i = 0; i < availabilitiles.Count(); i++)
                    {
                        if (i == 0)
                        {
                            if (availabilitiles.Count() == 1)
                            {
                                availabilitiles[i].Disable = true;
                            }
                            else
                            {
                                if (!((Convert.ToDateTime(availabilitiles[i].StartTime) == Convert.ToDateTime(availabilitiles[i + 1].StartTime).AddMinutes(-15)) &&
                                    availabilitiles[i].CoachId == availabilitiles[i + 1].CoachId))
                                {
                                    availabilitiles[i].Disable = true;
                                }
                            }
                        }
                        else if (availabilitiles.Count() == i + 1)
                        {
                            availabilitiles[i].HasNextAvail = false;
                        }
                        else
                        {
                            if (!((Convert.ToDateTime(availabilitiles[i].StartTime) == Convert.ToDateTime(availabilitiles[i - 1].StartTime).AddMinutes(+15)) &&
                                availabilitiles[i].CoachId == availabilitiles[i - 1].CoachId) &&
                                !((Convert.ToDateTime(availabilitiles[i].StartTime) == Convert.ToDateTime(availabilitiles[i + 1].StartTime).AddMinutes(-15)) &&
                                availabilitiles[i].CoachId == availabilitiles[i + 1].CoachId))
                            {
                                availabilitiles[i].Disable = true;
                            }
                            else if (!((Convert.ToDateTime(availabilitiles[i].StartTime) == Convert.ToDateTime(availabilitiles[i + 1].StartTime).AddMinutes(-15)) &&
                                availabilitiles[i].CoachId == availabilitiles[i + 1].CoachId))
                            {
                                availabilitiles[i].HasNextAvail = false;
                            }
                        }
                    }
                    availabilitiles.RemoveAll(x => x.Disable == true || x.HasNextAvail == false);
                    availabilitiles = availabilitiles.OrderBy(x => x.StartTime).ThenBy(x => x.CoachId).ToList();
                }
                response.Availabilities = availabilitiles;
            }
            return response;
        }

        #endregion GetFreeSlot

        #region Calculate daylight transition time

        private DateTime[] GetTransitionTimes(int year, TimeZoneInfo timeZoneInfo)
        {
            TimeZoneInfo.AdjustmentRule[] adjustments = timeZoneInfo.GetAdjustmentRules();
            if (adjustments.Length == 0)
                return null;

            int startYear = year;
            int endYear = startYear;
            DateTime[] dates = new DateTime[2];

            TimeZoneInfo.AdjustmentRule adjustment = GetAdjustment(adjustments, year);
            if (adjustment == null)
                return null;

            TimeZoneInfo.TransitionTime startTransition, endTransition;

            // Determine if starting transition is fixed
            startTransition = adjustment.DaylightTransitionStart;
            // Determine if starting transition is fixed and display transition info for year
            if (startTransition.IsFixedDateRule)
                dates[0] = new DateTime(year, startTransition.Month, startTransition.Day);
            else
                dates[0] = DisplayTransitionInfo(startTransition, startYear);

            // Determine if ending transition is fixed and display transition info for year
            endTransition = adjustment.DaylightTransitionEnd;

            // Does the transition back occur in an earlier month (i.e.,
            // the following year) than the transition to DST? If so, make
            // sure we have the right adjustment rule.
            if (endTransition.Month < startTransition.Month)
            {
                endTransition = GetAdjustment(adjustments, year + 1).DaylightTransitionEnd;
                endYear++;
            }

            if (endTransition.IsFixedDateRule)
                dates[1] = new DateTime(year, endTransition.Month, endTransition.Day);
            else
                dates[1] = DisplayTransitionInfo(endTransition, endYear);

            return dates;
        }

        private static TimeZoneInfo.AdjustmentRule GetAdjustment(TimeZoneInfo.AdjustmentRule[] adjustments, int year)
        {
            // Iterate adjustment rules for time zone
            foreach (TimeZoneInfo.AdjustmentRule adjustment in adjustments)
            {
                // Determine if this adjustment rule covers year desired
                if (adjustment.DateStart.Year <= year && adjustment.DateEnd.Year >= year)
                    return adjustment;
            }
            return null;
        }

        private DateTime DisplayTransitionInfo(TimeZoneInfo.TransitionTime transition, int year)
        {
            // For non-fixed date rules, get local calendar
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            // Get first day of week for transition
            // For example, the 3rd week starts no earlier than the 15th of the month
            int startOfWeek = transition.Week * 7 - 6;
            // What day of the week does the month start on?
            int firstDayOfWeek = (int)cal.GetDayOfWeek(new DateTime(year, transition.Month, 1));
            // Determine how much start date has to be adjusted
            int transitionDay;
            int changeDayOfWeek = (int)transition.DayOfWeek;

            if (firstDayOfWeek <= changeDayOfWeek)
                transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
            else
                transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);

            // Adjust for months with no fifth week
            if (transitionDay > cal.GetDaysInMonth(year, transition.Month))
                transitionDay -= 7;

            return new DateTime(year, transition.Month, transitionDay, transition.TimeOfDay.Hour, transition.TimeOfDay.Minute, transition.TimeOfDay.Second);
        }

        #endregion Calculate daylight transition time

        #region ScheduleAppointment

        public bool UpdateTextResponseFromTwilio(string smsSid, string textResponse)
        {
            var fromDate = DateTime.UtcNow.AddHours(-2);
            var toDate = DateTime.UtcNow.AddHours(2);
            var appointment = context.Appointments.Where(x => x.MessageSID == smsSid && x.Date > fromDate && x.Date < toDate).FirstOrDefault();
            if (appointment != null)
            {
                if (string.IsNullOrWhiteSpace(appointment.TextResponse))
                    appointment.TextResponse = textResponse;
                else
                    appointment.TextResponse += textResponse;
                context.Appointments.Attach(appointment);
                context.Entry(appointment).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<int> UpdateMessageSID(Dictionary<int, string> SIDs)
        {
            List<int> notFound = new List<int>();
            foreach (var sid in SIDs)
            {
                try
                {
                    var appointment = context.Appointments.Where(x => x.Id == sid.Key).FirstOrDefault();
                    if (appointment != null)
                    {
                        appointment.MessageSID = sid.Value;
                        appointment.UpdatedOn = DateTime.UtcNow;
                        context.Appointments.Attach(appointment);
                        context.Entry(appointment).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        notFound.Add(sid.Key);
                    }
                }
                catch (Exception ex)
                {
                    LogReader logReader = new LogReader();
                    var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                    logReader.WriteLogMessage(logEvent);
                }
            }
            return notFound;
        }

        public ScheduleAppointmentResponse ScheduleAppointment(ScheduleAppointmentRequest request)
        {
            ScheduleAppointmentResponse response = new ScheduleAppointmentResponse();
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);

            var appointmentDate = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(request.Appointment.Date), timeZone);
            DateTime futureApptDate;
            if (request.Appointment.Minutes == 15)
                futureApptDate = appointmentDate;
            else
                futureApptDate = appointmentDate.AddMinutes(15);
            var avail = context.Availabilities
                .Where(x => (x.StartTime >= appointmentDate && x.StartTime <= futureApptDate
                && x.CoachId == request.Appointment.CoachId && x.Active == true && x.IsBooked == false)).Count();

            if ((avail >= 1 && request.Appointment.Minutes == 15) || (avail == 2 && request.Appointment.Minutes == 30))
            {
                int appointmentId;
                DAL.Appointment newAppointment = new DAL.Appointment();
                newAppointment.Active = true;
                newAppointment.UserId = request.Appointment.UserId;
                newAppointment.CoachId = request.Appointment.CoachId;
                newAppointment.Comments = request.Appointment.Comments;
                newAppointment.Minutes = request.Appointment.Minutes;
                newAppointment.Type = request.Appointment.Type;
                newAppointment.Date = Convert.ToDateTime(appointmentDate);
                newAppointment.CreatedOn = DateTime.UtcNow;
                newAppointment.CreatedBy = request.Appointment.CreatedBy;
                newAppointment.VideoRequired = request.Appointment.VideoRequired;
                context.Appointments.Add(newAppointment);
                context.SaveChanges();
                appointmentId = newAppointment.Id;
                response.apptId = appointmentId;
                ParticipantReader participantReader = new ParticipantReader();
                participantReader.EditContactRequirementAlertActiveStatus(request.Appointment.UserId, null);
                //add to participant's dashboard
                CommonReader reader = new CommonReader();
                reader.AddDashboardMessage(request.Appointment.UserId, IncentiveMessageTypes.Appointment, null, appointmentId);
                UpdateAvailabilities(appointmentDate, appointmentDate.AddMinutes(request.Appointment.Minutes), request.Appointment.CoachId);
            }
            else
            {
                response.Status = 2;
                response.ErrorMessage = "Sorry, this appointment has been taken. Please go back and try again.";
            }
            return response;
        }

        #endregion ScheduleAppointment

        public void UpdateAvailabilities(DateTime startDate, DateTime endDate, int coachId, bool book = true)
        {
            var avails = context.Availabilities
                   .Where(x => (x.StartTime >= startDate && x.StartTime < endDate
                   && x.CoachId == coachId && x.Active == true)).ToList();
            foreach (var availability in avails)
            {
                availability.IsBooked = book;
                availability.UpdatedOn = DateTime.UtcNow;
                context.Entry(availability).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        #region GetAppointments
        public GetAppointmentTypeResponse GetAppointmentType(GetAppointmentTypesRequest request)
        {
            GetAppointmentTypeResponse response = new GetAppointmentTypeResponse();
            var appoinmentTypes = context.AppointmentTypes.ToList();
            response.AppointmentTypes = Utility.mapper.Map<IList<DAL.AppointmentType>, IList<AppointmentTypesDto>>(appoinmentTypes).ToList();
            return response;
        }

        public GetAppointmentsCountResponse GetAppointmentsCount(GetAppointmentsCountRequest request)
        {
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            var startDate = TimeZoneInfo.ConvertTimeToUtc(request.StartDate, custTZone);
            var endDate = TimeZoneInfo.ConvertTimeToUtc(request.EndDate, custTZone);

            GetAppointmentsCountResponse response = new GetAppointmentsCountResponse();

            var appointments = context.Appointments
                .Where(x => ((x.CoachId == request.coachId || request.coachId == null) &&
                    (x.Date >= startDate || startDate == null) && (x.Date <= endDate || endDate == null) && (x.Active == true || (x.Active == false && x.InActiveReason == 5)))).ToList();

            if (appointments != null && appointments.Count > 0)
            {
                List<CountByDate> counts = new List<CountByDate>();
                foreach (var appointment in appointments)
                {
                    CountByDate count = new CountByDate();
                    count.Date = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(appointment.Date.ToString()), custTZone).Date;
                    count.NoOfRecords = 1;
                    counts.Add(count);
                }
                response.appointmentsCount = counts.GroupBy(x => x.Date).Select(x => new CountByDate
                {
                    Date = x.Key,
                    NoOfRecords = x.Count()
                }).ToList();
            }
            return response;
        }

        public GetAppointmentsResponse GetAppointments(GetAppointmentsRequest request)
        {
            DateTime startDate = DateTime.MinValue, endDate = DateTime.MinValue;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            if (request.StartDate.HasValue)
                request.StartDate = DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Unspecified);
            if (request.EndDate.HasValue)
                request.EndDate = DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Unspecified);
            if (request.StartDate.HasValue)
                startDate = TimeZoneInfo.ConvertTimeToUtc(request.StartDate.Value, custTZone);
            if (request.EndDate.HasValue)
                endDate = TimeZoneInfo.ConvertTimeToUtc(request.EndDate.Value, custTZone);

            GetAppointmentsResponse response = new GetAppointmentsResponse();

            var appointments = context.Appointments.Include("User").Include("User.TimeZone").Include("User.UsersinPrograms").Include("User.UsersinPrograms.ProgramsinPortal").Include("User1").Include("User1.AdminProperty").Include("User2").Include("AppointmentFeedback").Include("AppointmentType")
                .Where(x => ((x.CoachId == request.coachId || request.coachId == null) && ((x.Date >= startDate || startDate == DateTime.MinValue) && (!request.portalStartDate.HasValue || x.Date >= request.portalStartDate))
                    && ((x.Date <= endDate || endDate == DateTime.MinValue)) /*&& x.Date <= request.portalEndDate) && x.Date >= DateTime.UtcNow*/ && (x.UserId == request.userId || request.userId == null) && (x.Active == true || (x.Active == false && x.InActiveReason == 5)))).OrderByDescending(x => x.Date).ToList();

            if (appointments.Count > 0)
            {
                response.Appointments = new List<AppointmentDTO>();
                var order = appointments.Where(x => x.Active).ToList().Count;
                foreach (var appointment in appointments)
                {
                    AppointmentDTO apt = new AppointmentDTO();
                    var newDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(appointment.Date.ToString()), custTZone);
                    apt.UTCDate = appointment.Date;
                    apt.Date = newDate.ToShortDateString();
                    apt.StartTime = newDate.ToShortTimeString();
                    apt.PhoneNumber = appointment.User.PhoneNumber;
                    apt.InActiveReason = appointment.InActiveReason;
                    apt.ParticipantPic = !string.IsNullOrEmpty(appointment.User.Picture) ? "/ProfilePictures/" + appointment.User.Picture : appointment.User.Gender == 1 ? "/Images/avatar-male.svg" : "/Images/avatar-female.svg";
                    apt.EndTime = newDate.AddMinutes(appointment.Minutes).ToShortTimeString();
                    apt.Type = appointment.Type;
                    apt.UserName = appointment.User.FirstName + " " + appointment.User.LastName;
                    apt.UserProgram = appointment.User.UsersinPrograms.Select(x => x.ProgramsinPortal.NameforAdmin).FirstOrDefault();
                    apt.Comments = appointment.Comments;
                    apt.TextResponse = appointment.TextResponse;
                    apt.AppointmentType = Utility.mapper.Map<AppointmentType, AppointmentTypesDto>(appointment.AppointmentType);
                    apt.Id = appointment.Id;
                    apt.CoachName = appointment.User1.FirstName + " " + appointment.User1.LastName;
                    if (appointment.User1.AdminProperty != null)
                    {
                        apt.CoachBio = appointment.User1.AdminProperty.Profile;
                        if (appointment.User1.AdminProperty.Video.HasValue)
                            apt.CoachMeetingId = appointment.User1.AdminProperty.MeetingId;
                    }
                    apt.CoachPic = appointment.User1.Picture;
                    apt.Minutes = appointment.Minutes;
                    apt.CoachId = appointment.User1.Id;
                    apt.UserId = appointment.UserId;
                    if (appointment.User.TimeZoneId.HasValue)
                        apt.UserTimeZone = appointment.User.TimeZone.TimeZoneId;
                    apt.ScheduledBy = appointment.User2.FirstName + " " + appointment.User2.LastName;
                    apt.Active = appointment.Active;
                    apt.InActiveReason = appointment.InActiveReason;
                    apt.VideoRequired = appointment.VideoRequired;
                    if (apt.Active)
                        apt.Order = order--;
                    if (appointment.AppointmentFeedback != null)
                    {
                        AppointmentFeedbackDto feedback = new AppointmentFeedbackDto();
                        feedback.Comments = appointment.AppointmentFeedback.Comments;
                        feedback.Rating = appointment.AppointmentFeedback.Rating;
                        apt.AppointmentFeedback = feedback;
                    }
                    response.Appointments.Add(apt);
                }
            }
            return response;
        }

        public GetCancellationReasonResponse GetCancellationReasons(GetCancellationReasonRequest request)
        {
            GetCancellationReasonResponse response = new GetCancellationReasonResponse();
            var appointment = context.Appointments.Include("User").Include("User.Organization").Where(x => x.Id == request.AppId).FirstOrDefault();
            var appointmentstart = appointment.Date;
            var date = Convert.ToDateTime(DateTime.UtcNow);
            List<CancellationReason> reasonTypes = new List<CancellationReason>();
            if (date >= appointmentstart.AddHours(-1))
            {
                reasonTypes = context.CancellationReasons.ToList();
            }
            else
            {
                reasonTypes = context.CancellationReasons.Where(x => x.Id != 5).ToList();
            }
            response.ReasonTypes = Utility.mapper.Map<IList<DAL.CancellationReason>, IList<CancellationReasonDto>>(reasonTypes).ToList();
            if (appointment.User.Organization != null && appointment.User.Organization.OwnCoach)
            {
                foreach (var reason in response.ReasonTypes)
                {
                    reason.Reason = reason.Reason.Replace("INTERVENT", appointment.User.Organization.Name);
                }
            }
            return response;

        }

        public GetAppointmentDetailsResponse GetAppointmentDetails(GetAppointmentDetailsRequest request)
        {
            GetAppointmentDetailsResponse response = new GetAppointmentDetailsResponse();
            var appoinment = context.Appointments.Include("User").Include("User1").Where(x => x.Id == request.apptId && x.Active == true).FirstOrDefault();
            if (appoinment != null)
            {
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
                AppointmentDTO dto = new AppointmentDTO();
                dto.Minutes = appoinment.Minutes;
                dto.Type = appoinment.Type;
                dto.User1 = Utility.mapper.Map<DAL.User, UserDto>(appoinment.User1);
                dto.Date = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(appoinment.Date.ToString()), custTZone).ToString();
                response.appointment = dto;
            }
            return response;
        }

        #endregion GetAppointments

        #region GetCoachAvailability

        public GetCoachAvailabilityResponse GetCoachAvailability(GetCoachAvailabilityRequest request)
        {
            GetCoachAvailabilityResponse response = new GetCoachAvailabilityResponse();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
            var availability = context.Availabilities.Where(x => x.CoachId == request.coachId && x.Active == true).ToList();
            if (availability != null && availability.Count > 0)
            {
                List<AvailabilityDto> availabilitiles = new List<AvailabilityDto>();
                for (int i = 0; i < availability.Count; i++)
                {
                    var StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(availability[i].StartTime.ToString()), custTZone);
                    var EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(availability[i].EndTime.ToString()), custTZone);
                    AvailabilityDto avail = new AvailabilityDto();
                    avail.CoachId = Convert.ToInt32((availability[i].CoachId.ToString()));
                    avail.AvailDate = StartDateTime.ToShortDateString();
                    avail.StartTime = Convert.ToDateTime(StartDateTime.ToString());
                    avail.EndTime = Convert.ToDateTime(StartDateTime.AddMinutes(15).ToString());
                    avail.RefId = availability[i].RefId.ToString();
                    availabilitiles.Add(avail);
                    StartDateTime = StartDateTime.AddMinutes(15);
                }
                response.availabilityList = availabilitiles;
            }
            return response;
        }

        #endregion

        #region GetCoachAvailabilityDetails

        public GetCoachAvailabilityDetailsResponse GetCoachAvailabilityDetails(GetCoachAvailabilityDetailsRequest request)
        {
            GetCoachAvailabilityDetailsResponse response = new GetCoachAvailabilityDetailsResponse();
            TimeZoneInfo manRefZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            request.StartDate = DateTime.SpecifyKind(request.StartDate.Value, DateTimeKind.Unspecified);
            DateTime? avlStartDateTime = request.StartDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.StartDate.Value, manRefZone) : request.StartDate;
            var availability = context.Availabilities.Where(x => x.RefId == request.RefId && (!avlStartDateTime.HasValue || x.StartTime >= avlStartDateTime)).OrderBy(x => x.StartTime).ToList();
            if (availability != null && availability.Count > 0)
            {
                if (availability.Count > 1)
                {
                    var StartDateTime = Convert.ToDateTime(availability[0].StartTime.ToString());
                    var EndDateTime = Convert.ToDateTime(availability[availability.Count - 1].StartTime.ToString());
                    StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(StartDateTime, manRefZone);
                    EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(EndDateTime, manRefZone);
                    //get from and to date
                    if (StartDateTime.Date != EndDateTime.Date)
                    {
                        response.FromDate = StartDateTime.Date.ToString();
                        response.ToDate = EndDateTime.Date.ToString();
                    }
                    response.StartDate = StartDateTime.ToString();
                    response.EndDate = EndDateTime.AddMinutes(15).ToString();
                    List<string> daysList = new List<string>();
                    for (int i = 1; i < availability.Count; i++)
                    {
                        var dStartDate = Convert.ToDateTime(availability[i].StartTime.ToString());
                        dStartDate = TimeZoneInfo.ConvertTimeFromUtc(dStartDate, manRefZone);
                        if (!daysList.Contains(dStartDate.DayOfWeek.ToString().Substring(0, 2).ToUpper()))
                        {
                            daysList.Add((dStartDate.DayOfWeek.ToString().Substring(0, 2).ToUpper()));
                        }
                    }
                    response.Days = daysList;
                }
                else
                {
                    var StartDateTime = Convert.ToDateTime(availability[0].StartTime.ToString());
                    StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(StartDateTime, manRefZone);
                    response.StartDate = StartDateTime.ToString();
                    response.EndDate = StartDateTime.AddMinutes(15).ToString();
                }
            }
            return response;
        }

        #endregion

        #region SetCoachAvailability

        static object lockobj = new object();
        public SetCoachAvailabilityResponse SetCoachAvailability(SetCoachAvailabilityRequest request)
        {
            SetCoachAvailabilityResponse response = new SetCoachAvailabilityResponse();
            List<string> status = new List<string>();
            var availability = CheckAvailabilityExist(request);
            if (availability.availabilityList.Count == 0)
            {
                TimeZoneInfo meanRefZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
                DateTime startFirstRecord = request.StartDateTime;
                DateTime endFirstRecord = request.EndDateTime;
                var mTable = new DataTable();
                DataRow mRow = mTable.NewRow();
                mTable.Columns.Add("StartDate", typeof(DateTime));
                long refId;
                ///create refid
                lock (lockobj)
                {
                    refId = DateTime.Now.Ticks;
                }
                while (startFirstRecord < endFirstRecord)
                {
                    mTable.Rows.Add(TimeZoneInfo.ConvertTimeToUtc(startFirstRecord, meanRefZone));
                    startFirstRecord = startFirstRecord.AddMinutes(15);
                }
                if (!string.IsNullOrEmpty(request.Days))
                {
                    List<string> daysList = request.Days.Split('-').ToList<string>();
                    foreach (DateTime day in EachDay(request.FromDate.Value.AddDays(1), request.ToDate.Value))
                    {
                        if (daysList.Contains(day.DayOfWeek.ToString().Substring(0, 2).ToUpper()))
                        {
                            var newStartTime = day.Date.AddHours(request.StartDateTime.Hour).AddMinutes(request.StartDateTime.Minute);
                            var newEndTime = day.Date.AddHours(request.EndDateTime.Hour).AddMinutes(request.EndDateTime.Minute);
                            while (newStartTime < newEndTime)
                            {
                                mTable.Rows.Add(TimeZoneInfo.ConvertTimeToUtc(newStartTime, meanRefZone));
                                newStartTime = newStartTime.AddMinutes(15);
                            }
                        }
                    }
                }
                StoredProcedures sp = new StoredProcedures();
                var avail = new SqlParameter("Avail", SqlDbType.Structured);
                avail.Value = mTable;
                avail.TypeName = "dbo.SetAvail";
                SqlParameter sqlCoachId = new SqlParameter("CoachId", request.coachId);
                SqlParameter sqlRefId = new SqlParameter("RefId", refId);
                SqlParameter createdBy = new SqlParameter("CreatedBy", request.CreatedBy);
                SqlParameter createdOn = new SqlParameter("CreatedOn", DateTime.UtcNow);
                sp.SetCoachAvailability(avail, sqlCoachId, sqlRefId, createdBy, createdOn);
                status.Add("success");
            }
            else
            {
                for (var i = 0; i < availability.availabilityList.Count; i++)
                {
                    status.Add(availability.availabilityList[i].StartTime.ToString());
                }
                //response.availabilityList = availability.availabilityList;
            }
            response.status = status;
            return response;
        }

        public GetCoachAvailabilityResponse CheckAvailabilityExist(SetCoachAvailabilityRequest request)
        {
            GetCoachAvailabilityResponse availabilityList = new GetCoachAvailabilityResponse();
            List<AvailabilityDto> availabilitiles = new List<AvailabilityDto>();
            TimeZoneInfo meanRefZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            DateTime startFirstRecord = TimeZoneInfo.ConvertTimeToUtc(request.StartDateTime, meanRefZone);
            DateTime endLastRecord = request.ToDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.ToDate.Value.AddDays(1), meanRefZone) : TimeZoneInfo.ConvertTimeToUtc(request.EndDateTime, meanRefZone);
            var availability = context.Availabilities
                    .Where(x => x.CoachId == request.coachId && x.StartTime >= startFirstRecord && x.StartTime <= endLastRecord && x.Active).ToList();
            foreach (var available in availability)
            {
                var StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(available.StartTime.ToString()), meanRefZone);
                var EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(available.EndTime.ToString()), meanRefZone);
                while (StartDateTime < EndDateTime)
                {
                    if (request.Days == null || request.Days.Count() == 0 || (request.Days.Contains(StartDateTime.DayOfWeek.ToString().Substring(0, 2).ToUpper())) || (request.StartDateTime.Date == StartDateTime.Date))
                    {
                        AvailabilityDto avail = new AvailabilityDto();
                        avail.CoachId = Convert.ToInt32((available.CoachId.ToString()));
                        avail.AvailDate = StartDateTime.ToShortDateString();
                        avail.StartTime = Convert.ToDateTime(StartDateTime.ToString());
                        avail.EndTime = Convert.ToDateTime(StartDateTime.AddMinutes(15).ToString());
                        avail.HasNextAvail = true;
                        availabilitiles.Add(avail);
                    }
                    StartDateTime = StartDateTime.AddMinutes(15);
                }
            }
            if (request.StartDateTime.TimeOfDay != request.EndDateTime.TimeOfDay)
            {
                var startDateTime = request.StartDateTime;
                var endDateTime = request.EndDateTime;
                for (int i = 0; i < availabilitiles.Count(); i++)
                {
                    if (availabilitiles[i].StartTime.TimeOfDay >= startDateTime.TimeOfDay && availabilitiles[i].EndTime.TimeOfDay <= endDateTime.TimeOfDay)
                        availabilitiles[i].Disable = false;
                    else
                        availabilitiles[i].Disable = true;
                }
                availabilitiles.RemoveAll(x => x.Disable == true);
            }
            availabilityList.availabilityList = availabilitiles;
            return availabilityList;
        }

        public IEnumerable<DateTime> EachDay(DateTime fromDate, DateTime toDate)
        {
            for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
                yield return day;
        }

        #endregion

        #region DeleteCoachAvailability

        public DeleteCoachAvailabilityResponse DeleteCoachAvailability(DeleteCoachAvailabilityRequest request)
        {
            DeleteCoachAvailabilityResponse response = new DeleteCoachAvailabilityResponse();
            TimeZoneInfo meanRefZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            var startTime = TimeZoneInfo.ConvertTimeToUtc(request.startTime, meanRefZone);
            var endTime = TimeZoneInfo.ConvertTimeToUtc(request.endTime, meanRefZone);
            List<DAL.Availability> availabilities = null;
            List<DAL.Availability> bookedAvailabilities = null;
            if (request.RefId > 0 && (request.AllFuture.HasValue && request.AllFuture.Value))
            {
                bookedAvailabilities = context.Availabilities.Where(x => x.RefId == request.RefId && x.StartTime >= startTime && x.StartTime <= request.ToDate.Value && x.IsBooked == true).ToList();
                if (bookedAvailabilities.Count() == 0)
                    availabilities = context.Availabilities
                        .Where(x => x.RefId == request.RefId && x.StartTime >= startTime && new DateTime(1, 1, 1, x.StartTime.Hour, x.StartTime.Minute, x.StartTime.Second).TimeOfDay >= startTime.TimeOfDay && x.StartTime <= request.ToDate.Value).ToList();
            }
            else
            {
                bookedAvailabilities = context.Availabilities.Where(x => x.RefId == request.RefId && x.StartTime >= startTime && x.EndTime <= endTime && x.IsBooked == true).ToList();
                if (bookedAvailabilities.Count() == 0)
                    availabilities = context.Availabilities.Where(x => x.RefId == request.RefId && x.StartTime >= startTime && x.EndTime <= endTime).ToList();
            }
            if (availabilities != null && availabilities.Count > 0)
            {
                var mTable = new DataTable();
                DataRow mRow = mTable.NewRow();
                mTable.Columns.Add("Id", typeof(Int32));

                if (!string.IsNullOrEmpty(request.Days))
                {
                    List<string> daysList = request.Days.Split('-').ToList<string>();
                    for (int i = 0; i < availabilities.Count; i++)
                    {
                        var time = TimeZoneInfo.ConvertTimeToUtc(availabilities[i].StartTime, meanRefZone);
                        if (daysList.Contains(time.DayOfWeek.ToString().Substring(0, 2).ToUpper()))
                        {
                            mTable.Rows.Add(availabilities[i].Id);
                        }
                        else if (availabilities[i].StartTime <= endTime)
                        {
                            mTable.Rows.Add(availabilities[i].Id);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < availabilities.Count; i++)
                    {
                        mTable.Rows.Add(availabilities[i].Id);
                    }
                }
                StoredProcedures sp = new StoredProcedures();
                var avail = new SqlParameter("AvailId", SqlDbType.Structured);
                avail.Value = mTable;
                avail.TypeName = "dbo.DeleteAvail";
                SqlParameter updatedBy = new SqlParameter("UpdatedBy", request.UpdatedBy);
                SqlParameter updatedOn = new SqlParameter("UpdatedOn", DateTime.UtcNow);
                sp.DeleteCoachAvailability(avail, updatedBy, updatedOn);
                response.Status = true;
            }
            else
            {
                foreach (var available in bookedAvailabilities)
                {
                    var StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(available.StartTime.ToString()), meanRefZone);
                    response.bookedAvailabilities = new List<string>();
                    response.bookedAvailabilities.Add(Convert.ToDateTime(StartDateTime).ToString());
                    response.Status = false;
                }
            }
            return response;
        }

        #endregion

        #region GetCoachCalendar

        public GetCoachCalendarResponse GetCoachCalendar(GetCoachCalendarRequest request)
        {
            GetCoachCalendarResponse response = new GetCoachCalendarResponse();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZoneId);
            var availability = context.Availabilities.Where(x => ((x.StartTime >= request.FromDate) && (x.StartTime <= request.ToDate)) && x.Active == true && x.CoachId == request.CoachId && x.IsBooked != true).ToList();
            if (availability != null && availability.Count > 0)
            {
                List<AvailabilityDto> availabilitiles = new List<AvailabilityDto>();
                for (int i = 0; i < availability.Count; i++)
                {
                    var StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(availability[i].StartTime.ToString()), custTZone);
                    var EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(availability[i].EndTime.ToString()), custTZone);
                    AvailabilityDto avail = new AvailabilityDto();
                    avail.CoachId = Convert.ToInt32((availability[i].CoachId.ToString()));
                    avail.AvailDate = StartDateTime.ToShortDateString();
                    avail.StartTime = Convert.ToDateTime(StartDateTime.ToString());
                    avail.EndTime = Convert.ToDateTime(StartDateTime.AddMinutes(15).ToString());
                    availabilitiles.Add(avail);
                    StartDateTime = StartDateTime.AddMinutes(15);
                }
                response.Availabilities = availabilitiles;
            }
            var appointments = context.Appointments.Include("User").Include("User1")
                .Where(x => ((((x.Date >= request.FromDate) && (x.Date <= request.ToDate)) || (x.Minutes == 30 && (request.FromDate >= x.Date && request.FromDate <= x.Date.AddMinutes(30)))) && x.CoachId == request.CoachId && (x.Active == true || (x.Active == false && x.InActiveReason == 5)))).ToList();
            if (appointments != null && appointments.Count() > 0)
            {
                response.Appointments = new List<AppointmentDTO>();
                foreach (var appointment in appointments)
                {
                    var startDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(appointment.Date.ToString()), custTZone);
                    DateTime? endDate = null;
                    if (appointment.Minutes == 30)
                        endDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(appointment.Date.AddMinutes(30).ToString()), custTZone);
                    else
                        endDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(appointment.Date.AddMinutes(15).ToString()), custTZone);
                    while (startDate < endDate)
                    {
                        AppointmentDTO apt = new AppointmentDTO();
                        if (response.Appointments.Where(x => x.Id == appointment.Id).Count() == 0)
                        {
                            apt.Date = startDate.ToString();
                            apt.UserName = appointment.User.FirstName.Substring(0, 1) + appointment.User.LastName.Substring(0, 1);
                            apt.Id = appointment.Id;
                            apt.Minutes = appointment.Minutes;
                            apt.Active = appointment.Active;
                            apt.InActiveReason = appointment.InActiveReason;
                            response.Appointments.Add(apt);
                        }
                        startDate = startDate.AddMinutes(15);
                    }
                }
            }
            return response;
        }

        #endregion

        public CancelAppointmentResponse CancelAppointment(CancelAppointmentRequest request)
        {
            CancelAppointmentResponse response = new CancelAppointmentResponse();
            var appointment = context.Appointments.Include("User").Include("User.Organization").Include("User.Organization.Portals").Where(x => x.Id == request.id).FirstOrDefault();
            if (appointment != null)
            {
                appointment.Active = false;
                appointment.InActiveReason = request.reason;
                appointment.UpdatedBy = request.updatedBy;
                appointment.UpdatedOn = DateTime.UtcNow;
                context.Appointments.Attach(appointment);
                context.Entry(appointment).State = EntityState.Modified;
                context.SaveChanges();
                UpdateAvailabilities(appointment.Date, appointment.Date.AddMinutes(appointment.Minutes), appointment.CoachId, false);
                response.userId = appointment.UserId;
                response.coachId = appointment.CoachId;
                response.orgContactNumber = appointment.User.Organization.ContactNumber;
                response.orgContactEmail = appointment.User.Organization.ContactEmail;
                response.portalId = appointment.User.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().Id;
                response.success = true;
                response.AptTime = appointment.Date;
                response.integrationWith = appointment.User.Organization.IntegrationWith;
            }
            return response;
        }

        public EditApptCommentResponse EditAppointment(EditApptCommentRequest request)
        {
            EditApptCommentResponse response = new EditApptCommentResponse();
            var appointment = context.Appointments.Include("User").Where(x => x.Id == request.id).FirstOrDefault();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            bool isThirty = false;
            if (appointment != null)
            {
                var apptMinutes = appointment.Minutes;
                appointment.Comments = request.comments;
                if (appointment.VideoRequired != request.videoRequired || appointment.Minutes != request.length)
                    response.changeinAppointment = true;
                appointment.VideoRequired = request.videoRequired;
                appointment.UpdatedOn = DateTime.UtcNow;
                appointment.UpdatedBy = request.updatedBy;
                if (appointment.Minutes != request.length)
                {
                    if (appointment.Minutes < request.length)
                    {
                        var startDate = appointment.Date.AddMinutes(15);
                        var avlStartDate = TimeZoneInfo.ConvertTimeFromUtc(startDate, custTZone);
                        var availabilities = GetFreeSlots(false, appointment.Date.AddMinutes(15), appointment.Date.AddMinutes(30), custTZone, null, appointment.CoachId, null, null, false, appointment.User.OrganizationId, true, null, request.stateId).Availabilities.Where(x => x.StartTimeString.Equals(avlStartDate.ToString())).ToList().Count;
                        if (availabilities == 0)
                        {
                            response.success = false;
                            return response;
                        }
                        isThirty = true;
                    }
                    appointment.Minutes = (byte)request.length;
                }
                context.Appointments.Attach(appointment);
                context.Entry(appointment).State = EntityState.Modified;
                context.SaveChanges();
                if (apptMinutes != request.length)
                {
                    if (isThirty)
                        UpdateAvailabilities(appointment.Date, appointment.Date.AddMinutes(30), appointment.CoachId);
                    else
                        UpdateAvailabilities(appointment.Date.AddMinutes(15), appointment.Date.AddMinutes(30), appointment.CoachId, false);
                }
            }
            response.appointment = Utility.mapper.Map<DAL.Appointment, AppointmentDTO>(appointment);
            response.success = true;
            return response;
        }

        public GetAppointmentsResponse ListAppointments(GetAppointmentsRequest request)
        {
            var organizationsList = new int?[] { };
            PortalReader reader = new PortalReader();
            organizationsList = reader.GetFilteredOrganizationsList(request.userId.Value).Organizations.Select(x => x.Id).ToArray();

            DateTime startDate = DateTime.MinValue, endDate = DateTime.MinValue;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            if (request.StartDate.HasValue)
                startDate = TimeZoneInfo.ConvertTimeToUtc(request.StartDate.Value, custTZone);
            if (request.EndDate.HasValue)
                endDate = TimeZoneInfo.ConvertTimeToUtc(request.EndDate.Value, custTZone);

            GetAppointmentsResponse response = new GetAppointmentsResponse();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.Appointments.Where(x => ((x.CoachId == request.coachId || request.coachId == null) && (x.Date >= startDate || startDate == DateTime.MinValue)
                     && (x.Date <= endDate || endDate == DateTime.MinValue) && (x.Active == true || (x.Active == false && x.InActiveReason == 5))
                     && (organizationsList.Count() != 0 && organizationsList.Contains(x.User.OrganizationId))))
                     .OrderBy(x => x.Date).Count();
            }
            response.totalRecords = totalRecords;
            var appointments = context.Appointments.Include("User").Include("User.Organization").Include("User1").Include("AppointmentType")
                .Where(x => ((x.CoachId == request.coachId || request.coachId == null) && (x.Date >= startDate || startDate == DateTime.MinValue)
                    && (x.Date <= endDate || endDate == DateTime.MinValue) && (x.Active == true || (x.Active == false && x.InActiveReason == 5))
                    && (organizationsList.Count() != 0 && organizationsList.Contains(x.User.OrganizationId))))
                    .OrderBy(x => x.Date).Skip(request.page * request.pageSize).Take(request.pageSize).ToList();
            if (appointments.Count > 0)
            {
                response.Appointments = new List<AppointmentDTO>();
                foreach (var appointment in appointments)
                {
                    AppointmentDTO apt = new AppointmentDTO();
                    string PhoneNumber = "";
                    var newDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(appointment.Date.ToString()), custTZone);
                    apt.Date = newDate.ToShortDateString();
                    apt.StartTime = newDate.ToShortTimeString();
                    apt.EndTime = newDate.AddMinutes(appointment.Minutes).ToShortTimeString();
                    apt.Minutes = appointment.Minutes;
                    apt.Id = appointment.Id;
                    apt.UserId = appointment.User.Id;
                    apt.UserName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(appointment.User.FirstName.ToLower() + " " + appointment.User.LastName.ToLower());
                    apt.Comments = appointment.Comments;
                    apt.ApptType = appointment.AppointmentType.Type;
                    apt.CoachName = appointment.User1.FirstName + " " + appointment.User1.LastName.Substring(0, 1);
                    apt.Company = appointment.User.Organization.Name.IndexOf(" ") >= 0 ? appointment.User.Organization.Name.Substring(0, appointment.User.Organization.Name.IndexOf(" "))
                                  : appointment.User.Organization.Name;
                    apt.Language = string.IsNullOrEmpty(appointment.User.LanguagePreference) ? "" : appointment.User.LanguagePreference;
                    if (!string.IsNullOrEmpty(appointment.User.CellNumber))
                        PhoneNumber = PhoneNumber + "C: " + appointment.User.CellNumber + (appointment.User.ContactMode == 3 ? "*" : "") + "; ";
                    if (!string.IsNullOrEmpty(appointment.User.HomeNumber))
                        PhoneNumber = PhoneNumber + "H: " + appointment.User.HomeNumber + (appointment.User.ContactMode == 1 ? "*" : "") + "; ";
                    if (!string.IsNullOrEmpty(appointment.User.WorkNumber))
                        PhoneNumber = PhoneNumber + "W: " + appointment.User.WorkNumber + (appointment.User.ContactMode == 2 ? "*" : "") + "; ";
                    apt.Active = appointment.Active;
                    apt.InActiveReason = appointment.InActiveReason;
                    apt.PhoneNumber = PhoneNumber;
                    apt.CoachId = appointment.CoachId;
                    apt.VideoRequired = appointment.VideoRequired;
                    apt.TextResponse = appointment.TextResponse;
                    response.Appointments.Add(apt);
                }
            }
            return response;
        }

        public bool RevertNoShow(int id)
        {
            var appointment = context.Appointments.Where(x => x.Id == id).FirstOrDefault();
            if (appointment != null)
            {
                appointment.Active = true;
                appointment.InActiveReason = null;
                context.Appointments.Attach(appointment);
                context.Entry(appointment).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public AppointmentMoveResponse MoveAppointments(MoveAppointmentsRequest request)
        {
            AppointmentMoveResponse response = new AppointmentMoveResponse();
            List<AppointmentDTO> AssignedList = new List<AppointmentDTO>();
            List<AppointmentDTO> NotAssignedList = new List<AppointmentDTO>();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            string languages = "";
            var coach = context.Users.Include("AdminProperty").Include("UserRoles").Include("Languages").Where(x => x.Id == request.CoachId).FirstOrDefault();
            foreach (var appt in request.Appointments)
            {
                bool thirthyMinutes = appt.Minutes == 30;
                var apptUser = context.Users.Include("Organization").Where(x => x.Id == appt.UserId).FirstOrDefault();
                if (!String.IsNullOrEmpty(apptUser.LanguagePreference) && apptUser.LanguagePreference != "en-us")
                {
                    CommonReader reader = new CommonReader();
                    var language = reader.GetLanguages(apptUser.LanguagePreference);
                    if (language.Languages != null)
                        languages = language.Languages.FirstOrDefault().Id.ToString();
                }
                var startTime = Convert.ToDateTime(appt.Date + ' ' + appt.StartTime);
                var apptDate = TimeZoneInfo.ConvertTimeToUtc(startTime, custTZone);
                List<string> coachIds = request.ToCoachIds.Split('-').ToList<string>();
                int organizationId;
                if (apptUser.Organization.OwnCoach)
                    organizationId = apptUser.OrganizationId;
                else
                    organizationId = 2;
                int? stateId = null;
                if (apptUser.Organization.IntegrationWith == (int)IntegrationPartner.LMC)
                    stateId = apptUser.State;
                var avail = GetFreeSlots(thirthyMinutes, apptDate, apptDate.AddMinutes(appt.Minutes), custTZone, null, null, null, null, true, organizationId, true, languages, stateId).Availabilities.
                    Where(x => x.CoachId != appt.CoachId && (((string.IsNullOrEmpty(request.ToCoachIds) && coach.UserRoles.Where(R => R.Code == x.UserRoleCode).Any()) || coachIds.Contains(x.CoachId.ToString()))) && x.StartTimeString.Equals(startTime.ToString())).ToList();
                if (avail != null && avail.Count() > 0 && apptDate > DateTime.UtcNow)
                {
                    var appointment = context.Appointments.Where(x => x.Id == appt.Id).FirstOrDefault();
                    if (appointment != null)
                    {
                        appointment.CoachId = avail[0].CoachId;
                        context.Appointments.Attach(appointment);
                        context.Entry(appointment).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    UpdateAvailabilities(appointment.Date, appointment.Date.AddMinutes(appointment.Minutes), appt.CoachId, false);
                    appt.CoachId = avail[0].CoachId;
                    AccountReader acctReader = new AccountReader();
                    var user = acctReader.GetUserById(avail[0].CoachId);
                    appt.CoachName = user.FirstName + " " + user.LastName;
                    AssignedList.Add(appt);
                    UpdateAvailabilities(appointment.Date, appointment.Date.AddMinutes(appointment.Minutes), appt.CoachId);
                }
                else
                {
                    AppointmentDTO Appointment = new AppointmentDTO();
                    Appointment = appt;
                    NotAssignedList.Add(Appointment);
                }
            }
            response.AssignedList = AssignedList;
            response.NotAssignedList = NotAssignedList;
            return response;
        }

        public bool AddAppointmentFeedback(AddAppointmentFeedbackRequest request)
        {
            DAL.AppointmentFeedback appointmentFeedback = new DAL.AppointmentFeedback();
            appointmentFeedback.AppointmentId = request.id;
            appointmentFeedback.Rating = request.rating;
            appointmentFeedback.Comments = request.comments;
            context.AppointmentFeedbacks.Add(appointmentFeedback);
            context.SaveChanges();
            return true;
        }

        public int? CancelFutureAppointments(string UniqueId, int OrganizationId)
        {
            var User = context.Users.Where(x => x.UniqueId == UniqueId && x.OrganizationId == OrganizationId).FirstOrDefault();
            if (User != null)
            {
                var appts = context.Appointments.Where(x => x.UserId == User.Id && x.Active == true && x.Date > DateTime.UtcNow).Select(x => x.Id).ToList();
                CancelAppointmentRequest request = new CancelAppointmentRequest();
                foreach (int apptId in appts)
                {
                    request.id = apptId;
                    request.reason = 4;
                    request.comments = "Membership terminated";
                    CancelAppointment(request);
                }
                return User.Id;
            }
            else
                return null;
        }

    }
}