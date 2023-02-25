using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace SpreadsheetApp
{
    public partial class Form1 : Form

    {
         SharableSpreadSheet SpreadSheet;
        public Form1()
        {
            SpreadSheet = new SharableSpreadSheet(10, 10, 100);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            for (int i = 1; i <= SpreadSheet.matrix[0].Count; i++)
            {
                listView1.Columns.Add("" + i);
            }
            for (int i = 0; i < SpreadSheet.matrix.Count; i++)
            {
                ListViewItem item = new ListViewItem(SpreadSheet.matrix[i][0]);
                for (int j = 1; j < SpreadSheet.matrix[i].Count; j++)
                {
                    item.SubItems.Add(SpreadSheet.matrix[i][j]);
                }
                listView1.Items.Add(item);
            }
            
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SpreadSheet.save("C:\\Sample.txt");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = textBox1.Text;
            if(fileName == "")
            {
                return;
            }
            SpreadSheet.load(fileName);
            listView1.Clear();

            for (int i = 1; i <= SpreadSheet.matrix[0].Count; i++)
            {
                listView1.Columns.Add("" + i);
            }
            for (int i = 0; i < SpreadSheet.matrix.Count; i++)
            {
                ListViewItem item = new ListViewItem(SpreadSheet.matrix[i][0]);
                for (int j = 1; j < SpreadSheet.matrix[i].Count; j++)
                {
                    item.SubItems.Add(SpreadSheet.matrix[i][j]);
                }
                listView1.Items.Add(item);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //search in the SpreadSheet

            Tuple <int,int> returnStr = SpreadSheet.searchString(textBox2.Text);
            listView1.Items[returnStr.Item1].UseItemStyleForSubItems = false;
            listView1.Items[returnStr.Item1].SubItems[returnStr.Item2].BackColor = Color.Pink;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //find all:
            bool t;
            if(textBox4.Text == "True")
            {
                t = true;
            }
            else
            {
                t = false;
            }
            Tuple<int, int>[] returnStr = SpreadSheet.findAll(textBox3.Text,t);

            for( int i =0; i < returnStr.Length; i++)
            {
                listView1.Items[returnStr[i].Item1].UseItemStyleForSubItems = false;
                listView1.Items[returnStr[i].Item1].SubItems[returnStr[i].Item2].BackColor = Color.Pink;
            }

           
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < SpreadSheet.matrix.Count; i++)
            {
                for(int j = 0; j < SpreadSheet.matrix[i].Count; j++)
                {
                    listView1.Items[i].UseItemStyleForSubItems = false;
                    listView1.Items[i].SubItems[j].BackColor = Color.Empty;
                }

            }
        }
    }
}
