var PatientId;
var religionId;
/*-------------------Episode of care------------------------*/

function newEpisodeOfCare() {
    history.pushState({}, '', `?patientId=${$('#patientId').val()}`);
    window.location.href = `/EpisodeOfCare/Create?System=O4MtPatientId&Value=${$('#patientId').val()}`;
}

function editEntity(event,id) {
    window.location.href = `/EpisodeOfCare/Edit?EpisodeOfCareId=${id}`;
    event.preventDefault();
}

function removeEOCEntry(event, id) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/EpisodeOfCare/DeleteEOC?eocId=${id}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success(`Success`);
            //$(event.srcElement).parent().parent().parent().parent().remove();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}
/*-------------------Episode of care------------------------*/

function submitForm(form,e) {
    $(form).validate({
        ignore: []
    }); 
    if ($(form).valid()) {
        var contactAddress = {
            City: $("#contactCity").val(),
            State: $("#contactState").val(),
            PostalCode: $("#contactPostalCode").val(),
            Country: $("#contactCountry").val(),
            Street: $("#contactStreet").val()
        };

        var contactName = {
            Given: $("#contactName").val(),
            Family: $("#contactFamily").val()
        };

        var contactTelecoms = GetTelecoms('PatientContactTelecom');

        var contactPerson =
        {
            Relationship: $("#relationship").val(),
            Name: contactName,
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
        request['BirthDate'] = toDateStringIfValue($("#birthDate").val());
        request['Deceased'] = $("#deceased").is(":checked");
        request['DeceasedDateTime'] = toDateStringIfValue($("#deceasedDateTime").val());
        request['ReligionId'] = $("#religion").val();
        request['CitizenshipId'] = $("#citizenship").val();
        request['MultipleBirth'] = $("#multipleBirth").val();
        request['MultipleBirthNumber'] = $("#multipleBirthNumber").val();
        request['Addresses'] = getAddresses();
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
                toastr.options.onHidden = function () { window.location.href = `/Patient/Edit?patientId=${data.Id}`; }
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
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

function deceasedChanged(isDeceasedChecked) {
    var $deceasedDateTimeContainer = $("#deceasedDateTimeContainer");
    if (isDeceasedChecked) {
        $deceasedDateTimeContainer.show();
    } else {
        $deceasedDateTimeContainer.hide();
        $("#deceasedDateTime")
            .val("")
            .removeClass("error");
        $deceasedDateTimeContainer.find("label.error").remove();
    }
}

$(document).on("change", "#deceased", function () {
    deceasedChanged($(this).is(":checked"));
});

$(document).on("change", "#deceasedDateTime", function () {
    if ($(this).hasClass("error")) {
        var $deceasedDateTimeContainer = $("#deceasedDateTimeContainer");
        $(this).removeClass("error");
        $deceasedDateTimeContainer.find("label.error").remove();
    }
});

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
        result.push({
            preferred: $(element).val() == selected ? true : false,
            language: $(element).val(),
            id: $(element).attr("data-id")
        })
    })

    return result;
}

$(document).ready(function () {
    setPatientId();
    renderInitialData();
    setCommonValidatorMethods();
    setValidationFunctions();
});

function setPatientId() {
    var url = new URL(window.location.href);
    var patientId = url.searchParams.get("patientId");

    if (patientId) {
        PatientId = patientId;
    } else {
        PatientId = null;
    }
}

function renderInitialData() {
    $("input[name=radioPreferred]").each(function () {
        if ($(this).is(":checked")) {
            let preferredText = document.createElement('span');
            $(preferredText).addClass("preferred-text-class");
            preferredText.innerHTML = " (Preferred)";
            $(this).closest('div').addClass("preferred-language-text-active").append(preferredText);;
        } else {
            $(this).closest('div').removeClass("preferred-language-text-active");
        }
    });
    deceasedChanged($("#deceased").is(":checked"));

    initCustomEnumSelect2(hasSelectedReligion(), religionId, "religion", "religion", "ReligiousAffiliationType");
    initCustomEnumSelect2(false, 0, "countryId", "country", "Country");
}

function hasSelectedReligion() {
    return religionId;
}

function setValidationFunctions() {
    $.validator.addMethod(
        "deceasedDateTime",
        function (value, element) {
            if ($("#deceased").is(":checked")) {
                return $(element).val();
            } else {
                return true;
            }
        },
        "Please enter deceased datetime."
    );
    $('#idPatientInfo').validate({});
    $('[name="deceasedDateTime"]').each(function () {
        $(this).rules('add', {
            deceasedDateTime: true
        });
    });
}

