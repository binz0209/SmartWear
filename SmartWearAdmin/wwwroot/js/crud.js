$(document).ready(function () {
    // Xử lý XÓA danh mục
    $(".delete-btn").on("click", function (event) {
        event.preventDefault();

        var categoryId = $(this).data("id");

        Swal.fire({
            title: "Bạn có chắc chắn?",
            text: "Dữ liệu này sẽ bị xóa vĩnh viễn!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Xóa!",
            cancelButtonText: "Hủy"
        }).then((result) => {
            if (result.isConfirmed) {
                $.post(`?handler=Delete&id=${categoryId}`)
                    .done(function () {
                        Swal.fire("Đã xóa!", "Dữ liệu đã được xóa.", "success")
                            .then(() => location.reload());
                    })
                    .fail(function () {
                        Swal.fire("Lỗi!", "Không thể xóa dữ liệu.", "error");
                    });
            }
        });
    });

    // Xử lý THAY ĐỔI TRẠNG THÁI (Bật/Tắt)
    $(".toggle-status").on("click", function (event) {
        event.preventDefault();

        var categoryId = $(this).data("id");
        var field = $(this).data("field");
        var icon = $(this).find("i");

        // Lưu class icon cũ để khôi phục nếu có lỗi
        var oldClass = icon.attr("class");

        // Hiển thị spinner loading
        icon.attr("class", "fa fa-spinner fa-spin text-secondary");

        // Vô hiệu hóa click khi đang loading
        $(this).css("pointer-events", "none");

        $.post(`?handler=ToggleStatus&id=${categoryId}&field=${field}`)
            .done(function (data) {
                if (data.success) {
                    // Cập nhật icon dựa vào dữ liệu mới từ server
                    if (data.newValue) {
                        icon.attr("class", "fa fa-check-circle text-success");
                    } else {
                        icon.attr("class", "fa fa-times-circle text-danger");
                    }
                } else {
                    Swal.fire("Lỗi!", "Không thể cập nhật trạng thái.", "error");
                    icon.attr("class", oldClass); // Khôi phục icon nếu lỗi
                }
            })
            .fail(function () {
                Swal.fire("Lỗi!", "Không thể cập nhật trạng thái.", "error");
                icon.attr("class", oldClass); // Khôi phục icon nếu lỗi
            })
            .always(function () {
                // Bật lại click sau khi hoàn thành
                $(this).css("pointer-events", "auto");
            }.bind(this)); // Bind lại `this` để tránh lỗi `pointer-events`
    });
});
