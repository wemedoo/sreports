var deleteItems = [];
var chosenItems = [];

function submitItem() {
    var selectedValue = $("#selectId").val();
    var indicator = "admin";
    var chosen = chosenItems;
    var deleted = deleteItems;
    var chosenArray = '';
    var deletedArray = '';
    var title = $("#templateTitle").val();

    for (i = 0; i < chosen.length; i++)
        chosenArray += "Chosen=" + chosen[i] + "&";
    for (i = 0; i < deleted.length; i++)
        deletedArray += "Deleted=" + deleted[i] + "&";
    if (title != "")
        window.location.href = `/CTCAE/GetPatient?${chosenArray}${deletedArray}SelectedValue=${selectedValue}&Indicator=${indicator}&Title=${encodeURIComponent(title)}`;
    else
        toastr.error(`Please enter template title!`);
}

function submitToList(letter, template) {
    var checkedLetter = letter;
    var chosen = chosenItems;
    var deleted = deleteItems;
    var indicator = template;
    var title = $("#templateTitle").val();


    var chosenArray = '';
    var deletedArray = '';
    for (i = 0; i < chosen.length; i++)
        chosenArray += "Chosen=" + chosen[i] + "&";
    for (i = 0; i < deleted.length; i++)
        deletedArray += "Deleted=" + deleted[i] + "&";

    window.location.href = `/CTCAE/GetAdmin?CheckedLetter=${checkedLetter}&Indicator=${indicator}&${chosenArray}${deletedArray}&Title=${encodeURIComponent(title)}`;
}