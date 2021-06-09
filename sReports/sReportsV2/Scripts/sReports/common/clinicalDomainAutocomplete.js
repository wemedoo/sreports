$(document).on('keyup', '#clinicalDomain', function (e) {
    if (e.which !== 40 && e.which !== 38 && e.which !== 13) {
        let searchValue = $("#clinicalDomain").val();
        if (searchValue.length > 2) {
            $.ajax({
                method: 'get',
                url: `/Form/ReloadClinicalDomain?term=${searchValue}`,
                contentType: 'application/json',
                success: function (data) {
                    $('#clinicalOptions').html(data);
                    $('#clinicalOptions').show();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    toastr.error(jqXHR.responseText);
                }
            });
        }
    }

});


function optionClicked(e, value, translation) {
    console.log(value);
    let exist = false;
    $("#clinicals").find('div').each(function (index, element) {
        if ($(element).attr("data-value") == value) {
            exist = true;
        }
    });

    addNewClinicalDomain(exist, value, translation);
    $("#clinicalDomain").val('');
}

function addNewClinicalDomain(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('clinical');
        let i = document.createElement('i');
        $(i).addClass('fas fa-times clinical-remove');
        $(item).append(i);
        $("#clinicals").append(item);
        $('#clinicalOptions').hide();
    }
}
$(document).on("click", '.designer-form-modal-body', function (e) {
    if (!$(e.currentTarget).hasClass('dropdown-search') || $(e.currentTarget).closest('dropdown-search').length == 0) {
        $("#clinicalOptions").hide();
    }
});

$(document).on("click", '.clinical-remove', function (e) {
    $(this).closest(".clinical").remove();
});

function setClinicalDomain() {
    let result = [];
    $("#clinicals").find('.clinical').each(function (index, element) {
        result.push($(element).attr("data-value"));
    });

    return result;
}

var li = $('.option');
var liSelected = null;
var counter = 0;
$(document).on('keydown', '#search', function (e) {
    console.log(++counter);
    let next;
    if (e.which === 40) {
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
    } else if (e.which === 38) {
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
    else if (e.which === 13) {
        $(liSelected).click();
    }

    e.stopImmediatePropagation();
});
