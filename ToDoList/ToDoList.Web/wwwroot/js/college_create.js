'use strict';

$(document).ready(function () {
    $('#Time').prop('disabled', true);
    $('#checkbox').change(function () {
        if (document.getElementById("checkbox").checked) {
            $('#Time').prop('disabled', false);
        } else {
            $('#Time').prop('disabled', true);
        }
    });
});
