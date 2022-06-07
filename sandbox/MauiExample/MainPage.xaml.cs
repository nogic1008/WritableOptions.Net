using System.ComponentModel;
using Nogic.WritableOptions;

namespace MauiExample;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private readonly IWritableOptions<AppOption> _options;

    public static readonly BindableProperty LastChangedProperty = BindableProperty.Create(nameof(LastChanged), typeof(string), typeof(MainPage));
    public string LastChanged
    {
        get => (string)GetValue(LastChangedProperty);
        set => SetValue(LastChangedProperty, value);
    }

    public static readonly BindableProperty ApiKeyProperty = BindableProperty.Create(nameof(ApiKey), typeof(string), typeof(MainPage));
    public string ApiKey
    {
        get => (string)GetValue(ApiKeyProperty);
        set => SetValue(ApiKeyProperty, value);
    }

    public MainPage(IWritableOptions<AppOption> options)
    {
        ArgumentException.ThrowIfNull(options);
        _options = options;
        _options.OnChange(UpdateViewFromOptions);
        InitializeComponent();
        UpdateViewFromOptions(_options.CurrentValue, string.Empty);
    }

    private void UpdateViewFromOptions(AppOption option, string section)
    {
        LastChanged = option.LastChanged.ToString();
        ApiKey = option.ApiKey ?? "";
        OnBindingContextChanged();
    }

    private void OnClicked(object sender, EventArgs e)
    {
        var current = _options.CurrentValue;
        current.ApiKey = Guid.NewGuid().ToString();
        current.LastChanged = DateTime.Now;
        _options.Update(current, true);
    }
}
