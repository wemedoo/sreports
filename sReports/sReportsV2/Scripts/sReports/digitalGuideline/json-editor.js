var editorTree;
var editorCode;
var schema;

function showJsonEditor(json = jsonTest) {
    console.log(json);
    var ajv = new Ajv({
        allErrors: true,
        verbose: true,
        schemaId: 'auto'
    });

    if (!editorCode) {
        editorCode = new JSONEditor(document.getElementById('jsoneditorCode'), {
            ajv: ajv,
            mode: 'code',
            onChangeText: function (jsonString) {
                try {
                    //editorTree.updateText(jsonString);
                    console.log(jsonString);
                    //cy.add(JSON.parse(jsonString));
                }
                catch (exception) {
                    console.log(exception);
                }

            },
            onError: function (error) {
                console.log(error);
            }
        });
    }
    editorCode.set(json);
    $('#jsoneditorContainer').show();
}

function getSchema() {
    schema = schemaJson;
}  
