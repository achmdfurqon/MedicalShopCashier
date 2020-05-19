var users = null;
$('#Role').select2();
$('#UserRole').select2();

$(function () {
    users = $("#Table").DataTable({
        'responsive': true,
        'ajax': "/Admin/GetUsers",
        'columns': [
            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: "name" },
            { data: "userName" },
            { data: "role" },
            {
                render: function (data, type, row) {
                    return "<a style='color:#ffc107;' onclick=GetById('" + row.id + "')> <i class='fa fa-edit fa-lg'></i></a >" +
                        " | <a style='color:#dc3545;' onclick=Delete('" + row.id + "')> <i class='fa fa-trash fa-lg'></i></a >" 
                }
            }
        ],
        'columnDefs': [
            {
                "orderable": false,
                "targets": [0, 1, 2, 3, 4]
            },
            {
                "searchable": false,
                "targets": [0, 4]
            }
        ],
        'order': []
    });
});

// Clear Screen Input 
function ClearScreen() {
    document.getElementById("IdText").disabled = true;
    $("#IdText").val('');
    $("#Name").val('');
    $("#UserName").val('');
    $("#Email").val('');
    $("#Password").val('');
    $("#Role").val('');
}

// Validation Input
function Save() {
    if ($("#Name").val().trim() == "" || $("#UserName").val().trim() == "" || $("#Email").val().trim() == "" || $("#Password").val().trim() == "" || $("#Role").val().trim() == "") {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Please Fill Empty Field'
        })
    }
    else if ($("#IdText").val() == "") {
        debugger;
        var user = new Object();
        user.Name = $("#Name").val();
        user.UserName = $("#UserName").val();
        user.Email = $("#Email").val();
        user.PasswordHash = $("#Password").val();
        user.Role = $("#Role").val();
        debugger;
        $.ajax({
            "url": "/Admin/CreateUser",
            "type": "POST",
            "dataType": "json",
            "data": user
        }).then((result) => {
            if (result.statusCode == 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Your data has been saved',
                    text: 'Success!'
                }).then((hasil) => {
                    users.ajax.reload();
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
}

// Get Id
function GetById(id) {
    debugger
    //$.ajax({
    //    "url": "/Admin/GetRole/",
    //    "type": "GET",
    //    "dataType": "json",
    //    "data": { Id: id }
    //}).then((result) => {
    //    document.getElementById("UserId").disabled = true;
    //    debugger
        $("#UserId").val(id);
        //$("#Role").val(result.data.role);
        $("#roleModal").modal("show");
        //}
    //})
}

// Edit 
function Assign() {
    var Role = $("#UserRole").val();
    var Id = $("#UserId").val();
    debugger
    $.ajax({
        "url": "/Admin/AssignRole/",
        "type": "POST",
        "dataType": "json",
        "data": { id: Id, role: Role }
    }).then((result) => {
        if (result.statusCode == 200) {
            $("#roleModal").modal("hide");
            Swal.fire({
                icon: 'success',
                title: 'Your data has been updated',
                text: 'Success!'
            }).then((result) => {
                users.ajax.reload();
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

function Remove() {
    var Role = new Object();
    Role.Name = $("#UserRole").val();
    var Id = $("#UserId").val();
    debugger
    $.ajax({
        "url": "/Admin/RemoveRole/",
        "type": "POST",
        "dataType": "json",
        "data": { id: Id, role: Role }
    }).then((result) => {
        if (result.statusCode == 200) {
            $("#roleModal").modal("hide");
            Swal.fire({
                icon: 'success',
                title: 'Your data has been updated',
                text: 'Success!'
            }).then((result) => {
                users.ajax.reload();
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
                "url": "/Admin/DeleteUser/",
                "dataType": "json",
                "data": { Id: id }
            }).then((hasil) => {
                debugger
                if (hasil.data.statusCode == 200) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Your data has been deleted',
                        text: 'Deleted!'
                    }).then((result) => {
                        users.ajax.reload();
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
