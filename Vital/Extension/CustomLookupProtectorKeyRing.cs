using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Vital.Configuration;

namespace Vital.Extension
{
    // Defining a custom Lookup Protector Key Ring
    public class CustomLookupProtectorKeyRing : ILookupProtectorKeyRing
    {
        private readonly IOptions<KeyRingSettings> _keyRingSettings;
        public CustomLookupProtectorKeyRing(IOptions<KeyRingSettings> keyRingSettings)
        {
            _keyRingSettings = keyRingSettings;
        }
        // Indexer to get the key associated with the provided keyId
        public string this[string keyId]
        {
            get
            {
                // Return first keyId from all keys that matches input keyId
                return GetAllKeyIds().Where(x => x == keyId).FirstOrDefault();
            }
        }

        // Property to get current Key Id
        public string CurrentKeyId
        {
            get
            {
                // Define a key
                string keyString = _keyRingSettings.Value.Key;
                byte[] key = Convert.FromBase64String(keyString);

                // Convert key to string format and return
                var currentKey = Convert.ToBase64String(key);
                return currentKey;
            }
        }

        // Method to get all Key Ids
        public IEnumerable<string> GetAllKeyIds()
        {
            // Create a list to store Key Ids
            var list = new List<string>();
            
            // Defining 16 bytes length keys
            byte[] key = Convert.FromBase64String(_keyRingSettings.Value.R);
            byte[] key2 = Convert.FromBase64String(_keyRingSettings.Value.G);
            byte[] key3 = Convert.FromBase64String(_keyRingSettings.Value.B); 

            // Convert each key to string and add it to the list
            list.Add(Convert.ToBase64String(key));
            list.Add(Convert.ToBase64String(key2));
            list.Add(Convert.ToBase64String(key3));

            // Return the list of Key Ids
            return list;
        }
    }
}
