function CreateSleepOverview(sleepData){
    // Create chart instance
    var chart = am4core.create("sleep-chartdiv", am4charts.PieChart);

    // Add and configure Series
    var pieSeries = chart.series.push(new am4charts.PieSeries());
    pieSeries.dataFields.value = "litres";
    pieSeries.dataFields.category = "country";
    pieSeries.labels.template.disabled = true;
    pieSeries.ticks.template.disabled = true;
    pieSeries.tooltip.getFillFromObject = false;
    pieSeries.tooltip.background.fill = am4core.color("#484848");
    pieSeries.slices.template.tooltipText = "[font-size: 12px #bcbcbc]{category}: [font-size: 16px #ffffff]{value.value}";

    // Put a thick white border around each Slice
    pieSeries.colors.list = [
      am4core.color("#6871ff"),
      am4core.color("#dc6dff"),
    ];
    //pieSeries.slices.template.strokeWidth = 0;

    // Add a legend
    chart.legend = new am4charts.Legend();
    chart.legend.labels.template.text = "[#999999]{category}[/]";

    var marker = chart.legend.markers.template.children.getIndex(0);
    marker.cornerRadius(12, 12, 12, 12);
    marker.strokeWidth = 1;
    marker.strokeOpacity = 1;
    var markerTemplate = chart.legend.markers.template
    markerTemplate.width = 11;
    markerTemplate.height = 11;

    chart.data = sleepData;

    /* Create a cursor */
    chart.cursor = new am4charts.XYCursor();
}