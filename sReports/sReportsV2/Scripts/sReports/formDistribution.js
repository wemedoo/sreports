var dependantElements = [];
/*function submitDistributionConfigForm() {
    $('#distributionForm').validate();
    if ($('#distributionForm').valid()) {
        let postData = {};
        postData['LastUpdate'] = $('#lastUpdate').val();
        postData['Fields'] = getParameters();
        postData['ThesaurusId'] = $('#thesaurusId').val();
        postData['FormDistributionId'] = $('#formDistributionId').val();
        $.ajax({
            type: "POST",
            url: `/FormDistribution/SetParameters`,
            data: postData,
            success: function (data) {
                toastr.success('Success');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
            }
        });
        return false;
    }

}*/

function submitDistributionConfigForm() {
        let postData = {};
        postData['LastUpdate'] = $('#lastUpdate').val();
        postData['Fields'] = getParametersFromData();
        postData['ThesaurusId'] = $('#thesaurusId').val();
        postData['FormDistributionId'] = $('#formDistributionId').val();
        $.ajax({
            type: "POST",
            url: `/FormDistribution/SetParameters`,
            data: postData,
            success: function (data) {
                toastr.success('Success');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error(`Error: ${errorThrown}`);
            }
        });
        return false;

}

function getParametersFromData() {
    
    let fields = {};

    $('.field').find('input').each(function (index, element) {
        let fieldId = $(element).data('id');

        let model = $(element).data('model');
        var my_object = JSON.parse(decodeURIComponent(model));
        setElementValue(element, my_object);

        if (fields[fieldId]) {
            let values = fields[fieldId]['Values'];
            if (values.length == 1 && (!values[0].DependOn || values[0].DependOn.length == 0)) {
                setElementValue(element, values[0]);
            } else {
                let foundEqual = false;
                for (let valueElement of values) {
                    if (arraysEqual(valueElement.DependOn, my_object.DependOn)) {
                        setElementValue(element, valueElement);
                        foundEqual = true;
                        break;
                    }
                }
                if (!foundEqual) {
                    values.push(my_object);
                }
            }
        } else {
            let fieldLabel = $(element).data('label');
            let fieldType = $(element).data('type');
            let field = {
                Values: [],
                Id: fieldId,
                Label: fieldLabel,
                Type: fieldType,
                ThesaurusId: $(element).data('fieldthesaurusid'),
                RelatedVariables: dependantElements[fieldId] ?? JSON.parse(decodeURIComponent($(element).data('relatedvariables')))
            };

            field['Values'].push(my_object);
            fields[fieldId] = field;
        }
});
    let result = [];
    Object.keys(fields).forEach(x => result.push(fields[x]));
    return result;
}

function createNewField(element) {
    let fieldLabel = $(element).data('label');
    let fieldType = $(element).data('type');
    let field = {
        Values: [],
        Id: fieldId,
        Label: fieldLabel,
        Type: fieldType,
        RelatedVariables: dependantElements['fieldId']
    };

    field['Values'].push(my_object);
}

function arraysEqual(a, b) {
    if (a === b) return true;
    if (a == null || b == null) return false;
    if (a.length != b.length) return false;

    // If you don't care about the order of the elements inside
    // the array, you should sort both arrays here.
    // Please note that calling sort on an array will modify that array.
    // you might want to clone your array first.

    for (var i = 0; i < a.length; ++i) {
        if (a[i].Id !== b[i].Id || a[i].Value !== b[i].Value) return false;
    }
    return true;
}

function setElementValue(element, valueForUpdate) {
    if ($(element).data('field') == 'deviation') {
        valueForUpdate.NormalDistributionParameters.Deviation = $(element).val();
    }
    else if ($(element).data('field') == 'mean') {
        valueForUpdate.NormalDistributionParameters.Mean = $(element).val();
    }
    else {
        let thesaurusId = $(element).data('value');
        let value = valueForUpdate.Values.find(x => x.ThesaurusId == thesaurusId);
        value.SuccessProbability = $(element).val();
    }
}
function getParameters() {
    let fields = [];
    $('.field').each(function (index, element) {
        let fieldType = $(element).data('fieldtype');
        let fieldId = $(element).attr('id');
        switch (fieldType) {
            case 'number':
                fields.push(getNumberParameters(fieldId));
                break;
            case 'select':
            case 'radio':
                fields.push(getRadioParameters(fieldId));
                break;
            case 'checkbox':
                fields.push(getCheckboxParameters(fieldId));
                break;
        }
    });
    return fields;
}

function getNumberParameters(id) {
    let result = {};
    result['Id'] = id;
    $('#' + id).find('input').each(function (index, element) {
        let id = $(element).attr('id');
        console.log(id);
        if (id.includes('mean')) {
            result['Mean'] = $(element).val();
        } else if (id.includes('standard_deviation_')) {
            result['StandardDeviation'] = $(element).val();
        }
    });
    console.log(result);
    return result;
}

function getRadioParameters(id) {
    let result = {};
    result['Id'] = id;
    let values = [];
    $(`#${id}`).find('input').each(function (index, element) {
        let value = {};
        value['ThesaurusId'] = $(element).data('thesaurusid');
        value['SuccessProbability'] = $(element).val();
        values.push(value);
    });

    result['Values'] = values;
    console.log(result);
    return result;
}

function getCheckboxParameters(id) {
    let result = {};
    result['Id'] = id;
    let values = [];
    $(`#${id}`).find('input').each(function (index, element) {
        let value = {};
        value['ThesaurusId'] = $(element).data('thesaurusid');
        value['SuccessProbability'] = $(element).val();
        values.push(value);
    });

    result['Values'] = values;
    console.log(result);
    return result;
}


function addRelation(event, id) {
    event.preventDefault();
    $('#targetVariable').val(id);

    $('#relationModal').modal('show');
    //$('#relationModal').modal('show');
    //$('#relationModal').modal('hide');
}

$(document).on('change', '#relation-item', function (e) {
    var selected = $(this).find('option:selected');
    var type = selected.data('type');
    if (type == 'number') {
        $('.boundaries-container').show();

    } else {
        $('.boundaries-container').hide();
    }
});

function createRelatedField() {
    var selected = $('#relation-item').find('option:selected');
    var type = selected.data('type');

    let lowerBoundary = $('#lowerBoundary').val();
    let upperBoundary = $('#upperBoundary').val();
    if (!dependantElements[$('#targetVariable').val()]) {
        dependantElements[$('#targetVariable').val()] = [];
    }
    let dependent = dependantElements[$('#targetVariable').val()];

    let relatedVariable = {
        Id: $(selected).val()
    };

    if (type == 'number') {
        if (lowerBoundary) {
            relatedVariable['LowerBoundary'] = lowerBoundary;
        }
        if (upperBoundary) {
            relatedVariable['UpperBoundary'] = upperBoundary;
        }

    }
    dependent.push(relatedVariable);

    let postData = {
        TargetVariable: $('#targetVariable').val(),
        ThesaurusId: $('#thesaurusId').val(),
        RelatedVariables: dependent
    };

    $.ajax({
        method: 'POST',
        data: postData,
        url: '/FormDistribution/RenderInputsForDependentVariable',
        success: function (data) {
            $(`#${postData.TargetVariable}-probabilities`).html(data);
            $('#relationModal').modal('hide');

        },
        error: function () {

        }
    });
}