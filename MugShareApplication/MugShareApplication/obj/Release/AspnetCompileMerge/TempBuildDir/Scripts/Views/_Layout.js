/*--------------------------------------------------------------------------------------
    KEY PRESS FUNCTIONS:

        -numCheck()
        -noSpaces()

----------------------------------------------------------------------------------------*/
/*
    Function: numCheck

    Determines which key was pressed, and ignores any key press that was not a number,
    backspace, or tab.

    Parameters:

        e - key press event
*/
function numCheck(e) {
    var key;

    if (window.event) {
        key = e.keyCode;
    }
    else if (e.which) {
        key = e.which;
    }

    //if ((key >= 48 && key <= 57) || (key >= 96 && key <= 105) || key == 8 || key == 9)
    if ((key < 48 || key > 57) && key != 8 && key != 9) {
        e.preventDefault();
    }
}

/*
    Function: noSpaces

    Determines which key was pressed, and ignores any spacebar presses detected.

    Parameters:

        e - key press event
*/
function noSpaces(e) {
    var key;

    if (window.event) {
        key = e.keyCode;
    }
    else if (e.which) {
        key = e.which;
    }

    if (key == 32) {
        e.preventDefault();
    }
}

/*--------------------------------------------------------------------------------------
    CHART FUNCTIONS:

        -DrawBarChart()
        -DrawLineChart()

----------------------------------------------------------------------------------------*/
/*
    Function: DrawBarChart

    Draws a bar chart based on parameters passed in function call.

    Parameters:

        chartID - ID of canvas tag in HTML page
        data - data used to determine the height of each bar in the chart
        title - title of the chart
        x_chartLabels - labels for each bar on the x-axis
        y_chartLabel - label for the y-axis
        chartColors - color of each bar in chart
*/
function DrawBarChart(chartID, data, title, x_chartGridValues, x_chartLabel, y_chartLabel, chartColors) {
    var chart = document.getElementById(chartID);
    var myChart = new Chart(chart, {
        type: 'bar',
        data: {
            labels: x_chartGridValues,
            datasets: [{
                data: data,
                backgroundColor: chartColors,
                borderColor: "black",
                borderWidth: 1
            }]
        },
        options: {
            legend: {
                display: false,
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    },
                    scaleLabel: {
                        display: true,
                        labelString: y_chartLabel
                    }
                }],
                xAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: x_chartLabel
                    }
                }]
            },
            title: {
                display: true,
                text: title
            }
        }
    });
}

/*
    Function: DrawLineChart

    Draws a line chart based on parameters passed in function call.

    Parameters:

        chartID - ID of canvas tag in HTML page
        data - data used to plot the line graph
        title - title of the chart
        x_chartLabels - labels for each point on the x-axis
        y_chartLabel - label for the y-axis
*/
function DrawLineChart(chartID, data, title, x_chartLabels, y_chartLabel) {
    var chart = document.getElementById(chartID);
    var myChart = new Chart(chart, {
        type: 'line',
        data: {
            labels: x_chartLabels,
            datasets: [{
                data: data,
                borderWidth: 1,
                borderColor: 'rgba(0, 0, 225, 1)',
                pointBackgroundColor: 'rgba(0, 0, 176, 1)',
                lineTension: 0,
                spanGaps: false,
                fill: false
            }]
        },
        options: {
            legend: {
                display: false,
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    },
                    scaleLabel: {
                        display: true,
                        labelString: y_chartLabel
                    }
                }]
            },
            title: {
                display: true,
                text: title
            }
        }
    });
}