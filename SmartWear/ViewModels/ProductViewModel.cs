namespace SmartWear.ViewModels
{
    public class ProductViewModel
    {
        public Guid Id { get; set; } // Tương ứng với 'Id'
        public string Name { get; set; } // Tương ứng với 'Name'
        public string Description { get; set; } // Tương ứng với 'Description'
        public decimal Price { get; set; } // Tương ứng với 'Price'
        public string ImageUrl { get; set; } // Tương ứng với 'ImageUrl'
        public int StockQuantity { get; set; } // Tương ứng với 'StockQuantity'
        public Guid CategoryId { get; set; } // Tương ứng với 'CategoryId'
        public DateTime CreatedOn { get; set; } // Tương ứng với 'CreatedOn'
        public DateTime? ModifiedOn { get; set; } // Tương ứng với 'ModifiedOn', nullable vì có thể không có giá trị
        public bool IsDeleted { get; set; } // Tương ứng với 'IsDeleted'
        public DateTime? DeletedOn { get; set; } // Tương ứng với 'DeletedOn', nullable
        public string Color { get; set; } // Tương ứng với 'Color'
        public string Size { get; set; } // Tương ứng với 'Size'
        public int Rating { get; set; } // Tương ứng với 'Rating', có thể tính toán từ ProductReviews
        public int ReviewCount { get; set; } // Tương ứng với 'ReviewCount', có thể tính toán từ ProductReviews
        public string CategoryName { get; set; } // Tên của danh mục, có thể lấy từ Category.Name
    }
}