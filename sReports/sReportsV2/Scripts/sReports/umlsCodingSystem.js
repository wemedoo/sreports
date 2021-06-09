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
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error ${errorThrown}`);
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
    $(event.srcElement).closest('tr').addClass('selected').siblings().removeClass('selected');
    $(event.srcElement).closest('tr').addClass('active-umls').siblings().removeClass('active-umls');
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
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error ${errorThrown}`);
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
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error ${errorThrown}`);
        }
    });
}


function selectDefinition(event) {
    if ($(event.srcElement).hasClass("single-definition")) {
        $(event.srcElement).addClass('active').siblings().removeClass('active');
    } else {
        var element = $(event.srcElement).closest('.single-definition');
        $(element).addClass('active').siblings().removeClass('active');
    }
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
        populateUmlsDefinitions();
        populateO4MTDefinitions();
        populateSynonyms();
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
}