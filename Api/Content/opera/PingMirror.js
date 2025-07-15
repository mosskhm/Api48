// This script is used to mirror incoming HTTP pingbacks.
//
// The functionality is a part of the OPX Payment Enable v3. The script should
// "forward" an incoming request headers to the URL specified in the GET variable.
//
// Copyright (c) 2014, Opera Sopftware ASA. All rights reserved. 
//
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// * Redistributions of source code must retain the above copyright 
//    notice, this list of conditions and the following disclaimer. 
// * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
// * Neither the name of AdMarvel/Opera Software nor the names of its 
//    contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission. 
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL AdMarvel, Inc. BE LIABLE FOR ANY 
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

var http = require('http');
var https = require('https');
var url = require('url');

var scriptVersion = '190702.2';    // Script version
var PORT;                          // Port to listen on for incomming requests
http.globalAgent.maxSockets = 100; // Number of simultaneous ping forwards

// GET parameters to be forwarded as HTTP headers
var PARAMETERS_TO_HEADERS = [];

// Headers NOT to forward
var EXCLUDED_HEADERS = [
    'authorization',
    'cache-control',
    'cookie',
    'connection',
    'content-length',
    'content-md5',
    'host',
    'origin',
    'proxy-authorization'
];

// List of domains to which ping forwarding is allowed (Opera Mini transcoders)
var ALLOWED_TARGET_DOMAINS = [
    'opera.com',
    'opera-mini.net',
    'operamini.com',
    'op-test.net'];

// Max number of URLs that a ping can be forwarded to
var MAX_TARGET_URLS = 3;

//
// Return some debug information, in HTML format, including incoming HTTP headers
//
function createDebugOutput(request) {
    var output = '<h1>Debug information</h1>';

    output += '<b>Script version:</b> ' + scriptVersion;
    // Print all incoming HTTP headers
    output += '<h2>HTTP headers</h2><table border="1">';
    for (var header in request.headers) {
        if (Object.prototype.hasOwnProperty.call(request.headers, header)) {
            output += '<tr><td>' + header + '</td><td>'
                + request.headers[header] + '</td></tr>';
        }
    }
    output += '</table>';

    // Print all incoming request paramters
    output += '<h2>GET parameters</h2><table border="1">';
    var parameters = url.parse(request.url, true)['query'];
    for (var parameter in parameters) {
        if (Object.prototype.hasOwnProperty.call(parameters, parameter)) {
            output += '<tr><td>' + parameter + '</td><td>'
                + parameters[parameter] + '</td></tr>';
        }
    }
    output += '</table>';
    return output;
}

//
// Print usage information
//
function printUsage() {
    console.log("Usage: %s %s <port>", process.argv[0], process.argv[1]);
}

//
// Check that all required GET paramters are present and valid
//
function validateGetParameters(getParameters) {
    //Object.prototype.hasOwnProperty.call(getParameters,'t')

    if (!Object.prototype.hasOwnProperty.call(getParameters, 't') || getParameters['t'].length < 1) {
        return 'Missing mandatory parameter t';
    }

    if (!Object.prototype.hasOwnProperty.call(getParameters, 'pid') || getParameters['pid'].length < 1) {
        return 'Missing mandatory parameter pid';
    }
    return true;
}

//
// Check if the ping destination is whitelisted
//
function isPingDestinationWhitelisted(domain) {
    for (var i = 0; i < ALLOWED_TARGET_DOMAINS.length; i++) {
        // Match whitelisted domains including subdomains
        // For example: /^opera\.com$|.*\.opera\.com $/i
        var regex = new RegExp('^'
            + ALLOWED_TARGET_DOMAINS[i].replace('.', '\\.')
            + '$|.*\\.'
            + ALLOWED_TARGET_DOMAINS[i].replace('.', '\\.')
            + '$', 'i');

        if (regex.test(domain)) {
            return true;
        }
    }
    // console.log(domain + ' is NOT whitelisted');
    return false;
}

