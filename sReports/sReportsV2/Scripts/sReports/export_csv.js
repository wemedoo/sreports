function exportCSV(event) {
    event.stopPropagation();
    event.preventDefault();
    var fd = new FormData(),
        myFile = document.getElementById("file").files[0];
    var title = myFile.name;
    getDocument(`/CSVExport/ExportCSV`, `${title}.csv`);
}

function exportFromUMLS(event) {
    event.stopPropagation();
    event.preventDefault();
    var term = $("#term").val();
    getDocument(`/CSVExport/ExportFromUMLS?Term=${term}`, `${term}.csv`);
}

function getDocument(url, title) {
    $.ajax({
        url: url,
        xhr: function () {
            var xhr = new XMLHttpRequest();
            xhr.responseType = 'blob';
            return xhr;
        },
        success: function (data) {
            const url = window.URL.createObjectURL(data);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = title;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        }
    });
}

function upload() {
    var fd = new FormData(),
        myFile = document.getElementById("file").files[0];

    fd.append('file', myFile);

    $.ajax({
        url: `/CSVExport/SetFirstCSV`,
        data: fd,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(` Error: ${xhr.status} ${thrownError}`);
        }
    });
    return false;
}

function uploadSecond() {
    var fd = new FormData(),
        myFile = document.getElementById("file2").files[0];

    fd.append('file', myFile);

    $.ajax({
        url: `/CSVExport/SetSecondCSV`,
        data: fd,
        processData: false,
        contentType: false,
        type: 'POST',
        success: function (data) {
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(` Error: ${xhr.status} ${thrownError}`);
        }
    });
    return false;
}

document.getElementById("file").onchange = function () {
    document.getElementById("uploadFile").value = this.value.replace("C:\\fakepath\\", "");
    upload();
};

document.getElementById("file2").onchange = function () {
    document.getElementById("uploadFile2").value = this.value.replace("C:\\fakepath\\", "");
    uploadSecond();
};