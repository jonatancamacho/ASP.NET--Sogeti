$(document).ready(function () {
    $(".sort-arrow").each(function() {
        $(this).click(function(event) {
            event.preventDefault();
            setSort(this);
        });
    });

    $("#applyFiltersButton").click(function () {
        toggleUseFilters();


    });

    $("#toggleFiltersButton").click(function () {
        showFilterOptions = !showFilterOptions;

        toggleShowFilters();
    });

    setupDateTimePickers();
    setupFilters();
    toggleUseFilters();
    toggleShowFilters();
});

var currentSortElement, useFilters = false, showFilterOptions = true;

function toggleShowFilters() {
    if (showFilterOptions) {
        $("#toggleFiltersButton").val("Hide");
        $("#filtersBody").slideDown();
    } else {
        $("#toggleFiltersButton").val("Show");
        $("#filtersBody").slideUp()
    }
}

function setSort(element) {
    currentSortElement = element;
    sendFilterAndSortRequest();
}

function toggleUseFilters() {
    useFilters = !useFilters;

    sendFilterAndSortRequest();
    toggleUseFiltersElements();
}


function toggleUseFiltersElements() {
    if (useFilters)
        $("#applyFiltersButton").val("Deactivate filters");
    else
        $("#applyFiltersButton").val("Apply filters");
}

function sendFilterAndSortRequest() {
        
    $.ajax({
        url: "/Log/FilterAndSort",
        data: getFilterAndSortData(),
        success: function (data) {
            var desc = $(currentSortElement).data("desc");
            $("#table-container").html(data);
            $(currentSortElement).data("desc", !desc);
            $(currentSortElement).html(desc ? "&#x25BC;" : "&#x25B2;");
        }
    });
}

function getFilterAndSortData() {

    var sort, desc;

    // Make sure we get proper sort and desc attributes
    if (currentSortElement != undefined) {
        sort = $(currentSortElement).data("col");
        desc = $(currentSortElement).data("desc");
    } else {
        sort = "date";
        desc = true;
    }

    if (useFilters) {
        user = $("#filterUser").val();
        startDate = $("#filterDateStart").find("input").val();
        endDate = $("#filterDateEnd").find("Input").val();
        car = $("#filterCar").val();
        showUncompleted = $("#filterShowUncompleted").is(":checked");
    } else {
        user = "";
        startDate = "";
        endDate = "";
        car = "";
        showUncompleted = false
    }


    return {
        sort: sort,
        desc: desc,
        user: user,
        startDate: startDate,
        endDate: endDate,
        car: car,
        showUncompleted : showUncompleted
    }
}

function setupFilters() {

    // Setup textfields
    var availableTextFieldFilters = [{
        elementId: "#filterCar",
        url: "/Log/Cars",
        displayKey: "RegistrationNumber"
    }, {
        elementId: "#filterUser",
        url: "/Log/Users",
        displayKey: "Email"
    }];

    $.each(availableTextFieldFilters, function (index, value) {
        var hound = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.obj.whitespace(value.displayKey),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            prefetch: {
                url: value.url
            }
        });

        hound.initialize();

        var typeahead = $(value.elementId).typeahead({
            highlight: true,
            minLength: 0
        }, {
            displayKey: value.displayKey,
            source: hound.ttAdapter()
        });

        typeahead.on('typeahead:selected', function (evt, data) {
            if (useFilters)
                sendFilterAndSortRequest();
        });

    });

    // Setup checkboxes
    $("#filterShowUncompleted").change(function () {
        sendFilterAndSortRequest();
    });
}

function setupDateTimePickers() {

    // Makes sure that the date pickers have the first day and last day of the month as default.
    var date = new Date(),
        year = date.getFullYear(),
        month = date.getMonth();
    var firstDay = new Date(year, month, 1);
    var lastDay = new Date(year, month + 1, 0);


    var datepickerSettings = {
        locale: "sv",
        format: "YYYY-MM-DD",
        defaultDate : undefined
    };
   
    var startDateElement = $("#filterDateStart"),
        endDateElement = $("#filterDateEnd"),
        endDateExport = $("#endDateField"),
        startDateExport = $("#startDateField");

    datepickerSettings.defaultDate = firstDay;
    startDateElement.datetimepicker(datepickerSettings);
    startDateElement.on("dp.change", function (e) {
        endDateElement.data("DateTimePicker").minDate(e.date); // Makes sure that we can't add dates before the start date as the end date.
        startDateExport.val(startDateElement.find("input").val());
        if (useFilters)
            sendFilterAndSortRequest();
    })

    datepickerSettings.defaultDate = lastDay;
    endDateElement.datetimepicker(datepickerSettings);
    endDateElement.on("dp.change", function (e) {
        startDateElement.data("DateTimePicker").maxDate(e.date); // Makes sure that we can't add dates after the end date as the start date.
        endDateExport.val(endDateElement.find("input").val());
        if (useFilters)
            sendFilterAndSortRequest();
    })

    startDateExport.val(startDateElement.find("input").val());
    endDateExport.val(endDateElement.find("input").val());
}