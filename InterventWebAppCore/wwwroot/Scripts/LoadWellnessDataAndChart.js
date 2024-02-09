function GetHealthData(currentPage, pageSize, totalRecord, isTeamsBP, url, weightText, waistText, graphandData) {
    var formdata = 'page=' + currentPage + '&pageSize=' + pageSize + '&totalRecords=' + totalRecord;
    var healthData = [];
    var statusData = [];
    var isDiabetic = false;
    $('#wellness-results').html("");
    $.ajax({
        url: url,
        type: 'POST',
        dataType: "json",
        async: false,
        data: formdata,
        success: function (data) {
            if (data != null && data.Records != null && data.Records.length > 0) {
                isDiabetic = data.isDiabetic;
                if (graphandData) {
                    totalRecords = data.TotalRecords;
                    totalPages = parseInt((totalRecords + pageSize - 1) / pageSize); 
                    $("#health-data-body").html("");
                    $('#pagination-centered').addClass("hide");
                    var html = "<table class='table basic-table2'>";
                    html += "<thead><tr><th><span>Date</span></th><th><span>Source</span></th><th><span>" + weightText + "</span></th><th><span>SBP</span></th><th><span>DBP</span></th><th><span>" + waistText + "</span></th><th><span>PA (mins)</span></th>";
                    if (isDiabetic)
                        html += "<th><span>A1C</span></th>";
                    html += "</tr></thead>";
                    for (var i = 0; i < data.Records.length; i++) {
                        var path = "data-modal-path='../AddWellnessData/";
                        if (isTeamsBP) {
                            path = "data-modal-path='../AddEditTeamsBP_PPR/";
                        }
                        html += "<tr><td><a style='color:deepskyblue' href='#' data-open='add-wellness-data' " + path + data.Records[i].Id + "'>" + toLocalDateFormat(data.Records[i].CollectedOn) + "</a></td>" + "<td>" + (data.Records[i].SourceHRA != null ? "HRA" : (data.Records[i].SourceFollowUp != null ? "FollowUp" : (data.Records[i].UserId == data.Records[i].UpdatedBy ? "Self-entered" : "Coaching"))) + "</td>" + "<td>" + (data.Records[i].Weight != null ? data.Records[i].Weight : "") + "</td>" +
                            "<td>" + (data.Records[i].SBP != null ? data.Records[i].SBP : "") + "</td>" + "<td>" + (data.Records[i].DBP != null ? data.Records[i].DBP : "") + "</td>" + "<td>" + (data.Records[i].WaistCircumference != null ? data.Records[i].WaistCircumference : "") + "</td>" +
                            "<td>" + (data.Records[i].ExerMin != null ? data.Records[i].ExerMin : "") + "</td>";
                        if (isDiabetic)
                            html += "<td>" + (data.Records[i].A1C != null ? data.Records[i].A1C : "") + "</td>";
                        html += "</tr>";
                    }
                    for (var i = 0; i < data.AllRecords.length; i++) {
                        if (isDiabetic)
                            healthData.push({ date: data.AllRecords[i].CollectedOn, data1: data.AllRecords[i].Weight, data2: data.AllRecords[i].SBP, data3: data.AllRecords[i].DBP, data4: data.AllRecords[i].A1C });
                        else
                            healthData.push({ date: data.AllRecords[i].CollectedOn, data1: data.AllRecords[i].Weight, data2: data.AllRecords[i].SBP, data3: data.AllRecords[i].DBP });
                        statusData.push({ date: data.AllRecords[i].CollectedOn, data1: data.AllRecords[i].HealthyEating, data2: data.AllRecords[i].PhysicallyActive, data3: data.AllRecords[i].ManageStress, data4: data.AllRecords[i].Motivation });
                    }
                    if (totalRecords > pageSize)
                        $('#pagination-centered').removeClass("hide");
                    $("#wellness-results").append(html);
                    $(document).foundation();
                    AddPager();
                }
                else
                {
                    for (var i = 0; i < data.AllRecords.length; i++) {
                        if (isDiabetic)
                            healthData.push({ date: data.AllRecords[i].CollectedOn, data1: data.AllRecords[i].Weight, data2: data.AllRecords[i].SBP, data3: data.AllRecords[i].DBP, data4: data.AllRecords[i].A1C });
                        else
                            healthData.push({ date: data.AllRecords[i].CollectedOn, data1: data.AllRecords[i].Weight, data2: data.AllRecords[i].SBP, data3: data.AllRecords[i].DBP });
                     }
                }
            }
            else {
                $('.content-table').addClass("hide");
                $('#pagination-centered').addClass("hide");
            }
            $("[data-open]").on('click', function (e) {
                showmodal($(this), e)
            });
        },
        complete: function () {
            $(document).foundation();
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        RedirectToErrorPage(jqXHR.status);
    });
    return [healthData, statusData, isDiabetic];
}
function CreateChart(data, name, label1, label2, label3, label4) {
    am4core.useTheme(am4themes_animated);
    // Themes end
    am4core.addLicense("CH39169069");
    // Create chart instance
    var chart = am4core.create(name, am4charts.XYChart);

    // Increase contrast by taking evey second color
    chart.colors.step = 2;

    // Add data
    chart.data = data;

    // Create axes
    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.minGridDistance = 50;

    // Create series
    function createAxisAndSeries(field, name, opposite, bullet) {
        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

        var series = chart.series.push(new am4charts.LineSeries());
        series.dataFields.valueY = field;
        series.dataFields.dateX = "date";
        series.strokeWidth = 2;
        valueAxis.maxPrecision = 1;
        series.yAxis = valueAxis; 
        series.name = name;
        series.tooltipText = "{name}: [bold]{valueY}[/]";
        series.tensionX = 0.8;

        var interfaceColors = new am4core.InterfaceColorSet();

        switch (bullet) {
            case "triangle":
                var bullet = series.bullets.push(new am4charts.Bullet());
                bullet.width = 12;
                bullet.height = 12;
                bullet.horizontalCenter = "middle";
                bullet.verticalCenter = "middle";

                var triangle = bullet.createChild(am4core.Triangle);
                triangle.stroke = interfaceColors.getFor("background");
                triangle.strokeWidth = 2;
                triangle.direction = "top";
                triangle.width = 12;
                triangle.height = 12;
                break;
            case "rectangle":
                var bullet = series.bullets.push(new am4charts.Bullet());
                bullet.width = 10;
                bullet.height = 10;
                bullet.horizontalCenter = "middle";
                bullet.verticalCenter = "middle";

                var rectangle = bullet.createChild(am4core.Rectangle);
                rectangle.stroke = interfaceColors.getFor("background");
                rectangle.strokeWidth = 2;
                rectangle.width = 10;
                rectangle.height = 10;
                break;
            default:
                var bullet = series.bullets.push(new am4charts.CircleBullet());
                bullet.circle.stroke = interfaceColors.getFor("background");
                bullet.circle.strokeWidth = 2;
                break;
        }
        valueAxis.renderer.line.strokeOpacity = 1;
        valueAxis.renderer.line.strokeWidth = 2;
        valueAxis.renderer.line.stroke = series.stroke;
        valueAxis.renderer.labels.template.fill = series.stroke;
        valueAxis.renderer.opposite = opposite;
        valueAxis.renderer.grid.template.disabled = true;
    }

    createAxisAndSeries("data1", label1, false, "circle");
    createAxisAndSeries("data2", label2, false, "triangle");
    createAxisAndSeries("data3", label3, true, "rectangle");
    if(label4 != null)
        createAxisAndSeries("data4", label4, true, "rectangle");

    // Add legend
    chart.legend = new am4charts.Legend();

    // Add cursor
    chart.cursor = new am4charts.XYCursor();

    return chart;
}