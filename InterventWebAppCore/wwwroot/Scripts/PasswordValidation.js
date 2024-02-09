
$(document).ready(function () {
    $('#inpassword').on('keypress', function (e) {
        var code = e.keyCode || e.which;
        codeStr = String.fromCharCode(code);
        if (/^[a-z]$/.test(codeStr)) {
            $("li.lower-char").addClass("is-active");
        }
        else if (/^[A-Z]$/.test(codeStr)) {
            $("li.upper-char").addClass("is-active");
        }
        else if (/^[0-9]$/.test(codeStr)) {
            $("li.number-char").addClass("is-active");
        }
    });

    $('#inpassword').on("keyup", function (e) {
        var str = $('#inpassword').val();
        if (str.replace(/\s+/g, '').length >= 8) {
            $("li.min-length").addClass("is-active");
        }
        else {
            $("li.min-length").removeClass("is-active");
        }

        if (!/[a-z]/.test(str)) {
            $("li.lower-char").removeClass("is-active");
        }
        if (!/[A-Z]/.test(str)) {
            $("li.upper-char").removeClass("is-active");
        }
        if (!/[0-9]/.test(str)) {
            $("li.number-char").removeClass("is-active");
        }
    });

    $('#NewPassword').unbind('keypress').bind('keypress', function (e) {
        var code = e.keyCode || e.which;
        codeStr1 = String.fromCharCode(code);
        if (/^[a-z]$/.test(codeStr1)) {
            $("li.lower-char").addClass("is-active");
        }
        else if (/^[A-Z]$/.test(codeStr1)) {
            $("li.upper-char").addClass("is-active");
        }
        else if (/^[0-9]$/.test(codeStr1)) {
            $("li.number-char").addClass("is-active");
        }
    });

    $('#NewPassword').on("keyup", function (e) {
        var str = $('#NewPassword').val();
        if (str.replace(/\s+/g, '').length >= 8) {
            $("li.min-length").addClass("is-active");
        }
        else {
            $("li.min-length").removeClass("is-active");
        }

        if (!/[a-z]/.test(str)) {
            $("li.lower-char").removeClass("is-active");
        }
        if (!/[A-Z]/.test(str)) {
            $("li.upper-char").removeClass("is-active");
        }
        if (!/[0-9]/.test(str)) {
            $("li.number-char").removeClass("is-active");
        }
    });
});