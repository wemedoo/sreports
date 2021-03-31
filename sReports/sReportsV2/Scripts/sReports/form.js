var editorTree;
var editorCode;
var schema;
var isInitLoaded = false;

getSchema();

function clickedSubmit() {
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
}

function submitForm() {
    $('#fid').submit();
}


function InitForm() {
    showPageOnIndex(0);
    $(".chapter:eq( 0 )").collapse('show');
}

function redirectToCreate() {
    window.location.href = `/Form/CreateForm`;
}

function editEntity(event, thesaurusId, versionId) {
    event.preventDefault();
    window.location.href = `/Form/Edit?thesaurusId=${thesaurusId}&versionId=${versionId}`;
}

function showPageById(pageId) {
    var pageToShow = $("#" + pageId);
    var indexToShow = $(".page").index(pageToShow);
    showPageOnIndex(indexToShow);
}

function showPageOnIndex(newIndex) {
    var currIndex = $(".page:visible").index();
    if (!currIndex)
        currIndex = -1;
    var nrOfPages = $(".page").length;
    if (newIndex >= nrOfPages || newIndex < 0 || currIndex == newIndex)
        return;

    $(".page").hide(0);
    $(".page:eq( " + newIndex + " )").show(300);
    $(".page-selector a").removeClass("active");
    $(".page-selector a:eq( " + newIndex + " )").addClass("active");
}

function collapseChapter(element) {
    let id = $(element).data('target');
    $(`.chapter.collapse:not(${id})`).collapse('hide');
    $(`.chapter.collapse:not(${id}) .page`).hide();
    $(`${id}`).collapse('show');
    $(id).find('.page:eq(0)').show(function () {        
        $('map').imageMapResize();
        setTimeout(function () { triggerResize(); }, 100);
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

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Classes', $('#documentClass').val());
        addPropertyToObject(requestObject, 'ClassesOtherValue', $('#documentClassOtherInput').val());
        addPropertyToObject(requestObject, 'GeneralPurpose', $('#generalPurpose').val());
        addPropertyToObject(requestObject, 'ContextDependent', $('#contextDependent').val());
        addPropertyToObject(requestObject, 'ExplicitPurpose', $('#explicitPurpose').val());
        addPropertyToObject(requestObject, 'ScopeOfValidity', $('#scopeOfValidity').val());
        addPropertyToObject(requestObject, 'ClinicalDomain', $('#clinicalDomain').val());
        addPropertyToObject(requestObject, 'ClinicalContext', $('#clinicalContext').val());
        addPropertyToObject(requestObject, 'FollowUp', $('#documentFollowUpSelect').val());
        addPropertyToObject(requestObject, 'AdministrativeContext', $('#administrativeContext').val());
    }

    return requestObject;
}

function reloadTable(initLoad) {
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();

    $.ajax({
        type: 'GET',
        url: '/Form/ReloadTable',
        data: requestObject
    })
        .done(function (data) {
            $("#tableContainer").html(data);
        });
}

function getSchema() {
    schema = schemaJson;
}

function createNewFormDefinition() {
    var url = `/Form/Create`;
    //*small workaround for objectId will change in the future*/
    
    var formData = editorTree.get();
    if (formData.Id) {
        url = `${url}?formId=${formData.Id}`;
    }
    
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(formData),
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            toastr.options = {
                timeOut: 100
            }
            toastr.options.onHidden = function () { window.location.href = `/Form/GetAll`; }
            toastr.success("Success");
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }  
    });
}

