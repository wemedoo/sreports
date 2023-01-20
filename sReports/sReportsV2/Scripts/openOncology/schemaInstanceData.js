(function ($) {
    "use strict";

    //Show table drop menu - TABLE-FINDINGS-DATA
    $('.table-findings-data input').on('click', function (e) {
        $('#drop-menu').remove();
        let currentInput = $(e.target);
        currentInput.parent().append('<div tabindex="0" id="drop-menu" class="drop-menu-lg"><div id="findings-NF" class="drop-item"><span class="text-strong">NF</span>No Findings</div><div id="findings-MSF" class="drop-item"><span class="text-strong">MSF</span>Moderately significant findings</div><div id="findings-SF" class="drop-item"><span class="text-strong">SF</span>Significant findings</div><div id="comm" class="drop-item comment"><i class="fas fa-plus-circle"></i> Comment</div></div>');
        $('#drop-menu').focus();
        currentInput.focus();
        addInputValue('#findings-NF', currentInput);
        addInputValue('#findings-MSF', currentInput);
        addInputValue('#findings-SF', currentInput);
        addInputIcons('#comm', currentInput);
        currentInput.on('keypress', function (e) {
            e.preventDefault();
        });
    });

    //Show table drop menu - TABLE-SYMPTOMS-DATA
    $('.table-symptoms-data input').on('click', function (e) {
        $('#drop-menu').remove();
        let currentInput = $(e.target);
        currentInput.parent().append('<div tabindex="0" id="drop-menu" class="drop-menu-lg"><div id="symtoms-NS" class="drop-item"><span class="text-strong">NS</span>No Symtoms</div><div id="symtoms-MS" class="drop-item"><span class="text-strong">MS</span>Mild Symptoms</div><div id="symtoms-SS" class="drop-item"><span class="text-strong">SS</span>Severe Symptoms</div><div id="comm" class="drop-item comment"><i class="fas fa-plus-circle"></i> Comment</div></div>');
        $('#drop-menu').focus();
        currentInput.focus();
        currentInput.on('keypress', function (e) {
            e.preventDefault();
        });
        addInputValue('#symtoms-NS', currentInput);
        addInputValue('#symtoms-MS', currentInput);
        addInputValue('#symtoms-SS', currentInput);
        addInputIcons('#comm', currentInput);
    });

    //Show table drop menu - TABLE-TUMOR-RESPONSE
    $('.table-tumor-response input').on('click', function (e) {
        $('#drop-menu').remove();
        let currentInput = $(e.target);
        currentInput.parent().append('<div tabindex="0" id="drop-menu" class="drop-menu-lg"><div id="t-response-CR" class="drop-item"><span class="text-strong">CR</span>Complete Response</div><div id="t-response-SD" class="drop-item"><span class="text-strong">SD</span>Stable Disease</div><div id="t-response-PD" class="drop-item"><span class="text-strong">PD </span>Progressive Disease</div><div id="comm" class="drop-item comment"><i class="fas fa-plus-circle"></i> Comment</div></div>');
        $('#drop-menu').focus();
        currentInput.focus();
        currentInput.on('keypress', function (e) {
            e.preventDefault();
        });

        addInputValue('#t-response-CR', currentInput);
        addInputValue('#t-response-SD', currentInput);
        addInputValue('#t-response-PD', currentInput);
        addInputIcons('#comm', currentInput);
    });

    //Handle icons that are already on page
    $('.tbody-child td i').on('click', function (e) {
        const currentInput = $(e.target).siblings('input');
        handleInputClick(currentInput);
    });
    //Handle appended icons -after page loaded
    $('.tbody-child td').on('click', 'i', function (e) {
        const currentInput = $(e.target).siblings('input');
        handleInputClick(currentInput);
    });
    
    //Hide drop-menu on click outside
    $(document).on('click', function (e) {
        const el = $("#drop-menu");
        if (!el.is(e.target) && el.has(e.target).length === 0 && !($(e.target).parents('.tbody-child').length > 0)) {
            el.hide();
        }
    });

    // toggle labaratory button text on click  
    $(".toggle-lab").on("click", function (e) {
        let el = $(this);
        if (el.attr("aria-expanded") === 'false') {
            el.text(el.data("text-swap"));
        } else {
            el.text(el.data("text-original"));
        }
    });
    $('.labaratory-parent').on('click', function (e) {
        let el = $('button.toggle-lab');
        if (el.attr("aria-expanded") === 'false') {
            el.text(el.data("text-swap"));
        } else {
            el.text(el.data("text-original"));
        }
    });

    //toggle chart button text on click  
    $('.toggle-chart').on('click', function (e) {
        let el = $(this);
        let btnText = $('.btn-txt');
        if (el.attr("aria-expanded") === 'false') {
            btnText.text(el.data("text-swap"));
        } else {
            btnText.text(el.data("text-original"));
        }
    });

    //Scroll right arrow
    if ($(".table-wrapper").prop('scrollWidth') > $(".table-wrapper").width()) {
        $('#scroll-right').on('click', function (e) {
            scrollLeftRight("+=");
        });
        $('#scroll-left').on('click', function (e) {
            scrollLeftRight("-=");
        });
    } else {
        $('#scroll-right').hide();
        $('#scroll-left').hide();
    }

    // input field tooltip
    $('.input-field-input').tooltip({
        html: true,
        placement: "bottom",
    });
})(jQuery);

