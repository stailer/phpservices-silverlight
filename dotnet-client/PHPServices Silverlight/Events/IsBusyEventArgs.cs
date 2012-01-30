using System;
using System.Net;


namespace PHPServices.Events
{
        public delegate void IsBusyEventHandler(object sender, IsBusyEventArgs e);
        /// <summary>
        /// Sur changement de loading, en cours ou chargement terminé
        /// </summary>
        public class IsBusyEventArgs : EventArgs
        {
            public bool IsBusy { get; set; }
            public int TotalCurrentQueries { get; set; }

            /// <summary>
            /// mValue est vrai faux selon si on est en chargement ou pas.
            /// mQueries est le nombre de requêtes en cours
            /// </summary>
            /// <param name="mValue"></param>
            /// <param name="mQueries"></param>
            public IsBusyEventArgs(bool mValue, int mQueries)
            {
                this.IsBusy = mValue;
                this.TotalCurrentQueries = mQueries;
            }
        }
}
