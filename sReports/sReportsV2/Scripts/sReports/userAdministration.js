var activeContainerId = "personalData";
var isUserAdministration;
var isReadOnly;
var userCountryId;

function setUserAdministration(userAdministration) {
    isUserAdministration = userAdministration;
}

function setReadOnly(readOnly) {
    isReadOnly = readOnly;
}

function submitForm(form, e) {
    e.preventDefault();
    e.stopPropagation();
    submitData();
}

$(document).on("keypress", ".clinical-trial", function (e) {
    var key = e.which;
    if (key == 13)
    {
        let clinicalTrialId = $(this).attr('data-id');
        submitClinicalTrial(e, clinicalTrialId);
    }
});

function submitPersonalData() {
    let form = $("#idUserInfo");
    $(form).validate({
        ignore: []
    });

    if ($(form).valid() && $("#userInfo").find('.fa-times-circle').length === 0) {
        var request = {};

        request['Id'] = $("#userId").val();
        request['Username'] = $("#username").val();
        request['FirstName'] = $("#firstName").val();
        request['LastName'] = $("#lastName").val();
        request['Prefix'] = $('input[name=prefix]:checked').val();
        if (!validateEmailInput(request, "email")) return false;
        if (!validateEmailInput(request, "personalEmail")) return false;

        request["MiddleName"] = $("#middleName").val();
        request["AcademicPositions"] = getSelectedAcademicPositions();
        request["Address"] = getUserAddress();
        request["DayOfBirth"] = toDateStringIfValue($("#dayOfBirth").val());
        request["Roles"] = getUserRoles();

        $.ajax({
            type: "POST",
            url: `/UserAdministration/${isUserAdministration ? 'Create' : 'UpdateUserProfile'}`,
            data: request,
            success: function (data) {
                updateSystemIdAndDisplay(+request["Id"], data.Id);
                updateIdAndRowVersion(data);
                toastr.success(data.Message);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
        return true;

    }
    var errors = $('.error').get();
    if (errors.length !== 0) {
        $.each(errors, function (index, error) {
            $(error).closest('.collapse').collapse("show");
        });
    };

    return false;
}

function updateSystemIdAndDisplay(isEdit, systemId) {
    if (!isEdit) {
        $("#systemId").val(systemId);
        $("#systemIdInput").removeClass("d-none");
    }
}

function getOrganizations() {
    let institutions = [];
    $("#institutions").find('.institution-container').each(function (index, element) {
        let organizationId = $(element).attr('id').split('-')[1];
        institutions.push(getOrganization(organizationId));
    });

    return institutions;
}

function getOrganization(organizationId) {
    let institution = {};
    institution["IsPracticioner"] = $(`#isPractitioner-${organizationId}:checked`).val();
    institution["Qualification"] = $(`#qualification-${organizationId}`).val();
    institution["SeniorityLevel"] = $(`#seniority-${organizationId}`).val();
    institution["Speciality"] = $(`#speciality-${organizationId}`).val();
    institution["SubSpeciality"] = $(`#subspeciality-${organizationId}`).val();
    institution["OrganizationId"] = organizationId;
    institution["State"] = $(`#organizationState-${organizationId}`).val();

    return institution;
}

function getUserAddress() {
    let address = {};
    address["CountryId"] = $("#countryId").val();
    address["State"] = $("#state").val();
    address["City"] = $("#city").val();
    address["PostalCode"] = $("#postalCode").val();
    address["Street"] = $("#street").val();

    return address;
}

function getSelectedAcademicPositions() {
    var chkArray = [];

    $(".academic-position:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function getUserRoles() {
    var chkArray = [];

    $(".user-role:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function cancelUserEdit() {
    window.location.href = isUserAdministration ? '/UserAdministration/GetAll' : '/Home/Index'
}

function getSelectedOrganizations() {
    var chkArray = [];

    $(".chk:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

$(document).ready(function () {
    $('.vertical-line-user').each(function (index, element) {
        let count = $(element).closest('.child').children('.child').length;
        if (count == 1) {
            $(element).css('height', '26px');
        }
    });

    $(".chk:checkbox").each(function () {
        if ($(this).is(":checked")) {
            $(this).closest('div').addClass("organization-span-active");
        } else {
            $(this).closest('div').removeClass("organization-span-active");
        }
    });
    initCustomEnumSelect2(userCountryId, userCountryId, "countryId", "country", "Country");
});

$(document).on('click', '.organization-span', function (e) {
    e.stopPropagation();
    e.preventDefault();

    if ($("#readonlyField").val() != "True") {
        $('input[type="checkbox"]', this).attr('checked', true);
        $(".chk:checkbox").each(function () {
            if ($(this).is(":checked")) {
                $(this).closest('div').addClass("organization-span-active");
            } else {
                $(this).closest('div').removeClass("organization-span-active");
            }
        });
    }
})

$(document).on('click', '.organization-span-active', function (e) {
    e.stopPropagation();
    e.preventDefault();
    if ($("#readonlyField").val() != "True") {
        $('input[type="checkbox"]', this).attr('checked', false);
        $(this).closest('div').removeClass("organization-span-active");
    }
});

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

$(document).on('change', '#username', function (e) {
    e.stopPropagation();
    e.preventDefault();
    let username = $(this).val();
    let currentUsername = $("#currentUsername").val();


    $.ajax({
        type: "Get",
        url: `/UserAdministration/CheckUsername?username=${username}&currentUsername=${currentUsername}`,
        success: function (data) {
            $("#usernameValid").addClass("fa-check-circle");
            $("#usernameValid").removeClass("fa-times-circle");

        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#usernameValid").addClass("fa-times-circle");
            $("#usernameValid").removeClass("fa-check-circle");

        }
    });

});

$(document).on('change', '#email', function (e) {
    e.stopPropagation();
    e.preventDefault();
    let email = $(this).val();
    let currentEmail = $("#currentEmail").val();

    if (validateEmail(email)) {
        $.ajax({
            type: "Get",
            url: `/UserAdministration/CheckEmail?email=${email}&currentEmail=${currentEmail}`,
            success: function (data) {
                $("#emailValid").addClass("fa-check-circle");
                $("#emailValid").removeClass("fa-times-circle");
                $("#email").removeClass("error");
                $("#email-error").remove();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $("#emailValid").addClass("fa-times-circle");
                $("#emailValid").removeClass("fa-check-circle");

            }
        });
    }
});

$(document).on('click', '.user-tab', function (e) {
    
    let isSuccess = submitData();

    if (isSuccess) {
        $('.user-tab').removeClass('active');

        $(this).addClass('active');
        $('.user-cont').hide();

        let containerId = $(this).attr("data-id");
        toggleSaveBtn(containerId);

        activeContainerId = containerId;
        $(`#${containerId}`).show();
        handleArrowVisibility(activeContainerId);
    }

});

function toggleSaveBtn(containerId) {
    if (containerId === "clinicalData") {
        $(`#buttonGroupPrimary`).hide();
    } else if (containerId === "institutionData") {
        $(`#buttonGroupPrimary`).show();
        if (isUserAdministration) {
            $(`#buttonGroupPrimary`).find("button").show();
        } else {
            $(`#buttonGroupPrimary`).find("button").hide();
        }
    } else {
        $(`#buttonGroupPrimary`).show();
        $(`#buttonGroupPrimary`).find("button").show();
    }
}

function handleArrowVisibility(activeContainerId) {
    switch (activeContainerId) {
        case "personalData": {
            $('.user-arrow-right').show();
            $('.user-arrow-left').hide();
            return true;
        }
        case "institutionData": {
            $('.user-arrow-right').show();
            $('.user-arrow-left').show();
            return true;
        }
        case "clinicalData": {
            $('.user-arrow-right').hide();
            $('.user-arrow-left').show();
            return true;
        }
        default:
    }
}

function submitData() {
    if (isReadOnly) return true;
    switch (activeContainerId) {
        case "personalData":
            return submitPersonalData();
        case "institutionData":
            return submitInstitutionalData();
        case "clinicalData":
            return true;//submitClinicalData();
        default:
        // code block
    }}



function submitInstitutionalData() {
    if (isUserAdministration) {
        var request = {};

        request['Id'] = $("#userId").val();
        request['RowVersion'] = $("#RowVersion").val();
        request["UserOrganizations"] = getOrganizations();

        $.ajax({
            type: "POST",
            url: "/UserAdministration/UpdateUserOrganizations",
            data: request,
            success: function (data) {
                updateIdAndRowVersion(data);
                toastr.success(data.Message);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
    
    return true;
}

function updateIdAndRowVersion(data) {
    $("#userId").val(data["Id"]);
    $("#RowVersion").val(data["RowVersion"]);
}

function submitClinicalData() {
    var request = {};

    request['Id'] = $("#userId").val();
    request['RowVersion'] = $("#RowVersion").val();
    request["ClinicalTrials"] = getClinicalTrials();

    $.ajax({
        type: "POST",
        url: "/UserAdministration/UpdateUserClinicalTrials",
        data: request,
        success: function (data) {
            updateIdAndRowVersion(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });

    return true;
}

function getClinicalTrials() {
    let clinicalTrials = [];
    $("#clinicalTrials").find('.clinical-trial').each(function (index, element) {
        let clinicalTrialId = $(element).attr('data-id');
        clinicalTrials.push(getClinicalTrial(clinicalTrialId));
    });

    return clinicalTrials;
}

function collapseChapter(element) {
    let id = $(element).data('target');
    if ($(`${id}`).hasClass('show')) {
        $(`${id}`).collapse('hide');
        $(element).children('.institution-icon').removeClass('fa-angle-up');
        $(element).children('.institution-icon').addClass('fa-angle-down');

    } else {
        $(`${id}`).collapse('show');
        $(element).children('.institution-icon').removeClass('fa-angle-down');
        $(element).children('.institution-icon').addClass('fa-angle-up');
    }

}

function openInstitutionModal(e) {
    e.stopPropagation();
    e.preventDefault();

    $('#institutionModal').modal('show');
}

function addNewOrganizationData() {
    let organizationId = $("#newOrganization").val();
    if (organizationId) {
        let organizationIds = [];
        $('.institution-container').each(function (index, element) {
            organizationIds.push($(element).attr('id').split('-')[1]);
        });
        request = {};
        request["OrganizationsIds"] = organizationIds;
        request["OrganizationId"] = organizationId;

        $.ajax({
            type: "post",
            url: `/UserAdministration/LinkOrganization`,
            data: request,
            success: function (data) {
                $("#institutions").find(".no-result-content").hide();
                $("#institutions").append(data);
                $('#institutionModal').modal('hide');
                submitData();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    } else {
        toastr.warning("You have no organization selected yet!")
    }
}

function submitClinicalTrial(event, id) {
    event.preventDefault();
    event.stopPropagation();

    var request = {};
    request["ClinicalTrial"] = getClinicalTrial(id);
    request["UserId"] = $("#userId").val();
    request['RowVersion'] = $("#RowVersion").val();


    $.ajax({
        type: "POST",
        url: `/UserAdministration/SubmitClinicalTrial`,
        data: request,
        success: function (data, textStatus, xhr) {
            $("#clinicalData").html(data);
            //updateIdAndRowVersion(data);
            let clinicalTrialsMessage = $("#clinicalData").find("#clinicalTrialsMessage");
            toastr.success(clinicalTrialsMessage.text());
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}


function getClinicalTrial(id) {
    request = {};

    request['Id'] = id;
    request['Name'] = $(`#name-${id}`).val();
    request['Acronym'] = $(`#acronym-${id}`).val();
    request['SponosorId'] = $(`#sponosorId-${id}`).val();
    request['WemedooId'] = $(`#wemedooId-${id}`).val();
    request['Status'] = $(`#status-${id}:checked`).val();
    request['Role'] = $(`#role-${id}`).val();
    request['IsArchived'] = $(`#isArchived-${id}`).val();
    request['UserId'] = $("#userId").val();

    return request;
}

$(document).on('change', '.ct-name', function (e) {
    $(this).closest('.single-ct').find('.institution-header-name').text($(this).val());
});

$(document).on('change', '.ct-role', function (e) {
    var text = $(this).find(":selected").data("display");
    $(this).closest('.single-ct').find('.header-role-value').text(text);
});

$(document).on('click', '.ct-status', function (e) {
    var text = $(this).data("display");
    $(this).closest('.single-ct').find('.ct-status-value').text(text);
});


function archiveClinicalTrial(id) {
    let request = {};
    request["RowVersion"] = $("#RowVersion").val();
    request["UserId"] = $("#userId").val();
    request["ClinicalTrialId"] = id;
    $.ajax({
        type: "POST",
        url: `/UserAdministration/ArchiveClinicalTrial`,
        data: request,
        success: function (data) {
            updateIdAndRowVersion(data);
            let archivedItem = $('#notArchivedTrials').find(`[data-ct-id='${id}']:first`);
            $(`#isArchived-${id}`).val(true);
            $(`#ctBtnArchive-${id}`).hide();
            $("#archivedTrials").append(archivedItem);
            toastr.success(data.Message);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

$(document).on('click', '.ct-tab', function (e) {
    $('.ct-tab').each(function (index, element) {
        $(element).removeClass('active');
        if ($(element).hasClass('trials')) {
            $(element).find('img').attr('src', '/Content/img/icons/clinical_trials_black.svg');
        }
        if ($(element).hasClass('archived-trials')) {
            $(element).find('img').attr('src', '/Content/img/icons/archive_black.svg');
        }
    });

    $(this).addClass('active');
    if ($(this).hasClass('trials')) {
        $(this).children("img:first").attr('src', '/Content/img/icons/clinical_trials_green.svg');
        $('#notArchivedTrials').show();
        $('#archivedTrials').hide();
    }
    if ($(this).hasClass('archived-trials')) {
        $(this).children("img:first").attr('src', '/Content/img/icons/archive_green.svg');
        $('#notArchivedTrials').hide();
        $('#archivedTrials').show();
    }

});

$(document).on('click', '.user-arrow-left', function (e) {
    $('.user-tab').each(function (index, element) {
        if ($(element).hasClass('active')) {
            $(element).prev().click();
            return false;
        }
    });
});

$(document).on('click', '.user-arrow-right', function (e) {
    $('.user-tab').each(function (index, element) {
        if ($(element).hasClass('active')) {
            $(element).next().click();
            return false;
        }
    });
});

function cancelClinicalTrial(id, event) {
    let clinicalTrial = $(event.currentTarget).closest('.trial-container'); 

    $.ajax({
        type: "GET",
        url: `/UserAdministration/ResetClinicalTrial?clinicalTrialId=${id}&userId=${$("#userId").val()}`,
        success: function (data) {
            let test = $(event.currentTarget);
            $(clinicalTrial).html(data);
            if (id) {
                reloadTrialHeader(id, clinicalTrial);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function reloadTrialHeader(id) {
    $(`#ctHeader-${id}`).find('.institution-header-name').text($(`#name-${id}`).val());
    $(`#ctHeader-${id}`).find('.header-role-value').text($(`#role-${id}`).val());
    $(`#ctHeader-${id}`).find('.ct-status-value').text($(`#status-${id}:checked`).val());

}

function validateEmailInput(request, inputName, required = false) {
    if (validateEmail($(`#${inputName}`).val()) || !required) {
        request[`${capitalizeFirstLetter(inputName)}`] = $(`#${inputName}`).val();
        return true;
    }
    else {
        $(`#${inputName}`).addClass("error");
        $(`#${inputName}`).after(`<label id=\"${inputName}-error\" class=\"error\" for=\"${inputName}\">Please enter a valid email address.</label>`);
        return false;
    }
}

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}