using Management.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Domain.Entities;

public class SavedFile : BaseEntity
{
    public Guid SavedFileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ServerRelativePath { get; set; } = string.Empty;
    public string AbsolutePath { get; set; } = string.Empty;
}
