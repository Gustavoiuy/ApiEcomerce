
namespace ApiEcommerce.Models.Dtos;

public class CreateProductDto
{


    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public string ImgUrl { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    public int Stock { get; set; }

    public DateTime? UpdateTime { get; set; } = null;

    //Relacion con la category
    public int CategoryId { get; set; }

}
