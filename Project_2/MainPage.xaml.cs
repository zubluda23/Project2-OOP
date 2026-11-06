using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Maui.Controls;

namespace Project_2;

public partial class MainPage : ContentPage
{
    private readonly ConferenceManager manager;
    private string loadedXmlPath = string.Empty;

    public MainPage()
    {
        InitializeComponent();
        manager = new ConferenceManager();

        // Автоматично шукаємо conferences.xml поруч із програмою при відкритті вікна
        AutoLoadDefaultXml();
    }

    private void AutoLoadDefaultXml()
    {
        try
        {
            // Шукаємо файл у папці запуску або на рівень вище в проєкті
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string localFile = Path.Combine(baseDir, "conferences.xml");

            // Якщо файл не скопіювався в bin, шукаємо в корені вихідного коду проєкту
            if (!File.Exists(localFile))
            {
                string projectDir = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.FullName ?? "";
                string sourceFile = Path.Combine(projectDir, "conferences.xml");
                if (File.Exists(sourceFile))
                    localFile = sourceFile;
            }

            if (File.Exists(localFile))
            {
                loadedXmlPath = localFile;
                lblFilePath.Text = $"Завантажено автоматично: {Path.GetFileName(loadedXmlPath)}";
                PopulatePickers(loadedXmlPath);
            }
            else
            {
                lblFilePath.Text = "Натисніть кнопку вище та вкажіть шлях до conferences.xml";
            }
        }
        catch (Exception ex)
        {
            lblFilePath.Text = $"Помилка автозавантаження: {ex.Message}";
        }
    }

    private void PopulatePickers(string path)
    {
        var (fields, organizers) = manager.LoadAttributes(path);

        // Додаємо пункт скидання фільтра
        var fieldList = new List<string> { "-- Всі тематики --" };
        fieldList.AddRange(fields);

        var orgList = new List<string> { "-- Всі організатори --" };
        orgList.AddRange(organizers);

        pickerField.ItemsSource = fieldList;
        pickerOrganizer.ItemsSource = orgList;

        pickerField.SelectedIndex = 0;
        pickerOrganizer.SelectedIndex = 0;
    }

    public async void OnFileSelected(object? sender, EventArgs e)
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Оберіть файл conferences.xml",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xml" } },
                    { DevicePlatform.Android, new[] { "text/xml", "application/xml" } },
                    { DevicePlatform.iOS, new[] { "public.xml" } }
                })
            };

            var file = await FilePicker.Default.PickAsync(options);
            if (file != null)
            {
                loadedXmlPath = file.FullPath;
                lblFilePath.Text = $"Обрано: {loadedXmlPath}";
                PopulatePickers(loadedXmlPath);

                await DisplayAlert("Успіх", "Файл успішно завантажено, фільтри оновлено!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка вибору файлу", ex.Message, "OK");
        }
    }

    public void OnSearchClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(loadedXmlPath) || !File.Exists(loadedXmlPath))
        {
            DisplayAlert("Увага", "Файл conferences.xml не знайдено! Натисніть «Обрати XML-файл».", "OK");
            return;
        }

        // Обираємо патерн-стратегію
        if (rbSax.IsChecked)
            manager.SetStrategy(new SaxParserStrategy());
        else if (rbDom.IsChecked)
            manager.SetStrategy(new DomParserStrategy());
        else
            manager.SetStrategy(new LinqToXmlParserStrategy());

        // Отримуємо значення з інтерфейсу
        string? rawField = pickerField.SelectedItem?.ToString();
        string selectedField = (rawField == null || rawField.StartsWith("--")) ? string.Empty : rawField.Trim();

        string? rawOrg = pickerOrganizer.SelectedItem?.ToString();
        string selectedOrg = (rawOrg == null || rawOrg.StartsWith("--")) ? string.Empty : rawOrg.Trim();

        string keyword = txtKeyword.Text?.Trim() ?? string.Empty;

        var criteria = new FilterCriteria(selectedField, selectedOrg, keyword);
        var results = manager.FilterConferences(loadedXmlPath, criteria);

        if (results.Count == 0)
        {
            txtResults.Text = "За заданими критеріями конференцій не знайдено.";
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Знайдено результатів: {results.Count}\n");
        foreach (var item in results)
        {
            sb.AppendLine(item.GetInfo());
            sb.AppendLine(new string('-', 70));
        }

        txtResults.Text = sb.ToString();
    }

    public async void OnTransformClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(loadedXmlPath) || !File.Exists(loadedXmlPath))
        {
            await DisplayAlert("Увага", "Спочатку оберіть XML-файл!", "OK");
            return;
        }

        try
        {
            string folder = Path.GetDirectoryName(loadedXmlPath) ?? AppDomain.CurrentDomain.BaseDirectory;
            string xslPath = Path.Combine(folder, "transform.xsl");
            string htmlPath = Path.Combine(folder, "ConferencesReport.html");

            if (!File.Exists(xslPath))
            {
                await DisplayAlert("Помилка", $"Не знайдено transform.xsl поруч із файлом XML:\n{xslPath}", "OK");
                return;
            }

            manager.ExportToHtml(loadedXmlPath, xslPath, htmlPath);
            await DisplayAlert("Успіх", $"HTML-звіт успішно створено:\n{htmlPath}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка трансформації", ex.Message, "OK");
        }
    }

    public void OnClearClicked(object? sender, EventArgs e)
    {
        if (pickerField.ItemsSource != null) pickerField.SelectedIndex = 0;
        if (pickerOrganizer.ItemsSource != null) pickerOrganizer.SelectedIndex = 0;
        txtKeyword.Text = string.Empty;
        txtResults.Text = string.Empty;
        rbSax.IsChecked = true;
    }

    public async void OnExitRequested(object? sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
        if (answer)
        {
            Application.Current?.Quit();
        }
    }
}