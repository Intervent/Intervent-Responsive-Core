function CreatePAOverview(data1, data2){
    var chart = am4core.create("physical-activity-chartdiv", am4charts.XYChart);

    // Create axes
    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.startLocation = 0.5;
    dateAxis.endLocation = 0.4;
    dateAxis.renderer.grid.template.disabled = true;
    dateAxis.dateFormats.setKey("day", "dd\nMMM");
	dateAxis.renderer.labels.template.fill = am4core.color("#999999");
	dateAxis.renderer.labels.template.rotation = 45;
	dateAxis.renderer.labels.template.horizontalCenter = "right";
	dateAxis.renderer.labels.template.verticalCenter = "middle";

    // Create value axis
    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.min = 0;
    valueAxis.renderer.opposite = true;
    valueAxis.renderer.grid.template.disabled = true;
	valueAxis.renderer.labels.template.fill = am4core.color("#999999");
	valueAxis.renderer.line.strokeOpacity = 1;
	valueAxis.renderer.line.strokeWidth = 2;
	valueAxis.renderer.line.stroke = am4core.color("#fd6191");

    // Create series1
    var lineSeries = chart.series.push(new am4charts.LineSeries());
    lineSeries.dataFields.valueY = "value";
    lineSeries.dataFields.dateX = "date";
	lineSeries.stroke = am4core.color("#f46691");
    lineSeries.strokeWidth = 1;
    lineSeries.tensionX = 0.8;
    lineSeries.zIndex = 1;
	lineSeries.fill = am4core.color("#f46691");
    lineSeries.fillOpacity = 1;
    lineSeries.data = data1;

    /* Add a single HTML-based tooltip to first series */
    lineSeries.tooltipHTML = `<p style="font-size: 12px; margin-bottom: 4px; color: #bcbcbc">Weekly Goal Completed</p>
	    <p style="font-size: 16px; font-weight: bold; margin-bottom: 0; text-align: center; color: #fff" > {valueY} </p>
	    <p style="font-size: 16px; margin-bottom: 4px; text-align: center; color: #fff" > Steps </p>
	    <p style="font-size: 12px; margin-bottom: 4px; text-align: center; color: #bcbcbc" > {dateX.formatDate('dd MMMM')} </p>`;
	    lineSeries.tooltip.getFillFromObject = false;
	    lineSeries.tooltip.background.fill = am4core.color("#484848");
	    lineSeries.tooltip.pointerOrientation = "vertical";
	
	    var fillModifier = new am4core.LinearGradientModifier();
	    fillModifier.opacities = [0.6, 0];
	    fillModifier.offsets = [0, 1];
	    fillModifier.gradient.rotation = 90;
	    lineSeries.segments.template.fillModifier = fillModifier;
	
	    // Create series2
	   // var lineSeries2 = chart.series.push(new am4charts.LineSeries());
	   // lineSeries2.dataFields.valueY = "value";
	   // lineSeries2.dataFields.dateX = "date";
	   // lineSeries2.stroke = am4core.color("#ffd949");
	   // lineSeries2.strokeWidth = 1;
	   // lineSeries2.tensionX = 0.8;
	   // lineSeries2.fill = am4core.color("#ffd949");
	   // lineSeries2.fillOpacity = 1;
    //lineSeries2.data = data2;

    //var fillModifier = new am4core.LinearGradientModifier();
    //fillModifier.opacities = [0.4, 0];
    //fillModifier.offsets = [0, 0.5];
    //fillModifier.gradient.rotation = 90;
    //lineSeries2.segments.template.fillModifier = fillModifier;

    chart.cursor = new am4charts.XYCursor();

}