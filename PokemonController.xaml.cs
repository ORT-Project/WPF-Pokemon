using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Pokemon;

public partial class PokemonController : Window
{
    public PokemonController()
    {
        InitializeComponent();
        LoadPokemonDatasInTheSheet();
    }

    private async void LoadPokemonDatasInTheSheet()
    {
        try
        {
            List<Pokemon> pokemons = await LoadData();
            PokemonDataGrid.ItemsSource = pokemons;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du chargement des données : {ex.Message}");
        }
    }
    
    private async Task<List<Pokemon>> LoadData()
    {
        using (HttpClient client = new HttpClient())
        {
            // URL de l'API
            var response = await client.GetAsync("https://localhost:7274/api/Pokemon/GetPokemon");
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                // Convertir la réponse en liste d'objets Pokémon
                return await response.Content.ReadFromJsonAsync<List<Pokemon>>();
            }
            else
            {
                throw new Exception($"Erreur du serveur : {response.StatusCode}");
            }
        }
    }

    private async Task UpdatePokemonInApi(Pokemon pokemon)
    {
        using (HttpClient client = new HttpClient())
        {
            // Construire l'URL pour la mise à jour du Pokémon
            var response = await client.PutAsJsonAsync("https://localhost:7274/api/Pokemon/UpdatePokemon", pokemon);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erreur lors de la mise à jour du Pokémon.");
            }
        }
    }
    
    private async void PokemonDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        var editedPokemon = e.Row.Item as Pokemon;

        if (editedPokemon != null)
        {
            var editedCell = e.EditingElement as TextBox;
            string newValue = editedCell?.Text;

            switch (e.Column.Header.ToString())
            {
                case "Nom":
                    editedPokemon.Name = newValue; 
                    break;
                case "Type":
                    editedPokemon.Type = newValue; 
                    break;
                case "Attaque":
                    if (int.TryParse(newValue, out int attack))
                    {
                        editedPokemon.Attack = attack; 
                    }
                    break;
                case "Défense":
                    if (int.TryParse(newValue, out int defense))
                    {
                        editedPokemon.Defense = defense; 
                    }
                    break;
                case "Santé":
                    if (int.TryParse(newValue, out int health))
                    {
                        editedPokemon.Health = health;
                    }
                    break;
            }
            
            try
            {
                await UpdatePokemonInApi(editedPokemon);
                
                var pokemonList = PokemonDataGrid.ItemsSource as List<Pokemon>;
                PokemonDataGrid.ItemsSource = null;
                PokemonDataGrid.ItemsSource = pokemonList;
                // MessageBox.Show("Pokémon modifié avec succès !");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification : {ex.Message}");
            }
        }
    }


    private async void DeletePokemon_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var selectedPokemon = button?.Tag as Pokemon;

        if (selectedPokemon != null)
        {
            // Demande de confirmation avant de supprimer
            MessageBoxResult result = MessageBox.Show($"Voulez-vous vraiment supprimer le Pokémon : {selectedPokemon.Name} ?", 
                "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Appel de l'API pour supprimer le Pokémon
                    await DeletePokemonFromApi(selectedPokemon.Id);

                    // Supprime le Pokémon de la liste et met à jour le DataGrid
                    var pokemonList = PokemonDataGrid.ItemsSource as List<Pokemon>;
                    pokemonList.Remove(selectedPokemon);
                    PokemonDataGrid.ItemsSource = null; // Réinitialiser la source
                    PokemonDataGrid.ItemsSource = pokemonList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression : {ex.Message}");
                }
            }
        }
    }

    private async Task DeletePokemonFromApi(int pokemonId)
    {
        using (HttpClient client = new HttpClient())
        {
            // Construire l'URL de l'API pour supprimer le Pokémon
            var response = await client.DeleteAsync($"https://localhost:7274/api/Pokemon/DeletePokemon/{pokemonId}");
        
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erreur lors de la suppression du Pokémon.");
            }
        }
    }



}