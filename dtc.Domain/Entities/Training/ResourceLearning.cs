namespace dtc.Domain.Entities.Training
{
    public class ResourceLearning : BaseEntity
    {
        public Guid CourseId { get; private set; }
        public ResourceType ResourceType { get; private set; }
        public string Title { get; private set; }
        public string ResourceUrl { get; private set; }
        public bool IsActive { get; private set; }

        protected ResourceLearning() { }

        public ResourceLearning(
            Guid courseId,
            ResourceType resourceType,
            string title,
            string resourceUrl,
            Guid? createdBy = null)
        {
            Id = Guid.NewGuid();

            SetCourse(courseId);
            SetType(resourceType);
            SetTitle(title);
            SetUrl(resourceUrl);

            IsActive = true;
            SetCreated(createdBy);
        }

        // =========================
        // Behaviors
        // =========================

        public bool UpdateInfo(
            ResourceType? resourceType,
            string? title,
            string? resourceUrl,
            Guid? updatedBy = null)
        {
            bool changed = false;

            if (resourceType.HasValue)
                changed |= SetType(resourceType.Value);

            if (!string.IsNullOrWhiteSpace(title))
                changed |= SetTitle(title);

            if (!string.IsNullOrWhiteSpace(resourceUrl))
                changed |= SetUrl(resourceUrl);

            if (!changed)
                return false;

            SetUpdated(updatedBy);
            return true;
        }

        public void Activate(Guid? updatedBy = null)
        {
            if (IsActive) return;

            IsActive = true;
            SetUpdated(updatedBy);
        }

        public void Deactivate(Guid? updatedBy = null)
        {
            if (!IsActive) return;

            IsActive = false;
            SetUpdated(updatedBy);
        }

        // =========================
        // Internal setters
        // =========================

        private void SetCourse(Guid courseId)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId is required");

            CourseId = courseId;
        }

        private bool SetType(ResourceType type)
        {
            if (!Enum.IsDefined(typeof(ResourceType), type))
                throw new ArgumentException("Invalid ResourceType");

            if (ResourceType == type)
                return false;

            ResourceType = type;
            return true;
        }

        private bool SetTitle(string title)
        {
            var normalized = title.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("Title is required");

            if (Title == normalized)
                return false;

            Title = normalized;
            return true;
        }

        private bool SetUrl(string url)
        {
            var normalized = url.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("ResourceUrl is required");

            if (ResourceUrl == normalized)
                return false;

            ResourceUrl = normalized;
            return true;
        }
    }
}
