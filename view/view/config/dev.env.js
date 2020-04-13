'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  BASE_URL:'"http://localhost:8082/__system/admin/"',
  INFO_URL:'"https://api.qbcode.cn:8081/api/sj/admin/get"',
  STATIC_URL:'"https://static.qbcode.cn"',
  LOGO_URL:'"https://static.qbcode.cn/public/images/logo.svg"',
})
