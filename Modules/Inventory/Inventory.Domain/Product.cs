using Framework.Persistance.Interfaces;
using System;

namespace Inventory.Domain
{
    public class Product : ISecureEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }

        public Guid CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Guid? UpdatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime CreateDateTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime? LastUpdateDateTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int EntityTypeId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
