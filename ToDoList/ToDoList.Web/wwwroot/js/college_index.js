$(document).ready(function () {
    $('#TimeButton').click(function () {
        if ($('.Time').hasClass("d-none")) {
            $('.Time').removeClass("d-none");
        }
        else {
            $('.Time').addClass("d-none");
        }
    });
})
