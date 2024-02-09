function DateFormatter(id, format) {
    var date = document.getElementById(id);

    function checkValue(str, max) {
        if (str.charAt(0) !== '0' || str == '00') {
            var num = parseInt(str);
            if (isNaN(num) || num <= 0 || num > max) num = 1;
            str = num > parseInt(max.toString().charAt(0)) && num.toString().length == 1 ? '0' + num : num.toString();
        };
        return str;
    };

    date.addEventListener('input', function (e) {
        this.type = 'text';
        var input = this.value;
        if (/\D\/$/.test(input)) input = input.substr(0, input.length - 3);
        var values = input.split('/').map(function (v) {
            return v.replace(/\D/g, '')
        });
        if (format == "DD/MM/YYYY") {
            if (values[0]) values[0] = checkValue(values[0], 31);
            if (values[1]) values[1] = checkValue(values[1], 12);
        }
        else {
            if (values[0]) values[0] = checkValue(values[0], 12);
            if (values[1]) values[1] = checkValue(values[1], 31);
        }
        var output = values.map(function (v, i) {
            return v.length == 2 && i < 2 ? v + ' / ' : v;
        });
        this.value = output.join('').substr(0, 14);
    });

    date.addEventListener('blur', function (e) {
        this.type = 'text';
        var input = this.value;
        var values = input.split('/').map(function (v, i) {
            return v.replace(/\D/g, '')
        });
        var output = '';

        if (values.length == 3) {
            var year = values[2].length !== 4 ? parseInt(values[2]) + 2000 : parseInt(values[2]);
            var month = "", day = "";
            if (format == "DD/MM/YYYY") {
                day = parseInt(values[0]);
                month = parseInt(values[1]) - 1;
            }
            else {
                month = parseInt(values[0]) - 1;
                day = parseInt(values[1]);
            }
            var d = new Date(year, month, day);

            if (!isNaN(d)) {
                var dates = [d.getMonth() + 1, d.getDate(), d.getFullYear()];
                output = moment(data.Record.Date).format(format);
            };
        };
        this.value = output;

        console.log(this.value);
    });
}

function toSystemDateFormat(date, timeFormat) {
    if (typeof timeFormat != "undefined") {
        if (timeFormat == "12hour")
            return moment(date, $("#userDateFormat").val() + 'hh:mm A').format('YYYY/MM/DD hh:mm A');
        else
            return moment(date, $("#userDateFormat").val() + 'HH:mm').format('YYYY/MM/DD HH:mm');
    }
    else
        return moment(date, $("#userDateFormat").val()).format('YYYY/MM/DD');
}

function toLocalDateFormat(date, timeFormat) {
    if (typeof timeFormat != "undefined") {
        if (timeFormat == "12hour")
            return moment(date).format($("#userDateFormat").val() + ' hh:mm A');
        else
            return moment(date).format($("#userDateFormat").val() + ' HH:mm');
    }
    else
        return moment(date).format($("#userDateFormat").val());
}