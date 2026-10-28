using OpenAC.Net.Core.Extensions;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OpenAC.Net.NFSe.Nacional.Demo;

public class OpenConfig
{
    #region Fields

    private readonly Configuration config;
    private const string Key = @"a1!B78s!5(h23y1g12\t\98w";

    #endregion Fields

    #region Constructors

    private OpenConfig(Configuration config)
    {
        this.config = config;
    }

    #endregion Constructors

    #region Methods

    public void Set(string setting, object value)
    {
        var valor = string.Format(CultureInfo.InvariantCulture, "{0}", value);

        if (config.AppSettings.Settings[setting]?.Value != null)
            config.AppSettings.Settings[setting].Value = valor;
        else
            config.AppSettings.Settings.Add(setting, valor);
    }

    public void SetCrypt(string setting, string value)
    {
        var valor = !value.IsEmpty() ? value.Encrypt(Key) : value;

        if (config.AppSettings.Settings[setting]?.Value != null)
            config.AppSettings.Settings[setting].Value = valor;
        else
            config.AppSettings.Settings.Add(setting, valor);
    }

    public T Get<T>(string setting, T defaultValue)
    {
        var type = typeof(T);
        var value = config.AppSettings.Settings[setting]?.Value;
        if (string.IsNullOrEmpty(value)) return defaultValue;

        try
        {
            if (type.IsEnum || type.IsGenericType && type.GetGenericArguments()[0].IsEnum)
            {
                return (T)Enum.Parse(type, value);
            }

            return (T)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    public string GetCrypt(string setting, string defaultValue)
    {
        var value = config.AppSettings.Settings[setting]?.Value;
        if (string.IsNullOrEmpty(value)) return defaultValue;

        // Mantém o formato legado, lendo todos os blocos em vez de um único CryptoStream.Read.
        using var cipher = TripleDES.Create();
        var key = Rfc2898DeriveBytes.Pbkdf2(Key, Encoding.UTF8.GetBytes("aselrias38490a32"), 2, HashAlgorithmName.SHA1, 16);
        using var decryptor = cipher.CreateDecryptor(key, Encoding.UTF8.GetBytes("8947az34"));
        var encrypted = Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length));
    }

    public void Save()
    {
        config.Save(ConfigurationSaveMode.Minimal, true);
    }

    public static OpenConfig CreateOrLoad(string fileName = "openac.config")
    {
        if (!File.Exists(fileName))
        {
            var config = "<?xml version='1.0' encoding='utf-8' ?>" + Environment.NewLine +
                         "<configuration>" + Environment.NewLine +
                         "    <appSettings>" + Environment.NewLine +
                         "    </appSettings>" + Environment.NewLine +
                         "</configuration>";
            File.WriteAllText(fileName, config);
        }

        var configFileMap = new ExeConfigurationFileMap
        {
            ExeConfigFilename = fileName
        };

        var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        return new OpenConfig(configuration);
    }

    #endregion Methods
}
