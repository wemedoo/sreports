

function loadOutsideUsers() {
    let consensusId = $("#consensusId").val();
    if (consensusId) {
        $.ajax({
            method: 'get',
            url: `/FormConsensus/GetOutsideUsers?consensusId=${consensusId}`,
            contentType: 'application/json',
            success: function (data) {
                $("#usersOutsideSystem").html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.error(jqXHR.statusText);
            }
        })
    }
}

function loadInsideUsers() {
    let consensusId = $("#consensusId").val();
    if (consensusId) {
        $.ajax({
            method: 'get',
            url: `/FormConsensus/GetInsideUsers?consensusId=${consensusId}`,
            contentType: 'application/json',
            success: function (data) {
                $("#usersInsideSystem").html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.error(jqXHR.statusText);
            }
        })

    }
}

function reloadOrganizationUsersInfo() {
 $.ajax({
        method: 'get',
        url: `/FormConsensus/GetOrganizationUserInfo`,
        contentType: 'application/json',
        success: function (data) {
            $("#consensusOrganizationUserInfo").html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
 })
}


$(document).on('click', "#consensusBtn", function (e) {
    if ($(this).hasClass('active')) {
        deactivateConsensusMode();
    } else {
        activateConsensusMode();
    }
});

function deactivateConsensusMode() {
    $("#consensusBtn").removeClass('active');
    $('.consensus-hidden').show();
    $('.consensus-visible').hide();
    $('#formPreviewContainer').removeClass('w-100');
}

function activateConsensusMode() {
    $("#consensusBtn").addClass('active');
    $('.consensus-hidden').hide();
    loadConsensusPartial();
    $('#formPreviewContainer').addClass('w-100');
}

function loadConsensusTree() {
    let formId = $("#formId").val();

    $.ajax({
        method: 'get',
        url: `/FormConsensus/ReloadConsensusTree?formId=${formId}`,
        contentType: 'application/json',
        success: function (data) {
            $('#consensusTree').html(data);
            $('.consensus-visible').show();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
}



$(".consensus-checkbox").click(function () {
    let currentValue = $(this).is(':checked');
    let siblings;
    if ($(this).attr('name') === 'Form') {
        siblings = $(`.consensus-checkbox`);

    } else {
        siblings = $(`[name="${$(this).attr('name')}"]`);
        siblings.push($('[name="Form"]'));
    }

    $(siblings).each(function (index, element) {
        $(element).prop('checked', false);
    });

    $(this).prop('checked', currentValue);
});


function proceed() {
    let request = [];
    $(".consensus-checkbox").each(function (index, element) {
        if ($(element).is(":checked")) {
            let parameter = {};
            parameter["Level"] = $(element).attr('name');
            parameter["IsSame"] = $(element).attr('data-same');
            request.push(parameter);
        }
    });

    let formId = $("#nestable").find(`li[data-itemtype='form']`).attr('data-id');


    $.ajax({
        method: 'post',
        data: JSON.stringify(request),
        url: `/FormConsensus/ProceedConsensus?formId=${formId}`,
        contentType: 'application/json',
        success: function (data) {
            $('#consensusTree').html(data);
            $('.consensus-decision').hide();
            $("#usersCosensusTab").removeClass("d-none");
            $("#trackerTab").removeClass("d-none");

            toastr.success("Success")
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
}

function openAddQuestion(id) {
    clearCreateQuestion();
    $('.tree-item').addClass('c-tree-item-margin');
    $('.question-create').hide();
    $(`#qc-${id}`).show();
    //scrollToElement($(`#qc-${id}`), 1000, 50);
    $('.question-preview').hide();
}

function adddNewAnswer(id) {
    let clone = $(`#answers-${id}`).find('.answer:last').clone();
    let answerCount = $(`#answers-${id}`).find('.answer').length;
    $(clone).find('.answer-label').text(`Answer ${++answerCount}:`)
    $(clone).find('.answer-value').val('');
    $(`#answers-${id}`).append(clone);
}

function finalizeQuestion(id) {
    let request = {};
    request["ItemRef"] = id;
    request["Question"] = $(`#qc-${id}`).find('.question-value:first').val();
    request["Options"] = [];

    $(`#qc-${id}`).find('.answer-value').each(function (index, element) {
        if ($(element).val()) {
            request["Options"].push($(element).val());
        }
    });


    if (request["Question"] && request["Options"].length > 0) {
        submitQuestion(request);
    } else {
        toastr.error("Question and answers are required!")
    }
    
    
}

function submitQuestion(request) {
    let formId = $("#nestable").find(`li[data-itemtype='form']`).attr('data-id');
    let iterationId = $("#iterationId").val();
    $.ajax({
        method: 'post',
        data: JSON.stringify(request),
        url: `/FormConsensus/AddQuestion?formId=${formId}&iterationId=${iterationId}`,
        contentType: 'application/json',
        success: function (data) {
            $('#consensusTree').html(data);
            toastr.success("Succsess");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
}

function clearCreateQuestion() {
    $('.tree-item').removeClass('c-tree-item-margin');
    $('.question-create').hide();
    $('.question-preview').show();

    $(`.qc-container`).each(function (ind, ele) {
        let isFirst = true;
        $(ele).find('.answer').each(function (index, element) {
            if (isFirst) {
                isFirst = false;
                $(element).find('.answer-value').val('');
            } else {
                $(element).remove();
            }
        });
    })

    $(`.qc-container`).find('.question-value').val('');

}

function openPopupComment(id) {
    $('.popuptext').each(function (index, element) {
        if ($(element).closest('.popup').find('.fa-comment:first').attr('data-itemref') !== id) {
            $(element).removeClass('show');
        }
    });
    var popup = document.getElementById(`popup-${id}`);
    if (!$(popup).hasClass('show')) {
        popup.classList.toggle("show");
    } else
    {
        $(popup).removeClass('show');
    }
}

function showConsensusFormPreview(id, element) {
    let formId = $("#formId").val();

    $.ajax({
        method: 'get',
        url: `/FormConsensus/GetConsensusFormPreview?formId=${formId}`,
        contentType: 'application/json',
        success: function (data) {
            $(`#${id}`).html(data);
            showConsensusQuestionaire(id, element);
            $(`#${id}`).find('.form-button-container').hide();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
}

function showConsensusQuestionaire(id, element) {
    $(".consensus-tab").removeClass('active-item');
    $(element).addClass('active-item');
    $(".consensus-tab-target").hide();
    $(`#${id}`).show();
}

function filterOrganizationHierarchy() {
    let name = $('#organizationName').val();
    let countries = [];
    $('.country-filter-element').each(function (index, element) {
        countries.push($(element).attr('data-value'));
    });

    $.ajax({
        method: 'post',
        data: JSON.stringify(countries),
        url:`/FormConsensus/GetUserHierarchy?name=${name}`,
        contentType: 'application/json',
        success: function (data) {
            $("#organizationHierarchy").html(data);
            updateNumberOfSelectedUsers();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    })


    $("#consensusUsers").find('.selected-users-container:first').remove();

}

$(document).on('click', ".organization-checkbox", function (e) {
    if ($(this).is(':checked')) {
        $(this).closest('.user-children').addClass('item-active');
    } else {
        $(this).closest('.user-children').removeClass('item-active');
    }

    filterUsers();
});


function filterUsers() {
    let organizationIds = [];
    $(".organization-checkbox:checked").each(function (index, element) {
        organizationIds.push($(element).val());
    });

    $.ajax({
        method: 'post',
        data: JSON.stringify(organizationIds),
        url: `/FormConsensus/ReloadUsers`,
        contentType: 'application/json',
        success: function (data) {
            $("#consensusUsers").html(data);
            updateNumberOfSelectedUsers();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    })
}

$(document).on("click", ".consensus-organization-checkbox", function () {
    if ($(this).is(":checked")) {
        $(this).closest('.organization-with-users').find(".user-children").addClass('item-active');
    } else {
        $(this).closest('.organization-with-users').find(".user-children").removeClass('item-active');
    }
    $(this).closest('.organization-with-users').find(".consensus-user-checkbox").prop("checked", $(this).is(":checked"));

});

$(document).on("click", ".consensus-user-checkbox", function () {
    if ($(this).is(":checked")) {
        $(this).closest('.user-children').addClass('item-active');
    } else {
        $(this).closest('.user-children').removeClass('item-active');
    }

    let isEveryChecked = true;
    $(this).closest('.organization-with-users').find(".consensus-user-checkbox").each(function (index, element) {
        if (!$(element).is(":checked")) {
            isEveryChecked = false;
        }
    });

    $(this).closest(".organization-with-users").find(".consensus-organization-checkbox").prop("checked", isEveryChecked);


});

function openAddUserModal(e) {
    e.preventDefault();
    e.stopPropagation();
    $("#userId").val('');
    $("#addUserForm").find('input').val('');
    $("#addUserFormModal").modal('show');
}

function submitOutsideUser(e) {
    e.stopPropagation();
    e.preventDefault();
    $("#addUserForm").validate({
        ignore: []
    });
    $("#addUserForm").validate();
    if ($("#addUserForm").valid()) {

        updateOutsideUser();
        return true;
    }
    return false;
}

function updateOutsideUser() {
    let user = getUserFromModal();

    $.ajax({
        method: 'post',
        data: JSON.stringify(user),
        url: `/FormConsensus/CreateOutsideUser`,
        contentType: 'application/json',
        success: function (data) {
            $("#usersOutsideSystem").html(data);
            $("#addUserFormModal").modal('hide');
            $("#userId").val('');
            updateNumberOfSelectedUsers();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    });
}

function getUserFromModal() {
    let user = {};
    user["Id"] = $("#addUserForm").find("#userId").val();
    user["FirstName"] = $("#addUserForm").find("#firstName").val();
    user["LastName"] = $("#addUserForm").find("#lastName").val();
    user["Email"] = $("#addUserForm").find("#email").val();
    user["Institution"] = $("#addUserForm").find("#institution").val();
    user["InstitutionAddress"] = $("#addUserForm").find("#institutionAddress").val();

    let address = {};
    address["City"] = $("#addUserForm").find("#city").val();
    address["Country"] = $("#addUserForm").find("#country").val();
    address["PostalCode"] = $("#addUserForm").find("#postalCode").val();
    address["Street"] = $("#addUserForm").find("#street").val();
    address["StreetNumber"] = $("#addUserForm").find("#streetNumber").val();

    user["Address"] = address;
    user["ConsensusRef"] = $("#consensusId").val();

    return user;
}

function deleteOutsideUser(id) {
    $.ajax({
        method: 'post',
        url: `/FormConsensus/DeleteOutsideUser?userId=${id}&consensusId=${$("#consensusId").val()}`,
        contentType: 'application/json',
        success: function (data) {
            $(`#${id}`).remove();
            updateNumberOfSelectedUsers();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    });

}

function deleteInsideUser(id) {
    $.ajax({
        method: 'post',
        url: `/FormConsensus/DeleteInsideUser?userId=${id}&consensusId=${$("#consensusId").val()}`,
        contentType: 'application/json',
        success: function (data) {
            $(`#${id}`).remove();
            updateNumberOfSelectedUsers();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    });

}



function editOutsideUser(id) {
    $("#userId").val(id);
    $(`#${id}`).find('.outside-user-item').each(function (index, element) {
        $(`#${$(element).attr('data-modal-id')}`).val($(element).attr('data-modal-value'));
    });

    $("#addUserFormModal").modal('show');

}
function updateNumberOfSelectedUsers() {
    let sReportsUsersCount = $("#usersInsideSystem").find('.outside-user').length;
    let outsideUsersCount = $("#addedUsers").find(".outside-user").length;
    console.log(sReportsUsersCount + outsideUsersCount);
    $("#numOfSelectedUsers").text(sReportsUsersCount + outsideUsersCount);
}

function startConsensusFindingProcess() {
    let usersIds = [];
    $(".consensus-user-checkbox:checked").each(function (index, element) {
        usersIds.push($(element).val());
    });

    let outsideUsersIds = [];
    $("#addedUsers").find(".outside-user").each(function (index, element) {
        outsideUsersIds.push($(element).attr("id"));
    });

    let request = {};
    request["UsersIds"] = usersIds;
    request["OutsideUsersIds"] = outsideUsersIds;
    request["ConsensusId"] = $("#consensusId").val();
    request["EmailMessage"] = $("#emailMessage").val();


    $.ajax({
        method: 'post',
        data: JSON.stringify(request),
        url: `/FormConsensus/StartConsensusFindingProcess`,
        contentType: 'application/json',
        success: function (data) {
            toastr.success("Success")
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    });
}

function saveSelectedUsers() {
    let newUsers = [];
    $(".consensus-user-checkbox:checked").each(function (index, element) {
        newUsers.push($(element).val());
    });

    $.ajax({
        method: 'post',
        data: JSON.stringify(newUsers),
        url: `/FormConsensus/SaveUsers?consensusId=${$("#consensusId").val()}`,
        contentType: 'application/json',
        success: function (data) {
            $("#usersInsideSystem").html(data);
            toastr.success("Success")
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    });
}

function loadConsensusTrackerData(id, element) {
    $.ajax({
        method: 'get',
        url: `/FormConsensus/GetTrackerData?consensusId=${$("#consensusId").val()}`,
        contentType: 'application/json',
        success: function (data) {
            $("#consensusTracker").html(data);
            showConsensusQuestionaire(id, element);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    });
}

function startNewIteration(){
    $.ajax({
        method: 'get',
        url: `/FormConsensus/StartNewIteration?consensusId=${$("#consensusId").val()}`,
        contentType: 'application/json',
        success: function (data) {
            $("#consensusPartialContainer").html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
        }
    });
}

function loadConsensusPartial() {
    let formId = $("#nestable").find(`li[data-itemtype='form']`).attr('data-id');

    $.ajax({
        method: 'get',
        url: `/FormConsensus/LoadConsensusPartial?formId=${formId}`,
        contentType: 'application/json',
        success: function (data) {
            $('#consensusPartialContainer').html(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
}

function collapseIteration(header) {
    $("#iterations").find('.consensus-iteration-table').each(function (index, element) {
        if ($(element).attr("id") !== $(header).attr("data-id") && $(element).hasClass("show")) {
            let id = $(element).attr("id");
            $(`[data-id="${id}"]`).click();
        }
    });
}

function remindUser(userId, isOutsideUser, iterationId) {

    $.ajax({
        method: 'get',
        url: `/FormConsensus/RemindUser?userId=${userId}&consensusId=${$("#consensusId").val()}&isOutsideUser=${isOutsideUser}&iterationId=${iterationId}`,
        contentType: 'application/json',
        success: function (data) {
            toastr.success('Successs');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
}