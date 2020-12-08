$(function () {

	$('#tblCompanies').DataTable({
		processing: true,
		serverSide: true,

		lengthMenu: [2, 10, 25, 50],
		pagingType: "first_last_numbers",
		//"ajax": "@(Url.PageLink("Index", "OnGetCompanyListAsync"))",

		columns: [
			{ "data": "id" },
			{ "data": "name" },
			{ "data": "description" }
		],

		language: {
			processing: 'загрузка...', // "Processing..."
			loadingRecords: 'загрузка...', // "Loading..."

			lengthMenu: "Показывать _MENU_ записей", // "Show _MENU_ entries"
			search: "Поиск:", // "Search:"

			info: "Показано с _START_ по _END_ из _TOTAL_ записей", // "Showing _START_ to _END_ of _TOTAL_ entries"
			paginate: {
				first: "Первая",
				previous: "Предыдущая",
				next: "Следующая",
				last: "Последняя"
			}
		}
	});

});