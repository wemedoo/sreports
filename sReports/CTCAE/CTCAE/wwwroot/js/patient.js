var chosenItems = [];
var deleteItems = [];
var selectedItem = [];

function submitPatient() {
    var date = $("#date").val();
    var patientId = $("#patientId").val();
    var visitNo = $("#visitNo").val();
    var formInstanceId = $("#formInstanceId").val();
    var organizationRef = $("#organizationRef").val();
    var fromIndex = "yes";

    window.location.href = `/CTCAE/GetPatient?PatientId=${patientId}&Date=${date}&VisitNo=${visitNo}&FromIndex=${fromIndex}&FormInstanceId=${formInstanceId}&OrganizationRef=${organizationRef}`;
}

function backToHome() {
    window.location.href = `/CTCAE/Index`;
}

function submitToReview() {
    var grades = getCheckedTypes();
    var terms = getChosenTerms();
    var codes = getMedraCodes();
    var all = getAllTerms();
    var checkedVal = getCheckedTypes();
    var description1 = getDescription('.description');
    var description2 = getDescription('.description2');
    var description3 = getDescription('.description3');
    var description4 = getDescription('.description4');
    var desc;
    var description = [];
    var count = 0;
    var iterator = 0;

    for (i = 0; i < all.length; i++) {
        if (all[count] != terms[iterator]) {
            count++;
        }
        else {
            if (checkedVal[iterator] === "Grade 1") {
                desc = description1[i];
            }
            else if (checkedVal[iterator] === "Grade 2") {
                desc = description2[i];
            }
            else if (checkedVal[iterator] === "Grade 3") {
                desc = description3[i];
            }
            else {
                desc = description4[i];
            }
            description.push(desc);
            iterator++;
            count++;
        }
    }
    var gradeArray = '';
    var descArray = '';
    var termArray = '';
    var codeArray = '';
    for (i = 0; i < grades.length; i++)
    {
        gradeArray += "Grades=" + grades[i] + "&";
        descArray += "Description=" + description[i] + "&";
        termArray += "Terms=" + terms[i] + "&";
        codeArray += "Codes=" + codes[i] + "&";
    }

    if (checkedVal.length >= 1)
        window.location.href = `/CTCAE/GetReview?${gradeArray}${descArray}${termArray}${codeArray}`;
    else
        toastr.error(`Please choose at least one grade!`);
}

function submitToSymptoms(letter, indicator) {
    var grades = getCheckedTypes();
    var terms = getChosenTerms();
    var selectedValue = $("#selectId").val();
    var checkedLetter = letter;
    var chosen = chosenItems;
    var deleted = deleteItems;
    var all = getAllTerms();
    var checkedVal = getCheckedTypes();
    var description1 = getDescription('.description');
    var description2 = getDescription('.description2');
    var description3 = getDescription('.description3');
    var description4 = getDescription('.description4');
    var desc;
    var count = 0;
    var iterator = 0;

    if (indicator == "indicator") {
        for (i = 0; i < all.length; i++) {
            if (all[count] != terms[iterator]) {
                count++;
            }
            else {
                if (checkedVal[iterator] === "Grade 1") {
                    desc = description1[i];
                }
                else if (checkedVal[iterator] === "Grade 2") {
                    desc = description2[i];
                }
                else if (checkedVal[iterator] === "Grade 3") {
                    desc = description3[i];
                }
                else {
                    desc = description4[i];
                }
                selectedItem.push(desc);
                iterator++;
                count++;
            }
        }
    }
    var gradeArray = '';
    var selectedArray = '';
    var termArray = '';
    for (i = 0; i < grades.length; i++) {
        gradeArray += "Grades=" + grades[i] + "&";
        selectedArray += "SelectedItem=" + selectedItem[i] + "&";
        termArray += "Terms=" + terms[i] + "&";
    }
    var chosenArray = '';
    var deletedArray = '';
    for (i = 0; i < chosen.length; i++)
        chosenArray += "Chosen=" + chosen[i] + "&";
    for (i = 0; i < deleted.length; i++)
        deletedArray += "Deleted=" + deleted[i] + "&";

    window.location.href = `/CTCAE/GetSymptoms?CheckedLetter=${checkedLetter}&SelectedValue=${selectedValue}&${gradeArray}${selectedArray}${termArray}${chosenArray}${deletedArray}`;
}

