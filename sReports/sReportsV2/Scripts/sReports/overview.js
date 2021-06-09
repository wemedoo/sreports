var max = 0;
var steps = 10;
var chartData = {};

function respondCanvas() {
    var c = $('#documentsPerThesaurus');
    var ctx = c.get(0).getContext("2d");
    var container = c.parent().parent();
    var $container = $(container);
    c.attr('width', $container.width()); //max width
    c.attr('height', $container.height() - 60); //max height


    
    //Call a function to redraw other content (texts, images etc)
    var chart = new Chart(ctx, chartData);
   
}

Chart.scaleService.updateScaleDefaults('category', {
    ticks: {
        callback: function (tick) {
            var characterLimit = 20;
            if (tick && tick.length >= 20) {
                return tick.slice(0, tick.length).substring(0, characterLimit - 1).trim() + '...';
            }
            return tick;
        }
    }
});

function getDocumentChartData() {
    var c = $('#documentsPerThesaurus');
    var ctx = c.get(0).getContext("2d");
    gradient = ctx.createLinearGradient(0, 0, 0, 450);
    gradient.addColorStop(0.5, '#34b5bf');
    gradient.addColorStop(1, '#1c94a3');


    $.ajax({
        url: '/FormInstance/GetDocumentsPerDomain',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            chartData = {
                type: 'bar',
                data:
                {
                    labels: response.map(x => x.Domain),
                    datasets: [
                        {
                            label: '# Documents',//#2a796b,
                            fillColor: 'white',
                            backgroundColor: gradient,
                            data: response.map(x => x.Count)

                        }
                    ]
                },
                options: {
                    scales: {
                        xAxes: [{
                            gridLines: {
                                display: true,
                                color:'transparent',
                                drawBorder: true,
                                zeroLineColor:'lightgray'
                            },
                            ticks: {
                                beginAtZero: true,
                                fontColor: "black",
                                fontSize: 13
                            },
                            display: true
                        }],
                        yAxes: [{
                            gridLines: {
                                display: true,
                                color: '#e0e0e0',
                                drawBorder: true,
                                zeroLineColor: 'lightgray'
                            },
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                    },
                    ticks: {
                        beginAtZero: true,
                        fontSize: 15,
                        fontColor: 'lightgrey',
                        maxTicksLimit: 5,
                        padding: 25
                    },
                    tooltips: {
                        callbacks: {
                            title: function (tooltipItems, data) {
                                let label = data.labels[tooltipItems[0].index];
                                var item = response[tooltipItems[0].index];
                                if (item) {
                                    label = `${label} (${item.Domain})`;
                                }
                                return label;
                            },
                            label: function (tooltipItem, data) {
                                var label = data.datasets[tooltipItem.datasetIndex].label || '';

                                if (label) {
                                    label = `${label}: ${Math.round(tooltipItem.yLabel * 100) / 100}`;
                                }
                                return label;
                            }
                        },
                        enabled: false,
                        custom: function (tooltipModel) {
                            // Tooltip Element
                            var tooltipEl = document.getElementById('chartjs-tooltip');

                            // Create element on first render
                            if (!tooltipEl) {
                                tooltipEl = document.createElement('div');
                                tooltipEl.id = 'chartjs-tooltip';
                                tooltipEl.innerHTML = '<table></table>';
                                document.body.appendChild(tooltipEl);
                            }

                            // Hide if no tooltip
                            if (tooltipModel.opacity === 0) {
                                tooltipEl.style.opacity = 0;
                                return;
                            }

                            // Set caret Position
                            tooltipEl.classList.remove('above', 'below', 'no-transform');
                            if (tooltipModel.yAlign) {
                                tooltipEl.classList.add(tooltipModel.yAlign);
                            } else {
                                tooltipEl.classList.add('no-transform');
                            }

                            function getBody(bodyItem) {
                                console.log(bodyItem.lines);
                                return bodyItem.lines;
                            }

                            // Set Text
                            if (tooltipModel.body) {
                                var titleLines = tooltipModel.title || [];
                                var bodyLines = tooltipModel.body.map(getBody);

                                //var innerHtml = '<thead>';
                                var count;
                                var ti;


                                bodyLines.forEach(function (body, i) {
                                    count = body[0].split(':')[1];
                                });

                                titleLines.forEach(function (title) {
                                    ti = title;
                                });

                               
                                //innerHtml += '</tbody>';

                                var innerHtml = `<div><div class=\"tooltip-label-value\">value</div><div class=\"tooltip-value\">${count}</div> <div class=\"tooltip-label-value\">document</div><div class=\"tooltip-value\">${ti}</div></div>`;


                                var tableRoot = tooltipEl.querySelector('table');
                                tableRoot.innerHTML = innerHtml;
                            }

                            // `this` will be the overall tooltip
                            var position = this._chart.canvas.getBoundingClientRect();

                            // Display, position, and set styles for font
                            tooltipEl.style.opacity = 1;
                            tooltipEl.style.position = 'absolute';
                            tooltipEl.style.left = position.left + window.pageXOffset + tooltipModel.caretX - 40 + 'px';
                            tooltipEl.style.top = position.top + window.pageYOffset + tooltipModel.caretY + 'px';
                            tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
                            tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
                            tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
                            tooltipEl.style.pointerEvents = 'none';
                            tooltipEl.style.width = '254px';
                            tooltipEl.style.backgroundColor = 'white';
                            tooltipEl.style.borderRadius = '8px';
                            tooltipEl.style.boxShadow = ' 0 2px 14px 0 rgba(199, 199, 199, 0.5)';
                            tooltipEl.style.padding = "20px";

                        }
                    },
                    legend: {
                        display: true,
                        labels: {
                            fontColor: 'rgb(0, 0, 0)',

                        }
                    }
                   
                }
            }
           //max = Math.max.apply(Math, response.map(x => x.Count));
            steps = 10;

            respondCanvas();
        }
    });
};

function getTotalChartData() {
    $.ajax({
        url: '/ThesaurusEntry/GetEntriesCount',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            $('#totalThesaurusEntries').html(response.Total);
            $('#totalUmls').html(response.TotalUmls);
            $('.items').show();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    })
}

function getOrganizationUsersCountData() {
    $.ajax({
        url: '/Organization/GetUsersByOrganizationCount',
        method: 'GET',
        success: function (response) {
            $('#organizationUsersCount').html(response);

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

$(document).ready(function () {
    $(window).resize(respondCanvas);

    getDocumentChartData();
    getTotalChartData();
    getOrganizationUsersCountData();
});

