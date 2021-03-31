var codeValidator;
var closestRow;

$(document).ready(function () {
    if ($('#O40MTID').val()) {
        loadTree();
    }
});

function reloadTable(initLoad) {
    setFilterFromUrl();
    let requestObject =  getFilterParametersObject();
    requestObject.Page = getPageNum();
    requestObject.PageSize = getPageSize();
    $.ajax({
        type: 'GET',
        url: '/ThesaurusEntry/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

function loadTree() {
    $.ajax({
        type: 'GET',
        url: `/Form/GetDocumentsByThesaurusId?o4MtId=${$('#O40MTID').val()}`,
        success: function (data) {
            $('#treeContainer').html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

function loadDocumentProperties(id) {
    $.ajax({
        type: 'GET',
        url: `/Form/GetDocumentProperties?id=${id}`,
        success: function (data) {
            $('#documentPropertiesData').html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

function createThesaurusEntry() {
    window.location.href = "/ThesaurusEntry/Create";
}

function editEntity(event,id) {
    window.location.href = `/ThesaurusEntry/Edit?thesaurusEntryId=${id}`;
    event.preventDefault();
}

$(document).on("keypress", ".tag-input", function (e) {
    if (e.which === 13) {
        let isAppended = appendTagToContainer($(this).val().trim(), $(e.currentTarget).data("tag"), $(e.currentTarget).data("language"));
        if (isAppended) {
            $(this).val('');
        }
        return false;
    }
});

$(document).on('click', "*[data-action='remove-tag']", function (e) {
    let id = $(e.currentTarget).data("id");
    let tagType = $(e.currentTarget).data("tag");
    let language = $(e.currentTarget).data("language");
    $(`#tag-${id}-${tagType}-${language}`).remove();
});

$(document).on('click', '.button-plus', function (e) {
    let tagType = $(e.currentTarget).data("tag");
    let input = $(e.currentTarget).siblings(`input[data-tag=${tagType}]`)[0];
    let inputValue = $(input).val().trim();
    let isAppendend = appendTagToContainer(inputValue, tagType, $(e.currentTarget).data("language"));
    if (isAppendend) {
        $(input).val('');
    }
    return false;
});

function submitThesaurusEntryForm(form) {
    $('#thesaurusEntryForm').validate();
    if ($(form).valid()) {
        let data = {};
        if ($("#Id").length > 0) {
            data['id'] = $("#Id").val();
        }
        data['o40MtId'] = $('#O40MTID').val();
        data['translations'] = getFormTranslations($(form).serializeArray());
        data['parentId'] = $('#parentId').val();
        data['umlsDefinitions'] = $('#UmlsDefinitions').html();
        data['umlsName'] = $('#UmlsName').val(); 
        data['umlsCode'] = $('#UmlsCode').val();
        data['codes'] = GetCodes();
        data['LastUpdate'] = $('#lastUpdate').val();

        $.ajax({
            type: "POST",
            url: `/ThesaurusEntry/Create`,
            data: data,
            success: function (data) {
                toastr.options = {
                    timeOut: 100
                }
                toastr.options.onHidden = function () { window.location.href = `/ThesaurusEntry/GetAll`; }
                toastr.success("Success");

                $("[name='O40MTID']").val(data);
                $("[name='O40MTID']").closest('.row').removeClass('d-none');
            },
            error: function (xhr, ajaxOptions, thrownError) {
                toastr.error(`${thrownError} `);
            }
        });
    }
    return false;
}

function GetCodes() {
    let result = [];
    $('#codeTable tr').each(function (index, element) {

        let codeSystem = $(element).find('[data-property="codeSystem"]')[0];
        let codeVersion = $(element).find('[data-property="codeVersion"]')[0];
        let codeCode = $(element).find('[data-property="codeCode"]')[0];
        let codeValue = $(element).find('[data-property="codeValue"]')[0];
        let codeVersionPublishDate = $(element).find('[data-property="codeVersionPublishDate"]')[0];

        if ($(codeSystem).data('value') && $(codeVersion).data('value') && $(codeCode).data('value') && $(codeValue).data('value') && $(codeVersionPublishDate).data('value')) {
            result.push({
                System: $(codeSystem).data('value'),
                Version: $(codeVersion).data('value'),
                Code: $(codeCode).data('value'),
                Value: $(codeValue).data('value'),
                VersionPublishDate: new Date($(codeVersionPublishDate).data('value')).toUTCString()
            });
        }
    });
    return result;
}

function getFormTranslations(data) {
    let result = [];
    languages.forEach(language => {
        let object = {};
        object['language'] = language.Value;
        object[`definition`] = data.find(x => x.name === `definition-${language.Value}`).value;
        object[`preferredTerm`] = data.find(x => x.name === `preferredTerm-${language.Value}`).value;
        object[`synonyms`] = getTagValue('synonym', language.Value);
        object[`similarTerms`] = getTagValue('similarTerm', language.Value);
        object[`abbreviations`] = getTagValue('abbreviation', language.Value);
        result.push(object);
    });
    return result;
}

function getTagValue(tagType, language) {
    let result = [];
    $(`*[data-info=tag-value-${tagType}-${language}]`).each(function (index, value) {
        result.push($(this).html());
    });
    return result;
}

function appendTagToContainer(value, tagType, language) {
    if (!value || existsTagValue(value, tagType, language)) {
        return false;
    }
    $(`#${tagType}-values-${language}`).append(createSingleTag(value, tagType, language));
    return true;
}

function createSingleTag(value, tagType, language) {
    var id = value.replace(/\W/g, "-").toLowerCase();
    var removeIcon = getNewRemoveIcon(id, tagType, language);
   
    var element = getNewSignleTagContainer(id, tagType, language);
    $(element).append(getNewSingleTagValue(tagType, language, value));
    $(element).append(removeIcon);

    return element;
}

function getNewRemoveIcon(id, tagType, language) {
    var removeIcon = document.createElement('span');
    $(removeIcon).addClass('remove-tag-button');
    $(removeIcon).attr("data-id", id);
    $(removeIcon).attr('data-action', `remove-tag`);
    $(removeIcon).attr('data-tag', tagType);
    $(removeIcon).attr('data-language', language);
    $(removeIcon).append(getNewFasIcon());
    return removeIcon;
}

function getNewFasIcon() {
    var icon = document.createElement('i');
    $(icon).addClass('fas').addClass('fa-times');
    return icon;
}

function getNewSingleTagValue(tagType, language,value) {
    var text = document.createElement('span');
    $(text).addClass('single-tag-value');
    $(text).attr('data-info', `tag-value-${tagType}-${language}`);
    $(text).html(value);

    return text;
}

function getNewSignleTagContainer(id, tagType, language) {
    var element = document.createElement('div');
    $(element).attr("id", `tag-${id}-${tagType}-${language}`);
    $(element).addClass('single-tag');

    return element;
}

function existsTagValue(value, tagType, language) {
    let id = value.replace(/\W/g, "-").toLowerCase();
    if ($(`#${tagType}-values-${language}`).find(`#tag-${id}-${tagType}-${language}`).length > 0) {
        return true;
    }
    return false;
}

function addNewTranslation() {
    if ($('#newTranslationLanguage').val() && $('#newTranslationValue').val()) {
        $("#translationsContainer").append(getNewTranslationDOM($('#newTranslationLanguage').val(), $('#newTranslationValue').val()));
    }
    console.log($('#newTranslationValue').val());
    console.log($('#newTranslationLanguage').val());
}

function getNewTranslationDOM(language, value) {
    let languageElement = document.createElement('span');
    $(languageElement).addClass('language');
    $(languageElement).html(language);

    let valueElement = document.createElement('span');
    $(valueElement).addClass('value');
    $(valueElement).html(value);

    let translationElement = document.createElement('div');
    $(translationElement).addClass('single-translation');
    $(translationElement).append(languageElement);
    $(translationElement).append(valueElement);

    return translationElement;
}

function selectParent(e, language) {
    if ($(e.srcElement).hasClass('active')) {
        $(e.srcElement).removeClass('active');
        $('#treeStructure').html('');
        $('#parentId').val('');
    } else {
        $(e.srcElement).siblings().removeClass('active');
        $(e.srcElement).addClass('active');
        loadParent($(e.srcElement).data("id"), language);
        $('#parentId').val($(e.srcElement).data("id"));

    }
}

function backToList() {
    window.location.href = "/ThesaurusEntry/GetAll";
}

function loadParent(parentId, language) {
    $.ajax({
        type: "GET",
        url: `/ThesaurusEntry/GetTreeById?thesaurusEntryId=${parentId}`,
        success: function (data) {
            let thesaurusEntry = getThesaurusEntryFromForm(language);
            $('#treeStructure').html(getTreeStructureDOM(data, createTreeElementWithContent(thesaurusEntry, true)));
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });
}

function getThesaurusEntryFromForm(language) {
    let result = null;
    if ($('#Id').val()) {
        result = {
            O40MTId: $('#O40MTID').val(),
            Id: $('#Id').val(),
            Definition: $(`#definition-${language}`).val() ? $(`#definition-${language}`).val() : $(`#definition-en`).val()
        };
    }
    return result;
}

function getTreeStructureDOM(data, child) {
    let element = createTreeElementWithContent(data, false, child);
    let structure = element;
    if (data.Parent) {
        structure = getTreeStructureDOM(data.Parent, element);
    }
    return structure;
}

function createTreeElementWithContent(data, current, child = null) {
    let divText = document.createElement('div');
    $(divText).html(data ? `${data.O40MTId}[${data.Definition ? data.Definition : ''}]` : 'Current Entry');
    $(divText).addClass('tree-item-value');
    if (current) {
        $(divText).addClass('current');
    }

    let element = document.createElement('div');

    $(element).addClass('tree-node');
    if (data) {
        $(element).attr('id', data.id);
    }
    $(element).append(divText);

    if (child) {
        $(element).append(child);
    }

    return element;
}

function removeThesaurusEntry(event, id, lastUpdate) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/ThesaurusEntry/Delete?thesaurusEntryId=${id}&lastUpdate=${lastUpdate}`,
        success: function (data) {
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};
    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'O40MtId', $('#o40MtId').val());
        addPropertyToObject(requestObject, 'PreferredTerm', $('#preferredTerm').val());
        addPropertyToObject(requestObject, 'Synonym', $('#synonym').val());
        addPropertyToObject(requestObject, 'SimilarTerm', $('#similarTerm').val());
        addPropertyToObject(requestObject, 'Abbreviation', $('#abbreviation').val());
        addPropertyToObject(requestObject, 'UmlsCode', $('#umlsCode').val());
        addPropertyToObject(requestObject, 'UmlsName', $('#umlsName').val());
    }

    return requestObject;
}

$(document).on('change', '#selectedLanguage', function () {
    console.log($(this).val());

    $(`#myTabContent #${$(this).val()}`)
        .addClass('show active')
        .siblings()
        .removeClass('show active');
});

function auto_grow(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight+2) + "px";
}

function editCodeThesaurus(event, system, version, code, value, versionPublishDate) {
    $('#codeSystem').val(system);
    $('#codeVersion').val(version);
    $('#codeValue').val(value);
    $('#codeCode').val(code);
    $('#codeVersionPublishDate').val(versionPublishDate);

    $(event.currentTarget).closest('.tr').remove();

    event.stopPropagation();
    $('#codeModal').modal('show');

}
function deleteCode(e) {
    $(e.currentTarget).closest('.tr').remove();
}

function showCodeModal(event) {
    event.stopPropagation();
    if (codeValidator)
    {
        codeValidator.resetForm();
    }

    resetCodeForm();
    closestRow = document.createElement('div');
    $('#forEditing').val('');
    $('#codeModal').modal('show');
}
function resetCodeForm() {
    $('#codeSystem').val('');
    $('#codeVersion').val('');
    $('#codeVersionPublishDate').val('');
    $('#codeCode').val('');
    $('#codeValue').val('');

    resetValidationColor();
}

function resetValidationColor() {
    $('#codeSystem').removeClass('error');
    $('#codeVersion').removeClass('error');
    $('#codeCode').removeClass('error');
    $('#codeValue').removeClass('error');
    $('#codeVersionPublishDate').removeClass('error');
}

function addNewCode(e) {
    e.preventDefault();
    e.stopPropagation();

    codeValidator = $('#newCodeForm').validate(
        {
            rules: {
                codeSystem: {
                    required: true
                },
                codeVersion: {
                    required: true   
                },
                codeValue: {
                    required: true   
                },
                codeCode: {
                    required: true
                },
                bevalid: {
                    codeValid: true
                }
            },
            errorPlacement: function (error, element) {
                if (error[0].innerText == 'Code already exist!') {
                    error.insertBefore($("#bevalid"));
                }
                else {
                    error.insertAfter(element);
                }
            }
        });

    if ($('#newCodeForm').valid()) {

        let system = document.createElement('td');
        $(system).attr("data-property", 'codeSystem');
        $(system).attr("data-value", $('#codeSystem').val());
        $(system).html($('#codeSystem option:selected').text());

        let version = document.createElement('td');
        $(version).attr("data-property", 'codeVersion');
        $(version).attr("data-value", $('#codeVersion').val());
        $(version).html($('#codeVersion').val());

        let code = document.createElement('td');
        $(code).attr("data-property", 'codeCode');
        $(code).attr("data-value", $('#codeCode').val());
        $(code).html($('#codeCode').val());

        let value = document.createElement('td');
        $(value).attr("data-property", 'codeValue');
        $(value).attr("data-value", $('#codeValue').val());
        $(value).html($('#codeValue').val());

        let versionPublishDate = document.createElement('td');
        $(versionPublishDate).attr("data-property", 'codeVersionPublishDate');
        $(versionPublishDate).attr("data-value", $('#codeVersionPublishDate').val());
        let formatted_date = new Date($(versionPublishDate).data('value')).toLocaleDateString();
        $(versionPublishDate).html(formatted_date);

        let del = document.createElement('a');
        $(del).addClass("dropdown-item delete-code");
        $(del).attr("href", '#');
        $(del).html("Delete");

        let edit = document.createElement('a');
        $(edit).addClass("dropdown-item edit-code");
        $(edit).attr("href", '#');
        $(edit).html("Edit");

        let dropDownMenu = document.createElement('div');
        $(dropDownMenu).addClass("dropdown-menu");
        $(dropDownMenu).append(edit).append(del);

        let icon = document.createElement('i');
        $(icon).addClass("fas fa-bars");

        let a = document.createElement('a');
        $(a).addClass("dropdown-button");
        $(a).attr("href", "#");
        $(a).attr("role", "button");
        $(a).attr("data-toggle", "dropdown");
        $(a).attr("aria-haspopup", "true");
        $(a).attr("aria-expanded", "false");
        $(a).append(icon);

        let div = document.createElement('div');
        $(div).addClass("dropdown show");
        $(div).append(dropDownMenu).append(a);

        let lastTD = document.createElement('td');
        $(lastTD).append(div);

        let coding = document.createElement('tr');
        $(coding).addClass("tr edit-raw");

        $(coding).append(system).append(version).append(code).append(value).append(versionPublishDate).append(lastTD);

        if ($('#forEditing').val()) {
            closestRow.replaceWith(coding);
        } else {
            $('#codeTable tbody').append(coding);
        }

        $('#codeModal').modal('hide');
    }
}

$(document).on('click', '.dropdown-item.edit-code', function (e) {
    editCodeSystem($(this).closest('td'));
});

$(document).on('click', '.dropdown-item.edit-code', function (e) {
    editCodeSystem($(this).closest('td'));
});

$(document).on('click', '.tr.edit-raw', function (e) {
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item')) {
        editCodeSystem(e.target);
    }
});

function editCodeSystem(elementTd) {
    $(`#${$(elementTd).data('property')}`).val($(elementTd).data('value'));
    $(elementTd).siblings().each(function () {
        $(`#${$(this).data('property')}`).val($(this).data('value'));
    });

    closestRow = $(elementTd).closest('tr');

    $('#forEditing').val('forEdit');
    if (codeValidator) {
        codeValidator.resetForm();
    }
    resetValidationColor();

    $('#codeModal').modal('show');
}

$(document).on('click', '.dropdown-item.delete-code', function (e) {
    $(this).closest(".tr").remove();
});

$.validator.addMethod('codeValid', function (val, element, options) {
    var isValid = true;
    let system = $("#codeSystem").val();
    let version = $("#codeVersion").val();
    let code = $("#codeCode").val();
    let value = $("#codeValue").val();


    $(`#codeTable > tbody > tr`).each(function () {
        if ($(closestRow).find("td:eq(0)").data('value') == system && $(closestRow).find("td:eq(1)").text() == version && $(closestRow).find("td:eq(2)").text() == code && $(closestRow).find("td:eq(3)").text() == value) {
            isValid = true;
        }
        else {
            if ($(this).find("td:eq(0)").data('value') == system && $(this).find("td:eq(1)").text() == version && $(this).find("td:eq(2)").text() == code && $(this).find("td:eq(3)").text() == value) {
                isValid = false;
            }
        }

    });

    return isValid;
},
    "Code already exist!"
);