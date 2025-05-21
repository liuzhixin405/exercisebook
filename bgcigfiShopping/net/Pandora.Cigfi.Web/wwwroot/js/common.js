//交易对审核状态
function TickerStatusformation(value, row, index) {
    switch (value) {
        case -1:
            value = "<p style='color:red'>审核不通过</p>";
            break;
        case -2:
            value = "<p style='color:red'>行情不更新</p>";
            break;
        case -3:
            value = "<p style='color:red'>永久下架</p>";
            break;
        case -4:
            value = "<p style='color:red'>价差太大</p>";
            break;
        case -5:
            value = "<p style='color:red'>交易对1为空</p>";
            break;
        case 0:
            value = "<p style='color:goldenrod'>待审核</p>";
            break;
        case 1:
            value = "<p style='color:green'>审核通过</p>";
            break;
        case 2:
            value = "<p style='color:Highlight'>待确认</p>";
            break;
        default:
            value = "<p style='color:red'>未知</p>";
            break;
    }
    return value;
}

function timeTransformation(value, row, index) {

  
    if (null ==value||value === "0001-01-01T00:00:00" || value === "1990/1/1 0:00:00"  || value ==="0001/1/1 0:00:00") {
        value = "-";
    } else {
        var date = new Date(value);
        var year = date.getFullYear().toString().substring(2),
            month = date.getMonth() + 1,//月份是从0开始的
            day = date.getDate(),
            hour = date.getHours(),
            min = date.getMinutes(),
            sec = date.getSeconds();
        var newTime = year + '-' +
            month + '-' +
            day + '<br>' +
            hour + ':' +
            min + ':' +
            sec;
        value = newTime;
    }
    return value;
}


function timeTransformationExt(value, row, index) {
   
    if (value === "0001-01-01T00:00:00" || value === "1990/1/1 0:00:00" || value === "0001/1/1 0:00:00") {
        value = "-";
    } else {
        var date = new Date(value * 1000);

        var year = date.getFullYear(),
            month = date.getMonth() + 1,//月份是从0开始的
            day = date.getDate(),
            hour = date.getHours(),
            min = date.getMinutes(),
            sec = date.getSeconds();
        var newTime = year + '-' +
            month + '-' +
            day + '<br>' +
            hour + ':' +
            min + ':' +
            sec;
        value = newTime;
    }
    return value;
}


function unixtimeTransformation(value, row, index) {
    return fromUnixTime(value);
   
}

function fromUnixTime(time) {
    let unixtime = time;
    let unixTimestamp = new Date(unixtime * 1000);
    let Y = unixTimestamp.getFullYear()
          let M = ((unixTimestamp.getMonth() + 1) >= 10 ? (unixTimestamp.getMonth() + 1) : '0' + (unixTimestamp.getMonth() + 1))
         let D = (unixTimestamp.getDate() >= 10 ? unixTimestamp.getDate() : '0' + unixTimestamp.getDate())
    let HH = (unixTimestamp.getHours() >= 10 ? unixTimestamp.getHours() : '0' + unixTimestamp.getHours())
    let MM = (unixTimestamp.getMinutes() >= 10 ? unixTimestamp.getMinutes() : '0' + unixTimestamp.getMinutes())
    let SS = (unixTimestamp.getSeconds() >= 10 ? unixTimestamp.getSeconds() : '0' + unixTimestamp.getSeconds())
    let toDay = Y + '-' + M + '-' + D + ' ' + HH + ':' + MM + ':' + SS;
    return toDay;
}
//产生随机数函数
function RndNum(n) {
    var rnd = "";
    for (var i = 0; i < n; i++)
        rnd += Math.floor(Math.random() * 10);
    return rnd;
}

function sortBy_Header( tagID,columnName) {
 
    tagID = "#" + tagID;
    var number = $(tagID).attr("data-number");
    if (number == "1") {
        $(tagID + " .Sort_Down").hide();
        $(tagID + " .Sort_Up").show();
        $(tagID).attr("data-number", "0");
        $("#btn_query").attr("data-key", columnName);
        $("#btn_query").attr("data-value", "ASC");
    } else {
        $(tagID + " .Sort_Down").show();
        $(tagID + " .Sort_Up").hide();
        $(tagID).attr("data-number", "1");
        $("#btn_query").attr("data-key", columnName);
        $("#btn_query").attr("data-value", "DESC");
    }
    doSearch();
}

function getCheckBoxIds(clsname) {
    var ids = '';
    $('.' + clsname).each(function (i, elemt) {
        if ($(this).is(':checked')) {
            ids += $(this).attr('cid') + ',';
        }
    });
    return ids;
}