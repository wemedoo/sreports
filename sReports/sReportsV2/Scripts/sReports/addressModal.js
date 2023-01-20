function showAddressModal(e) {
    e.stopPropagation();
    resetAddressForm();
    $('#addressModal').modal('show');
}

function resetAddressForm() {
    $('#addressEntityId').val('0');
    $('#city').val('');
    $('#postalCode').val('');
    $('#state').val('');
    $('#country').val('');
    $('#street').val('');
    $('#addressType').val('');
    $('#countryId').val('').trigger("change");
}

function submitAddress(e) {
    e.preventDefault();
    e.stopPropagation();
    if ($('#newAddressForm').valid()) {
        let addressEntity = {
            addressEntityId : $("#addressEntityId").val(),
            city : $('#city').val(),
            postalCode : $('#postalCode').val(),
            state: $('#state').val(),
            country: $("#select2-countryId-container").attr('title'),
            street : $('#street').val(),
            addressType : $('#addressType').val(),
            countryId : $('#countryId').val()
        }

        if (+addressEntity["addressEntityId"]) {
            editAddressInTable(addressEntity);
        } else {
            addAddressToTable(addressEntity);
        }

        modifyAddressTable();
        resetAddressForm();
        $('#addressModal').modal('hide');
    }
}

function editAddressInTable(addressEntity) {
    let addressRow = $("#addresses").find(`tr[data-value="${addressEntity["addressEntityId"]}"]`);
    $(addressRow)
        .attr("data-addressType", addressEntity["addressType"])
        .attr("data-countryId", addressEntity["countryId"])
    $(addressRow).children("[data-property]").each(function (index, addressCell) {
        let propertyName = $(addressCell).attr("data-property");
        let newPropertyValue = addressEntity[propertyName];
        $(addressCell).attr("data-value", newPropertyValue);
        $(addressCell).text(displayCellValue(newPropertyValue));
    });
}

function addAddressToTable(addressEntity) {
    let cityEl = addNewCell("city", addressEntity["city"], "custom-td-first");
    let postalCodeEl = addNewCell("postalCode", addressEntity["postalCode"], "custom-td");
    let stateEl = addNewCell("state", addressEntity["state"], "custom-td");
    let countryEl = addNewCell("country", addressEntity["country"], "custom-td");
    let streetEl = addNewCell("street", addressEntity["street"], "custom-td");

    let address = document.createElement('tr');
    $(address)
        .attr("data-value", addressEntity["addressEntityId"])
        .attr("data-addressType", addressEntity["addressType"])
        .attr("data-countryId", addressEntity["countryId"])
        .addClass('address-entry');

    $(address)
        .append(cityEl)
        .append(postalCodeEl)
        .append(stateEl)
        .append(countryEl)
        .append(streetEl)
        .append(createActionsCell());
    $(`#addresses tbody`).append(address);
}

function addNewCell(cellName, cellValue, cellClass) {
    let el = document.createElement('td');
    $(el).attr("data-property", cellName);
    $(el).attr("data-value", cellValue);
    $(el).text(displayCellValue(cellValue));
    $(el).addClass(cellClass);
    $(el).attr("title", cellValue);
    $(el).tooltip();

    return el;
}

function displayCellValue(cellValue) {
    return cellValue ? cellValue : "N/E";
}

function createActionsCell() {
    let div = document.createElement('td');
    $(div).addClass('custom-td-last position-relative');

    let removeAddressIcon = document.createElement('i');
    $(removeAddressIcon).addClass('remove-icon remove-address');

    let editAddressIcon = document.createElement('img');
    $(editAddressIcon)
        .addClass('edit-address')
        .attr("src", "/Content/img/icons/editing.svg");

    $(div)
        .append(removeAddressIcon)
        .append(editAddressIcon);
    return div;
}

$(document).on('click', '.remove-address', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(e.currentTarget).closest('tr').remove();
    modifyAddressTable();
});

$(document).on('click', '.edit-address, .address-entry', function (e) {
    editAddress(e, $(this));
});

function editAddress(e, $el) {
    showAddressModal(e);
    var addressRow = $el.closest("tr");
    setAddressFormValues(addressRow);
}

function setAddressFormValues(addressRow) {
    $("#addressEntityId").val($(addressRow).attr("data-value"));
    $("#addressType").val($(addressRow).attr("data-addressType"));
    initCountryComponent($(addressRow).attr("data-countryId"));

    $(addressRow).children("[data-property]").each(function (index, addressCell) {
        let propertyName = $(addressCell).attr("data-property");
        let propertyValue = $(addressCell).attr("data-value");
        $(`#${propertyName}`).val(propertyValue);
    });
}

function initCountryComponent(countryId) {
    initCustomEnumSelect2(countryId, countryId, "countryId", "country", "Country");
}

function getAddresses() {
    let result = [];
    $(`#addresses table tbody tr`).each(function (index, addressRow) {
        result.push(getAddress(addressRow));
    });
    return result;
}

function getAddress(addressRow) {
    let addressEntity = {
        Id: $(addressRow).attr("data-value"),
        CountryId: $(addressRow).attr("data-countryId"),
        AddressTypeId: $(addressRow).attr("data-addressType")
    }

    $(addressRow).children("[data-property]").each(function (index, addressCell) {
        let propertyName = $(addressCell).attr("data-property");
        let propertyValue = $(addressCell).attr("data-value");
        addressEntity[propertyName] = propertyValue;
    });
    return addressEntity;
}

function modifyAddressTable() {
    if (hasNoRow()) {
        $("#addresses").addClass("identifier-line-bottom");
    } else {
        $("#addresses").removeClass("identifier-line-bottom");
    }
}

function hasNoRow() {
    return $(`#addresses tbody`).children(".address-entry").length == 0;
}