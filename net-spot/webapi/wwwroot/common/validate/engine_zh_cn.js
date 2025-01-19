(function($) {
	$.fn.validationEngineLanguage = function() {
	};
	
	var ajaxurl = "https://www.bfx.un/account/"

	$.validationEngineLanguage = {
		newLang : function() {
			$.validationEngineLanguage.allRules = {
				"required" : { // Add your regex rules here, you can take telephone as an example
					"regex" : "none",
					"alertText" : "* 不可为空",
					"alertTextCheckboxMultiple" : "* 请选择一个项目",
					"alertTextCheckboxe" : "* 您必须钩选此栏",
					"alertTextDateRange" : "* 日期范围不可空白"
				},
				"dateRange" : {
					"regex" : "none",
					"alertText" : "* 无效的 ",
					"alertText2" : " 日期范围"
				},
				"dateTimeRange" : {
					"regex" : "none",
					"alertText" : "* 无效的 ",
					"alertText2" : " 时间范围"
				},
				"minSize" : {
					"regex" : "none",
					"alertText" : "* 最少 ",
					"alertText2" : " 个字符"
				},
				"maxSize" : {
					"regex" : "none",
					"alertText" : "* 最多 ",
					"alertText2" : " 个字符"
				},
				"groupRequired" : {
					"regex" : "none",
					"alertText" : "* 你必需选填其中一个栏位"
				},
				"min" : {
					"regex" : "none",
					"alertText" : "* 最小值为"
				},
				"max" : {
					"regex" : "none",
					"alertText" : "* 最大值为 "
				},
				"maxBuy":{
					"regex" : "none",
					"alertText" : "* 您当前最多能购买 "
				},
				"past" : {
					"regex" : "none",
					"alertText" : "* 日期必需早于 "
				},
				"future" : {
					"regex" : "none",
					"alertText" : "* 日期必需晚于 "
				},
				"maxCheckbox" : {
					"regex" : "none",
					"alertText" : "* 最多选取 ",
					"alertText2" : " 个项目"
				},
				"minCheckbox" : {
					"regex" : "none",
					"alertText" : "* 请选择 ",
					"alertText2" : " 个项目"
				},
				"equals" : {
					"regex" : "none",
					"alertText" : "* 请输入与上面相同的密码"
				},
				"equals_email" : {
					"regex" : "none",
					"alertText" : "* 请输入与上面相同的邮箱地址"
				},
				"phone" : {
					// credit: jquery.h5validate.js / orefalo
					"regex" : /^([\+][0-9]{1,3}[ \.\-])?([\(]{1}[0-9]{2,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$/,
					"alertText" : "* 无效的电话号码"
				},
				"email" : {
					// Shamelessly lifted from Scott Gonzalez via the Bassistance Validation plugin http://projects.scottsplayground.com/email_address_validation/
					"regex" : /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))[a-zA-z]$/i,
					"alertText" : "* 邮件地址无效"
				},
				"integer" : {
					"regex" : /^[\-\+]?\d+$/,
					"alertText" : "* 不是有效的整数"
				},
				"block_integer" : {
					"regex" : /^\d+(00)$/,
					"alertText" : "* 不是有效的比特币数量"
				},
				"number" : {
					// Number, including positive, negative, and floating decimal. credit: orefalo
					"regex" : /^[\-\+]?(([0-9]+)([\.,]([0-9]+))?|([\.,]([0-9]+))?)$/,
					"alertText" : "* 无效的数字"
				},
				"pnumber" : {
					// Number, including positive, negative, and floating decimal. credit: orefalo
					"regex" : /^(([0-9]+)([\.,]([0-9]+))?|([\.,]([0-9]+))?)$/,
					"alertText" : "* 无效的数字"
				},
				"date" : {
					"regex" : /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])$/,
					"alertText" : "* 无效的日期，格式必需为 YYYY-MM-DD"
				},
				"time" : {
					"regex" : /^([[0-1][0-9]|2[0-3]]):([0-5][0-9]):([0-5][0-9])$/,
					"alertText" : "* 无效的时间，格式必需为 HH:mm:ss"
				},
				"ipv4" : {
					"regex" : /^((([01]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))[.]){3}(([0-1]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))$/,
					"alertText" : "* 无效的 IP 地址"
				},
				"url" : {
					"regex" : /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i,
					"alertText" : "* Invalid URL"
				},
				"onlyNumberSp" : {
					"regex" : /^[0-9\ ]+$/,
					"alertText" : "* 只能填数字"
				},
				"loanRmbSp" : {
					"regex" : /^(([1-9]{1}\d*)|([0]{1}))(\.(\d){1,2})?$/,
					"alertText" : "* 请输入正确的金额"
				},
				"loanBtcSp" : {
					"regex" : /^(([1-9]{1}\d*)|([0]{1}))(\.(\d){1,4})?$/,
					"alertText" : "* 请输入正确数量的比特币"
				},
				"currency_xiaoshu" : {
					"regex" : /^(([1-9]{1}\d*)|([0]{1}))\.(\d){2}$/,
					"alertText" : "* 请输入小数点后两位"
				},
				'interest' : {
					"regex" : /^(([1-9]{1}\d*)|([0]{1}))(\.(\d){1,2})?$/,
					"alertText" : "* 请输入正确利率"
				},
				'yuyue_interest' : {
					"regex" : /^(([1-9]{1}\d*)|([0]{1}))(\.(\d){1,2})?$/,
					"alertText" : "* 最小单位为0.01%"
				},
				"btcAddress" : {
					"regex" : /^[1|3][a-zA-Z1-9]{26,33}$/,
					"alertText" : "* 请输入正确的比特币地址"
				},
				"onlyLetterSp" : {
					"regex" : /^[a-zA-Z\ \']+$/,
					"alertText" : "* 只接受英文字母大小写"
				},
				"onlyLetterNumber" : {
					"regex" : /^[0-9a-zA-Z]+$/,
					"alertText" : "* 不接受特殊字符"
				},
				"notOnlyNumberOrLetter" : {
					"regex" : /^\w*(([a-z][0-9])|([0-9][a-z]))[`~!@#$%^&*()_+-=]*\w*$/i,
					"alertText" : " * 请输入符合要求的密码"
				},
				"illegal" : {
					"regex" : /[^-+=|,0-9a-zA-Z!@#$%^&*?_.~+\/\\(){}\[\]<>]/g,
					"alertText" : "* 不能包含非法字符"
				},
				"allNumber" : {
					"regex" : /^\d+$/,
					"alertText" : "* 不能是纯数字"
				},
				"allLetter" : {
					"regex" : /^[a-zA-Z]+$/,
					"alertText" : "* 不能是纯字母"
				},
				"allCharacter" : {
					"regex" : /^[-+=|,!@#$%^&*?_.~+\/\\(){}\[\]<>]+$/,
					"alertText" : "* 不能是纯符号"
				},
				"allSame" : {
					"regex" : /^([\s\S])\1*$/,
					"alertText" : "* 不能为同一字符"
				},
				
				// --- CUSTOM RULES -- Those are specific to the demos, they can be removed or changed to your likings
				"ajaxUserCall" : {
					"url" : "ajaxValidateFieldUser",
					// you may want to pass extra data on the ajax call
					"extraData" : "name=eric",
					"alertText" : "* 此名称已被其他人使用"
				//"alertTextLoad": "* 正在确认名称是否有其他人使用，请稍等。"
				},
				"ajax_check_email" : {
					"url" : ajaxurl + "ajax.php?m=check_email",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* 此帐号名称可以使用",
					"alertText" : "* 此帐号不可用"
				//"alertTextLoad": "* 正在确认帐号名称是否有其他人使用，请稍等。"
				},
				"ajax_check_email_exist" : {
					"url" : ajaxurl + "ajax.php?m=check_email_exist",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* 输入正确",
					"alertText" : "* 该邮箱未注册"
				//"alertTextLoad": "* 正在确认帐号是否存在，请稍等……"
				},
				"ajax_check_vcode" : {
					"url" : ajaxurl + "ajax.php?m=check_vcode&jsoncallback=?",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* 验证OK",
					"alertText" : "* 此验证码不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				'ajax_check_google_code' : {
					'url' : ajaxurl + 'ajax.php?m=check_google_code&jsoncallback=?',
					'alertText' : '* 双重验证密码错误'
				},
				"ajax_check_bind_phone" : {
					"url" : ajaxurl + "ajax.php?m=check_bind_phone",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* 手机号码OK",
					"alertText" : "* 此手机号不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},

				"ajax_check_sms" : {
					"url" : ajaxurl + "ajax.php?m=check_sms",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* 验证OK",
					"alertText" : "* 此验证不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},

				//两个验证码的情况
				"ajax_check_sms_0" : {
					"url" : ajaxurl + "ajax.php?m=check_sms",
					// you may want to pass extra data on the ajax call
					"extraData" : "id=0",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* 验证OK",
					"alertText" : "* 此验证码不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_check_sms_1" : {
					"url" : ajaxurl + "ajax.php?m=check_sms",
					// you may want to pass extra data on the ajax call
					"extraData" : "id=1",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* 验证OK",
					"alertText" : "* 此验证不可用"
				// "alertTextLoad": "* 正在验证，请稍等。"
				},
				//实盘
				"ajax_check_buy_price" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_buy_price&jsoncallback=?",
					"alertText" : "* 此价格不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_check_buy_volume" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_buy_volume&jsoncallback=?",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_check_sell_price" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_sell_price&jsoncallback=?",
					"alertText" : "* 此价格不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_check_sell_volume" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_sell_volume&jsoncallback=?",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				//st
				"ajax_st_check_buy_price" : {
					"url" : ajaxurl
							+ "ajax.php?m=st_check_buy_price&jsoncallback=?",
					"alertText" : "* 此价格不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_st_check_buy_volume" : {
					"url" : ajaxurl
							+ "ajax.php?m=st_check_buy_volume&jsoncallback=?",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_st_check_sell_price" : {
					"url" : ajaxurl
							+ "ajax.php?m=st_check_sell_price&jsoncallback=?",
					"alertText" : "* 此价格不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_st_check_sell_volume" : {
					"url" : ajaxurl
							+ "ajax.php?m=st_check_sell_volume&jsoncallback=?",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_check_deposit_amount" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_deposit_amount&jsoncallback=?",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* OK",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_check_deposit_amount_tpay" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_deposit_amount_tpay&jsoncallback=?",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* OK",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},

				"ajax_check_cny_withdraw_amount" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_cny_withdraw_amount&jsoncallback=?",
					// you may want to pass extra data on the ajax call
					//"extraData": "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* OK",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajax_check_btc_withdraw_amount" : {
					"url" : ajaxurl
							+ "ajax.php?m=check_btc_withdraw_amount&jsoncallback=?",
					// you may want to pass extra data on the ajax call
					"extraData" : "name=eric",
					// if you provide an "alertTextOk", it will show as a green prompt when the field validates
					//"alertTextOk": "* OK",
					"alertText" : "* 此量不可用"
				//"alertTextLoad": "* 正在验证，请稍等。"
				},
				"ajaxNameCall" : {
					// remote json service location
					"url" : "ajaxValidateFieldName",
					// error
					"alertText" : "* 此名称已被其他人使用"
				// if you provide an "alertTextOk", it will show as a green prompt when the field validates
				//"alertTextOk": "* 此名称可以使用",
				// speaks by itself
				//"alertTextLoad": "* 正在确认名称是否有其他人使用，请稍等。"
				},
				"ajaxTitleCheckCall" : {
					// remote json service location
					"url" : "/admin_seo/ajaxTitleCheckCall",
					// error
					"alertText" : "* 此标题已经存在"
				// speaks by itself
				//"alertTextLoad": "* 正在确认标题是否存在，请稍等。"
				},
				"validate2fields" : {
					"alertText" : "* 请输入 HELLO"
				},
				//tls warning:homegrown not fielded 
				"dateFormat" : {
					"regex" : /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])$|^(?:(?:(?:0?[13578]|1[02])(\/|-)31)|(?:(?:0?[1,3-9]|1[0-2])(\/|-)(?:29|30)))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(?:(?:0?[1-9]|1[0-2])(\/|-)(?:0?[1-9]|1\d|2[0-8]))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(0?2(\/|-)29)(\/|-)(?:(?:0[48]00|[13579][26]00|[2468][048]00)|(?:\d\d)?(?:0[48]|[2468][048]|[13579][26]))$/,
					"alertText" : "* 无效的日期格式"
				},
				//tls warning:homegrown not fielded 
				"dateTimeFormat" : {
					"regex" : /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])\s+(1[012]|0?[1-9]){1}:(0?[1-5]|[0-6][0-9]){1}:(0?[0-6]|[0-6][0-9]){1}\s+(am|pm|AM|PM){1}$|^(?:(?:(?:0?[13578]|1[02])(\/|-)31)|(?:(?:0?[1,3-9]|1[0-2])(\/|-)(?:29|30)))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^((1[012]|0?[1-9]){1}\/(0?[1-9]|[12][0-9]|3[01]){1}\/\d{2,4}\s+(1[012]|0?[1-9]){1}:(0?[1-5]|[0-6][0-9]){1}:(0?[0-6]|[0-6][0-9]){1}\s+(am|pm|AM|PM){1})$/,
					"alertText" : "* 无效的日期或时间格式",
					"alertText2" : "可接受的格式： ",
					"alertText3" : "mm/dd/yyyy hh:mm:ss AM|PM 或 ",
					"alertText4" : "yyyy-mm-dd hh:mm:ss AM|PM"
				},
				'invalid_no' : {
					"alertText" : "无效的身份证"
				}
			};

		}
	};
	$.validationEngineLanguage.newLang();
})(jQuery);


function checkSameMail(field, rules, i, options){
	if(field.val() !== $('#email').val()){
		return '* 请输入与上面相同的邮箱地址';
	}
}

function checkSameNo(field, rules, i, options){
  if(field.val() !== $('#no').val()){
    return '* 请输入与上面相同的证件号码';
  }
}

function validate_date(field, rules, i, options) {
  var begin = $.trim($('#date_begin').val());
  var end = $.trim($('#date_end').val());
  if (begin && end) {
    var s = Date.parse(begin);
    var e = Date.parse(end);
    if (e < s) {
      return '* 结束时间应大于开始时间';
    }
  }
}

function validate_time(field, rules, i, options){
	var s = $.trim($('#form-validation-field-0').val());
	var e = $.trim($('#form-validation-field-1').val());
	if(s && e){
		if(!compareTime(s,e)){
			return '* 结束时间应大于开始时间';
		}
	}
}

function compareTime(time_s,time_e){
	var D = new Date();
	var now = D.getFullYear()+'/'+ (D.getMonth()+1)+'/'+ D.getDate();
	var s = Date.parse(now+' '+time_s);
	var e = Date.parse(now+' '+time_e);
	return e>s;
}

function getIdCardInfo(cardNo) {
    var info = {
        isTrue : false, // 身份证号是否有效。默认为 false
        year : null,// 出生年。默认为null
        month : null,// 出生月。默认为null
        day : null,// 出生日。默认为null
        isMale : false,// 是否为男性。默认false
        isFemale : false // 是否为女性。默认false
    };

    if (!cardNo && 15 != cardNo.length && 18 != cardNo.length) {
        info.isTrue = false;
        return info;
    }

    if (15 == cardNo.length) {
        var year = cardNo.substring(6, 8);
        var month = cardNo.substring(8, 10);
        var day = cardNo.substring(10, 12);
        var p = cardNo.substring(14, 15); // 性别位
        var birthday = new Date(year, parseFloat(month) - 1, parseFloat(day));
        // 对于老身份证中的年龄则不需考虑千年虫问题而使用getYear()方法
        if (birthday.getYear() != parseFloat(year)
            || birthday.getMonth() != parseFloat(month) - 1
            || birthday.getDate() != parseFloat(day)) {
            info.isTrue = false;
        } else {
            info.isTrue = true;
            info.year = birthday.getFullYear();
            info.month = birthday.getMonth() + 1;
            info.day = birthday.getDate();
            if (p % 2 == 0) {
                info.isFemale = true;
                info.isMale = false;
            } else {
                info.isFemale = false;
                info.isMale = true;
            }
        }
        return info;
    }

    if (18 == cardNo.length) {
        var year = cardNo.substring(6, 10);
        var month = cardNo.substring(10, 12);
        var day = cardNo.substring(12, 14);
        var p = cardNo.substring(14, 17);
        var birthday = new Date(year, parseFloat(month) - 1, parseFloat(day));
        // 这里用getFullYear()获取年份，避免千年虫问题
        if (birthday.getFullYear() != parseFloat(year)
            || birthday.getMonth() != parseFloat(month) - 1
            || birthday.getDate() != parseFloat(day)) {
            info.isTrue = false;
            return info;
        }

        var Wi = [ 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 ];// 加权因子
        var Y = [ 1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2 ];// 身份证验证位值.10代表X

        // 验证校验位
        var sum = 0; // 声明加权求和变量
        var _cardNo = cardNo.split("");

        if (_cardNo[17].toLowerCase() == 'x') {
            _cardNo[17] = 10;// 将最后位为x的验证码替换为10方便后续操作
        }
        for ( var i = 0; i < 17; i++) {
            sum += Wi[i] * _cardNo[i];// 加权求和
        }
        var i = sum % 11;// 得到验证码所位置

        if (_cardNo[17] != Y[i]) {
            return info.isTrue = false;
        }

        info.isTrue = true;
        info.year = birthday.getFullYear();
        info.month = birthday.getMonth() + 1;
        info.day = birthday.getDate();

        if (p % 2 == 0) {
            info.isFemale = true;
            info.isMale = false;
        } else {
            info.isFemale = false;
            info.isMale = true;
        }
        return info;
    }
    return info;
}

function check_user_no(field, rules, i, options){
    var no_type = $("#type").val();
    if(no_type == 0){
        var no = field.val();
        var info = getIdCardInfo(no);
        if(info.isTrue == false){
            return options.allrules.invalid_no.alertText;
        }
    }
}