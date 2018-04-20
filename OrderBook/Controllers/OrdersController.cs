﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace OrderBook.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly OrderBook orderBook;

        public OrdersController(OrderBook orderBook)
        {
            this.orderBook = orderBook;
        }

        // GET api/orders
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var asks = await this.orderBook.GetAsksAsync();
                var bids = await this.orderBook.GetBidsAsync();
                var asksCount = await this.orderBook.GetAsksCountAsync();
                var bidsCount = await this.orderBook.GetBidsCountAsync();
                var view = new OrderBookViewModel {
                    Asks = asks,
                    Bids = bids,
                    AsksCount = asksCount,
                    BidsCount = bidsCount,
                };
                return this.Json(view);
            }
            catch (Exception)
            {
                return new ContentResult { StatusCode = 503, Content = "The service was unable to process the request. Please try again." };
            }
        }

        [Route("bids")]
        [HttpGet]
        public async Task<IActionResult> Bids()
        {
            try
            {
                var bids = await this.orderBook.GetBidsAsync();
                return this.Json(bids);
            }
            catch (InvalidOrderException ex)
            {
                return new ContentResult { StatusCode = 400, Content = ex.Message };
            }
            catch (Exception)
            {
                return new ContentResult { StatusCode = 503, Content = "The service was unable to process the request. Please try again." };
            }
        }

        [Route("asks")]
        [HttpGet]
        public async Task<IActionResult> Asks()
        {
            try
            {
                var bids = await this.orderBook.GetAsksAsync();
                return this.Json(bids);
            }
            catch (Exception)
            {
                return new ContentResult { StatusCode = 503, Content = "The service was unable to process the request. Please try again." };
            }
        }

        [Route("bid")]
        [HttpPost]
        public async Task<IActionResult> Bid([FromBody] Order order)
        {
            try
            {
                await this.orderBook.AddBidAsync(order);
                return this.Ok(order.Id);
            }
            catch (InvalidOrderException ex)
            {
                return new ContentResult { StatusCode = 400, Content = ex.Message };
            }
            catch (FabricNotPrimaryException)
            {
                return new ContentResult { StatusCode = 410, Content = "The primary replica has moved. Please re-resolve the service." };
            }
            catch (FabricException)
            {
                return new ContentResult { StatusCode = 503, Content = "The service was unable to process the request. Please try again." };
            }
        }

        [Route("ask")]
        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] Order order)
        {
            try
            {
                await this.orderBook.AddAskAsync(order);
                return this.Ok(order.Id);
            }
            catch (InvalidOrderException ex)
            {
                return new ContentResult { StatusCode = 400, Content = ex.Message };
            }
            catch (FabricNotPrimaryException)
            {
                return new ContentResult { StatusCode = 410, Content = "The primary replica has moved. Please re-resolve the service." };
            }
            catch (FabricException)
            {
                return new ContentResult { StatusCode = 503, Content = "The service was unable to process the request. Please try again." };
            }
        }
    }
}
