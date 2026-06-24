using System;
using System.Windows;
using System.Windows.Controls;
using CipherLib.ConstVal;
using CipherLib.Service;

namespace CipherDesktop;

public partial class MainWindow : Window
{
    private CipherWorkflow? _workflow;
    private string _currentConfiguration = "";

    public MainWindow()
    {
        InitializeComponent();
        DesktopDebugLogger.Info($"Desktop application started. Debug log: {DesktopDebugLogger.FilePath}");
        ApplyConfiguration();
    }

    private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BuilderOptionsPanel == null)
        {
            return;
        }

        BuilderOptionsPanel.Visibility = GetSelectedMode() == CipherWorkflowMode.BuilderManual
            ? Visibility.Visible
            : Visibility.Collapsed;

        DesktopDebugLogger.Info($"Mode changed: {GetModeDisplayName()}");
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

    private void UseResultButton_Click(object sender, RoutedEventArgs e)
    {
        DesktopDebugLogger.Info($"Use Result as Input clicked. ResultLength={ResultTextBox.Text.Length}");
        InputTextBox.Text = ResultTextBox.Text;
        SetStatus("Result copied to input.");
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        DesktopDebugLogger.Info("Clear button clicked.");
        InputTextBox.Clear();
        ResultTextBox.Clear();
        SetStatus("Input and result cleared.");
    }

    private bool ApplyConfiguration()
    {
        try
        {
            DesktopDebugLogger.Info($"Applying configuration: {GetConfigurationSnapshot()}");
            _workflow = new CipherWorkflow(CreateWorkflowOptions());
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

            DesktopDebugLogger.Info($"Operation started: Operation={(isEncryption ? "Encrypt" : "Decrypt")}; {GetConfigurationSnapshot()}; Input='{input}'");

            string output = isEncryption
                ? _workflow!.Encrypt(input)
                : _workflow!.Decrypt(input);

            ResultTextBox.Text = output;

            SetStatus(isEncryption ? "Text encrypted." : "Text decrypted.");
            DesktopDebugLogger.Info($"Operation completed: Operation={(isEncryption ? "Encrypt" : "Decrypt")}; Output='{output}'");
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
        if (_workflow == null || GetConfigurationSignature() != _currentConfiguration)
        {
            return ApplyConfiguration();
        }

        return true;
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
            1 => CipherWorkflowMode.BuilderDefault,
            2 => CipherWorkflowMode.BuilderManual,
            3 => CipherWorkflowMode.AbstractFactory,
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
            _ => throw new ArgumentException("Unknown cipher type.")
        };
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
        return ((ComboBoxItem)ModeComboBox.SelectedItem).Content?.ToString() ?? "Factory";
    }

    private static string GetSelectedComboText(ComboBox comboBox)
    {
        return ((ComboBoxItem)comboBox.SelectedItem).Content?.ToString() ?? "";
    }

    private string GetAlphabetVariant()
    {
        return GetSelectedComboText(AlphabetVariantComboBox);
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

    private void SetStatus(string message)
    {
        StatusTextBlock.Text = message;
        DesktopDebugLogger.Info($"Status changed: {message}");
    }
}
