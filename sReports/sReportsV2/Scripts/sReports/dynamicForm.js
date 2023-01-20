var isInitLoaded = false;
var isNEValidationErrorShown = false;

$(document).ready(function () {
    $.validator.addMethod(
        "regex",
        function (value, element) {
            let regexp = $(element).data('regex');
            let elementValue = $(element).val();
            if (regexp) {
                var re = new RegExp(regexp);
                return this.optional(element) || re.test(elementValue) || isSpecialValueSelected($(element));
            }
            else {
                return true;
            }

        },
        "Please check your input."
    );

    $('#fid').validate();

    $('[data-type="regex"]').each(function () {
        var regexDescription = $(this).data('regexdescription');
        $(this).rules('add', {
            required: $(this).data('regex-required') === 'True',
            regex: true,
            messages: { // optional custom messages
                regex: regexDescription
            }
        });
    });

    $.validator.addMethod(
        "hasNE",
        function (value, element) {
            if (value.toUpperCase() === "N/E") {
                if (!isNEValidationErrorShown) {
                    isNEValidationErrorShown = true;
                    toastr.options.onHidden = function () { isNEValidationErrorShown = false; }
                    toastr.error('N/E is a reserved word in the SmartOncology system and can not be used as a field option and can not be entered in the text field types.');
                }

                return false;
            } else {
                return true;
            }
        },
        "N/E is reserved value."
    );

    $('input.form-element-field, textarea.form-element-field').each(function () {
        $(this).rules('add', {
            hasNE: true
        });
    });
    InitForm();
    configureImageMap();
});

function configureImageMap() {
    $('map').imageMapResize();
    setTimeout(function () { triggerResize(); }, 100);
}

function clickedSubmit(event) {
    event.preventDefault();
    passInputValidation();
    if ($('#fid').valid() && !inputsAreInvalid("form instance")) {
        let filesData = [];
        $.each($('#fid').find('input[type="file"]'), function (index, value) {
            if (value.files[0]) {
                filesData.push({ id: $(value).data('id'), content: value.files[0] });
            }
        });
        if (filesData.length > 0) {
            sendFileData(filesData, setImageUrl, submitForm);
        } else {
            submitForm();
        }
    } else {
        $('#fid').find('input').each(function (index, element) {
            validateInput(element, event);
        });
    }


    return false;
}

