function submitOrganizationForm() {
    $('#idOrganization').validate();

    if ($('#idOrganization').valid()) {
        var request = {};
        var address = {
            Id: $('#addressId').val(),
            City: $("#city").val(),
            State: $("#state").val(),
            PostalCode: $("#postalCode").val(),
            Country: $("#country").val(),
            Street:$('#street').val()
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
        request['ParentId'] = $("#parentId").val() ? $("#parentId").val().trim() : '';
        request['PrimaryColor'] = $("#primaryColor").val();
        request['SecondaryColor'] = $("#secondaryColor").val();
        request['LogoUrl'] = $("#logoUrl").val();
        request['Email'] = $("#email").val();
        request['RowVersion'] = $("#rowVersion").val();
        request['ClinicalDomain'] = setClinicalDomain();
        
        $.ajax({
            type: "POST",
            url: "/Organization/Create",
            data: request,
            success: function (data) {
                toastr.options = {
                    timeOut: 100
                }
                toastr.options.onHidden = function () { window.location.href = `/Organization/GetAll`; }
                toastr.success("Success");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });

    }
    return false;
}

function getSelectedTypes(selector) {
    var chkArray = [];

    $(`${selector}:checked`).each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function checkLogoUrl() {
    let result = false;
    var arr = ["jpg", "jpeg", "bmp", "png"];
    var ext = $('#logoUrl').val().substring($('#logoUrl').val().lastIndexOf(".") + 1);
    if ($('#logoUrl').val() !== '' && arr.find(x => x == ext) == null) {
        result = true;
    }
    return result;
}

function cancelOrganizationEdit() {
    window.location.href = `/Organization/GetAll`;
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
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
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

$(document).ready(function () {
    jQuery.validator.addMethod("invalidLogoUrl", function (value, element) {
        return !checkLogoUrl();
    }, "Logo url is invalid, allowed extensions are: jpg, jpeg, bmp, png");


    $("#idOrganization").validate({
        onkeyup: false,
        rules: {
            LogoUrl: {
                invalidLogoUrl: true
            }
        },
        messages: {
            LogoUrl: {
                remote: "Logo url is invalid, allowed extensions are: jpg, bmp, png"
            }
        }
    });
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
});

$(document).on('click', '#secondaryColorInput', function (e) {
    $('#secondaryColor').click();
});

$(document).on('change', '#secondaryColor', function (e) {
    var color = $('#secondaryColor').val();
    $('#colorSecondary').css('background-color', color);
});

$(document).ready(function () {
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
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
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