var calendarEl;
var calendar;
function getApt(coachId) {
    $("#loader-wrapper").fadeIn();
    $.ajax({
        type: "POST",
        async: true,
        dataType: "json",
        url: "GetCoachAvailability",
        cache: true,
        data: { coachId: coachId },
        success: function (data) {
            if (data != false || data != null) {
                eventSource = convertAvail(data);
                try {
                    if (calendar != null)
                        calendar.destroy();
                } catch (e) { }

                calendarEl = document.getElementById('calendar');
                calendar = new FullCalendar.Calendar(calendarEl, {
                    initialView: 'timeGridWeek',
                    headerToolbar: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'dayGridMonth,timeGridWeek,timeGridDay'
                    },
                    slotDuration: '00:15:00', // 15 minutes
                    allDaySlot: false,
                    selectable: true,
                    selectMirror: true,
                    unselectAuto: false,
                    selectLongPressDelay: 0,
                    longPressDelay: 0,
                    height: 600,
                    initialDate: new Date(year, month, day),
                    titleFormat: { year: 'numeric', month: 'short', day: 'numeric' },
                    select: function (info) {
                        isMonth = false;
                        if (moment(info.start).isUTC()) {
                            isMonth = true;                           
                        }
                        newStart = info.start;
                        newEnd = info.end;
                        CreateModal(newStart, newEnd, coachId, isMonth);
                    },
                    eventClick: function (events, element) {
                        UpdateModal(events.event, element, coachId);
                    },
                    events: eventSource,
                    eventDisplay: 'block',
                    eventBorderColor: 'transparent',
                    eventBackgroundColor: '#2DCFED',
                    eventTextColor: '#fff',
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
    $("#loader-wrapper").fadeOut();
}

function convertAvail(data) {
    var eventList = new Array();
    if (data.availabilityList != null) {
        for (var i = 0; i < data.availabilityList.length; i++) {
            start = new moment(data.availabilityList[i].StartTimeString);
            end = new moment(data.availabilityList[i].EndTimeString)
            var event = {
                id: data.availabilityList[i].RefId,
                title: "Avail",
                start: start.toISOString(),
                end: end.toISOString(),
                allDay: false,
                textColor: 'black'

            }
            eventList.push(event);
        }
        return eventList;
    }
}

function CreateModal(start, end, coachId, isMonth) {
    $("#coachId").val(coachId);
    var currentDate = new moment();
    currentDate.set('hour', 0).set('minute', 0).set('second', 0);   
    if (start < currentDate) {
        alert('Please select a future date');
        return;
    }
    currentDate.date(currentDate.date() + 1);
    $("#H1").html("Add Coach Availabiliy");
    $("#avail-error").addClass("hide");
    $("#availAppointment-error").addClass("hide");
    $('#event_edit_container').foundation('open');
    $("#StartTime").prop("disabled", false);
    $("#EndTime").prop("disabled", false);
    $("#saveAvail").show();
    $("#deleteAvail").hide();
    var $dialogContent = $("#event_edit_container");
    resetForm($dialogContent);
   $("#startDate").fdatepicker();
   $("#EndDate").fdatepicker({
       format: $("#userDateFormat").val().toLowerCase()
   });
   $("#EndDate").fdatepicker({
       format: $("#userDateFormat").val().toLowerCase()
    }).on('show', function () {
        $(".datepicker").css("position", "fixed");
    });
   $("#EndDate").fdatepicker({
       format: $("#userDateFormat").val().toLowerCase()
    }).on('hide', function () {
        $(".datepicker").css("position", "absolute");
    });
    $("#AllFutureDiv").hide();
    $("#multiSlot").show();
    $("#deleteButton").hide();
    if (isMonth)
    {
        startDate = start.format($("#userDateFormat").val());
        $('#SelectedDateText').val(startDate);
        sTime = startDate + " 12:00:00 AM";
        sTime = new Date(sTime);
        
        eTime = startDate + " 12:15:00 AM";
        eTime = new Date(eTime);
        $('#StartTime').html(getTimeDropDown(sTime));
        $('#EndTime').html(getTimeDropDown(eTime));
    }
    else
    {
        $('#SelectedDateText').val(moment(start).format($("#userDateFormat").val()));
        $('#StartTime').html(getTimeDropDown(new Date(start)));
        $('#EndTime').html(getTimeDropDown(new Date(end)));
    }
    $("#coachId").val(coachId);
    $("#eventId").val("");
    calendar.unselect()
}

function CreateSlot(fromDate, toDate, startDateTime, endDateTime, coachId) {
    var Days = getRepeatDays();
    $.ajax({
        type: "POST",
        async: true,
        dataType: "json",
        url: "SetCoachAvailability",
        cache: true,
        data: { startDateTime: moment(startDateTime).format('DD-MMM-YYYY HH:mm'), endDateTime: moment(endDateTime).format('DD-MMM-YYYY HH:mm'), Days: Days, fromDate: fromDate, toDate: toDate, coachId: coachId },
        success: function (data) {
            if (data.status != null) {
                if (data.status[0] == "success") {
                    window.location.href = "SetCoachAvailability?coachId=" + coachId + "&startDate=" + moment(startDateTime).format('DD-MMM-YYYY HH:mm');
                }
                else {
                    var message = "";
                    for (var i = 0; i < data.status.length; i++) {
                        message = message + "<br>" + data.status[i]
                    }
                    message = message + "<br><br>";
                    $("#availList").html(message);
                    $("#avail-error").removeClass("hide");
                }
            }
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
            RedirectToErrorPage(jqXHR.status);
        });
}

function UpdateModal(events, element, coachId) {
    var $dialogContent = $("#event_edit_container");
    resetForm($dialogContent);
    $("#avail-error").addClass("hide");
    $("#availAppointment-error").addClass("hide");
    $("#coachId").val(coachId);
    $("#H1").html("Delete Coach Availabiliy");
    $("#multiSlot").hide();
    $("#startDate").fdatepicker();
    $("#EndDate").fdatepicker({
        format: $("#userDateFormat").val().toLowerCase()
    });
    $("#EndDate").fdatepicker({
        format: $("#userDateFormat").val().toLowerCase()
    }).on('show', function () {
        $(".datepicker").css("position", "fixed");
    });
    $('input[name=AllFutureCheckBox]').prop('checked', false);
    $("#eventId").val(events.id);
    $.ajax({
        type: "POST",
        async: true,
        dataType: "json",
        url: "GetCoachAvailabilityDetails",
        cache: true,
        data: { refId: events.id, startDate: "" + events.start.toLocaleDateString() + " " + events.start.toLocaleTimeString()+"" },
        success: function (data) {
            if (data != false || data != null) {
                var uStartDate = new Date(data.StartDate);
                var uEndDate = new Date(uStartDate.getFullYear(), uStartDate.getMonth(), uStartDate.getDate(), new Date(data.EndDate).getHours(), new Date(data.EndDate).getMinutes());
                if (data.FromDate != null) {
                    var uFromDate = new Date(data.FromDate);
                }
                if (data.ToDate != null) {
                    var uToDate = new Date(data.ToDate);
                }
                $('#SelectedDateText').val(moment(uStartDate).format($("#userDateFormat").val()));
                $('#StartTime').html(getTimeDropDown(uStartDate));
                $("#StartTime option:contains(" + moment(uStartDate, "HH:mm TT") + ")").prop('selected', 'selected');
                $('#EndTime').html(getTimeDropDown(uEndDate));
                $("#EndTime option:contains(" + moment(uEndDate, "HH:mm TT") + ")").prop('selected', 'selected');
                if (uFromDate == null) {
                    $("#startDate").val("");
                    $("#EndDate").val("");
                    $("#AllFutureDiv").hide();
                }
                else {
                    $("#startDate").val(moment(uFromDate).format($("#userDateFormat").val()));
                    $("#EndDate").val(moment(uToDate).format($("#userDateFormat").val()));
                    $("#AllFutureDiv").show();
                }
                if (data.Days != null) {
                    for (var i = 0; i < data.Days.length; i++) {
                        $('li[data-value="' + data.Days[i].Id + '"]').trigger("click");
                    }
                }
                $('#event_edit_container').foundation('open');
                $("#StartTime").prop("disabled", true);
                $("#EndTime").prop("disabled", true);
                $("#saveAvail").hide();
                $("#deleteAvail").show();
            }
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
            RedirectToErrorPage(jqXHR.status);
        });
}

$('#saveAvail').on('click', function () {
    var fromDate = toSystemDateFormat($("#startDate").val());
    var toDate = toSystemDateFormat($("#EndDate").val());
    var startDateTime = $("#StartTime option:selected").val();
    var endDateTime = $("#EndTime option:selected").val();
    var repeatDays = $("#repeat-days").val();
    var coachId = $("#coachId").val();
    var eventId = $("#eventId").val();
    if (repeatDays != null) {
        if (toDate == "") {
            alert("Please select End Date");
            return;
        }
        if (new Date(toDate) <= new Date(fromDate)) {
            alert("End Date should be greater than start date");
            return;
        }
    }
    if (eventId == undefined || eventId == null || eventId == "")
        CreateSlot(fromDate, toDate, startDateTime, endDateTime, coachId)

});

$('#deleteAvail').on('click', function () {
    var allFuture = false;
    if ($('input[name="AllFutureCheckBox"]:checked').length > 0) {
        allFuture = true;
        var toDate = toSystemDateFormat($("#EndDate").val());
    }
    else {
        var toDate = "";
    }
    var startTime = $("#StartTime option:selected").val();
    var endTime = $("#EndTime option:selected").val();
    var eventId = $("#eventId").val();
    DeleteSlot(startTime, endTime, eventId, allFuture, toDate);
});

function DeleteSlot(startTime, endTime, refId, allFuture, toDate) {
    var Days = getRepeatDays();
    $.ajax({
        type: "POST",
        async: true,
        dataType: "json",
        url: "DeleteCoachAvailability",
        cache: true,
        data: { refId: refId, startTime: moment(startTime).format('DD-MMM-YYYY HH:mm'), endTime: moment(endTime).format('DD-MMM-YYYY HH:mm'), toDate: toDate, allFuture: allFuture, days: Days },
        success: function (data) {
            if (data != null && data.Status != false) {
                window.location.href = "SetCoachAvailability?coachId=" + $("#coachId").val() + "&startDate=" + moment(startTime).format('DD-MMM-YYYY HH:mm');
            }
            else {
                var message = "";
                for (var i = 0; i < data.bookedAvailabilities.length; i++) {
                    message = message + "<br>" + data.bookedAvailabilities[i];
                }
                message = message + "<br><br>";
                $("#availAppointmenList").html(message);
                $("#availAppointment-error").removeClass("hide");
            }
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
            RedirectToErrorPage(jqXHR.status);
        });
}

function getTimeDropDown(date) {
    var nextDate = new Date(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes());
    var ohtml = "";
    while (nextDate.getDate() == date.getDate()) {
        ohtml += "<option value='" + nextDate + "'>" + moment(nextDate).format("h:mm A") + "</option>";
        nextDate.setMinutes(nextDate.getMinutes() + 15);
    }
    return ohtml;

}

function getRepeatDays() {
    var days = "";
    var selectObject = $('[name="repeat-days"]').find("option:selected");
    for (var i = 0; i < selectObject.length; i++) {
        if (days == "") {
            days = selectObject[i].value
        }
        else {
            days = days + "-" + selectObject[i].value;
        }
    }
    return days;
}

function CoachChange() {
    var coachId = $('#coach').val();
    if (coachId != null) {
        $('#calendar').show();
        getApt(coachId);
    }
    else {
        $('#calendar').hide();
    }
}

$(document).ready(function () {
    if ($('#coach').val() == null) {
        $('#calendar').hide();
    }
});

function resetForm($dialogContent) {
    $dialogContent.find("text").val("");
    $('#StartTime').val('');
    $('#EndTime').val('');
    $("#startDate").val('');
    $("#EndDate").val('');
    $("#EndDate").attr("disabled", "disabled");
    $("#startDate").attr("disabled", "disabled");
    $('[name="repeat-days"]').val('');
}

$('[name="repeat-days"]').on('change', function () {
    var selectedOptions = $('[name="repeat-days"]').find("option:selected").length
    if (selectedOptions >= 1) {
        $("#EndDate").removeAttr("disabled");
        $("#startDate").val($('#SelectedDateText').val());
    }
    else {
        $("#EndDate").attr("disabled", "disabled");
        $("#startDate").val("");
        $("#EndDate").val("");
    }
});

$("#StartTime").on('change', function (e) {
    $("#EndTime").empty();
    var options =
    $("#StartTime option").filter(function (e) {
        return $(this).attr("value") > $("#StartTime option:selected").val();
    }).clone();
    $("#EndTime").append(options);
});

function AllFutureCheckBoxClicked() {
    if ($('input[name="AllFutureCheckBox"]:checked').length > 0) {
        $("#EndDate").removeAttr("disabled");
        $("#multiSlot").show();
    }
    else {
        $("#EndDate").attr("disabled", "disabled");
        $("#multiSlot").hide();
    }
}
