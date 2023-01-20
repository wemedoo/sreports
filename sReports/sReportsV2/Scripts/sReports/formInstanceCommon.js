function isSpecialValueSelected($fieldInput) {
    var $neRadio = getSpecialValueElement($fieldInput);
    return $neRadio.is(":checked");
}

function unsetSpecialValue($fieldInput) {
    var $neRadio = getSpecialValueElement($fieldInput);
    return $neRadio.prop("checked", false);
}

function getSpecialValueElement($fieldInput) {
    var inputName = $fieldInput.attr("name");
    return $(`input[name="${inputName}"][spec-value]`);
}

$(document).on('click', '.ne-radio', function (e) {
    if ($(this).is(":checked")) {
        var $radioInput = $(this);
        var fieldName = $radioInput.attr("name");
        setInputFieldToDefault($radioInput, fieldName, false);
    }
});

function resetValue(event) {
    event.preventDefault();
    var $resetLink = $(event.currentTarget);
    var fieldName = $resetLink.data("field-name");
    $(`.ne-radio[name=${fieldName}]`).prop("checked", false);

    setInputFieldToDefault($resetLink, fieldName, true);
}

function setInputFieldToDefault($element, fieldName, isReset) {
    var fieldContainer = $element.closest(".form-element");
    $(fieldContainer).find(`[name=${fieldName}]`).not("[spec-value]").each(function () {
        setInputToDefault($(this), isReset);
    });
}

function setDependentInputFieldToDefault($element, visible) {
    var isRequired = $element.data("is-required") === 'True';
    $element.find(`[name]`).each(function () {
        $el = $(this);
        if (isSpecialValue($el)) {
            if (visible) {
                $el.prop("checked", false);
            } else {
                $el.prop("checked", isRequired);
            }
        } else {
            setInputToDefault($el, !isRequired || visible);
        }
    });
}

function isSpecialValue($el) {
    return $el.attr("spec-value") != undefined;
}

function setInputToDefault($el, isReset) {
    if (isInputFile($el)) {
        handleRemoveFile($el.closest(".repetitive-field"));
        return;
    }

    if (isInputDate($el)) {
        if (isReset) {
            $el.siblings(".field-date-btn").removeClass("pe-none");
        } else {
            $el.siblings(".field-date-btn").addClass("pe-none");
        }
        $el.datepicker("destroy").removeAttr('id');
        $el.val('');
        $el.trigger("change");
        return;
    }

    if (isInputTime($el)) {
        if (isReset) {
            $el.siblings(".field-time-btn").removeClass("pe-none");
        } else {
            $el.siblings(".field-time-btn").addClass("pe-none");
        }
        $el.attr("readonly", !isReset);
        $el.val('');
        return;
    }

    $el.attr("data-set-default-value", true);
    if ($el.is("select")) {
        $el.val('');
        $el.attr("disabled", !isReset);
        $el.trigger("change");
        return;
    }

    if (isInputCheckboxOrRadio($el)) {
        $el.prop("checked", false);
        $el.attr("disabled", !isReset);
        $el.trigger("change");
    } else {
        if (!isInputCalculative($el)) {
            $el.attr("readonly", !isReset);
        }
        $el.val('');
        $el.trigger("change");
    }
}

function isInputDate($el) {
    return $el.hasClass("field-date-input");
}

function isInputTime($el) {
    return $el.hasClass("field-time-input");
}

function isInputCheckboxOrRadio($el) {
    var inputType = $el.attr("type");
    return inputType == "checkbox" || inputType == "radio";
}

function isInputFile($el) {
    return $el.attr("type") == "file";
}

function isInputCalculative($el) {
    return $el.hasClass("field-calculative");
}