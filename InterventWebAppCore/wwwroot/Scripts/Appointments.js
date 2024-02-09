function DeleteAppointment(appId) {
    var $dialogContent = $("#event_edit_container");
    $dialogContent.dialog({
        modal: true,
        title: title,
        close: function () {
            $dialogContent.dialog("destroy");
            $dialogContent.hide();
        },
        buttons: [{
            text: save,
            "id": "btnsave",
            click: function () {
                var comments = $("#Comments").val();
                DeleteApt(appId);
                $dialogContent.dialog("close");
            },
        },
        {
            text: cancel,
            "id": "btncancel",
            click: function () {
                $dialogContent.dialog("close");
            }
        }]
    }).show();
    $("#CancelCommentsdiv").hide();
}

function DeleteApt(appId) {
    var comments = $("#CancelComments").val();
    var reason = $("#InactiveReason").val();

    $.ajax({
        type: "POST",
        async: true,
        dataType: "json",
        url: "DeleteAppointment",
        cache: true,
        data: {
            AppId: appId, Reason: reason, Comments: comments
        },
        success: function (data) {
            if (data == "success")
                window.location.href = "Confirmation?scheduled=true";
            else {
                alert("Failed to save the changes. Either there is no availability or someone has taken this slot. " +
                    + "Please refresh the browser to see other available slots.");
            }
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
            RedirectToErrorPage(jqXHR.status);
        });
}

$("#InactiveReason").on('change', function () {
    if ($("#InactiveReason").val() == "5") {
        $("#CancelCommentsdiv").show();
    }
    else {
        $("#CancelCommentsdiv").hide();
    }
});