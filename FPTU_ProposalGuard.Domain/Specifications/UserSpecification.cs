using System.Linq.Expressions;
using FPTU_ProposalGuard.Domain.Common.Enums;
using FPTU_ProposalGuard.Domain.Entities;
using FPTU_ProposalGuard.Domain.Specifications.Params;
using Microsoft.EntityFrameworkCore;

namespace FPTU_ProposalGuard.Domain.Specifications;

public class UserSpecification : BaseSpecification<User>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public UserSpecification(UserSpecParams specParams, int pageIndex, int pageSize)
        : base(e =>
            // Search with terms
            string.IsNullOrEmpty(specParams.Search) ||
            (
                // Email
                (!string.IsNullOrEmpty(e.Email) && e.Email.Contains(specParams.Search)) ||
                // Phone
                (!string.IsNullOrEmpty(e.Phone) && e.Phone.Contains(specParams.Search)) ||
                // Address
                (!string.IsNullOrEmpty(e.Address) && e.Address.Contains(specParams.Search)) ||
                // Individual FirstName and LastName search
                (!string.IsNullOrEmpty(e.FirstName) && e.FirstName.Contains(specParams.Search)) ||
                (!string.IsNullOrEmpty(e.LastName) && e.LastName.Contains(specParams.Search)) ||
                // Full Name search
                (!string.IsNullOrEmpty(e.FirstName) &&
                 !string.IsNullOrEmpty(e.LastName) &&
                 (e.FirstName + " " + e.LastName).Contains(specParams.Search))
            ))
    {
        // Pagination
        PageIndex = pageIndex;
        PageSize = pageSize;
        
        // Filter all admin roles
        AddFilter(u => u.Role.RoleName != nameof(Role.Administration));
        
        // Enable split query
        EnableSplitQuery();
        
        // Include role 
        ApplyInclude(q => q.Include(e => e.Role));
        
        // Default order by first name
        AddOrderBy(e => e.FirstName ?? string.Empty);
        
        if (!string.IsNullOrEmpty(specParams.FirstName)) // With first name
        {
            AddFilter(x => x.FirstName == specParams.FirstName);
        }

        if (!string.IsNullOrEmpty(specParams.LastName)) // With last name
        {
            AddFilter(x => x.LastName == specParams.LastName);
        }

        if (!string.IsNullOrEmpty(specParams.Gender)) // With gender
        {
            AddFilter(x => x.Gender == specParams.Gender);
        }

        if (specParams.IsActive != null) // With status
        {
            AddFilter(x => x.IsActive == specParams.IsActive);
        }

        if (specParams.IsDeleted != null) // Is deleted
        {
            AddFilter(x => x.IsDeleted == specParams.IsDeleted);
        }

        if (specParams.CreateDateRange != null
            && specParams.CreateDateRange.Length > 1) // With range of creation date
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
        
        if (specParams.ModifiedDateRange != null
            && specParams.ModifiedDateRange.Length > 1) // With range of modification date
        {
            if (specParams.ModifiedDateRange[0].HasValue && specParams.ModifiedDateRange[1].HasValue)
            {
                AddFilter(x => x.ModifiedDate >= specParams.ModifiedDateRange[0]!.Value.Date &&
                               x.ModifiedDate <= specParams.ModifiedDateRange[1]!.Value.Date);
            }
            else if ((specParams.ModifiedDateRange[0] is null && specParams.ModifiedDateRange[1].HasValue))
            {
                AddFilter(x => x.ModifiedDate <= specParams.ModifiedDateRange[1]);
            }
            else if (specParams.ModifiedDateRange[0].HasValue && specParams.ModifiedDateRange[1] is null)
            {
                AddFilter(x => x.ModifiedDate >= specParams.ModifiedDateRange[0]);
            }
        }
        
        if (specParams.DobRange != null
            && specParams.DobRange.Length > 1) // With range of dob
        {
            if (specParams.DobRange[0].HasValue && specParams.DobRange[1].HasValue)
            {
                AddFilter(x => x.Dob >= specParams.DobRange[0]!.Value.Date &&
                               x.Dob <= specParams.DobRange[1]!.Value.Date);
            }
            else if ((specParams.DobRange[0] is null && specParams.DobRange[1].HasValue))
            {
                AddFilter(x => x.Dob <= specParams.DobRange[1]);
            }
            else if (specParams.DobRange[0].HasValue && specParams.DobRange[1] is null)
            {
                AddFilter(x => x.Dob >= specParams.DobRange[0]);
            }
        }
        
        // Apply Sorting
        if (!string.IsNullOrEmpty(specParams.Sort))
        {
            var sortBy = specParams.Sort.Trim();
            var isDescending = sortBy.StartsWith("-");
            var propertyName = isDescending ? sortBy.Substring(1) : sortBy;

            ApplySorting(propertyName, isDescending);
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

        // Use Reflection to dynamically apply sorting
        var parameter = Expression.Parameter(typeof(User), "x");
        var property = Expression.Property(parameter, propertyName);
        var sortExpression =
            Expression.Lambda<Func<User, object>>(Expression.Convert(property, typeof(object)), parameter);

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