$(document).on('change', '#selectedLanguage', function () {
    $(`#myTabContent #${$(this).val()}`)
        .addClass('show active')
        .siblings()
        .removeClass('show active');
    updatePreferredLanguageCheckbox($(this).val());
});

$(document).on('change', '#preferredLanguage', function () {
    let checkedAction = this.checked;
    let selectedLangauge = $("#selectedLanguage").val();
    if (checkedAction) {
        $(this).attr('data-value', selectedLangauge);
    } else {
        let currentPreferredLanguage = $(this).attr('data-value');
        if (selectedLangauge === currentPreferredLanguage) {
            $(this).attr('data-value', '');
        }
    }
});

$(document).on('change', '#targetingLanguage', function () {
    let newTargetLanguage = $(this).val();
    let showSubmitModalAppear = checkTargetTranslationInputValues();
    if (showSubmitModalAppear) {
        $('#gtTranslationSubmitModal').modal('show');
        $('#gtTranslationSubmitModal').attr('new-target-lang', newTargetLanguage);
    } else {
        updateTargetLanguageView(newTargetLanguage);
    }
});

function goToSystem(link) {
    window.open(link);
}

function reloadTable() {
    let filter = getFilterParametersObject();
    let selectedSystems = getSelectedVocabularySystems();
    setCAndCVocabularyFilterElements(selectedSystems);
    showCodeViewAllIfFiltersExist(selectedSystems);
    checkUrlPageParams();
    filter["Page"] = getPageNum();
    filter["PageSize"] = 5;

    $.ajax({
        type: "post",
        data: filter,
        url: `/ThesaurusGlobal/ReloadCodes`,
        success: function (data, textStatus, jqXHR) {
            $("#codesTableContainer").html(data);
            $("#pageSizeSelector").hide();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        requestObject = defaultFilter;
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Id', $('#id').val());
        addPropertyToObject(requestObject, 'CodeSystems', getSelectedSystems());

    }

    return requestObject;
}

function openCodingSystemModal(event) {
    event.preventDefault();
    event.stopPropagation();
    $("#codesModal").modal("show");
}

function openVocabularyModal(event) {
    event.preventDefault();
    event.stopPropagation();
    $("#vocabularyModal").modal("show");
}

function viewAllVocabularyCodes(event) {
    event.preventDefault();
    event.stopPropagation();
    resetVocabularyFilters();
    reloadTable();
}

function resetVocabularyFilters() {
    document.querySelectorAll('.filter-element').forEach(function (a) {
        a.remove();
    });
    $('[name="vocabulary"]:checkbox:checked').prop("checked", false);
}

function filterVocabularies(event) {
    var queryParams = new URLSearchParams(window.location.search);
    queryParams.set("page", "1");
    history.replaceState(null, null, "?" + queryParams.toString());

    reloadTable();

    $("#vocabularyModal").modal("hide");

}

function getSelectedSystems() {
    let selectedValues = [];
    $('[name="vocabulary"]:checkbox:checked').each(function (index, element) {
        selectedValues.push($(element).val());
    })

    return selectedValues;
}

$('.synonym-input').keypress(function (e) {
    if (e.which == 13 && $(this).val()) {
        addSynonym(e);
    }
});

$('.abbreviation-input').keypress(function (e) {
    if (e.which == 13 && $(this).val()) {
        addAbbreviation(e);
    }
});

$(document).on("click", ".abbreviation-add", function (e) {
    addAbbreviation(e);
});

$(document).on("click", ".synonym-add", function (e) {
    addSynonym(e);
});

function addAbbreviation(e) {
    let value = $(e.currentTarget).closest(".input").find("input").val();
    if (value && !checkIfExist(e, value)) {
        let shouldNotifyChange = $(e.currentTarget).attr('track-form-change');

        let removeBtn = document.createElement('div');
        $(removeBtn).addClass("abbreviation-remove item-remove");
        if (shouldNotifyChange) $(removeBtn).attr('track-form-change', true);

        let abbreviation = document.createElement('div');
        $(abbreviation).addClass("rounded-item abbreviation-color abbreviation-item");
        $(abbreviation).text(value);
        $(abbreviation).append(removeBtn);
        $(abbreviation).attr("data-value", value);
        $(e.currentTarget).closest('.gt-items-container').find(".items-container").append(abbreviation);

        if (shouldNotifyChange) notifyContributeFormChange();
    }

    $(e.currentTarget).closest(".input").find("input").val('');
}

function addSynonym(e) {
    let value = $(e.currentTarget).closest(".input").find("input").val();

    if (value && !checkIfExist(e, value)) {
        let shouldNotifyChange = $(e.currentTarget).attr('track-form-change');

        let removeBtn = document.createElement('div');
        $(removeBtn).addClass("synonym-remove item-remove");
        if (shouldNotifyChange) $(removeBtn).attr('track-form-change', true);

        let synonym = document.createElement('div');
        $(synonym).addClass("rounded-item synonym-color synonym-item");
        $(synonym).text(value);
        $(synonym).append(removeBtn);
        $(synonym).attr("data-value", value);
        $(e.currentTarget).closest('.gt-items-container').find(".items-container").append(synonym);

        if (shouldNotifyChange) notifyContributeFormChange();
    }

    $(e.currentTarget).closest(".input").find("input").val('');
}

function checkIfExist(e, value) {
    let exist = false;
    $(e.currentTarget).closest('.gt-items-container').find(".items-container").find(".rounded-item").each(function (index, element) {

        if ($(element).attr("data-value") === value) {
            exist = true;
        }
    });

    return exist;
}

function changeState(event, state) {
    event.preventDefault();
    event.stopPropagation();
    $(".thesaurus-state-dropdown").removeClass('active-item');
    $(event.currentTarget).addClass('active-item');
    $("#state").attr('data-value', state);
    $("#state").text(state);

    $("#menuState").removeClass('show');
}

$(document).on("click", ".item-remove", function () {
    $(this).closest(".rounded-item").remove();

    let shouldNotifyChange = $(this).attr('track-form-change');
    if (shouldNotifyChange) notifyContributeFormChange();
})


function addCode(event) {
    event.preventDefault();
    event.stopPropagation();

    if ($("#codesForm").valid()) {
        $.ajax({
            type: "post",
            data: getNewCode(),
            url: `/ThesaurusGlobal/CreateCode?thesaurusEntryId=${$("#id").val()}`,
            success: function (data, textStatus, jqXHR) {
                goToEdit(data.Id);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });

        $("#codesModal").modal("hide");
    }
}

function getNewCode() {
    return {
        "Id" : $("#codeId").val(),
        "Code" : $("#code").val(),
        "Value" : $("#string").val(),
        "Link" : $("#link").val(),
        "CodeSystemId" : $("#system").val()
    }
}

function addTableRow(code, string, systemValue, systemLabel, link) {
    let tr = document.createElement('tr');
    $(tr).append(getCodeTd(code));
    $(tr).append(getValueTd(string));
    $(tr).append(getSystemTd(systemValue, systemLabel));
    $(tr).append(getLinkTd(link));

    $("#codesTable").find('tbody').append(tr);
}

function getCodeTd(code) {
    let td = document.createElement('td');
    $(td).attr('data-value', code);
    let div = document.createElement('div');
    $(div).addClass('td-code');
    $(div).text(code);
    $(td).append(div);
    return td;
}

function getSystemTd(value, label) {
    let td = document.createElement('td');
    $(td).attr('data-value', value);
    $(td).text(label);

    return td;
}

function getValueTd(value) {
    let td = document.createElement('td');
    $(td).attr('data-value', value);
    $(td).text(value);

    return td;
}

function getLinkTd(link) {
    let td = document.createElement('td');
    $(td).attr('data-value', link);
    $(td).addClass('td-link');
    $(td).text(link);

    return td;
}

function goToEdit(id) {
    window.location.href = `/ThesaurusGlobal/Create?Id=${id}`
}

function submitThesaurus(event) {
    if (!isPreferredLanguageChosen()) {
        toastr.error('Please choose preferred language for thesaurus.');
        return;
    }

    let thesaurus = {};
    thesaurus["State"] = $("#state").attr('data-value');
    thesaurus["Id"] = $("#id").val();
    thesaurus["Translations"] = GetTranslations();
    thesaurus = GetConnectionOntology(thesaurus);
    thesaurus["PreferredLanguage"] = $("#preferredLanguage").attr('data-value');

    $.ajax({
        type: "post",
        data: thesaurus,
        url: `/ThesaurusGlobal/Create`,
        success: function (data, textStatus, jqXHR) {
            goToEdit(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function submitConnectionWithOntology(event) {
    let thesaurus = {};
    thesaurus["Id"] = $("#id").val();
    thesaurus = GetConnectionOntology(thesaurus);

    $.ajax({
        type: "post",
        data: thesaurus,
        url: `/ThesaurusGlobal/SubmitConnectionWithOntology`,
        success: function (data, textStatus, jqXHR) {
            toastr.options = {
                timeOut: 2000
            };
            toastr.options.onHidden = function () {
                goToEdit(data);
            };
            toastr.success('Connection with ontology is updated');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function GetTranslations() {
    let translations = [];

    $("#myTabContent").find(".tab-pane").each(function (index, element) {
        let translation = {};
        translation["Id"] = $(element).find(".translation-id").val();
        translation["Definition"] = $(element).find(".thesaurus-definition").val();
        translation["PreferredTerm"] = $(element).find(".thesaurus-term").val();
        translation["Language"] = $(element).attr("id");
        translation["Synonyms"] = GetSynonyms(element);
        translation["Abbreviations"] = GetAbbreviations(element);
        translation["ThesaurusEntryId"] = $("#id").val();


        translations.push(translation);
    });

    return translations;
}

function GetConnectionOntology(thesaurus) {

    thesaurus["UriClassLink"] = $("#uriClassLink").val();
    thesaurus["UriClassGUI"] = $("#uriClassGUI").val();
    thesaurus["UriSourceLink"] = $("#uriSourceLink").val();
    thesaurus["UriSourceGUI"] = $("#uriSourceGUI").val();

    return thesaurus;
}

function GetSynonyms(element) {
    let synonyms = [];
        
    $(element).find(".synonym-item").each(function (index, ele) {
        synonyms.push($(ele).attr("data-value"));
    });

    return synonyms;
}


function GetAbbreviations(element) {
    let synonyms = [];

    $(element).find(".abbreviation-item").each(function (index, ele) {
        synonyms.push($(ele).attr("data-value"));
    });

    return synonyms;
}

function editCode(event, id) {
    event.preventDefault();
    event.stopPropagation();

    $("#codeId").val(id);
    let code = $(`#${id}`).find(".code-value").attr("data-value");
    let value = $(`#${id}`).find(".value-value").attr("data-value");
    let system = $(`#${id}`).find(".system-value").attr("data-value");
    let link = $(`#${id}`).find(".link-value").attr("data-value");


    $("#code").val(code);
    $("#string").val(value);
    $("#system").val(system);
    $("#link").val(link);  

    $("#codesModal").modal("show");

}

function deleteCode(event, id) {
    event.preventDefault();
    event.stopPropagation();


    $.ajax({
        type: "delete",
        url: `/ThesaurusGlobal/DeleteCode?thesaurusId=${$("#id").val()}&codeId=${id}`,
        success: function (data, textStatus, jqXHR) {
            goToEdit(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function isPreferredLanguageChosen() {
    return $("#preferredLanguage").attr('data-value');
}

function updatePreferredLanguageCheckbox(selectedLangauge){
    let preferredLanguage = $("#preferredLanguage").attr('data-value');
    if (preferredLanguage === selectedLangauge) {
        $('#preferredLanguage').prop('checked', true);
    } else {
        $('#preferredLanguage').prop('checked', false);
    }
}

function contributeToTranslation(event, thesaurusId, returnUrl) {
    event.preventDefault();
    event.stopPropagation();

    if (thesaurusId) {
        window.location.href = `/ThesaurusGlobal/ContributeToTranslation?Id=${thesaurusId}&ReturnUrl=${returnUrl}`
    }
}

function submitThesaurusTranslationConfirm(event) {
    event.stopPropagation();
    event.preventDefault();

    submitThesaurusTranslation(event);

    let newTargetLanguage = $("#gtTranslationSubmitModal").attr('new-target-lang');
    updateTargetLanguageView(newTargetLanguage);

    $('#gtTranslationSubmitModal').modal('hide');
    $('#gtTranslationSubmitModal').removeAttr('new-target-lang');
}

function submitThesaurusTranslationClose(event) {
    event.stopPropagation();
    event.preventDefault();

    let newTargetLanguage = $("#gtTranslationSubmitModal").attr('new-target-lang');
    updateTargetLanguageView(newTargetLanguage);

    $('#gtTranslationSubmitModal').modal('hide');
    $('#gtTranslationSubmitModal').removeAttr('new-target-lang');
}

function submitThesaurusTranslation(event) {
    event.stopPropagation();
    event.preventDefault();

    $.ajax({
        type: "post",
        data: getTargetTranslation(),
        url: `/ThesaurusGlobal/ContributeToTranslation`,
        success: function (data, textStatus, jqXHR) {
            toastr.success("Thesaurus translation is successfully saved.");
            $('.show.active').removeAttr('form-changed');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getTargetTranslation() {
    let targetingTranslation = $("#myTabContent").find(".tab-pane.show.active")[0];
    if (!targetingTranslation) return;

    let translation = {};
    translation["Id"] = $(targetingTranslation).find(".translation-id").val();
    translation["Definition"] = $(targetingTranslation).find(".thesaurus-definition").val();
    translation["PreferredTerm"] = $(targetingTranslation).find(".thesaurus-term").val();
    translation["Language"] = $(targetingTranslation).attr("id");
    translation["Synonyms"] = GetSynonyms(targetingTranslation);
    translation["Abbreviations"] = GetAbbreviations(targetingTranslation);
    translation["ThesaurusEntryId"] = $("#thesaurusId").val();

    return translation;
}

function checkTargetTranslationInputValues() {
    let formChanged = $('.show.active').attr('form-changed');
    return formChanged;
}

$(document).on('focus', '.translation-input-field', function () {
    $(this).data('val', $(this).val());
});

$(document).on('blur', '.translation-input-field', function () {
    var currentValue = $(this).val();
    var previousValue = $(this).data('val');

    if (currentValue != previousValue) {
        notifyContributeFormChange();
    }
    $(this).data('val', currentValue);
});

function notifyContributeFormChange() {
    $('.show.active').attr('form-changed', true);
}

function updateTargetLanguageView(newTargetLanguage) {
    $(`#myTabContent #${newTargetLanguage}`)
        .addClass('show active')
        .siblings()
        .removeClass('show active')
        .removeAttr('form-changed');
}

function setCAndCVocabularyFilterElements(selectedSystems) {
    document.querySelectorAll('.filter-element').forEach(function (a) {
        a.remove();
    });
    for (let selectedSystem of selectedSystems) {
        let element = document.createElement('div');
        $(element).addClass('filter-element');
        $(element).html(selectedSystem['label']);

        $(element).append(getSystemTagCloseSign(selectedSystem, element));
        $('#filterElements').append(element);
    }
}

function getSelectedVocabularySystems() {
    let selectedValues = [];
    $('[name="vocabulary"]:checkbox:checked').each(function () {
        let label = $(this).closest('.vocabulary-checkbox-label').find('span').text();
        let id = $(this).attr("id");
        selectedValues.push({ 'id': id, 'label': label });
    })


    return selectedValues;
}

function getSystemTagCloseSign(systemValue) {
    let x = document.createElement('img');
    $(x).attr('src', "/Content/img/icons/Administration_remove.svg");
    $(x).addClass('ml-2');
    $(x).addClass('remove-system-filter');
    $(x).attr('system-id', systemValue['id']);
    $(x).css('font-size', '12px');
    $(x).css('width', '15px');
    $(x).css('padding', '5px');

    return x;
}

$(document).on('click', '.remove-system-filter', function (e) {
    let checkboxId = $(this).attr("system-id");
    let filterElementToUncheck = $(`#${checkboxId}`);
    filterElementToUncheck.prop("checked", false);
    reloadTable();
});

function showCodeViewAllIfFiltersExist(selectedSystems) {
    if (selectedSystems.length > 0) {
        $("#codesViewAll").show();
    } else {
        $("#codesViewAll").hide();
    }
}