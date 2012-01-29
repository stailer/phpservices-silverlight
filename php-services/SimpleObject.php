<?php
class SimpleObject 
{   
    /**
     * A n'utiliser que s'il y a une différence d'encodage entre 2 serveurs contenant l'application
     * @param type $data
     * @return mixed 
     */
    public function val($data)
    {
         return (UTF8_ENCODING) ? utf8_decode($data) : $data;
    }
    
    /**
     * Rempli la propriété $key de l'objet avec l'élement $key du tableau
     * @param string $key
     * @param array $array
     * @return mixed 
     */
    public function setValueObject($key, $array)
    {
         $val = $this->val($array[$key]);
         $this->{$key} = $val;
    }
}