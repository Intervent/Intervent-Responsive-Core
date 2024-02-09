function validateSignIn(data, username, password, organizationEmail, loginURL, streamURL) {
    if (data.EmailValidationRequired) {
        $('#register-modal').foundation('open');
    }
    else {
        var loginmodel = {};
        var logindata = {
            'UserName': username,
            'Password': password,
            'OrganizationEmail': organizationEmail
        };
        loginmodel = (logindata);
        $.ajax({
            url: loginURL,
            type: 'POST',
            dataType: "json",
            data: loginmodel,
            success: function (data) {
                if (data == "success") {
                    window.location.href = streamURL;
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
                RedirectToErrorPage(jqXHR.status);
            });
    }
}