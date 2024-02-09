function GenerateNoteContent(Type, header, content, appointmentId, count, noteId){
    var textQuestion = "";
    var confirmType = "";
    if(Type == 1){
        textQuestion = "Did you complete a telephonic coaching session with the participant?";
        confirmType = "Please make sure this is a coaching note. Not coaching attempts or outreach attempts.";
    }
    else if (Type == 2) {
        textQuestion= "Are you sure this is an outreach call?";
        confirmType = "A CCR or coach making an outreach call to enroll a person in coaching or do an HRA. This could also be a result of a call from the outbound call manager.";
    }
    else if (Type == 3) {
        textQuestion= "Are you sure this is a 'tracking' call?";
        confirmType = "A 'tracking' call is a call made to participants who are enrolled in a coaching program and do not have a future appointment.";
    }
    else if (Type == 4) {
        textQuestion= "Are you sure you want to save a 'note'?";
        confirmType = "Any stable notes.";
    }
    else if (Type == 5) {
        textQuestion= "Are you sure this is an 'other' call?";
        confirmType = "Any other call that is not 'coaching', 'outreach', or 'tracking'.";
    }
    else if (Type == 6) {
        textQuestion= "Are you sure this is 'other referral'?";
        confirmType = "Please make sure this is a referral to or from any healthcare entity.";
    }
    else if(Type == 7){
        textQuestion = "Are you sure this is a 'Biometric Review' Note?";
        confirmType = "Please make sure this is a Biometric Review Only Note.";
    }
    else if (Type == 8) {
        textQuestion= "Are you sure this is a PCP referral?";
        confirmType = "Please make sure this is a PCP referral Note.";
    }
    else if (Type == 9) {
        textQuestion= "Are you sure this is a Navigation Call?";
        confirmType = "Please make sure this is a Navigation Call.";
    }
    else if (Type == 10) {
        textQuestion= "Are you sure this is a Critical Alert?";
        confirmType = "Please make sure this is a Critical Alert.";
    }
    else if (Type == 12) {
        textQuestion = "Are you sure this is a Complaint?";
        confirmType = "Please make sure this is a Complaint.";
    }
    else if (Type == 13) {
        textQuestion = "Are you sure this is a Marketing Feedback?";
        confirmType = "Please make sure this is a Marketing Feedback.";
    }
    else if (Type == 14) {
        textQuestion = "Are you sure this is a Labs Outreach call?";
        confirmType = "Please make sure this is a Labs Outreach call.";
    }
    if(Type == 1 || Type == 7){
        if (appointmentId)
        {
            if(header == '#add-note #note-header')
            {
                apptId = appointmentId; 
            } 
            else{
                appId = appointmentId; 
            }
            $(header).text(textQuestion);
            $(content).text(confirmType);
        }
        else if (parseInt(count) > 0)
        {
            if(header == '#add-note #note-header'){
                if ($("#Note_RefId2").val() == "" && parseInt(noteId)==0) {
                    $(header).text("Please select the Appointment for which note is added");
                    $(content).text("");
                    $("#add-note #NoteappId").show();
                    $("#ApptAddNote").removeClass('hide');
                    $("#saveNote").addClass('hide');
                    $("#NoteappId").attr("required", "required");
                }
                else {
                    $(header).text(textQuestion);
                    $(content).text(confirmType);
                } 
            }
            else{
                $(header).text("Please select the Appointment for which note is added");
                $("#appId").show();
                $(content).text("");
                $("#ApptNote").removeClass('hide');
                $("#saveChatNote").addClass('hide');
            }
        }
        else {
                $(header).text(textQuestion);
                $(content).text(confirmType);
        }
        return;
    }
    else{
        $(header).text(textQuestion);
        $(content).text(confirmType);
        return;
    }
}