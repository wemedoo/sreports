function goToBrowse() {
    window.location.href = '/ThesaurusGlobal/Browse'
}

function goToLogin() {
    window.location.href = `/User/Login?ReturnUrl=/`
}

function goToRegistration() {
    window.location.href = `/User/Login?ReturnUrl=/&isLogin=false`
}

function goToHomepage() {
    window.location.href = '/ThesaurusGlobal'
}

function goBack(url) {
    window.location.href = url;
}

function manageUsers() {
    window.location.href = '/ThesaurusGlobal/Users';
}

function logout(e) {
    e.preventDefault();
    e.stopPropagation();

    window.location.href = '/User/Logout';

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

function goToCreateThesaurus() {
    window.location.href = '/ThesaurusGlobal/Create'
}

function submitContactFormReCaptcha() {
    $("#formContact").submit();
}

function submitContactForm(form, e) {
    e.preventDefault();
    e.stopPropagation();
    toggleRecaptchaError('hide');
    $(form).validate();
    if ($(form).valid()) {
        $.ajax({
            type: "post",
            data: getContactFormData(form),
            url: `/ThesaurusGlobal/Contact`,
            success: function (data, textStatus, jqXHR) {
                if (data.reCaptchaInputValid) {
                    toastr.success('Contact form is sucessfully submitted');
                    resetContactMessageTextArea();
                    setTimeout(function () { goToHomepage() }, 2000);
                } else {
                    toggleRecaptchaError('show');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

function getContactFormData(form) {
    let contactFormData = {};
    $(form).find(':input').each(function () {
        let attributeName = $(this).attr("name");
        let attributeValue = $(this).val();
        contactFormData[attributeName] = attributeValue;
    });
    return contactFormData;
}

function resetContactMessageTextArea() {
    $("#contactMessage").val("");
}

function toggleRecaptchaError(actionName) {
    if (actionName === 'show') {
        $('#recaptcha-error').show();
    } else {
        $('#recaptcha-error').hide();
    }
}

function showConnectionOntologyModal(event) {
    event.preventDefault();
    event.stopPropagation();
    $("#connectionOntologyModal").modal("show");
}

function addConnectionOntology(event) {
    event.preventDefault();
    event.stopPropagation();
    submitConnectionWithOntology(event);
    $("#connectionOntologyModal").modal("hide");
}