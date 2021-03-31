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
                            label: '# Documents',//#2a796b
                            backgroundColor: response.map(x => '#2a796b'),
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
                            display: true
                        }],
                        yAxes: [{
                            gridLines: {
                                display: true,
                                color: 'transparent',
                                drawBorder: true,
                                zeroLineColor: 'lightgray'
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
                        }
                    }
                }
            }
            max = Math.max.apply(Math, response.map(x => x.Count));
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