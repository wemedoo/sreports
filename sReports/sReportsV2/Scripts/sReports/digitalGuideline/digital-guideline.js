var toJson = function (res) {
    return res.json();
};

$(document).on('click', '.update-guideline', function (e) {
    let guidelineData = editorCode.get();
    if (!cy) {
        initializeGraph(guidelineData);
    } else {
        cy.json({ elements: guidelineData.guidelineElements })
    }
});

function submitGuidline (e) {
    let jsonData = editorCode.get();
    console.log(jsonData);
    $.ajax({
        method: 'post',
        data: JSON.stringify(jsonData),
        url: '/DigitalGuideline/Create',
        contentType: 'application/json',
        success: function (data) {
            toastr.success('Success');
            updateGuidelineWithLastUpdate(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function previewNode(data) {
    console.log(data);
    $.ajax({
        method: 'post',
        url: '/DigitalGuideline/PreviewNode',
        data: data,
        success: function (data) {
            $('#nodePreview').html(data);
            $('#showNodePreviewButton').click();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    })
    //$('#nodePreview').html(JSON.stringify(data));
}

function showJsonData(event) {
    if (event) {
        event.preventDefault();
    }
    $(event.target).siblings().removeClass('active');
    $(event.target).addClass('active');
    $('#jsoneditorCode').show();
    $('#nodePreview').hide();
}


function showNodePreview(event) {
    event.preventDefault();
    if ($('#nodePreview').hasClass('active')) {
        return;
    }

    $(event.target).siblings().removeClass('active');
    $(event.target).addClass('active');
    $('#jsoneditorCode').hide();
    $('#nodePreview').show();
}

$(document).on('click', '.section-tab', function (e) {
    console.log('clicked');
    if ($(this).hasClass('active')) {
        return;
    }

    $(this).siblings().removeClass('active');
    $(this).addClass('active');
    let target = $(this).attr('data-target');
    $(`#${target}`).siblings().removeClass('active');
    $(`#${target}`).addClass('active');
})

$(document).on('click', '.publication-show-full-details', function (e) {
    $(this).siblings('.publication-full-details').toggle();

    $(this).toggleClass('active');
})

function updateGuidelineWithLastUpdate(data) {
    $('#lastUpdate').val(data.LastUpdate);
    let jsonData = editorCode.get();
    jsonData['lastUpdate'] = data.LastUpdate;
    editorCode.set(jsonData);
}