$(document).ready(function () {
    jQuery.validator.addMethod("duplicate", function (value, element) {
        return !identifierExists();
    }, "Identifier already defined");

    $("#newIdentifierForm").validate({
        onkeyup: false,
        rules: {
            newIdentifierValue: {
                required: true,
                remote: {
                    type: 'GET',
                    url: `/${activeEntity}/ExistIdentifier`,
                    async: false,
                    data: {
                        System: function () {
                            return $('#newIdentifierSystem').val();
                        },
                        Value: function () {
                            return $('#newIdentifierValue').val();
                        },
                        Use: function () {
                            return $('#newIdentifierUse').val();
                        },
                        Type: "",
                        _: function () {
                            return new Date().getTime();
                        }
                    }
                },
                duplicate: true
            }
        },
        messages: {
            newIdentifierValue: {
                remote: "Duplicate identifier."
            }
        }
    });
});

function identifierExists() {
    let result = false;
    $('#identifierContainer').find('table tbody tr').each(function (index, element) {
        if ($(element).find(`[data-value="${$('#newIdentifierSystem').val()}"]`).length && $(element).find(`[data-value="${$('#newIdentifierValue').val()}"]`).length ) {
            result = true;
        }
    });
    return result;
}

function addNewIdentifier(e) {
    e.preventDefault();
    e.stopPropagation();

    if ($('#newIdentifierForm').valid()) {
        let system = document.createElement('td');
        $(system).attr("data-property", 'system');
        $(system).attr("data-value", $('#newIdentifierSystem').val());
        $(system).html($('#newIdentifierSystem option:selected').text());
        $(system).addClass("custom-td-first");
        $(system).attr("title", $('#newIdentifierSystem option:selected').text());
        $(system).tooltip();

        let value = document.createElement('td');
        $(value).attr("data-property", 'value');
        $(value).attr("data-value", $('#newIdentifierValue').val());
        $(value).html($('#newIdentifierValue').val());
        $(value).addClass("custom-td");
        $(value).attr("title", $('#newIdentifierValue').val());
        $(value).tooltip();

        let use = document.createElement('td');
        $(use).attr("data-property", 'use');
        $(use).attr("data-value", $('#newIdentifierUse').val());
        $(use).html($('#newIdentifierUse option:selected').text());
        $(use).addClass("custom-td");
        $(use).attr("title", $('#newIdentifierUse option:selected').text());
        $(use).tooltip();

        let identifier = document.createElement('tr');
        $(identifier).addClass('edit-raw');

        $(identifier).append(system).append(value).append(use).append(createRemoveIdentifierButton());
        $('#identifierContainer tbody').append(identifier);

        if ($('#identifierContainer tbody').children(".edit-raw").length == 1 ) {
            document.getElementById("identifierContainer").classList.remove("identifier-line-bottom");
        }
        $('#identifierModal').modal('hide');
    }
}

function createRemoveIdentifierButton() {
    let div = document.createElement('td');
    $(div).addClass('remove-identifier');

    let i = document.createElement('i');
    $(i).addClass('remove-icon');

    $(div).append(i);
    $(div).addClass("custom-td-last");
    div.style.position = "relative";
    return div;
}

function GetIdentifiers() {
    let result = [];
    $('#identifierContainer table tr').each(function (index, element) {

        let identifierSystem = $(element).find('[data-property="system"]')[0];
        let identifierValue = $(element).find('[data-property="value"]')[0];
        let identifierUse = $(element).find('[data-property="use"]')[0];


        if ($(identifierSystem).data('value') && $(identifierValue).data('value')) {
            result.push({
                Id: $(element).attr('data-value'),
                System: $(identifierSystem).data('value'),
                Value: $(identifierValue).data('value'),
                Use: $(identifierUse).data('value')
            });
        }
    });

    console.log(result);
    return result;
}

$(document).on('click', '.remove-identifier', function (e) {
    if ($('#identifierContainer tbody').children(".edit-raw").length == 1) {
        document.getElementById("identifierContainer").classList.add("identifier-line-bottom");
    }
    $(e.currentTarget).closest('tr').remove();
});

function showIdentifierModal(e) {
    e.stopPropagation();

    $('#newIdentifierValue').val('');
    $('#newIdentifierSystem').val('');
    $('#newIdentifierUse').val('');

    $('#identifierModal').modal('show');
}