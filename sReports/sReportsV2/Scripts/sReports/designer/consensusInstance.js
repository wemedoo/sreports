function submitConsensusInstance(userId, consensusId, isOutsideUser) {
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
            location.reload();
            toastr.success("Success");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.statusText);
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
        question["Comment"] = $(`#qcomment-${$(element).attr('id').split('-')[1]}`).val();

        questions.push(question);
    });

    return questions;
}

function loadConsensusInstanceTree() {
    let formId = $("#formId").val();
    let consensusInstanceId = $("#consensusInstanceId").val();

    $.ajax({
        method: 'get',
        url: `/FormConsensus/ReloadConsensusInstanceTree?formId=${formId}&consensusInstanceId=${consensusInstanceId}&iterationId=${$("#iterationId").val()}`,
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