$(document).on('keyup', '#clinicalTrial', function (e) {
    if (e.which !== downArrow && e.which !== upArrow && e.which !== enter) {
        let searchValue = $(this).val();
        if (searchValue.length > 2) {
            $.ajax({
                method: 'get',
                url: `/SmartOncology/ReloadClinicalTrial?name=${searchValue}`,
                contentType: 'application/json',
                success: function (data) {
                    $('#clinicalTrialOptions').html(data);
                    $('#clinicalTrialOptions').show();
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr, thrownError);
                }
            });
        }
    }
});

$(document).on('keydown', '#search', function (e) {
    let next;
    if (e.which === downArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).next();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').eq(0).addClass('selected');
            }
        } else {
            liSelected = $('.option').eq(0).addClass('selected');
        }
    } else if (e.which === upArrow) {
        if (liSelected) {
            $(liSelected).removeClass('selected');
            next = $(liSelected).prev();
            if (next.length > 0) {
                liSelected = $(next).addClass('selected');
            } else {
                liSelected = $('.option').last().addClass('selected');
            }
        } else {
            liSelected = $('.option').last().addClass('selected');
        }
    }
    else if (e.which === enter) {
        $(liSelected).click();
    }

    e.stopImmediatePropagation();
});

$(document).on("click", '.sidebar-shrink', function (e) {
    let target = e.target;
    let isClicalDomainInput = $(target).attr('id') === 'clinicalTrial';
    if (!$(target).hasClass('option') && !isClicalDomainInput) {
        $("#clinicalTrialOptions").hide();
    }
});

function optionClicked(e, id, name) {
    let isAppended = appendTagToContainer(id, name, 'clinicalTrials');
    if (isAppended) {
        $("#clinicalTrial").val('');
    }
    $('#clinicalTrialOptions').hide();
}

var enter = 13;
var downArrow = 40;
var upArrow = 38;

var li = $('.option');
var liSelected = null;
