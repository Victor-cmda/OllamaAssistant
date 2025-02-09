using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.Json;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private readonly IOllamaService _ollamaService;

        public HomePage()
        {
            this.InitializeComponent();
            _ollamaService = new OllamaService();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckOllamaStatus();
        }

        private async Task CheckOllamaStatus()
        {
            try
            {
                LoadingRing.IsActive = true;
                StatusInfoBar.IsOpen = false;

                var status = await _ollamaService.CheckOllamaStatus();

                StatusInfoBar.IsOpen = true;

                if (status.IsAvailable)
                {
                    StatusInfoBar.Severity = InfoBarSeverity.Success;
                    var jsonDocument = JsonDocument.Parse(status.Version);
                    var version = jsonDocument.RootElement.GetProperty("version").GetString();
                    StatusInfoBar.Message = $"Ollama está disponível. Versão: {version}";
                }
                else
                {
                    StatusInfoBar.Severity = InfoBarSeverity.Error;
                    StatusInfoBar.Message = $"Ollama não está disponível. Erro: {status.Error}";
                }
            }
            catch (Exception ex)
            {
                StatusInfoBar.IsOpen = true;
                StatusInfoBar.Severity = InfoBarSeverity.Error;
                StatusInfoBar.Message = $"Erro ao verificar status: {ex.Message}";
            }
            finally
            {
                LoadingRing.IsActive = false;
            }
        }
    }
}
