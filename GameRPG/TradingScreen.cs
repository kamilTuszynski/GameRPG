using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace GameRPG
{
    public partial class TradingScreen : Form
    {
        private Player _currentPlayer;

        public TradingScreen(Player player)
        {
            _currentPlayer = player;
            InitializeComponent();

            DataGridViewCellStyle rightAllignedCellStyle = new DataGridViewCellStyle();
            rightAllignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nazwa",
                Width=120,
                DataPropertyName="Description"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ilość",
                Width = 30,
                DefaultCellStyle = rightAllignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Cena",
                Width = 35,
                DefaultCellStyle = rightAllignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Sprzedaj 1",
                UseColumnTextForButtonValue = true,
                Width=70,
                DataPropertyName="ItemID"
            });

            dgvMyItems.DataSource = _currentPlayer.Inventory;
            dgvMyItems.CellClick += dgvMyItems_CellClick;




            dgvVendorItems.RowHeadersVisible = false;
            dgvVendorItems.AutoGenerateColumns = false;

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nazwa",
                Width = 120,
                DataPropertyName = "Description"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Cena",
                Width = 35,
                DefaultCellStyle = rightAllignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Kup 1",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });

            dgvVendorItems.DataSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;
            dgvVendorItems.CellClick += dgvVendorItems_CellClick;
        }

        private void dgvMyItems_CellClick(object sender,DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==4)
            {
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;

                Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));

                if (itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
                    MessageBox.Show("Nie możesz sprzedać "+ itemBeingSold.Name);
                else
                {
                    _currentPlayer.RemoveItemFromInventory(itemBeingSold);
                    _currentPlayer.Gold += itemBeingSold.Price;
                }
            }
        }

        private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==3)
            {
                var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;

                Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));

                if (_currentPlayer.Gold >= itemBeingBought.Price)
                {
                    _currentPlayer.AddItemToInventory(itemBeingBought);
                    _currentPlayer.Gold -= itemBeingBought.Price;
                }
                else
                    MessageBox.Show("Nie posiadasz wystarczająco złota do zakupu " + itemBeingBought.Name);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
