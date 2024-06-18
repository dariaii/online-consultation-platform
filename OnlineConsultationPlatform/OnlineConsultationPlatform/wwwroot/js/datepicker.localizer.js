; (function ($) {
    $.fn.datepicker.language['bg'] = {
        days: ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'],
        daysShort: ['Нд', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
        daysMin: ['Н', 'П', 'В', 'С', 'Ч', 'П', 'С'],
        months: ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'],
        monthsShort: ['Ян', 'Февр', 'Мар', 'Апр', 'Май', 'Юни', 'Юли', 'Авг', 'Септ', 'Окт', 'Ноем', 'Дек'],
        today: 'Днес',
        clear: 'Изчисти',
        dateFormat: 'dd.mm.yyyy',
        timeFormat: 'hh:ii',
        firstDay: 1
    };
})(jQuery);

$('.air-datepicker').datepicker({
    autoClose: true,
    language: 'current',
    dateFormat: 'dd/mm/yyyy',
    today: 'Today',
    language: 'bg',
    timepicker: true,
    minutesStep: 15,
    minDate: new Date(new Date().setMinutes(0))
});
