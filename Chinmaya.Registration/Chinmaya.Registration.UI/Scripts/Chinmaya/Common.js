$(".Phone").click(function () {
    $('#changePhone').empty('');
    $.ajax({
        "url": "../Common/EditPhoneNumber",
        "type": "GET",
        success: function (data) {
            $("#changePhone").html(data);
            $("#changePhone").modal();
        },
    });
});

$(".Email").click(function () {
    $('#changeEmail').empty('');
    $.ajax({
        "url": "../Common/EditEmail",
        "type": "GET",
        success: function (data) {
            $("#changeEmail").html(data);
            $("#changeEmail").modal();
        },
    });
});

$(".Address").click(function () {
    $('#changeAddress').empty('');
    $.ajax({
        "url": "../Common/EditAddress",
        "type": "GET",
        success: function (data) {
            $("#changeAddress").html(data);
            $("#changeAddress").modal();
        },
    });
});