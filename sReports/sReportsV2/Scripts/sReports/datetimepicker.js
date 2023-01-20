function openDateTimeDatePicker(event) {
    console.log($(event.currentTarget).closest('.datetime-picker-container'));
    let $el = $(event.currentTarget).closest('.datetime-picker-container').find('input[data-date-input]');
    $el.initDatePicker(true, true);
    $el.datepicker('show');
}

function openDateTimeTimePicker(event) {
    event.stopPropagation();
    event.preventDefault();
    $(event.currentTarget).closest('.datetime-picker-container').find(".time-part").timepicker({ timeFormat: 'HH:mm' });
    $(event.currentTarget).closest('.datetime-picker-container').find(".time-part").focus();
}

function showDatePicker(event) {
    var pickerButton = event.target;
    var $datepicker = $(pickerButton).siblings('input[data-date-input]');
    $datepicker
        .initDatePicker(true, true)
        .focus();
}

// DateTime field handlers
var defaultTimePart = "00:00"
$(document).on("click", ".ui-corner-all", function () {
    $('.time-helper').each(function (index, element) {
        handleTimePartChange(element);
    });
});

$(document).on("change", ".date-helper", function () {
    handleDateHelper($(this), true);
});

function handleDateHelper($el, resetTimePart = false) {
    var datePart = $el.val();
    var newDatePart = '';
    var newTimePart = '';
    if (datePart) {
        newDatePart = toFullDateISO($el.val());
        if (resetTimePart) {
            newTimePart = defaultTimePart;
        }
    }
    $el.closest('.datetime-picker-container').find('.form-element-field').val(newDatePart);
    $el.closest('.datetime-picker-container').find('.time-helper').val(newTimePart);
}

$(document).on("change", ".time-helper", function () {
    handleTimePartChange(this);
});

function handleTimePartChange(timeElement) {
    let fullDateTime = $(timeElement).closest('.datetime-picker-container').find('.form-element-field');
    let timeValue = $(timeElement).val().split(" ")[0];
    if ($(fullDateTime).val()) {
        let newValue = `${$(fullDateTime).val().split("T")[0]}T${timeValue ? timeValue : defaultTimePart}`;
        $(fullDateTime).val(newValue);
    } else {
        $(timeElement).val('');
    }
}

var timeDelimiter = ':';
$(document).on("keypress", "input.time-helper", function (e) {
    if (isForbiddenTimeInputCharacter(e.key)) {
        e.preventDefault();
    }

    var allowedInputLength = getAllowedTimeLength();
    var value = $(this).val();
    var inputLength = value.length;

    if (inputLength >= allowedInputLength) {
        e.preventDefault();
    }

    if (isNotTimeForTimeDelimiter(inputLength)) {
        if (e.key == timeDelimiter) {
            e.preventDefault();
        }
    }

    if (isTimeForTimeDelimiter(inputLength)) {
        value += timeDelimiter;
    }

    $(this).val(value);
});

$(document).on("blur", "input.time-helper", function (e) {
    validateTimeInput($(this));
});

function validateTimeInput($input) {
    var timeInputValue = $input.val();
    var timeInputLength = timeInputValue.length;
    var inputDoesNotMatch = toDateStringIfValue(`2000-01-01T${timeInputValue}`) === "Invalid Date";
    var invalid = timeInputLength > 0 && (inputDoesNotMatch || timeInputLength != getAllowedTimeLength());
    if (invalid) {
        $input.addClass("error");
        return false;
    } else {
        $input.removeClass("error");
        return true;
    }
}

function isForbiddenTimeInputCharacter(inputCharacter) {
    return isNaN(inputCharacter) && inputCharacter !== timeDelimiter && inputCharacter != "Enter";
}

function isTimeForTimeDelimiter(inputLength) {
    return inputLength === 2;
}

function isNotTimeForTimeDelimiter(inputLength) {
    return inputLength !== 1;
}

function getAllowedTimeLength() {
    return timeFormatDisplay.length;
}