function scrollLeftRight(direction, offset = 125) {
    $('.table-wrapper').animate({ scrollLeft: direction + offset });
}

function handleInputClick(currentInput) {
    $('#drop-menu').remove();

    addDropMenu(currentInput);

    $('#drop-menu').focus();
    currentInput.focus();

    addInputIcons('#edit-dose', currentInput);
    addInputIcons('#same-dose', currentInput);
    addInputIcons('#stop', currentInput);
    addInputIcons('#comm', currentInput);

    currentInput.on('keypress', function () {
        $('#drop-menu').remove();
        removeInputIcons(currentInput)
    });
}

function renderDropMenu() {
    return `<div tabindex = "0" id = "drop-menu" class="${expandCssClass}" >
            <div id="edit-dose" class="drop-item"><i>
                <img src="/Content/open-oncology/images/icons/edit-icon.svg" alt="Edit">
                </i> Edit dose
            </div>
            ${renderSameDoseMenuItem()}
            <div id="stop" class="drop-item"><i><svg width="11" height="11" viewBox="0 0 11 11" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M6.42545 5.45455L10.7127 1.16727C10.9745 0.905455 10.9745 0.469091 10.7127 0.207273C10.4509 -0.0545454 10.0145 -0.0545454 9.75273 0.207273L5.45455 4.48364L1.16727 0.196364C0.905455 -0.0654545 0.469091 -0.0654545 0.207273 0.196364C-0.0545454 0.458182 -0.0545454 0.894545 0.207273 1.15636L4.48364 5.45455L0.196364 9.74182C-0.0654545 10.0036 -0.0654545 10.44 0.196364 10.7018C0.327273 10.8327 0.501818 10.8982 0.676364 10.8982C0.850909 10.8982 1.02545 10.8327 1.15636 10.7018L5.45455 6.41455L9.74182 10.7018C9.87273 10.8327 10.0473 10.8982 10.2218 10.8982C10.3964 10.8982 10.5709 10.8327 10.7018 10.7018C10.9636 10.44 10.9636 10.0036 10.7018 9.74182L6.42545 5.45455Z" fill="#C84A4A"/></svg></i> Stop</div>
            <div id="comm" class="drop-item comment"><i class="fas fa-plus-circle"></i> Comment</div>
        </div >
    `;
}

