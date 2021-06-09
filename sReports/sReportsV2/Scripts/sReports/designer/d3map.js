var world;
var margin = 50;
var width = 975;
var height = 475;

var projection;
var path;

var data = [];
var color;

$(document).ready(function () {
    console.log(d3.geo);
    projection = d3.geoEquirectangular();
        
    path = d3.geoPath(projection);
    color = d3.scaleSequential()
        .domain(d3.extent(Array.from(data.values())))
        .interpolator(d3.interpolateHsl("#000", "#fff"))
        .unknown("#ccc");

    $.ajax({
        method: 'get',
        url: `/FormConsensus/GetMapObject`,
        contentType: 'application/json',
        success: function (data) {
            world = JSON.parse(data);
            console.log(d3);
            drawMap();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            toastr.error(jqXHR.responseText);
        }
    });
});

function drawMap() {
    let countries = topojson.feature(world, world.objects.countries);
    let outline = ({ type: "Sphere" })
    let svg = d3.select(`svg#map`);
    const defs = svg.append("defs");

    defs.append("path")
        .attr("id", "outline")
        .attr("d", path(outline))
        .style("border","unset");

    defs.append("clipPath")
        .attr("id", "clip")
        .append("use")
        .attr("xlink:href", new URL("#outline", window.location.toString()));
    

    const g = svg.append("g")
        .attr("clip-path", `url(${new URL("#clip", window.location.toString())})`);;

    g.append("use")
        .attr("xlink:href", new URL("#outline", window.location.toString()))
        .attr("fill", "white");

    g.append("g")
        .selectAll("path")
        .data(countries.features)
        .enter().append("path")
            .attr("data-country", d => d.properties.name)
            .attr("fill", "#d3d3d3")
            .attr("d", path)
        .append("title")
       .text(d => `${d.properties.name}`);

    g.append("path")
        .datum(topojson.mesh(world, world.objects.countries, (a, b) => a !== b))
        .attr("fill", "none")
        .attr("stroke", "white")
        .attr("stroke-linejoin", "round")
        .attr("d", path);

    svg.append("use")
        .attr("xlink:href", new URL("#outline", window.location.toString()))
        .attr("fill", "none")
        .attr("stroke", "#fff"); //border

    var countriesNames = countries.features.map(function (v) {
        return v.properties.name;
    });

    $("#countriesAutocomplete").autocomplete({
        source: countriesNames,
        select: function (e, ui) {
            e.preventDefault();
            autocompleteSelect(ui.item.value);
        }
    });
}


function autocompleteSelect(country) {
    $(`[data-country="${country}"]`).attr("fill", "#1c94a3");
    $("#countriesAutocomplete").val('');

    let element = document.createElement('div');
    $(element).addClass('country-filter-element');
    $(element).text(country);
    $(element).css("width", "fit-content");
    $(element).attr('data-value', country);

    let imgRemove = document.createElement('img');
    $(imgRemove).attr('src', '../Content/img/icons/Administration_remove.svg');
    $(imgRemove).addClass('remove-country-filter');

    $(element).append(imgRemove);
    $('#filteredCountries').append(element);

    filterOrganizationHierarchy();
}

$(document).on("click", ".remove-country-filter", function () {
    let country = $(this).closest('.country-filter-element').attr('data-value');
    $(`[data-country="${country}"]`).attr("fill", "#d3d3d3");
    $(this).closest('.country-filter-element').remove();

    filterOrganizationHierarchy();
});