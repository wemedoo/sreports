    function setCustomNumberFields(element) {
        if (element) {
            $(element).attr('data-min', encodeURIComponent($('#min').val()));
            $(element).attr('data-max', encodeURIComponent($('#max').val()));
            $(element).attr('data-step', encodeURIComponent($('#step').val()));

            setCommonStringFields(element);
        }
    }