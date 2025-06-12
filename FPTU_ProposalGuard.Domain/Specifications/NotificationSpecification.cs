using System.Linq.Expressions;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Specifications.Params;
using Microsoft.EntityFrameworkCore;

namespace FPTU_ProposalGuard.Domain.Specifications;

public class NotificationSpecification : BaseSpecification<Notification>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public NotificationSpecification(NotificationSpecParams specParams, int pageIndex, int pageSize)
        : base(n =>
            // search with term
            string.IsNullOrEmpty(specParams.Search) ||
            (
                // Title
                (!string.IsNullOrEmpty(n.Title) && n.Title.Contains(specParams.Search)) ||
                // Message
                (!string.IsNullOrEmpty(n.Message) && n.Message.Contains(specParams.Search))
            )
        )
    {
        // Assign page size and page index
        PageIndex = pageIndex;
        PageSize = pageSize;
        
        // Enable split query
        EnableSplitQuery();
        
        // Apply include
        ApplyInclude(q => q
            .Include(n => n.CreatedBy)
            .Include(n => n.Recipient)
        );
        
        // Progress filter
        if (specParams.RecipientId != null && specParams.RecipientId != Guid.Empty)
        {
            AddFilter(n => n.RecipientId == specParams.RecipientId);
        }
        if (specParams.IsRead != null)
        {
            AddFilter(n => n.IsRead == specParams.IsRead);
        }
        if (specParams.Type != null)
        {
            AddFilter(n => n.Type == specParams.Type);
        }        
        if (specParams.CreateDateRange != null
            && specParams.CreateDateRange.Length > 1) // With range of dob
        {
            if (specParams.CreateDateRange[0].HasValue && specParams.CreateDateRange[1].HasValue)
            {
                AddFilter(x => x.CreateDate.Date >= specParams.CreateDateRange[0]!.Value.Date &&
                               x.CreateDate.Date <= specParams.CreateDateRange[1]!.Value.Date);
            }
            else if ((specParams.CreateDateRange[0] is null && specParams.CreateDateRange[1].HasValue))
            {
                AddFilter(x => x.CreateDate <= specParams.CreateDateRange[1]);
            }
            else if (specParams.CreateDateRange[0].HasValue && specParams.CreateDateRange[1] is null)
            {
                AddFilter(x => x.CreateDate >= specParams.CreateDateRange[0]);
            }
        }
        
        // Progress sorting
        if (!string.IsNullOrEmpty(specParams.Sort))
        {
            // Check is descending sorting 
            var isDescending = specParams.Sort.StartsWith("-");
            if (isDescending)
            {
                specParams.Sort = specParams.Sort.Trim('-');
            }
            
            // Uppercase sort value
            specParams.Sort = specParams.Sort.ToUpper();

            // Apply sorting
            ApplySorting(specParams.Sort, isDescending);
        }
        else
        {
            // Default order by create date
            AddOrderByDescending(u => u.CreateDate);
        }
    }
    
    private void ApplySorting(string propertyName, bool isDescending)
    {
        if (string.IsNullOrEmpty(propertyName)) return;

        // Initialize expression parameter with type of Employee (x)
        var parameter = Expression.Parameter(typeof(Notification), "x");
        // Assign property base on property name (x.PropertyName)
        var property = Expression.Property(parameter, propertyName);
        // Building a complete sort lambda expression (x => x.PropertyName)
        var sortExpression =
            Expression.Lambda<Func<Notification, object>>(Expression.Convert(property, typeof(object)), parameter);

        if (isDescending)
        {
            AddOrderByDescending(sortExpression);
        }
        else
        {
            AddOrderBy(sortExpression);
        }
    }
}