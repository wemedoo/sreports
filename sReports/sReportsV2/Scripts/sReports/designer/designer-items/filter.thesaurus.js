$(document).on('click', '#applySearchButton', function (e) {
    currentPage = 1;
    reloadTable();
});


$(document).on('click', '.thesaurus-button-dropdown', function (e) {
    e.stopPropagation();
    $(this).toggleClass('active');
    $(this).siblings('.active-value').toggleClass('active');
    $('.thesaurus-full-info-container').slideToggle();
})

$(document).on('click', '.active-value', function (e) {
    e.stopPropagation();
    $(this).toggleClass('active');
    $(this).siblings('.thesaurus-button-dropdown').toggleClass('active');
    $('.thesaurus-full-info-container').slideToggle();
});

$(document).on('keypress', function (e) {
    if (e.which === 13) {
        e.stopPropagation();
        currentPage = 1;
        reloadTable();
    }
});





$(document).on('click', '.search-thesaurus-table .select-button:not(".selected")', function (e) {
    let td = $(this).parent();
    let o4mtid = $(td).data('o4mtid');
    loadThesaurusData(o4mtid);
    
    let selected = $('.select-button.selected').not('.hide').first();
    $(selected).addClass('hide');
    $(selected).siblings('.select-button').removeClass('hide');

    $(this).addClass('hide');
    $(this).siblings('.select-button').removeClass('hide');
    $("#activeThesaurus").attr('data-value', o4mtid);
    setThesaurusDetailsContainer(o4mtid);
})

$(document).on('click', '.thesaurus-preview-container .select-button:not(".selected")', function (e) {

    //think about that to work from preview

    let td = $(this).parent();
    let o4mtid = $(td).data('o4mtid');
    loadThesaurusData(o4mtid);

    //let selected = $('.select-button.selected').not('.hide').first();
    //$(selected).addClass('hide');
    //$(selected).siblings('.select-button').removeClass('hide');

    $(this).addClass('hide');
    $(this).siblings('.select-button').removeClass('hide');
    $("#activeThesaurus").attr('data-value', o4mtid);
    setThesaurusDetailsContainer(o4mtid);
})

function setThesaurusDetailsContainer(o4mtid) {
    $('#thesaurusId').val(o4mtid);
    $('#thesaurusId').removeClass('hide').removeClass('error');
    $('#thesaurusId').siblings('label.error').remove();
    if ($('.current-thesaurus-tag.selected').hasClass('hide')) {
        $('.current-thesaurus-tag.selected').toggleClass('hide');
        $('.current-thesaurus-tag.no-selection').toggleClass('hide');

        $('#thesaurusLabelPlaceholder').addClass('hide');
    }

    $('#thesaurusWarning').addClass('d-none');
}

function reloadTable() {
    
    let inputVal = $('#thesaurusSearchInput').val();
    let activeThesaurus = $('#thesaurusId').val();

    if (inputVal) {
        let requestObject = {};
        requestObject.Page = currentPage;
        requestObject.PageSize = getPageSize();
        requestObject.PreferredTerm = inputVal;
        $.ajax({
            method: 'get',
            url: `/ThesaurusEntry/ReloadSearchTable?preferredTerm=${inputVal}&activeThesaurus=${activeThesaurus}`,
            data: requestObject,
            success: function (data) {
                $('#tableContainer').html(data);
                $('#tableContainer').removeClass('w-50');
                $('#reviewContainer').hide();
            },
            error: function () {

            }
        });
    }

}

function loadThesaurusData(thesaurusId) {
    if (thesaurusId) {
        $.ajax({
            method: 'get',
            url: `/ThesaurusEntry/ThesaurusProperties?o4mtid=${thesaurusId}`,
            success: function (data) {
                $('#activeThesaurusInfo').html(data);
            },
            error: function () {

            }
        });
    }
}

$(document).on('blur', ".item-title", function (e) {
    let term = $("#designerFormModalMainContent").find('.item-title').val();
    $("#designerFormModalMainContent").find('#thesaurusSearchInput').val(term);
    $('#applySearchButton').click();
    $("#designerFormModalMainContent").find('.thesaurus-full-info-container').show();
});

function showThesaurusReview(o4mtId, event) {
    $('.thesaurus-review').each(function (index, element) {
        $(element).removeClass('active');
        $(element).addClass('ml-auto');
    })

    $('.single-table').find('tr').each(function (index, element) {
        $(element).removeClass('tr-active');
        $(element).find('.select-button').each(function (i, ele) {
            $(ele).addClass('hide');
        })
    });

    $('#tableContainer').addClass('w-50');


    $(event.currentTarget).addClass('active');
    $(event.currentTarget).closest('tr').addClass('tr-active');

    loadThesaurusPreview(o4mtId);
}

function closeThesaurusPreview() {
    currentPage = 1;
    reloadTable();
}

function loadThesaurusPreview(thesaurusId) {
    if (thesaurusId) {
        $.ajax({
            method: 'get',
            url: `/ThesaurusEntry/ThesaurusPreview?o4mtid=${thesaurusId}&activeThesaurus=${$('#activeThesaurus').attr('data-value')}`,
            success: function (data) {
                $('#reviewContainer').html(data);
                $('#reviewContainer').show();

            },
            error: function () {

            }
        });
    }
}


