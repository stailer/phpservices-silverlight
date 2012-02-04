#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

using PHPServices;
using PHPServices.Classes;
#endregion


namespace PhpServicesExample
{

    public partial class MainPage : UserControl, INotifyPropertyChanged
    {
        #region properties
        private PhpClient php;
        
        private string _ReturnText;
        public string ReturnText
        {
            get { return _ReturnText; }
            set
            {
                _ReturnText = value;
                OnPropertyChanged("ReturnText");
            }
        }
        #endregion


        public MainPage()
        {
            this.DataContext = this;
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                // adress PHP Services
                // Headers data
                // Class with Services_Methodname1_Completed
                this.php = new PhpClient("http://localhost/php-services/", "application/x-www-form-urlencoded", this);

                //this.Sample1();
                //this.Sample2();
                //this.Sample3();

                this.Sample4();
            };
        }




        public void Services_Example_HelloWorld_Completed(string myReturn)
        {
            this.ReturnText = myReturn;
        }

        public void Sample1()
        {
            // return in Services_Example_HelloWorld_Completed
            this.php.ExecuteAsync("Services_Example", "HelloWorld");
        }





        public void Services_Example_GetName_Completed(string myReturn)
        {
            this.ReturnText = myReturn;
        }

        public void Sample2()
        {
            List<PhpClientParameter> parameters = new List<PhpClientParameter>();
            parameters.Add(new PhpClientParameter("firstname", "Jeff"));
            parameters.Add(new PhpClientParameter("lastname", "DOE"));

            this.php.ExecuteAsync("Services_Example", "GetName", parameters);
        }




        public void Services_Example_GetNameObject_Completed(string myReturn)
        {
            this.ReturnText = myReturn;
        }

        public void Sample3()
        {
            List<PhpClientParameter> parameters = new List<PhpClientParameter>();

            dynamic MyCustomer = new { firstname = "Jeff", lastname = "DOE" };
            parameters.Add( new PhpClientParameter("client", MyCustomer));

            this.php.ExecuteAsync("Services_Example", "GetNameObject", parameters);
        }







        public void Services_Example_MyTask1_Completed(string myReturn)
        {
            this.ReturnText = myReturn;
        }


        public void Services_Example_MyTask2_Completed(string myReturn)
        {
            this.ReturnText += myReturn;
        }


        public void Services_Example_MyTask3_Completed(string myReturn)
        {
            this.ReturnText += myReturn;
        }


        private void Sample4()
        {
            this.php.BatchFlag = new Batch();
            this.php.BatchFlag.Completed += (sender, e) =>
            {
                this.ReturnText +=  ", All is completed : )"; // Called when all task are ok
            };


            Guid tsk1 = this.php.ExecuteAsyncBatch("Services_Example", "MyTask1");
            Guid tsk2 = this.php.ExecuteAsyncBatch("Services_Example", "MyTask2"); 
            Guid tsk3 = this.php.ExecuteAsyncBatch("Services_Example", "MyTask3"); 


            BatchAction act1 = this.php.BatchFlag.Find(tsk1);  
            act1.IsActionCompletedChanged += (sender, e) =>
            {
               this.php.BatchFlag.Run(tsk2); // Called when my task1 is completed
            };


            BatchAction act2 = this.php.BatchFlag.Find(tsk2);
            act2.IsActionCompletedChanged += (sender, e) =>
            {
               this.php.BatchFlag.Run(tsk3);   // Called when my task2 is completed
            };

            this.php.BatchFlag.Run(tsk1);
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
