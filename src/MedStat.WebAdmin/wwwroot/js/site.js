// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Site Nav panel
$('#sitenav_header_btnCollapse').click(function() {
	$(document.body).removeClass('ms-sitenav-expanded');
});
$('#sitenav_header_btnExpand').click(function () {
	$(document.body).addClass('ms-sitenav-expanded');
});
