var weekDays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']
var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
var adDay, adMonth, adYear, dDay, dMonth, dYear, startDay;

$(document).ready(function () {
    CallDayView();
    CallMonthView();
})

function movetoDay(day, month, year) {
    toggleViews();
    CallDayView(day, month, year)
}

function CallDayView(day, month, year) {
    var firstDay;
    if (day != undefined)
        firstDay = new Date(year, month, day);
    else
        firstDay = new Date();
    startDay = firstDay;
    return printDay();
}

function movetoMonth() {
    toggleViews();
    CallMonthView();
}


function CallMonthView() {
    // Initializing global variables
    adDay = new Date().getDate();
    adMonth = new Date().getMonth();
    adYear = new Date().getFullYear();
    dDay = adDay;
    dMonth = adMonth;
    dYear = adYear;
    return printCalendar();
}

function printCalendar() {
    $("#calendar-month").html("");
    var dWeekDayOfMonthStart = new Date(dYear, dMonth, 1).getDay();
    var dLastDayOfMonth = new Date(dYear, dMonth + 1, 0).getDate();
    var dLastDayOfPreviousMonth = dLastDayOfMonth - dWeekDayOfMonthStart;
    $('.month-picker-label').html(months[dMonth] + ' ' + dYear);
    for (var i = 0; i < weekDays.length; i++) {
        $("#calendar-month").append("<li class='cell day-label'>" + weekDays[i] + "</li>");
    }
    var apptCount = {};
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "../Scheduler/GetAppointmentsCount",
        async: false,
        data: { year: dYear, month: (dMonth + 1), coachId: coachId },
        success: function (data) {
            if (data != false || data != null) {
                if (data.Records != null) {
                    var eventList = data.Records;
                    for (var i = 0; i < eventList.length; i++) {
                        var parsedDate = new Date(eventList[i].DateString)
                        var day = parsedDate.getDate();
                        var month = parsedDate.getMonth() + 1;
                        apptCount[day + '-' + month] = eventList[i].NoOfRecords;
                    }
                }
            }
        }
    });
    var day = 1;
    var dayOfNextMonth = 1;
    var html = "";
    for (var i = 0; i < 42; i++) {
        var appts = "";
        var phoneClass = "";
        if (apptCount[(day) + '-' + (dMonth + 1)] != undefined) {
            appts = apptCount[(day) + '-' + (dMonth + 1)];
            phoneClass = 'fa fa-phone';
        }
        if (i < dWeekDayOfMonthStart) {
            html = html + "<li class='cell not-available'><a href><span class='date'></span></a></li>";
        }
        else if (day <= dLastDayOfMonth) {
            if (day == dDay && adMonth == dMonth && adYear == dYear) {
                if (appts > 0) {
                    html = html + "<li class='cell today'><a onClick='movetoDay(" + day + "," + dMonth + "," + dYear + ");'><span class='date'>" + day++ + "</span><span class='appointment-count'><i class='" + phoneClass + "'></i>" + appts + "</span></a></li>";
                } else {
                    html = html + "<li class='cell today'><a onClick='movetoDay(" + day + "," + dMonth + "," + dYear + ");'><span class='date'>" + day++ + "</span></a></li>";
                }
            }
            else {
                if (appts > 0) {
                    html = html + "<li class='cell'><a onClick='movetoDay(" + day + "," + dMonth + "," + dYear + ");'><span class='date'>" + day++ + " </span><span class='appointment-count'><i class='" + phoneClass + "'></i>" + appts + "</span></a></li>";
                } else {
                    html = html + "<li class='cell'><a onClick='movetoDay(" + day + "," + dMonth + "," + dYear + ");'><span class='date'>" + day++ + " </span></a></li>";
                }
            }
        }
        else {
            html = html + "<li class='cell not-available'><a href><span class='date'></span></a></li>";
        }
    }
    $("#calendar-month").append(html)
}

