$(function () {

	$('#tblCompanies').DataTable({
		processing: true,
		serverSide: true,

		lengthMenu: [2, 10, 25, 50],
		pagingType: "first_last_numbers",
		//"ajax": "@(Url.PageLink("Index", "OnGetCompanyListAsync"))",

		columns: [
			{ data: "id" },
			{ data: "name" },
			{ data: "description" },
			{ data: "accountCnt" },
			{ data: "trackedPersonCnt" }
		],

		language: {
			processing: 'загрузка...', // "Processing..."
			loadingRecords: 'загрузка...', // "Loading..."

			lengthMenu: "Показывать _MENU_ записей", // "Show _MENU_ entries"
			search: "Поиск:", // "Search:"

			//info: "Показано с _START_ по _END_ из _TOTAL_ записей", // "Showing _START_ to _END_ of _TOTAL_ entries"
			info: "Всего записей: _TOTAL_", // "Total records: _TOTAL_"
			paginate: {
				first: "Первая",
				previous: "Предыдущая",
				next: "Следующая",
				last: "Последняя"
			}
		}
	});

});