$(document).ready(function () {
    reloadTable(true);
});

var currentPage = 1;
var tableConfigs = [];
var defaultFilter;
$(document).on('keypress', '.filter-item input', function (e) {
    if (e.which === 13) {
        try {
            let urlPageParams = `?page=1&pageSize=${getPageSize()}`;
            let filter = getFilterParametersObject();
            let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

            history.pushState({}, '', fullUrlParams);

        } catch (error) {
            pushStateWithoutFilter(1);
        }

        currentPage = 1;
        reloadTable();
    }
});
function pushStateWithoutFilter(num)
{
    history.pushState({}, '', `?page=${num}&pageSize=${getPageSize()}`);
}

function changePage(num,e, url, container, pageNumIdentifier) {
    e.preventDefault();
    if (url) {
        changePageSecondary(num, e, url, container, pageNumIdentifier);
        return;
    }
    else {
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
    let urlPageParams = `?page=1&pageSize=${getPageSize()}`;
    let filter = getFilterParametersObject();
    let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

    history.pushState({}, '', fullUrlParams);
    currentPage = 1;
    reloadTable();
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
        let urlPageParams = `?page=1&pageSize=${getPageSize()}`;
        let filter = getFilterParametersObject();
        let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

        history.pushState({}, '', fullUrlParams);

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
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item')) {
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
