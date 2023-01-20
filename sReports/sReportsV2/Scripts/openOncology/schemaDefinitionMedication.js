/////////////
//// Quantity - Basic functionality schemaDefinitionMedicaion
/////////////
$(document).on('click', '.minus', function (e) {
    var operationDiv = $(this).parent();
    const $input = $(operationDiv).find('input');

    if (isPremedicationOperation(operationDiv)) {
        let valueToSet = setValueInput($input, -1);
        addNewDayItem(e, '_', valueToSet);
    } else {
        if ($input.val() > 0) {
            removeDayModal(e, 'num-day', 'num-day-wrapper', 'num-day__', 1);
        }
    }

    return false;
});
$(document).on('click', '.plus', function (e) {
    var operationDiv = $(this).parent();
    const $input = $(operationDiv).find('input');

    if (isPremedicationOperation(operationDiv)) {
        if ($input.val() < 0) {
            removeDayModal(e, 'prem-day', 'prem-day-wrapper', 'prem-day_-', -1);
        }
    } else {
        let valueToSet = setValueInput($input, 1);
        addNewDayItem(e, '__', valueToSet);
    }

    return false;
});

function setSelect2(id, selector, getElementAction, getAutocompleteAction, minimumInputLength = 3) {
    let initDataSource = [];
    $.ajax({
        type: 'GET',
        url: `/SmartOncology/${getElementAction}?id=${id}`,
        success: function (data) {
            if (!jQuery.isEmptyObject(data)) {
                initDataSource.push(data);
            }
            $(`#${selector}`).select2({
                minimumInputLength: minimumInputLength,
                allowClear: true,
                placeholder: '',
                ajax: {
                    url: `/SmartOncology/${getAutocompleteAction}`,
                    dataType: 'json',
                    data: function (params) {
                        return {
                            Term: params.term,
                            Page: params.page,
                            ExcludeId: $("#id").val()
                        };
                    },
                },
                data: initDataSource
            });
            if (initDataSource.length > 0) {
                $(`#${selector}`).val(initDataSource[0].id.toString()).trigger('change');
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function updateMedication(form, event) {
    event.preventDefault();
    event.stopPropagation();

    if (getSchemaId() == 0) {
        toastr.warning('Please enter schema name first');
        return;
    }

    $(form).validate({
        ignore: []
    });

    if ($(form).valid()) {
        let request = getMedicationObject();
        let isNewObject = request.Id == 0;
        $.ajax({
            type: "POST",
            url: "/SmartOncology/UpdateSchemaMedication",
            data: request,
            success: function (data) {
                toastr.success("Schema medication is updated sucessfully!");
                request['Id'] = data.Id;
                updateMedicationOnUI(request, isNewObject, data.RowVersion);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

function getMedicationObject() {
    let schemaId = getSchemaId();
    let request = {};

    request['Id'] = getMedicationId();
    request['Name'] = $("#inputMedicationName").val();
    request['Dose'] = $("#inputAmount").val();
    request['UnitId'] = $("#inputUnit").val();
    request['PreparationInstruction'] = $("#inputPreparationInstruction").val();
    request['ApplicationInstruction'] = $("#inputApplicationInstruction").val();
    request['AdditionalInstruction'] = $("#inputAdditionalInstruction").val();
    request['RouteOfAdministration'] = $("#inputRouteOfAdministration").val();
    request['BodySurfaceCalculationFormula'] = $("#inputDoseCalculationFormula").val();
    request['SameDoseForEveryAplication'] = $("input[name=inputSameDose]:checked").val();
    request['HasMaximalCumulativeDose'] = $("input[name=inputHasMaximulaCumulativeDose]:checked").val();
    request['CumulativeDose'] = $("#inputMaxCumulativeDose").val();
    request['CumulativeDoseUnitId'] = $("#inputMaxCumulativeDoseUnit").val();
    request['WeekendHolidaysExcluded'] = $("input[name=inputHolidaysExcluded]:checked").val();
    request['MaxDayNumberOfApplicationiDelay'] = $("#inputMaxDaysOfDelay").val();
    request['IsSupportiveMedication'] = $("input[name=inputSupportiveMedication]:checked").val();
    request['SupportiveMedicationReserve'] = $("#inputAsReserve:checked").val();
    request['SupportiveMedicationAlternative'] = $("#inputHasAlternative:checked").val();
    request['ChemotherapySchemaId'] = schemaId;
    request['RowVersion'] = getRowVersionMedication();

    request['MedicationDoses'] = getDoses();

    return request;
}

function getMedicationId() {
    return $("#inputMedicationId").val();
}

function getMedicationDoseId() {
    return $('#doseId').val();
}

function getDoses() {
    var doses = [];
    $("#prem-day-wrapper .day-item, #num-day-wrapper .day-item").each(function () {
        var dose = getDose($(this));
        doses.push(dose);
    });

    return doses;
}

function getDose(dayDose) {
    var dataSet = $(dayDose).attr("data-set");
    var attrs = decodeURIComponent(dataSet);
    var dose = JSON.parse(attrs);

    return dose;
}

function updateMedicationOnUI(medication, isNewObject, rowVersion) {
    updateMedicationId(medication.Id);
    updateRowVersionMedication(rowVersion);
    updateMedicationContent(medication, isNewObject);
}

function updateMedicationId(id) {
    $("#inputMedicationId").val(id);
}

function updateMedicationContent(medication, isNewObject) {
    let content = `
        <h3>${medication.Name}</h3>
        <span class="icon">
            <img src="/Content/open-oncology/images/icons/edit-icon.svg" alt="Edit">
        </span>
    `;
    if (isNewObject) {
        $("#medicationsPreview").append(`
            <div class="schema-card-item item-link update-medication" data-id="${medication.Id}">
                <div class="item-header pointer">${content}</div>
            </div>
        `);
        selectActiveMedication($(`.update-medication[data-id="${medication.Id}"]`), medication.Id);
    } else {
        var elementToUpdate = $(`.update-medication[data-id="${medication.Id}"]`);
        $(elementToUpdate).children(".item-header").html(content);
    }
}

/////////////////////////////[jquery] functions for medications
// Add day item field
$(document).on('click', '.close-add-med', function (e) {
    $('.new-medication-wrapper').hide();
    unselectPreviousActiveMedication();
});

function setValueInput($input, sign) {
    let valueToSet = parseInt($input.val()) + sign;
    $input.val(valueToSet);
    $input.change();

    return valueToSet;
}

function isPremedicationOperation(el) {
    return $(el).attr("id") == 'prem-day';
}

function addNewDayItem(e, downline, inputVal) {
    let request = {
        DayNumber: inputVal,
        IntervalId: '',
        UnitId: '',
        Id: 0,
        MedicationId: getMedicationId(),
    };

    let parentId = $(e.target).parent().attr('id');
    let dayTagId = parentId + downline + inputVal;
    $('#' + parentId + '-wrapper').append(`<div id="${dayTagId}" class="day-item">DAY <span class="num">${inputVal}</span></div>`);

    updateMedicationDose(request, dayTagId, false, false);
}

function removeDayConfirm(e) {
    e.preventDefault();
    $('#deleteModal').modal('hide');

    removeDayAction();
}

function removeDayAction() {
    var doseId = getDoseId();
    if (doseId) {
        $.ajax({
            type: "DELETE",
            url: "/SmartOncology/DeleteMedicationDose",
            data: getDeleteDoseObject(doseId),
            success: function (data) {
                removeDayOnUI();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    } else {
        removeDayOnUI();
    }
}

function removeDayOnUI() {
    var elId = document.getElementById("buttonSubmitDelete").getAttribute('data-id');
    var dayTagId = elId.split('||')[0];
    var operationDivId = elId.split('||')[1];
    $(`#${dayTagId}`).remove();

    var sign = operationDivId === 'prem-day' ? 1 : -1;
    setValueInput($(`#${operationDivId}`).find('input'), sign);
}

function getDeleteDoseObject(doseId) {
    var request = {};

    request['RowVersion'] = getRowVersionMedication();
    request['Id'] = doseId;
    request['MedicationId'] = getMedicationId();

    return request;
}

function getDoseId() {
    var elId = document.getElementById("buttonSubmitDelete").getAttribute('data-id');
    var dayTagId = elId.split('||')[0];

    let dataSet = $('#' + dayTagId).attr("data-set");

    if (dataSet) {
        let dose = JSON.parse(decodeURIComponent(dataSet));
        return dose.Id;
    } else {
        return "";
    }

}

function removeDayModal(e, operationDivId, wrapperId, elId, sign) {
    let elIndex = $(`#${wrapperId} .day-item`).length;
    var confirmMessage = `Are you sure you want to remove DAY ${elIndex * sign}?`;
    showDeleteModal(e, `${elId}${elIndex}||${operationDivId}`, 'removeDayConfirm');
    $('.confirm-deletion').text(confirmMessage);
}

///////////
// Modal - Add new medicaction dose
///////////
//Show modal - Standardised Dosing Time
$(document).on('click', '.days-wrapper-body .day-item', function () {
    resetDoseModalState();

    $('body').addClass('modal-active');
    $('.modal-med-day').css('display', 'block').trigger('lowZIndex');
    let curEl = $(this).attr('id');
    modalEl = curEl;

    $("#day-item-modal").removeClass("green");
    $("#day-item-modal").removeClass("orange");

    let dose = {};
    if ($(this)[0].hasAttribute("data-set")) {
        //Get data from element
        let dataSet = $('#' + modalEl).attr("data-set");
        dose = JSON.parse(decodeURIComponent(dataSet));

        if (dose.IntervalId) {
            // Set modal title day-item background style
            var parentElement = $(this).parent().attr('id');
            if (parentElement === "num-day-wrapper") {
                $("#day-item-modal").addClass("green");
            } else if (parentElement === "prem-day-wrapper") {
                $("#day-item-modal").addClass("orange");
            }
        }
    }

    populateDoseModalWithData(dose);

    let medicationName = $("#inputMedicationName").val() ? $("#inputMedicationName").val() : "New medication";
    $("#medicationDoseModalTitle").text(medicationName);
    let daynumber = getDayNumber(modalEl);
    $("#day-item-modal").children('span').text(daynumber);
});

function populateDoseModalWithData(dose) {
    $.ajax({
        type: "POST",
        data: dose,
        url: `/SmartOncology/EditMedicationDoseContent`,
        success: function (data) {
            $("#doseModalContent").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

//Hide modals
$(document).on('click', '.close-modal', function () {
    closeModal('.modal-med-day');
});

// Handle #interval input on change
$(document).on('change', "#interval", function (e) {
    //Initial - clear inputs
    var intervalStartAt = $('option:selected', this).attr('startAt');
    $('#cycle-starts').val(intervalStartAt);

    let intervalVal = $(this).val();
    setDosingTimeUI(intervalVal);
});

function setDosingTimeUI(intervalId) {
    $.ajax({
        type: "GET",
        url: `/SmartOncology/EditMedicationDoseTimes?interval=${intervalId}`,
        success: function (data) {
            $(".dose-time-wrapper").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

// Handle modal SAVE button - set data-set to right element;
$(document).on('click', '#save-dose', function () {
    $('#' + modalEl).attr('data-set', '');
    let intervalVal = $('#interval').val();

    if (!intervalVal) {
        toastr.warning("Please fill out data.");
        return;
    }

    let daynumber = modalEl === 'select-all' ? 0 : getDayNumber(modalEl);
    let elementObj = {
        DayNumber: daynumber,
        IntervalId: intervalVal,
        UnitId: $('#dose-time-unit').val(),
        Id: getMedicationDoseId(),
        MedicationId: getMedicationId(),
        StartsAt: $('#cycle-starts').val()
    };

    var doses = [];
    $('.dose-time-wrapper .dose-time-value').each(function () {
        var dose = $(this).children('input').val();
        var time = $(this).children('.label').text().trim();
        doses.push({
            'Time': time,
            'Dose': dose,
            'MedicationDoseId': getMedicationDoseId(),
            'Id': 0
        });
    });
    elementObj.MedicationDoseTimes = doses;

    if (modalEl === 'select-all') {
        updateMedicationDoseInBatch(elementObj);
    } else {
        updateMedicationDose(elementObj, modalEl, true, true);
    }
    // Reset state
    resetDoseModalState();
    closeModal('.modal-med-day');
});

function resetDoseModalState() {
    $('#doseId').val("");
    $('#interval').val('');
    $('#cycle-starts').val('');
    $('.dose-time-wrapper').empty();
}

function saveDataIntoDayTag(elementObj, activeDayEl, setBackground) {
    let dataSet = encodeURIComponent(JSON.stringify(elementObj));
    $('#' + activeDayEl).attr('data-set', dataSet);
    if (setBackground) {
        $('#' + activeDayEl).addClass("background");
    }
}

function saveDataIntoDayTagInBatch(elementObj, values = {}) {
    $('#num-day-wrapper .day-item').each(function () {
        var data = elementObj;
        var dayNumber = getDayNumber($(this).attr("id"));
        data.DayNumber = dayNumber;
        if (isBatch(values)) {
            data.Id = values[dayNumber];
        }
        $(this).attr('data-set', encodeURIComponent(JSON.stringify(data)));
        $(this).addClass('background');
    });
}

function isBatch(values) {
    return !jQuery.isEmptyObject(values);
}

function updateMedicationDose(request, activeEl, setBackground, succesMsg) {
    if (request.MedicationId > 0) {
        request['RowVersion'] = getRowVersionMedication();
        $.ajax({
            type: "POST",
            url: "/SmartOncology/UpdateSchemaMedicationDose",
            data: request,
            success: function (data) {
                if (succesMsg) {
                    toastr.success('Standardised Dosing Time [Day ' + request.DayNumber + '] is updated!');
                }
                request['Id'] = data.Id;
                saveDataIntoDayTag(request, activeEl, setBackground);
                updateRowVersionMedication(data.RowVersion);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    } else {
        saveDataIntoDayTag(request, activeEl, setBackground);
    }
}

function updateMedicationDoseInBatch(elementObj) {
    if (elementObj.MedicationId > 0) {
        var request = getMedicationDoseInBatchObject(elementObj);

        $.ajax({
            type: "POST",
            url: "/SmartOncology/UpdateSchemaMedicationDoseInBatch",
            data: request,
            success: function (data) {
                toastr.success('Standardised Dosing Time [Day ALL] is updated!');
                saveDataIntoDayTagInBatch(elementObj, data.IdsPerDays);
                updateRowVersionMedication(data.RowVersion);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    } else {
        saveDataIntoDayTagInBatch(elementObj);
    }
}

function getMedicationDoseInBatchObject(elementObj) {
    var request = {};

    request['Doses'] = $('#num-day-wrapper .day-item').map(function () {
        var obj = { ...elementObj };
        obj.DayNumber = getDayNumber($(this).attr("id"));
        return obj;
    }).get();
    request['MedicationId'] = getMedicationId();
    request['RowVersion'] = getRowVersionMedication();

    return request;
}

//Select all days - new medication
$(document).on('click', '#select-all', function (e) {
    if (dayItemsExist()) {
        resetDoseModalState();

        $('body').addClass('modal-active');
        $('.modal-med-day').css('display', 'block').trigger('lowZIndex');
        let curEl = $(this).attr('id');
        modalEl = curEl;

        $("#day-item-modal").removeClass("green");
        $("#day-item-modal").removeClass("orange");
        $("#day-item-modal").addClass("green");

        populateDoseModalWithData({});

        let medicationName = $("#inputMedicationName").val() ? $("#inputMedicationName").val() : "New medication";
        $("#medicationDoseModalTitle").text(medicationName);
        $("#day-item-modal").children('span').text('ALL');
    }
});

function dayItemsExist() {
    return $('#num-day-wrapper .day-item').length > 0;
}

function getDayNumber(doseId) {
    return doseId.slice(9, doseId.length);
}

$(document).on('click', 'input[name="inputHasMaximulaCumulativeDose"]', function (e) {
    shouldCumulativeDoseContainerBeDisplayed();
});

function shouldCumulativeDoseContainerBeDisplayed() {
    if ($('#hasCumulativeDose').prop('checked')) {
        $('.cumulative-dose-wrapper').fadeIn(150);
    } else {
        $('.cumulative-dose-wrapper :input').val("");
        $('.cumulative-dose-wrapper').fadeOut(150);
    }
}

function updateRowVersionMedication(rowVersion) {
    $('#rowVersionMedication').val(rowVersion);
}

function getRowVersionMedication() {
    return $('#rowVersionMedication').val();
}