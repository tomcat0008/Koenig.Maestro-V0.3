﻿import React from "react"
import ReactDOM from "react-dom"
import Bootstrap from 'bootstrap/dist/js/bootstrap.bundle'
import MainMenu from './components/mainMenu/MainMenu'

import Dashboard from './components/admin/Dashboard'
import 'jquery/dist/jquery.min'
import 'axios/dist/axios'
var appName = "Maestro";
//ReactDOM.render(<MainMenu  />, document.getElementById('mainMenu'));

ReactDOM.render(<Dashboard />, document.getElementById('content'));
