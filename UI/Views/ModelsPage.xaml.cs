using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using UI.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModelsPage : Page
    {
        private readonly IOllamaService _ollamaService;

        public ModelsPage()
        {
            this.InitializeComponent();
            _ollamaService = new OllamaService();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadModels();
        }

        private async Task LoadModels()
        {
            try
            {
                LoadingRing.IsActive = true;
                ModelsListView.ItemsSource = null;

                var models = await _ollamaService.GetModels();
                ModelsListView.ItemsSource = models;

                EmptyState.Visibility = models.Any() ? Visibility.Collapsed : Visibility.Visible;
                ModelsListView.Visibility = models.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Erro",
                    Content = $"Erro ao carregar modelos: {ex.Message}",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
            finally
            {
                LoadingRing.IsActive = false;
            }
        }


        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadModels();
        }

        private async void AddModelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogContent = new AddModelDialog(_ollamaService);

            var dialog = new ContentDialog
            {
                Title = "Adicionar Modelo",
                PrimaryButtonText = null,
                CloseButtonText = null,
                Content = dialogContent,
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
            await LoadModels();
        }

        private async void SelectModel_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var model = (OllamaModel)button.DataContext;

            try
            {
                LoadingRing.IsActive = true;
                var success = await _ollamaService.SelectModel(model.Name);

                if (success)
                {
                    await LoadModels();
                }
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Erro",
                    Content = $"Erro ao selecionar modelo: {ex.Message}",
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            finally
            {
                LoadingRing.IsActive = false;
            }
        }

        private async void RemoveModel_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var model = (OllamaModel)button.DataContext;

            var dialog = new ContentDialog
            {
                Title = "Confirmar Exclusão",
                Content = $"Tem certeza que deseja remover o modelo {model.Name}?",
                PrimaryButtonText = "Sim",
                CloseButtonText = "Não"
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    LoadingRing.IsActive = true;
                    var success = await _ollamaService.RemoveModel(model.Name);

                    if (success)
                    {
                        await LoadModels();
                    }
                }
                catch (Exception ex)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Erro",
                        Content = $"Erro ao remover modelo: {ex.Message}",
                        CloseButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                }
                finally
                {
                    LoadingRing.IsActive = false;
                }
            }
        }

        private void ModelsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Implementar
        }
    }

}
