var express = require('express');
var remotes = require('strong-remoting').create();
var SharedClass = require('strong-remoting').SharedClass;

remotes.exports = {
  simple: require('./simple'),
  contract: require('./contract')
};

remotes.addClass(new SharedClass('SimpleClass', require('./simple-class')));
remotes.addClass(new SharedClass('ContractClass', require('./contract-class')));

var app = express();
app.use(require('morgan')('strong-remoting> :method :url :status'));
app.use(remotes.handler('rest'));

var server = require('http')
  .createServer(app)
  .listen(3001, function() {
    console.log(
      'Remoting Test Server listening on http://localhost:3001/');
  });

