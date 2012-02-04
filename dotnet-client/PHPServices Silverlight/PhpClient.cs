#region usings
using System;
using System.Net;

using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;

using Newtonsoft.Json;

using PHPServices.Events;
using PHPServices.Classes;
#endregion

namespace PHPServices
{
    /// <summary>
    /// Classe permettant de piloter des services php plus simplement avec le WebClient
    /// Propose un système de suivi des requêtes lancées
    /// Propose un système de batch afin de contrôler les retours de requête
    /// </summary>
    public class PhpClient : IDisposable, INotifyPropertyChanged
    {
        private string UrlServices { get; set; }
        private string ContentType { get; set; }
        private object Caller { get; set; }

        public Batch BatchFlag { get; set; }


        #region gestion du chargement (loading) voir : ExecuteAsync
        public event IsBusyEventHandler IsBusyChanged;
        
        
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        


        private int _BusyQueries = 0;
        public int BusyQueries {
            get {
                return _BusyQueries;
            }
            set {

                if (value <= 0)
                {
                    value = 0;
                    this.IsBusy = false;
                    _BusyQueries = 0;
                }
                else
                {
                    this.IsBusy = true;
                    _BusyQueries = value;
                }

                if (IsBusyChanged != null)
                     OnIsBusyChanged(new IsBusyEventArgs(this.IsBusy, value));

                OnPropertyChanged("BusyQueries");
            }
        }



        protected virtual void OnIsBusyChanged(IsBusyEventArgs e)
        {
            IsBusyChanged(this, e);
        }
        #endregion


        #region constructeur avec différentes types de prise en charge + Disposing
        public PhpClient(string UrlServices, object Caller)
        {
            this.UrlServices = UrlServices;
            this.Caller = Caller; 
        }

        public PhpClient(string UrlServices, string ContentType, object Caller)
        {
            this.UrlServices = UrlServices;
            this.ContentType = ContentType; 
            this.Caller= Caller;
        }


        public void Dispose()
        {
            this.Caller = null;
        }
        #endregion




        #region ExecuteController - Exécute automatiquement dans la classe "caller" concernée la méthode : NomClasseEtMethodePHP_Completed
        private void ExecuteController(UploadStringCompletedEventArgs e, object launcher)
        {
            try
            {
                SrvClientReturnObject obj = JsonConvert.DeserializeObject<SrvClientReturnObject>(e.Result);

                if (obj.MethodName != string.Empty)
                {
                    Type myTypeObj = launcher.GetType();
                    MethodInfo myMethodInfo = myTypeObj.GetMethod(obj.MethodName);

                    if (myMethodInfo.GetParameters().Count() == 1)
                    {
                        ParameterInfo info = myMethodInfo.GetParameters()[0];

                        // va chercher la méthode DeserializeObject<> dans la classe JsonConvert
                        // ou alors on peut utiliser aussi le numéro de méthode dans la liste GetMethods()
                        MethodInfo deserializeDefinition = typeof(JsonConvert).GetMethods().First(m => m.IsGenericMethodDefinition && m.Name == "DeserializeObject");

                        // fabnrication d'une nouvelle méthode de type DeserializeObject<info.ParameterType
                        MethodInfo deserializeMethod = deserializeDefinition.MakeGenericMethod(info.ParameterType);

                        // lance les résultats
                        var ResultatObject = deserializeMethod.Invoke(launcher, new object[] { obj.CustomResult });

                        // exécute la méthode sur la classe appelante avec en paramètre les résultats
                        myMethodInfo.Invoke(launcher, new object[] { ResultatObject });

                        myMethodInfo = null;
                        info = null;
                        deserializeDefinition = null;
                    }
                    else
                        myMethodInfo.Invoke(launcher, new object[] { });

                    myTypeObj = null;
                    myMethodInfo = null;
                }
            }
            catch (Exception err)
            {
                throw new Exception("SrvClient : ExecuteController - " + err.Message);
            }
        }
        #endregion




