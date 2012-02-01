<?php
class Services_Example 
{

    public function HelloWorld()
    {
        return 'HelloWorld from PHP !';
    }
    
    
    public function GetName($firstname, $lastname)
    {
        return 'Hello from php, '.$firstname.' '.$lastname.' !';
    }
    
    public function GetNameObject($myCustomer)
    {
        return 'Hello from php, '.$myCustomer->firstname.' '.$myCustomer->lastname.' !';
    }
}