// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function() {

	// Init Site Nav panel (Expand/Collapse button)
	$('#sitenav_header_btnCollapse').click(function () {
		$(document.body).removeClass('ms-sitenav-expanded');
	});
	$('#sitenav_header_btnExpand').click(function () {
		$(document.body).addClass('ms-sitenav-expanded');
	});


	// Init Switch language control
	$('#cbxRuLanguage').change(function () {

		var langCookieName = $(this).attr('data-cookie-name');
		var langCookieValue = this.checked ? 'c=ru-RU|uic=ru-RU' : 'c=en-US|uic=en-US';

		Cookies.remove(langCookieName);
		Cookies.remove(langCookieName, { path: '/' });
		Cookies.set(langCookieName, langCookieValue, { expires: 365 });

		document.location = document.location.href;

	});

	// Auto Init DataTables
	window.setTimeout(function () {

		var tables = $("table[ms-datatable]");
		if (tables.length > 0)
			tables.msDataTable();

	}, 50);

	// Page init
	if (window.pageInit != null && $.isFunction(window.pageInit)) {
		window.setTimeout(window.pageInit, 50);
	}

});
