function reloadPatientTree() {
    let patientId = $("#patientTree").attr("name");
    $.ajax({
        type: "GET",
        url: `/Patient/ReloadTree?patientId=${patientId}`,
        data: {},
        success: function (data) {
            $("#patientTree").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function createDiagnosticReportFromPatient(e, episodeOfCareId, formId, encounterId) {
    let referralParams = [];
    if (referrals) {
        referrals.forEach(x => {
            if (x) {
                referralParams.push(`referrals=${x}`);
            }
        });
    }

    $.ajax({
        type: "GET",
        url: `/DiagnosticReport/CreateFromPatient?encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}&formId=${formId}&${referralParams.join('&')}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
            $("#diagnosticReport").modal("hide");

        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function createNewDocument(e, patientId) {
    e.preventDefault();
    e.stopPropagation();

    let selectedItem = $("div.active");
    let encounterId = "";
    let episodeOfCareId = "";
    if ($(selectedItem).hasClass('form-instance')) {
        encounterId = $(selectedItem).closest(".enc").children(".encounter").attr("id");
        episodeOfCareId = $(selectedItem).closest('.parent').children(".episode-of-care").attr("id");
    }
    if ($(selectedItem).hasClass('encounter')) {
        encounterId = $(selectedItem).attr("id");
        episodeOfCareId = $(selectedItem).closest('.parent').children(".episode-of-care").attr("id");
    }
    if ($(selectedItem).hasClass('episode-of-care')) {
        episodeOfCareId = $(selectedItem).attr("id");
    }

    $.ajax({
        type: "GET",
        url: `/Patient/CreateDocument?patientId=${patientId}&encounterId=${encounterId}&episodeOfCareId=${episodeOfCareId}`,
        data: {},
        success: function (data) {
            $("#diagnosticReportModalContent").html(data);
            $("#diagnosticReport").modal("show");

        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function showAvailableFormsPatientTree(e, episodeOfCareId, encounterId, referralParams) {
   
    $.ajax({
        type: "GET",
        url: `/DiagnosticReport/ModalListForms?episodeOfCareId=${episodeOfCareId}&encounterId=${encounterId}&${referralParams.join('&')}`,
        data: {},
        success: function (data) {
            $("#diagnosticReportModalContent").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.warning(`${thrownError}`);
        }
    });

    // window.location.href = `/DiagnosticReport/ListForms?episodeOfCareId=${episodeOfCareId}&encounterId=${encounterId}`;
}

function addNewEncounter(event,episodeOfCareId)
{
    event.preventDefault();
    event.stopPropagation();
    $.ajax({
        type: "GET",
        url: `/Encounter/CreateFromPatient?episodeOfCareId=${episodeOfCareId}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function addNewEpisodeOfCare(patientId) {
    $.ajax({
        type: "GET",
        url: `/EpisodeOfCare/CreateFromPatient?patientId=${patientId}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

$(document).on('click', '.patient', function (e) {
    let patientId = $(this).attr('id');
    $.ajax({
        type: "GET",
        url: `/Patient/EditPatientInfo?patientId=${patientId}`,
        data: {},
        success: function (data) {
            $("#patientContainer").html(data);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
});

$(document).on('click', '.episode-of-care', function (e) {
    console.log("episode-of-care");
    if (!$(this).hasClass("active")) {
        let eocId = $(this).attr('id');

        $.ajax({
            type: "GET",
            url: `/EpisodeOfCare/EditFromPatient?episodeOfCareId=${eocId}`,
            data: {},
            success: function (data) {
                $("#patientContainer").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });
        setActiveElement(this);
    }
    toggleCollapse($(this).find('.fas').first());
});

$(document).on('click', '.encounter', function (e) {
    if (!$(this).hasClass("active")) {
        let encounterId = $(this).attr('id');

        $.ajax({
            type: "GET",
            url: `/Encounter/EditFromPatient?encounterId=${encounterId}`,
            data: {},
            success: function (data) {
                $("#patientContainer").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });
        setActiveElement(this);
    }
    toggleCollapse($(this).find('.fas').first());
});

$(document).on('click', '.form-instance', function (e) {
    if (!$(this).hasClass("active")) {
        let formInstanceId = $(this).attr('id');

        $.ajax({
            type: "GET",
            url: `/DiagnosticReport/EditFromPatient?formInstanceId=${formInstanceId}`,
            data: {},
            success: function (data) {
                e.preventDefault();
                $("#patientContainer").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });

        setActiveElement(this);
    }
});

$(document).on('click', '.fas', function (e) {
    e.stopPropagation();
    e.preventDefault();
    toggleCollapse(this);
});

$(document).on('change', '#episodes', function (e) {
    console.log($(this).val());
    $(".referral").each(function (index, element) {
        $(element).hide();
    });
    $(".encountersContainer").children().each(function (index, element) {
        $(element).hide();
        if ($(element).hasClass('encounter-active')){
            $(element).removeClass('encounter-active');
        }
    });
    $(`#enc-${$(this).val()}`).show();
    $(`#enc-${$(this).val()}`).addClass('encounter-active');
});

$(document).on('change', '[name^="Encounters-"]', function (e) {
    let episodeOfCareId = $(this).attr("name").split('-')[1];
    let encounterId = $(this).val();
    console.log(episodeOfCareId);
    console.log(encounterId);

    $(".referral").each(function (index, element) {
        $(element).hide();
    });

    $(`#referral-${episodeOfCareId}-${encounterId}`).show();

});

function documentNew(e) {
    e.preventDefault();
    e.stopPropagation();
    let encounterId = $('.encounter-active').children('select').val();
    let referralParams = [];

    $(`[name^="referral-${encounterId}"]:checked`).each(function (index, element) {
        if ($(element).val()) {
            referralParams.push(`referrals=${$(element).val()}`);
        }
    });

    showAvailableFormsPatientTree(e, $("#episodes").val(), encounterId, referralParams);
}

function toggleCollapse(element, setActive) {
    if ($(element).hasClass('fa-minus')) {
        $(element).removeClass('fa-minus').addClass('fa-plus');
    } else {
        $(element).removeClass('fa-plus').addClass('fa-minus');
        hideSiblings(element);
    }

    $(element).closest('li').children('ul').first().toggleClass('expanded');
    if (setActive) {
        $(element).siblings().addClass('active');
    }
}

function hideSiblings(element) {
    let siblings = $(element).closest('li').siblings();
    $.each(siblings, function (index, value) {
        let icon = $(value).find('.fas').first();
        if ($(icon).hasClass('fa-minus')) {
            $(icon).removeClass('fa-minus').addClass('fa-plus');
        }

        $(value).children('ul').removeClass('expanded');
    });
}

function setActiveElement(element) {
    $('.tree-ul .content.active').removeClass('active');
    $(element).addClass('active');
}
