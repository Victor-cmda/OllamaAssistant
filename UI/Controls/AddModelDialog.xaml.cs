using Core.Entities;
using Core.Interfaces;
using Core.Records;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UI.Controls
{
    public partial class AddModelDialog : UserControl
    {
        private readonly IOllamaService _ollamaService;
        private readonly List<ModelSuggestion> _suggestions;
        public string SelectedModel => ModelNameBox.Text;

        public AddModelDialog(IOllamaService ollamaService)
        {
            InitializeComponent();
            _ollamaService = ollamaService;

            _suggestions = new List<ModelSuggestion>
            {
                new("llama2", "Modelo versátil para tarefas gerais"),
                new("llama2:13b", "Versão maior do Llama2, mais precisa"),
                new("codellama", "Especializado em código"),
                new("mistral", "Eficiente e balanceado"),
                new("neural-chat", "Otimizado para conversas"),
                new("starling-lm", "Bom para diálogos naturais"),
                new("dolphin-phi", "Leve e eficiente"),
                new("orca-mini", "Versão compacta, recursos limitados")
            };

            PopularModels.ItemsSource = _suggestions;

            DownloadProgress.Visibility = Visibility.Collapsed;
            StatusText.Visibility = Visibility.Collapsed;
        }

        private void ModelSuggestion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ModelSuggestion suggestion)
            {
                ModelNameBox.Text = suggestion.Name;
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ModelNameBox.Text))
            {
                StatusText.Text = "Por favor, selecione um modelo";
                StatusText.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                DownloadButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
                ModelNameBox.IsEnabled = false;
                DownloadProgress.Visibility = Visibility.Visible;
                StatusText.Visibility = Visibility.Visible;
                DownloadProgress.Value = 0;

                var progress = new Progress<PullProgress>(status =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        DownloadProgress.Value = status.ProgressPercentage;
                        StatusText.Text = $"Baixando: {status.DownloadedSize} / {status.TotalSize}";
                    });
                });

                var result = await _ollamaService.PullModel(ModelNameBox.Text, progress);

                if (result.Success)
                {
                    StatusText.Text = "Download concluído com sucesso!";
                    await Task.Delay(1000);

                    if (this.Parent is ContentDialog dialog)
                    {
                        dialog.Hide();
                    }
                }
                else
                {
                    StatusText.Text = "Falha ao baixar o modelo";
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Erro: {ex.Message}";
            }
            finally
            {
                DownloadButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
                ModelNameBox.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is ContentDialog dialog)
            {
                dialog.Hide();
            }
        }

        private void ViewLibrary_Click(object sender, RoutedEventArgs e)
        {
            // Implementar
        }
    }
}
