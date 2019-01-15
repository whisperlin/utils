var express = require("express");
var app = express();

app.use(express.static("LayaDemo")).listen(9999);