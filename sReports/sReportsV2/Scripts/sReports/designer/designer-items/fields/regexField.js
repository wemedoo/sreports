$(document).on('change', '#regexPredefined', function (e) {
    $('#regex').val($(this).val());
})
function setCustomRegexFields(element) {
    if (element) {
        $(element).attr('data-regex', encodeURIComponent($('#regex').val()));
        $(element).attr('data-regexdescription', encodeURIComponent($('#regexDescription').val()));
        setCommonStringFields(element);
    }
}