var preventPushStateWhenReload;
$(document).ready(function () {
    reloadTable(true);
});

var currentPage = 1;
var tableConfigs = [];
var defaultFilter;
var columnName;
var switchcount = 0;
var isAscending = null;

$(document).on('keypress', '.filter-item input', function (e) {
    if (e.which === 13) {
        if (inputsAreInvalid()) {
            return;
        }
        try {
            pushState();
            callCorrespondingFilter($(this));

        } catch (error) {
            pushStateWithoutFilter(1);
        }

        currentPage = 1;
        reloadTable();
    }
});

function callCorrespondingFilter($filterInput) {
    if (belongsToAdvanceFilter($filterInput)) {
        advanceFilter();
    } else {
        mainFilter();
    }
}

function belongsToAdvanceFilter($filterInput) {
    return $filterInput.parents('#advancedFilterForm').length == 1;
}

function changePage(num,e, url, container, pageNumIdentifier, preventPushHistoryState) {
    e.preventDefault();
    if (url) {
        changePageSecondary(num, e, url, container, pageNumIdentifier, preventPushHistoryState);
        return;
    }
    else {
        if (!preventPushHistoryState) {
            if (currentPage !== num) {
                try {
                    let urlPageParams = `?page=${num}&pageSize=${getPageSize()}`;
                    let filter = getFilterParametersObject();
                    let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

                    history.pushState({}, '', fullUrlParams);

                } catch (error) {
                    pushStateWithoutFilter(num);
                }
            }
        }
    }
    
    currentPage = num;
    reloadTable();
}

function getFilterUrlParams(filter) {
    let result = "";
    for (const property in filter) {
         result = result.concat(`&${property}=${filter[property]}`);
    }

    return result;
}

function filterData() {
    if (inputsAreInvalid()) {
        return;
    }
    setDatetimeFieldsIfNotAlreadySet();
    pushState();
    currentPage = 1;
    reloadTable();
}

function setDatetimeFieldsIfNotAlreadySet() {
    $(".date-helper").each(function () {
        handleDateHelper($(this));
    });
    $(".time-helper").each(function () {
        handleTimePartChange(this);
    });
}

function addPropertyToObject(object, name, value) {
    if (value) {
        object[name] = value;
    }
}

function getPageSize() {
    let url = new URL(window.location.href);
    let pageSize = url.searchParams.get("pageSize");
    if (pageSize) {
        userPageSize = pageSize;
    }

    var select = document.getElementById("pageSizeSelector");
    return select ? select.options[select.selectedIndex].value : userPageSize;
}

$(document).on('change', '.pageSizeSelector', function () {
    currentPage = 1;
    try {
        pushState();

    } catch (error) {
        pushStateWithoutFilter(1);
    }

    reloadTable(true);

    tableConfigs.forEach(x => {
        reloadSecondaryTable(x.url, x.container, x.pageNumIdentifier);
    });

    updatePageSize(getPageSize());
});

function clickedRow(e, id, version) {
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item') && !$(e.target).hasClass('dots') && !$(e.target).hasClass('table-more')) {
        editEntity(event, id, version);
    }
}

function getDefaultFilter(){
    let result = {};
    if (defaultFilter) {
        Object.keys(defaultFilter).forEach(x => {
            if (defaultFilter[x]) {
                result[x] = defaultFilter[x];
            }
        });
    }

    return result;
}

function getPageNum() {
    var url = new URL(window.location.href);
    var page = url.searchParams.get("page");
    let pageNum;
    if (page) {
        pageNum = page;
    } else {
        pageNum = "1";
    }

    return pageNum;
}

$(document).ready(function () {
    $('.tooltip-tipable').tooltip();
});

function advancedFilterModal(event) {
    event.preventDefault();
    event.stopPropagation();
    $("#advancedFilterModal").modal("show");
}

