var common = {
    init: function () {
        common.registerEvent();
    },
    registerEvent: function () {
        $('.btn-active').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Users/ChangeStatus",
                data: { id: id },
                datatype: "json",
                type: "POST",
                success: function (response) {
                    console.log(response);
                    if (response.status == true) {
                        btn.text('Mở');
                        btn.removeClass('btn-danger').addClass('btn-primary');
                    }
                    else {
                        btn.text('Khoá');
                        btn.removeClass('btn-primary').addClass('btn-danger');
                    }
                }
            });
        });
        $(document).on("click", ".delete-link", function (e) {
            e.preventDefault();
            var id = $(this).data("id");
            var confirmMessage = $(this).data("confirm");

            $("#confirmMessage").text(confirmMessage);
            $("#confirmDelete").data("id", id); // Lưu trữ ID để sử dụng trong xử lý xóa

            $("#confirmModal").modal("show");
        });
        $(document).on("click", "#confirmDelete", function (e) {
            e.preventDefault();
            var id = $(this).data("id");
            //if (confirm($(this).data("confirm"))) {
            $.ajax({
                url: "/Admin/Users/DeleteId/" + id,
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=UTF-8",
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = '/Admin/Users';
                        //$("#getCodeModal").modal("toggle");
                    }
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                }
            });
            //}
        });
        $('#SearchString').on('change', function (event) {
            event.preventDefault();
            var form = $(event.target).parents('form');
            form.submit();
        });

        $("#SearchString").autocomplete({
            minLength: 0,
            source: function (request, response) {
                $.ajax({
                    url: "/Admin/Users/ListName",
                    dataType: "json",
                    data: {
                        q: request.term
                    },
                    success: function (res) {
                        response(res.data);
                    },
                    error: function (response) {
                        alert(response.responseText);
                    }
                });
            },
            focus: function (event, ui) {
                $("#SearchString").val(ui.item.label);
                return false;
            },
            select: function (event, ui) {
                $("#SearchString").val(ui.item.label);
                return false;
            }
        });
      /*  $("#form-search").on("submit", function () {
            event.preventDefault();
            $("#loadingModal").modal('show');

            $.ajax({
                type: 'POST',
                url: '@Url.Action("Index", "Users")',
                data: $(this).serialize(),
                success: function (result) {
                    $('#targetElement').html(result);
                },
                error: function (xhr, status, error) {
                    alert('Có lỗi xảy ra khi tìm kiếm. Vui lòng thử lại.');
                }
            });
            
        });*/

       
        $(function () {
            $('#alertBox').removeClass('hide');
            $('#alertBox').delay(5000).slideUp(500);
        });
        
    }
}
common.init();
function callIndexAction(select) {
    $("#loadingModal").modal('show');
    setTimeout(function () {
        $("#form-search").submit();
    }, 1000);
}

/*$(document).on('click', '.pagination a', function (event) {
    event.preventDefault();
    var page = $(this).attr('data-page');
    var form = $('#form-search');
    var formData = form.serialize() + '&page=' + page;
    $.ajax({
        type: 'POST',
        url: '/Admin/Users/Index',
        data: formData,
        success: function (result) {
            //$('#userTable').html($(result).find('#userTable').html());
        },
        error: function (xhr, status, error) {
            alert('Có lỗi xảy ra khi phân trang. Vui lòng thử lại.');
        }
    });
});*/