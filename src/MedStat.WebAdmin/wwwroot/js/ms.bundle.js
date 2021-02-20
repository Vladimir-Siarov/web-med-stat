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
'use strict';

/*!
 * Common functionality for MedStat.Admin site.
 *
 * Copyright (c) 2021 MedStat Company.
 */

/**
 * Depends on:
 *  - jQuery
 *  - js.cookie
*/

window.ms = window.ms || {};

window.ms.siteNav = {

	stateCookieName: 'ms-siteNav-state',

	expand: function (isSaveState, withTransition) {

		this.setTransition(withTransition);

		$(document.body).addClass('ms-sitenav-expanded');

		if (isSaveState)
			this.setState('expanded');
	},
	collapse: function (isSaveState, withTransition) {

		this.setTransition(withTransition);

		$(document.body).removeClass('ms-sitenav-expanded');

		if (isSaveState)
			this.setState('collapsed');
	},

	setTransition: function (withTransition) {

		if (withTransition) {
			$(document.body).addClass('with-transition');
		}
		else {
			$(document.body).removeClass('with-transition');
		}
	},
	setState: function (state) {

		Cookies.remove(this.stateCookieName);
		Cookies.remove(this.stateCookieName, { path: '/' });
		Cookies.set(this.stateCookieName, state);
	},

	initMenuState: function() {

		var siteNavState = Cookies.get(this.stateCookieName);

		if (siteNavState === 'expanded') {
			this.expand(false);
		}
		else if (siteNavState === 'collapsed') {
			this.collapse(false);
		}
		else { // auto expand / collapse

			if ($(window).width() >= 960) { // lg
				this.expand(false);
			}
		}
	},

	init: function () {

		this.initMenuState();


		// Init Expand/Collapse buttons:

		$('#sitenav_header_btnCollapse').click(function () {
			window.ms.siteNav.collapse(true, true);
		});
		$('#sitenav_header_btnExpand').click(function () {
			window.ms.siteNav.expand(true, true);
		});
	}
};


$(function() {

	// Init Site Nav panel
	window.setTimeout(function() {
		window.ms.siteNav.init();
	}, 50);
	

	// Init Switch language control
	$('#cbxRuLanguage').change(function () {

		var langCookieName = $(this).attr('data-cookie-name');
		var langCookieValue = this.checked ? 'c=ru-RU|uic=ru-RU' : 'c=en-US|uic=en-US';

		Cookies.remove(langCookieName);
		Cookies.remove(langCookieName, { path: '/' });
		Cookies.set(langCookieName, langCookieValue, { expires: 365 });

		document.location = document.location.href;

	});

	// Page init
	if (window.pageInit != null && $.isFunction(window.pageInit)) {
		window.setTimeout(window.pageInit, 50);
	}

});