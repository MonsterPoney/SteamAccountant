using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SteamWalletHistory {
    class Program {
        [STAThread]
        static void Main() {
            Console.OutputEncoding = UTF8Encoding.UTF8;

            tagStart:
            Console.WriteLine("type 'help' or 'h' for help, or enter to continue");
            string read = Console.ReadLine().ToLower();
            if (read == "help" || read == "h")
                Console.WriteLine(@"---------------- EN--------------------------------

Obtain a .txt file of your wallet history :
    From internet or Steam client, click on your profile then ""Account detail"".
Then ""View purchase history"".

Go at the bottom of the page and click on ""Show more transactions"" if it's present.
Repeat the opération untill all transactions are showed.
Select all the page (CTRL + A) and copy it (CTRL + C)
Paste it (CTRL + V) on a notepad and save the file.

When the program starts, it asks you to indicate the path of the folder.


--------------- FR --------------------------------

Obtenir un fichier .txt de votre historique portefeuille:
    Depuis internet ou le client Steam, cliquez sur votre profil puis ""Détail du compte"".
Puis ""Voir votre historique d'achats"".

Défiler tout en bas de la page et cliquez sur ""Afficher plus de transactions"" s'il est présent.
Répéter cette opération jusqu'a ce que toutes les transactions sont présentes.
Sélectionner toute la page(CTRL + A) et copiez la(CTRL + C)
Collez la ensuite(CTRL + V) dans un bloc-note ou notepad et enregistrez le fichier.

Lorsque le programme est lancé, vous serez ammenez a indiquer le chemin du fichier.");
            double credit = 0;
            int creditNb = 0;
            double buy = 0;
            int buyNb = 0;
            double refund = 0;
            int refundNb = 0;
            string firstTransaction = "";
            int lineNb = 0;
            char currency = ' ';

            try {
                OpenFileDialog openFile = new OpenFileDialog
                {
                    InitialDirectory = "c:\\",
                    Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                    FilterIndex = 1
                };

                string path;
                if (openFile.ShowDialog() == DialogResult.OK) {
                    path = openFile.FileName;
                } else
                    goto tagStart;
                StreamReader reader = new StreamReader(path);
                string sample = "";
                for (int i = 0; i < 40; i++) {
                    sample += reader.ReadLine();
                }
                if (sample.Contains('€'))
                    currency = '€';
                else if (sample.Contains('$'))
                    currency = '$';
                //else if (sample.Contains('¥'))
                //    currency = '¥';
                else {
                    Console.WriteLine("No currency detected");
                    goto tagStart;
                }
                // Reset StreamReader
                reader.Close();
                reader = new StreamReader(path);

                while (!reader.EndOfStream) {
                    string cl = reader.ReadLine();
                    lineNb++;
                    // French || English || Espanol,Espana
                    if (cl == "Crédité" || cl == "Credit" || cl == "Crédito") {
                        string line = reader.ReadLine().Replace('-', '0');
                        creditNb++;
                        Console.WriteLine("Credit : " + line);
                        lineNb++;
                        credit += double.Parse(line.Split(currency)[0]);
                    } else if (cl == "Achat" || cl == "Purchase" || cl == "Compra") {
                        // Consume a line
                        reader.ReadLine();
                        string line = reader.ReadLine().Replace('-', '0');
                        buyNb++;
                        lineNb += 2;
                        Console.WriteLine("Purchase : " + line);
                        buy += +double.Parse(line.Split(currency)[0]);
                    } else if (cl == "Remboursement" || cl == "Refund" || cl == "Reembolso") {
                        // Consume 2 lines
                        reader.ReadLine();
                        reader.ReadLine();
                        string line = reader.ReadLine().Replace('-', '0');
                        refundNb++;
                        lineNb += 3;
                        Console.WriteLine("Refund : " + line);
                        refund += double.Parse(line.Split(currency)[0]);
                    }
                }
                // Reset StreamReader
                reader.Close();
                reader = new StreamReader(path);

                // Take first transaction w/ date
                for (int i = 0; i < lineNb - 5; i++) {
                    reader.ReadLine();
                }
                firstTransaction = reader.ReadLine().Replace("\t", "");
                firstTransaction += " : " + reader.ReadLine();

            }
            catch (Exception e) {
                Console.WriteLine("Exception : " + e.Message + e.StackTrace + e.Source);
            }
            Console.WriteLine($"\nSteam shop total sale(s) : {Math.Round(credit, 2)}{currency} for {creditNb} transactions\n " +
                $"Purchase(s) : {Math.Round(buy, 2)}{currency} for {buyNb} transactions\n " +
                $"Refund(s) : {Math.Round(refund, 2)}{currency} for {refundNb} refunds\n" +
                $"Purchases - refunds : {Math.Round(buy - refund, 2)}{currency}\n" +
                $"Gain : {Math.Round(credit - (buy - refund), 2) }{currency}\n" +
                $"First transaction : {firstTransaction}\n\n" +
                "Press any key to exit");
            Console.Read();
        }
    }
}
