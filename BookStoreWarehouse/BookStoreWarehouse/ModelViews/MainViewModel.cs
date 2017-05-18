using BookStoreWarehouse.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookStoreWarehouse.ModelViews
{
    class MainViewModel : INotifyPropertyChanged
    {
        private static MainViewModel instance;
        public Control Controller { get; set; }



        public static MainViewModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new MainViewModel();
                return instance;
            }
        }

        private MainViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged == null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        internal void click()
        {
            throw new NotImplementedException();
        }

        public void startNewChat(string GUID, string address)
        {
            ClientInfo.Instance.initChat(new Guid(GUID), address);
        }

        public void addNewChat(Guid guid, string username)
        {

        }
    }
}
