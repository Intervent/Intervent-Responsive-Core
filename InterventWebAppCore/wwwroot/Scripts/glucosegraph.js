function CreateGlucoseOverview(glucData1, glucData2, glucData3) {
    // Create chart instance
    var chart = am4core.create("glucose-chartdiv", am4charts.XYChart);

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
    valueAxis.renderer.labels.template.fill = am4core.color("#999999");

    // Create series
    var series1 = chart.series.push(new am4charts.LineSeries());
    series1.dataFields.valueY = "value";
    series1.dataFields.dateX = "date";
    series1.stroke = am4core.color("#6871ff");
    series1.strokeWidth = 1;
    series1.tensionX = 0.8;
    series1.fill = am4core.color("#6871ff");
    series1.fillOpacity = 1;
    series1.bullets.push(new am4charts.CircleBullet());
    series1.data = glucData1;

    /* Add a single HTML-based tooltip to first series */
    series1.tooltipHTML = `<p style="font-size: 14px; margin-bottom: 4px; color: #bcbcbc;" > {dateX.formatDate('dd MMMM')} </p>
    <ul>
    <li style="color: #6871ff; font-size: 20px;">
      <p style="font-size: 12px; margin-bottom: 10px; color: #bcbcbc;">Pre-meal Glucose <br><strong style="font-size: 16px; color: #fff;">{valueY}</strong> mg/dl</p>
    </li>
    </ul>`;
    series1.tooltip.getFillFromObject = false;
    series1.tooltip.background.fill = am4core.color("#484848");
    series1.tooltip.pointerOrientation = "vertical";
    series1.tooltip.background.filters.clear();

    var fillModifier1 = new am4core.LinearGradientModifier();
    fillModifier1.opacities = [0.2, 0];
    fillModifier1.offsets = [0, 0.5];
    fillModifier1.gradient.rotation = 90;
    series1.segments.template.fillModifier = fillModifier1;

    var series2 = chart.series.push(new am4charts.LineSeries());
    series2.dataFields.valueY = "value";
    series2.dataFields.dateX = "date";
    series2.stroke = am4core.color("#dc6dff");
    series2.strokeWidth = 1;
    series2.tensionX = 0.8;
    series2.fill = am4core.color("#dc6dff");
    series2.fillOpacity = 1;
    series2.bullets.push(new am4charts.CircleBullet());
    series2.data = glucData2;

    /* Add a single HTML-based tooltip to first series */
    series2.tooltipHTML = `<ul>
    <li style="color: #dc6dff; font-size: 20px;">
      <p style="font-size: 12px; margin-bottom: 10px; color: #bcbcbc;">Post-meal Glucose <br><strong style="font-size: 16px; color: #fff;">{valueY}</strong> mg/dl</p>
    </li>
    </ul>`;
    series2.tooltip.getFillFromObject = false;
    series2.tooltip.background.fill = am4core.color("#484848");
    series2.tooltip.pointerOrientation = "vertical";
    series2.tooltip.background.filters.clear();

    var fillModifier2 = new am4core.LinearGradientModifier();
    fillModifier2.opacities = [0.2, 0];
    fillModifier2.offsets = [0, 0.5];
    fillModifier2.gradient.rotation = 90;
    series2.segments.template.fillModifier = fillModifier2;
    var series3 = chart.series.push(new am4charts.LineSeries());
    series3.dataFields.valueY = "value";
    series3.dataFields.dateX = "date";
    series3.stroke = am4core.color("#d2f241");
    series3.strokeWidth = 1;
    series3.tensionX = 0.8;
    series3.fill = am4core.color("#d2f241");
    series3.fillOpacity = 1;
    series3.bullets.push(new am4charts.CircleBullet());
    series3.data = glucData3;

    /* Add a single HTML-based tooltip to first series */
    series3.tooltipHTML = `<p style="font-size: 14px; margin-bottom: 4px; color: #bcbcbc;" > {dateX.formatDate('dd MMMM')} </p><ul>
    <li style="color: #d2f241; font-size: 20px;">
      <p style="font-size: 12px; margin-bottom: 10px; color: #bcbcbc;">Unknown<br><strong style="font-size: 16px; color: #fff;">{valueY}</strong> mg/dl</p>
    </li>
    </ul>`;
    series3.tooltip.getFillFromObject = false;
    series3.tooltip.background.fill = am4core.color("#484848");
    series3.tooltip.pointerOrientation = "vertical";
    series3.tooltip.background.filters.clear();

    var fillModifier3 = new am4core.LinearGradientModifier();
    fillModifier3.opacities = [0.2, 0];
    fillModifier3.offsets = [0, 0.5];
    fillModifier3.gradient.rotation = 90;
    series3.segments.template.fillModifier = fillModifier3;

    /* Create a cursor */
    chart.cursor = new am4charts.XYCursor();
}