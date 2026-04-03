namespace dtc.Domain.Entities.Blogs;

public class Blog : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string? Avatar { get; private set; }
    public Guid CategoryId { get; private set; }
    public string? Summary { get; private set; }
    public string Content { get; private set; } = default!;
    public bool Status { get; private set; }   // true = published

    protected Blog() { }

    // =========================
    // CREATE
    // =========================
    public Blog(
        string title,
        Guid categoryId,
        string content,
        Guid? createdBy,
        string? summary = null,
        string? avatar = null)
    {
        Id = Guid.NewGuid();
        SetTitle(title);
        SetCategory(categoryId);
        SetContent(content);

        Summary = Normalize(summary);
        Avatar = Normalize(avatar);
        Status = false; // mặc định là Draft

        SetCreated(createdBy);
    }

    // =========================
    // UPDATE
    // =========================
    public bool Update(
        string? title,
        Guid? categoryId,
        string? content,
        string? summary,
        string? avatar,
        Guid? updatedBy)
    {
        bool changed = false;

        if (!string.IsNullOrWhiteSpace(title) && Title != title.Trim())
        {
            SetTitle(title);
            changed = true;
        }

        if (categoryId.HasValue && categoryId.Value != Guid.Empty && CategoryId != categoryId.Value)
        {
            CategoryId = categoryId.Value;
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(content) && Content != content.Trim())
        {
            SetContent(content);
            changed = true;
        }

        if (summary != null && Summary != summary.Trim())
        {
            Summary = Normalize(summary);
            changed = true;
        }

        if (avatar != null && Avatar != avatar.Trim())
        {
            Avatar = Normalize(avatar);
            changed = true;
        }

        if (!changed)
            return false;

        SetUpdated(updatedBy);
        return true;
    }

    // =========================
    // STATUS
    // =========================
    public void Publish(Guid? updatedBy)
    {
        if (Status) return;

        Status = true;
        SetUpdated(updatedBy);
    }

    public void Unpublish(Guid? updatedBy)
    {
        if (!Status) return;

        Status = false;
        SetUpdated(updatedBy);
    }

    // =========================
    // PRIVATE RULES
    // =========================
    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));

        Title = title.Trim();
    }

    private void SetContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));

        Content = content.Trim();
    }

    private void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId is invalid", nameof(categoryId));

        CategoryId = categoryId;
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
