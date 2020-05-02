var s = null;
$(function () {
    s = $("#sTable").DataTable({
        'paging': true,
        'serverSide': true,
        'responsive': true,
        'ajax': "/Suppliers/Data",
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
                data: "createDate",
                render: function (data) {
                    return moment(data).format('DD MMMM YYYY');
                }
            },
            {
                render: function (data, type, row) {
                        return '<a style="color:#ffc107;" onclick="sGetById(' + row.id + ')"> <i class="fa fa-edit fa-lg"></i></a >' +
                            ' | <a style="color:#dc3545;" onclick="sDelete(' + row.id + ')"> <i class="fa fa-trash fa-lg"></i></a >'
                }
            }
        ],
        'columnDefs': [
            {
                "orderable": false,
                "targets": [ 0,1,2,3 ]
            },
            {
                "searchable": false,
                "targets": [0, 3]
            }
        ],
        'order': []
    });
});

// Clear Screen Input 
function sClearScreen() {
    document.getElementById("sIdText").disabled = true;
    $("#sIdText").val('');
    $("#sNameText").val('');
    $("#sUpdate").hide();
    $("#sSave").show();
}

// Validation Input
function sValidation() {
    if ($("#sNameText").val().trim() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill To Do List'
        })
    }
    else if ($("#sIdText").val() == "" || $("#sIdText").val() == " ") {
        sSave();
    }
    else {
        debugger
        sEdit($("#sIdText").val());
    }
}

// Save 
function sSave() {
    debugger;
    var supplier = new Object();
    supplier.Name = $("#sNameText").val();
    debugger;
    $.ajax({
        "url": "/Admin/CreateSupplier",
        "type": "POST",
        "dataType": "json",
        "data": supplier
    }).then((result) => {
        if (result.statusCode == 200) {
            Swal.fire({
                icon: 'success',
                title: 'Your data has been saved',
                text: 'Success!'
            }).then((hasil) => {
                s.ajax.reload();
            });
            $("#sModal").modal("hide");
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
function sGetById(id) {
    debugger
    $.ajax({
        "url": "/Admin/GetSupplier/",
        "type": "GET",
        "dataType": "json",
        "data": { Id: id }
    }).then((result) => {
        debugger
        //if (result.data != null) {
            document.getElementById("sIdText").disabled = true;
            debugger
            $("#sIdText").val(result.data.id);
            $("#sNameText").val(result.data.name);
            debugger
            $("#sModal").modal("show");
            $("#sUpdate").show();
            $("#sSave").hide();
        //}
    })
}

// Edit 
function sEdit(id) {
    var supplier = new Object();
    debugger
    supplier.Id = id;
    supplier.Name = $("#sNameText").val();
    $.ajax({
        "url": "/Admin/EditSupplier/",
        "type": "POST",
        "dataType": "json",
        "data": { Id: supplier.Id, Name: supplier.Name }
    }).then((result) => {
        if (result.statusCode == 200) {
            $("#sModal").modal("hide");
            Swal.fire({
                icon: 'success',
                title: 'Your data has been updated',
                text: 'Success!'
            }).then((result) => {
                s.ajax.reload();
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
function sDelete(id) {
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
                "url": "/Admin/RemoveSupplier/",
                "dataType": "json",
                "data": { Id: id }
            }).then((hasil) => {
                debugger
                if (hasil.statusCode == 200) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Your data has been deleted',
                        text: 'Deleted!'
                    }).then((result) => {
                        s.ajax.reload();
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
