using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Data;

namespace Victorien_2728981_UA1
{
    public partial class MainWindow : Window
    {
        private string operationCourante = "";                // Contient la chaîne d'opération complète
        private string entreeCourante = "";                  // Contient le nombre actuellement saisi
        private bool nouvelleEntree = true;                 // Indique quand commencer une nouvelle saisie
        private bool resultatAffiche = false;              // Indique si un résultat a été affiché
      private double dernierResultat = 0;                  // Stocke le dernier résultat pour la continuation

        public MainWindow()
        {
            InitializeComponent();
        }

        // Gérer les clics sur les boutons numériques
        private void BoutonNombre_Click(object sender, RoutedEventArgs e)
        {
            Button bouton = sender as Button;

            // Si un résultat a été affiché, permettre à l'utilisateur de continuer l'opération
            if (resultatAffiche)
            {
                entreeCourante = "";                       // Réinitialiser l'entrée actuelle
                resultatAffiche = false;
                nouvelleEntree = false;                     // Permet de continuer l'opération après le résultat
            }

            // Si c'est une nouvelle entrée, on commence à entrer un nouveau nombre
            if (nouvelleEntree)
            {
                entreeCourante = "";                    // Réinitialise l'entrée actuelle
                nouvelleEntree = false;                // Pas une nouvelle entrée
            }

            entreeCourante += bouton.Content.ToString();        // Ajouter le chiffre à l'entrée actuelle
            OperationLabel.Content = operationCourante + entreeCourante; // Met à jour le label d'opération
        }

        // Gérer les clics sur les boutons d'opérateurs (+, -, *, ÷)
        private void BouttonOperateur_Click(object sender, RoutedEventArgs e)
        {
            Button bouton = sender as Button;
            string symboleOperateur = bouton.Content.ToString();

            //  continuer l'opération
            if (resultatAffiche)
            {
                operationCourante += symboleOperateur + " ";                           // Ajouter l'opérateur à l'opération
                resultatAffiche = false;                                // Ne plus considérer le résultat affiché
            }
            else if (!string.IsNullOrEmpty(entreeCourante))
            {
                operationCourante += entreeCourante + " " + symboleOperateur + " ";  // Ajoute l'entrée actuelle et l'opérateur
                entreeCourante = ""; // Réinitialiser l'entrée actuelle pour le prochain nombre
            }

            OperationLabel.Content = operationCourante;                 // Affiche l'opération complète
            nouvelleEntree = true;                                // Prêt pour une nouvelle entrée
        }


        // Effectuer le calcul lorsque "=" est cliqué
        private void BouttonEgale_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(entreeCourante))
            {
                operationCourante += entreeCourante; // Ajoute l'entrée courante à l'opération
            }

            try
            {
                // Vérifier si l'opération courante contient une fonction trigonométrique
                if (operationCourante.Contains("sin") || operationCourante.Contains("cos") || operationCourante.Contains("tan") ||
                    operationCourante.Contains("arcsin") || operationCourante.Contains("arcos") || operationCourante.Contains("arctan"))
                {
                    FonctionEvaluationTri(); // Évaluer la fonction trigonométrique
                    return; // Sortir de la méthode après avoir évalué la fonction trigonométrique
                }

                // Remplacer les symboles pour l'évaluation
                string operationAEvaluer = operationCourante.Replace("÷", "/").Replace("x", "*")
                    .Replace("π", Math.PI.ToString()).Replace("e", Math.E.ToString());

                // Calcul
                var resultat = Evaluate(operationAEvaluer);                  // Évaluation de l'opération
                ResultLabel.Content = resultat.ToString();                   // Affiche le résultat
                dernierResultat = resultat;
                resultatAffiche = true;
            }
            catch (Exception ex)
            {
                ResultLabel.Content = "Erreur";           // exection a toute autre operation
                                                
            }

