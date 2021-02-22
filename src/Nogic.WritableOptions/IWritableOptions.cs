using Microsoft.Extensions.Options;

namespace Nogic.WritableOptions
{
    public interface IWritableOptions<TOptions> : IOptionsSnapshot<TOptions> where TOptions : class, new()
    {
        void Update(TOptions changedValue);
    }
}
