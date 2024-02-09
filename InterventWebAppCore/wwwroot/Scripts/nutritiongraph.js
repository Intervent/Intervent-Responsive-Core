function CreateNutritionOverview(nutritionData){
    // Create chart instance
    var chart = am4core.create("nutrition-chartdiv", am4charts.XYChart);

    // Add data
    chart.data = nutritionData;

    // Create axes
    var categoryAxis = chart.yAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "year";
    categoryAxis.numberFormatter.numberFormat = "#";
    categoryAxis.renderer.inversed = true;
    categoryAxis.renderer.labels.template.fill = am4core.color("#999999");

    var  valueAxis = chart.xAxes.push(new am4charts.ValueAxis()); 
    valueAxis.renderer.grid.template.disabled = true;
    valueAxis.renderer.labels.template.fill = am4core.color("#999999");

    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.dataFields.valueX = "income";
    series.dataFields.categoryY = "year";
    series.name = "Income";
    series.columns.template.propertyFields.fill = "color";
    series.columns.template.height = am4core.percent(10);

    /* Create a cursor */
    chart.cursor = new am4charts.XYCursor();
}
