function loadThesaurusTree() {
    $.ajax({
        type: 'GET',
        url: `/Form/FilterThesaurusTree?o4MtId=${$('#Id').val()}&searchTerm=&thesaurusPageNum=${thesaurusPageNum}`,
        success: function (data) {
            $('#treeThesaurusContainer').html(data);
            loadThesaurusTreeStructure();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function loadThesaurusTreeStructure() {
    setFieldValueThesaurusAppearance('.fv2');
    setFieldOrFieldSetThesaurusAppearance('.f2');
    setFieldOrFieldSetThesaurusAppearance('.fs2');
    setPageOrChapterThesaurusAppearance('.p2', true);
    setPageOrChapterThesaurusAppearance('.c2', true);

    calculateLineHeight('.thesaurus-item-tree', '.c2:visible', 0);
    calculateLineHeight('.c2:visible', '.p2:visible', 0);
    calculateLineHeight('.p2:visible', '.fs2:visible', 0);
    calculateLineHeight('.fs2:visible', '.f2:visible', -19);
    calculateLineHeight('.f2:visible', '.fv2:visible', -19);
    setTreeLine();

    if ($("#treeThesaurusContainer").find(".no-result-content").length > 0)
        $("#foundInTotal").text(($("#foundTotalCount").val())).hide();

    if ($("#foundTotalCount").val() != undefined) {
        $("#foundInTotal").text($("#foundCountFilter").val()).show();
    }

    removeLoadMoreButton();
}

function loadMoreThesaurus() {
    thesaurusPageNum += 15;
    removeLoadMoreThesaurus();
    $.ajax({
        type: 'GET',
        url: `/Form/FilterThesaurusTree?o4MtId=${$('#Id').val()}&searchTerm=${$('#quickSearch').val()}&thesaurusPageNum=${thesaurusPageNum}`,
        success: function (data) {
            $('#thesaurusTreeItems').append(data);
            document.getElementById("loadMoreThesaurus").remove();
            loadThesaurusTreeStructure();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function quickSearchTerm() {
    thesaurusPageNum = 0;
    $.ajax({
        type: 'GET',
        url: `/Form/FilterThesaurusTree?o4MtId=${$('#Id').val()}&searchTerm=${$('#quickSearch').val()}&thesaurusPageNum=${thesaurusPageNum}`,
        success: function (data) {
            $('#treeThesaurusContainer').html(data);
            loadThesaurusTreeStructure();
            loadDocumentProperties("");
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

$("#quickSearch").keyup(function (event) {
    if (event.keyCode === 13) {
        quickSearchTerm();
    }
});

$(document).ready(function () {
    removeLoadMoreButton();
})

function removeLoadMoreButton() {
    if ($('#treeThesaurusContainer').find(".thesaurus-item-tree").length < thesaurusPageNum + 15)
        removeLoadMoreThesaurus();
    else
        document.getElementById("loadMoreThesaurus").style.display = "block";
}

function removeLoadMoreThesaurus() {
    if (document.getElementById("loadMoreThesaurus") != null)
        document.getElementById("loadMoreThesaurus").style.display = "none";
}

function setFieldValueThesaurusAppearance(indicatorClass) {
    $(indicatorClass).each(function (index, element) {
        if (!$(element).hasClass('main')) {
            $(element).hide();
        }
    });
}

function setFieldOrFieldSetThesaurusAppearance(indicatorClass) {
    $(indicatorClass).each(function (ind, elem) {
        if (!$(elem).hasClass('main') && $(elem).find('.main').length === 0) {
            $(elem).hide();
        }
    });
}

function setPageOrChapterThesaurusAppearance(indicatorClass, isEditThesaurus) {
    $(indicatorClass).each(function (ind, elem) {
        if (isEditThesaurus)
            $(elem).addClass("tree-background");
        if (!$(elem).hasClass('main') && $(elem).find('.main').length === 0) {
            $(elem).hide();
        }
    });
}

function setTreeLine() {
    $('.tree-item').each(function (index, treeItem) {
        let line = $(treeItem).children('.tree-item:visible:first').children('.line-tree:first');
        $(line).css("height", "23px");
        $(line).css("top", "unset");
    });
}

function calculateLineHeight(indicatorClass, secondClass, topElement) {
    $(indicatorClass).each(function (index, treeItem) {
        var countArray = getCountArray(treeItem, secondClass);
        var i = 0;
        $(treeItem).find(secondClass).each(function (index, treeFieldItem) {
            let h = 42;
            let twoLineHeight = parseInt($(treeFieldItem).css('height'), 10) - h - countArray[i] * h;
            if (twoLineHeight != 0)
                h += 12;
            let top = topElement;
            if (countArray[i - 1] >= 0) {
                if (indicatorClass != '.c2:visible' && indicatorClass != '.c:visible') {
                    h = h + countArray[i - 1] * 42 + twoLineHeight;
                    if (topElement == -19)
                        top = top - countArray[i - 1] * 42 - twoLineHeight;
                    else
                        top = top - countArray[i - 1] * 42 - 19 - twoLineHeight;
                    if ((secondClass == '.f2:visible' || secondClass == '.f:visible') & countArray[i - 1] > 0) {
                        top -= 19;
                        h += 19;
                    }
                }
                else {
                    h = h + countArray[i - 1] * 42;
                    top = top - countArray[i - 1] * 42 - 19;
                }
            }
            $(treeFieldItem).children('.line-tree').css('height', `${h}px`);
            $(treeFieldItem).children('.line-tree').css('top', `${top}px`);
            i++;
        });
    });
}

function getCountArray(treeItem, secondClass){
    var countArray = [];
    $(treeItem).find(secondClass).each(function (index, treeFieldItem) {
        var count = $(treeFieldItem).find(".tree-item:visible").length;
        countArray.push(count);
    });
    return countArray;
}
