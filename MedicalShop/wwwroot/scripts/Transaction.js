
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
                    return '<a style="color:blue;" onclick="Detail(' + row.id + ')"> <i class="fa fa-info fa-lg"></i></a >'
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

var d = null;
function Detail(id) {
    debugger
    $("#Modal").modal("show");
    if (d == null) {
        LoadDetail(id);
    } else {
        d.ajax.url("/Cashier/GetTransactionById/" + id).load();
    }
    //$.ajax({
    //    "url": "/Cashier/GetTransactionById/",
    //    "dataType": "json",
    //    "data": { Id: id },
    //    "success": function (result) {
    //        debugger
    //        $("#tblTransaction").DataTable({
    //            paging: false,
    //            ordering: false,
    //            searching: false,
    //            info: false,
    //            data: result, //.data,
    //            columns: [
    //                {
    //                    data: null,
    //                    render: function (data, type, row, meta) {
    //                        return meta.row + meta.settings._iDisplayStart + 1;
    //                    }
    //                },
    //                { data: "product.name" },
    //                { data: "product.price" },
    //                { data: "quantity" },
    //                { data: "subTotal" }
    //            ]
    //        });
    //    }
    //}) //.then((hasil) => {
    //    $("#tblTransaction").DataTable({
    //        paging: false,
    //        ordering: false,
    //        searching: false,
    //        info: false,
    //        data: hasil,
    //        columns: [
    //            {
    //                data: null,
    //                render: function (data, type, row, meta) {
    //                    return meta.row + meta.settings._iDisplayStart + 1;
    //                }
    //            },
    //            { data: "product.name" },
    //            { data: "product.price" },
    //            { data: "quantity" },
    //            { data: "subTotal" }
    //        ]
    //    });
    //})    
}

function LoadDetail(id) {
    d = $("#tblTransaction").DataTable({
        ajax: "/Cashier/GetTransactionById/" + id,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
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