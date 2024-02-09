var webForm = document.getElementById("in-web-form");
var baseURL = "https://uat.myintervent.com/InterventWebApp";


document.addEventListener("DOMContentLoaded", function (event) {

    createForm();
});

function createForm() {
    var style = document.createElement("link");
    style.setAttribute("href", "/Content/css/in-web-form.css");

    style.setAttribute("href", baseURL + "/Content/css/in-web-form.css");
    style.setAttribute("rel", "stylesheet");
    style.setAttribute("type", "text/css");

    var script = document.createElement("script");
    script.setAttribute("src", baseURL + "/Scripts/jquery-3.6.1.min.js");

    var head = document.getElementsByTagName("head")[0];
    head.appendChild(style);
    var body = document.getElementsByTagName("body")[0];
    body.appendChild(script);

    var form = document.createElement("form");
    form.setAttribute("class", "intervent-web-form");
    form.setAttribute("id", "intervent-web-form");
    form.setAttribute("name", "intervent_web_form");
    form.addEventListener("submit", function (evt) {
        if (validateEmail()) {
            var spinner = document.getElementById("in-loading-spinner");
            document.getElementById("in_client_code").value = location.origin;
            spinner.classList.remove("hide");

            var jqxhr = $.post(baseURL + '/WebForm/WebFormApi/SaveWebForm', $('#intervent-web-form').serialize())
                .done(function (data) {
                    spinner.classList.add("hide");
                    $("#in_form_message").html(data);
                    $("#intervent-web-form").hide();
                })
                .fail(function (data) {
                    spinner.classList.add("hide");
                    alert(data.statusText);
                });
            evt.preventDefault();
        }
        else {
            evt.preventDefault();
        }
    });

    var firstName = document.createElement("input");
    firstName.setAttribute("type", "text");
    firstName.setAttribute("id", "in_first_name");
    firstName.setAttribute("name", "in_first_name");
    firstName.setAttribute("maxlength", "50");
    firstName.setAttribute("required", "required");
    var fNameLabel = document.createElement("label");
    fNameLabel.textContent += "First Name";
    fNameLabel.append(firstName);

    var lastName = document.createElement("input");
    lastName.setAttribute("type", "text");
    lastName.setAttribute("id", "in_last_name");
    lastName.setAttribute("name", "in_last_name");
    lastName.setAttribute("maxlength", "50");
    lastName.setAttribute("required", "required");
    var lNameLabel = document.createElement("label");
    lNameLabel.textContent += "Last Name";
    lNameLabel.append(lastName);

    var emailID = document.createElement("input");
    emailID.setAttribute("type", "email");
    emailID.setAttribute("id", "in_email_id");
    emailID.setAttribute("name", "in_email_id");
    emailID.addEventListener("blur", validateEmail);
    emailID.setAttribute("required", "required");
    var mailLabel = document.createElement("label");
    mailLabel.textContent += "Email";
    mailLabel.append(emailID);

    var confirmEmailID = document.createElement("input");
    confirmEmailID.setAttribute("type", "email");
    confirmEmailID.setAttribute("id", "in_confirm_email_id");
    confirmEmailID.setAttribute("name", "in_confirm_email_id");
    confirmEmailID.setAttribute("required", "required");
    confirmEmailID.addEventListener("blur", validateEmail);
    confirmEmailID.addEventListener("input", function () {
        document.getElementById("in_confirm_email_id").setCustomValidity('');
    });

    var confirmMailLabel = document.createElement("label");
    confirmMailLabel.textContent += "Confirm Email";
    confirmMailLabel.append(confirmEmailID);

    var phoneNo = document.createElement("input");
    phoneNo.setAttribute("type", "text");
    phoneNo.setAttribute("id", "in_phone_no");
    phoneNo.setAttribute("name", "in_phone_no");
    phoneNo.setAttribute("maxlength", "15");
    phoneNo.setAttribute("required", "required");
    var numberLabel = document.createElement("label");
    numberLabel.textContent += "Phone Number";
    numberLabel.append(phoneNo);

    var clientCode = document.createElement("input");
    clientCode.setAttribute("type", "hidden");
    clientCode.setAttribute("id", "in_client_code");
    clientCode.setAttribute("name", "in_client_code");
    // Inquiry type select tag 
    var arrayInquiry = [
        ["", "--Select--"]
    ];

    var selectList = document.createElement("select");
    selectList.setAttribute("id", "in_inquiry_type");
    selectList.setAttribute("name", "in_inquiry_type");
    selectList.setAttribute("required", "required");
    selectList.addEventListener("click", addInquiryDropdown);
    var selectLabel = document.createElement("label");
    selectLabel.textContent += "Subject of Your Message";
    selectLabel.append(selectList);

    for (var i = 0; i < arrayInquiry.length; i++) {
        var option = document.createElement("option");
        option.value = arrayInquiry[i][0];
        option.text = arrayInquiry[i][1];
        selectList.appendChild(option);
    }
    // Inquiry type select tag end

    // Pogo automatic monitor radio buttons
    var pogoMonitorDiv = document.createElement("div");
    pogoMonitorDiv.setAttribute("class", "in_full_length_container");
    var monitorLabelMain = document.createElement("label");
    monitorLabelMain.textContent += "Do you have a POGO Automatic Monitor?";

    var monitorRadioBtn1 = document.createElement("input");
    monitorRadioBtn1.setAttribute("type", "radio");
    monitorRadioBtn1.setAttribute("id", "in_monitor_radio_yes");
    monitorRadioBtn1.setAttribute("name", "in_pogo_monitor");
    monitorRadioBtn1.addEventListener("click", displaySerialNumber);
    monitorRadioBtn1.setAttribute("value", "true");
    monitorRadioBtn1.setAttribute("required", "required");
    var monitorLabelYes = document.createElement("label");
    monitorLabelYes.setAttribute("for", "in_monitor_radio_yes");
    monitorLabelYes.setAttribute("class", "in_pogo_monitor");
    monitorLabelYes.textContent += "Yes";

    var monitorRadioBtn2 = document.createElement("input");
    monitorRadioBtn2.setAttribute("type", "radio");
    monitorRadioBtn2.setAttribute("id", "in_monitor_radio_no");
    monitorRadioBtn2.setAttribute("name", "in_pogo_monitor");
    monitorRadioBtn2.addEventListener("click", displaySerialNumber);
    monitorRadioBtn2.setAttribute("value", "false");
    var monitorLabelNo = document.createElement("label");
    monitorLabelNo.setAttribute("for", "in_monitor_radio_no");
    monitorLabelNo.setAttribute("class", "in_pogo_monitor");
    monitorLabelNo.textContent += "No";

    var monitorNumber = document.createElement("input");
    monitorNumber.setAttribute("type", "text");
    monitorNumber.setAttribute("id", "in_monitor_number");
    monitorNumber.setAttribute("name", "in_monitor_number");
    monitorNumber.setAttribute("placeholder", "Enter Serial Number");
    var monitorNumberLabel = document.createElement("label");
    monitorNumberLabel.textContent += "Serial Number";
    monitorNumberLabel.setAttribute("id", "in_pogo_serial_number");
    monitorNumberLabel.setAttribute("class", "hide");
    monitorNumberLabel.append(monitorNumber);

    pogoMonitorDiv.append(monitorLabelMain);
    pogoMonitorDiv.append(monitorRadioBtn1);
    pogoMonitorDiv.append(monitorLabelYes);
    pogoMonitorDiv.append(monitorRadioBtn2);
    pogoMonitorDiv.append(monitorLabelNo);
    pogoMonitorDiv.append(monitorNumberLabel);
    // Pogo automatic monitor radio buttons end

    var pogoFeedbackDiv = document.createElement("div");
    pogoFeedbackDiv.setAttribute("class", "in_full_length_container");
    var feedback = document.createElement("textarea");
    feedback.setAttribute("id", "in_feedback");
    feedback.setAttribute("name", "in_feedback");
    feedback.setAttribute("required", "required");
    var feedbackLabel = document.createElement("label");
    feedbackLabel.textContent += "Question / Feedback";
    feedbackLabel.append(feedback);
    pogoFeedbackDiv.append(feedbackLabel);

    var button = document.createElement("button");
    button.setAttribute("type", "submit");
    button.append("Submit");
    button.setAttribute("id", "in-form-validate");
    button.setAttribute("class", "in-button");

    var spinner = document.createElement("div");
    spinner.setAttribute("class", "loading-spinner hide");
    spinner.setAttribute("id", "in-loading-spinner");

    var fieldset1 = document.createElement("fieldset");
    var legend1 = document.createElement("legend");
    legend1.textContent += "How can we help you?";
    fieldset1.append(legend1);

    var captcha = document.createElement("div");
    captcha.setAttribute("class", "g-recaptcha");
    captcha.setAttribute("data-sitekey", "6LfmyjAfAAAAAGXLhh3qYUoVL6lr50LtqYNFy9A7");

    var messager = document.createElement("div");
    messager.setAttribute("id", "in_form_message");
    messager.setAttribute("class", "intervent-result");

    fieldset1.append(pogoMonitorDiv);
    fieldset1.append(fNameLabel);
    fieldset1.append(lNameLabel);
    fieldset1.append(mailLabel);
    fieldset1.append(confirmMailLabel);
    fieldset1.append(numberLabel);
    fieldset1.append(selectLabel);
    fieldset1.append(pogoFeedbackDiv);
    form.append(clientCode);
    form.append(fieldset1);
    button.prepend(spinner);
    form.append(button);
    form.append(captcha);
    webForm.append(form);
    webForm.append(messager);

}

