function showChartModal(formId) {
    event.preventDefault();
    $('#chartModal').modal('show');

    // If checkboxes already loaded -> skip fields retrieval
    if ($('#fields-chkbox-list').length == 0) {

        $.ajax({
            type: 'GET',
            url: `/FormInstance/GetFieldsToPlot`,
            data: {
                'formId': formId
            },
            traditional: true,
            success: function (data) {
                $('#checkbox-fields-to-plot').html(data);
                createDataCaptureChart(); // creates empty chart
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
    else {
        clearModal();
    }


}

function submitFieldsToPlot(event, formId) {
    event.preventDefault();

    let chkArray = [];
    $("input[name=fieldChkbox]:checked").each(function () {
        chkArray.push($(this).val());
    });

    copyDateToHiddenField($("#chartDateTimeFrom").val(), "dateTimeFrom");
    copyDateToHiddenField($("#chartDateTimeTo").val(), "dateTimeTo");

    requestObject = {
        'formDefinitionId': formId,
        'fieldThesaurusIds': chkArray,
        'DateTimeFrom': toLocaleDateStringIfValue($('#dateTimeFrom').val()),
        'DateTimeTo': toLocaleDateStringIfValue($('#dateTimeTo').val())
    }

    if (validateChartFilters()) {
        $.ajax({
            type: 'GET',
            url: `/FormInstance/GetFormInstanceFieldsById`,
            data: requestObject,
            traditional: true,
            dataType: 'json',
            success: function (data) {
                console.log(data);
                $.each(data, function () { // todo review
                    plotDataCaptureFields(this);

                });
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }    
}

function plotDataCaptureFields(fieldsToPlotKeyVal) {

    let dataSetArray = [];

    $.each(Object.keys(fieldsToPlotKeyVal), function () {

        key = this;
        let dataSet = [];
        
        for (let i = 0; i < fieldsToPlotKeyVal[key].Value.length; i++) {
            dataSet.push({
                t: new Date(fieldsToPlotKeyVal[this].Date[i]),
                y: fieldsToPlotKeyVal[this].Value[i]
            });
        }

        dataSetArray.push({
            label: key,
            data: sortDataSetByTime(dataSet),
            borderColor: randomColor(),
            fill: false,
            lineTension: 0  // 0:straight line, 1: curve line
        });

    });

    const data = {
        labels: [], // without setting labels, x axis will fit data
        datasets: dataSetArray
    };    

    createDataCaptureChart(data);
}

function createDataCaptureChart(data) {

    let ctx = document.getElementById('data-capture-chart');
    return new Chart(ctx, {
        type: 'line',
        data: data,
        options: {
            title: {
                display: true,
                text: $("#form-title").val()
            },
            scales: {
                xAxes: [{
                    type: 'time',
                    ticks: {
                        source: 'data',
                        autoSkip: true, // displays only X axis dates where there is a data point                        
                    },
                    time: {
                        unit: 'day',
                        displayFormats: {
                            day: 'dd/MM/yyyy'
                        },
                        tooltipFormat: 'dd/MM/yyyy'
                    }
                }],
                bounds: 'ticks'
            }
        }
    });
}

// Keep Dropdown selection open when selecting checkboxes
$(document).on('click', '#fields-chkbox-list', function (e) {
    e.stopPropagation();
    let numCheckBoxesSelected = $("input[name=fieldChkbox]:checked").length;
    if (numCheckBoxesSelected > 0) {
        $("button.arrow-select").removeClass("error");
    } else {
        $("button.arrow-select").addClass("error");
    }
});

$(document).on('click', '.form-checkbox-label', function (e) {
    updateCheckBoxes();
});

// ---------------  Helpers  ---------------

// Enables/Disables checkbox selection when limit reached & Updates selection counter
function updateCheckBoxes() {
    let numCheckBoxesSelected = $("input[name=fieldChkbox]:checked").length;

    if (numCheckBoxesSelected == 5) {
        $("input[name=fieldChkbox]:unchecked").prop('disabled', true);
    }
    else {
        $("input[name=fieldChkbox]").prop('disabled', false);
    }

    $('#dropdown-button').html(`${numCheckBoxesSelected}/5 selected`)
}

function sortDataSetByTime(dataset) {
    return dataset.sort(function (a, b) {
        return a['t'] - b['t']
    });
}

function randomColor() {
    var r = Math.floor(Math.random() * 255);
    var g = Math.floor(Math.random() * 255);
    var b = Math.floor(Math.random() * 255);
    return "rgb(" + r + "," + g + "," + b + ")";
};

function clearModal() {
    createDataCaptureChart();
    $("input[name=fieldChkbox]:checked").prop('checked', false);
    updateCheckBoxes();
    $(".date-chart-filter").val(""); 
}

function validateChartFilters() {
    return $("#chartForm").valid();
}

function setValidationChartFunctions() {
    setCommonValidatorMethods();
    $.validator.addMethod(
        "fieldsToPlot",
        function (value, element) {
            return $("input[name=fieldChkbox]:checked").length > 0;
        },
        "This field is required."
    );
    $("#chartForm").validate({
        ignore: [],
        errorPlacement: function (error, element) {
            handleErrorPlacementInForChartFilters(error, element);
        }
    });
    $('[name="validateFieldsToPlot"]').each(function () {
        $(this).rules('add', {
            fieldsToPlot: true
        });
    });
}

function handleErrorPlacementInForChartFilters(error, element) {
    if (isDateOrTimeInput(element)) {
        handleErrorPlacementForDateOrTime(error, element);
    } else if (isFieldsToPlot(element)) {
        handleErrorPlacementInChartForRadio(error, element);
    } else {
        handleErrorPlacementForOther(error, element);
    }
}

function isFieldsToPlot(element) {
    return element.attr("name") == "validateFieldsToPlot";
}

function handleErrorPlacementInChartForRadio(error, element) {
    var targetContainerForErrors = getElementWhereErrorShouldBeAdded(element);
    error.appendTo(targetContainerForErrors);
    $("button.arrow-select").addClass("error");
}