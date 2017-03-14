namespace PopcornApi.Services.Caching
{
    /// <summary>
    /// Caching service
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// Cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        void SetCache(string key, string value);

        /// <summary>
        /// Cache
        /// </summary>
        /// <param name="key">Key</param>
        string GetCache(string key);
    }
}
