$(document).ready(function () {
    $('#schemaNameMobile').select2({
        minimumInputLength: 3,
        placeholder: 'Type schema\'s name',
        allowClear: true,
        ajax: {
            url: `/SmartOncology/GetAutocompleteSchemaData`,
            dataType: 'json',
            data: function (params) {
                return {
                    Term: params.term,
                    Page: params.page,
                    ExcludeId: $('#id').val(),
                }
            }
        }
    });
    $('#schemaNameMobile').on('change', function (e) {
        viewSchema(e, 'Mobile');
    });
    $('#schemaNameMobile').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });

    $('#patientNameMobile').select2({
        minimumInputLength: 3,
        placeholder: 'Type patient\'s name',
        allowClear: true,
        ajax: {
            url: `/SmartOncology/GetAutocompletePatientData`,
            dataType: 'json',
            data: function (params) {
                return {
                    Term: params.term,
                    Page: params.page,
                    ExcludeId: $('#id').val(),
                }
            }
        }
    });

    $('#patientNameMobile').on('change', function (e) {
        viewPatientData(e, 'Mobile');
        resetSchema();
    });
    $('#patientNameMobile').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });

    $('#patientName').select2({
        minimumInputLength: 3,
        placeholder: 'Type patient\'s name',
        allowClear: true,
        ajax: {
            url: `/SmartOncology/GetAutocompletePatientData`,
            dataType: 'json',
            data: function (params) {
                return {
                    Term: params.term,
                    Page: params.page,
                    ExcludeId: $('#id').val(),
                }
            }
        }
    });

    $('#patientName').on('change', function (e) {
        viewPatientData(e);
        resetSchema();
    });
    $('#patientName').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });

    $('#schemaName').select2({
        minimumInputLength: 3,
        placeholder: 'Type schema\'s name',
        allowClear: true,
        ajax: {
            url: `/SmartOncology/GetAutocompleteSchemaData`,
            dataType: 'json',
            data: function (params) {
                return {
                    Term: params.term,
                    Page: params.page,
                    ExcludeId: $('#id').val(),
                }
            }
        }
    });
    $('#schemaName').on('change', function (e) {
        viewSchema(e);
    });
    $('#schemaName').on('select2:opening select2:closing', function (e) {
        autocompleteEventHandler(e);
    });
});

function resetSchema() {
    $("#schemaData").html('');
}

