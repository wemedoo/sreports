function setCustomTextFields(element) {

    if (element) {
        $(element).attr('data-minlength', encodeURIComponent($('#minLength').val()));
        $(element).attr('data-maxlength', encodeURIComponent($('#maxLength').val()));
        setCommonStringFields(element);
    }

}