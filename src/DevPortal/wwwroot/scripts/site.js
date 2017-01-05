/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/bootstrap/dist/js/bootstrap.js" />

var app = {};

(function (app) {
    app.init = function () {
        $(document.body).on('click', '[data-action="show-comments"]', showCommentsClick);
    }


    function showCommentsClick() {
        var footer = $(this).closest(".panel").find('.panel-footer');
        footer.show();
        footer.find('input').focus();
    }

    app.init();
})(app);