function addInquiryDropdown() {
    var jqxhr1 = $.get(baseURL + '/WebForm/WebFormApi/GetInquiryTypes', { in_client_code: location.origin })
        .done(function (data) {
            if ($('#in_inquiry_type option').length <= 1) {
                if (data.Success) {
                    for (i = 0; i < data.Record.InquiryTypes.length; i++) {
                        $("#in_inquiry_type").append("<option value=" + data.Record.InquiryTypes[i].Id + ">" + data.Record.InquiryTypes[i].Type + "</option>");
                    }
                }
            }
        })
        .fail(function (data) {
            alert(data.statusText);
        });
}

function displaySerialNumber() {

    if (document.getElementById('in_monitor_radio_yes').checked) {
        document.getElementById("in_pogo_serial_number").classList.remove("hide");
        document.getElementById("in_monitor_number").setAttribute("required", "required");
    }
    else {
        document.getElementById("in_pogo_serial_number").classList.add("hide");
        document.getElementById("in_monitor_number").removeAttribute("required");
    }
}
function validateEmail() {
    if ($("#in_email_id").val().toLowerCase() == $("#in_confirm_email_id").val().toLowerCase()) {
        document.getElementById("in_confirm_email_id").setCustomValidity("");
        return true;
    }
    else if ($("#in_confirm_email_id").val() == '') {
        document.getElementById("in_confirm_email_id").setCustomValidity("Please fill out this field");
        return false;
    }
    else {
        document.getElementById("in_confirm_email_id").setCustomValidity("The email did not match");
        return false;
    }
}

