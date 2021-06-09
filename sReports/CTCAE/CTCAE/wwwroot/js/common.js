var deleteItems = [];
var chosenItems = [];

function removeSympClass(elem, medDraCode, name) {
    var a = document.getElementById(medDraCode);
    for (i = 0; i < a.length; i++) {
        a[i].classList.add('symptoms-active');
    }
    if (a.classList.contains('symptoms-active')) {
        elem.classList.remove('symptoms-active');
        deleteItems.push(name);
    }
    else {
        elem.classList.add('symptoms-active');
    }
}

function addItemToList(name, code) {
    var id = "medDraCode+" + code;
    var a = document.getElementById(id);

    if (a.classList.contains('class-remove')) {
        for (var i = 0; i < chosenItems.length; i++) {
            if (chosenItems[i] === name) {
                chosenItems.splice(i, 1);
            }
        }
    }
    else {
        chosenItems.push(name);
    }
}

$(document).ready(function () {
    $(".chkbx:checkbox").change(function () {
        $(".chkbx:checkbox").each(function () {
            if ($(this).is(":checked")) {
                $(this).closest('li').addClass("symptoms-active");
                $(this).closest('input').addClass("class-remove");
            } else {
                $(this).closest('li').removeClass("symptoms-active");
                $(this).closest('input').removeClass("class-remove");
            }
        });
    });
});