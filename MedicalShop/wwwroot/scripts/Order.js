var orders = null;
var pId = 0;
$(function () {
    orders = $("#orderTable").DataTable({
        'responsive': true,
        'ajax': "/Cashier/GetOrders",
        'columns': [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "product.name" },
            { data: "product.price" },
            { data: "quantity" },
            { data: "subTotal" },
            {
                render: function (data, type, row) {
                    return '<a style="color:#dc3545;" onclick="Delete(' + row.id + ')"> <i class="fa fa-trash fa-lg"></i></a >'
                }
            }
        ],
        'columnDefs': [
            {
                "orderable": false,
                "targets": [ 0,1,2,3,4,5 ]
            }
        ],
        'order':[ ]
    });

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

    var tot = sumtotal(); 
    $("#transactionBox").hide();
});

// Clear Screen Input 
function ClearScreen() {
    document.getElementById("IdText").disabled = true;
    $("#IdText").val('');
    $("#selectProduct").val('');
    $("#QtyText").val('');
}

// Validation Input
function Validation() {
    if ($("#selectProduct").val() == "" || $("#QtyText").val().trim() == "" ) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill Empty Field'
        })
    }
    else if (pId == 0 ) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Product is Not Found'
        })
    }
    else if ($("#IdText").val() == "") {
        Save();
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill Empty Field'
        })
    }
}

// Save 
function Save() {
    debugger
    var order = new Object();
    order.ProductId = pId; //$("#selectProduct").val();
    order.Quantity = $("#QtyText").val();
    debugger
    $.ajax({
        "url": "/Cashier/CreateOrder",
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
                pId = 0;
            });
            $("#Modal").modal("hide");
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

// Delete 
function Delete(id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.value) {
            debugger
            $.ajax({
                "url": "/Cashier/CancelOrder/",
                "dataType": "json",
                "data": { Id: id }
            }).then((hasil) => {
                debugger
                if (hasil.data[0] != 0) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Your data has been deleted',
                        text: 'Deleted!'
                    }).then((result) => {
                        orders.ajax.reload();
                        sumtotal();
                    });
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Your data not deleted',
                        text: 'Failed!'
                    })
                }
            })
        }
    })
}

function sumtotal() {
    debugger
    //var sum = 0;
    //$(".subtotal").each(function () {
    //    sum += parseFloat($(this).text());
    //});
    //$('#total').text(sum);
    $.ajax({
        "url": "/Cashier/GetOrders",
        "type": "GET",
        "dataType": "json"
    }).then((result) => {
        debugger
        $('#total').text(result.total);
        $('#TotalText').val(result.total);
    })
}

function submitTransaction() {
    $("#CashText").val('');
}

function Validate() {
    if ($("#CashText").val().trim() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill Cost Field'
        })
    }
    else if ($("#CashText").val() < $('#TotalText').val()) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Cash is Less than Total Price'
        })
    }
    else {
        AddTransaction();
    }
}

function AddTransaction() {
    var transaction = new Object();
    transaction.Cash = $("#CashText").val();
    transaction.TotalPrice = $("#TotalText").val();
    debugger;
    $.ajax({
        "url": "/Cashier/CreateTransaction",
        "type": "POST",
        "dataType": "json",
        "data": transaction
    }).then((result) => {
        debugger
        if (result.statusCode == 200) {
            Swal.fire({
                icon: 'success',
                title: 'Your data has been saved',
                text: 'Success!'
            }).then((hasil) => {
                $.ajax({
                    "url": "/Cashier/GetTransaction",
                    "type": "GET",
                    "dataType": "json"
                }).then((result) => {
                    debugger
                    //if (result.data != null) {
                    $("#tableTransaction").DataTable({
                        paging: false,
                        ordering: false,
                        searching: false,
                        info: false,
                        data: result.data.sales,
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
                    })
                    debugger
                    $("#tcode").text(result.data.code);
                    $("#tdate").text(moment(result.data.date).format('DD MMMM YYYY, HH:mm'));
                    $("#totalp").text(result.data.totalPrice);
                    $("#cash").text(result.data.cash);
                    $("#change").text(result.data.change);                   
                    //}
                })
                debugger
                $("#ordersBox").hide();
                $("#transactionBox").show();
            });
            $("#tModal").modal("hide");
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

function BackOrders() {
    $("#transactionBox").hide();
    orders.ajax.reload();
    $("#ordersBox").show();
}