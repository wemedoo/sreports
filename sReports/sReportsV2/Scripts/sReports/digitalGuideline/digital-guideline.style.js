var MIN_WIDTH = 220;

var guidelineStyle = [
        {
            selector: 'node',
            style: {
                'label': 'data(title)',
                "shape": function (element) {
                    let elementData = element.data();
                    let result = 'round-rectangle';
                    switch (elementData.type) {
                        case 'Statement':
                            result = 'round-rectangle';
                            break;
                        case 'Decision':
                            result = 'round-diamond';
                            break;
                        case 'Event':
                            result = 'ellipse';
                            break;
                        default:
                            result = 'round-rectangle';
                            
                            break;
                    }
                    return result;
                },
                //"height": 40,
                "width": function (element) {
                    let data = element.data();
                    if (data.title) {
                        console.log(data.title.length * 5);
                        return data.title.length * 5 > 200 ? data.title.length * 5 : MIN_WIDTH
                    } else {
                        return MIN_WIDTH;
                    }
                },
                'padding': '20',
                "background-color": function (element) {
                    let elementData = element.data();
                    if (elementData.state == "Completed") {
                        return '#0000ff';
                    }
                    else if (elementData.state == "Active") {
                        return '#00ff00';
                    }
                    else {
                        return '#f0f1f1';
                    }
                },
                "color": function (element) {
                    let elementData = element.data();
                    if (elementData.state == "Completed") {
                        return 'white';
                    }
                    else
                        return 'black';
                },
                'border-width': '1px',
                'border-color': 'black',
                'text-halign': 'center',
                'text-valign': 'center',
                'text-max-width': '200',
                'text-wrap': 'wrap',
                'text-overflow-wrap': 'break-word'
            }
        },
        {
            selector: 'node:selected',
            style: {
                'background-color': '#1c94a3',
                'color': 'white'
            }
        },
        /*{
            selector: 'node:active',
            style: {
                'background-color': 'green',
            }
        },*/
        {
            selector: 'node:active',
            style: {
                //'background-color': '#eef6ec',
                //'overlay-color': '#eef6ec'
            }
        },
        {
            selector: 'edge',
            style: {
                'width': '1',
                'label': 'data(title)',
                'text-valign': 'top',
                'curve-style': 'straight',
                //'line-style':'dotted',
                'line-color': 'black',
                'target-arrow-shape': 'triangle', // there are far more options for this property here: http://js.cytoscape.org/#style/edge-arrow,
                'target-arrow-color': 'black'
            }
        },
        {
            selector: '.edge-label-background',
            style: {
                "text-background-opacity": 1,
                'text-background-color': 'white'
            }
        }

    ]