using System;
using System.Net;


namespace PHPServices
{
    /// <summary>
    /// Paramètre envoyé à la classe PhpClient
    /// </summary>
    public class PhpClientParameter
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public PhpClientParameter(string Key, object Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

    }
}
