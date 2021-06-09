function openDateTimePicker(event) {
    console.log($(event.currentTarget).closest('.datetime-picker-container'));
    $(event.currentTarget).closest('.datetime-picker-container').find('input:first').datepicker({ dateFormat: 'yy-mm-dd' });
    $(event.currentTarget).closest('.datetime-picker-container').find('input:first').datepicker('show');
}

function openTimePicker(event) {
    event.stopPropagation();
    event.preventDefault();
    $(event.currentTarget).closest('.datetime-picker-container').find(".time-part").timepicker();
    $(event.currentTarget).closest('.datetime-picker-container').find(".time-part").focus();
}

$(document).on("click", ".ui-corner-all", function () {
    $('.time-helper').each(function (index, element) {
        let fullDateTime = $(element).closest('.datetime-picker-container').find('.form-element-field');
        let timeValue = $(element).val().split(" ")[0];
        if ($(element).val().split(" ")[1] === "PM") {
            let incrementedHours = parseInt(timeValue.split(":")[0]) + 12;
            if (incrementedHours > 23) {
                incrementedHours = 12;
            }
            timeValue = `${incrementedHours}:${timeValue.split(":")[1]}`;
        }
        let newValue = `${$(fullDateTime).val().split("T")[0]}T${timeValue}`;
        $(fullDateTime).val(newValue);
    });
});

$(document).on("change", ".date-helper", function () {
    let value = `${$(this).val()}T12:00`;
    $(this).closest('.datetime-picker-container').find('.form-element-field').val(value);
    $(this).closest('.datetime-picker-container').find('.time-helper').val("12:00");

});