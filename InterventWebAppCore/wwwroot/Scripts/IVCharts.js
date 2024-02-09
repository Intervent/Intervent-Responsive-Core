function CreateAMChart(chartName, current, currentText, currentcolor, currentYear, goal1, goalText1, label, includeChar) {
    am4core.addLicense("CH39169069");
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true; 
    var year = currentYear.split("|");
    currentYear = year[0];
    var chart = am4core.create(chartName, am4charts.XYChart);
    var chartTitle = chart.titles.create();
    chartTitle.text = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;
    chart.data = [{
        "risk": currentText,
        "percentage": current,
        "color": currentcolor,
        "year": currentYear,
        "moreInfo": typeof year[1] != 'undefined' ? year[1] : ""
    }, {
        "risk": goalText1,
        "percentage": goal1,
        "color": "#20698a",
        "year": goalText1
    }];

    // Create axes

    var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "risk";
    categoryAxis.renderer.grid.template.location = 0;
    categoryAxis.renderer.labels.template.fontSize = 12;
    categoryAxis.renderer.minGridDistance = 20;


    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.labels.template.fontSize = 12;

    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.columns.template.width = am4core.percent(35);
    series.dataFields.valueY = "percentage";
    series.dataFields.categoryX = "risk";
    series.columns.template.propertyFields.fill = "color";
    series.columns.template.propertyFields.stroke = "color";
    series.columns.template.tooltipText = "{year}: [bold]{valueY}{moreInfo}[/]";
    series.showOnInit = false;
    var bullet = series.bullets.push(new am4charts.LabelBullet());
    bullet.label.adapter.add("text", function (center, target) {
        var values = target.dataItem.values;
        return values.valueY.value > 0
            ? values.valueY.value + includeChar
            : "";
    });
    bullet.label.verticalCenter = "bottom";
    bullet.label.dy = -5;
    bullet.label.fontSize = 10;
    chart.maskBullets = false;
    return chart;
}

function CreateThreeChartAMChart(chartName, start, startText, startColor, startYear, current, currentText, currentcolor, currentYear, goal1, goalText1, label, includeChar) {
    am4core.addLicense("CH39169069");
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true;
    var sYear = startYear.split("|");
    startYear = sYear[0];
    var year = currentYear.split("|");
    currentYear = year[0];
    var chart = am4core.create(chartName, am4charts.XYChart);
    var chartTitle = chart.titles.create();
    chartTitle.text = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;
    chart.data = [{
        "risk": startText,
        "percentage": start,
        "color": startColor,
        "year": startYear,
        "moreInfo": typeof sYear[1] != 'undefined' ? sYear[1] : ""
    }, {
        "risk": currentText,
        "percentage": current,
        "color": currentcolor,
        "year": currentYear,
        "moreInfo": typeof year[1] != 'undefined' ? year[1] : ""
    }, {
        "risk": goalText1,
        "percentage": goal1,
        "color": "#20698a",
        "year": "Goal"
    }];

    // Create axes

    var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "risk";
    categoryAxis.renderer.grid.template.location = 0;
    categoryAxis.renderer.labels.template.fontSize = 12;
    categoryAxis.renderer.minGridDistance = 20;
    categoryAxis.renderer.labels.template.maxWidth = 65;
    categoryAxis.events.on("sizechanged", function (ev) {
        var axis = ev.target;
        var cellWidth = axis.pixelWidth / (axis.endIndex - axis.startIndex);
        if (cellWidth < axis.renderer.labels.template.maxWidth) {
            axis.renderer.labels.template.rotation = -45;
            axis.renderer.labels.template.horizontalCenter = "right";
            axis.renderer.labels.template.verticalCenter = "middle";
            axis.renderer.labels.template.wrap = false;
        }
        else {
            axis.renderer.labels.template.rotation = 0;
            axis.renderer.labels.template.horizontalCenter = "middle";
            axis.renderer.labels.template.verticalCenter = "top";
            axis.renderer.labels.template.wrap = true;
            axis.renderer.labels.template.maxWidth = cellWidth;
        }
    });

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.labels.template.fontSize = 12;

    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.columns.template.width = am4core.percent(35);
    series.dataFields.valueY = "percentage";
    series.dataFields.categoryX = "risk";
    series.columns.template.propertyFields.fill = "color";
    series.columns.template.propertyFields.stroke = "color";
    series.columns.template.tooltipText = "{year}: [bold]{valueY}{moreInfo}[/]";
    series.showOnInit = false;
    var bullet = series.bullets.push(new am4charts.LabelBullet());
    bullet.label.adapter.add("text", function (center, target) {
        var values = target.dataItem.values;
        return values.valueY.value > 0
            ? values.valueY.value + includeChar
            : "";
    });
    bullet.label.verticalCenter = "bottom";
    bullet.label.dy = -5;
    bullet.label.fontSize = 10;
    chart.maskBullets = false;
    return chart;
}