function thesaurusMoreModal(event, id) {
    event.preventDefault();
    event.stopPropagation();

    $.ajax({
        type: 'GET',
        url: `/ThesaurusEntry/ThesaurusMoreContent?id=${id}`,
        success: function (data) {
            $('#thesaurusMoreModalContent').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });

    $("#thesaurusMoreModal").modal("show");
}


function pushState() {
    console.log('push state');
    if (!preventPushStateWhenReload) {
        let urlPageParams = `?page=1&pageSize=${getPageSize()}`;
        let filter = getFilterParametersObject();
        let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

        history.pushState({}, '', fullUrlParams);
    }
}

function pushStateWithoutFilter(num) {
    if (!preventPushStateWhenReload) {
        history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
    }
}

function appendDeleteIcon() {
    let deleteElement = document.createElement('img');
    $(deleteElement).addClass("edit-svg-size");
    var deleteIcon = document.getElementById("deleteIcon").src;
    $(deleteElement).attr("src", deleteIcon);

    return deleteElement;
}

function appendEditIcon() {
    let editElement = document.createElement('img');
    $(editElement).addClass("edit-svg-size");
    var editIcon = document.getElementById("editIcon").src;
    $(editElement).attr("src", editIcon);

    return editElement;
}

function setFilterTagsFromUrl() {
    let url = new URL(window.location.href);
    let entries = url.searchParams.entries();
    let params = paramsToObject(entries);

    if (defaultFilter) {
        defaultFilter = params;
    }
    document.querySelectorAll('.filter-element').forEach(function (a) {
        a.remove();
    });
    for (let param in params) {
        console.log(param);
        console.log(params[param]);
        if (params[param] && param !== 'page' && param !== 'pageSize') {
            let element = document.createElement('div');
            $(element).addClass('filter-element');
            if (isDateTimeFilter(param)) {
                $(element).html(getDateTimeFilterTag(params, param));
            } else {
                $(element).html(params[param]);
            }
            
            $(element).append(getTagCloseSign(param, params[param]));
            $('#filterElements').append(element);
        }
    }
}

function setFilterTagsFromObj(requestObject) {
    if(defaultFilter) {
        defaultFilter = params;
    }
    document.querySelectorAll('.filter-element').forEach(function (a) {
        a.remove();
    });
    let requestObjectForDisplay = Object.assign({}, requestObject);
    let params = getFilterParametersObjectForDisplay(requestObjectForDisplay);
    for (let param in params) {
        if (params[param] && isNonPagingParam(param)) {
            let element = document.createElement('div');
            $(element).addClass('filter-element');
            if (isDateTimeFilter(param)) {
                $(element).html(getDateTimeFilterTag(params, param));
            } else {
                $(element).html(params[param]);
            }

            $(element).append(getTagCloseSign(param, requestObject[param]));
            $('#filterElements').append(element);
        }
    }
}

function getTagCloseSign(name, value) {
    let x = document.createElement('img');
    $(x).attr('src', "/Content/img/icons/Administration_remove.svg");
    $(x).addClass('ml-2');
    $(x).addClass('remove-filter');
    $(x).attr('name', name);
    $(x).css('font-size', '12px');
    $(x).css('width', '15px');
    $(x).css('padding', '5px');
    $(x).attr('data-value', value);

    return x;
}

function setAdvancedFilterBtnStyle(object, excludePropertiesList) {
    if (objectHasNoProperties(object, excludePropertiesList)) {
        $('#advancedId').children('div:first').removeClass('btn-advanced');
        $('#advancedId').find('button:first').addClass('btn-advanced-link');
        $('#advancedId').find('img:first').css('display', 'none');
    } else {
        $('#advancedId').children('div:first').addClass('btn-advanced');
        $('#advancedId').find('button:first').removeClass('btn-advanced-link');
        $('#advancedId').find('img:first').css('display', 'inline-block');
    }
}

function getFilterParameterObjectForDisplay(filterObject, attributeName) {
    if (filterObject.hasOwnProperty(attributeName)) {
        let attributeId = filterObject[attributeName];
        let attributeDisplay = $(`option#${attributeName}_${attributeId}`).attr('data-display');
        if (attributeDisplay) {
            addPropertyToObject(filterObject, attributeName, attributeDisplay);
        }
    }
}

function isNonPagingParam(param) {
    return param.toLowerCase() != 'page' && param.toLowerCase() != 'pagesize';
}

function sortTable(column) {
    if (switchcount == 0) {
        if (columnName == column)
            isAscending = checkIfAsc(isAscending);
        else
            isAscending = true;
        switchcount++;
    }
    else {
        if (columnName != column)
            isAscending = true;
        else
            isAscending = checkIfAsc(isAscending);
        switchcount--;
    }
    columnName = column;
    reloadTable(columnName, isAscending);
}

function checkIfAsc(isAscending) {
    if (!isAscending)
        return true;
    else
        return false;
}

function addSortArrows()
{
    var element = document.getElementById(columnName);
    if (element != null) {
        element.classList.remove("sort-arrow");
        if (isAscending) {
            element.classList.remove("sort-arrow-desc");
            element.classList.add("sort-arrow-asc");
        }
        else {
            element.classList.remove("sort-arrow-asc");
            element.classList.add("sort-arrow-desc");
        }
    }
}