function submitForm() {
    $.ajax({
        url: $('#fid').attr('action'),
        type: "POST",
        contentType: 'application/x-www-form-urlencoded',
        data: $('#fid').serialize(),
        success: function (data, textStatus, xhr) {
            toastr.success("Success");
            handleSuccessFormSubmit();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });

    return false;
}

function InitForm() {
    showPageOnIndex(0);
    $(".chapter:eq( 0 )").collapse('show');
}

function showPageById(pageId, event) {
    event.preventDefault();
    event.stopPropagation();
    $('.pages-link').each(function (index, element) {
        $(element).removeClass('active');
    });
    $(event.currentTarget).closest('.pages-link').addClass('active');
    var pageToShow = $(event.currentTarget).closest(".chapters-container").find("#" + pageId);
    var indexToShow = $(event.currentTarget).closest(".chapters-container").find(".page").index(pageToShow);
    showPageOnIndex(indexToShow);
}

function showPageOnIndex(newIndex) {
    var currIndex = $(".page").index($(".page:visible"));
    if (!currIndex)
        currIndex = -1;
    var nrOfPages = $(".page").length;
    if (newIndex >= nrOfPages || newIndex < 0 || currIndex == newIndex)
        return;

    $(".page").hide(0);
    $(".page:eq( " + newIndex + " )").show(300);
    $(".page-selector a").removeClass("active");
    $(".page-selector a:eq( " + newIndex + " )").addClass("active");
    setTimeout(function () { triggerResize(); }, 100);
}

function collapseChapter(element) {
    let id = $(element).data('target');
    
    $(element).closest(".chapters-container").find(`.chapter.collapse:not(${id})`).collapse('hide');
    $(element).closest(".chapters-container").find(`.chapter.collapse:not(${id}) .page`).hide();
    $(element).closest(".chapters-container").find(`${id}`).collapse('show');
    $(element).closest(".chapters-container").find(id).find('.page:eq(0)').show(function () {
        configureImageMap();
    });
}

$('.collapse').on('shown.bs.collapse', function (e) {
    e.preventDefault();
    if (isInitLoaded) {
        scrollToElement(this, 1000, 50);
    } else {
        isInitLoaded = true;
    }
});

function triggerResize() {
    var el = document;
    var event = document.createEvent('HTMLEvents');
    event.initEvent('resize', true, false);
    el.dispatchEvent(event);
}

function showHelpModal(data) {
    $('#helpModalTitle').html(data.Title);
    $('#helpModalBody').html(data.Content);
    $('#helpModal').modal('toggle');
}

$(document).on('click', 'area', function (e) {
    e.preventDefault();
    e.stopPropagation();

    let id = $(this).attr('data-fieldset');
    let row = $(`#${id}`).parent().parent();
    $(row).find('.form-fieldset').hide();
    $(`#${id}`).show();
    scrollToElement($(`#${id}`).first(), 1500);
})

function scrollToElement(element, duration, additionalOffset) {
    let scrollToId = $(element).attr('id');
    if ($(`#${scrollToId}`).length > 0) {
        $([document.documentElement, document.body]).animate({
            scrollTop: $(`#${scrollToId}`).offset().top - getScrollOffset() - (additionalOffset ? additionalOffset : 0)
        }, duration);
    }
}

function getScrollOffset() {
    let offset = 70;
    if ($(window).width() <= 768) {
        offset = 50;
    }
    return offset;
}

function calculateFormula(formulaElement, formula, identifiersAndVariables, idPrefix) {
    let shouldResetCalculation = false;
    Object.keys(identifiersAndVariables).forEach(x => {
        let value = '';
        let element = getElementByIdentifier(idPrefix, x);

        if ($(element).attr("data-set-default-value")) {
            shouldResetCalculation = true;
            $(element).removeAttr("data-set-default-value");
            return;
        }

        if ($(element).attr('type') === 'radio') {
            value = getFormulaVariableValueFromRadio(element);
        }
        else if ($(element).attr('type') === 'checkbox') {
            value = getFormulaVariableValueFromCheckbox(element);
        }
        else if ($(element).attr('type') === 'number') {
            value = $(element).val();
        }
        else if ($(element).is('select')) {
            value = getFormulaVariableValueFromSelect(element);
        }
        formula = tryReplaceVariableWithValue(formula, value, identifiersAndVariables[x])
    });

    executeFormulaCalculation(formula, formulaElement, shouldResetCalculation);
}

function executeFormulaCalculation(formula, formulaElement, shouldResetCalculation) {
    var $calculativeField = $(`input[name*='-${formulaElement}-1'][type='text']`);
    try {
        var executedExpression = shouldResetCalculation ? "" : eval(formula).toFixed(4);
        $calculativeField.val(executedExpression);
        removeFieldErrorIfValid($calculativeField);
    } catch (e) {
        console.log(e);
    }
}

function tryReplaceVariableWithValue(formula, value, variableName) {
    if (value || value == 0) {
        var replace = new RegExp(`\\[${variableName}\\]`, 'g');
        formula = formula.replace(replace, value);
        return formula;
    }
    else {
        throw `[${x}] not defined in formula ${formula} `;
    }
}

function getFormulaVariableValueFromRadio(element) {
    let result;
    let checked;
    if ($(element).is(":checked")) {
        checked = $(element);
    } else {
        checked = $(element)
            .parent()
            .parent()
            .find("input:checked");
    }
    result = checked.data('numericvalue');
    return result;
}

function getFormulaVariableValueFromCheckbox(element) {
    let result = 0;
    let checkedCheckboxes = $(element)
        .parent()
        .parent()
        .find("input:checked");
    $.each(checkedCheckboxes, function (index, checkbox) {
        if ($(checkbox).data('numericvalue')) {
            result += $(checkbox).data('numericvalue');
        }
    });

    if ($(element).is(":checked")) {
        result += $(element).data('numericvalue');
    }

    return result;
}

function getFormulaVariableValueFromSelect(element) {
    let result = '';
    let selected = $(element).find(':selected');
    if (selected) {
        result = $(selected).data('numericvalue');
    }

    return result;
}

function getElementByIdentifier(idPrefix, x) {
    let element = tryGetFieldFromSameFieldSet(idPrefix, x);
    if (element.length == 0) {
        element = getFirstFieldByName(x);
    }

    return element[0];
}

function tryGetFieldFromSameFieldSet(idPrefix, x) {
    return $(`[name="${idPrefix}-${x}-1"]`).first();
}

function getFirstFieldByName(x) {
    return $(`[name*="-${x}-1"]`).first();
}

function checkNumOfFields(formElement) {

    let repetitiveCount = $(formElement).children('.repetitive-values').children(".field-group").length;

    if (repetitiveCount && repetitiveCount === 1) {
        $(formElement).children('.repetitive-values:last').children(".field-group").each(function (index, element) {
            $(element).find('.remove-repetitive').hide();
        });
    } else {
        $(formElement).children('.repetitive-values:last').children(".field-group").each(function (index, element) {
            $(element).find('.remove-repetitive').show();
        });
    }
}

function getInputType(clone) {

    let inputType = "input";
    if (clone.children(".repetitive-field:last").find("input").length === 0) {
        inputType = "textarea";
    }
    return inputType;
}

function checkNumOfFieldSets(closestFsContainer) {

    let numOfRepetitiveFs = $(closestFsContainer).children(".field-set").length;


    if (numOfRepetitiveFs === 1) {
        //hide remove buttons
        $(closestFsContainer).children(".field-set").each(function (index, element) {
            $(element).children("div:first").children("div:last").children("button:first").hide();
        });
    }
    else {
        //show remove inputs  
        $(closestFsContainer).children(".field-set").each(function (index, element) {
            $(element).children("div:first").children("div:last").children("button:first").show();
        });
    }

}

function getNewNameValue(clone, inputType) {
    let nameValue = clone.find(inputType).attr("name"); // for ex fs1-1-1-1
    let nameValues = nameValue.split("-");
    let currentPosition = nameValues[3];
    let intValOfPosition = parseInt(currentPosition);
    intValOfPosition++;
    let newNameValue = `${nameValues[0]}-${nameValues[1]}-${nameValues[2]}-${intValOfPosition}`;
    
    return newNameValue;
}

function getNewNameValueDateTime(clone, inputType) {

    let nameValue = clone.find(".date-time-local").attr("name"); // for ex fs1-1-1-1
    let nameValues = nameValue.split("-");
    let currentPosition = nameValues[3];
    let intValOfPosition = parseInt(currentPosition);
    intValOfPosition++;
    let newNameValue = `${nameValues[0]}-${nameValues[1]}-${nameValues[2]}-${intValOfPosition}`;

    return newNameValue;
}

$(document).on('click', '.button-plus-repetitive', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();

    let closestField = $(event.currentTarget).closest(".form-element");
    let closestFieldContainer = $(closestField).children(".repetitive-values:last").children(".field-group:last");
    resetDateFieldsBeforeClone(closestFieldContainer);

    let clone = $(closestFieldContainer).clone();

    if ($(clone).hasClass('field-group-date-time')) {
        cloneDateTimeInput(clone);
    } else {
        cloneInput(clone);
    }

    clone.appendTo($(closestField).children(".repetitive-values:last"));
    checkNumOfFields($(event.currentTarget).closest('.form-element'));
});

