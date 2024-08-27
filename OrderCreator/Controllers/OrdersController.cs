using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderCreator.Data;
using OrderCreator.Models;
using OrderCreator.Sagas;

namespace OrderCreator.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderCreatorContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(OrderCreatorContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchString, string searchCategory)
        {
            var orders = from o in _context.OrderState
                         select o;

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchCategory)
                {
                    case "Id":
                        orders = orders.Where(s => s.Id.ToString().Contains(searchString));
                        break;
                    case "CorrelationId":
                        orders = orders.Where(s => s.CorrelationId.ToString().Contains(searchString));
                        break;
                    case "CurrentState":
                        orders = orders.Where(s => s.CurrentState.Contains(searchString));
                        break;
                    case "OrderName":
                        orders = orders.Where(s => s.OrderName.Contains(searchString));
                        break;
                    case "OrderDescription":
                        orders = orders.Where(s => s.OrderDescription.Contains(searchString));
                        break;
                    case "Customer":
                        orders = orders.Where(s => s.Customer.Contains(searchString));
                        break;
                    case "Sender":
                        orders = orders.Where(s => s.Sender.Contains(searchString));
                        break;
                    default:
                        break;
                }
            }
            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderState = await _context.OrderState
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderState == null)
            {
                return NotFound();
            }

            return View(orderState);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CorrelationId,CurrentState,OrderName,OrderDescription,Customer,Sender")] OrderState orderState)
        {
            if (ModelState.IsValid)
            {
                orderState.CorrelationId = Guid.NewGuid();
                var SubmitOrder = new OrderSubmitted
                {
                    OrderId = Guid.NewGuid(),
                    Customer = orderState.Customer,
                    Sender = orderState.Sender,
                    OrderName = orderState.OrderName,
                    OrderDescription = orderState.OrderDescription
                };
                await _publishEndpoint.Publish(SubmitOrder);
                return RedirectToAction(nameof(Index));
            }
            return View(orderState);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderState = await _context.OrderState.FindAsync(id);
            if (orderState == null)
            {
                return NotFound();
            }
            return View(orderState);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CorrelationId,CurrentState,OrderName,OrderDescription,Customer,Sender")] OrderState orderState)
        {
            if (id != orderState.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderState);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderStateExists(orderState.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orderState);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderState = await _context.OrderState
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderState == null)
            {
                return NotFound();
            }

            return View(orderState);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderState = await _context.OrderState.FindAsync(id);
            if (orderState != null)
            {
                _context.OrderState.Remove(orderState);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderStateExists(int id)
        {
            return _context.OrderState.Any(e => e.Id == id);
        }
    }
}
