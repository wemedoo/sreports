$.ajaxSetup({
    statusCode: {
        401: function (response) {
            var returnUrl = encodeURI(window.location.pathname + window.location.search);
            var loginUrl = '/User/Login?ReturnUrl=' + returnUrl;

            window.location.href = loginUrl;
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
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
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
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
            }
        });
    }

}

function setActiveOrganization(event, value, targetUrl) {
    let formData = { value: value };
    $.ajax({
        type: "PUT",
        url: `/UserConfiguration/UpdateOrganization`,
        data: formData,
        success: function (data) {
            $(event.srcElement).parent().siblings().removeClass('active');
            $(event.srcElement).parent().addClass('active');
            if (targetUrl) {
                window.location.href = `${targetUrl}`;
            } else {
                window.location.href = `${location.protocol}//${location.host}${location.pathname}${getRemovedPageInfo() ? '?' + getRemovedPageInfo() : ''}`;
            }
            
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
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
                //alert('server connection established');
            }
            if (realXHR.readyState === 2) {
                //alert('request received');
            }
            if (realXHR.readyState === 3) {
                // alert('processing request');
            }
            if (realXHR.readyState === 4) {
                $("#loaderOverlay").hide(100);
                $('#loaderOverlay').hide();

                //alert('request finished and response is ready');
            }
        }, false);
        return realXHR;
    }
    window.XMLHttpRequest = newXHR;
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

    $('input, select, textbox, textarea').focusin(function () {

        if ($(this).prev().length > 0) {
            setLabelColor($(this).prev(), '#4dbbc8');
        } else {
            setLabelColor($(this).parent().parent().prev(), '#4dbbc8');
        }        
    });

    $(document).on('show.bs.dropdown', function (event) {
        console.log($(event.relatedTarget));
        if ($(event.relatedTarget).hasClass("dropdown-button")) {
            let alredyActive = $(this).children('img:first').hasClass('active');
            $('.dots').each(function (index, element) {
                if ($(element).hasClass('active')) {
                    $(element).removeClass('active');
                }
                $(element).attr('src', '../Content/img/icons/3dots.png');
            });
            let button = $(event.relatedTarget); // Get the text of the element
            let dots = $(button).children('img:first');
            $(dots).attr('src', '../Content/img/icons/dots-active.png');
            $(dots).addClass('active');

            let t = $(event.relatedTarget).children('button:first');
            if ($(event.relatedTarget).children('button:first').hasClass('btns')) {
                $('#btns').children('img:first').attr('src', '../Content/img/icons/dropdown_open.svg');
            }
        }



    });

    $(document).on('hide.bs.dropdown', function (event) {
        
        let button = $(event.relatedTarget);
        if ($(button).closest('.dropdown').children('.dropdown-menu').hasClass('show')) {
            let dots = $(button).children('img:first');
            if ($(dots).hasClass('dots')) {
                $(dots).attr('src', '../Content/img/icons/3dots.png');
                $(dots).removeClass('active');
            }

            $('#btns').children('img:first').attr('src', '../Content/img/icons/dropdown.svg');

        }


        $(button).closest('.dropdown').children('.dropdown-menu').find('.dropdown-item-custom').each(function (index, element) {
                $(element).find('i:first').removeClass('fa-angle-down');
                $(element).find('i:first').addClass('fa-angle-right');
        });
        
    });

    $('input, select, textbox, textarea').focusout(function () {
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

});




$(window).on('beforeunload', function (event) {
    $("#loaderOverlay").show(100);
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

function sendFileData(fileData, setFieldCallback, filesUploadedCallBack) {
    var uploaded = 0;
    for (let i = 0; i < fileData.length; i++) {
        var fd = new FormData();
        fd.append('file', fileData[i].content);

        $.ajax({
            url: `/Blob/Create`,
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
                toastr.success(`Success`);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(` Error: ${xhr.status} ${thrownError}`);
            }
        });
    }
}

function setImageUrl(id, data) {
    $(`#${id}`).attr('value', data);
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
   
    console.log($(this).closest('.filter-element').attr('data-value'));
    $(this).closest('.filter-element').remove();

    $(document).find('.filter-input').each(function (ind, ele) {
        $(ele).val("");
        $(ele).prop("selected", false);
    });

    $(".remove-filter").each(function (index, element) {
        console.log($(element).attr('name'));
        $(`#${firstLetterToLower($(element).attr('name'))}`).val($(element).attr('data-value'));
    });

    advanceFilter();

    $('#advancedId').children('div:first').removeClass('btn-advanced');
    $('#advancedId').find('button:first').addClass('btn-advanced-link');
    $('#advancedId').find('img:first').css('display', 'none');
});

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
    if (idleTime >= 900000) {
        //after 15 minute inactivity modal shows
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


