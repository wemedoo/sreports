function reloadTable() {
    //setFilterFromUrl();

    let requestObject = {};
    checkUrlPageParams();
    checkSecondaryPage();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();



    $.ajax({
        type: 'GET',
        url: '/FormDistribution/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
            $('#tableContainer').show();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
            $('#tableContainer').hide();
        }
    });
}

function generateDocuments(formId) {
    let numOfDocuments = +$(`#num_of_documents_${formId}`).val();
    if (numOfDocuments && numOfDocuments > 0) {
        let requestObject = {};
        requestObject['formDistributionId'] = formId;
        requestObject['numOfDocuments'] = numOfDocuments;
        $.ajax({
            type: 'GET',
            url: '/FormDistribution/GenerateDocuments',
            data: requestObject,
            success: function (data) {
                toastr.success('Documents are generated');

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
                $('#tableContainer').hide();
            }
        });      
    } else {
        toastr.warning('Please define number of documents');
    }
}

function editEntity(id) {
    history.pushState(null, '', `?page=${currentPage}&pageSize=${getPageSize()}`);
    window.location.href = `/FormDistribution/Edit?formDistributionId=${id}`;
}
