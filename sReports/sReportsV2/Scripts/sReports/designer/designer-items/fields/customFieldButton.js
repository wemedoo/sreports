$(document).on('change', '#customActionType', function (e) {
    if ($(this).val() == 'ControllerAction') {
        $('.controller-action-container').show();
        $('.javascript-action-container').hide();
    } else if ($(this).val() == 'JavascriptAction') {
        $('.controller-action-container').hide();
        $('.javascript-action-container').show();
    }
});


function setCustomCustomFieldButtonFields(element) {
    if (element) {

        let customAction = {};
        let customActionType = $('#customActionType').val();
        if (customActionType) {
            customAction['actionType'] = customActionType;
            if (customActionType == 'ControllerAction') {
                customAction['actionname'] = $('#actionName').val();
                customAction['controllername'] = $('#controllerName').val();
            } else if (customActionType == 'JavascriptAction') {
                customAction['methodname'] = $('#methodName').val();
            }
            $(element).attr('data-customaction', encodeURIComponent(JSON.stringify(customAction)))
        }
    }

}