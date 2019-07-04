$(document).ready(function () {
    $('#table').DataTable({
        responsive: true,
        paging: true,
        "bLengthChange": false
    });
    if (hasAccess == true) {
        var banButtton = '<button class="btn dataTool btn-sm btn-primary" data-toggle="modal" data-target="#BanModal">Ban</button>   ';
        var revokeButton = '<button class="btn dataTool btn-sm btn-primary" data-toggle="modal" data-target="#RevokeModal">Revoke</button>';
        $("#table_wrapper .row .col-sm-12:first").append("<div class='mt-1'>" + banButtton + revokeButton + "</div>");
    }
    
});

$("#banForm").submit(function (e) {

    e.preventDefault();

    var form = $(this);
    var url = form.attr('action');

    $.ajax({
        type: "POST",
        url: url,
        data: form.serialize(),
        success: function (data) {
            $.notify({
                icon: 'fas fa-gavel',
                message: data
            }, {
                    type: 'info',
                    allow_dismiss: false,
                    animate: {
                        enter: 'animated fadeInDown',
                        exit: 'animated fadeOutUp'
                    },
                    onShow: null,
                    onShown: null,
                    onClose: null,
                    onClosed: null
            });
            form[0].reset();
        },
        error: function (data) {
            console.log(data);
        }
    });

    $('#BanModal').modal('toggle');
    
});

$("#revokeForm").submit(function (e) {

    e.preventDefault();

    var form = $(this);
    var url = form.attr('action');

    $.ajax({
        type: "DELETE",
        url: url,
        data: form.serialize(),
        success: function (data) {
            $.notify({
                icon: 'fas fa-times-circle',
                message: data
            }, {
                    type: 'info',
                    allow_dismiss: false,
                    animate: {
                        enter: 'animated fadeInDown',
                        exit: 'animated fadeOutUp'
                    },
                    onShow: null,
                    onShown: null,
                    onClose: null,
                    onClosed: null
                });
            form[0].reset();
        },
        error: function (data) {
            console.log(data);
        }
    });

    $('#RevokeModal').modal('toggle');

});
