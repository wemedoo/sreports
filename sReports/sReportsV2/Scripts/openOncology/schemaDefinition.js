(function ($) {
    //"use strict";
    ///////////
    // TREATMENT CYCLES  -  Show more options for LIMITED therapy
    ///////////
    $('input[name="limited"]').on('click', function (e) {
        if ($('#limited').prop('checked')) {
            $('.select-limited').fadeIn(150);
        } else {
            $('.select-limited .select-input').val("");
            $('.select-limited').fadeOut(150);
        }
    });
    ///////////////////////////////////////////

    //Hide modals
    $('.modal-indication .close-modal').on('click', function () {
        closeModal('.modal-indication');
        $('#indication-values').empty();
    });
    $('.modal-schema-name .close-modal').on('click', function () {
        closeModal('.modal-schema-name');
        $('#schemaNameInput').val('');
    });
    
    ///////////////////////////////////////////

    //Indication - modal
    $('#edit-indication').on('click', function () {
        $('.modal-indication').css('display', 'block').trigger('lowZIndex');
        $('body').addClass('modal-active');
        let arr = [];
        $('.indication-body p').each(function () {
            let indicationText = $(this).text();
            let el = { "id": $(this).attr('data-id'), "name": indicationText };
            arr.push(el);
        });
        arr.forEach(el => {
            $('#indication-values').append(createSingleTag(el['name'], el['id']));
        });
        if (arr.length > 0) {
            $(".modal-indication .divider").removeClass("no-background");
        } else {
            $(".modal-indication .divider").addClass("no-background");
        }
    });

    $('#save-indication').on('click', function (e) {
        saveIndications();
    });

    function saveIndications() {
        if (!isSchemaNameSpecified()) return;

        let arr = [];
        
        arr = getTagValues();

        closeModal('.modal-indication');
        $(".add-new-indication").val('');
        $('#indication-values').empty();

        updateIndications(arr);
    }

    function getTagValues() {
        let result = [];
        $(`*[data-tag-info]`).each(function () {
            let el = { "Id": $(this).attr('data-tag-info'), "Name": $(this).html() };
            result.push(el);
        });
        return result;
    }

    $('.modal-indication').on('click', "*[data-action='remove-tag']", function (e) {
        $(this).parent().remove();
    });

    $('.add-item').on('click', function (e) {
        addIndicaitonItem($(this).parent().find('input'));
    });

    $('.add-new-indication').on('keypress', function (e) {
        if (e.which == 13) {
            addIndicaitonItem($(this));
        }
    });

    function addIndicaitonItem(input) {
        const inputVal = $(input).val().trim();

        let isAppended = appendTagToContainer(inputVal);
        if (isAppended) {
            $(input).val('');
        }
    }

    function appendTagToContainer(value) {
        if (!value || existsTagValue(value)) {
            return false;
        }
        $(`#indication-values`).append(createSingleTag(value, 0));
        return true;
    }

    function existsTagValue(value) {
        let id = value.replace(/\W/g, "-").toLowerCase();
        if ($(`#indication-values`).find(`#tag-indication-${id}`).length > 0) {
            return true;
        }
        return false;
    }

    function createSingleTag(value, entityId) {
        var elId = value.replace(/\W/g, "-").toLowerCase();
        var removeIcon = getNewRemoveIcon();

        var element = getNewSingleTagContainer(elId);
        $(element).append(getNewSingleTagValue(value, entityId));
        $(element).append(removeIcon);

        return element;
    }

    function getNewRemoveIcon() {
        var removeIcon = document.createElement('img');
        $(removeIcon).addClass('ml-2');
        $(removeIcon).addClass('tag-value');
        $(removeIcon).addClass('tag-value-synonym');
        $(removeIcon).attr("src", "/Content/img/icons/Administration_remove.svg");
        $(removeIcon).attr('data-action', `remove-tag`);
        return removeIcon;
    }

    function getNewSingleTagValue(value, entityId) {
        var text = document.createElement('span');
        $(text).addClass('single-tag-value');
        $(text).attr('data-tag-info', entityId);
        $(text).html(value);

        return text;
    }

    function getNewSingleTagContainer(name) {
        var element = document.createElement('div');
        $(element).attr("id", `tag-indication-${name}`);
        $(element).addClass('tags-element');
        $(element).addClass('synonyms-element');

        return element;
    }

    //Schema Name - modal
    $('#edit-schema-name').on('click', function () {
        $('.modal-schema-name').css('display', 'block').trigger('lowZIndex');
        $('body').addClass('modal-active');
        let schemaName = getSchemaName();
        $('#schemaNameInput').val(schemaName);
    });
    $('#save-schema-name').on('click', function (e) {
        var schemaName = $('#schemaNameInput').val();
        if (schemaName === '') {
            toastr.warning("Please enter schema name!");
            return;
        }
        $('.schema-name-body').removeClass('d-none');
        $('#schemaName').text(schemaName);
        closeModal('.modal-schema-name');
        $('#schemaNameInput').val('');
        updateSchemaName(schemaName);
    });
})(jQuery);

let modalEl;