function submitToAdmin(letter, template) {
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

function submitToSummary() {
    window.location.href = `/CTCAE/GetSummary`;
}

function getAllTerms() {
    var chkArray = [];
    $('.terms').each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function getDescription(description) {
    var chkArray = [];
    $(description).each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function getCheckedTypes() {
    var chkArray = [];

    $(".chk:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function getChosenTerms() {
    var chkArray = [];
    $(".chk:checked").each(function () {
        chkArray.push($(this).data('myval'));
    });

    return chkArray;
}

function getMedraCodes() {
    var chkArray = [];
    $(".chk:checked").each(function () {
        chkArray.push($(this).data('codes'));
    });

    return chkArray;
}

function getCheckedLetter() {
    var chkArray = [];

    $(".chk3:checked").each(function () {
        chkArray.push($(this).val());
    });

    return chkArray;
}

function submitItem() {
    var selectedValue = $("#selectId").val();
    var indicator = $("#selectId").val();
    var chosen = chosenItems;
    var deleted = deleteItems;
    var chosenArray = '';
    var deletedArray = '';
    for (i = 0; i < chosen.length; i++)
        chosenArray += "Chosen=" + chosen[i] + "&";
    for (i = 0; i < deleted.length; i++)
        deletedArray += "Deleted=" + deleted[i] + "&";

    window.location.href = `/CTCAE/GetPatient?${chosenArray}${deletedArray}SelectedValue=${selectedValue}&Indicator=${indicator}`;
}

function submitToList(letter, indicator) {
    var grades = getCheckedTypes();
    var terms = getChosenTerms();
    var selectedValue = $("#selectId").val();
    var checkedLetter = letter;
    var chosen = chosenItems;
    var deleted = deleteItems;
    var all = getAllTerms();
    var checkedVal = getCheckedTypes();
    var description1 = getDescription('.description');
    var description2 = getDescription('.description2');
    var description3 = getDescription('.description3');
    var description4 = getDescription('.description4');
    var desc;
    var count = 0;
    var iterator = 0;

    if (indicator == "indicator") {
        for (i = 0; i < all.length; i++) {
            if (all[count] != terms[iterator]) {
                count++;
            }
            else {
                if (checkedVal[iterator] === "Grade 1") {
                    desc = description1[i];
                }
                else if (checkedVal[iterator] === "Grade 2") {
                    desc = description2[i];
                }
                else if (checkedVal[iterator] === "Grade 3") {
                    desc = description3[i];
                }
                else {
                    desc = description4[i];
                }
                selectedItem.push(desc);
                iterator++;
                count++;
            }
        }
    }
    var gradeArray = '';
    var selectedArray = '';
    var termArray = '';
    for (i = 0; i < grades.length; i++) {
        gradeArray += "Grades=" + grades[i] + "&";
        selectedArray += "SelectedItem=" + selectedItem[i] + "&";
        termArray += "Terms=" + terms[i] + "&";
    }
    var chosenArray = '';
    var deletedArray = '';
    for (i = 0; i < chosen.length; i++)
        chosenArray += "Chosen=" + chosen[i] + "&";
    for (i = 0; i < deleted.length; i++)
        deletedArray += "Deleted=" + deleted[i] + "&";

    window.location.href = `/CTCAE/GetSymptoms?CheckedLetter=${checkedLetter}&SelectedValue=${selectedValue}&${gradeArray}${selectedArray}${termArray}${chosenArray}${deletedArray}`;
}

function reviewEdit() {
    var selectedValue = $("#selectId").val();
    var indicator = $("#selectId").val();

    window.location.href = `/CTCAE/GetPatient?SelectedValue=${selectedValue}&Indicator=${indicator}`;
}

function getOption() {
    var date = $("#date").val();
    var patientId = $("#patientId").val();
    var visitNo = $("#visitNo").val();
    var selectedValue = $("#selectId").val();
    var indicator = $("#selectId").val();
    var selectId = 'id';

    window.location.href = `/CTCAE/GetPatient?PatientId=${patientId}&Date=${date}&VisitNo=${visitNo}&SelectedValue=${selectedValue}&SelectId=${selectId}&Indicator=${indicator}`;
}

function editTemplate() {
    window.location.href = `/CTCAE/TemplateTable`;
}

function backToPatient() {
    window.location.href = `/CTCAE/GetPatient`;
}

function removeTemplate(event) {
    event.stopPropagation();
    var id = document.getElementById("buttonSubmitDelete").getAttribute('data-deleteid')
    window.location.href = `/CTCAE/Delete?TemplateId=${id}`;
}

function showDeleteModal(e, id) {
    e.stopPropagation();
    e.preventDefault();
    document.getElementById("buttonSubmitDelete").setAttribute('data-deleteid', id);
    $('#deleteModal').modal('show');
}

function hideDeleteModal(e) {
    e.stopPropagation();
    e.preventDefault();
    $('#deleteModal').modal('hide');
}

function editEntity(event, id) {
    window.location.href = `/CTCAE/Edit?TemplateId=${id}`;
    event.preventDefault();
}

$(document).on('click', '.rect-indicator', function () {
    $('input[type="radio"]', this).attr('checked', true);
    $(".chk:radio").each(function () {
        if ($(this).is(":checked")) {
            $(this).closest('ul').addClass("rectangle-grade-active");
        } else {
            $(this).closest('ul').removeClass("rectangle-grade-active");
        }
    });
})

$(document).ready(function () {
    $(".chk:radio").each(function () {
        if ($(this).is(":checked")) {
            $(this).closest('ul').addClass("rectangle-grade-active");
        } else {
            $(this).closest('ul').removeClass("rectangle-grade-active");
        }
    });
});