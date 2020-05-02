function uLogin() {    
    var login = new Object();
    login.Email = $("#email").val();
    login.PasswordHash = $("#password").val();
    if ($("#email").val().trim() == "") {
        $("#login-alert").show();
        $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
        $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">x</button>Please fill email');
    }
    else if ($("#password").val().trim() == "") {
        $("#login-alert").show();
        $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
        $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">x</button>Please fill password');
    }
    else {
        debugger;
        $.ajax({
            "url": "/Users/Login",
            "type": "POST",
            "dataType": "json",
            "data": login //{ Email: login.Email, PasswordHash: login.PasswordHash }
        }).then((result) => {
            debugger;
            if (result.statusCode == 200) {
                window.location.href = '/Admin'
            }
            else {
                $("#login-alert").removeAttr('hidden');
                $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
                $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">x</button>Email / Password is Invalid');
            }
        })
    }
}

function uRegister() {
    var reg = new Object();
    reg.Name = $("#name").val();
    reg.Email = $("#email").val();
    reg.UserName = $("#username").val();
    reg.PasswordHash = $("#password").val();
    if ($("#email").val().trim() == "") {
        $("#login-alert").removeAttr('hidden');
        $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
        $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true"' +
            'onclick=uClearAlert();>&times;</button>Please fill email');
    }
    else if ($("#name").val().trim() == "") {
        $("#login-alert").removeAttr('hidden');
        $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
        $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true"' +
            'onclick=uClearAlert();>&times;</button>Please fill name');
    }
    else if ($("#username").val().trim() == "") {
        $("#login-alert").removeAttr('hidden');
        $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
        $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true"' +
            'onclick=uClearAlert();>&times;</button>Please fill username');
    }
    else if ($("#password").val().trim() == "") {
        $("#login-alert").removeAttr('hidden');
        $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
        $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true"' +
            'onclick=uClearAlert();>&times;</button>Please fill password');
    }
    
    else {
        debugger;
        $.ajax({
            "url": "/Users/Register",
            "type": "POST",
            "dataType": "json",
            "data": reg
        }).then((result) => {
            debugger;
            if (result.statusCode == 200) {                
                Swal.fire({
                    icon: 'success',
                    title: 'Your Register is Successful',
                    text: 'Success!'
                }).then((result) => {
                    window.location.href = '/Users'
                });
            }
            else {
                $("#login-alert").removeAttr('hidden');
                $("#login-alert").attr("class", "alert alert-danger alert-dismissible");
                $("#login-alert").html('<button type="button" class="close" data-dismiss="alert" aria-hidden="true"' +
                    'onclick=uClearAlert();>&times;</button>Register is Failed');
            }
        })
    }
}