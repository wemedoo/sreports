//date extension methods
$.fn.initDatePicker = function (chooseSepareteMonthYear = false, yearRange = false) {
    var datePickerOptions = {};
    datePickerOptions['dateFormat'] = df;

    if (chooseSepareteMonthYear) {
        datePickerOptions['changeMonth'] = true;
        datePickerOptions['changeYear'] = true;
    }
    if (yearRange) {
        datePickerOptions['yearRange'] = "-110:+1";
    }
    return this.datepicker(datePickerOptions);
}

var rcheckableType = (/^(?:checkbox|radio)$/i);
var
	rbracket = /\[\]$/,
	rCRLF = /\r?\n/g,
	rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i,
	rsubmittable = /^(?:input|select|textarea|keygen)/i;

jQuery.fn.extend({
	// is invoked on $('<FORM_SELECTOR>').serialize()
	serializeArray: function () {
		return this.map(function () {

			// Can add propHook for "elements" to filter or add form elements
			var elements = jQuery.prop(this, "elements");
			return elements ? jQuery.makeArray(elements) : this;
		})
			.filter(function () {
				var type = this.type;

				// Use .is( ":disabled" ) so that fieldset[disabled] works
				return this.name && !jQuery(this).is(":disabled") &&
					rsubmittable.test(this.nodeName) && !rsubmitterTypes.test(type) &&
					(this.checked || !rcheckableType.test(type));
			})
			.map(function (i, elem) {
				var val = jQuery(this).val();

				if (val == null) {
					return null;
				} else {
					if (isDateInput(jQuery(elem))) {
						val = formatDateToValid(val);
					}
				}

				if (Array.isArray(val)) {
					return jQuery.map(val, function (val) {
						return { name: elem.name, value: val.replace(rCRLF, "\r\n") };
					});
				}

				return { name: elem.name, value: val.replace(rCRLF, "\r\n") };
			}).get();
	}
});

function isDateInput($el) {
	return $el.hasClass("date-field-input");
}

//date helpers methods
function toDateStringIfValue(value) {
	return value ? (new Date(formatDateToValid(value))).toDateString() : value;
}

function toDateISOStringIfValue(value) {
	var date = new Date(formatDateToValid(value));
	var [day, month, year] = extractDate(date);
	
	return `${year}-${formatTo2Digits(month)}-${formatTo2Digits(day)}`;
}

function toLocaleDateStringIfValue(valueInUtc) {
	return valueInUtc ? `${valueInUtc}${getUtcTimezoneOffset()}` : valueInUtc;
}

function toValidTimezoneFormat(value) {
	return value.replace(' ', '+')
}

function formatDateToValid(value) {
	if (df == 'dd/mm/yy') {
		return value.replace(/(\d+[/])(\d+[/])/, '$2$1');
	} else {
		return value;
	}
}

function getUtcTimezoneOffset() {
	var offsetInMins = new Date().getTimezoneOffset();
	var offsetSign = offsetInMins * (-1) > 0 ? '+' : '-';
	var offsetHours = Math.abs(offsetInMins / 60);
	var offset = offsetSign + formatTo2Digits(offsetHours) + ":00";

	return offset;
}

function formatTo2Digits(inputDigit) {
	return ('0' + inputDigit).slice(-2);
}

function extractDate(date) {
	var day = date.getDate();
	var month = date.getMonth() + 1;
	var year = date.getFullYear();
	return [day, month, year];
}

function setValueForDateTime(paramName, paramValue) {
	if (paramName === "DateTimeFrom" || paramName === "DateTimeTo") {
		let dateTime = paramValue.slice(0, 16);
		let date = toDateFormatDisplay(dateTime.split("T")[0]);
		let time = dateTime.split("T")[1].slice(0, 5);
		$(`#${firstLetterToLower(paramName)}`).val(dateTime);
		$(`#${firstLetterToLower(paramName)}`).closest('.datetime-picker-container').find('.time-helper').val(time);
		$(`#${firstLetterToLower(paramName)}`).closest('.datetime-picker-container').find('input:first').val(date);
	} else if (paramName === "BirthDate") {
		let dateTime = paramValue.slice(0, 16);
		let formattedDate = toDateFormatDisplay(dateTime.split("T")[0]);
		$("#birthDate").val(formattedDate);
		$("#BirthDateTemp").val(formattedDate);
    }
}

function toDateFormatDisplay(utcDate) {
	var [day, month, year] = extractDate(new Date(utcDate));
	return `${formatTo2Digits(day)}/${formatTo2Digits(month)}/${year}`;
}

