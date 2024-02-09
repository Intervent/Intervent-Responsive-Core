    function convertArrayOfObjectsToCSV(args){
    var result, ctr, keys, columnDelimiter, lineDelimiter, data;

        data = args.data || null;
        if (data == null || !data.length) {
            return null;
        }       

        columnDelimiter = args.columnDelimiter || ',';
        lineDelimiter = args.lineDelimiter || '\n';

        keys = Object.keys(data[0]);

        result = '';
        result += 'sep=' + columnDelimiter;
        result += lineDelimiter;
        result += keys.join(columnDelimiter);
        result += lineDelimiter;

        data.forEach(function(item) {
            ctr = 0;
            keys.forEach(function(key) {
                if (ctr > 0)
                    result += columnDelimiter;
                var text = "";
                if(item[key] != null){
                    text = item[key].toString();
                }
                if(text){
                    text = text.replace(/\r\n/g, '');
                    text = text.replace(/,/g, '');
                    text = text.replace(/|/g, '');
                }
                result += text;
                ctr++;
            });
            result += lineDelimiter;
        });

        return result;
    }

    function downloadCSV(downloaddata, filename) {
        var data, filename, link;
        var csv = convertArrayOfObjectsToCSV({
            data: downloaddata
        });
        if (csv == null) {
            $('#download-spinner').parent().prop('disabled', '');
            $('#download-spinner').addClass('hide');
            return;
        }

        filename = filename;

        var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });

        if (navigator.msSaveBlob) { // IE 10+
            navigator.msSaveBlob(blob, filename)
        }
        else {
            var link = document.createElement("a");
            if (link.download !== undefined) {
                // feature detection, Browsers that support HTML5 download attribute
                var url = URL.createObjectURL(blob);
                link.setAttribute("href", url);
                link.setAttribute("download", filename);
                link.style = "visibility:hidden";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                $('#download-spinner').parent().prop('disabled', '');
                $('#download-spinner').addClass('hide');
            }
        }
    }