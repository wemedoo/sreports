function getAvailableFieldsForCalculation(identifierTypes) {
    let fields = [];
    $(`#nestable li.dd-item[data-itemtype=field][data-type=number]`).each(function (index, element) {
        fields.push(generateIdentifierVariableObject(element, identifierTypes));
    });

    $(`#nestable li.dd-item[data-itemtype=field][data-type=radio]`).each(function (index, element) {
        fields.push(generateIdentifierVariableObject(element, identifierTypes));
    });

    $(`#nestable li.dd-item[data-itemtype=field][data-type=checkbox]`).each(function (index, element) {
        fields.push(generateIdentifierVariableObject(element, identifierTypes));
    });

    $(`#nestable li.dd-item[data-itemtype=field][data-type=select]`).each(function (index, element) {
        fields.push(generateIdentifierVariableObject(element, identifierTypes));
    });

    return fields;
}

function generateIdentifierVariableObject(element, identifierTypes) {
    let dataProps = generateObjectFromDataProperties(element, configByType['field'].excludeProperties);
    return {
        Id: dataProps['id'],
        Label: dataProps['label'],
        VariableName: identifierTypes ? identifierTypes[dataProps['id']] : ''
    };
}

function loadCalculativeTree(data) {
    $.ajax({
        method: 'POST',
        data: JSON.stringify(data),
        url: '/Form/GetCalculativeTree',
        contentType: 'application/json',
        success: function (data) {
            $('#calculativeTree').html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
}

function setCustomCalculativeFields(element) {
    if (element) {
        let formula = $('#formula').val();
        let fieldsForCalculationWithVariables = getFieldsForCalculationWithVariables(formula);
        if (fieldsForCalculationWithVariables['valid']) {
            $(element).attr('data-identifiersandvariables', encodeURIComponent(JSON.stringify(fieldsForCalculationWithVariables['fieldsAndVariables'])));
            $(element).attr('data-formula', encodeURIComponent(formula));
            setCommonStringFields(element);
        }
    }
}

function getFieldsForCalculationWithVariables(formula) {
    let result = {};
    result['valid'] = true;
    result['allVariablesAssignedToField'] = true;
    result['duplicateVariableAssignment'] = true;
    let variables = getVariablesFromFormula(formula);
    if (variables) {
        result['fieldsAndVariables'] = {};
        variables.forEach(x => {
            let variableName = getVariableName(x);
            let fieldsWithAssignedVariables = getFieldForVariable(variableName);

            if (fieldsWithAssignedVariables.length == 0) {
                result['allVariablesAssignedToField'] = false;
                result['valid'] = false;
                return result;
            } else if (fieldsWithAssignedVariables.length > 1) {
                result['duplicateVariableAssignment'] = false;
                result['valid'] = false;
                return result;
            } else {
                let fieldId = $(fieldsWithAssignedVariables).attr('data-fieldid');
                result['fieldsAndVariables'][fieldId] = variableName;
            }
        });
    }
    return result;
}

function getFieldForVariable(variableName) {
    let result = [];
    $('.calculative-field-variable-input').each(function (index, element) {
        if ($(this).val().trim() == variableName) {
            result.push(element);
        }
    });

    return result;
}

function getVariableName(variable) {
    return variable.replace("[", "").replace("]", "").trim()
}

function getVariablesFromFormula(formula) {
    return formula.match(/\[.*?\]/g);
}

$(document).on('focusin', '.calculative-variable-input-container .designer-form-input', function (e) {
    let variableName = $(this).parent().siblings('.calculative-variable-name');
    $(variableName).addClass('active');
})

$(document).on('focusout', '.calculative-variable-input-container .designer-form-input', function (e) {
    let variableName = $(this).parent().siblings('.calculative-variable-name');
    $(variableName).removeClass('active');
})

$(document).ready(function () {
    jQuery.validator.addMethod("allVariablesAssignedToField", function (value, element) {
        let result = getFieldsForCalculationWithVariables($(element).val());
        return result['allVariablesAssignedToField'];//error
    }, "*Not all variables are assigned to fields");

    jQuery.validator.addMethod("duplicateVariableAssignment", function (value, element) {
        let result = getFieldsForCalculationWithVariables($(element).val());
        return result['duplicateVariableAssignment'];//error
    }, "Some of variables are assigned to more than one field");
})