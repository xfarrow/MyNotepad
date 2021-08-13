using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyNotepad
{
    public partial class Vai_alla_riga : Form
    {
        myBloccoNote formChiamante;
        public Vai_alla_riga()
        {
            InitializeComponent();
        }
        public Vai_alla_riga(myBloccoNote formChiamanteObj)
        {
            InitializeComponent();
            formChiamante = formChiamanteObj;
        }

        private void vai_alla_riga_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(vaiAllaRiga_textbox.Text)) return; //do nothing
            try
            {
                this.Hide();
                formChiamante.vaiARigo(int.Parse(vaiAllaRiga_textbox.Text));
            }
            catch(System.FormatException)
            {
                MessageBox.Show("Ammessi solo valori numerici");
            }
        }
    }
}