        #region ExecuteAsync / ExecuteAsyncBatch - gère l'envoie de données de façon générique aux services PHP
        /// <summary>
        /// Cette méthode est utilisée pour automatiser un système de service avec php de type : classe->méthode( paramètres )
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="datas"></param>
        /// <param name="userToken"></param>
        public void ExecuteAsync(string serviceName, string methodName, List<PhpClientParameter> datas, object userToken)
        {
            // une requête de plus vient d'être exécutée
            this.BusyQueries++;

            // construction requête asynchrone
            WebClient srv = new WebClient();
            srv.UploadStringCompleted += (sender, e) =>
            {
                this.ExecuteController(e, this.Caller);
                srv = null;
                this.BusyQueries--;
            };

            this.ExecuteOnPHPServer(srv, serviceName, methodName, datas, userToken);
        }





        /// <summary>
        /// Version batchée de ExecuteAsync. Permet une exécution en chaine dans n'importe quel ordre d'appel en fonction de la précédente BatchAction Completed
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="datas"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public Guid ExecuteAsyncBatch(string serviceName, string methodName, List<PhpClientParameter> datas, object userToken)
        {
            Guid ky = BatchFlag.Add(BatchExecutionModes.Manual, (b) =>
            {
                // une requête de plus vient d'être exécutée
                this.BusyQueries++;

                // construction requête asynchrone
                WebClient srv = new WebClient();
                srv.UploadStringCompleted += (sender, e) =>
                {
                    this.ExecuteController(e, this.Caller);

                    srv = null;
                    this.BusyQueries--;

                    b.IsCompleted = true; 
                };

                this.ExecuteOnPHPServer(srv, serviceName, methodName, datas, b);
            });

            return ky;
        }
        #endregion



        #region surcharges ExecuteAsync / ExecuteAsyncBatch
        public void ExecuteAsync(string serviceName, string methodName)
        {
            this.ExecuteAsync(serviceName, methodName, new List<PhpClientParameter>(), null);
        }

        public void ExecuteAsync(string serviceName, string methodName, List<PhpClientParameter> datas)
        {
            this.ExecuteAsync(serviceName, methodName, datas, null);
        }


        public void ExecuteAsync(string serviceName, string methodName, object userToken)
        {
            this.ExecuteAsync(serviceName, methodName, new List<PhpClientParameter>(), userToken);
        }







        
        public Guid ExecuteAsyncBatch(string serviceName, string methodName)
        {
            return this.ExecuteAsyncBatch(serviceName, methodName, new List<PhpClientParameter>(), null);
        }

        public Guid ExecuteAsyncBatch(string serviceName, string methodName, List<PhpClientParameter> datas)
        {
            return this.ExecuteAsyncBatch(serviceName, methodName, datas, null);
        }


        public Guid ExecuteAsyncBatch(string serviceName, string methodName, object userToken)
        {
            return this.ExecuteAsyncBatch(serviceName, methodName, new List<PhpClientParameter>(), userToken);
        }
        #endregion



        #region ExecuteOnPHPServer
        /// <summary>
        /// Lancement de la requête Webclient.UploadStringAsync par le batch ou en direct
        /// </summary>
        /// <param name="srv"></param>
        /// <param name="serviceName"></param>
        /// <param name="methodName"></param>
        /// <param name="datas"></param>
        /// <param name="userToken"></param>
        private void ExecuteOnPHPServer(WebClient srv, string serviceName, string methodName, List<PhpClientParameter> datas, object userToken)
        {

            // construction et envoi de la requête
            datas.Add(new PhpClientParameter("serviceName", serviceName));
            datas.Add(new PhpClientParameter("methodName", methodName));

            string strDatas = "";
            foreach (PhpClientParameter data in datas)
            {
                if (strDatas != string.Empty)
                    strDatas += "&";

                if (data.Key == "serviceName" || data.Key == "methodName")
                    strDatas += data.Key + "=" + data.Value;
                else
                    strDatas += data.Key + "=" + JsonConvert.SerializeObject(data.Value);
            }

            srv.Headers["Content-Type"] = this.ContentType;


            // lancement!
            srv.UploadStringAsync(new Uri(this.UrlServices + "Controller.php"), "POST", strDatas, userToken);
        }
        #endregion


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

    } // fin classe


} // fin namespace