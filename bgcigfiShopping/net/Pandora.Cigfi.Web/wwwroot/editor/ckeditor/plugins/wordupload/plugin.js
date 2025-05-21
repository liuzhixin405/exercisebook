CKEDITOR.plugins.add('wordupload', {
    icons: 'wordupload',
    init: function (editor) {
        //Plugin logic goes here.
        CKEDITOR.dialog.add('wordupload', this.path + 'dialogs/wordupload.js');
        editor.addCommand('insertWordupload', new CKEDITOR.dialogCommand('wordupload'));
        editor.ui.addButton('wordupload', {
            label: '上传word文件',
            command: 'insertWordupload',
            toolbar: 'insert,100'
        });
    }
});