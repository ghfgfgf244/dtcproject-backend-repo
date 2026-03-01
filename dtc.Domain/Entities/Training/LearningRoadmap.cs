namespace dtc.Domain.Entities.Training
{
    public class LearningRoadmap
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int OrderNo { get; private set; }

        protected LearningRoadmap() { }

        public LearningRoadmap(
            Guid courseId,
            string title,
            string description,
            int orderNo)
        {
            Id = Guid.NewGuid();

            SetCourse(courseId);
            SetTitle(title);
            SetDescription(description);
            SetOrder(orderNo);
        }

        // =========================
        // Behaviors
        // =========================

        public bool UpdateContent(
            string? title,
            string? description)
        {
            bool changed = false;

            if (!string.IsNullOrWhiteSpace(title))
                changed |= SetTitle(title);

            if (!string.IsNullOrWhiteSpace(description))
                changed |= SetDescription(description);

            return changed;
        }

        public bool ChangeOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new ArgumentException("OrderNo must be greater than 0");

            if (OrderNo == newOrder)
                return false;

            OrderNo = newOrder;
            return true;
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

        private bool SetDescription(string description)
        {
            var normalized = description.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("Description is required");

            if (Description == normalized)
                return false;

            Description = normalized;
            return true;
        }

        private bool SetOrder(int orderNo)
        {
            if (orderNo <= 0)
                throw new ArgumentException("OrderNo must be greater than 0");

            if (OrderNo == orderNo)
                return false;

            OrderNo = orderNo;
            return true;
        }
    }
}
