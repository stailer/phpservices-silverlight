<?php
// paramétrage des classes en autochargement
function __autoload($class_name) {
    $class_name = str_replace('_', '/', $class_name);
    require_once $class_name . '.php';
}