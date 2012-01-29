<?php
header('Content-type: application/json; charset=utf-8');

error_reporting(E_ALL | E_STRICT | E_NOTICE);

require_once 'Config.php';
require_once 'Autoloader.php';

// récupération de la classe de service appelée et de la méthode
$serviceName = trim($_POST['serviceName']);
$methodName = trim($_POST['methodName']);


// instanciation
$class = new $serviceName();


// récupération des paramètres
$parameters = array();
foreach ($_POST as $key => $value)
{
    if ($key !== 'serviceName' && $key !== 'methodName')
    {
         $parameters[] =  (get_magic_quotes_gpc() === 1) ? json_decode(stripslashes($value)) : json_decode($value) ;
    }
}


// objet spécifique de retour côté client
$ret = new stdClass();
// nom de la méthode appelée côté client
$ret->MethodName = get_class($class).'_'.$methodName.'_Completed';
// appel du service correspondant à l'appel du client et encodage JSON
$ret->CustomResult = json_encode(call_user_func_array(array($class, $methodName), $parameters));


//sortie
echo json_encode($ret);