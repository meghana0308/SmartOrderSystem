using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Models.DTOs.Reports;
using SmartOrder.API.Models.DTOs.Inventory;

namespace SmartOrder.API.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================
    // Sales Reports
    // =========================

    [HttpGet("sales/by-date")]
    [Authorize(Roles = "SalesExecutive")]
    public async Task<IActionResult> GetSalesByDate(
        DateTime fromDate,
        DateTime toDate)
    {
        var sales = await _context.Orders
            .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                OrdersCount = g.Count(),
                TotalSales = g.Sum(o => o.TotalAmount)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();

        return Ok(sales);
    }

    [HttpGet("sales/by-product")]
    [Authorize(Roles = "SalesExecutive")]
    public async Task<IActionResult> GetSalesByProduct(
        DateTime fromDate,
        DateTime toDate)
    {
        var report = await _context.Orders
            .Where(o => o.OrderDate >= fromDate && o.OrderDate < toDate.AddDays(1))
            .SelectMany(o => o.OrderItems)
            .GroupBy(oi => new
            {
                oi.ProductId,
                oi.Product.Name
            })
            .Select(g => new ProductSalesReportDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalQuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToListAsync();

        return Ok(report);
    }

    // =========================
    // Top-Selling Products
    // =========================
    [HttpGet("sales/top-products")]
    [Authorize(Roles = "SalesExecutive,WarehouseManager")]
    public async Task<IActionResult> GetTopSellingProducts(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int top = 10) // optional: limit top N products
    {
        var ordersQuery = _context.Orders.AsQueryable();

        if (fromDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.OrderDate >= fromDate.Value);
        if (toDate.HasValue)
            ordersQuery = ordersQuery.Where(o => o.OrderDate < toDate.Value.AddDays(1));

        var report = await ordersQuery
            .SelectMany(o => o.OrderItems)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new ProductSalesReportDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalQuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(top)
            .ToListAsync();

        return Ok(report);
    }

    // =========================
    // Low-Selling Products Report
    // =========================
    [HttpGet("sales/low-selling")]
    [Authorize(Roles = "SalesExecutive,WarehouseManager")]
    public async Task<IActionResult> GetLowSellingProducts(int top = 10, DateTime? fromDate = null, DateTime? toDate = null)
    {
        fromDate ??= DateTime.UtcNow.AddMonths(-1); // default last 1 month
        toDate ??= DateTime.UtcNow;

        var report = await _context.Orders
            .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
            .SelectMany(o => o.OrderItems)
            .GroupBy(oi => new
            {
                oi.ProductId,
                oi.Product.Name
            })
            .Select(g => new ProductSalesReportDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalQuantitySold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderBy(x => x.TotalRevenue) // ascending for low-selling
            .Take(top)
            .ToListAsync();

        return Ok(report);
    }



}
