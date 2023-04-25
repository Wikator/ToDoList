$(document).ready(function () {
    $('#TimeButton').click(function () {
        if ($('.Time').hasClass("d-none")) {
            $('.Time').removeClass("d-none");
        }
        else {
            $('.Time').addClass("d-none");
        }
    });
    $('#GroupButton').click(function () {
        if ($('#GroupTasks').hasClass("d-none")) {
            $('#GroupTasks').removeClass("d-none");
            $('#AllTasks').addClass("d-none");
        }
        else {
            $('#GroupTasks').addClass("d-none");
            $('#AllTasks').removeClass("d-none");
        }
    });
})