function printDay() {
    $("#calendar-day").html("");
    $('.day-picker-label').html(startDay.toDateString());
    var endDate = new Date(startDay);
    endDate.setDate(startDay.getDate() + 1);
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "../Scheduler/GetAppointments",
        async: false,
        data: { startDate: startDay.toDateString(), endDate: endDate.toDateString(), coachId: coachId },
        success: function (data) {
            if (data != false || data != null) {
                if (data.Records != null) {
                    $("#noApptDay").hide();
                    var html = "";
                    for (var i = data.Records.length - 1; i >= 0; i--) {
                        var noShow = "", revertNoShow = "", textResponse = "";
                        if (data.Records[i].InActiveReason == 5) {
                            noShow = " (NS)";
                            revertNoShow = "<a class='button secondary tiny' onclick='RevertNoShow(" + data.Records[i].Id + ")'>Revert</a>";
                        }
                        if (data.Records[i].TextResponse)
                            textResponse = " <span id='TextResponse_" + data.Records[i].Id + "' data-tooltip aria-haspopup='true' title=''><i class='fa fa-info-circle has-tip tip-left' aria-hidden='true'></i></span>";
                        var link = "../Participant/ParticipantProfile/" + data.Records[i].UserId;
                        html += "<li class='cell'>";
                        html += "<div class='grid-x collapse'>";
                        html += "<div class='cell small-12 medium-6'>";
                        html += "<a href='" + link + "'><img src='../Images/avatar-male.png' />" + data.Records[i].UserName + "</a>";
                        html += "</div>";
                        html += "<div class='cell small-7 medium-4'>";
                        html += "<a href='" + link + "'><span class='appointment-time'>" + data.Records[i].StartTime + "-" + data.Records[i].EndTime + noShow + textResponse + "</span></a>";
                        html += "</div>";
                        html += "<div class='cell small-5 medium-2 text-right'>";
                        var dateTimeNow = new Date();
                        var utcNow = new Date(dateTimeNow.getTime() + dateTimeNow.getTimezoneOffset() * 60000)
                        var apptDate = new Date(data.Records[i].UTCDate);
                        apptDate.setDate(apptDate.getDate() + 1);
                        if (data.Records[i].Active == true && apptDate > utcNow)
                            html += "<a data-open='cancel-appointment' class='button secondary tiny' data-modal-path='" + cancelUrl + "?appid=" + data.Records[i].Id + "'>Cancel</a>";
                        else if (revertNoShow != "")
                            html += revertNoShow;

                        html += "</div>";
                        html += "</div>";
                        html += "</li>";
                    }
                    $("#calendar-day").append(html);
                    for (var i = data.Records.length - 1; i >= 0; i--) {
                        if (data.Records[i].TextResponse)
                            document.getElementById("TextResponse_" + data.Records[i].Id + "").title = "Text Response:" + data.Records[i].TextResponse;
                    }
                }
                else {
                    $("#noApptDay").show();
                    if (startDay.toDateString() == new Date().toDateString()) {
                        $("#noApptDay").html("You have no appointments today.");
                    }
                    else {
                        $("#noApptDay").html("You have no appointments on this day.");
                    }
                }
            }
            $("[data-open]").on('click', function (e) {
                showmodal($(this), e)
            });
        }
    });
}


$("#nextMonth").on('click', function () {
    if (dMonth < 11) {
        dMonth++;
    } else {
        dMonth = 0;
        dYear++;
    }
    printCalendar();
});

$("#prevMonth").on('click', function () {
    if (dMonth > 0) {
        dMonth--;
    } else {
        dMonth = 11;
        dYear--;
    }
    printCalendar();
});

$("#nextDay").on('click', function () {
    startDay.setDate(startDay.getDate() + 1);
    printDay();
});

$("#prevDay").on('click', function () {
    startDay.setDate(startDay.getDate() - 1);
    printDay();
});


function toggleViews() {
    $('#month-view').toggleClass('hide');
    $('#day-view').toggleClass('hide');
}