using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Domain.Common;

public class BaseEntity
{
    public String? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public String? LastModifiedBy { get;set; }
    public DateTime? LastModifiedDate { get; set; }
}
