/*!
 * jQuery plugins for MedStat.Admin site
 *
 * Copyright (c) 2020 MedStat Company
 */

(function ($) {

	$.fn.msDataTable = function (options) {

		var defOptions = {
			processing: true,
			serverSide: true,

			lengthMenu: [10, 25, 50],
			pagingType: "first_last_numbers",
			
			language: window.msDataTableOptions != null ? window.msDataTableOptions.language : null
		}

		var newOptions = $.extend(defOptions, options || {});

		this.DataTable(newOptions);

		return this;

	};

}(jQuery));