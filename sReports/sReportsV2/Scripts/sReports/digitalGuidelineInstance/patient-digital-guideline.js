function addNewGuidelineInstance(e) {
    e.preventDefault();
    let episodeOfCareId = $('#patientGeneralInfoContainer').attr('data-eocid');
    $.ajax({
        method: 'get',
        url: `/DigitalGuidelineInstance/ListDigitalGuidelines?episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            let modalMainContent = document.getElementById('guidelineInstanceMainContent');
            modalMainContent.innerHTML = data;
            $('body').addClass('no-scrollable');
            $('.guideline-instance-modal').addClass('show');
            $('.guideline-instance-modal').trigger('lowZIndex');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function insertGuidelineInstance(e) {
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a digital guideline');
        return;
    }
    let digitalGuidelineId = $(selectedFormElement).attr('data-guidelineid');
    let title = $(selectedFormElement).attr('data-guidelinetitle');
    let episodeOfCareId = $('#patientGeneralInfoContainer').attr('data-eocid');
    $.ajax({
        type: "POST",
        url: `/DigitalGuidelineInstance/Create?episodeOfCareId=${episodeOfCareId}&digitalGuidelineId=${digitalGuidelineId}&title=${title}`,
        success: function (data) {
            reloadDigitalGuidelineTable(episodeOfCareId);
            closeGuidelineInstanceModal();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function searchDigitalGuideline(){
    var title = $('#searchDigitalGuideline').val();
    $.ajax({
        method: 'GET',
        url: `/DigitalGuidelineInstance/FilterDigitalGuidelines?title=${title}`,
        success: function (data) {
            $('#formsContainer').html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function removeGuidelineInstance(){
    var guidelineInstanceId = document.getElementById("buttonSubmitDelete").getAttribute('data-id')

    $.ajax({
        type: "DELETE",
        url: `/DigitalGuidelineInstance/Delete?guidelineInstanceId=${guidelineInstanceId}`,
        success: function (data) {
            toastr.success(`Success`);
            console.log(guidelineInstanceId);
            $(`#${guidelineInstanceId}`).remove();
            $('#deleteModal').modal('hide');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
    //reloadDigitalGuidelineTable(episodeOfCareId);
}

function previewInstanceNode(event, data, elementId, guidelineId) {
    $.ajax({
        method: 'post',
        url: `/DigitalGuidelineInstance/PreviewInstanceNode?guidelineInstanceId=${elementId}&guidelineId=${guidelineId}`,
        data: data,
        success: function (data) {
            $(`#nodePreview-${elementId}`).html(data);
            $('#showInstanceNodeButton').click();
            showInstanceNodePreview(event, elementId);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function previewInstanceDecisionNode(event, data, elementId, guidelineId) {
    $.ajax({
        method: 'post',
        url: `/DigitalGuidelineInstance/PreviewInstanceDecisionNode?guidelineInstanceId=${elementId}&guidelineId=${guidelineId}`,
        data: data,
        success: function (data) {
            $(`#nodePreview-${elementId}`).html(data);
            $('#showInstanceNodeButton').click();
            showInstanceNodePreview(event, elementId);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function unselectInstanceNode(elementId) {
    $(`#nodePreview-${elementId}`).hide();
}

function showInstanceNodePreview(event, elementId) {
    event.preventDefault();
    console.log(elementId);
    $(event.target).siblings().removeClass('active');
    $(event.target).addClass('active');
    $(`#nodePreview-${elementId}`).show();
}

function loadGraph(id, digitalGuidelineId, refreshIfExisting) {
    if (!refreshIfExisting) {
        $(".guideline-container").remove();
        $(".guideline-header").remove();
    }
    toggleGuidelineInstances(id);
    $.ajax({
        method: 'GET',
        url: `/DigitalGuidelineInstance/LoadGraph?guidelineInstanceId=${id}&guidelineId=${digitalGuidelineId}`,
        success: function (data) {
            appendGraphToTable(id, data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function toggleGuidelineInstances(guidelineInstanceId) {
    var previousSelected = $('.single-guideline-instance-container.selected');
    $(previousSelected).removeClass('selected');

    var currentSelected = $(`#${guidelineInstanceId}`);
    setGuidelineInstanceArrows($(currentSelected).siblings().find('i.fas.fa-chevron-down'), $(currentSelected).siblings().find('i.fas.fa-chevron-up'));

    toggleCurrentSelected($(currentSelected));
    $(currentSelected).addClass('selected');
}

function toggleCurrentSelected(currentSelected) {
    if ($(currentSelected).find('i.fas.fa-chevron-up').hasClass("hide")) {
        setGuidelineInstanceArrows($(currentSelected).find('i.fas.fa-chevron-up'), $(currentSelected).find('i.fas.fa-chevron-down'));
    } else {
        setGuidelineInstanceArrows($(currentSelected).find('i.fas.fa-chevron-down'), $(currentSelected).find('i.fas.fa-chevron-up'));
    }
}

function setGuidelineInstanceArrows($itemsToShow, $itemsToHide) {
    $itemsToShow.removeClass("hide");
    $itemsToHide.addClass("hide");
}

$(document).on('keyup', '#searchDigitalGuideline', function (e) {
    searchDigitalGuideline();
})

function backToPatient(patientId, episodeOfCareId) {
    window.location.href = `/Patient/Edit?patientId=${patientId}&episodeOfCareId=${episodeOfCareId}`;
}

function markAsCompleted(id) {
    var value = $("#valueId").val();
    var guidelineInstanceId = $("#nodeGuidelineInstanceId").val();
    var guidelineId = $("#nodeGuidelineId").val();
    $.ajax({
        method: 'GET',
        url: `/DigitalGuidelineInstance/MarksAsCompleted?value=${value}&nodeId=${id}&guidelineInstanceId=${guidelineInstanceId}&guidelineId=${guidelineId}`,
        success: function (data) {
            appendGraphToTable(guidelineInstanceId, data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
    loadGraph(guidelineInstanceId, guidelineId, true);
}

function appendGraphToTable(guidelineInstanceId, data) {
    if (!document.getElementById(`digitalInstanceCy-${guidelineInstanceId}`).classList.contains("collapseGraph")) {
        $(`#digitalInstanceCy-${guidelineInstanceId}`).html(data);
        var x = document.getElementsByClassName("digitalInstanceCy");
        for (i = 0; i < x.length; i++) {
            if (x[i].id == `digitalInstanceCy-${guidelineInstanceId}`) {
                $(`#digitalInstanceCy-${guidelineInstanceId}`).addClass("collapseGraph");
            }
            else {
                $(`#${x[i].id}`).removeClass("collapseGraph");
            }
        }
    }
    else
        $(".digitalInstanceCy").removeClass("collapseGraph");
}

function addValueFromDocument(e) {
    e.preventDefault();
    let episodeOfCareId = $('#episodeOfCareId').val();
    $.ajax({
        method: 'get',
        url: `/DigitalGuidelineInstance/ListGuidelineDocuments?episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            let modalMainContent = document.getElementById('guidelineDocumentMainContent');
            modalMainContent.innerHTML = data;
            $('body').addClass('no-scrollable');
            $('.guideline-document-modal').addClass('show');
            $('.guideline-document-modal').trigger('lowZIndex');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function searchGuidelineDocument() {
    var title = $('#searchDigitalGuidelineDocument').val();
    let episodeOfCareId = $('#patientGeneralInfoContainer').attr('data-eocid');

    $.ajax({
        method: 'GET',
        url: `/DigitalGuidelineInstance/FilterGuidelineDocuments?episodeOfCareId=${episodeOfCareId}&title=${title}`,
        success: function (data) {
            $('#formsDocumentContainer').html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

$(document).on('keyup', '#searchDigitalGuidelineDocument', function (e) {
    searchGuidelineDocument();
})

function insertValueFromDocument(e) {
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a document');
        return;
    }
    let formInstanceId = $(selectedFormElement).data('forminstanceid');
    let thesaurusId = $("#nodeThesaurusId").val() ? $("#nodeThesaurusId").val() : -1;
    $.ajax({
        method: 'GET',
        url: `/DigitalGuidelineInstance/GetValueFromDocument?formInstanceId=${formInstanceId}&thesaurusId=${thesaurusId}`,
        success: function (data) {
            if (data == "")
                toastr.error(`Not found field with same thesaurus id in selected document!`)
            else
                document.getElementById('valueId').value = data;
            closeGuidelineInstanceModal();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function chooseCondition(event, nodeId, digitalGuidelineId, guidelineInstanceId) {
    event.preventDefault();
    $.ajax({
        method: 'get',
        url: `/DigitalGuidelineInstance/GetConditions?nodeId=${nodeId}&digitalGuidelineId=${digitalGuidelineId}&guidelineInstanceId=${guidelineInstanceId}`,
        data: {},
        success: function (data) {
            let modalMainContent = document.getElementById('guidelineConditionMainContent');
            modalMainContent.innerHTML = data;
            $('body').addClass('no-scrollable');
            $('.guideline-condition-modal').addClass('show');
            $('.guideline-condition-modal').trigger('lowZIndex');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function saveCondition() {
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a condition');
        return;
    }
    let condition = $(selectedFormElement).data('condition');
    var nodeId = $('#nodeId').val();
    let guidelineInstanceId = $('#guidelineInstanceId').val();
    let digitalGuidelineId = $('#digitalGuidelineId').val();

    $.ajax({
        method: 'get',
        url: `/DigitalGuidelineInstance/SaveCondition?condition=${condition}&nodeId=${nodeId}&guidelineInstanceId=${guidelineInstanceId}&digitalGuidelineId=${digitalGuidelineId}`,
        data: {},
        success: function (data) {
            closeGuidelineInstanceModal();
            appendGraphToTable(guidelineInstanceId, data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
    loadGraph(guidelineInstanceId, digitalGuidelineId, true);
}


function reloadDigitalGuidelineTable(eocId) {
    $.ajax({
        type: "GET",
        url: `/DigitalGuidelineInstance/GuidelineInstanceTable?episodeOfCareId=${eocId}`,
        data: {},
        success: function (data) {
            $("#guidelineTableContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function closeGuidelineInstanceModal() {
    $('.digital-guideline-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
    $('.digital-guideline-modal').trigger('defaultZIndex');
}

$(document).on('click', '.close-guideline-instance-modal-button', function (e) {
    closeGuidelineInstanceModal();
});

$(document).on('click', '.single-form-list-element', function (e) {
    $(this).siblings().removeClass('active');
    $(this).toggleClass('active');
});