function isDateTimeFilter(paramName) {
	return paramName === "DateTimeFrom" || paramName === "DateTimeTo" || paramName === "BirthDate";
}

function getDateTimeFilterTag(params, param) {
	let value;
	let partsOfDate = params[param].split('T')[0].split('-');

	if (param === "DateTimeFrom") {
		value = `From: ${partsOfDate[2]}/${partsOfDate[1]}/${partsOfDate[0]} ${params[param].split('T')[1].slice(0, 5)}`;
	} else if (param === "DateTimeTo") {
		value = `To: ${partsOfDate[2]}/${partsOfDate[1]}/${partsOfDate[0]} ${params[param].split('T')[1].slice(0, 5)}`;
	} else if (param === "BirthDate") {
		value = `${partsOfDate[2]}/${partsOfDate[1]}/${partsOfDate[0]}`;
    }

	return value;
}

var dateDelimiter = '/';
$(document).on("keypress", "input[data-date-input]", function (e) {
	if (isForbiddenDateInputCharacter(e.key)) {
		e.preventDefault();
	}

	var allowedInputLength = getAllowedDateLength();
	var value = $(this).val();
	var inputLength = value.length;

	if (inputLength >= allowedInputLength) {
		e.preventDefault();
	}

	if (isNotTimeForDateDelimiter(inputLength) ) {
		if (e.key == dateDelimiter) {
			e.preventDefault();
		}
	}

	if (isTimeForDateDelimiter(inputLength)) {
		value += dateDelimiter;
	}

	$(this).val(value);
});

$(document).on("blur", "input[data-date-input]", function (e) {
	validateDateInput($(this));
});

function validateDateInput($input) {
	var inputValue = $input.val();
	var inputLength = inputValue.length;
	var parsedDate = toDateStringIfValue(inputValue);
	var invalid = inputLength > 0 && (inputLength !== getAllowedDateLength() || parsedDate === "Invalid Date");
	if (invalid) {
		$input.addClass("error");
		return false;
	} else {
		$input.removeClass("error");
		return true;
	}
}

function isForbiddenDateInputCharacter(inputCharacter) {
	return isNaN(inputCharacter) && inputCharacter !== dateDelimiter && inputCharacter != "Enter";
}

function isTimeForDateDelimiter(inputLength) {
	return inputLength === 2 || inputLength === 5;
}

function isNotTimeForDateDelimiter(inputLength) {
	return inputLength !== 1 || inputLength !== 3;
}

function getAllowedDateLength() {
	return dateFormatDisplay.length;
}

var defaultTimePart = "00:00";
function toFullDateISO(dateInputValue) {
	let datePart = toDateISOStringIfValue(dateInputValue);
	return `${datePart}T${defaultTimePart}`;
}

function copyDateToHiddenField(value, hiddenField) {
	if (value) {
		$(`#${hiddenField}`).val(toFullDateISO(value));
	}
}

function setDateTimeValidatorMethods() {
	$.validator.addMethod(
		"dateInputValidation",
		function (value, element) {
			if ($(element).is("[data-date-input]")) {
				return validateDateInput($(element));
			} else {
				return true;
			}
		},
		`Please put your date in [${dateFormatDisplay}] format.`
	);

	$.validator.addMethod(
		"timeInputValidation",
		function (value, element) {
			if ($(element).hasClass("time-part")) {
				return validateTimeInput($(element));
			} else {
				return true;
			}
		},
		`Please put your time in [${timeFormatDisplay}] format.`
	);

	$('form:has([data-date-input])').validate({
		errorPlacement: function (error, element) {
			handleErrorPlacement(error, element);
		}
	});

	$("input[data-date-input]").each(function () {
		$(this).rules('add', {
			dateInputValidation: true
		});
	});

	$("input.time-part").each(function () {
		$(this).rules('add', {
			timeInputValidation: true
		});
	});
}

function isDateOrTimeInput(element) {
	return element.hasClass("time-part") || element.is("[data-date-input]");
}

function handleErrorPlacement(error, element) {
	if (isDateOrTimeInput(element)) {
		handleErrorPlacementForDateOrTime(error, element);
	} else if (isRadioOrCheckbox(element)) {
		handleErrorPlacementForRadioOrCheckbox(error, element);
	} else {
		handleErrorPlacementForOther(error, element);
	}
}

function handleErrorPlacementForDateOrTime(error, element) {
	var targetContainerForErrors = getElementWhereErrorShouldBeAdded(element);
	modifyIfSecondError(targetContainerForErrors, error);
	error.appendTo(targetContainerForErrors);
}