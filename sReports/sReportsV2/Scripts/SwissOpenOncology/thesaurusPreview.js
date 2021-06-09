

$(document).on('change', '#selectedLanguage', function () {
    console.log($(this).val());

    $(`#myTabContent #${$(this).val()}`)
        .addClass('show active')
        .siblings()
        .removeClass('show active');
});

function goToSystem(link) {
    window.open(link);
}

function reloadTable() {
    let filter = getFilterParametersObject();
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
            toastr.error(`${thrownError} `);
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

    if (!checkIfExist(e, value)) {
        let removeBtn = document.createElement('div');
        $(removeBtn).addClass("abbreviation-remove item-remove");

        let abbreviation = document.createElement('div');
        $(abbreviation).addClass("rounded-item abbreviation-color");
        $(abbreviation).text(value);
        $(abbreviation).append(removeBtn);
        $(abbreviation).attr("data-value", value);
        $(e.currentTarget).closest('.gt-items').find(".items-container").append(abbreviation);
    }

    $(e.currentTarget).closest(".input").find("input").val('');
}

function addSynonym(e) {
    let value = $(e.currentTarget).closest(".input").find("input").val();

    if (!checkIfExist(e, value)) {
        let removeBtn = document.createElement('div');
        $(removeBtn).addClass("synonym-remove item-remove");

        let abbreviation = document.createElement('div');
        $(abbreviation).addClass("rounded-item synonym-color");
        $(abbreviation).text(value);
        $(abbreviation).append(removeBtn);
        $(abbreviation).attr("data-value", value);
        $(e.currentTarget).closest('.gt-items').find(".items-container").append(abbreviation);
    }

    $(e.currentTarget).closest(".input").find("input").val('');
}

function checkIfExist(e, value) {
    let exist = false;
    $(e.currentTarget).closest('.gt-items').find(".items-container").find(".rounded-item").each(function (index, element) {

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
})


function addCode(event) {
    
    let code = {};
    code["Id"] = $("#codeId").val();
    //code["System"] = {Id:$("#system").val()};
    code["Code"] = $("#code").val();
    code["Value"] = $("#string").val();
    code["Link"] = $("#link").val();
    code["CodeSystemId"] = $("#system").val();

    console.log($("#id").val());

    $.ajax({
        type: "post",
        data: code,
        url: `/ThesaurusGlobal/CreateCode?tid=${$("#id").val()}`,
        success: function (data, textStatus, jqXHR) {
            goToEdit(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });

    $("#codesModal").modal("hide");


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
    let thesaurus = {};
    thesaurus["State"] = $("#state").attr('data-value');
    thesaurus["Id"] = $("#id").val();
    thesaurus["Translations"] = GetTranslations();

    $.ajax({
        type: "post",
        data: thesaurus,
        url: `/ThesaurusGlobal/Create`,
        success: function (data, textStatus, jqXHR) {
            goToEdit(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
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

function GetSynonyms(element) {
    let synonyms = [];
        
    $(element).find(".synonym-color").each(function (index, ele) {
        synonyms.push($(ele).attr("data-value"));
    });

    return synonyms;
}


function GetAbbreviations(element) {
    let synonyms = [];

    $(element).find(".abbreviation-color").each(function (index, ele) {
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
            toastr.error(`${thrownError} `);
        }
    });
}