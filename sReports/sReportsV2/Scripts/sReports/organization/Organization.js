function clickedSubmit() {
    $('#idOrganization').validate();

    if ($('#idOrganization').valid()) {
        let fileData = getFileData();
        if (fileData.length > 0) {
            sendFileData(fileData, setImageUrl, submitOrganizationForm, '/Blob/UploadLogo');
        } else {
            submitOrganizationForm();
        }
    }
    return false;
}

function submitOrganizationForm() {
    $.ajax({
        type: "POST",
        url: "/Organization/Create",
        data: getOrganizationData(),
        success: function (data) {
            toastr.options = {
                timeOut: 100
            }
            toastr.options.onHidden = function () { window.location.href = `/Organization/GetAll`; }
            toastr.success("Success");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getOrganizationData() {
    var request = {};
    var address = {
        Id: $('#addressId').val(),
        City: $("#city").val(),
        State: $("#state").val(),
        PostalCode: $("#postalCode").val(),
        CountryId: $("#countryId").val(),
        Street: $('#street').val()
    };

    request['Type'] = getSelectedTypes('.chk');
    request['Id'] = $("#id").val();
    request['Activity'] = $("#activity").val();
    request['Name'] = $("#name").val();

    request['Alias'] = $("#alias").val();
    request['Telecom'] = GetTelecoms('OrganizationTelecom');
    request['Identifiers'] = GetIdentifiers();
    request['AddressId'] = $('#addressId').val(),
    request['Address'] = address;
    request['ParentId'] = $("#parentId").val();
    request['PrimaryColor'] = $("#primaryColor").val();
    request['SecondaryColor'] = $("#secondaryColor").val();
    request['LogoUrl'] = $("#logoUrl").val();
    request['Email'] = $("#email").val();
    request['RowVersion'] = $("#rowVersion").val();
    request['ClinicalDomain'] = setClinicalDomain();
    request['Impressum'] = $("#impressum").val();

    return request;
}

function getSelectedTypes(selector) {
    var chkArray = [];

    $(`${selector}:checked`).each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function reloadHierarchy() {
    let parentId = $('#parentId').val() ? $('#parentId').val() : '';
    if ($("#name").val()) {
        $.ajax({
            type: 'GET',

            url: `/Organization/ReloadHierarchy?parentId=${parentId}`,
            success: function (data) {
                let content = $(data);
                let name = $(content).find('#organizationHierarchyActiveName')[0];
                $(name).html(getNameAndCity());
                $("#organizationHierarchyContainer").html($(content));
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    } else {
        $("#organizationHierarchyContainer").html($(appendPlaceholder()));
    }
}

function appendPlaceholder() {
    let element = document.createElement('div');
    $(element).addClass("no-result-content");
    let noResultElement = document.createElement('img');
    var noResultIcon = document.getElementById("notFound").src;
    $(noResultElement).attr("src", noResultIcon);
    let brElement = document.createElement('br');
    let noResult = document.createElement('div');
    $(noResult).addClass("no-result-text");
    var noResultFoundText = noResultFound;
    $(noResult).append(noResultFoundText);
    $(element).append(noResultElement).append(brElement).append(noResult);

    return element;
}

function getNameAndCity() {
    let name = $('#name').val();
    let city = $('#city').val();
    return `${name} ${city ? ', '+ city : ''}`;

}

$('#name').on('blur', function (e) {
    reloadHierarchy();
});

$('#parentId').on('change', function (e) {
    reloadHierarchy();
});

$('#city').on('blur', function (e) {
    reloadHierarchy();
});

$('#parentId').on('select2:opening', function (e) {
    $(this).addClass('focused');
});

$('#parentId').on('select2:closing', function (e) {
    $(this).removeClass('focused');
});

function cancelOrganization() {
    window.location.href = '/Organization/GetAll';
}

$(document).on('click', '#primaryColorInput', function (e) {
    $('#primaryColor').click();
});

$(document).on('change', '#primaryColor', function (e) {
    var color = $('#primaryColor').val();
    $('#colorPrimary').css('background-color', color);
});

$(document).ready(function () {
    var color = $('#primaryColor').val();
    $('#colorPrimary').css('background-color', color);
    var color = $('#secondaryColor').val();
    $('#colorSecondary').css('background-color', color);
});

$(document).on('click', '#secondaryColorInput', function (e) {
    $('#secondaryColor').click();
});

$(document).on('change', '#secondaryColor', function (e) {
    var color = $('#secondaryColor').val();
    $('#colorSecondary').css('background-color', color);
});

function setCountryAutocomplete() {
    $.ajax({
        method: 'get',
        url: `/FormConsensus/GetMapObject`,
        contentType: 'application/json',
        success: function (data) {
            setAutocompleteByData(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function setAutocompleteByData(data) {
    world = JSON.parse(data);
    let countries = topojson.feature(world, world.objects.countries);
    let countriesNames = countries.features.map(function (v) {
        return v.properties.name;
    });

    $("#country").autocomplete({
        source: countriesNames
    });
}

$(document).on("change", "#uploadLogo", function () {
    var fileInput = this;
    if (fileInput.files[0]) {
        var file = fileInput.files[0];
        var type = file.type;
        if (validUploadFormat(type)) {
            var fileName = $(fileInput).val().split("\\")[2];
            $('.file-name-display')
                .text(fileName)
                .attr('title', fileName);
            $('#logo-action-btns').removeClass("d-none");
        } else {
            removeLogo();
            $(fileInput).closest('.file-field').addClass('error');
            $("#logoUrlError").text("Logo url is invalid, allowed extensions are: jpg, jpeg, bmp, png, svg");
            setTimeout(function () {
                $(fileInput).closest('.file-field').removeClass('error');
                $("#logoUrlError").text("");
            }, 2000);
        }
    }
});

function validUploadFormat(type) {
    var allowedImageFormats = [
        'image/svg+xml',
        'image/png',
        'image/jpeg',
        'image/bmp'
    ];

    return allowedImageFormats.includes(type);
}

function removeLogo() {
    $('.file-name-display')
        .text('')
        .attr('title', '');
    $('#logoUrl').val('');
    $('#uploadLogo').val('');
    $('#logo-action-btns').addClass("d-none");
}

$(document).on("click", ".upload-logo-btn", function () {
    $("#uploadLogo").click();
});

function getFileData() {
    let filesData = [];
    $.each($('#idOrganization').find('input[type="file"]'), function (index, value) {
        if (value.files[0]) {
            filesData.push({ id: $(value).data('id'), content: value.files[0] });
        }
    });
    return filesData;
}

//function impressumWordCounter()
$('#impressum').on('keyup', function (e) {
    let charCount = $('#impressum').val().length;

    $('.char-limit-text').html(`${charCount}/600`);
});
