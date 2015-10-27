$(document).ready(function () {
    $("#SelectedCarId").change(function () {
        var end = this.value;
        $.get("/Report/LastRegisteredOdometerStatus", { carId: end }, function (data) {
            $("#OdometerStart").val(data);
        });
    });
});