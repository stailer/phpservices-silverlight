<?php
class SimpleObject 
{   
    /**
     * A n'utiliser que s'il y a une différence d'encodage entre 2 serveurs contenant l'application
     * Il s'agit ici des paramètres envoyées par .NET à PHP
     * @param type $data
     * @return mixed 
     */
    public static function valParameter($data)
    {
         return (UTF8_ENCODING) ? utf8_decode($data) : $data;
    }
    
    /**
     * A n'utiliser que s'il y a une différence d'encodage entre 2 serveurs contenant l'application
     * Il s'agit ici des valeurs envoyées à .NET par PHP
     * @param type $data
     * @return mixed 
     */
    public static function valReturn($data)
    {
         return (UTF8_ENCODING) ? utf8_encode($data) : $data;
    }
    
    /**
     * Rempli la propriété $key de l'objet avec l'élement $key du tableau
     * @param string $key
     * @param array $array
     * @return mixed 
     */
    public function setValueObject($key, $array)
    {
         $val = self::valReturn($array[$key]);
         $this->{$key} = $val;
    }
}