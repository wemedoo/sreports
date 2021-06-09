$(document).on('click', '#submit-chapter-info', function (e) {
    if ($('#chapterGeneralInfoForm').valid()) {
        createNewThesaurusIfNotSelected();

        let title = $('#title').val();
        let element = getElement('chapter', title);

        $(element).attr('data-title', encodeURIComponent($('#title').val()));
        $(element).attr('data-thesaurusid', encodeURIComponent($('#thesaurusId').val()));
        $(element).attr('data-description', encodeURIComponent($('#description').val()));
        $(element).attr('data-isreadonly', encodeURIComponent($('#isReadonly').is(":checked")));

        updateTreeItemTitle(element, title);
        closDesignerFormModal(true);
        clearErrorFromElement($(element).attr('data-id'));

    }
    else {
        toastr.error("Chapter informations are not valid");
    }
})