﻿using AuctionService.Data.DbContext;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Controllers
{
    [Route("api/Auctions")]
    [ApiController]
    public class AuctionController(AuctionDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AuctionDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuction()
        {
            var auctions = await _context.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            return auction is not null ? _mapper.Map<AuctionDto>(auction) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
        {
            var auction = _mapper.Map<Auction>(dto);
            auction.Seller = "test";
            _context.Auctions.Add(auction);

            var result = await _context.SaveChangesAsync() > 0;

            return result ? CreatedAtAction(nameof(GetAuctionById),
                new { auction.Id }, _mapper.Map<AuctionDto>(auction)) :
                BadRequest("No se pudieron guardar los cambios en la base de datos.");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
        {
            var auction = await _context.Auctions.Include(x => x.Item)
                .FirstOrDefaultAsync (x => x.Id == id);

            if (auction is null) return NotFound();

            auction.Item.Make = dto.Make ?? auction.Item.Make;
            auction.Item.Model = dto.Model ?? auction.Item.Model;
            auction.Item.Color = dto.Color ?? auction.Item.Color;
            auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = dto.Year ?? auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;

            return result ? Ok() : BadRequest("Hubo un problema al actualizar.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction is null) return NotFound();

            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync() > 0;

            return result ? Ok() : BadRequest("Hubo un problema al actualizar.");
        }
    }
}