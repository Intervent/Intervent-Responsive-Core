var calendar;
var timeList = new Array();

function getApt(coachId) {
    $.ajax({
        type: "POST",
        async: true,
        dataType: "json",
        url: coachUrl,
        cache: true,
        data: { coachId: coachId, userTimeZone: userTimeZone },
        success: function (data) {
            avilNextDay = false;
            if (data != false || data != null) {
                if (data.Appointments == null && data.Availabilities == null) {
                    $('#calendar').hide();
                    return;
                }
                timeList = new Array();
                var eventSource = convertData(data);
                try {
                    if (calendar != null) {
                        calendar.destroy();
                    }

                } catch (e) { }

                var calendarEl = document.getElementById('calendar');
                var calendar = new FullCalendar.Calendar(calendarEl, {
                    initialView: 'timeGridWeek',
                    headerToolbar: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'dayGridMonth,timeGridWeek,timeGridDay'
                    },
                    slotDuration: '00:15:00', // 15 minutes
                    allDaySlot: false,
                    selectable: false,
                    selectMirror: true,
                    selectLongPressDelay: 0,
                    longPressDelay: 0,
                    height: 600,
                    eventClick: function (eventClickInfo) {
                        if (bookUrl != "")
                            BookAppointment(eventClickInfo.event, eventClickInfo.el, coachId)
                    },
                    events: eventSource,
                    eventBorderColor: 'transparent',
                    eventBackgroundColor: '#2DCFED',
                    eventTextColor: '#ffffff',
                    textColor: '#ffffff',
                    eventDisplay: 'block',
                    views: {
                        dayGridMonth: {
                            // options apply to dayGridMonth, dayGridWeek, and dayGridDay views
                            eventTimeFormat: {
                                hour: '2-digit',
                                minute: '2-digit',
                                meridiem: 'short'
                            }
                        }
                    }
                });
                calendar.render();
            }

        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
            RedirectToErrorPage(jqXHR.status);
        });
}

function CoachChange() {
    var coachId = $('#coach').val();
    if (coachId != null) {
        $('#calendar').show();
        $('#coach').prop('disabled', true);
        getApt(coachId);
        $('#coach').prop('disabled', false);
    }
    else {
        $('#calendar').hide();
    }
}

function convertData(data) {
    var eventList = new Array();
    if (showAppointment == true) {
        eventList = convertAppt(data, eventList);
    }
    if (data.Availabilities != null) {
        for (var i = 0; i < data.Availabilities.length; i++) {
            var currAvail = data.Availabilities[i];
            start = new moment(currAvail.StartTimeString);
            end = new moment(currAvail.EndTimeString)
            var event = {
                title: "Avail",
                start: start.toISOString(),
                end: end.toISOString(),
                allDay: false,
                editable: false
            }
            eventList.push(event);
        }
    }
    return eventList;
}

function convertAppt(data, eventList) {
    if (data.Appointments != null) {
        for (var i = 0; i < data.Appointments.length; i++) {
            var aDate = new Date(data.Appointments[i].Date);
            if (data.Appointments[i].InActiveReason == 5) {
                noShow = " (NS)";
            }
            else {
                noShow = "";
            }
            var event = {
                id: data.Appointments[i].Id,
                title: data.Appointments[i].UserName + noShow,
                start: aDate,
                end: new Date(aDate.getFullYear(), aDate.getMonth(), aDate.getDate(), aDate.getHours(), aDate.getMinutes() + data.Appointments[i].Minutes),
                allDay: false,
                editable: false,
                color: '#ff674f'
            }
            eventList.push(event);

        }
    }
    return eventList;
}

function BookAppointment(events, element, coachId) {
    $.ajax({
        type: 'GET',
        url: bookUrl,
        data: { coachId: coachId, minutes: 15, startTime: moment(events.start).format('MM/DD/YYYY hh:mm A'), timeZone: timeZone },
        success: function (data) {
            $("#book-appointment").html(data);
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
            RedirectToErrorPage(jqXHR.status);
        });
    $('#book-appointment').foundation('open');
    $('#add-appointment').foundation('close');
}