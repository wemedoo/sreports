$('#umlsTerm').keypress(function (event) {
    if (event.keyCode === 13 || event.which === 13) {
        searchByTerm(true);
        event.preventDefault();
    }
});

function searchByTerm(clearTable) {
    if (clearTable) {
        restartTable();
    }

    $.ajax({
        type: "GET",
        url: `/Umls/Search?term=${$('#umlsTerm').val()}`,
        data: getSearchRequestObject(),
        success: function (data) {
            if (data.trim()) {
                handleNonEmptrySearchResult(data);
            } else {
                handleEmptySearchResult();
            }

        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function handleNonEmptrySearchResult(data) {
    $('#loadMoreButtonContainer').show();
    $('#conceptNoResult').hide();
    $('#conceptsTableBody').append(data);
    document.getElementById("conceptHeaderId").classList.add("atoms-border");
    document.getElementById("conceptTableId").classList.remove("table-border");
}

function handleEmptySearchResult() {
    $('#loadMoreButtonContainer').hide();
    $('#conceptNoResult').show();
    document.getElementById("conceptHeaderId").classList.remove("atoms-border");
    document.getElementById("conceptTableId").classList.add("table-border");
    currentPage = currentPage + 1;
}

function getSearchRequestObject() {
    let requestObject = {};
    requestObject.searchTerm = $('#umlsTerm').val();
    requestObject.Page = currentPage;
    requestObject.PageSize = 25;

    return requestObject;
}

function restartTable() {
    $('#conceptsTableBody').empty();
    $('#loadMoreButtonContainer').hide();
    loadDefinitions("");
    loadAtoms("");

    currentPage = 1;
}

function showModal(event) {
    event.stopPropagation();
    $('#loadMoreButtonContainer').hide();
    $('#umlsModal').modal('show');
}

function loadDetails(event, id) {
    var selectedRow = $(event.srcElement).closest('tr');
    if ($(selectedRow).hasClass('selected')) {
        $(selectedRow).removeClass('selected active-umls');
        id = 0;
    } else {
        $(selectedRow).addClass('selected active-umls');
    }
    $(selectedRow).siblings().removeClass('selected active-umls');
    loadAtoms(id);
    loadDefinitions(id);
}

function loadAtoms(id) {
    $.ajax({
        type: "GET",
        url: `/Umls/GetAtoms?id=${id}`,
        success: function (data) {
            $('#atomsData').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function loadDefinitions(id) {
    $.ajax({
        type: "GET",
        url: `/Umls/GetDefinitions?id=${id}`,
        success: function (data) {
            $('#definitionsData').html(data);
            $('#collapseO4MTSpecificFields').collapse('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}


function selectDefinition(event) {
    var selectedLiItem;
    if ($(event.srcElement).hasClass("single-definition")) {
        selectedLiItem = $(event.srcElement);
    } else {
        selectedLiItem = $(event.srcElement).closest('.single-definition');
    }
    selectUnselectDefinition(selectedLiItem);
}

function selectUnselectDefinition(element) {
    if ($(element).hasClass('active')) {
        $(element).removeClass('active');
    } else {
        $(element).addClass('active');
    }
    $(element).siblings().removeClass('active');
}

function confirmUmlsSelection() {
    var item = $('#conceptsTableBody tr.selected')[0];
    if (item) {
        var ui = $(item).find("[data-field='ui']")[0].innerHTML;
        var name = $(item).find("[data-field='name']")[0].innerHTML;
        $('#UmlsCode').val(ui);
        $('#UmlsName').val(name);
        console.log(`https://uts.nlm.nih.gov/metathesaurus.html?cui=${ui}`);
        $("#umlsLink").attr("href", `https://uts.nlm.nih.gov/metathesaurus.html?cui=${ui}`);
        $("#umlsLink").text(`https://uts.nlm.nih.gov/metathesaurus.html?cui=${ui}`);
        populateCodes(item);
        $('#umlsModal').modal('toggle');
    }
}

function populateO4MTDefinitions() {
    if ($('.definition-data ul:first-child').text().trim() === "NO_RESULT") {
        return;
    }
    var selectedDefinition = $('.definition-data .single-definition.active').clone();
    languages.forEach(language => {
        if (!$(`#definition-${language.Value}`).val()) {
            $(selectedDefinition).find("[data-field='rootSource']").remove();
            $(`#definition-${language.Value}`).val($(selectedDefinition).text().trim());
        }
    });
}

function populateCodes(umlsConcept) {
    populateWithUmlsConcept(umlsConcept);
    populateWithAtoms();
    submitThesaurusEntryForm();
}

function populateWithAtoms() {
    $.each($("#atomsData [data-field='language'][data-value='ENG']"), function (index, value) {
        let code = {};
        $.each($(value).siblings(), function (i, nameColumn) {
            let fieldName = $(nameColumn).data("field");
            let fieldValue = $(nameColumn).data("value");
            code[fieldName] = fieldValue;
        });
        let codeSystemId = "-11";
        let codeSystem = code['rootSource'];
        let codeSystemDisplay = codeSystem;
        let codeVersion = "";
        let codeCode = code['ui'];
        let codeValue = code['name'];
        let codeVersionPublishDate = "";

        if (!existsCodeValue('#codeTable tbody', codeCode)) {
            let codingRow = addNewCodeRowToTable(codeSystemId, codeSystem, codeSystemDisplay, codeVersion, codeCode, codeValue, codeVersionPublishDate);
            $('#codeTable tbody').append(codingRow);
        }
    });
}

function populateWithUmlsConcept(umlsConcept) {
    let codeSystemId = "-1";
    let codeSystem = "MTH";
    let codeSystemDisplay = "UMLS";
    let codeVersion = "";
    let codeCode = $(umlsConcept).find("[data-field='ui']")[0].innerHTML;
    let codeValue = $(umlsConcept).find("[data-field='name']")[0].innerHTML;
    let codeVersionPublishDate = "";

    if (!existsCodeValue('#codeTable tbody', codeCode)) {
        let codingRow = addNewCodeRowToTable(codeSystemId, codeSystem, codeSystemDisplay, codeVersion, codeCode, codeValue, codeVersionPublishDate);
        $('#codeTable tbody').append(codingRow);
    }
}

function populateUmlsDefinitions() {
    if ($('.definition-data ul:first-child').text().trim() === "NO_RESULT") {
        return;
    }
    $('#UmlsDefinitions').html('');
    $.each($('.definition-data .single-definition'), function (key, value) {
        value.children[2].remove();
        var definitionIcon = document.createElement('i');
        definitionIcon.classList.add('definition-icon');
        $('#UmlsDefinitions').append(definitionIcon);
        $('#UmlsDefinitions').append($(value).html());
        $('#UmlsDefinitions').append("<hr />");
    });
    $('#UmlsDefinitions').addClass("definition-padding");
}

function populateSynonyms() {
    $.each($("#atomsData [data-field='language'][data-value='ENG']"), function (index, value) {
        $.each($(value).siblings("[data-field='name']"), function (i, nameColumn) {
            console.log($(nameColumn).data("value"));
            appendTagToContainer($(nameColumn).data("value"), 'synonym', 'en');
        });
    });
}

$(document).ready(function () {
    $('#collapseO4MTSpecificFields').collapse('show');
})

$(document).on('click', '.close-custom-modal-button', function (e) {
    closeCustomModal();
});

function closeCustomModal() {
    $('#umlsModal').modal('hide');
    $('.custom-modal').trigger('defaultZIndex');
}