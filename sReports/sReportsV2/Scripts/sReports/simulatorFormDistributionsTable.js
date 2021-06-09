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

function generateDocuments() {
    let numOfDocuments = +$(`#generateDocumentsCount`).val();
    if (numOfDocuments && numOfDocuments > 0) {
        let requestObject = {};
        requestObject['formId'] = $('#formId').val();
        requestObject['numOfDocuments'] = numOfDocuments;
        $.ajax({
            type: 'GET',
            url: '/FormDistribution/GenerateDocuments',
            data: requestObject,
            success: function (data) {
                toastr.success('Documents are generated');
                $('#generateDocumentsModal').modal('hide');


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

function openGenerateDocumentsModal(event, formTitle, formId) {
    //event.stopPropagation();
    event.preventDefault();
    event.stopPropagation();
    $('#formId').val(formId);
    $('#formTitle').text(formTitle);
    $('#generateDocumentsModal').modal('show');
}

function editEntity(id) {
    history.pushState(null, '', `?page=${currentPage}&pageSize=${getPageSize()}`);
    window.location.href = `/FormDistribution/Edit?formDistributionId=${id}`;
}
