// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// all pages js for sidebar
$(document).ready(function () {

    $('#dismiss, .overlay').on('click', function () {
        // hide sidebar
        $('#sidebar').removeClass('active');
        // hide overlay
        $('.overlay').removeClass('active');
    });

    $('#sidebarCollapse').on('click', function () {
        // open sidebar
        $('#sidebar').addClass('active');
        // fade in the overlay
        $('.overlay').addClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expanded=true]').attr('aria-expanded', 'false');
    });
});

// FAQ page thumb-right
$("#accordion").on("hide.bs.collapse show.bs.collapse", e => {
    $(e.target)
        .prev()
        .find("img:last-child")
        .toggleClass("right rotated");
});
$("#accordion2").on("hide.bs.collapse show.bs.collapse", e => {
    $(e.target)
        .prev()
        .find("img:last-child")
        .toggleClass("right rotated");
});

// All pages Change navbar
window.onscroll = function () { scrollFunction() };
function scrollFunction() {
    if (document.body.scrollTop > 80 || document.documentElement.scrollTop > 80) {
        document.getElementById("navbar").style.backgroundColor = "rgba(19, 19, 19, 0.8)";
    }
}


// reset password validation ---------------------------------------------------------------------------

function fnRemoveValidationOnChange(event) { //event= argument
    var control = event;
    console.log(event);
    //$("#passwordMatch").css('display', 'none');
    if ($(control).val().length > 0) {
        $(control).removeClass("hasError");
        $(control).next("span").removeClass("colourValidation").css("display", "none");
    }
    else {
        $(control).removeClass("hasError");
        $(control).next("span").removeClass("colourValidation").css("display", "none");
    }
}

function fnPwdValidation() {
    var isValid = true;
    var $storepassword = $("#storepassword");
    var $oldPassword = $("#oldpassword");
    var $newPassword = $("#newPassword");
    var $confirmPassword = $("#repeatpassword");
    var $oldPwdValidation = $("#oldPwdValidation"); //object declaration
    var $newPwdValidation = $("#newPwdValidation"); //object declaration
    var $confirmPwdValidation = $("#confirmPwdValidation"); //object declaration

    if (($oldPassword.val() == undefined || $oldPassword.val() == null || $oldPassword.val() == "") && ($newPassword.val() == undefined || $newPassword.val() == null || $newPassword.val() == "") && ($confirmPassword.val() == undefined || $confirmPassword.val() == null || $confirmPassword.val() == "")) {
        $oldPassword.addClass("hasError");
        $oldPwdValidation.text("Please enter this field.").css("display", "block").addClass("colourValidation");
        $newPassword.addClass("hasError");
        $newPwdValidation.text("Please enter this field.").css("display", "block").addClass("colourValidation");
        $confirmPassword.addClass("hasError");
        $confirmPwdValidation.text("Please enter this field.").css("display", "block").addClass("colourValidation");
        //$('#generalValidationError').addClass("shown");
        isValid = false;
    }
    else if ($oldPassword.val() != $storepassword.val()) {
        $oldPassword.addClass("hasError");
        $oldPwdValidation.text("You have entered wrong current password.").css("display", "block").addClass("colourValidation");
    }
    else if ($newPassword.val() != $confirmPassword.val()) {
        $confirmPassword.addClass("hasError");
        $confirmPwdValidation.text("Password does not match.").css("display", "block").addClass("colourValidation");
    }
    return isValid;
    // else {
    //    var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!#\$%\^&\*])(?=.{8,})/;
    //    isValid = regex.test($Password.val());
    //    if (isValid == false) {
    //        $Password.addClass("hasError");
    //        $('#generalValidationError').addClass("shown");

    //    }
    //}
}

function updatepwdcust() {
    debugger;
    var isValid = fnPwdValidation();
    var data = {};
    var storepassword = $("#storepassword").val();
    data.password = $("#oldpassword").val();
    data.newPassword = $("#newPassword").val();
    data.confirmPassword = $("#repeatpassword").val();
    if (isValid) {
        $.ajax({
            type: 'POST',
            url: '/CustomerPages/ChangePassword',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: data,
            success: function (response) {
                alert("valid password");
            },
        });
    }
}
function updatepwdsp() {
    debugger;
    var isValid = fnPwdValidation();
    var data = {};
    var storepassword = $("#storepassword").val();
    data.password = $("#oldpassword").val();
    data.newPassword = $("#newPassword").val();
    data.confirmPassword = $("#repeatpassword").val();
    if (isValid) {
        $.ajax({
            type: 'POST',
            url: '/ProviderPages/ChangePassword',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: data,
            success: function (response) {
                alert("valid password");
            },
        });
    }
}


// reset password validation ---------------------------------------------------------------------------