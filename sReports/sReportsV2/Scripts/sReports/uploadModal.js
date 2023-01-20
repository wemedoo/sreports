$(document).on("change", ".upload-input", function () {
    if ($(this).val()) {
        $("#uploadSubmit").attr("disabled", false);
        $(".upload-plus-btn").attr("src", "/Content/img/icons/pdf_remove_file.svg");
        $(".upload-plus-btn").addClass('remove-file');
        $(".upload-plus-btn").removeClass('upload-plus-btn');
    } else {
        $("#uploadSubmit").attr("disabled", true);
        $(".remove-file").attr("src", "/Content/img/icons/browse.svg");
        $(".remove-file").addClass('upload-plus-btn');
        $(".remove-file").removeClass('remove-file');
    }
});

$(document).on("click", ".remove-file", function () {
    $("#uploadSubmit").attr("disabled", true);
    $(".upload-input").val("");
    $("#uploadFile").val("");
    $(".remove-file").attr("src", "/Content/img/icons/browse.svg");
    $(".remove-file").addClass('upload-plus-btn');
    $(".remove-file").removeClass('remove-file');
});


$(document).on("click", ".upload-plus-btn", function () {
    $("#file").click();
});