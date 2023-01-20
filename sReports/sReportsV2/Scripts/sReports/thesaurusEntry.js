var codeValidator;
var closestRow;
var idEdit = null;
var thesaurusPageNum = 0;


$(document).ready(function () {
    if ($('#Id').val()) {
       loadThesaurusTree();
       loadReviewTree();
    }
    $('#collapseFoundIn').collapse('show');
    setValidationCodeFunctions();
});

function loadReviewTree() {
    if ($("#reviewTree").length) {
        let id = $('#Id').val();
        $.ajax({
            type: 'GET',
            url: `/ThesaurusEntry/GetReviewTree?id=${id}`,
            success: function (data) {
                $('#reviewTree').html(data);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
}

function reloadTable() {
    $('#advancedFilterModal').modal('hide');
    setFilterTagsFromUrl();
    setFilterFromUrl();
    let requestObject = getFilterParametersObject();
    setAdvancedFilterBtnStyle(requestObject, ['Id', 'State', 'PreferredTerm', 'page', 'pageSize']);
    requestObject.Page = getPageNum();
    requestObject.PageSize = getPageSize();
    requestObject.IsAscending = isAscending;
    requestObject.ColumnName = columnName;

    $.ajax({
        type: 'GET',
        url: '/ThesaurusEntry/ReloadTable',
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
            addSortArrows();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function loadDocumentProperties(id) {
    if (id != "") {
        $.ajax({
            type: 'GET',
            url: `/Form/GetDocumentProperties?id=${id}`,
            success: function (data) {
                $('#documentPropertiesData').html(data);
                var documentPoperty = document.getElementById("collapseDocumentProperties");
                documentPoperty.classList.add("show");
                var codeElement = document.getElementById("documentArrow");
                resetDocumentArrow(codeElement);
                var element = document.getElementById("documentProperties");
                checkCodeElement(codeElement, element);
            },
            error: function (xhr, textStatus, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
    else
    {
        $('#documentPropertiesData').html("");
        var documentPoperty = document.getElementById("collapseDocumentProperties");
        documentPoperty.classList.remove("show");
        var codeElement = document.getElementById("documentArrow");
        resetDocumentArrow(codeElement);
    }
}

function createThesaurusEntry() {
    window.location.href = "/ThesaurusEntry/Create";
}

function editThesaurusEntry(id) {
    window.location.href = `/ThesaurusEntry/Edit?thesaurusEntryId=${id}`;
}

function editEntity(event, id) {
    editThesaurusEntry(id);
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

$(document).on('click', '.plus-button-synonym', function (e) {
    let tagType = $(e.currentTarget).data("tag");
    let input = $(e.currentTarget).siblings(`input[data-tag=${tagType}]`)[0];
    let inputValue = $(input).val().trim();
    let isAppendend = appendTagToContainer(inputValue, tagType, $(e.currentTarget).data("language"));
    if (isAppendend) {
        $(input).val('');
    }
    return false;
});

$(document).on('click', '.plus-button-similar', function (e) {
    $('#similarTermSource').val('');
    $('#similarTermName').val('');
    $('#similarTermDefinition').val('');
    $('#similarTermEntryDateTime').val('');
    idEdit = null;

    showSimilarTermModal(e);
});

function addNewSimilarTerm(event)
{
    let type = $('#similarTermSource').val();
    let name = $('#similarTermName').val();
    let definition = $('#similarTermDefinition').val();

    if (idEdit) {
        editExistingSimilarElement(name, definition, type);
    }
    else{
        createNemSimilarElement(name, definition, type);
    }

    $('#similarTermSource').val('');
    $('#similarTermName').val('');
    $('#similarTermDefinition').val('');
    $('#similarTermEntryDateTime').val('');

    $('#similarTermModal').modal('hide');

}

function editExistingSimilarElement(name, definition, type) {
    let itemForEdit = $(`#${idEdit}`);
    console.log(itemForEdit);
    $(itemForEdit).attr("data-type", type);
    $(itemForEdit).attr("data-name", name);
    $(itemForEdit).attr("data-definition", definition);
    $(itemForEdit).find('.source').text(type);
    $(itemForEdit).find('.name').text(name);
    $(itemForEdit).find('.definition').text(definition);

    idEdit = null;
}

function createGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
function getElement(name, definition, type) {
    let element = document.createElement('tr');
    $(element).attr('id', createGuid());
    $(element).addClass('tags-element');
    $(element).addClass('tr');
    $(element).attr('data-name', name);
    $(element).attr('data-definition', definition);
    $(element).attr('data-type', type);
    $(element).attr('data-language', $('#selectedLanguage').val());

    return element;
}

function getTdSource(type) {
    let tdSource = document.createElement('td');
    $(tdSource).addClass('custom-td-first');
    $(tdSource).addClass('similar-element');
    $(tdSource).addClass('source');
    $(tdSource).text(type);

    return tdSource;
}

function getTdName(name) {
    let tdName = document.createElement('td');
    $(tdName).addClass('custom-td');
    $(tdName).addClass('similar-element');
    $(tdName).addClass('name');
    $(tdName).text(name);

    return tdName;
}

function getTdDefinition(definition) {
    let tdDefinition = document.createElement('td');
    $(tdDefinition).addClass('custom-td');
    $(tdDefinition).addClass('similar-element');
    $(tdDefinition).addClass('definition');
    $(tdDefinition).text(definition);

    return tdDefinition;
}

function getTdDateTime() {
    let tdDateTime = document.createElement('td');
    $(tdDateTime).addClass('custom-td');
    $(tdDateTime).addClass('similar-element');
    $(tdDateTime).addClass('date-time'); 

    return tdDateTime;
}

function getTdDropdown() {
    let tdDropdown = document.createElement('td');
    $(tdDropdown).addClass('custom-td-last');
    $(tdDropdown).css("padding", "unset");

    return tdDropdown;
}

function getDotsElement() {
    let a = document.createElement('a');
    $(a).addClass('dropdown-button');
    $(a).attr("href", "#");
    $(a).attr("role", "button");
    $(a).attr("data-toggle", "dropdown");
    $(a).attr("aria-haspopup", "true");
    $(a).attr("aria-expanded", "false");

    let img = document.createElement('img');
    $(img).addClass('codes-dots');
    $(img).attr("src", "/Content/img/icons/dots-active.png");

    $(a).append($(img));

    return a;
}

function getDropdownMenuA1() {
    let dropdownMenuA1 = document.createElement('a');
    $(dropdownMenuA1).addClass('dropdown-item');
    $(dropdownMenuA1).addClass('edit-sim');
    $(dropdownMenuA1).attr("href", "#");


    let img1 = document.createElement('img');
    $(img1).addClass("edit-svg-size");
    $(img1).attr("src", "/Content/img/icons/edit.svg");

    $(dropdownMenuA1).append($(img1)).append("Edit");

    return dropdownMenuA1;
}

function getDropdownMenuA2() {
    let dropdownMenuA2 = document.createElement('a');
    $(dropdownMenuA2).addClass('dropdown-item');
    $(dropdownMenuA2).addClass('delete-sim');
    $(dropdownMenuA2).attr("href", "#");


    let img2 = document.createElement('img');
    $(img2).addClass("edit-svg-size");
    $(img2).attr("src", "/Content/img/icons/remove.svg");

    $(dropdownMenuA2).append($(img2)).append("Delete");

    return dropdownMenuA2;
}

function createNemSimilarElement(name, definition, type) {
    let element = getElement(name, definition, type);
    let tdSource = getTdSource(type);
    let tdName = getTdName(name);
    let tdDefinition = getTdDefinition(definition);
    let tdDateTime = getTdDateTime();
    let tdDropdown = getTdDropdown();

    let div = document.createElement('div');
    $(div).addClass('dropdown show');
    $(tdDropdown).append(getDotsElement());

    let dropdownMenu = document.createElement('div');
    $(dropdownMenu).addClass('dropdown-menu');

    let dropdownMenuA1 = getDropdownMenuA1();
    let dropdownMenuA2 = getDropdownMenuA2();
    $(dropdownMenu).append(dropdownMenuA1).append(dropdownMenuA2);
    $(tdDropdown).append(dropdownMenu);


    $(element).append(tdSource).append(tdName).append(tdDefinition).append(tdDateTime).append(tdDropdown);

    $(`#similarTerm-values-${$('#selectedLanguage').val()}`).find('.sim-table-body').append($(element));
}

$(document).on('click', '.similar-element', function () {
    editSimilarTerm(this);
});

function editSimilarTerm(element) {
    idEdit = $(element).closest('.tags-element').attr('id');


    $('#similarTermSource').val($(`#${idEdit}`).attr('data-type'));
    $('#similarTermName').val($(`#${idEdit}`).attr('data-name'));
    $('#similarTermDefinition').val($(`#${idEdit}`).attr('data-definition'));
    $('#similarTermEntryDateTime').val($(`#${idEdit}`).attr('data-entry-date-time'));
    if ($(`#${idEdit}`).attr('data-entry-date-time')) {
        $('#similarTermEntryDateTime').closest('.advanced-filter-item').show();
    } else {
        $('#similarTermEntryDateTime').closest('.advanced-filter-item').hide();
    }
    $('#similarTermModal').modal('show');
}

$(document).on('click', '.remove-similar', function () {
    $(this).closest('.tags-element').remove();
});

function saveAndCreateNew(event) {
    event.preventDefault();
    event.stopPropagation();

    submitThesaurusEntryForm(function () {
        createThesaurusEntry();
    });
}

function submitThesaurusEntryForm(callback) {
    var form = $('#thesaurusEntryForm');
    $(form).validate();
    if ($(form).valid()) {
        let data = {};
        data['id'] = $("#Id").val();
        data['state'] = $('#thesaurusState').val();
        data['translations'] = getFormTranslations($(form).serializeArray());
        data['parentId'] = $('#parentId').val();
        data['umlsDefinitions'] = $('#UmlsDefinitions').html();
        data['umlsName'] = $('#UmlsName').val(); 
        data['umlsCode'] = $('#UmlsCode').val();
        data['codes'] = getCodes();

        $.ajax({
            type: "POST",
            url: `/ThesaurusEntry/Create`,
            data: data,
            success: function (data) {
                toastr.options = {
                    timeOut: 100
                }
                toastr.success("Success");

                if (callback) {
                    callback();
                    return;
                }

                editThesaurusEntry(data.Id);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
    return false;
}

function getCodes() {
    let result = [];
    $('#codeTable').find('tr').each(function (index, element) {
        let codeId = $(element).find('[data-property="codeId"]')[0];
        let codeSystem = $(element).find('[data-property="codeSystem"]')[0];
        let codeVersion = $(element).find('[data-property="codeVersion"]')[0];
        let codeCode = $(element).find('[data-property="codeCode"]')[0];
        let codeValue = $(element).find('[data-property="codeValue"]')[0];
        let codeVersionPublishDate = $(element).find('[data-property="codeVersionPublishDate"]')[0];

        if ($(codeSystem).data('value') && $(codeCode).data('value') && $(codeValue).data('value')) {
            result.push({
                Id: $(codeId).data("value"),
                CodeSystemId: $(codeSystem).data('value'),
                Version: $(codeVersion).data('value'),
                Code: $(codeCode).data('value'),
                Value: $(codeValue).data('value'),
                CodeSystemAbbr: $(codeSystem).data('system'),
                VersionPublishDate: toDateStringIfValue($(codeVersionPublishDate).data('value'))
            });
        }
    });
    return result;
}

function getFormTranslations(data) {
    let result = [];
    languages.forEach(language => {
        let object = {};
        object['language'] = language.value;
        object[`definition`] = data.find(x => x.name === `definition-${language.value}`).value;
        object[`preferredTerm`] = data.find(x => x.name === `preferredTerm-${language.value}`).value;
        object[`synonyms`] = getTagValue('synonym', language.value);
        object[`abbreviations`] = getTagValue('abbreviation', language.value);
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
    var removeIcon = document.createElement('img');
    $(removeIcon).addClass('ml-2');
    $(removeIcon).addClass('tag-value');
    $(removeIcon).addClass('tag-value-synonym');
    $(removeIcon).attr("src", "/Content/img/icons/Administration_remove.svg");
    $(removeIcon).attr("data-id", id);
    $(removeIcon).attr('data-action', `remove-tag`);
    $(removeIcon).attr('data-tag', tagType);
    $(removeIcon).attr('data-language', language);
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
    $(element).addClass('filter-element');
    $(element).addClass('synonyms-element');

    return element;
}

function existsCodeValue(element, selector) {
    return $(element + `:contains(${selector})`).length > 0;
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
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
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

function removeThesaurusEntry(event, id) {
    event.stopPropagation();
    $.ajax({
        type: "DELETE",
        url: `/ThesaurusEntry/Delete?thesaurusEntryId=${id}`,
        success: function (data) {
            $(`#row-${id}`).remove();
        },
        error: function (xhr, textStatus, thrownError) {
            handleResponseError(xhr, thrownError);
        }
    });
}

function getFilterParametersObject() {
    let requestObject = {};
    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'Id', $('#O40MtIdTemp').val());
        addPropertyToObject(requestObject, 'PreferredTerm', $('#PreferredTermTemp').val());
        addPropertyToObject(requestObject, 'Synonym', $('#synonym').val());
        addPropertyToObject(requestObject, 'SimilarTerm', $('#similarTerm').val());
        addPropertyToObject(requestObject, 'Abbreviation', $('#abbreviation').val());
        addPropertyToObject(requestObject, 'UmlsCode', $('#umlsCode').val());
        addPropertyToObject(requestObject, 'UmlsName', $('#umlsName').val());
        addPropertyToObject(requestObject, 'State', $('#StateTemp').val());

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

function showUmlsModal(event) {
    event.stopPropagation();

    $('#umlsModal').modal('show');
}

function showCodeModal(event) {
    event.stopPropagation();

    resetCodeForm();
    closestRow = document.createElement('div');
    showCodeModalTitle("addCodeTitle");
    $('#codeModal').modal('show');
}

function showCodeModalTitle(titleId) {
    $(".code-form-title").hide();
    $(`#${titleId}`).show();
}

function showSimilarTermModal(event) {
    event.stopPropagation();
    if ($('#similarTermEntryDateTime').val()) {
        $('#similarTermEntryDateTime').closest('.advanced-filter-item').show();
    } else {
        $('#similarTermEntryDateTime').closest('.advanced-filter-item').hide();
    }
    $('#similarTermModal').modal('show');
}

function showAdministrativeModal(event) {
    event.stopPropagation();
    $('#administrativeModal').modal('show');
}

function resetCodeForm() {
    resetCodeValues();
    resetValidation();
    resetValidationColor();
}

function resetValidation() {
    if (codeValidator) {
        codeValidator.resetForm();
    }
}

function resetCodeValues() {
    $('#codeSystem').val('');
    $('#codeVersion').val('');
    $('#codeVersionPublishDate').val('');
    $('#codeCode').val('');
    $('#codeValue').val('');
    $('#codeId').val('');
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

    codeValidator = $('#newCodeForm').validate();
    if ($('#newCodeForm').valid()) {
        let codingObject = getNewCode();
        let thesaurusEntryId = $("#Id").val();

        if (thesaurusEntryId) {
            $.ajax({
                type: "post",
                data: codingObject,
                url: `/ThesaurusEntry/CreateCode?thesaurusEntryId=${thesaurusEntryId}`,
                success: function (data, textStatus, jqXHR) {
                    if (codingObject["id"]) {
                        closestRow.replaceWith(data);
                    } else {
                        $('#codeTable tbody').append(data);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    handleResponseError(xhr, thrownError);
                }
            });

            $('#codeModal').modal('hide');
        } else {
            var codeRow = addNewCodeRowToTable(
                codingObject.codeSystemId,
                codingObject.codeSystem,
                codingObject.codeSystem,
                codingObject.version,
                codingObject.code,
                codingObject.value,
                codingObject.versionPublishDate
            );
            $('#codeTable tbody').append(codeRow);
            submitThesaurusEntryForm();
        }
    }
}

function getNewCode() {
    return {
        "id" : $('#codeId').val(),
        "codeSystemId" : $('#codeSystem').val(),
        "codeSystem": $('#codeSystem option:selected').text(),
        "version" : $('#codeVersion').val(),
        "code" : $('#codeCode').val(),
        "value": $('#codeValue').val(),
        "versionPublishDate": toDateStringIfValue($('#codeVersionPublishDate').val())
    }
}

function addNewCodeRowToTable(codeSystemId, codeSystem, codeSystemDisplay, codeVersion, codeCode, codeValue, codeVersionPublishDate) {
    let system = document.createElement('td');
    $(system).attr("data-property", 'codeSystem');
    $(system).attr("data-value", codeSystemId);
    $(system).attr("data-system", codeSystem);
    $(system).html(codeSystemDisplay);
    $(system).addClass("custom-td-first");

    let version = document.createElement('td');
    $(version).attr("data-property", 'codeVersion');
    $(version).attr("data-value", codeVersion);
    $(version).html(codeVersion);
    $(version).addClass("custom-td");

    let code = document.createElement('td');
    $(code).attr("data-property", 'codeCode');
    $(code).attr("data-value", codeCode);
    $(code).html(codeCode);
    $(code).addClass("custom-td");

    let value = document.createElement('td');
    $(value).attr("data-property", 'codeValue');
    $(value).attr("data-value", codeValue);
    $(value).html(codeValue);
    $(value).addClass("custom-td");

    let versionPublishDate = document.createElement('td');
    $(versionPublishDate).attr("data-property", 'codeVersionPublishDate');

    if (codeVersionPublishDate) {
        $(versionPublishDate).attr("data-value", codeVersionPublishDate);
        let formatted_date = $(versionPublishDate).attr('data-value');
        $(versionPublishDate).html(formatted_date);
    } else {
        $(versionPublishDate).attr("data-value", "");
        $(versionPublishDate).html("");
    }

    $(versionPublishDate).addClass("custom-td");

    let del = document.createElement('a');
    $(del).addClass("dropdown-item delete-code");
    $(del).attr("href", '#');
    $(del).append(appendDeleteIcon()).append(deleteItem);

    let edit = document.createElement('a');
    $(edit).addClass("dropdown-item edit-code");
    $(edit).attr("href", '#');
    $(edit).append(appendEditIcon()).append(editItem);

    let dropDownMenu = document.createElement('div');
    $(dropDownMenu).addClass("dropdown-menu");
    $(dropDownMenu).append(edit).append(del);

    let icon = document.createElement('img');
    $(icon).addClass("dots-active");
    $(icon).attr("src", "/Content/img/icons/dots-active.png");

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
    lastTD.style.padding = "unset";
    $(lastTD).addClass("custom-td-last");
    $(lastTD).append(div);

    let coding = document.createElement('tr');
    $(coding).addClass("tr edit-raw");

    $(coding).append(system).append(version).append(code).append(value).append(versionPublishDate).append(lastTD);

    return coding;
}

$(document).on('click', '.dropdown-item.edit-code', function (e) {
    e.stopPropagation();
    e.preventDefault();
    editCodeModal($(this).closest('td'));
});

$(document).on('click', '.tr.edit-raw', function (e) {
    e.stopPropagation();
    e.preventDefault();
    if (!$(e.target).hasClass('dropdown-button') && !$(e.target).hasClass('fa-bars') && !$(e.target).hasClass('dropdown-item')) {
        editCodeModal(e.target);
    }
});

function editCodeModal(elementTd) {
    resetCodeForm();
    setCodeFormValues(elementTd);

    closestRow = $(elementTd).closest('tr');

    showCodeModalTitle("editCodeTitle");
    $('#codeModal').modal('show');
}

function setCodeFormValues(elementTd) {
    $(`#${$(elementTd).data('property')}`).val($(elementTd).data('value'));
    $(elementTd).siblings().each(function () {
        $(`#${$(this).data('property')}`).val($(this).data('value'));
    });
}

$(document).on('click', '.dropdown-item.delete-code', function (e) {
    var codeRow = $(this).closest("tr")
    var codeId = $(codeRow).data("id");
    if (codeId) {
        $.ajax({
            type: "delete",
            url: `/ThesaurusEntry/DeleteCode?codeId=${codeId}`,
            success: function (data, textStatus, jqXHR) {
            },
            error: function (xhr, ajaxOptions, thrownError) {
                handleResponseError(xhr, thrownError);
            }
        });
    }
    $(codeRow).remove();
});

function setValidationCodeFunctions() {
    $.validator.addMethod('codeValid', function (val, element, options) {
        let isValid = true;

        let formObject = {
            codeSystem: $("#codeSystem").val(),
            codeVersion: $("#codeVersion").val(),
            codeCode: $("#codeCode").val(),
            codeValue: $("#codeValue").val()
        }

        let codeId = $("#codeId").val();

        $(`#codeTable > tbody > tr`).each(function () {
            if ($(this).data('id') != codeId) {
                let rowCellEqual = [];
                let rowIsEqual = false;
                $(this).find("td[data-property]").each(function () {
                    let propertyName = $(this).data('property');
                    let propertyValue = $(this).data('value');

                    if (formObject[propertyName]) {
                        rowCellEqual.push(formObject[propertyName] == propertyValue);
                    }
                })
                rowIsEqual = !(rowCellEqual.length < 4 || rowCellEqual.some(x => !x));
                if (rowIsEqual) {
                    isValid = false;
                }
            }
        });

        return isValid;
    },
        "Code already exist!"
    );
    $('[name="codeSystem"]').each(function () {
        $(this).rules('add', {
            codeValid: true
        });
    });
}

function advanceFilter() {
    $('#O40MtIdTemp').val($('#id').val());
    $('#PreferredTermTemp').val($('#preferredTerm').val());
    $('#StateTemp').val($('#state').val());

    filterData();
    //clearFilters();
}

function mainFilter() {
    $('#id').val($('#O40MtIdTemp').val());
    $('#preferredTerm').val($('#PreferredTermTemp').val());
    $('#state').val($('#StateTemp').val());

    filterData();
    //clearFilters();
}

function clearFilters() {
    $('#o40MtId').val('');
    $('#preferredTerm').val('');
    $('#synonym').val('');
    $('#similarTerm').val('');
    $('#abbreviation').val('');
    $('#umlsCode').val('');
    $('#umlsName').val('');
    $('#state').val('');
    $('#PreferredTermTemp').val('');
    $('#O40MtIdTemp').val('');
    $('#StateTemp').val('');
}

$(document).on('click', '#codesTable', function (e) {
    var codeElement = document.getElementById("codesTable");
    var arrowElement = document.getElementById("codesArrow");
    checkArrowClass(arrowElement);
    checkActiveClass(codeElement);
});

$(document).on('click', '#umlsspecificFields', function (e) {
    var codeElement = document.getElementById("umlsspecificFields");
    var arrowElement = document.getElementById("umlsSpecificFieldsArrow");
    checkArrowClass(arrowElement);
    checkActiveClass(codeElement);
});

$(document).on('click', '#collapseO4MTSpecificField', function (e) {
    var codeElement = document.getElementById("collapseO4MTSpecificField");
    var arrowElement = document.getElementById("specificFieldsArrow");
    checkActiveClass(codeElement);
    checkArrowClass(arrowElement);
});

$(document).on('click', '#foundIn', function (e) {
    var codeElement = document.getElementById("foundArrow");
    var element = document.getElementById("foundIn");
    checkCodeElement(codeElement, element);
});

$(document).on('click', '#documentProperties', function (e) {
    var codeElement = document.getElementById("documentArrow");
    var element = document.getElementById("documentProperties");
    checkCodeElement(codeElement, element)
});

$(document).on('click', '#administrativeButton', function (e) {
    var containerWidth = document.getElementById("containerFluid").offsetWidth - 47;
    if ($(document.body)[0].scrollHeight > $(window).height())
        document.getElementById("collapseAdministrativeData").style.width = containerWidth - 30 - 30 + "px";
    else
        document.getElementById("collapseAdministrativeData").style.width = containerWidth - 30 - 30 - 10 + "px";

    var arrowElement = document.getElementById("administrativeArrow");
    if ($(arrowElement).hasClass("administrative-arrow")) {
        arrowElement.classList.remove("administrative-arrow");
        arrowElement.classList.add("administrative-arrow-up");
        $("#collapseAdministrativeData").removeClass("d-none");
        showAdministrativeArrowIfOverflow('collapseAdministrativeData');
    } else {
        arrowElement.classList.remove("administrative-arrow-up");
        arrowElement.classList.add("administrative-arrow");
        $("#collapseAdministrativeData").addClass("d-none");
    }
});

$(document).on('click', '.arrow-scroll-right', function (e) {
    e.preventDefault();
    $('#arrowRight').animate({
        scrollLeft: "+=500px"
    }, "slow");
});

$(document).on('click', '.arrow-scroll-left', function (e) {
    e.preventDefault();
    $('#arrowRight').animate({
        scrollLeft: "-=500px"
    }, "slow");
});

$(document).on('click', '#codeVersionPublishDate', function () {
    $("#codeCalendar").click();
});

function resetDocumentArrow(codeElement) {
    codeElement.classList.remove("arrow-tree");
    codeElement.classList.add("arrow-tree-inactive");
}

function checkArrowClass(arrowElement) {
    if ($(arrowElement).hasClass("administrative-state-arrow-down")) {
        arrowElement.classList.remove("administrative-state-arrow-down");
        arrowElement.classList.add("administrative-state-arrow-up");
    }
    else {
        arrowElement.classList.remove("administrative-state-arrow-up");
        arrowElement.classList.add("administrative-state-arrow-down");
    }
}

function checkActiveClass(codeElement) {
    if ($(codeElement).hasClass("umls-content")) {
        codeElement.classList.remove("umls-content");
        codeElement.classList.add("umls-content-active");
    }
    else {
        codeElement.classList.remove("umls-content-active");
        codeElement.classList.add("umls-content");
    }
}

function checkCodeElement(codeElement, element) {
    if ($(codeElement).hasClass("arrow-tree-inactive")) {
        codeElement.classList.remove("arrow-tree-inactive");
        codeElement.classList.add("arrow-tree");
        element.classList.add("umls-border");
    }
    else {
        codeElement.classList.remove("arrow-tree");
        codeElement.classList.add("arrow-tree-inactive");
        element.classList.remove("umls-border");
    }
}

$(document).on('click', '.dropdown-item.edit-sim', function (e) {
    e.stopPropagation();
    e.preventDefault();
    editSimilarTerm($(e.currentTarget));
});

$(document).on('click', '.dropdown-item.delete-sim', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(e.currentTarget).closest('.tags-element').remove();
});