//
// Add whitelisted GET paramters to the outgoing HTTP headers
function addGetParametersAsHeaders(query, outgoingHeaders) {
    for (var i = 0; i < PARAMETERS_TO_HEADERS.length; i++) {
        // All header names in outgoingHeaders are already converted to lowercase 
        if ((outgoingHeaders[PARAMETERS_TO_HEADERS[i].toLowerCase()] === undefined
            || outgoingHeaders[PARAMETERS_TO_HEADERS[i].toLowerCase()].length == 0)) {
            for (var key in query) {
                if (key.toLowerCase() == PARAMETERS_TO_HEADERS[i].toLowerCase()) {
                    outgoingHeaders[PARAMETERS_TO_HEADERS[i]] = query[key];
                }
            }
        }
    }
    return outgoingHeaders;
}

//
// Forward the ping to its requested destination (Opera transcoder server)
//
function forwardPing(request) {
    var outgoingHeaders = request.headers;
    var getParameters = url.parse(request.url, true)['query'];

    // Remove excluded headers
    for (var header in EXCLUDED_HEADERS) {
        delete outgoingHeaders[header];
    }

    // Add special Opera headers
    outgoingHeaders['opera-remote-address'] = request.connection.remoteAddress;
    outgoingHeaders['opera-querystring'] = url.parse(request.url, false)['query'];

    // Add whitelsited GET parameters as HTTP headers
    outgoingHeaders = addGetParametersAsHeaders(getParameters, outgoingHeaders);

    // If only one paramter, convert to array to be able to iterate over it
    if (typeof getParameters.t == "string")
        getParameters.t = [getParameters.t];

    var requests = [];
    for (var i = 0; i < Math.min(getParameters.t.length, MAX_TARGET_URLS); i++) {
        // If no protocol is specified we assume it's HTTP
        if (getParameters.t[i].substring(0, 4) != 'http')
            getParameters.t[i] = 'http://' + getParameters.t[i];

        var isHttps = (getParameters.t[i].substring(0, 5) === 'https');

        var options = url.parse(getParameters.t[i], true);

        // Check that the destination domain is whitelisted
        if (options.hostname == null || !isPingDestinationWhitelisted(options.hostname)) {
            var now = new Date();
            console.error('%s %s is not a whitelisted as a ping receiver.',
                now.toISOString(),
                options.hostname);
            continue;
        }

        // Send the request and handle the response
        options.headers = outgoingHeaders;
        options.headers['host'] = options.hostname;
        if (isHttps) {
            requests[i] = https.request(options, function (response) {
                var now = new Date();
                console.log('%s Successfully forwarded ping to %s%s',
                    now.toISOString(),
                    this.options.hostname,
                    this.options.path);
            }.bind({ options: options }));
        } else {
            requests[i] = http.request(options, function (response) {
                var now = new Date();
                console.log('%s Successfully forwarded ping to %s%s',
                    now.toISOString(),
                    this.options.hostname,
                    this.options.path);
            }.bind({ options: options }));
        }

        requests[i].on('error', function (e) {
            var now = new Date();
            console.error('%s Could not forward ping to %s%s (%s)',
                now.toISOString(),
                this.options.hostname,
                this.options.path, e);
        }.bind({ options: options }));

        requests[i].end();
    }
}

//
// Set which port to listen to from the arguments
//
function setPortFromArgs() {
    if (process.argv.length >= 3 && process.argv[2] % 1 == 0) {
        PORT = process.argv[2];
    } else {
        console.error('Missing or illegal port argument');
        printUsage();
        process.exit(1);
    }
}


//
// Program execution starts here
//

// Set the port which to listen on
setPortFromArgs();

var server = http.createServer(function (request, response) {
    var getParameters = url.parse(request.url, true)['query'];



    if (getParameters['debug'] !== undefined) {
        response.write(createDebugOutput(request));
    }
    var isGetParametersOk = validateGetParameters(getParameters);
    if (isGetParametersOk === true) {
        forwardPing(request);
    } else {
        var date = new Date();
        console.error('%s %s (%s%s)',
            date.toISOString(),
            isGetParametersOk,
            request.headers['host'],
            request.url);
    }
    response.end();
});

server.on('error', function (e) {
    console.error('Unable to start ping forwarding server on port %s (%s)', PORT, e.code);
    if (PORT < 1024 && e.code == 'EACCES') {
        console.error('Root privileges usually needed to open ports below 1024');
    }
    process.exit(1);
});

server.listen(PORT);
console.log('Ping forwarding server running at port %s', PORT);