function CreateThreeWTwoGoalChart(chartName, minVal, minText, current, currentText, currentcolor, currentYear, goal1, goalText1, label, includeChar) {
    am4core.addLicense("CH39169069");
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true;
    var chart = am4core.create(chartName, am4charts.XYChart);
    var chartTitle = chart.titles.create();
    chartTitle.text = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;
    chart.data = [{
        "risk": currentText,
        "percentage": current,
        "color": currentcolor,
        "year": currentYear
    }, {
        "risk": goalText1,
        "percentage": goal1,
        "color": "#20698a",
        "year": "Goal"
    },
    {
        "risk": minText,
        "percentage": minVal,
        "color": "#20698a",
        "year": minText
    }];

    // Create axes

    var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "risk";
    categoryAxis.renderer.grid.template.location = 0;
    categoryAxis.renderer.labels.template.fontSize = 12;
    categoryAxis.renderer.minGridDistance = 20;
    categoryAxis.renderer.labels.template.maxWidth = 65;
    categoryAxis.events.on("sizechanged", function (ev) {
        var axis = ev.target;
        var cellWidth = axis.pixelWidth / (axis.endIndex - axis.startIndex);
        if (cellWidth < axis.renderer.labels.template.maxWidth) {
            axis.renderer.labels.template.rotation = -45;
            axis.renderer.labels.template.horizontalCenter = "right";
            axis.renderer.labels.template.verticalCenter = "middle";
            axis.renderer.labels.template.wrap = false;
        }
        else {
            axis.renderer.labels.template.rotation = 0;
            axis.renderer.labels.template.horizontalCenter = "middle";
            axis.renderer.labels.template.verticalCenter = "top";
            axis.renderer.labels.template.wrap = true;
            axis.renderer.labels.template.maxWidth = cellWidth;
        }
    });

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.labels.template.fontSize = 12;

    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.columns.template.width = am4core.percent(35);
    series.dataFields.valueY = "percentage";
    series.dataFields.categoryX = "risk";
    series.columns.template.propertyFields.fill = "color";
    series.columns.template.propertyFields.stroke = "color";
    series.columns.template.tooltipText = "{year}: [bold]{valueY}[/]";
    series.showOnInit = false;
    var bullet = series.bullets.push(new am4charts.LabelBullet());
    bullet.label.adapter.add("text", function (center, target) {
        var values = target.dataItem.values;
        return values.valueY.value > 0
            ? values.valueY.value + includeChar
            : "";
    });
    bullet.label.verticalCenter = "bottom";
    bullet.label.dy = -5;
    bullet.label.fontSize = 10;
    chart.maskBullets = false;
    return chart;
}