function deleteFormDefinition(event, id, lastUpdate) {
    $.ajax({
        type: "DELETE",
        url: `/Form/Delete?formId=${id}&lastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
            toastr.success('Removed');
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function showJsonEditor(json = jsonTest) {
    var ajv = new Ajv({
        allErrors: true,
        verbose: true,
        schemaId: 'auto'
    });
    $('#jsoneditor').html('');
    $('.form-item').removeClass('active');
    $('#formContainer').hide();

    editorTree = new JSONEditor(document.getElementById('jsoneditorTree'), {
        ajv: ajv,
        mode: 'tree',
        onChangeText: function (jsonString) {
            try {
                editorTree.updateText(jsonString);
                $('.jsoneditor-format').click();
            }
            catch (exception) {
                console.log(exception);
            }
        },
        onError: function (error) {
            console.log(error);
        }
    });

    // create editor 2
    editorCode = new JSONEditor(document.getElementById('jsoneditorCode'), {
        ajv: ajv,
        mode: 'code',
        onChangeText: function (jsonString) {
            try {
                editorTree.updateText(jsonString);
            }
            catch (exception) {
                console.log(exception);
            }

        },
        onError: function (error) {
            console.log(error);
        }
    });

    editorTree.set(json);
    editorCode.set(json);
    $('#jsoneditorContainer').show();
}

function cancelJsonEditor() {
    window.location.href = '/Form/GetAll';
}

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

$('area').on('click', function (e) {
    e.preventDefault();
    let id = $(this).attr('id');
    $(`[data-map-area="${id}"]`).show().siblings(`:not([data-map-area="${id}"])`).hide();

    scrollToElement($(`[data-map-area="${id}"]`).first(), 1500);
});

function scrollToElement(element, duration, additionalOffset) {
    let scrollToId = $(element).attr('id');
    console.log(scrollToId);
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
    console.log(offset);
    return offset;
}

function goToPatient(id) {
    window.location.href = `/Patient/Edit?patientId=${id}`;
}

function goToEpisodeOfCare(id) {
    window.location.href = `/EpisodeOfCare/Edit?EpisodeOfCareId=${id}`;
} 

function calculateFormula(formulaElement, formula, fieldIds, idPrefix) {
   
    fieldIds.forEach(x => {
        let value = '';
        let element = $(`[name="${idPrefix}-${x}-1"]`);

        if ($(element).attr('type') === 'radio') {
            let checked = $(`input[name="${idPrefix}-${x}-1"]:checked`);
            value = checked.data('numericvalue');
        }
        else if ($(element).attr('type') === 'checkbox') {
            let checkedCheckboxes = $(`input[name="${idPrefix}-${x}-1"]:checked`);
            let summed = 0;
            $.each(checkedCheckboxes, function (index, checkbox) {
                if ($(checkbox).data('numericvalue')) {
                    summed += $(checkbox).data('numericvalue');
                }
            });
            value = summed;
        }
        else if ($(element).attr('type') === 'number') {
            if ($(element).val() !== '') {
                value = $(element).val();
            }
        }
        else if ($(element).is('select')) {
            let selected = $(element).find(':selected');
            if (selected) {
                value = $(selected).data('numericvalue');
            }
        }

        if (value || value == 0) {
            var replace = new RegExp(`\\[${x}\\]`, 'g');
            formula = formula.replace(replace, value);
        }
        else {
            throw `[${x}] not defined in formula ${formula} `;
        }
    });

    $(`input[name='${idPrefix}-${formulaElement}-1']`).val(eval(formula).toFixed(4));
}



var schemaJson = {
    'id': 'http://json-schema.org/draft-04/schema#',
    '$schema': 'http://json-schema.org/draft-04/schema#',
    'description': 'Core schema meta-schema',
    'definitions': {
        'schemaArray': {
            'type': 'array',
            'minItems': 1,
            'items': { '$ref': '#' }
        },
        'positiveInteger': {
            'type': 'integer',
            'minimum': 0
        },
        'positiveIntegerDefault0': {
            'allOf': [{ '$ref': '#/definitions/positiveInteger' }, { 'default': 0 }]
        },
        'simpleTypes': {
            'enum': ['array', 'boolean', 'integer', 'null', 'number', 'object', 'string']
        },
        'stringArray': {
            'type': 'array',
            'items': { 'type': 'string' },
            'minItems': 1,
            'uniqueItems': true
        }
    },
    'type': 'object',
    'properties': {
        'id': {
            'type': 'string'
        },
        '$schema': {
            'type': 'string'
        },
        'title': {
            'type': 'string'
        },
        'description': {
            'type': 'string'
        },
        'default': {},
        'multipleOf': {
            'type': 'number',
            'minimum': 0,
            'exclusiveMinimum': true
        },
        'maximum': {
            'type': 'number'
        },
        'exclusiveMaximum': {
            'type': 'boolean',
            'default': false
        },
        'minimum': {
            'type': 'number'
        },
        'exclusiveMinimum': {
            'type': 'boolean',
            'default': false
        },
        'maxLength': { '$ref': '#/definitions/positiveInteger' },
        'minLength': { '$ref': '#/definitions/positiveIntegerDefault0' },
        'pattern': {
            'type': 'string',
            'format': 'regex'
        },
        'additionalItems': {
            'anyOf': [
                { 'type': 'boolean' },
                { '$ref': '#' }
            ],
            'default': {}
        },
        'items': {
            'anyOf': [
                { '$ref': '#' },
                { '$ref': '#/definitions/schemaArray' }
            ],
            'default': {}
        },
        'maxItems': { '$ref': '#/definitions/positiveInteger' },
        'minItems': { '$ref': '#/definitions/positiveIntegerDefault0' },
        'uniqueItems': {
            'type': 'boolean',
            'default': false
        },
        'maxProperties': { '$ref': '#/definitions/positiveInteger' },
        'minProperties': { '$ref': '#/definitions/positiveIntegerDefault0' },
        'required': { '$ref': '#/definitions/stringArray' },
        'additionalProperties': {
            'anyOf': [
                { 'type': 'boolean' },
                { '$ref': '#' }
            ],
            'default': {}
        },
        'definitions': {
            'type': 'object',
            'additionalProperties': { '$ref': '#' },
            'default': {}
        },
        'properties': {
            'type': 'object',
            'additionalProperties': { '$ref': '#' },
            'default': {}
        },
        'patternProperties': {
            'type': 'object',
            'additionalProperties': { '$ref': '#' },
            'default': {}
        },
        'dependencies': {
            'type': 'object',
            'additionalProperties': {
                'anyOf': [
                    { '$ref': '#' },
                    { '$ref': '#/definitions/stringArray' }
                ]
            }
        },
        'enum': {
            'type': 'array',
            'minItems': 1,
            'uniqueItems': true
        },
        'type': {
            'anyOf': [
                { '$ref': '#/definitions/simpleTypes' },
                {
                    'type': 'array',
                    'items': { '$ref': '#/definitions/simpleTypes' },
                    'minItems': 1,
                    'uniqueItems': true
                }
            ]
        },
        'format': { 'type': 'string' },
        'allOf': { '$ref': '#/definitions/schemaArray' },
        'anyOf': { '$ref': '#/definitions/schemaArray' },
        'oneOf': { '$ref': '#/definitions/schemaArray' },
        'not': { '$ref': '#' }
    },
    'dependencies': {
        'exclusiveMaximum': ['maximum'],
        'exclusiveMinimum': ['minimum']
    },
    'default': {}
};

function checkNumOfFields(formElement) {

    let repetitiveCount = $(formElement).children('.repetitive-values').children(".repetitive-field").length;

    if (repetitiveCount && repetitiveCount === 1) {
        $(formElement).children('.repetitive-values:last').children(".repetitive-field").each(function (index, element) {
            $(element).children('div:last').children('button:last').hide();
        });
    } else {
        $(formElement).children('.repetitive-values:last').children(".repetitive-field").each(function (index, element) {
            $(element).children('div:last').children('button:last').show();
        });
    }
}

function getInputType(clone) {
    let inputType = "input";
    if (clone.children("input").length === 0) {
        inputType = "textarea";
    }
    return inputType;
}

function checkNumOfFieldSets(closestFsContainer) {

    let numOfRepetitiveFs = $(closestFsContainer).children(".field-set").length;


    if (numOfRepetitiveFs === 1) {
        //hide remove buttons
        $(closestFsContainer).children(".field-set").each(function (index, element) {
            $(element).children("div:first").children("div:first").children("button:last").hide();
        });
    }
    else {
       //show remove inputs  
        $(closestFsContainer).children(".field-set").each(function (index, element) {
            $(element).children("div:first").children("div:first").children("button:last").show();
        });
    }
   
}

function getNewNameValue(clone,inputType) {
    let nameValue = clone.children(inputType).attr("name"); // for ex fs1-1-1-1
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

    let closestFieldSet = $(event.currentTarget).closest(".form-element");
    let lastDiv = $(closestFieldSet).children(".repetitive-values:last").children(".repetitive-field:last");
    let clone = $(lastDiv).clone();

    let inputType = getInputType(clone);
    let newNameValue = getNewNameValue(clone, inputType);
    
    clone.children(inputType).attr("name", newNameValue);
    clone.children(inputType).val(' ');

    clone.appendTo($(closestFieldSet).children(".repetitive-values:last"));
    checkNumOfFields($(event.currentTarget).closest('.form-element'));
});

function setName(currentInput) {
    let inputName = $(currentInput).attr("name");
    let nameValues = inputName.split("-");
    let fieldSetCounter = parseInt(nameValues[1]);
    fieldSetCounter++;
    let newNameValue = `${nameValues[0]}-${fieldSetCounter}-${nameValues[2]}-${nameValues[3]}`;
    $(currentInput).attr("name", newNameValue);
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

function setRadioId(currentInput) {
    let inputId = $(currentInput).attr("id");
    if (inputId) {
        let idValues = inputId.split("-");
        let fieldSetCounter = parseInt(idValues[1]);
        fieldSetCounter++;
        let newId = `${idValues[0]}-${fieldSetCounter}`;
        for (i = 2; i < idValues.length; i++){
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
    console.log($(currentInput).attr('type'));
    if ($(currentInput).attr('type') === "radio" || $(currentInput).attr('type') === "checkbox") {
        $(currentInput).prop('checked', false);
    } else {
        $(currentInput).val(null);
    }
}

function setCheckBoxFormField(clone)
{
    $(clone).children("div").each(function (key, value) {
        setId(value);
        setDependableVisibility(value);
        $(value).children("label").each(function (key, val) {
            let currentInput = $(val).children("input:first");
            resetValues(currentInput);
            setName(currentInput);
            setRadioId(currentInput);
        });
    });
}

function setRadioFormField(clone)
{
    $(clone).children('.form-radio').each(function (index, value) {
        $(value).children("label").each(function (key, val) {
            let currentInput = $(val).children("input:first");
            resetValues(currentInput);
            setName(currentInput);
            setRadioId(currentInput);
        });

        let idToSet = $(value).children("label:first").children("input:first").attr('name');
        $(value).attr("id", idToSet);

    });
}

function setSelectFormField(clone)
{
    $(clone).children(".form-select").each(function (key, value) {
        let currentInput = $(value).children("select:first");
        resetValues(currentInput);
        setName(currentInput);
        setRadioId(currentInput);
    });
}

$(document).on('click', '.button-fieldset-repetitive',function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    
    let closestFieldSet = $(event.currentTarget).closest(".form-fieldset").children(".field-set-container:last").children(".field-set:last");
    let clone = $(closestFieldSet).clone();

    setClonedFields(clone);
   
    clone.appendTo($(event.currentTarget).closest(".form-fieldset").children(".field-set-container:last"));
    let closestFsContainer = $(event.currentTarget).closest(".form-fieldset").children(".field-set-container:first");
    checkNumOfFieldSets(closestFsContainer);
});

function setClonedFields(clone){
    setNonSelectableFormFields(clone);
    setCheckBoxFormField(clone);
    setSelectFormField(clone);
    setRadioFormField(clone);
    setCalculativeField(clone);
}

function setCalculativeField(clone) {
    $(clone).children('.calculative').each(function (index, value) {
        $(value).children("input").each(function (ind, input) {
            $(input).attr("name", $(value).attr("id"));
            $(input).val(null);
        });
    });
}

function setNonSelectableFormFields(clone)
{
    let isFirst = true;
    $(clone).children("fieldset").each(function (key, value) {
        setDependableVisibility(value);
        setId(value);
        isFirst = true;
        $(value).children("div:first").each(function (key, val) {
            $(val).children("div").each(function (key, currInput) {
                if (isFirst) {
                    let currentInput = GetInput(currInput);
                    resetValues(currentInput);
                    setName(currentInput);
                    isFirst = false;
                    let removeButton = $(currInput).children("div:last").children("button:first");
                    if ($(removeButton).hasClass("remove-repetitive")) {
                        $(removeButton).css("display", "none");
                    }
                }
                else {
                    $(currInput).remove();
                }

            });
        });
    });
}

function GetInput(currInput)
{
    let currentInput = $(currInput).children("input:first");
    if (currentInput.length == 0) {
        currentInput = $(currInput).children("textarea:first");
    }
    return currentInput;
}

$(document).on('click', '.remove-repetitive', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    let closestFieldSet = $(event.currentTarget).closest(".form-element");
    $(event.target).closest(".repetitive-field").remove();
    checkNumOfFields(closestFieldSet);
});

$(document).on('click', '.remove-field-set', function (event) {
    event.stopPropagation();
    event.stopImmediatePropagation();

    let closestFsContainer = $(event.currentTarget).closest(".field-set-container");
    $(event.currentTarget).closest('.field-set').remove();
    checkNumOfFieldSets(closestFsContainer);
    
});

$(document).ready(function () {
    $.validator.addMethod(
        "regex",
        function (value, element) {
            let regexp = $(element).data('regex');
            let elementValue = $(element).val();
            if (regexp) {
                var re = new RegExp(regexp);
                return this.optional(element) || re.test(elementValue);
            }
            else {
                return true;
            }

        },
        "Please check your input."
    );

    $('#fid').validate({
        errorPlacement: function (error, element) {
            if (element.attr("type") == 'checkbox' || element.attr("type") === 'radio') {
                error.appendTo(element.parent().parent());
            }
            else {
                error.appendTo(element.parent());
            }
        }
    });

    $('[data-type="regex"]').each(function () {
        var regexDescription = $(this).data('regexdescription');
        $(this).rules('add', {
            required: true,
            regex: true,
            messages: { // optional custom messages
                regex: regexDescription
            }
        });
    });
    InitForm();

    /*$('map').imageMapResize();
    setTimeout(function () { triggerResize(); }, 100);*/

   
});


