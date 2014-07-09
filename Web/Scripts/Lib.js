/*
Lib
Utility functions used by rest of the site
*/
var Lib = function () {
	var
		/*
		CallJSON
		Makes a call to the given URL and receives a JSON-encoded item back
		*/
		CallJSON = function (url, params, callback) {
			var siteRoot = '/';
			url = siteRoot + url;
			$.getJSON(url, params, function (result) {
				OnRequestComplete();
				console.log("JSON result", result);
				// Check for error type?
				if (result.IsError) {
					// Format an error message
					var errorMessage = '';
					if (result.Exceptions.length == 1) {
						errorMessage = result.Exceptions[0];
					} else {
						errorMessage = $('<ul></ul>');
						$.each(result.Exceptions, function(ind, ex) {
							errorMessage.append($('<li></li>').html(ex));
						});
					}
					ShowError(errorMessage);
					return;
				}else{
					// Back to the user-specified callback
					callback(result);
				}
			});
		},
		/*
		Round
		JS implementations of toFixed() etc are inconsistent, so we roll our own
		Adapted from http://stackoverflow.com/questions/10015027/javascript-tofixed-not-rounding
		*/
		Round = function( number, precision ) {
			var multiplier = Math.pow( 10, precision + 1 ),
				wholeNumber = Math.floor( number * multiplier );
			return Math.round( wholeNumber / 10 ) * 10 / multiplier;
		},
		
		/*
		InitializePage
		Makes initial changes/setup to the page. Designed to be called as soon as initial DOM is rendered
		*/
		InitializePage = function() {
			var showDev = Cookies.GetItem('showdevelopermode');
			if (showDev == 'true') ToggleDeveloperMode();
			
			// When a user clicks a 'loadable' button, we adjust the style to give user feedback - typically done before Ajax calls
			$('body').on('click', '.loadable', function() {
				$(this).addClass('loading');
			});

			// Show or hide the dev panel
			$('body').on('click', '.toggle-developer-mode', function () {
				Lib.ToggleDeveloperMode();
			});
			
			// Ajax error handlers can bubble to user
			$.ajaxSetup({
				error: function (e, jqxhr, settings, exception) {
					if (e.status != 200) {
						OnRequestComplete();

						// If the transfer is interupted, we get an empty message back - no point showing
						if (e.responseText) {
							ShowError(e.responseText);
						}
					}
				}
			});

		},
		/*
		OnRequestComplete
		Should be called after each Ajax request
		*/
		OnRequestComplete = function() {
			$('body').find('.loading').removeClass('loading');
		},
		/*
		ShowError
		Notifies the user of the given error
		*/
		ShowError = function(msg) {
			ShowMessage(msg, true);
		},
		/*
		ShowMessage
		Quick notification to the user
		*/
		ShowMessage = function (msg, isError) {
			console.log("Message", msg);
			var $msg = $('<div></div>').addClass('popup-message-con').toggleClass('error-msg', isError);
			$msg.html(msg).hide();
			
			$('body').append($msg);
			
			// Close on click?
			$msg.bind('click', function() {
				CloseMessage($msg);
			});

			// Show
			$msg.fadeIn();
			setTimeout(function() {
				CloseMessage($msg);
			}, 5000);
		},
		CloseMessage = function($msg) {
			$msg.fadeOut(200, function() {
				$msg.remove();
			});
		},
		/*
		Helper routine to nicely present a json object in HTML, so a human can read it. Requires corresponding CSS
		*/
		FormatJson = function(json, linkText) {

			try {
				if (typeof(json) === "string") json = JSON.parse(json);
			}catch(e) {
				console.error(json);
				return 'Invalid JSON response returned. Please check the browser console.';
			}

			var fn_formatproperty = function(name, value) {


				// Add name
				var propertyNameClass = 'property-con';

				// Property value
				var valueType = typeof(value);
				var valueDesc = '';

				switch (valueType) {
				case "object":
				case "array":
					// Highlight items that are complex objects
					propertyNameClass += ' expandable';

					// Recurse
					for (var propName in value) {
						var propValue = value[propName];
						if (typeof(propValue) === "function") continue;
						valueDesc += fn_formatproperty(propName, propValue);
					}
					break;
				default:
					valueDesc = value;
				}

				// Add the name and value

				var result = '<div class="' + propertyNameClass + '">';
				result += '<span class="property-name">' + name + '</span>';
				result += '<span class="property-value">' + valueDesc + '</span>';

				result += '</div>';
				return result;
			};

			var html = '<div class="json-desc">' + fn_formatproperty(linkText || 'Click to expand', json) + '</div>';
			return html;
		},
		ToggleDeveloperMode = function() {
			$('body').toggleClass('show-developer-console');
			
			// Save to cookie so we can restore later
			var isShowing = $('body').hasClass('show-developer-console');
			Cookies.SetItem('showdevelopermode', isShowing);
		},
		Cookies = function() {
			return {
				GetItem: function(sKey) {
					return decodeURIComponent(document.cookie.replace(new RegExp("(?:(?:^|.*;)\\s*" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*([^;]*).*$)|^.*$"), "$1")) || null;
				},
				SetItem: function(sKey, sValue, vEnd, sPath, sDomain, bSecure) {
					if (!sKey || /^(?:expires|max\-age|path|domain|secure)$/i.test(sKey)) {
						return false;
					}
					var sExpires = "";
					if (vEnd) {
						switch (vEnd.constructor) {
						case Number:
							sExpires = vEnd === Infinity ? "; expires=Fri, 31 Dec 9999 23:59:59 GMT" : "; max-age=" + vEnd;
							break;
						case String:
							sExpires = "; expires=" + vEnd;
							break;
						case Date:
							sExpires = "; expires=" + vEnd.toUTCString();
							break;
						}
					}
					document.cookie = encodeURIComponent(sKey) + "=" + encodeURIComponent(sValue) + sExpires + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "") + (bSecure ? "; secure" : "");
					return true;
				},
				RemoveItem: function(sKey, sPath, sDomain) {
					if (!sKey || !this.HasItem(sKey)) {
						return false;
					}
					document.cookie = encodeURIComponent(sKey) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "");
					return true;
				},
				HasItem: function(sKey) {
					return (new RegExp("(?:^|;\\s*)" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
				}
			};
		}();
		;

	return {
		Round: Round,
		CallJSON: CallJSON,
		ShowMessage: ShowMessage,
		Cookies: Cookies,
		FormatJson: FormatJson,
		InitializePage: InitializePage,
		ToggleDeveloperMode: ToggleDeveloperMode
	};
}();