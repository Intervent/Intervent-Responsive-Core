$(document).ready(function () {
    $("#AppointmentDate").datepicker();
});

$('[name="CancelApp"]').on('click', function (event) {
    if (event.target.value == "CO") {
        $("#CancelCommentsDiv").show();
    }
    else {
        $("#CancelCommentsDiv").hide();
    }
});

$("#cancelButton").on('click', function () {
    var AppStatus = $('[name="CancelApp"]:checked').val();
    var Comments = $("#CancelComments").val();
    $.ajax({
        type: "POST",
        url: "DeleteAppointment",
        data: JSON.stringify({ AppRef: $("#AppRef").val(), AppointmentStatus: AppStatus, Comments: Comments, AppointmentDeleted: true }),
        success: function () {
            window.location.href = "Appointments?deleted=true";
        },
        dataType: "json",
        contentType: "application/json"
    });
}).fail(function (jqXHR, textStatus, errorThrown) {
        RedirectToErrorPage(jqXHR.status);
    });

$('form').validate({
    showErrors: function (errorMap, errorList) {
        var i, el, length = errorList.length;
        if (length > 0) {
            $("#error").text("Please fill the required fields");
            $("#error").addClass('show');
        }
        else {
            $("#error").addClass('hide');
        }
    }
});