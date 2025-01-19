/*
International Telephone Input v1.1.12
https://github.com/Bluefieldscom/intl-tel-input.git
*/
// wrap in UMD - see https://github.com/umdjs/umd/blob/master/jqueryPlugin.js
(function(factory) {
    if (typeof define === "function" && define.amd) {
        define([ "jquery" ], function($) {
            factory($, window, document);
        });
    } else {
        factory(jQuery, window, document);
    }
})(function($, window, document, undefined) {
    "use strict";
    var pluginName = "intlTelInputCity", id = 1, // give each instance it's own id for namespaced event handling
    defaults = {
        // don't insert international dial codes
        nationalMode: false,
        // if there is just a dial code in the input: remove it on blur, and re-add it on focus
        autoHideDialCode: true,
        // default country
        defaultCountry: "",
        // character to appear between dial code and phone number
        dialCodeDelimiter: " ",
        // position the selected flag inside or outside of the input
        defaultStyling: "inside",
        // display only these countries
        onlyCountries: [],
        // the countries at the top of the list. defaults to united states and united kingdom
        preferredCountries: [ "gb" ],
        // specify the path to the libphonenumber script to enable validation
        validationScript: ""
    }, keys = {
        UP: 38,
        DOWN: 40,
        ENTER: 13,
        ESC: 27,
        PLUS: 43,
        A: 65,
        Z: 90
    }, windowLoaded = false;
    // keep track of if the window.load event has fired as impossible to check after the fact
    $(window).load(function() {
        windowLoaded = true;
    });
    function Plugin(element, options) {
        this.element = element;
        this.options = $.extend({}, defaults, options);
        this._defaults = defaults;
        // event namespace
        this.ns = "." + pluginName + id++;
        this._name = pluginName;
        this.init();
    }
    Plugin.prototype = {
        init: function() {
            // process all the data: onlyCounties, preferredCountries, defaultCountry etc
            this._processCountryData();
            // generate the markup
            this._generateMarkup();
            // set the initial state of the input value and the selected flag
            // this._setInitialState();
            // start all of the event listeners: autoHideDialCode, input keyup, selectedFlag click
            this._initListeners();
        },
        /********************
     *  PRIVATE METHODS
     ********************/
        // prepare all of the country data, including onlyCountries, preferredCountries and
        // defaultCountry options
        _processCountryData: function() {
            // set the instances country data objects
            this._setInstanceCountryData();
            // set the preferredCountries property
            this._setPreferredCountries();
        },
        // process onlyCountries array if present
        _setInstanceCountryData: function() {
            var that = this;
            if (this.options.onlyCountries.length) {
                var newCountries = [], newCountryCodes = {};
                $.each(this.options.onlyCountries, function(i, countryCode) {
                    var countryData = that._getCountryData(countryCode, true);
                    if (countryData) {
                        newCountries.push(countryData);
                        // add this country's dial code to the countryCodes
                        var dialCode = countryData.dialCode;
                        if (newCountryCodes[dialCode]) {
                            newCountryCodes[dialCode].push(countryCode);
                        } else {
                            newCountryCodes[dialCode] = [ countryCode ];
                        }
                    }
                });
                // maintain country priority
                for (var dialCode in newCountryCodes) {
                    if (newCountryCodes[dialCode].length > 1) {
                        var sortedCountries = [];
                        // go through all of the allCountryCodes countries for this dialCode and create a new (ordered) array of values (if they're in the newCountryCodes array)
                        for (var i = 0; i < allCountryCodes[dialCode].length; i++) {
                            var country = allCountryCodes[dialCode][i];
                            if ($.inArray(newCountryCodes[dialCode], country)) {
                                sortedCountries.push(country);
                            }
                        }
                        newCountryCodes[dialCode] = sortedCountries;
                    }
                }
                this.countries = newCountries;
                this.countryCodes = newCountryCodes;
            } else {
                this.countries = allCountries;
                this.countryCodes = allCountryCodes;
            }
        },
        // process preferred countries - iterate through the preferences,
        // fetching the country data for each one
        _setPreferredCountries: function() {
            var that = this;
            this.preferredCountries = [];
            $.each(this.options.preferredCountries, function(i, countryCode) {
                var countryData = that._getCountryData(countryCode, false);
                if (countryData) {
                    that.preferredCountries.push(countryData);
                }
            });
        },
        // generate all of the markup for the plugin: the selected flag overlay, and the dropdown
        _generateMarkup: function() {
            // telephone input
            this.telInput = $(this.element);
            // containers (mostly for positioning)
            var mainClass = "intl-tel-input";
            if (this.options.defaultStyling) {
                mainClass += " " + this.options.defaultStyling;
            }
            this.telInput.wrap($("<div>", {"class": mainClass}));
            var flagsContainer = $("<div>", {"class": "flag-dropdown"}).insertAfter(this.telInput);

            // currently selected flag (displayed to left of input)
            var selectedFlag = $("<div>", {"class": "selected-flag-new"}).appendTo(flagsContainer);
            this.selectedFlagInner = $("<div>", {"class": "flag v-hide"}).appendTo(selectedFlag);

            // CSS triangle
            $("<div>", {"class": "arrow"}).appendTo(this.selectedFlagInner);

            // country list contains: preferred countries, then divider, then all countries
            this.countryListDiv = $("<div>", {"class": "country-wrap-list v-hide"}).appendTo(flagsContainer);
            this.countryList = $("<ul>", {"class": "country-list-new"}).appendTo(this.countryListDiv);
            if (this.preferredCountries.length) {
                this._appendListItems(this.preferredCountries, "preferred");
                $("<li>", {"class": "divider"}).appendTo(this.countryList);
            }
            $("<input>", {"class": "search-input-new", "type": "text", "id": "searchInputNew", "placeholder": "搜索"}).insertBefore(".country-list-new");
            this._appendListItems(this.countries, "");
            // now we can grab the dropdown height, and hide it properly
            this.dropdownHeight = this.countryList.outerHeight();
            this.countryListDiv.removeClass("v-hide").addClass("hide");
            // this is useful in lots of places
            this.countryListItems = this.countryList.children(".country");
        },
        // add a country <li> to the countryList <ul> container
        _appendListItems: function(countries, className) {
            // we create so many DOM elements, I decided it was faster to build a temp string
            // and then add everything to the DOM in one go at the end
            var tmp = "";
            // for each country
            $.each(countries, function(i, c) {
                // open the list item
                tmp += "<li class='country " + className + "' data-dial-code='" + c.dialCode + "' data-country-code='" + c.iso2 + "'>";
                // add the flag
                tmp += "<div class='flag " + c.iso2 + "'></div>";
                // and the country name and dial code
                tmp += "<span class='country-name'>" + c.name + "</span>";
                // tmp += "<span class='dial-code'>+" + c.dialCode + "</span>";
                // close the list item
                tmp += "</li>";
            });
            this.countryList.append(tmp);
        },
        // set the initial state of the input value and the selected flag
        _setInitialState: function() {
            var flagIsSet = false;
            // if the input is pre-populated, then just update the selected flag accordingly
            // however, if no valid international dial code was found, flag will not have been set
            if (this.telInput.val()) {
                flagIsSet = this._updateFlagFromInputVal();
            }
            if (!flagIsSet) {
                // flag is not set, so set to the default country
                var defaultCountry;
                // check the defaultCountry option, else fall back to the first in the list
                if (this.options.defaultCountry) {
                    defaultCountry = this._getCountryData(this.options.defaultCountry, false);
                } else {
                    defaultCountry = this.preferredCountries.length ? this.preferredCountries[0] : this.countries[0];
                }
                this._selectFlag(defaultCountry.iso2);
                // if autoHideDialCode is disabled, insert the default dial code
                if (!this.options.autoHideDialCode) {
                    this._resetToDialCode(defaultCountry.dialCode);
                }
            }
        },
        // initialise the main event listeners: input keyup, and click selected flag
        _initListeners: function() {
            var that = this;
            // auto hide dial code option (ignore if in national mode)
            if (this.options.autoHideDialCode && !this.options.nationalMode) {
                this._initAutoHideDialCode();
            }
            // update flag on keyup (by extracting the dial code from the input value).
            // use keyup instead of keypress because we want to update on backspace
            // and instead of keydown because the value hasn't updated when that event is fired
            // NOTE: better to have this one listener all the time instead of starting it on focus
            // and stopping it on blur, because then you've got two listeners (focus and blur)
            this.telInput.on("keyup" + this.ns, function() {
                that._updateFlagFromInputVal();
            });
            // toggle country dropdown on click
            var selectedFlag = this.selectedFlagInner.parent();
            selectedFlag.on("click" + this.ns, function(e) {
                // only intercept this event if we're opening the dropdown
                // else let it bubble up to the top ("click-off-to-close" listener)
                // we cannot just stopPropagation as it may be needed to close another instance
                if (that.countryListDiv.hasClass("hide") && !that.telInput.prop("disabled")) {
                    that._showDropdown();
                }
            });
            // if the user has specified the path to the validation script
            // inject a new script element for it at the end of the body
            if (this.options.validationScript) {
                var injectValidationScript = function() {
                    var script = document.createElement("script");
                    script.type = "text/javascript";
                    script.src = that.options.validationScript;
                    document.body.appendChild(script);
                };
                // if the plugin is being initialised after the window.load event has already been fired
                if (windowLoaded) {
                    injectValidationScript();
                } else {
                    // wait until the load event so we don't block any other requests e.g. the flags image
                    $(window).load(injectValidationScript);
                }
            }
        },
        // on focus: if empty add dial code. on blur: if just dial code, then empty it
        _initAutoHideDialCode: function() {
            var that = this;
            // mousedown decides where the cursor goes, so if we're focusing
            // we must prevent this from happening
            this.telInput.on("mousedown" + this.ns, function(e) {
                if (!that.telInput.is(":focus") && !that.telInput.val()) {
                    e.preventDefault();
                    // but this also cancels the focus, so we must trigger that manually
                    that._focus();
                }
            });
            // on focus: if empty, insert the dial code for the currently selected flag
            this.telInput.on("focus" + this.ns, function() {
                if (!$.trim(that.telInput.val())) {
                    var countryData = that.getSelectedCountryData();
                    that._resetToDialCode(countryData.dialCode);
                    // after auto-inserting a dial code, if the first key they hit is '+' then assume
                    // they are entering a new number, so remove the dial code.
                    // use keypress instead of keydown because keydown gets triggered for the shift key
                    // (required to hit the + key), and instead of keyup because that shows the new '+'
                    // before removing the old one
                    that.telInput.one("keypress" + that.ns, function(e) {
                        if (e.which == keys.PLUS) {
                            that.telInput.val("");
                        }
                    });
                }
            });
            // on blur: if just a dial code then remove it
            this.telInput.on("blur" + this.ns, function() {
                var value = $.trim(that.telInput.val());
                if (value) {
                    if ($.trim(that._getDialCode(value) + that.options.dialCodeDelimiter) == value) {
                        that.telInput.val("");
                    }
                }
                that.telInput.off("keypress" + that.ns);
            });
        },
        // focus input and put the cursor at the end
        _focus: function() {
            this.telInput.focus();
            var input = this.telInput[0];
            // works for Chrome, FF, Safari, IE9+
            if (input.setSelectionRange) {
                var len = this.telInput.val().length;
                input.setSelectionRange(len, len);
            }
        },
        // show the dropdown
        _showDropdown: function() {
            this._setDropdownPosition();
            // update highlighting and scroll to active list item
            var activeListItem = this.countryList.children(".active");
            this._highlightListItem(activeListItem);
            // show it
            this.countryListDiv.removeClass("hide");
            this._scrollTo(activeListItem);
            // bind all the dropdown-related listeners: mouseover, click, click-off, keydown
            this._bindDropdownListeners();
            // update the arrow
            this.selectedFlagInner.children(".arrow").addClass("up");
        },
        //输入搜索内容
        _writeCurrentCountry: function() {
            var that = this;
            var filter = $("#searchInputNew").val();
            var all = this.countries;
            var newHtml = "";
            var className = "";
            if (filter && filter.length > 0) {
                for (var i = 0;  i < all.length; i++) {
                    var map = all[i];
                    var showname = map.name;
                    var dialCode = map.dialCode;
                    if (showname && (showname.toUpperCase().startsWith(filter.toUpperCase()) || showname.toUpperCase().indexOf(filter.toUpperCase()) > 0)) {
                        newHtml += "<li class='country " + className + "' data-dial-code='" + map.dialCode + "' data-country-code='" + map.iso2 + "'>"
                            + "<div class='flag " + map.iso2 + "'></div>"
                            + "<span class='country-name'>" + map.name + "</span>"
                            // + "<span class='dial-code'>+" + map.dialCode + "</span>"
                            + "</li>";
                    }
                }
                if(!newHtml) {
                    newHtml += "<li class='country-none text-center'>"
                        + "<div>" + MSG['findCodeNull'] + "</div>"
                        + "</li>";
                }
                this.countryList.html(newHtml);
            } else {
                this.countryList.html("");
                if (this.preferredCountries.length) {
                    this._appendListItems(this.preferredCountries, "preferred");
                    $("<li>", {"class": "divider"}).appendTo(this.countryList);
                }
                this._appendListItems(this.countries, "");
            }
        },

        // decide where to position dropdown (depends on position within viewport, and scroll)
        _setDropdownPosition: function() {
            var inputTop = this.telInput.offset().top, windowTop = $(window).scrollTop(), // dropdownFitsBelow = (dropdownBottom < windowBottom)
            dropdownFitsBelow = inputTop + this.telInput.outerHeight() + this.dropdownHeight < windowTop + $(window).height(), dropdownFitsAbove = inputTop - this.dropdownHeight > windowTop;
            // dropdownHeight - 1 for border
            var cssTop = !dropdownFitsBelow && dropdownFitsAbove ? "-" + (this.dropdownHeight - 1) + "px" : "";
            this.countryListDiv.css("top", cssTop);
        },
        // we only bind dropdown listeners when the dropdown is open
        _bindDropdownListeners: function() {
            var that = this;
            // when mouse over a list item, just highlight that one
            // we add the class "highlight", so if they hit "enter" we know which one to select
            this.countryList.on("mouseover" + this.ns, ".country", function(e) {
                that._highlightListItem($(this));
            });
            // listen for country selection
            this.countryList.on("click" + this.ns, ".country", function(e) {
                that._selectListItem($(this));
            });
            // click off to close
            // (except when this initial opening click is bubbling up)
            // we cannot just stopPropagation as it may be needed to close another instance
            var isOpening = true;
            $("html").on("click" + this.ns, function(e) {
                if (!isOpening) {
                    that._closeDropdown();
                }
                isOpening = false;
            });
            this.countryListDiv.on("click" + this.ns, ".search-input-new", function() {
                isOpening = true;
            });
            this.countryListDiv.on('keyup' + this.ns, ".search-input-new", function() {
                that._writeCurrentCountry();
            });
            // listen for up/down scrolling, enter to select, or letters to jump to country name.
            // use keydown as keypress doesn't fire for non-char keys and we want to catch if they
            // just hit down and hold it to scroll down (no keyup event).
            // listen on the document because that's where key events are triggered if no input has focus
            $(document).on("keydown" + this.ns, function(e) {
                // prevent down key from scrolling the whole page,
                // and enter key from submitting a form etc
                // e.preventDefault();
                // if (e.which == keys.UP || e.which == keys.DOWN) {
                //     // up and down to navigate
                //     that._handleUpDownKey(e.which);
                // } else if (e.which == keys.ENTER) {
                //     // enter to select
                //     that._handleEnterKey();
                // }
                // else if (e.which == keys.ESC) {
                //     // esc to close
                //     that._closeDropdown();
                // } else if (e.which >= keys.A && e.which <= keys.Z) {
                //     // upper case letters (note: keyup/keydown only return upper case letters)
                //     // cycle through countries beginning with that letter
                //     that._handleLetterKey(e.which);
                // } else if (e.which == keys.BACKSPACE) {
                //     e = e ? e : event;
                //     var target = $.event.fix(e).target;
                //     if($(target).attr("id")=="searchInput"){//该处的fileNam是对应的input输入框的id的值
                //         $(target).val("");
                //     }
                // }
            });
        },
        // highlight the next/prev item in the list (and ensure it is visible)
        _handleUpDownKey: function(key) {
            var current = this.countryList.children(".highlight").first();
            var next = key == keys.UP ? current.prev() : current.next();
            if (next.length) {
                // skip the divider
                if (next.hasClass("divider")) {
                    next = key == keys.UP ? next.prev() : next.next();
                }
                this._highlightListItem(next);
                this._scrollTo(next);
            }
        },
        // select the currently highlighted item
        _handleEnterKey: function() {
            var currentCountry = this.countryList.children(".highlight").first();
            if (currentCountry.length) {
                this._selectListItem(currentCountry);
            }
        },
        // iterate through the countries starting with the given letter
        _handleLetterKey: function(key) {
            var letter = String.fromCharCode(key);
            // filter out the countries beginning with that letter
            var countries = this.countryListItems.filter(function() {
                return $(this).text().charAt(0) == letter && !$(this).hasClass("preferred");
            });
            if (countries.length) {
                // if one is already highlighted, then we want the next one
                var highlightedCountry = countries.filter(".highlight").first(), listItem;
                // if the next country in the list also starts with that letter
                if (highlightedCountry && highlightedCountry.next() && highlightedCountry.next().text().charAt(0) == letter) {
                    listItem = highlightedCountry.next();
                } else {
                    listItem = countries.first();
                }
                // update highlighting and scroll
                this._highlightListItem(listItem);
                this._scrollTo(listItem);
            }
        },
        // update the selected flag using the input's current value
        _updateFlagFromInputVal: function() {
            var that = this;
            // try and extract valid dial code from input
            var dialCode = this._getDialCode(this.telInput.val());
            if (dialCode) {
                // check if one of the matching countries is already selected
                var countryCodes = this.countryCodes[dialCode.replace(/\D/g, "")], alreadySelected = false;
                $.each(countryCodes, function(i, c) {
                    if (that.selectedFlagInner.hasClass(c)) {
                        alreadySelected = true;
                    }
                });
                if (!alreadySelected) {
                    this._selectFlag(countryCodes[0]);
                }
                // valid international dial code found
                return true;
            }
            // valid international dial code not found
            return false;
        },
        // reset the input value to just a dial code
        _resetToDialCode: function(dialCode) {
            // if nationalMode is enabled then don't insert the dial code
            var value = this.options.nationalMode ? "" : "+" + dialCode + this.options.dialCodeDelimiter;
            this.telInput.val(value);
        },
        // remove highlighting from other list items and highlight the given item
        _highlightListItem: function(listItem) {
            this.countryListItems.removeClass("highlight");
            listItem.addClass("highlight").siblings().removeClass("highlight");
        },
        // find the country data for the given country code
        // the ignoreOnlyCountriesOption is only used during init() while parsing the onlyCountries array
        _getCountryData: function(countryCode, ignoreOnlyCountriesOption) {
            var countryList = ignoreOnlyCountriesOption ? allCountries : this.countries;
            for (var i = 0; i < countryList.length; i++) {
                if (countryList[i].iso2 == countryCode) {
                    return countryList[i];
                }
            }
            return null;
        },
        // update the selected flag and the active list item
        _selectFlag: function(countryCode) {
            this.selectedFlagInner.attr("class", "flag " + countryCode);
            // update the title attribute
            var countryData = this._getCountryData(countryCode);
            // this.selectedFlagInner.parent().attr("title", countryData.name + ": +" + countryData.dialCode);
            // update the active list item
            var listItem = this.countryListItems.children(".flag." + countryCode).first().parent();
            this.countryListItems.removeClass("active");
            listItem.addClass("active");
            $("#countryOrregion").val(countryData.name);
            $("#countryCode").val(countryData.iso2);
            $("#currCountryName").val(countryData.name);
            choseCityCode = true;
            $(".label-country").hide();
        },
        // called when the user selects a list item from the dropdown
        _selectListItem: function(listItem) {
            // update selected flag and active list item
            var countryCode = listItem.attr("data-country-code");
            this._selectFlag(countryCode);
            this._closeDropdown();
            // update input value
            if (!this.options.nationalMode) {
                this._updateNumber("+" + listItem.attr("data-dial-code"));
                this.telInput.trigger("change");
            }
            // focus the input
            this._focus();
        },
        // close the dropdown and unbind any listeners
        _closeDropdown: function() {
            this.countryListDiv.addClass("hide");
            // update the arrow
            this.selectedFlagInner.children(".arrow").removeClass("up");
            // unbind event listeners
            $(document).off("keydown" + this.ns);
            $("html").off("click" + this.ns);
            $("#searchInputNew").val("");//关闭下拉框后，搜索框内的值这位空
            this.countryList.html("");
            this.countryList.off("click" + this.ns);
            this.countryList.off("mouseover" + this.ns);
            // unbind both hover and click listeners
            this.countryList.off(this.ns);
            if (this.preferredCountries.length) {
                // $("<li>", {"class": "divider"}).appendTo(this.countryList);
                this._appendListItems(this.preferredCountries, "preferred");
            }
            this._appendListItems(this.countries, "");
        },
        // check if an element is visible within it's container, else scroll until it is
        _scrollTo: function(element) {
            var container = this.countryListDiv, 
            containerHeight = container.height(), 
            containerTop = container.offset().top, 
            containerBottom = containerTop + containerHeight, 
            elementHeight = element.outerHeight(), 
            elementTop = element ? element.offset() ? element.offset().top : 0 : 0, 
            elementBottom = elementTop + elementHeight, 
            newScrollTop = elementTop - containerTop + container.scrollTop();
            if (elementTop < containerTop) {
                // scroll up
                container.scrollTop(newScrollTop);
            } else if (elementBottom > containerBottom) {
                // scroll down
                var heightDifference = containerHeight - elementHeight;
                container.scrollTop(newScrollTop - heightDifference);
            }
        },
        // replace any existing dial code with the new one
        _updateNumber: function(newDialCode) {
            var inputVal = this.telInput.val(), prevDialCode = this._getDialCode(inputVal), newNumber;
            // if the previous number contained a valid dial code, replace it
            // (if more than just a plus character)
            if (prevDialCode.length > 1) {
                newNumber = inputVal.replace(prevDialCode, newDialCode);
                // if the old number was just the dial code,
                // then we will need to add the space again
                if (inputVal == prevDialCode) {
                    newNumber += this.options.dialCodeDelimiter;
                }
            } else {
                // if the previous number didn't contain a dial code, we should persist it
                var existingNumber = inputVal && inputVal.substr(0, 1) != "+" ? $.trim(inputVal) : "";
                newNumber = newDialCode + this.options.dialCodeDelimiter + existingNumber;
            }
            this.telInput.val(newNumber);
        },
        // try and extract a valid international dial code from a full telephone number
        // Note: returns the raw string inc plus character and any whitespace/dots etc
        _getDialCode: function(inputVal) {
            var dialCode = "";
            inputVal = $.trim(inputVal);
            // only interested in international numbers (starting with a plus)
            if (inputVal.charAt(0) == "+") {
                var numericChars = "";
                // iterate over chars
                for (var i = 0; i < inputVal.length; i++) {
                    var c = inputVal.charAt(i);
                    // if char is number
                    if ($.isNumeric(c)) {
                        numericChars += c;
                        // if current numericChars make a valid dial code
                        if (this.countryCodes[numericChars]) {
                            // store the actual raw string (useful for matching later)
                            dialCode = inputVal.substring(0, i + 1);
                        }
                        // longest dial code is 4 chars
                        if (numericChars.length == 4) {
                            break;
                        }
                    }
                }
            }
            return dialCode;
        },
        /********************
     *  PUBLIC METHODS
     ********************/
        // get the country data for the currently selected flag
        getSelectedCountryData: function() {
            // rely on the fact that we only set 2 classes on the selected flag element:
            // the first is "flag" and the second is the 2-char country code
            var countryCode = this.selectedFlagInner.attr("class").split(" ")[1];
            return this._getCountryData(countryCode);
        },
        // validate the input val - assumes the global function isValidNumber
        // pass in true if you want to allow national numbers (no country dial code)
        isValidNumber: function(allowNational) {
            var val = $.trim(this.telInput.val()), countryData = this.getSelectedCountryData(), countryCode = allowNational ? countryData.iso2 : "";
            return window.isValidNumber(val, countryCode);
        },
        // update the selected flag, and insert the dial code
        selectCountry: function(countryCode) {
            // check if already selected
            if (!this.selectedFlagInner.hasClass(countryCode)) {
                this._selectFlag(countryCode);
                if (!this.options.autoHideDialCode) {
                    var countryData = this._getCountryData(countryCode, false);
                    this._resetToDialCode(countryData.dialCode);
                }
            }
        },
        // set the input value and update the flag
        setNumber: function(number) {
            this.telInput.val(number);
            this._updateFlagFromInputVal();
        },
        // remove plugin
        destroy: function() {
            // stop listeners
            this.telInput.off(this.ns);
            this.selectedFlagInner.parent().off(this.ns);
            // remove markup
            var container = this.telInput.parent();
            container.before(this.telInput).remove();
        }
    };
    // adapted to allow public functions
    // using https://github.com/jquery-boilerplate/jquery-boilerplate/wiki/Extending-jQuery-Boilerplate
    $.fn[pluginName] = function(options) {
        var args = arguments;
        // Is the first parameter an object (options), or was omitted,
        // instantiate a new instance of the plugin.
        if (options === undefined || typeof options === "object") {
            return this.each(function() {
                if (!$.data(this, "plugin_" + pluginName)) {
                    $.data(this, "plugin_" + pluginName, new Plugin(this, options));
                }
            });
        } else if (typeof options === "string" && options[0] !== "_" && options !== "init") {
            // If the first parameter is a string and it doesn't start
            // with an underscore or "contains" the `init`-function,
            // treat this as a call to a public method.
            // Cache the method call to make it possible to return a value
            var returns;
            this.each(function() {
                var instance = $.data(this, "plugin_" + pluginName);
                // Tests that there's already a plugin-instance
                // and checks that the requested public method exists
                if (instance instanceof Plugin && typeof instance[options] === "function") {
                    // Call the method of our plugin instance,
                    // and pass it the supplied arguments.
                    returns = instance[options].apply(instance, Array.prototype.slice.call(args, 1));
                }
                // Allow instances to be destroyed via the 'destroy' method
                if (options === "destroy") {
                    $.data(this, "plugin_" + pluginName, null);
                }
            });
            // If the earlier cached method gives a value back return the value,
            // otherwise return this to preserve chainability.
            return returns !== undefined ? returns : this;
        }
    };
    /*多语言*/
    var message = {};
    message["zh_CN"] = {
        // Afghanistan: "阿富汗",
        Albania: "阿尔巴尼亚",
        Algeria: "阿尔及利亚",
        AmericanSamoa: "美属萨摩亚",
        Andorra: "安道尔",
        Angola: "安哥拉",
        Anguilla: "安圭拉",
        AntiguaAndBarbuda: "安提瓜和巴布达",
        Argentina: "阿根廷",
        Armenia: "亚美尼亚",
        Aruba: "阿鲁巴",
        Australia: "澳大利亚",
        Austria: "奥地利",
        Azerbaijan: "阿塞拜疆",
        Bahamas: "巴哈马",
        Bahrain: "巴林",
        Bangladesh: "孟加拉国",
        Barbados: "巴巴多斯",
        Belarus: "白俄罗斯",
        Belgium: "比利时",
        Belize: "伯利兹",
        Benin: "贝宁",
        Bermuda: "百慕大群岛",
        Bhutan: "不丹",
        Bolivia: "玻利维亚",
        BosniaAndHerzegovina: "波斯尼亚和黑塞哥维那",
        Botswana: "博茨瓦纳",
        Brazil: "巴西",
        Brunei: "文莱",
        Bulgaria: "保加利亚",
        BurkinaFaso: "布基纳法索",
        Burundi: "布隆迪",
        Cambodia: "柬埔寨",
        Cameroon: "喀麦隆",
        Canada: "加拿大",
        CapeVerde: "开普",
        CaymanIslands: "开曼群岛",
        CentralAfricanRepublic: "中非共和国",
        Chad: "乍得",
        Chile: "智利",
        // China: "中国大陆",
        Colombia: "哥伦比亚",
        Comoros: "科摩罗",
        CookIslands: "库克群岛",
        CostaRica: "哥斯达黎加",
        Croatia: "克罗地亚",
        Cuba: "古巴",
        Curacao: "库拉索",
        Cyprus: "塞浦路斯",
        Czech: "捷克",
        DemocraticRepublicOfTheCongo: "刚果民主共和国",
        Denmark: "丹麦",
        Djibouti: "吉布提",
        Dominica: "多米尼加",
        DominicanRepublic: "多米尼加共和国",
        Ecuador: "厄瓜多尔",
        Egypt: "埃及",
        ElSalvador: "萨尔瓦多",
        EquatorialGuinea: "赤道几内亚",
        Eritrea: "厄立特里亚",
        Estonia: "爱沙尼亚",
        Ethiopia: "埃塞俄比亚",
        FaroeIslands: "法罗群岛",
        Fiji: "斐济",
        Finland: "芬兰",
        France: "法国",
        FrenchGuiana: "法属圭亚那",
        FrenchPolynesia: "法属波利尼西亚",
        Gabon: "加蓬",
        Gambia: "冈比亚",
        Georgia: "格鲁吉亚",
        Germany: "德国",
        Ghana: "加纳",
        Gibraltar: "直布罗陀",
        Greece: "希腊",
        Greenland: "格陵兰岛",
        Grenada: "格林纳达",
        Guadeloupe: "瓜德罗普岛",
        Guam: "关岛",
        Guatemala: "瓜地马拉",
        Guinea: "几内亚",
        GuineaBissau: "几内亚比绍共和国",
        Guyana: "圭亚那",
        Haiti: "海地",
        Honduras: "洪都拉斯",
        HongKong: "中国香港",
        Hungary: "匈牙利",
        Iceland: "冰岛",
        India: "印度",
        Indonesia: "印度尼西亚",
        Iran: "伊朗",
        Iraq: "伊拉克",
        Ireland: "爱尔兰",
        Israel: "以色列",
        Italy: "意大利",
        IvoryCoast: "象牙海岸",
        Jamaica: "牙买加",
        Japan: "日本",
        Jordan: "约旦",
        Kazakhstan: "哈萨克斯坦",
        Kenya: "肯尼亚",
        Kiribati: "基里巴斯",
        Kuwait: "科威特",
        Kyrgyzstan: "吉尔吉斯斯坦",
        Laos: "老挝",
        Latvia: "拉脱维亚",
        Lebanon: "黎巴嫩",
        Lesotho: "莱索托",
        Liberia: "利比里亚",
        Libya: "利比亚",
        Liechtenstein: "列支敦士登",
        Lithuania: "立陶宛",
        Luxembourg: "卢森堡",
        Macau: "中国澳门",
        Macedonia: "马其顿",
        Madagascar: "马达加斯加",
        Malawi: "马拉维",
        // Malaysia: "马来西亚",
        Maldives: "马尔代夫",
        Mali: "马里",
        Malta: "马耳他",
        Martinique: "马提尼克",
        Mauritania: "毛里塔尼亚",
        Mauritius: "毛里求斯",
        Mayotte: "马约特",
        Mexico: "墨西哥",
        Moldova: "摩尔多瓦",
        Monaco: "摩纳哥",
        Mongolia: "蒙古",
        Montenegro: "黑山",
        Montserrat: "蒙特塞拉特岛",
        Morocco: "摩洛哥",
        Mozambique: "莫桑比克",
        Myanmar: "缅甸",
        Namibia: "纳米比亚",
        Nepal: "尼泊尔",
        Netherlands: "荷兰",
        NewCaledonia: "新喀里多尼亚",
        NewZealand: "新西兰",
        Nicaragua: "尼加拉瓜",
        Niger: "尼日尔",
        Nigeria: "尼日利亚",
        Norway: "挪威",
        Oman: "阿曼",
        Pakistan: "巴基斯坦",
        Palau: "帕劳",
        Palestine: "巴勒斯坦",
        Panama: "巴拿马",
        PapuaNewGuinea: "巴布亚新几内亚",
        Paraguay: "巴拉圭",
        Peru: "秘鲁",
        Philippines: "菲律宾",
        Poland: "波兰",
        Portugal: "葡萄牙",
        PuertoRico: "波多黎各",
        Qatar: "卡塔尔",
        RepublicOfTheCongo: "刚果共和国",
        ReunionIsland: "留尼汪",
        Romania: "罗马尼亚",
        Russia: "俄罗斯",
        Rwanda: "卢旺达",
        SaintKittsandNevis: "圣基茨和尼维斯",
        SaintLucia: "圣露西亚",
        SaintPierreandMiquelon: "圣彼埃尔和密克隆岛",
        SaintVincentandtheGrenadines: "圣文森特和格林纳丁斯",
        Samoa: "萨摩亚",
        SanMarino: "圣马力诺",
        SaoTomeandPrincipe: "圣多美和普林西比",
        SaudiArabia: "沙特阿拉伯",
        Senegal: "塞内加尔",
        Serbia: "塞尔维亚",
        Seychelles: "塞舌尔",
        SierraLeone: "塞拉利昂",
        Singapore: "新加坡",
        SaintMaarten: "圣马丁岛（荷兰部分）",
        Slovakia: "斯洛伐克",
        Slovenia: "斯洛文尼亚",
        SolomonIslands: "所罗门群岛",
        Somalia: "索马里",
        SouthAfrica: "南非",
        SouthKorea: "韩国",
        Spain: "西班牙",
        SriLanka:"斯里兰卡",
        Sudan: "苏丹",
        Suriname: "苏里南",
        Swaziland: "斯威士兰",
        Sweden: "瑞典",
        Switzerland: "瑞士",
        Syria: "叙利亚",
        Taiwan: "中国台湾",
        Tajikistan: "塔吉克斯坦",
        Tanzania: "坦桑尼亚",
        Thailand: "泰国",
        TimorLeste: "东帝汶",
        Togo: "多哥",
        Tonga: "汤加",
        TrinidadandTobago: "特立尼达和多巴哥",
        Tunisia: "突尼斯",
        Turkey: "土耳其",
        Turkmenistan: "土库曼斯坦",
        TurksandCaicosIslands: "特克斯和凯科斯群岛",
        Uganda: "乌干达",
        Ukraine: "乌克兰",
        UnitedArabEmirates: "阿拉伯联合酋长国",
        UnitedKingdom: "英国",
        // UnitedStates: "美国",
        Uruguay: "乌拉圭",
        Uzbekistan: "乌兹别克斯坦",
        Vanuatu: "瓦努阿图",
        Venezuela: "委内瑞拉",
        Vietnam: "越南",
        VirginIslandsBritish: "英属处女群岛",
        VirginIslandsUS: "美属维尔京群岛",
        Yemen: "也门",
        Zambia: "赞比亚",
        Zimbabwe: "津巴布韦",
        findCodeNull: "抱歉，未找到匹配的区号",
    };
    message["zh_HK"] = {
        // Afghanistan: "Afghanistan",
        Albania: "Albania",
        Algeria: "Algeria",
        AmericanSamoa: "American Samoa",
        Andorra: "Andorra",
        Angola: "Angola",
        Anguilla: "Anguilla",
        AntiguaAndBarbuda: "Antigua and Barbuda",
        Argentina: "Argentina",
        Armenia: "Armenia",
        Aruba: "Aruba",
        Australia: "Australia",
        Austria: "Austria",
        Azerbaijan: "Azerbaijan",
        Bahamas: "Bahamas",
        Bahrain: "Bahrain",
        Bangladesh: "Bangladesh",
        Barbados: "Barbados",
        Belarus: "Belarus",
        Belgium: "Belgium",
        Belize: "Belize",
        Benin: "Benin",
        Bermuda: "Bermuda",
        Bhutan: "Bhutan",
        Bolivia: "Bolivia",
        BosniaAndHerzegovina: "Bosnia and Herzegovina",
        Botswana: "Botswana",
        Brazil: "Brazil",
        Brunei: "Brunei",
        Bulgaria: "Bulgaria",
        BurkinaFaso: "Burkina Faso",
        Burundi: "Burundi",
        Cambodia: "Cambodia",
        Cameroon: "Cameroon",
        Canada: "Canada",
        CapeVerde: "Cape Verde",
        CaymanIslands: "Cayman Islands",
        CentralAfricanRepublic: "Central African Republic",
        Chad: "Chad",
        Chile: "Chile",
        // China: "China",
        Colombia: "Colombia",
        Comoros: "Comoros",
        CookIslands: "Cook Islands",
        CostaRica: "Costa Rica",
        Croatia: "Croatia",
        Cuba: "Cuba",
        Curacao: "Curacao",
        Cyprus: "Cyprus",
        Czech: "Czech",
        DemocraticRepublicOfTheCongo: "Democratic Republic of the Congo",
        Denmark: "Denmark",
        Djibouti: "Djibouti",
        Dominica: "Dominica",
        DominicanRepublic: "Dominican Republic",
        Ecuador: "Ecuador",
        Egypt: "Egypt",
        ElSalvador: "El Salvador",
        EquatorialGuinea: "Equatorial Guinea",
        Eritrea: "Eritrea",
        Estonia: "Estonia",
        Ethiopia: "Ethiopia",
        FaroeIslands: "Faroe Islands",
        Fiji: "Fiji",
        Finland: "Finland",
        France: "France",
        FrenchGuiana: "French Guiana",
        FrenchPolynesia: "French Polynesia",
        Gabon: "Gabon",
        Gambia: "Gambia",
        Georgia: "Georgia",
        Germany: "Germany",
        Ghana: "Ghana",
        Gibraltar: "Gibraltar",
        Greece: "Greece",
        Greenland: "Greenland",
        Grenada: "Grenada",
        Guadeloupe: "Guadeloupe",
        Guam: "Guam",
        Guatemala: "Guatemala",
        Guinea: "Guinea",
        GuineaBissau: "Guinea-Bissau",
        Guyana: "Guyana",
        Haiti: "Haiti",
        Honduras: "Honduras",
        HongKong: "Hong Kong",
        Hungary: "Hungary",
        Iceland: "Iceland",
        India: "India",
        Indonesia: "Indonesia",
        Iran: "Iran",
        Iraq: "Iraq",
        Ireland: "Ireland",
        Israel: "Israel",
        Italy: "Italy",
        IvoryCoast: "Ivory Coast",
        Jamaica: "Jamaica",
        Japan: "Japan",
        Jordan: "Jordan",
        Kazakhstan: "Kazakhstan",
        Kenya: "Kenya",
        Kiribati: "Kiribati",
        Kuwait: "Kuwait‎",
        Kyrgyzstan: "Kyrgyzstan",
        Laos: "Laos",
        Latvia: "Latvia",
        Lebanon: "Lebanon",
        Lesotho: "Lesotho",
        Liberia: "Liberia",
        Libya: "Libya",
        Liechtenstein: "Liechtenstein",
        Lithuania: "Lithuania",
        Luxembourg: "Luxembourg",
        Macau: "Macau",
        Macedonia: "Macedonia",
        Madagascar: "Madagascar",
        Malawi: "Malawi",
        // Malaysia: "Malaysia",
        Maldives: "Maldives",
        Mali: "Mali",
        Malta: "Malta",
        Martinique: "Martinique",
        Mauritania: "Mauritania",
        Mauritius: "Mauritius",
        Mayotte: "Mayotte",
        Mexico: "Mexico",
        Moldova: "Moldova",
        Monaco: "Monaco",
        Mongolia: "Mongolia",
        Montenegro: "Montenegro",
        Montserrat: "Montserrat",
        Morocco: "Morocco",
        Mozambique: "Mozambique",
        Myanmar: "Myanmar",
        Namibia: "Namibia",
        Nepal: "Nepal",
        Netherlands: "Netherlands",
        NewCaledonia: "New Caledonia",
        NewZealand: "New Zealand",
        Nicaragua: "Nicaragua",
        Niger: "Niger",
        Nigeria: "Nigeria",
        Norway: "Norway",
        Oman: "Oman",
        Pakistan: "Pakistan",
        Palau: "Palau",
        Palestine: "Palestine",
        Panama: "Panama",
        PapuaNewGuinea: "Papua New Guinea",
        Paraguay: "Paraguay",
        Peru: "Peru",
        Philippines: "Philippines",
        Poland: "Poland",
        Portugal: "Portugal",
        PuertoRico: "Puerto Rico",
        Qatar: "Qatar",
        RepublicOfTheCongo: "Republic Of The Congo",
        ReunionIsland: "Réunion Island",
        Romania: "Romania",
        Russia: "Russia",
        Rwanda: "Rwanda",
        SaintKittsandNevis: "Saint Kitts and Nevis",
        SaintLucia: "Saint Lucia",
        SaintPierreandMiquelon: "Saint Pierre and Miquelon",
        SaintVincentandtheGrenadines: "Saint Vincent and the Grenadines",
        Samoa: "Samoa",
        SanMarino: "San Marino",
        SaoTomeandPrincipe: "Sao Tome and Principe",
        SaudiArabia: "SaudiArabia",
        Senegal: "Senegal",
        Serbia: "Serbia",
        Seychelles: "Seychelles",
        SierraLeone: "Sierra Leone",
        Singapore: "Singapore",
        SaintMaarten: "Saint Maarten",
        Slovakia: "Slovakia",
        Slovenia: "Slovenia",
        SolomonIslands: "Solomon Islands",
        Somalia: "Somalia",
        SouthAfrica: "South Africa",
        SouthKorea: "South Korea",
        Spain: "Spain",
        SriLanka:"Sri Lanka",
        Sudan: "Sudan",
        Suriname: "Suriname",
        Swaziland: "Swaziland",
        Sweden: "Sweden",
        Switzerland: "Switzerland",
        Syria: "Syria",
        Taiwan: "Taiwan",
        Tajikistan: "Tajikistan",
        Tanzania: "Tanzania",
        Thailand: "Thailand",
        TimorLeste: "Timor-Leste",
        Togo: "Togo",
        Tonga: "Tonga",
        TrinidadandTobago: "Trinidad and Tobago",
        Tunisia: "Tunisia",
        Turkey: "Turkey",
        Turkmenistan: "Turkmenistan",
        TurksandCaicosIslands: "Turks and Caicos Islands",
        Uganda: "Uganda",
        Ukraine: "Ukraine",
        UnitedArabEmirates: "United Arab Emirates",
        UnitedKingdom: "United Kingdom",
        // UnitedStates: "United States",
        Uruguay: "Uruguay",
        Uzbekistan: "Uzbekistan",
        Vanuatu: "Vanuatu",
        Venezuela: "Venezuela",
        Vietnam: "Vietnam",
        VirginIslandsBritish: "Virgin Islands,British",
        VirginIslandsUS: "Virgin Islands,US",
        Yemen: "Yemen",
        Zambia: "Zambia",
        Zimbabwe: "Zimbabwe",
        findCodeNull: "Sorry, no matching area code was found",
    };
    message["en_US"] = {
        // Afghanistan: "Afghanistan",
        Albania: "Albania",
        Algeria: "Algeria",
        AmericanSamoa: "American Samoa",
        Andorra: "Andorra",
        Angola: "Angola",
        Anguilla: "Anguilla",
        AntiguaAndBarbuda: "Antigua and Barbuda",
        Argentina: "Argentina",
        Armenia: "Armenia",
        Aruba: "Aruba",
        Australia: "Australia",
        Austria: "Austria",
        Azerbaijan: "Azerbaijan",
        Bahamas: "Bahamas",
        Bahrain: "Bahrain",
        Bangladesh: "Bangladesh",
        Barbados: "Barbados",
        Belarus: "Belarus",
        Belgium: "Belgium",
        Belize: "Belize",
        Benin: "Benin",
        Bermuda: "Bermuda",
        Bhutan: "Bhutan",
        Bolivia: "Bolivia",
        BosniaAndHerzegovina: "Bosnia and Herzegovina",
        Botswana: "Botswana",
        Brazil: "Brazil",
        Brunei: "Brunei",
        Bulgaria: "Bulgaria",
        BurkinaFaso: "Burkina Faso",
        Burundi: "Burundi",
        Cambodia: "Cambodia",
        Cameroon: "Cameroon",
        Canada: "Canada",
        CapeVerde: "Cape Verde",
        CaymanIslands: "Cayman Islands",
        CentralAfricanRepublic: "Central African Republic",
        Chad: "Chad",
        Chile: "Chile",
        // China: "China",
        Colombia: "Colombia",
        Comoros: "Comoros",
        CookIslands: "Cook Islands",
        CostaRica: "Costa Rica",
        Croatia: "Croatia",
        Cuba: "Cuba",
        Curacao: "Curacao",
        Cyprus: "Cyprus",
        Czech: "Czech",
        DemocraticRepublicOfTheCongo: "Democratic Republic of the Congo",
        Denmark: "Denmark",
        Djibouti: "Djibouti",
        Dominica: "Dominica",
        DominicanRepublic: "Dominican Republic",
        Ecuador: "Ecuador",
        Egypt: "Egypt",
        ElSalvador: "El Salvador",
        EquatorialGuinea: "Equatorial Guinea",
        Eritrea: "Eritrea",
        Estonia: "Estonia",
        Ethiopia: "Ethiopia",
        FaroeIslands: "Faroe Islands",
        Fiji: "Fiji",
        Finland: "Finland",
        France: "France",
        FrenchGuiana: "French Guiana",
        FrenchPolynesia: "French Polynesia",
        Gabon: "Gabon",
        Gambia: "Gambia",
        Georgia: "Georgia",
        Germany: "Germany",
        Ghana: "Ghana",
        Gibraltar: "Gibraltar",
        Greece: "Greece",
        Greenland: "Greenland",
        Grenada: "Grenada",
        Guadeloupe: "Guadeloupe",
        Guam: "Guam",
        Guatemala: "Guatemala",
        Guinea: "Guinea",
        GuineaBissau: "Guinea-Bissau",
        Guyana: "Guyana",
        Haiti: "Haiti",
        Honduras: "Honduras",
        HongKong: "Hong Kong",
        Hungary: "Hungary",
        Iceland: "Iceland",
        India: "India",
        Indonesia: "Indonesia",
        Iran: "Iran",
        Iraq: "Iraq",
        Ireland: "Ireland",
        Israel: "Israel",
        Italy: "Italy",
        IvoryCoast: "Ivory Coast",
        Jamaica: "Jamaica",
        Japan: "Japan",
        Jordan: "Jordan",
        Kazakhstan: "Kazakhstan",
        Kenya: "Kenya",
        Kiribati: "Kiribati",
        Kuwait: "Kuwait‎",
        Kyrgyzstan: "Kyrgyzstan",
        Laos: "Laos",
        Latvia: "Latvia",
        Lebanon: "Lebanon",
        Lesotho: "Lesotho",
        Liberia: "Liberia",
        Libya: "Libya",
        Liechtenstein: "Liechtenstein",
        Lithuania: "Lithuania",
        Luxembourg: "Luxembourg",
        Macau: "Macau",
        Macedonia: "Macedonia",
        Madagascar: "Madagascar",
        Malawi: "Malawi",
        // Malaysia: "Malaysia",
        Maldives: "Maldives",
        Mali: "Mali",
        Malta: "Malta",
        Martinique: "Martinique",
        Mauritania: "Mauritania",
        Mauritius: "Mauritius",
        Mayotte: "Mayotte",
        Mexico: "Mexico",
        Moldova: "Moldova",
        Monaco: "Monaco",
        Mongolia: "Mongolia",
        Montenegro: "Montenegro",
        Montserrat: "Montserrat",
        Morocco: "Morocco",
        Mozambique: "Mozambique",
        Myanmar: "Myanmar",
        Namibia: "Namibia",
        Nepal: "Nepal",
        Netherlands: "Netherlands",
        NewCaledonia: "New Caledonia",
        NewZealand: "New Zealand",
        Nicaragua: "Nicaragua",
        Niger: "Niger",
        Nigeria: "Nigeria",
        Norway: "Norway",
        Oman: "Oman",
        Pakistan: "Pakistan",
        Palau: "Palau",
        Palestine: "Palestine",
        Panama: "Panama",
        PapuaNewGuinea: "Papua New Guinea",
        Paraguay: "Paraguay",
        Peru: "Peru",
        Philippines: "Philippines",
        Poland: "Poland",
        Portugal: "Portugal",
        PuertoRico: "Puerto Rico",
        Qatar: "Qatar",
        RepublicOfTheCongo: "Republic Of The Congo",
        ReunionIsland: "Réunion Island",
        Romania: "Romania",
        Russia: "Russia",
        Rwanda: "Rwanda",
        SaintKittsandNevis: "Saint Kitts and Nevis",
        SaintLucia: "Saint Lucia",
        SaintPierreandMiquelon: "Saint Pierre and Miquelon",
        SaintVincentandtheGrenadines: "Saint Vincent and the Grenadines",
        Samoa: "Samoa",
        SanMarino: "San Marino",
        SaoTomeandPrincipe: "Sao Tome and Principe",
        SaudiArabia: "SaudiArabia",
        Senegal: "Senegal",
        Serbia: "Serbia",
        Seychelles: "Seychelles",
        SierraLeone: "Sierra Leone",
        Singapore: "Singapore",
        SaintMaarten: "Saint Maarten",
        Slovakia: "Slovakia",
        Slovenia: "Slovenia",
        SolomonIslands: "Solomon Islands",
        Somalia: "Somalia",
        SouthAfrica: "South Africa",
        SouthKorea: "South Korea",
        Spain: "Spain",
        SriLanka:"Sri Lanka",
        Sudan: "Sudan",
        Suriname: "Suriname",
        Swaziland: "Swaziland",
        Sweden: "Sweden",
        Switzerland: "Switzerland",
        Syria: "Syria",
        Taiwan: "Taiwan",
        Tajikistan: "Tajikistan",
        Tanzania: "Tanzania",
        Thailand: "Thailand",
        TimorLeste: "Timor-Leste",
        Togo: "Togo",
        Tonga: "Tonga",
        TrinidadandTobago: "Trinidad and Tobago",
        Tunisia: "Tunisia",
        Turkey: "Turkey",
        Turkmenistan: "Turkmenistan",
        TurksandCaicosIslands: "Turks and Caicos Islands",
        Uganda: "Uganda",
        Ukraine: "Ukraine",
        UnitedArabEmirates: "United Arab Emirates",
        UnitedKingdom: "United Kingdom",
        // UnitedStates: "United States",
        Uruguay: "Uruguay",
        Uzbekistan: "Uzbekistan",
        Vanuatu: "Vanuatu",
        Venezuela: "Venezuela",
        Vietnam: "Vietnam",
        VirginIslandsBritish: "Virgin Islands,British",
        VirginIslandsUS: "Virgin Islands,US",
        Yemen: "Yemen",
        Zambia: "Zambia",
        Zimbabwe: "Zimbabwe",
        findCodeNull: "Sorry, no matching area code was found",
    };
    message["ja_JP"] = {
        // Afghanistan: "Afghanistan",
        Albania: "Albania",
        Algeria: "Algeria",
        AmericanSamoa: "American Samoa",
        Andorra: "Andorra",
        Angola: "Angola",
        Anguilla: "Anguilla",
        AntiguaAndBarbuda: "Antigua and Barbuda",
        Argentina: "Argentina",
        Armenia: "Armenia",
        Aruba: "Aruba",
        Australia: "Australia",
        Austria: "Austria",
        Azerbaijan: "Azerbaijan",
        Bahamas: "Bahamas",
        Bahrain: "Bahrain",
        Bangladesh: "Bangladesh",
        Barbados: "Barbados",
        Belarus: "Belarus",
        Belgium: "Belgium",
        Belize: "Belize",
        Benin: "Benin",
        Bermuda: "Bermuda",
        Bhutan: "Bhutan",
        Bolivia: "Bolivia",
        BosniaAndHerzegovina: "Bosnia and Herzegovina",
        Botswana: "Botswana",
        Brazil: "Brazil",
        Brunei: "Brunei",
        Bulgaria: "Bulgaria",
        BurkinaFaso: "Burkina Faso",
        Burundi: "Burundi",
        Cambodia: "Cambodia",
        Cameroon: "Cameroon",
        Canada: "Canada",
        CapeVerde: "Cape Verde",
        CaymanIslands: "Cayman Islands",
        CentralAfricanRepublic: "Central African Republic",
        Chad: "Chad",
        Chile: "Chile",
        // China: "China",
        Colombia: "Colombia",
        Comoros: "Comoros",
        CookIslands: "Cook Islands",
        CostaRica: "Costa Rica",
        Croatia: "Croatia",
        Cuba: "Cuba",
        Curacao: "Curacao",
        Cyprus: "Cyprus",
        Czech: "Czech",
        DemocraticRepublicOfTheCongo: "Democratic Republic of the Congo",
        Denmark: "Denmark",
        Djibouti: "Djibouti",
        Dominica: "Dominica",
        DominicanRepublic: "Dominican Republic",
        Ecuador: "Ecuador",
        Egypt: "Egypt",
        ElSalvador: "El Salvador",
        EquatorialGuinea: "Equatorial Guinea",
        Eritrea: "Eritrea",
        Estonia: "Estonia",
        Ethiopia: "Ethiopia",
        FaroeIslands: "Faroe Islands",
        Fiji: "Fiji",
        Finland: "Finland",
        France: "France",
        FrenchGuiana: "French Guiana",
        FrenchPolynesia: "French Polynesia",
        Gabon: "Gabon",
        Gambia: "Gambia",
        Georgia: "Georgia",
        Germany: "Germany",
        Ghana: "Ghana",
        Gibraltar: "Gibraltar",
        Greece: "Greece",
        Greenland: "Greenland",
        Grenada: "Grenada",
        Guadeloupe: "Guadeloupe",
        Guam: "Guam",
        Guatemala: "Guatemala",
        Guinea: "Guinea",
        GuineaBissau: "Guinea-Bissau",
        Guyana: "Guyana",
        Haiti: "Haiti",
        Honduras: "Honduras",
        HongKong: "Hong Kong",
        Hungary: "Hungary",
        Iceland: "Iceland",
        India: "India",
        Indonesia: "Indonesia",
        Iran: "Iran",
        Iraq: "Iraq",
        Ireland: "Ireland",
        Israel: "Israel",
        Italy: "Italy",
        IvoryCoast: "Ivory Coast",
        Jamaica: "Jamaica",
        Japan: "Japan",
        Jordan: "Jordan",
        Kazakhstan: "Kazakhstan",
        Kenya: "Kenya",
        Kiribati: "Kiribati",
        Kuwait: "Kuwait‎",
        Kyrgyzstan: "Kyrgyzstan",
        Laos: "Laos",
        Latvia: "Latvia",
        Lebanon: "Lebanon",
        Lesotho: "Lesotho",
        Liberia: "Liberia",
        Libya: "Libya",
        Liechtenstein: "Liechtenstein",
        Lithuania: "Lithuania",
        Luxembourg: "Luxembourg",
        Macau: "Macau",
        Macedonia: "Macedonia",
        Madagascar: "Madagascar",
        Malawi: "Malawi",
        // Malaysia: "Malaysia",
        Maldives: "Maldives",
        Mali: "Mali",
        Malta: "Malta",
        Martinique: "Martinique",
        Mauritania: "Mauritania",
        Mauritius: "Mauritius",
        Mayotte: "Mayotte",
        Mexico: "Mexico",
        Moldova: "Moldova",
        Monaco: "Monaco",
        Mongolia: "Mongolia",
        Montenegro: "Montenegro",
        Montserrat: "Montserrat",
        Morocco: "Morocco",
        Mozambique: "Mozambique",
        Myanmar: "Myanmar",
        Namibia: "Namibia",
        Nepal: "Nepal",
        Netherlands: "Netherlands",
        NewCaledonia: "New Caledonia",
        NewZealand: "New Zealand",
        Nicaragua: "Nicaragua",
        Niger: "Niger",
        Nigeria: "Nigeria",
        Norway: "Norway",
        Oman: "Oman",
        Pakistan: "Pakistan",
        Palau: "Palau",
        Palestine: "Palestine",
        Panama: "Panama",
        PapuaNewGuinea: "Papua New Guinea",
        Paraguay: "Paraguay",
        Peru: "Peru",
        Philippines: "Philippines",
        Poland: "Poland",
        Portugal: "Portugal",
        PuertoRico: "Puerto Rico",
        Qatar: "Qatar",
        RepublicOfTheCongo: "Republic Of The Congo",
        ReunionIsland: "Réunion Island",
        Romania: "Romania",
        Russia: "Russia",
        Rwanda: "Rwanda",
        SaintKittsandNevis: "Saint Kitts and Nevis",
        SaintLucia: "Saint Lucia",
        SaintPierreandMiquelon: "Saint Pierre and Miquelon",
        SaintVincentandtheGrenadines: "Saint Vincent and the Grenadines",
        Samoa: "Samoa",
        SanMarino: "San Marino",
        SaoTomeandPrincipe: "Sao Tome and Principe",
        SaudiArabia: "SaudiArabia",
        Senegal: "Senegal",
        Serbia: "Serbia",
        Seychelles: "Seychelles",
        SierraLeone: "Sierra Leone",
        Singapore: "Singapore",
        SaintMaarten: "Saint Maarten",
        Slovakia: "Slovakia",
        Slovenia: "Slovenia",
        SolomonIslands: "Solomon Islands",
        Somalia: "Somalia",
        SouthAfrica: "South Africa",
        SouthKorea: "South Korea",
        Spain: "Spain",
        SriLanka:"Sri Lanka",
        Sudan: "Sudan",
        Suriname: "Suriname",
        Swaziland: "Swaziland",
        Sweden: "Sweden",
        Switzerland: "Switzerland",
        Syria: "Syria",
        Taiwan: "Taiwan",
        Tajikistan: "Tajikistan",
        Tanzania: "Tanzania",
        Thailand: "Thailand",
        TimorLeste: "Timor-Leste",
        Togo: "Togo",
        Tonga: "Tonga",
        TrinidadandTobago: "Trinidad and Tobago",
        Tunisia: "Tunisia",
        Turkey: "Turkey",
        Turkmenistan: "Turkmenistan",
        TurksandCaicosIslands: "Turks and Caicos Islands",
        Uganda: "Uganda",
        Ukraine: "Ukraine",
        UnitedArabEmirates: "United Arab Emirates",
        UnitedKingdom: "United Kingdom",
        // UnitedStates: "United States",
        Uruguay: "Uruguay",
        Uzbekistan: "Uzbekistan",
        Vanuatu: "Vanuatu",
        Venezuela: "Venezuela",
        Vietnam: "Vietnam",
        VirginIslandsBritish: "Virgin Islands,British",
        VirginIslandsUS: "Virgin Islands,US",
        Yemen: "Yemen",
        Zambia: "Zambia",
        Zimbabwe: "Zimbabwe",
        findCodeNull: "Sorry, no matching area code was found",
    };
    message["ko_KR"] = {
        // Afghanistan: "Afghanistan",
        Albania: "Albania",
        Algeria: "Algeria",
        AmericanSamoa: "American Samoa",
        Andorra: "Andorra",
        Angola: "Angola",
        Anguilla: "Anguilla",
        AntiguaAndBarbuda: "Antigua and Barbuda",
        Argentina: "Argentina",
        Armenia: "Armenia",
        Aruba: "Aruba",
        Australia: "Australia",
        Austria: "Austria",
        Azerbaijan: "Azerbaijan",
        Bahamas: "Bahamas",
        Bahrain: "Bahrain",
        Bangladesh: "Bangladesh",
        Barbados: "Barbados",
        Belarus: "Belarus",
        Belgium: "Belgium",
        Belize: "Belize",
        Benin: "Benin",
        Bermuda: "Bermuda",
        Bhutan: "Bhutan",
        Bolivia: "Bolivia",
        BosniaAndHerzegovina: "Bosnia and Herzegovina",
        Botswana: "Botswana",
        Brazil: "Brazil",
        Brunei: "Brunei",
        Bulgaria: "Bulgaria",
        BurkinaFaso: "Burkina Faso",
        Burundi: "Burundi",
        Cambodia: "Cambodia",
        Cameroon: "Cameroon",
        Canada: "Canada",
        CapeVerde: "Cape Verde",
        CaymanIslands: "Cayman Islands",
        CentralAfricanRepublic: "Central African Republic",
        Chad: "Chad",
        Chile: "Chile",
        // China: "China",
        Colombia: "Colombia",
        Comoros: "Comoros",
        CookIslands: "Cook Islands",
        CostaRica: "Costa Rica",
        Croatia: "Croatia",
        Cuba: "Cuba",
        Curacao: "Curacao",
        Cyprus: "Cyprus",
        Czech: "Czech",
        DemocraticRepublicOfTheCongo: "Democratic Republic of the Congo",
        Denmark: "Denmark",
        Djibouti: "Djibouti",
        Dominica: "Dominica",
        DominicanRepublic: "Dominican Republic",
        Ecuador: "Ecuador",
        Egypt: "Egypt",
        ElSalvador: "El Salvador",
        EquatorialGuinea: "Equatorial Guinea",
        Eritrea: "Eritrea",
        Estonia: "Estonia",
        Ethiopia: "Ethiopia",
        FaroeIslands: "Faroe Islands",
        Fiji: "Fiji",
        Finland: "Finland",
        France: "France",
        FrenchGuiana: "French Guiana",
        FrenchPolynesia: "French Polynesia",
        Gabon: "Gabon",
        Gambia: "Gambia",
        Georgia: "Georgia",
        Germany: "Germany",
        Ghana: "Ghana",
        Gibraltar: "Gibraltar",
        Greece: "Greece",
        Greenland: "Greenland",
        Grenada: "Grenada",
        Guadeloupe: "Guadeloupe",
        Guam: "Guam",
        Guatemala: "Guatemala",
        Guinea: "Guinea",
        GuineaBissau: "Guinea-Bissau",
        Guyana: "Guyana",
        Haiti: "Haiti",
        Honduras: "Honduras",
        HongKong: "Hong Kong",
        Hungary: "Hungary",
        Iceland: "Iceland",
        India: "India",
        Indonesia: "Indonesia",
        Iran: "Iran",
        Iraq: "Iraq",
        Ireland: "Ireland",
        Israel: "Israel",
        Italy: "Italy",
        IvoryCoast: "Ivory Coast",
        Jamaica: "Jamaica",
        Japan: "Japan",
        Jordan: "Jordan",
        Kazakhstan: "Kazakhstan",
        Kenya: "Kenya",
        Kiribati: "Kiribati",
        Kuwait: "Kuwait‎",
        Kyrgyzstan: "Kyrgyzstan",
        Laos: "Laos",
        Latvia: "Latvia",
        Lebanon: "Lebanon",
        Lesotho: "Lesotho",
        Liberia: "Liberia",
        Libya: "Libya",
        Liechtenstein: "Liechtenstein",
        Lithuania: "Lithuania",
        Luxembourg: "Luxembourg",
        Macau: "Macau",
        Macedonia: "Macedonia",
        Madagascar: "Madagascar",
        Malawi: "Malawi",
        // Malaysia: "Malaysia",
        Maldives: "Maldives",
        Mali: "Mali",
        Malta: "Malta",
        Martinique: "Martinique",
        Mauritania: "Mauritania",
        Mauritius: "Mauritius",
        Mayotte: "Mayotte",
        Mexico: "Mexico",
        Moldova: "Moldova",
        Monaco: "Monaco",
        Mongolia: "Mongolia",
        Montenegro: "Montenegro",
        Montserrat: "Montserrat",
        Morocco: "Morocco",
        Mozambique: "Mozambique",
        Myanmar: "Myanmar",
        Namibia: "Namibia",
        Nepal: "Nepal",
        Netherlands: "Netherlands",
        NewCaledonia: "New Caledonia",
        NewZealand: "New Zealand",
        Nicaragua: "Nicaragua",
        Niger: "Niger",
        Nigeria: "Nigeria",
        Norway: "Norway",
        Oman: "Oman",
        Pakistan: "Pakistan",
        Palau: "Palau",
        Palestine: "Palestine",
        Panama: "Panama",
        PapuaNewGuinea: "Papua New Guinea",
        Paraguay: "Paraguay",
        Peru: "Peru",
        Philippines: "Philippines",
        Poland: "Poland",
        Portugal: "Portugal",
        PuertoRico: "Puerto Rico",
        Qatar: "Qatar",
        RepublicOfTheCongo: "Republic Of The Congo",
        ReunionIsland: "Réunion Island",
        Romania: "Romania",
        Russia: "Russia",
        Rwanda: "Rwanda",
        SaintKittsandNevis: "Saint Kitts and Nevis",
        SaintLucia: "Saint Lucia",
        SaintPierreandMiquelon: "Saint Pierre and Miquelon",
        SaintVincentandtheGrenadines: "Saint Vincent and the Grenadines",
        Samoa: "Samoa",
        SanMarino: "San Marino",
        SaoTomeandPrincipe: "Sao Tome and Principe",
        SaudiArabia: "SaudiArabia",
        Senegal: "Senegal",
        Serbia: "Serbia",
        Seychelles: "Seychelles",
        SierraLeone: "Sierra Leone",
        Singapore: "Singapore",
        SaintMaarten: "Saint Maarten",
        Slovakia: "Slovakia",
        Slovenia: "Slovenia",
        SolomonIslands: "Solomon Islands",
        Somalia: "Somalia",
        SouthAfrica: "South Africa",
        SouthKorea: "South Korea",
        Spain: "Spain",
        SriLanka:"Sri Lanka",
        Sudan: "Sudan",
        Suriname: "Suriname",
        Swaziland: "Swaziland",
        Sweden: "Sweden",
        Switzerland: "Switzerland",
        Syria: "Syria",
        Taiwan: "Taiwan",
        Tajikistan: "Tajikistan",
        Tanzania: "Tanzania",
        Thailand: "Thailand",
        TimorLeste: "Timor-Leste",
        Togo: "Togo",
        Tonga: "Tonga",
        TrinidadandTobago: "Trinidad and Tobago",
        Tunisia: "Tunisia",
        Turkey: "Turkey",
        Turkmenistan: "Turkmenistan",
        TurksandCaicosIslands: "Turks and Caicos Islands",
        Uganda: "Uganda",
        Ukraine: "Ukraine",
        UnitedArabEmirates: "United Arab Emirates",
        UnitedKingdom: "United Kingdom",
        // UnitedStates: "United States",
        Uruguay: "Uruguay",
        Uzbekistan: "Uzbekistan",
        Vanuatu: "Vanuatu",
        Venezuela: "Venezuela",
        Vietnam: "Vietnam",
        VirginIslandsBritish: "Virgin Islands,British",
        VirginIslandsUS: "Virgin Islands,US",
        Yemen: "Yemen",
        Zambia: "Zambia",
        Zimbabwe: "Zimbabwe",
        findCodeNull: "Sorry, no matching area code was found",
    };
    message["ru_RU"] = {
        // Afghanistan: "Afghanistan",
        Albania: "Albania",
        Algeria: "Algeria",
        AmericanSamoa: "American Samoa",
        Andorra: "Andorra",
        Angola: "Angola",
        Anguilla: "Anguilla",
        AntiguaAndBarbuda: "Antigua and Barbuda",
        Argentina: "Argentina",
        Armenia: "Armenia",
        Aruba: "Aruba",
        Australia: "Australia",
        Austria: "Austria",
        Azerbaijan: "Azerbaijan",
        Bahamas: "Bahamas",
        Bahrain: "Bahrain",
        Bangladesh: "Bangladesh",
        Barbados: "Barbados",
        Belarus: "Belarus",
        Belgium: "Belgium",
        Belize: "Belize",
        Benin: "Benin",
        Bermuda: "Bermuda",
        Bhutan: "Bhutan",
        Bolivia: "Bolivia",
        BosniaAndHerzegovina: "Bosnia and Herzegovina",
        Botswana: "Botswana",
        Brazil: "Brazil",
        Brunei: "Brunei",
        Bulgaria: "Bulgaria",
        BurkinaFaso: "Burkina Faso",
        Burundi: "Burundi",
        Cambodia: "Cambodia",
        Cameroon: "Cameroon",
        Canada: "Canada",
        CapeVerde: "Cape Verde",
        CaymanIslands: "Cayman Islands",
        CentralAfricanRepublic: "Central African Republic",
        Chad: "Chad",
        Chile: "Chile",
        // China: "China",
        Colombia: "Colombia",
        Comoros: "Comoros",
        CookIslands: "Cook Islands",
        CostaRica: "Costa Rica",
        Croatia: "Croatia",
        Cuba: "Cuba",
        Curacao: "Curacao",
        Cyprus: "Cyprus",
        Czech: "Czech",
        DemocraticRepublicOfTheCongo: "Democratic Republic of the Congo",
        Denmark: "Denmark",
        Djibouti: "Djibouti",
        Dominica: "Dominica",
        DominicanRepublic: "Dominican Republic",
        Ecuador: "Ecuador",
        Egypt: "Egypt",
        ElSalvador: "El Salvador",
        EquatorialGuinea: "Equatorial Guinea",
        Eritrea: "Eritrea",
        Estonia: "Estonia",
        Ethiopia: "Ethiopia",
        FaroeIslands: "Faroe Islands",
        Fiji: "Fiji",
        Finland: "Finland",
        France: "France",
        FrenchGuiana: "French Guiana",
        FrenchPolynesia: "French Polynesia",
        Gabon: "Gabon",
        Gambia: "Gambia",
        Georgia: "Georgia",
        Germany: "Germany",
        Ghana: "Ghana",
        Gibraltar: "Gibraltar",
        Greece: "Greece",
        Greenland: "Greenland",
        Grenada: "Grenada",
        Guadeloupe: "Guadeloupe",
        Guam: "Guam",
        Guatemala: "Guatemala",
        Guinea: "Guinea",
        GuineaBissau: "Guinea-Bissau",
        Guyana: "Guyana",
        Haiti: "Haiti",
        Honduras: "Honduras",
        HongKong: "Hong Kong",
        Hungary: "Hungary",
        Iceland: "Iceland",
        India: "India",
        Indonesia: "Indonesia",
        Iran: "Iran",
        Iraq: "Iraq",
        Ireland: "Ireland",
        Israel: "Israel",
        Italy: "Italy",
        IvoryCoast: "Ivory Coast",
        Jamaica: "Jamaica",
        Japan: "Japan",
        Jordan: "Jordan",
        Kazakhstan: "Kazakhstan",
        Kenya: "Kenya",
        Kiribati: "Kiribati",
        Kuwait: "Kuwait‎",
        Kyrgyzstan: "Kyrgyzstan",
        Laos: "Laos",
        Latvia: "Latvia",
        Lebanon: "Lebanon",
        Lesotho: "Lesotho",
        Liberia: "Liberia",
        Libya: "Libya",
        Liechtenstein: "Liechtenstein",
        Lithuania: "Lithuania",
        Luxembourg: "Luxembourg",
        Macau: "Macau",
        Macedonia: "Macedonia",
        Madagascar: "Madagascar",
        Malawi: "Malawi",
        // Malaysia: "Malaysia",
        Maldives: "Maldives",
        Mali: "Mali",
        Malta: "Malta",
        Martinique: "Martinique",
        Mauritania: "Mauritania",
        Mauritius: "Mauritius",
        Mayotte: "Mayotte",
        Mexico: "Mexico",
        Moldova: "Moldova",
        Monaco: "Monaco",
        Mongolia: "Mongolia",
        Montenegro: "Montenegro",
        Montserrat: "Montserrat",
        Morocco: "Morocco",
        Mozambique: "Mozambique",
        Myanmar: "Myanmar",
        Namibia: "Namibia",
        Nepal: "Nepal",
        Netherlands: "Netherlands",
        NewCaledonia: "New Caledonia",
        NewZealand: "New Zealand",
        Nicaragua: "Nicaragua",
        Niger: "Niger",
        Nigeria: "Nigeria",
        Norway: "Norway",
        Oman: "Oman",
        Pakistan: "Pakistan",
        Palau: "Palau",
        Palestine: "Palestine",
        Panama: "Panama",
        PapuaNewGuinea: "Papua New Guinea",
        Paraguay: "Paraguay",
        Peru: "Peru",
        Philippines: "Philippines",
        Poland: "Poland",
        Portugal: "Portugal",
        PuertoRico: "Puerto Rico",
        Qatar: "Qatar",
        RepublicOfTheCongo: "Republic Of The Congo",
        ReunionIsland: "Réunion Island",
        Romania: "Romania",
        Russia: "Russia",
        Rwanda: "Rwanda",
        SaintKittsandNevis: "Saint Kitts and Nevis",
        SaintLucia: "Saint Lucia",
        SaintPierreandMiquelon: "Saint Pierre and Miquelon",
        SaintVincentandtheGrenadines: "Saint Vincent and the Grenadines",
        Samoa: "Samoa",
        SanMarino: "San Marino",
        SaoTomeandPrincipe: "Sao Tome and Principe",
        SaudiArabia: "SaudiArabia",
        Senegal: "Senegal",
        Serbia: "Serbia",
        Seychelles: "Seychelles",
        SierraLeone: "Sierra Leone",
        Singapore: "Singapore",
        SaintMaarten: "Saint Maarten",
        Slovakia: "Slovakia",
        Slovenia: "Slovenia",
        SolomonIslands: "Solomon Islands",
        Somalia: "Somalia",
        SouthAfrica: "South Africa",
        SouthKorea: "South Korea",
        Spain: "Spain",
        SriLanka:"Sri Lanka",
        Sudan: "Sudan",
        Suriname: "Suriname",
        Swaziland: "Swaziland",
        Sweden: "Sweden",
        Switzerland: "Switzerland",
        Syria: "Syria",
        Taiwan: "Taiwan",
        Tajikistan: "Tajikistan",
        Tanzania: "Tanzania",
        Thailand: "Thailand",
        TimorLeste: "Timor-Leste",
        Togo: "Togo",
        Tonga: "Tonga",
        TrinidadandTobago: "Trinidad and Tobago",
        Tunisia: "Tunisia",
        Turkey: "Turkey",
        Turkmenistan: "Turkmenistan",
        TurksandCaicosIslands: "Turks and Caicos Islands",
        Uganda: "Uganda",
        Ukraine: "Ukraine",
        UnitedArabEmirates: "United Arab Emirates",
        UnitedKingdom: "United Kingdom",
        // UnitedStates: "United States",
        Uruguay: "Uruguay",
        Uzbekistan: "Uzbekistan",
        Vanuatu: "Vanuatu",
        Venezuela: "Venezuela",
        Vietnam: "Vietnam",
        VirginIslandsBritish: "Virgin Islands,British",
        VirginIslandsUS: "Virgin Islands,US",
        Yemen: "Yemen",
        Zambia: "Zambia",
        Zimbabwe: "Zimbabwe",
        findCodeNull: "Sorry, no matching area code was found",
    };
    var MSG = message[locale];
    // MSG[""]
    /********************
   *  STATIC METHODS
   ********************/
    // get the country data object
    $.fn[pluginName].getCountryData = function() {
        return allCountries;
    };
    // set the country data object
    $.fn[pluginName].setCountryData = function(obj) {
        allCountries = obj;
    };
    // Tell JSHint to ignore this warning: "character may get silently deleted by one or more browsers"
    // jshint -W100
    // Array of country objects for the flag dropdown.
    // Each contains a name, country code (ISO 3166-1 alpha-2) and dial code.
    // Originally from https://github.com/mledoze/countries
    // then modified using the following JavaScript:
    /*
var result = [];
_.each(countries, function(c) {
  // ignore countries without a dial code
  if (c.callingCode[0].length) {
    result.push({
      // var locals contains country names with localised versions in brackets
      n: _.findWhere(locals, {
        countryCode: c.cca2
      }).name,
      i: c.cca2.toLowerCase(),
      d: c.callingCode[0]
    });
  }
});
JSON.stringify(result);
*/
    // then with a couple of manual re-arrangements to be alphabetical
    // then changed Kazakhstan from +76 to +7
    // then manually removed quotes from property names as not required
    // Note: using single char property names to keep filesize down
    // n = name
    // i = iso2 (2-char country code)
    // d = dial code
    var allCountries = $.each([
    //     {
    //     n: MSG["Afghanistan"],
    //     i: "af",
    //     d: "93"
    // },
        {
        n: MSG["Albania"],
        i: "al",
        d: "355"
    }, {
        n: MSG["Algeria"],
        i: "dz",
        d: "213"
    }, {
        n: MSG["AmericanSamoa"],
        i: "as",
        d: "1684"
    }, {
        n: MSG["Andorra"],
        i: "ad",
        d: "376"
    }, {
        n: MSG["Angola"],
        i: "ao",
        d: "244"
    }, {
        n: MSG["Anguilla"],
        i: "ai",
        d: "1264"
    }, {
        n: MSG["AntiguaAndBarbuda"],
        i: "ag",
        d: "1268"
    }, {
        n: MSG["Argentina"],
        i: "ar",
        d: "54"
    }, {
        n: MSG["Armenia"],
        i: "am",
        d: "374"
    }, {
        n: MSG["Aruba"],
        i: "aw",
        d: "297"
    }, {
        n: MSG["Australia"],
        i: "au",
        d: "61"
    }, {
        n: MSG["Austria"],
        i: "at",
        d: "43"
    }, {
        n: MSG["Azerbaijan"],
        i: "az",
        d: "994"
    }, {
        n: MSG["Bahamas"],
        i: "bs",
        d: "1242"
    }, {
        n: MSG["Bahrain"],
        i: "bh",
        d: "973"
    }, {
        n: MSG["Bangladesh"],
        i: "bd",
        d: "880"
    }, {
        n: MSG["Barbados"],
        i: "bb",
        d: "1246"
    }, {
        n: MSG["Belarus"],
        i: "by",
        d: "375"
    }, {
        n: MSG["Belgium"],
        i: "be",
        d: "32"
    }, {
        n: MSG["Belize"],
        i: "bz",
        d: "501"
    }, {
        n: MSG["Benin"],
        i: "bj",
        d: "229"
    }, {
        n: MSG["Bermuda"],
        i: "bm",
        d: "1441"
    }, {
        n: MSG["Bhutan"],
        i: "bt",
        d: "975"
    }, {
        n: MSG["Bolivia"],
        i: "bo",
        d: "591"
    }, {
        n: MSG["BosniaAndHerzegovina"],
        i: "ba",
        d: "387"
    }, {
        n: MSG["Botswana"],
        i: "bw",
        d: "267"
    }, {
        n: MSG["Brazil"],
        i: "br",
        d: "55"
    }, {
        n: MSG["Brunei"],
        i: "bn",
        d: "673"
    }, {
        n: MSG["Bulgaria"],
        i: "bg",
        d: "359"
    }, {
        n: MSG["BurkinaFaso"],
        i: "bf",
        d: "226"
    }, {
        n: MSG["Burundi"],
        i: "bi",
        d: "257"
    }, {
        n: MSG["Cambodia"],
        i: "kh",
        d: "855"
    }, {
        n: MSG["Cameroon"],
        i: "cm",
        d: "237"
    }, {
        n: MSG["Canada"],
        i: "ca",
        d: "1"
    }, {
        n: MSG["CapeVerde"],
        i: "cv",
        d: "238"
    }, {
        n: MSG["CaymanIslands"],
        i: "ky",
        d: "1345"
    }, {
        n: MSG["CentralAfricanRepublic"],
        i: "cf",
        d: "236"
    }, {
        n: MSG["Chad"],
        i: "td",
        d: "235"
    }, {
        n: MSG["Chile"],
        i: "cl",
        d: "56"
    },
    //     {
    //     n: MSG["China"],
    //     i: "cn",
    //     d: "86"
    // },
        {
        n: MSG["Colombia"],
        i: "co",
        d: "57"
    }, {
        n: MSG["Comoros"],
        i: "km",
        d: "269"
    }, {
        n: MSG["CookIslands"],
        i: "ck",
        d: "682"
    }, {
        n: MSG["CostaRica"],
        i: "cr",
        d: "506"
    }, {
        n: MSG["Croatia"],
        i: "hr",
        d: "385"
    }, {
        n: MSG["Cuba"],
        i: "cu",
        d: "53"
    }, {
        n: MSG["Curacao"],
        i: "cw",
        d: "5999"
    }, {
        n: MSG["Cyprus"],
        i: "cy",
        d: "357"
    }, {
        n: MSG["Czech"],
        i: "cz",
        d: "420"
    }, {
        n: MSG["DemocraticRepublicOfTheCongo"],
        i: "cd",
        d: "243"
    }, {
        n: MSG["Denmark"],
        i: "dk",
        d: "45"
    }, {
        n: MSG["Djibouti"],
        i: "dj",
        d: "253"
    }, {
        n: MSG["Dominica"],
        i: "dm",
        d: "1767"
    }, {
        n: MSG["DominicanRepublic"],
        i: "do",
        d: "1809"
    }, {
        n: MSG["Ecuador"],
        i: "ec",
        d: "593"
    }, {
        n: MSG["Egypt"],
        i: "eg",
        d: "20"
    }, {
        n: MSG["ElSalvador"],
        i: "sv",
        d: "503"
    }, {
        n: MSG["EquatorialGuinea"],
        i: "gq",
        d: "240"
    }, {
        n: MSG["Eritrea"],
        i: "er",
        d: "291"
    }, {
        n: MSG["Estonia"],
        i: "ee",
        d: "372"
    }, {
        n: MSG["Ethiopia"],
        i: "et",
        d: "251"
    }, {
        n: MSG["FaroeIslands"],
        i: "fo",
        d: "298"
    }, {
        n: MSG["Fiji"],
        i: "fj",
        d: "679"
    }, {
        n: MSG["Finland"],
        i: "fi",
        d: "358"
    }, {
        n: MSG["France"],
        i: "fr",
        d: "33"
    }, {
        n: MSG["FrenchGuiana"],
        i: "gf",
        d: "594"
    }, {
        n: MSG["FrenchPolynesia"],
        i: "pf",
        d: "689"
    }, {
        n: MSG["Gabon"],
        i: "ga",
        d: "241"
    }, {
        n: MSG["Gambia"],
        i: "gm",
        d: "220"
    }, {
        n: MSG["Georgia"],
        i: "ge",
        d: "995"
    }, {
        n: MSG["Germany"],
        i: "de",
        d: "49"
    }, {
        n: MSG["Ghana"],
        i: "gh",
        d: "233"
    }, {
        n: MSG["Gibraltar"],
        i: "gi",
        d: "350"
    }, {
        n: MSG["Greece"],
        i: "gr",
        d: "30"
    }, {
        n: MSG["Greenland"],
        i: "gl",
        d: "299"
    }, {
        n: MSG["Grenada"],
        i: "gd",
        d: "1473"
    }, {
        n: MSG["Guadeloupe"],
        i: "gp",
        d: "590"
    }, {
        n: MSG["Guam"],
        i: "gu",
        d: "1671"
    }, {
        n: MSG["Guatemala"],
        i: "gt",
        d: "502"
    }, {
        n: MSG["Guinea"],
        i: "gn",
        d: "224"
    }, {
        n: MSG["GuineaBissau"],
        i: "gw",
        d: "245"
    }, {
        n: MSG["Guyana"],
        i: "gy",
        d: "592"
    }, {
        n: MSG["Haiti"],
        i: "ht",
        d: "509"
    }, {
        n: MSG["Honduras"],
        i: "hn",
        d: "504"
    }, {
        n: MSG["HongKong"],
        i: "hk",
        d: "852"
    }, {
        n: MSG["Hungary"],
        i: "hu",
        d: "36"
    }, {
        n: MSG["Iceland"],
        i: "is",
        d: "354"
    }, {
        n: MSG["India"],
        i: "in",
        d: "91"
    }, {
        n: MSG["Indonesia"],
        i: "id",
        d: "62"
    }, {
        n: MSG["Iran"],
        i: "ir",
        d: "98"
    }, {
        n: MSG["Iraq"],
        i: "iq",
        d: "964"
    }, {
        n: MSG["Ireland"],
        i: "ie",
        d: "353"
    }, {
        n: MSG["IvoryCoast"],
        i: "ci",
        d: "225"
    }, {
        n: MSG["Israel"],
        i: "il",
        d: "972"
    }, {
        n: MSG["Italy"],
        i: "it",
        d: "39"
    }, {
        n: MSG["Jamaica"],
        i: "jm",
        d: "1876"
    }, {
        n: MSG["Japan"],
        i: "jp",
        d: "81"
    }, {
        n: MSG["Jordan"],
        i: "jo",
        d: "962"
    }, {
        n: MSG["Kazakhstan"],
        i: "kz",
        d: "7"
    }, {
        n: MSG["Kenya"],
        i: "ke",
        d: "254"
    }, {
        n: MSG["Kiribati"],
        i: "ki",
        d: "686"
    }, {
        n: MSG["Kuwait‎"],
        i: "kw",
        d: "965"
    }, {
        n: MSG["Kyrgyzstan"],
        i: "kg",
        d: "996"
    }, {
        n: MSG["Laos"],
        i: "la",
        d: "856"
    }, {
        n: MSG["Latvia"],
        i: "lv",
        d: "371"
    }, {
        n: MSG["Lebanon"],
        i: "lb",
        d: "961"
    }, {
        n: MSG["Lesotho"],
        i: "ls",
        d: "266"
    }, {
        n: MSG["Liberia"],
        i: "lr",
        d: "231"
    }, {
        n: MSG["Libya"],
        i: "ly",
        d: "218"
    }, {
        n: MSG["Liechtenstein"],
        i: "li",
        d: "423"
    }, {
        n: MSG["Lithuania"],
        i: "lt",
        d: "370"
    }, {
        n: MSG["Luxembourg"],
        i: "lu",
        d: "352"
    }, {
        n: MSG["Macau"],
        i: "mo",
        d: "853"
    }, {
        n: MSG["Macedonia"],
        i: "mk",
        d: "389"
    }, {
        n: MSG["Madagascar"],
        i: "mg",
        d: "261"
    }, {
        n: MSG["Malawi"],
        i: "mw",
        d: "265"
    },
    //     {
    //     n: MSG["Malaysia"],
    //     i: "my",
    //     d: "60"
    // },
        {
        n: MSG["Maldives"],
        i: "mv",
        d: "960"
    }, {
        n: MSG["Mali"],
        i: "ml",
        d: "223"
    }, {
        n: MSG["Malta"],
        i: "mt",
        d: "356"
    }, {
        n: MSG["Martinique"],
        i: "mq",
        d: "596"
    }, {
        n: MSG["Mauritania"],
        i: "mr",
        d: "222"
    }, {
        n: MSG["Mauritius"],
        i: "mu",
        d: "230"
    }, {
        n: MSG["Mayotte"],
        i: "yt",
        d: "269"
    }, {
        n: MSG["Mexico"],
        i: "mx",
        d: "52"
    }, {
        n: MSG["Moldova"],
        i: "md",
        d: "373"
    }, {
        n: MSG["Monaco"],
        i: "mc",
        d: "377"
    }, {
        n: MSG["Mongolia"],
        i: "mn",
        d: "976"
    }, {
        n: MSG["Montenegro"],
        i: "me",
        d: "382"
    }, {
        n: MSG["Montserrat"],
        i: "ms",
        d: "1664"
    }, {
        n: MSG["Morocco"],
        i: "ma",
        d: "212"
    }, {
        n: MSG["Mozambique"],
        i: "mz",
        d: "258"
    }, {
        n: MSG["Myanmar"],
        i: "mm",
        d: "95"
    }, {
        n: MSG["Namibia"],
        i: "na",
        d: "264"
    }, {
        n: MSG["Nepal"],
        i: "np",
        d: "977"
    }, {
        n: MSG["Netherlands"],
        i: "nl",
        d: "31"
    }, {
        n: MSG["NewCaledonia"],
        i: "nc",
        d: "687"
    }, {
        n: MSG["NewZealand"],
        i: "nz",
        d: "64"
    }, {
        n: MSG["Nicaragua"],
        i: "ni",
        d: "505"
    }, {
        n: MSG["Niger"],
        i: "ne",
        d: "227"
    }, {
        n: MSG["Nigeria"],
        i: "ng",
        d: "234"
    }, {
        n: MSG["Norway"],
        i: "no",
        d: "47"
    }, {
        n: MSG["Oman"],
        i: "om",
        d: "968"
    }, {
        n: MSG["Pakistan"],
        i: "pk",
        d: "92"
    }, {
        n: MSG["Palau"],
        i: "pw",
        d: "680"
    }, {
        n: MSG["Palestine"],
        i: "bl",
        d: "970"
    }, {
        n: MSG["Panama"],
        i: "pa",
        d: "507"
    }, {
        n: MSG["PapuaNewGuinea"],
        i: "pg",
        d: "675"
    }, {
        n: MSG["Paraguay"],
        i: "py",
        d: "595"
    }, {
        n: MSG["Peru"],
        i: "pe",
        d: "51"
    }, {
        n: MSG["Philippines"],
        i: "ph",
        d: "63"
    }, {
        n: MSG["Poland"],
        i: "pl",
        d: "48"
    }, {
        n: MSG["Portugal"],
        i: "pt",
        d: "351"
    }, {
        n: MSG["PuertoRico"],
        i: "pr",
        d: "1787"
    }, {
        n: MSG["Qatar"],
        i: "qa",
        d: "974"
    }, {
        n: MSG["RepublicOfTheCongo"],
        i: "cg",
        d: "242"
    }, {
        n: MSG["ReunionIsland"],
        i: "re",
        d: "262"
    }, {
        n: MSG["Romania"],
        i: "ro",
        d: "40"
    }, {
        n: MSG["Russia"],
        i: "ru",
        d: "7"
    }, {
        n: MSG["Rwanda"],
        i: "rw",
        d: "250"
    }, {
        n: MSG["SaintKittsandNevis"],
        i: "kn",
        d: "1869"
    }, {
        n: MSG["SaintLucia"],
        i: "lc",
        d: "1758"
    }, {
        n: MSG["SaintPierreandMiquelon"],
        i: "pm",
        d: "508"
    }, {
        n: MSG["SaintVincentandtheGrenadines"],
        i: "vc",
        d: "1784"
    }, {
        n: MSG["Samoa"],
        i: "ws",
        d: "685"
    }, {
        n: MSG["SanMarino"],
        i: "sm",
        d: "378"
    }, {
        n: MSG["SaoTomeandPrincipe"],
        i: "st",
        d: "239"
    }, {
        n: MSG["SaudiArabia"],
        i: "sa",
        d: "966"
    }, {
        n: MSG["Senegal"],
        i: "sn",
        d: "221"
    }, {
        n: MSG["Serbia"],
        i: "rs",
        d: "381"
    }, {
        n: MSG["Seychelles"],
        i: "sc",
        d: "248"
    }, {
        n: MSG["SierraLeone"],
        i: "sl",
        d: "232"
    }, {
        n: MSG["Singapore"],
        i: "sg",
        d: "65"
    }, {
        n: MSG["SaintMaarten"],
        i: "sx",
        d: "1721"
    }, {
        n: MSG["Slovakia"],
        i: "sk",
        d: "421"
    }, {
        n: MSG["Slovenia"],
        i: "si",
        d: "386"
    }, {
        n: MSG["SolomonIslands"],
        i: "sb",
        d: "677"
    }, {
        n: MSG["Somalia"],
        i: "so",
        d: "252"
    }, {
        n: MSG["SouthAfrica"],
        i: "za",
        d: "27"
    }, {
        n: MSG["SouthKorea"],
        i: "kr",
        d: "82"
    }, {
        n: MSG["Spain"],
        i: "es",
        d: "34"
    }, {
        n: MSG["SriLanka"],
        i: "lk",
        d: "94"
    }, {
        n: MSG["Sudan"],
        i: "sd",
        d: "249"
    }, {
        n: MSG["Suriname"],
        i: "sr",
        d: "597"
    }, {
        n: MSG["Swaziland"],
        i: "sz",
        d: "268"
    }, {
        n: MSG["Sweden"],
        i: "se",
        d: "46"
    }, {
        n: MSG["Switzerland"],
        i: "ch",
        d: "41"
    }, {
        n: MSG["Syria"],
        i: "sy",
        d: "963"
    }, {
        n: MSG["Taiwan"],
        i: "tw",
        d: "886"
    }, {
        n: MSG["Tajikistan"],
        i: "tj",
        d: "992"
    }, {
        n: MSG["Tanzania"],
        i: "tz",
        d: "255"
    }, {
        n: MSG["Thailand"],
        i: "th",
        d: "66"
    }, {
        n: MSG["TimorLeste"],
        i: "tl",
        d: "670"
    }, {
        n: MSG["Togo"],
        i: "tg",
        d: "228"
    }, {
        n: MSG["Tonga"],
        i: "to",
        d: "676"
    }, {
        n: MSG["TrinidadandTobago"],
        i: "tt",
        d: "1868"
    }, {
        n: MSG["Tunisia"],
        i: "tn",
        d: "216"
    }, {
        n: MSG["Turkey"],
        i: "tr",
        d: "90"
    }, {
        n: MSG["Turkmenistan"],
        i: "tm",
        d: "993"
    }, {
        n: MSG["TurksandCaicosIslands"],
        i: "tc",
        d: "1649"
    }, {
        n: MSG["Uganda"],
        i: "ug",
        d: "256"
    }, {
        n: MSG["Ukraine"],
        i: "ua",
        d: "380"
    }, {
        n: MSG["UnitedArabEmirates"],
        i: "ae",
        d: "971"
    }, {
        n: MSG["UnitedKingdom"],
        i: "gb",
        d: "44"
    },
    //     {
    //     n: MSG["UnitedStates"],
    //     i: "us",
    //     d: "1"
    // },
        {
        n: MSG["VirginIslandsBritish"],
        i: "vg",
        d: "1340"
    }, {
        n: MSG["VirginIslandsUS"],
        i: "vi",
        d: "1284"
    }, {
        n: MSG["Uruguay"],
        i: "uy",
        d: "598"
    }, {
        n: MSG["Uzbekistan"],
        i: "uz",
        d: "998"
    }, {
        n: MSG["Vanuatu"],
        i: "vu",
        d: "678"
    }, {
        n: MSG["Venezuela"],
        i: "ve",
        d: "58"
    }, {
        n: MSG["Vietnam"],
        i: "vn",
        d: "84"
    }, {
        n: MSG["Yemen"],
        i: "ye",
        d: "967"
    }, {
        n: MSG["Zambia"],
        i: "zm",
        d: "260"
    }, {
        n: MSG["Zimbabwe"],
        i: "zw",
        d: "263"
    } ], function(i, c) {
        c.name = c.n;
        c.iso2 = c.i;
        c.dialCode = c.d;
        delete c.n;
        delete c.i;
        delete c.d;
    });
    // JavaScript object mapping dial code to country code.
    // This is used when the user enters a number,
    // to quickly look up the corresponding country code.
    // Generated from the above array using this JavaScript:
    /*
var uniqueDCs = _.unique(_.pluck(intlDataFull.countries, dialCode));
var cCodes = {};
_.each(uniqueDCs, function(dc) {
  cCodes[dc] = _.pluck(_.filter(intlDataFull.countries, function(c) {
    return c[dialCode] == dc;
  }), iso2);
});
 */
    // Then reference this google code project for clash priority:
    // http://libphonenumber.googlecode.com/svn/trunk/javascript/i18n/phonenumbers/metadata.js
    // then updated vatican city to +379
    var allCountryCodes = {
        // "1": [ "us", "ca" ],
        "7": [ "ru", "kz" ],
        "20": [ "eg" ],
        "27": [ "za" ],
        "30": [ "gr" ],
        "31": [ "nl" ],
        "32": [ "be" ],
        "33": [ "fr" ],
        "34": [ "es" ],
        "36": [ "hu" ],
        "39": [ "it" ],
        "40": [ "ro" ],
        "41": [ "ch" ],
        "43": [ "at" ],
        "44": [ "gb", "gg", "im", "je" ],
        "45": [ "dk" ],
        "46": [ "se" ],
        "47": [ "no" ],
        "48": [ "pl" ],
        "49": [ "de" ],
        "51": [ "pe" ],
        "52": [ "mx" ],
        "53": [ "cu" ],
        "54": [ "ar" ],
        "55": [ "br" ],
        "56": [ "cl" ],
        "57": [ "co" ],
        "58": [ "ve" ],
        // "60": [ "my" ],
        "61": [ "au", "cc", "cx" ],
        "62": [ "id" ],
        "63": [ "ph" ],
        "64": [ "nz", "pn" ],
        "65": [ "sg" ],
        "66": [ "th" ],
        "81": [ "jp" ],
        "82": [ "kr" ],
        "84": [ "vn" ],
        // "86": [ "cn" ],
        "90": [ "tr" ],
        "91": [ "in" ],
        "92": [ "pk" ],
        // "93": [ "af" ],
        "94": [ "lk" ],
        "95": [ "mm" ],
        "98": [ "ir" ],
        "211": [ "ss" ],
        "212": [ "ma", "eh" ],
        "213": [ "dz" ],
        "216": [ "tn" ],
        "218": [ "ly" ],
        "220": [ "gm" ],
        "221": [ "sn" ],
        "222": [ "mr" ],
        "223": [ "ml" ],
        "224": [ "gn" ],
        "225": [ "ci" ],
        "226": [ "bf" ],
        "227": [ "ne" ],
        "228": [ "tg" ],
        "229": [ "bj" ],
        "230": [ "mu" ],
        "231": [ "lr" ],
        "232": [ "sl" ],
        "233": [ "gh" ],
        "234": [ "ng" ],
        "235": [ "td" ],
        "236": [ "cf" ],
        "237": [ "cm" ],
        "238": [ "cv" ],
        "239": [ "st" ],
        "240": [ "gq" ],
        "241": [ "ga" ],
        "242": [ "cg" ],
        "243": [ "cd" ],
        "244": [ "ao" ],
        "245": [ "gw" ],
        "246": [ "io" ],
        "248": [ "sc" ],
        "249": [ "sd" ],
        "250": [ "rw" ],
        "251": [ "et" ],
        "252": [ "so" ],
        "253": [ "dj" ],
        "254": [ "ke" ],
        "255": [ "tz" ],
        "256": [ "ug" ],
        "257": [ "bi" ],
        "258": [ "mz" ],
        "260": [ "zm" ],
        "261": [ "mg" ],
        "262": [ "re"],
        "263": [ "zw" ],
        "264": [ "na" ],
        "265": [ "mw" ],
        "266": [ "ls" ],
        "267": [ "bw" ],
        "268": [ "sz" ],
        "269": [ "km", "yt" ],
        "290": [ "sh" ],
        "291": [ "er" ],
        "297": [ "aw" ],
        "298": [ "fo" ],
        "299": [ "gl" ],
        "350": [ "gi" ],
        "351": [ "pt" ],
        "352": [ "lu" ],
        "353": [ "ie" ],
        "354": [ "is" ],
        "355": [ "al" ],
        "356": [ "mt" ],
        "357": [ "cy" ],
        "358": [ "fi", "ax" ],
        "359": [ "bg" ],
        "370": [ "lt" ],
        "371": [ "lv" ],
        "372": [ "ee" ],
        "373": [ "md" ],
        "374": [ "am" ],
        "375": [ "by" ],
        "376": [ "ad" ],
        "377": [ "mc", "xk" ],
        "378": [ "sm" ],
        "379": [ "va" ],
        "380": [ "ua" ],
        "381": [ "rs" ],
        "382": [ "me" ],
        "385": [ "hr" ],
        "386": [ "si" ],
        "387": [ "ba" ],
        "389": [ "mk" ],
        "420": [ "cz" ],
        "421": [ "sk" ],
        "423": [ "li" ],
        "500": [ "fk", "gs" ],
        "501": [ "bz" ],
        "502": [ "gt" ],
        "503": [ "sv" ],
        "504": [ "hn" ],
        "505": [ "ni" ],
        "506": [ "cr" ],
        "507": [ "pa" ],
        "508": [ "pm" ],
        "509": [ "ht" ],
        "590": [ "gp", "bl", "mf" ],
        "591": [ "bo" ],
        "592": [ "gy" ],
        "593": [ "ec" ],
        "594": [ "gf" ],
        "595": [ "py" ],
        "596": [ "mq" ],
        "597": [ "sr" ],
        "598": [ "uy" ],
        "670": [ "tl" ],
        "672": [ "nf" ],
        "673": [ "bn" ],
        "674": [ "nr" ],
        "675": [ "pg" ],
        "676": [ "to" ],
        "677": [ "sb" ],
        "678": [ "vu" ],
        "679": [ "fj" ],
        "680": [ "pw" ],
        "681": [ "wf" ],
        "682": [ "ck" ],
        "683": [ "nu" ],
        "685": [ "ws" ],
        "686": [ "ki" ],
        "687": [ "nc" ],
        "688": [ "tv" ],
        "689": [ "pf" ],
        "690": [ "tk" ],
        "691": [ "fm" ],
        "692": [ "mh" ],
        "850": [ "kp" ],
        "852": [ "hk" ],
        "853": [ "mo" ],
        "855": [ "kh" ],
        "856": [ "la" ],
        "880": [ "bd" ],
        "886": [ "tw" ],
        "960": [ "mv" ],
        "961": [ "lb" ],
        "962": [ "jo" ],
        "963": [ "sy" ],
        "964": [ "iq" ],
        "965": [ "kw" ],
        "966": [ "sa" ],
        "967": [ "ye" ],
        "968": [ "om" ],
        "970": [ "ps" ],
        "971": [ "ae" ],
        "972": [ "il" ],
        "973": [ "bh" ],
        "974": [ "qa" ],
        "975": [ "bt" ],
        "976": [ "mn" ],
        "977": [ "np" ],
        "992": [ "tj" ],
        "993": [ "tm" ],
        "994": [ "az" ],
        "995": [ "ge" ],
        "996": [ "kg" ],
        "998": [ "uz" ],
        "1204": [ "ca" ],
        "1236": [ "ca" ],
        "1242": [ "bs" ],
        "1246": [ "bb" ],
        "1249": [ "ca" ],
        "1250": [ "ca" ],
        "1264": [ "ai" ],
        "1268": [ "ag" ],
        "1284": [ "vg" ],
        "1289": [ "ca" ],
        "1306": [ "ca" ],
        "1340": [ "vi" ],
        "1343": [ "ca" ],
        "1345": [ "ky" ],
        "1365": [ "ca" ],
        "1387": [ "ca" ],
        "1403": [ "ca" ],
        "1416": [ "ca" ],
        "1418": [ "ca" ],
        "1431": [ "ca" ],
        "1437": [ "ca" ],
        "1438": [ "ca" ],
        "1441": [ "bm" ],
        "1450": [ "ca" ],
        "1473": [ "gd" ],
        "1506": [ "ca" ],
        "1514": [ "ca" ],
        "1519": [ "ca" ],
        "1548": [ "ca" ],
        "1579": [ "ca" ],
        "1581": [ "ca" ],
        "1587": [ "ca" ],
        "1604": [ "ca" ],
        "1613": [ "ca" ],
        "1639": [ "ca" ],
        "1647": [ "ca" ],
        "1649": [ "tc" ],
        "1664": [ "ms" ],
        "1670": [ "mp" ],
        "1671": [ "gu" ],
        "1672": [ "ca" ],
        "1684": [ "as" ],
        "1705": [ "ca" ],
        "1709": [ "ca" ],
        "1721": [ "sx" ],
        "1742": [ "ca" ],
        "1758": [ "lc" ],
        "1767": [ "dm" ],
        "1778": [ "ca" ],
        "1780": [ "ca" ],
        "1782": [ "ca" ],
        "1784": [ "vc" ],
        "1787": [ "pr" ],
        "1807": [ "ca" ],
        "1809": [ "do" ],
        "1819": [ "ca" ],
        "1825": [ "ca" ],
        "1867": [ "ca" ],
        "1868": [ "tt" ],
        "1869": [ "kn" ],
        "1873": [ "ca" ],
        "1876": [ "jm" ],
        "1902": [ "ca" ],
        "1905": [ "ca" ],
        "4779": [ "sj" ],
        "5997": [ "bq" ],
        "5999": [ "cw" ]
    };
});