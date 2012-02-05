<?php
class SimpleObject 
{   
    /**
     * A n'utiliser que s'il y a une diff�rence d'encodage entre 2 serveurs contenant l'application
     * Il s'agit ici des param�tres envoy�es par .NET � PHP
     * @param type $data
     * @return mixed 
     */
    public static function valParameter($data)
    {
         return (UTF8_ENCODING) ? utf8_decode($data) : $data;
    }
    
    /**
     * A n'utiliser que s'il y a une diff�rence d'encodage entre 2 serveurs contenant l'application
     * Il s'agit ici des valeurs envoy�es � .NET par PHP
     * @param type $data
     * @return mixed 
     */
    public static function valReturn($data)
    {
         return (UTF8_ENCODING) ? utf8_encode($data) : $data;
    }
    
    /**
     * Rempli la propri�t� $key de l'objet avec l'�lement $key du tableau
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