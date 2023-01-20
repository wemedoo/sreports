
$(document).on('keyup', '#user', function (e) {
    if (e.which !== 40 && e.which !== 38 && e.which !== 13) {
        let searchValue = $("#user").val();
        if (searchValue.length > 2) {
            $.ajax({
                method: 'get',
                url: `/UserAdministration/GetByName?searchValue=${searchValue}`, 
                contentType: 'application/json',
                success: function (data) {
                    $('#userOptions').html(data);
                    $('#userOptions').show();
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr, thrownError);
                }
            });
        }
    }

});

$(document).on('keydown', '#user-search', function (e) {
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

$(document).on("click", '.main-content', function (e) {  // TODO : Discuss if .main-content is appropriate
    if (!$(e.currentTarget).hasClass('dropdown-search') || $(e.currentTarget).closest('dropdown-search').length == 0) {
        $("#userOptions").hide();
    }
});

$(document).on("click", '.user-remove', function (e) {
    $(this).closest(".selected-user").remove();
});

$(document).on("click", '.sidebar-shrink', function (e) {
    let target = e.target;
    let isUserInput = $(target).attr('id') === 'user';
    if (!$(target).hasClass('option') && !isUserInput) {
        $("#userOptions").hide();
    }
});

function userOptionClicked(e, value, translation) {
    console.log(value);
    let exist = false;
    $("#selected-users").find('div').each(function (index, element) {
        if ($(element).attr("data-value") == value) {
            exist = true;
        }
    });

    addNewUser(exist, value, decodeLocalizedString(translation));
    $("#user").val('');
}

function addNewUser(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('selected-user'); 
        let i = document.createElement('i');
        $(i).addClass('fas fa-times user-remove'); 
        $(item).append(i);
        $("#selected-users").append(item); 
    }
    $('#userOptions').hide();
}


function setUser() {
    let result = [];
    $("#selected-users").find('.selected-user').each(function (index, element) {
        result.push($(element).attr("data-value"));
    });

    return result;
}

var li = $('.option');
var liSelected = null;
var counter = 0;