function CreateFourChartAMChart(chartName, minVal, minText, value, valueText, valueColor, currentYear, goal1, goal1Text, goal2, goal2Text, label) {
    am4core.addLicense("CH39169069");
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true;
    var chart = am4core.create(chartName, am4charts.XYChart);
    var chartTitle = chart.titles.create();
    chartTitle.html = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;
    chart.data = [{
        "risk": valueText,
        "percentage": value,
        "color": valueColor,
        "year": currentYear
    },
    {
        "risk": goal1Text,
        "percentage": goal1,
        "color": "#67b7dc",
        "year": goal1Text
    },
    {
        "risk": goal2Text,
        "percentage": goal2,
        "color": "#20698a",
        "year": goal2Text
    },
    {
        "risk": minText,
        "percentage": minVal,
        "color": "#20698a",
        "year": minText
    }];

    // Create axes

    var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "risk";
    categoryAxis.renderer.grid.template.location = 0;
    categoryAxis.renderer.labels.template.fontSize = 12;
    categoryAxis.renderer.minGridDistance = 20;
    categoryAxis.renderer.labels.template.maxWidth = 65;
    categoryAxis.events.on("sizechanged", function (ev) {
        var axis = ev.target;
        var cellWidth = axis.pixelWidth / (axis.endIndex - axis.startIndex);
        if (cellWidth < axis.renderer.labels.template.maxWidth) {
            axis.renderer.labels.template.rotation = -45;
            axis.renderer.labels.template.horizontalCenter = "right";
            axis.renderer.labels.template.verticalCenter = "middle";
            axis.renderer.labels.template.wrap = false;
        }
        else {
            axis.renderer.labels.template.rotation = 0;
            axis.renderer.labels.template.horizontalCenter = "middle";
            axis.renderer.labels.template.verticalCenter = "top";
            axis.renderer.labels.template.wrap = true;
            axis.renderer.labels.template.maxWidth = cellWidth;
        }
    });

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.labels.template.fontSize = 12;
    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.columns.template.width = am4core.percent(35);
    series.dataFields.valueY = "percentage";
    series.dataFields.categoryX = "risk";
    series.columns.template.propertyFields.fill = "color";
    series.columns.template.propertyFields.stroke = "color";
    series.columns.template.tooltipText = "{year}: [bold]{valueY}[/]";
    series.showOnInit = false;
    var bullet = series.bullets.push(new am4charts.LabelBullet());
    bullet.label.verticalCenter = "bottom";
    bullet.label.dy = -5;
    bullet.label.fontSize = 10;
    bullet.label.adapter.add("text", function (center, target) {
        var values = target.dataItem.values;
        return values.valueY.value > 0
            ? values.valueY.value
            : "";
    });
    var label1 = categoryAxis.renderer.labels.template;
    label1.wrap = true;
    label1.maxWidth = 120;
    chart.maskBullets = false;
    return chart;
}

function CreateCACChart(chartName, value, text, label) {
    am4core.addLicense("CH39169069");
    var finalvalue, endvalue = 700;
    if (value <= 700) {
        finalvalue = value;
    }
    else {
        finalvalue = 700;
    }

    // Themes end
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true;
    // create chart
    var chart = am4core.create(chartName, am4charts.GaugeChart);
    var chartTitle = chart.titles.create();
    chartTitle.text = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;
    chart.hiddenState.properties.opacity = 0; // this makes initial fade in effect

    chart.innerRadius = -15;

    var axis = chart.xAxes.push(new am4charts.ValueAxis());
    axis.renderer.labels.template.fontSize = 12;
    axis.min = 1;
    axis.max = endvalue;
    axis.strictMinMax = true;
    axis.renderer.labels.template.adapter.add("text", function (text, target) {
        return "";
    });
    axis.renderer.grid.template.stroke = new am4core.InterfaceColorSet().getFor("background");
    axis.renderer.grid.template.strokeOpacity = 1;

    var colorSet = new am4core.ColorSet();

    var range0 = axis.axisRanges.create();
    range0.value = 1;
    range0.endValue = 10;
    range0.axisFill.fillOpacity = 1;
    range0.axisFill.fill = "green";
    range0.axisFill.zIndex = - 1;

    var range1 = axis.axisRanges.create();
    range1.value = 10;
    range1.endValue = 100;
    range1.axisFill.fillOpacity = 1;
    range1.axisFill.fill = "yellow";
    range1.axisFill.zIndex = -1;

    var range2 = axis.axisRanges.create();
    range2.value = 100;
    range2.endValue = 400;
    range2.axisFill.fillOpacity = 1;
    range2.axisFill.fill = "orange";
    range2.axisFill.zIndex = -1;

    var range3 = axis.axisRanges.create();
    range3.value = 400;
    range3.endValue = endvalue;
    range3.axisFill.fillOpacity = 1;
    range3.axisFill.fill = "#ff3939";
    range3.axisFill.zIndex = -1;

    var hand = chart.hands.push(new am4charts.ClockHand());
    hand.axis = axis;
    hand.innerRadius = am4core.percent(20);
    hand.startWidth = 10;
    hand.pin.disabled = true;

    hand.showValue(finalvalue);

    var label1 = chart.chartContainer.createChild(am4core.Label);
    label1.text = value;
    label1.align = "center";

    return chart;
}

