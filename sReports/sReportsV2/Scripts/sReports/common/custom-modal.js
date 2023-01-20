$(document).on('click', '.custom-modal-body', function (e) {
    e.stopPropagation();
});

$(document).on('click', '.custom-modal', function (e) {
    closeCustomModal();
});

$(document).on('click', '.close-custom-modal-button', function (e) {
    closeCustomModal();
});

function closeCustomModal() {
    $('.custom-modal').removeClass('show');
    $('body').removeClass('no-scrollable');
    $('.custom-modal').trigger('defaultZIndex');
}