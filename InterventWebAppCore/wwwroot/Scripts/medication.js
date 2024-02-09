
$(document).foundation()

$(function () {
    var screen_height = $(window).height();
    var drugs = [];
    var screen_width = $(window).width();
    $(".step-block").css("height", screen_height);

    if (screen_width >= 1024) {
        $(".step-block").css("height", screen_height);
    }

    // Set counter default to zero
    var count = $("#counter").val();
    if (count == "")
        count = 0;
    var counter = parseInt(count);
    // Display total
    $("#counter").text(counter);

    // When button is clicked
    $("#add").on('click', function () {
        $('#CounterError').removeClass("is-visible");
        //Add 10 to counter
        counter = counter + 1;
        // Display total
        $("#counter").text(counter);
    });


    //Subtract
    $("#subtract").on('click', function () {
        if (counter > 0) {
            counter = counter - 1;
            $("#counter").text(counter);
        }
    });

    $(".row-open").on('click', function () {
        $(this).parent('tr').next(".expanded-row").slideToggle(200);
    });

    $("#btncnt1").on('click', function () {
        var ValidationList = ["Condition", "search-id"];
        validator(ValidationList);
        var condition = $('#Condition').val();
        var conditonText = $("#Condition option:selected").text();
        var drug = $("#search-id").val();
        var drugid = $("#DrugId").val();
        if (drugid) {
            var list = ["DrugId"];
            validator(list);
        }
        if (mDosage == "") {
            mDosage = naText;
        }
        if (condition != '' && drug != '') {
            $("#drug-conditon").html(conditonText);
            $("#drug").html(mDrugText);
            $("#dosage").html(mDosage);
            $("#Ingredient").html(mIngredient);
            $(".step-one, .step-three").hide();
            $(".step-two").fadeIn();
            $(".list-one").hide();
            $(".list-two").fadeIn();
            $(".list-btn").removeClass('active');
            $(".list-btn2").addClass('active');
        }
        count = $("#counter").val();
        if (count == "")
            count = 0;
        counter = parseInt(count);
    });

    $("#btncnt2, .edit-tbl").on('click', function () {
        $('#duplicateWarning').hide();
        var ValidationList = ["Frequency"];
        validator(ValidationList);
        var frequency = $('#Frequency').val();
        var frequencyText = $("#Frequency option:selected").text();
        var formulationText = $("#Formulation option:selected").text();
        var countText = $("#counter").text();
        if (!((counter > 0 || $("#counter").val() != '')))
            $('#CounterError').addClass("is-visible");
        if (frequency != '' && (counter > 0 || $("#counter").val() != '')) {
            $("#drugFormulation").html(countText + " " + formulationText);
            $("#drugFrequency").html(frequencyText);
            $(".step-one, .step-two").hide();
            $(".step-three").fadeIn();
            $(".list-one").hide();
            $(".list-two").fadeIn();
            $(".dsg-lst").fadeIn()
            $(".list-btn").removeClass('active');
            $(".list-btn3").addClass('active');
        }
    });

    $(".edit-option").on('click', function () {
        $(".step-two, .step-three").hide();
        $(".step-one").fadeIn();
        $(".list-two").hide();
        $(".list-one").fadeIn();
        $(".list-btn").removeClass('active');
        $(".list-btn1").addClass('active');
        $("#counter").val($("#counter").text());
    });

    //Jquery for add medication dropdown
    $("#search-id").autocomplete({
        minLength: minSearchLength,
        appendTo: "#ini-medi-list",
        source: function (request, response) {
            $.ajax({
                url: listdrugurl,
                async: true,
                type: 'POST',
                dataType: "json",
                data: { search: request.term },
                success: function (data) {
                    drugs = data.Drugs;
                    response(drugs.slice(0, sliceListLength));
                    if (drugs.length == 0) {
                        $(".ini-medi-list").show();
                        $("#no-results").show().html("<i class='fa fa-frown-o'></i> <h6>" + emptyResultText +"</h6>");
                    } else {
                        $("#no-results").empty().hide();
                    }
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                RedirectToErrorPage(jqXHR.status);
            });
        },
        select: function (event, ui) {
            $(".ini-medi-list").hide();
            $("#no-results").hide();
        },
        change: function (event, ui) {
            if (ui.item == null) {
                mDrugText = $("#search-id").val();
                mDosage = naText;
                mIngredient = null;
            }
            else {
                $('#Formulation').val(ui.item.FormulationType);
                mDrugText = ui.item.label;
                mDosage = ui.item.Dosage;
                mIngredient = ui.item.Ingredient;
                if ($("#isAdmin").val() == "0") {
                    ChangeIcon();
                }
            }
        }
    });

    $("#search-id").on("autocompleteopen", function (event, ui) {
        $(".ini-medi-list").show();

        var pane1 = $('.medi-search-list');
        pane1.jScrollPane({
            showArrows: false,
            resizeSensor: true
        });
        var api1 = pane1.data().jsp;
	});

	$("input#search-id").on('focusout', function(event) {
		if ($(event.relatedTarget).is("a#advancedSearch")) {
            event.preventDefault();
            $(".step-one").hide();
            $(".medi-adv-search").fadeIn();
            $(".ini-medi-list").hide();
            $("#no-results").hide();
            $('#search-filter').val('');
            $('#SFormulation').val('');
            $('#search-results').html('');
            var wt = $(window).width();
            if (wt < 640) {
                $(".left-white-bar").hide();
            }
        }
        else {
            $(".ini-medi-list").hide();
            $("#no-results").hide()
        }
    });

    function validator(list) {
        for (var i = 0; i < list.length; i++) {
            if ($('#' + list[i] + '').val() == "")
                $('#' + list[i] + 'Error').addClass("is-visible");
        }
    }

    $(".adv-search-back").on("click", function () {
        $(".medi-adv-search").hide();
        $(".step-one").fadeIn();
        var wt = $(window).width();
        if (wt < 640) {
            $(".left-white-bar").show();
        }
    });

    $("#medication-filter").on('click', function () {
        var searchtext = $('#search-filter').val();
        var sform = $('#SFormulation option:selected').text();
        $.ajax({
            url: listdrugurl,
            async: true,
            type: 'POST',
            dataType: "json",
            data: { search: searchtext, form: sform },
            success: function (data) {
                var searchHtml = "<div class='adv-search-list'><ul>";
                for (var i = 0; i <= data.Drugs.length - 1; i++) {
                    var searchData = data.Drugs[i];
                    searchHtml = searchHtml + "<li><a onclick='SelectDrug(" + '"' + (searchData.label) + '"' + ", " + '"' + (searchData.Dosage) + '"' + ", " + '"' + (searchData.Ingredient) + '"' + "," + searchData.FormulationType + ")'><div class='medi-name'><img src='" + Drug_Formulation[searchData.FormulationType] + "' alt=''><p><span>" + (searchData.label) + "</span>" + (searchData.Ingredient) + "</p></div><div class='medi-type text-right'><span> " + (searchData.ProductsForm) + "</span>" + (searchData.Dosage) + "</div></a></li>";
                }
                searchHtml = searchHtml + "</ul></div>";
                $('#search-results').html("");
                $('#search-results').append(searchHtml);


                var pane = $('.adv-search-list');
                pane.jScrollPane({
                    showArrows: false,
                    resizeSensor: true
                });
                var api = pane.data().jsp;
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
                RedirectToErrorPage(jqXHR.status);
            });
    }).on("click", function (ev) {
        ev.preventDefault();
    });

});