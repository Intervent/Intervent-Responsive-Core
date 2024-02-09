var callTemplate = {
    init: function () {
        $('#closeCallTemplate').on('click', function () {
            $('#add-calltemplate-data').foundation('close');
        });

        $('#add-callTemplate-button').on('click', function () {
            $('#apptCallTemplateId').val('');
        });

        $('#noOfCalls').off('blur');

        $('#noOfCalls').on('blur', function () {
            var numberOfCalls = parseInt($(this).val());
            for (i = 1; i <= numberOfCalls; i++) {
                if (!$("#call" + i).length) {
                    $('#callIntervals').append('<div id="divCall' + i + '" class="cell medium-12"><label>Call ' + i + '</label><input type="text" id="call' + i + '" /><small class="form-error" id="error">Enter Number</small></div>');
                }
            }
            var existingIntervals = $('#callIntervals').children();
            for (j = numberOfCalls + 1; j <= existingIntervals.length; j++) {
                $('#' + 'divCall' + j).remove();
            }
        });

        $('#saveCallTemplate').off('click');

        $('#saveCallTemplate').on('click', function () {
            //get the values
            var templateId = $('#apptCallTemplateId').val() == '' ? null : $('#apptCallTemplateId').val();
            var templateName = $('#templateName').val();
            var noOfWeeks = $('#noOfWeeks').val();
            var noOfCalls = $('#noOfCalls').val();
            var isActive = $('#isActive').is(':checked');
            var callIntervals = new Array();
            for (var i = 1; i <= noOfCalls; i++) {
                if (!isNaN(parseFloat($('#' + 'call' + i).val())) && isFinite($('#' + 'call' + i).val())) {
                    callIntervals.push($('#' + 'call' + i).val());
                }
                else {
                    $('#error').show();
                }
            }
            if (noOfCalls == callIntervals.length) {
                $.ajax({
                    type: "POST",
                    dataType: 'json',
                    url: 'SaveApptCallTemplate',
                    cache: false,
                    data: { templateId: templateId, templateName: templateName, noOfWeeks: noOfWeeks, noOfCalls: noOfCalls, isActive: isActive, intervalList: callIntervals },
                    success: function (data) {
                        $('#add-calltemplate-data').foundation('close');
                        location.reload();

                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                        RedirectToErrorPage(jqXHR.status);
                    });
            }
        });
    }
}