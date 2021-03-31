var PatientId;
/*-------------------Episode of care------------------------*/
function reloadTable() {
    if ($('#patientId').val()) {
        $('#episodeOfCares').show();
        let requestObject = {};
        checkUrlPageParams();
        requestObject.Page = currentPage;
        requestObject.IdentifierType = 'O4MtPatientId';
        requestObject.IdentifierValue = $('#patientId').val();
        requestObject.PageSize = getPageSize();

        $.ajax({
            type: 'GET',
            url: '/EpisodeOfCare/ReloadTable',
            data: requestObject,
            success: function (data) {
                $("#episodeOfCaresContainer").html(data);
                $("#episodeOfCaresContainer").find('#collapseFilter').hide();
                $('#episodeOfCaresContainer').collapse('show');

            },
            error: function (xhr, textStatus, thrownError) {
                toastr.error(`Error: ${thrownError}`);
            }
        });
    }
}
function newEpisodeOfCare() {
    history.pushState({}, '', `?patientId=${$('#patientId').val()}`);
    window.location.href = `/EpisodeOfCare/Create?System=O4MtPatientId&Value=${$('#patientId').val()}`;
}

function editEntity(event,id) {
    window.location.href = `/EpisodeOfCare/Edit?EpisodeOfCareId=${id}`;
    event.preventDefault();
}

function removeEOCEntry(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/EpisodeOfCare/DeleteEOC?eocId=${id}&&LastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}
/*-------------------Episode of care------------------------*/

function submitForm(form,e) {
 
    $(form).validate({
        ignore: []
    }); 

    if ($(form).valid()) {

        var address = {
            City: $("#city").val(),
            State: $("#state").val(),
            PostalCode: $("#postalCode").val(),
            Country: $("#country").val(),
            Street: $("#street").val()
        };
        var contactAddress = {
            City: $("#contactCity").val(),
            State: $("#contactState").val(),
            PostalCode: $("#contactPostalCode").val(),
            Country: $("#contactCountry").val(),
            Street: $("#contactStreet").val()
        };

        var conName = {
            Given: $("#contactName").val(),
            Family: $("#contactFamily").val()
        };

        var contactTelecoms = GetTelecoms('PatientContactTelecom');

        var contactPerson =
        {
            Relationship: $("#relationship").val(),
            Name: {
                Given: $("#contactName").val(),
                Family: $("#contactFamily").val()
            },
            Address: contactAddress,
            Gender: $("#contactGender").val(),
            Telecoms: contactTelecoms
        };

        var request = {};

        request['Id'] = $("#patientId").val();
        request['Activity'] = $("#activity").val();
        request['Name'] = $("#name").val();
        request['FamilyName'] = $('#familyName').val();
        request['Name'] = $("#name").val();
        request['Gender'] = $("#gender").val();
        request['BirthDate'] = $("#birthDate").val() ? new Date($("#birthDate").val()).toUTCString() : "";
        request['LastUpdate'] = $("#lastUpdate").val();
        request['MultipleBirth'] = $("#multipleBirth").val();
        request['MultipleBirthNumber'] = $("#multipleBirthNumber").val();
        request['Address'] = address;
        request['ContactPerson'] = contactPerson;
        request['Telecoms'] = GetTelecoms('PatientTelecom');
        request['Language'] = $("#language").val();
        request['UniqueMasterCitizenNumber'] = $("#umcn").val();
        request['Identifiers'] = GetIdentifiers();
        request['Communications'] = getCommunications();

        $.ajax({
            type: "POST",
            url: "/Patient/Create",
            data: request,
            success: function (data) {
                $('#episodeOfCares').show();
                toastr.options = {
                    timeOut: 100
                }
                toastr.options.onHidden = function () { window.location.href = `/Patient/GetAll`; }
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });

    }
    var errors = $('.error').get();
    if (errors.length !== 0) {
        $.each(errors, function (index, error) {
            $(error).closest('.collapse').collapse("show");
        });
    };

    return false;
}

function selectChanged()
{
    if ($('#multipleBirth1').is(':visible')) {
        $('#multipleBirth1').hide();
        $('#multipleBirthNumber').val(1);
    }
    else
    {
        $('#multipleBirth1').show();
    }
}

function goToAllPatient() {
    window.location.href = "/Patient/GetAll";
}

function cancelPatientEdit() {
    window.location.href = '/Patient/GetAll';
}

function getCommunications() {
    let result = [];
    let selected = $('input[name=radioPreferred]:checked').val();

    $('input[name=radioPreferred]').each(function (index, element) {
        result.push({ preferred: $(element).val() == selected ? true : false, language: $(element).val() })
    })

    return result;
}

$(document).on('click', '.button-plus-language', function (e) {
    if (ValidateLanguage() && $('#language').val()) {
        let language = document.createElement('td');
        $(language).attr("data-property", 'language');
        $(language).attr("data-value", $('#language').val());
        $(language).html($('#language option:selected').text());

        let input = document.createElement('input');
        $(input).addClass("form-radio-field");
        $(input).attr("value", $('#language').val());
        $(input).attr("name", 'radioPreferred');
        $(input).attr("type", 'radio');

        if ($('#tableBody').find('input:radio[name=radioPreferred]').length == 0) {
            $(input).attr('checked', true);
        }

        let preferred = document.createElement('td');
        $(preferred).attr("data-property", 'preferred');
        $(preferred).attr("data-value", $('#preferred').val());

        $(preferred).append(input);

        let trElement = document.createElement('tr');
        $(trElement).addClass("tr edit-raw");

        $(trElement).append(language).append(preferred).append(createRemoveLanguageButton());

        $("#tableBody").append(trElement);
    }
});

function ValidateLanguage() {
    var isValid = true;
    let language = $("#language").val();

    $(`#communicationTable > tbody > tr`).each(function () {
        if ($(this).find("td:eq(0)").data('value') == language) {
            isValid = false;
            toastr.error(`This language already added`);
        }

    });
    return isValid;
}

function createRemoveLanguageButton() {
    let div = document.createElement('td');
    $(div).addClass('remove-language');

    let i = document.createElement('i');
    $(i).addClass('fas fa-times');

    $(div).append(i);
    return div;
}


$(document).on('click', '.remove-language', function (e) {
    $(e.currentTarget).closest('tr').remove();
});

function removeLanguage(r) {
    $(r).closest("tr").remove();
}

$(document).ready(function () {
    var url = new URL(window.location.href);
    var patientId = url.searchParams.get("patientId");

    if (patientId) {
        PatientId = patientId;
    } else {
        PatientId = null;
    }
});

function pushStateWithoutFilter(num) {
    if (PatientId) {
        history.pushState({}, '', `?patientId=${PatientId}&page=${num}&pageSize=${getPageSize()}`);
    } else {
        history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
    }
}



