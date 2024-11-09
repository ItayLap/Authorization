// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

$(document).ready(function () {
    $("#registerForm").validate({
        rules: {
            "Input.Email": {
                required: true,
                email: true

            }, "Input.Password": {
                required: true,
                minlength: 8,
                passwordComplexity: true

            }, "Input.ConfirmPassword": {
                required: true,
                equalTo: "#Input_Password"

            }, "Input.FirstName": {
                required: true,
                minlength: 2,

            }, "Input.LastName": {
                required: true,
                minlength: 2,

            }, "Input.DateOfBirth": {
                required: true,
                minimumAge: 13
            }
        }
    })
})
