using IssueTracker.Domain.Common;
using IssueTracker.Domain.Issue.Events;

namespace IssueTracker.Domain.Issue;

public class Issue : Aggregate
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid? AssigneId { get; private set; }
    public Guid? ReporterId { get; private set; }
    public bool Closed { get; private set; }

    private Issue(Guid id, string title, string description, Guid? assigneId = null, Guid? reporterId = null) {
        Id = id;
        Title = title; 
        Description = description;
        AssigneId = assigneId;
        ReporterId = reporterId;
        Closed = false;
        EnqueueDomainEvent(new IssueCreatedEvent(Id,title,description,assigneId,reporterId));
    }
    public static Issue CreateNewIssue(Guid id, string title, string description, Guid? assigneId = null, Guid? reporterId = null) 
        => new(id,title, description, assigneId, reporterId);

    public void Assign(Guid assigneId)
    {
        AssigneId = assigneId;
        EnqueueDomainEvent(new IssueAssignedEvent(Id,assigneId));
    }

    public void Close()
    {
        Closed = true;
        EnqueueDomainEvent(new IssueClosedEvent(Id));
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case IssueCreatedEvent e:
                OnCreated(e);
                break;
            case IssueAssignedEvent e:
                OnAssigned(e);
                break;
            case IssueClosedEvent e:
                OnClosed(e);
                break;
        }
    }

    private void OnClosed(IssueClosedEvent _)
    {
        Closed = true;
    }

    private void OnAssigned(IssueAssignedEvent e)
    {
        AssigneId = e.AssigneId;
    }

    private void OnCreated(IssueCreatedEvent e)
    {
        Id = e.Id;
        Title = e.Title;
        Description = e.Description;
        AssigneId = e.AssigneId;
        ReporterId = e.ReporterId;
        Closed = false;
    }

    public Issue() { }
}