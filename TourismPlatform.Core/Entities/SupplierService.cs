using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismPlatform.Core.Entities;

public class SupplierService
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        public string Currency { get; set; } = "COP";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Supplier Supplier { get; set; } = null!;
    }
