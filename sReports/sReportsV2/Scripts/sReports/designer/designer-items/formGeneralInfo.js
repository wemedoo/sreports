$(document).on('click', '#submit-general-form-info', function (e) {

    if ($('#formGeneralInfoForm').valid()) {
        createNewThesaurusIfNotSelected();
        let element = getElement('chapter');
        let title = $('#title').val();
        $(element).attr('data-title', decodeURIComponent(title));
        $(element).attr('data-thesaurusid', decodeURIComponent($('#thesaurusId').val()));
        $(element).attr('data-state', decodeURIComponent($('#state').val()));
        $(element).attr('data-disablepatientdata', decodeURIComponent(!$('#disablePatientData').is(":checked")));
        setFormVersion(element);
        setDocumentProperties(element);
        setEpisodeOfCareProperties(element);

        updateTreeItemTitle(element, title);
        closDesignerFormModal(true);
        clearErrorFromElement($(element).attr('data-id'));

    }
    else {
        toastr.error("Form informations are not valid");
    }
})

$(document).on('blur', '.version', function (e) {
    let element = getElement();
    let activeVersion = getDataProperty(element, 'activeversion');
    let minorVersion = $('#versionMinor').val();
    let majorVersion = $('#versionMajor').val();
    console.log(activeVersion);
    if (activeVersion && Object.keys(activeVersion).length > 2 &&( minorVersion != activeVersion.Minor || majorVersion != activeVersion.Major)) {
        $('#versionWarning').show();
    } else {
        $('#versionWarning').hide();
    }
})

$(document).on('change', '#documentClass', function () {
    $('#documentClassOtherInput').val('');
    if (this.value === "Other") {
        $('#documentClassOther').removeClass("d-none");
    } else {
        console.log("test");
        $('#documentClassOther').removeClass("d-inline-block");
        $('#documentClassOther').addClass("d-none");
    }
});

$(document).on('change', '#generalPurpose', function () {
    $('#contextDependent').attr('selectedIndex', 0);
    if (this.value === "ContextDependent") {
        $('#documentContextDependent').removeClass("d-none");
    } else {
        $('#documentContextDependent').removeClass("d-inline-block");
        $('#documentContextDependent').addClass("d-none");
    }
});

$(document).on('change', '#clinicalContext', function () {
    $('#documentFollowUpSelect').attr('selectedIndex', 0);

    if (this.value === "FollowUp") {
        $('#documentFollowUp').removeClass('d-none');
    } else {
        $('#documentFollowUp').removeClass('d-inline-block');
        $('#documentFollowUp').addClass('d-none');
    }
});

function setFormVersion(element) {
    let activeVersion = getDataProperty(element, 'activeversion');
    let minorVersion = $('#versionMinor').val();
    let majorVersion = $('#versionMajor').val();


    if (minorVersion != activeVersion.Minor || majorVersion != activeVersion.Major) {
        activeVersion.Id = null;
    }
    activeVersion.Minor = minorVersion;
    activeVersion.Major = majorVersion;
    $(element).attr('data-version', encodeURIComponent(JSON.stringify(activeVersion)));
}

function setDocumentProperties(element) {
    let documentProperties = getDataProperty(element, 'documentproperties');
    documentProperties['description'] = $('#description').val();
    setDocumentClass(documentProperties);
    setScopeOfValidity(documentProperties);
    documentProperties['clinicalDomain'] = setClinicalDomain();
    setClinicalContext(documentProperties);
    setAdministrativeContext(documentProperties);
    setDocumentPurpose(documentProperties);

    $(element).attr('data-documentproperties', encodeURIComponent(JSON.stringify(documentProperties)));
}



function setEpisodeOfCareProperties(element) {
    let episodeOfCareData = getDataProperty(element, 'episodeofcare')

    episodeOfCareData['status'] = $('#episodeOfCareStatus').val();
    episodeOfCareData['type'] = $('#episodeOfCareType').val();
    episodeOfCareData['diagnosisRole'] = $('#episodeOfCareDignosisRole').val();
    episodeOfCareData['diagnosisCondition'] = $('#episodeOfCareDiagnosisCondition').val();
    episodeOfCareData['diagnosisRank'] = $('#episodeOfCareDiagnosisRank').val();

    $(element).attr('data-episodeofcare', encodeURIComponent(JSON.stringify(episodeOfCareData)));

}

function setDocumentClass(documentProperties) {
    let documentClass = $('#documentClass').val();
    let documentClassOtherInput = $('#documentClassOtherInput').val();
    if (documentClass) {
        documentProperties['class'] = documentProperties['class'] || {};
        documentProperties['class']['class'] = documentClass;
        documentProperties['class']['other'] = documentClassOtherInput;
    }
    else {
        documentProperties['class'] = null;
    }
}

function setScopeOfValidity(documentProperties) {
    let scopeOfValidity = $('#scopeOfValidity').val();
    let scopeOfValidityValue = $('#scopeOfValidityValue').val();
    if (scopeOfValidity) {
        documentProperties['scopeOfValidity'] = documentProperties['scopeOfValidity'] || {};
        documentProperties['scopeOfValidity']['scopeOfValidity'] = scopeOfValidity;
        documentProperties['scopeOfValidity']['value'] = scopeOfValidityValue;
    }
    else {
        documentProperties['scopeOfValidity'] = null;
    }
}

function setClinicalContext(documentProperties) {
    let clinicalContext = $('#clinicalContext').val();
    let followUp = $('#documentFollowUpSelect').val();

    if (clinicalContext) {
        documentProperties['clinicalContext'] = documentProperties['clinicalContext'] || {};
        documentProperties['clinicalContext']['clinicalContext'] = clinicalContext;
        documentProperties['clinicalContext']['followUp'] = followUp;
    } else {
        documentProperties['clinicalContext'] = null;
    }
}

function setAdministrativeContext(documentProperties) {
    let administrativeContext = $('#administrativeContext').val();

    if (administrativeContext) {
        documentProperties['administrativeContext'] = administrativeContext;
    } else {
        documentProperties['administrativeContext'] = null;
    }
}

function setDocumentPurpose(documentProperties) {
    documentProperties['purpose'] = documentProperties.purpose || {};
    let generalPurpose = $('#generalPurpose').val();
    let contextDependent = $('#contextDependent').val();
    let explicitPurpose = $('#explicitPurpose').val();

    if (generalPurpose || explicitPurpose) {
        documentProperties['purpose'] = documentProperties.purpose || {};
        if (generalPurpose) {
            documentProperties['purpose']['generalPurpose'] = documentProperties['purpose']['generalPurpose'] || {}
            documentProperties['purpose']['generalPurpose']['generalPurpose'] = generalPurpose;
            documentProperties['purpose']['generalPurpose']['contextDependent'] = contextDependent;
        }
        if (explicitPurpose) {
            documentProperties['purpose']['explicitPurpose'] = explicitPurpose
        }
    }
}



