$(document).ready(function () {
    $(".schema-start-datepicker").initDatePicker();

    $('.schema-start-datepicker').datepicker('setDate', 'today');

    $(".schema-start-datepicker").on("focus", function () {
        $(this).removeClass("error");
    });

    $(".schema-start-datepicker").on("change", function () {
        showSchemaOnDateChange();
    });

    $(".schema-start-datepicker").on("keypress", function (e) {
        if (e.key == "Enter") {
            showSchemaOnDateChange();
        }
    });
});

function showSchemaOnDateChange() {
    var viewDisplayType = getViewDisplayType();
    var schemaId = $(`#schemaName${viewDisplayType}`).val();
    if (schemaId) {
        var request = getViewSchemaRequest(schemaId, viewDisplayType);
        getSchema(request);
    }
}