function cloneInput(clone) {

    let inputType = getInputType(clone);
    let newNameValue = getNewNameValue($(clone).children(".repetitive-field:last"), inputType);
    cloneResetAndNeSection(clone, newNameValue);

    clone.children(".repetitive-field:last").children(inputType).attr("name", newNameValue);
    clone.children(".repetitive-field:last").children(inputType).val(null);
}

function cloneDateTimeInput(clone) {
    let inputType = getInputType(clone);
    let newNameValue = getNewNameValueDateTime($(clone).children(".repetitive-field:last"), inputType);
    cloneResetAndNeSection(clone, newNameValue);

    clone.children(".repetitive-field:last").find(inputType).each(function (i, e) {
        if ($(e).attr("name")) {
            $(e).attr("name", newNameValue);
        }
        $(e).val(null);
    });
}

function cloneResetAndNeSection(clone, newName) {
    $(clone).find(".ne-link").attr("data-field-name", newName);
    $(clone).find(".ne-radio").attr("name", newName);
}

function setName(currentInput) {
    let inputName = $(currentInput).attr("name");
    let nameValues = inputName.split("-");
    let fieldSetCounter = parseInt(nameValues[1]);
    fieldSetCounter++;
    let newNameValue = `${nameValues[0]}-${fieldSetCounter}-${nameValues[2]}-${nameValues[3]}`;
    $(currentInput).attr("name", newNameValue);

    return newNameValue;
}

