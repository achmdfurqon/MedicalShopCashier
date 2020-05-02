var orders = null;
var pId = 0;
var sId = 0;
$(function () {
    orders = $("#orderTable").DataTable({
        'responsive': true,
        'ajax': "/Manager/GetPurchases",
        'columns': [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "product.name" },
            {
                data: "date",
                render: function (data) {
                    var createdate = moment(data).format('DD MMMM YYYY, HH:mm');
                    return createdate;
                }
            },
            { data: "quantity" },
            { data: "cost" },
            { data: "supplier.name" }
        ],
        'columnDefs': [
            {
                "orderable": false,
                "targets": [ 0,1,2,3,4,5 ]
            }
        ],
        'order':[ ]
    });    
    var tot = sumtotal();
    
    $.ajax({
        url: "/Manager/ProductAutocomplete",
        type: "GET",
        success: function (result) {
            var data = result.data;
            $('#selectProduct').autocomplete({
                lookup: data,
                onSelect: function (suggestion) {
                    debugger
                    pId = suggestion.data;
                }
            });
        }
    });
    $.ajax({
        url: "/Manager/SupplierAutocomplete",
        type: "GET",
        success: function (result) {
            debugger
            var data = result.data;
            $('#selectSupplier').autocomplete({
                lookup: data,
                onSelect: function (suggestion) {
                    debugger
                    sId = suggestion.data;
                }
            });
        }
    });
});

// Clear Screen Input 
function ClearScreen() {
    document.getElementById("IdText").disabled = true;
    $("#IdText").val('');
    $("#selectProduct").val('');
    $("#selectSupplier").val('');
    $("#QtyText").val('');
    $("#CostText").val('');
}

// Validation Input
function Validation() {
    if ($("#QtyText").val().trim() == "" || $("#CostText").val().trim() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill Empty Field'
        })
    }
    else if ($("#QtyText").val().trim() < 1 || $("#CostText").val().trim() < 1000) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Quantity or Cost is Too Less'
        })
    }
    else if (pId == 0 || sId == 0) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Product or Supplier is Not Found'
        })
    }
    else if ($("#IdText").val() == "") {
        Save();
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill Correctly'
        })
    }
}

// Save 
function Save() {
    debugger;   
    var order = new Object();
    order.ProductId = pId;
    order.SupplierId = sId;
    order.Quantity = $("#QtyText").val();
    order.Cost = $("#CostText").val();
    debugger;
    $.ajax({
        "url": "/Manager/CreatePurchase",
        "type": "POST",
        "dataType": "json",
        "data": order
    }).then((result) => {
        if (result.statusCode == 200) {
            Swal.fire({
                icon: 'success',
                title: 'Your data has been saved',
                text: 'Success!'
            }).then((hasil) => {
                orders.ajax.reload();
                sumtotal();
            });
            $("#Modal").modal("hide");
            pId = 0;
            sId = 0;
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Your data not saved',
                text: 'Failed!'
            })
        }
    })
}

function sumtotal() {
    debugger
    $.ajax({
        "url": "/Manager/GetPurchases",
        "type": "GET",
        "dataType": "json"
    }).then((result) => {
        debugger
        $('#total').text(result.total);
    })
}