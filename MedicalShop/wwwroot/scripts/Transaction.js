
$(function () {
    $("#tblTransactions").DataTable({
        'responsive': true,
        'ajax': "/Cashier/GetTransactions",
        'columns': [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "code" },
            {
                data: "date",
                render: function (data) {
                    return moment(data).format('DD MMMM YYYY, HH:mm');
                }
            },
            { data: "totalPrice" },
            { data: "cash" },
            { data: "change" },
            {
                render: function (data, type, row) {
                    return '<a style="color:#dc3545;" onclick="Detail(' + row.sales + ')"> <i class="fa fa-trash fa-lg"></i></a >'
                }
            }
        ],
        'columnDefs': [
            {
                "orderable": false,
                "targets": [ 0,1,2,3,4,5,6 ]
            }
        ],
        'order':[ ]
    });
});

function Detail(sales) {
    debugger
    $("#Modal").modal("show");
    $("#tblTransaction").DataTable({
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        data: sales,
        columns: [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "product.name" },
            { data: "product.price" },
            { data: "quantity" },
            { data: "subTotal" }
        ]
    });
}