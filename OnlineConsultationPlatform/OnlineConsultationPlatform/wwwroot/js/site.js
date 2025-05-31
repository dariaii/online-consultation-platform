$(document).on('click', '.sidebar-toggle', function () {
    $(".main-sidebar").toggleClass("is-closed");
});

$(window).on('resize', function () {
    if (window.innerWidth <= 900) {
        $(".main-sidebar").addClass("is-closed");
    }
    else {
        $(".main-sidebar").removeClass("is-closed");
    }
});

$(window).on('load', function () {
    if (window.innerWidth <= 900) {
        $(".main-sidebar").addClass("is-closed");
    }
});

let url = window.location.href;
$('ul#sidebar li a').each(function () {
    if (this.href === url.split('?')[0]) {
        $(this).closest('li').addClass('active');
    }
});

$(window).on('load', function () {
    if ($('#datepickers-container').length > 0) {
        console.log($('#datepickers-container'))
        setTimeout(() => {
            $('#datepickers-container input[name="hours"]').attr('aria-label', 'Select hour');
            $('#datepickers-container input[name="minutes"]').attr('aria-label', 'Select minute');
        }, 100);
    }
});