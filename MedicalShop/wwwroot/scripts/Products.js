var p = null;
$(function () {
    p = $("#pTable").DataTable({
        'paging': true,
        'serverSide': true,
        'responsive': true,
        'ajax': "/Products/Data", //+ $('#orderby').val(),
        'columns': [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "name"
            },
            {
                data: "price"
            },
            {
                data: "stock"
            },
            {
                data: "description"
            },
            {
                render: function (data, type, row) {
                    return '<a style="color:#ffc107;" onclick="pGetById(' + row.id + ')"> <i class="fa fa-edit fa-lg"></i></a >' +
                        ' | <a style="color:#dc3545;" onclick="pDelete(' + row.id + ')"> <i class="fa fa-trash fa-lg"></i></a >'
                }
            }
        ],
        'columnDefs': [
            {
                "orderable": false,
                "targets": [ 0,1,2,3,4,5 ]
            },
            {
                "searchable": false,
                "targets": [0, 5]
            }
        ],
        'order':[]
    });
});

//$('#orderby').change(function () {
//    debugger;
//    p.ajax.url('/Product/Data/' + $('#orderby').val()).load();
//});

// Clear Screen Input 
function pClearScreen() {
    document.getElementById("pIdText").disabled = true;
    $("#pIdText").val('');
    $("#pNameText").val('');
    $("#pPriceText").val('');
    $("#pDescText").val('');
    $("#pUpdate").hide();
    $("#pSave").show();
}

// Validation Input
function pValidation() {
    if ($("#pNameText").val().trim() == "" || $("#pPriceText").val().trim() == "" || $("#pDescText").val().trim() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill empty field'
        })
    }
    else if ($("#pIdText").val() == "" || $("#pIdText").val() == " ") {
        pSave();
    }
    else {
        debugger
        pEdit($("#pIdText").val());
    }
}

// Save 
function pSave() {
    debugger;
    var product = new Object();
    product.Name = $("#pNameText").val();
    product.Price = $("#pPriceText").val();
    product.Description = $("#pDescText").val();
    debugger;
    $.ajax({
        "url": "/Admin/CreateProduct",
        "type": "POST",
        "dataType": "json",
        "data": product
    }).then((result) => {
        if (result.statusCode == 200) {
            Swal.fire({
                icon: 'success',
                title: 'Your data has been saved',
                text: 'Success!'
            }).then((hasil) => {
                p.ajax.reload();
            });
            $("#pModal").modal("hide");
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

// Get Id
function pGetById(id) {
    debugger
    $.ajax({
        "url": "/Admin/GetProduct/",
        "type": "GET",
        "dataType": "json",
        "data": { Id: id }
    }).then((result) => {
        debugger
        //if (result.data != null) {
            document.getElementById("pIdText").disabled = true;
            debugger
            $("#pIdText").val(result.data.id);
            $("#pNameText").val(result.data.name);
            $("#pPriceText").val(result.data.price);
            $("#pDescText").val(result.data.description);
            debugger
            $("#pModal").modal("show");
            $("#pUpdate").show();
            $("#pSave").hide();
        //}
    })
}

// Edit 
function pEdit(id) {
    var product = new Object();
    debugger
    product.Id = id;
    product.Name = $("#pNameText").val();
    product.Price = $("#pPriceText").val();
    product.Description = $("#pDescText").val();
    $.ajax({
        "url": "/Admin/EditProduct/",
        "type": "POST",
        "dataType": "json",
        "data": { id: product.Id, product: product }
    }).then((result) => {
        if (result.statusCode == 200) {
            $("#pModal").modal("hide");
            Swal.fire({
                icon: 'success',
                title: 'Your data has been updated',
                text: 'Success!'
            }).then((result) => {
                p.ajax.reload();
            });
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Your data not updated',
                text: 'Failed!'
            })
        }
    })
}

// Delete 
function pDelete(id) {
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
                "url": "/Admin/RemoveProduct/",
                "dataType": "json",
                "data": { Id: id }
            }).then((hasil) => {
                debugger
                if (hasil.data.isSuccessStatusCode) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Your data has been deleted',
                        text: 'Deleted!'
                    }).then((result) => {
                        p.ajax.reload();
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
