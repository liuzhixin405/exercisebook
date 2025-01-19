function onTradingViewReady() {
	var symbol = $('#transaction_code').val();
    var lang = "zh";
	if ("en_US" == locale) {
		lang = "en";
	}

    // btc2ckusd    btc2ckusd   eos2eth
    var mydata = {
        symbol: symbol || '',
        interval: '15',
        localStor: {}
    }

    if (localStorage['tradeinterval']) {
        mydata.localStor = JSON.parse(localStorage['tradeinterval']);
        mydata.interval = JSON.parse(localStorage['tradeinterval']).interval;
    }
    
    var _datafeed = new Datafeeds.UDFCompatibleDatafeed(window.location.origin + "/futuresApi/marketKline.do", 1800);
    var widget = window.tvWidget = new TradingView.widget({
        width: '100%',
        height: '100%',
        symbol: mydata.symbol,
        interval: mydata.interval,
        container_id: "tv_chart_container",
        timezone: "Asia/Shanghai",
        datafeed: _datafeed,
        library_path: window.location.origin + "/common/js/charting_library/",
        locale: lang,
        drawings_access: {
            type: 'black',
            tools: [{
                    name: "Regression Trend"
                }]
        },
        disabled_features: [
//            "use_localstorage_for_settings",
            "header_resolutions",
            "header_symbol_search",
            "header_chart_type",
            "header_compare",
            "header_undo_redo",
            "header_screenshot",
            "header_saveload",
            "header_widget",
            "study_templates", // 模板
            "timeframes_toolbar",
            "pane_context_menu",
            "volume_force_overlay",
        ],
        enabled_features: [
            "keep_left_toolbar_visible_on_small_screens",
            "adaptive_logo",
            "show_dom_first_time",
            "side_toolbar_in_fullscreen_mode",
            "move_logo_to_main_pane",
            "hide_last_na_study_output", // 隐藏成交量n/a
        ],
        studies_overrides: {
            "volume.volume.color.0": IsWhite ? "rgba(213, 89, 89, .7)" : "rgba(225, 99, 99, .7)",
            "volume.volume.color.1": IsWhite ? "rgba(98, 195, 120, .7)" : "rgba(71, 190, 135, .7)"
        },
        overrides: {
            // 处理指标 ＋ — 效果
            "paneProperties.legendProperties.showLegend": true,
            "paneProperties.legendProperties.showStudyArguments": true,
            "paneProperties.legendProperties.showStudyTitles": true,
            "paneProperties.legendProperties.showStudyValues": true,
            "paneProperties.legendProperties.showSeriesTitle": true,
            "paneProperties.legendProperties.showSeriesOHLC": true,
            //    蜡烛样式
            "mainSeriesProperties.candleStyle.upColor": IsWhite ? "#62C378" : "#47BE87",		// 62C378
            "mainSeriesProperties.candleStyle.downColor": IsWhite ? "#D55959" : "#E16363",	// D55959
            "mainSeriesProperties.candleStyle.borderUpColor": IsWhite ? "#62C378" : "#47BE87",
            "mainSeriesProperties.candleStyle.borderDownColor": IsWhite ? "#D55959" : "#E16363",
            "mainSeriesProperties.candleStyle.wickUpColor": IsWhite ? "#62C378" : "#47BE87",
            "mainSeriesProperties.candleStyle.wickDownColor": IsWhite ? "#D55959" : "#E16363",
            // 背景色
            "paneProperties.background": IsWhite ? "#ffffff" : "#0d1923",
            // 竖直网格线颜色
            "paneProperties.vertGridProperties.color": "rgba(148, 171, 192,0.1)",
            // 水平网格线颜色
            "paneProperties.horzGridProperties.color": "rgba(148, 171, 192,0.1)",
            // 十指光标颜色
            "paneProperties.crossHairProperties.color": "#4A5F78",
            // 坐标轴线颜色
            "scalesProperties.lineColor": IsWhite ? "#94ABC0" : "#4A5F78",
            // 坐标轴文本颜色
            "scalesProperties.textColor": IsWhite ? "#4A5F78" : "#94ABC0",
            // 分时图
            "mainSeriesProperties.areaStyle.color1": "rgba(0, 153, 195, .1)",
            "mainSeriesProperties.areaStyle.color2": "rgba(0, 153, 195, 0)",
            "mainSeriesProperties.areaStyle.linecolor": "rgba(0, 153, 195, 1)",
            // 成交量高度设置  large, medium, small, tiny
            "volumePaneSize": "medium",
            // 调整k线顶部的边距
            "paneProperties.topMargin": 8
        },
        custom_css_url: IsWhite ? 'bundles/whitek.css?v=10' : 'bundles/newk.css?v=10', // 样式表
        toolbar_bg: IsWhite ? '#ffffff' : '#0d1923',
        theme: IsWhite ? 'Light' : 'Dark',
        loading_screen: { backgroundColor: IsWhite ? "#ffffff" : "#0d1923" }
    });

    widget.onChartReady(function () {

        // 学习线 
    	var localStudies = localStorage['tv.studies'];
    	if (!localStudies || localStudies.length == 0 ) {
	        widget.chart().createStudy('Moving Average', false, false, [5], {});
	        widget.chart().createStudy('Moving Average', false, false, [10], {});
	        widget.chart().createStudy('Moving Average', false, false, [30], {});
	        widget.chart().createStudy('Moving Average', false, false, [60], {});
    	} else {
    		var studies =  JSON.parse(localStudies);
    		$.each(studies, function(i, study) {
    			var inputs = study.inputs;
    			var styles = {};
				$.each(study.styles, function(index, item) {
					$.each(item, function(id, it) {
						var key = index + "." + id;
						var val = it;
						styles[key] = val;
					});
				});
			
				widget.chart().createStudy(study.name, false, false, inputs, styles);
    		});
    	}
        
        var name = "line";
	    if (mydata.localStor.type != 3) {
		    for (var item in TradingViewPeriodMap) {
		    	var val = TradingViewPeriodMap[item];
		    	if (val == mydata.interval) {
		    		name = item;
		    		break;
		    	}
		    }
	    } else {
	    	window.tvWidget.chart().setChartType(mydata.localStor.type);
	    }
        
        $("#chart_toolbar .chart_toolbar_tabgroup a").removeClass("selectsed");
	    $("#chart_toolbar_periods_vert ul a").removeClass("selectsed");
	    $("#chart_toolbar .chart_toolbar_tabgroup a").each(function () {
	        if ($(this).parent().attr("name") == name) {
	            $(this).addClass("selectsed")
	        }
	    });
	    $("#chart_toolbar_periods_vert ul a").each(function () {
	        if ($(this).parent().attr("name") == name) {
	        	$(".chart_dropdown_t .chart_adio").html($(this).html());
	            $(this).addClass("selectsed")
	        }
	    });
	    
	    
	    setTimeout("subscribeEvent()", 2000);
    	
    });
}