            nouvelleEntree = true; // Prêt pour une nouvelle entrée
            entreeCourante = ""; // Réinitialiser l'entrée courante
        }


        // Methode pour le boutton CE
        private void BouttonEffacerEntree_Click(object sender, RoutedEventArgs e)
        {
            entreeCourante = "";
            OperationLabel.Content = operationCourante + entreeCourante;
        }

        // Methode pour le boutton C
        private void BouttonEffacer_Click(object sender, RoutedEventArgs e)
        {
            operationCourante = "";
            entreeCourante = "";
            OperationLabel.Content = "0";
            ResultLabel.Content = "0";
            resultatAffiche = false;
            dernierResultat = 0;
        }

        // Methode pour le boutton back
        private void BouttonBack_Click(object sender, RoutedEventArgs e)
        {
            if (entreeCourante.Length > 0)
            {
                entreeCourante = entreeCourante.Remove(entreeCourante.Length - 1);
                OperationLabel.Content = operationCourante + entreeCourante;
            }
        }

        // methode    plus/moins
        private void BouttonPlusMoins_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(entreeCourante, out double entreeParsee))
            {
                entreeParsee = -entreeParsee;
                entreeCourante = entreeParsee.ToString();
                OperationLabel.Content = operationCourante + entreeCourante;
            }
        }

        // Gérer les fonctions trigonométriques
        private void BouttonTrigo_Click(object sender, RoutedEventArgs e)
        {
            Button bouton = sender as Button;
            string fonctionTrigo = bouton.Content.ToString().ToLower();

            // Ajouter la fonction trigonométrique et ouvrir la parenthèse
            operationCourante += fonctionTrigo + "(";
            OperationLabel.Content = operationCourante+"";
            entreeCourante = "";
            nouvelleEntree = false;
        }

        // Gérer le clic du bouton π
        private void BouttonPi_Click(object sender, RoutedEventArgs e)
        {
            entreeCourante = " ";
            operationCourante += "π ";
            OperationLabel.Content = operationCourante;
        }

        // Gérer le clic du bouton e
        private void BoutonExpo_Click(object sender, RoutedEventArgs e)
        {
            entreeCourante = " ";
            operationCourante += "e";
            OperationLabel.Content = operationCourante;
        }

        // Convertit les degrés en radians
        private double ConvertDegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }



        // Effectuer le calcul de la fonction trigonométrique
        private void FonctionEvaluationTri()
        {
            if (double.TryParse(entreeCourante, out double entreeParsee))
            {
                try
                {
                    double resultat = 0; // Initialiser le résultat
                    string fonctionTrigonometrie = operationCourante.Split('(')[0]; // Obtenir le nom de la fonction trigonométrique

                    switch (fonctionTrigonometrie)
                    {
                        case "sin":
                            resultat = Math.Sin(ConvertDegreesToRadians(entreeParsee)); // Calculer le sinus
                            break;
                        case "cos":
                            resultat = Math.Cos(ConvertDegreesToRadians(entreeParsee)); // Calculer le cosinus
                            break;
                        case "tan":
                            if (entreeParsee == 90)
                            {
                                ResultLabel.Content = "tan("+ entreeParsee+") non definit ";
                                return;
                            }
                            resultat = Math.Tan(ConvertDegreesToRadians(entreeParsee)); // Calculer la tangente
                            break;
                        case "arcsin":
                            if (entreeParsee < -1 || entreeParsee > 1 )
                            {
                                ResultLabel.Content = "arcsin definit entre[-1;1]";
                                return;
                            }
                            resultat = Math.Asin(entreeParsee) * 180 / Math.PI; // CALCULER arc sinus
                            break;
                        case "arcos":
                            if (entreeParsee < -1 || entreeParsee > 1)
                            {
                                
                              ResultLabel.Content= "arcos definit entre[-1;1]";
                                return;
                            }
                            resultat = Math.Acos(entreeParsee) * 180 / Math.PI;
                            break;
                        case "arctan":
                            resultat = Math.Atan(entreeParsee) * 180 / Math.PI;
                            break;
                        default:
                            ResultLabel.Content = "ERREUR:non prise en charge";
                            return;
                            
                    }

                    // Ajouter le résultat dans le label
                    ResultLabel.Content = resultat.ToString(); // Affiche le résultat
                    operationCourante += ")"; // Finaliser l'opération 
                    OperationLabel.Content = operationCourante; // Affiche l'opération complète
                }
                
                catch (Exception ex)
                {
                    ResultLabel.Content = "ERREUR"; // Afficher une erreur générique
                }
            }
        }

       

        // Évaluer l'opération complète
        private double Evaluate(string operation)
        {
            DataTable table = new DataTable();
            return Convert.ToDouble(table.Compute(operation, ""));
        }
    }
}

