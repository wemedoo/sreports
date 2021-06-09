$(document).on('change', '#type', function (e) {
    let selectetFieldType = $(this).val();
    console.log(selectetFieldType);
    if (selectetFieldType) {
        $.ajax({
            method: "post",
            url: '/Form/GetFieldInfoCustomForm',
            data: JSON.stringify(getDataForAjax()),
            contentType: 'application/json',
            success: function (data) {
                showOrHideRepetitiveFields(stringFields.includes(selectetFieldType));
                $('#customFields').html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.error(jqXHR.responseText);
            }
        })
    }

});

$(document).on('click', '#submit-field-info', function (e) {
    if ($('#fieldGeneralInfoForm').valid()) {
        createNewThesaurusIfNotSelected();

        let label = $('#label').val();
        let type = $('#type').find(":selected").val();
        let elementId = $('#elementId').val();
        let element = getElement('field', label);

        if (element) {
            updateTreeIfFieldTypeIsChanged(element);
            $(element).attr('data-id', encodeURIComponent(elementId));
            $(element).attr('data-label', encodeURIComponent($('#label').val()));
            $(element).attr('data-thesaurusid', encodeURIComponent($('#thesaurusId').val()));
            $(element).attr('data-description', encodeURIComponent($('#description').val()));
            $(element).attr('data-isbold', encodeURIComponent($('#isBold').is(":checked")));
            $(element).attr('data-isrequired', encodeURIComponent($('#isRequired').is(":checked")));
            $(element).attr('data-isreadonly', encodeURIComponent($('#isReadonly').is(":checked")));
            $(element).attr('data-isvisible', encodeURIComponent($('#isVisible').is(":checked")));
            $(element).attr('data-fhirtype', encodeURIComponent($('#fhirType').val()))
            $(element).attr('data-ishiddenonpdf', encodeURIComponent($('#isHiddenOnPdf').is(":checked")))
            $(element).attr('data-unit', encodeURIComponent($('#unit').val()))
            $(element).attr('data-type', encodeURIComponent(type))
            setHelp(element);
            setCustomFieldsByType(element, type);

            updateTreeItemTitle(element, label);
            closDesignerFormModal(true);
            clearErrorFromElement($(element).attr('data-id'));
        }
    }
    else {
        toastr.error("Field informations are not valid");
    }
})

function updateTreeIfFieldTypeIsChanged(element) {
    console.log('update tree');
    let oldType = decode($(element).attr('data-type'));
    let newType = $('#type').find(":selected").val();
    if (oldType != newType) {
        if (selectableFields.includes(oldType) && !selectableFields.includes(newType)) {
            let ol = $(element).find('ol').first();
            $(ol).remove();
        } else if (!selectableFields.includes(oldType) && selectableFields.includes(newType)) {
            $(element).append(createDragAndDropOrderedlist('field', $(element).attr('data-id')));
        }
    }
}

function getDataForAjax() {
    let result = {
        id: $('#elementId').val(),
        label: $('#label').val(),
        thesaurusId: $('#thesaurusId').val(),
        description: $('#description').val(),
        isbold: $('#isBold').is(":checked"),
        isRequired: $('#isRequired').is(":checked"),
        isReadonly: $('#isReadonly').is(":checked"),
        isVisible: $('#isVisible').is(":checked"),
        fhirType: $('#fhirType').val(),
        isHiddenOnPdf: $('#isHiddenOnPdf').is(":checked"),
        unit: $('#unit').val(),
        type: $('#type').find(":selected").val(),
        help: {
            content: CKEDITOR.instances.helpContent.getData(),
            title: $('#helpTitle').val()
        }
    };
    return result;
}

function setHelp(element) {
    $(element).attr('data-help', encodeURIComponent(JSON.stringify(getHelp(element))));
}

function getHelp(element) {
    console.log('setting help')
    let helpContent = CKEDITOR.instances.helpContent.getData();
    let helpTitle = $('#helpTitle').val();
    let help = null;
    if (helpTitle || helpContent) {
        help = getDataProperty(element, 'help') || {};
        help['content'] = helpContent;
        help['title'] = helpTitle;
    }
    return help;
}

function setCustomFieldsByType(element, type) {
    console.log('type');
    switch (type) {
        case 'calculative':
            setCustomCalculativeFields(element);
            break;
        case 'checkbox':
            setCustomCheckboxFields(element);
            break;
        case 'custom-button':
            setCustomCustomFieldButtonFields(element);
            break;
        case 'date':
            setCommonStringFields(element);
            break;
        case 'datetime':
            setCommonStringFields(element);
            break;
        case 'email':
            setCommonStringFields(element);
            break;
        case 'file':
            setCommonStringFields(element);
            break;
        case 'long-text':
            setCommonStringFields(element);
            break;
        case 'number':
            setCustomNumberFields(element);
            break;
        case 'radio':
            setCustomRadioFields(element);
            break;
        case 'select':
            setCustomSelectFields(element);
            break;
        case 'regex':
            setCustomRegexFields(element);
            break;
        case 'text':
            setCustomTextFields(element);
    }
}

function setCommonStringFields(element) {
    if (element) {
        $(element).attr('data-isrepetitive', encodeURIComponent($('#isRepetitive').find(":selected").val()));
        $(element).attr('data-numberofrepetitions', encodeURIComponent($('#numberOfRepetitions').val() || 0));
    }
}

function showOrHideRepetitiveFields(flag) {
    if (flag) {
        $('.repetitive-field-group').show();
    } else {
        $('.repetitive-field-group').hide();
    }
}


function initializeValidator() {
    $('#fieldGeneralInfoForm').validate({
        rules: {
            'formula': {
                allVariablesAssignedToField: true,
                duplicateVariableAssignment: true
            }
        },
        ignore: '.manual-validation'
    });
}