//Literature reference - modal
$(document).on('click', '.edit-reference', function () {
    var referenceId = $(this).attr("data-id");
    $.ajax({
        type: "GET",
        url: `/SmartOncology/GetSchemaReference?id=${referenceId}`,
        success: function (data) {
            $('.modal-reference').css('display', 'block').trigger('lowZIndex');
            $('body').addClass('modal-active');
            $("#literatureReferenceModal").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
});

function closeModal(modalName) {
    $('body').removeClass('modal-active');
    $(modalName).css('display', 'none').trigger('defaultZIndex');
}

function updateSchemaName(schemaName) {
    let schemaId = getSchemaId();
    let request = {};
    request['Name'] = schemaName;
    request['Id'] = schemaId;
    request['RowVersion'] = getRowVersion();

    $.ajax({
        type: "POST",
        url: "/SmartOncology/UpdateSchemaName",
        data: request,
        success: function (data) {
            toastr.success("Success");
            updateSchemaDataOnUI(data.Id, data.RowVersion, schemaName);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function updateGeneralProperties() {
    if (!isSchemaNameSpecified()) return;

    let request = getGeneralProperties();
    $.ajax({
        type: "POST",
        url: "/SmartOncology/UpdateSchemaGeneralProperties",
        data: request,
        success: function (data) {
            toastr.success("Schema is updated sucessfully!");
            updateSchemaDataOnUI(data.Id, data.RowVersion);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getGeneralProperties() {
    let schemaId = getSchemaId();
    let request = {};
    request['Id'] = schemaId;
    request['LengthOfCycle'] = getLengthOfCycle();
    request['NumOfCycles'] = $("#cycles").val();
    request['AreCoursesLimited'] = $("input[name=limited]:checked").val();
    request['NumOfLimitedCourses'] = getLengthOfCourses(request['AreCoursesLimited']);
    request['RowVersion'] = getRowVersion();

    return request;
}

function getLengthOfCycle() {
    var weeks = $("#weeks").val() ? +$("#weeks").val() : 0;
    var days = $("#days").val() ? +$("#days").val() : 0;

    var lengthOfCycle = weeks * 7 + days;
    return lengthOfCycle;
}

function getLengthOfCourses(areCoursesLimited) {
    return areCoursesLimited === "true" ? $("#s-limited").val() : 0;
}

function isSchemaNameSpecified() {
    if (!getSchemaName()) {
        toastr.warning('Please enter schema name first');
        return false;
    } else {
        return true;
    }
}

function getSchemaName() {
    return $('#schemaName').text();
}

function getSchemaId() {
    return $('.chemotherapy-schema').attr('data-id');
}

function updateIndications(indications) {
    if (indications.length > 0) {
        $('.indication-body').removeClass('d-none');
    } else {
        $('.indication-body').addClass('d-none');
    }
    $('.indication-body').empty();

    let schemaId = getSchemaId();
    let request = {};
    request['Indications'] = indications;
    request['ChemotherapySchemaId'] = schemaId;
    request['RowVersion'] = getRowVersion();

    $.ajax({
        type: "POST",
        url: "/SmartOncology/UpdateSchemaIndications",
        data: request,
        success: function (data) {
            toastr.success("Success");
            updateSchemaDataOnUI(data.Id, data.RowVersion);
            updateIndicationIds(data.Indications);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function updateIndicationIds(indications) {
    indications.forEach(el => {
        $('.indication-body').append('<p id="indication-item_' + el['Id'] + '" class="schema-text" data-id="' + el['Id'] + '">' + el['Name'] + '</p>')
    });
}

function updateSchemaDataOnUI(id, rowVersion, name = '') {
    updateDisplaySchemaId(id);
    updateRowVersion(rowVersion);
    updateDisplaySchemaNameInBreadcrumb(name);
}

function updateDisplaySchemaId(id) {
    $('.chemotherapy-schema').attr('data-id', id);
    $('#previewSchemaBtn').attr('data-id', id);
    $("#previewSchemaBtn").removeClass("d-none");
}

function updateRowVersion(rowVersion) {
    $('#rowVersion').val(rowVersion);
}

function getRowVersion() {
    return $('#rowVersion').val();
}

function updateDisplaySchemaNameInBreadcrumb(name) {
    if (name) {
        $('.breadcrumb-active').empty();
        $('.breadcrumb-active').append(`<a>${name}</a>`);
    }
}

$(document).on('click', '#previewSchemaBtn', function (e) {
    var id = $(this).attr('data-id');
    viewEntity(id);
});

function viewEntity(id) {
    window.location.href = `/SmartOncology/PreviewSchema/${id}`;
}

//medication modal
$(document).on('click', '.update-medication', function (e) {
    let medicationId = $(this).attr("data-id");
    $.ajax({
        type: "GET",
        url: `/SmartOncology/GetSchemaMedication?id=${medicationId}`,
        success: function (data) {
            $(".new-medication-wrapper").html(data);
            $('.new-medication-wrapper').show();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
    selectActiveMedication(this, medicationId);
});

function selectActiveMedication(activeMedication, medicationId) {
    unselectPreviousActiveMedication();
    if (medicationId > 0) {
        $(activeMedication).addClass("active");
    }
}

function unselectPreviousActiveMedication() {
    $('.update-medication.active').removeClass("active");
}