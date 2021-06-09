function goToBrowse() {
    window.location.href = '/ThesaurusGlobal/Browse'
}

function goToLogin() {
    window.location.href = `/User/Login?ReturnUrl=/`
}

function goToRegistration() {
    window.location.href = `/User/Login?ReturnUrl=/&isLogin=false`
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



