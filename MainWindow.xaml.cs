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

namespace WPF_Pokemon;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void OnClickGoToPagePokemon(object sender, RoutedEventArgs args)
    {
        PokemonController pokemonController = new PokemonController();
        pokemonController.Show();
        this.Close();
    }
    
    private void OnClickGoToPageUser(object sender, RoutedEventArgs args)
    {
        UserController userController = new UserController();
        userController.Show();
        this.Close();
    }

    private void OnClickGeneratePokemon(object sender, RoutedEventArgs args)
    {
        
    }
}