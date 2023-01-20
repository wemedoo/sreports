$(document).on('click', '#submit-fieldset-info', function (e) {
    if ($('#fieldsetGeneralInfoForm').valid()) {
        createNewThesaurusIfNotSelected();

        let label = $('#label').val();
        let elementId = $('#elementId').val();
        let element = getElement('fieldset', label);
        if (element) {
            $(element).attr('data-id', encodeURIComponent(elementId));
            $(element).attr('data-label', encodeURIComponent(label));
            $(element).attr('data-thesaurusid', encodeURIComponent($('#thesaurusId').val()));
            $(element).attr('data-description', encodeURIComponent($('#description').val()));
            $(element).attr('data-isbold', encodeURIComponent($('#isBold').is(":checked")));
            $(element).attr('data-fhirtype', encodeURIComponent($('#fhirType').val()));
            $(element).attr('data-isrepetitive', encodeURIComponent($('#isRepetitive').val()));
            $(element).attr('data-numberofrepetitions', encodeURIComponent($('#numberOfRepetitions').val()));
            setFieldSetLayout(element);
            setFieldSetHelp(element);

            updateTreeItemTitle(element, label);
            closDesignerFormModal(true);
            clearErrorFromElement($(element).attr('data-id'));
        }
    }
    else {
        toastr.error("Field set informations are not valid");
    }
})

function setFieldSetLayout(element) {
    let layoutType = $('#layoutType').val();
    let layouMaXItems = $('#layoutMaxItems').val();
    let layout = null;
    if (layoutType || layouMaXItems) {
        layout = getDataProperty(element, 'layoutstyle') || {};
        layout['layoutType'] = layoutType;
        layout['maxItems'] = layouMaXItems;
    }

    $(element).attr('data-layoutstyle', encodeURIComponent(JSON.stringify(layout)));

}

function setFieldSetHelp(element) {
    let helpContent = CKEDITOR.instances.helpContent.getData();
    let helpTitle = $('#helpTitle').val();
    let help = null;
    if (helpTitle || helpContent) {
        help = getDataProperty(element, 'help') || {};
        help['content'] = helpContent;
        help['title'] = helpTitle;
    }

    $(element).attr('data-help', encodeURIComponent(JSON.stringify(help)));

}

$(document).on('mouseover', '.fieldset-custom-dd-handle', function (e) {
    $(e.target).closest('li').children('button').addClass('white');
});

$(document).on('mouseout', '.fieldset-custom-dd-handle', function (e) {
    $(e.target).closest('li').children('button').removeClass('white');
});