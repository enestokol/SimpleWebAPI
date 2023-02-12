using Microsoft.AspNetCore.Mvc;
using SimpleWebAPI.Models.Base;
using SimpleWebAPI.Models.Request;
using SimpleWebAPI.Wrappers;

namespace SimpleWebAPI.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    private readonly List<ProductModel> _productList = new()
    {
        new ProductModel
        {
            Id = 1,
            Name = "Computer",
            Price = 40000
        },
        new ProductModel
        {
            Id = 2,
            Name = "Printer",
            Price = 5000
        },
        new ProductModel
        {
            Id = 3,
            Name = "Tablet",
            Price = 10000
        },
        new ProductModel
        {
            Id = 4,
            Name = "Monitor",
            Price = 7000
        }
    };
    
    public ProductController(ILogger<ProductController> logger) => _logger = logger;

    /// <summary>
    /// Return product list
    /// </summary>
    /// <param name="name">filter by given product name value (case-insensitive)</param>
    /// <param name="sort">sort by given property name</param>
    /// <returns>product list by filtered by name and sort by property</returns>
    [HttpGet]
    public IActionResult List([FromQuery] string? name, [FromQuery] string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return Ok(string.IsNullOrWhiteSpace(name)
                ? _productList
                : _productList.Where(x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase)));
        
        Func<ProductModel, object>? orderByFunc = null;
        if (sort.StartsWith("id")) orderByFunc = productModel => productModel.Id;
        else if (sort.StartsWith("name")) orderByFunc = productModel => productModel.Name;
        else if (sort.StartsWith("price")) orderByFunc = productModel => productModel.Price;

        return orderByFunc == null
            ? Ok(string.IsNullOrWhiteSpace(name)
                ? _productList
                : _productList.Where(x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase)))
            : sort.EndsWith("_desc")
                ? Ok(string.IsNullOrWhiteSpace(name)
                    ? _productList.OrderByDescending(orderByFunc)
                    : _productList.Where(x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                        .OrderByDescending(orderByFunc))
                : Ok(string.IsNullOrWhiteSpace(name)
                    ? _productList.OrderBy(orderByFunc)
                    : _productList.Where(x => x.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                        .OrderBy(orderByFunc));
    }
    
    /// <summary>
    /// Return Single Product
    /// </summary>
    /// <param name="id"></param>
    /// <returns>If product found by given id then return Ok with found product otherwise return Notfound</returns>
    [HttpGet("{id:int}")]
    public IActionResult Get([FromRoute]int id)
    {
        var product = _productList.FirstOrDefault(x => x.Id == id);
        return product == null
            ? NotFound(new ErrorModel
            {
                ErrorMessage = "Product not found by given Id"
            })
            : Ok(product);
    }


    /// <summary>
    /// Return Single Product
    /// </summary>
    /// <param name="productCreateModel">Product Create Model</param>
    /// <returns>If product found by given id then return Ok with found product otherwise return Notfound</returns>
    [HttpPost]
    public IActionResult Add([FromBody]ProductCreateModel productCreateModel)
    {
        var existProduct = _productList.Any(x =>
            string.Equals(x.Name, productCreateModel.Name, StringComparison.CurrentCultureIgnoreCase));
        if (existProduct)
            return BadRequest(new ErrorModel
            {
                ErrorMessage = "Name must be unique"
            });
        
        
        var lastProductId = _productList.MaxBy(x => x.Id)?.Id;
        var newProduct = new ProductModel
        {
            Id = (int)(lastProductId==null? 1:lastProductId+1),
            Name = productCreateModel.Name,
            Price = productCreateModel.Price
        };
        _productList.Add(newProduct);
        return StatusCode(StatusCodes.Status201Created,newProduct);
    }


    /// <summary>
    /// Update Product
    /// </summary>
    /// <param name="id">Id of te resource that is requested to be updated</param>
    /// <param name="productUpdateModel">Product Update model</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ProductUpdateModel productUpdateModel)
    {
        if (id != productUpdateModel.Id)
            return BadRequest(new ErrorModel
            {
                ErrorMessage = "Id was not match"
            });
        
        var product = _productList.FirstOrDefault(x => x.Id == id);
        if (product == null)
        {
            return NotFound(new ErrorModel
            {
                ErrorMessage = "Product Not Found"
            });
        }
        
        var existProduct = _productList.Any(x =>x.Id!=product.Id &&
            string.Equals(x.Name, productUpdateModel.Name, StringComparison.CurrentCultureIgnoreCase));
        if (existProduct)
            return BadRequest(new ErrorModel
            {
                ErrorMessage = "Name must be unique"
            });

        product.Name = productUpdateModel.Name;
        product.Price = productUpdateModel.Price;

        return Ok(productUpdateModel);
    }

    [HttpPatch("updateName/{id:int}")]
    public IActionResult UpdateNameById(int id, ProductNameUpdateModel productNameUpdateModel)
    {
        if (id != productNameUpdateModel.Id)
            return BadRequest(new ErrorModel
            {
                ErrorMessage = "Id was not match"
            });
        
        var product = _productList.FirstOrDefault(x => x.Id == id);
        if (product == null)
        {
            return NotFound(new ErrorModel
            {
                ErrorMessage = "Product Not Found"
            });
        }
        
        var existProduct = _productList.Any(x =>x.Id!=product.Id &&
                                                string.Equals(x.Name, productNameUpdateModel.Name, StringComparison.CurrentCultureIgnoreCase));
        if (existProduct)
            return BadRequest(new ErrorModel
            {
                ErrorMessage = "Name must be unique"
            });

        product.Name = productNameUpdateModel.Name;

        return Ok(productNameUpdateModel);
    }

    /// <summary>
    /// Remove product by Id
    /// </summary>
    /// <param name="id">Id of te resource that is requested to be deleted</param>
    /// <returns>Empty content with 204 http status code</returns>
    [HttpDelete("{id:int}")]
    public IActionResult Remove([FromRoute] int id)
    {
        var product = _productList.FirstOrDefault(x => x.Id == id);

        if (product == null)
            return NotFound(new ErrorModel
            {
                ErrorMessage = "Product not found by given Id"
            });
        
        _productList.Remove(product);
        return NoContent();

    }
    
}