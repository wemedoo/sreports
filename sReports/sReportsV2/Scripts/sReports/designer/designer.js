var configByType = {
    form: {
        excludeProperties: ['itemtype', 'activeversion'],
        formUrl: '/Form/GetFormGeneralInfoForm',
        title: 'Form information',
        handleClass: '.dd-nohandle'
    },
    chapter: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetChapterInfoForm',
        title: 'Chapter information',
        handleClass: '.dd-handle'
    },
    page: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetPageInfoForm',
        title: 'Page general info',
        handleClass: '.dd-handle'
    },
    fieldset: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetFieldSetInfoForm',
        title: 'Fieldset information',
        handleClass: '.dd-handle'
    },
    field: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetFieldInfoForm',
        title: 'Field information',
        handleClass: '.dd-handle'
    },
    fieldvalue: {
        excludeProperties: ['itemtype'],
        formUrl: '/Form/GetFieldValueInfoForm',
        title: 'Option Information',
        handleClass: '.dd-handle'
    }
};

var selectableFields = ['checkbox', 'select', 'radio'];
var stringFields = ['text', 'date', 'datetime', 'calculative', 'number', 'regex', 'long-text', 'file', 'email'];
var dependableFields = [];
var start, end, clicked;

$(document).on('mousedown', '.dd-item', function (e) {
    if (!$(e.target).hasClass('add-item-button')) {
        e.stopPropagation();
        start = +new Date();
        clicked = e.currentTarget;
    } else {
        return;
    }

});

$(document).on('mouseup', '.dd-item', function (e) {
    end = +new Date();
    var diff = end - start;
    if (diff <= 280) {
        if (clicked) {
            clickedEvent($(clicked).attr('data-id'));
        }
    }
    clicked = undefined;
});

function clickedEvent(id) {
    console.log(id);
}

$(document).on('click', '.remove-button', function (e) {
    e.stopPropagation();
    let element = $(e.currentTarget).closest('.dd-item');
    let targetId = $(element).attr('data-id');
    $('.remove-modal-button').attr('data-target', `${targetId}`);
    showDeleteModal(e, "", "deleteFormItem");
});

function deleteFormItem(e) {
    let status = $(e.currentTarget).attr('data-remove-status');
    if (status === 'confirm') {
        let itemToRemove = $(e.currentTarget).attr('data-target');
        $(`[data-id='${itemToRemove}']`).remove();
    }
    //updateNestableData("nestable");

    $('#deleteModal').modal('hide');
    let nestable = $('#nestableFormPartial').data('nestable');
    nestable.managePlaceholderElements('nestableFormPartial');
    submitFullFormDefinition();
}

$(document).on('click', '.remove-modal-button', function (e) {
    let status = $(e.currentTarget).attr('data-remove-status');
    if (status === 'confirm') {
        let itemToRemove = $(e.currentTarget).attr('data-target');
        $(`[data-id=${itemToRemove}]`).remove();
    }

    $('#confirmRemovalModal').modal('hide');
});

$(document).on('click', '.edit-button', function (e) {
    e.stopPropagation();
    e.preventDefault();
    let element = $(e.currentTarget).closest('li.dd-item');
    let type = $(element).attr('data-itemtype');
    showForm(configByType[type], element, null);
});

var parentId;
$(document).on('click', '.add-item-button', function (e) {
    let type = $(e.currentTarget).attr('data-itemtype');
    parentId = $(this).attr('data-parentid');

    showForm(configByType[type], null);
    $('.add-item-button').removeClass('active');
    $(this).addClass('active');
});

