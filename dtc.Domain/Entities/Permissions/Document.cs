
namespace dtc.Domain.Entities.Permissions
{
    public class Document : BaseEntity
    {
        public Guid UserId { get; private set; }
        
        // Cloudinary specific fields
        public string ProviderPublicId { get; private set; } = string.Empty; // e.g. "user_docs/id_123"
        public string Version { get; private set; } = string.Empty;          // e.g. "167890123"
        public string ResourceType { get; private set; } = string.Empty;     // "image", "raw", or "video"
        
        public string FileName { get; private set; } = string.Empty;         // Original name (e.g. "cccd.pdf")
        public string Extension { get; private set; } = string.Empty;        // .pdf, .jpg, .png
        public int Size { get; private set; }               // Byte
        
        public bool IsVerified { get; private set; }

        protected Document() { }

        public Document(
            Guid userId,
            string providerPublicId,
            string version,
            string resourceType,
            string fileName,
            string extension,
            int size)
        {
            SetOwner(userId);
            SetFile(providerPublicId, version, resourceType, fileName, extension, size);
            IsVerified = false;
        }

        // ========================
        // Behaviors
        // ========================

        public void SetOwner(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty");

            UserId = userId;
        }

        public void SetFile(
            string publicId, 
            string version, 
            string resourceType, 
            string name, 
            string extension, 
            int size)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("ProviderPublicId is required");

            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Version is required");

            if (string.IsNullOrWhiteSpace(resourceType))
                throw new ArgumentException("ResourceType is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("FileName is required");

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Extension is required");

            if (size <= 0)
                throw new ArgumentException("Size must be greater than 0");

            ProviderPublicId = publicId;
            Version = version;
            ResourceType = resourceType;
            FileName = name;
            Extension = extension;
            Size = size;
        }

        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("FileName cannot be empty");

            FileName = newName;
        }

        public void ChangeFile(
            string publicId, 
            string version, 
            string resourceType, 
            string extension, 
            int size)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("ProviderPublicId is required");

            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Version is required");

            if (string.IsNullOrWhiteSpace(resourceType))
                throw new ArgumentException("ResourceType is required");

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Extension is required");

            if (size <= 0)
                throw new ArgumentException("Size must be greater than 0");

            ProviderPublicId = publicId;
            Version = version;
            ResourceType = resourceType;
            Extension = extension;
            Size = size;

            IsVerified = false;
        }

        public void Verify()
        {
            if (IsVerified) return;
            IsVerified = true;
        }

        public void Unverify()
        {
            if (!IsVerified) return;
            IsVerified = false;
        }
    }
}