function setId(currentInput) {
    let inputId = $(currentInput).attr("id");
    if (inputId) {
        let idValues = inputId.split("-");
        let fieldSetCounter = parseInt(idValues[1]);
        fieldSetCounter++;
        let newId = `${idValues[0]}-${fieldSetCounter}-${idValues[2]}-${idValues[3]}`;
        $(currentInput).attr("id", newId);
    }
}

function setIdForFile(currentInput) {
    setIdByParam("id", currentInput);
    setIdByParam("data-id", currentInput);
}

function setIdByParam(attr, currentInput) {
    let inputId = $(currentInput).attr(attr);
    if (inputId) {
        let idValues = inputId.split("-");
        let fieldSetCounter = parseInt(idValues[2]);
        fieldSetCounter++;
        let newId = `${idValues[0]}-${idValues[1]}-${fieldSetCounter}-${idValues[3]}-${idValues[4]}`;
        $(currentInput).attr(attr, newId);
    }
}

function setRadioId(currentInput) {
    let inputId = $(currentInput).attr("id");
    if (inputId) {
        let idValues = inputId.split("-");
        let fieldSetCounter = parseInt(idValues[1]);
        fieldSetCounter++;
        let newId = `${idValues[0]}-${fieldSetCounter}`;
        for (i = 2; i < idValues.length; i++) {
            newId += "-" + idValues[i];
        }

        $(currentInput).attr("id", newId);
    }

}

function setDependableVisibility(field) {
    if ($(field).attr("data-dependables") == "False") {
        $(field).hide();
    }
}

function resetValues(currentInput) {
    if ($(currentInput).attr('type') === "radio" || $(currentInput).attr('type') === "checkbox") {
        $(currentInput).prop('checked', false);
    } else {
        $(currentInput).val(null);
    }
}

function setCheckBoxFormField(clone) {
    $(clone).children(".form-checkbox").each(function (key, value) {
        setId(value);
        setDependableVisibility(value);
        $(value).children('.checkbox-container').children("label").each(function (key, val) {
            let currentInput = $(val).children("input:first");
            resetValues(currentInput);
            let newNameValue = setName(currentInput);
            cloneResetAndNeSection(value, newNameValue);
            setRadioId(currentInput);
        });
    });
}

function setRadioFormField(clone) {
    $(clone).children('.form-radio').each(function (index, value) {
        $(value).children('.radio-container').children("label").each(function (key, val) {
            let currentInput = $(val).children("input:first");
            resetValues(currentInput);
            let newNameValue = setName(currentInput);
            cloneResetAndNeSection(value, newNameValue);
            setRadioId(currentInput);
        });

        let idToSet = $(value).children("label:first").children("input:first").attr('name');
        $(value).attr("id", idToSet);

    });
}

function setSelectFormField(clone) {
    $(clone).children(".form-select").each(function (key, value) {
        let currentInput = $(value).find("select:first");
        resetValues(currentInput);
        let newNameValue = setName(currentInput);
        cloneResetAndNeSection(value, newNameValue);
        setRadioId(currentInput);
    });
}

$(document).on('click', '.button-fieldset-repetitive', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();

    let closestFieldSet = $(event.currentTarget).closest(".form-fieldset").children(".field-set-container:last").children(".field-set:last");
    resetDateFieldsBeforeClone(closestFieldSet);
    let clone = $(closestFieldSet).clone();

    setClonedFields(clone);

    clone.appendTo($(event.currentTarget).closest(".form-fieldset").children(".field-set-container:last"));
    let closestFsContainer = $(event.currentTarget).closest(".form-fieldset").children(".field-set-container:first");
    checkNumOfFieldSets(closestFsContainer);
    showAddNewForLast(closestFsContainer);
});

function resetDateFieldsBeforeClone(fieldset) {
    $(fieldset).find('.datetime-picker-container').each(function (index, value) {
        $(value).find('input:first').datepicker("destroy").removeAttr('id');
    });
}

function setClonedFields(clone) {
    setNonSelectableFormFields(clone);
    setCheckBoxFormField(clone);
    setSelectFormField(clone);
    setRadioFormField(clone);
    setCalculativeField(clone);
    setFileField(clone);
}

function setFileField(clone) {
    clone.find(".file-name-div").hide();
}

function setCalculativeField(clone) {
    $(clone).find('.calculative').each(function (index, value) {
        $(value).find("input").each(function (ind, input) {
            $(input).attr("name", $(value).attr("id"));
            $(input).val(null);
        });
    });
}