function renderSameDoseMenuItem() {
    if (!expandCssClass) {
        return '<div id="same-dose" class="drop-item"><i><svg width="16" height="9" viewBox="0 0 16 9" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M0.545401 4.09094H13.4127L11.0672 1.75094C10.8533 1.53705 10.8533 1.19028 11.0672 0.976393C11.2811 0.762508 11.6279 0.762508 11.8418 0.976393L15.1145 4.24912C15.1602 4.29576 15.1955 4.35152 15.2181 4.41276C15.2548 4.48153 15.2736 4.55845 15.2727 4.63639V4.69094C15.2694 4.73737 15.2603 4.7832 15.2454 4.8273C15.2485 4.84536 15.2485 4.86379 15.2454 4.88185C15.2209 4.94214 15.1858 4.99756 15.1418 5.04548L11.869 8.31821C11.7666 8.42147 11.6272 8.47956 11.4818 8.47956C11.3363 8.47956 11.1969 8.42147 11.0945 8.31821C10.9912 8.21579 10.9331 8.07638 10.9331 7.93094C10.9331 7.7855 10.9912 7.64608 11.0945 7.54367L13.4127 5.18185H0.545401C0.244154 5.18185 -5.34058e-05 4.93764 -5.34058e-05 4.63639C-5.34058e-05 4.33515 0.244154 4.09094 0.545401 4.09094Z" fill="#00925D"/></svg></i> Same dose</div>';
    } else {
        return '';
    }
}

//Remove icons inside input field
function removeInputIcons(currentInput) {
    currentInput.siblings(".same-dose-icon").remove();
    currentInput.siblings(".stop-dose-icon").remove();
}

///////////
//DROP MENU ON CLICK - Add value inside input field - currentInput = <input>
//////////
function addInputValue(id, currentInput) {
    if (id === '#findings-NF') {
        $('#findings-NF').on('click', function (e) {
            currentInput.val("NF");
            $('#drop-menu').remove();
        });
    } else if (id === '#findings-MSF') {
        $('#findings-MSF').on('click', function (e) {
            currentInput.val("MSF");
            $('#drop-menu').remove();
        });
    } else if (id === '#findings-SF') {
        $('#findings-SF').on('click', function (e) {
            currentInput.val("SF");
            $('#drop-menu').remove();
        });
    } else if (id === '#symtoms-NS') {
        $('#symtoms-NS').on('click', function (e) {
            currentInput.val("NS");
            $('#drop-menu').remove();
        });
    } else if (id === '#symtoms-MS') {
        $('#symtoms-MS').on('click', function (e) {
            currentInput.val("MS");
            $('#drop-menu').remove();
        });
    } else if (id === '#symtoms-SS') {
        $('#symtoms-SS').on('click', function (e) {
            currentInput.val("SS");
            $('#drop-menu').remove();
        });
    } else if (id === '#t-response-CR') {
        $('#t-response-CR').on('click', function (e) {
            currentInput.val("CR");
            $('#drop-menu').remove();
        });
    } else if (id === '#t-response-SD') {
        $('#t-response-SD').on('click', function (e) {
            currentInput.val("SD");
            $('#drop-menu').remove();
        });
    } else if (id === '#t-response-PD') {
        $('#t-response-PD').on('click', function (e) {
            currentInput.val("PD");
            $('#drop-menu').remove();
        });
    }
}

