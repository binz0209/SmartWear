ClassicEditor
    .create(document.querySelector('#editor'), {
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

                fetch('/Admin/Products/UploadImage', {
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

function callIndexAction(select) {
    $("#loadingModal").modal('show');
    setTimeout(function () {
        $("#form-search").submit();
    }, 1000);
}

ImageFile.onchange = evt => {
    const [file] = ImageFile.files
    if (file) {
        preview.src = URL.createObjectURL(file);
    }
}