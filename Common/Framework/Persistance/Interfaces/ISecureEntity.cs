using Framework.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance.Interfaces
{
    public interface ISecureEntity
    {
        Guid CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }
        DateTime CreateDateTime { get; set; }
        DateTime? LastUpdateDateTime { get; set; }
        int EntityTypeId { get; set; }
    }
}