function setNonSelectableFormFields(clone) {
    let isFirst = true;
    $(clone).find("fieldset").each(function (key, value) {
        setDependableVisibility(value);
        setId(value);
        isFirst = true;
        $(value).children(".repetitive-values:first").each(function (key, val) {
            $(val).children(".field-group").each(function (k, v) {
                $(v).find(".remove-repetitive").hide();
                $(v).children(".repetitive-field").each(function (key, currInput) {
                    if (isFirst) {
                        let cr = GetInput(currInput);

                        $(cr).each(function (k, currentInput) {
                            resetValues(currentInput);
                            let newNameValue = setName(currentInput);
                            cloneResetAndNeSection(v, newNameValue);
                            setIdForFile(currentInput);
                            
                            isFirst = false;
                            let removeButton = $(currInput).children("div:last").children("button:first");
                            if ($(removeButton).hasClass("remove-repetitive")) {
                                $(removeButton).css("display", "none");
                            }
                        });
                       
                    }
                    else {
                        $(v).remove();
                    }

                });
            });


        });
    });
}

function GetInput(currInput) {
    let currentInput = $(currInput).find("input");
    if (currentInput.length == 0) {
        currentInput = $(currInput).find("textarea:first");
    }
    return currentInput;
}

$(document).on('click', '.remove-repetitive', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    let closestFieldSet = $(event.currentTarget).closest(".form-element");
    $(event.target).closest(".field-group").remove();

    checkNumOfFields(closestFieldSet);
});

$(document).on('click', '.remove-field-set', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();

    let closestFsContainer = $(event.currentTarget).closest(".field-set-container");
    $(event.currentTarget).closest('.field-set').remove();
    checkNumOfFieldSets(closestFsContainer);
    showAddNewForLast(closestFsContainer);

});

function showAddNewForLast(closestFsContainer) {
    let numOfRepetitiveFieldSet = $(closestFsContainer).children(".field-set").length;
    $(closestFsContainer).children(".field-set").each(function (index, element) {
        if (numOfRepetitiveFieldSet - 1 == index) {
            $(element).children('div:first').children('.fieldset-repetitive').children('div:last').show();
        } else {
            $(element).children('div:first').children('.fieldset-repetitive').children('div:last').hide();
        }
    });

}

$(document).on('click', '.chapter-li', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    $('.form-accordion').each(function (index, element) {
        $(element).hide();
    });

    let accordionId = $(this).attr('id').replace('li', 'acc');
    collapseChapter($(`#${accordionId}`).children('.form-accordion-header:first'));
    $(`#${accordionId}`).show();

    $('.chapter-li').each(function (i, e) {
        if ($(e).hasClass('active')) {
            $(e).removeClass('active');
        }
    });
    $('.pages-link').each(function (i, e) {
        if ($(e).hasClass('active')) {
            $(e).removeClass('active');
        }
    });

    $(this).addClass('active');
    $(`#${accordionId}`).find('.pages-link:first').addClass('active');

    if (accordionId == "administrativeChapter-acc") {
        showAdministrativeArrowIfOverflow('administrative-container-form-instance');
    }
});

$(document).on('click', '.form-des', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    $(this).closest('.main-content').find('.form-description:first').show();
});

function showDescription(element, elementDesc, description) {
    if ($(element).find(".fa-info-circle").length > 0) {
        $(element).closest(elementDesc).find(description).toggle();
    }
}

$(document).on('click', '.chapter-des', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    showDescription(this, '.form-accordion', '.chapter-description:first');
});

$(document).on('click', '.page-des', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    showDescription(this, '.page', '.page-description:first');
});

$(document).on('click', '.field-set-des', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    showDescription(this, '.field-set', '.fieldset-description:first');
});



$(document).on('click', '.x-des', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    $(this).closest('.desc').hide();
    //$(this).closest('.field-set').find('.field-set-des').find('.fa-angle-up').addClass('fa-angle-down');
    //$(this).closest('.field-set').find('.field-set-des').find('.fa-angle-up').removeClass('fa-angle-up');.
    $(this).closest(".des-container").find("div:first").find('.fa-angle-up').addClass('fa-angle-down');
    $(this).closest(".des-container").find("div:first").find('.fa-angle-up').removeClass('fa-angle-up');
});

