var thesaurusId = "";
var _interval;
var valueSelect = null;
var page = 0;
var pageLoad = 0;

function reloadTable() {
    let requestObject = {};
    checkUrlPageParams();
    requestObject.Page = currentPage;
    requestObject.PageSize = getPageSize();
    requestObject.PredefinedType = setPredefinedTypeFromUrl();
    requestObject.PreferredTerm = $("#preferredTerm").val();

    if (!requestObject.Page) {
        requestObject.Page = 1;
    }

    $.ajax({
        type: 'GET',
        url: `/Administration/ReloadThesaurusTable`,
        data: requestObject,
        success: function (data) {
            $("#tableContainer").html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(`Error: ${errorThrown}`);
        }
    });

}

function filterThesaurusData() {
    let urlPageParams = `?page=1&pageSize=${getPageSize()}`;
    let filter = getFilterParametersObject();
    let fullUrlParams = urlPageParams.concat(getFilterUrlParams(filter));

    history.pushState({}, '', fullUrlParams);
    currentPage = 1;
    reloadTable();
}

$(document).ready(function () {
    var item = setPreferedTermFromUrl();
    if (item != null) {
        document.getElementById("preferredTerm").value = item;
        reloadTable();
    }
});

function getFilterParametersObject() {
    var predefinedType = setPredefinedTypeFromUrl();
    var preferredTerm = $("#preferredTerm").val();

    let requestObject = {};
    if (defaultFilter) {
        requestObject = getDefaultFilter();
        defaultFilter = null;
    } else {
        addPropertyToObject(requestObject, 'PredefinedType', predefinedType);
        addPropertyToObject(requestObject, 'PreferredTerm', preferredTerm);
    }

    return requestObject;
}

function setPredefinedTypeFromUrl() {
    if (valueSelect == null) {
        let url = new URL(window.location.href);
        let entries = url.searchParams.entries();
        let params = paramsToObject(entries);
        valueSelect = Object.values(params)[2];

        return valueSelect;
    }
    else
        return valueSelect;
}

$(document).on('click', '.ui-menu-item-wrapper', function (e) {
    filterThesaurusData();
});

$("#preferredTerm").bind('keypress', function (event) {
    clearTimeout(_interval);
    _interval = setTimeout(1000);
    if (event.keyCode === 13) {
        $("#filterSearch").click();
        event.preventDefault();
    }
    event.stopPropagation();
});

$(document).on('keydown', function (e) {
    if (e.keyCode == 13) {
        if (($('#preferredTerm').val()) != null) {
            $('#preferredTerm').autocomplete('close');
            filterThesaurusData();
        }
    }
})

$(document).on('click', '.see-all', function (e) {
    e.stopPropagation();
    e.preventDefault();
    let that = $("#preferredTerm").autocomplete("instance");

    pageLoad += 10;
    var inputValue = $("#preferredTerm").val();
    $.ajax({
        type: "POST",
        url: `/Administration/Autocomplete?searchValue=${inputValue}&page=${pageLoad}`,
        success: function (data, e) {
            let ul = $('.ui-menu').first();
            var dataArr = [];
            for (m = 0; m < data.length; m++)
                dataArr.push({ "label": data[m], "value": data[m] });

            $.each(dataArr, function (index, item) {
                that._renderItemData(ul, item);
                $(ul).find("li").addClass("ui-menu-item");
                $(ul).find("div").addClass("ui-menu-item-wrapper");
            });

            if (data.length == 10) {
                $("<div>", {
                    href: "#",
                    class: "autocomplete-line",
                    id: "lineId"
                }).appendTo($(ul));
                $("<a>", {
                    href: "#",
                    class: "see-all"
                }).html("Load more").appendTo(ul);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            toastr.error(`${thrownError} `);
        }
    });
    this.remove();
    document.getElementById("lineId").remove();
})

$(document).ready(function () {
    $(function () {
        $("#preferredTerm").autocomplete({
            source: function (request, response) {
                var inputValue = $("#preferredTerm").val();
                $.ajax({
                    type: "POST",
                    url: `/Administration/Autocomplete?searchValue=${inputValue}&page=${page}`,
                    success: function (data, e) {
                        pageLoad = 0;
                        response(data)
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        toastr.error(`${thrownError} `);
                    }
                });
            },
            create: function () {
                $(this).data('ui-autocomplete')._renderMenu = function (ul, items) {
                    var that = this;
                    $.each(items, function (index, item) {
                        that._renderItemData(ul, item);
                    });
                }
            },
            open: function () {
                let ul = $('.ui-menu').first();
                var parent = document.getElementsByClassName("ui-menu-item");
                var count = parent.length;
                if (count == 10) {
                    $("<div>", {
                        href: "#",
                        class: "autocomplete-line",
                        id: "lineId"
                    }).appendTo($(ul));
                    $("<a>", {
                        href: "#",
                        class: "see-all"
                    }).html("Load more").appendTo($(ul));
                }
            },
            minLength: 3
        })
    });
})
