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
		var clipRect = page.evaluate(function() { return document.querySelector("svg").getBoundingClientRect(); });
		page.clipRect = clipRect;
        page.render('chart.png');
        phantom.exit();
});