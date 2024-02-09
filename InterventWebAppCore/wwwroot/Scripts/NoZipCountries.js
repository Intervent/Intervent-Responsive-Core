function CountryChange(countrydiv, unit, state, zip, url, race, raceUrl, zipRequired, zipUrl) {
    if (unit !== '' && $("#" + countrydiv).val() === 236) {
        $('#' + unit).val('Imperial');
    }
    $("#" + state).empty();
    var countryId = $('#' + countrydiv).val();
    if (zipRequired) {
        if (countryId != "") {
            $.ajax({
                type: "POST",
                dataType: 'json',
                url: zipUrl,
                data: { CountryId: countryId },
                success: function (data) {
                    var hasZip = data.HasZipCode;
                    if (hasZip) {
                        if ($("input[id='SkipValidation']:checked").length != 0)
                            element.required = true;
                        $('#' + zip).prop("disabled", false);
                    }
                    else {
                        $('#' + zip).prop("disabled", true);
                        element.required = false;
                        $("#" + zip).val('');
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                RedirectToErrorPage(jqXHR.status);
            });
        }
        else {
            if ($("input[id='SkipValidation']:checked").length != 0)
                element.required = true;
            $('#' + zip).prop("disabled", false);
        }
    }
    if (countryId !== '') {
        $("#" + race).empty();
        $.ajax({
            type: "POST",
            dataType: 'json',
            url: url,
            data: { CountryId: countryId },
            success: function (data) {
                $("#" + state).append("<option value=''></option>");
                for (i = 0; i < data.Records.length; i++) {
                    $("#" + state).append("<option value=" + data.Records[i].Id + ">" + data.Records[i].Name + "</option>");
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            RedirectToErrorPage(jqXHR.status);
        });
        if (race !== '') {
            $.ajax({
                type: "POST",
                dataType: 'json',
                url: raceUrl,
                data: { CountryId: countryId },
                success: function (data) {
                    $("#" + race).append("<option value=''></option>");
                    for (i = 0; i < data.Records.length; i++) {
                        $("#" + race).append("<option value=" + data.Records[i].Id + ">" + data.Records[i].Name + "</option>");
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                RedirectToErrorPage(jqXHR.status);
            });
        }
    }
}