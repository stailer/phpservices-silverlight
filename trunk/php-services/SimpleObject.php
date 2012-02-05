<?php
class SimpleObject 
{   
    /**
     * A n'utiliser que s'il y a une diff�rence d'encodage entre 2 serveurs contenant l'application
     * @param type $data
     * @return mixed 
     */
    public static function val($data)
    {
         return (UTF8_ENCODING) ? utf8_decode($data) : $data;
    }
    
    /**
     * Rempli la propri�t� $key de l'objet avec l'�lement $key du tableau
     * @param string $key
     * @param array $array
     * @return mixed 
     */
    public function setValueObject($key, $array)
    {
         $val = self::val($array[$key]);
         $this->{$key} = $val;
    }
}