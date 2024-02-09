function CreateBPOverview(bpData){
    var chart = am4core.create("bp-chartdiv", am4charts.XYChart);

    chart.data = bpData;

    // Create axes
    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.minGridDistance = 20;
    dateAxis.startLocation = 0;
    dateAxis.endLocation = 1;
    dateAxis.renderer.grid.template.disabled = true;
    dateAxis.dateFormats.setKey("day", "dd\nMMM");
	dateAxis.renderer.labels.template.fill = am4core.color("#999999");
	dateAxis.renderer.labels.template.rotation = 68;
	dateAxis.renderer.labels.template.horizontalCenter = "right";
	dateAxis.renderer.labels.template.verticalCenter = "middle";

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.opposite = true;
    valueAxis.renderer.labels.template.fill = am4core.color("#999999");

    var series = chart.series.push(new am4charts.ColumnSeries());
    series.dataFields.dateX = "date";
    series.dataFields.openValueY = "open";
    series.dataFields.valueY = "close";
    series.sequencedInterpolation = true;
    series.fillOpacity = 0;
    series.strokeOpacity = 1;
    series.columns.template.width = 0.01;

    /* Add a single HTML-based tooltip to first series */
    series.tooltipHTML = `<p style="font-size: 14px; margin-bottom: 4px; color: #bcbcbc;" > {dateX.formatDate('dd MMMM')} </p>
    <ul>
    <li style="color: #a367dc; font-size: 20px;">
	    <p style="font-size: 12px; margin-bottom: 10px; color: #bcbcbc;">Systolic Blood Pressure <br><strong style="font-size: 16px; color: #fff;">{valueY}</strong> mmHg</p>
    </li>
    <li style="color: #67b7dc; font-size: 20px;"> 
	    <p style="font-size: 12px; margin-bottom: 0; color: #bcbcbc;">Diastolic Blood Pressure <br><strong style="font-size: 16px; color: #fff;">{openValueY}</strong> mmHg</p>
    </li>
    </ul>`;
    series.tooltip.getFillFromObject = false;
    series.tooltip.background.fill = am4core.color("#484848");
    series.tooltip.pointerOrientation = "vertical";
    series.tooltip.background.filters.clear();

    var openBullet = series.bullets.create(am4charts.CircleBullet);
    openBullet.locationY = 1;

    var closeBullet = series.bullets.create(am4charts.CircleBullet);

    closeBullet.fill = chart.colors.getIndex(4);
    closeBullet.stroke = closeBullet.fill;

    //chart.scrollbarX = new am4core.Scrollbar().hide();
    //chart.scrollbarY = new am4core.Scrollbar().hide();


    /* Create a cursor */
    chart.cursor = new am4charts.XYCursor();
}