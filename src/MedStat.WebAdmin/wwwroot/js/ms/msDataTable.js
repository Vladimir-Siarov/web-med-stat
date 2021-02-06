/*!
 * Custom "msDataTable" jQuery plugin for MedStat.Admin site,
 * which extends original "DataTable" plugin from DataTables.net
 *
 * Copyright (c) 2020 MedStat Company
 */

/** 
 * Depends on:
 *  - jQuery
 *  - jquery.dataTables (DataTables.net)
 */

window.ms = window.ms || {};

// Provides extra functionality for DataTable control
window.ms.DataTableToolsClass = function () {

	// <TH> attributes processing:

	// Creates column definition object by: 
	//  - specified attribute
	//  - column index
	function getColumnDefByAttribute(attr, columnIndex) {

		switch (attr.name) {

			case 'data-ms-html-template':
				{
					var htmlTemplate = $(attr.value).html();

					return getColumnDefByHtmlTemplate(htmlTemplate, columnIndex);
				};

			case 'data-ms-max-length':
				{
					var maxLength = parseInt(attr.value);
					if (maxLength !== NaN) {

						return getColumnDefByMaxLength(maxLength, columnIndex);
					}
					else
						return null;
				}

			default:
				return null;
		}
	};

	// Creates column definition object with custom "render" function.
	// That function parses specified HTML 
	//	and replace "{X}" string with appropriate X property of row data object.
	function getColumnDefByHtmlTemplate(htmlTemplate, columnIndex) {

		if (htmlTemplate != null && htmlTemplate != '') {

			//console.log('htmlTemplate: ', htmlTemplate);

			var reBrackets = /\{(.*?)\}/g;
			var matches = htmlTemplate.match(reBrackets);

			if (matches != null && matches.length > 0) {

				//console.log('matches: ', matches);

				var columnDef = {
					targets: columnIndex,
					render: function (data, type, row, meta) {

						//console.log('start render html template');

						var resultHtml = htmlTemplate + '';
						for (var j = 0; j < matches.length; j++) {

							var propertyName = matches[j].replace('{', '').replace('}', ''); // extract property name from '{X}'

							resultHtml = resultHtml.replace(matches[j], row[propertyName]);
						}

						return resultHtml;
					}
				}

				return columnDef;
			}
		}

		return null;
	};

	// Creates column definition object with custom "render" function.
	// If column data length greatest than specified,
	//   then function trims column data and wrap it by SPAN element with "title" attribute.
	function getColumnDefByMaxLength(maxLength, columnIndex) {

		var columnDef = {
			targets: columnIndex,
			render: function (data, type, row, meta) {

				return type === 'display' && data != null && data.length > maxLength
					? '<span title="' + data + '">' + data.substr(0, maxLength) + '...</span>'
					: data;
			}
		}

		return columnDef;
	};


	// Checks all TH elements in specified tableEl for special attributes
	// and creates DataTable options with appropriate column def. options. 
	function preProcessThAttributes(tableEl) {

		var columnDefsOptions = {
			columnDefs: []
		};

		var thArray = $(tableEl).find('th');
		for (var thIndex = 0; thIndex < thArray.length; thIndex++) {

			var attributes = thArray[thIndex].attributes;
			for (var attrIndex = 0; attrIndex < attributes.length; attrIndex++) {

				var columnDef = getColumnDefByAttribute(attributes[attrIndex], thIndex);
				if (columnDef != null) {

					columnDefsOptions.columnDefs.push(columnDef);
					break; // doesn't support several attributes in the same element
				}
			}
		}

		return columnDefsOptions;
	}


	// <table> attribute processing

	// Creates DataTable option based on table element attributes.
	function preProcessTableAttributes(tableEl) {

		var options = {};

		$.each(tableEl.attributes, function (i, attr) {
			//console.log(i, attr.name, attr.value);

			switch (attr.name) {

				case 'data-ms-page-length-control':
					$(attr.value).each(function () {

						var pageLength = parseInt(this.value);
						if (pageLength != null) {

							//options.lengthChange = false; // disable default Page Length control
							options.pageLength = pageLength;

							// remove default Page Length control
							options.dom = options.dom != null ? options.dom : $.fn.dataTable.defaults.dom;
							options.dom = options.dom
								.replace('l>', '>'); // by idea we need to check that "l" not inside single quotes, because it is class name. 
						}

					});
					break;

				case 'data-ms-search-control':
					$(attr.value).each(function () {

						// remove default Search control
						options.dom = options.dom != null ? options.dom : $.fn.dataTable.defaults.dom;
						options.dom = options.dom
							.replace('f>', '>'); // by idea we need to check that "f" not inside single quotes, because it is class name. 

						//console.log('new dom', options.dom);
					});
					break;

			}
		});

		//console.log('preProcessTableAttributes: options', options);
		return options;
	}

	// Add extra functionality (via DT Api) to the DataTable instance
	// based on his table element attributes.
	function postProcessTableAttributes(dataTable) {

		$.each(dataTable.table().node().attributes, function (i, attr) {
			//console.log(i, attr.name, attr.value);

			switch (attr.name) {

				case 'data-ms-page-length-control':
					$(attr.value).change(function () {
						dataTable.page.len(parseInt(this.value)).draw();
					});
					break;

				case 'data-ms-search-control':
					$(attr.value).on('keyup', function () {
						//TODO: Need optimization
						dataTable.search(this.value).draw();
					});
					break;

			}
		});
	}


	// Public methods:

	this.preProcessCustomAttributes = function (tableEl) {

		var columnDefsOptions = preProcessThAttributes(tableEl);
		var tableOptions = preProcessTableAttributes(tableEl);

		return $.extend(columnDefsOptions, tableOptions);

	}

	this.postProcessCustomAttributes = function (dataTable) {

		postProcessTableAttributes(dataTable);

	}
};


// jQuery plugin
(function ($) {

	$.fn.msDataTable = function (options) {

		var jQueryThis = this;

		this.each(function() {

			var tableEl = this;
			var dataTableTools = new window.ms.DataTableToolsClass();

			// Default grid options
			var defOptions = {
				processing: true,
				serverSide: true,

				lengthMenu: [10, 25, 50],
				pagingType: "first_last_numbers",

				language: window.msDataTableOptions != null ? window.msDataTableOptions.language : null
			}

			// Define render functions for columns with special attributes
			var attributesOptions = dataTableTools.preProcessCustomAttributes(tableEl);

			var newOptions = $.extend(defOptions, attributesOptions, options || {});
			var dataTable = $(tableEl).DataTable(newOptions);

			dataTableTools.postProcessCustomAttributes(dataTable);

		});

		return jQueryThis;

	};

}(jQuery));


// Auto Init DataTables
window.setTimeout(function () {

		var tables = $("table[ms-datatable]");
		if (tables.length > 0)
			tables.msDataTable();

	}, 50);