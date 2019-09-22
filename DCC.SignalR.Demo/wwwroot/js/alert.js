"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/AlertHub").build();

$(".loading").hide();

$.fn.dataTable.moment = function (format, locale) {
    var types = $.fn.dataTable.ext.type;
    // Add type detection
    types.detect.unshift(function (d) {
        var dateParse = moment(d);
        var date = moment(dateParse, format, locale, true).isValid() ?
            'moment–' + format :
            null;
        return date;
    });
    // Add sorting method – use an integer for the sorting
    types.order['moment–' + format + '–pre'] = function (d) {
        return moment(d, format, locale, true).unix();
    };
};
$.fn.dataTable.moment('M/D/YYYY hh:m:s');

function formatDate(dateVal) {
    const newDate = new Date(dateVal);

    const sMonth = padValue(newDate.getMonth() + 1);
    const sDay = padValue(newDate.getDate());
    const sYear = newDate.getFullYear();
    var sHour = newDate.getHours();
    const sMinute = padValue(newDate.getMinutes());
    var sAmPm = "AM";

    const iHourCheck = parseInt(sHour);

    if (iHourCheck > 12) {
        sAmPm = "PM";
        sHour = iHourCheck - 12;
    }
    else if (iHourCheck === 0) {
        sHour = "12";
    }

    sHour = padValue(sHour);

    return sMonth + "/" + sDay + "/" + sYear + " " + sHour + ":" + sMinute + " " + sAmPm;
}

function padValue(value) {
    return value < 10 ? `0${value}` : value;
}
function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
}


connection.on("ReceiveMessage",
    function (MemberId, RulesTripped, eTan, timestamp, id) {
        const table = document.getElementById("TableAlerts");
        var value = $('.dataTables_filter input').val();
        if (!isEmptyOrSpaces(value) && value.toLowerCase() !== MemberId.toLowerCase()) {

            return false;
        } else {
            const msg = RulesTripped.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

            const row = table.insertRow(-1);
            const cell1 = row.insertCell(0);
            const cell2 = row.insertCell(1);
            const cell3 = row.insertCell(2);
            const cell4 = row.insertCell(3);
            const cell5 = row.insertCell(4);
            // Add some text to the new cells:
            row.id = id;
            cell1.innerHTML = MemberId;
            cell2.innerHTML = msg;
            cell3.innerHTML = eTan;
            cell4.innerHTML = formatDate(timestamp);
            cell5.innerHTML = "<button class=\"btn btn-primary FraudButton" +
                id +
                "\" data-rowId=" +
                id +
                ">Fraud</button> " +
                " <button class=\"btn btn-primary NonFraudButton" +
                id +
                "\" data-rowId=" +
                id +
                ">Not Fraud</button>";
            $(`.DeleteButton${id}`).click(function (event) {
                const id = this.dataset.rowid;
                connection.invoke("RemoveMessage", id).catch(function (err) {
                    return console.error(err.toString());
                });
                event.preventDefault();
            });
            $(`.NonFraudButton${id}`).click(function (event) {
                const id = this.dataset.rowid;
                connection.invoke("NonFraudMessage", id).catch(function (err) {
                    return console.error(err.toString());
                });
                event.preventDefault();
            });
        }
    });
$(".FraudButton").click(function (event) {
    const id = this.dataset.rowid;
    //marks the message as fraud in DB
    connection.invoke("RemoveMessage", id, true).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
$(".NonFraudButton").click(function (event) {
    const id = this.dataset.rowid;
    //marks the message as not fraud in DB
    connection.invoke("RemoveMessage", id, false).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("DeleteMessage",
    function (id) {
        var table = $("#TableAlerts").DataTable();
        var rows = table.row('tr#' + id).remove().draw();
    });

connection.start().catch(function (err) {
    return console.error(err.toString());
});

async function start() {
    try {
        await connection.start();
        console.log('connected');
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 500);
    }
};

connection.onclose(async () => {
    await start();
});


// Set up the DataTable grid
$('#TableAlerts').DataTable({
    "columnDefs": [
        { "orderable": false, "targets": 1 },
        { "orderable": false, "targets": 2 },
        { "targets": 3, type: 'date' },
        { "orderable": false, "targets": -1 }
    ],
    "order": [[3, 'asc']],
    "pageLength": 50,
    "lengthMenu": [[25, 50, 150, -1], [25, 50, 150, "All"]],
    stateSave: true

});
$('#TableAlerts').on('search.dt', function () {
    var value = $('.dataTables_filter input').val();
    if (isEmptyOrSpaces(value)) {
        location.reload();
        $(".loading").show();
    }
});

$('#btnReset').click(function () {
    $('#StartDateTimePicker').val('');
    $('#EndDateTimePicker').val('');
    $("#DataForm").submit();
});
$.datetimepicker.setLocale('en');

$('#StartDateTimePicker').datetimepicker({
    inline: false,
    format: 'm/d/Y H:i',
    formatTime: 'H:i',
    lang: 'en',
    step: 60
});
$('#EndDateTimePicker').datetimepicker({
    inline: false,
    format: 'm/d/Y H:i',
    formatTime: 'H:i',
    step: 60
}); 

