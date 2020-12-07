$(function () {

	$('#tblCompanies').DataTable({
		"processing": true,
		"serverSide": true,
		"lengthMenu": [2, 10, 25, 50],
		//"ajax": "@(Url.PageLink("Index", "OnGetCompanyListAsync"))",
		"columns": [
			{ "data": "id" },
			{ "data": "name" },
			{ "data": "description" }
		]
	});

});