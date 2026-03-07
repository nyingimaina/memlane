namespace Memlane.Api.Providers
{
    public interface ICompressionProviderFactory
    {
        ICompressionProvider GetProvider(string? name);
    }

    public class CompressionProviderFactory : ICompressionProviderFactory
    {
        private readonly IEnumerable<ICompressionProvider> _providers;

        public CompressionProviderFactory(IEnumerable<ICompressionProvider> providers)
        {
            _providers = providers;
        }

        public ICompressionProvider GetProvider(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                // Default to Zip for v1.0 compatibility if not specified
                return _providers.First(p => p.ProviderName == "Zip");
            }

            var provider = _providers.FirstOrDefault(p => p.ProviderName.Equals(name, StringComparison.OrdinalIgnoreCase));
            return provider ?? _providers.First(p => p.ProviderName == "Zip");
        }
    }
}
