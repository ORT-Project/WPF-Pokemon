using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Pokemon;

public partial class UserController : Window
{
    public UserController()
    {
        InitializeComponent();
        LoadUserDatasInTheSheet();
    }
    
    private async void LoadUserDatasInTheSheet()
    {
        try
        {
            List<User> users = await LoadData();
            UserDataGrid.ItemsSource = users;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du chargement des données : {ex.Message}");
        }
    }
    
    private async Task<List<User>> LoadData()
    {
        using (HttpClient client = new HttpClient())
        {
            // URL de l'API
            var response = await client.GetAsync("https://localhost:7274/api/User/GetUsers");
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                // Convertir la réponse en liste d'objets Pokémon
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            else
            {
                throw new Exception($"Erreur du serveur : {response.StatusCode}");
            }
        }
    }
    
    private async Task UpdateUserInApi(User user)
    {
        using (HttpClient client = new HttpClient())
        {
            // Construire l'URL pour la mise à jour du Pokémon
            var response = await client.PutAsJsonAsync("https://localhost:7274/api/User/UpdateUser", user);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erreur lors de la mise à jour de l'utilisateur.");
            }
        }
    }
    
    private async void UserDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        var editedUser = e.Row.Item as User;

        if (editedUser != null)
        {
            var editedCell = e.EditingElement as TextBox;
            string newValue = editedCell?.Text;

            switch (e.Column.Header.ToString())
            {
                case "Nom":
                    editedUser.Name = newValue; 
                    break;
                case "Sexe":
                    editedUser.Sexe = newValue; 
                    break;
                case "Money":
                    if (int.TryParse(newValue, out int money))
                    {
                        editedUser.Money = money; 
                    }
                    break;
            }
            
            try
            {
                await UpdateUserInApi(editedUser);
                
                var userList = UserDataGrid.ItemsSource as List<User>;
                UserDataGrid.ItemsSource = null;
                UserDataGrid.ItemsSource = userList;
                // MessageBox.Show("Pokémon modifié avec succès !");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification : {ex.Message}");
            }
        }
    }


    private async void DeleteUser_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var selectedUser = button?.Tag as User;

        if (selectedUser != null)
        {
            // Demande de confirmation avant de supprimer
            MessageBoxResult result = MessageBox.Show($"Voulez-vous vraiment supprimer l'utilisateur : {selectedUser.Name} ?", 
                "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Appel de l'API pour supprimer le Pokémon
                    await DeleteUserFromApi(selectedUser.Id);

                    // Supprime le Pokémon de la liste et met à jour le DataGrid
                    var userList = UserDataGrid.ItemsSource as List<User>;
                    userList.Remove(selectedUser);
                    UserDataGrid.ItemsSource = null; // Réinitialiser la source
                    UserDataGrid.ItemsSource = userList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}");
                }
            }
        }
    }

    private async Task DeleteUserFromApi(int userId)
    {
        using (HttpClient client = new HttpClient())
        {
            // Construire l'URL de l'API pour supprimer le Pokémon
            var response = await client.DeleteAsync($"https://localhost:7274/api/User/DeleteUser/{userId}");
        
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erreur lors de la suppression de cet Utilisateur.");
            }
        }
    }
    
    private async void AddUser_Click(object sender, RoutedEventArgs e)
    {
        string name = UserNameInput.Text;
        string sexe = (UserSexeInput.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (!int.TryParse(UserMoneyInput.Text, out int money))
        {
            MessageBox.Show("Veuillez entrer un montant valide.");
            return;
        }
    
        User newUser = new User { Name = name, Sexe = sexe, Money = money };
    
        try
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync("https://localhost:7274/api/User/InsertUser", newUser);
                response.EnsureSuccessStatusCode();
            }
        
            var userList = await LoadData();
            UserDataGrid.ItemsSource = null;
            UserDataGrid.ItemsSource = userList;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'ajout : {ex.Message}");
        }
    }

    private void OnClickGoToPageMainWindow(object sender, RoutedEventArgs args)
    {
        MainWindow mainController = new MainWindow();
        mainController.Show();
        this.Close();
    }
    
    private void OnClickGoToPagePokemon(object sender, RoutedEventArgs args)
    {
        PokemonController pokemonController = new PokemonController();
        pokemonController.Show();
        this.Close();
    }
    
    private void OnClickGoToPageGenerator(object sender, RoutedEventArgs args)
    {
        UserController userController = new UserController();
        userController.Show();
        this.Close();
    }
}