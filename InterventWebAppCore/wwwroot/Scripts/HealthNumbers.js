function UpdateMetric(ctrl, event) {
    switch ($(ctrl).attr('id')) {
        case "HeightFt":
        case "HeightInch":
            onImperialHeightChange();
            break;
        case "Weight":
            $("#WeightMetric").val(ValidateData(($("#Weight").val() / WeightConvUnit).toFixed(1)));
            break;
        case "Waist":
            $("#WaistMetric").val(ValidateData(($("#Waist").val() * WaistConvUnit).toFixed(1)));
            break;
        case "TotalChol":
            $("#TotalCholMetric").val(ValidateData(($("#TotalChol").val() * CholConvUnit).toFixed(1)));
            break;
        case "Trig":
            $("#TrigMetric").val(ValidateData(($("#Trig").val() * TrigConvUnit).toFixed(1)));
            break;
        case "HDL":
            $("#HDLMetric").val(ValidateData(($("#HDL").val() * HDLConvUnit).toFixed(1)));
            break;
        case "LDL":
            $("#LDLMetric").val(ValidateData(($("#LDL").val() * LDLConvUnit).toFixed(1)));
            break;
        case "Glucose":
            $("#GlucoseMetric").val(ValidateData(($("#Glucose").val() / GlucoseConvUnit).toFixed(1)));
            break;
        case "DesiredWeight":
            $("#DesiredWeightMetric").val(ValidateData(($("#DesiredWeight").val() / WeightConvUnit).toFixed(1)));
            break;
    }
}

function UpdateImperial(ctrl, event) {
    switch ($(ctrl).attr('id')) {
        case "HeightCM":
            onMetricHeightChange();
            break;
        case "WeightMetric":
            $("#Weight").val(ValidateData(($("#WeightMetric").val() * WeightConvUnit).toFixed(1)));
            break;
        case "WaistMetric":
            $("#Waist").val(ValidateData(($("#WaistMetric").val() / WaistConvUnit).toFixed(1)));
            break;
        case "TotalCholMetric":
            $("#TotalChol").val(ValidateData(($("#TotalCholMetric").val() / CholConvUnit).toFixed(1)));
            break;
        case "TrigMetric":
            $("#Trig").val(ValidateData(($("#TrigMetric").val() / TrigConvUnit).toFixed(1)));
            break;
        case "HDLMetric":
            $("#HDL").val(ValidateData(($("#HDLMetric").val() / HDLConvUnit).toFixed(1)));
            break;
        case "LDLMetric":
            $("#LDL").val(ValidateData(($("#LDLMetric").val() / LDLConvUnit).toFixed(1)));
            break;
        case "GlucoseMetric":
            $("#Glucose").val(ValidateData(($("#GlucoseMetric").val() * GlucoseConvUnit).toFixed(1)));
            break;
        case "DesiredWeightMetric":
            $("#DesiredWeight").val(ValidateData(($("#DesiredWeightMetric").val() * WeightConvUnit).toFixed(1)));
            break;
    }
}

function onImperialHeightChange() {
    var foot = $("#HeightFt").val();
    var inches = $("#HeightInch").val();
    if (foot.length == 0) {
        foot = 0;
    }
    if (inches.length == 0) {
        inches = 0;
    }
    $("#HeightCM").val(ValidateData(((parseFloat(foot * 12) * HeightConvUnit) + (parseFloat(inches) * HeightConvUnit)).toFixed(1)));
}

function onMetricHeightChange() {
    var length = parseFloat($("#HeightCM").val() / HeightConvUnit).toFixed(2);
    $("#HeightFt").val(ValidateData(parseFloat(Math.floor(length / 12))));
    $("#HeightInch").val(ValidateData(parseFloat((length - (12 * Math.floor(length / 12)))).toFixed(1)));
}

function ValidateInchs() {
    var height = $('#HeightFt').val();
    var inchDisabled = height >= 8 ? true : false;
    if ('@Model.readOnly' == 'False')
        $('#HeightInch').prop('disabled', inchDisabled);
    if (inchDisabled)
        $('#HeightInch').val('')
}

function compareBP() {
    return (parseInt($("#SBP").val()) > parseInt($("#DBP").val())) || (!$.isNumeric($("#SBP").val()) || !$.isNumeric($("#DBP").val()));
}

function IsBMIOutOfRange() {
    var feet, inches;
    var height;
    var weight;
    var inchestocmmultiplier = 2.54;
    var poundtokgmultiplier = 0.45359237;

    if ($("#HeightFt").length && $("#HeightInch").length) {
        feet = $('#HeightFt').val();
        inches = $("#HeightInch").val();
        height = (feet * 12 + Number(inches)) * inchestocmmultiplier;
        weight = $('#Weight').val();
        weight = weight * poundtokgmultiplier;
    }
    else {
        height = $('#HeightCM').val();
        weight = $('#Weight').val();
    }
    var BMI = ((weight / (height * height)) * 10000);
    if (BMI < 18.5 || BMI > 45) {
        return true;
    }
    return false;
}

function ValidateData(data) {
    if (data == 0 || data == 0.0)
        return '';
    else
        return data;
}
