'use strict';

/*!
 * Modal popup control for MedStat.Admin site.
 *
 * Copyright (c) 2021 MedStat Company.
 */
 
/**
 * Depends on:
 *  - jQuery
 *  - Bootstrap
*/

window.ms = window.ms || {};

// Provides extra functionality for Bootstrap Modal component based on system behavior.
// "options" param is the "PopupDialogModel" server model:
//	{
//		controlId
//		...,
//		contentFrameId,
//		titleId,
//		localization: 
//		{
//			loading
//		}
//	}
window.ms.PopupDialogClass = function(options) {

	//console.log('options:', options, `#${options.ControlId}`);

	var bootstrapModal = null;


	this.setTitle = function (htmlTitle) {

		$(`#${options.titleId}`).html(htmlTitle);
	};

	this.setContent = function(htmlContent) {

		$(`#${options.contentFrameId}`).get(0).contentDocument.body.innerHTML = htmlContent;
	};

	this.setContentUrl = function (contentUrl) {

		$(`#${options.contentFrameId}`).get(0).src = contentUrl;
	};

	this.getBootstrapModal = function() {

		// lazy initialization allows us load Bootstrap JS in 'async' mode instead of 'defer'.
		if (bootstrapModal == null) {

			bootstrapModal = new bootstrap.Modal(document.getElementById(`${options.controlId}`), { focus: true });
		}

		return bootstrapModal;
	};


	this.init = function() {

		// Add event handlers:

		// iFrame events:
		$(`#${options.contentFrameId}`).get(0).addEventListener("load", function (e) {

			//console.log('iFrame load:', e, this);
			// Change popup height according to content height
			$(this).height($(this.contentDocument.body).height());
		});

		// popup content events:
		$(window).bind('ms_popup_loaded', function (e, popupParams) {

			//console.log('ms_popup_loaded:', e, popupParams, this);
			// Set popup title according content page title
			if (popupParams != null) {

				$(`#${options.titleId}`).html(popupParams.title); // we expect that only one popup can be open at one time
			}
		});

		return this;
	};

	this.show = function (contentUrl) {

		this.setTitle('');
		this.setContent(options.localization.loading);
		this.getBootstrapModal().show();

		if (contentUrl != null) {

			this.setContentUrl(contentUrl);
		}
	};
};