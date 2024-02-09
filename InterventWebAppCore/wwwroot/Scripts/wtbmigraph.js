function CreateWTOverview(wtData) {
    // Create chart instance
    var chart = am4core.create("wtbmi-chartdiv", am4charts.XYChart);

    // Create axes
    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.minGridDistance = 20;
    dateAxis.startLocation = 0.5;
    dateAxis.endLocation = 0.5;
    dateAxis.renderer.grid.template.disabled = true;
    dateAxis.dateFormats.setKey("day", "dd\nMMM");
	dateAxis.renderer.labels.template.fill = am4core.color("#999999");
	dateAxis.renderer.labels.template.rotation = 68;
	dateAxis.renderer.labels.template.horizontalCenter = "right";
	dateAxis.renderer.labels.template.verticalCenter = "middle";

    // Create value axis
    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.opposite = true;
    valueAxis.renderer.strokeWidth = 1;
    valueAxis.renderer.labels.template.fill = am4core.color("#999999");

    // Create series
    var series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueY = "value";
    series.dataFields.dateX = "date";
    series.stroke = am4core.color("#fd6191");
    series.strokeWidth = 1;
    series.tensionX = 0.8;
    series.fill = am4core.color("#fd6191");
    series.fillOpacity = 1;
    series.bullets.push(new am4charts.CircleBullet());
    series.data = wtData;

    /* Add a single HTML-based tooltip to first series */
    series.tooltipHTML = `<p style="font-size: 14px; margin-bottom: 4px; color: #bcbcbc;" > {dateX.formatDate('dd MMMM')} </p>
<ul>
<li style="color: #fd6191; font-size: 20px;">
  <p style="font-size: 12px; margin-bottom: 10px; color: #bcbcbc;">Weight <br><strong style="font-size: 16px; color: #fff;">{valueY}</strong> IBS</p>
</li>
</ul>`;
    series.tooltip.getFillFromObject = false;
    series.tooltip.background.fill = am4core.color("#484848");
    series.tooltip.pointerOrientation = "vertical";
    series.tooltip.background.filters.clear();

    var fillModifier = new am4core.LinearGradientModifier();
    fillModifier.opacities = [0.2, 0];
    fillModifier.offsets = [0, 0.4];
    fillModifier.gradient.rotation = 90;
    series.segments.template.fillModifier = fillModifier;

    /* Create a cursor */
    chart.cursor = new am4charts.XYCursor();
}