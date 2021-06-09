function showDetails(e, O40MTId, preferredTerm, definition, synonyms, similars, abbreaviations) {
    e.stopPropagation();
    document.getElementById('thesaurusO40MTId').innerHTML = O40MTId;
    document.getElementById('thesaurusPreferredTerm').innerHTML = preferredTerm;
    document.getElementById('thesaurusDefinition').innerHTML = definition;
    document.getElementById('thesaurusSynonyms').innerHTML = synonyms;
    document.getElementById('thesaurusSimilars').innerHTML = similars;
    document.getElementById('thesaurusAbbreaviations').innerHTML = abbreaviations;
    $('#thesaurusDetails').modal('show');
}

function hideDetails(e) {
    e.stopPropagation();
    $('#thesaurusDetails').modal('hide');
}
