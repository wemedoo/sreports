$(document).on('change', '#actionParams', function (e) {
    let element = $('.dependable-inputs').find('[name="actionParams"]').first();
    validateDependableInputs(element, null, null);
});

$(document).on('change', '#condition', function (e) {
    let element = $('.dependable-inputs').find('[name="condition"]').first();
    validateDependableInputs(null, element, null);
});

$(document).on('change', '#actionType', function (e) {
    let element = $('.dependable-inputs').find('[name="actionType"]').first();
    validateDependableInputs(null, null, element);
});

$(document).on('click', '.remove-dependable-item', function (e) {
    $(this).parent().remove();
});

$(document).on('click', '.submitDependableForm', function (e) {
    let row = $(this).closest('div.dependable-item-row');
    let actionParamsInput = $(row).find('[name="actionParams"]').first();
    let conditionInput = $(row).find('[name="condition"]').first();
    let actionTypeInput = $(row).find('[name="actionType"]').first();
    let isValid = validateDependableInputs(actionParamsInput, conditionInput, actionTypeInput);
    if (!isValid) {
        return;
    }

    let tr = document.createElement('div');
    $(tr).addClass('dependable-item-row');
    $(tr).append(`<div class="dependable-item-cell" data-dependablefieldname="condition" data-dependablefieldvalue="${$(conditionInput).find("option:selected").val()}">${$(conditionInput).find("option:selected").text()}</div>`)
    $(tr).append(`<div class="dependable-item-cell" data-dependablefieldname="actionParams" data-dependablefieldvalue="${$(actionParamsInput).find("option:selected").val()}">${$(actionParamsInput).find("option:selected").text()}</div>`)
    $(tr).append(`<div class="dependable-item-cell last-cell" data-dependablefieldname="actionType" data-dependablefieldvalue="${$(actionTypeInput).find("option:selected").val()}">${$(actionTypeInput).find("option:selected").text()}</div>`)
    $(tr).append(`<div class="remove-dependable-item"><i class="fas fa-minus-circle"></i></div>`)

    $(row).before(tr);

    $(actionParamsInput).val('');
    $(actionTypeInput).val('');
    $(conditionInput).val('');
});

function validateDependableInputs(actionParamsInput, conditionInput, actionTypeInput) {
    let valid = true;
    valid = valid && validateActionParamsInput(actionParamsInput);
    valid = valid && validateConditionInput(conditionInput);
    valid = valid && validateActionTypeInput(actionTypeInput);

    return valid;
}

function validateActionParamsInput(actionParamsInput) {
    let actionParams = actionParamsInput ? $(actionParamsInput).val() : '';
    let valid = true;
    $('[data-dependablefieldname="actionParams"]').each(function (index, element) {
        if ($(this).attr('data-dependablefieldvalue') == actionParams) {

            valid = false;
            addErrorToDependableInput(actionParamsInput, 'Duplicate entry');
        }
    });
    if (actionParamsInput && !actionParams) {
        valid = false;
        addErrorToDependableInput(actionParamsInput, 'The field is required');
    } else if (valid) {
        $(actionParamsInput).siblings('.error').remove();
    }

    return valid;
}

function validateConditionInput(conditionInput) {
    let condition = conditionInput ? $(conditionInput).val() : '';
    let valid = true;
    if (conditionInput && !condition) {
        valid = false;
        addErrorToDependableInput(conditionInput, 'The field is required');
    } else {
        $(conditionInput).siblings('.error').remove();
    }

    return valid;
}

function validateActionTypeInput(actionTypeInput) {
    let actionType = actionTypeInput ? $(actionTypeInput).val() : '';

    let valid = true;

    if (actionTypeInput && !actionType) {
        valid = false;
        addErrorToDependableInput(actionTypeInput, 'The field is required');

    } else {
        $(actionTypeInput).siblings('.error').remove();
    }

    return valid;
}

function setLabels() {
    $('[data-dependablefieldname="actionParams"').each(function (index, element) {
        let id = $(this).attr('data-dependablefieldvalue');
        let valueElement = $(`li[data-itemtype="field"][data-id="${encodeURIComponent(id)}"]`).first();
        let label = decode($(valueElement).attr('data-label'));
        $(this).html(label);
    })
}

function loadFields() {
    $("#nestable").find('li[data-itemtype="field"]').each(function () {
        let label = decode($(this).attr('data-label'));
        let id = decode($(this).attr('data-id'));
        if (label && id) {
            $('#actionParams').append(`<option value='${id}'>${label}</option>`)
        }
    })
}

function loadOptions(id) {
    $("#nestable").find(`[data-id=${id}]`).find('li[data-itemtype="fieldvalue"]').each(function () {
        let label = decode($(this).attr('data-label'));
        let value = decode($(this).attr('data-value'));
        if (value && label) {
            $('#condition').append(`<option value=${value}>${label}</option>`)
        }
    })
}

function addErrorToDependableInput(input, message) {
    if (input.siblings('.error').length == 0) {
        $(input).after(`<label id="name-error" class="error">${message}</label>`)
    }
}

function setCommonSelectableFields(element) {
    if (element) {
        $(element).attr('data-dependables', encodeURIComponent(JSON.stringify(getDependableFields())));
    }
}

function getDependableFields() {
    let result = [];
    $('.dependable-item-row').not('.dependable-inputs').each(function (index, element) {
        let actionParams = $(this).find('[data-dependablefieldname="actionParams"').first();
        let condition = $(this).find('[data-dependablefieldname="condition"').first();
        let actionType = $(this).find('[data-dependablefieldname="actionType"').first();

        result.push({
            ActionParams: $(actionParams).attr('data-dependablefieldvalue'),
            Condition: $(condition).attr('data-dependablefieldvalue'),
            ActionType: $(actionType).attr('data-dependablefieldvalue')
        })
    })
    console.log(result);
    return result;
}