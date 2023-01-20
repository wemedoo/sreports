function showAllergyModal(event) {
    event.preventDefault();

    $('#allergyModal').modal('show');
}

function addAllergy(event) {
    event.preventDefault();

    var allergy = $('#allergyName').val();
    if (allergy) {
        var li = $('<li></li>').text(allergy);
        $('#patientAllergies').removeClass("d-none").append(li);

        $('#allergyModal').modal('hide');
        $('#allergyName').val('')
    }
}

function autocompleteEventHandler(event) {
    if (event.type == 'select2:opening') {
        $(event.target).siblings('img').hide();
    } else if (event.type == 'select2:closing') {
        if ($(event.target).val()) $(event.target).siblings('img').show();
    }
}

function viewPatientData(event, viewDisplayType = '') {
    let patientId = $(event.target).val();
    $.ajax({
        url: `/SmartOncology/ViewPatientDataProgressNote?id=${patientId}&viewDisplayType=${viewDisplayType}`,
        success: function (data) {
            $(`#patientData${viewDisplayType}`).html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function viewSchema(event, viewDisplayType = '') {
    if (canGetSchema(viewDisplayType)) {
        let schemaId = $(event.target).val();
        var request = getViewSchemaRequest(schemaId, viewDisplayType);
        getSchema(request);
    }
}

function getViewSchemaRequest(id, viewDisplayType) {
    if (validateDateInput($(`#schemaStart${viewDisplayType}`))) {
        var schemaStartDate = $(`#schemaStart${viewDisplayType}`).val();
        return { id: id ? id : 0, schemaStartDate: toDateStringIfValue(schemaStartDate), viewDisplayType: viewDisplayType }
    } else {
        return null;
    }    
}

function canGetSchema(viewDisplayType) {
    var schemaStartDate = $(`#schemaStart${viewDisplayType}`).val();
    if (!schemaStartDate) {
        displayMissingSchemaErrorMessage(viewDisplayType);
        return false;
    }

    var patientId = $(`#patientName${viewDisplayType}`).val();
    if (!patientId) {
        displayMissingPatientErrorMessage(viewDisplayType);
        return false;
    }

    return true;
}

function getSchema(request) {
    if (request) {
        $.ajax({
            type: 'GET',
            data: request,
            url: `/SmartOncology/ViewSchema`,
            success: function (data) {
                $("#schemaData").html(data);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

$(document).on("click", ".toggle-date-section", function () {
    $(this).toggleClass('active');
    $(".collapse-date").collapse('toggle'); // toggle collapse
});

//Toggle patient details - desktop
$(document).on('click', '.toggle', function () {
    $('.details').toggleClass("collapse-left");
    $('.arrow-right').toggleClass("show");
});

function changeLanguage(event) {
    event.preventDefault();
    $(".lang-item").removeClass("active");
    $(event.target).addClass("active");
}

function getViewDisplayType() {
    return $('#viewDisplayType').val();
}