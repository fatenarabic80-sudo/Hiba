using System.ComponentModel.DataAnnotations;
using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Web.ViewModels;

public class CheckoutViewModel
{
    [Required, StringLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string ShippingCity { get; set; } = string.Empty;

    public CartDto Cart { get; set; } = new();
}