function CreateFiveChartAMChart(chartName, minVal, minText, start, startText, startColor, startYear, current, currentText, currentColor, currentYear, goal1, goal1Text, goal2, goal2Text, label) {
    am4core.addLicense("CH39169069");
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true;
    var chart = am4core.create(chartName, am4charts.XYChart);
    var chartTitle = chart.titles.create();
    chartTitle.html = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;
    chart.data = [{
        "risk": startText,
        "percentage": start,
        "color": startColor,
        "year": startYear
    }, {
        "risk": currentText,
        "percentage": current,
        "color": currentColor,
        "year": currentYear
    },
    {
        "risk": goal1Text,
        "percentage": goal1,
        "color": "#67b7dc",
        "year": "Goal"
    },
    {
        "risk": goal2Text,
        "percentage": goal2,
        "color": "#20698a",
        "year": "Goal"
    },
    {
        "risk": minText,
        "percentage": minVal,
        "color": "#67b7dc",
        "year": "Min"
    }];
    var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "risk";
    categoryAxis.renderer.grid.template.location = 0;
    categoryAxis.renderer.labels.template.fontSize = 12;
    categoryAxis.renderer.minGridDistance = 20;
    categoryAxis.renderer.labels.template.maxWidth = 65;
    categoryAxis.events.on("sizechanged", function (ev) {
        var axis = ev.target;
        var cellWidth = axis.pixelWidth / (axis.endIndex - axis.startIndex);
        if (cellWidth < axis.renderer.labels.template.maxWidth) {
            axis.renderer.labels.template.rotation = -45;
            axis.renderer.labels.template.horizontalCenter = "right";
            axis.renderer.labels.template.verticalCenter = "middle";
            axis.renderer.labels.template.wrap = false;
        }
        else {
            axis.renderer.labels.template.rotation = 0;
            axis.renderer.labels.template.horizontalCenter = "middle";
            axis.renderer.labels.template.verticalCenter = "top";
            axis.renderer.labels.template.wrap = true;
            axis.renderer.labels.template.maxWidth = cellWidth;
        }
    });


    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.renderer.labels.template.fontSize = 12;
    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.columns.template.width = am4core.percent(35);
    series.dataFields.valueY = "percentage";
    series.dataFields.categoryX = "risk";
    series.columns.template.propertyFields.fill = "color";
    series.columns.template.propertyFields.stroke = "color";
    series.columns.template.tooltipText = "{year}: [bold]{valueY}[/]";
    series.showOnInit = false;
    var bullet = series.bullets.push(new am4charts.LabelBullet());
    bullet.label.verticalCenter = "bottom";
    bullet.label.dy = -5;
    bullet.label.fontSize = 10;
    bullet.label.adapter.add("text", function (center, target) {
        var values = target.dataItem.values;
        return values.valueY.value > 0
            ? values.valueY.value
            : "";
    });
    chart.maskBullets = false;
    return chart;
}

function CreatePAIChart(chartName, value, text, label) {
    var finalvalue, endvalue = 15.00;
    if (value <= 15) {
        finalvalue = value;
    }
    else {
        finalvalue = 15;
    }
    am4core.addLicense("CH39169069");
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true;
    // create chart
    var chart = am4core.create(chartName, am4charts.GaugeChart);
    chart.hiddenState.properties.opacity = 0; // this makes initial fade in effect
    var chartTitle = chart.titles.create();
    chartTitle.text = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;
    chart.innerRadius = -15;

    var axis = chart.xAxes.push(new am4charts.ValueAxis());
    axis.renderer.labels.template.fontSize = 12;
    axis.min = 0;
    axis.max = endvalue;
    axis.strictMinMax = true;
    axis.renderer.grid.template.stroke = new am4core.InterfaceColorSet().getFor("background");
    axis.renderer.grid.template.strokeOpacity = 1;

    var range0 = axis.axisRanges.create();
    range0.value = 0;
    range0.endValue = 1.5;
    range0.axisFill.fillOpacity = 1;
    range0.axisFill.fill = "#ff3939";
    range0.axisFill.zIndex = - 1;

    var range1 = axis.axisRanges.create();
    range1.value = 1.5;
    range1.endValue = 3.75;
    range1.axisFill.fillOpacity = 1;
    range1.axisFill.fill = "yellow";
    range1.axisFill.zIndex = -1;

    var range2 = axis.axisRanges.create();
    range2.value = 3.76;
    range2.endValue = endvalue;
    range2.axisFill.fillOpacity = 1;
    range2.axisFill.fill = "orange";
    range2.axisFill.zIndex = -1;

    var hand = chart.hands.push(new am4charts.ClockHand());
    hand.axis = axis;
    hand.innerRadius = am4core.percent(20);
    hand.startWidth = 10;
    hand.pin.disabled = true;

    hand.showValue(finalvalue);
    return chart;
}

