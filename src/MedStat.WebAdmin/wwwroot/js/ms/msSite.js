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
	window.ms.siteNav.init();
	

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
