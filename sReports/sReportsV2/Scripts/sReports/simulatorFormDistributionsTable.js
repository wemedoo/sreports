function reloadTable() {
    if (columnName != null) {
        reloadSecondaryTable("/FormDistribution/ReloadForms", "formsTableContainer", "formsCurrentPage")
    }
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
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
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
