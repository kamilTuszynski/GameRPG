﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Engine
{
    public class InventoryItem : INotifyPropertyChanged
    {
        private Item _details;
        private int _quantity;

        public Item Details
        {
            get { return _details; }
            set
            {
                _details = value;
                OnPropertyChanged("Details");
                OnPropertyChanged("Description");
            }
        }
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }
        public string Description
        {
            get { return Details.Name; }
        }
        public int ItemID
        {
            get { return Details.ID; }
        }
        public int Price
        {
            get { return Details.Price; }
        }

        public InventoryItem(Item Details,int Quantity)
        {
            this.Details = Details;
            this.Quantity = Quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
