using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype_Styles_C64_True_Type_Fonts
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                fill(radioButton1.Text);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                fill(radioButton2.Text);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            fill(radioButton1.Text);
        }
        public String getUnicodeByPETSCIIforStylesC64TTF(byte[] dataBytes, int fontType, bool noClear)
        {
            // fontType
            // 0 .. Großschrift / Grafikzeichen (Zeichensatz 1, Standard)
            // 1 .. Groß-/ Kleinschrift (Zeichensatz 2)
            // 2 .. revers Großschrift / Grafikzeichen (Zeichensatz 1, Standard)
            // 3 .. revers Groß-/ Kleinschrift (Zeichensatz 2)
            // 4 .. Zeichensatz 1
            // 5 .. Zeichensatz 2
            // 6 .. CP1252
            String ret = "";
            byte[] b = new byte[2];
            switch (fontType)
            {
                case 0: b[1] = 0xE0; break;
                case 1: b[1] = 0xE1; break;
                case 2: b[1] = 0xE2; break;
                case 3: b[1] = 0xE3; break;
                case 4: b[1] = 0xEE; break;
                case 5: b[1] = 0xEF; break;
                case 6: b[1] = 0x00; break; // CodePage 1252
                case 7: b[1] = 0x00; break; // CodePage 1252 alle ab 32
                default: return "";
            }
            foreach (byte petB in dataBytes)
            {
                if (fontType == 6)
                {
                    if (((petB >= 0x20 && petB <= 0x7E) || petB == 0x85 || petB == 0x92 || petB == 0x93 || petB == 0x94 || petB == 0x95 || petB == 0x97) || noClear)
                    {
                        ret += Encoding.GetEncoding(1252).GetString(new byte[1] { petB }); // String Builder verwenden!
                    }
                    else
                    {
                        ret += '\uE000'; // Clear
                    }
                }
                else if (fontType == 7)
                {
                    if (((petB >= 0x20) && petB != 0x7f && petB != 0x81 && petB != 0x8d && petB != 0x8f && petB != 0x90 && petB != 0x98 && petB != 0x9d) || noClear)
                    {
                        ret += Encoding.GetEncoding(1252).GetString(new byte[1] { petB }); // String Builder verwenden!
                    }
                    else
                    {
                        ret += '\uE000'; // Clear
                    }
                }
                else
                {
                    b[0] = petB;
                    ret += Encoding.Unicode.GetString(b); // String Builder verwenden!
                }
            }
            return ret;
        }
        public String getBlock(int fontType, bool noClear)
        {
            int x, y, val;
            string st = "";
            byte[] ba = new byte[16];
            for (y = 0; y <= 15; y++)
            {
                for (x = 0; x <= 15; x++)
                {
                    val = y * 16 + x;
                    ba[x] = (byte)val;
                }
                st += getUnicodeByPETSCIIforStylesC64TTF(ba, fontType, noClear) + Environment.NewLine;
            }
            return (st);
        }
        private void fill(string fontName)
        {
            listView1.Clear();
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[0].Text = "xx in HEX";
            listView1.Columns[0].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[1].Text = "xx in DEC";
            listView1.Columns[1].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[2].Text = "Courier New";
            listView1.Columns[2].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[3].Text = "CP 1252";
            listView1.Columns[3].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[4].Text = "($E0 xx)";
            listView1.Columns[4].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[5].Text = "($E1 xx)";
            listView1.Columns[5].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[6].Text = "($E2 xx)";
            listView1.Columns[6].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[7].Text = "($E3 xx)";
            listView1.Columns[7].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[8].Text = "($EE xx)";
            listView1.Columns[8].Width = 70;
            listView1.Columns.Add(new ColumnHeader());
            listView1.Columns[9].Text = "($EF xx)";
            listView1.Columns[9].Width = 70;
            int i;
            for (i = 0; i <= 255; i++)
            {
                String[] sa = new string[10];
                sa[0] = i.ToString("X2");
                sa[1] = i.ToString();
                sa[2] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 7, true); // CP 1252
                sa[3] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 6, true); // CP 1252
                sa[4] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 0, true);
                sa[5] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 1, true);
                sa[6] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 2, true);
                sa[7] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 3, true);
                sa[8] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 4, true);
                sa[9] = getUnicodeByPETSCIIforStylesC64TTF(new byte[] { (byte)i }, 5, true);

                ListViewItem listViewItem1 = new ListViewItem(sa);
                listViewItem1.SubItems[2].Font = new Font("Courier New", 10);
                listViewItem1.SubItems[3].Font = new Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
                listViewItem1.SubItems[4].Font = new Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
                listViewItem1.SubItems[5].Font = new Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
                listViewItem1.SubItems[6].Font = new Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
                listViewItem1.SubItems[7].Font = new Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
                listViewItem1.SubItems[8].Font = new Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
                listViewItem1.SubItems[9].Font = new Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
                listViewItem1.UseItemStyleForSubItems = false;
                listView1.Items.Add(listViewItem1);
            }
            
            textBox1.Font = new System.Drawing.Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
            textBox1.Text = "";
            textBox1.Text += getBlock(0, true);
            textBox2.Font = new System.Drawing.Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
            textBox2.Text = "";
            textBox2.Text += getBlock(2, true);
            textBox3.Font = new System.Drawing.Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
            textBox3.Text = "";
            textBox3.Text += getBlock(4, true);

            textBox4.Font = new System.Drawing.Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
            textBox4.Text = "";
            textBox4.Text += getBlock(1, true);
            textBox5.Font = new System.Drawing.Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
            textBox5.Text = "";
            textBox5.Text += getBlock(3, true);
            textBox6.Font = new System.Drawing.Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
            textBox6.Text = "";
            textBox6.Text += getBlock(5, true);

            textBox7.Font = new System.Drawing.Font(fontName, 8F, System.Drawing.GraphicsUnit.Pixel);
            textBox7.Text = "";
            textBox7.Text += getBlock(6, false);
        }
    }
}
