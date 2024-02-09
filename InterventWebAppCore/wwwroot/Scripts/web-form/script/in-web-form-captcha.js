var webForm = document.getElementById("in-web-form");
var baseURL = "http://uat.myintervent.com/InterventWebApp";

document.addEventListener("DOMContentLoaded", function () {
    createForm();
});

function createForm() {
    var style = document.createElement("link");
    style.setAttribute("href", baseURL + "/Content/css/in-web-form.css");
    style.setAttribute("rel", "stylesheet");
    style.setAttribute("type", "text/css");

    var script = document.createElement("script");
    script.setAttribute("src", baseURL + "/Scripts/jquery-3.6.1.min.js");

    var script1 = document.createElement("script");
    script1.setAttribute("src", "https://www.google.com/recaptcha/api.js");
    script1.async = true;
    script1.defer = true;

    var head = document.getElementsByTagName("head")[0];
    head.appendChild(style);
    var body = document.getElementsByTagName("body")[0];
    body.appendChild(script);
    body.appendChild(script1);

    var form = document.createElement("form");
    form.setAttribute("class", "intervent-web-form");
    form.setAttribute("id", "intervent-web-form");
    form.setAttribute("name", "intervent_web_form");
    form.addEventListener("submit", function (evt) {
        var response = grecaptcha.getResponse();
        if (response.length == 0) {
            alert("please verify you are human!");
            evt.preventDefault();
            return false;
        }
        var jqxhr = $.post(baseURL + '/WebForm/WebFormApi/SaveWebForm', $('#intervent-web-form').serialize())
            .success(function (data) {
                $("#in_form_message").html(data + ".\n Thank you.");
                $("#intervent-web-form").hide();
            })
            .fail(function (data) {
                alert(data);
            });
        evt.preventDefault();
    });

    var firstName = document.createElement("input");
    firstName.setAttribute("type", "text");
    firstName.setAttribute("id", "in_first_name");
    firstName.setAttribute("name", "in_first_name");
    firstName.setAttribute("maxlength", "50");
    firstName.setAttribute("placeholder", "Enter First Name");
    firstName.setAttribute("required", "required");
    var fNameLabel = document.createElement("label");
    fNameLabel.textContent += "First Name";
    fNameLabel.append(firstName);

    var lastName = document.createElement("input");
    lastName.setAttribute("type", "text");
    lastName.setAttribute("id", "in_last_name");
    lastName.setAttribute("name", "in_last_name");
    lastName.setAttribute("maxlength", "50");
    lastName.setAttribute("placeholder", "Enter Last Name");
    lastName.setAttribute("required", "required");
    var lNameLabel = document.createElement("label");
    lNameLabel.textContent += "Last Name";
    lNameLabel.append(lastName);

    var emailID = document.createElement("input");
    emailID.setAttribute("type", "email");
    emailID.setAttribute("id", "in_email_id");
    emailID.setAttribute("name", "in_email_id");
    emailID.setAttribute("maxlength", "50");
    emailID.setAttribute("placeholder", "Enter E-Mail");
    emailID.setAttribute("required", "required");
    var mailLabel = document.createElement("label");
    mailLabel.textContent += "Enter Email";
    mailLabel.append(emailID);

    var phoneNo = document.createElement("input");
    phoneNo.setAttribute("type", "text");
    phoneNo.setAttribute("id", "in_phoneno");
    phoneNo.setAttribute("name", "in_phoneno");
    phoneNo.setAttribute("maxlength", "15");
    phoneNo.setAttribute("placeholder", "Enter Phone");
    phoneNo.setAttribute("required", "required");
    var numberLabel = document.createElement("label");
    numberLabel.textContent += "Enter Phone Number";
    numberLabel.append(phoneNo);

    var clientCode = document.createElement("input");
    clientCode.setAttribute("type", "hidden");
    clientCode.setAttribute("id", "in_client_code");
    clientCode.setAttribute("name", "in_client_code");
    clientCode.setAttribute("value", "test123");

    var array = [
        "My CGM values appear to be inaccurate",
        "My sensor failed during the warm up phase",
        "I am experiencing skin Reaction at the insertion site",
        "My CGM is not accepting calibration",
        "My sensor has fallen off or has started to peel off",
        "I am getting no readings, ??? or sensor error displayed for over 1 hour",
        "My sensor failed after successfully completing the warm up phase",
        "I am experiencing bleeding or there is blood at the insertion site"
    ];

    var selectList = document.createElement("select");
    selectList.setAttribute("id", "in_complaint_code");
    selectList.setAttribute("name", "in_complaint_code");
    selectList.setAttribute("required", "required");
    var selectLabel = document.createElement("label");
    selectLabel.textContent += "Reason for Complaint";
    selectLabel.append(selectList);

    for (var i = 0; i < array.length; i++) {
        var option = document.createElement("option");
        option.value = array[i];
        option.text = array[i];
        selectList.appendChild(option);
    }

    var button = document.createElement("input");
    button.setAttribute("type", "submit");
    button.setAttribute("value", "Submit");
    button.setAttribute("id", "in-form-validate");
    button.setAttribute("class", "in-button");

    var fieldset1 = document.createElement("fieldset");
    var legend1 = document.createElement("legend");
    legend1.textContent += "Personal Information";
    fieldset1.append(legend1);

    var fieldset2 = document.createElement("fieldset");
    var legend2 = document.createElement("legend");
    legend2.textContent += "Contact Information";
    fieldset2.append(legend2);

    var captcha = document.createElement("div");
    captcha.setAttribute("class", "g-recaptcha");
    captcha.setAttribute("data-sitekey", "6LfmyjAfAAAAAGXLhh3qYUoVL6lr50LtqYNFy9A7");

    var messager = document.createElement("div");
    messager.setAttribute("id", "in_form_message");
    messager.setAttribute("class", "intervent-result");

    fieldset1.append(fNameLabel);
    fieldset1.append(lNameLabel);
    fieldset2.append(mailLabel);
    fieldset2.append(numberLabel);
    fieldset2.append(selectLabel);
    form.append(clientCode);
    form.append(fieldset1);
    form.append(fieldset2);
    form.append(button);
    form.append(captcha);
    webForm.append(form);
    webForm.append(messager);
}