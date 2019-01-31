﻿using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Repository.Models.Product;
using Web.Api.Dtos;
using Web.Api.Dtos.Commons;
using Web.Api.Helpers.Pagging;
using Web.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Web.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/resources/")]
    [Authorize]
    public class ProductController : BasePaggingController<Product>
    {
        /*
         * Provide just what is necessary, no more, no less
         * Use a hanful of http status code
         * 
         * 200 -   Ok
         * 201 -   Created
         * 400 -   Bad Request (Show why is bad Request)
         * 404 -   Not Found
         * 401 -   Unauthorized
         * 403 -   Forbidden
         */

        private IProductService _productService;

        public ProductController(IProductService productService, IUrlHelper urlHelper) : base(urlHelper)
        {
            _productService = productService;
        }

        #region Methods Get
        [HttpGet("products", Name = "products")]
        [ProducesResponseType(200, Type = typeof(PagedList<ProductDTO>))]
        public IActionResult Products(PagingParams pagingParams)
        {
            var model = _productService.GetAllPagging(pagingParams);

            Response.Headers.Add("X-Pagination", model.GetHeader().ToJson());

            var outputModel = new PagingDTO<ProductDTO>()
            {
                Paging = model.GetHeader(),
                Links = GetLinks(model, "products"),
                Items = model.List.Select(m => ToConvertDTO(m)).ToList(),
            };
            return Ok(outputModel);
        }

        [HttpGet("products/{id}")]
        [ProducesResponseType(200, Type = typeof(ProductDTO))]
        [ProducesResponseType(404)] 
        public IActionResult Product(string id)
        {

            var product = _productService.Get(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(ToConvertDTO(product));
        }

        [HttpGet("products/{id}/invoices")]
        [ProducesResponseType(200, Type = typeof(ProductInvoicesDTO))]
        [ProducesResponseType(404)]
        public IActionResult ProductInvoices(string id)
        {

            var product = _productService.GetInvoices(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(new ProductInvoicesDTO()
            {
                ProductDetail = product.ProductDetail,
                ProductGrade = product.ProductGrade,
                ProdutItems = product.ProdutItens.Select( p => new ProductItemDTO()
                {
                    Description = p.Description
                }).ToList()
            });
        }
        #endregion

        #region Methods Post/Put
        [HttpPost("product")]
        [ProducesResponseType(201, Type = typeof(ProductDTO))]
        [ProducesResponseType(400)]
        public IActionResult Post([FromBody] ProductDTO obj)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = ToConvertObj(obj);
            _productService.Add(product);

            return CreatedAtAction(nameof(Product), new { id = product.Id }, ToConvertDTO(product)); 
        }

        [HttpPut("product")]
        [ProducesResponseType(200, Type =typeof(ProductDTO))]
        [ProducesResponseType(400)]
        public IActionResult Put([FromBody] ProductDTO obj)
        {

            if(!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            return Ok(obj);
        }
        #endregion

        #region Convertions
        private ProductDTO ToConvertDTO(Product obj)
        {
            return new ProductDTO()
            {
                Id = obj.Id,
                Description = obj.Description,
                Active = obj.Active
            };
        }

        private Product ToConvertObj(ProductDTO obj)
        {
            return new Product()
            {
                Description = obj.Description,
                Active = obj.Active
            };
        }
        #endregion

    }
}