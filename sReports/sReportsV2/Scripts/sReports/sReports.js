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

function setActiveOrganization(event, value) {
    let formData = { value: value };
    $.ajax({
        type: "PUT",
        url: `/User/UpdateOrganization`,
        data: formData,
        success: function (data) {
            $(event.srcElement).parent().siblings().removeClass('active');
            $(event.srcElement).parent().addClass('active');
            window.location.href = `${location.protocol}//${location.host}${location.pathname}${getRemovedPageInfo() ? '?' + getRemovedPageInfo() : ''}`;
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

    $('input, select, textbox, textarea').focusin(function () {

        if ($(this).prev().length > 0) {
            setLabelColor($(this).prev(), '#337ab7');
        } else {
            setLabelColor($(this).parent().parent().prev(), '#337ab7');
        }        
    });

    $('input, select, textbox, textarea').focusout(function () {
        if ($(this).prev().length > 0) {
            setLabelColor($(this).prev(), '#525252');
        } else {
            setLabelColor($(this).parent().parent().prev(), '#525252');
        }  
    });

    

    function setLabelColor(element, color) {
        $(element).css('color', color);
    }

    window.addEventListener('popstate', function (event) {
       
        //reloadTable();
        this.window.location.reload(true);
        //checkUrlPageParams();
        //reloadTable();
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
