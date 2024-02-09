function ValidateDecimal(ctrl, event) {
    var $this = $(ctrl);
    if ((event.which != 46 || $this.val().indexOf('.') != -1) &&
        ((event.which < 48 || event.which > 57) &&
            (event.which != 0 && event.which != 8))) {
        event.preventDefault();
    }

    var text = $(ctrl).val();

    if ((text.indexOf('.') != -1) &&
        (text.substring(text.indexOf('.')).length > 1) &&
        (event.which != 0 && event.which != 8) &&
        ($(ctrl)[0].selectionStart >= text.length - 1)) {
        event.preventDefault();
    }
}

function ValidateTwoDecimalPoint(ctrl, event) {
    var $this = $(ctrl);
    if ((event.which != 46 || $this.val().indexOf('.') != -1) &&
        ((event.which < 48 || event.which > 57) &&
            (event.which != 0 && event.which != 8))) {
        event.preventDefault();
    }

    var text = $(ctrl).val();

    if ((text.indexOf('.') != -1) &&
        (text.substring(text.indexOf('.')).length > 2) &&
        (event.which != 0 && event.which != 8) &&
        ($(ctrl)[0].selectionStart >= text.length - 1)) {
        event.preventDefault();
    }
}

function ValidateNumeric(ctrl, event) {
    var $this = $(ctrl);
    if ((event.which < 48 || event.which > 57) &&
            (event.which != 0 && event.which != 8)) {
        event.preventDefault();
    }
}