$(document).on('click', '.form-element-field', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    $('.form-element-field').each(function (index, element) {
        $(element).closest('.form-element-border').removeClass('active');
    });

    $(this).closest('.form-element-border').addClass('active');

});

$(document).on('blur', '.form-element-field', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    $(this).closest('.form-element-border').removeClass('active');
});

function showHideDescription(event) {
    let des = $(event.currentTarget).closest(".form-element").find(".form-element-description");
    if ($(des).is(':visible')) {
        $(des).hide();
    } else {
        $(des).show();
    }
}

$(document).on('click', '.arrow-scroll-right-page', function (e) {
    e.preventDefault();
    scrollTabs("+=500px");
});

$(document).on('click', '.arrow-scroll-left-page', function (e) {
    e.preventDefault();
    scrollTabs("-=500px");
});

function scrollTabs(scrollDirection) {
    let activeChapterId = $('.chapter-li.active').data('id');
    $(`#arrowRight-chapter-${activeChapterId}`).animate({
        scrollLeft: scrollDirection
    }, "slow");
}

$(document).on('click', '.arrow-scroll-right-form', function (e) {
    e.preventDefault();
    e.stopPropagation();

    $('#idWorkflow').animate({
        scrollLeft: "+=500px"
    }, "slow");
});

$(document).on('click', '.arrow-scroll-left-form', function (e) {
    e.preventDefault();
    $('#idWorkflow').animate({
        scrollLeft: "-=500px"
    }, "slow");
});

$(document).on("change", ".file", function () {
    $(this).closest(".repetitive-field").find(".file-name-text").text($(this).val().split("\\")[2]);
    $(this).closest(".repetitive-field").find(".file-name-div").show();
    var $fileNameField = $(this).siblings(".file-hid");
    if (isSpecialValueSelected($(this))) {
        unsetSpecialValue($(this));
    }
    removeFieldErrorIfValid($fileNameField, $fileNameField.attr("id"));
});

$(document).on("change", ".file-hid", function () {
    var $fileNameField = $(this);
    removeFieldErrorIfValid($fileNameField, $fileNameField.attr("id"));
});

function removeFile(event) {
    handleRemoveFile($(event.currentTarget).closest(".repetitive-field"));
}

function handleRemoveFile($field) {
    $field.find(".file-name-div").hide();
    $field.find(".file-hid").val('');
    $field.find("input[type='file']").val('');
}

$(document).on("click", ".file-choose", function () {
    $(this).closest(".repetitive-field").find(".file").click();
});

$(document).on("change", ".field-date-input", function () {
    removeFieldErrorIfValid($(this), $(this).attr("id"));
    removeFieldErrorIfValidForTimeInput($(this));
});

function removeFieldErrorIfValidForTimeInput($correspondantDateInput) {
    var $timeField = $correspondantDateInput.closest(".datetime-picker-container").find(".field-time-input");
    removeFieldErrorIfValid($timeField, $timeField.attr("id"));
}

$('input').on('blur', function (e) {
    validateInput(this, e);

    return false;
});

function validateInput(input,e) {
    e.preventDefault();
    e.stopPropagation();

    if (skipInputValidation($(input))) {
        return;
    }
    $(input).closest(".repetitive-field").removeClass('repetitive-error');
    if ($(input).hasClass("error")) {
        $(input).closest(".repetitive-field").addClass('repetitive-error');
    }
}

function skipInputValidation($input) {
    return $input.hasClass("ne-radio")
        || $input.hasClass("date-time-local")
        || $input.hasClass("field-time-input")
        || $input.attr("type") == "hidden"
        || $input.attr("type") == "file";
}

function passInputValidation() {
    $.each($('#fid').find('input[type="file"]'), function (index, value) {
        if (value.files[0]) {
            setImageUrl($(value).data('id'), "temp_value");
        }
    });
}

function removeFieldErrorIfValid($field, customFieldName = '') {
    if ($field.hasClass("error")) {
        $field.removeClass("error");
        var fieldName = customFieldName ? customFieldName : $field.attr("name");
        var $fieldErrorMessage = $(`#${fieldName}-error`);
        $fieldErrorMessage.remove();
    }
    $field.closest(".repetitive-field").removeClass('repetitive-error');
}

function goToFormInstanceEdit(id, versionId) {
    window.open(`/FormInstance/Edit?VersionId=${versionId}&FormInstanceId=${id}`, '_blank');
}