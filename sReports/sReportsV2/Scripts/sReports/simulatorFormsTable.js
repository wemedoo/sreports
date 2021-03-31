function reloadSecondaryTable(url, container, pageNumIdentifier) {
    //setFilterFromUrl();
    let requestObject = {};
    checkUrlPageParams();
    checkSecondaryPage();
    requestObject.Page = window[pageNumIdentifier];
    requestObject.PageSize = getPageSize();


    $.ajax({
        type: 'GET',
        url: `${url}`,
        data: requestObject,
        success: function (data) {
            $(`#${container}`).html(data);
            $(`#${container}`).show();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
            $(`#${container}`).hide();
        }
    });
}

function changePageSecondary(num, e, url, container, pageNumIdentifier) {
    e.preventDefault();
    formsCurrentPage = num;
    history.pushState({}, '', `?page=${currentPage}&pageSize=${getPageSize()}&secondaryPage=${num}`);
    reloadSecondaryTable(url, container, pageNumIdentifier);
}

function redirectToDistributionParams(thesaurusId) {
    window.location.href = `/FormDistribution/GetByThesaurusId?thesaurusId=${thesaurusId}`;
}