///////////
//DROP MENU ON CLICK - Add icons inside input field - currentInput = <input>
//////////
function addInputIcons(id, currentInput) {
    if (id === '#same-dose') {
        //Add same dose icon in input
        $('#same-dose').on('click', function (e) {
            removeInputIcons(currentInput);
            currentInput.val("");
            currentInput.parent().append('<i class="same-dose-icon"><svg width="21" height="12" viewBox="0 0 21 12" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M0.749987 5.25002H18.4425L15.2175 2.03252C14.9234 1.73843 14.9234 1.26161 15.2175 0.967517C15.5116 0.673425 15.9884 0.673425 16.2825 0.967517L20.7825 5.46752C20.8453 5.53165 20.8939 5.60832 20.925 5.69252C20.9754 5.78708 21.0012 5.89284 21 6.00002V6.07502C20.9955 6.13886 20.9829 6.20187 20.9625 6.26252C20.9667 6.28734 20.9667 6.31269 20.9625 6.33752C20.9288 6.42042 20.8805 6.49662 20.82 6.56252L16.32 11.0625C16.1792 11.2045 15.9875 11.2844 15.7875 11.2844C15.5875 11.2844 15.3958 11.2045 15.255 11.0625C15.113 10.9217 15.0331 10.73 15.0331 10.53C15.0331 10.33 15.113 10.1383 15.255 9.99752L18.4425 6.75002H0.749987C0.335773 6.75002 -1.33514e-05 6.41423 -1.33514e-05 6.00002C-1.33514e-05 5.5858 0.335773 5.25002 0.749987 5.25002Z" fill="#00925D"/></svg></i>');
            $('#drop-menu').remove();
        });
    } else if (id === "#stop") {
        //Add Stop dose icon in input
        $('#stop').on('click', function (e) {
            removeInputIcons(currentInput);
            currentInput.val("");
            currentInput.parent().append('<i class="stop-dose-icon"><svg width="15" height="15" viewBox="0 0 15 15" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M8.835 7.5L14.73 1.605C15.09 1.245 15.09 0.645 14.73 0.285C14.37 -0.075 13.77 -0.075 13.41 0.285L7.5 6.165L1.605 0.27C1.245 -0.09 0.645 -0.09 0.285 0.27C-0.075 0.63 -0.075 1.23 0.285 1.59L6.165 7.5L0.27 13.395C-0.09 13.755 -0.09 14.355 0.27 14.715C0.45 14.895 0.69 14.985 0.93 14.985C1.17 14.985 1.41 14.895 1.59 14.715L7.5 8.82L13.395 14.715C13.575 14.895 13.815 14.985 14.055 14.985C14.295 14.985 14.535 14.895 14.715 14.715C15.075 14.355 15.075 13.755 14.715 13.395L8.835 7.5Z" fill="#C84A4A"/></svg> </i>');
            $('#drop-menu').remove();
            resetCell(currentInput);
        });
    } else if (id === '#comm') {
        //Add Comment icon in input  *** basic funcionality ***
        $('#comm').on('click', function (e) {
            currentInput.parent().append('<div class="input-comment"><svg width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M1.08831 0.195801C0.492821 0.195801 0 0.688623 0 1.28411V7.93654C0 8.53203 0.492821 9.02569 1.08831 9.02569H1.99019V11.1326C1.99021 11.2948 2.08155 11.4432 2.22638 11.5164C2.3712 11.5895 2.54485 11.5749 2.67542 11.4786L6.00164 9.02569H10.67C11.2655 9.02569 11.7583 8.53203 11.7583 7.93654V1.28411C11.7583 0.688622 11.2655 0.195801 10.67 0.195801H1.08831Z" fill="#FE5959"/><path d="M2.5 3.5H9.5" stroke="white" stroke-linecap="square"/><path d="M2.5 5.5H9.5" stroke="white" stroke-linecap="square"/></svg></div>')
            $('#drop-menu').remove();
        });
    } else if (id === '#edit-dose') {
        //Add Comment icon in input  *** basic funcionality ***
        $('#edit-dose').on('click', function (e) {
            removeInputIcons(currentInput);
            showMedicationDoseInstanceModal($(this).closest('.input-field'));
            $('#drop-menu').remove();
        });
    }
}

function addDropMenu(currentInput) {
    const { top, left } = getPosition(currentInput);
    var currentTd = currentInput.parent();

    var dropMenu = renderDropMenu();
    currentTd.append(dropMenu);

    var offset = 2;
    var scrollOffset = 225;
    if (needToScroll(left, 350)) {
        if (isCloseToTheLastDoses(currentTd)) {
            var height = getHeight($("#drop-menu"));
            $("#drop-menu").css({
                "top": top - height - 20,
                "left": left
            });
            return;
        }

        if (!isCloseToTheLastDoses(currentTd, 2)) {
            scrollLeftRight("+=", scrollOffset);
            offset -= scrollOffset;
        }
    }

    var width = getWidth(currentTd);
    $("#drop-menu").css({
        "top": top,
        "left": left + width + offset
    });
}

function isCloseToTheLastDoses(currentTd, threshold = 0) {
    var id = $(currentTd).attr("id");
    return $(`#${id} ~ td`).length <= threshold;
}

function needToScroll(elLeftPosition, threshold) {
    return $(document).width() - elLeftPosition < threshold;
}

function getPosition(el) {
    var position = $(el).offset();
    return {
        left: position.left,
        top: position.top - window.scrollY
    };
}

function getWidth(el) {
    return $(el).width();
}

function getHeight(el) {
    return $(el).height();
}