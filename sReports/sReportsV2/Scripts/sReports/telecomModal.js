var activeTelecomContainer = '';
function showTelecomModal(e, container) {
    e.stopPropagation();
    activeTelecomContainer = container;
    resetTelecomForm();
    $('#telecomModal').modal('show');
}

function resetTelecomForm() {
    $('#newTelecomSystem').val('');
    $('#newTelecomValue').val('');
    $('#newTelecomUse').val('');
}

function addNewTelecom(e) {
    e.preventDefault();
    e.stopPropagation();
    if ($('#newTelecomForm').valid()) {
        let system = document.createElement('td');
        $(system).attr("data-property", 'system');
        $(system).attr("data-value", $('#newTelecomSystem').val());
        $(system).html($('#newTelecomSystem option:selected').text());
        $(system).addClass("custom-td-first");
        $(system).attr("title", $('#newTelecomSystem option:selected').text());
        $(system).tooltip();

        let value = document.createElement('td');
        $(value).attr("data-property", 'value');
        $(value).attr("data-value", $('#newTelecomValue').val());
        $(value).html($('#newTelecomValue').val());
        $(value).addClass("custom-td");
        $(value).attr("title", $('#newTelecomValue').val());
        $(value).tooltip();
       

        let use = document.createElement('td');
        $(use).attr("data-property", 'use');
        $(use).attr("data-value", $('#newTelecomUse').val());
        $(use).html($('#newTelecomUse option:selected').text());
        $(use).addClass("custom-td");
        $(use).attr("title", $('#newTelecomUse option:selected').text());
        $(use).tooltip();

        let identifier = document.createElement('tr');
        $(identifier).addClass('edit-raw');

        $(identifier).append(system).append(value).append(use).append(createRemoveTelecomButton());
        $(`#telecomsFor${activeTelecomContainer} tbody`).append(identifier);

        if ($(`#telecomsFor${activeTelecomContainer} tbody`).children(".edit-raw").length == 1) {
            document.getElementById(`telecomsFor${activeTelecomContainer}`).classList.remove("identifier-line-bottom")
        }

        resetTelecomForm();
        activeTelecomContainer = '';
        $('#telecomModal').modal('hide');
    }
}

function createRemoveTelecomButton() {
    let div = document.createElement('td');
    $(div).addClass('remove-telecom');

    let i = document.createElement('i');
    $(i).addClass('remove-icon');

    $(div).append(i);
    $(div).addClass("custom-td-last");
    div.style.position = "relative";
    return div;
}

function GetTelecoms(container) {
    let result = [];
    $(`#telecomsFor${container} table tr`).each(function (index, element) {

        let telecomSystem = $(element).find('[data-property="system"]')[0];
        let telecomValue = $(element).find('[data-property="value"]')[0];
        let telecomUse = $(element).find('[data-property="use"]')[0];


        if ($(telecomSystem).data('value') && $(telecomValue).data('value') && $(telecomUse).data('value')) {
            result.push({
                Id: $(element).data("value"),
                System: $(telecomSystem).data('value'),
                Value: $(telecomValue).data('value'),
                Use: $(telecomUse).data('value')
            });
        }
    });

    return result;
}

$(document).on('click', '.remove-telecom', function (e) {
    if ($(`#telecomsForPatientTelecom tbody`).children(".edit-raw").length == 1) {
        document.getElementById(`telecomsForPatientTelecom`).classList.add("identifier-line-bottom")
    }
    if ($(`#telecomsForPatientContactTelecom tbody`).children(".edit-raw").length == 1) {
        document.getElementById(`telecomsForPatientContactTelecom`).classList.add("identifier-line-bottom")
    }
    $(e.currentTarget).closest('tr').remove();
});