$(document).on('click', '.plus-button', function (e) {
    if (ValidateLanguage() && $('#language').val()) {
        let language = createLanguageElement();
        let input = createRadioInput();

        let removeButton = createRemoveLanguageButton();
        removeButton.id = "removeButtonId";

        let preferredText = createPreferredText();

        if ($('#tableBody').find('input:radio[name=radioPreferred]').length == 0) {
            $(input).attr('checked', true);
            $(language).addClass("preferred-language-text-active");
            $(language).append(preferredText);
            $(removeButton).addClass("preferred-language-text-active");
        }

        $(removeButton).addClass("right-remove-button");

        let preferred = createRadioField();

        let preferred2 = document.createElement('div');
        $(preferred2).addClass("preferred-language-group");

        let divElement = document.createElement('div');

        $(preferred).append(input);
        $(preferred2).append(preferred).append(language).append(removeButton)

        $("#tableBody").append(preferred2).append(divElement);

    }
});

function createLanguageElement() {
    let language = document.createElement('span');
    $(language).attr("data-property", 'language');
    $(language).attr("data-value", $('#language').val());
    $(language).addClass("preferred-language-text");
    language.id = "firstLanguage";
    $(language).html($('#language option:selected').text());

    return language;
}

function createRadioInput() {
    let input = document.createElement('input');
    $(input).addClass("form-radio-field");
    $(input).attr("value", $('#language').val());
    $(input).attr("name", 'radioPreferred');
    $(input).attr("type", 'radio');
    $(input).attr("data-id", '0');
    $(input).attr("data-no-color-change", "true");

    return input;
}

function createPreferredText() {
    let preferredText = document.createElement('span');
    $(preferredText).addClass("preferred-text-class");
    preferredText.innerHTML = " (Preferred)";

    return preferredText;
}

function createRadioField() {
    let preferred = document.createElement('span');
    $(preferred).attr("data-property", 'preferred');
    $(preferred).attr("data-value", $('#preferred').val());
    $(preferred).addClass("radio-space");
    $(preferred).css("margin-right", "13px");

    return preferred;
}

function ValidateLanguage() {
    var isValid = true;
    let language = $("#language").val();

    $(`#tableBody > div`).each(function () {
        if ($(this).find("span:eq(1)").data('value') == language) {
            isValid = false;
            toastr.error(`This language already added`);
        }

    });
    return isValid;
}

function createRemoveLanguageButton() {
    let span = document.createElement('span');
    $(span).addClass('remove-language-button');

    let i = document.createElement('i');
    $(i).addClass('fas fa-times');

    $(span).append(i);
    return span;
}

$(document).on('click', '.remove-language-button', function (e) {
    var currentEl = e.currentTarget;
    $(currentEl).closest('div').remove();
});

function pushStateWithoutFilter(num) {
    if (PatientId) {
        history.pushState({}, '', `?patientId=${PatientId}&page=${num}&pageSize=${getPageSize()}`);
    } else {
        history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
    }
}

function showGeneralInfo(event, element){
    event.stopPropagation();
    setTagActiveClass(element);
    setTagIconActiveClass("general-icon");

    $("#contactPersonPartial").hide();
    $("#patientTelecomPartial").hide();
    $("#patientInfoPartial").show();
}

function showTelecomInfo(event, element) {
    event.stopPropagation();
    setTagActiveClass(element);
    setTagIconActiveClass("telecom-icon");

    $("#contactPersonPartial").hide();
    $("#patientTelecomPartial").show();
    $("#patientInfoPartial").hide();
}

function showContactPerson(event, element) {
    event.stopPropagation();
    setTagActiveClass(element);
    setTagIconActiveClass("contact-icon");

    $("#contactPersonPartial").show();
    $("#patientTelecomPartial").hide();
    $("#patientInfoPartial").hide();
}

function setTagActiveClass(element) {
    $('.tab-item').removeClass('active');
    $(element).addClass('active');
}

function setTagIconActiveClass(iconId) {
    $('.tab-icon').removeClass('active');
    document.getElementById(iconId).classList.add('active');
}

$(document).on('click', '[name="radioPreferred"]', function () {
    let selected = $('input[name=radioPreferred]:checked').val();
    var element = document.getElementById("firstLanguage");
    element.classList.remove("preferred-language-text-active");
    var removeElement = document.getElementById("removeButtonId");
    if (removeElement) {
        removeElement.classList.remove("preferred-language-text-active");
        removeElement.classList.add("right-remove-button");
    }
    
    let preferredText = document.createElement('span');
    $(preferredText).addClass("preferred-text-class");
    preferredText.innerHTML = " (Preferred)";

    $(`#tableBody > div`).each(function () {
        $(this).removeClass("preferred-language-text-active");
        $(this).find(".preferred-text-class").remove();

        if ($(this).find("span:eq(1)").data('value') == selected) {
            $(this).find("span:eq(1)").append(preferredText);
            $(this).addClass("preferred-language-text-active");
        }
    });
});