function handleSuccessFormSubmit() {
    let versionId = $('input[name=VersionId]').val();
    let thesaurusId = $('input[name=thesaurusId]').val();
    let formDefinitionId = $('input[name=formDefinitionId]').val();
    window.location.href = `/FormInstance/GetAllByFormThesaurus?versionId=${versionId}&thesaurusId=${thesaurusId}&formDefinitionId=${formDefinitionId}`;
}