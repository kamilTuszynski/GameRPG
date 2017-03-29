using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Engine
{
    public class Vendor
    {
        public string Name { get; set; }
        public BindingList<InventoryItem> Inventory { get; private set; }

        public Vendor(string Name)
        {
            this.Name = Name;
            Inventory = new BindingList<InventoryItem>();
        }

        public void AddItemToInventory(Item itemToAdd,int quantity=1)
        {
            InventoryItem ii = Inventory.SingleOrDefault(i => i.Details.ID == itemToAdd.ID);

            if (ii == null)
            {
                Inventory.Add(new InventoryItem(itemToAdd, 1));
            }
            else
            {
                ii.Quantity += quantity;
            }
            OnPropertyChanged("Inventory");
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(i => i.Details.ID == itemToRemove.ID);

            if (item == null)
            {
                //moze jakis blad, bo takiego itemka nie ma w ekwipunku
            }
            else
            {
                item.Quantity -= quantity;
                if (item.Quantity < 0)
                    item.Quantity = 0;
                if (item.Quantity == 0)
                    Inventory.Remove(item);
                OnPropertyChanged("Inventory");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
