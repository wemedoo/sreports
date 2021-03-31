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
        $(system).addClass('truncate');
        $(system).addClass("tooltip-tipable");
        $(system).attr("title", $('#newTelecomSystem option:selected').text());
        $(system).tooltip();

        let value = document.createElement('td');
        $(value).attr("data-property", 'value');
        $(value).attr("data-value", $('#newTelecomValue').val());
        $(value).html($('#newTelecomValue').val());
        $(value).addClass('truncate');
        $(value).addClass( "tooltip-tipable");
        $(value).attr("title", $('#newTelecomValue').val());
        $(value).tooltip();
       

        let use = document.createElement('td');
        $(use).attr("data-property", 'use');
        $(use).attr("data-value", $('#newTelecomUse').val());
        $(use).html($('#newTelecomUse option:selected').text());
        $(use).addClass('truncate');
        $(use).addClass("tooltip-tipable");
        $(use).attr("title", $('#newTelecomUse option:selected').text());
        $(use).tooltip();

        let identifier = document.createElement('tr');

        $(identifier).append(system).append(value).append(use).append(createRemoveIdentifierButton());
        $(`#telecomsFor${activeTelecomContainer} tbody`).append(identifier);

        resetTelecomForm();
        activeTelecomContainer = '';

        $('#telecomModal').modal('hide');
    }
}

function createRemoveTelecomButton() {
    let div = document.createElement('div');
    $(div).addClass('remove-telecom');

    let i = document.createElement('i');
    $(i).addClass('fas fa-times');

    $(div).append(i);
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
                System: $(telecomSystem).data('value'),
                Value: $(telecomValue).data('value'),
                Use: $(telecomUse).data('value')
            });
        }
    });

    return result;
}

$(document).on('click', '.remove-telecom', function (e) {
    $(e.currentTarget).closest('.telecom').remove();
});

