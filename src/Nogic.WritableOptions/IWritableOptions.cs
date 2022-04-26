using Microsoft.Extensions.Options;

namespace Nogic.WritableOptions;

/// <summary>
/// Used to access or update the value of <typeparamref name="TOptions"/>.
/// </summary>
/// <typeparam name="TOptions">Options type.</typeparam>
public interface IWritableOptions<TOptions> : IOptionsSnapshot<TOptions>, IOptionsMonitor<TOptions> where TOptions : class, new()
{
    /// <summary>
    /// Update current <typeparamref name="TOptions"/> value.
    /// </summary>
    /// <param name="changedValue">value that you want to update.</param>
    /// <param name="reload">Reload all configuration immediately or not.</param>
    void Update(TOptions changedValue, bool reload = false);
}
