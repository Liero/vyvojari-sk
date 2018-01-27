/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/bootstrap/dist/js/bootstrap.js" />
/// <reference path="../lib/marked/lib/marked.js" />
/// <reference path="../lib/bootstrap-tagsinput/dist/bootstrap-tagsinput.js" />

jQuery.validator.setDefaults({
    errorClass: 'text-danger'
});

var app = {};

(function (app) {
    var markdownTimer;

    app.init = function () {
        $(document.body).on('click', '[data-action="show-comments"]', showCommentsClick);

        $("input[data-provider=tagseditor]").tagsinput({
            confirmKeys: [13, 32, 44, 59], //13=enter, 32=space, 44=;59=,
            tagClass: 'badge badge-secondary'
        });

        $(document.body).on('key-press', '[data-action="submit"]', submitForm)

        //enable markdown formatting on blur and on keypress with delay
        var $markdownInputs = $("[data-provide='markdown']");
        $markdownInputs.blur(function () { formatMarkdown($(this)); });
        $markdownInputs.keypress(function () {
            clearTimeout(markdownTimer);
            markdownTimer = setTimeout(formatMarkdown, 2000, $(this));
        });
    }

    function formatMarkdown($input) {
        clearTimeout(markdownTimer);
      
        $markdown = $(`[aria-labelledby='${$input.attr('id')}']`);
        var content = marked($input.val())
        $markdown.html(content);
    }

    function showCommentsClick() {
        var footer = $(this).closest(".panel").find('.panel-footer');
        footer.show();
        footer.find('input').focus();
    }

    function submitForm() {
        $(this).closest('form').submit();
    }

    app.init();
})(app);

