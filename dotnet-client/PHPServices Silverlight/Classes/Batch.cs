using System;
using System.Net;

using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using PHPServices.Events;

public enum BatchExecutionModes
{
    Auto,
    Manual
}

namespace PHPServices.Classes
{
    /// <summary>
    /// Classe permettant de synchroniser des tâches Async
    /// Utilisation : http://blog.naviso.fr/wordpress/?p=264
    /// Auteurs : JF.Cambot / S.Blanchard
    /// </summary>
    public class Batch
    {
        /// <summary>
        /// Dictionnaire
        /// </summary>

        private Dictionary<Guid, BatchAction> batchs = new Dictionary<Guid,BatchAction>();
        
        /// <summary>
        /// Evenement
        /// </summary>
        public event System.EventHandler Completed;

        /// <summary>
        /// Constructeur
        /// </summary>
        public Batch()
        {
        }

        /// <summary>
        /// Lancement des Batchs avec des Threads
        /// </summary>
        /// <param name="useNewThread"></param>
        public Batch(bool useNewThread)
        {
            this.UseNewThread = useNewThread;
        }

        /// <summary>
        /// Utilsation de thread pour le lancement
        /// </summary>
        public bool UseNewThread
        {
            get;
            private set;
        }

        /// <summary>
        /// Ajouter une action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Guid Add(Action<BatchAction> action)
        {
            return this.Add(BatchExecutionModes.Auto, action);
        }

        /// <summary>
        /// Ajouter une action
        /// </summary>
        /// <param name="action"></param>
        public Guid Add(BatchExecutionModes executionMode, Action<BatchAction> action )
        {
            Guid key = Guid.NewGuid();

            batchs.Add(key, new BatchAction(this, action, executionMode));

            return key;
        }

        /// <summary>
        /// Execution d'une action en mode thread ou non
        /// </summary>
        /// <param name="action"></param>
        private void Run(BatchAction action)
        {
            if (this.UseNewThread == false)
            {
                action.Run();
            }
            else
            {

                action.RunThreaded();
            }
        }

        /// <summary>
        /// Fait fonctionner les Batchs marqués Auto
        /// </summary>
        public void Run()
        {
            foreach (BatchAction action in this.batchs.Values)
            {
                if (action.ExecutionMode == BatchExecutionModes.Auto)
                {
                    this.Run(action);
                }
            }
        }


        /// <summary>
        /// Lance un batch marqué Manual à l'aide de sa clé
        /// </summary>
        /// <param name="key"></param>
        public bool Run(Guid key)
        {
            BatchAction action;

            if (this.batchs.TryGetValue(key, out action) == true)
            {
                if (action.ExecutionMode == BatchExecutionModes.Manual)
                {
                    this.Run(action);
                }
                else
                {
                    throw new Exception("cette action ne peut être executée car elle est marquée Auto en mode d'exécution et non Manual !");
                }

                return true;
            }

            return false;
        }




        /// <summary>
        /// Retrouver
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public BatchAction Find(Guid key)
        {
            BatchAction action;

            if (this.batchs.TryGetValue(key, out action) == true)
            {
                return action;
            }

            return null;
        }

        /// <summary>
        /// vérifier que tout est complet
        /// </summary>
        public bool CheckCompleted()
        {
            foreach( BatchAction action in this.batchs.Values )
            {
                if( action.IsCompleted == false )
                {
                    return false;
                }
            }

            if (this.Completed != null)
            {
                this.Completed(this, null);
            }

            return true;
        }

        /// <summary>
        /// Nettoyage
        /// </summary>
        public void Clears()
        {
            this.batchs.Clear();
        }

        /// <summary>
        /// Nettoyage de IsCompleted
        /// </summary>
        public void ClearIsCompleted()
        {
            foreach( BatchAction action in this.batchs.Values )
            {
                action.IsCompleted = false;
            }
        }
    }

    /// <summary>
    /// Message de Batch
    /// </summary>

    public class BatchAction : INotifyPropertyChanged
    {

        public event IsActionCompletedEventHandler IsActionCompletedChanged;
        protected virtual void OnIsActionCompletedChanged(IsActionCompletedEventArgs e)
        {
            IsActionCompletedChanged(this, e);
        }


        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="key"></param>
        /// <param name="tag"></param>
        public BatchAction(Batch batch, Action<BatchAction> action, BatchExecutionModes executionMode)
        {
            this.Batch = batch;
            this.action = action;
            this.ExecutionMode = executionMode;
        }

        /// <summary>
        /// Mode d'execution
        /// </summary>
        public BatchExecutionModes ExecutionMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Tag
        /// </summary>
        public object UserState
        {
            get;
            set;
        }

        private static object locker = new Object();

        /// <summary>
        /// Est-ce complet
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return this.isCompleted;
            }

            set
            {
                lock (locker)
                {
                    if (this.isCompleted != value)
                    {
                        this.isCompleted = value;

                        if (value == true)
                        {
                            this.Batch.CheckCompleted();
                            OnPropertyChanged("IsCompleted");
                            OnIsActionCompletedChanged(new IsActionCompletedEventArgs(value));
                        }
                    }
                }


            }
        }


        private bool isCompleted = false;

        /// <summary>
        /// Batch
        /// </summary>
        public Batch Batch
        {
            get;
            private set;
        }

        /// <summary>
        /// action
        /// </summary>

        private Action<BatchAction> action;

        /// <summary>
        /// Run
        /// </summary>
        public void Run()
        {
            this.IsCompleted = false;
            action.Invoke(this);
        }

        /// <summary>
        /// Demarrage threadé
        /// </summary>
        public void RunThreaded()
        {
            Thread thread = new Thread(new ThreadStart(this.Run));
            thread.Start();
        }



        
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
