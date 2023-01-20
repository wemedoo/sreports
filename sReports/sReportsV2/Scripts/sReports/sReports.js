$.ajaxSetup({
    statusCode: {
        401: function (response) {
            var returnUrl = encodeURI(window.location.pathname + window.location.search);
            var loginUrl = '/User/Login?ReturnUrl=' + returnUrl;

            window.location.href = loginUrl;
        },
        403: function (response) {
            toastr.error(response.responseText ? response.responseText : response.statusText);
        }
    }
});

document.addEventListener("change", function (event) {
    let element = event.target;
    if (element && element.matches(".form-element-field")) {
        element.classList[element.value ? "add" : "remove"]("-hasvalue");
    }
});

function showFormDefinitionData(event, formId) {
    $('.table-active').removeClass('table-active');
    $(event.srcElement).closest('tr').addClass('table-active');
}

function setActiveLanguage(event, value) {
    event.preventDefault();
    let params = changeLanParam(value);
    if (simplifiedApp) {
        $(event.srcElement).parent().siblings().removeClass('active');
        $(event.srcElement).parent().addClass('active');
        window.location.href = `${location.protocol}//${location.host}${location.pathname}${params}`;
    } else {

    let formData = { value: value };
    $.ajax({
        type: "PUT",
        url: `/User/UpdateLanguage`,
        data: formData,
        success: function (data) {
            $(event.srcElement).parent().siblings().removeClass('active');
            $(event.srcElement).parent().addClass('active');
            window.location.href = `${location.protocol}//${location.host}${location.pathname}${getRemovedPageInfo() ? '?'+ getRemovedPageInfo():''}`;
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
    }

}

function changeLanParam(newValue) {
    let params = location.search.split('&');
    let newParams = [];
    params.filter(x => !x.includes('language') && !x.includes('Language')).forEach(x => {
        newParams.push(x);
    })
    newParams.push(`language=${newValue}`);

    return newParams.join('&');
}

function updatePageSize(value) {
    if (!simplifiedApp) {
        let formData = { PageSize: value };
        $.ajax({
            type: "PUT",
            url: `/User/UpdatePageSizeSettings`,
            data: formData,
            success: function (data) {
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }

}

function setActiveOrganization(event, value, targetUrl) {
    console.log('updating organizations');
    event.preventDefault();
    $.ajax({
        type: "PUT",
        url: `/UserAdministration/UpdateOrganization?organizationId=${value}`,
        success: function (data) {
            $(event.srcElement).parent().siblings().removeClass('active');
            $(event.srcElement).parent().addClass('active');
            if (targetUrl) {
                window.location.href = `${targetUrl}`;
            } else {
                window.location.href = `${location.protocol}//${location.host}${location.pathname}${getRemovedPageInfo() ? '?' + getRemovedPageInfo() : ''}`;
            }
            
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getRemovedPageInfo(){
    var url = window.location.search;
    let splitted = url.replace("?", '').split('&');
    return splitted.filter(x => !x.includes('page=') && !x.includes('pageSize=')).join('&');
}

function showHideLoaderOnAjaxRequest() {
    var oldXHR = window.XMLHttpRequest;

    function newXHR() {
        var realXHR = new oldXHR();
        realXHR.addEventListener("readystatechange", function () {
            if (realXHR.readyState === 1) {
                $("#loaderOverlay").show();
                //console.log('server connection established');
            }
            if (realXHR.readyState === 2) {
                //console.log('request received');
            }
            if (realXHR.readyState === 3) {
                //console.log('processing request');
            }
            if (realXHR.readyState === 4) {
                var multiFileDownLoad = GetResponseBooleanHeader(realXHR, "MultiFile", false);
                if (realXHR.responseType == "blob" || multiFileDownLoad) {
                    hideLoaderForMultiFileResponse(realXHR);
                } else {
                    hideLoader();
                }
                //console.log('request finished and response is ready');
            }
        }, false);
        return realXHR;
    }
    window.XMLHttpRequest = newXHR;
}

function hideLoader() {
    $("#loaderOverlay").hide(100);
    $('#loaderOverlay').hide();
}

function hideLoaderForMultiFileResponse(xhr) {
    var lastFile = GetResponseBooleanHeader(xhr, "LastFile", true);
    if (lastFile) {
        hideLoader();
    }
}

function GetResponseBooleanHeader(xhr, name, predifinedValue) {
    var headerParam = xhr.getResponseHeader(name);
    return headerParam == null ? predifinedValue : JSON.parse(headerParam);
}

function checkUrlPageParams() {

    var url = new URL(window.location.href);
    var page = url.searchParams.get("page");
    var pageSize = url.searchParams.get("pageSize");

    if (page && pageSize) {
        currentPage = page;
        $('#pageSizeSelector').val(pageSize);
    }
    else {
        currentPage = 1;
    }
}

function checkSecondaryPage() {

    var url = new URL(window.location.href);
    var secondaryPage = url.searchParams.get("secondaryPage");

    if (secondaryPage) {
        formsCurrentPage = secondaryPage;
    } else {
        formsCurrentPage = 1;
    }
}



$(document).ready(function () {
    showHideLoaderOnAjaxRequest();
    $('#closeSidebar, #showSidebar').on('click', function () {
        $('body').toggleClass('sidebar-on');
    });

    $('#menuShrinkBtn').on('click', function () {
        $('body').toggleClass('sidebar-shrink');
    });


    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
        
        console.log("Clicked");
        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
        }
        var $subMenu = $(this).next(".dropdown-menu");
        $subMenu.toggleClass('show');


        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
            $('.dropdown-submenu .show').removeClass("show");
        });

        return false;
    });

   

    $('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
        console.log('test');
        $('.dropdown-item-custom').each(function (index, element) {
            switchAngles(element);
        });
       
        return false;
    });

    $('.dropdown-menu').on('change', function (e) {
        $('.dropdown-item-custom').each(function (index, element) {
            switchAngles(element);
        });

        return false;
    });

    $(document).on('show.bs.dropdown', function (event) {
        console.log($(event.relatedTarget));
        if ($(event.relatedTarget).hasClass("dropdown-button")) {
            let alredyActive = $(this).children('img:first').hasClass('active');
            $('.dots').each(function (index, element) {
                if ($(element).hasClass('active')) {
                    $(element).removeClass('active');
                }
                $(element).attr('src', '/Content/img/icons/3dots.png');
            });
            let button = $(event.relatedTarget); // Get the text of the element
            let dots = $(button).children('img:first');
            $(dots).attr('src', '/Content/img/icons/dots-active.png');
            $(dots).addClass('active');

            let t = $(event.relatedTarget).children('div:first');
            if ($(event.relatedTarget).children('div:first').hasClass('btns')) {
                $('#btns').children('img:first').attr('src', '/Content/img/icons/dropdown_open.svg');
            }
        }
    });

    $(document).on('hide.bs.dropdown', function (event) {
        let button = $(event.relatedTarget);
        if ($(button).closest('.dropdown').children('.dropdown-menu').hasClass('show')) {
            let dots = $(button).children('img:first');
            if ($(dots).hasClass('dots')) {
                $(dots).attr('src', '/Content/img/icons/3dots.png');
                $(dots).removeClass('active');
            }

            $('#btns').children('img:first').attr('src', '/Content/img/icons/dropdown.svg');

        }

        $(button).closest('.dropdown').children('.dropdown-menu').find('.dropdown-item-custom').each(function (index, element) {
                $(element).find('i:first').removeClass('fa-angle-down');
                $(element).find('i:first').addClass('fa-angle-right');
        });
        
    });

    $(document).on('focusin', 'input, select, textbox, textarea', function () {
        if ($(this).data('no-color-change')) return;

        if ($(this).prev().length > 0) {
            setLabelColor($(this).prev(), '#4dbbc8');
        } else {
            setLabelColor($(this).parent().parent().prev(), '#4dbbc8');
        }
    });

    $(document).on('focusout', 'input, select, textbox, textarea', function () {
        if ($(this).data('no-color-change')) return;

        if ($(this).prev().length > 0) {
            setLabelColor($(this).prev(), '#000000');
        } else {
            setLabelColor($(this).parent().parent().prev(), '#000000');
        }  
    });

    $(document).on('click', '.close-modal' , function (e) {
        $(this).closest('.modal').modal('hide');

        return false;
    });

    $('#sessionBreakModal').on('hidden.bs.modal', function () {
        restartIdleInterval();
        clearInterval(secondsInteral);
    });


    function setLabelColor(element, color) {
        $(element).css('color', color);
    }

    window.addEventListener('popstate', function (event) {
        this.window.location.reload(true);
    }, false);

    setCommonValidatorMethods();
});

function setCommonValidatorMethods() {
    setDateTimeValidatorMethods();
}

function isRadioOrCheckbox(element) {
    return element.is(":radio") || element.is(":checkbox");
}

function getElementWhereErrorShouldBeAdded(element) {
    var parentFieldSelector = element.attr("data-parent-field");
    if (!parentFieldSelector) {
        parentFieldSelector = ".advanced-filter-item";
    }
    return element.closest(parentFieldSelector);
}

function modifyIfSecondError(targetContainerForErrors, error) {
    var elementHasAlreadyOneError = targetContainerForErrors.find("label.error").length == 1;
    if (elementHasAlreadyOneError) {
        error.css("bottom", "-37px");
    }
}

function handleErrorPlacementForRadioOrCheckbox(error, element) {
    error.appendTo(element.parent().parent());
}

function handleErrorPlacementForOther(error, element) {
    error.appendTo(element.parent());
}

$(window).on('beforeunload', function (event) {
    $("#loaderOverlay").show(100);
    setTimeout(function () {
        hideLoader();
    }, 1500);
});

function logout(e) {
    e.preventDefault();
    e.stopPropagation();

    window.location.href = '/User/Logout';

}

function switchAngles(element) {
    if ($(element).children('ul:first').hasClass('show')) {
        $(element).find('i:first').removeClass('fa-angle-right');
        $(element).find('i:first').addClass('fa-angle-down');
    }
    else {
        $(element).find('i:first').removeClass('fa-angle-down');
        $(element).find('i:first').addClass('fa-angle-right');
    }
}

function sendFileData(fileData, setFieldCallback, filesUploadedCallBack, url = '/Blob/Create') {
    var uploaded = 0;
    for (let i = 0; i < fileData.length; i++) {
        var fd = new FormData();
        fd.append('file', fileData[i].content);

        $.ajax({
            url: url,
            data: fd,
            processData: false,
            contentType: false,
            type: 'POST',
            success: function (data) {
                uploaded++;
                if (setFieldCallback) {
                    console.log('setting field: ' + fileData[i].id);
                    setFieldCallback(fileData[i].id, data);
                }

                if (uploaded === fileData.length) {
                    filesUploadedCallBack();
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

function setImageUrl(id, url) {
    $(`#${id}`).val(url);
}

$(".menu-btn-md").on("click", function () {
    if ($('.navbar-collapse').hasClass('show')) {
        $('body').css('overflow', 'auto');
    } else {
        $('body').css('overflow', 'hidden');
    }
    $(".navbar-collapse").collapse('toggle');
});

function setFilterFromUrl() {
    let url = new URL(window.location.href);
    let entries = url.searchParams.entries();
    let params = paramsToObject(entries);

    if (defaultFilter) {
        defaultFilter = params;
    }

    for (let param in params) {
        $(`#${param}`).val(`${params[param]}`);
        $(`#${param}Temp`).val(`${params[param]}`);
        setValueForDateTime(param, params[param]);
    }
}

function paramsToObject(entriesData) {
    let result = {};
    for (let tupple of entriesData) { // each 'entry' is a [key, value] tupple
        const [key, value] = tupple;
        result[key] = value;
    }
    return result;
}

function reviewThesaurus(event,id)
{
    event.stopPropagation();
    event.preventDefault();
    window.location.href = `/ThesaurusEntry/GetReviewTree?id=${id}&page=${1}&pageSize=10`;
    
    console.log(id);
}

$(document).on('click', '.remove-filter', function (e) {
   
    $(this).closest('.filter-element').remove();

    $(document).find('.filter-input').each(function (ind, ele) {
        $(ele).val("");
        $(ele).prop("selected", false);
        emptyValueForSelect2(ele);
    });

    $(".remove-filter").each(function (index, element) {
        $(`#${firstLetterToLower($(element).attr('name'))}`).val($(element).attr('data-value'));
        setValueForSelect2(element);
        setValueForDateTime($(element).attr('name'), $(element).attr('data-value'));
    });

    advanceFilter();
});

function emptyValueForSelect2(element) {
    if (isSelect2(element)) {
        $(element).val("").trigger("change");
    }
}

function setValueForSelect2(element) {
    let targetElement = $(`#${firstLetterToLower($(element).attr('name'))}`);
    if (isSelect2(targetElement)) {
        $(targetElement).val($(element).attr('data-value')).trigger("change");
    }
}

function isSelect2(element) {
    return $(element).data('select2Id');
}

function firstLetterToLower(string) {
    return string.charAt(0).toLowerCase() + string.slice(1);
}

function goToThesaurus(thesaurusId) {
    if (thesaurusId) {
        window.open(`/ThesaurusEntry/EditByO4MtId?id=${thesaurusId}`, '_blank');

        //window.location.href = `/ThesaurusEntry/EditByO4MtId?id=${thesaurusId}`;
    }
}


var idleTime = 0
var secondsInteral;
var totalSeconds;


document.addEventListener('mousemove', resetIdleTime, false);
document.addEventListener('keypress', resetIdleTime, false);

function resetIdleTime() {
    idleTime = 0
}


function checkIfIdle() {
    idleTime += 1000
    if (idleTime >= 7200000) {
        //after 2 hours inactivity modal shows
        showSessionBreakModal();
        clearInterval(idleInterval);
    }
}

var idleInterval = setInterval(checkIfIdle, 1000);
function restartIdleInterval() {
    idleInterval = setInterval(checkIfIdle, 1000);
}

function showSessionBreakModal() {
    $('#expiredSeconds').width(0);
    $('#remainingSeconds').css("width","100%");
    totalSeconds = 60;
    $("#seconds").text(totalSeconds);
    secondsInteral = setInterval(setTime, 1000);
    $('#sessionBreakModal').modal('show');

}


function setTime() {
    if (totalSeconds > 0) {
        --totalSeconds;
        $("#seconds").text(totalSeconds);
        let secondsLineWidth = $('#secondsLine').width();
        let oneSecondWidth = secondsLineWidth / 60;
        $('#remainingSeconds').width(oneSecondWidth * totalSeconds);
        $('#expiredSeconds').width(oneSecondWidth * (60 - totalSeconds));
    }
    else {
        $('#sessionBreakLogout').click();
    }
}

function continueSession(){
    $('#sessionBreakModal').modal('hide');
}

function decodeLocalizedString(value) {
    return $('<div/>').html(value).text();
}

function objectHasNoProperties(object, excludeList) {
    return !Object.keys(object).filter(x => !excludeList.includes(x)).length;
}

function handleResponseError(xhr, thrownError) {
    if (xhr.status != 403) {
        toastr.error(`${xhr.status} ${thrownError}`);
    }
}

$(document).on('show.bs.modal', '.modal, .custom-modal', function (e) {
    setLowerZIndexForSidebar();
});

$(document).on('hidden.bs.modal', '.modal, .custom-modal', function (e) {
    setDefaultZIndexForSidebar();
});

$(document).on('lowZIndex', '.custom-modal, .digital-guideline-modal, .modal-window', function (e) {
    setLowerZIndexForSidebar();
});

$(document).on('defaultZIndex', '.custom-modal, .digital-guideline-modal, .modal-window', function (e) {
    setDefaultZIndexForSidebar();
});

function setLowerZIndexForSidebar() {
    $('.sidebar').css("z-index", "1010");
}

function setDefaultZIndexForSidebar() {
    $('.sidebar').css("z-index", "1052");
}

function showAdministrativeArrowIfOverflow(administrativeContainerId) {
    var $administrativeChangeContainer = $(`#${administrativeContainerId}`);

    if ($administrativeChangeContainer.length > 0) {
        var childrenWidthTotal = 0;
        $administrativeChangeContainer.find('.workflow-item').each(function () {
            childrenWidthTotal += Math.round($(this).outerWidth());
        });

        var $arrows = $administrativeChangeContainer.find(".arrow-scroll");
        var overflow = childrenWidthTotal > $administrativeChangeContainer.outerWidth();
        if (overflow) {
            $arrows.removeClass('d-none');
        } else {
            $arrows.addClass('d-none');
        }
    }
}

function downloadImage(event, uri, fileName) {

    if (uri) {
        request = {};
        request['resourceId'] = uri;

        $.ajax({
            type: 'GET',
            url: `/Blob/Download`,
            data: request,
            xhr: function () {
                var xhr = new XMLHttpRequest();
                xhr.responseType = 'blob';
                return xhr;
            },
            success: function (data) {
                const url = window.URL.createObjectURL(data);
                const a = document.createElement('a');
                a.style.display = 'none';
                a.href = url;
                a.download = fileName;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

function pressButtonOnEnterKey(event, buttonId) {
    if (event.keyCode === 13) {  // when pressing Enter key
        event.preventDefault();
        $(`#${buttonId}`).click();
    }
}

function initCustomEnumSelect2(hasSelectedCustomEnum, customEnumId, customEnumName, customEnumDisplayName, customEnumType) {
    var $customEnumElement = $(`#${customEnumName}`);
    if (hasSelectedCustomEnum) {
        $.ajax({
            type: 'GET',
            url: `/Patient/GetCustomEnum?customEnumId=${customEnumId}`,
            success: function (data) {
                var initDataSource = [];
                if (!jQuery.isEmptyObject(data)) {
                    initDataSource.push(data);
                }
                initSelect2Component($customEnumElement, getCustomEnumSelect2Object(customEnumDisplayName, customEnumType, initDataSource));
                $customEnumElement.val(customEnumId).trigger("change");
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    } else {
        initSelect2Component($customEnumElement, getCustomEnumSelect2Object(customEnumDisplayName, customEnumType));
    }
}

function hasParamInUrl(name) {
    const urlParams = new URLSearchParams(location.search);
    return urlParams.has(name);
}

function getParamFromUrl(name) {
    const urlParams = new URLSearchParams(location.search);
    return urlParams.get(name);
}

function initSelect2Component($select2Element, select2Object) {
    if (!$select2Element.hasClass("select2-hidden-accessible")) {
        $select2Element.select2(select2Object);
    }
}

function getCustomEnumSelect2Object(placeholderName, customEnumType, initialData = []) {
    return {
        minimumInputLength: 3,
        placeholder: `Type ${placeholderName}\'s name`,
        allowClear: true,
        ajax: {
            url: `/Patient/GetAutoCompleteCustomEnumData?customEnumType=${customEnumType}`,
            dataType: 'json',
            data: function (params) {
                return {
                    Term: params.term,
                    Page: params.page,
                    ExcludeId: $('#id').val(),
                }
            }
        },
        data: initialData
    };
}

function inputsAreInvalid(inputformType = "search") {
    var numOfInvalidInputs = 0;
    $("input[data-date-input]").each(function () {
        var isInputValid = validateDateInput($(this));
        if (!isInputValid) {
            ++numOfInvalidInputs;
        }
    });
    $(".time-helper").each(function () {
        var isInputValid = validateTimeInput($(this));
        if (!isInputValid) {
            ++numOfInvalidInputs;
        }
    });
    var areInputsInvalid = numOfInvalidInputs > 0;
    if (areInputsInvalid) {
        toastr.error(`Some of ${inputformType} inputs are not valid!`);
    }
    return areInputsInvalid;
}