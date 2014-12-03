﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Personal_Editor
{
    public partial class Form1 : Form
    {
        private string[] paths = {}; //Files in the Personal GARC folder.
        private string mode = ""; //Should be "XY" or "ORAS" depending on game being edited

        private string[] natures = { };
        private string[] items = { };
        private string[] moves = { };
        private string[] species = { };
        private string[] abilities = { };

        private byte[] data = { };

        private ComboBox[] helditem_boxes;
        private ComboBox[] ability_boxes;
        private ComboBox[] typing_boxes;
        private ComboBox[] eggGroup_boxes;

        private MaskedTextBox[] byte_boxes;
        private MaskedTextBox[] ev_boxes;

        public string[] types = {
                "Normal",
                "Fighting",
                "Flying", 
                "Poison", 
                "Ground", 
                "Rock", 
                "Bug", 
                "Ghost",
                "Steel",
                "Fire", 
                "Water",
                "Grass",
                "Electric",
                "Psychic",
                "Ice",
                "Dragon",
                "Dark",
                "Fairy"};

        public string[] eggGroups = {
                "---",
                "Monster",
                "Water 1",
                "Bug",
                "Flying",
                "Field",
                "Fairy",
                "Grass",
                "Human-Like",
                "Water 3",
                "Mineral",
                "Amorphous",
                "Water 2",
                "Ditto",
                "Dragon",
                "Undiscovered" };

        public string[] EXPGroups = {
                "Medium-Fast",
                "Erratic",
                "Fluctuating",
                "Medium-Slow",
                "Fast",
                "Slow" };
        public string[] colors = {
                "Red",
                "Blue",
                "Yellow",
                "Green",
                "Black",
                "Brown",
                "Purple",
                "Gray",
                "White",
                "Pink"};

        public string[] tutormoves = { "Frenzy Plant", "Blast Burn", "Hydro Cannon",
                                         "Grass Pledge", "Fire Pledge", "Water Pledge",
                                         "Draco Meteor", "Dragon's Ascent"};

        public string[] tutor1 = { "Bug Bite", "Covet", "Super Fang", "Dual Chop", "Signal Beam", "Iron Head", "Seed Bomb", "Drill Run", "Bounce", "Low Kick", "Gunk Shot", "Uproar", "Thunder Punch", "Fire Punch", "Ice Punch" };
        public string[] tutor2 = {"Magic Coat",
"Block",
"Earth Power",
"Foul Play",
"Gravity",
"Magnet Rise",
"Iron Defense",
"Last Resort",
"Superpower",
"Electroweb",
"Icy Wind",
"Aqua Tail",
"Dark Pulse",
"Zen Headbutt",
"Dragon Pulse",
"Hyper Voice",
"Iron Tail"};
        public string[] tutor3 = {"Bind",
"Snore",
"Knock Off",
"Synthesis",
"Heat Wave",
"Role Play",
"Heal Bell",
"Tailwind",
"Sky Attack",
"Pain Split",
"Giga Drain",
"Drain Punch",
"Air Cutter",
"Focus Punch",
"Shock Wave",
"Water Pulse"};
        public string[] tutor4 = {"Gastro Acid",
"Worry Seed",
"Spite",
"After You",
"Helping Hand",
"Trick",
"Magic Room",
"Wonder Room",
"Endeavor",
"Outrage",
"Recycle",
"Snatch",
"Stealth Rock",
"Sleep Talk",
"Skill Swap"};

        public Form1()
        {
            InitializeComponent();
            helditem_boxes = new ComboBox[]
            {
                CB_HeldItem1,
                CB_HeldItem2,
                CB_HeldItem3
            };
            ability_boxes = new ComboBox[]
            {
                CB_Ability1,
                CB_Ability2,
                CB_Ability3
            };
            typing_boxes = new ComboBox[]
            {
                CB_Type1,
                CB_Type2
            };
            eggGroup_boxes = new ComboBox[]
            {
                CB_EggGroup1,
                CB_EggGroup2
            };
            byte_boxes = new MaskedTextBox[]
            {
                TB_BaseHP,
                TB_BaseATK,
                TB_BaseDEF,
                TB_BaseSPE,
                TB_BaseSPD,
                TB_BaseSPA,
                TB_Gender,
                TB_HatchCycles,
                TB_Friendship,
                TB_CatchRate
            };
            ev_boxes = new MaskedTextBox[]
            {
                TB_HPEVs,
                TB_ATKEVs,
                TB_DEFEVs,
                TB_SPEEVs,
                TB_SPAEVs,
                TB_SPDEVs
            };
        }

        public static string[] getStringList(string f)
        {
            object txt = Properties.Resources.ResourceManager.GetObject("text_" + f); // Fetch File, \n to list.
            List<string> rawlist = ((string)txt).Split(new char[] { '\n' }).ToList();

            string[] stringdata = new string[rawlist.Count];
            for (int i = 0; i < rawlist.Count; i++)
                stringdata[i] = rawlist[i];

            return stringdata;
        }

        private void B_Open_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            openPersonal(folderBrowserDialog.SelectedPath);
        }

        private void openPersonal(string folder)
        {
            this.paths = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
            #region Data Verification + Mode setting
            if (paths.Length == 800) //XY
            {
                for (int i = 0; i < 799; i++)
                {
                    FileInfo CurEntry = new FileInfo(paths[i]);
                    if (CurEntry.Length != 0x40) //First 799 files all have length 0x40
                    {
                        return;
                    }
                }
                FileInfo personal = new FileInfo(paths[799]);
                if (personal.Length != 0x40 * 799) //0xC7C0
                {
                    return;
                }
                this.mode = "XY";
                data = File.ReadAllBytes(paths[799]);
            }
            else if (paths.Length == 827) // ORAS
            {
                for (int i = 0; i < 826; i++)
                {
                    FileInfo CurEntry = new FileInfo(paths[i]);
                    if (CurEntry.Length != 0x50) //First 826 files all have length 0x50
                    {
                        return;
                    }
                }
                FileInfo personal = new FileInfo(paths[826]);
                if (personal.Length != 0x50 * 826) //0x10220
                {
                    return;
                }
                this.mode = "ORAS";
                data = File.ReadAllBytes(paths[826]);
            }
            else
            {
                return; //Don't load invalid folders
            }
            #endregion
            B_Open.Enabled = false;
            L_Mode.Text = "Mode: " + mode;
            loadStrings(); //Turn string resources into arrays
            CB_Species.Enabled = true;
            B_Save.Enabled = true;
            CB_Species.SelectedIndex = 0;
            TC_Pokemon.Visible = true;
        }

        private void loadStrings()
        {
            natures = getStringList("Natures");
            abilities = getStringList("Abilities");
            items = getStringList("Items");
            moves = getStringList("Moves");
            species = getStringList("Species");
            for (int i = 1; i <= 100; i++)
            {
                CLB_TMHM.Items.Add("TM" + i.ToString("00"));
            }
            for (int i = 1; i <= 7; i++)
            {
                CLB_TMHM.Items.Add("HM" + i.ToString("00"));
            }
            for (int i = 0; i < tutormoves.Length - 1; i++)
            {
                CLB_MoveTutors.Items.Add(tutormoves[i]);
            }
            if (mode == "XY")
            {
                string[] temp_abilities = new string[abilities.Length - 3]; //3 abilities added in ORAS
                Array.Copy(abilities, temp_abilities, temp_abilities.Length);
                abilities = temp_abilities;

                string[] temp_items = new string[719]; //719 items in XY
                Array.Copy(items, temp_items, temp_items.Length);
                items = temp_items;

                string[] temp_moves = new string[moves.Length - 4]; //4 new moves added in ORAS
                Array.Copy(moves, temp_moves, temp_moves.Length);
                moves = temp_moves;

                string[] temp_species = new string[799]; //799 species in XY
                Array.Copy(species, temp_species, temp_species.Length);
                species = temp_species;
            }
            else if (mode == "ORAS")
            {
                CLB_MoveTutors.Items.Add(tutormoves[tutormoves.Length - 1]); //Dragon's Ascent
                foreach (string tm in tutor1)
                {
                    CLB_OrasTutors.Items.Add(tm);
                }
                foreach (string tm in tutor2)
                {
                    CLB_OrasTutors.Items.Add(tm);
                }
                foreach (string tm in tutor3)
                {
                    CLB_OrasTutors.Items.Add(tm);
                }
                foreach (string tm in tutor4)
                {
                    CLB_OrasTutors.Items.Add(tm);
                }
                CLB_OrasTutors.Visible = true;
                CLB_OrasTutors.Enabled = true;
                Lbl_OrasTutors.Visible = true;
            }
            for (int i = 0; i < species.Length; i++)
            {
                CB_Species.Items.Add(i + " - " + species[i]);
            }
            foreach (ComboBox cb in helditem_boxes)
            {
                foreach (string it in items)
                {
                    cb.Items.Add(it);
                }
            }
            foreach (ComboBox cb in ability_boxes)
            {
                foreach (string ab in abilities)
                {
                    cb.Items.Add(ab);
                }
            }
            foreach (ComboBox cb in typing_boxes)
            {
                foreach (string ty in types)
                {
                    cb.Items.Add(ty);
                }
            }
            foreach (ComboBox cb in eggGroup_boxes)
            {
                foreach (string eg in eggGroups)
                {
                    cb.Items.Add(eg);
                }
            }
            foreach (string co in colors)
            {
                CB_Color.Items.Add(co);
            }
            foreach (string eg in EXPGroups)
            {
                CB_EXPGroup.Items.Add(eg);
            }
            
        }

        private void CB_Species_SelectedIndexChanged(object sender, EventArgs e)
        {
            int len = 0;
            if (mode == "ORAS")
            {
                len = 0x50;
            }
            else if (mode == "XY")
            {
                len = 0x40;
            }
            byte[] file = new byte[len];
            Array.Copy(data, len * CB_Species.SelectedIndex, file, 0, len);
            TB_BaseHP.Text = file[0].ToString("000");
            TB_BaseATK.Text = file[1].ToString("000");
            TB_BaseDEF.Text = file[2].ToString("000");
            TB_BaseSPE.Text = file[3].ToString("000");
            TB_BaseSPA.Text = file[4].ToString("000");
            TB_BaseSPD.Text = file[5].ToString("000");

            int EVs = BitConverter.ToInt16(file, 0xA);
            TB_HPEVs.Text = ((EVs >> 0) & 0x3).ToString("0");
            TB_ATKEVs.Text = ((EVs >> 2) & 0x3).ToString("0");
            TB_DEFEVs.Text = ((EVs >> 4) & 0x3).ToString("0");
            TB_SPEEVs.Text = ((EVs >> 6) & 0x3).ToString("0");
            TB_SPAEVs.Text = ((EVs >> 8) & 0x3).ToString("0");
            TB_SPDEVs.Text = ((EVs >> 10) & 0x3).ToString("0");

            CB_Type1.SelectedIndex = file[6];
            CB_Type2.SelectedIndex = file[7];

            TB_CatchRate.Text = file[8].ToString("000");
            TB_Stage.Text = file[9].ToString("0");

            CB_HeldItem1.SelectedIndex = BitConverter.ToUInt16(file, 0xC);
            CB_HeldItem2.SelectedIndex = BitConverter.ToUInt16(file, 0xE);
            CB_HeldItem3.SelectedIndex = BitConverter.ToUInt16(file, 0x10);

            TB_Gender.Text = file[0x12].ToString("000");
            TB_HatchCycles.Text = file[0x13].ToString("000");
            TB_Friendship.Text = file[0x14].ToString("000");

            CB_EXPGroup.SelectedIndex = file[0x15];

            CB_EggGroup1.SelectedIndex = file[0x16];
            CB_EggGroup2.SelectedIndex = file[0x17];

            CB_Ability1.SelectedIndex = file[0x18];
            CB_Ability2.SelectedIndex = file[0x19];
            CB_Ability3.SelectedIndex = file[0x1A];

            TB_FormeCount.Text = file[0x20].ToString("000");
            TB_FormeSprite.Text =  BitConverter.ToUInt16(file, 0x1E).ToString("000");

            int color = file[0x21] & 0xF;
            TB_RawColor.Text = file[0x21].ToString("000");
            CB_Color.SelectedIndex = color;

            TB_BaseExp.Text = BitConverter.ToUInt16(file, 0x22).ToString("000");

            TB_Height.Text = ((float)BitConverter.ToUInt16(file, 0x24) / 100).ToString("00.0");
            TB_Weight.Text = ((float)BitConverter.ToUInt16(file, 0x26) / 10).ToString("000.0");

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i*8 + j < CLB_TMHM.Items.Count)
                    {
                        CLB_TMHM.SetItemChecked(i * 8 + j, ((file[0x28 + i] >> j) & 0x1) == 1); //Bitflags for TMHM
                    }
                }
            }

            uint tutors = BitConverter.ToUInt32(file, 0x38);
            for (int t = 0; t < 8; t++)
            {
                if (t < CLB_MoveTutors.Items.Count)
                {
                    CLB_MoveTutors.SetItemChecked(t, ((tutors >> t) & 1) == 1);
                }
            }
            if (mode == "ORAS")
            {
                uint[] tutorm = new uint[4];
                for (int j = 0x40; j < 0x50; j += 4)
                {
                    tutorm[(j - 0x40) / 4] = BitConverter.ToUInt32(file, j);
                }
                int ofs = 0;
                for (int j = 0; j < tutorm.Length; j++)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (ofs + i < CLB_OrasTutors.Items.Count)
                        {
                            CLB_OrasTutors.SetItemChecked(ofs + i, ((tutorm[j] >> i) & 1) == 1);
                        }
                    }
                    //hardcode this, bad, I know
                    if (j == 0)
                    {
                        ofs += tutor1.Length;
                    }
                    else if (j == 1)
                    {
                        ofs += tutor2.Length;
                    }
                    else if (j == 2)
                    {
                        ofs += tutor3.Length;
                    }
                    else
                    {
                        ofs += tutor4.Length;
                    }
                }

            }

            Bitmap rawImg = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + CB_Species.SelectedIndex);
            Bitmap bigImg = new Bitmap(rawImg.Width * 2, rawImg.Height * 2);
            for (int x = 0; x < rawImg.Width; x++)
            {
                for (int y = 0; y < rawImg.Height; y++)
                {
                    Color c = rawImg.GetPixel(x, y);
                    bigImg.SetPixel(2 * x, 2 * y, c);
                    bigImg.SetPixel(2 * x + 1, 2 * y, c);
                    bigImg.SetPixel(2 * x, 2 * y + 1, c);
                    bigImg.SetPixel(2 * x + 1, 2 * y + 1, c);
                }
            }
            PB_MonSprite.Image = bigImg;
            PB_MonSprite2.Image = bigImg;
        }

        private void ByteLimiter(object sender, EventArgs e)
        {
            foreach (MaskedTextBox mtb in byte_boxes)
            {
                int val;
                Int32.TryParse(mtb.Text, out val);
                if (val > 255)
                {
                    mtb.Text = "255";
                }
            }
            foreach (MaskedTextBox mtb in ev_boxes)
            {
                int val;
                Int32.TryParse(mtb.Text, out val);
                if (val > 3)
                {
                    mtb.Text = "3";
                }
            }
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            int len = 0;
            if (mode == "XY")
            {
                len = 0x40;
            }
            else if (mode == "ORAS")
            {
                len = 0x50;
            }
            byte[] edits = new byte[len];
            Array.Copy(data, len * CB_Species.SelectedIndex, edits, 0, len); //edit raw data for easiness's sake
            edits[0] = Convert.ToByte(TB_BaseHP.Text);
            edits[1] = Convert.ToByte(TB_BaseATK.Text);
            edits[2] = Convert.ToByte(TB_BaseDEF.Text);
            edits[3] = Convert.ToByte(TB_BaseSPE.Text);
            edits[4] = Convert.ToByte(TB_BaseSPA.Text);
            edits[5] = Convert.ToByte(TB_BaseSPD.Text);

            ushort evs = 0;
            evs |= (ushort)((Convert.ToByte(TB_HPEVs.Text) & 3) << 0);
            evs |= (ushort)((Convert.ToByte(TB_ATKEVs.Text) & 3) << 2);
            evs |= (ushort)((Convert.ToByte(TB_DEFEVs.Text) & 3) << 4);
            evs |= (ushort)((Convert.ToByte(TB_SPEEVs.Text) & 3) << 6);
            evs |= (ushort)((Convert.ToByte(TB_SPAEVs.Text) & 3) << 8);
            evs |= (ushort)((Convert.ToByte(TB_SPDEVs.Text) & 3) << 10);
            Array.Copy(BitConverter.GetBytes(evs), 0, edits, 0xa, 2);

            edits[6] = (byte)CB_Type1.SelectedIndex;
            edits[7] = (byte)CB_Type2.SelectedIndex;

            edits[8] = Convert.ToByte(TB_CatchRate.Text);
            edits[9] = Convert.ToByte(TB_Stage.Text);

            Array.Copy(BitConverter.GetBytes((ushort)CB_HeldItem1.SelectedIndex),0, edits, 0xC,2);
            Array.Copy(BitConverter.GetBytes((ushort)CB_HeldItem2.SelectedIndex),0, edits, 0xE,2);
            Array.Copy(BitConverter.GetBytes((ushort)CB_HeldItem3.SelectedIndex),0, edits, 0x10,2);

            edits[0x12] = Convert.ToByte(TB_Gender.Text);
            edits[0x13] = Convert.ToByte(TB_HatchCycles.Text);
            edits[0x14] = Convert.ToByte(TB_Friendship.Text);

            edits[0x15] = (byte)CB_EXPGroup.SelectedIndex;

            edits[0x16] = (byte)CB_EggGroup1.SelectedIndex;
            edits[0x17] = (byte)CB_EggGroup2.SelectedIndex;

            edits[0x18] = (byte)CB_Ability1.SelectedIndex;
            edits[0x19] = (byte)CB_Ability2.SelectedIndex;
            edits[0x1A] = (byte)CB_Ability3.SelectedIndex;

            //edits[0x20] = Convert.ToByte(TB_FormeCount.Text);
            //Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(TB_FormeSprite.Text)),0, edits, 0x1E,2);
            byte color = Convert.ToByte(CB_Color.SelectedIndex);
            color |= (byte)(Convert.ToByte(TB_RawColor.Text) & (byte)0xF0);
            edits[0x21] = color;

            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(TB_BaseExp.Text)), 0, edits, 0x22, 2);

            float height = (float)Convert.ToDouble(TB_Height.Text);
            float weight = (float)Convert.ToDouble(TB_Weight.Text);
            height *= 100;
            weight *= 10;
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(height)), 0, edits, 0x24, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(weight)), 0, edits, 0x26, 2);

            //TMHM
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i * 8 + j < CLB_TMHM.Items.Count)
                    {
                        if (CLB_TMHM.GetItemChecked(i * 8 + j))
                        {
                            edits[0x28+i] |= (byte)(1<<j);
                        }
                    }
                }
            }
            uint tutors = 0;
            for (int t = 0; t < 8; t++)
            {
                if (t < CLB_MoveTutors.Items.Count && CLB_MoveTutors.GetItemChecked(t))
                {
                    tutors |= (uint)(1<<t);
                }
            }
            Array.Copy(BitConverter.GetBytes(tutors), 0, edits, 0x38, 4);

            if (mode == "ORAS")
            {
                uint[] tutorm = new uint[4];
                int ofs=0;
                for (int j = 0; j < tutor1.Length; j++)
                {
                    if (CLB_OrasTutors.GetItemChecked(ofs))
                    {
                        tutorm[0] |= (uint)(1<<j);
                    }
                    ofs++;
                }
                for (int j = 0; j < tutor2.Length; j++)
                {
                    if (CLB_OrasTutors.GetItemChecked(ofs))
                    {
                        tutorm[1] |= (uint)(1 << j);
                    }
                    ofs++;
                }
                for (int j = 0; j < tutor3.Length; j++)
                {
                    if (CLB_OrasTutors.GetItemChecked(ofs))
                    {
                        tutorm[2] |= (uint)(1 << j);
                    }
                    ofs++;
                }
                for (int j = 0; j < tutor4.Length; j++)
                {
                    if (CLB_OrasTutors.GetItemChecked(ofs))
                    {
                        tutorm[3] |= (uint)(1 << j);
                    }
                    ofs++;
                }
                for (int j = 0x40; j < 0x50; j += 4)
                {
                    Array.Copy(BitConverter.GetBytes(tutorm[(j - 0x40) / 4]), 0, edits, j, 4);
                }
            }

            File.WriteAllBytes(paths[CB_Species.SelectedIndex], edits);
            Array.Copy(edits, 0, data, CB_Species.SelectedIndex * len, len);
            File.WriteAllBytes(paths[paths.Length - 1], data);
        }

    }
}
