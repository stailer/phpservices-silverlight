<?php
class Services_Example 
{
    /**
     * Return HelloWorld
     */
    public function HelloWorld()
    {
        $obj = new SimpleObject();
         
        $obj->Mytext = $obj->val('Hello World !');

        return $obj;
    }
    
}