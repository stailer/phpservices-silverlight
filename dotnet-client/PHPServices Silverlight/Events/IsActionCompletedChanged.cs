using System;
using System.Net;


namespace PHPServices.Events
{
    public delegate void IsActionCompletedEventHandler(object sender, IsActionCompletedEventArgs e);

    public class IsActionCompletedEventArgs : EventArgs
    {
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Donne l'état IsComplted de l'action
        /// </summary>
        /// <param name="IsCompleted"></param>
        public IsActionCompletedEventArgs(bool IsCompleted)
        {
            this.IsCompleted = IsCompleted;
        }
    }
}
