var page = require('webpage').create(),
	system = require('system');

var url = system.args[1];

page.onError = function(msg, trace) {
    var msgStack = ['ERROR: ' + msg];
    if (trace && trace.length) {
        msgStack.push('TRACE:');
        trace.forEach(function(t) {
            msgStack.push(' -> ' + t.file + ': ' + t.line + (t.function ? ' (in function "' + t.function + '")' : ''));
        });
    }
    // uncomment to log into the console 
    console.error(msgStack.join('\n'));
};

page.open(url, function() {
	page.evaluate(function() {
		document.getElementById("charType3").checked = true;
		document.getElementById("priceMovingAverage1").value = 10;
		document.getElementById("volumeIndicator").value = "BarMA";
		document.getElementById("volumeMovingAverage").value = 10;
		drawChart();
	});
		
	var clipRect = page.evaluate(function() { return document.querySelector("svg").getBoundingClientRect(); });
	page.clipRect = clipRect;
	page.render('chart.png');
	phantom.exit();
});