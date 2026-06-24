using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CipherLib.ConstVal;
using CipherLib.Practical;
using CipherLib.Service;
using Microsoft.Win32;

namespace CipherDesktop;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<OperationHistoryEntry> _history = new();
    private readonly PracticalCipherFacade _facade = new();
    private CipherWorkflow? _workflow;
    private string _currentConfiguration = "";

    public MainWindow()
    {
        InitializeComponent();
        HistoryDataGrid.ItemsSource = _history;
        DesktopDebugLogger.Info($"Desktop application started. Debug log: {DesktopDebugLogger.FilePath}");
        ApplyConfiguration();
        UpdateCipherInfo();
        UpdateModeVisibility();
    }

    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BuilderOptionsPanel == null)
        {
            return;
        }

        UpdateModeVisibility();
        DesktopDebugLogger.Info($"Mode changed: {GetModeDisplayName()}");
    }

    private void CipherComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (KeyLabel == null)
        {
            return;
        }

        KeyLabel.Content = IsAesSelected() ? "Password" : "Key";
        UpdateCipherInfo();
        DesktopDebugLogger.Info($"Algorithm changed: {GetCipherDisplayName()}");
    }

    private void ApplyConfigurationButton_Click(object sender, RoutedEventArgs e)
    {
        DesktopDebugLogger.Info("Apply button clicked.");
        ApplyConfiguration();
    }

    private void EncryptButton_Click(object sender, RoutedEventArgs e)
    {
        DesktopDebugLogger.Info("Encrypt button clicked.");
        RunCipherOperation(isEncryption: true);
    }

    private void DecryptButton_Click(object sender, RoutedEventArgs e)
    {
        DesktopDebugLogger.Info("Decrypt button clicked.");
        RunCipherOperation(isEncryption: false);
    }

    private void VerifyButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ITextCipherStrategy strategy = CreateStrategy();
            string input = InputTextBox.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Text cannot be empty.");
            }

            string encrypted = strategy.Encrypt(input);
            string decrypted = strategy.Decrypt(encrypted);
            bool ok = decrypted == input;
            ResultTextBox.Text = encrypted;
            DetectedAlphabetTextBlock.Text = strategy.LastDetails;
            RoundTripTextBlock.Text = ok ? "OK" : "Failed";
            SetStatus(ok ? "Round-trip verification passed." : "Round-trip verification failed.");
        }
        catch (Exception ex)
        {
            DesktopDebugLogger.Error($"Verify failed. Snapshot: {GetConfigurationSnapshot()}; Input='{InputTextBox.Text}'", ex);
            MessageBox.Show(ex.Message, "Verify error", MessageBoxButton.OK, MessageBoxImage.Error);
            SetStatus("Verification failed.");
        }
    }

    private void UseResultButton_Click(object sender, RoutedEventArgs e)
    {
        DesktopDebugLogger.Info($"Use Result as Input clicked. ResultLength={ResultTextBox.Text.Length}");
        InputTextBox.Text = ResultTextBox.Text;
        SetStatus("Result copied to input.");
    }

    private void SwapButton_Click(object sender, RoutedEventArgs e)
    {
        (InputTextBox.Text, ResultTextBox.Text) = (ResultTextBox.Text, InputTextBox.Text);
        SetStatus("Input and result swapped.");
    }

    private void CopyResultButton_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(ResultTextBox.Text);
        SetStatus("Result copied to clipboard.");
    }

    private void PasteInputButton_Click(object sender, RoutedEventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            InputTextBox.Text = Clipboard.GetText();
            SetStatus("Clipboard text pasted into input.");
        }
    }

    private void OpenTextFileButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog(this) == true)
        {
            InputTextBox.Text = File.ReadAllText(dialog.FileName);
            SetStatus($"Loaded file: {dialog.FileName}");
        }
    }

    private void SaveResultButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = "cipher-result.txt"
        };

        if (dialog.ShowDialog(this) == true)
        {
            File.WriteAllText(dialog.FileName, ResultTextBox.Text);
            SetStatus($"Saved result: {dialog.FileName}");
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        DesktopDebugLogger.Info("Clear button clicked.");
        InputTextBox.Clear();
        ResultTextBox.Clear();
        RoundTripTextBlock.Text = "Not checked";
        SetStatus("Input and result cleared.");
    }

    private void RepeatSelectedHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (HistoryDataGrid.SelectedItem is not OperationHistoryEntry entry)
        {
            return;
        }

        InputTextBox.Text = entry.Input;
        ResultTextBox.Text = entry.Output;
        DetectedAlphabetTextBlock.Text = entry.AlphabetVariant;
        RoundTripTextBlock.Text = entry.RoundTripOk ? "OK" : "Failed";
        SetStatus("Selected history entry loaded.");
    }

    private void CopyHistoryOutputButton_Click(object sender, RoutedEventArgs e)
    {
        if (HistoryDataGrid.SelectedItem is OperationHistoryEntry entry)
        {
            Clipboard.SetText(entry.Output);
            SetStatus("History output copied.");
        }
    }

    private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        _history.Clear();
        SetStatus("History cleared.");
    }

    private void CipherInfoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateCipherInfo();
    }

    private bool ApplyConfiguration()
    {
        try
        {
            DesktopDebugLogger.Info($"Applying configuration: {GetConfigurationSnapshot()}");
            _workflow = IsAesSelected() ? null : new CipherWorkflow(CreateWorkflowOptions());
            _currentConfiguration = GetConfigurationSignature();

            SetStatus($"Configured {GetCipherDisplayName()} in {GetModeDisplayName()} mode.");
            DesktopDebugLogger.Info("Configuration applied successfully.");
            return true;
        }
        catch (Exception ex)
        {
            DesktopDebugLogger.Error($"Configuration failed. Snapshot: {GetConfigurationSnapshot()}", ex);
            MessageBox.Show(ex.Message, "Configuration error", MessageBoxButton.OK, MessageBoxImage.Error);
            SetStatus("Configuration failed.");
            return false;
        }
    }

    private void RunCipherOperation(bool isEncryption)
    {
        try
        {
            if (!EnsureService())
            {
                return;
            }

            string input = InputTextBox.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Text cannot be empty.");
            }

            ITextCipherStrategy strategy = CreateStrategy();
            string operation = isEncryption ? "Encrypt" : "Decrypt";
            var command = new CipherOperationCommand(operation, () => isEncryption ? strategy.Encrypt(input) : strategy.Decrypt(input));

            DesktopDebugLogger.Info($"Operation started: Operation={operation}; {GetConfigurationSnapshot()}; Input='{input}'");
            OperationHistoryEntry entry = _facade.Execute(strategy, command, BuildKeyPreview(), GetModeDisplayName(), input);

            ResultTextBox.Text = entry.Output;
            DetectedAlphabetTextBlock.Text = entry.AlphabetVariant;
            RoundTripTextBlock.Text = entry.RoundTripOk ? "OK" : "Failed";
            _history.Insert(0, entry);

            SetStatus(isEncryption ? "Text encrypted." : "Text decrypted.");
            DesktopDebugLogger.Info($"Operation completed: Operation={operation}; Output='{entry.Output}'");
        }
        catch (Exception ex)
        {
            DesktopDebugLogger.Error($"Operation failed: Operation={(isEncryption ? "Encrypt" : "Decrypt")}; {GetConfigurationSnapshot()}; Input='{InputTextBox.Text}'", ex);
            MessageBox.Show(ex.Message, "Cipher error", MessageBoxButton.OK, MessageBoxImage.Error);
            SetStatus("Operation failed.");
        }
    }

    private bool EnsureService()
    {
        if (IsAesSelected())
        {
            return true;
        }

        if (_workflow == null || GetConfigurationSignature() != _currentConfiguration)
        {
            return ApplyConfiguration();
        }

        return true;
    }

    private ITextCipherStrategy CreateStrategy()
    {
        if (IsAesSelected())
        {
            return new AesGcmTextCipherStrategy(KeyTextBox.Text);
        }

        return new ClassicalCipherStrategy(GetCipherDisplayName(), CreateWorkflowOptions());
    }

    private CipherWorkflowOptions CreateWorkflowOptions()
    {
        return new CipherWorkflowOptions
        {
            Mode = GetSelectedMode(),
            CipherType = GetCipherType(),
            Key = KeyTextBox.Text.Trim(),
            Salt = SaltTextBox.Text,
            AlphabetVariant = GetAlphabetVariant(),
            ErrorLogging = ErrorLoggingCheckBox.IsChecked == true,
            ProcessLogging = ProcessLoggingCheckBox.IsChecked == true
        };
    }

    private CipherWorkflowMode GetSelectedMode()
    {
        return ModeComboBox.SelectedIndex switch
        {
            2 => CipherWorkflowMode.BuilderDefault,
            3 => CipherWorkflowMode.BuilderManual,
            4 => CipherWorkflowMode.AbstractFactory,
            _ => CipherWorkflowMode.Factory
        };
    }

    private CipherType GetCipherType()
    {
        return GetCipherTag() switch
        {
            "vigenere" => CipherType.Vigenere,
            "beaufort" => CipherType.Beaufort,
            "autokey" => CipherType.AutoKey,
            "runningkey" => CipherType.RunningKey,
            _ => CipherType.Vigenere
        };
    }

    private bool IsAesSelected()
    {
        return GetCipherTag() == "aesgcm";
    }

    private string GetCipherTag()
    {
        return ((ComboBoxItem)CipherComboBox.SelectedItem).Tag?.ToString() ?? "vigenere";
    }

    private string GetCipherDisplayName()
    {
        return ((ComboBoxItem)CipherComboBox.SelectedItem).Content?.ToString() ?? "Vigenere";
    }

    private string GetModeDisplayName()
    {
        return ((ComboBoxItem)ModeComboBox.SelectedItem).Content?.ToString() ?? "Practical";
    }

    private static string GetSelectedComboText(ComboBox comboBox)
    {
        return ((ComboBoxItem)comboBox.SelectedItem).Content?.ToString() ?? "";
    }

    private string GetAlphabetVariant()
    {
        return IsAesSelected() ? "AES-GCM" : GetSelectedComboText(AlphabetVariantComboBox);
    }

    private string GetConfigurationSignature()
    {
        return string.Join("|",
            GetSelectedMode(),
            GetCipherTag(),
            KeyTextBox.Text,
            SaltTextBox.Text,
            GetAlphabetVariant(),
            ErrorLoggingCheckBox.IsChecked == true,
            ProcessLoggingCheckBox.IsChecked == true);
    }

    private string GetConfigurationSnapshot()
    {
        return $"Mode='{GetModeDisplayName()}'; Cipher='{GetCipherDisplayName()}'; Key='{KeyTextBox.Text}'; Salt='{SaltTextBox.Text}'; AlphabetVariant='{GetAlphabetVariant()}'; ErrorLogging={ErrorLoggingCheckBox.IsChecked == true}; ProcessLogging={ProcessLoggingCheckBox.IsChecked == true}";
    }

    private string BuildKeyPreview()
    {
        string key = KeyTextBox.Text;
        if (key.Length <= 4)
        {
            return new string('*', key.Length);
        }

        return $"{key[..2]}...{key[^2..]}";
    }

    private void UpdateModeVisibility()
    {
        BuilderOptionsPanel.Visibility = GetSelectedMode() == CipherWorkflowMode.BuilderManual
            ? Visibility.Visible
            : Visibility.Collapsed;

        AlphabetVariantComboBox.IsEnabled = !IsAesSelected() && GetSelectedMode() != CipherWorkflowMode.BuilderDefault;
    }

    private void UpdateCipherInfo()
    {
        if (CipherInfoTextBox == null)
        {
            return;
        }

        string selected = CipherInfoListBox?.SelectedItem is ListBoxItem item
            ? item.Content?.ToString() ?? GetCipherDisplayName()
            : GetCipherDisplayName();

        CipherInfoTextBox.Text = selected switch
        {
            "Vigenere" => "Vigenere is a classical polyalphabetic substitution cipher. It repeats the key across the input and shifts each character inside the selected alphabet.",
            "Beaufort" => "Beaufort is a reciprocal classical cipher: encryption and decryption use the same transformation. It is useful for comparing cipher behavior with Vigenere.",
            "AutoKey" => "AutoKey starts with a secret key and then extends the key stream with plaintext. It demonstrates stateful key expansion.",
            "RunningKey" => "RunningKey uses a key that must be at least as long as the text. In this project it is useful for demonstrating stricter validation rules.",
            "AES-GCM" => "AES-GCM is the practical modern mode in this app. The password is converted to a 256-bit key with PBKDF2-SHA256, then AES-GCM encrypts and authenticates the text.",
            "Design patterns" => "Patterns in use: Factory Method creates classical ciphers, Builder configures custom alphabets, Abstract Factory creates cipher component families, Singleton handles logging, Prototype clones sessions, Strategy selects classical/AES behavior, Command represents operations, and Facade coordinates practical execution/history.",
            _ => ""
        };
    }

    private void SetStatus(string message)
    {
        StatusTextBlock.Text = message;
        DesktopDebugLogger.Info($"Status changed: {message}");
    }
}