function showForm(config, element) {
    $.ajax({
        method: 'post',
        data: JSON.stringify(generateObjectFromDataProperties(element, config.excludeProperties)),
        url: config.formUrl,
        contentType: 'application/json',
        success: function (data) {
            $('#designerFormModalMainContent').html(data);
            $('body').addClass('no-scrollable');
            $('.designer-form-title-text').html(config.title);
            $('.designer-form-modal').addClass('show');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}


$(document).on('click', '#submit-full-form-definition', function (e) {
    e.preventDefault();
    e.stopPropagation();

    submitFullFormDefinition(true);
});

function submitFullFormDefinition(fullFormSubmit = false, reloadPartial = false) {
    setDependableFields();

    let formDefinition;
    if (editorTree) {
        formDefinition = editorTree.get();
    } else {
        formDefinition = getNestableFullFormDefinition($("#nestable").find(`li[data-itemtype='form']`).first());
    }
    let formDefinitionValidationSummary = validateFormDefinition(formDefinition);
    if (formDefinitionValidationSummary) {
        toastr.error(formDefinitionValidationSummary);
        return;
    }

    let createForm = isNewFormCreated(formDefinition);

    $.ajax({
        method: 'post',
        data: JSON.stringify(formDefinition),
        url: '/Form/Create',
        contentType: 'application/json',
        success: function (data) {
            updateFormWithLastUpdate(data);
            if (fullFormSubmit || createForm || isNewFormGenerated(data, createForm)) {
                window.location.href = `/Form/Edit?thesaurusId=${data.thesaurusId}&versionId=${data.versionId}`;
                toastr.success('Success');
            } else {
                updateAdministrativeData(data.id);
                reloadFormPartialIfNecessary(reloadPartial, data.lastUpdate, formDefinition);
            }
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function updateAdministrativeData(id) {
    $.ajax({
        method: 'GET',
        url: `/Form/GetFormAdministrativeData?formId=${id}`,
        contentType: 'application/json',
        success: function (data) {
            console.log('success');
            $('#idWorkflow').html(data);
            showAdministrativeArrowIfOverflow('administrative-container-form');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function updateFormWithLastUpdate(data) {
    $('li[data-itemtype=form]').attr('data-lastupdate', data.lastUpdate);
}

function isNewFormGenerated(data, createForm) {
    let x = $('li[data-itemtype=form]').attr('data-id');

    return $('li[data-itemtype=form]').attr('data-id') != data.id && !createForm;
}

function isNewFormCreated(formDefinition) {
    return !formDefinition.id;
}

function appendParameters(parameters) {
    var url = window.location.href;
    if (!url.indexOf('?') > -1) {
    } else {
        url += `?${parameters}`
    }
    window.location.href = url;
}


function updateNestableData(id) {
    let formDefinition = getNestableFullFormDefinition($(`#${id}`).find(`li[data-itemtype='form']`).first());
    if (id === "nestable") {
        getFormPartial(formDefinition);
        submitFullFormDefinition();
    }
    else {
        getNestableTree(formDefinition);
    }
}

function getFormPartial(formDefinition) {
    $.ajax({
        method: 'post',
        data: JSON.stringify(formDefinition),
        url: '/Form/CreateDragAndDropFormPartial',
        contentType: 'application/json',
        success: function (data) {
            $('#formPreviewContainer').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getNestableTree(formDefinition) {
    $.ajax({
        method: 'post',
        data: JSON.stringify(formDefinition),
        url: '/Form/CreateFormTreeNestable',
        contentType: 'application/json',
        success: function (data) {
            $('#formTreeNestable').html(data);
            submitFullFormDefinition();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function setDependableFields() {
    dependableFields = [];
    $(`li[data-itemtype='field']`).each(function (index, element) {
        let dependablesList = decodeToJsonOrText($(element).attr('data-dependables'));
        if (dependablesList) {
            dependablesList.forEach(x => {
                if (!dependableFields.includes(x.actionParams)) {
                    dependableFields.push(x.actionParams);
                }
            })
        }
    })
}


function getChapterWithData(chapterElement) {
    let chapter = generateObjectFromDataProperties(chapterElement, configByType['chapter'].excludeProperties);
    chapter.Pages = [];

    $(chapterElement).find(`li[data-itemtype='page']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, pageElement) {
        if (!$(pageElement).attr('data-ignore')) {
            chapter.Pages.push(getPageWithData(pageElement));
        }
    })

    return chapter;
}

function getPageWithData(pageElement) {
    let page = generateObjectFromDataProperties(pageElement, configByType['page'].excludeProperties);
    page.ListOfFieldSets = [];
    $(pageElement).find(`li[data-itemtype='fieldset']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, fieldsetElement) {
        if (!$(fieldsetElement).attr('data-ignore')) {
            page.ListOfFieldSets.push(getFieldSetWithData(fieldsetElement));
        }
    });

    return page;
}

function getFieldSetWithData(fieldsetElement) {
    let fieldset = generateObjectFromDataProperties(fieldsetElement, configByType['fieldset'].excludeProperties);
    fieldset.Fields = [];
    $(fieldsetElement).find(`li[data-itemtype='field']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, fieldElement) {
        if (!$(fieldElement).attr('data-ignore')) {
            fieldset.Fields.push(getFieldWithData(fieldElement));
        }
    })
    return [fieldset];
}

function getFieldWithData(fieldElement) {
    let field = generateObjectFromDataProperties(fieldElement, configByType['field'].excludeProperties);
    loadFieldValueIfSelectable(field, fieldElement);
    hideFieldIfDependable(field);
    return field;
}

function loadFieldValueIfSelectable(field, fieldElement) {
    if (selectableFields.includes($(fieldElement).attr('data-type'))) {
        field.Values = [];
        $(fieldElement)
            .find(`[data-itemtype='fieldvalue']`)
            .not('.add-item-button')
            .not('.form-select-placeholder')
            .not('.dd-item-placeholder')
            .each(function (index, fieldValueElement) {
                if (!$(fieldValueElement).attr('data-ignore')) {
                    let fieldValue = generateObjectFromDataProperties(fieldValueElement, configByType['fieldvalue'].excludeProperties);
                    field.Values.push(fieldValue);
                }

            })
    }
}

function hideFieldIfDependable(field) {
    if (dependableFields.includes(field.id)) {
        field.isvisible = false;
    }
}

function generateObjectFromDataProperties(element, excludeProperties) {
    let result = {};
    if (element) {
        let dataProps = element.length ? element[0].dataset : element.dataset;
        Object.keys(dataProps).filter(x => !excludeProperties.includes(x)).forEach(x => {
            result[x] = decodeToJsonOrText(dataProps[x]);
        });
    }

    return result;
}

function getFieldInfo(element) {
    let fieldData = null;

    return fieldData;
}

/*COMMON CODE*/
function updateTreeItemTitle(element, title) {
    let type = $(element).attr('data-itemtype');
    let titleContainer = $(element).children(configByType[type].handleClass).first();
    $(titleContainer).html(title);
    $(titleContainer).attr('title', title);
}

function getDataProperty(element, dataName) {
    let data = $(element).attr(`data-${dataName}`);
    let decodedData = data ? decodeURIComponent(data.replace(/\+/g, ' ')) : null;
    return JSON.parse(decodedData) || {};
}

function getElement(type, title) {
    console.log('getting element');
    let result;
    let elementId = $(`#elementId`).val();

    if ($("#nestable").find(`li[data-id='${elementId}']`).length > 0) {
        result = $("#nestable").find(`li[data-id=${elementId}]`)[0];
    } else {
        result = createNewDragAndDropElement(type, elementId, title);
    }

    return result;
}

function createNewDragAndDropElement(type, elementId, title) {
    let newElement = document.createElement('li');
    $(newElement).attr('data-id', elementId);
    $(newElement).addClass('dd-item');
    $(newElement).attr('data-itemtype', type);

    let handle = document.createElement('div');
    $(handle).addClass('dd-handle');
    $(handle).html(title);
    $(newElement).append(handle);

    $(newElement).append(createDragAndDropRemoveButton());
    $(newElement).append(createDragAndDropEditButton());
    if (type != 'fieldvalue' && type != 'field') {
        $(newElement).append(createDragAndDropOrderedlist(type, elementId));
    }


    let olParent = $("#nestable").find(`.dd-item[data-id='${parentId}']`).find('ol').first();
    $(olParent).find('li:last').before(newElement);
    $('#nestable').nestable('reinit');

    return newElement;
}

function createDragAndDropEditButton() {
    let editButton = document.createElement('div');
    $(editButton).addClass('edit-button');
    let icon = document.createElement('img');
    $(icon).attr('src', '/Content/img/icons/edit_pencil_03.svg');
    $(editButton).append(icon);

    return editButton;
}

function createDragAndDropRemoveButton() {
    let removeButton = document.createElement('div');
    $(removeButton).addClass('remove-button');
    let iconRemove = document.createElement('img');
    $(iconRemove).attr('src', '/Content/img/icons/remove_simulator.svg');
    $(removeButton).append(iconRemove);

    return removeButton;
}

function createDragAndDropOrderedlist(type, elementId) {
    let orderedList = document.createElement('ol');
    $(orderedList).addClass('dd-list');
    if (type !== 'fieldvalue') {
        $(orderedList).append(createDragAndDropInsertItemButton(type, elementId));
    }

    return orderedList;
}

function createDragAndDropInsertItemButton(type, elementId) {
    let child = getDragAndDropChildNameForParent(type);
    let li = document.createElement('li');
    $(li).addClass('add-item-button add-page-button dd-nodrag');
    $(li).attr('data-itemtype', child);
    $(li).attr('data-parentid', elementId);
    let div = document.createElement('div');
    let img = document.createElement('img');
    $(img).attr('src', '/Content/img/icons/add_new.svg');
    $(div).append(img).append(` Add new ${getPlaceholderText(child)}`);
    $(li).append(div);
    return li;
}

function getDragAndDropChildNameForParent(parentType) {
    switch (parentType) {
        case 'form':
            return 'chapter';
        case 'chapter':
            return 'page';
        case 'page':
            return 'fieldset';
        case 'fieldset':
            return 'field';
        case 'field':
            return 'fieldvalue';
    }
}

function getPlaceholderText(elementName) {
    return elementName === 'fieldvalue' ? 'option' : elementName;
}
 
function decodeToJsonOrText(value) {
    let decoded = decode(value);
    let result = '';
    try {
        result = JSON.parse(decoded);
    }
    catch{
        result = decoded;
    }

    return result;
}

function decode(value) {
    let val = '';
    if (value || value == 0) {
        val = `${value}`;
    }
    return decodeURIComponent(val.replace(/\+/g, ' '));
}



$(document).on('click', '#thesaurusShowModal', function (e) {
    let thesaurusId = $('#thesaurusId').val();
    if (thesaurusId) {
        $.ajax({
            method: 'get',
            url: `/ThesaurusEntry/ThesaurusProperties?o4mtid=${thesaurusId}`,
            success: function (data) {
                $('.active-thesaurus').html(data);
                $('#filterThesaurusModal').modal('show');

            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    } else {
        $('#filterThesaurusModal').modal('show');
    }

});

function reloadFormPreviewPartial() {
    submitFullFormDefinition(false, true);
    getNestableFormElements();
}

function reloadFormPartialIfNecessary(reloadPartial, lastUpdate, formDefinition) {
    if (reloadPartial) {
        formDefinition['lastupdate'] = decodeToJsonOrText(lastUpdate);
        getFormPartial(formDefinition);
    }
}

function getNestableFormElements() {
    $.ajax({
        method: 'post',
        url: '/Form/GetPredefinedFormElements',
        contentType: 'application/json',
        success: function (data) {
            $('#nestableFormElements').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

$(document).ready(function () {
    getNestableFormElements();
    setCanvasSize();
});

function setCanvasSize() {
    $('#designerCanvas').attr("width", $('.designer-main-content:first').width());
    $('#designerCanvas').attr("height", $('.designer-main-content:first').height());
}

$('.dropdown-select').on('click', '.menu-state li a', function () {
    var target = $(this);
    var targetValue = $(target).attr('data-value');
    $('li[data-itemtype=form]').attr('data-state', decodeURIComponent(targetValue));
   
    //Adds active class to selected item
    var previousSelected = $(target).parents('.dropdown-menu').find('.state-option.active');
    $(previousSelected).removeClass('active');
    $(target).addClass('active');

    let data = {
        State: targetValue,
        Id: decodeToJsonOrText($('li[data-itemtype=form]').attr('data-id')),
        LastUpdate: decodeToJsonOrText($('li[data-itemtype=form]').attr('data-lastupdate'))
    }
    console.log(data);
    $.ajax({
        method: 'PUT',
        data: data,
        url: `/Form/UpdateFormState`,
        success: function (data) {
            $('li[data-itemtype=form]').attr('data-lastupdate', encodeURIComponent(data.LastUpdate));
            updateAdministrativeData(data.Id);
            toastr.success('You have successfully changed the state');
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
            $(previousSelected).addClass('active');
            $(target).removeClass('active');
        }
        
    })
});

function getNestableFullFormDefinition(formElement) {
    let formDefinition = generateObjectFromDataProperties(formElement, configByType['form'].excludeProperties);
    formDefinition.id = formDefinition.id == 'formIdPlaceHolder' ? '' : formDefinition.id;
    formDefinition['Chapters'] = [];
    $(formElement).find(`li[data-itemtype='chapter']:not(.add-item-button):not(.dd-item-placeholder)`).each(function (index, chapterElement) {
        if (!$(chapterElement).attr('data-ignore')) {
            formDefinition.Chapters.push(getChapterWithData(chapterElement));
        }
    });

    return formDefinition;
}
/*END COMMON CODE*/

/*form modal*/
$(document).on('click', '.cancel-modal-action', function (e) {
    closDesignerFormModal();
});

$(document).on('click', '.designer-form-modal', function (e) {
    //Nikola has request to not close modal on clicking outside of modal
    //closDesignerFormModal();
});

$(document).on('click', '.designer-form-modal-body', function (e) {
    e.stopPropagation();
});

$(document).on('click', '.close-designer-form-modal-button', function (e) {
    closDesignerFormModal();
});

function closDesignerFormModal(reloadFormPreview) {
    $('.designer-form-modal').removeClass('show');
    $('body').removeClass('no-scrollable');

    if (reloadFormPreview) {
        reloadFormPreviewPartial();
    }
}

function showFormJson() {
    setDependableFields();
    let formDefinition = getNestableFullFormDefinition($("#nestable").find(`li[data-itemtype='form']`).first());
    $.ajax({
        method: 'post',
        data: JSON.stringify(formDefinition),
        url: `/Form/GetFormJson`,
        contentType: 'application/json',
        success: function (data) {
            $('#formTreeContainer').html(data);
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}
/*End form modal*/

$(document).on('click', '.drag-icon-container', function (e) {
    if ($("#nestableFormElements").css('display') === "none") {
        $("#nestableFormElements").show();
        $(this).addClass('expanded');

        removeCommentSection();
    } else {
        $("#nestableFormElements").hide();
        $(this).removeClass('expanded');
    }

    showNestableContainer();

});

$(document).on('click', "#closeDragAndDrop", function (e) {
    $("#nestableFormElements").hide();
});

$(document).on('hover', '.icon-container', function (e) {
    if ($('.dd-dragel').length > 0) {
        console.log('stopping propagation');
        e.stopPropagation();
        e.preventDefault();
    }
});

function createNewThesaurusIfNotSelected() {
    let thesaurusId = $("#designerFormModalMainContent").find('#thesaurusId').val();
    let preferredTerm = $("#designerFormModalMainContent").find('.item-title').val();
    let description = $("#designerFormModalMainContent").find('#description').val();

    if ((!thesaurusId || thesaurusId === "0" ) && preferredTerm) {

        $.ajax({
            method: 'post',
            url: `/ThesaurusEntry/CreateByPreferredTerm?preferredTerm=${preferredTerm}&description=${description}`,
            success: function (data) {
                setThesaurusDetailsContainer(data.Id);
                loadThesaurusData(data.Id);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            },
            async: false
        });
    }
}

$(document).on('click', ".comment-button", function (e) {

    e.stopPropagation();
    e.preventDefault();
    setCanvasSize();

    var $this = $(this);
    var formId = $this.attr('data-value');
    var dataToSend = { formId: formId };

    if (!$this.hasClass('active')) {
        $this.addClass('active');
        reloadComments(dataToSend)
        showCommentSection();  
        hideNestableContainer();
    }

    else {
        $this.removeClass('active');
        removeCommentSection();
        showNestableContainer();
    }
}); 

function hideNestableContainer() {
    $('.comments-hidden').hide();
    $('#formPreviewContainer').addClass('comments-active');
}

function showNestableContainer() {
    $('.comments-hidden').show();
    $('#formPreviewContainer').removeClass('comments-active');
}

function reloadComments(dataToSend, taggedCommentId = 0) {
    let urlParameter = taggedCommentId > 0 ? `?taggedCommentId=${taggedCommentId}`: '';
    $.ajax({
        method: 'post',
        url: `/Form/GetAllCommentsByForm${urlParameter}`,
        data: JSON.stringify(dataToSend),
        contentType: 'application/json',
        success: function (data) {
            $('#commentSection').html(data);
            redrawLines();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function clearLines() {
    let c = document.getElementById("designerCanvas");
    let ctx = c.getContext("2d");
    ctx.clearRect(0, 0, c.width, c.height);
}

function showCommentSection() {

    $("#nestableFormElements").hide();
    $('#dd-btn').removeClass('expanded');


    $('.icon-container .edit-button').css('display', 'none');
    $('.icon-container .remove-button').css('display', 'none');

    $('.add-comment-link').removeClass('comment-block-hide');
    $('.add-comment-link').addClass('comment-block-show');

    $("#commentSection").show();
}

function removeCommentSection() {
    $('.icon-container .edit-button').css('display', 'flex');
    $('.icon-container .remove-button').css('display', 'flex');

    $('.add-comment-link').addClass('comment-block-hide');
    $('.add-comment-link').removeClass('comment-block-show');

    $("#commentSection").hide();

    clearLines();
    removeTargetItemActive();
}

function addComment(id) {
    var dataToSend = {  fieldId: id };
    $.ajax({
        method: 'post',
        url: '/Form/AddCommentSection',
        data: JSON.stringify(dataToSend),
        contentType: 'application/json',
        success: function (data) {
            $('#newComment').html(data);  
            redrawLines();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
    $('#newComment').css('display', 'flex');
}

function redrawLines() {
    setTargetItemActive();
    setCanvasSize();
    clearLines();
    drawCommentLines();
}

function setTargetItemActive() {
    $('.comment-section').each(function (index, element) {
        let targetId = $(element).attr('data-field-id');
        $(`#${targetId}`).addClass('active');
    });
}

function removeTargetItemActive() {
    $('.comment-section').each(function (index, element) {
        let targetId = $(element).attr('data-field-id');
        $(`#${targetId}`).removeClass('active');
    });
}

function submitCommentForm(form, e) {
    $(form).validate({
        ignore: []
    });

    if ($(form).valid()) {
        let commentId = $(form).attr('data-id');
        var commentDataIn = {
            Value: $(`#commentText_${commentId}`).html(),
            FormRef: $("li[data-itemtype='form']:first").attr('data-id'),
            ItemRef: $("#itemRef").val(),
            TaggedUsers: getTaggedUsers(commentId)
        };

        $.ajax({
            type: 'post',
            url: '/Form/AddComment',
            data: JSON.stringify(commentDataIn),
            contentType: 'application/json',
            success: function (data) {
                $('#commentSection').html(data);
                redrawLines();
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
    return false;
}

function submitReplayComment(form, e) {
    $(form).validate({
        ignore: []
    });

    if ($(form).valid()) {
        let commentId = $(form).attr('data-id');
        var commText = $(form).find(`#commentText_${commentId}`).html();

        var dataToSend = {        
            commText: commText,
            commentId: commentId,
            TaggedUsers: getTaggedUsers(commentId)
        };

        $.ajax({
            type: 'post',
            url: '/Form/ReplayComment',
            data: JSON.stringify(dataToSend),
            contentType: 'application/json',
            success: function (data) {
                $('#commentSection').html(data); 
                redrawLines();
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }

    return false;
}

function replayComment(id) {
    $(`#${id}`).find('.replay-container:first').css('display', 'block');
    redrawLines();
}

function cancelReplay(id) {
    $(`#${id}`).find('.replay-container:first').css('display', 'none');
    redrawLines();
}

function cancelNewComment() {
    var $div = $('div#newComment');
    $div.css('display', 'none');
}

function sendCommentStatus(id, status) {

    var dataToSend = {
        commentId: id,
        status: status
    };

    $.ajax({
        method: 'post',
        url: '/Form/SendCommentStatus',
        data: JSON.stringify(dataToSend),
        contentType: 'application/json',
        success: function (data) {
            $('#commentSection').html(data);
            redrawLines();
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error("Your account cannot change the status of comments");
        }
    });
}

function drawCommentLines(){
    $('.comment-section').each(function (index, element) {
        let targetId = $(element).attr('data-field-id');
        let targetItem = $(`#${targetId}`);
        drawLine(targetItem, element);
    });
}

function drawLine(item, comment) {
    if ($('#comments-btn') && $('#comments-btn').hasClass('active')) {
        let canvasXoffset = $("#designerCanvas").offset().left;
        let canvasYoffset = $("#designerCanvas").offset().top;

        let itemPaddingLR = parseInt($(item).css('padding-right'));
        let itemPaddingBT = parseInt($(item).css('padding-top'));

        let x1 = $(item).offset().left - canvasXoffset + $(item).width() + itemPaddingLR * 2;
        let y1 = $(item).offset().top - canvasYoffset + ($(item).height() + itemPaddingBT * 2) / 2;

        let x2 = $(comment).offset().left - canvasXoffset;
        let y2 = $(comment).find('.comm-user-icon:first').offset().top - canvasYoffset + $(comment).find('.comm-user-icon:first').height() / 2;

        let c = document.getElementById("designerCanvas");
        let ctx = c.getContext("2d");
        ctx.beginPath();
        ctx.strokeStyle = '#d93d3d';
        ctx.moveTo(x1, y1);
        ctx.quadraticCurveTo(x1 + (x2 - x1) / 4, y1, x1 + (x2 - x1) / 2, y1 + (y2 - y1) / 2);
        ctx.stroke();

        ctx.moveTo(x1 + (x2 - x1) / 2, y1 + (y2 - y1) / 2);
        ctx.quadraticCurveTo(x1 + 3 * (x2 - x1) / 4, y2, x2, y2);
        ctx.stroke();
    }
}

function clearCommentLines() { }

$(document).click(function () {
    if ($('#comments-btn').hasClass('active')) {
        redrawLines();
    }
});

function getTaggedUsers(commentId) {
    let taggedUsers = $(`#commentText_${commentId}`).data();
    delete taggedUsers['text'];
    delete taggedUsers['id'];
    let taggedUserIds = []
    for (let key in taggedUsers) {
        if (taggedUsers[key]) {
            taggedUserIds.push(taggedUsers[key]);
        }
    }
    return taggedUserIds;
}

function renderViewWhenCommentsTabIsActive(taggedCommentId) {
    var $this = $(".comment-button");
    var formId = $this.attr('data-value');
    var dataToSend = { formId: formId };

    $this.addClass('active');
    reloadComments(dataToSend, taggedCommentId);
    showCommentSection();
    hideNestableContainer();
}

function renderViewWhenConsensusTabIsActive() {
    activateConsensusMode();
}

