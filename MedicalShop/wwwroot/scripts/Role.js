var tbl = null;
$(function () {
    tbl = $("#Table").DataTable({
        'ajax': "/Admin/List",
        'type': "GET",
        "columns": [
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
                render: function (data, type, row) {
                    return '<a style="color:#ffc107;" onclick="GetById(' + row.id + ')"> <i class="fa fa-edit fa-lg"></i></a >' +
                        ' | <a style="color:#dc3545;" onclick="Delete(' + row.id + ')"> <i class="fa fa-trash fa-lg"></i></a >'
                }
            }
        ]
    });
});

// Clear Screen Input To Do List
function ClearScreen() {
    document.getElementById("IdText").disabled = true;
    $("#IdText").val('');
    $("#NameText").val('');
    $("#Update").hide();
    $("#Save").show();
}

// Validation Input
function Validation() {
    if ($("#NameText").val().trim() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill To Do List'
        })
    }
    else if ($("#IdText").val() == "" || $("#IdText").val() == " ") {
        Save();
    }
    else {
        debugger
        Edit($("#IdText").val());
    }
}

function Save() {
    debugger;
    var role = new Object();
    role.Name = $("#NameText").val();
    debugger;
    $.ajax({
        "url": "/Admin/Create",
        "type": "POST",
        "data": role
    }).then((result) => {
        if (result.statusCode == 200) {
            Swal.fire({
                icon: 'success',
                title: 'Your data has been saved',
                text: 'Success!'
            }).then((hasil) => {
                tbl.ajax.reload();
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

function GetById(id) {
    debugger
    $.ajax({
        "url": "/Admin/Details/",
        "type": "GET",
        "data": { Id: id }
    }).then((result) => {
        debugger
        //if (result.data != null) {
            document.getElementById("IdText").disabled = true;
            $("#IdText").val(result.data.id);
            $("#NameText").val(result.data.name);            
        //}
    })
    debugger
    $("#Modal").modal("show");
    $("#Update").show();
    $("#Save").hide();
}

function Edit(id) {
    var role = new Object();
    debugger
    role.Id = id;
    role.Name = $("#NameText").val();
    $.ajax({
        "url": "/Admin/Edit/",
        "type": "POST",
        "dataType": "json",
        "data": { Id: role.Id, Name: role.Name }
    }).then((result) => {
        if (result.statusCode == 200) {
            $("#Modal").modal("hide");
            Swal.fire({
                icon: 'success',
                title: 'Your data has been updated',
                text: 'Success!'
            }).then((result) => {
                tbl.ajax.reload();
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
                "url": "/Admin/Delete/",
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
                        tbl.ajax.reload();
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