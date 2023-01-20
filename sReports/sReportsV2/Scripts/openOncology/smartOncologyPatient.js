function setInitialViewData(patientId) {
    if (patientId) {
        var selectedPatientEl = $(`.patient-table-entry[data-id="${patientId}"]`);
        handleOnPatientClick(selectedPatientEl);
    } else {
        addNewPatient();
    }
}

$(document).on('click', '.patient-table-entry', function (e) {
    handleOnPatientClick(e.currentTarget);
});

function handleOnPatientClick(el) {
    $('.patient-table-entry.active').removeClass('active');

    $(el).addClass('active');
    var patientId = $(el).data('id');

    viewPatient(patientId);
}

function searchTable(event) {
    event.preventDefault();

    reloadTable();
}

function reloadTable(selectedPatientId = 0) {
    let filter = getPatientSearchObject();
    filter["Page"] = 1;
    filter["PageSize"] = 20;
    filter["SelectedPatientId"] = selectedPatientId;

    emptyPatientDetailContainer();

    $.ajax({
        type: "get",
        data: filter,
        url: `/SmartOncology/ReloadPatientTable`,
        success: function (data, textStatus, jqXHR) {
            $(".smartOncologyPatientEntries").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

$('.patientSearchName').keypress(function (e) {
    $('.patientSearchName').not(this).val($(this).val());
    if (e.which == 13) {
        searchTable(e);
    }
});

function getPatientSearchObject() {
    let requestObject = {};

    requestObject['Name'] = $(".patientSearchName").val();

    return requestObject;
}

function viewPatient(id) {
    $.ajax({
        url: `/SmartOncology/ViewPatientData?id=${id}`,
        success: function (data) {
            $("#smart-oncology-patient-detail").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function editPatient(id) {
    $.ajax({
        url: `/SmartOncology/EditPatientData?id=${id}`,
        success: function (data) {
            $("#smart-oncology-patient-detail").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function addNewPatient() {
    $('.patient-table-entry.active').removeClass('active');
    $.ajax({
        url: `/SmartOncology/CreatePatientData`,
        success: function (data) {
            $("#smart-oncology-patient-detail").html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

$(document).on('focus', '.column-input > :input', function (e) {
    $(e.target).siblings('img.toggle').hide();
    $(e.target).closest('.column-input').addClass('focus');
});

$(document).on('blur', '.column-input > :input', function (e) {
    $(e.target).siblings('img.toggle').show();
    $(e.target).closest('.column-input').removeClass('focus');
});

$(document).on('focus', '#clinicalTrial', function (e) {
    $(event.target).closest('.column-input').find('img.toggle').hide();
    $(e.target).closest('.column-input').addClass('focus');
});

$(document).on('blur', '#clinicalTrial', function (e) {
    $(event.target).closest('.column-input').find('img.toggle').show();
    $(e.target).closest('.column-input').removeClass('focus');
});

function autocompleteEventHandler(event) {
    if (event.type == 'select2:opening') {
        $(event.target).closest('.column-input').find('img.toggle').hide();
    } else if (event.type == 'select2:closing') {
        $(event.target).closest('.column-input').find('img.toggle').show();
    }
}

$(document).on("keypress", ".patient-tag-input", function (e) {
    if (e.which === 13) {
        let isAppended = appendTagToContainer($(this).val().trim(), $(this).val().trim(), $(e.currentTarget).data("tag"));
        if (isAppended) {
            $(this).val('');
        }
        return false;
    }
});

function appendTagToContainer(value, displayValue, tagType) {
    if (!value || existsTagValue(value, tagType)) {
        return false;
    }
    $(`.repetitive-values[data-tag='${tagType}']`).append(createSingleTag(value, displayValue));
    return true;
}

function existsTagValue(value, tagType) {
    if ($(`.repetitive-values[data-tag='${tagType}']`).find(`.tag-text[data-value='${value}']`).length > 0) {
        return true;
    }
    return false;
}

function createSingleTag(value, displayValue) {
    var removeIcon = getNewRemoveIcon();

    var element = getNewSignleTagContainer();
    $(element).append(getNewSingleTagValue(value, displayValue));
    $(element).append(removeIcon);

    return element;
}

function getNewSignleTagContainer() {
    var element = document.createElement('div');
    $(element).addClass('repetitive-value');

    return element;
}

function getNewSingleTagValue(value, displayValue) {
    var text = document.createElement('span');
    $(text).addClass('tag-text');
    $(text).attr('data-value', value);
    $(text).html(displayValue);

    return text;
}

function getNewRemoveIcon() {
    var removeIcon = document.createElement('img');
    $(removeIcon).addClass('tag-icon');
    $(removeIcon).attr("src", "/Content/img/icons/Administration_remove.svg");
    $(removeIcon).attr('data-action', 'remove-tag');
    return removeIcon;
}

$(document).on('click', "*[data-action='remove-tag']", function (e) {
    $(e.target).closest('.repetitive-value').remove();
});

function submitPatientData(form, event) {
    $(form).validate({
        ignore: []
    });

    if ($(form).valid()) {
        var request = {};

        request['Id'] = $("#patientId").val();
        request['Activity'] = true;
        request['Name'] = $("#firstName").val();
        request['FamilyName'] = $('#secondName').val();
        request['Gender'] = $("#gender").val();
        request['BirthDate'] = toDateStringIfValue($("#dateOfBirth").val());

        request['Address'] = {};
        request['ContactPerson'] = { Name: {} };

        request['DesireToHaveChildren'] = $('input[name=desireToHaveChildren]:checked').val();
        request['FertilityConservation'] = $('input[name=fertilityConservation]:checked').val();
        request['SemenCryopreservation'] = $('input[name=semenCryopreservation]:checked').val();
        request['EggCellCryopreservation'] = $('input[name=eggCellCryopreservation]:checked').val();
        request['SexualHealthAddressed'] = $('input[name=sexualHealthAddressed]:checked').val();
        request['Contraception'] = $('input[name=contraception]:checked').val();
        request['PreviousTreatment'] = $('input[name=previousTreatment]:checked').val();
        request['TreatmentInCantonalHospitalGraubunden'] = $('input[name=treatmentInCantonalHospitalGraubunden]:checked').val();
        request['DiseaseContextAtInitialPresentation'] = $('input[name=diseaseContextAtInitialPresentation]:checked').val();
        request['DiseaseContextAtCurrentPresentation'] = $('input[name=diseaseContextAtCurrentPresentation]:checked').val();

        request['CapabilityToWork'] = $('#capabilityToWork').val();
        request['StageAtInitialPresentation'] = $('#stageAtInitialPresentation').val();
        request['StageAtCurrentPresentation'] = $('#stageAtCurrentPresentation').val();
        request['Anatomy'] = $('#anatomy').val();
        request['Morphology'] = $('#morphology').val();
        request['TherapeuticContext'] = $('#therapeuticContext').val();
        request['ChemotherapyType'] = $('#chemotherapyType').val();

        request['PatientInformedFor'] = $('#patientInformedFor').val();
        request['HistoryOfOncologicalDisease'] = $('#historyOfOncologicalDisease').val();
        request['IdentificationNumber'] = $('#identificationNumber').val();
        request['ChemotherapyCycle'] = $('#chemotherapyCycle').val();
        request['ChemotherapyCourse'] = $('#chemotherapyCourse').val();
        request['ConsecutiveChemotherapyDays'] = $('#consecutiveChemotherapyDays').val();

        request['PatientInfoSignedOn'] = toDateStringIfValue($("#patientInfoSignedOn").val());
        request['CopyDeliveredOn'] = toDateStringIfValue($("#copyDeliveredOn").val());
        request['FirstDayOfChemotherapy'] = toDateStringIfValue($("#firstDayOfChemotherapy").val());

        request['Allergies'] = getRepetitiveValues('allergy');
        request['HospitalOrPraxisOfPreviousTreatments'] = getRepetitiveValues('hospitalPreviosTreatment');
        request['ClinicalTrials'] = getRepetitiveValues('clinicalTrials');

        $.ajax({
            type: "POST",
            url: "/SmartOncology/EditPatientData",
            data: request,
            success: function (data) {
                toastr.options = {
                    timeOut: 100
                }
                toastr.options.onHidden = function () {
                    editPatient(data.Id);
                    reloadTable(data.Id);
                }
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }

    return false;
}

function getRepetitiveValues(tagType) {
    var values = [];
    $(`.repetitive-values[data-tag='${tagType}']`).find('.tag-text').each(function () {
        values.push($(this).data('value'));
    });

    return values.join(';');
}

function backOnCreate() {
    emptyPatientDetailContainer();
}

function emptyPatientDetailContainer() {
    $("#smart-oncology-patient-detail").html('');
}

$('.toggle').on('click', function () {
    $('.smart-oncology-patient-table').toggle();
    $('.arrow-right').toggleClass("show");
    $('.small-header').toggle();
});

$(document).on("click", "#closePatientNameIcon", function () {
    $('.patientSearchName').val('');
    reloadTable();
});

$(document).on('change', '#gender', function (e) {
    var gender = $(e.target).val();
    if (gender === 'Female') {
        $('.egg-cell-cryopreservation').fadeIn(150);
        $('.semen-cryopreservation').fadeOut(150);
        $('input[name=semenCryopreservation]').prop('checked', false);
    }
    if (gender === 'Male') {
        $('.semen-cryopreservation').fadeIn(150);
        $('.egg-cell-cryopreservation').fadeOut(150);
        $('input[name=eggCellCryopreservation]').prop('checked', false);
    }
});

$(document).on('click', 'input[name="previousTreatment"]', function (e) {
    if ($('#previousTreatmentYes').prop('checked')) {
        $('.treatment-graubunden').fadeIn(150);
        $('.stage-initial').fadeIn(150);
    } else {
        resetDependentFields();
        resetSubDependentFields();
    }
});

function resetDependentFields() {
    $('.treatment-graubunden').fadeOut(150);
    $('.stage-initial').fadeOut(150);

    $('input[name=treatmentInCantonalHospitalGraubunden]').prop('checked', false);
    $('#stageAtInitialPresentation').val('');
}

function resetSubDependentFields() {
    $('.hospital-praxis').fadeOut(150);
    $('.disease-context-initial').fadeOut(150);
    $('.history-oncological-disease').fadeOut(150);

    $('.repetitive-values[data-tag=hospitalPreviosTreatment]').empty();
    $('input[name=diseaseContextAtInitialPresentation]').prop("checked", false);
    $('#historyOfOncologicalDisease').val('');
}

$(document).on('click', 'input[name="treatmentInCantonalHospitalGraubunden"]', function (e) {
    if ($('#treatmentInCantonalHospitalGraubundenNo').prop('checked') && $('#previousTreatmentYes').prop('checked')) {
        $('.hospital-praxis').fadeIn(150);
        $('.disease-context-initial').fadeIn(150);
        $('.history-oncological-disease').fadeOut(150);
        $('#historyOfOncologicalDisease').val('');
    } else {
        $('.hospital-praxis').fadeOut(150);
        $('.disease-context-initial').fadeOut(150);
        $('.repetitive-values[data-tag=hospitalPreviosTreatment]').empty();
        $('input[name=diseaseContextAtInitialPresentation]').prop("checked", false);
        $('.history-oncological-disease').fadeIn(150);
    }
});
