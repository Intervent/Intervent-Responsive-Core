function CreateThreeAMChart(chartName, start, startText, startColor, startYear, current, currentText, currentcolor, currentYear, goal1, goalText1, label, includeChar, goallabel) {
    $(document).ready(function () {
        //alert(chartName);
        /*am4core.useTheme(am4themes_animated);*/
        var sYear = startYear.split("|");
        startYear = sYear[0];
        var year = currentYear.split("|");
        currentYear = year[0];
        am4core.addLicense("CH39169069");
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
            "year": goallabel
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
        series.columns.template.events.on("ready", function (ev) {
            console.log("ready ");
        }, this);
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
    });
}

function CreateFourAMChart(chartName, minVal, minText, start, startText, startColor, startYear, current, currentText, currentColor, currentYear, goal1, goal1Text, label, goallabel) {
    $(document).ready(function () {
        /*am4core.useTheme(am4themes_animated);*/
        am4core.addLicense("CH39169069");
        var chart = am4core.create(chartName, am4charts.XYChart);
        var chartTitle = chart.titles.create();
        chartTitle.text = label;
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
            "color": "#20698a",
            "year": goallabel
        },
        {
            "risk": minText,
            "percentage": minVal,
            "color": "#67b7dc",
            "year": "Min"
        }];

        // Create axes

        var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
        categoryAxis.dataFields.category = "risk";
        categoryAxis.renderer.grid.template.location = 0;
        categoryAxis.renderer.labels.template.fontSize = 12;
        categoryAxis.renderer.minGridDistance = 20;
        categoryAxis.renderer.labels.template.maxWidth = 75;
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
        series.columns.template.events.on("ready", function (ev) {
            console.log("ready ");
        }, this);
        var bullet = series.bullets.push(new am4charts.LabelBullet());
        bullet.label.adapter.add("text", function (center, target) {
            var values = target.dataItem.values;
            return values.valueY.value > 0
                ? values.valueY.value
                : "";
        });
        bullet.label.verticalCenter = "bottom";
        bullet.label.dy = -5;
        bullet.label.fontSize = 10;
        chart.maskBullets = false;
        return chart;
    });
}

function CreateFiveAMChart(chartName, minVal, minText, start, startText, startColor, startYear, current, currentText, currentColor, currentYear, goal1, goal1Text, goal2, goal2Text, label, goallabel) {
        /*am4core.useTheme(am4themes_animated);*/
        am4core.addLicense("CH39169069");
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
            "year": goallabel
        },
        {
            "risk": goal2Text,
            "percentage": goal2,
            "color": "#20698a",
            "year": goallabel
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
        series.columns.template.events.on("ready", function (ev) {
            console.log("ready ");
        }, this);
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
        bullet.label.verticalCenter = "bottom";
        bullet.label.dy = -5;
        bullet.label.fontSize = 10;
        chart.maskBullets = false;
        return chart;
}