function subscribeEvent() {
	var widget = window.tvWidget;
	
	// 创建指标
	widget.subscribe("study", function(e) {
		setTimeout("storeAllStudies()", 20);
	});
	
	// 更改指标
	widget.subscribe("study_properties_changed", function(e) {
		setTimeout("storeAllStudies()", 20);
	});
	
	// 删除指标
	widget.subscribe("study_event", function(e) {
		setTimeout("storeAllStudies()", 20);
	});
}

function storeAllStudies() {
	var widget = window.tvWidget;
	var studies = widget.activeChart().getAllStudies();
	var localStudies = [];
	$(studies).each(function() {
		var sty = widget.activeChart().getStudyById(this.id)._study;
		if (sty._metaInfo.is_price_study) {
			var study = {};
			study.name = this.name;
			study.styles = {};
			study.inputs = [];
			
			var properties = sty._properties;
			var styles = widget.activeChart().getStudyById(this.id)._study._properties.styles;
			var inputs = widget.activeChart().getStudyById(this.id)._study._inputs;
			$.each(inputs,function(index, item){
				study.inputs.push(item);
	        });
			$(styles._childs).each(function(){
				var filed = styles[this];
				var pro = {};
				$(filed._childs).each(function(){
					if (this != 'title') {
						pro[this] = filed[this]._value;
					}
				});
				study.styles[filed.title._value] = pro;
			});
			
			localStudies.push(study);
		}
	});
	
	localStorage.setItem('tv.studies', JSON.stringify(localStudies));
}

function stopTradingViewUpdate() {
	window.tvWidget._options.datafeed.stopUpdate();
}

function startTradingViewUpdate() {
	window.tvWidget._options.datafeed.startUpdate();
}

function updateTradingView() {
	window.tvWidget._options.datafeed.updateData();
}

$(function () {
    if (typeof(TradingView) != "undefined" && TradingView != null) {
	    TradingView.onready(function () {
	    	onTradingViewReady();
	    });
    } else {
    	var script=document.createElement('script');
		script.type="text/javascript";
		if(script.readyState){
			script.onreadystatechange = function() {
				if(script.readyState == "loaded" || script.readyState == "complete"){
					script.onreadystatechange = null;
					onTradingViewReady();
				}
			}
		} else {
			script.onload = function(){
				onTradingViewReady();
			}
		}
		script.src = window.location.origin + "/common/js/charting_library/charting_library.min.js?v=1.15";
		document.head.appendChild(script);
    }
})
