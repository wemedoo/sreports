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
    $('#conceptsTableBody').append(data);
}

function handleEmptySearchResult() {
    $('#loadMoreButtonContainer').hide();
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
    $('.umls-clearable-content').empty();
    $('#loadMoreButtonContainer').hide();    

    currentPage = 1;
}

function showModal(event) {
    /*$('#conceptsTableBody').html('');
    $('#umlsTerm').val('');*/
    event.stopPropagation();
    $('#loadMoreButtonContainer').hide();
    $('#umlsModal').modal('show');
}



function loadDetails(event, id) {
    $(event.srcElement).closest('tr').addClass('selected').siblings().removeClass('selected');
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

    if ($(event.srcElement).html().trim() === "NO_RESULT") {
        return;
    }

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
        $('#UmlsDefinitions').append($(value).html());
        $('#UmlsDefinitions').append("<hr />");
    });
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