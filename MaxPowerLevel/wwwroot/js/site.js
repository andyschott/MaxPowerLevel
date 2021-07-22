// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function updateCSSFilename(id, filename) {
    let el = $(document.getElementById(id));
    let path_re = new RegExp("^((?:.*/)*)([^/?]*)(\\?.*)?$");
    let url_parts = path_re.exec(el.attr('href'));
    el.attr('href',
        (url_parts[1] || '') + filename + (url_parts[3] || ''));
}

$('.switch-to-dark-mode').click(function() {
    updateCSSFilename('bulma-css', 'bulma-dark.css');
    document.cookie = 'theme=dark; path=/';
});

$('.switch-to-light-mode').click(function() {
    updateCSSFilename('bulma-css', 'bulma-default.css');
    document.cookie = 'theme=light; path=/';
});
