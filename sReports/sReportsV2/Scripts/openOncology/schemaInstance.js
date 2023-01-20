function cancelSchemaInstance(event) {
    event.preventDefault();
    event.stopPropagation();
    var viewDisplayType = getViewDisplayType();
    $(`#schemaName${viewDisplayType}`).val("").trigger("change");
    var request = getViewSchemaRequest('', viewDisplayType);
    getSchema(request);
}

function saveSchemaInstance(event) {
    event.preventDefault();
    event.stopPropagation();

    if (!canSubmitSchemaInstance()) return;

    var request = getSchemaInstance();
    $.ajax({
        type: 'POST',
        data: request,
        url: `/SmartOncology/EditSchemaInstance`,
        success: function (data) {
            if (!request.Id) {
                getSchemaInstanceTableData(data.Id, getCollapsedTables());
            } else {
                updateSchemaInstanceId(data.Id);
                updateRowVersion(data.RowVersion);
            }
            toastr.success('Schema instance is updated');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getSchemaInstanceTableData(id, openedElements) {
    $.ajax({
        type: 'GET',
        url: `/SmartOncology/ViewSchemaInstanceTableData/${id}`,
        success: function (data) {
            $("#schemaData").html(data);
            collapsePrevioslyOpenedTables(openedElements);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getCollapsedTables() {
    return $('.history-data [aria-expanded="true"]').map(function () { return $(this).attr("data-target"); }).get();
}

function collapsePrevioslyOpenedTables(collapseTargets) {
    for (collapseTarget of collapseTargets) {
        var collapseSource = $(`[data-target="${collapseTarget}"]`);
        $(collapseSource).attr("aria-expanded", "true");
        $(`${collapseTarget}`).addClass("show");
    }
}

function updateSchemaInstanceId(id) {
    $("#schemaInstanceId").val(id);
}

function getSchemaInstanceId() {
    return $('#schemaInstanceId').val();
}

function canSubmitSchemaInstance() {
    var viewDisplayType = getViewDisplayType();
    var patientId = $(`#patientName${viewDisplayType}`).val();
    var schemaId = $(`#schemaName${viewDisplayType}`).val();

    if (!patientId) {
        displayMissingPatientErrorMessage(viewDisplayType);
        return false;
    }

    if (!schemaId) {
        displayMissingPatientErrorMessage(viewDisplayType);
        return false;
    }

    return true;
}

function getSchemaInstance() {
    var request = {};

    var viewDisplayType = getViewDisplayType();
    var schemaInstanceId = getSchemaInstanceId();
    var schemaId = $(`#schemaName${viewDisplayType}`).val();
    var patientId = $(`#patientName${viewDisplayType}`).val();
    var schemaStartDate = toDateStringIfValue($(`#schemaStart${viewDisplayType}`).val());

    request['Id'] = schemaInstanceId;
    request['StartDate'] = schemaStartDate;
    request['ChemotherapySchemaId'] = schemaId;
    request['PatientId'] = patientId;
    request['RowVersion'] = getRowVersion();

    request['Medications'] = getMedications();

    return request;
}

function updateRowVersion(rowVersion) {
    $('#rowVersion').val(rowVersion);
}

function getRowVersion() {
    return $("#rowVersion").val();
}

function getMedications() {
    var medications = [];

    $(".table-medications tr").each(function () {
        var medication = getMedication($(this));
        medications.push(medication);
    });

    return medications;
}

function getMedication(medicationRow) {
    var medication = {};

    var medicationDefinitionId = $(medicationRow).attr('data-medication');
    medication['MedicationId'] = medicationDefinitionId;
    medication['ChemotherapySchemaInstanceId'] = getSchemaInstanceId();
    medication['Id'] = $(medicationRow).attr('data-medicationInstance');
    medication['MedicationDoses'] = getMedicationDoses(medicationRow, medicationDefinitionId);

    return medication;
}

function getMedicationDoses(medicationRow) {
    var medicationDoses = [];

    $(medicationRow).children('.input-field[data-hasValue="true"]').each(function () {
        var medicationDose = getMedicationDose($(this));
        medicationDoses.push(medicationDose);
    });

    return medicationDoses;
}

function getMedicationDose(doseCell) {
    var medicationDose = getDose(doseCell);

    medicationDose['Date'] = toDateStringIfValue($(doseCell).attr('data-date'));

    return medicationDose;
}

function getDose(dayDose) {
    var dataSet = $(dayDose).attr("data-set");
    if (dataSet) {
        return getDataFromElement(dataSet);
    } else {
        return {};
    }
}

function getDataFromElement(dataSet) {
    var attrs = decodeURIComponent(dataSet);
    return JSON.parse(attrs);
}

function displayMissingPatientErrorMessage(viewDisplayType) {
    $(`.select2-selection--single[aria-labelledby="select2-patientName${viewDisplayType}-container"]`).addClass('error');
    toastr.warning('Please select patient');
}

function displayMissingSchemaErrorMessage(viewDisplayType) {
    $(`#schemaStart${viewDisplayType}`).addClass('error');
    toastr.warning('Please select start date');
}

$(document).on('click', '.dose-date', function () {
    let schemaInstanceId = getSchemaInstanceId();

    if (schemaInstanceId) {
        var dayNumber = $(this).attr('data-day-number');
        var date = $(this).attr('data-date');
        showDelayDoseModal(dayNumber, date);
    }
});

function showDelayDoseModal(daynumber, date) {
    resetDelayDoseModalState();

    $('body').addClass('modal-active');
    $('.modal-delay-dose').css('display', 'block').trigger('lowZIndex');

    $("#day-delay-dose-modal").removeClass("green");
    $("#day-delay-dose-modal").removeClass("orange");

    $("#day-delay-dose-modal").children('span').text(daynumber);
    $(".date-title").text(date);
    $('#delayStartDay').val(daynumber);

    // Set modal title day-item background style
    if (daynumber > 0) {
        $("#day-delay-dose-modal").addClass("green");
    } else {
        $("#day-delay-dose-modal").addClass("orange");
    }
}

function resetDelayDoseModalState() {
    $('#delayFor').val('');
    $('#delayStartDay').val('');
}

$(document).on('click', '#save-delay-dose', function () {
    let schemaInstanceId = getSchemaInstanceId();

    if (schemaInstanceId) {
        let delayFor = $('#delayFor').val();

        if (!delayFor || delayFor > 15) {
            toastr.warning("Please fill out data.");
            return;
        }

        let request = {
            DayNumber: $("#delayStartDay").val(),
            ChemotherapySchemaInstanceId: schemaInstanceId,
            DelayFor: delayFor,
            ReasonForDelay: $("#reasonForDelay").val()
        };

        var collapsedElements = getCollapsedTables();

        $.ajax({
            type: "GET",
            url: "/SmartOncology/DelayDose",
            data: request,
            success: function (data) {
                toastr.success('Success');
                $("#schemaData").html(data);
                getSchemaInstanceTableData(request.ChemotherapySchemaInstanceId, collapsedElements);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }

    closeModal('.modal-delay-dose');
});

$(document).on('click', '.history-delay-dose', function () {
    let schemaInstanceId = getSchemaInstanceId();

    if (schemaInstanceId) {
        var viewDisplayType = getViewDisplayType();
        var schemaStartDate = toDateStringIfValue($(`#schemaStart${viewDisplayType}`).val());

        let request = {
            DayNumber: $(this).attr("data-day-number"),
            ChemotherapySchemaInstanceId: schemaInstanceId,
            StartDate: schemaStartDate
        };

        $.ajax({
            type: "GET",
            url: "/SmartOncology/ViewHistoryOfDayDose",
            data: request,
            success: function (data) {
                showHistoryDelayDoseModal(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
});

function showHistoryDelayDoseModal(content) {
    $('#history-delay-dose-content').html(content);
    $('#historyDelayDoseModal').modal("show");
}

function openPopupComment(event, id) {
    $(`.popuptext[id!="popup-${id}"]`).removeClass('show');
    var currentComment = $(event.currentTarget).find('.popuptext');
    if ($(currentComment).hasClass('show')) {
        $(currentComment).removeClass('show');
    } else {
        $(currentComment).addClass('show');
    }
}

$(document).on('scroll', function () {
    $("#drop-menu").remove();
});

$(document).on('click', '.input-field-input', function () {
    handleInputClick($(this));
});