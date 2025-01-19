function processData(list, type, desc, res) {
    for(var i = 0; i < list.length; i++) {
      list[i] = {
        value: Number(list[i][0]),
        volume: Number(list[i][1]),
      }
    }
    list.sort(function(a, b) {
      if (a.value > b.value) {
        return 1;
      }
      else if (a.value < b.value) {
        return -1;
      }
      else {
        return 0;
      }
    });
    
    if (desc) {
      for(var i = list.length - 1; i >= 0; i--) {
        if (i < (list.length - 1)) {
          list[i].totalvolume = list[i+1].totalvolume + list[i].volume;
        }
        else {
          list[i].totalvolume = list[i].volume;
        }
        var dp = {};
        dp["value"] = list[i].value;
        dp[type + "volume"] = list[i].volume;
        dp[type + "totalvolume"] = list[i].totalvolume;
        res.unshift(dp);
      }
    }
    else {
      for(var i = 0; i < list.length; i++) {
        if (i > 0) {
          list[i].totalvolume = list[i-1].totalvolume + list[i].volume;
        }
        else {
          list[i].totalvolume = list[i].volume;
        }
        var dp = {};
        dp["value"] = list[i].value;
        dp[type + "volume"] = list[i].volume;
        dp[type + "totalvolume"] = list[i].totalvolume;
        res.push(dp);
      }
    }
   
  }

var chart = AmCharts.makeChart("chart-depath", {
  "type": "serial",
  "theme": "light",
  "dataLoader": {
    "url": "",
    "format": "json",
    "reload": 3,
    "postProcess": function(data) {
      var res = [];
      processData(data.bids, "bids", true, res);
      processData(data.asks, "asks", false, res);
      return res;
    }
  },
  "graphs": [{
    "id": "bids",
    "fillAlphas": 0.1,
    "lineAlpha": 1,
    "lineThickness": 2, 
    "lineColor": "#0f0",
    "type": "step",
    "valueField": "bidstotalvolume",
    "balloonFunction": balloon,
    "categoryBalloonAlpha":0,
  }, {
    "id": "asks",
    "fillAlphas": 0.1,
    "lineAlpha": 1,
    "lineThickness": 2, 
    "lineColor": "#f00",
    "type": "step",
    "valueField": "askstotalvolume",
    "balloonFunction": balloon,
    "categoryBalloonAlpha":0,
  }, {
    "lineAlpha": 0,
    "fillAlphas": 0, 
    "lineColor": "#000",
    "type": "column",
    "clustered": true, 
    "valueField": "bidsvolume",
    "showBalloon": false
  }, {
    "lineAlpha": 0,
    "fillAlphas": 0, 
    "lineColor": "#000",
    "type": "column",
    "clustered": true, 
    "valueField": "asksvolume",
    "showBalloon": false
  }],
  "categoryField": "value",
  "chartCursor": {
	"categoryBalloonEnabled": false,
	"cursorAlpha":0,
	"bulletsEnabled":true,
	"bulletSize":10,
	"graphBulletAlpha":0.1,
	"zoomable":false,
  },
  "balloon": {
	"adjustBorderColor": true,
	"color": "#4a5f78",
	"cornerRadius": 1,
	"borderAlpha":0.1,
	"shadowColor":"FFFFFF",
	"fillColor": "#FFFFFF",
  },
  "valueAxes": [{
    "title": "",
    "gridAlpha":0, 
    "axisColor": IsWhite ? "#4a5f78" : "#94abc0",
    "color":IsWhite ? "#4a5f78" : "#94abc0",
  }],
  "categoryAxis": {
    "title": "",
    "minHorizontalGap": 100,
    "startOnAxis": true,
    "showFirstLabel": false,
    "showLastLabel": false,
    "gridAlpha":0,
    "axisColor":IsWhite ? "#4a5f78" : "#94abc0",
	"color":IsWhite ? "#4a5f78" : "#94abc0",
	"autoResize":false,
  },
});

function balloon(item, graph) {
  var txt;
  if (graph.id == "asks") {
    txt = "<fmt:message key='msg.pg.market.chartSell'/>: <strong>" + formatNumber(item.dataContext.value, graph.chart, 4) + "</strong><br />"
      + "<fmt:message key='msg.pg.market.chartTotalAmount'/>: <strong>" + formatNumber(item.dataContext.askstotalvolume, graph.chart, 4) + "</strong><br />"
      + "<fmt:message key='msg.pg.market.chartCurrentlyAmount'/>: <strong>" + formatNumber(item.dataContext.asksvolume, graph.chart, 4) + "</strong>";
  }
  else {
    txt = "<fmt:message key='msg.pg.market.chartBuy'/>: <strong>" + formatNumber(item.dataContext.value, graph.chart, 4) + "</strong><br />"
      + "<fmt:message key='msg.pg.market.chartTotalAmount'/>: <strong>" + formatNumber(item.dataContext.bidstotalvolume, graph.chart, 4) + "</strong><br />"
      + "<fmt:message key='msg.pg.market.chartCurrentlyAmount'/>: <strong>" + formatNumber(item.dataContext.bidsvolume, graph.chart, 4) + "</strong>";
  }
  return txt;
}

function formatNumber(val, chart, precision) {
  return AmCharts.formatNumber(
    val, 
    {
        precision: precision ? precision : chart.precision, 
        decimalSeparator: chart.decimalSeparator,
        thousandsSeparator: chart.thousandsSeparator
    }
  );
}