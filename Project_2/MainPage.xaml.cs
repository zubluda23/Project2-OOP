using System;
using System.Collections.Generic;
using System.IO;
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

        AutoLoadDefaultXml();
    }

    private void AutoLoadDefaultXml()
    {
        try
        {
            string defaultFile = Path.Combine(AppContext.BaseDirectory, "conferences.xml");

            if (File.Exists(defaultFile))
            {
                loadedXmlPath = defaultFile;
                lblFilePath.Text = $"Завантажено: {Path.GetFileName(loadedXmlPath)}";
                PopulatePickers(loadedXmlPath);
            }
            else
            {
                lblFilePath.Text = "Натисніть кнопку вище та оберіть XML-файл";
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

        var fieldList = new List<string> { "Всі тематики" };
        fieldList.AddRange(fields);

        var orgList = new List<string> { "Всі організатори" };
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
                PickerTitle = "Оберіть XML-файл конференцій",
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
            DisplayAlert("Увага", "Спочатку оберіть XML-файл!", "OK");
            return;
        }

        
        manager.SetStrategyBySelection(rbSax.IsChecked, rbDom.IsChecked);

        string selectedField = (pickerField.SelectedIndex <= 0) ? string.Empty : pickerField.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        string selectedOrg = (pickerOrganizer.SelectedIndex <= 0) ? string.Empty : pickerOrganizer.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        string keyword = txtKeyword.Text?.Trim() ?? string.Empty;

        var criteria = new FilterCriteria(selectedField, selectedOrg, keyword);

   
        txtResults.Text = manager.FormatSearchResults(loadedXmlPath, criteria);
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
            bool isSuccess = manager.ExportToHtml(loadedXmlPath, out string htmlPath);
            if (isSuccess)
            {
                await DisplayAlert("Успіх", $"HTML-звіт успішно створено:\n{htmlPath}", "OK");
            }
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