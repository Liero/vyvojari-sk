/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/bootstrap/dist/js/bootstrap.js" />
/// <reference path="../lib/bootstrap-tagsinput/dist/bootstrap-tagsinput.js" />

var app = {};

(function (app) {
    app.init = function () {
        $(document.body).on('click', '[data-action="show-comments"]', showCommentsClick);

        $("input[data-provider=tagseditor]").tagsinput({
                confirmKeys: [13, 32, 44, 59] //13=enter, 32=space, 44=;59=,
        });
    }


    function showCommentsClick() {
        var footer = $(this).closest(".panel").find('.panel-footer');
        footer.show();
        footer.find('input').focus();
    }

    app.init();
})(app);

