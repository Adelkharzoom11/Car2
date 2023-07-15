using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;









// Car Model Class

public class Car
{
    public int Id { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Gear { get; set; }
    public int Km { get; set; }
    public ICollection<Part> Parts { get; set; }
}

// Supplier Model Class
public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

// Part Model Class
public class Part
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }
    public ICollection<Car> Cars { get; set; }
}

// Sale Model Class
public class Sale
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}

// Customer Model Class
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
}






public class AutoPartsStoreContext : DbContext
{
    public DbSet<Car> Cars { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure the database connection
        optionsBuilder.UseSqlServer("YourConnectionString");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the relationships between entities
        modelBuilder.Entity<Part>()
            .HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId);

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Car)
            .WithMany()
            .HasForeignKey(s => s.CarId);

        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId);

        modelBuilder.Entity<Car>()
            .HasMany(c => c.Parts)
            .WithMany(p => p.Cars)
            .UsingEntity(j => j.ToTable("CarPart"));
    }
}












[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly AutoPartsStoreContext _context;

    public CarsController(AutoPartsStoreContext context)
    {
        _context = context;
    }

    // GET: api/cars
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars()
    {
        return await _context.Cars.ToListAsync();
    }

    // GET: api/cars/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Car>> GetCar(int id)
    {
        var car = await _context.Cars.FindAsync(id);

        if (car == null)
        {
            return NotFound();
        }

        return car;
    }

    // POST: api/cars
    [HttpPost]
    public async Task<ActionResult<Car>> PostCar(Car car)
    {
        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCar", new { id = car.Id }, car);
    }

    // PUT: api/cars/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCar(int id, Car car)
    {
        if (id != car.Id)
        {
            return BadRequest();
        }

        _context.Entry(car).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CarExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/cars/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CarExists(int id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }
}










[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly AutoPartsStoreContext _context;

    public SuppliersController(AutoPartsStoreContext context)
    {
        _context = context;
    }

    // GET: api/suppliers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
    {
        return await _context.Suppliers.ToListAsync();
    }

    // GET: api/suppliers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetSupplier(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);

        if (supplier == null)
        {
            return NotFound();
        }

        return supplier;
    }

    // POST: api/suppliers
    [HttpPost]
    public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSupplier", new { id = supplier.Id }, supplier);
    }

    // PUT: api/suppliers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSupplier(int id, Supplier supplier)
    {
        if (id != supplier.Id)
        {
            return BadRequest();
        }

        _context.Entry(supplier).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SupplierExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/suppliers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SupplierExists(int id)
    {
        return _context.Suppliers.Any(e => e.Id == id);
    }
}
















[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly AutoPartsStoreContext _context;

    public SalesController(AutoPartsStoreContext context)
    {
        _context = context;
    }

    // GET: api/sales
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
    {
        return await _context.Sales.ToListAsync();
    }

    // GET: api/sales/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetSale(int id)
    {
        var sale = await _context.Sales.FindAsync(id);

        if (sale == null)
        {
            return NotFound();
        }

        return sale;
    }

    // POST: api/sales
    [HttpPost]
    public async Task<ActionResult<Sale>> PostSale(Sale sale)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSale", new { id = sale.Id }, sale);
    }

    // PUT: api/sales/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSale(int id, Sale sale)
    {
        if (id != sale.Id)
        {
            return BadRequest();
        }

        _context.Entry(sale).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SaleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/sales/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null)
        {
            return NotFound();
        }

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SaleExists(int id)
    {
        return _context.Sales.Any(e => e.Id == id);
    }
}













[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AutoPartsStoreContext _context;

    public CustomersController(AutoPartsStoreContext context)
    {
        _context = context;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _context.Customers.ToListAsync();
    }

    // GET: api/customers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return customer;
    }

    // POST: api/customers
    [HttpPost]
    public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
    }

    // PUT: api/customers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, Customer customer)
    {
        if (id != customer.Id)
        {
            return BadRequest();
        }

        _context.Entry(customer).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CustomerExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/customers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CustomerExists(int id)
    {
        return _context.Customers.Any(e => e.Id == id);
    }
}













[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly AutoPartsStoreContext _context;

    public PartsController(AutoPartsStoreContext context)
    {
        _context = context;
    }

    // GET: api/parts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Part>>> GetParts()
    {
        return await _context.Parts.ToListAsync();
    }

    // GET: api/parts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Part>> GetPart(int id)
    {
        var part = await _context.Parts.FindAsync(id);

        if (part == null)
        {
            return NotFound();
        }

        return part;
    }

    // POST: api/parts
    [HttpPost]
    public async Task<ActionResult<Part>> PostPart(Part part)
    {
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetPart", new { id = part.Id }, part);
    }

    // PUT: api/parts/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPart(int id, Part part)
    {
        if (id != part.Id)
        {
            return BadRequest();
        }

        _context.Entry(part).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PartExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/parts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePart(int id)
    {
        var part = await _context.Parts.FindAsync(id);
        if (part == null)
        {
            return NotFound();
        }

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PartExists(int id)
    {
        return _context.Parts.Any(e => e.Id == id);
    }
}








































