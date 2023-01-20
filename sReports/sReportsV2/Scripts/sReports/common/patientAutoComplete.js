
$(document).on('keyup', '#patient', function (e) {
    if (e.which !== 40 && e.which !== 38 && e.which !== 13) {
        let searchValue = $("#patient").val();
        if (searchValue.length > 2) {
            $.ajax({
                method: 'get',
                url: `/Patient/GetByName?searchValue=${searchValue}`,
                contentType: 'application/json',
                success: function (data) {
                    $('#patientOptions').html(data);
                    $('#patientOptions').show();
                },
                error: function (xhr, textStatus, thrownError) {
                    handleResponseError(xhr.responseText);
                }
            });
        }
    }

});

$(document).on('keydown', '#patient-search', function (e) {
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
        $("#patientOptions").hide();
    }
});

$(document).on("click", '.patient-remove', function (e) {
    $(this).closest(".selected-patient").remove();
});

$(document).on("click", '.sidebar-shrink', function (e) {
    let target = e.target;
    let isPatientInput = $(target).attr('id') === 'patient';
    if (!$(target).hasClass('option') && !isPatientInput) {
        $("#patientOptions").hide();
    }
});

function patientOptionClicked(e, value, translation) {
    console.log(value);
    let exist = false;
    $("#selected-patients").find('div').each(function (index, element) {
        if ($(element).attr("data-value") == value) {
            exist = true;
        }
    });

    addNewPatient(exist, value, decodeLocalizedString(translation));
    $("#patient").val('');
}

function addNewPatient(exist, value, translation) {
    if (!exist) {
        let item = document.createElement('div');
        $(item).attr("data-value", value);
        $(item).text(translation);
        $(item).addClass('selected-patient');
        let i = document.createElement('i');
        $(i).addClass('fas fa-times patient-remove');
        $(item).append(i);
        $("#selected-patients").append(item);
    }
    $('#patientOptions').hide();
}

function setPatient() {
    let result = [];
    $("#selected-patients").find('.selected-patient').each(function (index, element) {
        result.push($(element).attr("data-value"));
    });

    return result;
}

$(document).on('keyup', '#patientSearch', function (e) {
    if (e.which !== 40 && e.which !== 38 && e.which !== 13) {
        page = 1;
        reloadPatients(false);
    }
});

function loadMorePatients() {
    page++;
    reloadPatients(true);
}

function reloadPatients(loadMore) {
    let requestObject = {};
    requestObject.SearchValue = $('#patientSearch').val().toLowerCase();
    requestObject.Page = page;
    requestObject.PageSize = 20;

    if (requestObject.SearchValue.length > 2) {
        $.ajax({
            method: 'get',
            url: `/Patient/ReloadPatients`,
            data: requestObject,
            contentType: 'application/json',
            success: function (data) {
                if (loadMore) {
                    $('#patientOptions').append(data);
                    document.getElementById("patientSearch").focus();
                }
                else
                    $('#patientOptions').html(data);
                $('#loadPatients').remove();
                $('#patientOptions').show();
                if (data.trim()) {
                    if ($('#patientOptions').find(".option").length >= requestObject.PageSize * page) {
                        $('#patientOptions').append(appendLoadMore());
                    }
                }
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr.responseText);
            }
        });
    }
}

function appendLoadMore() {
    let divElement = document.createElement('div');
    $(divElement).addClass("load-more-button-container");
    $(divElement).addClass("load-more-umls");
    divElement.id = "loadPatients";
    let loadMoreElement = document.createElement('div');
    $(loadMoreElement).addClass("load-more-button");
    $(loadMoreElement).addClass("load-concepts");
    loadMoreElement.onclick = function () { loadMorePatients() };
    var LoadMoreText = loadMore;
    var element = $(loadMoreElement).append(LoadMoreText)
    $(divElement).append(element);

    return divElement;
}

var li = $('.option');
var liSelected = null;
var counter = 0;
var page = 1;