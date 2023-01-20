function saveConsensusInstance(event) {
    submitConsensusInstance($(event.target).data('userid'), $(event.target).data('consensusid'), $(event.target).data('isoutsideuser'));
}

function submitConsensusInstance(userId, consensusId, isOutsideUser, autosave = false) {
    let consensusInstance = {};
    consensusInstance["ConsensusRef"] = consensusId;
    consensusInstance["UserRef"] = userId;
    consensusInstance["IsOutsideUser"] = isOutsideUser;
    consensusInstance["Questions"] = getQuestions();
    consensusInstance["Id"] = $("#consensusInstanceId").val();
    consensusInstance["IterationId"] = $("#iterationId").val();

    $.ajax({
        method: 'post',
        data: JSON.stringify(consensusInstance),
        url: `/FormConsensus/CreateConsensusInstance`,
        contentType: 'application/json',
        success: function (data) {
            if (autosave) {
                $('#consensusInstanceId').val(data.Id);
            } else {
                location.reload();
                toastr.success("Success");
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
}

function getQuestions() {
    let questions = [];
    $(".question-preview").each(function (index, element) {
        let question = {};
        question["ItemRef"] = $(element).data('id');
        question["Options"] = [];
        $(element).find(".consensus-radio").each(function (i, el) {
            question["Options"].push($(el).val());
        });
        question["Value"] = $(element).find(".consensus-radio:checked").val();
        question["Question"] = $(element).find('.qp-question').attr("data-value");
        question["Comment"] = $(`#qcomment-${$(element).data('id')}`).val();

        questions.push(question);
    });

    return questions;
}

function loadConsensusInstanceTree() {
    let formId = $("#formId").val();
    let consensusInstanceId = $("#consensusInstanceId").val();

    $.ajax({
        method: 'get',
        url: `/FormConsensus/ReloadConsensusInstanceTree?formId=${formId}&consensusInstanceId=${consensusInstanceId}&iterationId=${$("#iterationId").val()}&questionnaireViewType=${$("#questionnaireViewType").val()}`,
        contentType: 'application/json',
        success: function (data) {
            $('#consensusTree').html(data);
            $('.consensus-visible').show();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function showQuestionnaireSaveModal() {
    $("#saveModal").modal('show');
}

function questionnaireSaveModalDecision(event, decision) {
    event.preventDefault();
    $("#saveModal").modal('hide');

    if (decision == 'yes') {
        submitConsensusInstance($('#questionnaireSaveButton').data('userid'), $('#questionnaireSaveButton').data('consensusid'), $('#questionnaireSaveButton').data('isoutsideuser'), true);
    }

    $(".consensus-tab").removeClass('active-item');
    $("#consensusFormPreviewTab").addClass('active-item');
    showConsensusFormPreview();
}