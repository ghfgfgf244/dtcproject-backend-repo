
namespace dtc.Domain.Entities.Permissions
{
    public class Document : BaseEntity
    {
        public Guid UserId { get; private set; }
        public ResourceType ResourceType { get; private set; }
        public string FileUrl { get; private set; }
        public string FileName { get; private set; }
        public string Extension { get; private set; }
        public int Size { get; private set; }
        public bool IsVerified { get; private set; }

        protected Document() { }

        public Document(
            Guid userId,
            ResourceType resourceType,
            string fileUrl,
            string fileName,
            string extension,
            int size)
        {
            SetOwner(userId);
            SetResourceType(resourceType);
            SetFile(fileUrl, fileName, extension, size);
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

        public void SetResourceType(ResourceType type)
        {
            ResourceType = type;
        }

        public void SetFile(string url, string name, string extension, int size)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("FileUrl is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("FileName is required");

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Extension is required");

            if (size <= 0)
                throw new ArgumentException("Size must be greater than 0");

            FileUrl = url;
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

        public void ChangeFile(string url, string extension, int size)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("FileUrl is required");

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("Extension is required");

            if (size <= 0)
                throw new ArgumentException("Size must be greater than 0");

            FileUrl = url;
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