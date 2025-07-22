ClassicEditor
    .create(document.querySelector('#editor'), {
        extraPlugins: [CustomUploadAdapterPlugin], // Kết nối với chức năng upload ảnh
        plugins: [
            'Essentials', 'Paragraph', 'Heading', 'Bold', 'Italic', 'Underline',
            'Strikethrough', 'SourceEditing', 'Subscript', 'Superscript',
            'Alignment', 'Indent', 'IndentBlock', 'BlockQuote', 'Link', 'Image',
            'ImageCaption', 'ImageStyle', 'ImageToolbar', 'ImageUpload',
            'List', 'MediaEmbed', 'PasteFromOffice', 'Table', 'Highlight',
            'FontFamily', 'FontSize', 'FontColor', 'FontBackgroundColor',
            'GeneralHtmlSupport', 'CodeBlock', 'PageBreak', 'FindAndReplace',
            'HorizontalLine', 'SpecialCharacters'
        ],
        toolbar: [
            'heading', '|', 'bold', 'italic', 'underline', 'strikethrough',
            '|', 'alignment', 'outdent', 'indent', '|', 'link', 'blockQuote',
            'insertTable', 'mediaEmbed', '|', 'fontFamily', 'fontSize',
            'fontColor', 'fontBackgroundColor', '|', 'imageUpload',
            'codeBlock', 'pageBreak', 'findAndReplace', 'horizontalLine',
            'specialCharacters', 'undo', 'redo'
        ],
        image: {
            styles: [
                'full',
                'side'
            ],
            toolbar: [
                'imageStyle:full', 'imageStyle:side', '|', 'imageTextAlternative'
            ]
        }
    })
    .then(editor => {
        const initialData = document.querySelector('#hiddenNcontent').value;
        editor.setData(initialData);
        editor.model.document.on('change:data', () => {
            document.querySelector('#hiddenNcontent').value = editor.getData();
        });
    })
    .catch(error => {
        console.error(error);
    });


function CustomUploadAdapterPlugin(editor) {
    editor.plugins.get('FileRepository').createUploadAdapter = (loader) => {
        return new CustomUploadAdapter(loader);
    };
}
class CustomUploadAdapter {
    constructor(loader) {
        this.loader = loader;
    }

    upload() {
        return this.loader.file
            .then(file => new Promise((resolve, reject) => {
                const data = new FormData();
                data.append('upload', file);

                fetch('?handler=UploadImage', {
                    method: 'POST',
                    body: data,
                })
                    .then(response => response.json())
                    .then(result => {
                        if (result.uploaded) {
                            resolve({
                                default: result.url // Adjust depending on the API response
                            });
                        } else {
                            reject(result.error.message);
                        }
                    })
                    .catch(error => {
                        reject('Upload failed');
                    });
            }));
    }

    abort() {
        // Handle aborting the upload process
    }
}

ClassicEditor
    .create(document.querySelector('#editorSummary'), {
        extraPlugins: [CustomUploadAdapterPlugin],
        plugins: ['Essentials', 'Paragraph', 'Heading', 'Bold',
            'Italic',
            'Underline',
            'Strikethrough',
            'SourceEditing',
            'Subscript',
            'Superscript',
            'Alignment',
            'Indent',
            'IndentBlock',
            'BlockQuote',
            'Link',
            'Image',
            'ImageCaption',
            'ImageStyle',
            'ImageToolbar',
            'ImageUpload',
            'List',
            'MediaEmbed',
            'PasteFromOffice',
            'Table',
            'Highlight',
            'FontFamily',
            'FontSize',
            'FontColor',
            'FontBackgroundColor',
            'GeneralHtmlSupport'],
        image: {
            // Cấu hình các kiểu hình ảnh
            styles: [
                'full',
                'side'
            ],
            toolbar: [
                'imageStyle:full',
                'imageStyle:side',
                '|',
                'imageTextAlternative'
            ]
        },
    })
    .then(editor => {
        const initialData = document.querySelector('#hiddenSummary').value;
        editor.setData(initialData);
        editor.model.document.on('change:data', () => {
            document.querySelector('#hiddenSummary').value = editor.getData();
        });
    })
    .catch(error => {
        console.error(error);
    });

$("#ImageFile").on("change", function (evt) {
    const file = this.files[0];
    if (file) {
        let preview = $("#preview");
        preview.attr("src", URL.createObjectURL(file)); // Hiển thị ảnh xem trước

        // Disable nút Lưu khi bắt đầu upload
        $("#btnSave").prop("disabled", true);

        // Upload ảnh ngay khi chọn
        let formData = new FormData();
        formData.append("Upload", file);

        $.ajax({
            url: "?handler=UploadAvatar", // Razor Pages handler
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success) {
                    $("#AvatarPath").val(data.filePath); // Lưu đường dẫn vào input hidden
                    $("#btnSave").prop("disabled", false);
                } else {
                    alert("Upload thất bại: " + data.error);
                    $("#btnSave").prop("disabled", false);
                }
            },
            error: function () {
                alert("Lỗi khi upload ảnh!");
                $("#btnSave").prop("disabled", false);
            }
        });
    }
});