using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace GameRPG
{
    public partial class GameRPG : Form
    {
        private Player _player;        
        private const string PLAYER_DATA_FILE_NAME = "D://Programowanie//c#//RPG//GameRPG//PlayerData.xml";

        public GameRPG()
        {
            InitializeComponent();
            
            if(File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }

            //bindowanie statystyk
            lblHitPoints.DataBindings.Add(new Binding("Text",_player,"CurrentHitPoints"));
            lblMaxHitPoints.DataBindings.Add(new Binding("Text", _player, "MaxHitPoints"));
            lblExperience.DataBindings.Add(new Binding("Text", _player, "Experience"));
            lblGold.DataBindings.Add(new Binding("Text", _player, "Gold"));
            lblLevel.DataBindings.Add(new Binding("Text", _player, "Level"));



            //bindowanie ekwipunku
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.AutoGenerateColumns = false;

            dgvInventory.DataSource = _player.Inventory;

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nazwa",
                Width = 197,
                DataPropertyName = "Description"                
            });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText="Ilość",
                DataPropertyName="Quantity"
            });




            //bindowanie questow
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.AutoGenerateColumns = false;

            dgvQuests.DataSource = _player.Quests;

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Nazwa",
                Width = 197,
                DataPropertyName = "Name"
            });
            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Ukończony?",
                DataPropertyName = "IsCompleted"
            });



            //bindowanie combolist
            cboWeapons.DataSource = _player.Weapons;
            cboWeapons.DisplayMember = "Name";
            cboWeapons.ValueMember = "ID";

            if (_player.CurrentWeapon != null)
                cboWeapons.SelectedItem = _player.CurrentWeapon;

            cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

            cboPotions.DataSource = _player.HealingPotions;
            cboPotions.DisplayMember = "Name";
            cboPotions.ValueMember = "ID";

            _player.PropertyChanged += PlayerOnPropertyChanged;
            _player.OnMessage += DisplayMessage;
            _player.OnDeath += DisplayDeath;      
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            _player.MoveNorth();
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            _player.MoveWest();
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            _player.MoveEast();
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            _player.MoveSouth();
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;
            _player.UseWeapon(currentWeapon);        
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;
            _player.UsePotion(potion);            
        }

        private void btnTrade_Click(object sender, EventArgs e)
        {
            TradingScreen tradingScreen = new TradingScreen(_player);
            tradingScreen.StartPosition = FormStartPosition.CenterParent;
            tradingScreen.ShowDialog(this);
        }

        private void rtbMessages_TextChanged(object sender, EventArgs e)
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void GameRPG_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }

        private void PlayerOnPropertyChanged(object sender,PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="Weapons")
            {
                cboWeapons.DataSource = _player.Weapons;

                if(!_player.Weapons.Any())
                {
                    cboWeapons.Visible = false;
                    btnUseWeapon.Visible = false;
                }
            }
            if(e.PropertyName=="HealingPotions")
            {
                cboPotions.DataSource = _player.HealingPotions;

                if(!_player.HealingPotions.Any())
                {
                    cboPotions.Visible = false;
                    btnUsePotion.Visible = false;
                }
            }

            if (e.PropertyName == "CurrentLocation")
            {
                btnNorth.Visible = (_player.CurrentLocation.LocationToNorth != null);
                btnSouth.Visible = (_player.CurrentLocation.LocationToSouth != null);
                btnEast.Visible = (_player.CurrentLocation.LocationToEast != null);
                btnWest.Visible = (_player.CurrentLocation.LocationToWest != null);

                btnTrade.Visible = (_player.CurrentLocation.VendorWorkingHere != null);

                rtbLocation.Text = _player.CurrentLocation.Name + Environment.NewLine;
                rtbLocation.Text += _player.CurrentLocation.Description;

                if(_player.CurrentLocation.MonsterLivingHere==null)
                {
                    cboWeapons.Visible = false;
                    cboPotions.Visible = false;
                    btnUseWeapon.Visible = false;
                    btnUsePotion.Visible = false;
                }
                else
                {
                    cboWeapons.Visible = _player.Weapons.Any();
                    cboPotions.Visible = _player.HealingPotions.Any();
                    btnUseWeapon.Visible = _player.Weapons.Any(); ;
                    btnUsePotion.Visible = _player.HealingPotions.Any();
                }
            }
            
        }

        private void DisplayMessage(object sender,MessageEventArgs e)
        {
            rtbMessages.Text += e.Message + Environment.NewLine;

            if (e.AddNewLine)
                rtbMessages.Text += Environment.NewLine;

            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }

        private void DisplayDeath(object sender,DeathMessageEventArgs e)
        {
            MessageBox.Show(e.Message);
            Application.Exit();
        }


    }
}