function CreateADAChart(chartName, value, text, label) {
    var finalvalue, endvalue = 10.00;
    if (value <= 10) {
        finalvalue = value;
    }
    else {
        finalvalue = 10;
    }
    am4core.addLicense("CH39169069");
    //am4core.options.queue = true;
    //am4core.options.onlyShowOnViewport = true;
    // create chart
    var chart = am4core.create(chartName, am4charts.GaugeChart);
    chart.hiddenState.properties.opacity = 0; // this makes initial fade in effect
    var chartTitle = chart.titles.create();
    chartTitle.text = label;
    chartTitle.fontSize = 15;
    chartTitle.marginBottom = 30;

    chart.innerRadius = -15;

    var axis = chart.xAxes.push(new am4charts.ValueAxis());
    axis.renderer.labels.template.fontSize = 12;
    axis.min = 0;
    axis.max = endvalue;
    axis.strictMinMax = true;
    axis.renderer.grid.template.stroke = new am4core.InterfaceColorSet().getFor("background");
    axis.renderer.grid.template.strokeOpacity = 1;

    var range0 = axis.axisRanges.create();
    range0.value = 0;
    range0.endValue = 3.1;
    range0.axisFill.fillOpacity = 1;
    range0.axisFill.fill = "green";
    range0.axisFill.zIndex = - 1;

    var range1 = axis.axisRanges.create();
    range1.value = 3.1;
    range1.endValue = 5;
    range1.axisFill.fillOpacity = 1;
    range1.axisFill.fill = "yellow";
    range1.axisFill.zIndex = -1;

    var range2 = axis.axisRanges.create();
    range2.value = 5;
    range2.endValue = endvalue;
    range2.axisFill.fillOpacity = 1;
    range2.axisFill.fill = "red";
    range2.axisFill.zIndex = -1;

    var hand = chart.hands.push(new am4charts.ClockHand());
    hand.axis = axis;
    hand.innerRadius = am4core.percent(20);
    hand.startWidth = 10;
    hand.pin.disabled = true;

    hand.showValue(finalvalue);
    return chart;
}

function CreateGaugeChart(chartName, value) {
    am4core.addLicense("CH39169069");
    var chart = am4core.create(chartName, am4charts.GaugeChart);
    chart.hiddenState.properties.opacity = 0; // this makes initial fade in effect

    chart.innerRadius = -25;

    var axis = chart.xAxes.push(new am4charts.ValueAxis());
    axis.min = 0;
    axis.max = 100;
    axis.strictMinMax = true;
    axis.renderer.grid.template.stroke = new am4core.InterfaceColorSet().getFor("background");
    axis.renderer.grid.template.strokeOpacity = 0.3;

    var colorSet = new am4core.ColorSet();

    var range0 = axis.axisRanges.create();
    range0.value = 0;
    range0.endValue = 80;
    range0.axisFill.fillOpacity = 1;
    range0.axisFill.fill = am4core.color("#c34542");
    range0.axisFill.zIndex = - 1;

    var range01 = axis.axisRanges.create();
    range01.value = 80;
    range01.endValue = 90;
    range01.axisFill.fillOpacity = 1;
    range01.axisFill.fill = am4core.color("#e1aa02");
    range01.axisFill.zIndex = - 1;

    var range02 = axis.axisRanges.create();
    range02.value = 90;
    range02.endValue = 100;
    range02.axisFill.fillOpacity = 1;
    range02.axisFill.fill = am4core.color("#12bb78");
    range02.axisFill.zIndex = - 1;

    var label = chart.radarContainer.createChild(am4core.Label);
    label.isMeasured = false;
    label.fontSize = 20;
    label.x = am4core.percent(50);
    label.y = am4core.percent(100);
    label.horizontalCenter = "middle";
    label.verticalCenter = "bottom";
    label.text = value;

    var hand = chart.hands.push(new am4charts.ClockHand());
    hand.value = value;
    hand.axis = axis;
    hand.innerRadius = am4core.percent(30);
    hand.startWidth = 10;
    hand.pin.disabled = true;
    return chart;
}