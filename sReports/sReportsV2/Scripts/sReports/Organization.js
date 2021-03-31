function submitOrganizationForm(form) {
    $('#idOrganization').validate();

    if ($(form).valid()) {
        var request = {};
        var address = {
            City: $("#city").val(),
            State: $("#state").val(),
            PostalCode: $("#postalCode").val(),
            Country: $("#country").val(),
            Street:$('#street').val()
        };

        request['Type'] = getSelectedTypes();
        request['Id'] = $("#id").val();
        request['Activity'] = $("#activity").val();
        request['Name'] = $("#name").val();

        request['Alias'] = $("#alias").val();
        request['Telecom'] = GetTelecoms('OrganizationTelecom');
        request['Identifiers'] = GetIdentifiers();
        request['Address'] = address;
        request['PartOf'] = $("#partOf").val() ? $("#partOf").val().trim() : '';
        request['PrimaryColor'] = $("#primaryColor").val();
        request['SecondaryColor'] = $("#secondaryColor").val();
        request['LogoUrl'] = $("#logoUrl").val();
        request['LastUpdate'] = $("#lastUpdate").val();
        
        $.ajax({
            type: "POST",
            url: "/Organization/Create",
            data: request,
            success: function (data) {
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

function getSelectedTypes() {
    var chkArray = [];

    $(".chk:checked").each(function () {
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
    if ($("#partOf").val() && $("#partOf").val().trim() && $("#name").val()) {
        $.ajax({
            type: 'GET',

            url: `/Organization/ReloadHierarchy?partOf=${$('#partOf').val()}`,
            success: function (data) {
                let content = $(data);
                let name = $(content).find('#organizationHierarchyActiveName')[0];
                $(name).html($('#name').val());
                let city = $(content).find('#organizationHierarchyActiveCity')[0];
                $(city).html($('#city').val());
                $("#organizationHierarchyContainer").html($(content));
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
            }
        });
    } else {
        $("#organizationHierarchyContainer").html('');
    }
}

$('#name').on('blur', function (e) {
    reloadHierarchy();
});

$('#partOf').on('change', function (e) {
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

$('#partOf').on('select2:opening', function (e) {
    $(this).addClass('focused');
});

$('#partOf').on('select2:closing', function (e) {
    $(this).removeClass('focused');
});
