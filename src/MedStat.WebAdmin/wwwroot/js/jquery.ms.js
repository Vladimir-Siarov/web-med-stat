/*!
 * jQuery plugins for MedStat.Admin site
 *
 * Copyright (c) 2020 MedStat Company
 */

(function ($) {

	window.ms = window.ms || {};

	// Provides extra functionality for DataTable control
	window.ms.DataTableToolsClass = function() {

		// Creates column definition object by: 
		//  - specified attribute name
		//  - specified DOM element (TH)
		//  - column index
		function getColumnDefByAttribute(attrName, thEl, columnIndex) {

			switch (attrName) {

				case 'data-html-template-id':
					{
						var htmlTemplateId = $(thEl).attr('data-html-template-id');
						if (htmlTemplateId != null && htmlTemplateId != '') {

							return getColumnDefByHtmlTemplateId(htmlTemplateId, columnIndex);
						}
						else
							return null;
					};

				case 'data-max-length':
					{
						var maxLength = parseInt($(thEl).attr('data-max-length'));
						if (maxLength !== NaN) {

							return getColumnDefByMaxLength(maxLength, columnIndex);
						}
						else
							return null;
					}

				default:
					throw 'Attribute "' + attrName + '" is not supported';
			}
		};

		// Creates column definition object with custom "render" function.
		// That function parses HTML of specified element 
		//	and replace "{X}" string with appropriate X property of row data object.
		function getColumnDefByHtmlTemplateId(templateId, columnIndex) {

			var htmlTemplate = $('#' + templateId).html();
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

								resultHtml = resultHtml.replace(matches[j], row[matches[j].replace('{', '').replace('}', '')]);
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
		this.processThAttributes = function(tableEl) {

			var columnDefsOptions = {
				columnDefs: []
			};

			var attributes = [
					'data-html-template-id',
					'data-max-length'
			];

			var thArray = $(tableEl).find('th');
			for (var thIndex = 0; thIndex < thArray.length; thIndex++) {

				for (var attrIndex = 0; attrIndex < attributes.length; attrIndex++) {

					var columnDef = getColumnDefByAttribute(attributes[attrIndex], thArray[thIndex], thIndex);
					if (columnDef != null) {

						columnDefsOptions.columnDefs.push(columnDef);
						break; // doesn't support several attributes in the same element
					}
				}
			}

			return columnDefsOptions;
		}
	};


	// jQuery plugins:

	$.fn.msDataTable = function (options) {

		// Default grid options
		var defOptions = {
			processing: true,
			serverSide: true,

			lengthMenu: [10, 25, 50],
			pagingType: "first_last_numbers",
			
			language: window.msDataTableOptions != null ? window.msDataTableOptions.language : null
		}

		// Define render functions for columns with special attributes
		var dataTableTools = new window.ms.DataTableToolsClass();
		var columnDefsOptions = dataTableTools.processThAttributes(this);

		var newOptions = $.extend(defOptions, columnDefsOptions, options || {});

		this.DataTable(newOptions);

		return this;

	};


}(jQuery));