$(function() {
    $(document).ready(function() {
        var applyButton = document.getElementById("applyButton");
        var filterButton = document.getElementById("filterButton");
        var filterClearButton = document.getElementById("filterClearButton");

        var targetDate = new Date(); // gets today's date
        var addDays = -60; //number of days to add
        var newDate = kendo.date.addDays(targetDate, addDays);

        var baseApiUrl = "https://localhost:44376/";
        var dailyApiUrl = `${baseApiUrl}daily`;
        var hourlyApiUrl = `${baseApiUrl}hourly`;

        $(".configuration").hide();

        $("#dateTimePickerFrom").kendoDateTimePicker({
            value: new Date("2020-03-11 00:00:00"),
            format: "yyyy-MM-dd HH:mm:ss"
        });

        $("#dateTimePickerTo").kendoDateTimePicker({
            value: new Date("2020-03-12 00:00:00"),
            format: "yyyy-MM-dd HH:mm:ss"
        });

        // Get the data from the API based on the added from to dates
        var datePickerFrom = $("#dateTimePickerFrom").val();
        var datePickerTo = $("#dateTimePickerTo").val();

        apiDailyWithDate = `${dailyApiUrl}/?dateTimeFrom="${datePickerFrom}"&datetimeTo="${datePickerTo}"`;
        apiHourlyWithDate = `${hourlyApiUrl}/?dateTimeFrom="${datePickerFrom}"&datetimeTo="${datePickerTo}"`;

        $("#switch").kendoSwitch({
            checked: true,
            messages: {
                checked: "DAILY",
                unchecked: "HOURLY"
            },
            change: function(e) {
                var switchInstance = $("#switch").data("kendoSwitch");
                var grid = $("#TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER_Grid").data("kendoGrid");

                if (switchInstance.check()) {
                    var datePickerFrom = $("#dateTimePickerFrom").val();
                    var datePickerTo = $("#dateTimePickerTo").val();
                    apiDailyWithDate = `${dailyApiUrl}/?dateTimeFrom="${datePickerFrom}"&datetimeTo="${datePickerTo}"`;
                    $.getJSON(apiDailyWithDate, function(data) {
                        grid.setDataSource(data);
                    });
                } else {
                    var datePickerFrom = $("#dateTimePickerFrom").val();
                    var datePickerTo = $("#dateTimePickerTo").val();
                    apiHourlyWithDate = `${hourlyApiUrl}/?dateTimeFrom="${datePickerFrom}"&datetimeTo="${datePickerTo}"`;
                    $.getJSON(apiHourlyWithDate, function(data) {
                        grid.setDataSource(data);
                    });
                }
            }
        });

        filterButton.addEventListener("click", function() {
            $(".configuration").slideToggle(1000);
        });

        applyButton.addEventListener("click", function() {
            var switchInstance = $("#switch").data("kendoSwitch");
            var grid = $("#TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER_Grid").data("kendoGrid");
            if (switchInstance.check()) {
                var datePickerFrom = $("#dateTimePickerFrom").val();
                var datePickerTo = $("#dateTimePickerTo").val();
                apiDailyWithDate = `${dailyApiUrl}/?dateTimeFrom="${datePickerFrom}"&datetimeTo="${datePickerTo}"`;
                $.getJSON(apiDailyWithDate, function(data) {
                    grid.setDataSource(data);
                });
            } else {
                var datePickerFrom = $("#dateTimePickerFrom").val();
                var datePickerTo = $("#dateTimePickerTo").val();
                apiHourlyWithDate = `${hourlyApiUrl}/?dateTimeFrom="${datePickerFrom}"&datetimeTo="${datePickerTo}"`;
                $.getJSON(apiHourlyWithDate, function(data) {
                    grid.setDataSource(data);
                });
            };
        });

        filterClearButton.addEventListener("click", function() {
            // Get the data from the API based on the added from to dates
            $("#dateTimePickerFrom").data("kendoDateTimePicker").value(new Date("2020-03-11 00:00:00"));
            $("#dateTimePickerTo").data("kendoDateTimePicker").value(new Date("2020-03-12 00:00:00"));
        });

        // Start - Display all the KPI Levels Chart --------------------------------------------------------------
        $("#KPI_Chart").kendoChart({
            dataSource: {
                data: []
            },
            title: {
                text: "KPI Levels",
                font: "25px sans-serif",
                color: "#4A4E74"
            },
            series: [{
                type: "line",
                color: "#6989D2",
                aggregate: "avg",
                field: "maxRXLevel",
                categoryField: "date"
            }],
            categoryAxis: {
                baseUnit: "days",
                majorGridLines: {
                    visible: false
                },
                labels: {
                    rotation: -45,
                    format: "{0:dd/MM/yyyy hh:mm:ss}"
                }
            },
            valueAxis: {
                title: {
                    text: "maxRXLevel",
                    font: "25px sans-serif",
                    color: "#4A4E74"
                },
                line: {
                    visible: false
                }
            },
            tooltip: {
                visible: true,
                template: "#= value #, Link: #= dataItem.link #"
            }
        });

        $(".configuration").bind("change", function() {
            var chart = $("#KPI_Chart").data("kendoChart"),
                series = chart.options.series,
                axisTitle = chart.options.valueAxis,
                baseKpiUnitInputs = $("input:radio[name=baseKpiUnit]"),
                aggregateInputs = $("input:radio[name=aggregate]");

            for (var i = 0, length = series.length; i < length; i++) {
                series[i].aggregate = aggregateInputs.filter(":checked").val();
                series[i].field = baseKpiUnitInputs.filter(":checked").val();
            };
            axisTitle.title.text = baseKpiUnitInputs.filter(":checked").val();

            chart.refresh();
        });

        $("#config-switch").kendoSwitch({
            checked: true,
            messages: {
                checked: "DAYS",
                unchecked: "HOURS"
            },
            change: function(e) {
                var chart = $("#KPI_Chart").data("kendoChart"),
                    categoryAxis = chart.options.categoryAxis;

                var switchConfig = $("#config-switch").data("kendoSwitch");

                if (switchConfig.check()) {
                    categoryAxis.baseUnit = "days";
                } else {
                    categoryAxis.baseUnit = "hours";
                }

                chart.refresh();
            }
        });
        // End - Display all the KPI Levels Chart --------------------------------------------------------------

        $("#TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER_Grid").kendoGrid({
            // dataSource: gridDataSource,
            dataSource: {
                transport: {
                    read: {
                        url: apiDailyWithDate,
                        dataType: "json"
                    }
                }
            },
            dataBound: function(e) {
                var grid = e.sender,
                    kpiChart = $("#KPI_Chart").data("kendoChart");

                kpiChart.dataSource.data(grid.dataSource.data());
            },
            pageable: {
                pageSize: 10
            },
            height: 465,
            sortable: true,
            filterable: true,
            columns: [{
                field: "date",
                title: "Date",
                format: "{0:dd/MM/yyyy HH:mm:ss}",
                type: "date"
            }, {
                field: "link",
                title: "Link",
                type: "string"
            }, {
                field: "slot",
                title: "Slot",
                type: "string"
            }, {
                field: "neAlias",
                title: "NeAlias",
                type: "string"
            }, {
                field: "neType",
                title: "NeType",
                type: "string"
            }, {
                field: "maxRXLevel",
                title: "RX Level",
                type: "number"
            }, {
                field: "maxTXLevel",
                title: "TX Level",
                type: "number"
            }, {
                field: "rslDeviation",
                title: "RSL Deviation",
                type: "number"
            }]
        });
    })
});