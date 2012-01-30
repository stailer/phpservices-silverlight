using System;
using System.Net;


namespace PHPServices
{
    /// <summary>
    /// Paramètre de retour renvoyé par les services php
    /// </summary>
    public class SrvClientReturnObject
    {
        public string MethodName { get; set; }
        public object CustomResult { get; set; } 
    }

}
