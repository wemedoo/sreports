$(document).on('click', '.edit-patient-button', function (e) {
    let patientId = $(this).attr('id');
    $.ajax({
        method: 'get',
        url: `/Patient/EditPatientInfo?patientId=${patientId}`,
        success: function (data) {
            $('#patientContainer').html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(thrownError);
        }
    })
});

function reloadPatientTree(parentId, activeElementId, type) {
    console.log(activeElementId);
    let queryString = `patientId=${getPatientId()}`;
    if (type === 'encounter') {
        queryString = `${queryString}&episodeOfCareId=${parentId}&encounterId=${activeElementId}`;
    } else if (type === 'episodeofcare') {
        queryString = `${queryString}&episodeOfCareId=${activeElementId}`;
    }
    $.ajax({
        type: "GET",
        url: `/Patient/ReloadTree?${queryString}`,
        success: function (data) {
            $("#patientTree").html(data);
            setDefaultTreeSelectedValue(getPatientId(), activeElementId, type);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function addNewEpisodeOfCare(patientId) {
    if ($('.patient-tree').find('.expanded').length > 0) {
        window.history.pushState('', {}, `/Patient/Edit?patientId=${patientId}`)
    }
    loadEpisodeOfCareForm(patientId);
    setExpandedElement(null);
    setSelectedElement(null);
}

function loadEpisodeOfCareForm(patientId) {
    $.ajax({
        type: "GET",
        url: `/EpisodeOfCare/CreateFromPatient?patientId=${patientId}`,
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

$(document).on('click', '.single-episode-of-care-container', function (e) {
    if (!$(this).hasClass("selected")) {
        window.history.pushState({}, '', `/Patient/Edit?patientId=${getPatientId()}&episodeOfCareId=${$(this).attr('id')}`)
        reloadEpisodeOfCareForm($(this).attr('id'));
        loadEncountersData($(this).attr('id'));
    }
});

function loadEncountersData(episodeOfCareId) {
    if ($(`#${episodeOfCareId}`).find('.single-encounter').length > 0) {
        return;
    };

    $.ajax({
        method: 'GET',
        url: `/Encounter/PatientTreeItems?episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            $(`#${episodeOfCareId} .encounter-container-items`).html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    })
}

function addNewEncounter(event, episodeOfCareId) {
    event.preventDefault();
    event.stopPropagation();
    $.ajax({
        type: "GET",
        url: `/Encounter/CreateFromPatient?patientId=${getPatientId()}&episodeOfCareId=${episodeOfCareId}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
    setPushHistoryWhenAddingNewEncounter(episodeOfCareId);
    setSelectedElement(null);
    setExpandedElement(`#${episodeOfCareId}`);

}

$(document).on('click', '.single-encounter', function (e) {
    e.preventDefault();
    e.stopPropagation();
    if (!$(this).hasClass("selected")) {
        let episodeOfCareId = $(this).closest('.single-episode-of-care-container').attr('id');
        let encounterId = $(this).attr('id');
        reloadEncounterForm(encounterId);
        window.history.pushState({}, '', `/Patient/Edit?patientId=${getPatientId()}&episodeOfCareId=${episodeOfCareId}&encounterId=${$(this).attr('id')}`)
    }
});

function setDefaultTreeSelectedValue(parentId, activeElementId, type) {
    if (activeElementId) {
        if (type === 'episodeofcare') {
            reloadEpisodeOfCareForm(activeElementId);
        } else if (type === 'encounter') {
            reloadEncounterForm(activeElementId);
        } else if (type === 'forminstance') {
            let episodeOfCare = $(`#${parentId}`).closest('.single-episode-of-care-container');
            setExpandedElement(episodeOfCare);
            setSelectedElement($(`#${parentId}`));
            loadFormForEdit(activeElementId, episodeOfCare, parentId);
        }
    } else {
        if (parentId) {
            let encounter = $(`#${parentId}`).find('.single-encounter').last();
            reloadEncounterForm($(encounter).attr('id'));
        } else {
            loadEpisodeOfCareForm(getPatientId());
        }
    }
}

$(document).on('click', '.single-document-item:not(.add-new-document-item-button)', function (e) {
    window.history.pushState({}, '', `/Patient/Edit?patientId=${getPatientId()}&episodeOfCareId=${$('#eocId').val()}&encounterId=${$('#id').val()}&formInstanceId=${$(this).data('id')}`)
    loadFormForEdit($(this).data('id'), $('#eocId').val(), $('#id').val());
});

$(document).on('click', '.document-horizontal-single-item', function (e) {
    let encounterId = $('input[name=encounterId]').first().val();
    let formInstanceId = $(this).data('id');
    window.history.pushState({}, '', `/Patient/Edit?patientId=${getPatientId()}&episodeOfCareId=${getEpisodeOfCareId()}&encounterId=${encounterId}&formInstanceId=${formInstanceId}`)
    loadFormForEdit(formInstanceId, $('#episodeOfCareId').val(), encounterId);
});

$(document).on('click', '.single-pinned-item', function (e) {
    showForm($(this).data('id'));
})

function loadDynamicForm(e) {
    let selectedFormElement = $('.single-form-list-element.active').first();
    if (selectedFormElement.length === 0) {
        toastr.error('Please select a document');
        return;
    }
    let formId = $(selectedFormElement).data('id');

    showForm(formId);
}

function showForm(formId) {
    let episodeOfCareId = $('#eocId').val();
    let encounterId = $('#id').val();
    $.ajax({
        type: "GET",
        url: `/DiagnosticReport/CreateFromPatient?encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}&patientId=${getPatientId()}&formId=${formId}&${getReferralsAsParameter()}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
            closeCustomModal();

        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

$(document).on('click', '.show-form-referrals-button', function (e) {
    e.preventDefault();
    console.log('listing');
    let episodeOfCareId = $('#eocId').val();
    let encounterId = $('#id').val();
    $.ajax({
        method: 'get',
        url: `/Encounter/ListReferralsAndForms?encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}`,
        success: function (data) {
            let modalMainContent = document.getElementById('customModalMainContent');
            modalMainContent.innerHTML = data;
            $('body').addClass('no-scrollable');
            $('.custom-modal').addClass('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(thrownError);
        }
    })
})

$(document).on('click', '.single-referral-item', function (e) {
    $(this).toggleClass('active');
})

$(document).on('click', '.single-form-list-element', function (e) {
    $(this).siblings().removeClass('active');
    $(this).toggleClass('active');
});

var loadingForms;
$(document).on('keyup', '#searchCondition', function (e) {

    if (loadingForms) {
        loadingForms.abort();
    }
        loadingForms = $.ajax({
            method: 'GET',
            url: `/Encounter/ListForms?condition=${$(this).val()}`,
            success: function (data) {
                $('#formsContainer').html(data);
                loadingForms = null;
            },
            error: function (xhr, ajaxOptions, thrownError) {

            }
        })
})

function reloadEpisodeOfCareForm(eocId) {
    if (eocId != 0)
    {
        $.ajax({
            type: "GET",
            url: `/EpisodeOfCare/EditFromPatient?episodeOfCareId=${eocId}`,
            data: {},
            success: function (data) {
                $("#patientContainer").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });
        setExpandedElement($(`#${eocId}`));
        setSelectedElement($(`#${eocId}`));
    }
}

function reloadEncounterForm(encounterId) {
    $.ajax({
        type: "GET",
        url: `/Encounter/EditFromPatient?encounterId=${encounterId}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
    setSelectedElement($(`#${encounterId}`));
}


//collapsed is used to show/hide content of expanded episode of care
$(document).on('click', '.single-episode-of-care-container .arrow-icon', function (e) {
    let eocContainer = $(e.target).closest('.single-episode-of-care-container');
    if (eocContainer.hasClass('expanded')) {
        e.stopPropagation();
        e.preventDefault();
        $(eocContainer).toggleClass('collapsed');
    }
})


//expanded is used to keep information about expanded element, can be only an episode of care
function setExpandedElement(element) {
    $('.patient-tree').find('.single-episode-of-care-container.expanded').removeClass('expanded');
    if (element) {
        $(element).addClass('expanded');
    }
}

//selected is used to keep the information about selected element in the tree, eoc or encounter
function setSelectedElement(element) {
    console.log(element);
    $('.patient-tree').find('.selected').removeClass('selected');
    if (element) {
        $(element).addClass('selected');
        if ($(element).hasClass('single-encounter')) {
            expandParent(element);
        }
    }
}

function setPushHistoryWhenAddingNewEncounter(episodeOfCareId) {
    let selected = $('.patient-tree').find('.selected');
    if (selected.length > 0) {
        let selectedId = $(selected[0]).attr('id');
        if (selectedId !== episodeOfCareId) {
            window.history.pushState({}, '', `/Patient/Edit?patientId=${getPatientId()}&episodeOfCareId=${episodeOfCareId}`)
        }
    }
}

function expandParent(element) {
    let parent = $(element).closest('.single-episode-of-care-container').first();
    if (parent) {
        if (!$(parent).hasClass('expanded')) {
            setExpandedElement(parent);
        }
    }
}

function handleSuccessFormSubmit() {
    let encounterId = $('input[name=encounterId]').val();
    reloadEncounterForm(encounterId);
}

function getReferralsAsParameter() {
    let referralParams = [];
    let referrals = getSelectedReferrals();
    referrals.forEach(x => {
        if (x) {
            referralParams.push(`referrals=${x}`);
        }
    });

    return referralParams.join('&');
}

function getSelectedReferrals() {
    let result = [];
    $('.single-referral-item.active').each(function (index, element) {
        result.push($(this).data('id'));
    })

    return result;
}

function loadFormForEdit(formInstanceId, episodeOfCareId, encounterId) {

    $.ajax({
        type: "GET",
        url: `/DiagnosticReport/EditFromPatient?formInstanceId=${formInstanceId}&encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

$(document).on('click', '.scroll-right-button', function (e) {
    let documentsContainer = $(this).siblings('.documents-list-horizontal');
    var leftPos = $(documentsContainer).scrollLeft();
    $(documentsContainer).animate({ scrollLeft: leftPos + 200 }, 800);
});

$(document).on('click', '.scroll-left-button', function (e) {
    let documentsContainer = $(this).siblings('.documents-list-horizontal');
    var leftPos = $(documentsContainer).scrollLeft();
    $(documentsContainer).animate({ scrollLeft: leftPos - 200 }, 800);
});

$(document).on('click', '.pin-icon:not(.pinned)', function (e) {
    e.preventDefault();
    e.stopPropagation();

    let id = $(this)
        .parent()
        .attr('data-id');
    $(this).addClass('pinned');
    $.ajax({
        method: 'PUT',
        url: `/User/AddSuggestedForm?formId=${id}`,
        success: function (data) {
            toastr.success('Success');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    })
})


$(document).on('click', '.pinned', function (e) {
    e.preventDefault();
    e.stopPropagation();
    let id = $(this)
        .parent()
        .attr('data-id');
    $(this).removeClass('pinned');
    $.ajax({
        method: 'PUT',
        url: `/User/RemoveSuggestedForm?formId=${id.trim()}`,
        success: function (data) {
            toastr.success('Success');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    })
})
/*Modal*/

$(document).on('click', '.custom-modal-body', function (e) {
    e.stopPropagation();
});

$(document).on('click', '.custom-modal', function (e) {
    closeCustomModal();
});

$(document).on('click', '.close-custom-modal-button', function (e) {
    closeCustomModal();
});

$(document).on('click', '.cancel-dynamic-form', function (e) {
    if ($('input[name=formInstanceId]').val()) {
        let episodeOfCareId = $('.patient-tree').find('.expanded').attr('id');
        let encounterId = $('.patient-tree').find('.selected').attr('id');
        window.history.pushState({}, '', `/Patient/Edit?patientId=${getPatientId()}&episodeOfCareId=${episodeOfCareId}&encounterId=${encounterId}`);
    }
    handleSuccessFormSubmit();
})

function getPatientId() {
    return $("#patientTree").data("patientid");
}

function getEpisodeOfCareId() {
    let eocElement = $('.single-episode-of-care-container.expanded').first();
    return $(eocElement).attr('id');
}

function closeCustomModal() {
    $('.custom-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
}

function goToFormInstanceEdit(id, versionId) {
    window.location.href = `/FormInstance/Edit?VersionId=${versionId}&FormInstanceId=${id}`;
}