const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');
require('dotenv').config();

const app = express();

app.use(process.env.PATH_URL_PROXY_APP, createProxyMiddleware({ target: process.env.URL_APP, changeOrigin: true}));
app.listen(process.env.PATH_PORT_GATEWAY_PROXY);