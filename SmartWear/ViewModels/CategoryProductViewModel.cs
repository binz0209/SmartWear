using SmartWear.ViewModels;

public class CategoryProductViewModel
{
   
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<ProductViewModel> Products { get; set; }
}

