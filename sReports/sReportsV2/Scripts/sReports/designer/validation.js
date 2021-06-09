function validateFormDefinition(formDefinition) {
    clearPreviousErrors();
    let message = '';
    let defaultMessage = 'Please define the following fields for the form: <br/>';
    if (!formDefinition.thesaurusid && !formDefinition.thesaurusId) {
        message = 'Please define the following fields for the form: <br/>'
        message += '-Thesaurus <br/>';
        addError(formDefinition.id);
    }

    if (!formDefinition.version) {
        message = message || defaultMessage;
        message += '-Version<br/>';
        addError(formDefinition.id);
    }

    message += validateChapters(formDefinition.Chapters);

    return message;
}

function validateChapters(chapters) {
    let message = '';
    let defaultMessage = 'Please define the following fields for the chapter: <br/>';
    if (chapters) {
        chapters.forEach(function (chapter, index) {
            if (!chapter.thesaurusid) {
                message = 'Please define the following fields for the chapter: <br/>';
                message += '-Thesaurus <br/>';
                addError(chapter.id);
            }
            message += validatePages(chapter.Pages);
        });
    }

    return message;
}

function validatePages(pages) {
    let message = '';
    let defaultMessage = 'Please define the following fields for the page: <br/>';
    if (pages) {
        pages.forEach(function (page, index) {
            if (!page.thesaurusid) {
                message = 'Please define the following fields for the page: <br/>';
                message += '-Thesaurus <br/>';
                addError(page.id);
            }
            message += validateFieldSets(page.ListOfFieldSets);
        });
    }

    return message;
}

function validateFieldSets(fieldSets) {
    let message = '';
    let defaultMessage = 'Please define the following fields for the fieldset: <br/>';
    if (fieldSets) {
        fieldSets.forEach(function (listOfFS, index) {
            listOfFS.forEach(function (fieldSet, index) {
                if (!fieldSet.thesaurusid) {
                    message = 'Please define the following fields for the field set: <br/>';
                    message += '-Thesaurus <br/>';
                    addError(fieldSet.id);
                }
                message += validateFields(fieldSet.Fields);

            });
        });
    }

    return message;
}

function validateFields(fields) {
    let message = '';
    let defaultMessage = 'Please define the following fields for the field: <br/>';
    if (fields) {
        fields.forEach(function (field, index) {
            if (!field.thesaurusid) {
                message = 'Please define the following fields for the field: <br/>';
                message += '-Thesaurus <br/>';
                addError(field.id);
            }
            if (field.Values) {
                message += validateFieldValues(field.Values);
            }
        });
    }

    return message;
}

function validateFieldValues(values) {
    let message = '';
    let defaultMessage = 'Please define the following fields for the field value: <br/>';
    if (values) {
        values.forEach(function (value, index) {
            if (!value.thesaurusid) {
                message = 'Please define the following fields for the field value: <br/>';
                message += '-Thesaurus <br/>';
                addError(value.id);
            }
        });
    }



    return message;
}

function addError(id) {
    if (id) {
        $("#nestable").find(`[data-id='${id}']`).children('.dd-handle').addClass('nestable-error');
    } else {
        $("#nestable").find(`[data-itemtype='form']`).children('.dd-nohandle').addClass('nestable-error');
    }
}

function clearPreviousErrors() {
    $('.nestable-error').each(function (index, element) {
        $(element).removeClass('nestable-error');
    });
}

function clearErrorFromElement(id) {
    if (id && id !=="formIdPlaceHolder") {
        $("#nestable").find(`[data-id='${id}']`).children('.dd-handle').removeClass('nestable-error');
    } else {
        $("#nestable").find(`[data-itemtype='form']`).children('.dd-nohandle').removeClass('nestable-error');
    }
}
