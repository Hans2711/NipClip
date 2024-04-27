using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml.Serialization;

namespace NipClip.Classes.Clipboard
{
    [Serializable]
    [XmlInclude(typeof(StringClipboardEntry))]
    public abstract class ClipboardEntry : INotifyPropertyChanged
    {
        public virtual string type { get ; set; }

        public virtual DateTime crDate { get; set; }

        [XmlIgnore]
        public virtual bool toBeDeleted { get; set; }

        [XmlIgnore]
        public virtual float sortWeight { get; set; }

        public virtual string SortWeight
        {
            get
            {
                return this.sortWeight.ToString();
            }
            set
            {
                if (float.TryParse(value, out float floatValue))
                {
                    this.sortWeight = floatValue;
                }
                else
                {
                    // Handle the case where the string cannot be parsed into a float
                    // You might throw an exception or handle it differently based on your requirements
                }
            }
        }

        public virtual string? ToString()
        {
            return (string)Content;
        }

        public ClipboardEntry()
        {
            crDate = DateTime.Now;
            toBeDeleted = false;
        }

        [XmlIgnore]
        public virtual object Content { 
            get {
                if (WindowManager.applicationSettings.DecryptionEnabled) 
                {
                    return DecryptString(WindowManager.applicationSettings.EncryptionKey);
                } else
                {
                    return ByteToString(EncryptedContent);
                }
            }
            set
            {
                Encrypt(WindowManager.applicationSettings.EncryptionKey, (string)value);
                NotifyPropertyChanged();
            }
        }

        protected string ByteToString(byte[] bytes, bool isFallback = false)
        {
            if (isFallback)
            {
                this.toBeDeleted = true;
            }

            string response = string.Empty;

            for (int i = 0; i < bytes.Length; i++)
                response += (char)(bytes[i] + (i % 29));  // Using the index 'i' instead of the byte value

            return response;
        }

        public virtual List<ClipboardEntry> Split(string delimiter)
        {
            List<ClipboardEntry> parts = new List<ClipboardEntry>();
            string contentString = Content.ToString();
            var splitContents = contentString.Split(new string[] { delimiter }, StringSplitOptions.None);

            foreach (var part in splitContents)
            {
                var entry = (ClipboardEntry)this.MemberwiseClone();
                entry.Content = part;
                parts.Add(entry);
            }

            return parts;
        }

        public virtual string urlContent { get; set; }

        public virtual bool isUrl()
        {
            string content = ((string)Content).Trim();
            Uri uriResult;
            bool isUrl = Uri.TryCreate(content, UriKind.Absolute, out uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return isUrl;
        }

        public virtual void pasteToClipboard()
        {
            System.Windows.Clipboard.SetDataObject(this.Content);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool isEqual(ClipboardEntry other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.Content == null && other.Content == null)
            {
                return true;
            }

            if (this.Content == null || other.Content == null)
            {
                return false;
            }

            return this.Content.Equals(other.Content);
        }

        public void Reencrypt(string newKey)
        {
            this.Encrypt(newKey, DecryptString(ClipboardReader.encryptionKey));
        }

        public virtual byte[] EncryptedContent { get; set; }

        public virtual void Encrypt(string key, string content)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // Set the key size to 256 bits
                aesAlg.BlockSize = 128; // Set the block size to 128 bits

                // Ensure the key is of correct length
                byte[] keyBytes = new byte[32]; // 256 bits = 32 bytes
                byte[] keyHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
                Array.Copy(keyHash, keyBytes, Math.Min(keyHash.Length, keyBytes.Length));

                aesAlg.Key = keyBytes;
                aesAlg.IV = new byte[16]; // Initialization vector for AES

                // Create an encryptor to perform the stream transform
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Convert the content to bytes and encrypt it
                        byte[] bytesContent = Encoding.UTF8.GetBytes(content.ToString());
                        csEncrypt.Write(bytesContent, 0, bytesContent.Length);
                    }
                    EncryptedContent = msEncrypt.ToArray();
                }
            }

            if (EncryptedContent == null || EncryptedContent.Length == 0)
                EncryptedContent = new byte[] { 0 };
        }

        public virtual string DecryptString(string key)
        {
            if (EncryptedContent == null || EncryptedContent.Length == 0)
                return ByteToString(EncryptedContent, true);

            if (string.IsNullOrEmpty(key))
                return ByteToString(EncryptedContent, true);
        
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // Set the key size to 256 bits
                aesAlg.BlockSize = 128; // Set the block size to 128 bits

                // Ensure the key is of correct length
                byte[] keyBytes = new byte[32]; // 256 bits = 32 bytes
                byte[] keyHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
                Array.Copy(keyHash, keyBytes, Math.Min(keyHash.Length, keyBytes.Length));

                aesAlg.Key = keyBytes;
                aesAlg.IV = new byte[16]; // Initialization vector for AES

                // Create a decryptor to perform the stream transform
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(EncryptedContent))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        try
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream and return as string
                                return srDecrypt.ReadToEnd();
                            }
                        }
                        catch (Exception ex) {
                            this.toBeDeleted = true;
                        }
                    }
                }
            }

            return ByteToString(EncryptedContent, true);
        }
    }
}
