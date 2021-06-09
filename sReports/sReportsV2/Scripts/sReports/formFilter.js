$(document).on('change', '#classes', function () {
    $('#documentClassOtherInput').val('');
    if (this.value === "Other") {
        $('#documentClassOther').removeClass("d-none");
    } else {
        $('#documentClassOther').removeClass("d-block");
        $('#documentClassOther').addClass("d-none");
    }
});

$(document).on('change', '#generalPurpose', function () {
    $('#contextDependent').attr('selectedIndex', 0);
    if (this.value === "ContextDependent") {
        $('#documentContextDependent').removeClass("d-none");
    } else {
        $('#documentContextDependent').removeClass("d-block");
        $('#documentContextDependent').addClass("d-none");
    }
});

$(document).on('change', '#clinicalContext', function () {
    $('#documentFollowUpSelect').attr('selectedIndex', 0);

    if (this.value === "FollowUp") {
        $('#documentFollowUp').removeClass('d-none');
    } else {
        $('#documentFollowUp').removeClass('d-block');
        $('#documentFollowUp').addClass('d-none');
    }
});

function getFilterParametersObject() {
    let requestObject = {};

    if (defaultFilter) {
        result = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Title', encodeURIComponent($('#title').val()));
        addPropertyToObject(requestObject, 'ThesaurusId', $('#thesaurusId').val());
        addPropertyToObject(requestObject, 'State', $('#state').val());
        addPropertyToObject(requestObject, 'Classes', $('#classes').val());
        addPropertyToObject(requestObject, 'ClassesOtherValue', $('#documentClassOtherInput').val());
        addPropertyToObject(requestObject, 'GeneralPurpose', $('#generalPurpose').val());
        addPropertyToObject(requestObject, 'ContextDependent', $('#contextDependent').val());
        addPropertyToObject(requestObject, 'ExplicitPurpose', $('#explicitPurpose').val());
        addPropertyToObject(requestObject, 'ScopeOfValidity', $('#scopeOfValidity').val());
        addPropertyToObject(requestObject, 'ClinicalDomain', $('#clinicalDomain').val());
        addPropertyToObject(requestObject, 'ClinicalContext', $('#clinicalContext').val());
        addPropertyToObject(requestObject, 'FollowUp', $('#documentFollowUpSelect').val());
        addPropertyToObject(requestObject, 'AdministrativeContext', $('#administrativeContext').val());


    }
   
    return requestObject;
}