using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace MyNotepad
{
    public partial class myBloccoNote : Form
    {
        //Path del documento corrente
        private string documentPath;

        //Variabile contenente il testo non modificato
        private string testoCorrente = "";

        //Utile per operazioni di ricerca all'interno del documento (condiviso tra i metodi di ricerca e trova successivo)
        private int posizioneParolaDaCercareNelDocumento = 0;

        public myBloccoNote()
        {
            InitializeComponent();
        }

        // Pulsante "esci"
        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!chiediDiSalvareSeDocumentoModificato()) return;
            Application.Exit();
        }

        // Pulsante "nuovo"
        private void nuovoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!chiediDiSalvareSeDocumentoModificato()) return;
            testo_notepad.Clear();
            testoCorrente = "";
        }

        // Pulsante "apri"
        private void apriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!chiediDiSalvareSeDocumentoModificato()) return;
            open_dialog.ShowDialog();
            try
            {
                if (open_dialog.FileName != null)
                {
                    testo_notepad.Text = System.IO.File.ReadAllText(open_dialog.FileName);
                    testoCorrente = testo_notepad.Text;
                }
            }
            catch
            {
                // Nothing happens
            }
        }

        // Pulsante "salva con nome"
        private void salvaConNomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showSaveDialogPrompt();
        }

        // Visualizza a schermo la finestra di salvataggio con nome
        private void showSaveDialogPrompt()
        {
            try
            {
                if (save_dialog.ShowDialog() == DialogResult.OK && save_dialog.FileName != null)
                {
                    System.IO.File.WriteAllText(save_dialog.FileName, testo_notepad.Text);
                    documentPath = save_dialog.FileName;
                    testoCorrente = testo_notepad.Text;

                    //Titolo dell'eseguibile
                    this.Text = documentPath;
                }
            }
            catch (System.ArgumentException)
            {
            }
        }

        // Pulsante "salva"
        private void salvaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            salvaDocumento();
        }

        // Funzione di salvataggio documento
        private void salvaDocumento()
        {
            if (!String.IsNullOrEmpty(documentPath) && File.Exists(documentPath))
            {
                System.IO.File.WriteAllText(documentPath, testo_notepad.Text);
                testoCorrente = testo_notepad.Text;
            }
            else
            {
                showSaveDialogPrompt();
            }
        }

        // In alcuni casi è necessario chiedere quale azione intraprendere se il file è stato modificato
        // eppure si tenta di uscire o di crearne uno nuovo.
        // true = continua ; false = abort
        private bool chiediDiSalvareSeDocumentoModificato()
        {
            if (!testo_notepad.Text.Equals(testoCorrente))
            {
                DialogResult rispostaUtente = MessageBox.Show("Documento modificato, salvare?", "Attenzione", MessageBoxButtons.YesNoCancel);
                if (rispostaUtente == DialogResult.Yes)
                {
                    salvaDocumento();
                }
                else if (rispostaUtente == DialogResult.Cancel) return false;
            }
            return true;
        }

        // Pulsante "Informazioni su blocco note"
        private void informazioniSuBloccoNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MyNotepad\n\nVersione 1.0.2\n\nDeveloped by xfarrow", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        // Mostra o nascondi barra di stato
        private void checkbox_barra_di_stato_Click(object sender, EventArgs e)
        {
            if (barra_di_stato.Visible) barra_di_stato.Visible = false;
            else barra_di_stato.Visible = true;
        }

        private void selezionaTuttoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testo_notepad.SelectAll();
        }

        private void tagliaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testo_notepad.Cut();
        }

        private void copiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testo_notepad.Copy();
        }

        private void incollaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testo_notepad.Paste();
        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testo_notepad.SelectedText = "";
        }

        //Bottone "imposta pagina"
        private void impostaPaginaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDocument.DocumentName = documentPath;
            pageSetupDialog.ShowDialog();
        }

        //Bottone "stampa"
        private void stampaToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (schermataStampa.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string testoDaStampare = testo_notepad.Text;
            int caratteri_pagina = 0;
            int righe_pagina = 0;

            FontFamily FamigliaFont = new FontFamily("Arial");
            Font fontDiStampa = new Font(FamigliaFont, 16, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.MeasureString(testoDaStampare, fontDiStampa, e.MarginBounds.Size, StringFormat.GenericTypographic, out caratteri_pagina, out righe_pagina);
            e.Graphics.DrawString(testoDaStampare, fontDiStampa, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic);
            testoDaStampare = testoDaStampare.Substring(caratteri_pagina);
            e.HasMorePages = testoDaStampare.Length > 0;
        }

        private void annullaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testo_notepad.Undo();
        }

        private void cercaConGooogleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(testo_notepad.SelectedText))
            {
                MessageBox.Show("Selezionare prima qualcosa da cercare");
                return;
            }
            string testoDaCercare = testo_notepad.SelectedText;

            testoDaCercare.Replace(" ", "+");

            System.Diagnostics.Process.Start("https://www.google.it/search?q=" + testoDaCercare);
        }

        private void oraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime dataCorrente = DateTime.Now;
            string oraCorrenteStr = dataCorrente.Hour.ToString() + ":" + dataCorrente.Minute.ToString() + ":" + dataCorrente.Second.ToString();
            testo_notepad.Text = testo_notepad.Text.Insert(testo_notepad.SelectionStart, oraCorrenteStr);

            testo_notepad.SelectionStart += oraCorrenteStr.Length;
        }

        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime dataCorrente = DateTime.Now;
            string dataCorrenteStr = dataCorrente.Day.ToString() + "/" + dataCorrente.Month.ToString() + "/" + dataCorrente.Year.ToString();
            testo_notepad.Text = testo_notepad.Text.Insert(testo_notepad.SelectionStart, dataCorrenteStr);
            testo_notepad.SelectionStart += dataCorrenteStr.Length;
        }

        private void oraEDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime dataCorrente = DateTime.Now;
            string dataOraCorrenteStr = dataCorrente.Hour.ToString() + ":" + dataCorrente.Minute.ToString() + ":" + dataCorrente.Second.ToString() + " " + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
            testo_notepad.Text = testo_notepad.Text.Insert(testo_notepad.SelectionStart, dataOraCorrenteStr);
            testo_notepad.SelectionStart += dataOraCorrenteStr.Length;
        }

        // Bottone "trova"
        private void trovaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (barraCerca.Visible == false) barraCerca.Visible = true;
            else barraCerca.Visible = false;
        }

        // Bottone "cerca" (visibile solo quando barraCerca è visibile)
        private void Cerca_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox_cerca.Text))
            {
                findNext();
            }
            else
            {
                MessageBox.Show("Casella vuota", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Bottone "sostituisci" (visibile solo quando barraCerca è visibile)
        private void button_sostituisci_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox_cerca.Text) && !String.IsNullOrEmpty(textBox_sostituisciCon.Text))
            {
                testo_notepad.Text = testo_notepad.Text.Replace(textBox_cerca.Text, textBox_sostituisciCon.Text);
            }
            else
            {
                MessageBox.Show("Una o più caselle vuote", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Trova successivo
        private void trovaSuccessivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox_cerca.Text))
            {
                findNext();
            }
            else
            {
                MessageBox.Show("Casella vuota", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void findNext()
        {
            try
            {
                testo_notepad.Focus();
                posizioneParolaDaCercareNelDocumento = testo_notepad.Find(textBox_cerca.Text, posizioneParolaDaCercareNelDocumento, RichTextBoxFinds.None);
                if (posizioneParolaDaCercareNelDocumento == -1)
                {
                    MessageBox.Show("Testo non trovato", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    posizioneParolaDaCercareNelDocumento = 0;
                }
                else
                {
                    testo_notepad.Select(posizioneParolaDaCercareNelDocumento, textBox_cerca.Text.Length);
                    posizioneParolaDaCercareNelDocumento += textBox_cerca.Text.Length;
                }
            }
            catch
            {
                MessageBox.Show("Unexpected error", "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void vaiAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vai_alla_riga form_vai_alla_riga = new Vai_alla_riga(this);
            form_vai_alla_riga.Show();
        }

        public void vaiARigo(int rigo)
        {
            try
            {
                int indice = testo_notepad.GetFirstCharIndexFromLine(rigo - 1); //(gli umani contano da 1)
                testo_notepad.Select(indice, 0);
                testo_notepad.ScrollToCaret();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Numero di riga non valido");
            }
        }

        private void aCapoAutomaticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testo_notepad.WordWrap)
            {
                testo_notepad.WordWrap = false;
                aCapoAutomaticoToolStripMenuItem.Checked = false;
            }
            else
            {
                testo_notepad.WordWrap = true;
                aCapoAutomaticoToolStripMenuItem.Checked = true;
            }
        }

        private void carattereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                testo_notepad.Font = fontDialog.Font;
            }
        }

        private void aumentaZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testo_notepad.ZoomFactor < 2.0f)
            {
                testo_notepad.ZoomFactor += 0.2f;
                status_zoom.Text = "Zoom: " + (int)(testo_notepad.ZoomFactor * 100) + "%";
            }
        }

        private void diminuisciZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testo_notepad.ZoomFactor > 0.6f)
            {
                testo_notepad.ZoomFactor -= 0.2f;
                status_zoom.Text = "Zoom: " + (int)(testo_notepad.ZoomFactor * 100) + "%";
            }
        }

        private void aiutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/xfarrow/myNotepad");
        }

        private void status_primoPiano_Click(object sender, EventArgs e)
        {
            if (TopMost)
            {
                TopMost = false;
                status_primoPiano.Text = "Primo piano: OFF";
            }
            else
            {
                TopMost = true;
                status_primoPiano.Text = "Primo piano: ON";
            }
        }

        private void testo_notepad_TextChanged(object sender, EventArgs e)
        {
            numero_caratteri_statusBar.Text = testo_notepad.Text.Count().ToString();
        }

        private void tagliaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            testo_notepad.Cut();
        }

        private void copiaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            testo_notepad.Copy();
        }

        private void incollaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            testo_notepad.Paste();
        }

        private void selezionaTuttoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            testo_notepad.SelectAll();
        }

        private void myBloccoNote_Load(object sender, EventArgs e)
        {
            /*
             * Apre un file mediante tasto destro su un file -> apri con -> notepad.exe
             */
            if (Environment.GetCommandLineArgs().Count() > 1)
            {
                string[] argomenti = Environment.GetCommandLineArgs();
                testo_notepad.Text = System.IO.File.ReadAllText(argomenti[1]);
                testoCorrente = testo_notepad.Text;
            }
        }
    }
}
