function DisplayError(statusCode) {
    if (statusCode == 500)
        alert("We're sorry! The server encountered an internal error and was unable to complete your request. Please try again later.");
    else if (statusCode == 404)
        alert("The link you followed may be broken, or the page may have been removed.");
    else if (statusCode == 401)
        alert("You are not authorized to view this page.");
}

function RedirectToErrorPage(statusCode) {
    if (statusCode == 500)
        window.location.href = $('#errorUrl').val();
    else if (statusCode == 404)
        window.location.href = $('#notFoundUrl').val();
}